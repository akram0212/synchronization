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
using _01electronics_library;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for ViewCompanyWindow.xaml
    /// </summary>
    public partial class ViewCompanyWindow : Window
    {
        protected Employee loggedInUser;

        protected CommonQueries commonQueries;

        protected Company company;

        protected int phonesCount;
        protected int faxesCount;

        protected List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> branchesList;
        public ViewCompanyWindow(ref Employee mLoggedInUser, ref Company mCompany)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;
            company = mCompany;

            commonQueries = new CommonQueries();
            branchesList = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();

            companyNameTextBox.IsEnabled = false;
            primaryWorkFieldTextBox.IsEnabled = false;
            secondaryWorkFieldTextBox.IsEnabled = false;

            companyNameTextBox.Text = company.GetCompanyName();
            primaryWorkFieldTextBox.Text = company.GetCompanyPrimaryField();
            secondaryWorkFieldTextBox.Text = company.GetCompanySecondaryField();

            if (!InitializeBranchesComboBox())
                return;

        }
        private void InitializeCompanyInfo()
        {
            companyNameTextBox.IsEnabled = false;
            primaryWorkFieldTextBox.IsEnabled = false;
            secondaryWorkFieldTextBox.IsEnabled = false;

            companyNameTextBox.Text = company.GetCompanyName();
            primaryWorkFieldTextBox.Text = company.GetCompanyPrimaryField();
            secondaryWorkFieldTextBox.Text = company.GetCompanySecondaryField();

            if (!InitializeBranchesComboBox())
                return;

            if (!InitializeBranchPhones())
                return;
        }
        private bool InitializeBranchesComboBox()
        {
            branchComboBox.Items.Clear();

            if (!commonQueries.GetCompanyAddresses(company.GetCompanySerial(), ref branchesList))
                return false;

            for (int i = 0; i < branchesList.Count; i++)
                branchComboBox.Items.Add(branchesList[i].district + ", " + branchesList[i].city + ", " + branchesList[i].state_governorate + ", " + branchesList[i].country);

            branchComboBox.SelectedIndex = 0;

            return true;
        }
        private bool InitializeBranchPhones()
        {
            company.SetAddressSerial(branchesList[branchComboBox.SelectedIndex].address_serial);

            if (!company.QueryCompanyPhones())
                return false;

            AddPhone();

            return true;
        }
        private bool InitializeBranchFaxes()
        {
            company.SetAddressSerial(branchesList[branchComboBox.SelectedIndex].address_serial);

            if (!company.QueryCompanyFaxes())
                return false;

            AddFax();

            return true;
        }
        private void AddPhone()
        {
            for (int i = 0; i < company.GetCompanyPhones().Count(); i++)
            {
                if (company.GetCompanyPhones()[i] != String.Empty)
                {
                    WrapPanel PhoneWrapPanel = new WrapPanel();

                    Label PhoneLabel = new Label();
                    PhoneLabel.Style = (Style)FindResource("tableItemLabel");
                    PhoneLabel.Content = "Telephone";

                    TextBox telephoneTextBox = new TextBox();
                    telephoneTextBox.IsEnabled = false;
                    telephoneTextBox.Style = (Style)FindResource("textBoxStyle");
                    telephoneTextBox.Text = company.GetCompanyPhones()[i];

                    PhoneWrapPanel.Children.Add(PhoneLabel);
                    PhoneWrapPanel.Children.Add(telephoneTextBox);
                    companyPhonesWrapPanel.Children.Add(PhoneWrapPanel);
                }
            }
        }

        private void AddFax()
        {
            for (int i = 0; i < company.GetCompanyFaxes().Count(); i++)
            {
                if (company.GetCompanyFaxes()[i] != String.Empty)
                {
                WrapPanel FaxWrapPanel = new WrapPanel();

                Label FaxLabel = new Label();
                FaxLabel.Style = (Style)FindResource("tableItemLabel");
                FaxLabel.Content = "Fax";

                TextBox FaxTextBox = new TextBox();
                FaxTextBox.IsEnabled = false;
                FaxTextBox.Style = (Style)FindResource("textBoxStyle");
                FaxTextBox.Text = company.GetCompanyFaxes()[i];

                FaxWrapPanel.Children.Add(FaxLabel);
                FaxWrapPanel.Children.Add(FaxTextBox);
                companyPhonesWrapPanel.Children.Add(FaxWrapPanel);
                }
            }
        }

        private void OnTextChangedCompanyName(object sender, TextChangedEventArgs e)
        {

        }

        private void OnSelChangedBranch(object sender, SelectionChangedEventArgs e)
        {
            companyPhonesWrapPanel.Children.Clear();

            if (branchComboBox.SelectedItem != null)
            {
                InitializeBranchPhones();
                InitializeBranchFaxes();

            }
        }

        private void OnBtnClkAddBranch(object sender, RoutedEventArgs e)
        {
            AddBranchWindow addBranchWindow = new AddBranchWindow(ref loggedInUser, ref company);
            addBranchWindow.Closed += OnClosedAddBranchWindow;
            addBranchWindow.Show();
        }
        private void OnClosedAddBranchWindow(object sender, EventArgs e)
        {
            InitializeBranchesComboBox();
        }

        private void OnBtnClkAddDetails(object sender, RoutedEventArgs e)
        {
            AddComapnyDetailsWindow addComapnyDetailsWindow = new AddComapnyDetailsWindow(ref loggedInUser, ref company, branchComboBox.SelectedItem.ToString());
            addComapnyDetailsWindow.Closed += OnClosedAddBranchWindow;
            addComapnyDetailsWindow.Show();
        }
        private void OnClosedAddCompanyDetailsWindow(object sender, EventArgs e)
        {
            InitializeCompanyInfo();
        }
    }
}
