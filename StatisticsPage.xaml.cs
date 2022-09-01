
﻿using System;
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
using LiveCharts;
using LiveCharts.Wpf;
using _01electronics_library;
using System.Globalization;

namespace _01electronics_crm
{
    
    
    public partial class StatisticsPage : Page
    {

        private Employee loggedInUser;

        private CommonQueries commonQueries;
        private CommonFunctions commonFunctions;

        private SalesAnalytics salesAnalytics;

        private SQLServer sqlServer;

        private int countAmountComboSelectedIndex;
        private int summationVariable;

        private DateTime startDate;
        private DateTime endDate;

        CultureInfo cultureInfo;

        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> employees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();

        private bool initializationComplete;

        public StatisticsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();
            sqlServer = new SQLServer();
            loggedInUser = mLoggedInUser;
            commonQueries = new CommonQueries(sqlServer);
            commonFunctions = new CommonFunctions();

            initializationComplete = false;

            cultureInfo = new CultureInfo("en-US");

            InitializeDatePickers();

            salesAnalytics = new SalesAnalytics(sqlServer, startDatePicker.SelectedDate.Value, endDatePicker.SelectedDate.Value);
            InitializeEmployeesList();
            InitializeComboBoxes();
            
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //INITIALIZE FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        private void InitializeDatePickers()
        {
            endDate = commonFunctions.GetTodaysDate();
            startDatePicker.SelectedDate = DateTime.Parse("1/1/" + endDate.Year.ToString());
            endDatePicker.SelectedDate = endDate;
        }

        public bool InitializeEmployeesList()
        {
            employees.Clear();

            if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)
            {
                if (!commonQueries.GetDepartmentEmployees(COMPANY_ORGANISATION_MACROS.MARKETING_AND_SALES_DEPARTMENT_ID, ref employees))
                    return false;

                COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT tempEmployee = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT();
                tempEmployee.employee_id = loggedInUser.GetEmployeeId();
                tempEmployee.employee_name = loggedInUser.GetEmployeeName();

                employees.Add(tempEmployee);
            }
            else if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION)
            {
                if (!commonQueries.GetTeamEmployees(loggedInUser.GetEmployeeTeamId(), ref employees))
                    return false;
            }
            else
            {
                COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT tempEmployee = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT();
                tempEmployee.employee_id = loggedInUser.GetEmployeeId();
                tempEmployee.employee_name = loggedInUser.GetEmployeeName();
                
                employees.Add(tempEmployee);
            }

