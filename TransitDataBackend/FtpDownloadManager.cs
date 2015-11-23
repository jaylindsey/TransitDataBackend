using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TransitDataBackend
{
    public class FtpDownloadManager
    {
        public void FtpDownloader(bool detailedLogging)
        {
            string inputfilepath = @"D:\home\site\wwwroot\google_transit\google_transit.zip";
            string ftphost = "gisftp.metc.state.mn.us";
            string ftpfilepath = "/google_transit.zip";
            string ftpfullpath = "ftp://" + ftphost + ftpfilepath;
            TransitDataBackend.Logging("FTP Download: Starting Web Client", detailedLogging);
            using (WebClient request = new WebClient())
            {
                //request.Credentials = new NetworkCredential("UserName", "P@55w0rd");
                byte[] fileData = request.DownloadData(ftpfullpath);
                TransitDataBackend.Logging("FTP Download: Starting File Stream", detailedLogging);
                using (FileStream file = File.Create(inputfilepath))
                {
                    TransitDataBackend.Logging("FTP Download: Writing File", detailedLogging);
                    file.Write(fileData, 0, fileData.Length);
                }
                TransitDataBackend.Logging("FTP Download: Download Complete", true);
            }
        }
    }
}
