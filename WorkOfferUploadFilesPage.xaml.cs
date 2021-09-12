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

        //protected var doneEvent = new AutoResetEvent(false);
        protected BackgroundWorker uploadBackground;
        protected BackgroundWorker downloadBackground;

        string[] files = new string[10];

        protected String serverFolderPath;
        protected String serverFileName;

        protected String localFolderPath;
        protected String localFileName;

        protected bool fileUploaded;
        protected bool fileDownloaded;
        protected bool uploadComplete = true;


        WrapPanel wrapPanel = new WrapPanel();

        private Grid previousSelectedFile;
        private Grid currentSelectedFile;
        private Label currentLabel;

        List<string> ftpFiles;

        public WorkOfferBasicInfoPage workOfferBasicInfoPage;
        public WorkOfferProductsPage workOfferProductsPage;
        public WorkOfferPaymentAndDeliveryPage workOfferPaymentAndDeliveryPage;
        public WorkOfferAdditionalInfoPage workOfferAdditionalInfoPage;

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
                uploadFilesStackPanel.Children.Add(wrapPanel);

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
                    //UploadIconGrid.IsMouseDirectlyOverChanged += OnMouseOverIcons;
                    Image icon = new Image();

                    //PLEASE PUT THE ICONS IN THE ICONS FOLDER I CREATED IN THE PROJECT FOLDER AND ADD THEM TO PROJECT IN VISUAL STUDIO

                    if (ftpFiles[i].Contains(".pdf"))
                        icon = new Image { Source = new BitmapImage(new Uri("C:/Users/developer/Pictures/Saved Pictures/PDFIcon.jpg")) };
                    else
                        icon = new Image { Source = new BitmapImage(new Uri("C:/Users/developer/Pictures/Saved Pictures/WordIcon.jpg")) };

                    resizeImage(ref icon, 50, 50);
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
            else
            {
                Image icon = new Image();

                icon = new Image { Source = new BitmapImage(new Uri("C:/Users/developer/Pictures/Saved Pictures/drop-files-here-extra.jpg")) };
                icon.Tag = "base icon";
                icon.HorizontalAlignment = HorizontalAlignment.Center;
                icon.VerticalAlignment = VerticalAlignment.Center;
                resizeImage(ref icon, 250, 250);

                uploadFilesStackPanel.Children.Add(icon);
            }
        }

        public WorkOfferUploadFilesPage(ref WorkOffer mWorkOffer)
        {
            workOffer = mWorkOffer;
        }

        public void resizeImage(ref Image imgToResize, int width, int height)
        {
            imgToResize.Width = width;
            imgToResize.Height = height;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            workOfferBasicInfoPage.workOfferProductsPage = workOfferProductsPage;
            workOfferBasicInfoPage.workOfferPaymentAndDeliveryPage = workOfferPaymentAndDeliveryPage;
            workOfferBasicInfoPage.workOfferAdditionalInfoPage = workOfferAdditionalInfoPage;
            workOfferBasicInfoPage.workOfferUploadFilesPage = this;

            NavigationService.Navigate(workOfferBasicInfoPage);
        }
        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            workOfferProductsPage.workOfferBasicInfoPage = workOfferBasicInfoPage;
            workOfferProductsPage.workOfferPaymentAndDeliveryPage = workOfferPaymentAndDeliveryPage;
            workOfferProductsPage.workOfferAdditionalInfoPage = workOfferAdditionalInfoPage;
            workOfferProductsPage.workOfferUploadFilesPage = this;

            NavigationService.Navigate(workOfferProductsPage);
        }
        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {
            workOfferPaymentAndDeliveryPage.workOfferBasicInfoPage = workOfferBasicInfoPage;
            workOfferPaymentAndDeliveryPage.workOfferProductsPage = workOfferProductsPage;
            workOfferPaymentAndDeliveryPage.workOfferAdditionalInfoPage = workOfferAdditionalInfoPage;
            workOfferPaymentAndDeliveryPage.workOfferUploadFilesPage = this;

            NavigationService.Navigate(workOfferPaymentAndDeliveryPage);
        }
        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            workOffer.SetNoOfSavedOfferProducts();

            workOfferAdditionalInfoPage.workOfferBasicInfoPage = workOfferBasicInfoPage;
            workOfferAdditionalInfoPage.workOfferProductsPage = workOfferProductsPage;
            workOfferAdditionalInfoPage.workOfferPaymentAndDeliveryPage = workOfferPaymentAndDeliveryPage;
            workOfferAdditionalInfoPage.workOfferUploadFilesPage = this;

            NavigationService.Navigate(workOfferAdditionalInfoPage);

        }
        private void OnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///BUTTON CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void OnButtonClickAutomateWorkOffer(object sender, RoutedEventArgs e)
        {

        }
        //private void OnMouseOverIcons(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    previousSelectedFile = currentSelectedFile;
        //    currentSelectedFile = (Grid)sender;
        //    currentLabel = (Label)currentSelectedFile.Children[1];
        //    BrushConverter brush = new BrushConverter();

        //    if (previousSelectedFile != null)
        //    {
        //        previousSelectedFile.Background = (Brush)brush.ConvertFrom("#FFFFFF");
        //        Label previousLabel = (Label)previousSelectedFile.Children[1];
        //        previousLabel.Foreground = (Brush)brush.ConvertFrom("#000000");
        //    }

        //    currentSelectedFile.Background = (Brush)brush.ConvertFrom("#105A97");
        //    currentLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");

        //    //if(viewAddCondition == COMPANY_WORK_MACROS.OFFER_VIEW_CONDITION)
        //    //{
        //    //    downloadButton.Visibility = Visibility.Visible;
        //    //}
        //}

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

                downloadBackground.RunWorkerAsync();
            }
            //if(viewAddCondition == COMPANY_WORK_MACROS.OFFER_VIEW_CONDITION)
            //{
            //    downloadButton.Visibility = Visibility.Visible;
            //}
        }
        private void OnButtonClickOk(object sender, RoutedEventArgs e)
        { 
            //if (viewAddCondition == COMPANY_WORK_MACROS.OFFER_ADD_CONDITION || viewAddCondition == COMPANY_WORK_MACROS.OFFER_RESOLVE_CONDITION)
            //{
            //    workOffer.SetDrawingSubmissionDeadlineMinimum(drawingDeadlineFrom);
            //    workOffer.SetDrawingSubmissionDeadlineMaximum(drawingDeadlineTo);
            //    workOffer.SetWarrantyPeriod(warrantyPeriod);
            //    workOffer.SetOfferValidityPeriod(offerValidityPeriod);
            //    workOffer.SetOfferNotes(additionalDescription);
            //
            //    if (workOffer.GetSalesPersonId() == 0)
            //        MessageBox.Show("You need to choose sales person before adding a work offer!");
            //    else if (workOffer.GetCompanyName() == null)
            //        MessageBox.Show("You need to choose a company before adding a work offer!");
            //    else if (workOffer.GetAddressSerial() == 0)
            //        MessageBox.Show("You need to choose company address before adding a work offer!");
            //    else if (workOffer.GetContactId() == 0)
            //        MessageBox.Show("You need to choose a contact before adding a work offer!");
            //    else if (workOffer.GetOfferProduct1TypeId() != 0 && workOffer.GetProduct1PriceValue() == 0)
            //        MessageBox.Show("You need to add a price for product 1 before adding a work offer!");
            //    else if (workOffer.GetOfferProduct2TypeId() != 0 && workOffer.GetProduct2PriceValue() == 0)
            //        MessageBox.Show("You need to add a price for product 2 before adding a work offer!");
            //    else if (workOffer.GetOfferProduct3TypeId() != 0 && workOffer.GetProduct3PriceValue() == 0)
            //        MessageBox.Show("You need to add a price for product 3 before adding a work offer!");
            //    else if (workOffer.GetOfferProduct4TypeId() != 0 && workOffer.GetProduct4PriceValue() == 0)
            //        MessageBox.Show("You need to add a price for product 4 before adding a work offer!");
            //    else if (workOffer.GetPercentDownPayment() + workOffer.GetPercentOnDelivery() + workOffer.GetPercentOnInstallation() < 100)
            //        MessageBox.Show("Down payement, on delivery and on installation percentages total is less than 100%!!");
            //    else if (workOffer.GetDeliveryTimeMinimum() == 0 || workOffer.GetDeliveryTimeMaximum() == 0)
            //        MessageBox.Show("You need to set delivery time min and max before adding a work offer!");
            //    else if (workOffer.GetDeliveryPointId() == 0)
            //        MessageBox.Show("You need to set delivery point before adding a work offer!");
            //    else if (workOffer.GetOfferContractTypeId() == 0)
            //        MessageBox.Show("You need to set contract type before adding a work offer!");
            //    else if (workOffer.GetWarrantyPeriod() == 0 || workOffer.GetWarrantyPeriodTimeUnitId() == 0)
            //        MessageBox.Show("You need to set warranty period before adding a work offer!");
            //    else if (workOffer.GetOfferValidityPeriod() == 0 || workOffer.GetOfferValidityTimeUnitId() == 0)
            //        MessageBox.Show("You need to set validity period before adding a work offer!");
            //    else
            //    {
            //        if (workOffer.IssueNewOffer())
            //            MessageBox.Show("WorkOffer added succefully!");
            //
            //        //WorkOfferWindow workOfferWindow = new WorkOfferWindow(ref loggedInUser, ref workOffer, viewAddCondition);
            //
            //        NavigationWindow currentWindow = (NavigationWindow)this.Parent;
            //        currentWindow.Close();
            //    }
            //}
            //if (viewAddCondition == COMPANY_WORK_MACROS.OFFER_REVISE_CONDITION)
            //{
            //    workOffer.SetDrawingSubmissionDeadlineMinimum(drawingDeadlineFrom);
            //    workOffer.SetDrawingSubmissionDeadlineMaximum(drawingDeadlineTo);
            //    workOffer.SetWarrantyPeriod(warrantyPeriod);
            //    workOffer.SetOfferValidityPeriod(offerValidityPeriod);
            //    workOffer.SetOfferNotes(additionalDescription);
            //
            //    if (workOffer.GetSalesPersonId() == 0)
            //        MessageBox.Show("You need to choose sales person before adding a work offer!");
            //    else if (workOffer.GetCompanyName() == null)
            //        MessageBox.Show("You need to choose a company before adding a work offer!");
            //    else if (workOffer.GetAddressSerial() == 0)
            //        MessageBox.Show("You need to choose company address before adding a work offer!");
            //    else if (workOffer.GetContactId() == 0)
            //        MessageBox.Show("You need to choose a contact before adding a work offer!");
            //    else if (workOffer.GetOfferProduct1TypeId() != 0 && workOffer.GetProduct1PriceValue() == 0)
            //        MessageBox.Show("You need to add a price for product 1 before adding a work offer!");
            //    else if (workOffer.GetOfferProduct2TypeId() != 0 && workOffer.GetProduct2PriceValue() == 0)
            //        MessageBox.Show("You need to add a price for product 2 before adding a work offer!");
            //    else if (workOffer.GetOfferProduct3TypeId() != 0 && workOffer.GetProduct3PriceValue() == 0)
            //        MessageBox.Show("You need to add a price for product 3 before adding a work offer!");
            //    else if (workOffer.GetOfferProduct4TypeId() != 0 && workOffer.GetProduct4PriceValue() == 0)
            //        MessageBox.Show("You need to add a price for product 4 before adding a work offer!");
            //    else if (workOffer.GetPercentDownPayment() + workOffer.GetPercentOnDelivery() + workOffer.GetPercentOnInstallation() < 100)
            //        MessageBox.Show("Down payement, on delivery and on installation percentages total is less than 100%!!");
            //    else if (workOffer.GetDeliveryTimeMinimum() == 0 || workOffer.GetDeliveryTimeMaximum() == 0)
            //        MessageBox.Show("You need to set delivery time min and max before adding a work offer!");
            //    else if (workOffer.GetDeliveryPointId() == 0)
            //        MessageBox.Show("You need to set delivery point before adding a work offer!");
            //    else if (workOffer.GetOfferContractTypeId() == 0)
            //        MessageBox.Show("You need to set contract type before adding a work offer!");
            //    else if (workOffer.GetWarrantyPeriod() == 0 || workOffer.GetWarrantyPeriodTimeUnitId() == 0)
            //        MessageBox.Show("You need to set warranty period before adding a work offer!");
            //    else if (workOffer.GetOfferValidityPeriod() == 0 || workOffer.GetOfferValidityTimeUnitId() == 0)
            //        MessageBox.Show("You need to set validity period before adding a work offer!");
            //
            //
            //    else
            //    {
            //        if (workOffer.ReviseOffer())
            //            MessageBox.Show("Offer Revised successfully!");
            //
            //        // WorkOfferWindow workOfferWindow = new WorkOfferWindow(ref loggedInUser, ref workOffer, viewAddCondition);
            //
            //        NavigationWindow currentWindow = (NavigationWindow)this.Parent;
            //        currentWindow.Close();
            //    }
            //
            //}
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///ON DROP HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnDropUploadFilesStackPanel(object sender, DragEventArgs e)
        {
            if(ftpFiles.Count == 0)
            {
                uploadFilesStackPanel.Children.Clear();
                uploadFilesStackPanel.Children.Add(wrapPanel);
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] temp = (string[])e.Data.GetData(DataFormats.FileDrop);
                int tempCounter = 0;
                e.Effects = DragDropEffects.All;

                for (int i = 0; i < temp.Count(); i++)
                {
                    //files[numberOfFilesAdded] = (string)e.Data.GetData(DataFormats.FileDrop);
                    //e.Effects = DragDropEffects.Copy;
                    if (uploadComplete == true)
                    {
                        uploadComplete = false;
                        files[counter] = temp[i];

                        localFolderPath = temp[i];
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

                        resizeImage(ref icon, 50, 50);
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
                        //const result = await firstFunction()
                        uploadBackground.RunWorkerAsync();
                        while (uploadBackground.IsBusy)
                        {
                            System.Windows.Forms.Application.DoEvents();
                        }
                    }
                    
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///BACKGROUND COMPLETE HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected void OnUploadBackgroundComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            uploadComplete = true;

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

                resizeImage(ref icon, 50, 50);
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

                resizeImage(ref icon, 50, 50);
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
        protected void OnDownloadBackgroundComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Visibility = Visibility.Collapsed;
            Label currentStatusLabel = (Label)currentSelectedFile.Children[2];
            if (fileDownloaded == true)
                currentStatusLabel.Content = "Downloaded";
            else
                currentStatusLabel.Content = "Failed";
            //downloadButton.Visibility = Visibility.Visible;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///BUTTON CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //private void OnClickDownloadButton(object sender, RoutedEventArgs e)
        //{
        //    //downloadButton.Visibility = Visibility.Collapsed;

        //    System.Windows.Forms.FolderBrowserDialog downloadFile = new System.Windows.Forms.FolderBrowserDialog();

        //    if (downloadFile.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
        //        return;

        //    if (!integrityChecks.CheckFileEditBox(downloadFile.SelectedPath))
        //        return;

        //    serverFileName = currentLabel.Content.ToString();
        //    //serverFolderPath = BASIC_MACROS.OFFER_FILES_PATH + workOffer.GetOfferID() + "/" + serverFileName;
        //    integrityChecks.RemoveExtraSpaces(serverFileName, ref serverFileName);

        //    localFolderPath = downloadFile.SelectedPath;
        //    localFileName = serverFileName;
        //    integrityChecks.RemoveExtraSpaces(localFileName, ref localFileName);

        //    progressBar.Visibility = Visibility.Visible;

        //    downloadBackground.RunWorkerAsync();
        //}

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
            uploadComplete = false;


            uploadBackground.ReportProgress(50);
            if (ftpObject.UploadFile(localFolderPath, serverFolderPath + serverFileName))
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
            if (!ftpObject.DownloadFile(serverFolderPath + "/" + serverFileName, localFolderPath + "/" + localFileName))
            {
                fileDownloaded = false;
                return;
            }
            else
                fileDownloaded = true;
            downloadBackground.ReportProgress(100);
        }
    }
}
