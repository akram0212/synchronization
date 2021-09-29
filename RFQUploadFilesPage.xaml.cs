﻿using System;
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
    /// Interaction logic for RFQUploadFilesPage.xaml
    /// </summary>
    public partial class RFQUploadFilesPage : Page
    {
        protected Employee loggedInUser;
        protected RFQ rfq;

        protected SQLServer sqlDatabase;

        protected IntegrityChecks integrityChecks;
        protected FTPServer ftpObject;
        
        protected int counter;
        protected int viewAddCondition;

        protected BackgroundWorker uploadBackground;
        protected BackgroundWorker downloadBackground;

        protected String serverFolderPath;
        protected String serverFileName;

        protected String localFolderPath;
        protected String localFileName;

        protected bool fileUploaded;
        protected bool fileDownloaded;
        protected bool uploadThisFile = false;
        protected bool checkFileInServer = false;


        WrapPanel wrapPanel = new WrapPanel();

        private Grid previousSelectedFile;
        private Grid currentSelectedFile;
        private Grid overwriteFileGrid;
        private Label currentLabel;

        List<string> ftpFiles;

        ProgressBar progressBar = new ProgressBar();

        public RFQBasicInfoPage rfqBasicInfoPage;
        public RFQProductsPage rfqProductsPage;
        public RFQAdditionalInfoPage rfqAdditionalInfoPage;

        public RFQUploadFilesPage(ref Employee mLoggedInUser, ref RFQ mRFQ, int mViewAddCondition)
        {
            InitializeComponent();
            sqlDatabase = new SQLServer();
            ftpObject = new FTPServer();
            integrityChecks = new IntegrityChecks();

            loggedInUser = mLoggedInUser;
            rfq = mRFQ;
            viewAddCondition = mViewAddCondition;

            ftpFiles = new List<string>();

            counter = 0;

            progressBar.Style = (Style)FindResource("ProgressBarStyle");
            progressBar.HorizontalAlignment = HorizontalAlignment.Center;
            progressBar.Width = 200;

            uploadBackground = new BackgroundWorker();
            uploadBackground.DoWork += BackgroundUpload;
            uploadBackground.ProgressChanged += OnUploadProgressChanged;
            uploadBackground.RunWorkerCompleted += OnUploadBackgroundComplete;
            uploadBackground.WorkerReportsProgress = true;

            uploadFilesStackPanel.Children.Clear();

            downloadBackground = new BackgroundWorker();
            downloadBackground.DoWork += BackgroundDownload;
            downloadBackground.ProgressChanged += OnDownloadProgressChanged;
            downloadBackground.RunWorkerCompleted += OnDownloadBackgroundComplete;
            downloadBackground.WorkerReportsProgress = true;

            serverFolderPath = BASIC_MACROS.RFQ_FILES_PATH + rfq.GetRFQID() + "/";


            if (!ftpObject.CheckExistingFolder(serverFolderPath))
            {
                if (!ftpObject.CreateNewFolder(serverFolderPath))
                {
                    InsertErrorRetryButton();
                    return;
                }
            }
            else
            {
                ftpFiles.Clear();
                if (!ftpObject.ListFilesInFolder(serverFolderPath, ref ftpFiles))
                {
                    InsertErrorRetryButton();
                    return;
                }
                    
            }

            if (ftpFiles.Count != 0)
            {
                uploadFilesStackPanel.Children.Add(wrapPanel);

                for (int i = 0; i < ftpFiles.Count; i++)
                {
                    if (ftpFiles[i] != "." || ftpFiles[i] != "..")
                        InsertIconGridFromServer(i);
                }
                InsertAddFilesIcon();
            }
            else if (ftpFiles.Count == 0)
            {
                InsertDragAndDropOrBrowseGrid();
            }
        }

        ~RFQUploadFilesPage()
        {
            //GC.Collect();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INSERT FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void InsertAddFilesIcon()
        {
            Grid addFilesGrid = new Grid();
            addFilesGrid.Margin = new Thickness(24);
            addFilesGrid.Width = 250;

            RowDefinition addFilesRow1 = new RowDefinition();
            RowDefinition addFilesRow2 = new RowDefinition();
            addFilesGrid.RowDefinitions.Add(addFilesRow1);
            addFilesGrid.RowDefinitions.Add(addFilesRow2);

            addFilesGrid.MouseLeftButtonDown += OnClickAddFilesGrid;

            Image addFilesImage = new Image();

            addFilesImage = new Image { Source = new BitmapImage(new Uri(@"photos\addfiles_icon.jpg", UriKind.Relative)) };
            resizeImage(ref addFilesImage, 50, 50);
            addFilesGrid.Children.Add(addFilesImage);
            Grid.SetRow(addFilesImage, 0);

            Label addFilesLabel = new Label();
            addFilesLabel.HorizontalAlignment = HorizontalAlignment.Center;
            addFilesLabel.Content = "Double-Click to ADD FILES";
            addFilesGrid.Children.Add(addFilesLabel);
            Grid.SetRow(addFilesLabel, 1);

            wrapPanel.Children.Add(addFilesGrid);
        }
        private void InsertIconGridFromServer(int i)
        {
            Grid UploadIconGrid = new Grid();
            UploadIconGrid.Margin = new Thickness(24);
            UploadIconGrid.Width = 250;

            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            RowDefinition row3 = new RowDefinition();
            RowDefinition row4 = new RowDefinition();

            UploadIconGrid.RowDefinitions.Add(row1);
            UploadIconGrid.RowDefinitions.Add(row2);
            UploadIconGrid.RowDefinitions.Add(row3);
            UploadIconGrid.RowDefinitions.Add(row4);

            UploadIconGrid.MouseLeftButtonDown += OnClickIconGrid;

            Image icon = new Image();

            LoadIcon(ref icon, ftpFiles[i]);

            resizeImage(ref icon, 50, 50);
            UploadIconGrid.Children.Add(icon);
            Grid.SetRow(icon, 0);

            Label name = new Label();
            name.Content = ftpFiles[i];
            name.HorizontalAlignment = HorizontalAlignment.Center;
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
        private void InsertIconGrid(string mStatus, string localFolderPath)
        {
            Grid UploadIconGrid = new Grid();
            UploadIconGrid.Margin = new Thickness(24);
            UploadIconGrid.Width = 250;
            UploadIconGrid.MouseLeftButtonDown += OnClickIconGrid;

            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            RowDefinition row3 = new RowDefinition();
            RowDefinition row4 = new RowDefinition();


            UploadIconGrid.RowDefinitions.Add(row1);
            UploadIconGrid.RowDefinitions.Add(row2);
            UploadIconGrid.RowDefinitions.Add(row3);
            UploadIconGrid.RowDefinitions.Add(row4);

            Image icon = new Image();

            LoadIcon(ref icon, localFolderPath);

            resizeImage(ref icon, 50, 50);
            UploadIconGrid.Children.Add(icon);
            Grid.SetRow(icon, 0);

            Label name = new Label();
            name.Content = localFileName;
            name.HorizontalAlignment = HorizontalAlignment.Center;
            UploadIconGrid.Children.Add(name);
            Grid.SetRow(name, 1);

            Label status = new Label();
            BrushConverter brush = new BrushConverter();
            if (mStatus == "pending")
            {
                status.Content = "PENDING";
                status.Foreground = (System.Windows.Media.Brush)brush.ConvertFrom("#FFFF00");
            }
            else if (mStatus == "submitted")
            {
                status.Content = "SUBMITTED";
                status.Foreground = (System.Windows.Media.Brush)brush.ConvertFrom("#00FF00");
            }
            else if (mStatus == "failed")
            {
                status.Content = "FAILED";
                status.Foreground = (System.Windows.Media.Brush)brush.ConvertFrom("#FF0000");
            }
            status.HorizontalAlignment = HorizontalAlignment.Center;
            UploadIconGrid.Children.Add(status);
            Grid.SetRow(status, 2);

            wrapPanel.Children.Add(UploadIconGrid);
        }

        private void InsertDragAndDropOrBrowseGrid()
        {
            Grid grid = new Grid();
            grid.VerticalAlignment = VerticalAlignment.Center;
            grid.HorizontalAlignment = HorizontalAlignment.Center;

            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            RowDefinition row3 = new RowDefinition();

            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            grid.RowDefinitions.Add(row3);

            Image icon = new Image();

            icon = new Image { Source = new BitmapImage(new Uri(@"photos\drop_files_icon.jpg", UriKind.Relative)) };
            icon.HorizontalAlignment = HorizontalAlignment.Center;
            icon.VerticalAlignment = VerticalAlignment.Center;
            resizeImage(ref icon, 250, 150);
            grid.Children.Add(icon);
            Grid.SetRow(icon, 0);

            Label orLabel = new Label();
            orLabel.HorizontalAlignment = HorizontalAlignment.Center;
            orLabel.Content = "OR";
            orLabel.FontWeight = FontWeights.Bold;
            orLabel.FontSize = 20;
            orLabel.Foreground = Brushes.Gray;
            grid.Children.Add(orLabel);
            Grid.SetRow(orLabel, 1);

            Button browseFileButton = new Button();
            browseFileButton.Style = (Style)FindResource("buttonBrowseStyle");
            browseFileButton.Width = 200;
            browseFileButton.Background = null;
            browseFileButton.Foreground = Brushes.Gray;
            browseFileButton.Content = "BROWSE FILE";
            browseFileButton.Click += OnClickBrowseButton;
            grid.Children.Add(browseFileButton);
            Grid.SetRow(browseFileButton, 2);

            uploadFilesStackPanel.Children.Add(grid);
        }

        private void InsertErrorRetryButton()
        {
            Grid grid = new Grid();
            grid.VerticalAlignment = VerticalAlignment.Center;
            grid.HorizontalAlignment = HorizontalAlignment.Center;

            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            
            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);

            Image icon = new Image();

            icon = new Image { Source = new BitmapImage(new Uri(@"photos\no_internet_icon.jpg", UriKind.Relative)) };
            icon.HorizontalAlignment = HorizontalAlignment.Center;
            icon.VerticalAlignment = VerticalAlignment.Center;
            resizeImage(ref icon, 250, 250);
            grid.Children.Add(icon);
            Grid.SetRow(icon, 0);

            Button retryButton = new Button();
            retryButton.Style = (Style)FindResource("buttonBrowseStyle");
            retryButton.Width = 200;
            retryButton.Background = null;
            retryButton.Foreground = Brushes.Gray;
            retryButton.Content = "Retry";
            retryButton.Click += OnClickRetryButton;
            grid.Children.Add(retryButton);
            Grid.SetRow(retryButton, 1);

            uploadFilesStackPanel.Children.Add(grid);
        }

        private void LoadIcon(ref Image icon, string ftpFiles)
        {
            if (ftpFiles.Contains(".pdf"))
                icon = new Image { Source = new BitmapImage(new Uri(@"photos\pdf_icon.jpg", UriKind.Relative)) };

            else if (ftpFiles.Contains(".doc") || ftpFiles.Contains(".docs") || ftpFiles.Contains(".docx"))
                icon = new Image { Source = new BitmapImage(new Uri(@"photos\word_icon.jpg", UriKind.Relative)) };

            else if (ftpFiles.Contains(".txt") || ftpFiles.Contains(".rtf"))
                icon = new Image { Source = new BitmapImage(new Uri(@"photos\text_icon.jpg", UriKind.Relative)) };

            else if (ftpFiles.Contains(".xls") || ftpFiles.Contains(".xlsx") || ftpFiles.Contains(".csv"))
                icon = new Image { Source = new BitmapImage(new Uri(@"photos\excel_icon.jpg", UriKind.Relative)) };

            else if (ftpFiles.Contains(".jpg") || ftpFiles.Contains(".png") || ftpFiles.Contains(".raw") || ftpFiles.Contains(".jpeg") || ftpFiles.Contains(".gif"))
                icon = new Image { Source = new BitmapImage(new Uri(@"photos\image_icon.jpg", UriKind.Relative)) };

            else if (ftpFiles.Contains(".rar") || ftpFiles.Contains(".zip") || ftpFiles.Contains(".gzip"))
                icon = new Image { Source = new BitmapImage(new Uri(@"photos\winrar_icon.jpg", UriKind.Relative)) };

            else if (ftpFiles.Contains(".ppt") || ftpFiles.Contains(".pptx") || ftpFiles.Contains(".pptm"))
                icon = new Image { Source = new BitmapImage(new Uri(@"photos\powerpoint_icon.jpg", UriKind.Relative)) };

            else if (ftpFiles != ".." || ftpFiles != ".")
                icon = new Image { Source = new BitmapImage(new Uri(@"photos\unknown_icon.jpg", UriKind.Relative)) };
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///ON DROP HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnDropUploadFilesStackPanel(object sender, DragEventArgs e)
        {
            if (ftpFiles.Count == 0)
            {
                uploadFilesStackPanel.Children.Clear();
                uploadFilesStackPanel.Children.Add(wrapPanel);
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] temp = (string[])e.Data.GetData(DataFormats.FileDrop);

                e.Effects = DragDropEffects.All;

                for (int i = 0; i < temp.Count(); i++)
                {
                    localFolderPath = temp[i];
                    localFileName = System.IO.Path.GetFileName(localFolderPath);

                    serverFileName = localFileName;

                    CheckIfFileAlreadyUploaded(localFileName);

                    if (uploadThisFile == true && checkFileInServer == false)
                    {

                        if (wrapPanel.Children.Count != 0)
                            wrapPanel.Children.RemoveAt(wrapPanel.Children.Count - 1);

                        progressBar.Visibility = Visibility.Visible;

                        ftpFiles.Add(localFileName);

                        InsertIconGrid("pending", localFolderPath);

                        currentSelectedFile = (Grid)wrapPanel.Children[wrapPanel.Children.Count - 1];
                        currentSelectedFile.Children.Add(progressBar);
                        Grid.SetRow(progressBar, 3);

                        uploadBackground.RunWorkerAsync();

                        while (uploadBackground.IsBusy)
                        {
                            System.Windows.Forms.Application.DoEvents();
                        }

                        uploadThisFile = false;
                    }
                    else if (uploadThisFile == true)
                    {
                        uploadBackground.RunWorkerAsync();

                        while (uploadBackground.IsBusy)
                        {
                            System.Windows.Forms.Application.DoEvents();
                        }

                        uploadThisFile = false;
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///BUTTON CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///

        //private void OnClickCancelButton(object sender, RoutedEventArgs e)
        //{
            //    if (viewAddCondition != COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
            //    {
            //        if (ftpFiles.Count() == 0 && wrapPanel.Children.Count == 0)
            //            ftpObject.DeleteFtpDirectory(serverFolderPath);
            //
            //        else
            //        {
            //            for (int i = 0; i < ftpFiles.Count(); i++)
            //            {
            //                ftpObject.DeleteFtpFile(serverFolderPath + ftpFiles[i]);
            //            }
            //
            //            ftpObject.DeleteFtpDirectory(serverFolderPath);
            //        }
            //    }
            //
            //    NavigationWindow currentWindow = (NavigationWindow)this.Parent;
            //    currentWindow.Close();
            //
            //
            
        //}

        private void OnButtonClickOk(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;

            GC.Collect();

            currentWindow.Close();
        }

        private void OnClickBackButton(object sender, RoutedEventArgs e)
        {
            rfqAdditionalInfoPage.rfqBasicInfoPage = rfqBasicInfoPage;
            rfqAdditionalInfoPage.rfqProductsPage = rfqProductsPage;
            rfqAdditionalInfoPage.rfqUploadFilesPage = this;

            NavigationService.Navigate(rfqAdditionalInfoPage);
        }

        private void OnClickIconGrid(object sender, MouseButtonEventArgs e)
        {
            previousSelectedFile = currentSelectedFile;
            currentSelectedFile = (Grid)sender;
            currentLabel = (Label)currentSelectedFile.Children[1];
            BrushConverter brush = new BrushConverter();

            if (previousSelectedFile != null && previousSelectedFile != currentSelectedFile)
            {
                previousSelectedFile.Background = (Brush)brush.ConvertFrom("#FFFFFF");
                Label previousLabel = (Label)previousSelectedFile.Children[1];
                previousLabel.Foreground = (Brush)brush.ConvertFrom("#000000");
            }

            if (previousSelectedFile != currentSelectedFile)
            {
                currentSelectedFile.Background = (Brush)brush.ConvertFrom("#105A97");
                currentLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");
            }
            else
            {
                System.Windows.Forms.FolderBrowserDialog downloadFile = new System.Windows.Forms.FolderBrowserDialog();

                if (downloadFile.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                    return;

                if (!integrityChecks.CheckFileEditBox(downloadFile.SelectedPath))
                    return;

                serverFileName = currentLabel.Content.ToString();
                integrityChecks.RemoveExtraSpaces(serverFileName, ref serverFileName);

                localFolderPath = downloadFile.SelectedPath;
                localFileName = serverFileName;

                progressBar.Visibility = Visibility.Visible;
                currentSelectedFile.Children.Add(progressBar);
                Grid.SetRow(progressBar, 3);

                Label currentStatusLabel = (Label)currentSelectedFile.Children[2];
                currentStatusLabel.Content = "DOWNLOADING";
                currentStatusLabel.Foreground = (System.Windows.Media.Brush)brush.ConvertFrom("#FFFF00");

                downloadBackground.RunWorkerAsync();
            }
        }

        private void OnClickAddFilesGrid(object sender, MouseButtonEventArgs e)
        {
            previousSelectedFile = currentSelectedFile;
            currentSelectedFile = (Grid)sender;
            currentLabel = (Label)currentSelectedFile.Children[1];
            BrushConverter brush = new BrushConverter();

            if (previousSelectedFile != null && previousSelectedFile != currentSelectedFile)
            {
                previousSelectedFile.Background = (Brush)brush.ConvertFrom("#FFFFFF");
                Label previousLabel = (Label)previousSelectedFile.Children[1];
                previousLabel.Foreground = (Brush)brush.ConvertFrom("#000000");
            }

            if (previousSelectedFile != currentSelectedFile)
            {
                currentSelectedFile.Background = (Brush)brush.ConvertFrom("#105A97");
                currentLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");
            }
            else
            {
                OpenFileDialog uploadFile = new OpenFileDialog();

                if (uploadFile.ShowDialog() == false)
                    return;

                if (!integrityChecks.CheckFileEditBox(uploadFile.FileName))
                    return;

                localFolderPath = uploadFile.FileName;
                localFileName = System.IO.Path.GetFileName(localFolderPath);

                serverFileName = localFileName;

                CheckIfFileAlreadyUploaded(localFileName);

                if (uploadThisFile == true && checkFileInServer == false)
                {
                    ftpFiles.Add(localFileName);

                    //uploadFilesStackPanel.Children.Clear();
                    //uploadFilesStackPanel.Children.Add(wrapPanel);

                    if (wrapPanel.Children.Count != 0)
                        wrapPanel.Children.RemoveAt(wrapPanel.Children.Count - 1);

                    InsertIconGrid("pending", localFolderPath);

                    currentSelectedFile = (Grid)wrapPanel.Children[wrapPanel.Children.Count - 1];
                    currentSelectedFile.Children.Add(progressBar);
                    Grid.SetRow(progressBar, 3);

                    uploadBackground.RunWorkerAsync();

                    uploadThisFile = false;
                }
                else if (checkFileInServer == true)
                {
                    uploadBackground.RunWorkerAsync();

                    uploadThisFile = false;
                }
            }
        }

        private void OnClickBrowseButton(object sender, RoutedEventArgs e)
        {
            OpenFileDialog uploadFile = new OpenFileDialog();

            if (uploadFile.ShowDialog() == false)
                return;

            if (!integrityChecks.CheckFileEditBox(uploadFile.FileName))
                return;

            if (ftpFiles.Count == 0)
            {
                uploadFilesStackPanel.Children.Clear();
                uploadFilesStackPanel.Children.Add(wrapPanel);
            }

            localFolderPath = uploadFile.FileName;
            localFileName = System.IO.Path.GetFileName(localFolderPath);

            serverFileName = localFileName;

            ftpFiles.Add(localFileName);

            InsertIconGrid("pending", localFolderPath);
            currentSelectedFile = (Grid)wrapPanel.Children[wrapPanel.Children.Count - 1];
            currentSelectedFile.Children.Add(progressBar);
            Grid.SetRow(progressBar, 3);

            uploadBackground.RunWorkerAsync();
        }

        private void OnClickRetryButton(object sender, RoutedEventArgs e)
        {
            
            FTPServer fTPServer = new FTPServer();

            if (!fTPServer.CheckExistingFolder(serverFolderPath))
            {
                if (!fTPServer.CreateNewFolder(serverFolderPath))
                {
                    uploadFilesStackPanel.Children.Clear();
                    InsertErrorRetryButton();
                    return;
                }
            }
            else
            {
                ftpFiles.Clear();
                if (!fTPServer.ListFilesInFolder(serverFolderPath, ref ftpFiles))
                {
                    uploadFilesStackPanel.Children.Clear();
                    InsertErrorRetryButton();
                    return;
                }

            }

            if (ftpFiles.Count != 0)
            {
                uploadFilesStackPanel.Children.Clear();
                uploadFilesStackPanel.Children.Add(wrapPanel);

                for (int i = 0; i < ftpFiles.Count; i++)
                {
                    if (ftpFiles[i] != "." || ftpFiles[i] != "..")
                        InsertIconGridFromServer(i);
                }
                InsertAddFilesIcon();
            }
            else if (ftpFiles.Count == 0)
            {
                uploadFilesStackPanel.Children.Clear();
                InsertDragAndDropOrBrowseGrid();
            }

        }




        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///BACKGROUND COMPLETE HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected void OnUploadBackgroundComplete(object sender, RunWorkerCompletedEventArgs e)
        {

            if (checkFileInServer == false)
            {
                if (wrapPanel.Children.Count != 0)
                    wrapPanel.Children.RemoveAt(wrapPanel.Children.Count - 1);

                currentSelectedFile.Children.Remove(progressBar);

                if (fileUploaded == true)
                {
                    InsertIconGrid("submitted", localFolderPath);
                }
                else
                {
                    InsertIconGrid("failed", localFolderPath);
                }

                InsertAddFilesIcon();
            }

            else
            {
                overwriteFileGrid.Children.Remove(progressBar);

                BrushConverter brush = new BrushConverter();
                Label overwriteFileLabel = (Label)overwriteFileGrid.Children[2];

                if (fileUploaded == true)
                {
                    overwriteFileLabel.Content = "SUBMITTED";
                    overwriteFileLabel.Foreground = (System.Windows.Media.Brush)brush.ConvertFrom("#00FF00");
                }
                else
                {
                    overwriteFileLabel.Content = "Failed";
                    overwriteFileLabel.Foreground = (System.Windows.Media.Brush)brush.ConvertFrom("#FF0000");
                }
            }
        }
        protected void OnDownloadBackgroundComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            currentSelectedFile.Children.Remove(progressBar);
            Label currentStatusLabel = (Label)currentSelectedFile.Children[2];
            if (fileDownloaded == true)
                currentStatusLabel.Content = "Downloaded";
            else
                currentStatusLabel.Content = "Failed";
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///PROGRESS CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected void OnUploadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }
        protected void OnDownloadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///BACKGROUND FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected void BackgroundUpload(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker uploadBackground = sender as BackgroundWorker;

            uploadBackground.ReportProgress(50);
            if (ftpObject.UploadFile(localFolderPath, serverFolderPath + serverFileName, BASIC_MACROS.SEVERITY_HIGH))
                fileUploaded = true;
            else
                fileUploaded = false;


            uploadBackground.ReportProgress(75);

            uploadBackground.ReportProgress(100);
        }

        protected void BackgroundDownload(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker downloadBackground = sender as BackgroundWorker;

            downloadBackground.ReportProgress(50);
            if (!ftpObject.DownloadFile(serverFolderPath + "/" + serverFileName, localFolderPath + "/" + localFileName, BASIC_MACROS.SEVERITY_HIGH))
            {
                fileDownloaded = false;
                return;
            }
            else
                fileDownloaded = true;
            downloadBackground.ReportProgress(100);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///EXTRA FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void resizeImage(ref Image imgToResize, int width, int height)
        {
            imgToResize.Width = width;
            imgToResize.Height = height;
        }

        private void CheckIfFileAlreadyUploaded(string fileName)
        {

            if (ftpFiles.Count == 0)
                uploadThisFile = true;

            else
            {
                for (int i = 0; i < ftpFiles.Count(); i++)
                {
                    if (ftpFiles[i] == fileName)
                    {
                        var result = MessageBox.Show("This file has already been uploaded, are you sure you want to overwrite?", "FTP Server", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (result == MessageBoxResult.Yes)
                        {
                            uploadThisFile = true;

                            BrushConverter brush = new BrushConverter();
                            overwriteFileGrid = (Grid)wrapPanel.Children[i];
                            Label overwriteFileLabel = (Label)overwriteFileGrid.Children[2];
                            overwriteFileLabel.Content = "Overwriting";
                            overwriteFileLabel.Foreground = (System.Windows.Media.Brush)brush.ConvertFrom("#FFFF00");
                            overwriteFileGrid.Children.Add(progressBar);
                            Grid.SetRow(progressBar, 3);
                        }
                        else
                            uploadThisFile = false;

                        checkFileInServer = true;
                        break;
                    }
                    else
                    {
                        uploadThisFile = true;
                        checkFileInServer = false;
                    }
                }
            }

        }

        private void OnBtnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            rfqBasicInfoPage.rfqProductsPage = rfqProductsPage;
            rfqBasicInfoPage.rfqAdditionalInfoPage = rfqAdditionalInfoPage;
            rfqBasicInfoPage.rfqUploadFilesPage = this;

            NavigationService.Navigate(rfqBasicInfoPage);
        }

        private void OnBtnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            rfqProductsPage.rfqUploadFilesPage = this;
            rfqProductsPage.rfqBasicInfoPage = rfqBasicInfoPage;
            rfqProductsPage.rfqAdditionalInfoPage = rfqAdditionalInfoPage;

            NavigationService.Navigate(rfqProductsPage);
        }

        private void OnBtnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            rfqAdditionalInfoPage.rfqBasicInfoPage = rfqBasicInfoPage;
            rfqAdditionalInfoPage.rfqProductsPage = rfqProductsPage;
            rfqAdditionalInfoPage.rfqUploadFilesPage= this;

            NavigationService.Navigate(rfqAdditionalInfoPage);
        }
    }
}
