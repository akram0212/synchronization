﻿using _01electronics_library;
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

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for BrandsPage.xaml
    /// </summary>
    public partial class BrandsPage : Page
    {
        private Employee loggedInUser;
        private Product selectedProduct;
        List<COMPANY_WORK_MACROS.BRAND_STRUCT> brandsList;

        //SQL QUERY
        protected String sqlQuery;
        protected CommonQueries commonQueries;

        //SQL OBJECTS
        protected SQLServer sqlDatabase;

        protected FTPServer ftpServer;

        public BrandsPage(ref Employee mLoggedInUser, ref Product mSelectedProduct)
        {
            InitializeComponent();
            loggedInUser = mLoggedInUser;
            selectedProduct = mSelectedProduct;

            sqlDatabase = new SQLServer();
            ftpServer = new FTPServer();
            commonQueries = new CommonQueries();
            brandsList = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();

            QueryGetProductName();
            InitializeProductBrands();
            SetUpPageUIElements();
        }

        public void SetUpPageUIElements()
        {
            Label productTitleLabel = new Label();
            productTitleLabel.Content = selectedProduct.GetProductName();
            productTitleLabel.VerticalAlignment = VerticalAlignment.Stretch;
            productTitleLabel.HorizontalAlignment = HorizontalAlignment.Stretch;
            productTitleLabel.Margin = new Thickness(48, 24, 48, 24);
            productTitleLabel.Style = (Style)FindResource("primaryHeaderTextStyle");
            BrandsGrid.Children.Add(productTitleLabel);
            Grid.SetRow(productTitleLabel, 0);

            WrapPanel brandsWrapPanel = new WrapPanel();
            brandsWrapPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            brandsWrapPanel.Margin = new Thickness(50,0,0,0);

            for (int i = 0; i < brandsList.Count(); i++)
            {
                Grid gridI = new Grid();

                try
                {
                    Image brandLogo = new Image();
                    string src = String.Format(@"/01electronics_crm;component/photos/brands/" + brandsList[i].brandId + ".jpg");
                    brandLogo.Source = new BitmapImage(new Uri(src, UriKind.Relative));
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

                    gridI.Children.Add(brandLogo);

                    brandsWrapPanel.Children.Add(gridI);
                    
                }
                catch
                {

                }
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

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //QUERIES
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
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
        ///
        private void ImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            String tmp = currentImage.Tag.ToString();
            String Name = currentImage.Name.ToString();

            selectedProduct.SetBrandID(int.Parse(tmp));
            selectedProduct.SetBrandName(Name);

            ModelsPage productsPage = new ModelsPage(ref loggedInUser, ref selectedProduct);
            this.NavigationService.Navigate(productsPage);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //EXTERNAL TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void OnButtonClickedMyProfile(object sender, RoutedEventArgs e)
        {
            UserPortalPage userPortal = new UserPortalPage(ref loggedInUser);
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
            WorkOffersPage workOffers = new WorkOffersPage(ref loggedInUser);
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
            ProductsPage productsPage = new ProductsPage(ref loggedInUser);
            this.NavigationService.Navigate(productsPage);
        }

        private void OnButtonClickedMaintenanceContracts(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnButtonClickedProjects(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
