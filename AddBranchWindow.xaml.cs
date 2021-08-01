using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
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
    /// Interaction logic for AddBranchWindow.xaml
    /// </summary>
    public partial class AddBranchWindow : Window
    {
        protected CommonQueries commonQueries;
        protected SQLServer sqlServer;
        protected IntegrityChecks integrityChecker;

        protected Company company;
        protected Employee loggedInUser;

        protected List<BASIC_STRUCTS.COUNTRY_STRUCT> countries;
        protected List<BASIC_STRUCTS.STATE_STRUCT> states;
        protected List<BASIC_STRUCTS.CITY_STRUCT> cities;
        protected List<BASIC_STRUCTS.DISTRICT_STRUCT> districts;

        protected String sqlQuery;
        public AddBranchWindow(ref Employee mloggedInUser, ref Company mCompany)
        {
            InitializeComponent();

            commonQueries = new CommonQueries();

            countries = new List<BASIC_STRUCTS.COUNTRY_STRUCT>();
            states = new List<BASIC_STRUCTS.STATE_STRUCT>();
            cities = new List<BASIC_STRUCTS.CITY_STRUCT>();
            districts = new List<BASIC_STRUCTS.DISTRICT_STRUCT>();

            sqlServer = new SQLServer();
            integrityChecker = new IntegrityChecks();

            loggedInUser = mloggedInUser;
            company = mCompany;

            commonQueries.GetAllCountries(ref countries);

            for (int i = 0; i < countries.Count; i++)
                countryComboBox.Items.Add(countries[i].country_name);

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
                if (countryComboBox.SelectedItem != null && states[i].state_id / BASIC_MACROS.MAXIMUM_STATES_NO == countries[countryComboBox.SelectedIndex].country_id)
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
                if (stateComboBox.SelectedItem != null && cities[i].city_id / BASIC_MACROS.MAXIMUM_CITIES_NO == states[stateComboBox.SelectedIndex].state_id)
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
                if (cityComboBox.SelectedItem != null && districts[i].district_id / BASIC_MACROS.MAXIMUM_DISTRICTS_NO == cities[cityComboBox.SelectedIndex].city_id)
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

            //YOU DON'T NEED TO WRITE A NEW QUERY GET NEW BRANCH SERIAL
            //COMPANY CLASS HANDLES ALL THAT
            company.GetNewAddressSerial();

            InsertIntoCompanyAddress();

            this.Hide();

        }
        private bool CheckCompanyPhoneEditBox()
        {
            String inputString = telephoneTextBox.Text;
            String outputString = telephoneTextBox.Text;

            if (!integrityChecker.CheckCompanyPhoneEditBox(inputString, ref outputString, false))
                return false;

            company.AddCompanyPhone(outputString);
            telephoneTextBox.Text = company.GetCompanyPhones()[0];

            return true;
        }
        private bool CheckCompanyFaxEditBox()
        {
            String inputString = faxTextBox.Text;
            String outputString = faxTextBox.Text;

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
                System.Windows.Forms.MessageBox.Show("Country must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            company.SetCompanyCountry(countries[countryComboBox.SelectedIndex].country_id, countryComboBox.SelectedItem.ToString());

            return true;
        }

        private bool CheckStateComboBox()
        {
            if (stateComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("State must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            company.SetCompanyState(states[stateComboBox.SelectedIndex].state_id, stateComboBox.SelectedItem.ToString());

            return true;
        }

        private bool CheckCityComboBox()
        {
            if (cityComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("City must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            company.SetCompanyCity(cities[cityComboBox.SelectedIndex].city_id, cityComboBox.SelectedItem.ToString());

            return true;
        }

        private bool CheckDistrictComboBox()
        {
            if (districtComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("District must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


            company.SetCompanyDistrict(districts[districtComboBox.SelectedIndex].district_id, districtComboBox.SelectedItem.ToString());

            return true;
        }
        private bool InsertIntoCompanyAddress()
        {
            String sqlQueryPart1 = @"insert into erp_system.dbo.company_address values(";
            String sqlQueryPart2 = ", ";
            String sqlQueryPart3 = "getdate()) ;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += company.GetAddressSerial();
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
