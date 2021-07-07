using _01electronics_erp;
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

        private List<COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT> contactsList;
        private List<COMPANY_ORGANISATION_MACROS.COMPANY_LIST_STRUCT> companiesList;
        private List<BASIC_STRUCTS.COUNTRY_STRUCT> countries;
        private List<BASIC_STRUCTS.STATE_STRUCT> states;
        private List<BASIC_STRUCTS.CITY_STRUCT> cities;
        private List<BASIC_STRUCTS.DISTRICT_STRUCT> districts;

        private TreeViewItem[] companiesTreeArray = new TreeViewItem[COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_COMPANIES];

        private List<KeyValuePair<TreeViewItem, COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT>> contactsTreeArray = new List<KeyValuePair<TreeViewItem, COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT>>();

        public ContactsPage(ref Employee mLoggedInUser)
        {
            initializationObject = new SQLServer();

            contactsList = new List<COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT>();
            companiesList = new List<COMPANY_ORGANISATION_MACROS.COMPANY_LIST_STRUCT>();
            countries = new List<BASIC_STRUCTS.COUNTRY_STRUCT>();
            states = new List<BASIC_STRUCTS.STATE_STRUCT>();
            cities = new List<BASIC_STRUCTS.CITY_STRUCT>();
            districts = new List<BASIC_STRUCTS.DISTRICT_STRUCT>();

            commonQueries = new CommonQueries();
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            countryComboBox.IsEnabled = false; 
            stateComboBox.IsEnabled = false;
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;
            salesPersonComboBox.IsEnabled = false;

            ViewBtn.IsEnabled = false;

            InitializeCountriesComboBox();

            GetAllCompanies();
            GetAllContacts();

            InitializeCompaniesTree();

            stateCheckBox.IsEnabled = false;
            cityCheckBox.IsEnabled = false;
            districtCheckBox.IsEnabled = false;
            salesPersonCheckBox.IsEnabled = false;

        }
        
        public bool GetAllCompanies()
        {
            if (!commonQueries.GetEmployeeCompanies(loggedInUser.GetEmployeeId(), ref companiesList))
                return false;

            return true;
        }
        public bool GetAllContacts()
        {
            if (!commonQueries.GetEmployeeContacts(loggedInUser.GetEmployeeId(), ref contactsList))
                return false;

            return true;
        }

        public bool InitializeCountriesComboBox()
        {
            countryComboBox.Items.Clear();

            if (!commonQueries.GetAllCountries(ref countries))
                return false;

            for (int i = 0; i < countries.Count; i++)
                countryComboBox.Items.Add(countries[i].country_name);

            return true;
        }
        public bool InitializeStatesComboBox()
        {
            stateComboBox.Items.Clear();

            if (!commonQueries.GetAllCountryStates(countries[countryComboBox.SelectedIndex].country_id, ref states))
                return false;

            for (int i = 0; i < states.Count(); i++)
                stateComboBox.Items.Add(states[i].state_name);

            return true;
        }
        public bool InitializeCitiesComboBox()
        {
            cityComboBox.Items.Clear();

            if (!commonQueries.GetAllStateCities(states[stateComboBox.SelectedIndex].state_id, ref cities))
                return false;

            for (int i = 0; i < countries.Count; i++)
                cityComboBox.Items.Add(cities[i].city_name);

            return true;
        }
        public bool InitializeDistrictsComboBox()
        {
            districtComboBox.Items.Clear();

            if (!commonQueries.GetAllCityDistricts(cities[cityComboBox.SelectedIndex].city_id, ref districts))
                return false;

            for (int i = 0; i < countries.Count; i++)
                districtComboBox.Items.Add(districts[i].district_name);

            return true;
        }

        public bool InitializeCompaniesTree()
        {
            contactTreeView.Items.Clear();
            
            TreeViewItem ParentItem = new TreeViewItem();
            
            ParentItem.Header = loggedInUser.GetEmployeeName();
            ParentItem.Tag = loggedInUser.GetEmployeeId();

            contactTreeView.Items.Add(ParentItem);

            for (int i = 0; i < companiesList.Count(); i++)
            {
                if (countryCheckBox.IsChecked == true && countries[countryComboBox.SelectedIndex].country_id != companiesList[i].branchesList[0].address / (BASIC_MACROS.MAXIMUM_DISTRICTS_NO * BASIC_MACROS.MAXIMUM_STATES_NO * BASIC_MACROS.MAXIMUM_CITIES_NO))
                    continue;
                if (stateCheckBox.IsChecked == true && states[stateComboBox.SelectedIndex].state_id != companiesList[i].branchesList[0].address / (BASIC_MACROS.MAXIMUM_DISTRICTS_NO * BASIC_MACROS.MAXIMUM_CITIES_NO))
                    continue;
                if (cityCheckBox.IsChecked == true && cities[cityComboBox.SelectedIndex].city_id != companiesList[i].branchesList[0].address / BASIC_MACROS.MAXIMUM_DISTRICTS_NO)
                    continue;
                if (districtCheckBox.IsChecked == true && districts[districtComboBox.SelectedIndex].district_id != companiesList[i].branchesList[0].address)
                    continue;

                TreeViewItem ChildItem = new TreeViewItem();
                ChildItem.Header = companiesList[i].company_name;
                ChildItem.Tag = companiesList[i].company_serial;

                companiesTreeArray[companiesList[i].company_serial] = ChildItem;

                ParentItem.Items.Add(ChildItem);
            }

            InitializeContactsTree();

            return true;
        }
        public bool InitializeContactsTree()
        {

            for (int i = 0; i < contactsList.Count(); i++)
            {
                TreeViewItem companyTreeItem = companiesTreeArray[contactsList[i].company_serial];

                if (companyTreeItem != null)
                {
                    TreeViewItem contactTreeItem = new TreeViewItem();

                    contactTreeItem.Header = contactsList[i].contact_name;

                    companyTreeItem.Items.Add(contactTreeItem);

                    contactsTreeArray.Add(new KeyValuePair<TreeViewItem, COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT>(contactTreeItem, contactsList[i]));
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
        }

        //////////////////////////////////////////////////////////
        /// ON CHECK HANDLERS
        //////////////////////////////////////////////////////////
        private void OnCheckedCountryCheckBox(object sender, RoutedEventArgs e)
        {
            countryComboBox.IsEnabled = true;
        }

        private void OnCheckedStateCheckBox(object sender, RoutedEventArgs e)
        {
            stateComboBox.IsEnabled = true;
        }

        private void OnCheckedCityCheckBox(object sender, RoutedEventArgs e)
        {
            cityComboBox.IsEnabled = true; 
        }

        private void OnCheckedDistrictCheckBox(object sender, RoutedEventArgs e)
        {
            districtComboBox.IsEnabled = true;
        }
        private void OnCheckedSalesPersonCheckBox(object sender, RoutedEventArgs e)
        {

        }

        //////////////////////////////////////////////////////////
        /// ON SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////
        ///
        private void OnSelectionChangedCountryComboBox(object sender, SelectionChangedEventArgs e)
        {
            cityCheckBox.IsEnabled = false;
            districtCheckBox.IsEnabled = false;
            salesPersonCheckBox.IsEnabled = false;
            ViewBtn.IsEnabled = false;

            stateCheckBox.IsEnabled = true;

            stateCheckBox.IsChecked = false;
            cityCheckBox.IsChecked = false;
            districtCheckBox.IsChecked = false;

            if (!InitializeStatesComboBox())
                return;

            InitializeCompaniesTree();
           
        }
        private void OnSelectionChangedStateComboBox(object sender, SelectionChangedEventArgs e)
        {
            cityCheckBox.IsEnabled = true;
            cityCheckBox.IsChecked = false;
            districtCheckBox.IsChecked = false;
            districtCheckBox.IsEnabled = false;
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;
            ViewBtn.IsEnabled = false;

            if (!InitializeCitiesComboBox())
                return;

            InitializeCompaniesTree();

        }
        private void OnSelectionChangedCityComboBox(object sender, SelectionChangedEventArgs e)
        {
            districtCheckBox.IsEnabled = true;
            districtCheckBox.IsChecked = false;
            districtComboBox.IsEnabled = false;
            ViewBtn.IsEnabled = false;

            if (!InitializeDistrictsComboBox())
                return;

            InitializeCompaniesTree();
            
        }
        private void OnSelectionChangedDistrictComboBox(object sender, SelectionChangedEventArgs e)
        {
            ViewBtn.IsEnabled = false;
            InitializeCompaniesTree();
        }

        //////////////////////////////////////////////////////////
        /// ON UNCHECKED HANDLERS
        //////////////////////////////////////////////////////////
        private void OnUncheckedCountryCheckBox(object sender, RoutedEventArgs e)
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
        private void OnUncheckedStateCheckBox(object sender, RoutedEventArgs e)
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
        private void OnUncheckedCityCheckBox(object sender, RoutedEventArgs e)
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
        private void OnUncheckedDistrictCheckBox(object sender, RoutedEventArgs e)
        {
            districtComboBox.IsEnabled = false;
            ViewBtn.IsEnabled = false;
            districtComboBox.Items.Clear();
            salesPersonCheckBox.IsEnabled = false;
        }   
        private void OnUncheckedSalesPersonCheckBox(object sender, RoutedEventArgs e)
        {
            
        }

        //////////////////////////////////////////////////////////
        /// ON BTN CLICKED HANDLERS
        //////////////////////////////////////////////////////////
        private void OnBtnClickedAddCompany(object sender, RoutedEventArgs e)
        {
            ViewBtn.IsEnabled = false;
            AddCompanyWindow addCompanyWindow = new AddCompanyWindow(ref loggedInUser);
            addCompanyWindow.Closed += OnClosedAddCompanyWindow;
            addCompanyWindow.Show();
        }
        private void OnBtnClickedAddContact(object sender, RoutedEventArgs e)
        {
            ViewBtn.IsEnabled = false;

            AddContactWindow addContactWindow = new AddContactWindow(ref loggedInUser);
            addContactWindow.Closed += OnClosedAddContactWindow;
            addContactWindow.Show();
        }

        private void OnClosedAddCompanyWindow(object sender, EventArgs e)
        {
            GetAllCompanies();
            GetAllContacts();

            InitializeCompaniesTree();
        }
        private void OnClosedAddContactWindow(object sender, EventArgs e)
        {
            GetAllCompanies();
            GetAllContacts();

            InitializeCompaniesTree();
        }
        private void OnSelectedItemChangedTreeViewItem(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //VIEW BUTTON IS ENABLED ONCE ANY ITEM IS SELECTED
            ViewBtn.IsEnabled = true;
        }

        private void OnBtnClickedView(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = (TreeViewItem)contactTreeView.SelectedItem;
            //TreeViewItem parentItem = (TreeViewItem)selectedItem.Parent;
            //DependencyObject RootItem = selectedItem.Parent;

            if (!selectedItem.HasItems)
            {
                COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT currentContactStruct;
                currentContactStruct = contactsTreeArray.Find(current_item => current_item.Key.Tag == selectedItem.Tag).Value;

                Contact selectedContact = new Contact();
                selectedContact.InitializeContactInfo(currentContactStruct.sales_person_id, currentContactStruct.address_serial, currentContactStruct.contact_id);

                ViewContactWindow viewContactWindow = new ViewContactWindow(ref loggedInUser, ref selectedContact);
                viewContactWindow.Show();
            }
            else 
            {
                Company currentCompany = new Company();
                currentCompany.InitializeCompanyInfo(Convert.ToInt32(selectedItem.Tag));

                ViewCompanyWindow viewCompanyWindow = new ViewCompanyWindow(ref loggedInUser, ref currentCompany);
                viewCompanyWindow.Show();
            }
           
        }
    }


}
