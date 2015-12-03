using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Net.Mail;
using System.Threading;

namespace TransitDataBackend
{
    class DataUploader
    {
        //constructor
        public DataUploader(bool detailedLogging)
        {
            standardOutputStringBuilder = new StringBuilder(); //add this, you may also need to add using System.Text on top
            errorOutputStringBuilder = new StringBuilder(); //add this, you may also need to add using System.Text on top
            //string connectionString = @"Data Source = ffldfgjjcg.database.windows.net; Initial Catalog = metrotransit_db; Persist Security Info = True; User ID = metrotransitadmin; Password = Fyym4jzG9$5?; Timeout = 300";
            //string connectionString2 = @"Data Source = ffldfgjjcg.database.windows.net; Initial Catalog = metrotransit_db; Persist Security Info = True; User ID = metrotransitadmin; Password = Fyym4jzG9$5?";
            //databaseConnection = new SqlConnection(connectionString);

        }

        //properties
        StringBuilder standardOutputStringBuilder { get; set; }
        StringBuilder errorOutputStringBuilder { get; set; }
        SqlConnection databaseConnection { get; set; }
        SqlCommand command = new SqlCommand();
        string connectionString = @"Data Source = ffldfgjjcg.database.windows.net; Initial Catalog = metrotransit_db; Persist Security Info = True; User ID = metrotransitadmin; Password = Fyym4jzG9$5?; Timeout = 300";

        //functions
        public void updateDateTime()
        {
            string myDateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            using (SqlConnection databaseConnection = new SqlConnection(connectionString))
            {
                databaseConnection.Open();
                command.Connection = databaseConnection;
                command.Parameters.Clear();
                TransitDataBackend.Logging(String.Format("All files finished uploadig at {0}", myDateTime.ToString()), true);
                command.CommandType = CommandType.Text;
                command.CommandText = @"Truncate table batch_load_date " +
                                                "INSERT into dbo.batch_load_date(load_date) " +
                                                "VALUES ('" + myDateTime + "')";
                command.ExecuteNonQuery();
            }
        }
        public void CreateDirectionTable()
        {
            using (SqlConnection databaseConnection = new SqlConnection(connectionString))
            {
                databaseConnection.Open();
                command.Connection = databaseConnection;
                command.Parameters.Clear();
                TransitDataBackend.Logging("Creating Directions Table", true);
                command.CommandType = CommandType.Text;
                command.CommandText = @"insert into route_directions_slave (route_id, direction_long, direction_short) 
                                    select 
                                    route_id, 
                                    substring(trip_headsign, 1, iif(charindex(' ', trip_headsign) = 0, len(trip_headsign), (charindex(' ', trip_headsign) - 1))) as direction_long, 
                                    case substring(trip_headsign, 1, iif(charindex(' ', trip_headsign) = 0, len(trip_headsign), (charindex(' ', trip_headsign) - 1))) 
                                    when 'Eastbound' then 'E' 
                                    when 'Westbound' then 'W' 
                                    when 'Southbound' then 'S' 
                                    when 'Northbound' then 'N' 
                                    else 'O' 
                                    end as direction_short 
                                    from trips_slave
                                    group by route_id, substring(trip_headsign, 1, iif(charindex(' ', trip_headsign) = 0, len(trip_headsign), (charindex(' ', trip_headsign) - 1))) ";
                command.ExecuteNonQuery();
            }
        }
        public void CreateSlaveTables(string tableName)
        {
            using (SqlConnection databaseConnection = new SqlConnection(connectionString))
            {
                databaseConnection.Open();
                command.Connection = databaseConnection;
                command.Parameters.Clear();
                command.CommandText = "create_slave_tables";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@table_name", tableName);
                TransitDataBackend.Logging("Data Uploader: Creating Slave Tables..." + tableName, true);
                command.ExecuteReader();
                command.Parameters.Clear();
            }
        }
        public void RenameSlaveTables()
        {
            using (SqlConnection databaseConnection = new SqlConnection(connectionString))
            {
                databaseConnection.Open();
                command.Connection = databaseConnection;
                command.CommandText = "rename_slave_tables";
                command.CommandType = CommandType.StoredProcedure;
                TransitDataBackend.Logging("Data Uploader: Renaming Slave Tables...", true);
                command.ExecuteReader();
            }
        }
        public void DropSlaveTables(string tableName)
        {
            using (SqlConnection databaseConnection = new SqlConnection(connectionString))
            {
                databaseConnection.Open();
                command.Connection = databaseConnection;
                command.Parameters.Clear();
                command.CommandText = "drop_slave_tables";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@table_name", tableName);
                TransitDataBackend.Logging("Data Uploader: Dropping Slave Tables..." + tableName, true);
                command.ExecuteReader();
                command.Parameters.Clear();
            }
        }

