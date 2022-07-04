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

        private List<BASIC_STRUCTS.COUNTRY_CODES_STRUCT> countryCodes;
        public AddContactDetailsWindow(ref Employee mLoggedInUser, ref Contact mContact)
        {
            InitializeComponent();

            commonQueries = new CommonQueries();

            loggedInUser = mLoggedInUser;
            contact = mContact;

            integrityChecker = new IntegrityChecks();

            countryCodes = new List<BASIC_STRUCTS.COUNTRY_CODES_STRUCT>();

            InitializeCountryCodeCombo();
        }

        private bool InitializeCountryCodeCombo()
        {
            if (!commonQueries.GetCountryCodes(ref countryCodes))
                return false;

            countryCodeCombo.Items.Clear();

            for(int i = 0; i < countryCodes.Count; i++)
            {
                String temp = countryCodes[i].iso3 + "   " + countryCodes[i].phone_code;
                countryCodeCombo.Items.Add(temp);
            }

            countryCodeCombo.SelectedIndex = countryCodes.FindIndex(x1=> x1.country_id == contact.GetAddress() / 1000000);

            return true;
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

            if (!integrityChecker.CheckContactPhoneEditBox(inputString, ref outputString, true, ref errorMessage))
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
