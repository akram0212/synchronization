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
    /// Interaction logic for ClientVisitsPage.xaml
    /// </summary>
    public partial class ClientVisitsPage : Page
    {
        private Employee loggedInUser;
        protected CommonQueries commonQueries;
        protected CommonFunctions commonFunctions;
        protected List<COMPANY_WORK_MACROS.CLIENT_VISIT_STRUCT> visitsInfo;
        private Grid previousSelectedVisitItem;
        private Grid currentSelectedVisitItem;
        public ClientVisitsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();
            loggedInUser = mLoggedInUser;

            commonQueries = new CommonQueries();
            commonFunctions = new CommonFunctions();
            visitsInfo = new List<COMPANY_WORK_MACROS.CLIENT_VISIT_STRUCT>();

            viewButton.IsEnabled = false;

            yearCombo.IsEnabled = true;
            yearCombo.SelectedIndex = DateTime.Now.Year - BASIC_MACROS.CRM_START_YEAR;

            quarterCombo.IsEnabled = false;
            employeeCombo.IsEnabled = false;

            yearCheckBox.IsChecked = true;
            employeeCheckBox.IsEnabled = false;

            InitializeYearsComboBox();
            InitializeQuartersComboBox();
            GetVisits();
            InitializeStackPanel();
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
        private void GetVisits()
        {
            commonQueries.QueryGetClientVisits(ref visitsInfo);
        }
        private void InitializeStackPanel()
        {
            ClientVisitsStackPanel.Children.Clear();

            for (int i = 0; i < visitsInfo.Count; i++)
            {
                if (visitsInfo[i].sales_person_id == loggedInUser.GetEmployeeId())
                {
                    DateTime currentVisitDate = DateTime.Parse(visitsInfo[i].visit_date);

                    if (yearCheckBox.IsChecked == true && (yearCombo.SelectedItem == null || currentVisitDate.Year != int.Parse(yearCombo.SelectedItem.ToString())))
                        continue;

                    if (quarterCheckBox.IsChecked == true && (quarterCombo.SelectedItem == null || commonFunctions.GetQuarter(DateTime.Parse(visitsInfo[i].visit_date)) != quarterCombo.SelectedIndex + 1))
                        continue;

                    StackPanel currentStackPanel = new StackPanel();
                    currentStackPanel.Orientation = Orientation.Vertical;

                    Label dateOfVisitLabel = new Label();
                    dateOfVisitLabel.Content = visitsInfo[i].visit_date;
                    dateOfVisitLabel.Style = (Style)FindResource("stackPanelItemBody");

                    Label salesPersonNameLabel = new Label();
                    salesPersonNameLabel.Content = visitsInfo[i].sales_person_name;
                    salesPersonNameLabel.Style = (Style)FindResource("stackPanelItemBody");

                    Label companyAndContactLabel = new Label();
                    companyAndContactLabel.Content = visitsInfo[i].company_name + " - " + visitsInfo[i].contact_name;
                    companyAndContactLabel.Style = (Style)FindResource("stackPanelItemBody");

                    Label purposeAndResultLabel = new Label();
                    purposeAndResultLabel.Content = visitsInfo[i].visit_purpose + " - " + visitsInfo[i].visit_result;
                    purposeAndResultLabel.Style = (Style)FindResource("stackPanelItemBody");

                    Label newLineLabel = new Label();
                    newLineLabel.Content = "";
                    newLineLabel.Style = (Style)FindResource("stackPanelItemBody");

                    currentStackPanel.Children.Add(newLineLabel);
                    currentStackPanel.Children.Add(dateOfVisitLabel);
                    currentStackPanel.Children.Add(salesPersonNameLabel);
                    currentStackPanel.Children.Add(companyAndContactLabel);
                    currentStackPanel.Children.Add(purposeAndResultLabel);

                    Grid newGrid = new Grid();
                    ColumnDefinition column1 = new ColumnDefinition();
                    newGrid.ColumnDefinitions.Add(column1);
                    newGrid.MouseLeftButtonDown += OnBtnClickedVisitItem;
                    Grid.SetColumn(currentStackPanel, 0);

                    newGrid.Children.Add(currentStackPanel);
                    ClientVisitsStackPanel.Children.Add(newGrid);
                }
            }
        }
        private void OnBtnClickedVisitItem(object sender, RoutedEventArgs e)
        {
            viewButton.IsEnabled = true;
            previousSelectedVisitItem = currentSelectedVisitItem;
            currentSelectedVisitItem = (Grid)sender;
            BrushConverter brush = new BrushConverter();

            if (previousSelectedVisitItem != null)
            {
                previousSelectedVisitItem.Background = (Brush)brush.ConvertFrom("#FFFFFF");

                StackPanel previousSelectedStackPanel = (StackPanel)previousSelectedVisitItem.Children[0];
                Border previousSelectedBorder = (Border)previousSelectedVisitItem.Children[1];
                Label previousStatusLabel = (Label)previousSelectedBorder.Child;

                foreach (Label childLabel in previousSelectedStackPanel.Children)
                    childLabel.Foreground = (Brush)brush.ConvertFrom("#000000");
                
                previousStatusLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");
            }

            currentSelectedVisitItem.Background = (Brush)brush.ConvertFrom("#105A97");

            StackPanel currentSelectedStackPanel = (StackPanel)currentSelectedVisitItem.Children[0];
            Border currentSelectedBorder = (Border)currentSelectedVisitItem.Children[1];
            Label currentStatusLabel = (Label)currentSelectedBorder.Child;

            foreach (Label childLabel in currentSelectedStackPanel.Children)
                childLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");

            currentSelectedBorder.Background = (Brush)brush.ConvertFrom("#FFFFFF");
            currentStatusLabel.Foreground = (Brush)brush.ConvertFrom("#105A97");
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
            yearCombo.IsEnabled = true;
        }

        private void YearCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            yearCombo.IsEnabled = false;
        }

        private void YearComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitializeStackPanel();
        }

        private void QuarterCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            quarterCombo.IsEnabled = true;
        }

        private void QuarterCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            quarterCombo.IsEnabled = false;
        }

        private void QuarterComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitializeStackPanel();
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
