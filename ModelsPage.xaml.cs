using _01electronics_library;
using _01electronics_windows_library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for BorriCommercialUPSPage.xaml
    /// </summary>
    public partial class ModelsPage : Page
    {
        private Employee loggedInUser;
        protected CommonQueries commonQueries;

        protected List<COMPANY_WORK_MACROS.MODEL_STRUCT> brandModels;
        private Product selectedProduct;

        protected BackgroundWorker downloadBackground;

        Frame frame;
        protected FTPServer ftpServer;
        protected List<String> modelsNames;
        protected String returnMessage;
        protected bool fromServer;
        private Expander currentExpander;
        private Expander previousExpander;
        public ModelsPage(ref Employee mLoggedInUser, ref Product mSelectedProduct)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;
            selectedProduct = mSelectedProduct;
            ftpServer = new FTPServer();
            modelsNames = new List<String>();

            commonQueries = new CommonQueries();
            brandModels = new List<COMPANY_WORK_MACROS.MODEL_STRUCT>();

            downloadBackground = new BackgroundWorker();
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/01Electronics_ERP/" + selectedProduct.GetProductID() + "/" + selectedProduct.GetBrandID());
            downloadBackground.DoWork += BackgroundDownload;
            downloadBackground.ProgressChanged += OnDownloadProgressChanged;
            downloadBackground.RunWorkerCompleted += OnDownloadBackgroundComplete;
            downloadBackground.WorkerReportsProgress = true;

            //  MainWindow mainWindow = new MainWindow();
            //mainWindow.Navigated += OnNavigatingEventHandler;
            //new WebBrowserNavigatingEventHandler(webBrowser1_Navigating);

            downloadProgressBar.Visibility = Visibility.Visible;
            downloadBackground.RunWorkerAsync();
        }
        ~ModelsPage()
        {
            //DeletePhotosDestructor();
        }
        private void QueryGetModels()
        {
            COMPANY_WORK_MACROS.PRODUCT_STRUCT product = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
            product.typeId = selectedProduct.GetProductID();
            product.typeName = selectedProduct.GetProductName();

            COMPANY_WORK_MACROS.BRAND_STRUCT brand = new COMPANY_WORK_MACROS.BRAND_STRUCT();
            brand.brandId = selectedProduct.GetBrandID();
            brand.brandName = selectedProduct.GetBrandName();

            if (!commonQueries.GetCompanyModels(product, brand, ref brandModels))
                return;
        }

        public void SetUpPageUIElements()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                Label productTitleLabel = new Label();
                productTitleLabel.Content = selectedProduct.GetBrandName() + " " + selectedProduct.GetProductName();
                productTitleLabel.VerticalAlignment = VerticalAlignment.Stretch;
                productTitleLabel.HorizontalAlignment = HorizontalAlignment.Stretch;
                productTitleLabel.Margin = new Thickness(48, 24, 48, 24);
                productTitleLabel.Style = (Style)FindResource("primaryHeaderTextStyle");
                ModelsGrid.Children.Add(productTitleLabel);
                Grid.SetRow(productTitleLabel, 0);

                ftpServer.ListFilesInFolder(selectedProduct.GetModelFolderServerPath(), ref modelsNames, ref returnMessage);

                if (modelsNames.Count() == 0)
                {
                    string[] filesNames = Directory.GetFiles(selectedProduct.GetModelFolderLocalPath());
                    foreach (string file in filesNames)
                    {
                        modelsNames.Add(file);
                    }
                    //ftpServer.ListFilesInFolder(selectedProduct.GetFolderLocalPath(), ref modelsNames, ref returnMessage); 
                }
                for (int i = 0; i < brandModels.Count(); i++)
                {

                    if (brandModels[i].modelId != 0)
                    {
                        Grid currentModelGrid = new Grid();
                        currentModelGrid.Margin = new Thickness(55, 24, 24, 24);

                        ColumnDefinition column1 = new ColumnDefinition();
                        ColumnDefinition column2 = new ColumnDefinition();

                        currentModelGrid.ColumnDefinitions.Add(column1);
                        currentModelGrid.ColumnDefinitions.Add(column2);

                        Grid column1Grid = new Grid();

                        Border imageBorder = new Border();
                        imageBorder.Width = 200;
                        imageBorder.Height = 230;
                        imageBorder.BorderThickness = new Thickness(3);
                        imageBorder.Background = Brushes.White;
                        imageBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                        column1Grid.Children.Add(imageBorder);

                        selectedProduct.SetModelID(brandModels[i].modelId);


                        if (modelsNames.Exists(modelName => modelName == selectedProduct.GetPhotoLocalPath() || modelsNames.Exists(modelName2 => modelName2 == (selectedProduct.GetModelID() + ".jpg"))))
                        {
                            try
                            {
                                Image brandImage = new Image();
                                BitmapImage src = new BitmapImage();
                                src.BeginInit();
                                src.UriSource = new Uri(selectedProduct.GetPhotoLocalPath(), UriKind.Relative);
                                src.CacheOption = BitmapCacheOption.OnLoad;
                                src.EndInit();
                                brandImage.Source = src;
                                brandImage.Height = 220;
                                brandImage.Width = 190;
                                //brandImage.MouseDown += ImageMouseDown;
                                brandImage.Tag = brandModels[i].modelId.ToString();
                                column1Grid.Children.Add(brandImage);
                            }
                            catch
                            {
                                if (selectedProduct.DownloadPhotoFromServer())
                                {
                                    Image brandImage = new Image();
                                    BitmapImage src = new BitmapImage();
                                    src.BeginInit();
                                    src.UriSource = new Uri(selectedProduct.GetPhotoLocalPath(), UriKind.Relative);
                                    src.CacheOption = BitmapCacheOption.OnLoad;
                                    src.EndInit();
                                    brandImage.Source = src;
                                    brandImage.Height = 220;
                                    brandImage.Width = 190;
                                    //brandImage.MouseDown += ImageMouseDown;
                                    brandImage.Tag = brandModels[i].modelId.ToString();
                                    column1Grid.Children.Add(brandImage);
                                }

                            }
                        }

                        Grid.SetColumn(column1Grid, 0);

                        currentModelGrid.Children.Add(column1Grid);

                        Grid column2Grid = new Grid();

                        RowDefinition row1 = new RowDefinition();
                        row1.Height = new GridLength(30);

                        RowDefinition row2 = new RowDefinition();
                        row2.Height = new GridLength(200);

                        column2Grid.RowDefinitions.Add(row1);
                        column2Grid.RowDefinitions.Add(row2);

                        ColumnDefinition headerColumn = new ColumnDefinition();
                        ColumnDefinition expanderColumn = new ColumnDefinition();
                        expanderColumn.Width = new GridLength(150);
                        column2Grid.ColumnDefinitions.Add(headerColumn);
                        column2Grid.ColumnDefinitions.Add(expanderColumn);

                        Grid row1Grid = new Grid();

                        //ColumnDefinition headerColumn = new ColumnDefinition();
                        //ColumnDefinition expanderColumn = new ColumnDefinition();
                        //expanderColumn.Width = new GridLength(50);
                        //
                        //row1Grid.ColumnDefinitions.Add(headerColumn);
                        //row1Grid.ColumnDefinitions.Add(expanderColumn);

                        row1Grid.Background = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                        TextBox modelTextBox = new TextBox();
                        modelTextBox.VerticalAlignment = VerticalAlignment.Center;
                        modelTextBox.HorizontalAlignment = HorizontalAlignment.Center;
                        modelTextBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                        modelTextBox.VerticalContentAlignment = VerticalAlignment.Center;
                        modelTextBox.Foreground = Brushes.White;
                        modelTextBox.Background = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                        modelTextBox.FontWeight = FontWeights.DemiBold;
                        modelTextBox.IsReadOnly = true;
                        modelTextBox.BorderThickness = new Thickness(0);
                        modelTextBox.FontSize = 16;
                        modelTextBox.Height = 30;
                        modelTextBox.Text = " " + brandModels[i].modelName;
                        modelTextBox.TextWrapping = TextWrapping.Wrap;
                        Grid.SetRow(modelTextBox, 0);
                        Grid.SetColumn(modelTextBox, 0);
                        //Grid.SetColumnSpan(row1Grid, 2);



                        Expander expander = new Expander();
                        expander.Tag = i;
                        expander.ExpandDirection = ExpandDirection.Down;
                        //expander.VerticalAlignment = VerticalAlignment.Top;
                        //expander.HorizontalAlignment = HorizontalAlignment.Right;
                        expander.VerticalAlignment = VerticalAlignment.Stretch;
                        expander.HorizontalAlignment = HorizontalAlignment.Left;
                        expander.HorizontalContentAlignment = HorizontalAlignment.Center;

                        expander.Expanded += new RoutedEventHandler(OnExpandExpander);
                        expander.Collapsed += new RoutedEventHandler(OnCollapseExpander);
                        expander.Margin = new Thickness(10,0,0,0);

                        StackPanel expanderStackPanel = new StackPanel();
                        expanderStackPanel.Orientation = Orientation.Vertical;
                        //expanderStackPanel.MinHeight = 50;
                        //expanderStackPanel.MinWidth= 50;


                        BrushConverter brushConverter = new BrushConverter();

                        Button ViewButton = new Button();
                        ViewButton.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");
                        ViewButton.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                        // EditButton.Click += OnBtnClickEditProduct;
                        ViewButton.Content = "View";

                        Button EditButton = new Button();
                        EditButton.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");
                        EditButton.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                        // EditButton.Click += OnBtnClickEditProduct;
                        EditButton.Content = "Download Catalog";


                        expanderStackPanel.Children.Add(ViewButton);
                        expanderStackPanel.Children.Add(EditButton);

                        expander.Content = expanderStackPanel;

                        Grid.SetRow(expander, 0);
                        Grid.SetRowSpan(expander, 2);
                        Grid.SetColumn(expander, 1);
                        Grid.SetColumnSpan(expander, 2);

                        row1Grid.Children.Add(modelTextBox);
                        column2Grid.Children.Add(expander);


                        column2Grid.Children.Add(row1Grid);

                        Grid row2Grid = new Grid();
                        row2Grid.Background = Brushes.White;
                        Grid.SetColumnSpan(row2Grid, 2);

                        ScrollViewer scrollViewer2 = new ScrollViewer();
                        scrollViewer2.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                        scrollViewer2.VerticalAlignment = VerticalAlignment.Stretch;
                        scrollViewer2.Content = row2Grid;
                        Grid.SetRow(scrollViewer2, 1);
                        //Grid.SetColumnSpan(scrollViewer2, 2);

                        if (!selectedProduct.InitializeModelSummaryPoints(selectedProduct.GetProductID(), selectedProduct.GetBrandID(), selectedProduct.GetModelID()))
                            return;

                        for (int j = 0; j < 4; j++)
                        {
                            if (j < selectedProduct.modelSummaryPoints.Count())
                            {
                                RowDefinition summaryRow = new RowDefinition();
                                row2Grid.RowDefinitions.Add(summaryRow);

                                Grid textBoxGrid = new Grid();

                                TextBox pointsBox = new TextBox();
                                pointsBox.BorderThickness = new Thickness(0);
                                pointsBox.IsEnabled = false;
                                pointsBox.FontWeight = FontWeights.Bold;
                                pointsBox.Background = Brushes.White;
                                pointsBox.Text = "-" + selectedProduct.modelSummaryPoints[j];
                                pointsBox.TextWrapping = TextWrapping.Wrap;
                                pointsBox.Style = (Style)FindResource("miniTextBoxStyle");
                                Grid.SetRow(pointsBox, j);

                                textBoxGrid.Children.Add(pointsBox);

                                Grid.SetRow(textBoxGrid, j);
                                row2Grid.Children.Add(textBoxGrid);
                            }
                        }
                        if (selectedProduct.modelSummaryPoints.Count() == 0)
                        {
                            RowDefinition summaryRow = new RowDefinition();
                            row2Grid.RowDefinitions.Add(summaryRow);

                            Grid textBoxGrid = new Grid();

                            TextBox pointsBox = new TextBox();
                            pointsBox.BorderThickness = new Thickness(0);
                            pointsBox.IsEnabled = false;
                            pointsBox.FontWeight = FontWeights.Bold;
                            pointsBox.Background = Brushes.White;
                            pointsBox.Text = "";
                            pointsBox.TextWrapping = TextWrapping.Wrap;
                            pointsBox.Style = (Style)FindResource("miniTextBoxStyle");
                            Grid.SetRow(pointsBox, 0);

                            textBoxGrid.Children.Add(pointsBox);

                            Grid.SetRow(textBoxGrid, 0);
                            row2Grid.Children.Add(textBoxGrid);
                        }

                        Grid.SetRow(row2Grid, 1);

                        column2Grid.Children.Add(scrollViewer2);
                        Grid.SetColumn(column2Grid, 1);

                        currentModelGrid.Children.Add(column2Grid);
                        modelsWrapPanel.Children.Add(currentModelGrid);

                    }
                }
                if (brandModels.Count() == 0 || brandModels[0].modelId == 0)
                {
                    Image brandImage = new Image();
                    BitmapImage src = new BitmapImage();
                    src.BeginInit();
                    src.UriSource = new Uri("..\\..\\Photos\\models\\" + "00.jpg", UriKind.Relative);
                    src.CacheOption = BitmapCacheOption.OnLoad;
                    src.EndInit();
                    brandImage.Source = src;
                    brandImage.VerticalAlignment = VerticalAlignment.Center;
                    brandImage.Margin = new Thickness(100);
                    brandImage.HorizontalAlignment = HorizontalAlignment.Center;
                    brandImage.Tag = 00;
                    modelsWrapPanel.Children.Add(brandImage);
                }
                Grid.SetRow(modelsWrapPanel, 1);
            });

        }
        //private void //DeletePhotos()
        //{
        //    ModelsGrid.Children.Clear();
        //    for (int i = 0; i < brandModels.Count(); i++)
        //    {
        //        selectedProduct.SetModelID(brandModels[i].modelId);
        //        selectedProduct.GetNewModelPhotoLocalPath();
        //        selectedProduct.GetNewPhotoServerPath();

        //        try
        //        {
        //            System.Drawing.Image image2 = System.Drawing.Image.FromFile(selectedProduct.GetPhotoLocalPath());
        //            image2.Dispose();
        //            File.Delete(selectedProduct.GetPhotoLocalPath());

        //        }
        //        catch
        //        {

        //        }

        //    }
        //}
        //private void //DeletePhotosDestructor()
        //{
        //    for (int i = 0; i < brandModels.Count(); i++)
        //    {
        //        selectedProduct.SetModelID(brandModels[i].modelId);
        //        selectedProduct.GetNewModelPhotoLocalPath();
        //        selectedProduct.GetNewPhotoServerPath();

        //        try
        //        {
        //            System.Drawing.Image image2 = System.Drawing.Image.FromFile(selectedProduct.GetPhotoLocalPath());
        //            image2.Dispose();
        //            File.Delete(selectedProduct.GetPhotoLocalPath());

        //        }
        //        catch
        //        {

        //        }

        //    }
        //}
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //EXTERNAL TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnButtonClickedMyProfile(object sender, RoutedEventArgs e)
        {
            //DeletePhotos();
            StatisticsPage userPortal = new StatisticsPage(ref loggedInUser);
            this.NavigationService.Navigate(userPortal);
            //UserPortalPage userPortal = new UserPortalPage(ref loggedInUser);
            //this.NavigationService.Navigate(userPortal);
        }
        private void OnButtonClickedContacts(object sender, RoutedEventArgs e)
        {
            //DeletePhotos();
            ContactsPage contacts = new ContactsPage(ref loggedInUser);
            this.NavigationService.Navigate(contacts);
        }
        private void OnButtonClickedProducts(object sender, MouseButtonEventArgs e)
        {
            //DeletePhotos();
            CategoriesPage productsPage = new CategoriesPage(ref loggedInUser);
            this.NavigationService.Navigate(productsPage);
        }
        private void OnButtonClickedWorkOrders(object sender, MouseButtonEventArgs e)
        {
            //DeletePhotos();
            WorkOrdersPage workOrdersPage = new WorkOrdersPage(ref loggedInUser);
            this.NavigationService.Navigate(workOrdersPage);
        }
        private void OnButtonClickedWorkOffers(object sender, RoutedEventArgs e)
        {
            ////DeletePhotos();
            QuotationsPage workOffers = new QuotationsPage(ref loggedInUser);
            this.NavigationService.Navigate(workOffers);
        }
        private void OnButtonClickedRFQs(object sender, RoutedEventArgs e)
        {
            //DeletePhotos();
            RFQsPage rFQsPage = new RFQsPage(ref loggedInUser);
            this.NavigationService.Navigate(rFQsPage);
        }
        private void OnButtonClickedVisits(object sender, RoutedEventArgs e)
        {
            //DeletePhotos();
            ClientVisitsPage clientVisitsPage = new ClientVisitsPage(ref loggedInUser);
            this.NavigationService.Navigate(clientVisitsPage);
        }
        private void OnButtonClickedCalls(object sender, RoutedEventArgs e)
        {
            //DeletePhotos();
            ClientCallsPage clientCallsPage = new ClientCallsPage(ref loggedInUser);
            this.NavigationService.Navigate(clientCallsPage);
        }
        private void OnButtonClickedMeetings(object sender, RoutedEventArgs e)
        {
            //DeletePhotos();
            OfficeMeetingsPage officeMeetingsPage = new OfficeMeetingsPage(ref loggedInUser);
            this.NavigationService.Navigate(officeMeetingsPage);
        }
        private void OnButtonClickedStatistics(object sender, RoutedEventArgs e)
        {

        }
        private void OnButtonClickedMaintenanceOffer(object sender, MouseButtonEventArgs e)
        {
            MaintenanceOffersPage maintenanceOffersPage = new MaintenanceOffersPage(ref loggedInUser);
            this.NavigationService.Navigate(maintenanceOffersPage);
        }
        private void OnButtonClickedMaintenanceContracts(object sender, MouseButtonEventArgs e)
        {
            MaintenanceContractsPage maintenanceContractsPage = new MaintenanceContractsPage(ref loggedInUser);
            this.NavigationService.Navigate(maintenanceContractsPage);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //MOUSE DOWN HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void UPSImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            String tmp = currentImage.Tag.ToString();
            String Name = currentImage.Name.ToString();

            Product selectedProduct = new Product();
            selectedProduct.SetBrandID(int.Parse(tmp));
            selectedProduct.SetBrandName(Name);

            BrandsPage productsPage = new BrandsPage(ref loggedInUser, ref selectedProduct);
            this.NavigationService.Navigate(productsPage);
        }
        private void ImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            String tmp = currentImage.Tag.ToString();

            Product selectedProduct = new Product();
            selectedProduct.SetProductID(int.Parse(tmp));

            BrandsPage productsPage = new BrandsPage(ref loggedInUser, ref selectedProduct);
            this.NavigationService.Navigate(productsPage);
        }
        private void OnNavigatingEventHandler(object sender, NavigationWindow e)
        {
        }
        protected void BackgroundDownload(object sender, DoWorkEventArgs e)
        {
            QueryGetModels();

            downloadBackground.ReportProgress(50);

            SetUpPageUIElements();

            downloadBackground.ReportProgress(100);
        }

        protected void OnDownloadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            downloadProgressBar.Value = e.ProgressPercentage;
        }

        protected void OnDownloadBackgroundComplete(object sender, RunWorkerCompletedEventArgs e)
        {

            scrollViewer.Visibility = Visibility.Visible;
            downloadProgressBar.Visibility = Visibility.Collapsed;

        }

        private void OnButtonClickedProjects(object sender, MouseButtonEventArgs e)
        {

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Expander HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnExpandExpander(object sender, RoutedEventArgs e)
        {
            if (currentExpander != null)
                previousExpander = currentExpander;

            currentExpander = (Expander)sender;

            if (previousExpander != currentExpander && previousExpander != null)
                previousExpander.IsExpanded = false;

            Grid currentGrid = (Grid)currentExpander.Parent;

            currentExpander.VerticalAlignment = VerticalAlignment.Top;
        }

        private void OnCollapseExpander(object sender, RoutedEventArgs e)
        {
            Expander currentExpander = (Expander)sender;
            Grid currentGrid = (Grid)currentExpander.Parent;
            currentExpander.VerticalAlignment = VerticalAlignment.Top;
            currentExpander.Margin = new Thickness(12);
        }



    }
}
