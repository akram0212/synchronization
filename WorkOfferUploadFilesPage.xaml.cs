using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Spire.Doc;
using Spire.License;
using Microsoft.Win32;
using System.ComponentModel;
using _01electronics_library;
using System.IO;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for WorkOfferUploadFilesPage.xaml
    /// </summary>
    public partial class WorkOfferUploadFilesPage : Page
    {
        Employee loggedInUser;
        WorkOffer workOffer;
        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private IntegrityChecks integrityChecks;
        protected FTPServer ftpObject;

        int counter;
        int viewAddCondition;
        
        protected BackgroundWorker uploadBackground;
        protected BackgroundWorker downloadBackground;

        string[] files = new string[10];

        protected String serverFolderPath;
        protected String serverFileName;

        protected String localFolderPath;
        protected String localFileName;

        protected bool fileUploaded;

        WrapPanel wrapPanel = new WrapPanel();

        private Grid previousSelectedFile;
        private Grid currentSelectedFile;
        private Label currentLabel;

        List<string> ftpFiles;

        public WorkOfferUploadFilesPage(ref Employee mLoggedInUser, ref WorkOffer mWorkOffer, int mViewAddCondition)
        {
           
            InitializeComponent();

            sqlDatabase = new SQLServer();
            ftpObject = new FTPServer();
            integrityChecks = new IntegrityChecks();

            loggedInUser = mLoggedInUser;
            workOffer = mWorkOffer;
            viewAddCondition = mViewAddCondition;

            ftpFiles = new List<string>();

            counter = 0;

            uploadBackground = new BackgroundWorker();
            uploadBackground.DoWork += BackgroundUpload;
            uploadBackground.ProgressChanged += OnUploadProgressChanged;
            uploadBackground.RunWorkerCompleted += OnUploadBackgroundComplete;
            uploadBackground.WorkerReportsProgress = true;

            uploadFilesStackPanel.Children.Clear();
            uploadFilesStackPanel.Children.Add(wrapPanel);

            downloadBackground = new BackgroundWorker();
            downloadBackground.DoWork += BackgroundDownload;
            downloadBackground.ProgressChanged += OnDownloadProgressChanged;
            downloadBackground.RunWorkerCompleted += OnDownloadBackgroundComplete;
            downloadBackground.WorkerReportsProgress = true;

            serverFolderPath = BASIC_MACROS.OFFER_FILES_PATH + workOffer.GetOfferID() + "/";
            //integrityChecks.RemoveExtraSpaces(serverFolderPath, ref serverFolderPath);
            //serverFolderPath.Replace(@" /", @"");

            if (!ftpObject.CheckExistingFolder(serverFolderPath))
            {
                if (!ftpObject.CreateNewFolder(serverFolderPath))
                    return;
            }
            else
            {
                ftpFiles.Clear();
                if (!ftpObject.ListFilesInFolder(serverFolderPath, ref ftpFiles))
                  return;
                //if (!ftpObject.ListDirectory(serverFolderPath, ref ftpFiles))
                  //  return;
            }

            if (ftpFiles.Count != 0)
            {
                for (int i = 0; i < ftpFiles.Count; i++)
                {
                    Grid UploadIconGrid = new Grid();
                    UploadIconGrid.Margin = new Thickness(24);

                    RowDefinition row1 = new RowDefinition();
                    RowDefinition row2 = new RowDefinition();
                    RowDefinition row3 = new RowDefinition();

                    UploadIconGrid.RowDefinitions.Add(row1);
                    UploadIconGrid.RowDefinitions.Add(row2);
                    UploadIconGrid.RowDefinitions.Add(row3);

                    UploadIconGrid.MouseLeftButtonDown += OnClickIconGrid;

                    Image icon = new Image();

                    if (ftpFiles[i].Contains(".pdf"))
                        icon = new Image { Source = new BitmapImage(new Uri("C:/Users/developer/Pictures/Saved Pictures/PDFIcon.jpg")) };
                    else
                        icon = new Image { Source = new BitmapImage(new Uri("C:/Users/developer/Pictures/Saved Pictures/WordIcon.jpg")) };

                    resizeImage(ref icon);
                    UploadIconGrid.Children.Add(icon);
                    Grid.SetRow(icon, 0);

                    Label name = new Label();
                    name.Content = ftpFiles[i];
                    UploadIconGrid.Children.Add(name);
                    Grid.SetRow(name, 1);

                    Label status = new Label();
                    status.Content = "SUBMITTED";
                    status.HorizontalAlignment = HorizontalAlignment.Center;
                    BrushConverter brush = new BrushConverter();
                    status.Foreground = (System.Windows.Media.Brush)brush.ConvertFrom("#00FF00");
                    UploadIconGrid.Children.Add(status);
                    Grid.SetRow(status, 2);

                    wrapPanel.Children.Add(UploadIconGrid);
                }
            }
        }

        private void OnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {
            WorkOfferPaymentAndDeliveryPage offerPaymentAndDeliveryPage = new WorkOfferPaymentAndDeliveryPage(ref loggedInUser, ref workOffer, viewAddCondition);
            NavigationService.Navigate(offerPaymentAndDeliveryPage);
        }

        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            WorkOfferProductsPage offerProductsPage = new WorkOfferProductsPage(ref loggedInUser, ref workOffer, viewAddCondition);
            NavigationService.Navigate(offerProductsPage);
        }

        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            WorkOfferBasicInfoPage basicInfoPage = new WorkOfferBasicInfoPage(ref loggedInUser, ref workOffer, viewAddCondition);
            NavigationService.Navigate(basicInfoPage);
        }

        private void OnDropUploadFilesStackPanel(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && progressBar.Visibility == Visibility.Collapsed)
            {
                string[] temp  = (string[])e.Data.GetData(DataFormats.FileDrop);
                //files[numberOfFilesAdded] = (string)e.Data.GetData(DataFormats.FileDrop);
                //e.Effects = DragDropEffects.Copy;
                e.Effects = DragDropEffects.All;
                files[counter] = temp[0];

                localFolderPath = temp[0];
                localFileName = System.IO.Path.GetFileName(localFolderPath);

                serverFileName = localFileName;

                progressBar.Visibility = Visibility.Visible;

                counter++;

                Grid UploadIconGrid = new Grid();
                UploadIconGrid.Margin = new Thickness(24);

                RowDefinition row1 = new RowDefinition();
                RowDefinition row2 = new RowDefinition();
                RowDefinition row3 = new RowDefinition();

                UploadIconGrid.RowDefinitions.Add(row1);
                UploadIconGrid.RowDefinitions.Add(row2);
                UploadIconGrid.RowDefinitions.Add(row3);

                Image icon = new Image();

                if (localFolderPath.Contains(".pdf"))
                    icon = new Image { Source = new BitmapImage(new Uri("C:/Users/developer/Pictures/Saved Pictures/PDFIcon.jpg")) };
                else
                    icon = new Image { Source = new BitmapImage(new Uri("C:/Users/developer/Pictures/Saved Pictures/WordIcon.jpg")) };

                resizeImage(ref icon);
                UploadIconGrid.Children.Add(icon);
                Grid.SetRow(icon, 0);

                Label name = new Label();
                name.Content = localFileName;
                UploadIconGrid.Children.Add(name);
                Grid.SetRow(name, 1);

                Label status = new Label();
                status.Content = "PENDING";
                status.HorizontalAlignment = HorizontalAlignment.Center;
                BrushConverter brush = new BrushConverter();
                status.Foreground = (System.Windows.Media.Brush)brush.ConvertFrom("#FFFF00");
                UploadIconGrid.Children.Add(status);
                Grid.SetRow(status, 2);

                wrapPanel.Children.Add(UploadIconGrid);

                uploadBackground.RunWorkerAsync();
            }
        } 

        protected void BackgroundUpload(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker uploadBackground = sender as BackgroundWorker;



            uploadBackground.ReportProgress(50);
            if (ftpObject.UploadFile(localFolderPath, serverFolderPath + serverFileName))
                fileUploaded = true;
            else
                fileUploaded = false;    
            

            uploadBackground.ReportProgress(75);

            uploadBackground.ReportProgress(100);
        }

        protected void OnUploadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        protected void OnUploadBackgroundComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (wrapPanel.Children.Count != 0)
                wrapPanel.Children.RemoveAt(wrapPanel.Children.Count - 1);

            //uploadLabel.Visibility = Visibility.Visible;
            progressBar.Visibility = Visibility.Collapsed;

            if (fileUploaded == true)
            {
                Grid UploadIconGrid = new Grid();
                UploadIconGrid.Margin = new Thickness(24);

                RowDefinition row1 = new RowDefinition();
                RowDefinition row2 = new RowDefinition();
                RowDefinition row3 = new RowDefinition();

                UploadIconGrid.RowDefinitions.Add(row1);
                UploadIconGrid.RowDefinitions.Add(row2);
                UploadIconGrid.RowDefinitions.Add(row3);

                UploadIconGrid.MouseLeftButtonDown += OnClickIconGrid;

                Image icon = new Image();

                if (localFolderPath.Contains(".pdf"))
                    icon = new Image { Source = new BitmapImage(new Uri("C:/Users/developer/Pictures/Saved Pictures/PDFIcon.jpg")) };
                else
                    icon = new Image { Source = new BitmapImage(new Uri("C:/Users/developer/Pictures/Saved Pictures/WordIcon.jpg")) };

                resizeImage(ref icon);
                UploadIconGrid.Children.Add(icon);
                Grid.SetRow(icon, 0);

                Label name = new Label();
                name.Content = System.IO.Path.GetFileName(localFolderPath);
                UploadIconGrid.Children.Add(name);
                Grid.SetRow(name, 1);

                Label status = new Label();
                status.Content = "SUBMITTED";
                status.HorizontalAlignment = HorizontalAlignment.Center;
                BrushConverter brush = new BrushConverter();
                status.Foreground = (System.Windows.Media.Brush)brush.ConvertFrom("#00FF00");
                UploadIconGrid.Children.Add(status);
                Grid.SetRow(status, 2);

                wrapPanel.Children.Add(UploadIconGrid);
            }
            else
            {
                Grid UploadIconGrid = new Grid();
                UploadIconGrid.Margin = new Thickness(24);

                RowDefinition row1 = new RowDefinition();
                RowDefinition row2 = new RowDefinition();
                RowDefinition row3 = new RowDefinition();

                UploadIconGrid.RowDefinitions.Add(row1);
                UploadIconGrid.RowDefinitions.Add(row2);
                UploadIconGrid.RowDefinitions.Add(row3);

                UploadIconGrid.MouseLeftButtonDown += OnClickIconGrid;

                Image icon = new Image();

                if (localFolderPath.Contains(".pdf"))
                    icon = new Image { Source = new BitmapImage(new Uri("C:/Users/developer/Pictures/Saved Pictures/PDFIcon.jpg")) };
                else
                    icon = new Image { Source = new BitmapImage(new Uri("C:/Users/developer/Pictures/Saved Pictures/WordIcon.jpg")) };

                resizeImage(ref icon);
                UploadIconGrid.Children.Add(icon);
                Grid.SetRow(icon, 0);

                Label name = new Label();
                name.Content = System.IO.Path.GetFileName(localFolderPath);
                UploadIconGrid.Children.Add(name);
                Grid.SetRow(name, 1);

                Label status = new Label();
                status.Content = "FAILED";
                status.HorizontalAlignment = HorizontalAlignment.Center;
                BrushConverter brush = new BrushConverter();
                status.Foreground = (System.Windows.Media.Brush)brush.ConvertFrom("#FF0000");
                UploadIconGrid.Children.Add(status);
                Grid.SetRow(status, 2);

                wrapPanel.Children.Add(UploadIconGrid);
            }
        }

        public void resizeImage(ref Image imgToResize)
        {
            imgToResize.Width = 50;
            imgToResize.Height = 50;
        }

        private void OnClickIconGrid(object sender, MouseButtonEventArgs e)
        {
            
            previousSelectedFile = currentSelectedFile;
            currentSelectedFile = (Grid)sender;
            currentLabel = (Label)currentSelectedFile.Children[1];
            BrushConverter brush = new BrushConverter();

            if(previousSelectedFile != null)
            {
                previousSelectedFile.Background = (Brush)brush.ConvertFrom("#FFFFFF");
                Label previousLabel = (Label)previousSelectedFile.Children[1];
                previousLabel.Foreground = (Brush)brush.ConvertFrom("#000000");
            }

            currentSelectedFile.Background = (Brush)brush.ConvertFrom("#105A97");
            currentLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");

            if(viewAddCondition == COMPANY_WORK_MACROS.OFFER_VIEW_CONDITION)
            {
                downloadButton.Visibility = Visibility.Visible;
            }
        }
        protected void BackgroundDownload(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker downloadBackground = sender as BackgroundWorker;

            downloadBackground.ReportProgress(50);
            if (!ftpObject.DownloadFile(serverFolderPath + "/" + serverFileName, localFolderPath + "/" + localFileName))
                return;

            downloadBackground.ReportProgress(100);
        }

        protected void OnDownloadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        protected void OnDownloadBackgroundComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Visibility = Visibility.Collapsed;
            downloadButton.Visibility = Visibility.Visible;
        }

        private void OnClickDownloadButton(object sender, RoutedEventArgs e)
        {
            downloadButton.Visibility = Visibility.Collapsed;

            System.Windows.Forms.FolderBrowserDialog downloadFile = new System.Windows.Forms.FolderBrowserDialog();

            if (downloadFile.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;

            if (!integrityChecks.CheckFileEditBox(downloadFile.SelectedPath))
                return;

            serverFileName = currentLabel.Content.ToString();
            //serverFolderPath = BASIC_MACROS.OFFER_FILES_PATH + workOffer.GetOfferID() + "/" + serverFileName;
            integrityChecks.RemoveExtraSpaces(serverFileName, ref serverFileName);

            localFolderPath = downloadFile.SelectedPath;
            localFileName = System.IO.Path.GetFileName(downloadFile.SelectedPath);
            integrityChecks.RemoveExtraSpaces(localFileName, ref localFileName);

            progressBar.Visibility = Visibility.Visible;

            downloadBackground.RunWorkerAsync();
        }
    }
}
