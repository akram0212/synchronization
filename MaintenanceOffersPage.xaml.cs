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
    /// Interaction logic for MaintenanceOffers.xaml
    /// </summary>
    public partial class MaintenanceOffersPage : Page
    {
        private Employee loggedInUser;
        private SQLServer sqlDatabase;
        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;

        private MaintenanceOffer selectedMaintOffer;
        private int finalYear = Int32.Parse(DateTime.Now.Year.ToString());

        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> salesEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> preSalesEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT> maintOffers = new List<COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT> maintOffersAfterFiltering = new List<COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> productTypes = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
        private List<COMPANY_WORK_MACROS.BRAND_STRUCT> brandTypes = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();
        private List<COMPANY_WORK_MACROS.STATUS_STRUCT> offerStatuses = new List<COMPANY_WORK_MACROS.STATUS_STRUCT>();
        private List<COMPANY_WORK_MACROS.FAILURE_REASON_STRUCT> failureReasons = new List<COMPANY_WORK_MACROS.FAILURE_REASON_STRUCT>();

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
        public MaintenanceOffersPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();
            loggedInUser = mLoggedInUser;

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            selectedMaintOffer = new MaintenanceOffer(sqlDatabase);

            if (!GetMaintenanceOffers())
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

            SetMaintOffersStackPanel();
            SetMaintOffersGrid();
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //GET DATA FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool GetMaintenanceOffers()
        {
            if (!commonQueriesObject.GetMaintenanceOffers(ref maintOffers))
                return false;
            return true;
        }

        private bool GetFailureReasons()
        {
            if (!commonQueriesObject.GetOfferFailureReasons(ref failureReasons))
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
            commonQueriesObject.GetWorkOfferStatus(ref offerStatuses);

            for (int i = 0; i < offerStatuses.Count; i++)
            {
                statusComboBox.Items.Add(offerStatuses[i].status_name);
            }
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

        private void SetDefaultSettings()
        {
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
        private bool SetMaintOffersStackPanel()
        {
            maintOffersStackPanel.Children.Clear();

            maintOffersAfterFiltering.Clear();

            for (int i = 0; i < maintOffers.Count; i++)
            {
                DateTime currentWorkOfferDate = DateTime.Parse(maintOffers[i].issue_date);

                bool salesPersonCondition = selectedSales != maintOffers[i].sales_person_id;

                bool assigneeCondition = selectedPreSales != maintOffers[i].offer_proposer_id;

                bool productCondition = false;
                for (int productNo = 0; productNo < maintOffers[i].products.Count(); productNo++)
                    if (maintOffers[i].products[productNo].productType.typeId == selectedProduct)
                        productCondition |= true;

                bool brandCondition = false;
                for (int productNo = 0; productNo < maintOffers[i].products.Count(); productNo++)
                    if (maintOffers[i].products[productNo].productBrand.brandId == selectedBrand)
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

                if (statusCheckBox.IsChecked == true && maintOffers[i].offer_status_id != selectedStatus)
                    continue;

                StackPanel fullStackPanel = new StackPanel();
                fullStackPanel.Orientation = Orientation.Vertical;
                //fullStackPanel.Height = 90;

                maintOffersAfterFiltering.Add(maintOffers[i]);

                Label offerIdLabel = new Label();
                offerIdLabel.Content = maintOffers[i].offer_id;
                offerIdLabel.Style = (Style)FindResource("stackPanelItemHeader");

                Label salesLabel = new Label();
                salesLabel.Content = maintOffers[i].sales_person_name;
                salesLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label preSalesLabel = new Label();
                preSalesLabel.Content = maintOffers[i].offer_proposer_name;
                preSalesLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label companyAndContactLabel = new Label();
                companyAndContactLabel.Content = maintOffers[i].company_name + " -" + maintOffers[i].contact_name;
                companyAndContactLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label productTypeAndBrandLabel = new Label();
                List<COMPANY_WORK_MACROS.QUOTATION_PRODUCT_STRUCT> temp = maintOffers[i].products;

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
                contractTypeLabel.Content = maintOffers[i].contract_type;
                contractTypeLabel.Style = (Style)FindResource("stackPanelItemBody");

                Border borderIcon = new Border();
                borderIcon.Style = (Style)FindResource("BorderIcon");

                Label rfqStatusLabel = new Label();
                rfqStatusLabel.Content = maintOffers[i].offer_status;
                rfqStatusLabel.Style = (Style)FindResource("BorderIconTextLabel");

                if (maintOffers[i].offer_status_id == COMPANY_WORK_MACROS.PENDING_OUTGOING_QUOTATION)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500"));
                }
                else if (maintOffers[i].offer_status_id == COMPANY_WORK_MACROS.CONFIRMED_OUTGOING_QUOTATION)
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

                ListBoxItem reviseButton = new ListBoxItem();
                reviseButton.Content = "Revise Offer";
                reviseButton.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));

                ListBoxItem confirmButton = new ListBoxItem();
                confirmButton.Content = "Confirm Offer";
                confirmButton.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));

                ListBoxItem rejectButton = new ListBoxItem();
                rejectButton.Content = "Reject Offer";
                rejectButton.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));

                listBox.Items.Add(viewButton);

                if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID)
                {
                    listBox.Items.Add(reviseButton);

                    if (maintOffers[i].offer_status_id == COMPANY_WORK_MACROS.PENDING_OUTGOING_QUOTATION)
                        listBox.Items.Add(confirmButton);

                    if (maintOffers[i].offer_status_id != COMPANY_WORK_MACROS.CONFIRMED_OUTGOING_QUOTATION)
                        listBox.Items.Add(rejectButton);
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

                maintOffersStackPanel.Children.Add(grid);
            }

            return true;
        }
        private bool SetMaintOffersGrid()
        {

            maintOffersGrid.Children.Clear();
            maintOffersGrid.RowDefinitions.Clear();
            maintOffersGrid.ColumnDefinitions.Clear();



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

            maintOffersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            maintOffersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            maintOffersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            maintOffersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            maintOffersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            maintOffersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            maintOffersGrid.ColumnDefinitions.Add(new ColumnDefinition());

            maintOffersGrid.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(offerIdHeader, 0);
            Grid.SetColumn(offerIdHeader, 0);
            maintOffersGrid.Children.Add(offerIdHeader);

            Grid.SetRow(offerSalesHeader, 0);
            Grid.SetColumn(offerSalesHeader, 1);
            maintOffersGrid.Children.Add(offerSalesHeader);

            Grid.SetRow(offerPreSalesHeader, 0);
            Grid.SetColumn(offerPreSalesHeader, 2);
            maintOffersGrid.Children.Add(offerPreSalesHeader);

            Grid.SetRow(offerCompanyContactHeader, 0);
            Grid.SetColumn(offerCompanyContactHeader, 3);
            maintOffersGrid.Children.Add(offerCompanyContactHeader);

            Grid.SetRow(offerProductsHeader, 0);
            Grid.SetColumn(offerProductsHeader, 4);
            maintOffersGrid.Children.Add(offerProductsHeader);

            Grid.SetRow(offerContractTypeHeader, 0);
            Grid.SetColumn(offerContractTypeHeader, 5);
            maintOffersGrid.Children.Add(offerContractTypeHeader);

            Grid.SetRow(offerStatusHeader, 0);
            Grid.SetColumn(offerStatusHeader, 6);
            maintOffersGrid.Children.Add(offerStatusHeader);

            int currentRowNumber = 1;

            for (int i = 0; i < maintOffers.Count; i++)
            {
                DateTime currentWorkOfferDate = DateTime.Parse(maintOffers[i].issue_date);

                bool salesPersonCondition = selectedSales != maintOffers[i].sales_person_id;

                bool assigneeCondition = selectedPreSales != maintOffers[i].offer_proposer_id;

                bool productCondition = false;
                for (int productNo = 0; productNo < maintOffers[i].products.Count(); productNo++)
                    if (maintOffers[i].products[productNo].productType.typeId == selectedProduct)
                        productCondition |= true;

                bool brandCondition = false;
                for (int productNo = 0; productNo < maintOffers[i].products.Count(); productNo++)
                    if (maintOffers[i].products[productNo].productBrand.brandId == selectedBrand)
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

                if (statusCheckBox.IsChecked == true && maintOffers[i].offer_status_id != selectedStatus)
                    continue;


                RowDefinition currentRow = new RowDefinition();
                maintOffersGrid.RowDefinitions.Add(currentRow);

                Label offerIdLabel = new Label();
                offerIdLabel.Content = maintOffers[i].offer_id;
                offerIdLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(offerIdLabel, currentRowNumber);
                Grid.SetColumn(offerIdLabel, 0);
                maintOffersGrid.Children.Add(offerIdLabel);


                Label salesLabel = new Label();
                salesLabel.Content = maintOffers[i].sales_person_name;
                salesLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(salesLabel, currentRowNumber);
                Grid.SetColumn(salesLabel, 1);
                maintOffersGrid.Children.Add(salesLabel);


                Label preSalesLabel = new Label();
                preSalesLabel.Content = maintOffers[i].offer_proposer_name;
                preSalesLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(preSalesLabel, currentRowNumber);
                Grid.SetColumn(preSalesLabel, 2);
                maintOffersGrid.Children.Add(preSalesLabel);


                Label companyAndContactLabel = new Label();
                companyAndContactLabel.Content = maintOffers[i].company_name + " - " + maintOffers[i].contact_name;
                companyAndContactLabel.Style = (Style)FindResource("tableSubItemLabel");

                maintOffersGrid.Children.Add(companyAndContactLabel);
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


                List<COMPANY_WORK_MACROS.QUOTATION_PRODUCT_STRUCT> temp = maintOffers[i].products;

                for (int j = 0; j < temp.Count(); j++)
                {
                    COMPANY_WORK_MACROS.PRODUCT_STRUCT tempType1 = temp[j].productType;
                    COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand1 = temp[j].productBrand;
                    COMPANY_WORK_MACROS.MODEL_STRUCT tempModel1 = temp[j].productModel;

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

                maintOffersGrid.Children.Add(productGrid);
                Grid.SetRow(productGrid, currentRowNumber);
                Grid.SetColumn(productGrid, 4);



                Label contractTypeLabel = new Label();
                contractTypeLabel.Content = maintOffers[i].contract_type;
                contractTypeLabel.Style = (Style)FindResource("tableSubItemLabel");

                maintOffersGrid.Children.Add(contractTypeLabel);
                Grid.SetRow(contractTypeLabel, currentRowNumber);
                Grid.SetColumn(contractTypeLabel, 5);


                Border borderIcon = new Border();
                borderIcon.Style = (Style)FindResource("BorderIcon");

                Label rfqStatusLabel = new Label();
                rfqStatusLabel.Content = maintOffers[i].offer_status;
                rfqStatusLabel.Style = (Style)FindResource("BorderIconTextLabel");

                if (maintOffers[i].offer_status_id == COMPANY_WORK_MACROS.PENDING_OUTGOING_QUOTATION)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500"));
                }
                else if (maintOffers[i].offer_status_id == COMPANY_WORK_MACROS.CONFIRMED_OUTGOING_QUOTATION)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008000"));
                }
                else
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                }

                borderIcon.Child = rfqStatusLabel;

                maintOffersGrid.Children.Add(borderIcon);
                Grid.SetRow(borderIcon, currentRowNumber);
                Grid.SetColumn(borderIcon, 6);

                currentRowNumber++;
            }

            return true;
        }
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
        private void DisableViewButton()
        {
            //viewButton.IsEnabled = false;
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
        private void OnButtonClickedMaintOffers(object sender, RoutedEventArgs e)
        {
            MaintenanceOffersPage maintOffers = new MaintenanceOffersPage(ref loggedInUser);
            this.NavigationService.Navigate(maintOffers);
        }
        private void OnButtonClickedWorkOrders(object sender, RoutedEventArgs e)
        {
            WorkOrdersPage workOrders = new WorkOrdersPage(ref loggedInUser);
            this.NavigationService.Navigate(workOrders);
        }
        private void OnButtonClickedWorkOffers(object sender, RoutedEventArgs e)
        {
            QuotationsPage maintOffers = new QuotationsPage(ref loggedInUser);
            this.NavigationService.Navigate(maintOffers);
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

            SetMaintOffersStackPanel();
            SetMaintOffersGrid();
        }

        private void OnSelChangedQuarterCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (quarterComboBox.SelectedItem != null)
                selectedQuarter = quarterComboBox.SelectedIndex + 1;
            else
                selectedQuarter = 0;

            SetMaintOffersStackPanel();
            SetMaintOffersGrid();
        }

        private void OnSelChangedSalesCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (salesComboBox.SelectedItem != null)
                selectedSales = salesEmployeesList[salesComboBox.SelectedIndex].employee_id;
            else
                selectedSales = 0;



            SetMaintOffersStackPanel();
            SetMaintOffersGrid();
        }
        private void OnSelChangedPreSalesCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (preSalesComboBox.SelectedItem != null)
                selectedPreSales = preSalesEmployeesList[preSalesComboBox.SelectedIndex].employee_id;
            else
                selectedPreSales = 0;



            SetMaintOffersStackPanel();
            SetMaintOffersGrid();
        }

        private void OnSelChangedProductCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (productComboBox.SelectedItem != null)
                selectedProduct = productTypes[productComboBox.SelectedIndex].typeId;
            else
                selectedProduct = 0;

            SetMaintOffersStackPanel();
            SetMaintOffersGrid();
        }

        private void OnSelChangedBrandCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (brandComboBox.SelectedItem != null)
                selectedBrand = brandTypes[brandComboBox.SelectedIndex].brandId;
            else
                selectedBrand = 0;

            SetMaintOffersStackPanel();
            SetMaintOffersGrid();
        }

        private void OnSelChangedStatusCombo(object sender, SelectionChangedEventArgs e)
        {
            DisableViewButton();
            //DisableReviseButton();

            if (statusComboBox.SelectedItem != null)
                selectedStatus = offerStatuses[statusComboBox.SelectedIndex].status_id;
            else
                selectedStatus = 0;

            SetMaintOffersStackPanel();
            SetMaintOffersGrid();
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

        private void OnClosedMaintOfferWindow(object sender, EventArgs e)
        {
            //if (!SetMaintOffersGrid())
            //  return;

           SetMaintOffersStackPanel();
           SetMaintOffersGrid();
        }
        private void OnClickExportButton(object sender, RoutedEventArgs e)
        {
           // ExcelExport excelExport = new ExcelExport(maintOffersGrid);
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

            MaintenanceOffer maintOffer = new MaintenanceOffer(sqlDatabase);

            //maintOffer.InitializeMaintOfferInfo(maintOffersAfterFiltering[maintOffersStackPanel.Children.IndexOf(currentGrid)].order_serial, maintOffersAfterFiltering[maintOffersStackPanel.Children.IndexOf(currentGrid)].sales_person_id);
            MaintenanceOffersWindow maintOfferWindow = new MaintenanceOffersWindow(ref loggedInUser, ref maintOffer, viewAddCondition, false);

            maintOfferWindow.Show();
        }
       
        private void OnBtnClickViewOffer()
        {
            int viewAddCondition = COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION;

            MaintenanceOffer maintOffer = new MaintenanceOffer(sqlDatabase);

            //maintOffer.InitializeMaintOfferInfo(maintOffersAfterFiltering[maintOffersStackPanel.Children.IndexOf(currentGrid)].order_serial, maintOffersAfterFiltering[maintOffersStackPanel.Children.IndexOf(currentGrid)].sales_person_id);

            if (maintOffer.GetMaintOfferID() != null)
            {
                MaintenanceOffer maintenanceOffer = new MaintenanceOffer(sqlDatabase);

                //maintenanceOffer.CopyMaintOffer(maintOffer);

                MaintenanceOffersWindow workOfferWindow = new MaintenanceOffersWindow(ref loggedInUser, ref maintenanceOffer, viewAddCondition, false);

                workOfferWindow.Show();
            }
            else
            {
                MessageBox.Show("Selected maintOffer doesn't have an Offer");
            }
        }
        private void OnBtnClickConfirmOrder()
        {
            MaintenanceOffer maintOffer = new MaintenanceOffer(sqlDatabase);

           // maintOffer.InitializeMaintOfferInfo(maintOffersAfterFiltering[maintOffersStackPanel.Children.IndexOf(currentGrid)].order_serial, maintOffersAfterFiltering[maintOffersStackPanel.Children.IndexOf(currentGrid)].sales_person_id);

            //maintOffer.ConfirmOrder();

            //if (SetMaintOffersGrid())
            //    return;

           SetMaintOffersStackPanel();
           SetMaintOffersGrid();
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

            selectedMaintOffer = new MaintenanceOffer(sqlDatabase);

            MaintenanceOffersWindow workOfferWindow = new MaintenanceOffersWindow(ref loggedInUser, ref selectedMaintOffer, viewAddCondition, false);

            workOfferWindow.Closed += OnClosedMaintOfferWindow;
            workOfferWindow.Show();
        }
    }
}
