﻿using System;
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
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsList = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> productTypes = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
        private List<COMPANY_WORK_MACROS.BRAND_STRUCT> brandTypes = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();
        bool skipLoopProductFilter;
        bool skipLoopBrandFilter;
        
        public RFQsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;

            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

           // rfqsList = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();
           // productTypes = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
           // brandTypes = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();

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
                skipLoopProductFilter = true;
                skipLoopBrandFilter = true;

                if (yearCheckBox.IsChecked == true)
                { 
                    Int32 tempYearComboSelectedItem = Int32.Parse(yearCombo.SelectedItem.ToString());
                    DateTime tempYearList = DateTime.Parse(rfqsList[i].issue_date);
                    Int32 tempYearList1 = Int32.Parse(tempYearList.Year.ToString());
                    if (tempYearComboSelectedItem != tempYearList1)
                        continue;
                }

                if (employeeCheckBox.IsChecked == true)
                {
                    COMPANY_ORGANISATION_MACROS.TEAM_STRUCT employeeTeam = employeesList[employeeCombo.SelectedIndex].team;
                    if (employeeCombo.SelectedItem != null)
                    {
                        if (employeeTeam.team_id == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
                        {
                            string tempSalesPersonName = rfqsList[i].sales_person_name;

                            if (employeeCombo.SelectedItem.ToString() != tempSalesPersonName)
                                continue;
                        }
                        else if (employeeTeam.team_id == COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID)
                        {
                            string tempAssigneeName = rfqsList[i].assignee_name;

                            if (tempAssigneeName != employeeCombo.SelectedItem.ToString())
                                continue;
                        }
                        else
                        {
                            MessageBox.Show("The employee you chose is not from sales department so he doesnt have any RFQs");
                            i = rfqsList.Count;
                            continue;
                        }
                    }
                }

                if (quarterCheckBox.IsChecked == true)
                {
                    DateTime tempQuarterList;
                    Int32 tempMonth;

                    tempQuarterList = DateTime.Parse(rfqsList[i].issue_date);
                    tempMonth = tempQuarterList.Month;
                   // tempMonth = Int32.Parse(tempQuarterList.Month.ToString());

                    if (quarterCombo.SelectedIndex + 1 == BASIC_MACROS.FIRST_QUARTER)
                    {
                        if (tempMonth > BASIC_MACROS.FIRST_QUARTER_END_MONTH)
                            continue;
                    }

                    else if (quarterCombo.SelectedIndex + 1 == BASIC_MACROS.SECOND_QUARTER)
                    {
                        if (tempMonth < BASIC_MACROS.SECOND_QUARTER_START_MONTH || tempMonth > BASIC_MACROS.SECOND_QUARTER_END_MONTH)
                            continue;
                    }
                    else if (quarterCombo.SelectedIndex + 1 == BASIC_MACROS.THIRD_QUARTER)
                    {
                        if (tempMonth < BASIC_MACROS.THIRD_QUARTER_START_MONTH || tempMonth > BASIC_MACROS.THIRD_QUARTER_END_MONTH)
                            continue;
                    }
                    else
                    {
                        if (tempMonth < BASIC_MACROS.FOURTH_QUARTER_START_MONTH)
                            continue;
                    } 
                }


                if (productCheckBox.IsChecked == true)
                {
                    List<COMPANY_WORK_MACROS.RFQ_PRODUCT_STRUCT> tempType = rfqsList[i].products;

                    if (productCombo.SelectedItem != null)
                    {
                        for (int j = 0; j < COMPANY_WORK_MACROS.MAX_RFQ_PRODUCTS; j++)
                        {
                            COMPANY_WORK_MACROS.PRODUCT_STRUCT tempType0 = tempType[j].productType;

                            if (productCombo.SelectedIndex + 1 == tempType0.typeId)
                            {
                                skipLoopProductFilter = false;
                                j = COMPANY_WORK_MACROS.MAX_RFQ_PRODUCTS;
                            }
                        }
                    }
                }

                if (brandCheckBox.IsChecked == true)
                {
                    List<COMPANY_WORK_MACROS.RFQ_PRODUCT_STRUCT> tempBrand = rfqsList[i].products;

                    if (brandCombo.SelectedItem != null)
                    {
                        for (int j = 0; j < COMPANY_WORK_MACROS.MAX_RFQ_PRODUCTS; j++)
                        {
                            COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand0 = tempBrand[j].productBrand;
                            if (brandCombo.SelectedIndex + 1 == tempBrand0.brandId)
                            { 
                                skipLoopBrandFilter = false;
                                j = COMPANY_WORK_MACROS.MAX_RFQ_PRODUCTS;
                            }
                        }
                    }
                }

                if (statusCheckBox.IsChecked == true)
                {
                    if (statusCombo.SelectedItem != null)
                    {
                        string rfqStatus = rfqsList[i].rfq_status;
                        if (statusCombo.SelectedItem.ToString() != rfqStatus)
                            continue;
                    }
                }
                if (productCheckBox.IsChecked == true)
                {
                    if (skipLoopProductFilter == true)
                        continue;
                }

                if(brandCheckBox.IsChecked == true)
                {
                    if (skipLoopBrandFilter == true)
                        continue;
                }

                StackPanel fullStackPanel = new StackPanel();
                fullStackPanel.Orientation = Orientation.Vertical;
                //fullStackPanel.Height = 90;

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

                if(rfqsList[i].rfq_status_id == COMPANY_WORK_MACROS.PENDING_RFQ)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500"));
                }
                else if(rfqsList[i].rfq_status_id == COMPANY_WORK_MACROS.CONFIRMED_RFQ)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008000"));
                }
                else
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                }

                borderIcon.Child = rfqStatusLabel;

                fullStackPanel.Children.Add(rfqIDLabel);
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

                RFQsStackPanel.Children.Add(grid); 
            }
            //if (!commonQueriesObject.GetWorkOffers(ref offersList))
            //    return false;

            return true;
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
            WorkOffersPage workOffers = new WorkOffersPage(ref loggedInUser);
            this.NavigationService.Navigate(workOffers);
        }
        private void OnButtonClickedRFQs(object sender, RoutedEventArgs e)
        {
            //RFQsPage rfqs = new RFQsPage(ref loggedInUser);
            //this.NavigationService.Navigate(rfqs);
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
            var addRFQWindow = new AddRFQWindow(ref loggedInUser);
            addRFQWindow.Show();
        }

        private void OnBtnClickedView(object sender, RoutedEventArgs e)
        {

        }
    }
}
