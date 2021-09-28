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
using _01electronics_library;

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

        private int finalYear = Int32.Parse(DateTime.Now.Year.ToString());

        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> salesEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> preSalesEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_WORK_MACROS.WORK_ORDER_MAX_STRUCT> workOrders = new List<COMPANY_WORK_MACROS.WORK_ORDER_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.WORK_ORDER_MAX_STRUCT> workOrdersAfterFiltering = new List<COMPANY_WORK_MACROS.WORK_ORDER_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> productTypes = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
        private List<COMPANY_WORK_MACROS.BRAND_STRUCT> brandTypes = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();

        private int selectedYear;
        private int selectedQuarter;

        private int selectedSales;
        private int selectedPreSales;

        private int selectedProduct;
        private int selectedBrand;
        private int selectedStatus;
        private int salesPersonTeam;

        private Grid previousSelectedOrderItem;
        private Grid currentSelectedOrderItem;
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

            statusComboBox.Items.Add("Failed");
            statusComboBox.Items.Add("Confirmed");
            statusComboBox.Items.Add("Pending");
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
            viewButton.IsEnabled = false;
        }
        private void EnableViewButton()
        {
            viewButton.IsEnabled = true;
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

            for (int i = 0; i < workOrders.Count; i++)
            {
                DateTime currentWorkOrderDate = DateTime.Parse(workOrders[i].issue_date);

                bool salesPersonCondition = selectedSales != workOrders[i].sales_person_id;

                bool assigneeCondition = selectedPreSales != workOrders[i].offer_proposer_id;

                bool productCondition = false;
                for (int productNo = 0; productNo < workOrders[i].products.Count(); productNo++)
                    if (workOrders[i].products[productNo].productType.typeId == selectedProduct)
                        productCondition |= true;

                bool brandCondition = false;
                for (int productNo = 0; productNo < workOrders[i].products.Count(); productNo++)
                    if (workOrders[i].products[productNo].productBrand.brandId == selectedBrand)
                        brandCondition |= true;


                if (yearCheckBox.IsChecked == true && currentWorkOrderDate.Year != selectedYear)
                    continue;

                if (salesCheckBox.IsChecked == true && salesPersonCondition)
                    continue;

                if (preSalesCheckBox.IsChecked == true && assigneeCondition)
                    continue;

                if (quarterCheckBox.IsChecked == true && commonFunctionsObject.GetQuarter(currentWorkOrderDate) != selectedQuarter)
                    continue;

                if (productCheckBox.IsChecked == true && !productCondition)
                    continue;

                if (brandCheckBox.IsChecked == true && !brandCondition)
                    continue;

                if (statusCheckBox.IsChecked == true && workOrders[i].order_status_id != selectedStatus)
                    continue;

                StackPanel fullStackPanel = new StackPanel();
                fullStackPanel.Orientation = Orientation.Vertical;
                //fullStackPanel.Height = 90;

                workOrdersAfterFiltering.Add(workOrders[i]);

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
                companyAndContactLabel.Content = workOrders[i].company_name + " -" + workOrders[i].contact_name;
                companyAndContactLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label productTypeAndBrandLabel = new Label();
                List<COMPANY_WORK_MACROS.ORDER_PRODUCT_STRUCT> temp = workOrders[i].products;

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
                contractTypeLabel.Content = workOrders[i].contract_type;
                contractTypeLabel.Style = (Style)FindResource("stackPanelItemBody");

                Border borderIcon = new Border();
                borderIcon.Style = (Style)FindResource("BorderIcon");

                Label rfqStatusLabel = new Label();
                rfqStatusLabel.Content = workOrders[i].order_status;
                rfqStatusLabel.Style = (Style)FindResource("BorderIconTextLabel");

                if (workOrders[i].order_status_id == COMPANY_WORK_MACROS.PENDING_WORK_OFFER)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500"));
                }
                else if (workOrders[i].order_status_id == COMPANY_WORK_MACROS.CONFIRMED_WORK_OFFER)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008000"));
                }
                else
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                }

                borderIcon.Child = rfqStatusLabel;

                fullStackPanel.Children.Add(offerIdLabel);
                fullStackPanel.Children.Add(salesLabel);
                fullStackPanel.Children.Add(preSalesLabel);
                fullStackPanel.Children.Add(companyAndContactLabel);
                fullStackPanel.Children.Add(productTypeAndBrandLabel);
                fullStackPanel.Children.Add(contractTypeLabel);

                Grid grid = new Grid();
                ColumnDefinition column1 = new ColumnDefinition();
                ColumnDefinition column2 = new ColumnDefinition();
                column2.MaxWidth = 77;
                grid.ColumnDefinitions.Add(column1);
                grid.ColumnDefinitions.Add(column2);

                Grid.SetColumn(fullStackPanel, 0);
                Grid.SetColumn(borderIcon, 1);

                grid.Children.Add(fullStackPanel);
                grid.Children.Add(borderIcon);
                grid.MouseLeftButtonDown += OnBtnClickedWorkOfferItem;
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

            Label offerIdHeader = new Label();
            offerIdHeader.Content = "Offer ID";
            offerIdHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label offerSalesHeader = new Label();
            offerSalesHeader.Content = "Sales Engineer";
            offerSalesHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label offerPreSalesHeader = new Label();
            offerPreSalesHeader.Content = "Pre-Sales Engineer";
            offerPreSalesHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label offerCompanyContactHeader = new Label();
            offerCompanyContactHeader.Content = "Contact Info";
            offerCompanyContactHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label offerProductsHeader = new Label();
            offerProductsHeader.Content = "Products";
            offerProductsHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label offerContractTypeHeader = new Label();
            offerContractTypeHeader.Content = "Contract Type";
            offerContractTypeHeader.Style = (Style)FindResource("tableSubHeaderItem");

            Label offerStatusHeader = new Label();
            offerStatusHeader.Content = "Offer Status";
            offerStatusHeader.Style = (Style)FindResource("tableSubHeaderItem");

            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOrdersGrid.ColumnDefinitions.Add(new ColumnDefinition());

            workOrdersGrid.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(offerIdHeader, 0);
            Grid.SetColumn(offerIdHeader, 0);
            workOrdersGrid.Children.Add(offerIdHeader);

            Grid.SetRow(offerSalesHeader, 0);
            Grid.SetColumn(offerSalesHeader, 1);
            workOrdersGrid.Children.Add(offerSalesHeader);

            Grid.SetRow(offerPreSalesHeader, 0);
            Grid.SetColumn(offerPreSalesHeader, 2);
            workOrdersGrid.Children.Add(offerPreSalesHeader);

            Grid.SetRow(offerCompanyContactHeader, 0);
            Grid.SetColumn(offerCompanyContactHeader, 3);
            workOrdersGrid.Children.Add(offerCompanyContactHeader);

            Grid.SetRow(offerProductsHeader, 0);
            Grid.SetColumn(offerProductsHeader, 4);
            workOrdersGrid.Children.Add(offerProductsHeader);

            Grid.SetRow(offerContractTypeHeader, 0);
            Grid.SetColumn(offerContractTypeHeader, 5);
            workOrdersGrid.Children.Add(offerContractTypeHeader);

            Grid.SetRow(offerStatusHeader, 0);
            Grid.SetColumn(offerStatusHeader, 6);
            workOrdersGrid.Children.Add(offerStatusHeader);

            int currentRowNumber = 1;


            for (int i = 0; i < workOrders.Count; i++)
            {
                DateTime currentWorkOrderDate = DateTime.Parse(workOrders[i].issue_date);

                bool salesPersonCondition = selectedSales != workOrders[i].sales_person_id;

                bool assigneeCondition = selectedPreSales != workOrders[i].offer_proposer_id;

                bool productCondition = false;
                for (int productNo = 0; productNo < workOrders[i].products.Count(); productNo++)
                    if (workOrders[i].products[productNo].productType.typeId == selectedProduct)
                        productCondition |= true;

                bool brandCondition = false;
                for (int productNo = 0; productNo < workOrders[i].products.Count(); productNo++)
                    if (workOrders[i].products[productNo].productBrand.brandId == selectedBrand)
                        brandCondition |= true;


                if (yearCheckBox.IsChecked == true && currentWorkOrderDate.Year != selectedYear)
                    continue;

                if (salesCheckBox.IsChecked == true && salesPersonCondition)
                    continue;

                if (preSalesCheckBox.IsChecked == true && assigneeCondition)
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

                Label offerIdLabel = new Label();
                offerIdLabel.Content = workOrders[i].order_id;
                offerIdLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(offerIdLabel, currentRowNumber);
                Grid.SetColumn(offerIdLabel, 0);
                workOrdersGrid.Children.Add(offerIdLabel);


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


                Border borderIcon = new Border();
                borderIcon.Style = (Style)FindResource("BorderIcon");

                Label rfqStatusLabel = new Label();
                rfqStatusLabel.Content = workOrders[i].order_status;
                rfqStatusLabel.Style = (Style)FindResource("BorderIconTextLabel");

                if (workOrders[i].order_status_id == COMPANY_WORK_MACROS.OPEN_WORK_ORDER)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500"));
                }
                else if (workOrders[i].order_status_id == COMPANY_WORK_MACROS.CLOSED_WORK_ORDER)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                }
                //else
                //{
                //    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                //}

                borderIcon.Child = rfqStatusLabel;

                workOrdersGrid.Children.Add(borderIcon);
                Grid.SetRow(borderIcon, currentRowNumber);
                Grid.SetColumn(borderIcon, 6);

                currentRow.MouseLeftButtonDown += OnBtnClickedWorkOfferItem;

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
                selectedStatus = statusComboBox.SelectedIndex + 1;
            else
                selectedStatus = 0;

            SetWorkOrdersGrid();
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
        //BTN CLICKED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        private void OnBtnClickedAdd(object sender, RoutedEventArgs e)
        {
            int viewAddCondition = COMPANY_WORK_MACROS.OFFER_ADD_CONDITION;
            WorkOffer workOffer = new WorkOffer(sqlDatabase);

            WorkOfferWindow workOfferWindow = new WorkOfferWindow(ref loggedInUser, ref workOffer, viewAddCondition);
            workOfferWindow.Show();
        }

        private void OnBtnClickedView(object sender, RoutedEventArgs e)
        {
            WorkOffer selectedWorkOffer = new WorkOffer(sqlDatabase);

            commonQueriesObject.GetEmployeeTeam(workOrdersAfterFiltering[workOrdersStackPanel.Children.IndexOf(currentSelectedOrderItem)].sales_person_id, ref salesPersonTeam);


            //if (salesPersonTeam == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
            //{
            ////    selectedWorkOffer.InitializeSalesWorkOfferInfo(workOrdersAfterFiltering[workOrdersStackPanel.Children.IndexOf(currentSelectedOrderItem)].order_serial,
            ////                                                    workOrdersAfterFiltering[workOrdersStackPanel.Children.IndexOf(currentSelectedOrderItem)].order_version,
            ////                                                    workOrdersAfterFiltering[workOrdersStackPanel.Children.IndexOf(currentSelectedOrderItem)].offer_proposer_id);
            ////}
            //else
            //{
            ////    selectedWorkOffer.InitializeTechnicalOfficeWorkOfferInfo(workOrdersAfterFiltering[workOrdersStackPanel.Children.IndexOf(currentSelectedOrderItem)].order_serial,
            ////                                                    workOrdersAfterFiltering[workOrdersStackPanel.Children.IndexOf(currentSelectedOrderItem)].order_version,
            ////                                                    workOrdersAfterFiltering[workOrdersStackPanel.Children.IndexOf(currentSelectedOrderItem)].offer_proposer_id);
            //}

            int viewAddCondition = COMPANY_WORK_MACROS.OFFER_VIEW_CONDITION;
            WorkOfferWindow viewOffer = new WorkOfferWindow(ref loggedInUser, ref selectedWorkOffer, viewAddCondition);
            viewOffer.Show();
        }

        private void OnBtnClickedWorkOfferItem(object sender, RoutedEventArgs e)
        {
            EnableViewButton();
            //EnableReviseButton();
            previousSelectedOrderItem = currentSelectedOrderItem;
            currentSelectedOrderItem = (Grid)sender;
            BrushConverter brush = new BrushConverter();

            if (previousSelectedOrderItem != null)
            {
                previousSelectedOrderItem.Background = (Brush)brush.ConvertFrom("#FFFFFF");

                StackPanel previousSelectedStackPanel = (StackPanel)previousSelectedOrderItem.Children[0];
                Border previousSelectedBorder = (Border)previousSelectedOrderItem.Children[1];
                Label previousStatusLabel = (Label)previousSelectedBorder.Child;

                foreach (Label childLabel in previousSelectedStackPanel.Children)
                    childLabel.Foreground = (Brush)brush.ConvertFrom("#000000");

                if (workOrders[workOrdersStackPanel.Children.IndexOf(previousSelectedOrderItem)].order_status_id == COMPANY_WORK_MACROS.PENDING_RFQ)
                    previousSelectedBorder.Background = (Brush)brush.ConvertFrom("#FFA500");
                else if (workOrders[workOrdersStackPanel.Children.IndexOf(previousSelectedOrderItem)].order_status_id == COMPANY_WORK_MACROS.CONFIRMED_RFQ)
                    previousSelectedBorder.Background = (Brush)brush.ConvertFrom("#008000");
                else
                    previousSelectedBorder.Background = (Brush)brush.ConvertFrom("#FF0000");

                previousStatusLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");
            }

            currentSelectedOrderItem.Background = (Brush)brush.ConvertFrom("#105A97");

            StackPanel currentSelectedStackPanel = (StackPanel)currentSelectedOrderItem.Children[0];
            Border currentSelectedBorder = (Border)currentSelectedOrderItem.Children[1];
            Label currentStatusLabel = (Label)currentSelectedBorder.Child;

            foreach (Label childLabel in currentSelectedStackPanel.Children)
                childLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");

            currentSelectedBorder.Background = (Brush)brush.ConvertFrom("#FFFFFF");
            currentStatusLabel.Foreground = (Brush)brush.ConvertFrom("#105A97");
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
            WorkOffersPage workOrders = new WorkOffersPage(ref loggedInUser);
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

        private void OnClickExportButton(object sender, RoutedEventArgs e)
        {
            ExcelExport excelExport = new ExcelExport(workOrdersGrid);
        }

    }
}
