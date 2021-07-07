using _01electronics_crm;
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

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for ClientCallsPage.xaml
    /// </summary>
    public partial class ClientCallsPage : Page
    {
        private Employee loggedInUser;
        protected CommonQueries commonQueries;
        protected CommonFunctions commonFunctions;

        public ClientCallsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;

            commonQueries = new CommonQueries();
            commonFunctions = new CommonFunctions();

            InitializeYearsComboBox();
            InitializeQuartersComboBox();
        }
        private void InitializeYearsComboBox()
        {
            for (int year = BASIC_MACROS.CRM_START_YEAR; year <= DateTime.Now.Year; year++)
                yearCombo.Items.Add(year);

        }
        private void InitializeQuartersComboBox()
        {
            for (int i = 0; i < BASIC_MACROS.NO_OF_QUARTERS; i++)
                quarterCombo.Items.Add(commonFunctions.GetQuarterName(i + 1));

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
            //StatisticsPage statisticsPage = new StatisticsPage(ref loggedInUser);
            //this.NavigationService.Navigate(statisticsPage);
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

        private void OnButtonClickedWorkOrders(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnButtonClickedWorkOffers(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnButtonClickedRFQs(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnButtonClickedVisits(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnButtonClickedCalls(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnButtonClickedMeetings(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnButtonClickedStatistics(object sender, MouseButtonEventArgs e)
        {

        }

        private void YearCheckBoxChecked(object sender, RoutedEventArgs e)
        {

        }

        private void YearCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {

        }

        private void YearComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void QuarterCheckBoxChecked(object sender, RoutedEventArgs e)
        {

        }

        private void QuarterCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {

        }

        private void QuarterComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void EmployeeCheckBoxChecked(object sender, RoutedEventArgs e)
        {

        }

        private void EmployeeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ProductCheckBoxChecked(object sender, RoutedEventArgs e)
        {

        }

        private void ProductCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {

        }

        private void ProductComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BrandCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {

        }

        private void BrandCheckBoxChecked(object sender, RoutedEventArgs e)
        {

        }

        private void BrandComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void StatusCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {

        }

        private void StatusCheckBoxChecked(object sender, RoutedEventArgs e)
        {

        }

        private void StatusComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnBtnClickedAdd(object sender, RoutedEventArgs e)
        {

        }

        private void OnBtnClickedView(object sender, RoutedEventArgs e)
        {

        }

        private void EmployeeCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
