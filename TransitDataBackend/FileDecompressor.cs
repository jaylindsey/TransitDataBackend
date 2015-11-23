using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace TransitDataBackend
{
    class FileDecompressor
    {
        public void DecompressFile(string compressedFilePath, string extractPath, bool detailedLogging)
        {
            string filePath = @"D:\home\site\wwwroot\google_transit\";
            string[] fileArray = { "calendar.txt", "agency.txt", "routes.txt", "calendar_dates.txt", "trips.txt", "stops.txt", "shapes.txt", "stop_times.txt", "readme.txt", "notice.rtf" };
            foreach (string fileName in fileArray)
            {
                if (File.Exists(filePath + fileName))
                {
                    try
                    {
                        File.Delete(filePath + fileName);
                        TransitDataBackend.Logging("File Decompressor: Deleting Old Files:" + fileName + " deleted", detailedLogging);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }
                }
            }
            TransitDataBackend.Logging("File Decompressor: Extracting Files", detailedLogging);
            using (ZipArchive archive = ZipFile.OpenRead(compressedFilePath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    entry.ExtractToFile(Path.Combine(extractPath, entry.FullName));
                    string ZipArchiveEntryName = entry.ToString();
                    TransitDataBackend.Logging("File Decompressor:" + ZipArchiveEntryName + " extracted", detailedLogging);
                }
            }
        }
    }
}
