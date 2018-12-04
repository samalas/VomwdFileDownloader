using System;
using System.Linq;
using WinSCP;

namespace VomwdFileDownloader
{
    class Program
    {
 
        static int Main(string[] args)
        {
            try
            {
                
                string HostName = System.Configuration.ConfigurationManager.AppSettings["HostName"];
                string UserName = System.Configuration.ConfigurationManager.AppSettings["UserName"];
                string Password = System.Configuration.ConfigurationManager.AppSettings["Password"];
                string SshHostKeyFingerprint = System.Configuration.ConfigurationManager.AppSettings["sshHostKeyFingerprint"];

                // Setup session options
                SessionOptions sessionOptions = new SessionOptions()
                {
                    Protocol = Protocol.Sftp,
                    HostName = HostName,
                    UserName = UserName,
                    Password = Password,
                    SshHostKeyFingerprint = SshHostKeyFingerprint
                };

                //sftp.mobile-mms.com
                //vomwd
                //g3o4Dgb8lWLX
                //ssh-ed25519 256 93:00:b7:4a:fc:33:9a:7f:d5:4c:de:27:66:76:ee:d4


                using (Session session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);

                    string remotePath = System.Configuration.ConfigurationManager.AppSettings["remotePath"];
                    string localPath = System.Configuration.ConfigurationManager.AppSettings[@"localPath"];

                    ///home/vomwd/vomwd
                    //@"C:\Users\sach.samala\Documents\Testing VOMWD Downloader\"


                    // Get list of files in the directory
                    RemoteDirectoryInfo directoryInfo = session.ListDirectory(remotePath);

                    // Select the most recent file
                    RemoteFileInfo latest =
                        directoryInfo.Files
                            .Where(file => !file.IsDirectory)
                            .OrderByDescending(file => file.LastWriteTime)
                            .FirstOrDefault();

                    // Any file at all?
                    if (latest == null)
                    {
                        throw new Exception("No file found");
                    }

                    // Download the selected file
                    session.GetFiles(
                        RemotePath.EscapeFileMask(remotePath + "/" + latest.Name), localPath).Check();
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                return 1;
            }
        }
    }
}
