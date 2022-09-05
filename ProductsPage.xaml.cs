
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using _01electronics_library;
using _01electronics_windows_library;

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

        public ProductsPage(ref Employee mLoggedInUser, ref Product mSelectedProduct)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;
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


                gridI.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
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
                        expander.Tag = i;
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

                       

                        Button EditButton = new Button();
                        EditButton.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");
                        EditButton.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                       // EditButton.Click += OnBtnClickEditProduct;
                        EditButton.Content = "Edit";

                        



                        
                        expanderStackPanel.Children.Add(EditButton);

                        expander.Content = expanderStackPanel;

                        gridI.Children.Add(expander);
                        Grid.SetColumn(expander, 2);


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
            
            
            ColumnDefinition expanderColumn = currentGrid.ColumnDefinitions[0];
            currentExpander.VerticalAlignment = VerticalAlignment.Top;
            expanderColumn.Width = new GridLength(120);
        }

        private void OnCollapseExpander(object sender, RoutedEventArgs e)
        {
            Expander currentExpander = (Expander)sender;
            Grid currentGrid = (Grid)currentExpander.Parent;
            ColumnDefinition expanderColumn = currentGrid.ColumnDefinitions[0];
            currentExpander.VerticalAlignment = VerticalAlignment.Top;
            currentExpander.Margin = new Thickness(12);
            expanderColumn.Width = new GridLength(50);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

       

        private void addBtnMouseEnter(object sender, MouseEventArgs e)
        {
            
           // addBtn.Opacity = 1;

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
            // addBtn.Opacity = 0.5;

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
    }


}
