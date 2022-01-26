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
using LiveCharts;
using LiveCharts.Wpf;
using _01electronics_library;

namespace _01electronics_crm
{
    
    
    public partial class StatisticsPage : Page
    {

        private Employee loggedInUser;

        private CommonQueries commonQueries;

        private SalesAnalytics salesAnalytics;

        private SQLServer sqlServer;

        private int countAmountComboSelectedIndex;
        private int summationVariable;

        private DateTime startDate;
        private DateTime endDate;

        private List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT> categories = new List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT>();
        private List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT> types = new List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT>();
        private List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT> brands= new List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT>();
        private List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT> models = new List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT>();
        private List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT> employees = new List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT>();
        private List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT> rfqStatus = new List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT>();
        private List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT> quotationtatus = new List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT>();
        private List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT> orderStatus = new List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT>();

        public StatisticsPage( ref Employee mLoggedInUser)
        {
            InitializeComponent();
            sqlServer = new SQLServer();
            loggedInUser = mLoggedInUser;
            commonQueries = new CommonQueries(sqlServer);

            InitializeDatePickers();
            salesAnalytics = new SalesAnalytics(sqlServer, startDatePicker.SelectedDate.Value, endDatePicker.SelectedDate.Value);

            InitializeComboBoxes();
            
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //INITIALIZE FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        private void InitializeDatePickers()
        {
            startDate = new DateTime(2021,1,1);
            endDate = new DateTime(2021,12,31);
            startDatePicker.SelectedDate = startDate;
            endDatePicker.SelectedDate = endDate;
        }

        private bool InitializeComboBoxes()
        {
            countAmountComboBox.Items.Clear();
            countAmountComboBox.Items.Add("Count");
            countAmountComboBox.Items.Add("Amount");

            countAmountComboBox.SelectedIndex = 0;

            if (!commonQueries.GetProductCategories(ref categories))
                return false;


            return true;
        }

        private void InitializePieChart(Grid currentGrid ,PieChart mPieChart, List<SalesAnalyticsStructs.REFRESH_PIECHART_STRUCT> mRefreshList)
        {
            EmptyGrid(currentGrid);
            mRefreshList.Sort((s1, s2) => s2.value.CompareTo(s1.value));
            for(int i = 0; i < mRefreshList.Count; i++)
            {
                currentGrid.RowDefinitions.Add(new RowDefinition());

                Label salesLabel = new Label();
                salesLabel.Width = 150.00;
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
                valueLabel.Content = mRefreshList[i].value;
                currentGrid.Children.Add(valueLabel);
                Grid.SetRow(valueLabel, i);
                Grid.SetColumn(valueLabel, 1);

                mPieChart.Series.Add(new PieSeries { Title = mRefreshList[i].name, StrokeThickness = 0, Values = new ChartValues<Decimal> { mRefreshList[i].value } });
            }

            currentGrid.RowDefinitions.Add(new RowDefinition());

            Label totalLabel = new Label();
            totalLabel.Width = 150.00;
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

            if(countAmountComboBox.SelectedIndex == 0)
            {
                ///////////SALES PERSON ORDER////////////////////////
                salesPersonOrderManagerPieChart.Series.Clear();
                salesAnalytics.SetRefresListSalesOrdersCount();
                GetTotalValue();
                salesPersonOrderManagerLabel.Content = "Sales Total Ordered Count"; 
                InitializePieChart(salesPersonOrderManagerGrid, salesPersonOrderManagerPieChart, salesAnalytics.refreshList);

                ///////////CATEGORY ORDER/////////////////////////////
                categoryOrderManagerPieChart.Series.Clear();
                salesAnalytics.SetRefresListCategoryOrdersCount();
                GetTotalValue();
                categoryOrderManagerLabel.Content = "Category Total Ordered Count";
                InitializePieChart(categoryOrderManagerGrid, categoryOrderManagerPieChart, salesAnalytics.refreshList);

                ///////////TYPE ORDER////////////////////////////////
                typeOrderManagerPieChart.Series.Clear();
                salesAnalytics.SetRefresListTypeOrdersCount();
                GetTotalValue();
                typeOrderManagerLabel.Content = "Type Total Ordered Count";
                InitializePieChart(typeOrderManagerGrid, typeOrderManagerPieChart, salesAnalytics.refreshList);

                ///////////MODEL ORDER////////////////////////
                modelOrderManagerPieChart.Series.Clear();
                salesAnalytics.SetRefresListModelOrdersCount();
                GetTotalValue();
                modelOrderManagerLabel.Content = "Model Total Ordered Count";
                InitializePieChart(modelOrderManagerGrid, modelOrderManagerPieChart, salesAnalytics.refreshList);

                ///////////STATUS ORDER////////////////////////
                statusOrderManagerPieChart.Series.Clear();
                salesAnalytics.SetRefresListSalesOrdersCountStatus();
                GetTotalValue();
                statusOrderManagerLabel.Content = "Status Total Ordered Count";
                InitializePieChart(statusOrderManagerGrid, statusOrderManagerPieChart, salesAnalytics.refreshList);

                ///////////SALES PERSON QUOTATION////////////////////////
                salesPersonQuotedManagerPieChart.Series.Clear();
                salesAnalytics.SetRefresListSalesQuotationsCount();
                GetTotalValue();
                salesPersonQuotedManagerLabel.Content = "Sales Total Quoted Count";
                InitializePieChart(salesPersonQuotedManagerGrid, salesPersonQuotedManagerPieChart, salesAnalytics.refreshList);

                ///////////CATEGORY QUOTATION/////////////////////////////
                categoryQuotedManagerPieChart.Series.Clear();
                salesAnalytics.SetRefresListCategoryQuotationsCount();
                GetTotalValue();
                categoryQuotedManagerLabel.Content = "Category Total Quoted Count";
                InitializePieChart(categoryQuotedManagerGrid, categoryQuotedManagerPieChart, salesAnalytics.refreshList);

                ///////////TYPE QUOTATION/////////////////////////////
                typeQuotedManagerPieChart.Series.Clear();
                salesAnalytics.SetRefresListTypeQuotationsCount();
                GetTotalValue();
                typeQuotedManagerLabel.Content = "Type Total Quoted Count";
                InitializePieChart(typeQuotedManagerGrid, typeQuotedManagerPieChart, salesAnalytics.refreshList);

                ///////////MODEL QUOTATION/////////////////////////////
                modelQuotedManagerPieChart.Series.Clear();
                salesAnalytics.SetRefresListModelQuotationsCount();
                GetTotalValue();
                modelQuotedManagerLabel.Content = "Model Total Quoted Count";
                InitializePieChart(modelQuotedManagerGrid, modelQuotedManagerPieChart, salesAnalytics.refreshList);

                ///////////STATUS QUOTATION////////////////////////
                statusQuotedManagerPieChart.Series.Clear();
                salesAnalytics.SetRefresListSalesQuotedCountStatus();
                GetTotalValue();
                statusOrderManagerLabel.Content = "Status Total Quoted Count";
                InitializePieChart(statusQuotedManagerGrid, statusQuotedManagerPieChart, salesAnalytics.refreshList);

            }
            else
            {
                ///////////SALES PERSON ORDER////////////////////////
                salesPersonOrderManagerPieChart.Series.Clear();
                salesAnalytics.SetRefresListSalesOrdersAmount();
                GetTotalValue();
                salesPersonOrderManagerLabel.Content = "Sales Total Ordered Amount";
                InitializePieChart(salesPersonOrderManagerGrid, salesPersonOrderManagerPieChart, salesAnalytics.refreshList);

                ///////////CATEGORY ORDER/////////////////////////////
                categoryOrderManagerPieChart.Series.Clear();
                salesAnalytics.SetRefresListCategoryOrdersAmount();
                GetTotalValue();
                categoryOrderManagerLabel.Content = "Category Total Ordered Amount";
                InitializePieChart(categoryOrderManagerGrid, categoryOrderManagerPieChart, salesAnalytics.refreshList);

                ///////////TYPE ORDER////////////////////////////////
                typeOrderManagerPieChart.Series.Clear();
                salesAnalytics.SetRefresListTypeOrdersAmount();
                GetTotalValue();
                typeOrderManagerLabel.Content = "Type Total Ordered Amount";
                InitializePieChart(typeOrderManagerGrid, typeOrderManagerPieChart, salesAnalytics.refreshList);

                ///////////MODEL ORDER////////////////////////
                modelOrderManagerPieChart.Series.Clear();
                salesAnalytics.SetRefresListModelOrdersAmount();
                GetTotalValue();
                typeOrderManagerLabel.Content = "Model Total Ordered Amount";
                InitializePieChart(modelOrderManagerGrid, modelOrderManagerPieChart, salesAnalytics.refreshList);

                ///////////STATUS ORDER////////////////////////
                statusOrderManagerPieChart.Series.Clear();
                salesAnalytics.SetRefresListSalesOrdersAmountStatus();
                GetTotalValue();
                statusOrderManagerLabel.Content = "Status Total Ordered Amount";
                InitializePieChart(statusOrderManagerGrid, statusOrderManagerPieChart, salesAnalytics.refreshList);

                ///////////SALES PERSON QUOTATION////////////////////////
                salesPersonQuotedManagerPieChart.Series.Clear();
                salesAnalytics.SetRefresListSalesQuotationsAmount();
                GetTotalValue();
                salesPersonQuotedManagerLabel.Content = "Sales Total Quoted Amount";
                InitializePieChart(salesPersonQuotedManagerGrid, salesPersonQuotedManagerPieChart, salesAnalytics.refreshList);

                ///////////CATEGORY QUOTATION/////////////////////////////
                categoryQuotedManagerPieChart.Series.Clear();
                salesAnalytics.SetRefresListCategoryQuotationsAmount();
                GetTotalValue();
                categoryQuotedManagerLabel.Content = "Category Total Quoted Amount";
                InitializePieChart(categoryQuotedManagerGrid, categoryQuotedManagerPieChart, salesAnalytics.refreshList);

                ///////////TYPE QUOTATION/////////////////////////////
                typeQuotedManagerPieChart.Series.Clear();
                salesAnalytics.SetRefresListTypeQuotationsAmount();
                GetTotalValue();
                typeQuotedManagerLabel.Content = "Type Total Quoted Amount";
                InitializePieChart(typeQuotedManagerGrid, typeQuotedManagerPieChart, salesAnalytics.refreshList);

                ///////////MODEL QUOTATION/////////////////////////////
                modelQuotedManagerPieChart.Series.Clear();
                salesAnalytics.SetRefresListModelQuotationsAmount();
                GetTotalValue();
                modelQuotedManagerLabel.Content = "Model Total Quoted Amount";
                InitializePieChart(modelQuotedManagerGrid, modelQuotedManagerPieChart, salesAnalytics.refreshList);

                ///////////STATUS QUOTATION////////////////////////
                statusQuotedManagerPieChart.Series.Clear();
                salesAnalytics.SetRefresListSalesQuotedAmountStatus();
                GetTotalValue();
                statusQuotedManagerLabel.Content = "Status Total Quoted Amount";
                InitializePieChart(statusQuotedManagerGrid, statusQuotedManagerPieChart, salesAnalytics.refreshList);
            }
        }
        private void OnSelChangedStartDate(object sender, SelectionChangedEventArgs e)
        {
            if (countAmountComboBox.SelectedItem != null && startDatePicker.SelectedDate != null && endDatePicker.SelectedDate != null)
            {
                salesAnalytics = new SalesAnalytics(sqlServer, startDatePicker.SelectedDate.Value, endDatePicker.SelectedDate.Value);
                countAmountComboBox.SelectedIndex = -1;
                countAmountComboBox.SelectedIndex = countAmountComboSelectedIndex;
            }
        }

        private void OnSelChangedEndDate(object sender, SelectionChangedEventArgs e)
        {
            if (countAmountComboBox.SelectedItem != null && startDatePicker.SelectedDate != null && endDatePicker.SelectedDate != null)
            {
                salesAnalytics = new SalesAnalytics(sqlServer, startDatePicker.SelectedDate.Value, endDatePicker.SelectedDate.Value);
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

        private void OnButtonClickedMyProfile(object sender, MouseButtonEventArgs e)
        {

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

        private void OnButtonClickedStatistics(object sender, MouseButtonEventArgs e)
        {

        }

        
    }
}
