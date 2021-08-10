using _01electronics_library;
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

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for BorriCommercialUPSPage.xaml
    /// </summary>
    public partial class BorriCommercialUPSPage : Page
    {
        private Employee loggedInUser;
        protected CommonQueries commonQueries;
        protected List<COMPANY_WORK_MACROS.MODEL_STRUCT> productModels;
        private Product selectedProduct;
        public BorriCommercialUPSPage(ref Employee mLoggedInUser, ref Product mSelectedProduct)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;
            selectedProduct = mSelectedProduct;
            commonQueries = new CommonQueries();
            productModels = new List<COMPANY_WORK_MACROS.MODEL_STRUCT>();

            InitializeProducts();
        }
        private void InitializeProducts()
        {
            COMPANY_WORK_MACROS.PRODUCT_STRUCT product = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
            product.typeId = 2;

            COMPANY_WORK_MACROS.BRAND_STRUCT brand = new COMPANY_WORK_MACROS.BRAND_STRUCT();
            brand.brandId = 1;

            if (!commonQueries.GetCompanyModels(product, brand,ref productModels))
                return;
        }

        public void SetUpPageUIElements()
        {
            for(int i = 0; i < productModels.Count(); i++)
            {
                Grid currentProductGrid = new Grid();
                currentProductGrid.Margin = new Thickness(24);

                RowDefinition row1 = new RowDefinition();
                RowDefinition row2 = new RowDefinition();
                RowDefinition row3 = new RowDefinition();
                RowDefinition row4 = new RowDefinition();
                RowDefinition row5 = new RowDefinition();
                RowDefinition row6 = new RowDefinition();

                currentProductGrid.RowDefinitions.Add(row1);
                currentProductGrid.RowDefinitions.Add(row2);
                currentProductGrid.RowDefinitions.Add(row3);
                currentProductGrid.RowDefinitions.Add(row4);
                currentProductGrid.RowDefinitions.Add(row5);
                currentProductGrid.RowDefinitions.Add(row6);

                Label mainLabel = new Label();
                int productNumber = i + 1;
                mainLabel.Content = "Product " + productNumber;
                mainLabel.Style = (Style)FindResource("tableHeaderItem");
                currentProductGrid.Children.Add(mainLabel);
                Grid.SetRow(mainLabel, 0);
            }

        }

        /////////////////////////////////////////////////////////////////
        //EXTERNAL TABS
        /////////////////////////////////////////////////////////////////

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
        private void OnButtonClickedWorkOrders(object sender, MouseButtonEventArgs e)
        {
            WorkOrdersPage workOrdersPage = new WorkOrdersPage(ref loggedInUser);
            this.NavigationService.Navigate(workOrdersPage);
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

        /////////////////////////////////////////////////////////////////
        //MOUSE DOWN HANDLERS
        /////////////////////////////////////////////////////////////////
        private void UPSImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            UPSPage productsPage = new UPSPage(ref loggedInUser);
            this.NavigationService.Navigate(productsPage);
        }
        private void LegrandUPSImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("done");
        }
    }
}