        public void RenameSlaveIndexes()
        {
            using (SqlConnection databaseConnection = new SqlConnection(connectionString))
            {
                databaseConnection.Open();
                command.Connection = databaseConnection;
                command.Parameters.Clear();
                command.CommandText = "rename_slave_indexes";
                command.CommandType = CommandType.StoredProcedure;
                TransitDataBackend.Logging("Data Uploader: Renaming Slave Indexes...", true);
                command.ExecuteReader();
            }
        }
        public void CreateIndexes(string tableName)
        {
            using (SqlConnection databaseConnection = new SqlConnection(connectionString))
            {
                databaseConnection.Open();
                command.Connection = databaseConnection;
                command.CommandTimeout = 7200;
                command.Parameters.Clear();
                command.CommandText = "create_indexes";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@table_name", tableName);
                Console.Out.WriteLine("Data Uploader: Creating Slave Indexes..." + tableName);
                command.ExecuteReader();
                command.Parameters.Clear();
            }
        }
        public void LoadData(bool detailedLogging, string fileToUpload, int numberOfTries)
        {
            List<string> fileArray = new List<string>();

            if (fileToUpload.ToLower().Equals("all"))
            {
                string[] tempFileArray = { "calendar", "agency", "routes", "calendar_dates", "trips", "stops", "shapes", "stop_times" };
                foreach (string tempFile in tempFileArray)
                {
                    fileArray.Add(tempFile);
                }
            }
            else
            {
                //TODO: check fileToUpload value is acceptable, otherwise skip everything
                fileArray.Add(fileToUpload);
            }


            int i = 1;
            while (i < 2)
            {
                TransitDataBackend.Logging("Data Uploader: Try Number: " + i.ToString(), true);

                try
                {
                    Process bcpProcess;
                    try
                    {
                        foreach (string fileName in fileArray)
                        {
                            TransitDataBackend.Logging("Data Uploader: Uploading " + fileName, detailedLogging);
                            //Truncate target table before each table load
                            //this.TruncateTargetTable(fileName);
                            string processArgument = @"metrotransit_db.dbo." + fileName + @"_slave in D:\home\site\wwwroot\google_transit\" + fileName + @".txt -U metrotransitadmin -P Fyym4jzG9$5? -S ffldfgjjcg.database.windows.net -f D:\home\site\wwwroot\google_transit\" + fileName + @".xml -V -F 2";
                            if (fileName == "stop_times" || fileName == "shapes")
                            {
                                processArgument = processArgument + " -b 10000";
                            }
                            //Drop all slave tables incase they exist
                            this.DropSlaveTables(fileName);
                            //Create all slave tables
                            this.CreateSlaveTables(fileName);
                            //initiate the process that will execute the bcp.exe 
                            TransitDataBackend.Logging("Data Uploader: started bulk copying...", detailedLogging);
                            //initiate the object that will define the properties of this process
                            var bcpProcessStartInfo = new ProcessStartInfo();
                            //define executable name
                            bcpProcessStartInfo.FileName = "bcp.exe";
                            //define program arguments
                            bcpProcessStartInfo.Arguments = processArgument;
                            //indicate do not open a separate command window
                            bcpProcessStartInfo.CreateNoWindow = true;
                            //indicate do not use operating system shell to start the process.
                            bcpProcessStartInfo.UseShellExecute = false;
                            //redirect output stream so that we can collect it using event handler
                            bcpProcessStartInfo.RedirectStandardOutput = true;
                            //redirect error stream so that we can collect it using event handler
                            bcpProcessStartInfo.RedirectStandardError = true;

                            bcpProcess = new Process();
                            //indicate to process to use the previously defined arguments to use while executing the process
                            bcpProcess.StartInfo = bcpProcessStartInfo;
                            //define which methods are responsible for collecting output and error streams
                            bcpProcess.OutputDataReceived += new DataReceivedEventHandler(outputHandler);
                            bcpProcess.ErrorDataReceived += new DataReceivedEventHandler(errorHandler);

                            //start the process
                            bcpProcess.Start();
                            bcpProcess.BeginOutputReadLine();
                            bcpProcess.BeginErrorReadLine();
                            //wait for the program to terminate completely
                            bcpProcess.WaitForExit();
                            bcpProcess.Close();

                            //optional: display the error and output streams in the console
                            Console.Out.WriteLine("Output from bcp for this file: {0}", fileName);
                            Console.Out.WriteLine("===========================================================================");
                            Console.Out.WriteLine(standardOutputStringBuilder.ToString());
                            if (errorOutputStringBuilder.ToString().Length > 0)
                            {
                                Console.Out.WriteLine("Error from bcp for this file: {0}", fileName);
                                Console.Out.WriteLine("===========================================================================");
                                Console.Out.WriteLine(errorOutputStringBuilder.ToString());
                            }
                            //create slave indexes
                            this.CreateIndexes(fileName);
                            //create Directions Table only if fileName is trips
                            if (fileName.Equals("trips"))
                            {
                                this.CreateDirectionTable();
                            }
                            //rename the slave tables
                            this.RenameSlaveTables();
                            //rename slave indexes
                            this.RenameSlaveIndexes();

                            standardOutputStringBuilder.Clear();
                            errorOutputStringBuilder.Clear();
                            TransitDataBackend.Logging(String.Format("{0} file finished uploading", fileName), true);
                            TransitDataBackend.Logging(String.Format("============================================"), true);
                        }

                        //break from while
                        this.updateDateTime();

                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.Out.WriteLine("Data Uploader: Child Error message: {0}", ex.Message);
                        throw;

                    }
                    finally
                    {
                        //bcpProcess.Close();

                    }
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine("Data Uploader: Parent Error message: {0}", ex.Message);
                    //try while loop again
                    i++;
                    if (i == 2)
                    {
                        TransitDataBackend.Logging("Data Uploader: Sending Email Notification", true);
                        MailMessage objeto_mail = new MailMessage();
                        SmtpClient client = new SmtpClient();
                        client.Port = 587;
                        client.Host = "smtp.zoho.com";
                        client.Timeout = 10000;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.UseDefaultCredentials = false;
                        client.EnableSsl = true;
                        client.Credentials = new System.Net.NetworkCredential("admin@innovodynamics.com", "MQ&nw[5sN7f4");
                        objeto_mail.From = new MailAddress("admin@innovodynamics.com ");
                        objeto_mail.To.Add(new MailAddress("admin@innovodynamics.com"));
                        objeto_mail.To.Add(new MailAddress("jlindsey@innovodynamics.com"));
                        objeto_mail.Subject = "Error On Transit App";
                        objeto_mail.Body = String.Format("Message from standard output:{0}\nMessage from standard error:{1}\n Message from exception:{2}", standardOutputStringBuilder.ToString(), errorOutputStringBuilder.ToString(), ex.Message);
                        client.Send(objeto_mail);

                    }
                    continue;

                }
                finally
                {
                    if (databaseConnection != null && databaseConnection.State == ConnectionState.Open)
                    {
                        databaseConnection.Close();
                    }
                }
            }
        }



        public void outputHandler(object sender, DataReceivedEventArgs args) //add this
        {
            if (!String.IsNullOrEmpty(args.Data)) //add this
            {
                standardOutputStringBuilder.AppendLine(args.Data); //add this
            }

        }
        public void errorHandler(object sender, DataReceivedEventArgs args) //add this
        {
            if (!String.IsNullOrEmpty(args.Data)) //add this
            {
                errorOutputStringBuilder.AppendLine(args.Data); //add this
            }
        }
    }
}
