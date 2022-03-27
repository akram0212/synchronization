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
        public AddCompanyDetailsWindow(ref Employee mLoggedInUser, ref Company mCompany,String mSelectedBranch)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;
            company = mCompany;
            selectedBranch = mSelectedBranch;

            integrityChecker = new IntegrityChecks();

            BranchTextBox.Text = selectedBranch;
            BranchTextBox.IsEnabled = false;
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

            if (!integrityChecker.CheckCompanyPhoneEditBox(inputString, ref outputString, false))
                return false;

            company.AddCompanyPhone(outputString);
           // company.GetNumberOfSavedCompanyPhones();
            telephoneTextBox.Text = company.GetCompanyPhones()[company.GetNumberOfSavedCompanyPhones() - 1];

            return true;
        }

        private bool CheckCompanyFaxEditBox()
        {
            String inputString = faxTextBox.Text;
            String outputString = faxTextBox.Text;

            if (!integrityChecker.CheckCompanyFaxEditBox(inputString, ref outputString, false))
                return false;

            company.AddCompanyFax(outputString);
            faxTextBox.Text = company.GetCompanyFaxes()[company.GetNumberOfSavedCompanyFaxes() - 1];

            return true;
        }
        
    }
}
