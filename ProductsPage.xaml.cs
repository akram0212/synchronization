
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using _01electronics_library;

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

        public ProductsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;
            commonQueries = new CommonQueries();
            sqlDatabase = new SQLServer();
            ftpServer = new FTPServer();
            products = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
            productSummaryPoints = new List<string>();

            InitializeProducts();
            InitializeProducSummaryPoints();
            SetUpPageUIElements();
        }
        private void InitializeProducts()
        {
            if (!commonQueries.GetCompanyProducts(ref products))
                return;
        }
        public bool InitializeProducSummaryPoints()
        {
            productSummaryPoints.Clear();

            String sqlQueryPart1 = @"select summary_points
                                     from erp_system.dbo.products_summary_points
                                     inner join erp_system.dbo.products_type
                                     on products_type.id = products_summary_points.id
                                     where products_type.id > 0
                                     order by products_type.product_name;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count(); i++)
            {
                productSummaryPoints.Add(sqlDatabase.rows[i].sql_string[0]);
            }

            productSummaryPoints.Add("Others");

            return true;
        }

        public void SetUpPageUIElements()
        {
            for(int i = 0; i < products.Count(); i++)
            {
                RowDefinition rowI = new RowDefinition();
                ProductsGrid.RowDefinitions.Add(rowI);

                Grid gridI = new Grid();

                RowDefinition imageRow = new RowDefinition();
                gridI.RowDefinitions.Add(imageRow);

                String[] productName = new String[5];
                try
                {
                    productName = products[i].typeName.Split(' ');
                    Image productImage = new Image();

                    string src = String.Format(@"/01electronics_crm;component/photos/" + productName[0] + "_" + productName[1] + "_cover_photo.jpg");
                    productImage.Source = new BitmapImage(new Uri(src, UriKind.Relative));
                    productImage.Width = 1000;
                    productImage.Height = 400;
                    productImage.HorizontalAlignment = HorizontalAlignment.Stretch;
                    productImage.VerticalAlignment = VerticalAlignment.Stretch;
                    productImage.MouseDown += ImageMouseDown;
                    productImage.Tag = products[i].typeId.ToString();
                    gridI.Children.Add(productImage);
                    Grid.SetRow(productImage, 0);

                    TextBlock imageTextBlock = new TextBlock();
                    imageTextBlock.Background = new SolidColorBrush(Color.FromRgb(237, 237, 237));
                    imageTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                    imageTextBlock.Width = 350;
                    imageTextBlock.Height = 150;
                    imageTextBlock.Margin = new Thickness(100, -20, 0, 0);
                    imageTextBlock.Padding = new Thickness(15, 15, 15, 15);
                    imageTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
                    imageTextBlock.FontSize = 16;
                    imageTextBlock.TextWrapping = TextWrapping.Wrap;
                    imageTextBlock.Text = "  " + products[i].typeName;
                    imageTextBlock.Text += "\n\n";

                    imageTextBlock.Text += productSummaryPoints[i];
                    gridI.Children.Add(imageTextBlock);
                    Grid.SetRow(imageTextBlock, 0);
                    ProductsGrid.Children.Add(gridI);
                    Grid.SetRow(gridI, i);

                }
                catch
                {
                    gridI.Tag = i;
                    productName[0] = products[i].typeName;
                    Image productImage = new Image();
                    string src = String.Format(@"/01electronics_crm;component/photos/" + productName[0] + "_cover_photo.jpg");
                    productImage.Source = new BitmapImage(new Uri(src, UriKind.Relative));
                    productImage.Width = 1000;
                    productImage.Height = 400;
                    productImage.MouseDown += ImageMouseDown;
                    productImage.Tag = products[i].typeId.ToString();
                    gridI.Children.Add(productImage);
                    Grid.SetRow(productImage, 0);

                    TextBlock imageTextBlock = new TextBlock();
                    imageTextBlock.Background = new SolidColorBrush(Color.FromRgb(237, 237, 237));
                    imageTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                    imageTextBlock.Width = 350;
                    imageTextBlock.Height = 150;
                    imageTextBlock.Margin = new Thickness(100, -20, 0, 0);
                    imageTextBlock.Padding = new Thickness(15, 15, 15, 15);
                    imageTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
                    imageTextBlock.FontSize = 16;
                    imageTextBlock.TextWrapping = TextWrapping.Wrap;
                    imageTextBlock.Text = "  " + products[i].typeName;
                    imageTextBlock.Text += "\n\n";

                    imageTextBlock.Text += productSummaryPoints[i];
                    Grid.SetRow(imageTextBlock, 0);

                    gridI.Children.Add(imageTextBlock);
                    ProductsGrid.Children.Add(gridI);
                    Grid.SetRow(gridI, i);
                       
                }
            }
        }
      
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //EXTERNAL TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
        private void OnButtonClickedProducts(object sender, MouseButtonEventArgs e)
        {
            ProductsPage productsPage = new ProductsPage(ref loggedInUser);
            this.NavigationService.Navigate(productsPage);
        }
        private void OnButtonClickedWorkOrders(object sender, RoutedEventArgs e)
        {
            WorkOrdersPage workOrders = new WorkOrdersPage(ref loggedInUser);
            this.NavigationService.Navigate(workOrders);
        }
        private void OnButtonClickedWorkOffers(object sender, RoutedEventArgs e)
        {
            WorkOffersPage workOffers = new WorkOffersPage(ref loggedInUser);
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
       
    }
}
