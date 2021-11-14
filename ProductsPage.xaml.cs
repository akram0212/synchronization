
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
            InitializeProductSummaryPoints();
            SetUpPageUIElements();
        }
        private void InitializeProducts()
        {
            if (!commonQueries.GetCompanyProducts(ref products))
                return;
        }
        public void InitializeProductSummaryPoints()
        {
            if (!commonQueries.GetProductsSummaryPoints(ref productSummaryPoints))
                return;
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

                Image productImage = new Image();

                string src = String.Format(@"/01electronics_crm;component/photos/products/" + products[i].typeId + ".jpg");
                productImage.Source = new BitmapImage(new Uri(src, UriKind.Relative));
                productImage.HorizontalAlignment = HorizontalAlignment.Stretch;
                productImage.VerticalAlignment = VerticalAlignment.Stretch;
                productImage.MouseDown += ImageMouseDown;
                productImage.Tag = products[i].typeId.ToString();
                gridI.Children.Add(productImage);
                Grid.SetRow(productImage, 0);

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
                pointsTextBlock.Text = productSummaryPoints[i];
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
        private void OnButtonClickedProjects(object sender, MouseButtonEventArgs e)
        {
            ProjectsPage projectsPage = new ProjectsPage(ref loggedInUser);
            this.NavigationService.Navigate(projectsPage);
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
