using _01electronics_erp;
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
    /// Interaction logic for AddBranchWindow.xaml
    /// </summary>
    public partial class AddBranchWindow : Window
    {
        protected CommonQueries commonQueries;
        protected SQLServer sqlServer;
        protected IntegrityChecks integrityChecker;
        Company company;
        Employee loggedInUser;
        List<BASIC_STRUCTS.COUNTRY_STRUCT> countries;
        List<BASIC_STRUCTS.STATE_STRUCT> states;
        List<BASIC_STRUCTS.CITY_STRUCT> cities;
        List<BASIC_STRUCTS.DISTRICT_STRUCT> districts;
        protected string sqlQuery;
        public AddBranchWindow(ref Employee mloggedInUser, ref Company companyInfo)
        {
            InitializeComponent();

            commonQueries = new CommonQueries();
            countries = new List<BASIC_STRUCTS.COUNTRY_STRUCT>();
            states = new List<BASIC_STRUCTS.STATE_STRUCT>();
            cities = new List<BASIC_STRUCTS.CITY_STRUCT>();
            districts = new List<BASIC_STRUCTS.DISTRICT_STRUCT>();
            company = new Company();
            loggedInUser = new Employee();
            sqlServer = new SQLServer();
            integrityChecker = new IntegrityChecks();

            loggedInUser = mloggedInUser;
            this.company = companyInfo;

            commonQueries.GetAllCountries(ref countries);
            for (int i = 0; i < countries.Count; i++)
            {
                countryComboBox.Items.Add(countries[i].country_name);
            }

            companyNameTextBox.IsEnabled = false;
            primaryWorkFieldTextBox.IsEnabled = false;
            secondaryWorkFieldTextBox.IsEnabled = false;

            companyNameTextBox.Text = company.GetCompanyName();
            primaryWorkFieldTextBox.Text = company.GetCompanyPrimaryField();
            secondaryWorkFieldTextBox.Text = company.GetCompanySecondaryField();

            stateComboBox.IsEnabled = false;
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;
        }

        private void OnSelChangedCountry(object sender, SelectionChangedEventArgs e)
        {
            stateComboBox.IsEnabled = true;
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;

            if (countryComboBox.SelectedItem != null)
                commonQueries.GetAllCountryStates(countries[countryComboBox.SelectedIndex].country_id, ref states);
            stateComboBox.Items.Clear();
            for (int i = 0; i < states.Count(); i++)
            {
                if (countryComboBox.SelectedItem != null && states[i].state_id / 100 == countries[countryComboBox.SelectedIndex].country_id)
                    stateComboBox.Items.Add(states[i].state_name);
            }
        }

        private void OnSelChangedState(object sender, SelectionChangedEventArgs e)
        {
            cityComboBox.IsEnabled = true;
            districtComboBox.IsEnabled = false;

            if (stateComboBox.SelectedItem != null)
                commonQueries.GetAllStateCities(states[stateComboBox.SelectedIndex].state_id, ref cities);
            cityComboBox.Items.Clear();

            for (int i = 0; i < cities.Count; i++)
            {
                if (stateComboBox.SelectedItem != null && cities[i].city_id / 100 == states[stateComboBox.SelectedIndex].state_id)
                    cityComboBox.Items.Add(cities[i].city_name);
            }
        }

        private void OnSelChangedCity(object sender, SelectionChangedEventArgs e)
        {
            districtComboBox.IsEnabled = true;

            if (cityComboBox.SelectedItem != null)
                commonQueries.GetAllCityDistricts(cities[cityComboBox.SelectedIndex].city_id, ref districts);
            districtComboBox.Items.Clear();

            for (int i = 0; i < districts.Count; i++)
            {
                if (cityComboBox.SelectedItem != null && districts[i].district_id / 100 == cities[cityComboBox.SelectedIndex].city_id)
                    districtComboBox.Items.Add(districts[i].district_name);
            }
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
            if (!CheckCountryComboBox())
                return;
            if (!CheckStateComboBox())
                return;
            if (!CheckCityComboBox())
                return;
            if (!CheckDistrictComboBox())
                return;
            if (!CheckCompanyPhoneEditBox())
                return;
            if (!CheckCompanyFaxEditBox())
                return;

            AddCompanyAddress();

            MessageBox.Show("Branch Added Successfully");
            this.Hide();

        }
        private bool CheckCompanyPhoneEditBox()
        {
            string inputString = telephoneTextBox.Text;
            string outputString = telephoneTextBox.Text;

            if (!integrityChecker.CheckCompanyPhoneEditBox(inputString, ref outputString, false))
                return false;

            company.AddCompanyPhone(outputString);
            telephoneTextBox.Text = company.GetCompanyPhones()[0];

            return true;
        }
        private bool CheckCompanyFaxEditBox()
        {
            string inputString = faxTextBox.Text;
            string outputString = faxTextBox.Text;

            if (!integrityChecker.CheckCompanyFaxEditBox(inputString, ref outputString, false))
                return false;

            company.AddCompanyFax(outputString);
            faxTextBox.Text = company.GetCompanyFaxes()[0];

            return true;
        }
        private bool CheckCountryComboBox()
        {
            if (countryComboBox.SelectedItem == null)
            {
                MessageBox.Show("Country must be specified.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            company.SetCompanyCountry(countries[countryComboBox.SelectedIndex].country_id, countryComboBox.SelectedItem.ToString());

            return true;
        }

        private bool CheckStateComboBox()
        {
            if (stateComboBox.SelectedItem == null)
            {
                MessageBox.Show("State must be specified.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            company.SetCompanyState(states[stateComboBox.SelectedIndex].state_id, stateComboBox.SelectedItem.ToString());

            return true;
        }

        private bool CheckCityComboBox()
        {
            if (cityComboBox.SelectedItem == null)
            {
                MessageBox.Show("City must be specified.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            company.SetCompanyCity(cities[cityComboBox.SelectedIndex].city_id, cityComboBox.SelectedItem.ToString());

            return true;
        }

        private bool CheckDistrictComboBox()
        {
            if (districtComboBox.SelectedItem == null)
            {
                MessageBox.Show("District must be specified.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }


            company.SetCompanyDistrict(districts[districtComboBox.SelectedIndex].district_id, districtComboBox.SelectedItem.ToString());

            return true;
        }
        private bool AddCompanyAddress()
        {
            string sqlQueryPart1 = @"insert into erp_system.dbo.company_address values(";
            string sqlQueryPart2 = ", ";
            string sqlQueryPart3 = "getdate()) ;";

            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += company.GetAddressSerial()+1;
            sqlQuery += sqlQueryPart2;
            sqlQuery += company.GetCompanySerial();
            sqlQuery += sqlQueryPart2;
            sqlQuery += districts[districtComboBox.SelectedIndex].district_id;
            sqlQuery += sqlQueryPart2;
            sqlQuery += loggedInUser.GetEmployeeId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += sqlQueryPart3;

            if (!sqlServer.InsertRows(sqlQuery))
                return false;

            return true;
        }
    }
}
