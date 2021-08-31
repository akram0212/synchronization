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
    /// Interaction logic for ClientCallsPage.xaml
    /// </summary>
    public partial class ClientCallsPage : Page
    {
        private Employee loggedInUser;

        protected CommonQueries commonQueries;
        protected CommonFunctions commonFunctions;

        protected List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> listOfEmployees;

        protected List<COMPANY_WORK_MACROS.CLIENT_CALL_STRUCT> clientCalls;
        protected List<COMPANY_WORK_MACROS.CLIENT_CALL_STRUCT> filteredCalls;

        private Grid previousSelectedCallItem;
        private Grid currentSelectedCallItem;

        private int selectedYear;
        private int selectedQuarter;
        private int selectedEmployee;
        public ClientCallsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;

            commonQueries = new CommonQueries();
            commonFunctions = new CommonFunctions();

            listOfEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();

            clientCalls = new List<COMPANY_WORK_MACROS.CLIENT_CALL_STRUCT>();
            filteredCalls = new List<COMPANY_WORK_MACROS.CLIENT_CALL_STRUCT>();

            viewButton.IsEnabled = false;

            yearComboBox.IsEnabled = true;
            yearComboBox.SelectedIndex = DateTime.Now.Year - BASIC_MACROS.CRM_START_YEAR;

            quarterComboBox.IsEnabled = false;
            employeeComboBox.IsEnabled = false;

            yearCheckBox.IsChecked = true;
            employeeCheckBox.IsEnabled = false;

            InitializeYearsComboBox();
            InitializeQuartersComboBox();

            if (!InitializeEmployeeComboBox())
                return;

            GetCalls();
            InitializeStackPanel();
            SetDefaultSettings();
        }
        private void GetCalls()
        {
            commonQueries.GetClientCalls(ref clientCalls);
        }

        /////////////////////////////////////////////////////////////////
        //CONFIGURATION FUNCTIONS
        /////////////////////////////////////////////////////////////////
        ///
        private void SetDefaultSettings()
        {
            yearCheckBox.IsChecked = true;

            yearCheckBox.IsEnabled = false;

            if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION || loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION || loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.SENIOR_POSTION)
            {
                employeeCheckBox.IsChecked = false;
                employeeCheckBox.IsEnabled = true;
                employeeComboBox.IsEnabled = false;
            }
            else
            {
                employeeCheckBox.IsChecked = true;
                employeeCheckBox.IsEnabled = false;
                employeeComboBox.IsEnabled = false;
            }
        }

        /////////////////////////////////////////////////////////////////
        //SET FUNCTIONS
        /////////////////////////////////////////////////////////////////
        private void SetYearComboBox()
        {
            yearComboBox.SelectedIndex = DateTime.Now.Year - BASIC_MACROS.CRM_START_YEAR;
        }
        private void SetQuarterComboBox()
        {
            quarterComboBox.SelectedIndex = commonFunctions.GetCurrentQuarter() - 1;
        }
        private void SetEmployeeComboBox()
        {
            employeeComboBox.SelectedIndex = 0;

            for (int i = 0; i < listOfEmployees.Count; i++)
                if (loggedInUser.GetEmployeeId() == listOfEmployees[i].employee_id)
                    employeeComboBox.SelectedIndex = i;
        }

        //////////////////////////////////////////////////////////
        /// INITIALIZATION FUNCTIONS
        //////////////////////////////////////////////////////////
        private void InitializeYearsComboBox()
        {
            for (int year = BASIC_MACROS.CRM_START_YEAR; year <= DateTime.Now.Year; year++)
                yearComboBox.Items.Add(year);

        }
        private void InitializeQuartersComboBox()
        {
            for (int i = 0; i < BASIC_MACROS.NO_OF_QUARTERS; i++)
                quarterComboBox.Items.Add(commonFunctions.GetQuarterName(i + 1));

        }
        private bool InitializeEmployeeComboBox()
        {
            if (!commonQueries.GetTeamEmployees(loggedInUser.GetEmployeeTeamId(), ref listOfEmployees))
                return false;

            for (int i = 0; i < listOfEmployees.Count; i++)
                employeeComboBox.Items.Add(listOfEmployees[i].employee_name);

            return true;
        }
        private void InitializeStackPanel()
        {
            ClientCallsStackPanel.Children.Clear();
            filteredCalls.Clear();
            for (int i = 0; i < clientCalls.Count; i++)
            {
                DateTime currentCallDate = DateTime.Parse(clientCalls[i].call_date);

                if (yearCheckBox.IsChecked == true && currentCallDate.Year != selectedYear)
                    continue;

                if (quarterCheckBox.IsChecked == true && commonFunctions.GetQuarter(currentCallDate) != selectedQuarter)
                    continue;

                if (employeeCheckBox.IsChecked == true && clientCalls[i].sales_person_id != listOfEmployees[employeeComboBox.SelectedIndex].employee_id)
                    continue;

                filteredCalls.Add(clientCalls[i]);

                StackPanel currentStackPanel = new StackPanel();
                currentStackPanel.Orientation = Orientation.Vertical;

                Label salesPersonNameLabel = new Label();
                salesPersonNameLabel.Content = clientCalls[i].sales_person_name;
                salesPersonNameLabel.Style = (Style)FindResource("stackPanelItemHeader");

                Label dateOfCallLabel = new Label();
                dateOfCallLabel.Content = clientCalls[i].call_date;
                dateOfCallLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label companyAndContactLabel = new Label();
                companyAndContactLabel.Content = clientCalls[i].company_name + " - " + clientCalls[i].contact_name;
                companyAndContactLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label purposeAndResultLabel = new Label();
                purposeAndResultLabel.Content = clientCalls[i].call_purpose + " - " + clientCalls[i].call_result;
                purposeAndResultLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label lineLabel = new Label();
                lineLabel.Content = "";
                lineLabel.Style = (Style)FindResource("stackPanelItemBody");

                currentStackPanel.Children.Add(salesPersonNameLabel);
                currentStackPanel.Children.Add(dateOfCallLabel);
                currentStackPanel.Children.Add(companyAndContactLabel);
                currentStackPanel.Children.Add(purposeAndResultLabel);
                currentStackPanel.Children.Add(lineLabel);

                Grid newGrid = new Grid();
                ColumnDefinition column1 = new ColumnDefinition();

                newGrid.ColumnDefinitions.Add(column1);
                newGrid.MouseLeftButtonDown += OnBtnClickedCallItem;

                Grid.SetColumn(currentStackPanel, 0);

                newGrid.Children.Add(currentStackPanel);
                ClientCallsStackPanel.Children.Add(newGrid);
            }
        }

        //////////////////////////////////////////////////////////
        /// ON BTN CLICKED HANDLERS
        //////////////////////////////////////////////////////////
        private void OnBtnClickedCallItem(object sender, RoutedEventArgs e)
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
        private void OnClosedAddCallWindow(object sender, EventArgs e)
        {
            GetCalls();
            InitializeStackPanel();
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

        /////////////////////////////////////////////////////////////////
        //BTN CLICKED HANDLERS
        /////////////////////////////////////////////////////////////////
        private void OnBtnClickedAdd(object sender, RoutedEventArgs e)
        {
            AddClientCallWindow addClientCallWindow = new AddClientCallWindow(ref loggedInUser);
            addClientCallWindow.Closed += OnClosedAddCallWindow;
            addClientCallWindow.Show();
        }
        private void OnBtnClickedView(object sender, RoutedEventArgs e)
        {
            ClientCall selectedCall = new ClientCall();
            selectedCall.InitializeClientCallInfo(filteredCalls[ClientCallsStackPanel.Children.IndexOf(currentSelectedCallItem)].call_serial, filteredCalls[ClientCallsStackPanel.Children.IndexOf(currentSelectedCallItem)].sales_person_id);

            ViewClientCallWindow viewClientCallWindow = new ViewClientCallWindow(ref selectedCall);
            viewClientCallWindow.Show();
        }


        //////////////////////////////////////////////////////////
        /// ON SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////
        private void OnSelChangedYearCombo(object sender, SelectionChangedEventArgs e)
        {
            if (yearComboBox.SelectedItem != null)
                selectedYear = BASIC_MACROS.CRM_START_YEAR + yearComboBox.SelectedIndex;
            else
                selectedYear = 0;

            InitializeStackPanel();
            viewButton.IsEnabled = false;
        }
        private void OnSelChangedQuarterCombo(object sender, SelectionChangedEventArgs e)
        {
            if (quarterComboBox.SelectedItem != null)
                selectedQuarter = quarterComboBox.SelectedIndex + 1;
            else
                selectedQuarter = 0;

            InitializeStackPanel();
            viewButton.IsEnabled = false;
        }

        private void OnSelChangedEmployeeCombo(object sender, SelectionChangedEventArgs e)
        {
            if (employeeCheckBox.IsChecked == true)
                selectedEmployee = listOfEmployees[employeeComboBox.SelectedIndex].employee_id;
            else
                selectedEmployee = 0;

            InitializeStackPanel();
            viewButton.IsEnabled = false;
        }

        //////////////////////////////////////////////////////////
        /// ON CHECK HANDLERS
        //////////////////////////////////////////////////////////
        private void OnCheckYearCheckBox(object sender, RoutedEventArgs e)
        {
            yearComboBox.IsEnabled = true;

            SetYearComboBox();
        }
        private void OnCheckQuarterCheckBox(object sender, RoutedEventArgs e)
        {
            quarterComboBox.IsEnabled = true;

            SetQuarterComboBox();
        }
        private void OnCheckEmployeeCheckBox(object sender, RoutedEventArgs e)
        {
            employeeComboBox.IsEnabled = true;

            SetEmployeeComboBox();
        }

        //////////////////////////////////////////////////////////
        /// ON UNCHECK HANDLERS
        //////////////////////////////////////////////////////////
        private void OnUncheckYearCheckBox(object sender, RoutedEventArgs e)
        {
            yearComboBox.IsEnabled = false;
            yearComboBox.SelectedItem = null;

            currentSelectedCallItem = null;
            previousSelectedCallItem = null;
        }
        private void OnUncheckQuarterCheckBox(object sender, RoutedEventArgs e)
        {
            quarterComboBox.IsEnabled = false;
            quarterComboBox.SelectedItem = null;

            currentSelectedCallItem = null;
            previousSelectedCallItem = null;
        }
        private void OnUncheckEmployeeCheckBox(object sender, RoutedEventArgs e)
        {
            employeeComboBox.IsEnabled = false;
            employeeComboBox.SelectedItem = null;

            currentSelectedCallItem = null;
            previousSelectedCallItem = null;
        }
    }
}
