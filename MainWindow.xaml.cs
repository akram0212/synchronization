using _01electronics_library;
using System;
using System.ComponentModel;
using System.Windows.Navigation;
using _01electronics_windows_library;
using System.IO;
namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {

        
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        FTPServer ftpServer = new FTPServer();

        public MainWindow(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\products_photos\\products");

            backgroundWorker.DoWork += BackgroundStart;
            backgroundWorker.RunWorkerCompleted += BackgroundExecuteRest;

            backgroundWorker.RunWorkerAsync();
      

            StatisticsPage statisticsPage = new StatisticsPage(ref mLoggedInUser);
            this.NavigationService.Navigate(statisticsPage);

        }
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
            if (!ftpServer.DownloadFolder(BASIC_MACROS.MODELS_PHOTOS_PATH, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\products_photos\\", ref errorMessage))
            {
                return;
            }


        }

    }
}
