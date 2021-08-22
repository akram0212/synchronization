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
        private List<COMPANY_WORK_MACROS.RFQ_BASIC_STRUCT> stackPanelItems;
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
            stackPanelItems = new List<COMPANY_WORK_MACROS.RFQ_BASIC_STRUCT>();
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

            SetDefaultSettings();

            if (!GetRFQs())
                return;

            SetRFQsStackPanel();

            
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
                yearComboBox.Items.Add(year);
        }
        private void InitializeQuartersComboBox()
        {
            for (int i = 0; i < BASIC_MACROS.NO_OF_QUARTERS; i++)
                quarterComboBox.Items.Add(commonFunctionsObject.GetQuarterName(i + 1));
        }
        private bool InitializeEmployeeComboBox()
        {
            if (!commonQueriesObject.GetTeamEmployees(COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID, ref employeesList))
                return false;

            for (int i = 0; i < employeesList.Count; i++)
                employeeComboBox.Items.Add(employeesList[i].employee_name);

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

        /////////////////////////////////////////////////////////////////
        //CONFIGURATION FUNCTIONS
        /////////////////////////////////////////////////////////////////

        private void ResetComboBoxes()
        {
            yearComboBox.SelectedIndex = -1;
            quarterComboBox.SelectedIndex = -1;

            employeeComboBox.SelectedIndex = -1;

            productComboBox.SelectedIndex = -1;
            brandComboBox.SelectedIndex = -1;

            statusComboBox.SelectedIndex = -1;
        }

        private void DisableComboBoxes()
        {
            yearComboBox.IsEnabled = false;
            quarterComboBox.IsEnabled = false;
            employeeComboBox.IsEnabled = false;
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
            DisableResolveButton();
            DisableComboBoxes();
            ResetComboBoxes();

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
        }

        /////////////////////////////////////////////////////////////////
        //SET FUNCTIONS
        /////////////////////////////////////////////////////////////////
        private void SetYearComboBox()
        {
            yearComboBox.SelectedIndex = DateTime.Now.Year - BASIC_MACROS.CRM_START_YEAR;
        }
        private void SetQuarterComboBox()
        {
            if (yearComboBox.SelectedIndex == DateTime.Now.Year - BASIC_MACROS.CRM_START_YEAR)
                quarterComboBox.SelectedIndex = commonFunctionsObject.GetCurrentQuarter();
            else
                quarterComboBox.SelectedIndex = 0;
        }

        private void SetEmployeeComboBox()
        {
            employeeComboBox.SelectedIndex = 0;

            for (int i = 0; i < employeesList.Count; i++)
                if (loggedInUser.GetEmployeeId() == employeesList[i].employee_id)
                    employeeComboBox.SelectedIndex = i;
        }

        private bool SetRFQsStackPanel()
        {
            DisableReviseButton();
            DisableViewButton();
            DisableResolveButton();

            RFQsStackPanel.Children.Clear();

            if(stackPanelItems.Count() != 0)
                stackPanelItems.Clear();

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
                    //COMPARE ONLY BY ID NOT NAME
                    if (employeeCheckBox.IsChecked == true && employeeComboBox.SelectedItem != null  && employeesList[employeeComboBox.SelectedIndex].employee_id != rfqsList[i].sales_person_id)
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

        private void OnSelChangedYearCombo(object sender, SelectionChangedEventArgs e)
        {
            if (yearComboBox.SelectedItem != null)
                selectedYear = BASIC_MACROS.CRM_START_YEAR + yearComboBox.SelectedIndex;
            else
                selectedYear = 0;

            //currentSelectedRFQItem = null;
            SetRFQsStackPanel();
        }

        private void OnSelChangedQuarterCombo(object sender, SelectionChangedEventArgs e)
        {
            if (quarterComboBox.SelectedItem != null)
                selectedQuarter = quarterComboBox.SelectedIndex + 1;
            else
                selectedQuarter = 0;

            //currentSelectedRFQItem = null;
            SetRFQsStackPanel();
        }

        private void OnSelChangedEmployeeCombo(object sender, SelectionChangedEventArgs e)
        {
            if (employeeComboBox.SelectedItem != null)
            {
                selectedEmployee = employeesList[employeeComboBox.SelectedIndex].employee_id;
                selectedTeam = employeesList[employeeComboBox.SelectedIndex].team.team_id;
            }
            else
            {
                selectedEmployee = 0;
                selectedTeam = 0;
            }

            //currentSelectedRFQItem = null;
            SetRFQsStackPanel();
        }

        private void OnSelChangedProductCombo(object sender, SelectionChangedEventArgs e)
        {
            if (productComboBox.SelectedItem != null)
                selectedProduct = productTypes[productComboBox.SelectedIndex].typeId;
            else
                selectedProduct = 0;

            //currentSelectedRFQItem = null;
            SetRFQsStackPanel();
        }

        private void OnSelChangedBrandCombo(object sender, SelectionChangedEventArgs e)
        {
            if (brandComboBox.SelectedItem != null)
                selectedBrand = brandTypes[brandComboBox.SelectedIndex].brandId;
            else
                selectedBrand = 0;

            //currentSelectedRFQItem = null;
            SetRFQsStackPanel();
        }

        private void OnSelChangedStatusCombo(object sender, SelectionChangedEventArgs e)
        {
            if (statusComboBox.SelectedItem != null)
                selectedStatus = statusComboBox.SelectedIndex + 1;
            else
                selectedStatus = 0;

            //currentSelectedRFQItem = null;
            SetRFQsStackPanel();
        }

        /////////////////////////////////////////////////////////////////
        //CHECKED HANDLERS
        /////////////////////////////////////////////////////////////////
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
        private void OnCheckEmployeeCheckBox(object sender, RoutedEventArgs e)
        {
            employeeComboBox.IsEnabled = true;
            SetEmployeeComboBox();
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

        /////////////////////////////////////////////////////////////////
        //UNCHECKED HANDLERS FUNCTIONS
        /////////////////////////////////////////////////////////////////
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
        private void OnUncheckEmployeeCheckBox(object sender, RoutedEventArgs e)
        {
            employeeComboBox.IsEnabled = false;
            employeeComboBox.SelectedItem = null;

            //currentSelectedRFQItem = null;
            //previousSelectedRFQItem = null;
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
            int viewAddCondition = COMPANY_WORK_MACROS.RFQ_ADD_CONDITION;
            RFQ rfq = new RFQ(sqlDatabase);
            RFQWindow addRFQWindow = new RFQWindow(ref loggedInUser, ref rfq, viewAddCondition);
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
    }

}