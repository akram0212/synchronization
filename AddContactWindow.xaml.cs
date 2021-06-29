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

        public AddContactWindow()
        {
            InitializeComponent();
        }

        public AddContactWindow(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;

            sqlServer = new SQLServer();
            commonQueries = new CommonQueries();
            commonFunctions = new CommonFunctions();
            integrityChecker = new IntegrityChecks();

            contact = new Contact();

            InitializeComponent();

        }

        private void OnTextChangedFirstName(object sender, TextChangedEventArgs e)
        {

        }

        private void OnTextChangedLastName(object sender, TextChangedEventArgs e)
        {

        }

        private void OnSelChangedCompany(object sender, SelectionChangedEventArgs e)
        {

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
        
        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {

        }
    }
}
