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
    /// Interaction logic for MaintenanceContractsPage.xaml
    /// </summary>
    public partial class MaintenanceContractsPage : Page
    {
        private Employee loggedInUser;
        private SQLServer sqlDatabase;
        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;

        private MaintenanceContract selectedMaintContract;
        private int finalYear = Int32.Parse(DateTime.Now.Year.ToString());

        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> salesEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> preSalesEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_WORK_MACROS.WORK_ORDER_MAX_STRUCT> maintContracts = new List<COMPANY_WORK_MACROS.WORK_ORDER_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.WORK_ORDER_MAX_STRUCT> maintContractsAfterFiltering = new List<COMPANY_WORK_MACROS.WORK_ORDER_MAX_STRUCT>();
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
        public MaintenanceContractsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();
            loggedInUser = mLoggedInUser;
            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            selectedMaintContract = new MaintenanceContract(sqlDatabase);

            if (!GetMaintenanceContracts())
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

            SetMaintContractsStackPanel();
            SetMaintContractsGrid();
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //INTIALIZATION FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool GetMaintenanceContracts()
        {
            if (!commonQueriesObject.GetMaintenanceContracts(ref maintContracts))
                return false;
            return true;
        }
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


        private bool SetMaintContractsStackPanel()
        {
            maintContractsStackPanel.Children.Clear();

            maintContractsAfterFiltering.Clear();

            for (int i = 0; i < maintContracts.Count; i++)
            {
                DateTime currentMaintContractDate = DateTime.Parse(maintContracts[i].issue_date);

                bool salesPersonCondition = selectedSales != maintContracts[i].sales_person_id;
                bool assigneeCondition;

                if (selectedPreSales == maintContracts[i].offer_proposer_id || (selectedPreSales == maintContracts[i].sales_person_id))
                    assigneeCondition = true;
                else
                    assigneeCondition = false;

                bool productCondition = false;
                for (int productNo = 0; productNo < maintContracts[i].products.Count(); productNo++)
                    if (maintContracts[i].products[productNo].productType.typeId == selectedProduct)
                        productCondition |= true;

                bool brandCondition = false;
                for (int productNo = 0; productNo < maintContracts[i].products.Count(); productNo++)
                    if (maintContracts[i].products[productNo].productBrand.brandId == selectedBrand)
                        brandCondition |= true;


                if (yearCheckBox.IsChecked == true && currentMaintContractDate.Year != selectedYear)
                    continue;

                if (salesCheckBox.IsChecked == true && salesPersonCondition)
                    continue;

                if (preSalesCheckBox.IsChecked == true && !assigneeCondition)
                    continue;

                if (quarterCheckBox.IsChecked == true && commonFunctionsObject.GetQuarter(currentMaintContractDate) != selectedQuarter)
                    continue;

                if (productCheckBox.IsChecked == true && !productCondition)
                    continue;

                if (brandCheckBox.IsChecked == true && !brandCondition)
                    continue;

                if (statusCheckBox.IsChecked == true && maintContracts[i].order_status_id != selectedStatus)
                    continue;

                maintContractsAfterFiltering.Add(maintContracts[i]);

                StackPanel fullStackPanel = new StackPanel();
                fullStackPanel.Orientation = Orientation.Vertical;
                //fullStackPanel.Height = 90;


                Label offerIdLabel = new Label();
                offerIdLabel.Content = maintContracts[i].order_id;
                offerIdLabel.Style = (Style)FindResource("stackPanelItemHeader");

                Label salesLabel = new Label();
                salesLabel.Content = maintContracts[i].sales_person_name;
                salesLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label preSalesLabel = new Label();
                preSalesLabel.Content = maintContracts[i].offer_proposer_name;
                preSalesLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label companyAndContactLabel = new Label();
                companyAndContactLabel.Content = maintContracts[i].company_name + " -" + maintContracts[i].contact_name;
                companyAndContactLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label productTypeAndBrandLabel = new Label();
                List<COMPANY_WORK_MACROS.ORDER_PRODUCT_STRUCT> temp = maintContracts[i].products;

                for (int j = 0; j < temp.Count(); j++)
                {
                    COMPANY_WORK_MACROS.PRODUCT_STRUCT tempType1 = temp[j].productType;
                    COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand1 = temp[j].productBrand;

                    productTypeAndBrandLabel.Content += tempType1.typeName + " -" + tempBrand1.brandName;

                    if (j != temp.Count() - 1)
                        productTypeAndBrandLabel.Content += ", ";
                }
                productTypeAndBrandLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label contractTypeLabel = new Label();
                contractTypeLabel.Content = maintContracts[i].contract_type;
                contractTypeLabel.Style = (Style)FindResource("stackPanelItemBody");

                Border borderIcon = new Border();
                borderIcon.Style = (Style)FindResource("BorderIcon");

                Label contractStatusLabel = new Label();
                contractStatusLabel.Content = maintContracts[i].order_status;
                contractStatusLabel.Style = (Style)FindResource("BorderIconTextLabel");


                if (maintContracts[i].order_status_id == COMPANY_WORK_MACROS.PENDING_OUTGOING_QUOTATION)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500"));
                }
                else if (maintContracts[i].order_status_id == COMPANY_WORK_MACROS.CONFIRMED_OUTGOING_QUOTATION)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008000"));
                }
                else
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                }

                borderIcon.Child = contractStatusLabel;

                Expander expander = new Expander();
                expander.ExpandDirection = ExpandDirection.Down;
                expander.VerticalAlignment = VerticalAlignment.Center;
                expander.HorizontalAlignment = HorizontalAlignment.Center;
                expander.HorizontalContentAlignment = HorizontalAlignment.Center;
                expander.Expanded += new RoutedEventHandler(OnExpandExpander);
                expander.Collapsed += new RoutedEventHandler(OnCollapseExpander);

                ListBox listBox = new ListBox();
                listBox.HorizontalContentAlignment = HorizontalAlignment.Stretch;
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


                if (maintContracts[i].order_status_id != COMPANY_WORK_MACROS.CLOSED_WORK_ORDER && loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID)
                {
                    ListBoxItem confirmOrderButton = new ListBoxItem();
                    confirmOrderButton.Content = "Confirm MaintContract";
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

                maintContractsStackPanel.Children.Add(grid);
            }

            return true;
        }

        private bool SetMaintContractsGrid()
        {

            maintContractsGrid.Children.Clear();
            maintContractsGrid.RowDefinitions.Clear();
            maintContractsGrid.ColumnDefinitions.Clear();

            int counter = 0;

            Label orderIdHeader = new Label();
            orderIdHeader.Content = "MaintContract ID";
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
            orderStatusHeader.Content = "MaintContract Status";
            orderStatusHeader.Style = (Style)FindResource("tableSubHeaderItem");

            maintContractsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            maintContractsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            maintContractsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            maintContractsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            maintContractsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            maintContractsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            maintContractsGrid.ColumnDefinitions.Add(new ColumnDefinition());

            maintContractsGrid.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(orderIdHeader, 0);
            Grid.SetColumn(orderIdHeader, 0);
            maintContractsGrid.Children.Add(orderIdHeader);

            Grid.SetRow(orderSalesHeader, 0);
            Grid.SetColumn(orderSalesHeader, 1);
            maintContractsGrid.Children.Add(orderSalesHeader);

            Grid.SetRow(orderPreSalesHeader, 0);
            Grid.SetColumn(orderPreSalesHeader, 2);
            maintContractsGrid.Children.Add(orderPreSalesHeader);

            Grid.SetRow(orderCompanyContactHeader, 0);
            Grid.SetColumn(orderCompanyContactHeader, 3);
            maintContractsGrid.Children.Add(orderCompanyContactHeader);

            Grid.SetRow(orderProductsHeader, 0);
            Grid.SetColumn(orderProductsHeader, 4);
            maintContractsGrid.Children.Add(orderProductsHeader);

            Grid.SetRow(orderContractTypeHeader, 0);
            Grid.SetColumn(orderContractTypeHeader, 5);
            maintContractsGrid.Children.Add(orderContractTypeHeader);

            Grid.SetRow(orderStatusHeader, 0);
            Grid.SetColumn(orderStatusHeader, 6);
            maintContractsGrid.Children.Add(orderStatusHeader);

            int currentRowNumber = 1;


            for (int i = 0; i < maintContracts.Count; i++)
            {
                DateTime currentMaintContractDate = DateTime.Parse(maintContracts[i].issue_date);

                bool salesPersonCondition = selectedSales != maintContracts[i].sales_person_id;

                bool assigneeCondition;

                if (selectedPreSales == maintContracts[i].offer_proposer_id || (selectedPreSales == maintContracts[i].sales_person_id))
                    assigneeCondition = true;
                else
                    assigneeCondition = false;

                bool productCondition = false;
                for (int productNo = 0; productNo < maintContracts[i].products.Count(); productNo++)
                    if (maintContracts[i].products[productNo].productType.typeId == selectedProduct)
                        productCondition |= true;

                bool brandCondition = false;
                for (int productNo = 0; productNo < maintContracts[i].products.Count(); productNo++)
                    if (maintContracts[i].products[productNo].productBrand.brandId == selectedBrand)
                        brandCondition |= true;


                if (yearCheckBox.IsChecked == true && currentMaintContractDate.Year != selectedYear)
                    continue;

                if (salesCheckBox.IsChecked == true && salesPersonCondition)
                    continue;

                if (preSalesCheckBox.IsChecked == true && !assigneeCondition)
                    continue;

                if (quarterCheckBox.IsChecked == true && commonFunctionsObject.GetQuarter(currentMaintContractDate) != selectedQuarter)
                    continue;

                if (productCheckBox.IsChecked == true && !productCondition)
                    continue;

                if (brandCheckBox.IsChecked == true && !brandCondition)
                    continue;

                if (statusCheckBox.IsChecked == true && maintContracts[i].order_status_id != selectedStatus)
                    continue;

                RowDefinition currentRow = new RowDefinition();
                maintContractsGrid.RowDefinitions.Add(currentRow);

                Label orderIdLabel = new Label();
                orderIdLabel.Content = maintContracts[i].order_id;
                orderIdLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(orderIdLabel, currentRowNumber);
                Grid.SetColumn(orderIdLabel, 0);
                maintContractsGrid.Children.Add(orderIdLabel);


                Label salesLabel = new Label();
                salesLabel.Content = maintContracts[i].sales_person_name;
                salesLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(salesLabel, currentRowNumber);
                Grid.SetColumn(salesLabel, 1);
                maintContractsGrid.Children.Add(salesLabel);

                Label preSalesLabel = new Label();
                preSalesLabel.Content = maintContracts[i].offer_proposer_name;
                preSalesLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(preSalesLabel, currentRowNumber);
                Grid.SetColumn(preSalesLabel, 2);
                maintContractsGrid.Children.Add(preSalesLabel);

                Label companyAndContactLabel = new Label();
                companyAndContactLabel.Content = maintContracts[i].company_name + " - " + maintContracts[i].contact_name;
                companyAndContactLabel.Style = (Style)FindResource("tableSubItemLabel");

                maintContractsGrid.Children.Add(companyAndContactLabel);
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


                List<COMPANY_WORK_MACROS.ORDER_PRODUCT_STRUCT> temp = maintContracts[i].products;

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

                maintContractsGrid.Children.Add(productGrid);
                Grid.SetRow(productGrid, currentRowNumber);
                Grid.SetColumn(productGrid, 3);



                Label contractTypeLabel = new Label();
                contractTypeLabel.Content = maintContracts[i].contract_type;
                contractTypeLabel.Style = (Style)FindResource("tableSubItemLabel");

                maintContractsGrid.Children.Add(contractTypeLabel);
                Grid.SetRow(contractTypeLabel, currentRowNumber);
                Grid.SetColumn(contractTypeLabel, 4);

                Label contractStatusLabel = new Label();
                contractStatusLabel.Content = maintContracts[i].order_status;
                contractStatusLabel.Style = (Style)FindResource("tableSubItemLabel");

                maintContractsGrid.Children.Add(contractStatusLabel);
                Grid.SetRow(contractStatusLabel, currentRowNumber);
                Grid.SetColumn(contractStatusLabel, 6);


                //currentRow.MouseLeftButtonDown += OnBtnClickWorkorderItem;

                currentRowNumber++;


            }

            return true;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SELECTION CHANGED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnSelChangedYearCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            ////DisableReviseButton();

            if (yearComboBox.SelectedItem != null)
                selectedYear = BASIC_MACROS.CRM_START_YEAR + yearComboBox.SelectedIndex;
            else
                selectedYear = 0;

            //SetMaintContractsStackPanel();
            //SetMaintContractsGrid();
        }

        private void OnSelChangedQuarterCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (quarterComboBox.SelectedItem != null)
                selectedQuarter = quarterComboBox.SelectedIndex + 1;
            else
                selectedQuarter = 0;

            //SetMaintContractsStackPanel();
            //SetMaintContractsGrid();
        }

        private void OnSelChangedSalesCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (salesComboBox.SelectedItem != null)
                selectedSales = salesEmployeesList[salesComboBox.SelectedIndex].employee_id;
            else
                selectedSales = 0;



            //SetMaintContractsStackPanel();
            //SetMaintContractsGrid();
        }
        private void OnSelChangedPreSalesCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (preSalesComboBox.SelectedItem != null)
                selectedPreSales = preSalesEmployeesList[preSalesComboBox.SelectedIndex].employee_id;
            else
                selectedPreSales = 0;



            //SetMaintContractsStackPanel();
            //SetMaintContractsGrid();
        }

        private void OnSelChangedProductCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (productComboBox.SelectedItem != null)
                selectedProduct = productTypes[productComboBox.SelectedIndex].typeId;
            else
                selectedProduct = 0;

            //SetMaintContractsStackPanel();
            //SetMaintContractsGrid();
        }

        private void OnSelChangedBrandCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (brandComboBox.SelectedItem != null)
                selectedBrand = brandTypes[brandComboBox.SelectedIndex].brandId;
            else
                selectedBrand = 0;

            //SetMaintContractsStackPanel();
            //SetMaintContractsGrid();
        }

        private void OnSelChangedStatusCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (statusComboBox.SelectedItem != null)
                selectedStatus = orderStatuses[statusComboBox.SelectedIndex].status_id;
            else
                selectedStatus = 0;

            //SetMaintContractsStackPanel();
            //SetMaintContractsGrid();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CHECKED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
            WorkOrdersPage workOrdersPage = new WorkOrdersPage(ref loggedInUser);
            this.NavigationService.Navigate(workOrdersPage);
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
        private void OnClosedMaintContractWindow(object sender, EventArgs e)
        {
            if (!GetMaintenanceContracts())
                return;

            SetMaintContractsStackPanel();
            SetMaintContractsGrid();
        }
        private void OnClickExportButton(object sender, RoutedEventArgs e)
        {
            ExcelExport excelExport = new ExcelExport(maintContractsGrid);
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
                else if (currentItem.Content.ToString() == "Confirm MaintContract")
                {
                    OnBtnClickConfirmOrder();
                }

                tempListBox.SelectedIndex = -1;
            }
        }
        private void OnBtnClickView()
        {
            int viewAddCondition = COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION;

            MaintenanceContract MaintContract = new MaintenanceContract(sqlDatabase);

            //MaintContract.InitializeMaintContractInfo(maintContractsAfterFiltering[maintContractsStackPanel.Children.IndexOf(currentGrid)].order_serial);
            MaintenanceContractsWindow workOrderWindow = new MaintenanceContractsWindow(ref loggedInUser, ref MaintContract, viewAddCondition, false);

            workOrderWindow.Show();
        }
        private void OnBtnClickViewRFQ()
        {
            int viewAddCondition = COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION;

            MaintenanceContract MaintContract = new MaintenanceContract(sqlDatabase);

            //MaintContract.InitializeMaintContractInfo(maintContractsAfterFiltering[maintContractsStackPanel.Children.IndexOf(currentGrid)].order_serial);

            if (MaintContract.GetRFQID() != null)
            {
                RFQ rfq = new RFQ(sqlDatabase);

                rfq.CopyRFQ(MaintContract);

                RFQWindow rfqWindow = new RFQWindow(ref loggedInUser, ref rfq, viewAddCondition, false);
                rfqWindow.Show();
            }
            else
            {
                MessageBox.Show("Selected MaintenanceContract doesn't have an RFQ");
            }
        }

        private void OnBtnClickViewOffer()
        {
            int viewAddCondition = COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION;

            MaintenanceContract MaintContract = new MaintenanceContract(sqlDatabase);

            //MaintContract.InitializeMaintContractInfo(maintContractsAfterFiltering[maintContractsStackPanel.Children.IndexOf(currentGrid)].order_serial);

            if (MaintContract.GetMaintOfferID() != null)
            {
                MaintenanceOffer outgoingQuotation = new MaintenanceOffer(sqlDatabase);

                outgoingQuotation.CopyMaintOffer(MaintContract);

                MaintenanceOffersWindow workOfferWindow = new MaintenanceOffersWindow(ref loggedInUser, ref outgoingQuotation, viewAddCondition, false);

                workOfferWindow.Show();
            }
            else
            {
                MessageBox.Show("Selected MaintenanceContract doesn't have an Offer");
            }
        }
        private void OnBtnClickConfirmOrder()
        {
            MaintenanceContract MaintContract = new MaintenanceContract(sqlDatabase);

            //MaintContract.InitializeMaintContractInfo(maintContractsAfterFiltering[maintContractsStackPanel.Children.IndexOf(currentGrid)].order_serial);

            MaintContract.ConfirmMaintContract();

            if (GetMaintenanceContracts())
                return;

            SetMaintContractsStackPanel();
            SetMaintContractsGrid();
        }
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

        private void OnBtnClickAdd(object sender, RoutedEventArgs e)
        {
            viewAddCondition = COMPANY_WORK_MACROS.OUTGOING_QUOTATION_ADD_CONDITION;

            selectedMaintContract = new MaintenanceContract(sqlDatabase);

            MaintenanceContractsWindow workOfferWindow = new MaintenanceContractsWindow(ref loggedInUser, ref selectedMaintContract, viewAddCondition, false);

            workOfferWindow.Closed += OnClosedMaintContractWindow;
            workOfferWindow.Show();
        }
    }
}
