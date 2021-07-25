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
    /// Interaction logic for OfficeMeetingsPage.xaml
    /// </summary>
    public partial class OfficeMeetingsPage : Page
    {
       private Employee loggedInUser;

        protected CommonQueries commonQueries;
        protected CommonFunctions commonFunctions;

        protected List<COMPANY_WORK_MACROS.OFFICE_MEETING_BASIC_STRUCT> officeMeetings;
        protected List<COMPANY_WORK_MACROS.OFFICE_MEETING_BASIC_STRUCT> filteredMeetings;

        private Grid previousSelectedCallItem;
        private Grid currentSelectedCallItem;

        public OfficeMeetingsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;

            commonQueries = new CommonQueries();
            commonFunctions = new CommonFunctions();

            officeMeetings = new List<COMPANY_WORK_MACROS.OFFICE_MEETING_BASIC_STRUCT>();
            filteredMeetings = new List<COMPANY_WORK_MACROS.OFFICE_MEETING_BASIC_STRUCT>();

            viewButton.IsEnabled = false;

            yearCombo.IsEnabled = true;
            yearCombo.SelectedIndex = DateTime.Now.Year - BASIC_MACROS.CRM_START_YEAR;

            quarterCombo.IsEnabled = false;
            employeeCombo.IsEnabled = false;

            yearCheckBox.IsChecked = true;
            employeeCheckBox.IsEnabled = false;

            InitializeYearsComboBox();
            InitializeQuartersComboBox();

            GetMeetings();
            InitializeStackPanel();
        }

        private void GetMeetings()
        {
            commonQueries.GetOfficeMeetings(ref officeMeetings);
        }

        //////////////////////////////////////////////////////////
        /// INITIALIZATION FUNCTIONS
        //////////////////////////////////////////////////////////
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
        private void InitializeStackPanel()
        {
            OfficeMeetingsStackPanel.Children.Clear();
            filteredMeetings.Clear();
            for (int i = 0; i < officeMeetings.Count; i++)
            {
                if (officeMeetings[i].meeting_caller.employee_id == loggedInUser.GetEmployeeId())
                {
                    DateTime currentCallDate = DateTime.Parse(officeMeetings[i].meeting_date);

                    if (yearCheckBox.IsChecked == true && (yearCombo.SelectedItem == null || currentCallDate.Year != int.Parse(yearCombo.SelectedItem.ToString())))
                        continue;

                    if (quarterCheckBox.IsChecked == true && (quarterCombo.SelectedItem == null || commonFunctions.GetQuarter(DateTime.Parse(officeMeetings[i].meeting_date)) != quarterCombo.SelectedIndex + 1))
                        continue;

                    filteredMeetings.Add(officeMeetings[i]);

                    StackPanel currentStackPanel = new StackPanel();
                    currentStackPanel.Orientation = Orientation.Vertical;

                    Label dateOfMeetingLabel = new Label();
                    dateOfMeetingLabel.Content = officeMeetings[i].meeting_date;
                    dateOfMeetingLabel.Style = (Style)FindResource("stackPanelItemBody");

                    Label salesPersonNameLabel = new Label();
                    salesPersonNameLabel.Content = officeMeetings[i].meeting_caller.employee_name;
                    salesPersonNameLabel.Style = (Style)FindResource("stackPanelItemBody");

                    Label purposeLabel = new Label();
                    purposeLabel.Content = officeMeetings[i].meeting_purpose.purpose_name;
                    purposeLabel.Style = (Style)FindResource("stackPanelItemBody");

                    Label lineLabel = new Label();
                    lineLabel.Content = "";
                    lineLabel.Style = (Style)FindResource("stackPanelItemBody");

                    Label newLineLabel = new Label();
                    newLineLabel.Content = "";
                    newLineLabel.Style = (Style)FindResource("stackPanelItemBody");

                    currentStackPanel.Children.Add(newLineLabel);
                    currentStackPanel.Children.Add(dateOfMeetingLabel);
                    currentStackPanel.Children.Add(salesPersonNameLabel);
                    currentStackPanel.Children.Add(purposeLabel);
                    currentStackPanel.Children.Add(lineLabel);

                    Grid newGrid = new Grid();
                    ColumnDefinition column1 = new ColumnDefinition();

                    newGrid.ColumnDefinitions.Add(column1);
                    newGrid.MouseLeftButtonDown += OnBtnClickedMeetingItem;

                    Grid.SetColumn(currentStackPanel, 0);

                    newGrid.Children.Add(currentStackPanel);
                    OfficeMeetingsStackPanel.Children.Add(newGrid);
                }
            }
        }

        //////////////////////////////////////////////////////////
        /// ON BTN CLICKED HANDLERS
        //////////////////////////////////////////////////////////
        private void OnBtnClickedMeetingItem(object sender, RoutedEventArgs e)
        {
            viewButton.IsEnabled = true;
            previousSelectedCallItem = currentSelectedCallItem;
            currentSelectedCallItem = (Grid)sender;
            BrushConverter brush = new BrushConverter();

            if (previousSelectedCallItem != null)
            {
                previousSelectedCallItem.Background = (Brush)brush.ConvertFrom("#FFFFFF");

                StackPanel previousSelectedStackPanel = (StackPanel)previousSelectedCallItem.Children[0];

                foreach (Label childLabel in previousSelectedStackPanel.Children)
                    childLabel.Foreground = (Brush)brush.ConvertFrom("#000000");
            }

            currentSelectedCallItem.Background = (Brush)brush.ConvertFrom("#105A97");

            StackPanel currentSelectedStackPanel = (StackPanel)currentSelectedCallItem.Children[0];

            foreach (Label childLabel in currentSelectedStackPanel.Children)
                childLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");

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
            OfficeMeetingsPage officeMeetingsPage = new OfficeMeetingsPage(ref loggedInUser);
            this.NavigationService.Navigate(officeMeetingsPage);
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

        private void OnBtnClickedAdd(object sender, RoutedEventArgs e)
        {
            AddOfficeMeetingWindow addOfficeMeetingWindow = new AddOfficeMeetingWindow(ref loggedInUser);
            addOfficeMeetingWindow.Closed += OnClosedAddCallWindow;
            addOfficeMeetingWindow.Show();
        }

        private void OnBtnClickedView(object sender, RoutedEventArgs e)
        {
            OfficeMeeting selectedMeeting = new OfficeMeeting();
            selectedMeeting.InitializeOfficeMeetingInfo(filteredMeetings[OfficeMeetingsStackPanel.Children.IndexOf(currentSelectedCallItem)].meeting_serial);

            ViewOfficeMeetingWindow viewOfficeMeetingWindow = new ViewOfficeMeetingWindow(ref selectedMeeting);
            viewOfficeMeetingWindow.Show();
        }

        private void OnClosedAddCallWindow(object sender, EventArgs e)
        {
            GetMeetings();
            InitializeStackPanel();
        }

        private void YearCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            yearCombo.IsEnabled = true;
        }

        private void YearCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            yearCombo.IsEnabled = false;
            yearCombo.SelectedItem = null;

            currentSelectedCallItem = null;
            previousSelectedCallItem = null;
        }

        private void YearComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitializeStackPanel();
            viewButton.IsEnabled = false;
        }

        private void QuarterCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            quarterCombo.IsEnabled = true;
        }

        private void QuarterCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            quarterCombo.IsEnabled = false;
            quarterCombo.SelectedItem = null;

            currentSelectedCallItem = null;
            previousSelectedCallItem = null;
        }

        private void QuarterComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitializeStackPanel();
            viewButton.IsEnabled = false;
        }

        private void EmployeeCheckBoxChecked(object sender, RoutedEventArgs e)
        {

        }

        private void EmployeeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void EmployeeCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
