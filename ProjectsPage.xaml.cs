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
    /// Interaction logic for ProjectsPage.xaml
    /// </summary>
    public partial class ProjectsPage : Page
    {
        private String sqlQuery;

        private SQLServer initializationObject;

        CommonQueries commonQueries;
        private Employee loggedInUser;

        private List<BASIC_STRUCTS.COUNTRY_STRUCT> countries;
        private List<BASIC_STRUCTS.STATE_STRUCT> states;
        private List<BASIC_STRUCTS.CITY_STRUCT> cities;
        private List<BASIC_STRUCTS.DISTRICT_STRUCT> districts;

        private List<KeyValuePair<int, TreeViewItem>> salesTreeArray = new List<KeyValuePair<int, TreeViewItem>>();
        private List<KeyValuePair<int, TreeViewItem>> companiesTreeArray = new List<KeyValuePair<int, TreeViewItem>>();

        //private TreeViewItem[] companiesTreeArray = new TreeViewItem[COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_COMPANIES];

        private List<KeyValuePair<TreeViewItem, COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT>> contactsTreeArray = new List<KeyValuePair<TreeViewItem, COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT>>();

        public ProjectsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;
            countries = new List<BASIC_STRUCTS.COUNTRY_STRUCT>();
            states = new List<BASIC_STRUCTS.STATE_STRUCT>();
            cities = new List<BASIC_STRUCTS.CITY_STRUCT>();
            districts = new List<BASIC_STRUCTS.DISTRICT_STRUCT>();

            commonQueries = new CommonQueries();

            countryComboBox.IsEnabled = false;
            stateComboBox.IsEnabled = false;
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;

            stateCheckBox.IsEnabled = false;
            cityCheckBox.IsEnabled = false;
            districtCheckBox.IsEnabled = false;

            InitializeCountriesComboBox();

            //if (!CheckEmployeePosition())
            //    return;

            //if (!InitializeCompaniesList())
            //    return;

            //GetAllContacts();

            //SetDefaultSettings();

            //InitializeSalesTree();
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
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON CHECK HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void OnSelChangedCountryComboBox(object sender, SelectionChangedEventArgs e)
        {
            cityCheckBox.IsEnabled = false;
            districtCheckBox.IsEnabled = false;

            stateCheckBox.IsEnabled = true;

            stateCheckBox.IsChecked = false;
            cityCheckBox.IsChecked = false;
            districtCheckBox.IsChecked = false;

            if (!InitializeStatesComboBox())
                return;

            //InitializeSalesTree();
        }

        private void OnSelChangedStateComboBox(object sender, SelectionChangedEventArgs e)
        {
            cityCheckBox.IsEnabled = true;
            cityCheckBox.IsChecked = false;
            districtCheckBox.IsChecked = false;
            districtCheckBox.IsEnabled = false;
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;

            if (!InitializeCitiesComboBox())
                return;

            //InitializeSalesTree();
        }

        private void OnSelChangedCityComboBox(object sender, SelectionChangedEventArgs e)
        {
            districtCheckBox.IsEnabled = true;
            districtCheckBox.IsChecked = false;
            districtComboBox.IsEnabled = false;

            if (!InitializeDistrictsComboBox())
                return;

            //InitializeSalesTree();
        }

        private void OnSelChangedDistrictComboBox(object sender, SelectionChangedEventArgs e)
        {
            //InitializeSalesTree();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON UNCHECKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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


            stateCheckBox.IsEnabled = false;
            cityCheckBox.IsEnabled = false;
            districtCheckBox.IsEnabled = false;

            stateCheckBox.IsChecked = false;
            cityCheckBox.IsChecked = false;
            districtCheckBox.IsChecked = false;
            //InitializeSalesTree();

        }

        private void OnUncheckedStateCheckBox(object sender, RoutedEventArgs e)
        {
            stateComboBox.IsEnabled = false;
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;

            cityComboBox.SelectedItem = null;
            districtComboBox.SelectedItem = null;
            stateComboBox.SelectedItem = null;

            cityCheckBox.IsEnabled = false;
            districtCheckBox.IsEnabled = false;

            cityCheckBox.IsChecked = false;
            districtCheckBox.IsChecked = false;
            //InitializeSalesTree();
        }

        private void OnUncheckedCityCheckBox(object sender, RoutedEventArgs e)
        {
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;

            districtCheckBox.IsEnabled = false;

            cityComboBox.SelectedItem = null;
            districtComboBox.SelectedItem = null;

            cityCheckBox.IsChecked = false;
            districtCheckBox.IsChecked = false;
            //InitializeSalesTree();
        }

        private void OnUncheckedDistrictCheckBox(object sender, RoutedEventArgs e)
        {
            districtComboBox.SelectedItem = null;

            districtComboBox.IsEnabled = false;
            //InitializeSalesTree();
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON BTN CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnBtnClickAddProjectLocation(object sender, RoutedEventArgs e)
        {
            AddProjectLocationWindow addProjectLocationWindow = new AddProjectLocationWindow(ref loggedInUser);
            addProjectLocationWindow.Closed += OnClosedAddCompanyWindow;
            addProjectLocationWindow.Show();
        }
        private void OnBtnClickAddProject(object sender, RoutedEventArgs e)
        {
            AddProjectWindow addProjectWindow = new AddProjectWindow(ref loggedInUser);
            addProjectWindow.Closed += OnClosedAddContactWindow;
            addProjectWindow.Show();
        }

        private void OnClosedAddCompanyWindow(object sender, EventArgs e)
        {
            //employeesCompanies.Clear();

            //if (!InitializeCompaniesList())
            //    return;

            //GetAllContacts();

            //SetDefaultSettings();

            //InitializeSalesTree();
        }
        private void OnClosedAddContactWindow(object sender, EventArgs e)
        {
            //employeesCompanies.Clear();

            //if (!InitializeCompaniesList())
            //    return;

            //GetAllContacts();

            //SetDefaultSettings();

            //InitializeSalesTree();
        }
        private void OnSelectedItemChangedTreeViewItem(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem selectedItem = (TreeViewItem)projectsTreeView.SelectedItem;
            if (selectedItem != null)
            {
                try
                {
                    object parent = selectedItem.Parent;
                    TreeViewItem currentCompany = (TreeViewItem)parent;

                    try
                    {
                        object parent2 = currentCompany.Parent;
                        TreeViewItem currentSales = (TreeViewItem)parent2;

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

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //EXTERNAL TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
        private void OnButtonClickedProjects(object sender, MouseButtonEventArgs e)
        {
            ProjectsPage projectsPage = new ProjectsPage(ref loggedInUser);
            this.NavigationService.Navigate(projectsPage);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //BTN CLICKED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
