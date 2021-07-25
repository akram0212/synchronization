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
using System.Windows.Shapes;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for AddClientVisitWindow.xaml
    /// </summary>
    public partial class AddClientVisitWindow : Window
    {
        CommonQueries commonQueries;
        Employee loggedInUser;
        ClientVisit clientVisit;
        List<COMPANY_ORGANISATION_MACROS.COMPANY_LIST_STRUCT> companies;
        List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT> contacts;
        List<COMPANY_WORK_MACROS.VISIT_PURPOSE_STRUCT> visitPurposes;
        List<COMPANY_WORK_MACROS.VISIT_RESULT_STRUCT> visitResults;
        List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> branches;

        public AddClientVisitWindow(ref Employee mloggedInUser)
        {
            InitializeComponent();

            loggedInUser = mloggedInUser;

            CalendarDateRange cdr = new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-3));
            visitDatePicker.BlackoutDates.Add(cdr);

            clientVisit = new ClientVisit(loggedInUser);
            commonQueries = new CommonQueries();
            companies = new List<COMPANY_ORGANISATION_MACROS.COMPANY_LIST_STRUCT>();

            branches = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
            contacts = new List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT>();
            
            visitPurposes = new List<COMPANY_WORK_MACROS.VISIT_PURPOSE_STRUCT>();
            visitResults = new List<COMPANY_WORK_MACROS.VISIT_RESULT_STRUCT>();

            companyBranchComboBox.IsEnabled = false;
            contactComboBox.IsEnabled = false;

            GetEmployeeCompanies();

            InitializeCompanyNameComboBox();
            InitializeVisitPurposes();
            InitializeVisitResults();

        }
        public bool GetEmployeeCompanies()
        {
            if (!commonQueries.GetEmployeeCompanies(loggedInUser.GetEmployeeId(), ref companies))
                return false;

            return true;
        }
        private bool GetCompanyBranches()
        {
            branches.Clear();

            if (!commonQueries.GetCompanyAddresses(companies[companyNameComboBox.SelectedIndex].company_serial, ref branches))
                return false;

            //String sqlQueryPart1 = @"select company_address.address_serial,company_address.address,
            //     districts.district,cities.city,states_governorates.state_governorate,countries.country
            //     from erp_system.dbo.company_address
            //     inner join erp_system.dbo.company_name
            //     on company_address.company_serial = company_name.company_serial
            //     inner join erp_system.dbo.districts
            //     on company_address.address = districts.id
            //     inner join erp_system.dbo.cities
            //     on districts.city = cities.id
            //     inner join erp_system.dbo.states_governorates
            //     on cities.state_governorate = states_governorates.id
            //     inner join erp_system.dbo.countries
            //     on states_governorates.country = countries.id
            //     where company_name.added_by = ";
            //
            //sqlQuery = String.Empty;
            //sqlQuery += sqlQueryPart1;
            //sqlQuery += loggedInUser.GetEmployeeId();
            //
            //BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            //
            //queryColumns.sql_int = 2;
            //queryColumns.sql_string = 4;
            //
            //if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
            //    return false;
            //

            //for (int i = 0; i < branches.Count; i++)
            //{
            //    COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT tempItem;
            //
            //    tempItem.address_serial = sqlDatabase.rows[i].sql_int[0];
            //    tempItem.address = sqlDatabase.rows[i].sql_int[1];
            //
            //    tempItem.district = sqlDatabase.rows[i].sql_string[0];
            //    tempItem.state_governorate = sqlDatabase.rows[i].sql_string[1];
            //    tempItem.city = sqlDatabase.rows[i].sql_string[2];
            //    tempItem.country = sqlDatabase.rows[i].sql_string[3];
            //
            //    branches.Add(tempItem);
            //}

            return true;
        }

        private void InitializeCompanyNameComboBox()
        {
            for(int i = 0; i < companies.Count(); i++)
                companyNameComboBox.Items.Add(companies[i].company_name);

        }
        private void InitializeCompanyBranchesComboBox()
        {
            companyBranchComboBox.Items.Clear();
            
            for (int i = 0; i < branches.Count; i++)
                companyBranchComboBox.Items.Add(branches[i].district + ", " + branches[i].city + ", " + branches[i].state_governorate + ", " + branches[i].country);

            //YOU SHALL ALWAYS SELECT FIRST BRANCH ADDRESS, NO NEED TO CHECK FOR NO. OF BRANCHES
            //YOU SHALL SET THE SELECTED ITEM LIKE THIS, NO NEED TO SELECT DISTRICT, CITY, STATE AND COUNTRY
            companyBranchComboBox.SelectedIndex = 0;

            contactComboBox.IsEnabled = false;
        }
        private void InitializeCompanyContactsComboBox()
        {
            contactComboBox.Items.Clear();
            
            if(companyBranchComboBox.SelectedItem != null)
                commonQueries.GetCompanyContacts(loggedInUser.GetEmployeeId(), branches[companyBranchComboBox.SelectedIndex].address_serial, ref contacts);
            
            for (int i = 0; i < contacts.Count; i++)
                contactComboBox.Items.Add(contacts[i].contact_name);

            if (contacts.Count == 1)
                contactComboBox.SelectedIndex = 0;

        }
        //ANY FUNCTION THAT USES COMMONQUERIES OR MAKES A QUERY ITSELF SHOULD BE BOOL
        private bool InitializeVisitPurposes()
        {
            visitPurposeComboBox.Items.Clear();

            //YOU SHALL ALWAYS CHECK THAT COMMONQUERIES FUNCTION IS ALWAYS EXECUTED CORRECTLY
            if (!commonQueries.GetVisitPurposes(ref visitPurposes))
                return false;

            for (int i = 0; i < visitPurposes.Count; i++)
                visitPurposeComboBox.Items.Add(visitPurposes[i].purpose_name);

            return false;

        } 
        private bool InitializeVisitResults()
        {
            visitResultComboBox.Items.Clear();

            //YOU SHALL ALWAYS CHECK THAT COMMONQUERIES FUNCTION IS ALWAYS EXECUTED CORRECTLY
            if (!commonQueries.GetVisitResults(ref visitResults))
                return false;

            for (int i = 0; i < visitResults.Count; i++)
                visitResultComboBox.Items.Add(visitResults[i].result_name);

            return true;
        }
        
        
        private bool CheckCompanyNameComboBox()
        {
            if (companyNameComboBox.SelectedItem == null)
            {
                MessageBox.Show("Company must be specified.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        } 
        private bool CheckCompanyBranchComboBox()
        {
            if (companyBranchComboBox.SelectedItem == null)
            {
                MessageBox.Show("Branch must be specified.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

           return true;
        } 
        private bool CheckContactNameComboBox()
        {
            if (contactComboBox.SelectedItem == null)
            {
                MessageBox.Show("Contact must be specified.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        } 
        private bool CheckVisitDatePicker()
        {
            if (visitDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Visit Date must be specified.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            clientVisit.SetVisitDate(DateTime.Parse(visitDatePicker.SelectedDate.ToString()));

            return true;
        } 
        private bool CheckVisitPurposeComboBox()
        {
            if (visitPurposeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Visit Purpose must be specified.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }  
        private bool CheckVisitResultComboBox()
        {
            if (visitResultComboBox.SelectedItem == null)
            {
                MessageBox.Show("Visit Result must be specified.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
        private void OnSelChangedadditionalDescription(object sender, RoutedEventArgs e)
        {

        }

        private void OnSelChangedVisitResult(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnSelChangedVisitPurpose(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnSelChangedVisitDate(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnSelChangedContact(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnSelChangedBranch(object sender, SelectionChangedEventArgs e)
        {
            contactComboBox.IsEnabled = true;
            InitializeCompanyContactsComboBox();
        }

        private void OnSelChangedCompany(object sender, SelectionChangedEventArgs e)
        {
            companyBranchComboBox.IsEnabled = true;
            contactComboBox.IsEnabled = false;
            contactComboBox.Items.Clear();
            GetCompanyBranches();
            InitializeCompanyBranchesComboBox();
        }

        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {
            if (!CheckCompanyNameComboBox())
                return; 
            if (!CheckCompanyBranchComboBox())
                return;
            if (!CheckContactNameComboBox())
                return;
            if (!CheckVisitDatePicker())
                return;
            if (!CheckVisitPurposeComboBox())
                return; 
            if (!CheckVisitResultComboBox())
                return;

            //YOU SHALL INITIALIZE YOUR CLASS OBJECTS WITH THE INITIALIZATION FUNCTIONS
            clientVisit.InitializeSalesPersonInfo(loggedInUser.GetEmployeeId());
            clientVisit.InitializeBranchInfo(branches[companyBranchComboBox.SelectedIndex].address_serial);
            clientVisit.InitializeContactInfo(contacts[contactComboBox.SelectedIndex].contact_id);
            
            //THEN SET OTHER PARAMETERS
            clientVisit.SetVisitDate(Convert.ToDateTime(visitDatePicker.Text));

            clientVisit.SetVisitPurpose(visitPurposes[visitPurposeComboBox.SelectedIndex].purpose_id, visitPurposes[visitPurposeComboBox.SelectedIndex].purpose_name);
            clientVisit.SetVisitResult(visitResults[visitResultComboBox.SelectedIndex].result_id, visitResults[visitResultComboBox.SelectedIndex].result_name);
            

            clientVisit.SetVisitNotes(additionalDescriptionTextBox.Text.ToString());
            
            //LASTLY, YOU SHALL CALL THIS FUNCTION, IT GENERATES A NEW SERIAL, ID AND ANY OTHER PARAMETERS
            //AND INSERTS DATA INTO DATABASE
            clientVisit.IssueNewVisit();

            
            this.Hide();
        }
    }
}