            return true;
        }
        private bool InitializeComboBoxes()
        {
            countAmountComboBox.Items.Clear();
            countAmountComboBox.Items.Add("Count");
            countAmountComboBox.Items.Add("Amount");

            countAmountComboBox.SelectedIndex = 0;

            employeeComboBox.Items.Clear();

            for (int i = 0; i < employees.Count; i++)
            {
                employeeComboBox.Items.Add(employees[i].employee_name);
            }

            if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION || loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION)
            {
                employeeComboBox.Items.Add("All");
            }

            if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION || loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION)
            {
                initializationComplete = true;
                employeeComboBox.SelectedIndex = employeeComboBox.Items.Count - 1;
            }
            else
            {
                employeeComboBox.IsEnabled = false;
                initializationComplete = true;
                employeeComboBox.SelectedIndex = 0;
            }
            return true;
        }

        private void InitializePieChart(Grid currentGrid ,PieChart mPieChart, List<SalesAnalyticsStructs.REFRESH_PIECHART_STRUCT> mRefreshList, bool isModel)
        {
            EmptyGrid(currentGrid);
            mRefreshList.Sort((s1, s2) => s2.value.CompareTo(s1.value));

            for(int i = 0; i < mRefreshList.Count; i++)
            {
                currentGrid.RowDefinitions.Add(new RowDefinition());

                Label salesLabel = new Label();
                if (isModel == true)
                    salesLabel.Width = 350.00;
                else
                    salesLabel.Width = 150;
                salesLabel.Height = 30.00;
                salesLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                salesLabel.Content = mRefreshList[i].name;
                currentGrid.Children.Add(salesLabel);
                Grid.SetRow(salesLabel, i);
                Grid.SetColumn(salesLabel, 0);

                Label valueLabel = new Label();
                valueLabel.Width = 150.00;
                valueLabel.Height = 30.00;
                valueLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                if (countAmountComboBox.SelectedIndex == 1)
                {
                    string temp = "EGP " + mRefreshList[i].value.ToString("C", cultureInfo).Split('$')[1];
                    valueLabel.Content = temp;
                }
                else
                    valueLabel.Content = mRefreshList[i].value.ToString();
                currentGrid.Children.Add(valueLabel);
                Grid.SetRow(valueLabel, i);
                Grid.SetColumn(valueLabel, 1);

                mPieChart.Series.Add(new PieSeries { Title = mRefreshList[i].name, StrokeThickness = 0, Values = new ChartValues<Decimal> { mRefreshList[i].value } });
            }

            currentGrid.RowDefinitions.Add(new RowDefinition());

            Label totalLabel = new Label();
            if (isModel == true)
                totalLabel.Width = 350.00;
            else
                totalLabel.Width = 150;
            totalLabel.Height = 30.00;
            totalLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
            totalLabel.Content = "Total";
            Grid.SetRow(totalLabel, currentGrid.RowDefinitions.Count - 1);
            Grid.SetColumn(totalLabel, 0);
            currentGrid.Children.Add(totalLabel);

            Label totalValueLabel = new Label();
            totalValueLabel.Width = 150.00;
            totalValueLabel.Height = 30.00;
            totalValueLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
            if (countAmountComboBox.SelectedIndex == 1)
            {
                string temp = "EGP " + summationVariable.ToString("C", cultureInfo).Split('$')[1];
                totalValueLabel.Content = temp;
            }
            else
                totalValueLabel.Content = summationVariable.ToString();
            currentGrid.Children.Add(totalValueLabel);
            Grid.SetRow(totalValueLabel, currentGrid.RowDefinitions.Count - 1);
            Grid.SetColumn(totalValueLabel, 1);
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SET FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void EmptyGrid(Grid currentGrid)
        {
            for (int i = currentGrid.Children.Count - 1; i >= 0 ; i--)
            {
                Label currentLabel = (Label)currentGrid.Children[i];
                currentGrid.Children.Remove(currentLabel);
            }

            for (int i = currentGrid.RowDefinitions.Count - 1; i >= 0; i--)
                currentGrid.RowDefinitions.RemoveAt(i);
        }
        internal void RefreshData(PieChart mPieChart, List<SalesAnalyticsStructs.REFRESH_PIECHART_STRUCT> valuesList)
        {
            for(int i = 0; i < valuesList.Count; i++)
            {
                mPieChart.Series[i].Values[0] = valuesList[i].value;
            }
        }

        private void SetPieChartsAmount(int salesPersonId)
        {
            ///////////SALES PERSON ORDER////////////////////////
            salesPersonOrderManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListSalesOrdersAmount(salesPersonId);
            GetTotalValue();
            salesPersonOrderManagerLabel.Content = "Sales Total Ordered Amount";
            InitializePieChart(salesPersonOrderManagerGrid, salesPersonOrderManagerPieChart, salesAnalytics.refreshList, false);

            ///////////CATEGORY ORDER/////////////////////////////
            categoryOrderManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListCategoryOrdersAmount(salesPersonId);
            GetTotalValue();
            categoryOrderManagerLabel.Content = "Category Total Ordered Amount";
            InitializePieChart(categoryOrderManagerGrid, categoryOrderManagerPieChart, salesAnalytics.refreshList, false);

            ///////////TYPE ORDER////////////////////////////////
            typeOrderManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListTypeOrdersAmount(salesPersonId);
            GetTotalValue();
            typeOrderManagerLabel.Content = "Type Total Ordered Amount";
            InitializePieChart(typeOrderManagerGrid, typeOrderManagerPieChart, salesAnalytics.refreshList, false);

            ///////////BRAND ORDER////////////////////////////////
            brandOrderManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListBrandOrdersAmount(salesPersonId);
            GetTotalValue();
            brandOrderManagerLabel.Content = "Brand Total Ordered Amount";
            InitializePieChart(brandOrderManagerGrid, brandOrderManagerPieChart, salesAnalytics.refreshList, false);

            ///////////MODEL ORDER////////////////////////
            modelOrderManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListModelOrdersAmount(salesPersonId);
            GetTotalValue();
            modelOrderManagerLabel.Content = "Model Total Ordered Amount";
            InitializePieChart(modelOrderManagerGrid, modelOrderManagerPieChart, salesAnalytics.refreshList, true);

            ///////////STATUS ORDER////////////////////////
            statusOrderManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListSalesOrdersAmountStatus(salesPersonId);
            GetTotalValue();
            statusOrderManagerLabel.Content = "Status Total Ordered Amount";
            InitializePieChart(statusOrderManagerGrid, statusOrderManagerPieChart, salesAnalytics.refreshList, false);

            ///////////SALES PERSON QUOTATION////////////////////////
            salesPersonQuotedManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListSalesQuotationsAmount(salesPersonId);
            GetTotalValue();
            salesPersonQuotedManagerLabel.Content = "Sales Total Quoted Amount";
            InitializePieChart(salesPersonQuotedManagerGrid, salesPersonQuotedManagerPieChart, salesAnalytics.refreshList, false);

            ///////////CATEGORY QUOTATION/////////////////////////////
            categoryQuotedManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListCategoryQuotationsAmount(salesPersonId);
            GetTotalValue();
            categoryQuotedManagerLabel.Content = "Category Total Quoted Amount";
            InitializePieChart(categoryQuotedManagerGrid, categoryQuotedManagerPieChart, salesAnalytics.refreshList, false);

            ///////////TYPE QUOTATION/////////////////////////////
            typeQuotedManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListTypeQuotationsAmount(salesPersonId);
            GetTotalValue();
            typeQuotedManagerLabel.Content = "Type Total Quoted Amount";
            InitializePieChart(typeQuotedManagerGrid, typeQuotedManagerPieChart, salesAnalytics.refreshList, false);

            ///////////BRAND QUOTATION/////////////////////////////
            brandQuotedManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListBrandQuotationsAmount(salesPersonId);
            GetTotalValue();
            brandQuotedManagerLabel.Content = "Brand Total Quoted Amount";
            InitializePieChart(brandQuotedManagerGrid, brandQuotedManagerPieChart, salesAnalytics.refreshList, false);

            ///////////MODEL QUOTATION/////////////////////////////
            modelQuotedManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListModelQuotationsAmount(salesPersonId);
            GetTotalValue();
            modelQuotedManagerLabel.Content = "Model Total Quoted Amount";
            InitializePieChart(modelQuotedManagerGrid, modelQuotedManagerPieChart, salesAnalytics.refreshList, true);

            ///////////STATUS QUOTATION////////////////////////
            statusQuotedManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListSalesQuotedAmountStatus(salesPersonId);
            GetTotalValue();
            statusQuotedManagerLabel.Content = "Status Total Quoted Amount";
            InitializePieChart(statusQuotedManagerGrid, statusQuotedManagerPieChart, salesAnalytics.refreshList, false);
        }

        private void SetPieChartsCount(int salesPersonId)
        {
            ///////////SALES PERSON ORDER////////////////////////
            salesPersonOrderManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListSalesOrdersCount(salesPersonId);
            GetTotalValue();
            salesPersonOrderManagerLabel.Content = "Sales Total Ordered Count";
            InitializePieChart(salesPersonOrderManagerGrid, salesPersonOrderManagerPieChart, salesAnalytics.refreshList, false);

            ///////////CATEGORY ORDER/////////////////////////////
            categoryOrderManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListCategoryOrdersCount(salesPersonId);
            GetTotalValue();
            categoryOrderManagerLabel.Content = "Category Total Ordered Count";
            InitializePieChart(categoryOrderManagerGrid, categoryOrderManagerPieChart, salesAnalytics.refreshList, false);

            ///////////TYPE ORDER////////////////////////////////
            typeOrderManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListTypeOrdersCount(salesPersonId);
            GetTotalValue();
            typeOrderManagerLabel.Content = "Type Total Ordered Count";
            InitializePieChart(typeOrderManagerGrid, typeOrderManagerPieChart, salesAnalytics.refreshList, false);

            ///////////Brand ORDER////////////////////////////////
            brandOrderManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListBrandOrdersCount(salesPersonId);
            GetTotalValue();
            brandOrderManagerLabel.Content = "Brand Total Ordered Count";
            InitializePieChart(brandOrderManagerGrid, brandOrderManagerPieChart, salesAnalytics.refreshList, false);

            ///////////MODEL ORDER////////////////////////
            modelOrderManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListModelOrdersCount(salesPersonId);
            GetTotalValue();
            modelOrderManagerLabel.Content = "Model Total Ordered Count";
            InitializePieChart(modelOrderManagerGrid, modelOrderManagerPieChart, salesAnalytics.refreshList, true);

            ///////////STATUS ORDER////////////////////////
            statusOrderManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListSalesOrdersCountStatus(salesPersonId);
            GetTotalValue();
            statusOrderManagerLabel.Content = "Status Total Ordered Count";
            InitializePieChart(statusOrderManagerGrid, statusOrderManagerPieChart, salesAnalytics.refreshList, false);

            ///////////SALES PERSON QUOTATION////////////////////////
            salesPersonQuotedManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListSalesQuotationsCount(salesPersonId);
            GetTotalValue();
            salesPersonQuotedManagerLabel.Content = "Sales Total Quoted Count";
            InitializePieChart(salesPersonQuotedManagerGrid, salesPersonQuotedManagerPieChart, salesAnalytics.refreshList, false);

            ///////////CATEGORY QUOTATION/////////////////////////////
            categoryQuotedManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListCategoryQuotationsCount(salesPersonId);
            GetTotalValue();
            categoryQuotedManagerLabel.Content = "Category Total Quoted Count";
            InitializePieChart(categoryQuotedManagerGrid, categoryQuotedManagerPieChart, salesAnalytics.refreshList, false);

            ///////////TYPE QUOTATION/////////////////////////////
            typeQuotedManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListTypeQuotationsCount(salesPersonId);
            GetTotalValue();
            typeQuotedManagerLabel.Content = "Type Total Quoted Count";
            InitializePieChart(typeQuotedManagerGrid, typeQuotedManagerPieChart, salesAnalytics.refreshList, false);

            ///////////BRAND QUOTATION/////////////////////////////
            brandQuotedManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListBrandQuotationsCount(salesPersonId);
            GetTotalValue();
            brandQuotedManagerLabel.Content = "Brand Total Quoted Count";
            InitializePieChart(brandQuotedManagerGrid, brandQuotedManagerPieChart, salesAnalytics.refreshList, false);

            ///////////MODEL QUOTATION/////////////////////////////
            modelQuotedManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListModelQuotationsCount(salesPersonId);
            GetTotalValue();
            modelQuotedManagerLabel.Content = "Model Total Quoted Count";
            InitializePieChart(modelQuotedManagerGrid, modelQuotedManagerPieChart, salesAnalytics.refreshList, true);

            ///////////STATUS QUOTATION////////////////////////
            statusQuotedManagerPieChart.Series.Clear();
            salesAnalytics.SetRefresListSalesQuotedCountStatus(salesPersonId);
            GetTotalValue();
            statusQuotedManagerLabel.Content = "Status Total Quoted Count";
            InitializePieChart(statusQuotedManagerGrid, statusQuotedManagerPieChart, salesAnalytics.refreshList, false);

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //GET FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void GetTotalValue()
        {
            summationVariable = 0;

            for (int i = 0; i < salesAnalytics.refreshList.Count; i++)
                summationVariable += (int)salesAnalytics.refreshList[i].value;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SELECTION CHANGED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnSelChangedCountAmountFilterCombo(object sender, SelectionChangedEventArgs e)
        {
            if(countAmountComboBox.SelectedItem != null)
                countAmountComboSelectedIndex = countAmountComboBox.SelectedIndex;

            if (initializationComplete == true)
            {
                if (employeeComboBox.SelectedIndex != employees.Count)
                {
                    if (countAmountComboBox.SelectedIndex == 0)
                    {
                        SetPieChartsCount(employees[employeeComboBox.SelectedIndex].employee_id);
                    }
                    else
                    {
                        SetPieChartsAmount(employees[employeeComboBox.SelectedIndex].employee_id);
                    }
                }
                else
                {
                    if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)
                    {
                        if (countAmountComboBox.SelectedIndex == 0)
                        {
                            SetPieChartsCount(0);
                        }
                        else
                        {
                            SetPieChartsAmount(0);
                        }
                    }
                    else
                    {
                        if (countAmountComboBox.SelectedIndex == 0)
                        {
                            SetPieChartsCount(1);
                        }
                        else
                        {
                            SetPieChartsAmount(1);
                        }
                    }
                }
            }
        }

        private void OnSelChangeEmployeeCombo(object sender, SelectionChangedEventArgs e)
        {
            if (countAmountComboBox.SelectedItem != null)
                countAmountComboSelectedIndex = countAmountComboBox.SelectedIndex;

            if (initializationComplete == true)
            {
                if (employeeComboBox.SelectedIndex != employees.Count)
                {
                    COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT tempEmployee = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT();
                    List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> tempEmployeeList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
                    tempEmployee.employee_id = employees[employeeComboBox.SelectedIndex].employee_id;
                    tempEmployeeList.Add(tempEmployee);

                    salesAnalytics.SetEmployeesList(tempEmployeeList);

                    //if (countAmountComboBox.SelectedIndex == 0)
                    //{
                    //    SetPieChartsCount(employees[employeeComboBox.SelectedIndex].employee_id);
                    //}
                    //else
                    //{
                    //    SetPieChartsAmount(employees[employeeComboBox.SelectedIndex].employee_id);
                    //}

                    countAmountComboBox.SelectedIndex = -1;
                    countAmountComboBox.SelectedIndex = countAmountComboSelectedIndex;
                }
                else
                {
                    salesAnalytics.SetEmployeesList(employees);

                    //if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)
                    //{
                    //    if (countAmountComboBox.SelectedIndex == 0)
                    //    {
                    //        SetPieChartsCount(0);
                    //    }
                    //    else
                    //    {
                    //        SetPieChartsAmount(0);
                    //    }
                    //}
                    //else
                    //{
                    //    if (countAmountComboBox.SelectedIndex == 0)
                    //    {
                    //        SetPieChartsCount(1);
                    //    }
                    //    else
                    //    {
                    //        SetPieChartsAmount(1);
                    //    }
                    //}
                    
                    countAmountComboBox.SelectedIndex = -1;
                    countAmountComboBox.SelectedIndex = countAmountComboSelectedIndex;
                }
            }
        }

        private void OnSelChangedStartDate(object sender, SelectionChangedEventArgs e)
        {
            if (countAmountComboBox.SelectedItem != null && startDatePicker.SelectedDate != null && endDatePicker.SelectedDate != null)
            {
                salesAnalytics = new SalesAnalytics(sqlServer, startDatePicker.SelectedDate.Value, endDatePicker.SelectedDate.Value);

                if (employeeComboBox.SelectedIndex != employees.Count)
                {
                    COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT tempEmployee = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT();
                    List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> tempEmployeeList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
                    tempEmployee.employee_id = employees[employeeComboBox.SelectedIndex].employee_id;
                    tempEmployeeList.Add(tempEmployee);

                    salesAnalytics.SetEmployeesList(tempEmployeeList);
                }
                else
                    salesAnalytics.SetEmployeesList(employees);

                
                countAmountComboBox.SelectedIndex = -1;
                countAmountComboBox.SelectedIndex = countAmountComboSelectedIndex;
            }
        }

        private void OnSelChangedEndDate(object sender, SelectionChangedEventArgs e)
        {
            if (countAmountComboBox.SelectedItem != null && startDatePicker.SelectedDate != null && endDatePicker.SelectedDate != null)
            {
                salesAnalytics = new SalesAnalytics(sqlServer, startDatePicker.SelectedDate.Value, endDatePicker.SelectedDate.Value);

                if (employeeComboBox.SelectedIndex != employees.Count)
                {
                    COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT tempEmployee = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT();
                    List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> tempEmployeeList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
                    tempEmployee.employee_id = employees[employeeComboBox.SelectedIndex].employee_id;
                    tempEmployeeList.Add(tempEmployee);

                    salesAnalytics.SetEmployeesList(tempEmployeeList);
                }
                else
                    salesAnalytics.SetEmployeesList(employees);

                countAmountComboBox.SelectedIndex = -1;
                countAmountComboBox.SelectedIndex = countAmountComboSelectedIndex;
            }
        }

        

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CLICK HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnClickPieChart(object sender, MouseButtonEventArgs e)
        {

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
        private void OnButtonClickedMyProfile(object sender, MouseButtonEventArgs e)
        {
            StatisticsPage statisticsPage = new StatisticsPage(ref loggedInUser);
            this.NavigationService.Navigate(statisticsPage);
        }

        private void OnButtonClickedProjects(object sender, MouseButtonEventArgs e)
        {
            ProjectsPage projectsPage = new ProjectsPage(ref loggedInUser);
            this.NavigationService.Navigate(projectsPage);
        }

        private void OnButtonClickedMaintenanceContracts(object sender, MouseButtonEventArgs e)
        {
            MaintenanceContractsPage maintenanceContractsPage = new MaintenanceContractsPage(ref loggedInUser);
            this.NavigationService.Navigate(maintenanceContractsPage);
        }
        private void OnButtonClickedMaintenanceOffer(object sender, MouseButtonEventArgs e)
        {
            MaintenanceOffersPage maintenanceOffersPage = new MaintenanceOffersPage(ref loggedInUser);
            this.NavigationService.Navigate(maintenanceOffersPage);
        }


    }
}
