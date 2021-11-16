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
        private bool InitializeCities()
        {
            cityComboBox.Items.Clear();

            if (!commonQueries.GetAllStateCities(states[stateComboBox.SelectedIndex].state_id, ref cities))
                return false;

            for (int i = 0; i < cities.Count; i++)
            {
                if (stateComboBox.SelectedItem != null && cities[i].city_id / 100 == states[stateComboBox.SelectedIndex].state_id)
                    cityComboBox.Items.Add(cities[i].city_name);
            }

            return true;
        }
        private bool InitializeDistricts()
        {
            districtComboBox.Items.Clear();

            if (!commonQueries.GetAllCityDistricts(cities[cityComboBox.SelectedIndex].city_id, ref districts))
                return false;

            for (int i = 0; i < districts.Count; i++)
            {
                if (cityComboBox.SelectedItem != null && districts[i].district_id / 100 == cities[cityComboBox.SelectedIndex].city_id)
                    districtComboBox.Items.Add(districts[i].district_name);
            }

            return true;
        }
        private void OnSelChangedCountry(object sender, SelectionChangedEventArgs e)
        {
            if (countryComboBox.SelectedItem != null)
            {
                if (!commonQueries.GetAllCountryStates(countries[countryComboBox.SelectedIndex].country_id, ref states))
                    return;

                stateComboBox.IsEnabled = true;
                stateComboBox.Items.Clear();
                for (int i = 0; i < states.Count(); i++)
                {
                    if (countryComboBox.SelectedItem != null && states[i].state_id / 100 == countries[countryComboBox.SelectedIndex].country_id)
                        stateComboBox.Items.Add(states[i].state_name);
                }
            }
            else
            {
                stateComboBox.SelectedItem = null;
                stateComboBox.IsEnabled = false;
            }
        }

        private void OnSelChangedState(object sender, SelectionChangedEventArgs e)
        {
            if (stateComboBox.SelectedItem != null)
            {
                InitializeCities();
                cityComboBox.IsEnabled = true;
                cityComboBox.IsEditable = true;
            }
            else
            {
                cityComboBox.SelectedItem = null;
                cityComboBox.IsEnabled = false;
                cityComboBox.IsEditable = false;
            }
        }
        private void OnSelChangedCity(object sender, SelectionChangedEventArgs e)
        {
            if (cityComboBox.SelectedItem != null)
            {
                InitializeDistricts();
                districtComboBox.IsEnabled = true;
                districtComboBox.IsEditable = true;
            }
            else
            {
                districtComboBox.IsEnabled = false;
                districtComboBox.SelectedItem = null;
                districtComboBox.IsEditable = false;
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

            company.SetOwnerUser(loggedInUser.GetEmployeeId(), loggedInUser.GetEmployeeName());

            if (telephoneTextBox.Text != "" && faxTextBox.Text != "")
            {
                if (!company.IssueNewBranch(telephoneTextBox.Text, faxTextBox.Text))
                    return;
            }
            else
            {
                if (!company.GetNewAddressSerial())
                    return;

                if (!company.InsertIntoCompanyAddress())
                    return;

                if(telephoneTextBox.Text != "")
                {
                    if (!company.InsertIntoCompanyTelephone(telephoneTextBox.Text))
                        return;
                }

                if (faxTextBox.Text != "")
                {
                    if (!company.InsertIntoCompanyFax(faxTextBox.Text))
                        return;
                }
            }
            //company.GetNewAddressSerial();

            //if (!InsertIntoCompanyAddress())
            //    return;

            //if (!InsertIntoCompanyTelephone(telephoneTextBox.Text))
            //    return;

            // if (!InsertIntoCompanyFax(faxTextBox.Text))
            //    return;

            this.Close();

        }
        private bool CheckCompanyPhoneEditBox()
        {
            String inputString = telephoneTextBox.Text;
            String outputString = telephoneTextBox.Text;

            if (!integrityChecker.CheckCompanyPhoneEditBox(inputString, ref outputString, true))
                return false;

            company.AddCompanyPhone(outputString);
            telephoneTextBox.Text = outputString;

            return true;
        }
        private bool CheckCompanyFaxEditBox()
        {
            String inputString = faxTextBox.Text;
            String outputString = faxTextBox.Text;

            if (!integrityChecker.CheckCompanyFaxEditBox(inputString, ref outputString, false))
                return false;

            company.AddCompanyFax(outputString);
            faxTextBox.Text = outputString;

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
        private void OnClickCityImage(object sender, MouseButtonEventArgs e)
        {
            if (cityComboBox.Text.ToString() != "")
            {
                if (!cities.Exists(cityItem => cityItem.city_name == cityComboBox.Text.ToString()))
                {
                    int cityID = 0;

                    if (stateComboBox.SelectedItem == null || !commonQueries.GetMaxCityId(states[stateComboBox.SelectedIndex].state_id, ref cityID))
                        return;

                    if (!commonQueries.InsertNewCity(states[stateComboBox.SelectedIndex].state_id, cityID, cityComboBox.Text.ToString()))
                        return;

                    String tmp = cityComboBox.Text.ToString();

                    if (!InitializeCities())
                        return;

                    cityComboBox.SelectedItem = tmp;
                }
            }
        }

        private void OnClickDistrictImage(object sender, MouseButtonEventArgs e)
        {
            if (districtComboBox.Text.ToString() != "")
            {
                if (!districts.Exists(districtItem => districtItem.district_name == districtComboBox.Text.ToString()))
                {
                    int districtID = 0;

                    if (cityComboBox.SelectedItem == null || !commonQueries.GetMaxDistrictId(cities[cityComboBox.SelectedIndex].city_id, ref districtID))
                        return;

                    if (!commonQueries.InsertNewDistrict(cities[cityComboBox.SelectedIndex].city_id, districtID, districtComboBox.Text.ToString()))
                        return;

                    String tmp = districtComboBox.Text.ToString();

                    if (!InitializeDistricts())
                        return;

                    districtComboBox.SelectedItem = tmp;
                }
            }
        }
    }
}
