using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using _01electronics_library;
using Label = System.Windows.Controls.Label;
using ListBox = System.Windows.Controls.ListBox;
using Orientation = System.Windows.Controls.Orientation;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for WorkOrdersPage.xaml
    /// </summary>
    public partial class WorkOrdersPage : Page
    {
        private Employee loggedInUser;

        private SQLServer sqlDatabase;
        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        
        private WorkOrder selectedWorkOrder;
        private int finalYear = Int32.Parse(DateTime.Now.Year.ToString());

        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> salesEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> preSalesEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_WORK_MACROS.WORK_ORDER_MAX_STRUCT> workOrders = new List<COMPANY_WORK_MACROS.WORK_ORDER_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.WORK_ORDER_MAX_STRUCT> workOrdersAfterFiltering = new List<COMPANY_WORK_MACROS.WORK_ORDER_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> productTypes = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
        private List<COMPANY_WORK_MACROS.BRAND_STRUCT> brandTypes = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();
        private List<COMPANY_WORK_MACROS.STATUS_STRUCT> orderStatuses = new List<COMPANY_WORK_MACROS.STATUS_STRUCT>();

        private int selectedYear;
        private int selectedQuarter;

        private int selectedSales;
        private int selectedPreSales;

        private int selectedProduct;
        private int selectedBrand;
        private int selectedStatus;
        private int salesPersonTeam;

        int viewAddCondition;

        private Grid currentGrid;
        private Expander currentExpander;
        private Expander previousExpander;

        public WorkOrdersPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();
            loggedInUser = mLoggedInUser;

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            if (!GetWorkOrders())
                return;

            InitializeYearsComboBox();
            InitializeQuartersComboBox();
            InitializeStatusComboBox();

            if (!InitializeSalesComboBox())
                return;

            if (!InitializePreSalesComboBox())
                return;

            if (!InitializeProductsComboBox())
                return;

            if (!InitializeBrandsComboBox())
                return;

            SetDefaultSettings();

            SetWorkOrdersStackPanel();
            SetWorkOrdersGrid();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //GET DATA FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool GetWorkOrders()
        {
            if (!commonQueriesObject.GetWorkOrders(ref workOrders))
                return false;
            return true;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //INTIALIZATION FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///

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

        private bool InitializeSalesComboBox()
        {
            if (!commonQueriesObject.GetTeamEmployees(COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID, ref salesEmployeesList))
                return false;

            for (int i = 0; i < salesEmployeesList.Count; i++)
                salesComboBox.Items.Add(salesEmployeesList[i].employee_name);

            return true;
        }
        private bool InitializePreSalesComboBox()
        {
            if (!commonQueriesObject.GetTeamEmployees(COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID, ref preSalesEmployeesList))
                return false;

            for (int i = 0; i < preSalesEmployeesList.Count; i++)
                preSalesComboBox.Items.Add(preSalesEmployeesList[i].employee_name);

            return true;
        }

        private bool InitializeProductsComboBox()
        {
            if (!commonQueriesObject.GetCompanyProducts(ref productTypes))
                return false;

            for (int i = 0; i < productTypes.Count; i++)
                productComboBox.Items.Add(productTypes[i].typeName);

            return true;
        }

        private bool InitializeBrandsComboBox()
        {

            if (!commonQueriesObject.GetCompanyBrands(ref brandTypes))
                return false;

            for (int i = 0; i < brandTypes.Count; i++)
                brandComboBox.Items.Add(brandTypes[i].brandName);

            brandComboBox.IsEnabled = false;
            return true;
        }

        private void InitializeStatusComboBox()
        {
            if (!commonQueriesObject.GetWorkOrderStatus(ref orderStatuses))
                return;

            for (int i = 0; i < orderStatuses.Count; i++)
            {
                statusComboBox.Items.Add(orderStatuses[i].status_name);
            }

            statusComboBox.IsEnabled = false;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CONFIGURATION FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ResetComboBoxes()
        {
            yearComboBox.SelectedIndex = -1;
            quarterComboBox.SelectedIndex = -1;

            salesComboBox.SelectedIndex = -1;
            preSalesComboBox.SelectedIndex = -1;

            productComboBox.SelectedIndex = -1;
            brandComboBox.SelectedIndex = -1;

            statusComboBox.SelectedIndex = -1;
        }
        private void DisableComboBoxes()
        {
            yearComboBox.IsEnabled = false;
            quarterComboBox.IsEnabled = false;
            salesComboBox.IsEnabled = false;
            preSalesComboBox.IsEnabled = false;
            productComboBox.IsEnabled = false;
            brandComboBox.IsEnabled = false;
            statusComboBox.IsEnabled = false;
        }

        private void DisableViewButton()
        {
            //viewButton.IsEnabled = false;
        }
        private void EnableViewButton()
        {
            //viewButton.IsEnabled = true;
        }

        //private void DisableReviseButton()
        //{
        //    reviseButton.IsEnabled = false;
        //}
        //private void EnableReviseButton()
        //{
        //    reviseButton.IsEnabled = true;
        //}

        private void SetDefaultSettings()
        {
            DisableViewButton();
            ////DisableReviseButton();
            DisableComboBoxes();
            ResetComboBoxes();

            yearCheckBox.IsChecked = true;
            yearCheckBox.IsEnabled = false;

            if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)
            {
                salesCheckBox.IsChecked = false;
                salesCheckBox.IsEnabled = true;
                salesComboBox.IsEnabled = false;

                preSalesCheckBox.IsChecked = false;
                preSalesCheckBox.IsEnabled = true;
                preSalesComboBox.IsEnabled = false;
            }
            else if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION && loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
            {
                salesCheckBox.IsChecked = false;
                salesCheckBox.IsEnabled = true;
                salesComboBox.IsEnabled = false;

                preSalesCheckBox.IsChecked = false;
                preSalesCheckBox.IsEnabled = true;
                preSalesComboBox.IsEnabled = false;
            }
            else if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION && loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID)
            {
                preSalesCheckBox.IsChecked = false;
                preSalesCheckBox.IsEnabled = true;
                preSalesComboBox.IsEnabled = false;

                salesCheckBox.IsChecked = false;
                salesCheckBox.IsEnabled = true;
                salesComboBox.IsEnabled = false;
            }
            else if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
            {
                salesCheckBox.IsChecked = true;
                salesCheckBox.IsEnabled = false;
                salesComboBox.IsEnabled = false;

                preSalesCheckBox.IsChecked = false;
                preSalesCheckBox.IsEnabled = true;
                preSalesComboBox.IsEnabled = false;
            }
            else if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID)
            {
                preSalesCheckBox.IsChecked = true;
                preSalesCheckBox.IsEnabled = false;
                preSalesComboBox.IsEnabled = false;

                salesCheckBox.IsChecked = false;
                salesCheckBox.IsEnabled = true;
                salesComboBox.IsEnabled = false;
            }

            if (loggedInUser.GetEmployeeTeamId() != COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID)
            {
                addButton.IsEnabled = false;
            }
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
            if (yearComboBox.SelectedIndex == DateTime.Now.Year - BASIC_MACROS.CRM_START_YEAR)
                quarterComboBox.SelectedIndex = commonFunctionsObject.GetCurrentQuarter() - 1;
            else
                quarterComboBox.SelectedIndex = 0;
        }


        private bool SetWorkOrdersStackPanel()
        {
            workOrdersStackPanel.Children.Clear();

            workOrdersAfterFiltering.Clear();

            for (int i = 0; i < workOrders.Count; i++)
            {
                DateTime currentWorkOrderDate = DateTime.Parse(workOrders[i].issue_date);

                bool salesPersonCondition = selectedSales != workOrders[i].sales_person_id;
                bool assigneeCondition;

                if (selectedPreSales == workOrders[i].offer_proposer_id || (selectedPreSales == workOrders[i].sales_person_id))
                    assigneeCondition = true;
                else
                    assigneeCondition = false;

                bool productCondition = false;
                for (int productNo = 0; productNo < workOrders[i].products.Count(); productNo++)
                    if (workOrders[i].products[productNo].productType.typeId == selectedProduct)
                        productCondition |= true;

                bool brandCondition = false;
                for (int productNo = 0; productNo < workOrders[i].products.Count(); productNo++)
                    if (workOrders[i].products[productNo].productBrand.brandId == selectedBrand)
                        brandCondition |= true;
                
                if (searchCheckBox.IsChecked == true && searchTextBox.Text != null)
                {
                    String tempId = workOrders[i].order_id;
                    String tempCompanyName = workOrders[i].company_name;
                    String tempContactName = workOrders[i].contact_name;
                    bool containsID = tempId.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                    bool containsCompanyName = tempCompanyName.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                    bool containsContactName = tempContactName.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (containsID || containsCompanyName || containsContactName)
                    {

                    }
                    else
                        continue;
                }


                if (yearCheckBox.IsChecked == true && currentWorkOrderDate.Year != selectedYear)
                    continue;

                if (salesCheckBox.IsChecked == true && salesPersonCondition)
                    continue;

                if (preSalesCheckBox.IsChecked == true && !assigneeCondition)
                    continue;

                if (quarterCheckBox.IsChecked == true && commonFunctionsObject.GetQuarter(currentWorkOrderDate) != selectedQuarter)
                    continue;

                if (productCheckBox.IsChecked == true && !productCondition)
                    continue;

                if (brandCheckBox.IsChecked == true && !brandCondition)
                    continue;

                if (statusCheckBox.IsChecked == true && workOrders[i].order_status_id != selectedStatus)
                    continue;

                workOrdersAfterFiltering.Add(workOrders[i]);

                StackPanel fullStackPanel = new StackPanel();
                fullStackPanel.Orientation = Orientation.Vertical;
                //fullStackPanel.Height = 90;


                Label offerIdLabel = new Label();
                offerIdLabel.Content = workOrders[i].order_id;
                offerIdLabel.Style = (Style)FindResource("stackPanelItemHeader");

                Label salesLabel = new Label();
                salesLabel.Content = workOrders[i].sales_person_name;
                salesLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label preSalesLabel = new Label();
                preSalesLabel.Content = workOrders[i].offer_proposer_name;
                preSalesLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label companyAndContactLabel = new Label();
                companyAndContactLabel.Content = workOrders[i].company_name + " - " + workOrders[i].contact_name;
                companyAndContactLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label productTypeAndBrandLabel = new Label();
                List<COMPANY_WORK_MACROS.ORDER_PRODUCT_STRUCT> temp = workOrders[i].products;

                for (int j = 0; j < temp.Count(); j++)
                {
                    COMPANY_WORK_MACROS.PRODUCT_STRUCT tempType1 = temp[j].productType;
                    COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand1 = temp[j].productBrand;

                    productTypeAndBrandLabel.Content += tempType1.typeName + " - " + tempBrand1.brandName;

                    if (j != temp.Count() - 1)
                        productTypeAndBrandLabel.Content += ", ";
                }
                productTypeAndBrandLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label contractTypeLabel = new Label();
                contractTypeLabel.Content = workOrders[i].contract_type;
                contractTypeLabel.Style = (Style)FindResource("stackPanelItemBody");

                Border borderIcon = new Border();
                borderIcon.Style = (Style)FindResource("BorderIcon");

                Label rfqStatusLabel = new Label();
                rfqStatusLabel.Content = workOrders[i].order_status;
                rfqStatusLabel.Style = (Style)FindResource("BorderIconTextLabel");

                if (workOrders[i].order_status_id == COMPANY_WORK_MACROS.PENDING_OUTGOING_QUOTATION)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500"));
                }
                else if (workOrders[i].order_status_id == COMPANY_WORK_MACROS.CONFIRMED_OUTGOING_QUOTATION)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008000"));
                }
                else
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                }

                borderIcon.Child = rfqStatusLabel;

                Expander expander = new Expander();
                expander.ExpandDirection = ExpandDirection.Down;
                expander.VerticalAlignment = VerticalAlignment.Center;
                expander.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                expander.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                expander.Expanded += new RoutedEventHandler(OnExpandExpander);
                expander.Collapsed += new RoutedEventHandler(OnCollapseExpander);

                ListBox listBox = new ListBox();
                listBox.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
                listBox.SelectionChanged += new SelectionChangedEventHandler(OnSelChangedListBox);

                ListBoxItem viewButton = new ListBoxItem();
                viewButton.Content = "View";
                viewButton.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));

                ListBoxItem viewRFQButton = new ListBoxItem();
                viewRFQButton.Content = "View RFQ";
                viewRFQButton.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));

                ListBoxItem viewOfferButton = new ListBoxItem();
                viewOfferButton.Content = "View Offer";
                viewOfferButton.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
               
                listBox.Items.Add(viewButton);

                listBox.Items.Add(viewRFQButton);

                listBox.Items.Add(viewOfferButton);


                if (workOrders[i].order_status_id != COMPANY_WORK_MACROS.CLOSED_WORK_ORDER && loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID)
                {
                    ListBoxItem confirmOrderButton = new ListBoxItem();
                    confirmOrderButton.Content = "Confirm Order";
                    confirmOrderButton.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                    listBox.Items.Add(confirmOrderButton);

                }

                expander.Content = listBox;

                fullStackPanel.Children.Add(offerIdLabel);
                fullStackPanel.Children.Add(salesLabel);
                fullStackPanel.Children.Add(preSalesLabel);
                fullStackPanel.Children.Add(companyAndContactLabel);
                fullStackPanel.Children.Add(productTypeAndBrandLabel);
                fullStackPanel.Children.Add(contractTypeLabel);

                Grid grid = new Grid();
                ColumnDefinition column1 = new ColumnDefinition();
                ColumnDefinition column2 = new ColumnDefinition();
                ColumnDefinition column3 = new ColumnDefinition();
                column2.MaxWidth = 95;
                column3.MaxWidth = 50;

                grid.ColumnDefinitions.Add(column1);
                grid.ColumnDefinitions.Add(column2);
                grid.ColumnDefinitions.Add(column3);

                grid.Children.Add(fullStackPanel);
                grid.Children.Add(borderIcon);
                grid.Children.Add(expander);

                Grid.SetColumn(fullStackPanel, 0);
                Grid.SetColumn(borderIcon, 1);
                Grid.SetColumn(expander, 2);

                workOrdersStackPanel.Children.Add(grid);
            }

            return true;
        }

        private bool SetWorkOrdersGrid()
        {

            workOrdersGrid.Children.Clear();
            workOrdersGrid.RowDefinitions.Clear();
            workOrdersGrid.ColumnDefinitions.Clear();

            int counter = 0;

            Label orderIdHeader = new Label();
            orderIdHeader.Content = "Order ID";
            orderIdHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label orderSalesHeader = new Label();
            orderSalesHeader.Content = "Sales Engineer";
            orderSalesHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label orderPreSalesHeader = new Label();
            orderPreSalesHeader.Content = "Pre-Sales Engineer";
            orderPreSalesHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label orderCompanyContactHeader = new Label();
            orderCompanyContactHeader.Content = "Contact Info";
            orderCompanyContactHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label orderProductsHeader = new Label();
            orderProductsHeader.Content = "Products";
            orderProductsHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label orderContractTypeHeader = new Label();
            orderContractTypeHeader.Content = "Contract Type";
            orderContractTypeHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label orderStatusHeader = new Label();
            orderStatusHeader.Content = "Order Status";
            orderStatusHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label orderProjectHeader = new Label();
            orderProjectHeader.Content = "Order Project";
            orderProjectHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label orderProjectLocationsHeader = new Label();
            orderProjectLocationsHeader.Content = "Project Locations";
            orderProjectLocationsHeader.Style = (Style)FindResource("tableSubHeaderItem");

            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());

            workOrdersGrid.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(orderIdHeader, 0);
            Grid.SetColumn(orderIdHeader, 0);
            workOrdersGrid.Children.Add(orderIdHeader);

            Grid.SetRow(orderSalesHeader, 0);
            Grid.SetColumn(orderSalesHeader, 1);
            workOrdersGrid.Children.Add(orderSalesHeader);

            Grid.SetRow(orderPreSalesHeader, 0);
            Grid.SetColumn(orderPreSalesHeader, 2);
            workOrdersGrid.Children.Add(orderPreSalesHeader);

            Grid.SetRow(orderCompanyContactHeader, 0);
            Grid.SetColumn(orderCompanyContactHeader, 3);
            workOrdersGrid.Children.Add(orderCompanyContactHeader);

            Grid.SetRow(orderProductsHeader, 0);
            Grid.SetColumn(orderProductsHeader, 4);
            workOrdersGrid.Children.Add(orderProductsHeader);

            Grid.SetRow(orderContractTypeHeader, 0);
            Grid.SetColumn(orderContractTypeHeader, 5);
            workOrdersGrid.Children.Add(orderContractTypeHeader);

            Grid.SetRow(orderStatusHeader, 0);
            Grid.SetColumn(orderStatusHeader, 6);
            workOrdersGrid.Children.Add(orderStatusHeader);

            Grid.SetRow(orderProjectHeader, 0);
            Grid.SetColumn(orderProjectHeader, 7);
            workOrdersGrid.Children.Add(orderProjectHeader);

            Grid.SetRow(orderProjectLocationsHeader, 0);
            Grid.SetColumn(orderProjectLocationsHeader, 8);
            workOrdersGrid.Children.Add(orderProjectLocationsHeader);

            int currentRowNumber = 1;


            for (int i = 0; i < workOrders.Count; i++)
            {
                DateTime currentWorkOrderDate = DateTime.Parse(workOrders[i].issue_date);

                bool salesPersonCondition = selectedSales != workOrders[i].sales_person_id;

                bool assigneeCondition;

                if (selectedPreSales == workOrders[i].offer_proposer_id || (selectedPreSales == workOrders[i].sales_person_id))
                    assigneeCondition = true;
                else
                    assigneeCondition = false;

                bool productCondition = false;
                for (int productNo = 0; productNo < workOrders[i].products.Count(); productNo++)
                    if (workOrders[i].products[productNo].productType.typeId == selectedProduct)
                        productCondition |= true;

                bool brandCondition = false;
                for (int productNo = 0; productNo < workOrders[i].products.Count(); productNo++)
                    if (workOrders[i].products[productNo].productBrand.brandId == selectedBrand)
                        brandCondition |= true;


                if (searchCheckBox.IsChecked == true && searchTextBox.Text != null)
                {
                    String tempId = workOrders[i].order_id;
                    String tempCompanyName = workOrders[i].company_name;
                    String tempContactName = workOrders[i].contact_name;
                    bool containsID = tempId.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                    bool containsCompanyName = tempCompanyName.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                    bool containsContactName = tempContactName.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (containsID || containsCompanyName || containsContactName)
                    {

                    }
                    else
                        continue;
                }

                if (yearCheckBox.IsChecked == true && currentWorkOrderDate.Year != selectedYear)
                    continue;

                if (salesCheckBox.IsChecked == true && salesPersonCondition)
                    continue;

                if (preSalesCheckBox.IsChecked == true && !assigneeCondition)
                    continue;

                if (quarterCheckBox.IsChecked == true && commonFunctionsObject.GetQuarter(currentWorkOrderDate) != selectedQuarter)
                    continue;

                if (productCheckBox.IsChecked == true && !productCondition)
                    continue;

                if (brandCheckBox.IsChecked == true && !brandCondition)
                    continue;

                if (statusCheckBox.IsChecked == true && workOrders[i].order_status_id != selectedStatus)
                    continue;

                RowDefinition currentRow = new RowDefinition();
                workOrdersGrid.RowDefinitions.Add(currentRow);

                Label orderIdLabel = new Label();
                orderIdLabel.Content = workOrders[i].order_id;
                orderIdLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(orderIdLabel, currentRowNumber);
                Grid.SetColumn(orderIdLabel, 0);
                workOrdersGrid.Children.Add(orderIdLabel);


                Label salesLabel = new Label();
                salesLabel.Content = workOrders[i].sales_person_name;
                salesLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(salesLabel, currentRowNumber);
                Grid.SetColumn(salesLabel, 1);
                workOrdersGrid.Children.Add(salesLabel);

                Label preSalesLabel = new Label();
                preSalesLabel.Content = workOrders[i].offer_proposer_name;
                preSalesLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(preSalesLabel, currentRowNumber);
                Grid.SetColumn(preSalesLabel, 2);
                workOrdersGrid.Children.Add(preSalesLabel);

                Label companyAndContactLabel = new Label();
                companyAndContactLabel.Content = workOrders[i].company_name + " - " + workOrders[i].contact_name;
                companyAndContactLabel.Style = (Style)FindResource("tableSubItemLabel");

                workOrdersGrid.Children.Add(companyAndContactLabel);
                Grid.SetRow(companyAndContactLabel, currentRowNumber);
                Grid.SetColumn(companyAndContactLabel, 3);


                Grid productGrid = new Grid();
                productGrid.ShowGridLines = true;


                productGrid.ColumnDefinitions.Add(new ColumnDefinition());
                productGrid.ColumnDefinitions.Add(new ColumnDefinition());
                productGrid.ColumnDefinitions.Add(new ColumnDefinition());
                productGrid.ColumnDefinitions.Add(new ColumnDefinition());

                productGrid.RowDefinitions.Add(new RowDefinition());


                Label rowColumnHeader = new Label();
                rowColumnHeader.Style = (Style)FindResource("tableSubHeaderItem");

                productGrid.Children.Add(rowColumnHeader);
                Grid.SetRow(rowColumnHeader, 0);
                Grid.SetColumn(rowColumnHeader, 0);

                Label typeHeader = new Label();
                typeHeader.Content = "Type";
                typeHeader.Style = (Style)FindResource("tableSubHeaderItem");

                productGrid.Children.Add(typeHeader);
                Grid.SetRow(typeHeader, 0);
                Grid.SetColumn(typeHeader, 1);


                Label brandHeader = new Label();
                brandHeader.Content = "Brand";
                brandHeader.Style = (Style)FindResource("tableSubHeaderItem");

                productGrid.Children.Add(brandHeader);
                Grid.SetRow(brandHeader, 0);
                Grid.SetColumn(brandHeader, 2);


                Label modelHeader = new Label();
                modelHeader.Content = "Model";
                modelHeader.Style = (Style)FindResource("tableSubHeaderItem");

                productGrid.Children.Add(modelHeader);
                Grid.SetRow(modelHeader, 0);
                Grid.SetColumn(modelHeader, 3);


                List<COMPANY_WORK_MACROS.ORDER_PRODUCT_STRUCT> temp = workOrders[i].products;

                for (int j = 0; j < temp.Count(); j++)
                {
                    COMPANY_WORK_MACROS.PRODUCT_STRUCT tempType1 = temp[j].productType;
                    COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand1 = temp[j].productBrand;
                    COMPANY_WORK_MACROS.MODEL_STRUCT tempModel1 = temp[j].productModel;

                    if (tempType1.typeId == 0)
                        continue;

                    productGrid.RowDefinitions.Add(new RowDefinition());

                    int tempNumber = j + 1;
                    Label productNumberHeader = new Label();
                    productNumberHeader.Content = "Product" + " " + tempNumber;
                    productNumberHeader.Style = (Style)FindResource("tableSubHeaderItem");

                    productGrid.Children.Add(productNumberHeader);
                    Grid.SetRow(productNumberHeader, j + 1);
                    Grid.SetColumn(productNumberHeader, 0);

                    Label type = new Label();
                    type.Content = tempType1.typeName;
                    type.Style = (Style)FindResource("tableSubItemLabel");

                    productGrid.Children.Add(type);
                    Grid.SetRow(type, j + 1);
                    Grid.SetColumn(type, 1);

                    Label brand = new Label();
                    brand.Content = tempBrand1.brandName;
                    brand.Style = (Style)FindResource("tableSubItemLabel");

                    productGrid.Children.Add(brand);
                    Grid.SetRow(brand, j + 1);
                    Grid.SetColumn(brand, 2);

                    Label model = new Label();
                    model.Content = tempModel1.modelName;
                    model.Style = (Style)FindResource("tableSubItemLabel");

                    productGrid.Children.Add(model);
                    Grid.SetRow(model, j + 1);
                    Grid.SetColumn(model, 3);
                }

                workOrdersGrid.Children.Add(productGrid);
                Grid.SetRow(productGrid, currentRowNumber);
                Grid.SetColumn(productGrid, 4);



                Label contractTypeLabel = new Label();
                contractTypeLabel.Content = workOrders[i].contract_type;
                contractTypeLabel.Style = (Style)FindResource("tableSubItemLabel");

                workOrdersGrid.Children.Add(contractTypeLabel);
                Grid.SetRow(contractTypeLabel, currentRowNumber);
                Grid.SetColumn(contractTypeLabel, 5);


                Label rfqStatusLabel = new Label();
                rfqStatusLabel.Content = workOrders[i].order_status;
                rfqStatusLabel.Style = (Style)FindResource("tableSubItemLabel");

                workOrdersGrid.Children.Add(rfqStatusLabel);
                Grid.SetRow(rfqStatusLabel, currentRowNumber);
                Grid.SetColumn(rfqStatusLabel, 6);

                Label rfqProjectLabel = new Label();
                rfqProjectLabel.Content = workOrders[i].project_name;
                rfqProjectLabel.Style = (Style)FindResource("tableSubItemLabel");

                workOrdersGrid.Children.Add(rfqProjectLabel);
                Grid.SetRow(rfqProjectLabel, currentRowNumber);
                Grid.SetColumn(rfqProjectLabel, 7);

                
                Grid projectLocationsGrid = new Grid();
                projectLocationsGrid.ShowGridLines = true;

                projectLocationsGrid.RowDefinitions.Add(new RowDefinition());

                Label projectLocationsHeaderLabel = new Label();
                projectLocationsHeaderLabel.Content = "Projects Locations";
                projectLocationsHeaderLabel.Style = (Style)FindResource("tableSubHeaderItem");

                projectLocationsGrid.Children.Add(projectLocationsHeaderLabel);
                Grid.SetRow(projectLocationsHeaderLabel, 0);

                for(int k = 0; k < workOrders[i].project_locations.Count; k++)
                {
                    projectLocationsGrid.RowDefinitions.Add(new RowDefinition());

                    Label locationLabel = new Label();
                    locationLabel.Content = workOrders[i].project_locations[k].branch_Info.district + ", " + workOrders[i].project_locations[k].branch_Info.state_governorate + ", " + workOrders[i].project_locations[k].branch_Info.city + ", " + workOrders[i].project_locations[k].branch_Info.country;
                    locationLabel.Style = (Style)FindResource("tableSubItemLabel");

                    projectLocationsGrid.Children.Add(locationLabel);
                    Grid.SetRow(locationLabel, k + 1);
                }

                workOrdersGrid.Children.Add(projectLocationsGrid);
                Grid.SetRow(projectLocationsGrid, currentRowNumber);
                Grid.SetColumn(projectLocationsGrid, 8);

                //currentRow.MouseLeftButtonDown += OnBtnClickWorkorderItem;

                currentRowNumber++;


            }

            return true;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SELECTION CHANGED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnTextChangedSearchTextBox(object sender, TextChangedEventArgs e)
        {
            SetWorkOrdersStackPanel();
            SetWorkOrdersGrid();
        }
        private void OnSelChangedYearCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            ////DisableReviseButton();

            if (yearComboBox.SelectedItem != null)
                selectedYear = BASIC_MACROS.CRM_START_YEAR + yearComboBox.SelectedIndex;
            else
                selectedYear = 0;

            SetWorkOrdersStackPanel();
            SetWorkOrdersGrid();
        }

        private void OnSelChangedQuarterCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (quarterComboBox.SelectedItem != null)
                selectedQuarter = quarterComboBox.SelectedIndex + 1;
            else
                selectedQuarter = 0;

            SetWorkOrdersStackPanel();
            SetWorkOrdersGrid();
        }

        private void OnSelChangedSalesCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (salesComboBox.SelectedItem != null)
                selectedSales = salesEmployeesList[salesComboBox.SelectedIndex].employee_id;
            else
                selectedSales = 0;



            SetWorkOrdersStackPanel();
            SetWorkOrdersGrid();
        }
        private void OnSelChangedPreSalesCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (preSalesComboBox.SelectedItem != null)
                selectedPreSales = preSalesEmployeesList[preSalesComboBox.SelectedIndex].employee_id;
            else
                selectedPreSales = 0;



            SetWorkOrdersStackPanel();
            SetWorkOrdersGrid();
        }

        private void OnSelChangedProductCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (productComboBox.SelectedItem != null)
                selectedProduct = productTypes[productComboBox.SelectedIndex].typeId;
            else
                selectedProduct = 0;

            SetWorkOrdersStackPanel();
            SetWorkOrdersGrid();
        }

        private void OnSelChangedBrandCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (brandComboBox.SelectedItem != null)
                selectedBrand = brandTypes[brandComboBox.SelectedIndex].brandId;
            else
                selectedBrand = 0;

            SetWorkOrdersStackPanel();
            SetWorkOrdersGrid();
        }

        private void OnSelChangedStatusCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (statusComboBox.SelectedItem != null)
                selectedStatus = orderStatuses[statusComboBox.SelectedIndex].status_id;
            else
                selectedStatus = 0;

            SetWorkOrdersStackPanel();
            SetWorkOrdersGrid();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CHECKED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnCheckSearchCheckBox(object sender, RoutedEventArgs e)
        {
            searchTextBox.IsEnabled = true;
        }

        
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

        private void OnCheckSalesCheckBox(object sender, RoutedEventArgs e)
        {
            salesComboBox.IsEnabled = true;


            salesComboBox.SelectedIndex = 0;

            for (int i = 0; i < salesEmployeesList.Count; i++)
                if (loggedInUser.GetEmployeeId() == salesEmployeesList[i].employee_id)
                    salesComboBox.SelectedIndex = i;
        }
        private void OnCheckPreSalesCheckBox(object sender, RoutedEventArgs e)
        {
            preSalesComboBox.IsEnabled = true;


            preSalesComboBox.SelectedIndex = 0;

            for (int i = 0; i < preSalesEmployeesList.Count; i++)
                if (loggedInUser.GetEmployeeId() == preSalesEmployeesList[i].employee_id)
                    preSalesComboBox.SelectedIndex = i;
        }

        private void OnCheckProductCheckBox(object sender, RoutedEventArgs e)
        {
            productComboBox.IsEnabled = true;


            productComboBox.SelectedIndex = 0;
        }

        private void OnCheckBrandCheckBox(object sender, RoutedEventArgs e)
        {
            brandComboBox.IsEnabled = true;


            brandComboBox.SelectedIndex = 0;
        }

        private void OnCheckStatusCheckBox(object sender, RoutedEventArgs e)
        {
            statusComboBox.IsEnabled = true;


            statusComboBox.SelectedIndex = 0;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //UNCHECKED HANDLERS FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnUnCheckSearchCheckBox(object sender, RoutedEventArgs e)
        {
            searchTextBox.IsEnabled = false;
            searchTextBox.Text = null;
        }

        
        private void OnUncheckYearCheckBox(object sender, RoutedEventArgs e)
        {
            yearComboBox.SelectedItem = null;
            yearComboBox.IsEnabled = false;

        }


        private void OnUncheckQuarterCheckBox(object sender, RoutedEventArgs e)
        {
            quarterComboBox.SelectedItem = null;
            quarterComboBox.IsEnabled = false;

        }


        private void OnUncheckSalesCheckBox(object sender, RoutedEventArgs e)
        {
            salesComboBox.SelectedItem = null;
            salesComboBox.IsEnabled = false;

        }
        private void OnUncheckPreSalesCheckBox(object sender, RoutedEventArgs e)
        {
            preSalesComboBox.SelectedItem = null;
            preSalesComboBox.IsEnabled = false;

        }

        private void OnUncheckProductCheckBox(object sender, RoutedEventArgs e)
        {
            productComboBox.SelectedItem = null;
            productComboBox.IsEnabled = false;

        }

        private void OnUncheckBrandCheckBox(object sender, RoutedEventArgs e)
        {
            brandComboBox.SelectedItem = null;
            brandComboBox.IsEnabled = false;

        }

        private void OnUncheckStatusCheckBox(object sender, RoutedEventArgs e)
        {
            statusComboBox.SelectedItem = null;
            statusComboBox.IsEnabled = false;

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //BTN CLICKED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        private void OnBtnClickAdd(object sender, RoutedEventArgs e)
        {
            viewAddCondition = COMPANY_WORK_MACROS.ORDER_ADD_CONDITION;

            selectedWorkOrder = new WorkOrder(sqlDatabase);

            WorkOrderWindow workOrderWindow = new WorkOrderWindow(ref loggedInUser, ref selectedWorkOrder, viewAddCondition, false);

            workOrderWindow.Closed += OnClosedWorkOrderWindow;
            workOrderWindow.Show();
        }

        //private void OnBtnClickView(object sender, RoutedEventArgs e)
        //{
        //    OutgoingQuotation selectedWorkOffer = new OutgoingQuotation(sqlDatabase);
        //
        //    commonQueriesObject.GetEmployeeTeam(workOrdersAfterFiltering[workOrdersStackPanel.Children.IndexOf(currentSelectedOrderItem)].sales_person_id, ref salesPersonTeam);
        //
        //    int viewAddCondition = COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION;
        //    WorkOfferWindow viewOffer = new WorkOfferWindow(ref loggedInUser, ref selectedWorkOffer, viewAddCondition, false);
        //    viewOffer.Show();
        //}

        //private void OnBtnClickWorkOfferItem(object sender, RoutedEventArgs e)
        //{
        //    EnableViewButton();
        //    //EnableReviseButton();
        //    previousSelectedOrderItem = currentSelectedOrderItem;
        //    currentSelectedOrderItem = (Grid)sender;
        //    BrushConverter brush = new BrushConverter();
        //
        //    if (previousSelectedOrderItem != null)
        //    {
        //        previousSelectedOrderItem.Background = (Brush)brush.ConvertFrom("#FFFFFF");
        //
        //        StackPanel previousSelectedStackPanel = (StackPanel)previousSelectedOrderItem.Children[0];
        //        Border previousSelectedBorder = (Border)previousSelectedOrderItem.Children[1];
        //        Label previousStatusLabel = (Label)previousSelectedBorder.Child;
        //
        //        foreach (Label childLabel in previousSelectedStackPanel.Children)
        //            childLabel.Foreground = (Brush)brush.ConvertFrom("#000000");
        //
        //        if (workOrders[workOrdersStackPanel.Children.IndexOf(previousSelectedOrderItem)].order_status_id == COMPANY_WORK_MACROS.PENDING_RFQ)
        //            previousSelectedBorder.Background = (Brush)brush.ConvertFrom("#FFA500");
        //        else if (workOrders[workOrdersStackPanel.Children.IndexOf(previousSelectedOrderItem)].order_status_id == COMPANY_WORK_MACROS.CONFIRMED_RFQ)
        //            previousSelectedBorder.Background = (Brush)brush.ConvertFrom("#008000");
        //        else
        //            previousSelectedBorder.Background = (Brush)brush.ConvertFrom("#FF0000");
        //
        //        previousStatusLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");
        //    }
        //
        //    currentSelectedOrderItem.Background = (Brush)brush.ConvertFrom("#105A97");
        //
        //    StackPanel currentSelectedStackPanel = (StackPanel)currentSelectedOrderItem.Children[0];
        //    Border currentSelectedBorder = (Border)currentSelectedOrderItem.Children[1];
        //    Label currentStatusLabel = (Label)currentSelectedBorder.Child;
        //
        //    foreach (Label childLabel in currentSelectedStackPanel.Children)
        //        childLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");
        //
        //    currentSelectedBorder.Background = (Brush)brush.ConvertFrom("#FFFFFF");
        //    currentStatusLabel.Foreground = (Brush)brush.ConvertFrom("#105A97");
        //}

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
            QuotationsPage workOrders = new QuotationsPage(ref loggedInUser);
            this.NavigationService.Navigate(workOrders);
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
        private void OnClosedWorkOrderWindow(object sender, EventArgs e)
        {
            if (!GetWorkOrders())
                return;

            SetWorkOrdersStackPanel();
            SetWorkOrdersGrid();
        }
        private void OnClickExportButton(object sender, RoutedEventArgs e)
        {
            ExcelExport excelExport = new ExcelExport(workOrdersGrid);
        }

        private void OnExpandExpander(object sender, RoutedEventArgs e)
        {
            if (currentExpander != null)
                previousExpander = currentExpander;

            currentExpander = (Expander)sender;

            if (previousExpander != currentExpander && previousExpander != null)
                previousExpander.IsExpanded = false;

            Grid currentGrid = (Grid)currentExpander.Parent;
            ColumnDefinition expanderColumn = currentGrid.ColumnDefinitions[2];
            //expanderColumn.Width = new GridLength(Width = 120);
            currentExpander.VerticalAlignment = VerticalAlignment.Top;
            expanderColumn.MaxWidth = 120;
        }

        private void OnCollapseExpander(object sender, RoutedEventArgs e)
        {
            Expander currentExpander = (Expander)sender;
            Grid currentGrid = (Grid)currentExpander.Parent;
            ColumnDefinition expanderColumn = currentGrid.ColumnDefinitions[2];
            currentExpander.VerticalAlignment = VerticalAlignment.Center;
            expanderColumn.MaxWidth = 50;
        }

        private void OnSelChangedListBox(object sender, SelectionChangedEventArgs e)
        {
            ListBox tempListBox = (ListBox)sender;
            if (tempListBox.SelectedItem != null)
            {
                ListBoxItem currentItem = (ListBoxItem)tempListBox.Items[tempListBox.SelectedIndex];
                Expander currentExpander = (Expander)tempListBox.Parent;

                currentGrid = (Grid)currentExpander.Parent;

                if (currentItem.Content.ToString() == "View")
                {
                    OnBtnClickView();
                }
                else if (currentItem.Content.ToString() == "View RFQ")
                {
                    OnBtnClickViewRFQ();
                }
                else if (currentItem.Content.ToString() == "View Offer")
                {
                    OnBtnClickViewOffer();
                }
                else if (currentItem.Content.ToString() == "Confirm Order")
                {
                    OnBtnClickConfirmOrder();
                }

                tempListBox.SelectedIndex = -1;
            }
        }
        private void OnBtnClickView()
        {
            int viewAddCondition = COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION;

            WorkOrder workOrder = new WorkOrder(sqlDatabase);

            workOrder.InitializeWorkOrderInfo(workOrdersAfterFiltering[workOrdersStackPanel.Children.IndexOf(currentGrid)].order_serial);
            WorkOrderWindow workOrderWindow = new WorkOrderWindow(ref loggedInUser, ref workOrder, viewAddCondition, false);

            workOrderWindow.Show();
        }
        private void OnBtnClickViewRFQ()
        {
            int viewAddCondition = COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION;

            WorkOrder workOrder = new WorkOrder(sqlDatabase);

            workOrder.InitializeWorkOrderInfo(workOrdersAfterFiltering[workOrdersStackPanel.Children.IndexOf(currentGrid)].order_serial);

            if (workOrder.GetRFQID() != null)
            {
                RFQ rfq = new RFQ(sqlDatabase);

                rfq.CopyRFQ(workOrder);

                RFQWindow rfqWindow = new RFQWindow(ref loggedInUser, ref rfq, viewAddCondition, false);
                rfqWindow.Show();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Selected work order doesn't have an associated RFQ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnBtnClickViewOffer()
        {
            int viewAddCondition = COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION;

            WorkOrder workOrder = new WorkOrder(sqlDatabase);

            workOrder.InitializeWorkOrderInfo(workOrdersAfterFiltering[workOrdersStackPanel.Children.IndexOf(currentGrid)].order_serial);

            if (workOrder.GetOfferID() != null)
            {
                OutgoingQuotation outgoingQuotation = new OutgoingQuotation(sqlDatabase);

                outgoingQuotation.CopyWorkOffer(workOrder);

                WorkOfferWindow workOfferWindow = new WorkOfferWindow(ref loggedInUser, ref outgoingQuotation, viewAddCondition, false);

                workOfferWindow.Show();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Selected work order doesn't have an associated offer", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void OnBtnClickConfirmOrder()
        {
            WorkOrder workOrder = new WorkOrder(sqlDatabase);

            workOrder.InitializeWorkOrderInfo(workOrdersAfterFiltering[workOrdersStackPanel.Children.IndexOf(currentGrid)].order_serial);

            workOrder.ConfirmOrder();

            if (!GetWorkOrders())
                return;

            SetWorkOrdersStackPanel();
            SetWorkOrdersGrid();
        }

        
    }
}
