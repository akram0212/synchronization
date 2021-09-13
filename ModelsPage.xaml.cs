﻿using _01electronics_library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        public ModelsPage(ref Employee mLoggedInUser, ref Product mSelectedProduct)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;
            selectedProduct = mSelectedProduct;

            commonQueries = new CommonQueries();
            brandModels = new List<COMPANY_WORK_MACROS.MODEL_STRUCT>();

            downloadBackground = new BackgroundWorker();
            downloadBackground.DoWork += BackgroundDownload;
            downloadBackground.ProgressChanged += OnDownloadProgressChanged;
            downloadBackground.RunWorkerCompleted += OnDownloadBackgroundComplete;
            downloadBackground.WorkerReportsProgress = true;

            //  MainWindow mainWindow = new MainWindow();
            //mainWindow.Navigated += OnNavigatingEventHandler;
            //new WebBrowserNavigatingEventHandler(webBrowser1_Navigating);

            downloadBackground.RunWorkerAsync();

        }
        ~ ModelsPage()
        {
            DeletePhotosDestructor();
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
            Application.Current.Dispatcher.Invoke((Action)delegate {
                Label productTitleLabel = new Label();
                productTitleLabel.Content = selectedProduct.GetBrandName() + " " + selectedProduct.GetProductName();
                productTitleLabel.VerticalAlignment = VerticalAlignment.Stretch;
                productTitleLabel.HorizontalAlignment = HorizontalAlignment.Stretch;
                productTitleLabel.Margin = new Thickness(48, 24, 48, 24);
                productTitleLabel.Style = (Style)FindResource("primaryHeaderTextStyle");
                ModelsGrid.Children.Add(productTitleLabel);
                Grid.SetRow(productTitleLabel, 0);

                //WrapPanel modelsWrapPanel = new WrapPanel();
                //modelsWrapPanel.HorizontalAlignment = HorizontalAlignment.Stretch;

                //ScrollViewer scrollViewer = new ScrollViewer();
                //scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                //scrollViewer.VerticalAlignment = VerticalAlignment.Stretch;
                //scrollViewer.HorizontalAlignment = HorizontalAlignment.Stretch;
                // Grid.SetColumn(scrollViewer, 0);

                //ModelsGrid.Children.Add(scrollViewer);
                //Grid.SetRow(scrollViewer, 1);

                //scrollViewer.Content = modelsWrapPanel;

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
                        //Grid.SetColumn(imageBorder, 0);
                        column1Grid.Children.Add(imageBorder);

                        selectedProduct.SetModelID(brandModels[i].modelId);
                        selectedProduct.GetNewPhotoLocalPath();
                        selectedProduct.GetNewPhotoServerPath();

                        try
                        {
                            Image brandImage = new Image();
                            BitmapImage src = new BitmapImage();
                            src.BeginInit();
                            src.UriSource = new Uri(selectedProduct.GetPhotoLocalPath(), UriKind.Relative);
                            src.CacheOption = BitmapCacheOption.OnLoad;
                            src.EndInit();
                            //string src = String.Format(selectedProduct.GetPhotoLocalPath());
                            // brandImage.Source = new BitmapImage(new Uri(selectedProduct.GetPhotoLocalPath(), UriKind.Absolute));
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
                                //brandImage.Source = new BitmapImage(new Uri(selectedProduct.GetPhotoLocalPath(), UriKind.Relative));
                                brandImage.Height = 220;
                                brandImage.Width = 190;
                                //brandImage.MouseDown += ImageMouseDown;
                                brandImage.Tag = brandModels[i].modelId.ToString();
                                column1Grid.Children.Add(brandImage);
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

                        Grid row1Grid = new Grid();

                        Label modelLabel = new Label();
                        modelLabel.VerticalAlignment = VerticalAlignment.Top;
                        modelLabel.HorizontalAlignment = HorizontalAlignment.Stretch;
                        modelLabel.Content = brandModels[i].modelName;
                        modelLabel.Style = (Style)FindResource("tableSubHeaderItem");
                        Grid.SetRow(modelLabel, 0);
                        Grid.SetColumn(modelLabel, 0);

                        row1Grid.Children.Add(modelLabel);

                        column2Grid.Children.Add(row1Grid);

                        Grid row2Grid = new Grid();
                        row2Grid.Background = Brushes.White;

                        ScrollViewer scrollViewer2 = new ScrollViewer();
                        scrollViewer2.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                        scrollViewer2.VerticalAlignment = VerticalAlignment.Stretch;
                        scrollViewer2.Content = row2Grid;
                        Grid.SetRow(scrollViewer2, 1);

                        if (!selectedProduct.InitializeModelSummaryPoints(selectedProduct.GetProductID(), selectedProduct.GetBrandID(), selectedProduct.GetModelID()))
                            return;

                        for (int j = 0; j < 4; j++)
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

                        Grid.SetRow(row2Grid, 1);

                        column2Grid.Children.Add(scrollViewer2);
                        Grid.SetColumn(column2Grid, 1);

                        currentModelGrid.Children.Add(column2Grid);
                        modelsWrapPanel.Children.Add(currentModelGrid);

                    }
                }
                Grid.SetRow(modelsWrapPanel, 1);
            });
           
        }
        private void DeletePhotos()
        {
            ModelsGrid.Children.Clear();
            for (int i = 0; i < brandModels.Count(); i++)
            {
                selectedProduct.SetModelID(brandModels[i].modelId);
                selectedProduct.GetNewPhotoLocalPath();
                selectedProduct.GetNewPhotoServerPath();

                try
                {
                    System.Drawing.Image image2 = System.Drawing.Image.FromFile(selectedProduct.GetPhotoLocalPath());
                    image2.Dispose();
                    File.Delete(selectedProduct.GetPhotoLocalPath());

                }
                catch
                {

                }

            }
        }
        private void DeletePhotosDestructor()
        {
            for (int i = 0; i < brandModels.Count(); i++)
            {
                selectedProduct.SetModelID(brandModels[i].modelId);
                selectedProduct.GetNewPhotoLocalPath();
                selectedProduct.GetNewPhotoServerPath();

                try
                {
                    System.Drawing.Image image2 = System.Drawing.Image.FromFile(selectedProduct.GetPhotoLocalPath());
                    image2.Dispose();
                    File.Delete(selectedProduct.GetPhotoLocalPath());

                }
                catch
                {

                }

            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //EXTERNAL TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnButtonClickedMyProfile(object sender, RoutedEventArgs e)
        {
            DeletePhotos();
            UserPortalPage userPortal = new UserPortalPage(ref loggedInUser);
            this.NavigationService.Navigate(userPortal);
        }
        private void OnButtonClickedContacts(object sender, RoutedEventArgs e)
        {
            DeletePhotos();
            ContactsPage contacts = new ContactsPage(ref loggedInUser);
            this.NavigationService.Navigate(contacts);
        }
        private void OnButtonClickedProducts(object sender, MouseButtonEventArgs e)
        {
            DeletePhotos();
            ProductsPage productsPage = new ProductsPage(ref loggedInUser);
            this.NavigationService.Navigate(productsPage);
        }
        private void OnButtonClickedWorkOrders(object sender, MouseButtonEventArgs e)
        {
            DeletePhotos();
            WorkOrdersPage workOrdersPage = new WorkOrdersPage(ref loggedInUser);
            this.NavigationService.Navigate(workOrdersPage);
        }
        private void OnButtonClickedWorkOffers(object sender, RoutedEventArgs e)
        {
            DeletePhotos();
            WorkOffersPage workOffers = new WorkOffersPage(ref loggedInUser);
            this.NavigationService.Navigate(workOffers);
        }
        private void OnButtonClickedRFQs(object sender, RoutedEventArgs e)
        {
            DeletePhotos();
            RFQsPage rFQsPage = new RFQsPage(ref loggedInUser);
            this.NavigationService.Navigate(rFQsPage);
        }
        private void OnButtonClickedVisits(object sender, RoutedEventArgs e)
        {
            DeletePhotos();
            ClientVisitsPage clientVisitsPage = new ClientVisitsPage(ref loggedInUser);
            this.NavigationService.Navigate(clientVisitsPage);
        }
        private void OnButtonClickedCalls(object sender, RoutedEventArgs e)
        {
            DeletePhotos();
            ClientCallsPage clientCallsPage = new ClientCallsPage(ref loggedInUser);
            this.NavigationService.Navigate(clientCallsPage);
        }
        private void OnButtonClickedMeetings(object sender, RoutedEventArgs e)
        {
            DeletePhotos();
            OfficeMeetingsPage officeMeetingsPage = new OfficeMeetingsPage(ref loggedInUser);
            this.NavigationService.Navigate(officeMeetingsPage);
        }
        private void OnButtonClickedStatistics(object sender, RoutedEventArgs e)
        {

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
            BackgroundWorker downloadBackground = sender as BackgroundWorker;

            QueryGetModels();

            downloadBackground.ReportProgress(50);

            SetUpPageUIElements();

            //downloadBackground.ReportProgress(100);
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
    }
}
