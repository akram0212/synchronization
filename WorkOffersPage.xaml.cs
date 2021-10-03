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
    /// Interaction logic for WorkOffersPage.xaml
    /// </summary>
    public partial class WorkOffersPage : Page
    {
        private Employee loggedInUser;

        private SQLServer sqlDatabase;
        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;

        private WorkOffer selectedWorkOffer;

        private int finalYear = Int32.Parse(DateTime.Now.Year.ToString());

        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> salesEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> preSalesEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_WORK_MACROS.WORK_OFFER_MAX_STRUCT> workOffers = new List<COMPANY_WORK_MACROS.WORK_OFFER_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.WORK_OFFER_MAX_STRUCT> workOffersAfterFiltering = new List<COMPANY_WORK_MACROS.WORK_OFFER_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> productTypes = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
        private List<COMPANY_WORK_MACROS.BRAND_STRUCT> brandTypes = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();
        private List<COMPANY_WORK_MACROS.STATUS_STRUCT> offerStatuses = new List<COMPANY_WORK_MACROS.STATUS_STRUCT>();

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

        public WorkOffersPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            selectedWorkOffer = new WorkOffer(sqlDatabase);

            if (!GetWorkOffers())
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

            SetWorkOffersStackPanel();
            SetWorkOffersGrid();

            if(loggedInUser.GetEmployeeTeamId() != COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID)
            {
                addButton.IsEnabled = false;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //GET DATA FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool GetWorkOffers()
        {
            if (!commonQueriesObject.GetWorkOffers(ref workOffers))
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

            for(int i = 0; i < offerStatuses.Count; i++)
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


        private bool SetWorkOffersStackPanel()
        {
            workOffersStackPanel.Children.Clear();
            
            workOffersAfterFiltering.Clear();

            for (int i = 0; i < workOffers.Count; i++)
            {
                DateTime currentWorkOfferDate = DateTime.Parse(workOffers[i].issue_date);

                bool salesPersonCondition = selectedSales != workOffers[i].sales_person_id;

                bool assigneeCondition = selectedPreSales != workOffers[i].offer_proposer_id;

                bool productCondition = false;
                for (int productNo = 0; productNo < workOffers[i].products.Count(); productNo++)
                    if (workOffers[i].products[productNo].productType.typeId == selectedProduct)
                        productCondition |= true;

                bool brandCondition = false;
                for (int productNo = 0; productNo < workOffers[i].products.Count(); productNo++)
                    if (workOffers[i].products[productNo].productBrand.brandId == selectedBrand)
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

                if (statusCheckBox.IsChecked == true && workOffers[i].offer_status_id != selectedStatus)
                    continue;

                StackPanel fullStackPanel = new StackPanel();
                fullStackPanel.Orientation = Orientation.Vertical;
                //fullStackPanel.Height = 90;

                workOffersAfterFiltering.Add(workOffers[i]);

                Label offerIdLabel = new Label();
                offerIdLabel.Content = workOffers[i].offer_id;
                offerIdLabel.Style = (Style)FindResource("stackPanelItemHeader");

                Label salesLabel = new Label();
                salesLabel.Content = workOffers[i].sales_person_name;
                salesLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label preSalesLabel = new Label();
                preSalesLabel.Content = workOffers[i].offer_proposer_name;
                preSalesLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label companyAndContactLabel = new Label();
                companyAndContactLabel.Content = workOffers[i].company_name + " -" + workOffers[i].contact_name;
                companyAndContactLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label productTypeAndBrandLabel = new Label();
                List<COMPANY_WORK_MACROS.OFFER_PRODUCT_STRUCT> temp = workOffers[i].products;

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
                contractTypeLabel.Content = workOffers[i].contract_type;
                contractTypeLabel.Style = (Style)FindResource("stackPanelItemBody");

                Border borderIcon = new Border();
                borderIcon.Style = (Style)FindResource("BorderIcon");

                Label rfqStatusLabel = new Label();
                rfqStatusLabel.Content = workOffers[i].offer_status;
                rfqStatusLabel.Style = (Style)FindResource("BorderIconTextLabel");

                if (workOffers[i].offer_status_id == COMPANY_WORK_MACROS.PENDING_WORK_OFFER)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500"));
                }
                else if (workOffers[i].offer_status_id == COMPANY_WORK_MACROS.CONFIRMED_WORK_OFFER)
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

                listBox.Items.Add(viewButton);

                if(loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID)
                    listBox.Items.Add(reviseButton);

                if(workOffers[i].offer_status_id == COMPANY_WORK_MACROS.PENDING_WORK_OFFER)
                    listBox.Items.Add(confirmButton);

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

                workOffersStackPanel.Children.Add(grid);
            }

            return true;
        }

        private bool SetWorkOffersGrid()
        {

            workOffersGrid.Children.Clear();
            workOffersGrid.RowDefinitions.Clear();
            workOffersGrid.ColumnDefinitions.Clear();

            

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

            workOffersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOffersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOffersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOffersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOffersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOffersGrid.ColumnDefinitions.Add(new ColumnDefinition());
            workOffersGrid.ColumnDefinitions.Add(new ColumnDefinition());

            workOffersGrid.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(offerIdHeader, 0);
            Grid.SetColumn(offerIdHeader, 0);
            workOffersGrid.Children.Add(offerIdHeader);

            Grid.SetRow(offerSalesHeader, 0);
            Grid.SetColumn(offerSalesHeader, 1);
            workOffersGrid.Children.Add(offerSalesHeader);

            Grid.SetRow(offerPreSalesHeader, 0);
            Grid.SetColumn(offerPreSalesHeader, 2);
            workOffersGrid.Children.Add(offerPreSalesHeader);

            Grid.SetRow(offerCompanyContactHeader, 0);
            Grid.SetColumn(offerCompanyContactHeader, 3);
            workOffersGrid.Children.Add(offerCompanyContactHeader);

            Grid.SetRow(offerProductsHeader, 0);
            Grid.SetColumn(offerProductsHeader, 4);
            workOffersGrid.Children.Add(offerProductsHeader);

            Grid.SetRow(offerContractTypeHeader, 0);
            Grid.SetColumn(offerContractTypeHeader, 5);
            workOffersGrid.Children.Add(offerContractTypeHeader);

            Grid.SetRow(offerStatusHeader, 0);
            Grid.SetColumn(offerStatusHeader, 6);
            workOffersGrid.Children.Add(offerStatusHeader);

            int currentRowNumber = 1;

            for (int i = 0; i < workOffers.Count; i++)
            {
                DateTime currentWorkOfferDate = DateTime.Parse(workOffers[i].issue_date);

                bool salesPersonCondition = selectedSales != workOffers[i].sales_person_id;

                bool assigneeCondition = selectedPreSales != workOffers[i].offer_proposer_id;

                bool productCondition = false;
                for (int productNo = 0; productNo < workOffers[i].products.Count(); productNo++)
                    if (workOffers[i].products[productNo].productType.typeId == selectedProduct)
                        productCondition |= true;

                bool brandCondition = false;
                for (int productNo = 0; productNo < workOffers[i].products.Count(); productNo++)
                    if (workOffers[i].products[productNo].productBrand.brandId == selectedBrand)
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

                if (statusCheckBox.IsChecked == true && workOffers[i].offer_status_id != selectedStatus)
                    continue;


                RowDefinition currentRow = new RowDefinition();
                workOffersGrid.RowDefinitions.Add(currentRow);

                Label offerIdLabel = new Label();
                offerIdLabel.Content = workOffers[i].offer_id;
                offerIdLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(offerIdLabel, currentRowNumber);
                Grid.SetColumn(offerIdLabel, 0);
                workOffersGrid.Children.Add(offerIdLabel);
                

                Label salesLabel = new Label();
                salesLabel.Content = workOffers[i].sales_person_name;
                salesLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(salesLabel, currentRowNumber);
                Grid.SetColumn(salesLabel, 1);
                workOffersGrid.Children.Add(salesLabel);

                
                Label preSalesLabel = new Label();
                preSalesLabel.Content = workOffers[i].offer_proposer_name;
                preSalesLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(preSalesLabel, currentRowNumber);
                Grid.SetColumn(preSalesLabel, 2);
                workOffersGrid.Children.Add(preSalesLabel);


                Label companyAndContactLabel = new Label();
                companyAndContactLabel.Content = workOffers[i].company_name + " - " + workOffers[i].contact_name;
                companyAndContactLabel.Style = (Style)FindResource("tableSubItemLabel");

                workOffersGrid.Children.Add(companyAndContactLabel);
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


                List<COMPANY_WORK_MACROS.OFFER_PRODUCT_STRUCT> temp = workOffers[i].products;

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
                    Grid.SetRow(productNumberHeader, j+1);
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

                workOffersGrid.Children.Add(productGrid);
                Grid.SetRow(productGrid, currentRowNumber);
                Grid.SetColumn(productGrid, 4);



                Label contractTypeLabel = new Label();
                contractTypeLabel.Content = workOffers[i].contract_type;
                contractTypeLabel.Style = (Style)FindResource("tableSubItemLabel");

                workOffersGrid.Children.Add(contractTypeLabel);
                Grid.SetRow(contractTypeLabel, currentRowNumber);
                Grid.SetColumn(contractTypeLabel, 5);


                Border borderIcon = new Border();
                borderIcon.Style = (Style)FindResource("BorderIcon");

                Label rfqStatusLabel = new Label();
                rfqStatusLabel.Content = workOffers[i].offer_status;
                rfqStatusLabel.Style = (Style)FindResource("BorderIconTextLabel");

                if (workOffers[i].offer_status_id == COMPANY_WORK_MACROS.PENDING_WORK_OFFER)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500"));
                }
                else if (workOffers[i].offer_status_id == COMPANY_WORK_MACROS.CONFIRMED_WORK_OFFER)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008000"));
                }
                else
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                }

                borderIcon.Child = rfqStatusLabel;

                workOffersGrid.Children.Add(borderIcon);
                Grid.SetRow(borderIcon, currentRowNumber);
                Grid.SetColumn(borderIcon, 6);

                currentRowNumber++;
            }

            return true;
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
            
            SetWorkOffersStackPanel();
            SetWorkOffersGrid();
        }

        private void OnSelChangedQuarterCombo(object sender, SelectionChangedEventArgs e)
        {
            
            

            if (quarterComboBox.SelectedItem != null)
                selectedQuarter = quarterComboBox.SelectedIndex + 1;
            else
                selectedQuarter = 0;
            
            SetWorkOffersStackPanel();
            SetWorkOffersGrid();
        }

        private void OnSelChangedSalesCombo(object sender, SelectionChangedEventArgs e)
        {
            
            

            if (salesComboBox.SelectedItem != null)
                selectedSales = salesEmployeesList[salesComboBox.SelectedIndex].employee_id;
            else
                selectedSales = 0;

            

            SetWorkOffersStackPanel();
            SetWorkOffersGrid();
        }
        private void OnSelChangedPreSalesCombo(object sender, SelectionChangedEventArgs e)
        {
            
            

            if (preSalesComboBox.SelectedItem != null)
                selectedPreSales = preSalesEmployeesList[preSalesComboBox.SelectedIndex].employee_id;
            else
                selectedPreSales = 0;

            

            SetWorkOffersStackPanel();
            SetWorkOffersGrid();
        }

        private void OnSelChangedProductCombo(object sender, SelectionChangedEventArgs e)
        {
            
            

            if (productComboBox.SelectedItem != null)
                selectedProduct = productTypes[productComboBox.SelectedIndex].typeId;
            else
                selectedProduct = 0;
            
            SetWorkOffersStackPanel();
            SetWorkOffersGrid();
        }

        private void OnSelChangedBrandCombo(object sender, SelectionChangedEventArgs e)
        {
            
            

            if (brandComboBox.SelectedItem != null)
                selectedBrand = brandTypes[brandComboBox.SelectedIndex].brandId;
            else
                selectedBrand = 0;
            
            SetWorkOffersStackPanel();
            SetWorkOffersGrid();
        }

        private void OnSelChangedStatusCombo(object sender, SelectionChangedEventArgs e)
        {
        
            if (statusComboBox.SelectedItem != null)
                selectedStatus = offerStatuses[statusComboBox.SelectedIndex].status_id;
            else
                selectedStatus = 0;
            
            SetWorkOffersStackPanel();
            SetWorkOffersGrid();
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

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //BTN CLICKED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        private void OnBtnClickAdd(object sender, RoutedEventArgs e)
        {
            viewAddCondition = COMPANY_WORK_MACROS.OFFER_ADD_CONDITION;

            selectedWorkOffer = new WorkOffer(sqlDatabase);

            WorkOfferWindow workOfferWindow = new WorkOfferWindow(ref loggedInUser, ref selectedWorkOffer, viewAddCondition, false);
            
            workOfferWindow.Closed += OnClosedWorkOfferWindow;
            workOfferWindow.Show();
        }

        
        private void OnBtnClickView()
        {
            viewAddCondition = COMPANY_WORK_MACROS.OFFER_VIEW_CONDITION;

            commonQueriesObject.GetEmployeeTeam(workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].sales_person_id, ref salesPersonTeam);

            
            if (salesPersonTeam == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
            {
                selectedWorkOffer.InitializeSalesWorkOfferInfo(workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_serial,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_version,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_proposer_id);
            }
            else
            {
                selectedWorkOffer.InitializeTechnicalOfficeWorkOfferInfo(workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_serial,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_version,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_proposer_id);
            }

            
            WorkOfferWindow viewOffer = new WorkOfferWindow(ref loggedInUser, ref selectedWorkOffer, viewAddCondition, false);
            viewOffer.Show();
        }

        private void OnBtnClickReviseOffer()
        {
            viewAddCondition = COMPANY_WORK_MACROS.OFFER_REVISE_CONDITION;

            commonQueriesObject.GetEmployeeTeam(workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].sales_person_id, ref salesPersonTeam);

            if (salesPersonTeam == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
            {
                selectedWorkOffer.InitializeSalesWorkOfferInfo(workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_serial,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_version,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_proposer_id);
            }
            else
            {
                selectedWorkOffer.InitializeTechnicalOfficeWorkOfferInfo(workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_serial,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_version,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_proposer_id);
            }

            
            WorkOfferWindow reviseOffer = new WorkOfferWindow(ref loggedInUser, ref selectedWorkOffer, viewAddCondition, false);

            reviseOffer.Closed += OnClosedWorkOfferWindow;
            reviseOffer.Show();
        }

        private void OnBtnClickConfirmOffer()
        {

            commonQueriesObject.GetEmployeeTeam(workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].sales_person_id, ref salesPersonTeam);

            if (salesPersonTeam == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
            {
                selectedWorkOffer.InitializeSalesWorkOfferInfo(workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_serial,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_version,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_proposer_id);
            }
            else
            {
                selectedWorkOffer.InitializeTechnicalOfficeWorkOfferInfo(workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_serial,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_version,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_proposer_id);
            }

            selectedWorkOffer.ConfirmOffer();

            WorkOrder workOrder = new WorkOrder(sqlDatabase);

            if (salesPersonTeam == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
            {
                workOrder.InitializeSalesWorkOfferInfo(workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_serial,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_version,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_proposer_id);
            }
            else
            {
                workOrder.InitializeTechnicalOfficeWorkOfferInfo(workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_serial,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_version,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_proposer_id);
            }

            workOrder.IssueNewOrder();

            if (!GetWorkOffers())
                return;

            SetWorkOffersStackPanel();
            
        }

        private void OnButtonClickedWorkOffers(object sender, MouseButtonEventArgs e)
        {
            //WorkOffersPage workOffers = new WorkOffersPage(ref loggedInUser);
            //this.NavigationService.Navigate(workOffers);
        }

        private void OnBtnClickExport(object sender, RoutedEventArgs e)
        {
            ExcelExport excelExport = new ExcelExport(workOffersGrid);
        }


        private void OnClosedWorkOfferWindow(object sender, EventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.OFFER_VIEW_CONDITION)
            {
                viewAddCondition = COMPANY_WORK_MACROS.OFFER_VIEW_CONDITION;

                WorkOfferWindow viewOffer = new WorkOfferWindow(ref loggedInUser, ref selectedWorkOffer, viewAddCondition, true);
                viewOffer.Show();
            }

            if (!GetWorkOffers())
                return;

            SetWorkOffersStackPanel();
            SetWorkOffersGrid();
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
            ListBoxItem currentItem = (ListBoxItem)tempListBox.Items[tempListBox.SelectedIndex];
            Expander currentExpander = (Expander)tempListBox.Parent;

            currentGrid = (Grid)currentExpander.Parent;

            if (currentItem.Content.ToString() == "View")
            {
                OnBtnClickView();
            }
            else if (currentItem.Content.ToString() == "Revise Offer")
            {
                OnBtnClickReviseOffer();
            }
            else if (currentItem.Content.ToString() == "Confirm Offer")
            {
                OnBtnClickConfirmOffer();
            }
        }
    }
}

