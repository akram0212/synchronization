using _01electronics_library;
using System;
using System.ComponentModel;
using System.Windows.Navigation;
using _01electronics_windows_library;
using System.IO;
using System.Windows;
using System.Timers;
using System.Threading;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {

        BackgroundWorker syncWorker = new BackgroundWorker();
        bool canceled = false;
        int progress = 0;
        bool closedWindow = false;
        private BackgroundWorker backgroundWorker = new BackgroundWorker();


        //Timer timer=new Timer(60000);

        FTPServer ftpServer = new FTPServer();
        bool fileFound = true;
        public MainWindow(ref Employee mLoggedInUser)
        {
            SystemWatcher watcher = new SystemWatcher();

            syncWorker.DoWork += SyncWorker_DoWork;

            this.Closing += NavigationWindow_Closing;

            backgroundWorker.WorkerReportsProgress = true;
  

            InitializeComponent();
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics"))
            {
                fileFound = false;
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\erp_system\\products_photos");
                //ftpServer.GetModificationTime();

            }


            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\Upload"))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\Upload");
                //ftpServer.GetModificationTime();

            }

            if (!File.Exists(Directory.GetCurrentDirectory() + "\\LastInstruction.txt")) {

                File.Create(Directory.GetCurrentDirectory() + "\\LastInstruction.txt").Close();
            }


            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Client")) {

               Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Client");

            }


            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Server"))
            {

                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Server");

            }

            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Client.txt")) {

                 File.Create(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Client.txt");
            
            }


            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Server.txt"))
            {

                 File.Create(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Server.txt");

            }

            ftpServer.upload();


            if (fileFound == false)
            {
                backgroundWorker.DoWork += BackgroundStart;
                backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
                backgroundWorker.RunWorkerAsync();
            }

            //timer.Elapsed += (o, s) => Task.Factory.StartNew(() => OnTimerElapsed(o, s));
            //timer.Start();

            StatisticsPage statisticsPage = new StatisticsPage(ref mLoggedInUser);
            this.NavigationService.Navigate(statisticsPage);

        }

        public void c(string old,string path) {


            //this.Dispatcher.Invoke(() =>
            //{
            //    this.Close();
            //});

        }

        private void SyncWorker_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progress = e.ProgressPercentage;
        }


        //private void OnTimerElapsed(object o, ElapsedEventArgs s)
        //{
        //    if (ftpServer.CheckDateChanged() == false)
        //        MessageBox.Show("Nothing Changed");
        //    else {
        //        ftpServer.GetFileParsing();     
        //    }
        //}

        public MainWindow()
        {
        }


        private void BackgroundStart(object sender,DoWorkEventArgs e)
        {
            backgroundWorker.WorkerReportsProgress = true;

            String errorMessage = String.Empty;
            if (!ftpServer.DownloadFolder(BASIC_MACROS.MODELS_PHOTOS_PATH,Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\erp_system\\products_photos\\", ref errorMessage))
            {
                return;
            }


            backgroundWorker.ReportProgress(100);

            if (closedWindow == true)
            {
                CancelEventArgs cancelEventArgs = new CancelEventArgs();
                NavigationWindow_Closing(null, cancelEventArgs);
            }


        }

        //private void BackgroundExecuteRest(object sender,RunWorkerCompletedEventArgs e)
        //{
        //    String errorMessage = String.Empty;
        //    if (!ftpServer.DownloadFolder(BASIC_MACROS.MODELS_PHOTOS_PATH, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\products_photos\\", ref errorMessage))
        //    {
        //        return;
        //    }

        //    backgroundWorker.ReportProgress(100);
        //    if (closedWindow == true) {
        //        CancelEventArgs cancelEventArgs = new CancelEventArgs();
        //        cancelEventArgs.Cancel = false;
        //        NavigationWindow_Closing(null, cancelEventArgs);
        //    }

        //    Thread.CurrentThread.Suspend();
        //}

        private void NavigationWindow_Closing(object sender, CancelEventArgs e)
        {
            if (canceled == true)
                return;
            closedWindow = true;

            this.Dispatcher.Invoke(() =>
            {

                if (backgroundWorker.IsBusy == true)
                {

                    if (progress == 100)
                    {
                        canceled = true;
                        this.Close();

                    }
                    else
                    {
                        e.Cancel = true;
                        this.Hide();
                    }

                }
                else {
                    e.Cancel = false;
                }

            });
           
        }

        private void NavigationWindow_Closed(object sender, EventArgs e)
        {

        }
    }
}
