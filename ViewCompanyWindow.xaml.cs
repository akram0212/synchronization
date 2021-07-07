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
        public ViewCompanyWindow(ref Employee mLoggedInUser, int companySerial)
        {
            InitializeComponent();

            company = new Company();
            commonQueries = new CommonQueries();
            loggedInUser = new Employee();
            branchesList = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();


            company.InitializeCompanyInfo(companySerial);
            commonQueries.GetCompanyAddresses(company.GetCompanySerial(), ref branchesList);
            loggedInUser = mLoggedInUser;

            companyNameTextBox.IsEnabled = false;
            primaryWorkFieldTextBox.IsEnabled = false;
            secondaryWorkFieldTextBox.IsEnabled = false;

            companyNameTextBox.Text = company.GetCompanyName();
            primaryWorkFieldTextBox.Text = company.GetCompanyPrimaryField();
            secondaryWorkFieldTextBox.Text = company.GetCompanySecondaryField();

            for(int i = 0; i < branchesList.Count; i++)
                branchComboBox.Items.Add(branchesList[i].country + ",\t" + branchesList[i].state_governorate + ",\t" + branchesList[i].city + ",\t" + branchesList[i].district);
            
            company.QueryCompanyPhones();
            company.QueryCompanyFaxes();

            phonesCount = company.GetNumberOfSavedCompanyPhones();
            faxesCount = company.GetNumberOfSavedCompanyFaxes();

            if (phonesCount != 0)
                AddPhone();
            if (faxesCount != 0)
                AddFax();
        }
        private void AddPhone()
        {
            WrapPanel PhoneWrapPanel = new WrapPanel();

            Label PhoneLabel = new Label();
            PhoneLabel.Style = (Style)FindResource("tableItemLabel");
            PhoneLabel.Content = "Telephone";

            TextBox telephoneTextBox = new TextBox();
            telephoneTextBox.IsEnabled = false;
            telephoneTextBox.Style = (Style)FindResource("textBoxStyle");
            telephoneTextBox.Text = company.GetCompanyPhones()[0];

            PhoneWrapPanel.Children.Add(PhoneLabel);
            PhoneWrapPanel.Children.Add(telephoneTextBox);
            companyPhonesWrapPanel.Children.Add(PhoneWrapPanel);

        }
        
        private void AddFax()
        {
            WrapPanel FaxWrapPanel = new WrapPanel();

            Label FaxLabel = new Label();
            FaxLabel.Style = (Style)FindResource("tableItemLabel");
            FaxLabel.Content = "Fax";

            TextBox FaxTextBox = new TextBox();
            FaxTextBox.IsEnabled = false;
            FaxTextBox.Style = (Style)FindResource("textBoxStyle");
            FaxTextBox.Text = company.GetCompanyFaxes()[0];

            FaxWrapPanel.Children.Add(FaxLabel);
            FaxWrapPanel.Children.Add(FaxTextBox);
            companyPhonesWrapPanel.Children.Add(FaxWrapPanel);

        }

        private void OnTextChangedCompanyName(object sender, TextChangedEventArgs e)
        {

        }

        private void OnSelChangedBranch(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnBtnClkAddBranch(object sender, RoutedEventArgs e)
        {
            AddBranchWindow addBranchWindow = new AddBranchWindow(ref loggedInUser, ref company);
            addBranchWindow.Show();
        }
    }
}
