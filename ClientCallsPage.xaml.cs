using _01electronics_erp;
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

namespace TempCRM
{
    /// <summary>
    /// Interaction logic for ClientCallsPage.xaml
    /// </summary>
    public partial class ClientCallsPage : Page
    {
        private Employee loggedInUser;

        public ClientCallsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;
        }
        private void OnButtonClickedOrders(object sender, RoutedEventArgs e)
        {
            WorkOrdersPage workOrdersPage = new WorkOrdersPage(ref loggedInUser);
            this.NavigationService.Navigate(workOrdersPage);
        }
        private void OnButtonClickedOffers(object sender, RoutedEventArgs e)
        {
            WorkOffersPage workOffersPage = new WorkOffersPage(ref loggedInUser);
            this.NavigationService.Navigate(workOffersPage);
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
            StatisticsPage statisticsPage = new StatisticsPage(ref loggedInUser);
            this.NavigationService.Navigate(statisticsPage);
        }

        private void OnButtonClickedMyProfile(object sender, MouseButtonEventArgs e)
        {
            UserPortalPage userPortal = new UserPortalPage(ref loggedInUser);
            this.NavigationService.Navigate(userPortal);
        }

        private void OnButtonClickedContacts(object sender, MouseButtonEventArgs e)
        {
            ContactsPage contactsPage = new ContactsPage(ref loggedInUser);
            this.NavigationService.Navigate(contactsPage);
        }
    }
}
