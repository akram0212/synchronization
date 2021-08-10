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
    /// Interaction logic for BorriUPSPage.xaml
    /// </summary>
    public partial class BorriUPSPage : Page
    {
        private Employee loggedInUser;
        protected CommonQueries commonQueries;
        protected List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> productsTypes;
        private Product selectedProduct;
        public BorriUPSPage(ref Employee mLoggedInUser, ref Product mSelectedProduct)
        {
            InitializeComponent();
            loggedInUser = mLoggedInUser;
            selectedProduct = mSelectedProduct;

            commonQueries = new CommonQueries();

            FillProductsSummaryPoints();
        }
        public void FillProductsSummaryPoints()
        {
            //if(!commonQueries.GetCompanyModels())
        }

        /////////////////////////////////////////////////////////////////
        //EXTERNAL TABS
        /////////////////////////////////////////////////////////////////
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

        /////////////////////////////////////////////////////////////////
        //MOUSE DOWN HANDLERS
        /////////////////////////////////////////////////////////////////
        ///
        private void BorriCommercialUPSImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            selectedProduct.SetProductID(2);
            BorriCommercialUPSPage borriCommercialUPSPage = new BorriCommercialUPSPage(ref loggedInUser, ref selectedProduct);
            this.NavigationService.Navigate(borriCommercialUPSPage);
        } 
        private void BorriIndustrialUPSImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("done");
        }
        private void OnButtonClickedProducts(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("done");
        }
    }
}
