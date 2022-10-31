using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Timers;
using System.ComponentModel;
using System.Windows;
using System.Security.AccessControl;

namespace Server
{
    public class SystemWatcher
    {
        private String systemWatcherPath = @"C:\Users\akram.mohamed\AppData\Roaming\Server";
        private FileSystemWatcher systemWatcher;
        private FTPServer ftpServer;



        private BackgroundWorker backgroundWorker;
        string message = "";
        public SystemWatcher()
        {
            ftpServer = new FTPServer();

            if (!File.Exists(Directory.GetCurrentDirectory() + "\\Track.txt"))
            {
                File.Create(Directory.GetCurrentDirectory() + "\\Track.txt");

            }


            //AddDirectorySecurity($@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics"}", System.Security.Principal.WindowsIdentity.GetCurrent().Name, FileSystemRights.Write, AccessControlType.Allow);

            ////RemoveFileSecurity($@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics"}", System.Security.Principal.WindowsIdentity.GetCurrent().Name, FileSystemRights.Write, AccessControlType.Allow);



            systemWatcher = new FileSystemWatcher(systemWatcherPath);
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundStart;
            backgroundWorker.ProgressChanged += OnBackgroundWorkerProgressChanged;
            backgroundWorker.RunWorkerCompleted += OnBackgroundWorkerComplete;
            backgroundWorker.WorkerReportsProgress = true;

            systemWatcher = new FileSystemWatcher(systemWatcherPath);
            systemWatcher.NotifyFilter = NotifyFilters.Attributes
                | NotifyFilters.CreationTime
                | NotifyFilters.DirectoryName
                | NotifyFilters.FileName
                | NotifyFilters.LastAccess
                | NotifyFilters.LastWrite
                | NotifyFilters.Security
                | NotifyFilters.Size;

            systemWatcher.Changed += OnChanged;
            systemWatcher.Created += OnCreated;
            systemWatcher.Deleted += OnDeleted;
            systemWatcher.Renamed += OnRenamed;
            systemWatcher.Error += OnError;

            systemWatcher.Filter = "*.*";
            systemWatcher.IncludeSubdirectories = true;
            systemWatcher.EnableRaisingEvents = true;


        }

        private void OnBackgroundWorkerComplete(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void OnBackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void BackgroundStart(object sender, DoWorkEventArgs e)
        {

        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            else
            {
                String mPath = e.FullPath.Substring(systemWatcherPath.Length + 1, e.FullPath.Length - systemWatcherPath.Length - 1);
                mPath = message;
            }



        }
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            String mPath = e.FullPath.Substring(systemWatcherPath.Length + 1, e.FullPath.Length - systemWatcherPath.Length - 1);

            if(mPath.Substring(0,3)!="New")
            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Server.txt", $"{File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Server.txt").Length+1}" + " " +e.FullPath+","+e.ChangeType+'\n');

            
        }
        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            String mPath = e.FullPath.Substring(systemWatcherPath.Length + 1, e.FullPath.Length - systemWatcherPath.Length - 1);
            String temp = e.Name;
            int count = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Server.txt").Length + 1;

            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Server.txt", count+" "+e.FullPath+","+e.ChangeType+'\n');

        }
        private void OnRenamed(object sender, RenamedEventArgs e)
        {

            String mNewPath = e.FullPath.Substring(systemWatcherPath.Length + 1, e.FullPath.Length - systemWatcherPath.Length - 1);
            String mOldPath = e.OldFullPath.Substring(systemWatcherPath.Length + 1, e.OldFullPath.Length - systemWatcherPath.Length - 1);

            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Server.txt", $"{File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Server.txt").Length + 1}" + " " + e.OldFullPath + "," +e.FullPath+","+e.ChangeType + '\n');

        }


        private void OnError(object sender, ErrorEventArgs e)
        {
            PrintException(e.GetException());
        }

        private static void PrintException(Exception ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }



        public void AppendChanges(string Change) {

            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Track.txt")) {

                File.Create(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Track.txt");
           
            }

            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Track.txt", Change);
           
        }


        public static void AddDirectorySecurity(string FileName, string Account, FileSystemRights Rights, AccessControlType ControlType)
        {
            // Create a new DirectoryInfo object.
            DirectoryInfo dInfo = new DirectoryInfo(FileName);

            // Get a DirectorySecurity object that represents the
            // current security settings.
            DirectorySecurity dSecurity = dInfo.GetAccessControl();

            // Add the FileSystemAccessRule to the security settings.
            dSecurity.AddAccessRule(new FileSystemAccessRule(Account,
                                                            Rights,
                                                            ControlType));
            // Set the new access settings.
            dInfo.SetAccessControl(dSecurity);
        }


        public static void RemoveFileSecurity(string fileName, string account,
            FileSystemRights rights, AccessControlType controlType)
        {

            // Get a FileSecurity object that represents the
            // current security settings.
            FileSecurity fSecurity = File.GetAccessControl(fileName);

            // Remove the FileSystemAccessRule from the security settings.
            fSecurity.RemoveAccessRule(new FileSystemAccessRule(account,
                rights, controlType));

            // Set the new access settings.
            File.SetAccessControl(fileName, fSecurity);
        }


    }
}
