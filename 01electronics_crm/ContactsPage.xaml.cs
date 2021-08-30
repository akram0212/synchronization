using _01electronics_library;
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
        private int selectedEmployee;

        private List<KeyValuePair<int, List<COMPANY_ORGANISATION_MACROS.COMPANY_LIST_STRUCT>>> employeesCompanies;

        private List<KeyValuePair<int, List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT>>> employeesContacts;
        private List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT> contactsList;

        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> listOfEmployees;

        private List<BASIC_STRUCTS.COUNTRY_STRUCT> countries;
        private List<BASIC_STRUCTS.STATE_STRUCT> states;
        private List<BASIC_STRUCTS.CITY_STRUCT> cities;
        private List<BASIC_STRUCTS.DISTRICT_STRUCT> districts;

        private List<KeyValuePair<int, TreeViewItem>> salesTreeArray = new List<KeyValuePair<int, TreeViewItem>>();
        private List<KeyValuePair<int, TreeViewItem>> companiesTreeArray = new List<KeyValuePair<int, TreeViewItem>>();

        //private TreeViewItem[] companiesTreeArray = new TreeViewItem[COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_COMPANIES];

        private List<KeyValuePair<TreeViewItem, COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT>> contactsTreeArray = new List<KeyValuePair<TreeViewItem, COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT>>();

        public ContactsPage(ref Employee mLoggedInUser)
        {
            initializationObject = new SQLServer();

            employeesCompanies = new List<KeyValuePair<int, List<COMPANY_ORGANISATION_MACROS.COMPANY_LIST_STRUCT>>>();
            employeesContacts = new List<KeyValuePair<int, List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT>>>();
            listOfEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();

            contactsList = new List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT>();

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

            stateCheckBox.IsEnabled = false;
            cityCheckBox.IsEnabled = false;
            districtCheckBox.IsEnabled = false;
            salesPersonCheckBox.IsEnabled = true;

            ViewBtn.IsEnabled = false;

            InitializeCountriesComboBox();

            if (!InitializeEmployeeComboBox())
                return;

            GetAllContacts();

            SetDefaultSettings();

            InitializeSalesTree();

        }
        private void SetDefaultSettings()
        {
            if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION || loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION)
            {
                salesPersonCheckBox.IsChecked = false;
                salesPersonCheckBox.IsEnabled = true;
                salesPersonComboBox.IsEnabled = false;
            }
            else
            {
                salesPersonCheckBox.IsChecked = true;
                salesPersonCheckBox.IsEnabled = false;
                salesPersonComboBox.IsEnabled = false;
            }
        }
        private bool InitializeListOfEmployees()
        {
            for (int i = 0; i < listOfEmployees.Count; i++)
            {
                salesPersonComboBox.Items.Add(listOfEmployees[i].employee_name);

                List<COMPANY_ORGANISATION_MACROS.COMPANY_LIST_STRUCT> tmpList = new List<COMPANY_ORGANISATION_MACROS.COMPANY_LIST_STRUCT>();


                commonQueries.GetEmployeeCompanies(listOfEmployees[i].employee_id, ref tmpList);
                employeesCompanies.Add(new KeyValuePair<int, List<COMPANY_ORGANISATION_MACROS.COMPANY_LIST_STRUCT>>(listOfEmployees[i].employee_id, tmpList));
            }

            return true;
        }

        private bool InitializeEmployeeComboBox()
        {
            listOfEmployees.Clear();

            if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)
            {
                if (!commonQueries.GetDepartmentEmployees(loggedInUser.GetEmployeeDepartmentId(), ref listOfEmployees))
                    return false;
            }
            else
            {
                if (!commonQueries.GetTeamEmployees(loggedInUser.GetEmployeeTeamId(), ref listOfEmployees))
                    return false;
            }

            InitializeListOfEmployees();

            return true;
        }
        
        public bool GetAllContacts()
        {
            employeesContacts.Clear();

            for (int i = 0; i < listOfEmployees.Count; i++)
            {
                List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT> tmpList = new List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT>();
                commonQueries.GetEmployeeContacts(listOfEmployees[i].employee_id, ref tmpList);

                 employeesContacts.Add(new KeyValuePair<int, List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT>>(listOfEmployees[i].employee_id, tmpList));
                
            }

            return true;
        }
        private void SetEmployeeComboBox()
        {
            salesPersonComboBox.SelectedIndex = 0;

            for (int i = 0; i < listOfEmployees.Count; i++)
                if (loggedInUser.GetEmployeeId() == listOfEmployees[i].employee_id)
                    salesPersonComboBox.SelectedIndex = i;
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
            if (countryComboBox.SelectedItem != null)
            {
                if (!commonQueries.GetAllCountryStates(countries[countryComboBox.SelectedIndex].country_id, ref states))
                    return false;

                for (int i = 0; i < states.Count(); i++)
                    stateComboBox.Items.Add(states[i].state_name);
            }

            return true;
        }
        public bool InitializeCitiesComboBox()
        {
            cityComboBox.Items.Clear();
            if (stateComboBox.SelectedItem != null)
            {
                if (!commonQueries.GetAllStateCities(states[stateComboBox.SelectedIndex].state_id, ref cities))
                    return false;

                for (int i = 0; i < cities.Count; i++)
                    cityComboBox.Items.Add(cities[i].city_name);
            }

            return true;
        }
        public bool InitializeDistrictsComboBox()
        {
            districtComboBox.Items.Clear();
            if (cityComboBox.SelectedItem != null)
            {
                if (!commonQueries.GetAllCityDistricts(cities[cityComboBox.SelectedIndex].city_id, ref districts))
                    return false;

                for (int i = 0; i < districts.Count; i++)
                    districtComboBox.Items.Add(districts[i].district_name);
            }

            return true;
        }

        public bool InitializeSalesTree()
        {
            contactTreeView.Items.Clear();

            salesTreeArray.Clear();

            for (int j = 0; j < listOfEmployees.Count(); j++)
            {
                if (salesPersonComboBox.SelectedItem != null && listOfEmployees[salesPersonComboBox.SelectedIndex].employee_id != listOfEmployees[j].employee_id)
                    continue;

                TreeViewItem ParentItem = new TreeViewItem();

                ParentItem.Header = listOfEmployees[j].employee_name;
                ParentItem.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                ParentItem.FontSize = 14;
                ParentItem.FontWeight = FontWeights.SemiBold;
                ParentItem.FontFamily = new FontFamily("Sans Serif");
                ParentItem.Tag = listOfEmployees[j].employee_id;

                contactTreeView.Items.Add(ParentItem);

                salesTreeArray.Add(new KeyValuePair<int, TreeViewItem>(listOfEmployees[j].employee_id, ParentItem));

            }

            InitializeCompaniesTree();
            InitializeContactsTree();

            return true;
        }
        public bool InitializeCompaniesTree()
        {
            companiesTreeArray.Clear();

            for (int i = 0; i < employeesCompanies.Count(); i++)
            {
                // List<COMPANY_ORGANISATION_MACROS.COMPANY_LIST_STRUCT> tmpList = employeesCompanies[loggedInUser.GetEmployeeId()];


                // if (countryCheckBox.IsChecked == true && countryComboBox.SelectedItem != null && countries[countryComboBox.SelectedIndex].country_id != employeesCompanies[i].address / (BASIC_MACROS.MAXIMUM_DISTRICTS_NO * BASIC_MACROS.MAXIMUM_STATES_NO * BASIC_MACROS.MAXIMUM_CITIES_NO))
                // continue;
                //if (stateCheckBox.IsChecked == true && stateComboBox.SelectedItem != null && states[stateComboBox.SelectedIndex].state_id != tmpList[i].branchesList[0].address / (BASIC_MACROS.MAXIMUM_DISTRICTS_NO * BASIC_MACROS.MAXIMUM_CITIES_NO))
                // continue;
                // if (cityCheckBox.IsChecked == true && cityComboBox.SelectedItem != null && cities[cityComboBox.SelectedIndex].city_id != tmpList[i].branchesList[0].address / BASIC_MACROS.MAXIMUM_DISTRICTS_NO)
                // continue;
                //if (districtCheckBox.IsChecked == true && districtComboBox.SelectedItem != null && districts[districtComboBox.SelectedIndex].district_id != tmpList[i].branchesList[0].address)
                //  continue;
                if (salesPersonCheckBox.IsChecked == true && salesPersonComboBox.SelectedItem != null && employeesCompanies[i].Key != listOfEmployees[salesPersonComboBox.SelectedIndex].employee_id)
                    continue;

                TreeViewItem currentEmployeeTreeItem = salesTreeArray.Find(tree_item => tree_item.Key == employeesCompanies[i].Key).Value;

                for (int j = 0; j < employeesCompanies[i].Value.Count; j++)
                {
                    TreeViewItem ChildItem = new TreeViewItem();
                    ChildItem.Header = employeesCompanies[i].Value[j].company_name;
                    ChildItem.Tag = employeesCompanies[i].Value[j].company_serial;
                    ChildItem.FontSize = 13;
                    ChildItem.FontWeight = FontWeights.SemiBold;

                    companiesTreeArray.Add(new KeyValuePair<int, TreeViewItem>(employeesCompanies[i].Value[j].company_serial, ChildItem));

                    currentEmployeeTreeItem.Items.Add(ChildItem);
                }

                //if (!ParentItem.HasItems)
                //    contactTreeView.Items.Remove(ParentItem);
            }


            return true;
        }

        public bool InitializeContactsTree()
        {
            contactsTreeArray.Clear();

            for (int i = 0; i < employeesContacts.Count(); i++)
            {
                if(salesTreeArray.Exists(sales_item => sales_item.Key == employeesContacts[i].Key))
                {
                    for (int j = 0; j < employeesContacts[i].Value.Count; j++)
                    {
                        TreeViewItem companyTreeItem = new TreeViewItem();

                        if (!companiesTreeArray.Exists(company_item => company_item.Key == employeesContacts[i].Value[j].company_serial))
                            continue;

                        companyTreeItem = companiesTreeArray.Find(company_item => company_item.Key == employeesContacts[i].Value[j].company_serial).Value;
                        
                        TreeViewItem contactTreeItem = new TreeViewItem();

                        contactTreeItem.Header = employeesContacts[i].Value[j].contact.contact_name;
                        contactTreeItem.Tag = employeesContacts[i].Value[j].contact.contact_id;
                        contactTreeItem.FontSize = 12;
                        contactTreeItem.FontWeight = FontWeights.Normal;

                        companyTreeItem.Items.Add(contactTreeItem);

                        contactsTreeArray.Add(new KeyValuePair<TreeViewItem, COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT>(contactTreeItem, employeesContacts[i].Value[j]));


                    }
                }
                

            }

            return true;
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
            salesPersonComboBox.IsEnabled = true;

            SetEmployeeComboBox();
        }


        //////////////////////////////////////////////////////////
        /// ON SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////
        ///
        private void OnSelChangedCountryComboBox(object sender, SelectionChangedEventArgs e)
        {
            cityCheckBox.IsEnabled = false;
            districtCheckBox.IsEnabled = false;
            //salesPersonCheckBox.IsEnabled = false;
            ViewBtn.IsEnabled = false;

            stateCheckBox.IsEnabled = true;

            stateCheckBox.IsChecked = false;
            cityCheckBox.IsChecked = false;
            districtCheckBox.IsChecked = false;

            if (!InitializeStatesComboBox())
                return;

            InitializeSalesTree();
        }

        private void OnSelChangedStateComboBox(object sender, SelectionChangedEventArgs e)
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

            InitializeSalesTree();
        }

        private void OnSelChangedCityComboBox(object sender, SelectionChangedEventArgs e)
        {
            districtCheckBox.IsEnabled = true;
            districtCheckBox.IsChecked = false;
            districtComboBox.IsEnabled = false;
            ViewBtn.IsEnabled = false;

            if (!InitializeDistrictsComboBox())
                return;

            InitializeSalesTree();
        }

        private void OnSelChangedDistrictComboBox(object sender, SelectionChangedEventArgs e)
        {
            ViewBtn.IsEnabled = false;

            InitializeSalesTree();
        }
        private void OnSelChangedSalesPersonComboBox(object sender, SelectionChangedEventArgs e)
        {
            ViewBtn.IsEnabled = false;

            if (salesPersonCheckBox.IsChecked == true)
                selectedEmployee = listOfEmployees[salesPersonComboBox.SelectedIndex].employee_id;
            else
                selectedEmployee = 0;

            InitializeSalesTree();

        }

        //////////////////////////////////////////////////////////
        /// ON UNCHECKED HANDLERS
        //////////////////////////////////////////////////////////
        private void OnUncheckedCountryCheckBox(object sender, RoutedEventArgs e)
        {
            countryComboBox.IsEnabled = false;
            stateComboBox.IsEnabled = false;

            cityComboBox.IsEnabled = false;
            stateComboBox.IsEnabled = false;

            cityComboBox.SelectedItem = null;
            districtComboBox.SelectedItem = null;
            stateComboBox.SelectedItem = null;
            countryComboBox.SelectedItem = null;

            ViewBtn.IsEnabled = false;

            stateCheckBox.IsEnabled = false;
            cityCheckBox.IsEnabled = false;
            districtCheckBox.IsEnabled = false;
            // salesPersonCheckBox.IsEnabled = false;

            stateCheckBox.IsChecked = false;
            cityCheckBox.IsChecked = false;
            districtCheckBox.IsChecked = false;
            InitializeSalesTree();

        }

        private void OnUncheckedStateCheckBox(object sender, RoutedEventArgs e)
        {
            stateComboBox.IsEnabled = false;
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;
            ViewBtn.IsEnabled = false;

            cityComboBox.SelectedItem = null;
            districtComboBox.SelectedItem = null;
            stateComboBox.SelectedItem = null;

            cityCheckBox.IsEnabled = false;
            districtCheckBox.IsEnabled = false;
            // salesPersonCheckBox.IsEnabled = false;

            cityCheckBox.IsChecked = false;
            districtCheckBox.IsChecked = false;
            InitializeSalesTree();
        }

        private void OnUncheckedCityCheckBox(object sender, RoutedEventArgs e)
        {
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;
            ViewBtn.IsEnabled = false;

            districtCheckBox.IsEnabled = false;
            //salesPersonCheckBox.IsEnabled = false;

            cityComboBox.SelectedItem = null;
            districtComboBox.SelectedItem = null;

            cityCheckBox.IsChecked = false;
            districtCheckBox.IsChecked = false;
            InitializeSalesTree();
        }

        private void OnUncheckedDistrictCheckBox(object sender, RoutedEventArgs e)
        {
            districtComboBox.SelectedItem = null;

            districtComboBox.IsEnabled = false;
            ViewBtn.IsEnabled = false;
            //salesPersonCheckBox.IsEnabled = false;
            InitializeSalesTree();
        }
        private void OnUncheckedSalesPersonCheckBox(object sender, RoutedEventArgs e)
        {
            salesPersonComboBox.IsEnabled = false;
            salesPersonComboBox.SelectedItem = null;
            InitializeSalesTree();
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
            listOfEmployees.Clear();
            employeesCompanies.Clear();

            InitializeCountriesComboBox();

            if (!InitializeEmployeeComboBox())
                return;

            GetAllContacts();

            SetDefaultSettings();

            InitializeSalesTree();
        }
        private void OnClosedAddContactWindow(object sender, EventArgs e)
        {
            listOfEmployees.Clear();
            employeesCompanies.Clear();

            InitializeCountriesComboBox();

            if (!InitializeEmployeeComboBox())
                return;

            GetAllContacts();

            SetDefaultSettings();

            InitializeSalesTree();
        }
        private void OnSelectedItemChangedTreeViewItem(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //VIEW BUTTON IS ENABLED ONCE ANY ITEM IS SELECTED
            ViewBtn.IsEnabled = false;
            TreeViewItem selectedItem = (TreeViewItem)contactTreeView.SelectedItem;
            if (selectedItem != null)
            {
                try
                {
                    object parent = selectedItem.Parent;
                    TreeViewItem currentCompany = (TreeViewItem)parent;
                    ViewBtn.IsEnabled = true;

                    try
                    {
                        object parent2 = currentCompany.Parent;
                        TreeViewItem currentSales = (TreeViewItem)parent2;
                        ViewBtn.IsEnabled = true;

                    }
                    catch
                    {
                    }

                }
                catch
                {
                }
            }

        }

        private void OnBtnClickedView(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = (TreeViewItem)contactTreeView.SelectedItem;

            if (!selectedItem.HasItems)
            {
                COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT currentContactStruct = new COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT();
                currentContactStruct = contactsTreeArray.Find(current_item => current_item.Key.Tag == selectedItem.Tag).Value;

                Contact selectedContact = new Contact();
                object parent = selectedItem.Parent;
                TreeViewItem currentCompany = (TreeViewItem)parent;

                try
                {
                    object parent2 = currentCompany.Parent;
                    TreeViewItem currentSales = (TreeViewItem)parent2;

                    selectedContact.InitializeContactInfo(Convert.ToInt32
                        (currentSales.Tag), currentContactStruct.address_serial, currentContactStruct.contact.contact_id);

                    ViewContactWindow viewContactWindow = new ViewContactWindow(ref loggedInUser, ref selectedContact);
                    viewContactWindow.Show();
                }
                catch
                {
                    Company currentCompanyClass = new Company();
                    currentCompanyClass.InitializeCompanyInfo(Convert.ToInt32(selectedItem.Tag));

                    try
                    {
                        object parent3 = selectedItem.Parent;
                        TreeViewItem currentSales = (TreeViewItem)parent3;
                        ViewCompanyWindow viewCompanyWindow = new ViewCompanyWindow(ref loggedInUser, ref currentCompanyClass);
                        viewCompanyWindow.Show();
                    }
                    catch
                    {
                    }
                }

            }
            else
            {
                Company currentCompany = new Company();
                currentCompany.InitializeCompanyInfo(Convert.ToInt32(selectedItem.Tag));

                try
                {
                    object parent = selectedItem.Parent;
                    TreeViewItem currentSales = (TreeViewItem)parent;
                    ViewCompanyWindow viewCompanyWindow = new ViewCompanyWindow(ref loggedInUser, ref currentCompany);
                    viewCompanyWindow.Show();
                }
                catch
                {
                }

            }

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

        /////////////////////////////////////////////////////////////////
        //BTN CLICKED HANDLERS
        /////////////////////////////////////////////////////////////////
    }


}
