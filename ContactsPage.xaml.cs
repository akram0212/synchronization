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

        private List<KeyValuePair<int, List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>>> employeesCompanies;

        private List<KeyValuePair<int, List<COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT>>> employeesContacts;
        private List<COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT> contactsList;

        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> listOfEmployees;

        private List<BASIC_STRUCTS.PRIMARY_FIELD_STRUCT> primaryFieldsList;
        private List<BASIC_STRUCTS.SECONDARY_FIELD_STRUCT> secondaryFieldsList;
        
        private List<KeyValuePair<int, TreeViewItem>> salesTreeArray = new List<KeyValuePair<int, TreeViewItem>>();
        private List<KeyValuePair<int, TreeViewItem>> companiesTreeArray = new List<KeyValuePair<int, TreeViewItem>>();

        //private TreeViewItem[] companiesTreeArray = new TreeViewItem[COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_COMPANIES];

        private List<KeyValuePair<TreeViewItem, COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT>> contactsTreeArray = new List<KeyValuePair<TreeViewItem, COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT>>();

        public ContactsPage(ref Employee mLoggedInUser)
        {
            initializationObject = new SQLServer();

            employeesCompanies = new List<KeyValuePair<int, List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>>>();
            employeesContacts = new List<KeyValuePair<int, List<COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT>>>();
            listOfEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();

            contactsList = new List<COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT>();

            primaryFieldsList = new List<BASIC_STRUCTS.PRIMARY_FIELD_STRUCT>();
            secondaryFieldsList = new List<BASIC_STRUCTS.SECONDARY_FIELD_STRUCT>();
            
            commonQueries = new CommonQueries();
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            companyNameTextBox.IsEnabled = false;
            contactNameTextBox.IsEnabled = false;
            primaryFieldComboBox.IsEnabled = false;
            secondaryFieldComboBox.IsEnabled = false;
            salesPersonComboBox.IsEnabled = false;

            secondaryFieldCheckBox.IsEnabled = false;
            salesPersonCheckBox.IsEnabled = true;

            ViewBtn.IsEnabled = false;

            InitializePrimaryFieldComboBox();

            if (!CheckEmployeePosition())
                return;

            if (!InitializeCompaniesList())
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
        private bool InitializeSalesPersonComboBox()
        {
            salesPersonComboBox.Items.Clear();

            for (int i = 0; i < listOfEmployees.Count; i++)
            {
                salesPersonComboBox.Items.Add(listOfEmployees[i].employee_name);
            }

            return true;
        }
        private bool InitializeCompaniesList()
        {
            for (int i = 0; i < listOfEmployees.Count; i++)
            {
                List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> tmpList = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();

                commonQueries.GetEmployeeCompanies(listOfEmployees[i].employee_id, ref tmpList);
                employeesCompanies.Add(new KeyValuePair<int, List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>>(listOfEmployees[i].employee_id, tmpList));
            }
            return true;
        }
        private bool CheckEmployeePosition()
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

            InitializeSalesPersonComboBox();

            return true;
        }
        
        public bool GetAllContacts()
        {
            employeesContacts.Clear();

            for (int i = 0; i < listOfEmployees.Count; i++)
            {
                List<COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT> tmpList = new List<COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT>();
                commonQueries.GetEmployeeContacts(listOfEmployees[i].employee_id, ref tmpList);

                 employeesContacts.Add(new KeyValuePair<int, List<COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT>>(listOfEmployees[i].employee_id, tmpList));
                
            }

            return true;
        }
        private void SetEmployeeComboBox()
        {
            //salesPersonComboBox.SelectedIndex = 0;

            for (int i = 0; i < listOfEmployees.Count; i++)
                if (loggedInUser.GetEmployeeId() == listOfEmployees[i].employee_id)
                    salesPersonComboBox.SelectedIndex = i;
        }

    
        public bool InitializePrimaryFieldComboBox()
        {
            primaryFieldComboBox.Items.Clear();

            if (!commonQueries.GetPrimaryWorkFields(ref primaryFieldsList))
                return false;

            for (int i = 0; i < primaryFieldsList.Count; i++)
                primaryFieldComboBox.Items.Add(primaryFieldsList[i].field_name);

            return true;
        }
        public bool InitializeSecondaryFieldComboBox()
        {
            secondaryFieldComboBox.Items.Clear();
            if (primaryFieldComboBox.SelectedItem != null)
            {
                if (!commonQueries.GetSecondaryWorkFields(primaryFieldsList[primaryFieldComboBox.SelectedIndex].field_id, ref secondaryFieldsList))
                    return false;

                for (int i = 0; i < secondaryFieldsList.Count; i++)
                    secondaryFieldComboBox.Items.Add(secondaryFieldsList[i].field_name);
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
                if (salesPersonCheckBox.IsChecked == true && salesPersonComboBox.SelectedItem != null && employeesCompanies[i].Key != listOfEmployees[salesPersonComboBox.SelectedIndex].employee_id)
                    continue;

                TreeViewItem currentEmployeeTreeItem = salesTreeArray.Find(tree_item => tree_item.Key == employeesCompanies[i].Key).Value;

                for (int j = 0; j < employeesCompanies[i].Value.Count; j++)
                {
                    string temp = employeesCompanies[i].Value[j].company_name;
                    bool contains = temp.IndexOf(companyNameTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                    if (companyNameCheckBox.IsChecked == true && companyNameTextBox.Text != null && !contains)
                        continue;

                    if (primaryFieldCheckBox.IsChecked == true && primaryFieldComboBox.SelectedItem != null && primaryFieldsList[primaryFieldComboBox.SelectedIndex].field_id != employeesCompanies[i].Value[j].company_field / BASIC_MACROS.MAXIMUM_SECONDARY_FIELDS_NO)
                        continue;
                    if (secondaryFieldCheckBox.IsChecked == true && secondaryFieldComboBox.SelectedItem != null && secondaryFieldsList[secondaryFieldComboBox.SelectedIndex].field_id != employeesCompanies[i].Value[j].company_field)
                        continue;

                    TreeViewItem ChildItem = new TreeViewItem();
                    ChildItem.Header = employeesCompanies[i].Value[j].company_name;
                    ChildItem.Tag = employeesCompanies[i].Value[j].company_serial;
                    ChildItem.FontSize = 13;
                    ChildItem.FontWeight = FontWeights.SemiBold;

                    companiesTreeArray.Add(new KeyValuePair<int, TreeViewItem>(employeesCompanies[i].Value[j].company_serial, ChildItem));

                    currentEmployeeTreeItem.Items.Add(ChildItem);
                }

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

                        string temp = employeesContacts[i].Value[j].contact_name;
                        bool contains = temp.IndexOf(contactNameTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                        if (contactNameCheckBox.IsChecked == true && contactNameTextBox.Text != null && !contains)
                            continue;

                        companyTreeItem = companiesTreeArray.Find(company_item => company_item.Key == employeesContacts[i].Value[j].company_serial).Value;
                        
                        TreeViewItem contactTreeItem = new TreeViewItem();

                        contactTreeItem.Header = employeesContacts[i].Value[j].contact_name;
                        contactTreeItem.Tag = employeesContacts[i].Value[j].contact_id;
                        contactTreeItem.FontSize = 12;
                        contactTreeItem.FontWeight = FontWeights.Normal;

                        companyTreeItem.Items.Add(contactTreeItem);

                        contactsTreeArray.Add(new KeyValuePair<TreeViewItem, COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT>(contactTreeItem, employeesContacts[i].Value[j]));

                    }
                }
                
            }

            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON CHECK HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnCheckedCompanyNameCheckBox(object sender, RoutedEventArgs e)
        {
            companyNameTextBox.IsEnabled = true;
        }


        private void OnCheckedContactNameCheckBox(object sender, RoutedEventArgs e)
        {
            contactNameTextBox.IsEnabled = true;
        }


        private void OnCheckedPrimaryFieldCheckBox(object sender, RoutedEventArgs e)
        {
            primaryFieldComboBox.IsEnabled = true;
            primaryFieldComboBox.SelectedIndex = 0;
        }


        private void OnCheckedSecondaryFieldCheckBox(object sender, RoutedEventArgs e)
        {
            secondaryFieldComboBox.IsEnabled = true;
            secondaryFieldComboBox.SelectedIndex = 0;
        }
        private void OnCheckedSalesPersonCheckBox(object sender, RoutedEventArgs e)
        {
            salesPersonComboBox.IsEnabled = true;

            SetEmployeeComboBox();
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void OnTextChangedCompanyName(object sender, TextChangedEventArgs e)
        {
            
            InitializeSalesTree();
        }

        private void OnTextChangedContactName(object sender, TextChangedEventArgs e)
        {
            InitializeSalesTree();
        }

        private void OnSelChangedPrimaryFieldComboBox(object sender, SelectionChangedEventArgs e)
        {
            secondaryFieldCheckBox.IsEnabled = true;
            secondaryFieldCheckBox.IsChecked = false;
            secondaryFieldComboBox.IsEnabled = false;
            ViewBtn.IsEnabled = false;

            if (!InitializeSecondaryFieldComboBox())
                return;

            InitializeSalesTree();
        }

        private void OnSelChangedSecondaryFieldComboBox(object sender, SelectionChangedEventArgs e)
        {
            ViewBtn.IsEnabled = false;

            InitializeSalesTree();
        }
        private void OnSelChangedSalesPersonComboBox(object sender, SelectionChangedEventArgs e)
        {
            ViewBtn.IsEnabled = false;

            if (salesPersonCheckBox.IsChecked == true && salesPersonComboBox.SelectedItem != null)
                selectedEmployee = listOfEmployees[salesPersonComboBox.SelectedIndex].employee_id;
            else
                selectedEmployee = 0;

            InitializeSalesTree();

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON UNCHECKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnUncheckedCompanyNameCheckBox(object sender, RoutedEventArgs e)
        {
            companyNameTextBox.IsEnabled = false;
            companyNameTextBox.Text = null;

            ViewBtn.IsEnabled = false;

            InitializeSalesTree();

        }

        private void OnUncheckedContactNameCheckBox(object sender, RoutedEventArgs e)
        {
            contactNameTextBox.IsEnabled = false;
            contactNameTextBox.Text = null;

            ViewBtn.IsEnabled = false;

            InitializeSalesTree();
        }
        private void OnUncheckedPrimaryFieldCheckBox(object sender, RoutedEventArgs e)
        {
            primaryFieldComboBox.IsEnabled = false;
            secondaryFieldComboBox.IsEnabled = false;
            ViewBtn.IsEnabled = false;

            secondaryFieldCheckBox.IsEnabled = false;

            primaryFieldComboBox.SelectedItem = null;
            secondaryFieldComboBox.SelectedItem = null;

            primaryFieldCheckBox.IsChecked = false;
            secondaryFieldCheckBox.IsChecked = false;
            InitializeSalesTree();
        }

        private void OnUncheckedSecondaryFieldCheckBox(object sender, RoutedEventArgs e)
        {
            secondaryFieldComboBox.SelectedItem = null;

            secondaryFieldComboBox.IsEnabled = false;
            ViewBtn.IsEnabled = false;

            InitializeSalesTree();
        }
        private void OnUncheckedSalesPersonCheckBox(object sender, RoutedEventArgs e)
        {
            salesPersonComboBox.IsEnabled = false;
            salesPersonComboBox.SelectedItem = null;
            InitializeSalesTree();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON BTN CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnBtnClickAddCompany(object sender, RoutedEventArgs e)
        {
            ViewBtn.IsEnabled = false;
            AddCompanyWindow addCompanyWindow = new AddCompanyWindow(ref loggedInUser);
            addCompanyWindow.Closed += OnClosedAddCompanyWindow;
            addCompanyWindow.Show();
        }
        private void OnBtnClickAddContact(object sender, RoutedEventArgs e)
        {
            ViewBtn.IsEnabled = false;

            AddContactWindow addContactWindow = new AddContactWindow(ref loggedInUser);
            addContactWindow.Closed += OnClosedAddContactWindow;
            addContactWindow.Show();
        }

        private void OnClosedAddCompanyWindow(object sender, EventArgs e)
        {
            employeesCompanies.Clear();

            if (!InitializeCompaniesList())
                return;

            GetAllContacts();

            SetDefaultSettings();

            InitializeSalesTree();
        }
        private void OnClosedAddContactWindow(object sender, EventArgs e)
        {
            employeesCompanies.Clear();

            if (!InitializeCompaniesList())
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

        private void OnBtnClickView(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = (TreeViewItem)contactTreeView.SelectedItem;

            if (!selectedItem.HasItems)
            {
                COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT currentContactStruct = new COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT();
                currentContactStruct = contactsTreeArray.Find(current_item => current_item.Key.Tag == selectedItem.Tag).Value;

                Contact selectedContact = new Contact();
                object parent = selectedItem.Parent;
                TreeViewItem currentCompany = (TreeViewItem)parent;

                try
                {
                    object parent2 = currentCompany.Parent;
                    TreeViewItem currentSales = (TreeViewItem)parent2;

                    selectedContact.InitializeContactInfo(Convert.ToInt32
                        (currentSales.Tag), currentContactStruct.address_serial, currentContactStruct.contact_id);

                    ViewContactWindow viewContactWindow = new ViewContactWindow(ref loggedInUser, ref selectedContact);
                    viewContactWindow.Show();
                }
                catch
                {
                    Company currentCompanyClass = new Company();
                    currentCompanyClass.InitializeCompanyInfo(Convert.ToInt32(selectedItem.Tag));

                   // try
                    //{
                        object parent3 = selectedItem.Parent;
                        TreeViewItem currentSales = (TreeViewItem)parent3;
                        ViewCompanyWindow viewCompanyWindow = new ViewCompanyWindow(ref loggedInUser, ref currentCompanyClass);
                        viewCompanyWindow.Closed += OnClosedAddContactWindow;
                        viewCompanyWindow.Show();
                    //}
                    //catch
                    //{
                    //}
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
                    viewCompanyWindow.Closed += OnClosedAddContactWindow;
                    viewCompanyWindow.Show();
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
            QuotationsPage workOffers = new QuotationsPage(ref loggedInUser);
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
        private void OnButtonClickedMaintenanceContracts(object sender, MouseButtonEventArgs e)
        {

        }
        private void OnButtonClickedMaintenanceOffer(object sender, MouseButtonEventArgs e)
        {
            MaintenanceOffersPage maintenanceOffersPage = new MaintenanceOffersPage(ref loggedInUser);
            this.NavigationService.Navigate(maintenanceOffersPage);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //BTN CLICKED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }


}
