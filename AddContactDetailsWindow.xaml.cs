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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for AddContactDetailsWindow.xaml
    /// </summary>
    public partial class AddContactDetailsWindow : Window
    {
        protected Employee loggedInUser;

        protected CommonQueries commonQueries;
        protected IntegrityChecks integrityChecker;

        protected Contact contact;

        protected int phonesCount;
        protected int emailsCount;

        protected String errorMessage;
        public AddContactDetailsWindow(ref Employee mLoggedInUser, ref Contact mContact)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;
            contact = mContact;

            integrityChecker = new IntegrityChecks();
        }

        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {
            if (!CheckContactPhoneEditBox())
                return;

            if (!CheckContactEmailEditBox())
                return;

            if (telephoneTextBox.Text != String.Empty)
            {
                contact.AddNewContactPhone(telephoneTextBox.Text.ToString());
                contact.InsertIntoContactMobile(contact.GetNumberOfSavedContactPhones(), telephoneTextBox.Text.ToString());
            }

            if (emailTextBox.Text != String.Empty)
            {
                contact.AddNewContactEmail(emailTextBox.Text.ToString());
                contact.InsertIntoContactPersonalEmail(contact.GetNumberOfSavedContactEmails(), emailTextBox.Text.ToString());
            }

            this.Close();
        }

        private void OnTextChangedEmail(object sender, TextChangedEventArgs e)
        {

        }

        private void OnTextChangedTelephone(object sender, TextChangedEventArgs e)
        {

        }
        private bool CheckContactPhoneEditBox()
        {
            String inputString = telephoneTextBox.Text;
            String outputString = telephoneTextBox.Text;

            if (!integrityChecker.CheckCompanyPhoneEditBox(inputString, ref outputString, false, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            //contact.AddCompanyPhone(outputString);
            // contact.GetNumberOfSavedCompanyPhones();
            telephoneTextBox.Text = outputString;

            return true;
        }

        private bool CheckContactEmailEditBox()
        {
            String inputString = emailTextBox.Text;
            String outputString = emailTextBox.Text;

            if (!integrityChecker.CheckEmployeePersonalEmailEditBox(inputString, ref outputString, false, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            //contact.AddNewContactEmail(outputString);
            emailTextBox.Text = outputString;

            return true;
        }
    }
}
