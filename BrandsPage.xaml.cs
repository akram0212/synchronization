using _01electronics_library;
using _01electronics_windows_library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for BrandsPage.xaml
    /// </summary>
    public partial class BrandsPage : Page
    {
        private Employee loggedInUser;
        private Brand selectedProduct;
        List<COMPANY_WORK_MACROS.BRAND_STRUCT> brandsList;

        //SQL QUERY
        protected String sqlQuery;
        protected CommonQueries commonQueries;

        //SQL OBJECTS
        protected SQLServer sqlDatabase;

        protected FTPServer ftpServer;
        protected List<String> brandsNames;
        protected String returnMessage;
        protected int mViewAddCondition;
        private Expander currentExpander;
        private Expander previousExpander;
        public BrandsPage(ref Employee mLoggedInUser, ref Brand mSelectedProduct)
        {
            InitializeComponent();
            loggedInUser = mLoggedInUser;
            selectedProduct = mSelectedProduct;
            mViewAddCondition = COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION;

            sqlDatabase = new SQLServer();
            ftpServer = new FTPServer();
            commonQueries = new CommonQueries();
            brandsList = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();
            brandsNames = new List<String>();
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/01 Electronics/brands");

            QueryGetProductName();
            InitializeProductBrands();
            SetUpPageUIElements();
        }

        public void SetUpPageUIElements()
        {
            BrandsGrid.Children.Clear();
            BrandsGrid.RowDefinitions.Clear();
            BrandsGrid.ColumnDefinitions.Clear();

            Label productTitleLabel = new Label();
            productTitleLabel.Content = selectedProduct.GetProductName();
            productTitleLabel.Content = productTitleLabel.Content.ToString().ToUpper();
            productTitleLabel.VerticalAlignment = VerticalAlignment.Stretch;
            productTitleLabel.HorizontalAlignment = HorizontalAlignment.Stretch;
            productTitleLabel.Margin = new Thickness(48, 24, 48, 24);
            productTitleLabel.Style = (Style)FindResource("primaryHeaderTextStyle");
            mainGrid.Children.Add(productTitleLabel);
            Grid.SetRow(productTitleLabel, 0);



            WrapPanel brandsWrapPanel = new WrapPanel();
            brandsWrapPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            brandsWrapPanel.Margin = new Thickness(50, 0, 0, 0);

            for (int i = 0; i < brandsList.Count(); i++)
            {
                bool foundImage = true;
                if (brandsList[i].brandId == 0)
                    continue;
                Grid brandGrid = new Grid();

                selectedProduct.SetBrandID(brandsList[i].brandId);


                Image brandLogo = new Image();
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(selectedProduct.GetBrandPhotoLocalPath(), UriKind.Relative);
                src.CacheOption = BitmapCacheOption.OnLoad;


                if (!File.Exists(selectedProduct.GetBrandPhotoLocalPath())) {

                    brandGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(120) });
                    brandGrid.RowDefinitions.Add(new RowDefinition());

                    foundImage = false;
          
                    TextBlock label = new TextBlock();
                    label.Text = $"{brandsList[i].brandName.ToUpper()}";
                    BrushConverter converter = new BrushConverter();
                    label.Foreground = (Brush)converter.ConvertFrom("#105A97");
                    label.FontWeight = FontWeights.Bold;
                    label.HorizontalAlignment = HorizontalAlignment.Center;
                    label.VerticalAlignment = VerticalAlignment.Center;
                    label.Padding = new Thickness(10);
                    label.Height = 100;
                    label.Width = 250;
                    label.FontSize = 30;
                    label.Tag = brandsList[i].brandId.ToString();
                    label.Name= brandsList[i].brandName;
                    label.MouseLeftButtonDown += brandTextBlock_MouseLeftButtonDown;


                    brandGrid.Children.Add(label);
                    Grid.SetRow(label, 1);


                    brandGrid.Margin = new Thickness(0, 0, 20, 0);

                    ScaleTransform myScaleTransform1 = new ScaleTransform();
                    var e3 = new EventTrigger(UIElement.MouseEnterEvent);
                    e3.Actions.Add(new BeginStoryboard { Storyboard = (Storyboard)FindResource("expandStoryboard") });
                    var e4 = new EventTrigger(UIElement.MouseLeaveEvent);
                    e4.Actions.Add(new BeginStoryboard { Storyboard = (Storyboard)FindResource("shrinkStoryboard") });

                    myScaleTransform1.ScaleY = 1;
                    myScaleTransform1.ScaleX = 1;
                    label.RenderTransform = myScaleTransform1;

                    label.Triggers.Add(e3);
                    label.Triggers.Add(e4);

                }

                else
                {
                    try
                    {
                        src.EndInit();
                    }
                    catch (Exception c)
                    {
                        foundImage = false;
                        TextBlock label = new TextBlock();
                        label.Text = $"{brandsList[i].brandName.ToUpper()}";
                        label.VerticalAlignment = VerticalAlignment.Stretch;
                        label.HorizontalAlignment = HorizontalAlignment.Stretch;
                        BrushConverter converter = new BrushConverter();
                        label.Foreground = (Brush)converter.ConvertFrom("#105A97");
                        label.FontWeight = FontWeights.Bold;
                        label.Margin = new Thickness(0, 0, 20, 20);
                        label.Padding = new Thickness(10);
                        label.Background = Brushes.White;
                        label.FontSize = 30;
                        label.Tag = brandsList[i].brandId.ToString();
                        label.Name = brandsList[i].brandName;
                        label.MouseLeftButtonDown += brandTextBlock_MouseLeftButtonDown;

                        brandGrid.Children.Add(label);

                        Grid.SetRow(label, 1);


                    }
                }

                brandLogo.Source = src;
                brandLogo.Width = 300;
                brandLogo.MouseDown += ImageMouseDown;
                brandLogo.Margin = new Thickness(80, 100, 12, 12);
                brandLogo.Tag = brandsList[i].brandId.ToString();
                brandLogo.Name = brandsList[i].brandName;

                var e1 = new EventTrigger(UIElement.MouseEnterEvent);
                e1.Actions.Add(new BeginStoryboard { Storyboard = (Storyboard)FindResource("expandStoryboard") });
                var e2 = new EventTrigger(UIElement.MouseLeaveEvent);
                e2.Actions.Add(new BeginStoryboard { Storyboard = (Storyboard)FindResource("shrinkStoryboard") });
                ScaleTransform myScaleTransform = new ScaleTransform();
                myScaleTransform.ScaleY = 1;
                myScaleTransform.ScaleX = 1;
                brandLogo.RenderTransform = myScaleTransform;

                brandLogo.Triggers.Add(e1);
                brandLogo.Triggers.Add(e2);

                //if(brandsList[i].brandId == 0)
                //{
                //    Label othersLabel = new Label();
                //    othersLabel.Content = brandsList[i].brandName;
                //    othersLabel.Style = (Style)FindResource("tableHeaderItem");
                //    gridI.Children.Add(othersLabel);
                //}    



                Expander expander = new Expander();
                expander.Tag = brandsList[i].brandId.ToString();
                expander.ExpandDirection = ExpandDirection.Down;
                expander.VerticalAlignment = VerticalAlignment.Top;
                expander.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                expander.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                expander.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
                expander.VerticalContentAlignment = System.Windows.VerticalAlignment.Top;


                expander.Expanded += new RoutedEventHandler(OnExpandExpander);
                expander.Collapsed += new RoutedEventHandler(OnCollapseExpander);
                expander.Margin = new Thickness(12);

                StackPanel expanderStackPanel = new StackPanel();
                expanderStackPanel.Orientation = Orientation.Vertical;


                BrushConverter brushConverter = new BrushConverter();



                Button ViewButton = new Button();
                ViewButton.Background = (Brush)brushConverter.ConvertFrom("#FFFFFF");
                ViewButton.Foreground = (Brush)brushConverter.ConvertFrom("#105A97");
                ViewButton.Click += OnBtnClickViewBrand;
                ViewButton.Content = "View";


                expanderStackPanel.Children.Add(ViewButton);

                expander.Content = expanderStackPanel;
                expander.Margin = new Thickness(0, 75, 0, 0);

                if (foundImage == true)
                    brandGrid.Children.Add(brandLogo);




                brandGrid.Children.Add(expander);

                if(foundImage==false)
                Grid.SetRow(expander, 0);
                brandsWrapPanel.Children.Add(brandGrid);


            }

            if (brandsList.Count() == 0 || brandsList[0].brandId == 0)
            {
                Image brandImage = new Image();
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri("..\\..\\Photos\\brands\\" + "00.jpg", UriKind.Relative);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                brandImage.Source = src;
                brandImage.VerticalAlignment = VerticalAlignment.Center;
                brandImage.Margin = new Thickness(100);
                brandImage.HorizontalAlignment = HorizontalAlignment.Center;
                brandImage.Tag = 00;
                brandsWrapPanel.Children.Add(brandImage);
            }

            BrandsGrid.Children.Add(brandsWrapPanel);
            Grid.SetRow(brandsWrapPanel, 1);
            if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.ERP_SYSTEM_DEVELOPMENT_TEAM_ID ||
                loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.BUSINESS_DEVELOPMENT_TEAM_ID ||
                (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION && loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.BUSINESS_DEVELOPMENT_DEPARTMENT_ID))
            {
                addBtn.Visibility = Visibility.Visible;
            }
        }

        private void brandTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {


            TextBlock brandTextBlock = sender as TextBlock;
            String tmp = brandTextBlock.Tag.ToString();
            String Name = brandTextBlock.Name.ToString();

            selectedProduct.SetBrandID(int.Parse(tmp));
            selectedProduct.SetBrandName(Name);

            Model SelectedModel = new Model();

            SelectedModel.SetProductID(selectedProduct.GetProductID());
            SelectedModel.SetCategoryID(selectedProduct.GetCategoryID());
            SelectedModel.SetBrandID(selectedProduct.GetBrandID());

            SelectedModel.SetProductName(selectedProduct.GetProductName());
            SelectedModel.SetBrandName(selectedProduct.GetBrandName());
            SelectedModel.SetCategoryName(selectedProduct.GetCategoryName());

            ModelsPage productsPage = new ModelsPage(ref loggedInUser, ref SelectedModel /*ref selectedProduct*/);
            this.NavigationService.Navigate(productsPage);


        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //QUERIES
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool QueryGetProductName()
        {
            String sqlQueryPart1 = @"select product_name
                                     from erp_system.dbo.products_type
                                     where id = ";
            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += selectedProduct.GetProductID();

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            selectedProduct.SetProductName(sqlDatabase.rows[0].sql_string[0]);

            return true;
        }
        public void InitializeProductBrands()
        {
            if (!commonQueries.GetProductBrands(selectedProduct.GetProductID(), ref brandsList))
                return;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //MOUSE DOWN HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
       
        private void ImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            String tmp = currentImage.Tag.ToString();
            String Name = currentImage.Name.ToString();

            selectedProduct.SetBrandID(int.Parse(tmp));
            selectedProduct.SetBrandName(Name);

            Model SelectedModel = new Model();

            SelectedModel.SetProductID(selectedProduct.GetProductID());
            SelectedModel.SetCategoryID(selectedProduct.GetCategoryID());
            SelectedModel.SetBrandID(selectedProduct.GetBrandID());

            SelectedModel.SetProductName(selectedProduct.GetProductName());
            SelectedModel.SetBrandName(selectedProduct.GetBrandName());
            SelectedModel.SetCategoryName(selectedProduct.GetCategoryName());

            ModelsPage productsPage = new ModelsPage(ref loggedInUser, ref SelectedModel /*ref selectedProduct*/);
            this.NavigationService.Navigate(productsPage);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //EXTERNAL TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
       
        private void OnButtonClickedMyProfile(object sender, RoutedEventArgs e)
        {
            StatisticsPage userPortal = new StatisticsPage(ref loggedInUser);
            this.NavigationService.Navigate(userPortal);
        }
        private void OnButtonClickedContacts(object sender, RoutedEventArgs e)
        {
            ContactsPage contacts = new ContactsPage(ref loggedInUser);
            this.NavigationService.Navigate(contacts);
        }
        private void OnButtonClickedWorkOrders(object sender, RoutedEventArgs e)
        {

        }
        private void OnButtonClickedWorkOffers(object sender, RoutedEventArgs e)
        {
            QuotationsPage workOffers = new QuotationsPage(ref loggedInUser);
            this.NavigationService.Navigate(workOffers);
        }
        private void OnButtonClickedRFQs(object sender, RoutedEventArgs e)
        {
            RFQsPage rfqs = new RFQsPage(ref loggedInUser);
            this.NavigationService.Navigate(rfqs);
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
        private void OnButtonClickedProducts(object sender, MouseButtonEventArgs e)
        {
            CategoriesPage productsPage = new CategoriesPage(ref loggedInUser);
            this.NavigationService.Navigate(productsPage);
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
        private void OnButtonClickedProjects(object sender, MouseButtonEventArgs e)
        {

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
        private void onBtnAddClick(object sender, MouseButtonEventArgs e)
        {
            mViewAddCondition = COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION;
            AddBrand addBrandWindow = new AddBrand(ref selectedProduct, ref loggedInUser, ref mViewAddCondition ,ref brandsList);
            addBrandWindow.Closed += OnCloseBrandsWindow;
            addBrandWindow.Show();
        }

        private void OnCloseBrandsWindow(object sender, EventArgs e)
        {
            brandsList.Clear();

            QueryGetProductName();
            InitializeProductBrands();
            SetUpPageUIElements();
        }
        private void OnBtnClickViewBrand(object sender, RoutedEventArgs e)
        {
            Button viewButton = (Button)sender;
            StackPanel expanderStackPanel = (StackPanel)viewButton.Parent;
            Expander expander = (Expander)expanderStackPanel.Parent;
            selectedProduct.SetBrandID(int.Parse(expander.Tag.ToString()));

            
            
            mViewAddCondition = COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION;
            AddBrand addBrandWindow = new AddBrand(ref selectedProduct, ref loggedInUser, ref mViewAddCondition, ref brandsList);
            addBrandWindow.Show();
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Expander Handelers
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
            currentExpander.Margin = new Thickness(0, 75, 0, 0);
        }

        private void OnCollapseExpander(object sender, RoutedEventArgs e)
        {
            Expander currentExpander = (Expander)sender;
            Grid currentGrid = (Grid)currentExpander.Parent;
            currentExpander.VerticalAlignment = VerticalAlignment.Top;
            currentExpander.Margin = new Thickness(0, 75, 0, 0);
            //currentExpander.Margin = new Thickness(12);
        }
    }
}
