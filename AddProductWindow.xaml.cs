using _01electronics_library;
using _01electronics_windows_library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Button = System.Windows.Controls.Button;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using Label = System.Windows.Controls.Label;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using ProgressBar = System.Windows.Controls.ProgressBar;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for AddProductWindow.xaml
    /// </summary>
    public partial class AddProductWindow : Window
    {

        protected String errorMessage;
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

        protected Product product;

        List<string> ftpFiles;

        ProgressBar progressBar = new ProgressBar();

        
        public AddProductWindow()
        {
            counter = 0;
            InitializeComponent();
            sqlDatabase = new SQLServer();
            ftpObject = new FTPServer();
            integrityChecks = new IntegrityChecks();
            product = new Product();
            // viewAddCondition = mViewAddCondition;
            product.GetNewProductID();

            ftpFiles = new List<string>(); progressBar.Style = (Style)FindResource("ProgressBarStyle");
            progressBar.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
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

               serverFolderPath = product.GetProductFolderServerPath();

            InsertDragAndDropOrBrowseGrid();
            //if (!ftpObject.CheckExistingFolder(serverFolderPath))
            //{
            //    if (!ftpObject.CreateNewFolder(serverFolderPath))
            //    {
            //        InsertErrorRetryButton();
            //        return;
            //    }
            //}
            //else
            //{
            //    ftpFiles.Clear();
            //    if (!ftpObject.ListFilesInFolder(serverFolderPath, ref ftpFiles, ref errorMessage))
            //    {
            //        System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        InsertErrorRetryButton();
            //        return;
            //    }
            //}

            //if (ftpFiles.Count != 0)
            //{
            //    uploadFilesStackPanel.Children.Add(wrapPanel);

            //    for (int i = 0; i < ftpFiles.Count; i++)
            //    {
            //        if (ftpFiles[i] != "." || ftpFiles[i] != "..")
            //            InsertIconGridFromServer(i);
            //    }
            //    InsertAddFilesIcon();
            //}
            //else if (ftpFiles.Count == 0)
            //{
            //    InsertDragAndDropOrBrowseGrid();
            //}
        }

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
                    serverFileName = (String)product.GetProductID().ToString() + ".jpg";

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

        private void InsertIconGrid(string mStatus, string localFolderPath)
        {
            Grid UploadIconGrid = new Grid();
            UploadIconGrid.Margin = new Thickness(24);
            //UploadIconGrid.Width = 250;
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

            LoadIcon(ref icon, serverFolderPath);

            resizeImage(ref icon, 1800  , 600);
            UploadIconGrid.Children.Add(icon);
            Grid.SetRow(icon, 0);

            Label name = new Label();
            name.Content = localFileName;
            name.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
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
            status.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            UploadIconGrid.Children.Add(status);
            Grid.SetRow(status, 2);

            wrapPanel.Children.Add(UploadIconGrid);
        }
        private void LoadIcon(ref Image icon, string ftpFiles)
        {

            Image productImage = new Image();

            try
            {

                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(product.GetProductPhotoLocalPath(), UriKind.Relative);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                productImage.Source = src;
                productImage.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                productImage.VerticalAlignment = VerticalAlignment.Stretch;

                Grid.SetRow(productImage, 0);



                BrushConverter brushConverter = new BrushConverter();



                Button EditButton = new Button();
                EditButton.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");
                EditButton.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                // EditButton.Click += OnBtnClickEditProduct;
                EditButton.Content = "Edit";

            }
            catch
            {
                product.SetPhotoServerPath(product.GetProductFolderServerPath() + "/" + product.GetProductID() + ".jpg");
                if (product.DownloadPhotoFromServer())
                {
                    BitmapImage src = new BitmapImage();
                    src.BeginInit();
                    src.UriSource = new Uri(product.GetProductPhotoLocalPath(), UriKind.Relative);
                    src.CacheOption = BitmapCacheOption.OnLoad;
                    src.EndInit();
                    productImage.Source = src;
                    productImage.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    productImage.VerticalAlignment = VerticalAlignment.Stretch;
                    
                    Grid.SetRow(productImage, 0);
                }
            }

            icon = productImage;

            //if (productImage.Width!=1800 || productImage.Height!= 600)
            //{
            //    MessageBox.Show("Picture Should be 1800px X 600px ", "Error", (MessageBoxButtons)MessageBoxButton.OK, (MessageBoxIcon)MessageBoxImage.Error);
                
            //}
            //else
            //{
            //    icon = productImage;

            //}

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
                        var result = System.Windows.MessageBox.Show("This file has already been uploaded, are you sure you want to overwrite?", "FTP Server", MessageBoxButton.YesNo, MessageBoxImage.Information);
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

                if (!integrityChecks.CheckFileEditBox(downloadFile.SelectedPath, ref errorMessage))
                {
                    System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //serverFileName = currentLabel.Content.ToString();
                serverFileName = (String)product.GetProductID().ToString() + ".jpg";
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
        public void resizeImage(ref Image imgToResize, int width, int height)
        {
            imgToResize.Width = width;
            imgToResize.Height = height;
        }

        private void OnTextChangedTelephone(object sender, TextChangedEventArgs e)
        {

        }

        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {

        }

        private void AdditionalDescriptionTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (additionalDescriptionTextBox.Text.Length <= 150)
            {
               // quotation.SetQuotationAdditionalDescription(additionalDescriptionTextBox.Text.ToString());
                counterLabel.Content = 150 - additionalDescriptionTextBox.Text.Length;
            }
            else
            {
                additionalDescriptionTextBox.Text = additionalDescriptionTextBox.Text.Substring(0, additionalDescriptionTextBox.Text.Length - 1);
                additionalDescriptionTextBox.Select(additionalDescriptionTextBox.Text.Length, 0);
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///BACKGROUND FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected void BackgroundUpload(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker uploadBackground = sender as BackgroundWorker;

            uploadBackground.ReportProgress(50);
            if (ftpObject.UploadFile(localFolderPath, serverFolderPath + serverFileName, BASIC_MACROS.SEVERITY_HIGH, ref errorMessage))
                fileUploaded = true;
            else
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                fileUploaded = false;
            }

            uploadBackground.ReportProgress(75);

            uploadBackground.ReportProgress(100);
        }

        protected void BackgroundDownload(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker downloadBackground = sender as BackgroundWorker;

            downloadBackground.ReportProgress(50);
            if (!ftpObject.DownloadFile(serverFolderPath + "/" + serverFileName, localFolderPath + "/" + localFileName, BASIC_MACROS.SEVERITY_HIGH, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                fileDownloaded = false;
                return;
            }
            else
                fileDownloaded = true;
            downloadBackground.ReportProgress(100);
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
        ///INSERT FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

       
        private void InsertIconGridFromServer(int i)
        {
            Grid UploadIconGrid = new Grid();
            UploadIconGrid.Margin = new Thickness(24);
            //UploadIconGrid.Width = 250;

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

            resizeImage(ref icon, 1800, 600);
            UploadIconGrid.Children.Add(icon);
            Grid.SetRow(icon, 0);

            Label name = new Label();
            name.Content = ftpFiles[i];
            name.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            UploadIconGrid.Children.Add(name);
            Grid.SetRow(name, 1);

            Label status = new Label();
            status.Content = "SUBMITTED";
            status.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            BrushConverter brush = new BrushConverter();
            status.Foreground = (System.Windows.Media.Brush)brush.ConvertFrom("#00FF00");
            UploadIconGrid.Children.Add(status);
            Grid.SetRow(status, 2);

            wrapPanel.Children.Add(UploadIconGrid);
        }
       

        private void InsertDragAndDropOrBrowseGrid()
        {
            Grid grid = new Grid();

            grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            grid.VerticalAlignment = VerticalAlignment.Center;

            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            RowDefinition row3 = new RowDefinition();

            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            grid.RowDefinitions.Add(row3);

            Image icon = new Image();

            icon = new Image { Source = new BitmapImage(new Uri(@"Icons\drop_files_icon.jpg", UriKind.Relative)) };
            icon.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            icon.VerticalAlignment = VerticalAlignment.Center;
            resizeImage(ref icon, 250, 150);
            grid.Children.Add(icon);
            Grid.SetRow(icon, 0);

            Label orLabel = new Label();
            orLabel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
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
            grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();

            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);

            Image icon = new Image();

            icon = new Image { Source = new BitmapImage(new Uri(@"Icons\no_internet_icon.jpg", UriKind.Relative)) };
            icon.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
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
        private void OnClickBrowseButton(object sender, RoutedEventArgs e)
        {
            OpenFileDialog uploadFile = new OpenFileDialog();

            if (uploadFile.ShowDialog() == false)
                return;

            if (!integrityChecks.CheckFileEditBox(uploadFile.FileName, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (ftpFiles.Count == 0)
            {
                uploadFilesStackPanel.Children.Clear();
                uploadFilesStackPanel.Children.Add(wrapPanel);
            }

            localFolderPath = uploadFile.FileName;
            localFileName = System.IO.Path.GetFileName(localFolderPath);

            //serverFileName = localFileName;

            serverFileName = (String)product.GetProductID().ToString() + ".jpg";
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
                if (!fTPServer.ListFilesInFolder(serverFolderPath, ref ftpFiles, ref errorMessage))
                {
                    System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                
            }
            else if (ftpFiles.Count == 0)
            {
                uploadFilesStackPanel.Children.Clear();
                InsertDragAndDropOrBrowseGrid();
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

                if (!integrityChecks.CheckFileEditBox(uploadFile.FileName, ref errorMessage))
                {
                    System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
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

    }
}
