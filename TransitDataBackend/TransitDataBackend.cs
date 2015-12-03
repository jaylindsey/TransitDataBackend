using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using System.IO.Compression;

namespace TransitDataBackend
{
    class TransitDataBackend
    {
        //Parsing Program Arguments
        static bool detailedLogging = true;
        static bool downloadRemoteFiles = false;
        static string fileToUpload = "calendar";
        static int numberOfUploadTries = 1;

        static void Main(string[] args)
        {
            //Parse program arguments
            var parser = new TransitDataBackend();
            parser.ParseArguments(args);

            TransitDataBackend.Logging("Starting Program", true);

            if (downloadRemoteFiles)
            {
                TransitDataBackend.Logging("Starting Download Manager", true);
                var manager = new FtpDownloadManager();
                manager.FtpDownloader(detailedLogging);

                var decompressor = new FileDecompressor();
                TransitDataBackend.Logging("Starting Decompression", true);

                string filePath = @"D:\home\site\wwwroot\google_transit\google_transit.zip";
                string extractPath = @"D:\home\site\wwwroot\google_transit\";
                decompressor.DecompressFile(filePath, extractPath, detailedLogging);
            }
            //not needed as we are using bulk upload 
            //TransitDataBackend.Logging("Starting SQL Insert", true);
            //var dataloader = new SqlInsert();
            //dataloader.LoadDataIntoRouteTable(detailedLogging);
            //truncate the destination table before upload

            
            TransitDataBackend.Logging("Starting Data Uploader", true);
            //now upload
            var dataUploader = new DataUploader(detailedLogging);
            dataUploader.LoadData(detailedLogging, fileToUpload, numberOfUploadTries);
            
        }
        public static void Logging(string log, bool verbose)
        {
            if (verbose)
            {
                Console.Out.WriteLine(log);
            }
        }

        public void ParseArguments(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0].ToLower().Equals("verbose"))
                {
                    detailedLogging = true;
                }

                if (args.Length > 1)
                {
                    if (args[1].ToLower().Equals("skipdownload"))
                    {
                        downloadRemoteFiles = false;
                    }
                }

                if (args.Length > 2)
                {
                    fileToUpload = args[2];
                }

                if (args.Length > 3)
                {
                    if (!int.TryParse(args[3],out numberOfUploadTries))
                    {
                        numberOfUploadTries = 1;
                    }
                }
            }
            else
            {
                using (StreamReader argumentStream = new StreamReader(@"D:\home\site\wwwroot\google_transit\arguments.txt"))
                {
                    string argumentString = argumentStream.ReadToEnd();
                    string[] argArray = argumentString.Split(new Char[] { ' ', '\n' });

                    if (argArray[0].ToLower().Equals("verbose"))
                    {
                        detailedLogging = true;
                    }

                    if (argArray.Length > 1)
                    {
                        if (argArray[1].ToLower().Equals("skipdownload"))
                        {
                            downloadRemoteFiles = false;
                        }
                    }

                    if (argArray.Length > 2)
                    {
                        fileToUpload = argArray[2];
                    }

                    if (argArray.Length > 3)
                    {
                        if (!int.TryParse(argArray[3], out numberOfUploadTries))
                        {
                            numberOfUploadTries = 1;
                        }
                    }

                }
            }
        }

    }
}
