using _01electronics_library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

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

        List<BASIC_STRUCTS.PRIMARY_FIELD_STRUCT> primaryWorkFields;
        List<BASIC_STRUCTS.SECONDARY_FIELD_STRUCT> secondaryWorkFields;

        List<BASIC_STRUCTS.COUNTRY_STRUCT> countries;
        List<BASIC_STRUCTS.STATE_STRUCT> states;
        List<BASIC_STRUCTS.CITY_STRUCT> cities;
        List<BASIC_STRUCTS.DISTRICT_STRUCT> districts;

        List<BASIC_STRUCTS.COUNTRY_CODES_STRUCT> countryCodes;

        protected String errorMessage;

        public AddCompanyWindow(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            sqlServer = new SQLServer();
            commonQueries = new CommonQueries();
            commonFunctions = new CommonFunctions();
            integrityChecker = new IntegrityChecks();

            company = new Company();

            primaryWorkFields = new List<BASIC_STRUCTS.PRIMARY_FIELD_STRUCT>();
            secondaryWorkFields = new List<BASIC_STRUCTS.SECONDARY_FIELD_STRUCT>();

            countries = new List<BASIC_STRUCTS.COUNTRY_STRUCT>();
            states = new List<BASIC_STRUCTS.STATE_STRUCT>();
            cities = new List<BASIC_STRUCTS.CITY_STRUCT>();
            districts = new List<BASIC_STRUCTS.DISTRICT_STRUCT>();

            countryCodes = new List<BASIC_STRUCTS.COUNTRY_CODES_STRUCT>();

            if (!commonQueries.GetPrimaryWorkFields(ref primaryWorkFields))
                return;

            for (int i = 0; i < primaryWorkFields.Count; i++)
            {
                primaryWorkFieldComboBox.Items.Add(primaryWorkFields[i].field_name);
            }

            commonQueries.GetAllCountries(ref countries);
            for (int i = 0; i < countries.Count; i++)
            {
                countryComboBox.Items.Add(countries[i].country_name);
            }

            secondaryWorkField.IsEnabled = false;
            stateComboBox.IsEnabled = false;
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;

            InitializeCountryCodesCombo();


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

        private bool InitializeCountryCodesCombo()
        {
            if (!commonQueries.GetCountryCodes(ref countryCodes))
                return false;

            for (int i = 0; i < countryCodes.Count; i++)
            {
                String temp = countryCodes[i].iso3 + "   " + countryCodes[i].phone_code;
                countryCodeCombo.Items.Add(temp);
            }

            return true;
        }

        private void OnTextChangedCompanyName(object sender, TextChangedEventArgs e)
        {

        }

        private void OnSelChangedPrimaryWorkField(object sender, SelectionChangedEventArgs e)
        {
            secondaryWorkField.Items.Clear();
            secondaryWorkField.IsEnabled = true;

            if (primaryWorkFieldComboBox.SelectedItem != null)
                commonQueries.GetSecondaryWorkFields(primaryWorkFields[primaryWorkFieldComboBox.SelectedIndex].field_id, ref secondaryWorkFields);

            for (int i = 0; i < secondaryWorkFields.Count; i++)
            {
                secondaryWorkField.Items.Add(secondaryWorkFields[i].field_name);
            }
            secondaryWorkField.SelectedIndex = 0;
        }

        private void OnSelChangedSecondaryWorkField(object sender, SelectionChangedEventArgs e)
        {

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

                countryCodeCombo.SelectedIndex = countryCodes.FindIndex(x1 => x1.country_id == countries[countryComboBox.SelectedIndex].country_id);
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

                if (cityComboBox.SelectedItem != "Cairo")
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
        private void OnSelChangedCountryCodeCombo(object sender, SelectionChangedEventArgs e)
        {

        }
        private void OnTextChangedTelephone(object sender, TextChangedEventArgs e)
        {

        }

        private void OnTextChangedFax(object sender, TextChangedEventArgs e)
        {

        }

        private bool CheckCompanyNameEditBox()
        {
            //String inputString = companyNameTextBox.Text;
            //String outputString = companyNameTextBox.Text;

            //if (!integrityChecker.CheckCompanyNameEditBox(inputString, ref outputString, true))
            //    return false;

            company.SetCompanyName(companyNameTextBox.Text);
            companyNameTextBox.Text = company.GetCompanyName();

            return true;
        }

        private bool CheckCompanyPhoneEditBox()
        {
            String inputString = telephoneTextBox.Text;
            String outputString = telephoneTextBox.Text;

            if (!integrityChecker.CheckCompanyPhoneEditBox(inputString, ref outputString, countries[countryComboBox.SelectedIndex].country_id, true, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            company.AddCompanyPhone(outputString);
            telephoneTextBox.Text = company.GetCompanyPhones()[0];

            return true;
        }

        private bool CheckCompanyFaxEditBox()
        {
            String inputString = faxTextBox.Text;
            String outputString = faxTextBox.Text;

            if (!integrityChecker.CheckCompanyFaxEditBox(inputString, ref outputString, countries[countryComboBox.SelectedIndex].country_id, false, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (outputString != String.Empty)
            {
                company.AddCompanyFax(outputString);
                faxTextBox.Text = company.GetCompanyFaxes()[0];

            }
            return true;
        }

        private bool CheckPrimaryWorkFieldComboBox()
        {
            if (primaryWorkFieldComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Company's Primary Work Field must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            company.SetCompanyPrimaryField(primaryWorkFields[primaryWorkFieldComboBox.SelectedIndex].field_id, primaryWorkFieldComboBox.SelectedItem.ToString());

            return true;
        }

        private bool CheckSecondaryWorkFieldComboBox()
        {
            if (secondaryWorkField.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Company's Secondary Work Field must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            company.SetCompanySecondaryField(secondaryWorkFields[secondaryWorkField.SelectedIndex].field_id, secondaryWorkField.SelectedItem.ToString());

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
            if (districtComboBox.SelectedItem == null && districtComboBox.Items.Count != 0)
            {
                System.Windows.Forms.MessageBox.Show("District must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


            company.SetCompanyDistrict(districts[districtComboBox.SelectedIndex].district_id, districtComboBox.SelectedItem.ToString());

            return true;
        }

        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {
            if (!CheckCompanyNameEditBox())
                return;
            if (!CheckPrimaryWorkFieldComboBox())
                return;
            if (!CheckSecondaryWorkFieldComboBox())
                return;
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

            //YOU DON'T NEED TO WRITE A QUERY TO GET NEW SERIAL,
            //THE COMPANY CLASS HANDLES ALL THAT 
            company.GetNewCompanySerial();
            company.GetNewAddressSerial();
            company.SetOwnerUser(loggedInUser.GetEmployeeId(), loggedInUser.GetEmployeeName());

            company.InsertIntoCompanyName();
            company.InsertIntoCompanyAddress();
            company.InsertIntoCompanyWorkField();

            for (int i = 0; i < company.GetNumberOfSavedCompanyPhones(); i++)
                company.InsertIntoCompanyTelephone(company.GetCompanyPhones()[i]);

            for (int i = 0; i < company.GetNumberOfSavedCompanyFaxes(); i++)
                company.InsertIntoCompanyFax(company.GetCompanyFaxes()[i]);

            this.Close();
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
            if (districtComboBox.Text.ToString() != "" && cityComboBox.Text != "Cairo")
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
