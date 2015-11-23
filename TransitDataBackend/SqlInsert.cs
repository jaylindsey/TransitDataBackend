using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Data.SqlClient;
using System.Diagnostics;

namespace TransitDataBackend
{
    class SqlInsert
    {
        public void LoadDataIntoRouteTable(bool detailedLogging)
        {
            string connectionString = "Data Source=ffldfgjjcg.database.windows.net;Initial Catalog=metrotransit_db;Persist Security Info=True;User ID=metrotransitadmin;Password=Fyym4jzG9$5?";
            using (SqlConnection targetConnection = new SqlConnection(connectionString))
            {
                TransitDataBackend.Logging("SQL Insert: Starting Connection", detailedLogging);
                targetConnection.Open();
                //  Delete all from the destination table.         
                SqlCommand commandDelete = new SqlCommand();
                commandDelete.Connection = targetConnection;
                commandDelete.CommandText = "Truncate Table dbo.Routes";
                commandDelete.ExecuteNonQuery();

                //SqlCommand commandInsert = new SqlCommand();
                //commandInsert.Connection = targetConnection;
                //commandInsert.CommandText = @"BCP metrotransit_db.dbo.Routes IN C:\google_transit\routes.txt - U metrotransitadmin - P Fyym4jzG9$5? - b 1000 - t \t - r \n";
                //commandInsert.ExecuteNonQuery();


                //var bulkCopyApp = new Process();

                //var bcpProcessStartInfo = new ProcessStartInfo();
                //bcpProcessStartInfo.FileName = "bcp.exe";
                //bcpProcessStartInfo.Arguments = @"metrotransit_db.dbo.Routes IN C:\google_transit\routes.txt -U metrotransitadmin -P Fyym4jzG9$5? -F 2 -b 1000 -t \t -r \n -S ffldfgjjcg.database.windows.net -f C:\google_transit\routes.xml";
                //bcpProcessStartInfo.CreateNoWindow = true;
                //bcpProcessStartInfo.UseShellExecute = false;

                //bulkCopyApp.StartInfo = bcpProcessStartInfo;
                //bulkCopyApp.Start();
                ////string output = bulkCopyApp.StandardOutput.ReadToEnd();
                //bulkCopyApp.WaitForExit();
                try
                {
                    TransitDataBackend.Logging("SQL Insert: Starting stream reader", detailedLogging);
                    // Create an instance of StreamReader to read from a file.
                    // The using statement also closes the StreamReader.
                    using (StreamReader routesStream = new StreamReader(@"D:\home\site\wwwroot\google_transit\routes.txt"))
                    {
                        string tableRow;
                        string[] tableRowCellArray;
                        SqlCommand commandInsert = new SqlCommand();
                        commandInsert.Connection = targetConnection;
                        // Read and display lines from the file until the end of 
                        // the file is reached.
                        int i = 0;

                        while ((tableRow = routesStream.ReadLine()) != null)
                        {
                            TransitDataBackend.Logging("SQL Insert: Trimming Characters from Routes.txt", detailedLogging);
                            if (i != 0)
                            {


                                tableRowCellArray = tableRow.Split(new Char[] { ',', '\n' });
                                string routeLongName = tableRowCellArray[3].Trim();
                                int aposIndex = routeLongName.IndexOf("'");
                                if (aposIndex > -1)
                                {
                                    routeLongName = routeLongName.Replace("'", "''");
                                }
                                Console.WriteLine(" {0} {1} {2} {3} {4} {5} {6} {7} {8}", tableRowCellArray[0], tableRowCellArray[1].Trim(), tableRowCellArray[2], tableRowCellArray[3], tableRowCellArray[4], tableRowCellArray[5], tableRowCellArray[6], tableRowCellArray[7], tableRowCellArray[8]);
                                commandInsert.CommandText = @"INSERT INTO Routes (route_id, agency_id, route_short_name, route_long_name, route_desc, route_type, route_url, route_color, route_text_color) " +
                                                            "VALUES('" + tableRowCellArray[0] + "', '" + tableRowCellArray[1].Trim() + "', '" + tableRowCellArray[2] + "', '" + routeLongName + "', '" + tableRowCellArray[4] + "', '" + tableRowCellArray[5] + "', '" + tableRowCellArray[6] + "', '" + tableRowCellArray[7] + "', '" + tableRowCellArray[8] + "'); ";
                                commandInsert.ExecuteNonQuery();
                            }
                            i++;

                        }
                    }
                }
                catch (Exception e)
                {
                    // Let the user know what went wrong.
                    TransitDataBackend.Logging("The file could not be read:" + e.Message, true);
                }

            }
        }

    }
}


