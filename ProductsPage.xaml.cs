
using _01electronics_library;
using _01electronics_windows_library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        private Employee loggedInUser;
        private CommonQueries commonQueries;
        private List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> products;
        protected List<String> productSummaryPoints;
        protected String sqlQuery;
        protected SQLServer sqlDatabase;
        protected FTPServer ftpServer;
        protected Product selectedProduct;
        protected List<String> productsNames;
        protected String returnMessage;
        private Expander currentExpander;
        private Expander previousExpander;

        protected int mViewAddCondition;

        public ProductsPage(ref Employee mLoggedInUser, ref Product mSelectedProduct)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;
            mViewAddCondition = COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION;
            commonQueries = new CommonQueries();
            sqlDatabase = new SQLServer();
            ftpServer = new FTPServer();
            products = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
            productSummaryPoints = new List<string>();
            selectedProduct = mSelectedProduct;
            productsNames = new List<String>();
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/01Electronics_ERP/products");
            
            InitializeProducts();
            InitializeProductSummaryPoints();
            SetUpPageUIElements();

        }
        private void InitializeProducts()
        {
            if (!commonQueries.GetCompanyProducts(ref products, selectedProduct.GetCategoryID()))
                return;
        }
        public void InitializeProductSummaryPoints()
        {
            if (!commonQueries.GetProductsSummaryPoints(ref productSummaryPoints, selectedProduct.GetCategoryID()))
                return;
        }

        public void SetUpPageUIElements()
        {
            ProductsGrid.Children.Clear();
            ProductsGrid.RowDefinitions.Clear();
            ProductsGrid.ColumnDefinitions.Clear();

            ftpServer.ListFilesInFolder(selectedProduct.GetProductFolderServerPath(), ref productsNames, ref returnMessage);
            selectedProduct.GetProductFolderLocalPath();

            if (productsNames.Count() == 0)
            {
                string[] filesNames = Directory.GetFiles(selectedProduct.GetProductFolderLocalPath());
                foreach (string file in filesNames)
                {
                    productsNames.Add(file);
                }
                //ftpServer.ListFilesInFolder(selectedProduct.GetFolderLocalPath(), ref modelsNames, ref returnMessage); 
            }

            for (int i = 0; i < products.Count(); i++)
            {
                RowDefinition rowI = new RowDefinition();
                ProductsGrid.RowDefinitions.Add(rowI);

                Grid gridI = new Grid();


                RowDefinition imageRow = new RowDefinition();
                gridI.RowDefinitions.Add(imageRow);

                selectedProduct.SetProductID(products[i].typeId);
                //String productLocalPath = selectedProduct.GetPhotoLocalPath() + "\\" + products[i].typeId + ".jpg";

                if (productsNames.Exists(modelName => modelName == selectedProduct.GetProductPhotoLocalPath() || productsNames.Exists(modelName2 => modelName2 == (products[i].typeId + ".jpg"))))
                {
                    try
                    {
                        Image productImage = new Image();

                        //string src = String.Format(@"/01electronics_crm;component/photos/products/" + products[i].typeId + ".jpg");

                        BitmapImage src = new BitmapImage();
                        src.BeginInit();
                        src.UriSource = new Uri(selectedProduct.GetProductPhotoLocalPath(), UriKind.Relative);
                        src.CacheOption = BitmapCacheOption.OnLoad;
                        src.EndInit();
                        productImage.Source = src;
                        productImage.HorizontalAlignment = HorizontalAlignment.Stretch;
                        productImage.VerticalAlignment = VerticalAlignment.Stretch;
                        productImage.MouseDown += ImageMouseDown;
                        productImage.Tag = products[i].typeId.ToString();

                        gridI.Children.Add(productImage);
                        Grid.SetRow(productImage, 0);

                        Expander expander = new Expander();
                        expander.Tag = products[i].typeId.ToString();
                        expander.ExpandDirection = ExpandDirection.Down;
                        expander.VerticalAlignment = VerticalAlignment.Top;
                        expander.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                        expander.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;

                        expander.Expanded += new RoutedEventHandler(OnExpandExpander);
                        expander.Collapsed += new RoutedEventHandler(OnCollapseExpander);
                        expander.Margin = new Thickness(12);

                        StackPanel expanderStackPanel = new StackPanel();
                        expanderStackPanel.Orientation = Orientation.Vertical;


                        BrushConverter brushConverter = new BrushConverter();



                        Button ViewButton = new Button();
                        ViewButton.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");
                        ViewButton.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                        ViewButton.Click += OnBtnClickViewProduct;
                        ViewButton.Content = "View";






                        expanderStackPanel.Children.Add(ViewButton);

                        expander.Content = expanderStackPanel;

                        gridI.Children.Add(expander);
                        //Grid.SetColumn(expander, 1);


                    }
                    catch
                    {
                        selectedProduct.SetPhotoServerPath(selectedProduct.GetProductFolderServerPath() + "/" + products[i].typeId + ".jpg");
                        if (selectedProduct.DownloadPhotoFromServer())
                        {
                            Image productImage = new Image();
                            BitmapImage src = new BitmapImage();
                            src.BeginInit();
                            src.UriSource = new Uri(selectedProduct.GetProductPhotoLocalPath(), UriKind.Relative);
                            src.CacheOption = BitmapCacheOption.OnLoad;
                            src.EndInit();
                            productImage.Source = src;
                            productImage.HorizontalAlignment = HorizontalAlignment.Stretch;
                            productImage.VerticalAlignment = VerticalAlignment.Stretch;
                            productImage.MouseDown += ImageMouseDown;
                            productImage.Tag = products[i].typeId.ToString();
                            gridI.Children.Add(productImage);
                            Grid.SetRow(productImage, 0);
                            Expander expander = new Expander();
                            expander.Tag = products[i].typeId.ToString();
                            expander.ExpandDirection = ExpandDirection.Down;
                            expander.VerticalAlignment = VerticalAlignment.Top;
                            expander.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                            expander.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;

                            expander.Expanded += new RoutedEventHandler(OnExpandExpander);
                            expander.Collapsed += new RoutedEventHandler(OnCollapseExpander);
                            expander.Margin = new Thickness(12);

                            StackPanel expanderStackPanel = new StackPanel();
                            expanderStackPanel.Orientation = Orientation.Vertical;


                            BrushConverter brushConverter = new BrushConverter();



                            Button ViewButton = new Button();
                            ViewButton.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");
                            ViewButton.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                            ViewButton.Click += OnBtnClickViewProduct;
                            ViewButton.Content = "View";






                            expanderStackPanel.Children.Add(ViewButton);

                            expander.Content = expanderStackPanel;

                            gridI.Children.Add(expander);
                            //Grid.SetColumn(expander, 1);
                        }
                    }
                }
                Grid imageGrid = new Grid();
                imageGrid.Background = new SolidColorBrush(Color.FromRgb(237, 237, 237));
                imageGrid.Width = 350;
                imageGrid.Height = 150;
                imageGrid.Margin = new Thickness(100, -20, 0, 0);
                imageGrid.HorizontalAlignment = HorizontalAlignment.Left;

                RowDefinition headerRow = new RowDefinition();
                imageGrid.RowDefinitions.Add(headerRow);
                headerRow.Height = new GridLength(40);


                RowDefinition pointsRow = new RowDefinition();
                imageGrid.RowDefinitions.Add(pointsRow);

                Grid headerGrid = new Grid();
                RowDefinition headerGridRow = new RowDefinition();
                headerGrid.RowDefinitions.Add(headerGridRow);
                Grid.SetRow(headerGrid, 0);

                Label headerLabel = new Label();
                headerLabel.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                headerLabel.FontFamily = new FontFamily("Sans Serif");
                headerLabel.FontSize = 17;
                headerLabel.FontWeight = FontWeights.Bold;
                headerLabel.Padding = new Thickness(10);
                headerLabel.Content = products[i].typeName;

                Grid.SetRow(headerLabel, 0);
                headerGrid.Children.Add(headerLabel);
                imageGrid.Children.Add(headerGrid);

                TextBlock pointsTextBlock = new TextBlock();
                pointsTextBlock.Foreground = Brushes.Black;
                pointsTextBlock.TextWrapping = TextWrapping.Wrap;
                pointsTextBlock.FontSize = 15;
                pointsTextBlock.FontStyle = FontStyles.Italic;
                if (i < productSummaryPoints.Count)
                {
                    pointsTextBlock.Text = productSummaryPoints[i];
                }
                pointsTextBlock.Padding = new Thickness(20);

                Grid.SetRow(pointsTextBlock, 1);
                imageGrid.Children.Add(pointsTextBlock);

                gridI.Children.Add(imageGrid);
                Grid.SetRow(imageGrid, 0);
                ProductsGrid.Children.Add(gridI);
                Grid.SetRow(gridI, i);
            }
            if( loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.ERP_SYSTEM_DEVELOPMENT_TEAM_ID ||
                loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.BUSINESS_DEVELOPMENT_TEAM_ID ||
                ( loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION && loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.BUSINESS_DEVELOPMENT_DEPARTMENT_ID))
            {
                addBtn.Visibility = Visibility.Visible;
            }
        }

       

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //EXTERNAL TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnButtonClickedContacts(object sender, RoutedEventArgs e)
        {
            ContactsPage contacts = new ContactsPage(ref loggedInUser);
            this.NavigationService.Navigate(contacts);
        }
        private void OnButtonClickedProducts(object sender, MouseButtonEventArgs e)
        {
            CategoriesPage productsPage = new CategoriesPage(ref loggedInUser);
            this.NavigationService.Navigate(productsPage);
        }
        private void OnButtonClickedWorkOrders(object sender, RoutedEventArgs e)
        {
            WorkOrdersPage workOrders = new WorkOrdersPage(ref loggedInUser);
            this.NavigationService.Navigate(workOrders);
        }
        private void OnButtonClickedWorkOffers(object sender, RoutedEventArgs e)
        {
            QuotationsPage workOffers = new QuotationsPage(ref loggedInUser);
            this.NavigationService.Navigate(workOffers);
        }
        private void OnButtonClickedRFQs(object sender, RoutedEventArgs e)
        {
            RFQsPage rFQsPage = new RFQsPage(ref loggedInUser);
            this.NavigationService.Navigate(rFQsPage);
        }
        private void OnButtonClickedVisits(object sender, RoutedEventArgs e)
        {
            ClientVisitsPage clientVisitsPage = new ClientVisitsPage(ref loggedInUser);
            this.NavigationService.Navigate(clientVisitsPage);
        }
        private void OnButtonClickedCalls(object sender, RoutedEventArgs e)
        {
            ClientCallsPage clientCallsPage = new ClientCallsPage(ref loggedInUser);
            this.NavigationService.Navigate(clientCallsPage);
        }
        private void OnButtonClickedMeetings(object sender, RoutedEventArgs e)
        {
            OfficeMeetingsPage officeMeetingsPage = new OfficeMeetingsPage(ref loggedInUser);
            this.NavigationService.Navigate(officeMeetingsPage);
        }
        private void OnButtonClickedStatistics(object sender, RoutedEventArgs e)
        {

        }
        private void OnButtonClickedProjects(object sender, MouseButtonEventArgs e)
        {
            ProjectsPage projectsPage = new ProjectsPage(ref loggedInUser);
            this.NavigationService.Navigate(projectsPage);
        }
        private void OnButtonClickedMaintenanceContracts(object sender, MouseButtonEventArgs e)
        {
            MaintenanceContractsPage maintenanceContractsPage = new MaintenanceContractsPage(ref loggedInUser);
            this.NavigationService.Navigate(maintenanceContractsPage);
        }
        private void OnButtonClickedMaintenanceOffer(object sender, MouseButtonEventArgs e)
        {
            MaintenanceOffersPage maintenanceOffersPage = new MaintenanceOffersPage(ref loggedInUser);
            this.NavigationService.Navigate(maintenanceOffersPage);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //MOUSE DOWN HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void ImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            String tmp = currentImage.Tag.ToString();

            Product selectedProduct = new Product();
            selectedProduct.SetProductID(int.Parse(tmp));

            BrandsPage brandsPage = new BrandsPage(ref loggedInUser, ref selectedProduct);
            this.NavigationService.Navigate(brandsPage);
        }

        private void OnButtonClickedMyProfile(object sender, MouseButtonEventArgs e)
        {
            StatisticsPage statisticsPage = new StatisticsPage(ref loggedInUser);
            NavigationService.Navigate(statisticsPage);
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

    



        private void addBtnMouseEnter(object sender, MouseEventArgs e)
        {
            Storyboard storyboard = new Storyboard();
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 200);
            DoubleAnimation animation = new DoubleAnimation();

            animation.From = addBtn.Opacity;
            animation.To = 1.0;
            animation.Duration = new Duration(duration);

            Storyboard.SetTargetName(animation, addBtn.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));

            storyboard.Children.Add(animation);

            storyboard.Begin(this);
        }

        private void addBtnMouseLeave(object sender, MouseEventArgs e)
        {
           

            Storyboard storyboard = new Storyboard();
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 200);
            DoubleAnimation animation = new DoubleAnimation();

            animation.From = addBtn.Opacity;
            animation.To = 0.5;
            animation.Duration = new Duration(duration);

            Storyboard.SetTargetName(animation, addBtn.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));

            storyboard.Children.Add(animation);

            storyboard.Begin(this);

        }
        private void OnBtnClickViewProduct(object sender, RoutedEventArgs e)
        {
            Button viewButton = (Button)sender;
            StackPanel expanderStackPanel = (StackPanel)viewButton.Parent;
            Expander expander = (Expander)expanderStackPanel.Parent;
            selectedProduct.SetProductID(int.Parse(expander.Tag.ToString()));
            selectedProduct.InitializeProductInfo();
            mViewAddCondition = COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION;
            AddProductWindow addProductWindow = new AddProductWindow(ref selectedProduct, ref loggedInUser, ref mViewAddCondition);
            addProductWindow.Closed += OnCloseAddPRoductsWindow;
            addProductWindow.Show();
        }
       
        private void onBtnAddClick(object sender, MouseButtonEventArgs e)
        {
            mViewAddCondition = COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION;
            AddProductWindow addProductWindow = new AddProductWindow(ref selectedProduct , ref loggedInUser , ref mViewAddCondition);
            addProductWindow.Closed += OnCloseAddPRoductsWindow;
            addProductWindow.Show();
        }

        private void OnCloseAddPRoductsWindow(object sender, EventArgs e)
        {
            products.Clear();
            
            InitializeProducts();
            InitializeProductSummaryPoints();
            SetUpPageUIElements();
        }

    
    }


}
