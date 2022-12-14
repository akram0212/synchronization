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
    /// Interaction logic for ClientVisitsPage.xaml
    /// </summary>
    public partial class ClientVisitsPage : Page
    {
        private Employee loggedInUser;

        protected CommonQueries commonQueriesObject;
        protected CommonFunctions commonFunctionsObject;

        protected List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> listOfEmployees;

        protected List<COMPANY_WORK_MACROS.CLIENT_VISIT_STRUCT> visitsInfo;
        protected List<COMPANY_WORK_MACROS.CLIENT_VISIT_STRUCT> filteredVisits;

        private Grid previousSelectedVisitItem;
        private Grid currentSelectedVisitItem;

        private int selectedYear;
        private int selectedQuarter;
        private int selectedEmployee;

        private String errorMessage;

        public ClientVisitsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();
            loggedInUser = mLoggedInUser;

            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            listOfEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();

            visitsInfo = new List<COMPANY_WORK_MACROS.CLIENT_VISIT_STRUCT>();
            filteredVisits = new List<COMPANY_WORK_MACROS.CLIENT_VISIT_STRUCT>();

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

            GetVisits();
            InitializeStackPanel();
            InitializeGrid();

            SetDefaultSettings();
        }
        private void GetVisits()
        {
            commonQueriesObject.GetClientVisits(ref visitsInfo);
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
            quarterComboBox.SelectedIndex = commonFunctionsObject.GetCurrentQuarter() - 1;
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
                quarterComboBox.Items.Add(commonFunctionsObject.GetQuarterName(i + 1));

        }
        private bool InitializeEmployeeComboBox()
        {
            if (!commonQueriesObject.GetTeamEmployees(loggedInUser.GetEmployeeTeamId(), ref listOfEmployees))
                return false;

            for (int i = 0; i < listOfEmployees.Count; i++)
                employeeComboBox.Items.Add(listOfEmployees[i].employee_name);

            return true;
        }

        private void InitializeStackPanel()
        {
            ClientVisitsStackPanel.Children.Clear();
            filteredVisits.Clear();

            for (int i = 0; i < visitsInfo.Count; i++)
            {
                DateTime currentVisitDate = DateTime.Parse(visitsInfo[i].visit_date);

                if (yearCheckBox.IsChecked == true && currentVisitDate.Year != selectedYear)
                    continue;

                if (quarterCheckBox.IsChecked == true && commonFunctionsObject.GetQuarter(currentVisitDate) != selectedQuarter)
                    continue;

                if (employeeCheckBox.IsChecked == true && visitsInfo[i].sales_person_id != listOfEmployees[employeeComboBox.SelectedIndex].employee_id)
                    continue;

                filteredVisits.Add(visitsInfo[i]);

                StackPanel currentStackPanel = new StackPanel();
                currentStackPanel.Orientation = Orientation.Vertical;

                Label salesPersonNameLabel = new Label();
                salesPersonNameLabel.Content = visitsInfo[i].sales_person_name;
                salesPersonNameLabel.Style = (Style)FindResource("stackPanelItemHeader");

                Label dateOfVisitLabel = new Label();
                dateOfVisitLabel.Content = visitsInfo[i].visit_date;
                dateOfVisitLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label companyAndContactLabel = new Label();
                companyAndContactLabel.Content = visitsInfo[i].company_name + " - " + visitsInfo[i].contact_name;
                companyAndContactLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label purposeAndResultLabel = new Label();
                purposeAndResultLabel.Content = visitsInfo[i].visit_purpose + " - " + visitsInfo[i].visit_result;
                purposeAndResultLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label lineLabel = new Label();
                lineLabel.Content = "";
                lineLabel.Style = (Style)FindResource("stackPanelItemBody");

                Border statusBorder = new Border();
                statusBorder.Style = (Style)FindResource("BorderIcon");

                BrushConverter brushConverter = new BrushConverter();

                Label statusLabel = new Label();
                if (visitsInfo[i].visit_status_id == 1)
                {
                    statusLabel.Content = "Pending";
                    statusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500"));
                }
                else if (visitsInfo[i].visit_status_id < 5)
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
                currentStackPanel.Children.Add(dateOfVisitLabel);

                currentStackPanel.Children.Add(companyAndContactLabel);
                currentStackPanel.Children.Add(purposeAndResultLabel);

                for (int j = 0; j < visitsInfo[i].visit_approvals_rejections.Count; j++)
                {
                    Label approvalRejectorLabel = new Label();
                    if (visitsInfo[i].visit_approvals_rejections.Count > 1 && visitsInfo[i].visit_status_id > 4)
                    {
                        if (j != visitsInfo[i].visit_approvals_rejections.Count - 1)
                            approvalRejectorLabel.Content = "Approved by " + visitsInfo[i].visit_approvals_rejections[j].approver.employee_name;
                        else
                            approvalRejectorLabel.Content = "Rejected by " + visitsInfo[i].visit_approvals_rejections[j].approver.employee_name;
                    }
                    else
                    {
                        if (visitsInfo[i].visit_status_id < 5)
                            approvalRejectorLabel.Content = "Approved by " + visitsInfo[i].visit_approvals_rejections[j].approver.employee_name;
                        else
                            approvalRejectorLabel.Content = "Rejected by " + visitsInfo[i].visit_approvals_rejections[j].approver.employee_name;
                    }
                    approvalRejectorLabel.Style = (Style)FindResource("stackPanelItemBody");

                    currentStackPanel.Children.Add(approvalRejectorLabel);
                }

                currentStackPanel.Children.Add(lineLabel);

                Grid newGrid = new Grid();
                newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                newGrid.ColumnDefinitions.Add(new ColumnDefinition());
                newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100) });
                newGrid.MouseLeftButtonDown += OnBtnClickVisitItem;

                Image clientVisitIcon = new Image { Source = new BitmapImage(new Uri(@"icons\client_visit_icon.png", UriKind.Relative)) };
                ResizeImage(ref clientVisitIcon, 40, 40);
                newGrid.Children.Add(clientVisitIcon);
                Grid.SetColumn(clientVisitIcon, 0);

                newGrid.Children.Add(currentStackPanel);
                Grid.SetColumn(currentStackPanel, 1);

                newGrid.Children.Add(statusBorder);
                Grid.SetColumn(statusBorder, 2);

                ClientVisitsStackPanel.Children.Add(newGrid);

            }
        }

        private bool InitializeGrid()
        {

            clientVisitsGrid.Children.Clear();
            clientVisitsGrid.RowDefinitions.Clear();
            clientVisitsGrid.ColumnDefinitions.Clear();

            filteredVisits.Clear();

            Label salesPersonHeader = new Label();
            salesPersonHeader.Content = "Sales Person";
            salesPersonHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label dateOfVisitHeader = new Label();
            dateOfVisitHeader.Content = "Visit Date";
            dateOfVisitHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label contactInfoHeader = new Label();
            contactInfoHeader.Content = "Contact Info";
            contactInfoHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label purposeHeader = new Label();
            purposeHeader.Content = "Visit Purpose";
            purposeHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label resultHeader = new Label();
            resultHeader.Content = "Visit Result";
            resultHeader.Style = (Style)FindResource("tableSubHeaderItem");

            clientVisitsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            clientVisitsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            clientVisitsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            clientVisitsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            clientVisitsGrid.ColumnDefinitions.Add(new ColumnDefinition());

            clientVisitsGrid.RowDefinitions.Add(new RowDefinition());



            Grid.SetRow(salesPersonHeader, 0);
            Grid.SetColumn(salesPersonHeader, 0);
            clientVisitsGrid.Children.Add(salesPersonHeader);

            Grid.SetRow(dateOfVisitHeader, 0);
            Grid.SetColumn(dateOfVisitHeader, 1);
            clientVisitsGrid.Children.Add(dateOfVisitHeader);

            Grid.SetRow(contactInfoHeader, 0);
            Grid.SetColumn(contactInfoHeader, 2);
            clientVisitsGrid.Children.Add(contactInfoHeader);

            Grid.SetRow(purposeHeader, 0);
            Grid.SetColumn(purposeHeader, 3);
            clientVisitsGrid.Children.Add(purposeHeader);

            Grid.SetRow(resultHeader, 0);
            Grid.SetColumn(resultHeader, 4);
            clientVisitsGrid.Children.Add(resultHeader);

            int currentRowNumber = 1;

            for (int i = 0; i < visitsInfo.Count; i++)
            {
                DateTime currentVisitDate = DateTime.Parse(visitsInfo[i].visit_date);

                if (yearCheckBox.IsChecked == true && currentVisitDate.Year != selectedYear)
                    continue;

                if (quarterCheckBox.IsChecked == true && commonFunctionsObject.GetQuarter(currentVisitDate) != selectedQuarter)
                    continue;

                if (employeeCheckBox.IsChecked == true && visitsInfo[i].sales_person_id != listOfEmployees[employeeComboBox.SelectedIndex].employee_id)
                    continue;

                filteredVisits.Add(visitsInfo[i]);


                RowDefinition currentRow = new RowDefinition();

                clientVisitsGrid.RowDefinitions.Add(currentRow);

                Label salesPersonLabel = new Label();
                salesPersonLabel.Content = visitsInfo[i].sales_person_name;
                salesPersonLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(salesPersonLabel, currentRowNumber);
                Grid.SetColumn(salesPersonLabel, 0);
                clientVisitsGrid.Children.Add(salesPersonLabel);


                Label dateOfVisitLabel = new Label();
                dateOfVisitLabel.Content = visitsInfo[i].visit_date;
                dateOfVisitLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(dateOfVisitLabel, currentRowNumber);
                Grid.SetColumn(dateOfVisitLabel, 1);
                clientVisitsGrid.Children.Add(dateOfVisitLabel);


                Label contactInfoLabel = new Label();
                contactInfoLabel.Content = visitsInfo[i].company_name + " - " + visitsInfo[i].contact_name;
                contactInfoLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(contactInfoLabel, currentRowNumber);
                Grid.SetColumn(contactInfoLabel, 2);
                clientVisitsGrid.Children.Add(contactInfoLabel);


                Label purposeLabel = new Label();
                purposeLabel.Content = visitsInfo[i].visit_purpose;
                purposeLabel.Style = (Style)FindResource("tableSubItemLabel");

                clientVisitsGrid.Children.Add(purposeLabel);
                Grid.SetRow(purposeLabel, currentRowNumber);
                Grid.SetColumn(purposeLabel, 3);

                Label resultLabel = new Label();
                resultLabel.Content = visitsInfo[i].visit_result;
                resultLabel.Style = (Style)FindResource("tableSubItemLabel");

                clientVisitsGrid.Children.Add(resultLabel);
                Grid.SetRow(resultLabel, currentRowNumber);
                Grid.SetColumn(resultLabel, 4);

                //currentRow.MouseLeftButtonDown += OnBtnClickWorkOfferItem;

                currentRowNumber++;
            }

            return true;
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
        /// ON BTN CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnBtnClickVisitItem(object sender, RoutedEventArgs e)
        {
            viewButton.IsEnabled = true;
            previousSelectedVisitItem = currentSelectedVisitItem;
            currentSelectedVisitItem = (Grid)sender;
            BrushConverter brush = new BrushConverter();

            if (previousSelectedVisitItem != null)
            {
                previousSelectedVisitItem.Background = (Brush)brush.ConvertFrom("#FFFFFF");

                Image previousClientVistIcon = (Image)previousSelectedVisitItem.Children[0];
                previousClientVistIcon.Source = new BitmapImage(new Uri(@"icons\client_visit_icon.png", UriKind.Relative));
                ResizeImage(ref previousClientVistIcon, 40, 40);

                StackPanel previousSelectedStackPanel = (StackPanel)previousSelectedVisitItem.Children[1];

                foreach (Label childLabel in previousSelectedStackPanel.Children)
                    childLabel.Foreground = (Brush)brush.ConvertFrom("#105A97");

            }

            currentSelectedVisitItem.Background = (Brush)brush.ConvertFrom("#105A97");

            Image currentClientVistIcon = (Image)currentSelectedVisitItem.Children[0];
            currentClientVistIcon.Source = new BitmapImage(new Uri(@"icons\client_visit_icon_blue.png", UriKind.Relative));
            ResizeImage(ref currentClientVistIcon, 40, 40);

            StackPanel currentSelectedStackPanel = (StackPanel)currentSelectedVisitItem.Children[1];

            foreach (Label childLabel in currentSelectedStackPanel.Children)
                childLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");

            if (currentSelectedVisitItem != null)
            {
                viewButton.IsEnabled = true;

                if ((loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID && (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.SENIOR_POSTION || loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION)) || (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION && loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.MARKETING_AND_SALES_DEPARTMENT_ID))
                {
                    if (filteredVisits[ClientVisitsStackPanel.Children.IndexOf(currentSelectedVisitItem)].visit_status_id < 5)
                    {
                        rejectButton.IsEnabled = true;
                        approveButton.IsEnabled = true;
                    }
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //EXTERNAL TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnButtonClickedContacts(object sender, RoutedEventArgs e)
        {
            ContactsPage contacts = new ContactsPage(ref loggedInUser);
            this.NavigationService.Navigate(contacts);
        }
        private void OnButtonClickedProducts(object sender, MouseButtonEventArgs e)
        {
            CategoriesPage productsPage = new CategoriesPage(ref loggedInUser);
            this.NavigationService.Navigate(productsPage);
        }
        private void OnButtonClickedWorkOrders(object sender, RoutedEventArgs e)
        {
            WorkOrdersPage workOrders = new WorkOrdersPage(ref loggedInUser);
            this.NavigationService.Navigate(workOrders);
        }
        private void OnButtonClickedWorkOffers(object sender, RoutedEventArgs e)
        {
            QuotationsPage workOffers = new QuotationsPage(ref loggedInUser);
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
        private void OnButtonClickedMaintenanceOffer(object sender, MouseButtonEventArgs e)
        {
            MaintenanceOffersPage maintenanceOffersPage = new MaintenanceOffersPage(ref loggedInUser);
            this.NavigationService.Navigate(maintenanceOffersPage);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //BTN CLICKED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnBtnClickAdd(object sender, RoutedEventArgs e)
        {
            AddClientVisitWindow addClientVisitWindow = new AddClientVisitWindow(ref loggedInUser);
            addClientVisitWindow.Closed += OnClosedAddVisitWindow;
            addClientVisitWindow.Show();
        }
        private void OnBtnClickView(object sender, RoutedEventArgs e)
        {
            ClientVisit selectedVisit = new ClientVisit();
            selectedVisit.InitializeClientVisitInfo(filteredVisits[ClientVisitsStackPanel.Children.IndexOf(currentSelectedVisitItem)].visit_serial, filteredVisits[ClientVisitsStackPanel.Children.IndexOf(currentSelectedVisitItem)].sales_person_id);

            ViewClientVisitWindow viewClientVisitWindow = new ViewClientVisitWindow(ref selectedVisit);
            viewClientVisitWindow.Closed += OnClosedAddVisitWindow;
            viewClientVisitWindow.Show();
        }

        private void OnBtnClickExport(object sender, RoutedEventArgs e)
        {
            ExcelExport excelExport = new ExcelExport(clientVisitsGrid);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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

        private void OnSelChangedEmployeeCombo(object sender, RoutedEventArgs e)
        {
            if (employeeCheckBox.IsChecked == true)
                selectedEmployee = listOfEmployees[employeeComboBox.SelectedIndex].employee_id;
            else
                selectedEmployee = 0;

            InitializeStackPanel();
            InitializeGrid();
            viewButton.IsEnabled = false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON CHECK HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON UNCHECK HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnUncheckYearCheckBox(object sender, RoutedEventArgs e)
        {
            yearComboBox.IsEnabled = false;
            yearComboBox.SelectedItem = null;

            currentSelectedVisitItem = null;
            previousSelectedVisitItem = null;
        }
        private void OnUncheckQuarterCheckBox(object sender, RoutedEventArgs e)
        {
            quarterComboBox.IsEnabled = false;
            quarterComboBox.SelectedItem = null;

            currentSelectedVisitItem = null;
            previousSelectedVisitItem = null;
        }
        private void OnUncheckEmployeeCheckBox(object sender, RoutedEventArgs e)
        {
            employeeComboBox.IsEnabled = false;
            employeeComboBox.SelectedItem = null;

            currentSelectedVisitItem = null;
            previousSelectedVisitItem = null;
        }

        private void OnClosedAddVisitWindow(object sender, EventArgs e)
        {
            GetVisits();
            InitializeStackPanel();
            InitializeGrid();

            currentSelectedVisitItem = null;
            previousSelectedVisitItem = null;

            approveButton.IsEnabled = false;
            rejectButton.IsEnabled = false;
            viewButton.IsEnabled = false;
        }

        private void OnButtonClickedMaintenanceContracts(object sender, MouseButtonEventArgs e)
        {
            MaintenanceContractsPage maintenanceContractsPage = new MaintenanceContractsPage(ref loggedInUser);
            this.NavigationService.Navigate(maintenanceContractsPage);
        }






        public void ResizeImage(ref Image imgToResize, int width, int height)
        {
            imgToResize.Width = width;
            imgToResize.Height = height;
        }

        private void OnButtonClickedMyProfile(object sender, MouseButtonEventArgs e)
        {

            StatisticsPage statisticsPage = new StatisticsPage(ref loggedInUser);
            NavigationService.Navigate(statisticsPage);
        }

        private void OnBtnClickApprove(object sender, RoutedEventArgs e)
        {
            ClientVisit selectedVisit = new ClientVisit();
            selectedVisit.InitializeClientVisitInfo(filteredVisits[ClientVisitsStackPanel.Children.IndexOf(currentSelectedVisitItem)].visit_serial, filteredVisits[ClientVisitsStackPanel.Children.IndexOf(currentSelectedVisitItem)].sales_person_id);

            HUMAN_RESOURCE_MACROS.APPROVAL_REJECTION_CLASS_STRUCT approval = new HUMAN_RESOURCE_MACROS.APPROVAL_REJECTION_CLASS_STRUCT();
            approval.approver.employee_id = loggedInUser.GetEmployeeId();
            approval.approver.department_id = loggedInUser.GetEmployeeDepartmentId();
            approval.approver.team_id = loggedInUser.GetEmployeeTeamId();
            approval.approver.position_id = loggedInUser.GetEmployeePositionId();

            if (!selectedVisit.AddVisitApproval(approval, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            else
            {
                GetVisits();
                InitializeStackPanel();
                InitializeGrid();

                currentSelectedVisitItem = null;
                previousSelectedVisitItem = null;

                approveButton.IsEnabled = false;
                rejectButton.IsEnabled = false;
                viewButton.IsEnabled = false;
            }
        }

        private void OnBtnClickReject(object sender, RoutedEventArgs e)
        {
            ClientVisit selectedVisit = new ClientVisit();
            selectedVisit.InitializeClientVisitInfo(filteredVisits[ClientVisitsStackPanel.Children.IndexOf(currentSelectedVisitItem)].visit_serial, filteredVisits[ClientVisitsStackPanel.Children.IndexOf(currentSelectedVisitItem)].sales_person_id);

            HUMAN_RESOURCE_MACROS.APPROVAL_REJECTION_CLASS_STRUCT approval = new HUMAN_RESOURCE_MACROS.APPROVAL_REJECTION_CLASS_STRUCT();
            approval.approver.employee_id = loggedInUser.GetEmployeeId();
            approval.approver.department_id = loggedInUser.GetEmployeeDepartmentId();
            approval.approver.team_id = loggedInUser.GetEmployeeTeamId();
            approval.approver.position_id = loggedInUser.GetEmployeePositionId();

            if (!selectedVisit.AddVisitRejection(approval, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            else
            {
                GetVisits();
                InitializeStackPanel();
                InitializeGrid();

                currentSelectedVisitItem = null;
                previousSelectedVisitItem = null;

                approveButton.IsEnabled = false;
                rejectButton.IsEnabled = false;
                viewButton.IsEnabled = false;
            }
        }
    }
}
