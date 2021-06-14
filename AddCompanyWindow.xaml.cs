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
    /// Interaction logic for AddCompanyWindow.xaml
    /// </summary>
    public partial class AddCompanyWindow : Window
    {
        protected String sqlQuery;

        public Employee loggedInUser;
        public Company company;

        protected SQLServer sqlServer;
        protected CommonQueries commonQueries;
        protected CommonFunctions commonFunctions;
        protected IntegrityChecks integrityChecker;
        public AddCompanyWindow()
        {
            InitializeComponent();
        }

        public AddCompanyWindow(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;

            sqlServer = new SQLServer();
            commonQueries = new CommonQueries();
            commonFunctions = new CommonFunctions();
            integrityChecker = new IntegrityChecks();

            company = new Company();

            InitializeComponent();

        }

        private void OnBtnClkAdvanced(object sender, RoutedEventArgs e)
        {

        }

        private void OnTextChangedCompanyName(object sender, TextChangedEventArgs e)
        {

        }

        private void OnSelChangedPrimaryWorkField(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnSelChangedSecondaryWorkField(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnSelChangedCountry(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnSelChangedState(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnSelChangedCity(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnSelChangedDistrict(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnTextChangedTelephone(object sender, TextChangedEventArgs e)
        {

        }

        private void OnTextChangedFax(object sender, TextChangedEventArgs e)
        {

        }

        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {

        }
    }
}
