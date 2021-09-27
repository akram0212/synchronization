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
    /// Interaction logic for RFQsPage.xaml
    /// </summary>
    public partial class RFQsPage : Page
    {
        private Employee loggedInUser;

        private SQLServer sqlDatabase;
        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;

        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> salesEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> preSalesEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsList = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.RFQ_BASIC_STRUCT> stackPanelItems = new List<COMPANY_WORK_MACROS.RFQ_BASIC_STRUCT>();
        private List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> productTypes = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
        private List<COMPANY_WORK_MACROS.BRAND_STRUCT> brandTypes = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();

        private int selectedYear;
        private int selectedQuarter;

        private int selectedSales;
        private int selectedPreSales;

        private int selectedTeam;
        private int selectedProduct;
        private int selectedBrand;
        private int selectedStatus;

        private Grid previousSelectedRFQItem;
        private Grid currentSelectedRFQItem;

        public RFQsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            if (!GetRFQs())
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

            SetRFQsStackPanel();
            SetRFQsGrid();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //GET DATA FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool GetRFQs()
        {
            if (!commonQueriesObject.GetRFQs(ref rfqsList))
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

            return true;
        }

        private void InitializeStatusComboBox()
        {
            statusComboBox.Items.Add("Pending");
            statusComboBox.Items.Add("Confirmed");
            statusComboBox.Items.Add("Failed");
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

            productComboBox.SelectedIndex = -1;
            brandComboBox.SelectedIndex = -1;

            statusComboBox.SelectedIndex = -1;
        }

        private void DisableComboBoxes()
        {
            yearComboBox.IsEnabled = false;
            quarterComboBox.IsEnabled = false;
            salesComboBox.IsEnabled = false;
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

        private void DisableReviseButton()
        {
            reviseButton.IsEnabled = false;
        }
        private void EnableReviseButton()
        {
            reviseButton.IsEnabled = true;
        }

        private void EnableResolveButton()
        {
            resolveButton.IsEnabled = true;
        }

        private void DisableResolveButton()
        {
            resolveButton.IsEnabled = false;
        }

        private void SetDefaultSettings()
        {
            DisableViewButton();
            DisableReviseButton();
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

        private void SetSalesPersonComboBox()
        {
            salesComboBox.SelectedIndex = 0;

            for (int i = 0; i < salesEmployeesList.Count; i++)
                if (loggedInUser.GetEmployeeId() == salesEmployeesList[i].employee_id)
                    salesComboBox.SelectedIndex = i;
        }

        private void SetPreSalesEngineerComboBox()
        {
            preSalesComboBox.SelectedIndex = 0;

            for (int i = 0; i < preSalesEmployeesList.Count; i++)
                if (loggedInUser.GetEmployeeId() == preSalesEmployeesList[i].employee_id)
                    preSalesComboBox.SelectedIndex = i;
        }

        private bool SetRFQsStackPanel()
        {
            DisableReviseButton();
            DisableViewButton();
            DisableResolveButton();

            RFQsStackPanel.Children.Clear();

            currentSelectedRFQItem = null;

            if (stackPanelItems.Count() != 0)
                stackPanelItems.Clear();

            for (int i = 0; i < rfqsList.Count; i++)
            {
                DateTime currentRFQDate = DateTime.Parse(rfqsList[i].issue_date);

                bool salesPersonCondition = selectedSales != rfqsList[i].sales_person_id;

                bool assigneeCondition = selectedPreSales != rfqsList[i].assignee_id;

                bool productCondition = false;
                for (int productNo = 0; productNo < COMPANY_WORK_MACROS.MAX_RFQ_PRODUCTS; productNo++)
                    if (rfqsList[i].products[productNo].productType.typeId == selectedProduct)
                        productCondition |= true;

                bool brandCondition = false;
                for (int productNo = 0; productNo < COMPANY_WORK_MACROS.MAX_RFQ_PRODUCTS; productNo++)
                    if (rfqsList[i].products[productNo].productBrand.brandId == selectedBrand)
                        brandCondition |= true;

                if (yearCheckBox.IsChecked == true && currentRFQDate.Year != selectedYear)
                    continue;

                if (salesCheckBox.IsChecked == true && salesPersonCondition)
                    continue;

                if (preSalesCheckBox.IsChecked == true && assigneeCondition)
                    continue;
            
                if (quarterCheckBox.IsChecked == true && commonFunctionsObject.GetQuarter(currentRFQDate) != selectedQuarter)
                    continue;

                if (productCheckBox.IsChecked == true && !productCondition)
                    continue;

                if (brandCheckBox.IsChecked == true && !brandCondition)
                    continue;

                if (statusCheckBox.IsChecked == true && rfqsList[i].rfq_status_id != selectedStatus)
                    continue;

                COMPANY_WORK_MACROS.RFQ_BASIC_STRUCT currentItem = new COMPANY_WORK_MACROS.RFQ_BASIC_STRUCT();
                currentItem.sales_person = rfqsList[i].sales_person_id;
                currentItem.rfq_serial = rfqsList[i].rfq_serial;
                currentItem.rfq_version = rfqsList[i].rfq_version;

                stackPanelItems.Add(currentItem);

                StackPanel currentStackPanel = new StackPanel();
                currentStackPanel.Orientation = Orientation.Vertical;

                Label rfqIDLabel = new Label();
                rfqIDLabel.Content = rfqsList[i].rfq_id;
                rfqIDLabel.Style = (Style)FindResource("stackPanelItemHeader");

                Label salesLabel = new Label();
                salesLabel.Content = rfqsList[i].sales_person_name;
                salesLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label preSalesLabel = new Label();
                preSalesLabel.Content = rfqsList[i].assignee_name;
                preSalesLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label companyAndContactLabel = new Label();
                companyAndContactLabel.Content = rfqsList[i].company_name + " -" + rfqsList[i].contact_name;
                companyAndContactLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label productTypeAndBrandLabel = new Label();
                for (int j = 0; j < COMPANY_WORK_MACROS.MAX_RFQ_PRODUCTS; j++)
                {
                    
                    List<COMPANY_WORK_MACROS.RFQ_PRODUCT_STRUCT> temp = rfqsList[i].products;
                    COMPANY_WORK_MACROS.PRODUCT_STRUCT tempType1 = temp[j].productType;
                    COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand1 = temp[j].productBrand;

                    if (tempType1.typeId == 0)
                        continue;

                    productTypeAndBrandLabel.Content += tempType1.typeName + " -" + tempBrand1.brandName;

                    if (j != COMPANY_WORK_MACROS.MAX_RFQ_PRODUCTS - 1)
                        productTypeAndBrandLabel.Content += ", ";
                }
                productTypeAndBrandLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label contractTypeLabel = new Label();
                contractTypeLabel.Content = rfqsList[i].contract_type;
                contractTypeLabel.Style = (Style)FindResource("stackPanelItemBody");

                Border borderIcon = new Border();
                borderIcon.Style = (Style)FindResource("BorderIcon");

                Label rfqStatusLabel = new Label();
                rfqStatusLabel.Content = rfqsList[i].rfq_status;
                rfqStatusLabel.Style = (Style)FindResource("BorderIconTextLabel");

                BrushConverter brush = new BrushConverter();
                if (rfqsList[i].rfq_status_id == COMPANY_WORK_MACROS.PENDING_RFQ)
                    borderIcon.Background = (Brush)brush.ConvertFrom("#FFA500");
                else if (rfqsList[i].rfq_status_id == COMPANY_WORK_MACROS.CONFIRMED_RFQ)
                    borderIcon.Background = (Brush)brush.ConvertFrom("#008000");
                else
                    borderIcon.Background = (Brush)brush.ConvertFrom("#FF0000");

                borderIcon.Child = rfqStatusLabel;

                currentStackPanel.Children.Add(rfqIDLabel);
                currentStackPanel.Children.Add(salesLabel);
                currentStackPanel.Children.Add(preSalesLabel);
                currentStackPanel.Children.Add(companyAndContactLabel);
                currentStackPanel.Children.Add(productTypeAndBrandLabel);
                currentStackPanel.Children.Add(contractTypeLabel);

                Grid currentGrid = new Grid();
                ColumnDefinition column1 = new ColumnDefinition();
                ColumnDefinition column2 = new ColumnDefinition();
                column2.MaxWidth = 95;
                currentGrid.ColumnDefinitions.Add(column1);
                currentGrid.ColumnDefinitions.Add(column2);

                Grid.SetColumn(currentStackPanel, 0);
                Grid.SetColumn(borderIcon, 1);

                currentGrid.Children.Add(currentStackPanel);
                currentGrid.Children.Add(borderIcon);
                currentGrid.MouseLeftButtonDown += OnBtnClickedRFQItem;
                RFQsStackPanel.Children.Add(currentGrid);
            }

            return true;
        }

        private bool SetRFQsGrid()
        {

            rfqsGrid.Children.Clear();
            rfqsGrid.RowDefinitions.Clear();
            rfqsGrid.ColumnDefinitions.Clear();

            currentSelectedRFQItem = null;

            Label offerIdHeader = new Label();
            offerIdHeader.Content = "RFQ ID";
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

            rfqsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            rfqsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            rfqsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            rfqsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            rfqsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            rfqsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            rfqsGrid.ColumnDefinitions.Add(new ColumnDefinition());

            rfqsGrid.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(offerIdHeader, 0);
            Grid.SetColumn(offerIdHeader, 0);
            rfqsGrid.Children.Add(offerIdHeader);

            Grid.SetRow(offerSalesHeader, 0);
            Grid.SetColumn(offerSalesHeader, 1);
            rfqsGrid.Children.Add(offerSalesHeader);

            Grid.SetRow(offerPreSalesHeader, 0);
            Grid.SetColumn(offerPreSalesHeader, 2);
            rfqsGrid.Children.Add(offerPreSalesHeader);

            Grid.SetRow(offerCompanyContactHeader, 0);
            Grid.SetColumn(offerCompanyContactHeader, 3);
            rfqsGrid.Children.Add(offerCompanyContactHeader);

            Grid.SetRow(offerProductsHeader, 0);
            Grid.SetColumn(offerProductsHeader, 4);
            rfqsGrid.Children.Add(offerProductsHeader);

            Grid.SetRow(offerContractTypeHeader, 0);
            Grid.SetColumn(offerContractTypeHeader, 5);
            rfqsGrid.Children.Add(offerContractTypeHeader);

            Grid.SetRow(offerStatusHeader, 0);
            Grid.SetColumn(offerStatusHeader, 6);
            rfqsGrid.Children.Add(offerStatusHeader);

            int currentRowNumber = 1;

            for (int i = 0; i < rfqsList.Count; i++)
            {
                DateTime currentWorkOfferDate = DateTime.Parse(rfqsList[i].issue_date);

                bool salesPersonCondition = selectedSales != rfqsList[i].sales_person_id;

                bool assigneeCondition = selectedPreSales != rfqsList[i].assignee_id;

                bool productCondition = false;
                for (int productNo = 0; productNo < rfqsList[i].products.Count(); productNo++)
                    if (rfqsList[i].products[productNo].productType.typeId == selectedProduct)
                        productCondition |= true;

                bool brandCondition = false;
                for (int productNo = 0; productNo < rfqsList[i].products.Count(); productNo++)
                    if (rfqsList[i].products[productNo].productBrand.brandId == selectedBrand)
                        brandCondition |= true;


                if (yearCheckBox.IsChecked == true && currentWorkOfferDate.Year != selectedYear)
                    continue;

                if (salesCheckBox.IsChecked == true && salesPersonCondition)
                    continue;

                if (preSalesCheckBox.IsChecked == true && assigneeCondition)
                    continue;

                if (quarterCheckBox.IsChecked == true && commonFunctionsObject.GetQuarter(currentWorkOfferDate) != selectedQuarter)
                    continue;

                if (productCheckBox.IsChecked == true && !productCondition)
                    continue;

                if (brandCheckBox.IsChecked == true && !brandCondition)
                    continue;

                if (statusCheckBox.IsChecked == true && rfqsList[i].rfq_status_id != selectedStatus)
                    continue;


                RowDefinition currentRow = new RowDefinition();
                rfqsGrid.RowDefinitions.Add(currentRow);

                Label offerIdLabel = new Label();
                offerIdLabel.Content = rfqsList[i].rfq_id;
                offerIdLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(offerIdLabel, currentRowNumber);
                Grid.SetColumn(offerIdLabel, 0);
                rfqsGrid.Children.Add(offerIdLabel);


                Label salesLabel = new Label();
                salesLabel.Content = rfqsList[i].sales_person_name;
                salesLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(salesLabel, currentRowNumber);
                Grid.SetColumn(salesLabel, 1);
                rfqsGrid.Children.Add(salesLabel);


                Label preSalesLabel = new Label();
                preSalesLabel.Content = rfqsList[i].assignee_name;
                preSalesLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(preSalesLabel, currentRowNumber);
                Grid.SetColumn(preSalesLabel, 2);
                rfqsGrid.Children.Add(preSalesLabel);


                Label companyAndContactLabel = new Label();
                companyAndContactLabel.Content = rfqsList[i].company_name + " - " + rfqsList[i].contact_name;
                companyAndContactLabel.Style = (Style)FindResource("tableSubItemLabel");

                rfqsGrid.Children.Add(companyAndContactLabel);
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


                List<COMPANY_WORK_MACROS.RFQ_PRODUCT_STRUCT> temp = rfqsList[i].products;

                for (int j = 0; j < temp.Count(); j++)
                {
                    COMPANY_WORK_MACROS.PRODUCT_STRUCT tempType1 = temp[j].productType;
                    COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand1 = temp[j].productBrand;
                    COMPANY_WORK_MACROS.MODEL_STRUCT tempModel1 = temp[j].productModel;

                    if (tempType1.typeId != 0)
                    {
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
                }

                rfqsGrid.Children.Add(productGrid);
                Grid.SetRow(productGrid, currentRowNumber);
                Grid.SetColumn(productGrid, 4);



                Label contractTypeLabel = new Label();
                contractTypeLabel.Content = rfqsList[i].contract_type;
                contractTypeLabel.Style = (Style)FindResource("tableSubItemLabel");

                rfqsGrid.Children.Add(contractTypeLabel);
                Grid.SetRow(contractTypeLabel, currentRowNumber);
                Grid.SetColumn(contractTypeLabel, 5);


                Border borderIcon = new Border();
                borderIcon.Style = (Style)FindResource("BorderIcon");

                Label rfqStatusLabel = new Label();
                rfqStatusLabel.Content = rfqsList[i].rfq_status;
                rfqStatusLabel.Style = (Style)FindResource("BorderIconTextLabel");

                if (rfqsList[i].rfq_status_id == COMPANY_WORK_MACROS.PENDING_WORK_OFFER)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500"));
                }
                else if (rfqsList[i].rfq_status_id == COMPANY_WORK_MACROS.CONFIRMED_RFQ)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008000"));
                }
                else
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                }

                borderIcon.Child = rfqStatusLabel;

                rfqsGrid.Children.Add(borderIcon);
                Grid.SetRow(borderIcon, currentRowNumber);
                Grid.SetColumn(borderIcon, 6);

                //currentRow.MouseLeftButtonDown += OnBtnClickedWorkOfferItem;

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
            DisableReviseButton();

            if (yearComboBox.SelectedItem != null)
                selectedYear = BASIC_MACROS.CRM_START_YEAR + yearComboBox.SelectedIndex;
            else
                selectedYear = 0;

            //currentSelectedRFQItem = null;
            SetRFQsStackPanel();
            SetRFQsGrid();
        }

        private void OnSelChangedQuarterCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            DisableReviseButton();

            if (quarterComboBox.SelectedItem != null)
                selectedQuarter = quarterComboBox.SelectedIndex + 1;
            else
                selectedQuarter = 0;

            //currentSelectedRFQItem = null;
            SetRFQsStackPanel();
            SetRFQsGrid();
        }

        private void OnSelChangedSalesCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            DisableReviseButton();

            if (salesComboBox.SelectedItem != null)
                selectedSales = salesEmployeesList[salesComboBox.SelectedIndex].employee_id;
            else
                selectedSales = 0;

            //currentSelectedRFQItem = null;
            SetRFQsStackPanel();
            SetRFQsGrid();
        }
        private void OnSelChangedPreSalesCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            DisableReviseButton();

            if (preSalesComboBox.SelectedItem != null)
                selectedPreSales = preSalesEmployeesList[preSalesComboBox.SelectedIndex].employee_id;
            else
                selectedPreSales = 0;

            //currentSelectedRFQItem = null;
            SetRFQsStackPanel();
            SetRFQsGrid();
        }

        private void OnSelChangedProductCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            DisableReviseButton();

            if (productComboBox.SelectedItem != null)
                selectedProduct = productTypes[productComboBox.SelectedIndex].typeId;
            else
                selectedProduct = 0;

            //currentSelectedRFQItem = null;
            SetRFQsStackPanel();
            SetRFQsGrid();
        }

        private void OnSelChangedBrandCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            DisableReviseButton();

            if (brandComboBox.SelectedItem != null)
                selectedBrand = brandTypes[brandComboBox.SelectedIndex].brandId;
            else
                selectedBrand = 0;

            //currentSelectedRFQItem = null;
            SetRFQsStackPanel();
            SetRFQsGrid();
        }

        private void OnSelChangedStatusCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            DisableReviseButton();

            if (statusComboBox.SelectedItem != null)
                selectedStatus = statusComboBox.SelectedIndex + 1;
            else
                selectedStatus = 0;

            //currentSelectedRFQItem = null;
            SetRFQsStackPanel();
            SetRFQsGrid();
        }

        private void OnClosedRFQWindow(object sender, EventArgs e)
        {
            if (!GetRFQs())
                return;
            SetRFQsStackPanel();
            SetRFQsGrid();
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
        private void OnCheckSalesCheckBox(object sender, RoutedEventArgs e)
        {
            salesComboBox.IsEnabled = true;
            SetSalesPersonComboBox();
        }
        private void OnCheckPreSalesCheckBox(object sender, RoutedEventArgs e)
        {
            preSalesComboBox.IsEnabled = true;
            SetPreSalesEngineerComboBox();
        }
        private void OnCheckProductCheckBox(object sender, RoutedEventArgs e)
        {
            productComboBox.IsEnabled = true;
        }
        private void OnCheckBrandCheckBox(object sender, RoutedEventArgs e)
        {
            brandComboBox.IsEnabled = true;
        }
        private void OnCheckStatusCheckBox(object sender, RoutedEventArgs e)
        {
            statusComboBox.IsEnabled = true;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //UNCHECKED HANDLERS FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void OnUncheckYearCheckBox(object sender, RoutedEventArgs e)
        {
            yearComboBox.IsEnabled = false;
            yearComboBox.SelectedItem = null;
            
            //currentSelectedRFQItem = null;
            //previousSelectedRFQItem = null;
        }
        private void OnUncheckQuarterCheckBox(object sender, RoutedEventArgs e)
        {
            quarterComboBox.IsEnabled = false;
            quarterComboBox.SelectedItem = null;

            //currentSelectedRFQItem = null;
            //previousSelectedRFQItem = null;
        }
        private void OnUncheckSalesCheckBox(object sender, RoutedEventArgs e)
        {
            salesComboBox.IsEnabled = false;
            salesComboBox.SelectedItem = null;

            //currentSelectedRFQItem = null;
            //previousSelectedRFQItem = null;
        }

        private void OnUncheckedPreSalesCheckBox(object sender, RoutedEventArgs e)
        {
            preSalesComboBox.IsEnabled = false;
            preSalesComboBox.SelectedItem = null;
        }
        private void OnUncheckProductCheckBox(object sender, RoutedEventArgs e)
        {
            productComboBox.SelectedItem = null;
            productComboBox.IsEnabled = false;

            //currentSelectedRFQItem = null;
            //previousSelectedRFQItem = null;
        }
        private void OnUncheckBrandCheckBox(object sender, RoutedEventArgs e)
        {
            brandComboBox.SelectedItem = null;
            brandComboBox.IsEnabled = false;

            //currentSelectedRFQItem = null;
            //previousSelectedRFQItem = null;
        }
        private void OnUncheckStatusCheckBox(object sender, RoutedEventArgs e)
        {
            statusComboBox.SelectedItem = null;
            statusComboBox.IsEnabled = false;

            //currentSelectedRFQItem = null;
            //previousSelectedRFQItem = null;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //VIEWING TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnClickListView(object sender, MouseButtonEventArgs e)
        {
            listViewLabel.Style = (Style)FindResource("selectedMainTabLabelItem");
            tableViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");

            RFQsStackPanel.Visibility = Visibility.Visible;
            gridScrollViewer.Visibility = Visibility.Collapsed;
        }

        private void OnClickTableView(object sender, MouseButtonEventArgs e)
        {
            listViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");
            tableViewLabel.Style = (Style)FindResource("selectedMainTabLabelItem");

            RFQsStackPanel.Visibility = Visibility.Collapsed;
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
        private void OnButtonClickedWorkOrders(object sender, MouseButtonEventArgs e)
        {
            WorkOrdersPage workOrdersPage = new WorkOrdersPage(ref loggedInUser);
            this.NavigationService.Navigate(workOrdersPage);
        }
        private void OnButtonClickedWorkOffers(object sender, RoutedEventArgs e)
        {
            WorkOffersPage rfqsList = new WorkOffersPage(ref loggedInUser);
            this.NavigationService.Navigate(rfqsList);
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

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //BTN CLICKED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnBtnClickedAdd(object sender, RoutedEventArgs e)
        {
            int viewAddCondition = COMPANY_WORK_MACROS.RFQ_ADD_CONDITION;
            RFQ rfq = new RFQ(sqlDatabase);
            RFQWindow addRFQWindow = new RFQWindow(ref loggedInUser, ref rfq, viewAddCondition);
            addRFQWindow.Closed += OnClosedRFQWindow;
            addRFQWindow.Show();
        }
        private void OnBtnClickedView(object sender, RoutedEventArgs e)
        {
            RFQ selectedRFQ = new RFQ(sqlDatabase);


            selectedRFQ.InitializeRFQInfo(stackPanelItems[RFQsStackPanel.Children.IndexOf(currentSelectedRFQItem)].rfq_serial,
                                            stackPanelItems[RFQsStackPanel.Children.IndexOf(currentSelectedRFQItem)].rfq_version,
                                            stackPanelItems[RFQsStackPanel.Children.IndexOf(currentSelectedRFQItem)].sales_person);

            int viewAddCondition = COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION;
            RFQWindow viewRFQ = new RFQWindow(ref loggedInUser, ref selectedRFQ, viewAddCondition);
            viewRFQ.Show();

        }
        private void OnBtnClickedReviseRFQ(object sender, RoutedEventArgs e)
        {
            RFQ selectedRFQ = new RFQ(sqlDatabase);

            selectedRFQ.InitializeRFQInfo(stackPanelItems[RFQsStackPanel.Children.IndexOf(currentSelectedRFQItem)].rfq_serial,
                                            stackPanelItems[RFQsStackPanel.Children.IndexOf(currentSelectedRFQItem)].rfq_version,
                                            stackPanelItems[RFQsStackPanel.Children.IndexOf(currentSelectedRFQItem)].sales_person);

            int viewAddCondition = COMPANY_WORK_MACROS.RFQ_REVISE_CONDITION;
            RFQWindow reviseRFQ = new RFQWindow(ref loggedInUser, ref selectedRFQ, viewAddCondition);
            reviseRFQ.Closed += OnClosedRFQWindow;
            reviseRFQ.Show();
        }

        private void OnBtnClickedRFQItem(object sender, RoutedEventArgs e) 
        {
            EnableViewButton();
            EnableReviseButton();
            EnableResolveButton();

            previousSelectedRFQItem = currentSelectedRFQItem;
            currentSelectedRFQItem = (Grid)sender;
            BrushConverter brush = new BrushConverter();

            if (previousSelectedRFQItem != null)
            {
                previousSelectedRFQItem.Background = (Brush)brush.ConvertFrom("#FFFFFF");

                StackPanel previousSelectedStackPanel = (StackPanel)previousSelectedRFQItem.Children[0];
                Border previousSelectedBorder = (Border)previousSelectedRFQItem.Children[1];
                Label previousStatusLabel = (Label)previousSelectedBorder.Child;

                foreach (Label childLabel in previousSelectedStackPanel.Children)
                    childLabel.Foreground = (Brush)brush.ConvertFrom("#000000");

                if (rfqsList[RFQsStackPanel.Children.IndexOf(previousSelectedRFQItem)].rfq_status_id == COMPANY_WORK_MACROS.PENDING_RFQ)
                    previousSelectedBorder.Background = (Brush)brush.ConvertFrom("#FFA500");
                else if (rfqsList[RFQsStackPanel.Children.IndexOf(previousSelectedRFQItem)].rfq_status_id == COMPANY_WORK_MACROS.CONFIRMED_RFQ)
                    previousSelectedBorder.Background = (Brush)brush.ConvertFrom("#008000");
                else
                    previousSelectedBorder.Background = (Brush)brush.ConvertFrom("#FF0000");

                previousStatusLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");
            }

            currentSelectedRFQItem.Background = (Brush)brush.ConvertFrom("#105A97");

            StackPanel currentSelectedStackPanel = (StackPanel)currentSelectedRFQItem.Children[0];
            Border currentSelectedBorder = (Border)currentSelectedRFQItem.Children[1];
            Label currentStatusLabel = (Label)currentSelectedBorder.Child;

            foreach (Label childLabel in currentSelectedStackPanel.Children)
                childLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");

            currentSelectedBorder.Background = (Brush)brush.ConvertFrom("#FFFFFF");
            currentStatusLabel.Foreground = (Brush)brush.ConvertFrom("#105A97");
        }

        private void ResolveButtonClick(object sender, RoutedEventArgs e)
        {
            RFQ selectedRFQ = new RFQ(sqlDatabase);
            WorkOffer resolveWorkOffer = new WorkOffer(sqlDatabase);


             resolveWorkOffer.InitializeRFQInfo(stackPanelItems[RFQsStackPanel.Children.IndexOf(currentSelectedRFQItem)].rfq_serial,
                                            stackPanelItems[RFQsStackPanel.Children.IndexOf(currentSelectedRFQItem)].rfq_version,
                                            stackPanelItems[RFQsStackPanel.Children.IndexOf(currentSelectedRFQItem)].sales_person);
            resolveWorkOffer.LinkRFQInfo();
            int viewAddCondition = 3;

            WorkOfferWindow resolveOffer = new WorkOfferWindow(ref loggedInUser, ref resolveWorkOffer, viewAddCondition);
            resolveOffer.Show();
        }

        private void OnBtnClickedExport(object sender, RoutedEventArgs e)
        {
            ExcelExport excelExport = new ExcelExport(rfqsGrid);
        }

        
    }

}