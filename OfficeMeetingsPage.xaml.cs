using _01electronics_library;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

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

        protected List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> listOfEmployees;

        protected List<COMPANY_WORK_MACROS.OFFICE_MEETING_BASIC_STRUCT> officeMeetings;
        protected List<COMPANY_WORK_MACROS.OFFICE_MEETING_BASIC_STRUCT> filteredMeetings;

        private Grid previousSelectedCallItem;
        private Grid currentSelectedCallItem;

        private int selectedYear;
        private int selectedQuarter;
        private int selectedEmployee;

        private String errorMessage;

        public OfficeMeetingsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;

            commonQueries = new CommonQueries();
            commonFunctions = new CommonFunctions();

            listOfEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();

            officeMeetings = new List<COMPANY_WORK_MACROS.OFFICE_MEETING_BASIC_STRUCT>();
            filteredMeetings = new List<COMPANY_WORK_MACROS.OFFICE_MEETING_BASIC_STRUCT>();

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

            GetMeetings();
            InitializeStackPanel();
            InitializeGrid();
            SetDefaultSettings();
        }

        private void GetMeetings()
        {
            if (!commonQueries.GetOfficeMeetings(ref officeMeetings))
                return;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CONFIGURATION FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
            if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID || (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION && loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.MARKETING_AND_SALES_DEPARTMENT_ID))
            {
                addButton.IsEnabled = true;
            }
            else
                addButton.IsEnabled = false;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SET FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// INITIALIZATION FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
            OfficeMeetingsStackPanel.Children.Clear();
            filteredMeetings.Clear();
            for (int i = 0; i < officeMeetings.Count; i++)
            {
                DateTime currentMeetingDate = DateTime.Parse(officeMeetings[i].meeting_date);

                if (yearCheckBox.IsChecked == true && currentMeetingDate.Year != selectedYear)
                    continue;

                if (quarterCheckBox.IsChecked == true && commonFunctions.GetQuarter(currentMeetingDate) != selectedQuarter)
                    continue;

                if (employeeCheckBox.IsChecked == true && officeMeetings[i].meeting_caller.employee_id != listOfEmployees[employeeComboBox.SelectedIndex].employee_id)
                    continue;

                filteredMeetings.Add(officeMeetings[i]);

                StackPanel currentStackPanel = new StackPanel();
                currentStackPanel.Orientation = Orientation.Vertical;

                Label salesPersonNameLabel = new Label();
                salesPersonNameLabel.Content = officeMeetings[i].meeting_caller.employee_name;
                salesPersonNameLabel.Style = (Style)FindResource("stackPanelItemHeader");

                Label dateOfMeetingLabel = new Label();
                dateOfMeetingLabel.Content = officeMeetings[i].meeting_date;
                dateOfMeetingLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label purposeLabel = new Label();
                purposeLabel.Content = officeMeetings[i].meeting_purpose.purpose_name;
                purposeLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label lineLabel = new Label();
                lineLabel.Content = "";
                lineLabel.Style = (Style)FindResource("stackPanelItemBody");

                Border statusBorder = new Border();
                statusBorder.Style = (Style)FindResource("BorderIcon");

                BrushConverter brushConverter = new BrushConverter();

                Label statusLabel = new Label();
                if (officeMeetings[i].meeting_status_id == 1)
                {
                    statusLabel.Content = "Pending";
                    statusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500"));
                }
                else if (officeMeetings[i].meeting_status_id < 5)
                {
                    statusLabel.Content = "Approved";
                    statusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008000"));
                }
                else
                {
                    statusLabel.Content = "Rejected";
                    statusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                }
                statusLabel.Style = (Style)FindResource("BorderIconTextLabel");

                statusBorder.Child = statusLabel;

                currentStackPanel.Children.Add(salesPersonNameLabel);
                currentStackPanel.Children.Add(dateOfMeetingLabel);
                currentStackPanel.Children.Add(purposeLabel);

                for (int j = 0; j < officeMeetings[i].meeting_approvals_rejections.Count; j++)
                {
                    Label approvalRejectorLabel = new Label();

                    if (officeMeetings[i].meeting_status_id < 5)
                        approvalRejectorLabel.Content = "Approved by " + officeMeetings[i].meeting_approvals_rejections[j].approver.employee_name;
                    else
                        approvalRejectorLabel.Content = "Rejected by " + officeMeetings[i].meeting_approvals_rejections[j].approver.employee_name;

                    approvalRejectorLabel.Style = (Style)FindResource("stackPanelItemBody");

                    currentStackPanel.Children.Add(approvalRejectorLabel);
                }

                currentStackPanel.Children.Add(lineLabel);

                Grid newGrid = new Grid();
                newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                newGrid.ColumnDefinitions.Add(new ColumnDefinition());
                newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100) });
                newGrid.MouseLeftButtonDown += OnBtnClickMeetingItem;

                Image officeMeetingIcon = new Image { Source = new BitmapImage(new Uri(@"icons\office_meeting_icon.png", UriKind.Relative)) };
                ResizeImage(ref officeMeetingIcon, 40, 40);
                newGrid.Children.Add(officeMeetingIcon);
                Grid.SetColumn(officeMeetingIcon, 0);

                newGrid.Children.Add(currentStackPanel);
                Grid.SetColumn(currentStackPanel, 1);

                newGrid.Children.Add(statusBorder);
                Grid.SetColumn(statusBorder, 2);

                OfficeMeetingsStackPanel.Children.Add(newGrid);

            }
        }

        private void InitializeGrid()
        {
            officeMeetingsGrid.Children.Clear();
            officeMeetingsGrid.RowDefinitions.Clear();
            officeMeetingsGrid.ColumnDefinitions.Clear();

            Label salesPersonHeader = new Label();
            salesPersonHeader.Content = "Sales Person";
            salesPersonHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label dateOfMeetingHeader = new Label();
            dateOfMeetingHeader.Content = "Meeting Date";
            dateOfMeetingHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label meetingPurposeHeader = new Label();
            meetingPurposeHeader.Content = "Meeting Purpose";
            meetingPurposeHeader.Style = (Style)FindResource("tableSubHeaderItem");

            officeMeetingsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            officeMeetingsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            officeMeetingsGrid.ColumnDefinitions.Add(new ColumnDefinition());

            officeMeetingsGrid.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(salesPersonHeader, 0);
            Grid.SetColumn(salesPersonHeader, 0);
            officeMeetingsGrid.Children.Add(salesPersonHeader);

            Grid.SetRow(dateOfMeetingHeader, 0);
            Grid.SetColumn(dateOfMeetingHeader, 1);
            officeMeetingsGrid.Children.Add(dateOfMeetingHeader);

            Grid.SetRow(meetingPurposeHeader, 0);
            Grid.SetColumn(meetingPurposeHeader, 2);
            officeMeetingsGrid.Children.Add(meetingPurposeHeader);

            int currentRowNumber = 1;

            for (int i = 0; i < officeMeetings.Count; i++)
            {
                DateTime currentMeetingDate = DateTime.Parse(officeMeetings[i].meeting_date);

                if (yearCheckBox.IsChecked == true && currentMeetingDate.Year != selectedYear)
                    continue;

                if (quarterCheckBox.IsChecked == true && commonFunctions.GetQuarter(currentMeetingDate) != selectedQuarter)
                    continue;

                if (employeeCheckBox.IsChecked == true && officeMeetings[i].meeting_caller.employee_id != listOfEmployees[employeeComboBox.SelectedIndex].employee_id)
                    continue;

                filteredMeetings.Add(officeMeetings[i]);

                RowDefinition currentRow = new RowDefinition();
                officeMeetingsGrid.RowDefinitions.Add(currentRow);

                Label salesPersonLabel = new Label();
                salesPersonLabel.Content = officeMeetings[i].meeting_caller.employee_name;
                salesPersonLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(salesPersonLabel, currentRowNumber);
                Grid.SetColumn(salesPersonLabel, 0);
                officeMeetingsGrid.Children.Add(salesPersonLabel);


                Label dateOfMeeting = new Label();
                dateOfMeeting.Content = officeMeetings[i].meeting_date;
                dateOfMeeting.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(dateOfMeeting, currentRowNumber);
                Grid.SetColumn(dateOfMeeting, 1);
                officeMeetingsGrid.Children.Add(dateOfMeeting);


                Label meetingPurpose = new Label();
                meetingPurpose.Content = officeMeetings[i].meeting_purpose.purpose_name;
                meetingPurpose.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(meetingPurpose, currentRowNumber);
                Grid.SetColumn(meetingPurpose, 2);
                officeMeetingsGrid.Children.Add(meetingPurpose);


                //currentRow.MouseLeftButtonDown += OnBtnClickWorkOfferItem;

                currentRowNumber++;
            }

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //VIEWING TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnClickListView(object sender, MouseButtonEventArgs e)
        {
            listViewLabel.Style = (Style)FindResource("selectedMainTabLabelItem");
            tableViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");

            stackPanelScrollViewer.Visibility = Visibility.Visible;
            gridScrollViewer.Visibility = Visibility.Collapsed;
        }

        private void OnClickTableView(object sender, MouseButtonEventArgs e)
        {
            listViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");
            tableViewLabel.Style = (Style)FindResource("selectedMainTabLabelItem");

            stackPanelScrollViewer.Visibility = Visibility.Collapsed;
            gridScrollViewer.Visibility = Visibility.Visible;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //EXTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnButtonClickedMyProfile(object sender, MouseButtonEventArgs e)
        {
            StatisticsPage statisticsPage = new StatisticsPage(ref loggedInUser);
            NavigationService.Navigate(statisticsPage);
        }

        private void OnButtonClickedContacts(object sender, MouseButtonEventArgs e)
        {
            ContactsPage contactsPage = new ContactsPage(ref loggedInUser);
            this.NavigationService.Navigate(contactsPage);
        }
        private void OnButtonClickedProducts(object sender, MouseButtonEventArgs e)
        {
            CategoriesPage productsPage = new CategoriesPage(ref loggedInUser);
            this.NavigationService.Navigate(productsPage);
        }
        private void OnButtonClickedWorkOrders(object sender, RoutedEventArgs e)
        {
            WorkOrdersPage workOrdersPage = new WorkOrdersPage(ref loggedInUser);
            this.NavigationService.Navigate(workOrdersPage);
        }
        private void OnButtonClickedWorkOffers(object sender, RoutedEventArgs e)
        {
            QuotationsPage workOffersPage = new QuotationsPage(ref loggedInUser);
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
        }
        private void OnButtonClickedProjects(object sender, MouseButtonEventArgs e)
        {
            ProjectsPage projectsPage = new ProjectsPage(ref loggedInUser);
            this.NavigationService.Navigate(projectsPage);
        }
        private void OnButtonClickedMaintenanceOffer(object sender, MouseButtonEventArgs e)
        {
            MaintenanceOffersPage maintenanceOffersPage = new MaintenanceOffersPage(ref loggedInUser);
            this.NavigationService.Navigate(maintenanceOffersPage);
        }
        private void OnButtonClickedMaintenanceContracts(object sender, MouseButtonEventArgs e)
        {
            MaintenanceContractsPage maintenanceContractsPage = new MaintenanceContractsPage(ref loggedInUser);
            this.NavigationService.Navigate(maintenanceContractsPage);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //BTN CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnBtnClickAdd(object sender, RoutedEventArgs e)
        {
            AddOfficeMeetingWindow addOfficeMeetingWindow = new AddOfficeMeetingWindow(ref loggedInUser);
            addOfficeMeetingWindow.Closed += OnClosedAddCallWindow;
            addOfficeMeetingWindow.Show();
        }

        private void OnBtnClickView(object sender, RoutedEventArgs e)
        {
            OfficeMeeting selectedMeeting = new OfficeMeeting();
            selectedMeeting.InitializeOfficeMeetingInfo(filteredMeetings[OfficeMeetingsStackPanel.Children.IndexOf(currentSelectedCallItem)].meeting_serial);

            ViewOfficeMeetingWindow viewOfficeMeetingWindow = new ViewOfficeMeetingWindow(ref selectedMeeting);
            viewOfficeMeetingWindow.Closed += OnClosedAddCallWindow;
            viewOfficeMeetingWindow.Show();
        }

        private void OnBtnClickExport(object sender, RoutedEventArgs e)
        {
            ExcelExport excelExport = new ExcelExport(officeMeetingsGrid);
        }

        private void OnClosedAddCallWindow(object sender, EventArgs e)
        {
            GetMeetings();
            InitializeStackPanel();
            InitializeGrid();

            currentSelectedCallItem = null;
            previousSelectedCallItem = null;

            approveButton.IsEnabled = false;
            rejectButton.IsEnabled = false;
            viewButton.IsEnabled = false;
        }

        private void OnBtnClickMeetingItem(object sender, RoutedEventArgs e)
        {
            viewButton.IsEnabled = true;
            previousSelectedCallItem = currentSelectedCallItem;
            currentSelectedCallItem = (Grid)sender;
            BrushConverter brush = new BrushConverter();

            if (previousSelectedCallItem != null)
            {
                previousSelectedCallItem.Background = (Brush)brush.ConvertFrom("#FFFFFF");

                Image previousOfficeMeetingIcon = (Image)previousSelectedCallItem.Children[0];
                previousOfficeMeetingIcon.Source = new BitmapImage(new Uri(@"icons\office_meeting_icon.png", UriKind.Relative));
                ResizeImage(ref previousOfficeMeetingIcon, 40, 40);

                StackPanel previousSelectedStackPanel = (StackPanel)previousSelectedCallItem.Children[1];

                foreach (Label childLabel in previousSelectedStackPanel.Children)
                    childLabel.Foreground = (Brush)brush.ConvertFrom("#105A97");
            }

            currentSelectedCallItem.Background = (Brush)brush.ConvertFrom("#105A97");

            Image currentOfficeMeetingIcon = (Image)currentSelectedCallItem.Children[0];
            currentOfficeMeetingIcon.Source = new BitmapImage(new Uri(@"icons\office_meeting_icon_blue.png", UriKind.Relative));
            ResizeImage(ref currentOfficeMeetingIcon, 40, 40);

            StackPanel currentSelectedStackPanel = (StackPanel)currentSelectedCallItem.Children[1];

            foreach (Label childLabel in currentSelectedStackPanel.Children)
                childLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");

            if (currentSelectedCallItem != null)
            {
                viewButton.IsEnabled = true;

                if ((loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID && (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.SENIOR_POSTION || loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION)) || (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION && loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.MARKETING_AND_SALES_DEPARTMENT_ID))
                {
                    if (filteredMeetings[OfficeMeetingsStackPanel.Children.IndexOf(currentSelectedCallItem)].meeting_status_id < 5)
                    {
                        rejectButton.IsEnabled = true;
                        approveButton.IsEnabled = true;
                    }
                }
            }

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SELECTION CHANGED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnSelChangedYearCombo(object sender, SelectionChangedEventArgs e)
        {
            if (yearComboBox.SelectedItem != null)
                selectedYear = BASIC_MACROS.CRM_START_YEAR + yearComboBox.SelectedIndex;
            else
                selectedYear = 0;

            InitializeStackPanel();
            InitializeGrid();

            viewButton.IsEnabled = false;
        }
        private void OnSelChangedQuarterCombo(object sender, SelectionChangedEventArgs e)
        {
            if (quarterComboBox.SelectedItem != null)
                selectedQuarter = quarterComboBox.SelectedIndex + 1;
            else
                selectedQuarter = 0;

            InitializeStackPanel();
            InitializeGrid();
            viewButton.IsEnabled = false;
        }

        private void OnSelChangedEmployeeCombo(object sender, SelectionChangedEventArgs e)
        {
            if (employeeCheckBox.IsChecked == true)
                selectedEmployee = listOfEmployees[employeeComboBox.SelectedIndex].employee_id;
            else
                selectedEmployee = 0;

            InitializeStackPanel();
            InitializeGrid();
            viewButton.IsEnabled = false;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CHECKED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
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

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //UNCHECKED HANDLERS FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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

        public void ResizeImage(ref Image imgToResize, int width, int height)
        {
            imgToResize.Width = width;
            imgToResize.Height = height;
        }

        private void OnBtnClickApprove(object sender, RoutedEventArgs e)
        {
            HUMAN_RESOURCE_MACROS.APPROVAL_REJECTION_CLASS_STRUCT meetingApproval = new HUMAN_RESOURCE_MACROS.APPROVAL_REJECTION_CLASS_STRUCT();
            meetingApproval.approver.employee_id = loggedInUser.GetEmployeeId();
            meetingApproval.approver.employee_name = loggedInUser.GetEmployeeName();
            meetingApproval.approver.team_id = loggedInUser.GetEmployeeTeamId();
            meetingApproval.approver.department_id = loggedInUser.GetEmployeeDepartmentId();
            meetingApproval.approver.position_id = loggedInUser.GetEmployeePositionId();

            OfficeMeeting selectedMeeting = new OfficeMeeting();
            selectedMeeting.InitializeOfficeMeetingInfo(filteredMeetings[OfficeMeetingsStackPanel.Children.IndexOf(currentSelectedCallItem)].meeting_serial);

            if (!selectedMeeting.AddMeetingApproval(meetingApproval, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }

            else
            {
                GetMeetings();
                InitializeStackPanel();
                InitializeGrid();

                currentSelectedCallItem = null;
                previousSelectedCallItem = null;

                approveButton.IsEnabled = false;
                rejectButton.IsEnabled = false;
                viewButton.IsEnabled = false;
            }
        }

        private void OnBtnClickReject(object sender, RoutedEventArgs e)
        {
            HUMAN_RESOURCE_MACROS.APPROVAL_REJECTION_CLASS_STRUCT meetingApproval = new HUMAN_RESOURCE_MACROS.APPROVAL_REJECTION_CLASS_STRUCT();
            meetingApproval.approver.employee_id = loggedInUser.GetEmployeeId();
            meetingApproval.approver.employee_name = loggedInUser.GetEmployeeName();
            meetingApproval.approver.team_id = loggedInUser.GetEmployeeTeamId();
            meetingApproval.approver.department_id = loggedInUser.GetEmployeeDepartmentId();
            meetingApproval.approver.position_id = loggedInUser.GetEmployeePositionId();

            OfficeMeeting selectedMeeting = new OfficeMeeting();
            selectedMeeting.InitializeOfficeMeetingInfo(filteredMeetings[OfficeMeetingsStackPanel.Children.IndexOf(currentSelectedCallItem)].meeting_serial);

            if (!selectedMeeting.AddMeetingRejection(meetingApproval, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            else
            {
                GetMeetings();
                InitializeStackPanel();
                InitializeGrid();

                currentSelectedCallItem = null;
                previousSelectedCallItem = null;

                approveButton.IsEnabled = false;
                rejectButton.IsEnabled = false;
                viewButton.IsEnabled = false;
            }
        }
    }
}
