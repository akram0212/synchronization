using _01electronics_erp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for ContactsPage.xaml
    /// </summary>
    public partial class ContactsPage : Page
    {
        private String sqlQuery;

        private SQLServer initializationObject;

        TextBlock selectedTextBlock;
        private List<TextBlock> AllTextBlocks = new List<TextBlock>();

        CommonQueries commonQueries;
        private Employee loggedInUser;
        private int employeeID;
        List<COMPANY_ORGANISATION_MACROS.LIST_CONTACT_STRUCT> employeeContacts;
        List<COMPANY_ORGANISATION_MACROS.LIST_COMPANY_STRUCT> companys;
        List<BASIC_STRUCTS.COUNTRY_STRUCT> countries;
        List<BASIC_STRUCTS.STATE_STRUCT> states;
        List<BASIC_STRUCTS.CITY_STRUCT> cities;
        List<BASIC_STRUCTS.DISTRICT_STRUCT> districts;


        public ContactsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();
            initializationObject = new SQLServer();

            employeeContacts = new List<COMPANY_ORGANISATION_MACROS.LIST_CONTACT_STRUCT>();
            companys = new List<COMPANY_ORGANISATION_MACROS.LIST_COMPANY_STRUCT>();
            countries = new List<BASIC_STRUCTS.COUNTRY_STRUCT>();
            states = new List<BASIC_STRUCTS.STATE_STRUCT>();
            cities = new List<BASIC_STRUCTS.CITY_STRUCT>();
            districts = new List<BASIC_STRUCTS.DISTRICT_STRUCT>();

            commonQueries = new CommonQueries();
            loggedInUser = mLoggedInUser;
            employeeID = mLoggedInUser.GetEmployeeId();

            countryComboBox.IsEnabled = false; 
            stateComboBox.IsEnabled = false;
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;
            salesPersonComboBox.IsEnabled = false;

            commonQueries.GetAllCountries(ref countries);
            for (int i = 0; i < countries.Count; i++)
            {
                countryComboBox.Items.Add(countries[i].country_name);
            }
            
            commonQueries.GetAllStates(ref states);
            for (int i = 0; i < states.Count; i++)
            {
                stateComboBox.Items.Add(states[i].state_name);
            }
            
            commonQueries.GetAllCities(ref cities);
            for (int i = 0; i < cities.Count; i++)
            {
                cityComboBox.Items.Add(cities[i].city_name);
            } 
            
            commonQueries.GetAllDistricts(ref districts);
            for (int i = 0; i < districts.Count; i++)
            {
                districtComboBox.Items.Add(districts[i].district_name);
            }


            stateCheckBox.IsEnabled = false;
            cityCheckBox.IsEnabled = false;
            districtCheckBox.IsEnabled = false;
            salesPersonCheckBox.IsEnabled = false;

            InitializeCompaniesListBox();
            InitializeContactsListBox();


        }


        public bool InitializeCompaniesListBox()
        {
            treeViewItem.Items.Clear();
            string sqlQueryPart1 = @"select distinct company_name.company_serial, 

                                    company_address.address, 
									company_field_of_work.work_field, 
									company_address.address_serial, 
									company_name.company_name
                            from erp_system.dbo.company_name
                            inner join erp_system.dbo.company_field_of_work
                            on company_name.company_serial = company_field_of_work.company_serial

                            inner join erp_system.dbo.company_address
                            on company_name.company_serial = company_address.company_serial

                            left join erp_system.dbo.contact_person_info
                            on company_address.address_serial = contact_person_info.branch_serial

                            where contact_person_info.sales_person_id = ";

            String sqlQueryPart2 = " or company_name.added_by = ";

            String sqlQueryPart3 = " order by company_name;";

            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += employeeID;
            sqlQuery += sqlQueryPart2;
            sqlQuery += employeeID;
            sqlQuery += sqlQueryPart3;


            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 4;
            queryColumns.sql_string = 1;

            if (!initializationObject.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            for (int i = 0; i < initializationObject.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.LIST_COMPANY_STRUCT tmpCompanyStruct;
                tmpCompanyStruct.company_serial = initializationObject.rows[i].sql_int[0];
                tmpCompanyStruct.address = initializationObject.rows[i].sql_int[1];
                tmpCompanyStruct.work_field = initializationObject.rows[i].sql_int[2];
                tmpCompanyStruct.address_serial = initializationObject.rows[i].sql_int[3];
                tmpCompanyStruct.company_name = initializationObject.rows[i].sql_string[0];
                companys.Add(tmpCompanyStruct);
            }


            TreeViewItem ParentItem = new TreeViewItem();
            ParentItem.Header = loggedInUser.GetEmployeeName();
            ParentItem.Tag = employeeID.ToString();
            treeViewItem.Items.Add(ParentItem);

            for (int i = 0; i < companys.Count; i++)
            {
                TreeViewItem ChildItem = new TreeViewItem();
                ChildItem.Header = companys[i].company_name;
                ChildItem.Tag = companys[i].company_serial.ToString();
                ParentItem.Items.Add(ChildItem);
            }
            return true;
        }

        public bool InitializeContactsListBox()
        {
            string QueryPart1 = @"select contact_person_info.contact_id, 

                                    company_address.company_serial, 
									company_address.address_serial, 
									company_address.address, 
									contact_person_info.name, 
									departments_type.department
                            from erp_system.dbo.contact_person_info
                            inner join erp_system.dbo.company_address
                            on contact_person_info.branch_serial = company_address.address_serial

                            inner join erp_system.dbo.departments_type
                            on contact_person_info.department = departments_type.id

                            where contact_person_info.sales_person_id = ";

            String QueryPart2 = @" and contact_person_info.is_valid = 1 
                                  order by company_address.address; ";

            sqlQuery = string.Empty;
            sqlQuery += QueryPart1;
            sqlQuery += employeeID;
            sqlQuery += QueryPart2;
            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns2 = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns2.sql_int = 4;
            queryColumns2.sql_string = 2;

            if (!initializationObject.GetRows(sqlQuery, queryColumns2, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            for (int i = 0; i < initializationObject.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.LIST_CONTACT_STRUCT tmpCompanyStruct;
                tmpCompanyStruct.contact_id = initializationObject.rows[i].sql_int[0];
                tmpCompanyStruct.company_serial = initializationObject.rows[i].sql_int[1];
                tmpCompanyStruct.address_serial = initializationObject.rows[i].sql_int[2];
                tmpCompanyStruct.address = initializationObject.rows[i].sql_int[3];
                tmpCompanyStruct.contact_name = initializationObject.rows[i].sql_string[0];
                tmpCompanyStruct.department = initializationObject.rows[i].sql_string[1];
                employeeContacts.Add(tmpCompanyStruct);
            }
            foreach(TreeViewItem item in treeViewItem.Items)
            {
                foreach(TreeViewItem subItem in item.Items)
                {
                    for (int i = 0; i < employeeContacts.Count; i++)
                    {
                        if (subItem.Tag.ToString() == employeeContacts[i].address_serial.ToString())
                        {
                            TreeViewItem ChildItem = new TreeViewItem();
                            ChildItem.Header = employeeContacts[i].contact_name;
                            ChildItem.Tag = (employeeContacts[i].contact_id + employeeContacts[i].address_serial + employeeID).ToString();
                            subItem.Items.Add(ChildItem);
                   
                        }
                   
                    }
                   
                }
            }
           
            return true;
        }


        public void handlerfunction(object sender, RoutedEventArgs e)
        {
            selectedTextBlock = (TextBlock)sender;
            for (int i = 0; i < AllTextBlocks.Count; i++)
            {
                BrushConverter brush = new BrushConverter();
                AllTextBlocks[i].Foreground = (Brush)brush.ConvertFrom("Black");
            }
            BrushConverter brushStyle = new BrushConverter();
            selectedTextBlock.Foreground = (Brush)brushStyle.ConvertFrom("#FF105A97");
        }

        private void OnButtonClickedOrders(object sender, RoutedEventArgs e)
        {
        }
        private void OnButtonClickedOffers(object sender, RoutedEventArgs e)
        {
        }
        private void OnButtonClickedRFQs(object sender, RoutedEventArgs e)
        {
            
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

        private void OnButtonClickedMyProfile(object sender, MouseButtonEventArgs e)
        {
            UserPortalPage userPortal = new UserPortalPage(ref loggedInUser);
            this.NavigationService.Navigate(userPortal);
        }

        private void OnButtonClickedContacts(object sender, MouseButtonEventArgs e)
        {
            ContactsPage contacts = new ContactsPage(ref loggedInUser);
            this.NavigationService.Navigate(contacts);
            ContactsPage contactsPage = new ContactsPage(ref loggedInUser);
            this.NavigationService.Navigate(contactsPage);
        }

        private void countryCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            countryComboBox.IsEnabled = true;
        }

        private void stateCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            stateComboBox.IsEnabled = true;
        }

        private void cityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            cityComboBox.IsEnabled = true; 
        }

        private void districtCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            districtComboBox.IsEnabled = true;
        }

        private void salesPersonCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void countryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cityCheckBox.IsEnabled = false;
            districtCheckBox.IsEnabled = false;
            salesPersonCheckBox.IsEnabled = false;

            stateCheckBox.IsEnabled = true;

            stateCheckBox.IsChecked = false;
            cityCheckBox.IsChecked = false;
            districtCheckBox.IsChecked = false;

            stateComboBox.Items.Clear();
            int countryIndex = 0;
            for (int i = 0; i < countries.Count; i++)
            {
                if (countries[i].country_name == countryComboBox.SelectedItem)
                {
                    countryIndex = countries[i].country_id;
                }
            }
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].state_id/100 == countryIndex)
                    stateComboBox.Items.Add(states[i].state_name);
            }

            treeViewItem.Items.Clear();

            TreeViewItem ParentItem = new TreeViewItem();
            ParentItem.Tag = employeeID.ToString();
            ParentItem.Header = loggedInUser.GetEmployeeName();
            treeViewItem.Items.Add(ParentItem);
            
            for(int i = 0; i < companys.Count(); i++)
            {
                if(companys[i].address / 1000000 == countryIndex)
                {
                     TreeViewItem ChildItem = new TreeViewItem();
                     ChildItem.Header = companys[i].company_name;
                     ChildItem.Tag = companys[i].company_serial.ToString();
                     ParentItem.Items.Add(ChildItem);
                    
                }    
            }

            foreach (TreeViewItem item in treeViewItem.Items)
            {
                foreach (TreeViewItem subItem in item.Items)
                {
                    for (int i = 0; i < employeeContacts.Count; i++)
                    {
                        if (subItem.Tag.ToString() == employeeContacts[i].address_serial.ToString())
                        {
                            if (employeeContacts[i].address / 1000000 == countryIndex)
                            {
                                TreeViewItem ChildItem = new TreeViewItem();
                                ChildItem.Header = employeeContacts[i].contact_name;
                                ChildItem.Tag = (employeeContacts[i].contact_id + employeeContacts[i].address_serial + employeeID).ToString();
                                subItem.Items.Add(ChildItem);
                            }

                        }

                    }

                }
            }
        }

        private void stateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cityCheckBox.IsEnabled = true;
            cityCheckBox.IsChecked = false;
            districtCheckBox.IsChecked = false;
            districtCheckBox.IsEnabled = false;
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;

            cityComboBox.Items.Clear();
            int stateIndex = 0;
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].state_name == stateComboBox.SelectedItem)
                {
                    stateIndex = states[i].state_id;
                }
            }
            for (int i = 0; i < cities.Count; i++)
            {
                if (cities[i].city_id / 100 == stateIndex)
                cityComboBox.Items.Add(cities[i].city_name);
            }

            treeViewItem.Items.Clear();

            TreeViewItem ParentItem = new TreeViewItem();
            ParentItem.Tag = employeeID.ToString();
            ParentItem.Header = loggedInUser.GetEmployeeName();
            treeViewItem.Items.Add(ParentItem);
            for (int i = 0; i < companys.Count(); i++)
            {
                if (companys[i].address / 10000 == stateIndex)
                {
                    TreeViewItem ChildItem = new TreeViewItem();
                    ChildItem.Header = companys[i].company_name;
                    ChildItem.Tag = companys[i].company_serial.ToString();
                    ParentItem.Items.Add(ChildItem);

                }
            }
            foreach (TreeViewItem item in treeViewItem.Items)
            {
                foreach (TreeViewItem subItem in item.Items)
                {
                    for (int i = 0; i < employeeContacts.Count; i++)
                    {
                        if (subItem.Tag.ToString() == employeeContacts[i].address_serial.ToString())
                        {
                            if (employeeContacts[i].address / 10000 == stateIndex)
                            {
                                TreeViewItem ChildItem = new TreeViewItem();
                                ChildItem.Header = employeeContacts[i].contact_name;
                                ChildItem.Tag = (employeeContacts[i].contact_id + employeeContacts[i].address_serial + employeeID).ToString();
                                subItem.Items.Add(ChildItem);
                            }
                        }

                    }

                }

            }
        }

        private void cityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            districtCheckBox.IsEnabled = true;
            districtCheckBox.IsChecked = false;
            districtComboBox.IsEnabled = false;
            districtComboBox.Items.Clear();
            int cityIndex = 0;
            for(int i = 0; i < cities.Count; i++)
            {
                if(cities[i].city_name == cityComboBox.SelectedItem)
                {
                    cityIndex = cities[i].city_id;
                }
            }
            for (int i = 0; i < districts.Count; i++)
            {
                if (districts[i].district_id / 100 == cityIndex)
                    districtComboBox.Items.Add(districts[i].district_name);
            }
            treeViewItem.Items.Clear();

            TreeViewItem ParentItem = new TreeViewItem();
            ParentItem.Tag = employeeID.ToString();
            ParentItem.Header = loggedInUser.GetEmployeeName();
            treeViewItem.Items.Add(ParentItem);
            for (int i = 0; i < companys.Count(); i++)
            {
                if (companys[i].address / 100 == cityIndex)
                {
                    TreeViewItem ChildItem = new TreeViewItem();
                    ChildItem.Header = companys[i].company_name;
                    ChildItem.Tag = companys[i].company_serial.ToString();
                    ParentItem.Items.Add(ChildItem);

                }
            }
            foreach (TreeViewItem item in treeViewItem.Items)
            {
                foreach (TreeViewItem subItem in item.Items)
                {
                    for (int i = 0; i < employeeContacts.Count; i++)
                    {
                        if (subItem.Tag.ToString() == employeeContacts[i].address_serial.ToString())
                        {
                            if (employeeContacts[i].address / 100 == cityIndex)
                            {
                                TreeViewItem ChildItem = new TreeViewItem();
                                ChildItem.Header = employeeContacts[i].contact_name;
                                ChildItem.Tag = (employeeContacts[i].contact_id + employeeContacts[i].address_serial + employeeID).ToString();
                                subItem.Items.Add(ChildItem);
                            }
                        }

                    }

                }

            }
        }

        private void districtComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int districtIndex = 0;
            for (int i = 0; i < districts.Count; i++)
            {
                if (districts[i].district_name == districtComboBox.SelectedItem)
                {
                    districtIndex = districts[i].district_id;
                }
            }
            treeViewItem.Items.Clear();

            TreeViewItem ParentItem = new TreeViewItem();
            ParentItem.Tag = employeeID.ToString();
            ParentItem.Header = loggedInUser.GetEmployeeName();
            treeViewItem.Items.Add(ParentItem);
            for (int i = 0; i < companys.Count(); i++)
            {
                if (companys[i].address == districtIndex)
                {
                    TreeViewItem ChildItem = new TreeViewItem();
                    ChildItem.Header = companys[i].company_name;
                    ChildItem.Tag = companys[i].company_serial.ToString();
                    ParentItem.Items.Add(ChildItem);

                }
            }
            foreach (TreeViewItem item in treeViewItem.Items)
            {
                foreach (TreeViewItem subItem in item.Items)
                {
                    for (int i = 0; i < employeeContacts.Count; i++)
                    {
                        if (subItem.Tag.ToString() == employeeContacts[i].address_serial.ToString())
                        {
                            if (employeeContacts[i].address == districtIndex)
                            {
                                TreeViewItem ChildItem = new TreeViewItem();
                                ChildItem.Header = employeeContacts[i].contact_name;
                                ChildItem.Tag = (employeeContacts[i].contact_id + employeeContacts[i].address_serial + employeeID).ToString();
                                subItem.Items.Add(ChildItem);
                            }
                        }

                    }

                }

            }
        }

        private void countryCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            countryComboBox.IsEnabled = false;
            stateComboBox.IsEnabled = false;
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;

            countryComboBox.Items.Clear();
            stateComboBox.Items.Clear();
            cityComboBox.Items.Clear();
            districtComboBox.Items.Clear();

            stateCheckBox.IsEnabled = false;
            cityCheckBox.IsEnabled = false;
            districtCheckBox.IsEnabled = false;
            salesPersonCheckBox.IsEnabled = false;

            stateCheckBox.IsChecked = false;
            cityCheckBox.IsChecked = false;
            districtCheckBox.IsChecked = false;

        }

        private void stateCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            stateComboBox.IsEnabled = false;
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;

            stateComboBox.Items.Clear();
            cityComboBox.Items.Clear();
            districtComboBox.Items.Clear();

            cityCheckBox.IsEnabled = false;
            districtCheckBox.IsEnabled = false;
            salesPersonCheckBox.IsEnabled = false;

            cityCheckBox.IsChecked = false;
            districtCheckBox.IsChecked = false;
        }

        private void cityCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;

            cityComboBox.Items.Clear();
            districtComboBox.Items.Clear();

            districtCheckBox.IsEnabled = false;
            salesPersonCheckBox.IsEnabled = false;

            cityCheckBox.IsChecked = false;
            districtCheckBox.IsChecked = false;
        }

        private void districtCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            districtComboBox.IsEnabled = false;
            districtComboBox.Items.Clear();
            salesPersonCheckBox.IsEnabled = false;
        }
    }

}
