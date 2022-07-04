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
    /// Interaction logic for AddCompanyDetailsWindow.xaml
    /// </summary>
    public partial class AddCompanyDetailsWindow : Window
    {
        protected Employee loggedInUser;

        protected CommonQueries commonQueries;
        protected IntegrityChecks integrityChecker;

        protected Company company;
        protected String selectedBranch;

        protected int phonesCount;
        protected int faxesCount;

        protected String errorMessage;

        protected List<BASIC_STRUCTS.COUNTRY_CODES_STRUCT> countryCodes;
        protected List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> companyBranches;

        public AddCompanyDetailsWindow(ref Employee mLoggedInUser, ref Company mCompany,String mSelectedBranch)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;
            company = mCompany;
            selectedBranch = mSelectedBranch;

            integrityChecker = new IntegrityChecks();
            commonQueries = new CommonQueries();

            countryCodes = new List<BASIC_STRUCTS.COUNTRY_CODES_STRUCT>();
            companyBranches = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();

            BranchTextBox.Text = selectedBranch;
            BranchTextBox.IsEnabled = false;

            InitializeCountryCodeCombo();
        }

        public bool InitializeCountryCodeCombo()
        {
            if (!commonQueries.GetCountryCodes(ref countryCodes))
                return false;

            for(int i = 0; i < countryCodes.Count; i++)
            {
                String temp = countryCodes[i].iso3 + "   " + countryCodes[i].phone_code;
                
                countryCodeCombo.Items.Add(temp);
            }

            if (!commonQueries.GetCompanyAddresses(company.GetCompanySerial(), ref companyBranches))
                return false;

            if (companyBranches.Count > 0)
            {
                countryCodeCombo.SelectedIndex = countryCodes.FindIndex(x1 => x1.country_id == companyBranches[0].address / 1000000);
            }

            return true;
        }

        private void OnTextChangedFax(object sender, TextChangedEventArgs e)
        {

        }

        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {
            
            if (!CheckCompanyPhoneEditBox())
                return;
            if (!CheckCompanyFaxEditBox())
                return;

            if (telephoneTextBox.Text != String.Empty)
            {
                company.AddCompanyPhone(telephoneTextBox.Text.ToString());
                company.InsertIntoCompanyTelephone(telephoneTextBox.Text.ToString());
            }

            if (faxTextBox.Text != String.Empty)
            {
                company.AddCompanyFax(faxTextBox.Text.ToString());
                company.InsertIntoCompanyFax(faxTextBox.Text.ToString());
            }

            this.Close();
        }

        private void OnTextChangedTelephone(object sender, TextChangedEventArgs e)
        {

        }

        private void OnTextChangedBranch(object sender, TextChangedEventArgs e)
        {

        }
        private bool CheckCompanyPhoneEditBox()
        {
            String inputString = telephoneTextBox.Text;
            String outputString = telephoneTextBox.Text;

            if (!integrityChecker.CheckCompanyPhoneEditBox(inputString, ref outputString, companyBranches[0].address / 1000000, false, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            company.AddCompanyPhone(outputString);
           // company.GetNumberOfSavedCompanyPhones();
            telephoneTextBox.Text = company.GetCompanyPhones()[company.GetNumberOfSavedCompanyPhones() - 1];

            return true;
        }

        private bool CheckCompanyFaxEditBox()
        {
            String inputString = faxTextBox.Text;
            String outputString = faxTextBox.Text;

            if (!integrityChecker.CheckCompanyFaxEditBox(inputString, ref outputString, companyBranches[0].address / 1000000, false, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            company.AddCompanyFax(outputString);
            faxTextBox.Text = company.GetCompanyFaxes()[company.GetNumberOfSavedCompanyFaxes() - 1];

            return true;
        }
        
    }
}
