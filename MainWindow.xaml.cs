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
        bool closedWindow = false;
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        //Timer timer=new Timer(60000);

        //SystemWatcher fileSystemWatcher = new SystemWatcher();
        FTPServer ftpServer = new FTPServer();
        bool fileFound = true;
        public MainWindow(ref Employee mLoggedInUser)
        {

            this.Closing += NavigationWindow_Closing;

  
            InitializeComponent();
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\Track.txt"))
            {
                fileFound = false;
               File.Create(Directory.GetCurrentDirectory() + "\\Track.txt").Close();
                //ftpServer.GetModificationTime();

            }
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\products_photos");


            if (fileFound == false)
            {
                backgroundWorker.DoWork += BackgroundStart;
                backgroundWorker.RunWorkerAsync();
            }

            //timer.Elapsed += (o, s) => Task.Factory.StartNew(() => OnTimerElapsed(o, s));
            //timer.Start();

            StatisticsPage statisticsPage = new StatisticsPage(ref mLoggedInUser);
            this.NavigationService.Navigate(statisticsPage);

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

        private void BackgroundStart(object sender, DoWorkEventArgs e)
        {

            //ftpServer.CheckChangingTime(BASIC_MACROS.MODELS_PHOTOS_PATH);
            String errorMessage = String.Empty;
            if (!ftpServer.DownloadFolder(BASIC_MACROS.PRODUCTS_PHOTOS_PATH,Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\products_photos\\products\\", ref errorMessage))
            {
                return;
            }
        }

        private void BackgroundExecuteRest(object sender, RunWorkerCompletedEventArgs e)
        {
            String errorMessage = String.Empty;
            if (!ftpServer.DownloadFolder(BASIC_MACROS.MODELS_PHOTOS_PATH,Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\products_photos\\", ref errorMessage))
            {
                return;
            }

            backgroundWorker.ReportProgress(100);
            if (closedWindow == true)
            {
                CancelEventArgs cancelEventArgs = new CancelEventArgs();
                cancelEventArgs.Cancel = false;
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
            closedWindow = true;
            if (backgroundWorker.WorkerReportsProgress == true)
                this.Close();
            else
            {
                e.Cancel = true;
                this.Hide();
           }
        }

        private void NavigationWindow_Closed(object sender, EventArgs e)
        {


        }
    }
}
