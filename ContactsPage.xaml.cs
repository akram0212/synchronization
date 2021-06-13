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
        List<COMPANY_ORGANISATION_MACROS.CONTACT_AMATEUR_STRUCT> employeeContacts;
        List<COMPANY_ORGANISATION_MACROS.COMPANY_STRUCT> companys;
        List<KeyValuePair<long, string>> countries;
        List<KeyValuePair<long, string>> states;
        List<KeyValuePair<long, string>> cities;
        List<KeyValuePair<long, string>> districts;


        public ContactsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();
            initializationObject = new SQLServer();

            employeeContacts = new List<COMPANY_ORGANISATION_MACROS.CONTACT_AMATEUR_STRUCT>();
            companys = new List<COMPANY_ORGANISATION_MACROS.COMPANY_STRUCT>();
            countries = new List<KeyValuePair<long, string>>();
            states = new List<KeyValuePair<long, string>>();
            cities = new List<KeyValuePair<long, string>>();
            districts = new List<KeyValuePair<long, string>>();

            commonQueries = new CommonQueries();
            loggedInUser = mLoggedInUser;
            employeeID = mLoggedInUser.GetEmployeeId();

            countryComboBox.IsEnabled = false; 
            stateComboBox.IsEnabled = false;
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;
            salesPersonComboBox.IsEnabled = false;

            stateCheckBox.IsEnabled = false;
            cityCheckBox.IsEnabled = false;
            districtCheckBox.IsEnabled = false;
            salesPersonCheckBox.IsEnabled = false;
            getEmployeeCompanys(employeeID);

        }

      
        public bool getEmployeeCompanys(int employeeID)
        {

            string sqlQueryPart1 = "select company_serial,company_name from erp_system.dbo.company_name where added_by = ";

            String sqlQueryPart2 = " order by company_name;";

            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += employeeID;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!initializationObject.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            for(int i = 0; i < initializationObject.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.COMPANY_STRUCT tmpCompanyStruct;
                tmpCompanyStruct.company_serial = initializationObject.rows[i].sql_int[0];
                tmpCompanyStruct.company_name = initializationObject.rows[i].sql_string[0];
                companys.Add(tmpCompanyStruct);
            }

            TreeViewItem ParentItem = new TreeViewItem();
            ParentItem.Header = loggedInUser.GetEmployeeName();
            ParentItem.Tag = "1";
            treeViewItem.Items.Add(ParentItem);

            string QueryPart1 = "select branch_serial,contact_id, name from erp_system.dbo.contact_person_info where sales_person_id = ";

            String QueryPart2 = " order by company_name;";

            sqlQuery = string.Empty;
            sqlQuery += QueryPart1;
            sqlQuery += employeeID;
            sqlQuery += QueryPart2;
            for (int i = 0; i < companys.Count; i++)
            {
                TreeViewItem ChildItem = new TreeViewItem();
                ChildItem.Header = companys[i].company_name;
                ChildItem.Tag = companys[i].company_serial;
                ParentItem.Items.Add(ChildItem);
                //contacts = companys[i].Value.GetCompanyContacts();

                //for (int j = 0; j < contacts.Count; j++)
                //{
                //    TreeViewItem Child2Item = new TreeViewItem();
                //    Child2Item.Header = contacts[j].contact_name;
                //    ChildItem.Items.Add(Child2Item);
                //}

                //if(contacts.Count == 0)
                //{
                //    TreeViewItem Child2Item = new TreeViewItem();
                //    Child2Item.Header = "NONE";
                //    ChildItem.Items.Add(Child2Item);
                //}
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

            //countries.Add(new KeyValuePair<long, string>(companys[0].Value.GetCompanyCountryId(), companys[0].Value.GetCompanyCountry()));
            //countryComboBox.Items.Add(countries[0].Value);
            //for (int i = 1; i < companys.Count; i++)
            //{
            //    if (companys[i].Value.GetCompanyCountryId() != countries[0].Key && companys[i].Value.GetCompanyCountryId() != 0 && companys[i].Value.GetCompanyCountryId() != companys[i - 1].Value.GetCompanyCountryId())
            //    {
            //        countries.Add(new KeyValuePair<long, string>(companys[i].Value.GetCompanyCountryId(), companys[i].Value.GetCompanyCountry()));
            //        countryComboBox.Items.Add(companys[i].Value.GetCompanyCountry());
            //    }

            //}

            
            
        }

        private void stateCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            stateComboBox.IsEnabled = true;
            //states.Add(new KeyValuePair<long, string>(companys[0].Value.GetCompanyStateId(), companys[0].Value.GetCompanyState()));
            countryComboBox.Items.Add(countries[0].Value);
            //for (int i = 1; i < companys.Count; i++)
            //{
            //    if (companys[i].Value.GetCompanyStateId() != states[0].Key && companys[i].Value.GetCompanyStateId() != 0 && companys[i].Value.GetCompanyStateId() != companys[i - 1].Value.GetCompanyStateId())
            //    {
            //        states.Add(new KeyValuePair<long, string>(companys[i].Value.GetCompanyStateId(), companys[i].Value.GetCompanyState()));
            //        countryComboBox.Items.Add(companys[i].Value.GetCompanyState());
            //    }

            //}

            treeViewItem.Items.Clear();

            TreeViewItem ParentItem = new TreeViewItem();
            ParentItem.Header = loggedInUser.GetEmployeeName();
            treeViewItem.Items.Add(ParentItem);

            List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT> contacts = new List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT>();

            //for (int k = 0; k < states.Count; k++)
            //{
            //    for (int i = 0; i < companys.Count; i++)
            //    {
            //        if (companys[i].Value.GetCompanyStateId() == states[k].Key)
            //        {
            //            TreeViewItem ChildItem = new TreeViewItem();
            //            ChildItem.Header = companys[i].Value.GetCompanyName();
            //            ParentItem.Items.Add(ChildItem);
            //            contacts = companys[i].Value.getCompanyContacts();

            //            for (int j = 0; j < contacts.Count; j++)
            //            {
            //                TreeViewItem Child2Item = new TreeViewItem();
            //                Child2Item.Header = contacts[j].contact_name;
            //                ChildItem.Items.Add(Child2Item);
            //            }

            //            if (contacts.Count == 0)
            //            {
            //                TreeViewItem Child2Item = new TreeViewItem();
            //                Child2Item.Header = "NONE";
            //                ChildItem.Items.Add(Child2Item);
            //            }
            //        }
            //    }
            //}
            //stateComboBox.Items.Add("cairo");
        }

        private void cityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            cityComboBox.IsEnabled = true;
            cityComboBox.Items.Add("cairo");
        }

        private void districtCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            districtComboBox.IsEnabled = true;
            districtComboBox.Items.Add("new cairo");
        }

        private void salesPersonCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void countryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stateCheckBox.IsEnabled = true;
            string selectedItem = (string)countryComboBox.SelectedItem;

            treeViewItem.Items.Clear();

            TreeViewItem ParentItem = new TreeViewItem();
            ParentItem.Header = loggedInUser.GetEmployeeName();
            treeViewItem.Items.Add(ParentItem);
            //ParentItem.Tag = 
           // List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT> contacts = new List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT>();

            //for (int k = 0; k < countries.Count; k++)
            //{
            //    for (int i = 0; i < companys.Count; i++)
            //    {
            //        if (companys[i].Value.GetCompanyCountry() == selectedItem)
            //        {
            //            TreeViewItem ChildItem = new TreeViewItem();
            //            ChildItem.Header = companys[i].Value.GetCompanyName();
            //            ParentItem.Items.Add(ChildItem);
            //            contacts = companys[i].Value.getCompanyContacts();

            //            for (int j = 0; j < contacts.Count; j++)
            //            {
            //                TreeViewItem Child2Item = new TreeViewItem();
            //                Child2Item.Header = contacts[j].contact_name;
            //                ChildItem.Items.Add(Child2Item);
            //            }

            //            if (contacts.Count == 0)
            //            {
            //                TreeViewItem Child2Item = new TreeViewItem();
            //                Child2Item.Header = "NONE";
            //                ChildItem.Items.Add(Child2Item);
            //            }
            //        }
            //    }
            //}
        }

        private void stateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cityCheckBox.IsEnabled = true;
        }

        private void cityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            districtCheckBox.IsEnabled = true;
        }

        private void districtComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
