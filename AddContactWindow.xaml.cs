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
using _01electronics_erp;

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

        protected SQLServer sqlServer;
        protected CommonQueries commonQueries;
        protected CommonFunctions commonFunctions;
        protected IntegrityChecks integrityChecker;
        string address_serial;
        int address;
        int employeeID;
        int contactID;
        int personalEmailID;
        int mobileID;
        string firstName;
        string lastName;
        string company;
        string businessPhone;
        string personalPhone;
        string businessEmail;
        string personalEmail;

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
            employeeID = loggedInUser.GetEmployeeId();
            InitializeComponent();

            sqlServer = new SQLServer();
            commonQueries = new CommonQueries();
            commonFunctions = new CommonFunctions();
            integrityChecker = new IntegrityChecks();

            contact = new Contact();

            companies = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
            companyAddresses = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
            departments = new List<COMPANY_ORGANISATION_MACROS.DEPARTMENT_STRUCT>();
            teams = new List<COMPANY_ORGANISATION_MACROS.TEAM_STRUCT>();

            //this.address_serial = address_serial;
            //this.company = company;

            commonQueries.GetEmployeeCompanies(employeeID, ref companies);
            for (int i = 0; i < companies.Count; i++)
            {
                companyNameComboBox.Items.Add(companies[i].company_name);
            }

            commonQueries.GetDepartmentsType(ref departments);
            for (int i = 0; i < departments.Count; i++)
            {
                employeeDepartmentComboBox.Items.Add(departments[i].department_name);
            }

            commonQueries.GetTeamsType(ref teams);
            for (int i = 0; i < teams.Count; i++)
            {
                employeeTeamComboBox.Items.Add(teams[i].team_name);
            }

            contactGenderComboBox.Items.Add("Male");
            contactGenderComboBox.Items.Add("Female");

        }

        private void OnTextChangedFirstName(object sender, TextChangedEventArgs e)
        {

        }

        private void OnTextChangedLastName(object sender, TextChangedEventArgs e)
        {

        }

        private void OnSelChangedCompany(object sender, SelectionChangedEventArgs e)
        {
            if (companyNameComboBox.SelectedItem != null)
            {
                companyBranchComboBox.Items.Clear();
                commonQueries.GetCompanyAddresses(companies[companyNameComboBox.SelectedIndex].address_serial, ref companyAddresses);
                for (int i = 0; i < companyAddresses.Count; i++)
                {
                    companyBranchComboBox.Items.Add(companyAddresses[i].country + ",\t" + companyAddresses[i].state_governorate + ",\t" + companyAddresses[i].city + ",\t" + companyAddresses[i].district);
                }
            }
            else
            {
                MessageBox.Show("Company name must be specified.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnSelChangedBranch(object sender, SelectionChangedEventArgs e)
        {
            address_serial = companyAddresses[companyBranchComboBox.SelectedIndex].address_serial.ToString();
            address = companyAddresses[companyBranchComboBox.SelectedIndex].address;
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
            string inputString = employeeFirstNameTextBox.Text;
            string outputString = employeeFirstNameTextBox.Text;

            if (!integrityChecker.CheckContactNameEditBox(inputString, ref outputString, true))
                return false;

            firstName = outputString;
            contact.SetContactName(firstName + " " + lastName);
            employeeFirstNameTextBox.Text = firstName;

            return true;
        }

        private bool CheckContactLastNameEditBox()
        {
            string inputString = employeeLastNameTextBox.Text;
            string outputString = employeeLastNameTextBox.Text;

            if (!integrityChecker.CheckContactNameEditBox(inputString, ref outputString, true))
                return false;

            lastName = outputString;
            contact.SetContactName(firstName + " " + lastName);
            employeeLastNameTextBox.Text = lastName;

            return true;
        }
        
        private bool CheckContactBusinessPhoneEditBox()
        {
            string inputString = employeeBusinessPhoneTextBox.Text;
            string outputString = employeeBusinessPhoneTextBox.Text;

            if (!integrityChecker.CheckContactPhoneEditBox(inputString, ref outputString, true))
                return false;

            businessPhone = outputString;
            contact.AddNewContactPhone(businessPhone);
            employeeBusinessPhoneTextBox.Text = businessPhone;

            return true;
        } 
        
        private bool CheckContactPersonalPhoneEditBox()
        {
            string inputString = employeePersonalPhoneTextBox.Text;
            string outputString = employeePersonalPhoneTextBox.Text;

            if (!integrityChecker.CheckContactPhoneEditBox(inputString, ref outputString, false))
                return false;

            personalPhone = outputString;
            employeePersonalPhoneTextBox.Text = personalPhone;

            return true;
        }
        
        private bool CheckContactBusinessEmailEditBox()
        {
            string inputString = employeeBusinessEmailTextBox.Text;
            string outputString = employeeBusinessEmailTextBox.Text;

            if (!integrityChecker.CheckContactBusinessEmailEditBox(inputString, companyAddresses[0].address / 1000000, ref outputString, true))
                return false;

            businessEmail = outputString;
            contact.SetContactBusinessEmail(businessEmail);
            employeeBusinessEmailTextBox.Text = businessEmail;

            return true;
        }
        
        private bool CheckContactPersonalEmailEditBox()
        {
            string inputString = employeePersonalEmailTextBox.Text;
            string outputString = employeePersonalEmailTextBox.Text;

            if (!integrityChecker.CheckContactPersonalEmailEditBox(inputString, companyAddresses[0].address / 1000000, ref outputString, false))
                return false;

            personalEmail = outputString;
            employeePersonalEmailTextBox.Text = personalEmail;

            return true;
        }

        private bool CheckContactGenderComboBox()
        {
            if (contactGenderComboBox.SelectedItem == null)
            {
                MessageBox.Show("Contact gender must be specified.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            contact.SetContactGender(contactGenderComboBox.SelectedItem.ToString());
            return true;
        }

        private bool CheckCompanyComboBox()
        {
            if (companyNameComboBox.SelectedItem == null)
            {
                MessageBox.Show("Company must be specified.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            contact.SetCompanyName(company);

            return true; 
        } 
        
        private bool CheckCompanyBranchComboBox()
        {
            if (companyBranchComboBox.SelectedItem == null)
            {
                MessageBox.Show("Company branch must be specified.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true; 
        }
        
        private bool CheckDepartmentComboBox()
        {
            if (employeeDepartmentComboBox.SelectedItem == null)
            {
                MessageBox.Show("Employee Department must be specified.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            
            QueryGetMaxContactID();

            contact.SetAddressSerial(int.Parse(address_serial));
            contact.SetSalesPerson(loggedInUser);
            contact.SetContactId(contactID);

            QueryAddContactInfo();
            
            if (personalEmail != "")
            {
                QueryGetMaxPersonalEmailID();
                QueryAddContactPersonalEmail();
            }

            QueryGetMaxContactMobileID();
            QueryAddContactMobile(businessPhone);

            if (personalPhone != "")
            {
                QueryGetMaxContactMobileID();
                QueryAddContactMobile(personalPhone);
            }

            MessageBox.Show("Contact Added Successfully");
            this.Hide();
            ContactsPage contactsPage = new ContactsPage(ref loggedInUser);
            COMPANY_ORGANISATION_MACROS.LIST_CONTACT_STRUCT tmpCompanyStruct;
            tmpCompanyStruct.contact_id = contact.GetContactId();
            tmpCompanyStruct.company_serial = companies[companyNameComboBox.SelectedIndex].company_serial;
            tmpCompanyStruct.address_serial = int.Parse(address_serial);
            tmpCompanyStruct.address = address;
            tmpCompanyStruct.contact_name = contact.GetContactName();
            tmpCompanyStruct.department = contact.GetContactDepartment();
            contactsPage.employeeContacts.Add(tmpCompanyStruct);
            contactsPage.InitializeCompaniesTree();
        }

        private bool QueryAddContactInfo()
        {
            string sqlQueryPart1 = @"insert into erp_system.dbo.contact_person_info values(";
            string sqlQueryPart2 = ", ";
            string sqlQueryPart3 = "getdate()) ;";

            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += employeeID;
            sqlQuery += sqlQueryPart2;
            sqlQuery += contact.GetAddressSerial();
            sqlQuery += sqlQueryPart2;
            sqlQuery += contact.GetContactId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += "'" + contact.GetContactBusinessEmail() + "'";
            sqlQuery += sqlQueryPart2;
            sqlQuery += "'" + contact.GetContactName() + "'";
            sqlQuery += sqlQueryPart2;
            sqlQuery += "'" + contact.GetContactGender() + "'";
            sqlQuery += sqlQueryPart2;
            sqlQuery += contact.GetContactDepartmentId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += 1;
            sqlQuery += sqlQueryPart2;
            sqlQuery += sqlQueryPart3;

            if (!sqlServer.InsertRows(sqlQuery))
                return false;

            return true;
        }

        private bool QueryAddContactPersonalEmail()
        {
            string sqlQueryPart1 = @"insert into erp_system.dbo.contact_person_personal_emails values(";
            string sqlQueryPart2 = ", ";
            string sqlQueryPart3 = "getdate()) ;";

            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += employeeID;
            sqlQuery += sqlQueryPart2;
            sqlQuery += contact.GetAddressSerial();
            sqlQuery += sqlQueryPart2;
            sqlQuery += contact.GetContactId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += personalEmailID;
            sqlQuery += sqlQueryPart2;
            sqlQuery += "'" + personalEmail + "'"; 
            sqlQuery += sqlQueryPart2;
            sqlQuery += sqlQueryPart3;

            if (!sqlServer.InsertRows(sqlQuery))
                return false;

            return true;
        }
        
        private bool QueryAddContactMobile(string telephone)
        {
            string sqlQueryPart1 = @"insert into erp_system.dbo.contact_person_mobile values(";
            string sqlQueryPart2 = ", ";
            string sqlQueryPart3 = "getdate()) ;";

            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += employeeID;
            sqlQuery += sqlQueryPart2;
            sqlQuery += contact.GetAddressSerial();
            sqlQuery += sqlQueryPart2;
            sqlQuery += contact.GetContactId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += mobileID;
            sqlQuery += sqlQueryPart2;
            sqlQuery += "'" + telephone + "'";
            sqlQuery += sqlQueryPart2;
            sqlQuery += sqlQueryPart3;

            if (!sqlServer.InsertRows(sqlQuery))
                return false;


            return true;
        } 
        

        private bool QueryGetMaxContactID()
        {
            string sqlQueryPart1 = "select max(contact_id) from erp_system.dbo.contact_person_info where branch_serial = ";
            string sqlQueryPart2 = " and sales_person_id = ";
            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += int.Parse(address_serial);
            sqlQuery += sqlQueryPart2;
            sqlQuery += employeeID;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 0;

            if (!sqlServer.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            contactID = 1 + sqlServer.rows[0].sql_int[0];

            return true;
        } 
        private bool QueryGetMaxPersonalEmailID()
        {
            string sqlQueryPart1 = "select max(email_id) from erp_system.dbo.contact_person_personal_emails where branch_serial = ";
            string sqlQueryPart2 = " and sales_person_id = ";
            string sqlQueryPart3 = " and contact_id =  ";

            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += int.Parse(address_serial);
            sqlQuery += sqlQueryPart2;
            sqlQuery += employeeID;
            sqlQuery += sqlQueryPart3;
            sqlQuery += contactID;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 0;

            if (!sqlServer.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_LOW))
                personalEmailID = 1;

            else
                personalEmailID = 1 + sqlServer.rows[0].sql_int[0];

            return true;
        }
        
        private bool QueryGetMaxContactMobileID()
        {
            string sqlQueryPart1 = "select max(telephone_id) from erp_system.dbo.contact_person_mobile where branch_serial = ";
            string sqlQueryPart2 = " and sales_person_id = ";
            string sqlQueryPart3 = " and contact_id =  ";

            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += int.Parse(address_serial);
            sqlQuery += sqlQueryPart2;
            sqlQuery += employeeID;
            sqlQuery += sqlQueryPart3;
            sqlQuery += contactID;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 0;

            if (!sqlServer.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_LOW))
                mobileID = 1;

            else
                mobileID = 1 + sqlServer.rows[0].sql_int[0];

            return true;
        }

        private void OnSelChangedGender(object sender, SelectionChangedEventArgs e)
        {

        }

        

    
    }
}
