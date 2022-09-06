using _01electronics_library;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace _01electronics_crm
{

    /// <summary>
    /// Interaction logic for SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        private SQLServer sqlServer = new SQLServer();
        private IntegrityChecks integrityChecker = new IntegrityChecks();
        private CommonQueries commonQueries = new CommonQueries();

        private Employee signupEmployee = new Employee();

        String employeePassword;
        String confirmPassword;
        String employeeHashedPassword;

        protected String errorMessage;
        public SignUpPage()
        {
            InitializeComponent();
        }

        bool CheckemployeeBusinessEmailEdit()
        {
            String inputString = businessEmailTextBox.Text;
            String modifiedString = null;

            if (!integrityChecker.CheckEmployeeSignUpEmailEditBox(inputString, ref modifiedString, false, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (!signupEmployee.InitializeEmployeeInfo(modifiedString))
                return false;

            businessEmailTextBox.Text = signupEmployee.GetEmployeeBusinessEmail();

            return true;
        }

        bool CheckEmployeePersonalEmailEdit()
        {
            String inputString = personalEmailTextBox.Text;
            String modifiedString = null;

            if (!integrityChecker.CheckEmployeePersonalEmailEditBox(inputString, ref modifiedString, false, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            signupEmployee.SetEmployeePersonalEmail(modifiedString);
            personalEmailTextBox.Text = signupEmployee.GetEmployeePersonalEmail();

            return true;
        }

        bool CheckEmployeePasswordEdits()
        {
            employeePassword = passwordTextBox.Password;
            confirmPassword = confirmPasswordTextBox.Password;

            if (employeePassword != confirmPassword)
                return false;

            SHA256 hashing = SHA256.Create();

            byte[] hashedBytes = hashing.ComputeHash(Encoding.UTF8.GetBytes(employeePassword));

            StringBuilder employeePasswordBuilder = new StringBuilder(hashedBytes.Length * 2);

            foreach (byte currentByte in hashedBytes)
                employeePasswordBuilder.AppendFormat("{0:x2}", currentByte);

            employeeHashedPassword = employeePasswordBuilder.ToString();

            return true;
        }
        private void OnButtonClickedSignUp(object sender, RoutedEventArgs e)
        {
            if (!CheckemployeeBusinessEmailEdit())
                return;
            if (!CheckEmployeePasswordEdits())
                return;
            if (!CheckEmployeePersonalEmailEdit())
                return;

            if (!InsertIntoEmployeesPasswords())
                return;
            if (!InsertIntoEmployeePersonalEmails())
                return;

            SignInPage signIn = new SignInPage();
            this.NavigationService.Navigate(signIn);
        }

        private bool InsertIntoEmployeesPasswords()
        {
            String sqlQueryPart1 = "insert into erp_system.dbo.employees_passwords values (";
            String sqlQueryPart2 = ",'";
            String sqlQueryPart3 = "', getdate());";


            String sqlQuery;
            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += signupEmployee.GetEmployeeId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += employeeHashedPassword;
            sqlQuery += sqlQueryPart3;

            if (!sqlServer.InsertRows(sqlQuery))
                return false;

            return true;
        }

        private bool InsertIntoEmployeePersonalEmails()
        {
            String sqlQueryPart1 = "insert into erp_system.dbo.employees_personal_emails values(";
            String sqlQueryPart2 = ",'";
            String sqlQueryPart3 = "', getdate());";

            String sqlQuery;
            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += signupEmployee.GetEmployeeId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += signupEmployee.GetEmployeePersonalEmail();
            sqlQuery += sqlQueryPart3;

            if (!sqlServer.InsertRows(sqlQuery))
                return false;

            return true;
        }
    }
}
