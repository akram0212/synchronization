using _01electronics_erp;
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
    /// Interaction logic for WorkOffersPage.xaml
    /// </summary>
    public partial class WorkOffersPage : Page
    {
        private Employee loggedInUser;

        private SQLServer sqlDatabase;
        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;

        private int finalYear = Int32.Parse(DateTime.Now.Year.ToString());

        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> employeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_WORK_MACROS.WORK_OFFER_MAX_STRUCT> workOffers = new List<COMPANY_WORK_MACROS.WORK_OFFER_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.WORK_OFFER_MAX_STRUCT> workOffersAfterFiltering = new List<COMPANY_WORK_MACROS.WORK_OFFER_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> productTypes = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
        private List<COMPANY_WORK_MACROS.BRAND_STRUCT> brandTypes = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();

        private int selectedYear;
        private int selectedQuarter;
        private int selectedEmployee;
        private int selectedTeam;
        private int selectedProduct;
        private int selectedBrand;
        private int selectedStatus;

        private Grid previousSelectedOfferItem;
        private Grid currentSelectedOfferItem;

        public WorkOffersPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            if (!GetWorkOffers())
                return;

            InitializeYearsComboBox();
            InitializeQuartersComboBox();
            InitializeStatusComboBox();

            if (!InitializeEmployeeComboBox())
                return;

            if (!InitializeProductsComboBox())
                return;

            if (!InitializeBrandsComboBox())
                return;

            DisableComboBoxes();
            ResetComboBoxes();
            DisableViewBtn();

            ConfigureUIElements();

            SetWorkOffersStackPanel();
        }

        private void DisableEmployeeUIElements()
        {
            employeeCheckBox.IsChecked = true;
            employeeCheckBox.IsEnabled = false;
            employeeCombo.IsEnabled = false;
        }

        private void ResetComboBoxes()
        {
            yearCombo.SelectedItem = null;

            quarterCombo.SelectedItem = null;

            employeeCombo.SelectedItem = null;

            productCombo.SelectedItem = null;

            brandCombo.SelectedItem = null;

            statusCombo.SelectedItem = null;
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

        private void DisableViewBtn()
        {
            viewButton.IsEnabled = false;
        }
        private void EnableViewBtn()
        {
            viewButton.IsEnabled = true;
        }
        /////////////////////////////////////////////////////////////////
        //GET DATA FUNCTIONS
        /////////////////////////////////////////////////////////////////

        private bool GetWorkOffers()
        {
            if (!commonQueriesObject.GetWorkOffers(ref workOffers))
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
            if (!commonQueriesObject.GetDepartmentEmployees(COMPANY_ORGANISATION_MACROS.MARKETING_AND_SALES_DEPARTMENT_ID, ref employeesList))
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

            brandCombo.IsEnabled = false;
            return true;
        }

        private void InitializeStatusComboBox()
        {
           
            statusCombo.Items.Add("Failed");
            statusCombo.Items.Add("Confirmed");
            statusCombo.Items.Add("Pending");
            statusCombo.IsEnabled = false;
        }

        /////////////////////////////////////////////////////////////////
        //CONFIGURATION FUNCTIONS
        /////////////////////////////////////////////////////////////////

        private void ConfigureUIElements()
        {
            EnableYearUIElements();

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


        private bool SetWorkOffersStackPanel()
        {
            
            workOffersStackPanel.Children.Clear();

            for (int i = 0; i < workOffers.Count; i++)
            {
                DateTime currentWorkOfferDate = DateTime.Parse(workOffers[i].issue_date);

                bool salesPersonCondition =
                   selectedTeam == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID &&
                   selectedEmployee != workOffers[i].sales_person_id;

                bool assigneeCondition =
                    selectedTeam == COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID &&
                    selectedEmployee != workOffers[i].offer_proposer_id;

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

                if (employeeCheckBox.IsChecked == true && salesPersonCondition )
                    continue;

                if (employeeCheckBox.IsChecked == true && assigneeCondition)
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
                workOffersStackPanel.Children.Add(grid);
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
            currentSelectedOfferItem = null;
            SetWorkOffersStackPanel();
        }

        private void QuarterComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (quarterCombo.SelectedItem != null)
                selectedQuarter = quarterCombo.SelectedIndex + 1;
            else
                selectedQuarter = 0;
            currentSelectedOfferItem = null;
            SetWorkOffersStackPanel();
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
            currentSelectedOfferItem = null;
            SetWorkOffersStackPanel();
        }

        private void ProductComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productCombo.SelectedItem != null)
                selectedProduct = productTypes[productCombo.SelectedIndex].typeId;
            else
                selectedProduct = 0;
            currentSelectedOfferItem = null;
            SetWorkOffersStackPanel();
        }

        private void BrandComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (brandCombo.SelectedItem != null)
                selectedBrand = brandTypes[brandCombo.SelectedIndex].brandId;
            else
                selectedBrand = 0;
            currentSelectedOfferItem = null;
            SetWorkOffersStackPanel();
        }

        private void StatusComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (statusCombo.SelectedItem != null)
                selectedStatus = statusCombo.SelectedIndex + 1;
            else
                selectedStatus = 0;
            currentSelectedOfferItem = null;
            SetWorkOffersStackPanel();
        }

        /////////////////////////////////////////////////////////////////
        //CHECKED HANDLERS
        /////////////////////////////////////////////////////////////////

        private void YearCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            yearCombo.IsEnabled = true;
            currentSelectedOfferItem = null;
            SetYearComboBox();
        }

        private void QuarterCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            quarterCombo.IsEnabled = true;
            currentSelectedOfferItem = null;
            SetQuarterComboBox();
        }

        private void EmployeeCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            employeeCombo.IsEnabled = true;
            currentSelectedOfferItem = null;
            SetEmployeeComboBox();
        }

        private void ProductCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            productCombo.IsEnabled = true;
            currentSelectedOfferItem = null;

        }

        private void BrandCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            brandCombo.IsEnabled = true;
            currentSelectedOfferItem = null;
        }

        private void StatusCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            statusCombo.IsEnabled = true;
            currentSelectedOfferItem = null;
        }
        /////////////////////////////////////////////////////////////////
        //UNCHECKED HANDLERS FUNCTIONS
        /////////////////////////////////////////////////////////////////


        private void YearCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            yearCombo.SelectedItem = null;
            yearCombo.IsEnabled = false;
            currentSelectedOfferItem = null;
        }

        
        private void QuarterCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            quarterCombo.SelectedItem = null;
            quarterCombo.IsEnabled = false;
            currentSelectedOfferItem = null;
        }


        private void EmployeeCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            employeeCombo.SelectedItem = null;
            employeeCombo.IsEnabled = false;
            currentSelectedOfferItem = null;
        }

        private void ProductCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            productCombo.SelectedItem = null;
            productCombo.IsEnabled = false;
            currentSelectedOfferItem = null;
        }

        private void BrandCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            brandCombo.SelectedItem = null;
            brandCombo.IsEnabled = false;
            currentSelectedOfferItem = null;
        }

        private void StatusCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            statusCombo.SelectedItem = null;
            statusCombo.IsEnabled = false;
            currentSelectedOfferItem = null;
        }

        /////////////////////////////////////////////////////////////////
        //BUTTON CLICKED FUNCTIONS
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

        private void OnButtonClickedOrders(object sender, RoutedEventArgs e)
        {

        }
        private void OnButtonClickedOffers(object sender, RoutedEventArgs e)
        {

        }
        private void OnButtonClickedRFQs(object sender, RoutedEventArgs e)
        {
            RFQsPage rfqs = new RFQsPage(ref loggedInUser);
            this.NavigationService.Navigate(rfqs);
        }
        private void OnButtonClickedVisits(object sender, RoutedEventArgs e)
        {

        }
        private void OnButtonClickedCalls(object sender, RoutedEventArgs e)
        {

        }
        private void OnButtonClickedMeetings(object sender, RoutedEventArgs e)
        {

        }
        private void OnButtonClickedStatistics(object sender, RoutedEventArgs e)
        {

        }

        private void OnButtonClickedworkOrders(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnBtnClickedAdd(object sender, RoutedEventArgs e)
        {
            int viewAddCondition = 1;
            WorkOffer workOffer = new WorkOffer(sqlDatabase);

            WorkOfferWindow workOfferWindow = new WorkOfferWindow(ref loggedInUser, ref workOffer, viewAddCondition);
            workOfferWindow.Show();
        }

        private void OnBtnClickedView(object sender, RoutedEventArgs e)
        {
            WorkOffer selectedWorkOffer = new WorkOffer(sqlDatabase);

            selectedWorkOffer.InitializeSalesWorkOfferInfo( workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentSelectedOfferItem)].offer_serial,
                                                            workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentSelectedOfferItem)].offer_version);

            int viewAddCondition = 0;
            WorkOfferWindow viewOffer = new WorkOfferWindow(ref loggedInUser, ref selectedWorkOffer, viewAddCondition);
            viewOffer.Show();
        }
        private void OnBtnClickedWorkOfferItem(object sender, RoutedEventArgs e)
        {
            EnableViewBtn();
            previousSelectedOfferItem = currentSelectedOfferItem;
            currentSelectedOfferItem = (Grid)sender;
            BrushConverter brush = new BrushConverter();

            if (previousSelectedOfferItem != null)
            {
                previousSelectedOfferItem.Background = (Brush)brush.ConvertFrom("#FFFFFF");

                StackPanel previousSelectedStackPanel = (StackPanel)previousSelectedOfferItem.Children[0];
                Border previousSelectedBorder = (Border)previousSelectedOfferItem.Children[1];
                Label previousStatusLabel = (Label)previousSelectedBorder.Child;

                foreach (Label childLabel in previousSelectedStackPanel.Children)
                    childLabel.Foreground = (Brush)brush.ConvertFrom("#000000");

                if (workOffers[workOffersStackPanel.Children.IndexOf(previousSelectedOfferItem)].offer_status_id == COMPANY_WORK_MACROS.PENDING_RFQ)
                    previousSelectedBorder.Background = (Brush)brush.ConvertFrom("#FFA500");
                else if (workOffers[workOffersStackPanel.Children.IndexOf(previousSelectedOfferItem)].offer_status_id == COMPANY_WORK_MACROS.CONFIRMED_RFQ)
                    previousSelectedBorder.Background = (Brush)brush.ConvertFrom("#008000");
                else
                    previousSelectedBorder.Background = (Brush)brush.ConvertFrom("#FF0000");

                previousStatusLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");
            }

            currentSelectedOfferItem.Background = (Brush)brush.ConvertFrom("#105A97");

            StackPanel currentSelectedStackPanel = (StackPanel)currentSelectedOfferItem.Children[0];
            Border currentSelectedBorder = (Border)currentSelectedOfferItem.Children[1];
            Label currentStatusLabel = (Label)currentSelectedBorder.Child;

            foreach (Label childLabel in currentSelectedStackPanel.Children)
                childLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");

            currentSelectedBorder.Background = (Brush)brush.ConvertFrom("#FFFFFF");
            currentStatusLabel.Foreground = (Brush)brush.ConvertFrom("#105A97");
        }
    }
}

