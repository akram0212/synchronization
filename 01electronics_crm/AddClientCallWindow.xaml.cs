using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using _01electronics_library;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for AddClientCallWindow.xaml
    /// </summary>
    public partial class AddClientCallWindow : Window
    {
        CommonQueries commonQueries;
        Employee loggedInUser;
        ClientCall clientCall;
        List<COMPANY_ORGANISATION_MACROS.COMPANY_LIST_STRUCT> companies;
        List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT> contacts;
        List<COMPANY_WORK_MACROS.CALL_PURPOSE_STRUCT> callPurposes;
        List<COMPANY_WORK_MACROS.CALL_RESULT_STRUCT> callResults;
        List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> branches;
        public AddClientCallWindow(ref Employee mloggedInUser)
        {
            InitializeComponent();
            loggedInUser = mloggedInUser;

            DayOfWeek today = DateTime.Today.DayOfWeek;

            DayOfWeek firstDay = DayOfWeek.Sunday;
            DayOfWeek lastDay = DayOfWeek.Saturday;

            int minDate;
            int maxDate;

            minDate = today - firstDay + 1;
            maxDate = lastDay - today + 1;

            CalendarDateRange cdr = new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-minDate));
            CallDatePicker.BlackoutDates.Add(cdr);

            CalendarDateRange cdr2 = new CalendarDateRange(DateTime.Today.AddDays(maxDate), DateTime.MaxValue);
            CallDatePicker.BlackoutDates.Add(cdr2);

            //CalendarDateRange cdr = new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-3));
            //CallDatePicker.BlackoutDates.Add(cdr);

            clientCall = new ClientCall();
            commonQueries = new CommonQueries();
            companies = new List<COMPANY_ORGANISATION_MACROS.COMPANY_LIST_STRUCT>();

            branches = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
            contacts = new List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT>();

            callPurposes = new List<COMPANY_WORK_MACROS.CALL_PURPOSE_STRUCT>();
            callResults = new List<COMPANY_WORK_MACROS.CALL_RESULT_STRUCT>();

            companyBranchComboBox.IsEnabled = false;
            contactComboBox.IsEnabled = false;

            GetEmployeeCompanies();

            InitializeCompanyNameComboBox();
            InitializeCallPurposes();
            InitializeCallResults();
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

            return true;
        }

        //////////////////////////////////////////////////////////
        /// INITIALIZATION FUNCTIONS
        //////////////////////////////////////////////////////////
        private void InitializeCompanyNameComboBox()
        {
            for (int i = 0; i < companies.Count(); i++)
                companyNameComboBox.Items.Add(companies[i].company_name);

        }
        private void InitializeCompanyBranchesComboBox()
        {
            companyBranchComboBox.Items.Clear();

            for (int i = 0; i < branches.Count; i++)
                companyBranchComboBox.Items.Add(branches[i].district + ", " + branches[i].city + ", " + branches[i].state_governorate + ", " + branches[i].country);
        
            companyBranchComboBox.SelectedIndex = 0;

            //contactComboBox.IsEnabled = false;
        }
        private void InitializeCompanyContactsComboBox()
        {
            contactComboBox.Items.Clear();

            if (companyBranchComboBox.SelectedItem != null)
                commonQueries.GetCompanyContacts(loggedInUser.GetEmployeeId(), branches[companyBranchComboBox.SelectedIndex].address_serial, ref contacts);

            for (int i = 0; i < contacts.Count; i++)
                contactComboBox.Items.Add(contacts[i].contact_name);

            if (contacts.Count == 1)
                contactComboBox.SelectedIndex = 0;
            else
            {
                contactComboBox.IsEnabled = true;
                contactComboBox.SelectedIndex = 0;
            }
        }
        private bool InitializeCallPurposes()
        {
            CallPurposeComboBox.Items.Clear();

            if (!commonQueries.GetCallPurposes(ref callPurposes))
                return false;

            for (int i = 0; i < callPurposes.Count; i++)
                CallPurposeComboBox.Items.Add(callPurposes[i].purpose_name);

            return false;

        }
        private bool InitializeCallResults()
        {
            CallResultComboBox.Items.Clear();

            if (!commonQueries.GetCallResults(ref callResults))
                return false;

            for (int i = 0; i < callResults.Count; i++)
                CallResultComboBox.Items.Add(callResults[i].result_name);

            return true;
        }

        //////////////////////////////////////////////////////////
        /// CHECK HANDLERS
        //////////////////////////////////////////////////////////
        private bool CheckCompanyNameComboBox()
        {
            if (companyNameComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Company must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
        private bool CheckCompanyBranchComboBox()
        {
            if (companyBranchComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Branch must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
        private bool CheckContactNameComboBox()
        {
            if (contactComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Contact must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
        private bool CheckCallDatePicker()
        {
            if (CallDatePicker.SelectedDate == null)
            {
                System.Windows.Forms.MessageBox.Show("Call Date must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            clientCall.SetCallDate(DateTime.Parse(CallDatePicker.SelectedDate.ToString()));

            return true;
        }
        private bool CheckCallPurposeComboBox()
        {
            if (CallPurposeComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Call Purpose must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
        private bool CheckCallResultComboBox()
        {
            if (CallResultComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Call Result must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        //////////////////////////////////////////////////////////
        /// ON SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////
        ///
        private void OnSelChangedCompany(object sender, SelectionChangedEventArgs e)
        {
            companyBranchComboBox.IsEnabled = true;
            contactComboBox.IsEnabled = false;
            contactComboBox.Items.Clear();
            GetCompanyBranches();
            InitializeCompanyBranchesComboBox();
        }
        private void OnSelChangedBranch(object sender, SelectionChangedEventArgs e)
        {
            contactComboBox.IsEnabled = true;
            InitializeCompanyContactsComboBox();
        }
        private void OnSelChangedContact(object sender, SelectionChangedEventArgs e)
        {

        }
        private void OnSelChangedCallDate(object sender, SelectionChangedEventArgs e)
        {

        }
        private void OnSelChangedCallPurpose(object sender, SelectionChangedEventArgs e)
        {

        }
        private void OnSelChangedCallResult(object sender, SelectionChangedEventArgs e)
        {

        }
        private void OnSelChangedadditionalDescription(object sender, RoutedEventArgs e)
        {

        }

        //////////////////////////////////////////////////////////
        /// ON BTN CLICKED HANDLERS
        //////////////////////////////////////////////////////////
        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {
            if (!CheckCompanyNameComboBox())
                return;
            if (!CheckCompanyBranchComboBox())
                return;
            if (!CheckContactNameComboBox())
                return;
            if (!CheckCallDatePicker())
                return;
            if (!CheckCallPurposeComboBox())
                return;
            if (!CheckCallResultComboBox())
                return;

            clientCall.InitializeSalesPersonInfo(loggedInUser.GetEmployeeId());
            clientCall.InitializeBranchInfo(branches[companyBranchComboBox.SelectedIndex].address_serial);
            clientCall.InitializeContactInfo(contacts[contactComboBox.SelectedIndex].contact_id);

            clientCall.SetCallDate(Convert.ToDateTime(CallDatePicker.Text));

            clientCall.SetCallPurpose(callPurposes[CallPurposeComboBox.SelectedIndex].purpose_id, callPurposes[CallPurposeComboBox.SelectedIndex].purpose_name);
            clientCall.SetCallResult(callResults[CallResultComboBox.SelectedIndex].result_id, callResults[CallResultComboBox.SelectedIndex].result_name);


            clientCall.SetCallNotes(additionalDescriptionTextBox.Text.ToString());

            clientCall.IssueNewCall();

            this.Close();
        }
    }
}
