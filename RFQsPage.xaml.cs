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

        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> employeesList;
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsList;
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsListAfterFiltering;
        private List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> productTypes;
        private List<COMPANY_WORK_MACROS.BRAND_STRUCT> brandTypes;

        private int selectedYear;
        private int selectedQuarter;
        private int selectedEmployee;
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

            employeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
            rfqsList = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();
            rfqsListAfterFiltering = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();
            productTypes = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
            brandTypes = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();

            selectedYear = DateTime.Now.Year;
            selectedQuarter = commonFunctionsObject.GetCurrentQuarter();
            selectedEmployee = loggedInUser.GetEmployeeId();
            selectedTeam = -1;
            selectedProduct = -1;
            selectedBrand = -1;
            selectedStatus = -1;

            InitializeYearsComboBox();
            InitializeQuartersComboBox();

            if (!InitializeEmployeeComboBox())
                return;

            if (!InitializeProductsComboBox())
                return;

            if (!InitializeBrandsComboBox())
                return;

            InitializeStatusComboBox();

            DisableViewButton();
            DisableReviseButton();
            DisableResolveButton();
            DisableComboBoxes();
            ResetComboBoxes();

            ConfigureUIElements();

            if (!GetRFQs())
                return;

            SetRFQsStackPanel();

        }

        private void ResetComboBoxes()
        {
            yearCombo.SelectedItem = null;

            quarterCombo.SelectedItem = -1;

            employeeCombo.SelectedItem = -1;
            selectedEmployee = -1;
            selectedTeam = -1;

            productCombo.SelectedItem = -1;

            brandCombo.SelectedItem = -1;

            statusCombo.SelectedItem = -1;
        }

        private void DisableComboBoxes()
        {
            yearCombo.IsEnabled = false;
            quarterCombo.IsEnabled = false;
            employeeCombo.IsEnabled = false;
            productCombo.IsEnabled = false;
            brandCombo.IsEnabled = false;
            statusCombo.IsEnabled = false;
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
        /////////////////////////////////////////////////////////////////
        //GET DATA FUNCTIONS
        /////////////////////////////////////////////////////////////////
        private bool GetRFQs()
        {
            if (!commonQueriesObject.GetRFQs(ref rfqsList))
                return false;
            return true;
        }

        /////////////////////////////////////////////////////////////////
        //INTIALIZATION FUNCTIONS
        /////////////////////////////////////////////////////////////////
        ///
        private void InitializeYearsComboBox()
        {
            for (int year = BASIC_MACROS.CRM_START_YEAR; year <= DateTime.Now.Year; year++)
                yearCombo.Items.Add(year);
        }
        private void InitializeQuartersComboBox()
        {
            for (int i = 0; i < BASIC_MACROS.NO_OF_QUARTERS; i++)
                quarterCombo.Items.Add(commonFunctionsObject.GetQuarterName(i + 1));
        }
        private bool InitializeEmployeeComboBox()
        {
            if (!commonQueriesObject.GetCompanyEmployees(ref employeesList))
                return false;

            for (int i = 0; i < employeesList.Count; i++)
                employeeCombo.Items.Add(employeesList[i].employee_name);

            return true;
        }

        private bool InitializeProductsComboBox()
        {
            if (!commonQueriesObject.GetCompanyProducts(ref productTypes))
                return false;

            for (int i = 0; i < productTypes.Count; i++)
                productCombo.Items.Add(productTypes[i].typeName);

            return true;
        }
        private bool InitializeBrandsComboBox()
        {
            
            if (!commonQueriesObject.GetCompanyBrands(ref brandTypes))
                return false;

            for (int i = 0; i < brandTypes.Count; i++)
                brandCombo.Items.Add(brandTypes[i].brandName);

            return true;
        }

        private void InitializeStatusComboBox()
        {
            statusCombo.Items.Add("Pending");
            statusCombo.Items.Add("Confirmed");
            statusCombo.Items.Add("Failed");
            statusCombo.IsEnabled = false;
        }

        /////////////////////////////////////////////////////////////////
        //CONFIGURATION FUNCTIONS
        /////////////////////////////////////////////////////////////////
        private void ConfigureUIElements()
        {
            EnableYearUIElements();

            //THE CONDITION HERE IS WRONG, YOU SHOULD CHECK THE POSITION NOT THE TEAM
            if (loggedInUser.GetEmployeePositionId() >= COMPANY_ORGANISATION_MACROS.JUNIOR_POSTION)
            {
                DisableEmployeeUIElements();
                SetEmployeeComboBox();
            }

        }

        private void EnableYearUIElements()
        {
            yearCheckBox.IsChecked = true;
        }
        /////////////////////////////////////////////////////////////////
        //SET FUNCTIONS
        /////////////////////////////////////////////////////////////////
        private void SetYearComboBox()
        {
            yearCombo.SelectedIndex = DateTime.Now.Year - BASIC_MACROS.CRM_START_YEAR;
        }
        private void SetQuarterComboBox()
        {
            quarterCombo.SelectedIndex = commonFunctionsObject.GetCurrentQuarter();
        }
        private void SetEmployeeComboBox()
        {
            for (int i = 0; i < employeesList.Count; i++)
                if (loggedInUser.GetEmployeeId() == employeesList[i].employee_id)
                    employeeCombo.SelectedIndex = i;
        }
        private void DisableEmployeeUIElements()
        {
            employeeCheckBox.IsChecked = true;
            employeeCheckBox.IsEnabled = false;
            employeeCombo.IsEnabled = false;
        }

        private bool SetRFQsStackPanel()
        {
            DisableReviseButton();
            DisableViewButton();
            DisableResolveButton();
            RFQsStackPanel.Children.Clear();
            if(rfqsListAfterFiltering.Count() != 0)
                rfqsListAfterFiltering.Clear();

            for (int i = 0; i < rfqsList.Count; i++)
            {
                DateTime currentRFQDate = DateTime.Parse(rfqsList[i].issue_date);

                bool salesPersonCondition =
                    selectedTeam == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID &&
                    selectedEmployee != rfqsList[i].sales_person_id;

                bool assigneeCondition =
                    selectedTeam == COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID &&
                    selectedEmployee != rfqsList[i].assignee_id;

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

                if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
                {
                    if (employeeCheckBox.IsChecked == true && salesPersonCondition)
                        continue;
                }

                else if(loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID)
                {
                    if (employeeCheckBox.IsChecked == true && assigneeCondition)
                        continue;
                }
               
                else
                {
                    if (employeeCheckBox.IsChecked == true && employeeCombo.SelectedItem != null  && employeeCombo.SelectedItem.ToString() != rfqsList[i].sales_person_name)
                        continue;
                }

                if (quarterCheckBox.IsChecked == true && commonFunctionsObject.GetQuarter(currentRFQDate) != selectedQuarter)
                    continue;

                if (productCheckBox.IsChecked == true && !productCondition)
                    continue;

                if (brandCheckBox.IsChecked == true && !brandCondition)
                    continue;

                if (statusCheckBox.IsChecked == true && rfqsList[i].rfq_status_id != selectedStatus)
                    continue;

                rfqsListAfterFiltering.Add(rfqsList[i]);
                StackPanel currentStackPanel = new StackPanel();
                currentStackPanel.Orientation = Orientation.Vertical;
                //currentStackPanel.MouseLeftButtonDown += OnButtonRFQItem;

                //currentStackPanel.Height = 90;

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
                column2.MaxWidth = 77;
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
        

        /////////////////////////////////////////////////////////////////
        //SELECTION CHANGED HANDLERS
        /////////////////////////////////////////////////////////////////

        private void YearComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (yearCombo.SelectedItem != null)
                selectedYear = BASIC_MACROS.CRM_START_YEAR + yearCombo.SelectedIndex;
            else
                selectedYear = 0;
            currentSelectedRFQItem = null;
            SetRFQsStackPanel();
        }

        private void QuarterComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (quarterCombo.SelectedItem != null)
                selectedQuarter = quarterCombo.SelectedIndex + 1;
            else
                selectedQuarter = 0;
            currentSelectedRFQItem = null;
            SetRFQsStackPanel();
        }

        private void EmployeeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (employeeCombo.SelectedItem != null)
            {
                selectedEmployee = employeesList[employeeCombo.SelectedIndex].employee_id;
                selectedTeam = employeesList[employeeCombo.SelectedIndex].team.team_id;
            }
            else
            {
                selectedEmployee = 0;
                selectedTeam = 0;
            }
            currentSelectedRFQItem = null;
            SetRFQsStackPanel();
        }

        private void ProductComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productCombo.SelectedItem != null)
                selectedProduct = productTypes[productCombo.SelectedIndex].typeId;
            else
                selectedProduct = 0;
            currentSelectedRFQItem = null;
            SetRFQsStackPanel();
        }

        private void BrandComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (brandCombo.SelectedItem != null)
                selectedBrand = brandTypes[brandCombo.SelectedIndex].brandId;
            else
                selectedBrand = 0;
            currentSelectedRFQItem = null;
            SetRFQsStackPanel();
        }

        private void StatusComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (statusCombo.SelectedItem != null)
                selectedStatus = statusCombo.SelectedIndex + 1;
            else
                selectedStatus = 0;
            currentSelectedRFQItem = null;
            SetRFQsStackPanel();
        }

        /////////////////////////////////////////////////////////////////
        //CHECKED HANDLERS
        /////////////////////////////////////////////////////////////////
        private void YearCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            yearCombo.IsEnabled = true;
            SetYearComboBox();
        }
        private void QuarterCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            quarterCombo.IsEnabled = true;
            SetQuarterComboBox();
        }
        private void EmployeeCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            employeeCombo.IsEnabled = true;
            SetEmployeeComboBox();
        }
        private void ProductCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            productCombo.IsEnabled = true;
        }
        private void BrandCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            brandCombo.IsEnabled = true;
        }
        private void StatusCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            statusCombo.IsEnabled = true;
        }

        /////////////////////////////////////////////////////////////////
        //UNCHECKED HANDLERS FUNCTIONS
        /////////////////////////////////////////////////////////////////
        private void YearCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            yearCombo.IsEnabled = false;
            yearCombo.SelectedItem = null;
            
            currentSelectedRFQItem = null;
            previousSelectedRFQItem = null;
        }
        private void QuarterCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            quarterCombo.IsEnabled = false;
            quarterCombo.SelectedItem = null;

            currentSelectedRFQItem = null;
            previousSelectedRFQItem = null;
        }
        private void EmployeeCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            employeeCombo.IsEnabled = false;
            employeeCombo.SelectedItem = null;

            currentSelectedRFQItem = null;
            previousSelectedRFQItem = null;
        }
        private void ProductCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            productCombo.SelectedItem = null;
            productCombo.IsEnabled = false;

            currentSelectedRFQItem = null;
            previousSelectedRFQItem = null;
        }
        
        private void BrandCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            brandCombo.SelectedItem = null;
            brandCombo.IsEnabled = false;

            currentSelectedRFQItem = null;
            previousSelectedRFQItem = null;
        }
        private void StatusCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            statusCombo.SelectedItem = null;
            statusCombo.IsEnabled = false;

            currentSelectedRFQItem = null;
            previousSelectedRFQItem = null;
        }

        /////////////////////////////////////////////////////////////////
        //EXTERNAL TABS
        /////////////////////////////////////////////////////////////////

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

        /////////////////////////////////////////////////////////////////
        //BTN CLICKED HANDLERS
        /////////////////////////////////////////////////////////////////

        private void OnBtnClickedAdd(object sender, RoutedEventArgs e)
        {
            int viewAddCondition = 1;
            RFQ rfq = new RFQ(sqlDatabase);
            RFQWindow addRFQWindow = new RFQWindow(ref loggedInUser, ref rfq, viewAddCondition);
            addRFQWindow.Show();
        }
        private void OnBtnClickedView(object sender, RoutedEventArgs e)
        {
            RFQ selectedRFQ = new RFQ(sqlDatabase);

            selectedRFQ.InitializeRFQInfo(rfqsList[RFQsStackPanel.Children.IndexOf(currentSelectedRFQItem)].rfq_serial,
                                            rfqsList[RFQsStackPanel.Children.IndexOf(currentSelectedRFQItem)].rfq_version,
                                            rfqsList[RFQsStackPanel.Children.IndexOf(currentSelectedRFQItem)].sales_person_id);

            int viewAddCondition = 0;
            RFQWindow viewRFQ = new RFQWindow(ref loggedInUser, ref selectedRFQ, viewAddCondition);
            viewRFQ.Show();

        }
        private void OnBtnClickedReviseRFQ(object sender, RoutedEventArgs e)
        {
            RFQ selectedRFQ = new RFQ(sqlDatabase);

            selectedRFQ.InitializeRFQInfo(rfqsListAfterFiltering[RFQsStackPanel.Children.IndexOf(currentSelectedRFQItem)].rfq_serial,
                                            rfqsListAfterFiltering[RFQsStackPanel.Children.IndexOf(currentSelectedRFQItem)].rfq_version,
                                            rfqsListAfterFiltering[RFQsStackPanel.Children.IndexOf(currentSelectedRFQItem)].sales_person_id);

            int viewAddCondition = 2;
            RFQWindow reviseRFQ = new RFQWindow(ref loggedInUser, ref selectedRFQ, viewAddCondition);
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

            resolveWorkOffer.InitializeRFQInfo(rfqsListAfterFiltering[RFQsStackPanel.Children.IndexOf(currentSelectedRFQItem)].rfq_serial,
                                            rfqsListAfterFiltering[RFQsStackPanel.Children.IndexOf(currentSelectedRFQItem)].rfq_version,
                                            rfqsListAfterFiltering[RFQsStackPanel.Children.IndexOf(currentSelectedRFQItem)].sales_person_id);
            resolveWorkOffer.LinkRFQInfo();
            int viewAddCondition = 3;
            WorkOfferWindow resolveOffer = new WorkOfferWindow(ref loggedInUser, ref resolveWorkOffer, viewAddCondition);
            resolveOffer.Show();
        }
    }

}