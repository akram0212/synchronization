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
using _01electronics_erp;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for RFQsPage.xaml
    /// </summary>
    public partial class RFQsPage : Page
    {
        private Employee loggedInUser;

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;

        private int finalYear = Int32.Parse(DateTime.Now.Year.ToString());

        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> employeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();

        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsList;
        
        private List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> productTypes;
        private List<COMPANY_WORK_MACROS.BRAND_STRUCT> brandTypes;
        public RFQsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;

            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            rfqsList = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();
            //offersList = new List<COMPANY_WORK_MACROS.WORK_OFFER_MAX_STRUCT>();

            productTypes = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
            brandTypes = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();

            InitializeYearsComboBox();
            InitializeQuartersComboBox();
            InitializeStatusComboBox();

            if (!InitializeEmployeeComboBox())
                return;

            if (!InitializeProductsComboBox())
                return;

            if (!InitializeBrandsComboBox())
                return;

            if (!InitializeRFQsStackPanel())
              return;
            
        }

        private void InitializeYearsComboBox()
        {
            int initialYear = 2020;
            for (; initialYear <= finalYear; initialYear++)
                yearCombo.Items.Add(initialYear);

            yearCheckBox.IsChecked = true;
            yearCombo.SelectedItem = finalYear;
        }
        private void InitializeQuartersComboBox()
        {
            //INSTEAD OF HARD CODING YOUR COMBO, 
            //THIS FUNCTION IS BETTER SO EVERYTIME IN OUR PROJECT WE NEED TO LIST QUARTERS
            //WE ARE SURE THAT ALL HAVE THE SAME FORMAT
            for (int i = 0; i < BASIC_MACROS.NO_OF_QUARTERS; i++)
                quarterCombo.Items.Add(commonFunctionsObject.GetQuarterName(i + 1));

            quarterCheckBox.IsChecked = true;
            quarterCombo.SelectedItem = quarterCombo.Items.GetItemAt(0);
            //ALSO IF YOU NOTICE, I DIDN'T EVEN USE i < 4, I USED A PRE-DEFINED MACRO INSTEAD, SO THE CODE IS ALWAYS READABLE
        }
        private bool InitializeEmployeeComboBox()
        {
            if (!commonQueriesObject.GetCompanyEmployees(ref employeesList))
                return false;

            for (int i = 0; i < employeesList.Count; i++)
            {
                employeeCombo.Items.Add(employeesList[i].employee_name);
            }
            
            employeeCombo.IsEnabled = false;

            if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID || loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID)
            {
                employeeCheckBox.IsChecked = true;
                employeeCheckBox.IsEnabled = false;
                employeeCombo.IsEnabled = false;
                for (int i = 0; i < employeesList.Count; i++)
                {
                    if (loggedInUser.GetEmployeeId() == employeesList[i].employee_id)
                    {
                        //employeeCombo.Items.Clear();
                        //employeeCombo.Items.Add(employeesList[i].employee_name);
                        //employeeCombo.SelectedItem = employeeCombo.Items.GetItemAt(0);
                        employeeCombo.SelectedItem = employeeCombo.Items.GetItemAt(i);
                    }
                }
            }
            return true;
        }

        //THIS FUNCTIONS ACCESS SQL SERVER, SO YOU SHALL ALWAYS CHECK IF THE QUERY IS COMPLETED SUCCESSFULLY
        // IF NOT YOU SHALL STOP DATA ACCESS FOR THE CODE NOT TO CRASH
        private bool InitializeProductsComboBox()
        {
            if (!commonQueriesObject.GetCompanyProducts(ref productTypes))
                return false;

            for (int i = 0; i < productTypes.Count; i++)
                productCombo.Items.Add(productTypes[i].typeName);

            productCombo.IsEnabled = false;
            return true;
        }

        private bool InitializeBrandsComboBox()
        {
            //INTENTIONALLY LEFT IT EMPTY FOR YOU TO FILL
            if (!commonQueriesObject.GetCompanyBrands(ref brandTypes))
                return false;
            for (int i = 0; i < brandTypes.Count; i++)
                brandCombo.Items.Add(brandTypes[i].brandName);

            brandCombo.IsEnabled = false;
            return true;
        }

        private void InitializeStatusComboBox()
        {
            //for(int i = 0; i< COMPANY_WORK_MACROS.NO_OF_RFQS_STATUS; i++)
                //statusCombo.Items.Add(commonFunctionsObject.)
            statusCombo.Items.Add("Pending");
            statusCombo.Items.Add("Offered");
            statusCombo.Items.Add("Rejected");
            statusCombo.IsEnabled = false;
        }
        private bool InitializeRFQsStackPanel()
        {
            RFQsStackPanel.Children.Clear();

             if (!commonQueriesObject.GetRFQs(ref rfqsList))
               return false;
             
            for (int i = 0; i < rfqsList.Count; i++)
            {
                if(yearCheckBox.IsChecked == true)
                    if(rfqsList[i].issue_date == "")


                if (employeeCheckBox.IsChecked == true)
                {
                    COMPANY_ORGANISATION_MACROS.TEAM_STRUCT employeeTeam = employeesList[employeeCombo.SelectedIndex].team;
                    if (employeeTeam.team_id == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
                    {
                        if (employeeCombo.SelectedIndex + 1 != rfqsList[i].sales_person_id)
                            continue;
                    }
                    else if(employeeTeam.team_id == COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID)
                    {
                        if (employeeCombo.SelectedIndex + 1 != rfqsList[i].assignee_id)
                            continue;
                    }
                }

                if (productCheckBox.IsChecked == true)
                {
                    List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> tempProduct = rfqsList[i].products_type;
                    if (productCombo.SelectedIndex + 1 != tempProduct[0].typeId)
                    {
                        tempProduct.Clear();
                        continue;
                    }
                }


                if (brandCheckBox.IsChecked == true)
                {
                    List < COMPANY_WORK_MACROS.BRAND_STRUCT > tempBrand = rfqsList[i].products_brand;
                    if (brandCombo.SelectedIndex + 1 != tempBrand[0].brandId)
                    {
                        tempBrand.Clear();
                        continue;
                    }
                }
                if (statusCheckBox.IsChecked == true)
                    if (statusCombo.SelectedIndex + 1 != rfqsList[i].rfq_status_id)
                        continue;

                StackPanel fullStackPanel = new StackPanel();
                fullStackPanel.Orientation = Orientation.Vertical;

                Label rfqIDLabel = new Label();
                rfqIDLabel.Content = rfqsList[i].rfq_id;
               
                Label salesLabel = new Label();
                salesLabel.Content = rfqsList[i].sales_person_name;

                Label preSalesLabel = new Label();
                preSalesLabel.Content = rfqsList[i].assignee_name;

                Label companyAndContactLabel = new Label();
                companyAndContactLabel.Content = rfqsList[i].company_name + " -" + rfqsList[i].contact_name;

                Label productTypeAndBrandLabel = new Label();
                List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> tempProduct1 = rfqsList[i].products_type;
                List<COMPANY_WORK_MACROS.BRAND_STRUCT> tempBrand1 = rfqsList[i].products_brand;
                productTypeAndBrandLabel.Content = tempProduct1[0].typeName + " ," + tempBrand1[0].brandName;
                tempProduct1.Clear();
                tempBrand1.Clear();

                Label contractTypeLabel = new Label();
                contractTypeLabel.Content = rfqsList[i].contract_type;

                Label rfqStatusLabel = new Label();
                rfqStatusLabel.Content = rfqsList[i].rfq_status;
                
                fullStackPanel.Children.Add(rfqIDLabel);
                fullStackPanel.Children.Add(salesLabel);
                fullStackPanel.Children.Add(preSalesLabel);
                fullStackPanel.Children.Add(companyAndContactLabel);
                fullStackPanel.Children.Add(productTypeAndBrandLabel);
                fullStackPanel.Children.Add(contractTypeLabel);
                fullStackPanel.Children.Add(rfqStatusLabel);

                RFQsStackPanel.Children.Add(fullStackPanel); 
            }
            //if (!commonQueriesObject.GetWorkOffers(ref offersList))
            //    return false;

            return true;
        }
      
        private void OnBtnClickedAdd(object sender, RoutedEventArgs e)
        {

        }

        private void OnBtnClickedView(object sender, RoutedEventArgs e)
        {

        }

        private void YearComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitializeRFQsStackPanel();
        }

        private void QuarterComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitializeRFQsStackPanel();
        }

        private void EmployeeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitializeRFQsStackPanel();
        }

        private void ProductComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitializeRFQsStackPanel();
        }

        private void BrandComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitializeRFQsStackPanel();
        }

        private void StatusComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitializeRFQsStackPanel();
        }

        private void YearCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            yearCombo.IsEnabled = true;
            yearCombo.SelectedItem = finalYear;
        }

        private void YearCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            yearCombo.SelectedItem = null;
            yearCombo.IsEnabled = false;
        }

        private void QuarterCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            quarterCombo.IsEnabled = true;
            quarterCombo.SelectedItem = quarterCombo.Items.GetItemAt(0);
        }

        private void QuarterCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            quarterCombo.SelectedItem = null;
            quarterCombo.IsEnabled = false;
        }

        private void EmployeeCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            employeeCombo.IsEnabled = true;
        }

        private void EmployeeCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            employeeCombo.SelectedItem = null;
            employeeCombo.IsEnabled = false;
        }

        private void ProductCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            productCombo.IsEnabled = true;
        }

        private void ProductCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            productCombo.SelectedItem = null;
            productCombo.IsEnabled = false;
        }

        private void BrandCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            brandCombo.IsEnabled = true;
        }

        private void BrandCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            brandCombo.SelectedItem = null;
            brandCombo.IsEnabled = false;
        }

        private void StatusCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            statusCombo.IsEnabled = true;
        }

        private void StatusCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            statusCombo.SelectedItem = null;
            statusCombo.IsEnabled = false;
        }

        private void OnButtonClickedMyProfile(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnButtonClickedContacts(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnButtonClickedWorkOrders(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnButtonClickedOffers(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnButtonClickedRFQs(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnButtonClickedVisits(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnButtonClickedCalls(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnButtonClickedMeetings(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnButtonClickedStatistics(object sender, MouseButtonEventArgs e)
        {

        }


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
    }
}
