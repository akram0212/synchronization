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
    /// Interaction logic for AddContactWindow.xaml
    /// </summary>
    public partial class AddContactWindow : Window
    {
        protected String sqlQuery;

        public Employee loggedInUser;
        public Contact contact;

        public ClientVisit clientVisit;
        public ClientCall clientCall;
        public OfficeMeeting officeMeeting;

        protected SQLServer sqlServer;
        protected CommonQueries commonQueries;
        protected CommonFunctions commonFunctions;
        protected IntegrityChecks integrityChecker;

        String firstName;
        String lastName;

        List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> companies;
        List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> companyAddresses;
        List<COMPANY_ORGANISATION_MACROS.DEPARTMENT_STRUCT> departments;
        List<COMPANY_ORGANISATION_MACROS.TEAM_STRUCT> teams;
        public AddContactWindow()
        {
            InitializeComponent();
        }

        public AddContactWindow(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            sqlServer = new SQLServer();
            commonQueries = new CommonQueries();
            commonFunctions = new CommonFunctions();
            integrityChecker = new IntegrityChecks();
            clientCall = new ClientCall(sqlServer);
            clientVisit = new ClientVisit(sqlServer);
            officeMeeting = new OfficeMeeting(sqlServer);

            contact = new Contact();

            companies = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
            companyAddresses = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
            departments = new List<COMPANY_ORGANISATION_MACROS.DEPARTMENT_STRUCT>();
            teams = new List<COMPANY_ORGANISATION_MACROS.TEAM_STRUCT>();

            if (!commonQueries.GetEmployeeCompanies(loggedInUser.GetEmployeeId(), ref companies))
                return;
            for (int i = 0; i < companies.Count; i++)
            {
                companyNameComboBox.Items.Add(companies[i].company_name);
            }

            if (!commonQueries.GetDepartmentsType(ref departments))
                return;
            for (int i = 0; i < departments.Count; i++)
            {
                employeeDepartmentComboBox.Items.Add(departments[i].department_name);
            }

            contactGenderComboBox.Items.Add("Male");
            contactGenderComboBox.Items.Add("Female");

            companyBranchComboBox.IsEnabled = false;

        }

        private void OnTextChangedFirstName(object sender, TextChangedEventArgs e)
        {

        }

        private void OnSelChangedCompany(object sender, SelectionChangedEventArgs e)
        {
            if (companyNameComboBox.SelectedItem != null)
            {
                if (!commonQueries.GetCompanyAddresses(companies[companyNameComboBox.SelectedIndex].company_serial, ref companyAddresses))
                    return;

                companyBranchComboBox.Items.Clear();
                companyBranchComboBox.IsEnabled = true;
                for (int i = 0; i < companyAddresses.Count; i++)
                {
                    companyBranchComboBox.Items.Add(companyAddresses[i].country + ", " + companyAddresses[i].state_governorate + ", " + companyAddresses[i].city + ", " + companyAddresses[i].district);
                }
                companyBranchComboBox.SelectedIndex = 0;
            }
            else
            {
                companyBranchComboBox.IsEnabled = true;
                companyBranchComboBox.SelectedItem = null;
            }
        }

        private void OnSelChangedBranch(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnSelChangedDepartment(object sender, SelectionChangedEventArgs e)
        {
        }

        private void OnSelChangedTeam(object sender, SelectionChangedEventArgs e)
        {
        }
        private void OnTextChangedBusinessPhone(object sender, TextChangedEventArgs e)
        {
        }
        private void OnTextChangedPersonalPhone(object sender, TextChangedEventArgs e)
        {
        }

        private void OnTextChangedBusinessEmail(object sender, TextChangedEventArgs e)
        {
        }

        private void OnTextChangedPersonalEmail(object sender, TextChangedEventArgs e)
        {
        }

        private bool CheckContactFirstNameEditBox()
        {
            String inputString = employeeFirstNameTextBox.Text;
            String outputString = employeeFirstNameTextBox.Text;


            if (!integrityChecker.CheckContactNameEditBox(inputString, ref outputString, true))
                return false;

            firstName = outputString;
            contact.SetContactName(firstName + " " + lastName);
            employeeFirstNameTextBox.Text = firstName;

            return true;
        }

        private bool CheckContactLastNameEditBox()
        {
            String inputString = employeeLastNameTextBox.Text;
            String outputString = employeeLastNameTextBox.Text;

            if (!integrityChecker.CheckContactNameEditBox(inputString, ref outputString, true))
                return false;

            lastName = outputString;
            contact.SetContactName(firstName + " " + lastName);
            employeeLastNameTextBox.Text = lastName;

            return true;
        }
        
        private bool CheckContactBusinessPhoneEditBox()
        {
            String inputString = employeeBusinessPhoneTextBox.Text;
            String outputString = employeeBusinessPhoneTextBox.Text;

            if (!integrityChecker.CheckContactPhoneEditBox(inputString, ref outputString, true))
                return false;

            contact.AddNewContactPhone(outputString);
            employeeBusinessPhoneTextBox.Text = outputString;

            return true;
        } 
        
        private bool CheckContactPersonalPhoneEditBox()
        {
            String inputString = employeePersonalPhoneTextBox.Text;
            String outputString = employeePersonalPhoneTextBox.Text;

            if (!integrityChecker.CheckContactPhoneEditBox(inputString, ref outputString, false))
                return false;

            if(outputString!= string.Empty)
            {
               contact.AddNewContactPhone(outputString);
               employeePersonalPhoneTextBox.Text = outputString;
            }

            return true;
        }
        
        private bool CheckContactBusinessEmailEditBox()
        {
            String inputString = employeeBusinessEmailTextBox.Text;
            String outputString = employeeBusinessEmailTextBox.Text;

            if (!integrityChecker.CheckContactBusinessEmailEditBox(inputString, companyAddresses[0].address / 1000000, ref outputString, true))
                return false;

            //YOU SHALL USE THIS FUNCTION TO HANDLE IDS AND EMAILS AUTOMATICALLY
            contact.SetContactBusinessEmail(outputString);
            employeeBusinessEmailTextBox.Text = contact.GetContactBusinessEmail();

            return true;
        }
        
        private bool CheckContactPersonalEmailEditBox()
        {
            String inputString = employeePersonalEmailTextBox.Text;
            String outputString = employeePersonalEmailTextBox.Text;

            if (!integrityChecker.CheckContactPersonalEmailEditBox(inputString, companyAddresses[0].address / 1000000, ref outputString, false))
                return false;

            //YOU SHALL USE THIS FUNCTION TO HANDLE IDS AND EMAILS AUTOMATICALLY

            if (outputString != string.Empty)
            {
                contact.AddNewContactEmail(outputString);
                employeePersonalEmailTextBox.Text = outputString;
            }

            return true;
        }

        private bool CheckContactGenderComboBox()
        {
            if (contactGenderComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Contact gender must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            contact.SetContactGender(contactGenderComboBox.SelectedItem.ToString());
            return true;
        }

        private bool CheckCompanyComboBox()
        {
            if (companyNameComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Company must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            contact.SetCompanyName(companyNameComboBox.SelectedItem.ToString());

            return true; 
        } 
        
        private bool CheckCompanyBranchComboBox()
        {
            if (companyBranchComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Company branch must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true; 
        }
        
        private bool CheckDepartmentComboBox()
        {
            if (employeeDepartmentComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Employee Department must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            contact.SetContactDepartment(departments[employeeDepartmentComboBox.SelectedIndex].department_id, employeeDepartmentComboBox.SelectedItem.ToString());

            return true; 
        }

        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {
            if (!CheckContactFirstNameEditBox())
                return;
            if (!CheckContactLastNameEditBox())
                return;
            if (!CheckContactGenderComboBox())
                return;
            if (!CheckCompanyComboBox())
                return; 
            if (!CheckCompanyBranchComboBox())
                return; 
            if (!CheckDepartmentComboBox())
                return; 
            if (!CheckContactBusinessPhoneEditBox())
                return; 
            if (!CheckContactPersonalPhoneEditBox())
                return; 
            if (!CheckContactBusinessEmailEditBox())
                return;   
            if (!CheckContactPersonalEmailEditBox())
                return;

            //YOU DON'T NEED TO WRITE A FUNCTION TO GET NEW CONTACT ID, THE CONTACT CLASS HANDLES IT ALREADY

            contact.SetAddressSerial(companyAddresses[companyBranchComboBox.SelectedIndex].address_serial);

            contact.SetSalesPerson(loggedInUser);

            contact.IssueNewContact();

            //YOU DON'T NEED TO GET A NEW EMAIL/PHONE ID, THIS IS A NEW CONTACT, SO THE EMAIL ID SHALL BE 1,
            //ALSO, YOU SHALL ONLY USE CONTACT.ADDNEWPERSONALEMAIL() / CONTACT.ADDNEWCONTACTPHONE(), 
            //THIS FUNCTION SHALL HANDLE THE IDs BY ITSELF

            for (int i = 0; i < contact.GetNumberOfSavedContactPhones(); i++)
                contact.InsertIntoContactMobile(i + 1, contact.GetContactPhones()[i]);

            for (int i = 0; i < contact.GetNumberOfSavedContactEmails(); i++)
                contact.InsertIntoContactPersonalEmail(i + 1, contact.GetContactPersonalEmails()[i]);

            this.Close();
        }

        private void OnSelChangedGender(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnTextChangedLastName(object sender, TextChangedEventArgs e)
        {

        }

    }
}
