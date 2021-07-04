﻿using _01electronics_erp;
using System;
using System.Collections.Generic;
using System.IO;
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

        CommonQueries commonQueries;
        private Employee loggedInUser;
        private int employeeID;
        public List<COMPANY_ORGANISATION_MACROS.LIST_CONTACT_STRUCT> employeeContacts;
        List<COMPANY_ORGANISATION_MACROS.LIST_COMPANY_STRUCT> companies;
        List<BASIC_STRUCTS.COUNTRY_STRUCT> countries;
        List<BASIC_STRUCTS.STATE_STRUCT> states;
        List<BASIC_STRUCTS.CITY_STRUCT> cities;
        List<BASIC_STRUCTS.DISTRICT_STRUCT> districts;
        TreeViewItem[] companiesTreeArray = new TreeViewItem[COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_COMPANIES];
        TreeViewItem[] contactsTreeArray = new TreeViewItem[COMPANY_ORGANISATION_MACROS.MAX_CONTACTS_PER_COMPANY];

        public ContactsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();
            initializationObject = new SQLServer();

            employeeContacts = new List<COMPANY_ORGANISATION_MACROS.LIST_CONTACT_STRUCT>();
            companies = new List<COMPANY_ORGANISATION_MACROS.LIST_COMPANY_STRUCT>();
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

            ViewBtn.IsEnabled = false;

            QueryToGetAllCompanies();
            QueryToGetAllContacts();

            commonQueries.GetAllCountries(ref countries);

            for (int i = 0; i < countries.Count; i++)
            {
                countryComboBox.Items.Add(countries[i].country_name);
            }

            InitializeCompaniesTree();

            stateCheckBox.IsEnabled = false;
            cityCheckBox.IsEnabled = false;
            districtCheckBox.IsEnabled = false;
            salesPersonCheckBox.IsEnabled = false;

        }

        public bool QueryToGetAllCompanies()
        {
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
                companies.Add(tmpCompanyStruct);
            }
            return true;
        }

        public bool QueryToGetAllContacts()
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

            return true;
        }
        public bool InitializeCompaniesTree()
        {
            treeViewItem.Items.Clear();
            
            TreeViewItem ParentItem = new TreeViewItem();
            ParentItem.Header = loggedInUser.GetEmployeeName();
            ParentItem.Tag = (string)employeeID.ToString();
            treeViewItem.Items.Add(ParentItem);

            for (int i = 0; i < companies.Count(); i++)
            {
                if (countryCheckBox.IsChecked == true && countries[countryComboBox.SelectedIndex].country_id != companies[i].address / 1000000)
                    continue;
                if (stateCheckBox.IsChecked == true && states[stateComboBox.SelectedIndex].state_id != companies[i].address / 10000)
                    continue;
                if (cityCheckBox.IsChecked == true && cities[cityComboBox.SelectedIndex].city_id != companies[i].address / 100)
                    continue;
                if (districtCheckBox.IsChecked == true && districts[districtComboBox.SelectedIndex].district_id != companies[i].address)
                    continue;
                TreeViewItem ChildItem = new TreeViewItem();
                ChildItem.Header = companies[i].company_name;
                ChildItem.Tag = companies[i].address_serial.ToString();
                companiesTreeArray[companies[i].address_serial] = ChildItem;
               ParentItem.Items.Add(ChildItem);
            }
            InitializeContactsTree();
            return true;
        }

        public bool InitializeContactsTree()
        {

            for (int i = 0; i < employeeContacts.Count(); i++)
            {
                if (companiesTreeArray[employeeContacts[i].address_serial] != null)
                {
                    TreeViewItem ChildItem = new TreeViewItem();
                    ChildItem.Header = employeeContacts[i].contact_name;
                    ChildItem.Tag = (employeeContacts[i].contact_id + employeeContacts[i].address_serial + employeeID).ToString();
                    companiesTreeArray[employeeContacts[i].address_serial].Items.Add(ChildItem);
                }

            }

            return true;
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
        //    AddCompanyBtn.IsEnabled = false;
        //    AddCompanyBtn.Visibility = Visibility.Hidden;
        //    AddContactBtn.IsEnabled = false;
        //    AddContactBtn.Visibility = Visibility.Hidden;

            cityCheckBox.IsEnabled = false;
            districtCheckBox.IsEnabled = false;
            salesPersonCheckBox.IsEnabled = false;
            ViewBtn.IsEnabled = false;

            stateCheckBox.IsEnabled = true;

            stateCheckBox.IsChecked = false;
            cityCheckBox.IsChecked = false;
            districtCheckBox.IsChecked = false;

            if (countryComboBox.SelectedIndex != null)
                commonQueries.GetAllCountryStates(countries[countryComboBox.SelectedIndex].country_id, ref states);
            stateComboBox.Items.Clear();
            for (int i = 0; i < states.Count(); i++)
            {
                if (countryComboBox.SelectedIndex != null && states[i].state_id / 100 == countries[countryComboBox.SelectedIndex].country_id)
                    stateComboBox.Items.Add(states[i].state_name);
            }
            InitializeCompaniesTree();
           
        }

        private void stateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cityCheckBox.IsEnabled = true;
            cityCheckBox.IsChecked = false;
            districtCheckBox.IsChecked = false;
            districtCheckBox.IsEnabled = false;
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;
            ViewBtn.IsEnabled = false;

            if (stateComboBox.SelectedIndex != null)
                commonQueries.GetAllStateCities(states[stateComboBox.SelectedIndex].state_id, ref cities);
            cityComboBox.Items.Clear();

            for (int i = 0; i < cities.Count; i++)
            {
                if (stateComboBox.SelectedIndex != null && cities[i].city_id / 100 == states[stateComboBox.SelectedIndex].state_id)
                    cityComboBox.Items.Add(cities[i].city_name);
            }
            InitializeCompaniesTree();

        }

        private void cityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            districtCheckBox.IsEnabled = true;
            districtCheckBox.IsChecked = false;
            districtComboBox.IsEnabled = false;
            ViewBtn.IsEnabled = false;

            if (cityComboBox.SelectedItem != null)
              commonQueries.GetAllCityDistricts(cities[cityComboBox.SelectedIndex].city_id, ref districts);
            districtComboBox.Items.Clear();
          
            for (int i = 0; i < districts.Count; i++)
            {
                if (cityComboBox.SelectedItem != null && districts[i].district_id / 100 == cities[cityComboBox.SelectedIndex].city_id)
                    districtComboBox.Items.Add(districts[i].district_name);
            }
            InitializeCompaniesTree();
            
        }

        private void districtComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewBtn.IsEnabled = false;
            InitializeCompaniesTree();
        }

        private void countryCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            countryComboBox.IsEnabled = false;
            stateComboBox.IsEnabled = false;
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;
            ViewBtn.IsEnabled = false;

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
            ViewBtn.IsEnabled = false;

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
            ViewBtn.IsEnabled = false;

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
            ViewBtn.IsEnabled = false;
            districtComboBox.Items.Clear();
            salesPersonCheckBox.IsEnabled = false;
        }

        private void OnBtnClickedAddCompany(object sender, RoutedEventArgs e)
        {
            ViewBtn.IsEnabled = false;
            TreeViewItem selectedItem = (TreeViewItem)treeViewItem.SelectedItem;
            AddCompanyWindow addCompanyWindow = new AddCompanyWindow(ref loggedInUser);
            addCompanyWindow.Show();
        }

        private void OnBtnClickedAddContact(object sender, RoutedEventArgs e)
        {
            ViewBtn.IsEnabled = false;
            AddContactWindow addContactWindow = new AddContactWindow(ref loggedInUser);
            addContactWindow.Show();
        }

        private void treeViewItem_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ViewBtn.IsEnabled = true;

        }

        private void OnBtnClickedView(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = (TreeViewItem)treeViewItem.SelectedItem;
            bool check = false;
            bool checkContact = false;
            for (int i = 0; i < companiesTreeArray.Length; i++)
            {
                if (companiesTreeArray[i] == selectedItem)
                {
                    ViewCompanyWindow viewCompanyWindow = new ViewCompanyWindow();
                    viewCompanyWindow.Show();
                    check = true;
                }

            }
            if (check == false)
            {
                ViewContactWindow viewContactWindow = new ViewContactWindow();
                viewContactWindow.Show();
                checkContact = true;
            }
                //    for (int i = 0; i < companies.Count; i++)
                //    {
                //        if (companiesTreeArray[companies[i].address_serial] == selectedItem)
                //        {
                //            ViewContactWindow viewContactWindow = new ViewContactWindow();
                //            viewContactWindow.Show();
                //            checkContact = true;
                //        }
                //    }
                //    if (checkContact == false)
                //    {
                //        ViewBtn.IsEnabled = false;
                //    }

                //}
            }
        }

}
