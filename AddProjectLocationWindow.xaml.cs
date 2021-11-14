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
    /// Interaction logic for AddProjectLocationWindow.xaml
    /// </summary>
    public partial class AddProjectLocationWindow : Window
    {

        public Employee loggedInUser;

        protected SQLServer sqlServer;
        protected String sqlQuery;
        protected CommonQueries commonQueries;
        protected CommonFunctions commonFunctions;
        protected IntegrityChecks integrityChecker;

        List<BASIC_STRUCTS.COUNTRY_STRUCT> countries;
        List<BASIC_STRUCTS.STATE_STRUCT> states;
        List<BASIC_STRUCTS.CITY_STRUCT> cities;
        List<BASIC_STRUCTS.DISTRICT_STRUCT> districts;
        public AddProjectLocationWindow(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;

            sqlServer = new SQLServer();
            commonQueries = new CommonQueries();
            commonFunctions = new CommonFunctions();
            integrityChecker = new IntegrityChecks();

            countries = new List<BASIC_STRUCTS.COUNTRY_STRUCT>();
            states = new List<BASIC_STRUCTS.STATE_STRUCT>();
            cities = new List<BASIC_STRUCTS.CITY_STRUCT>();
            districts = new List<BASIC_STRUCTS.DISTRICT_STRUCT>();
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
                if (!commonQueries.GetAllStateCities(states[stateComboBox.SelectedIndex].state_id, ref cities))
                    return;

                cityComboBox.IsEnabled = true;
                cityComboBox.Items.Clear();

                for (int i = 0; i < cities.Count; i++)
                {
                    if (stateComboBox.SelectedItem != null && cities[i].city_id / 100 == states[stateComboBox.SelectedIndex].state_id)
                        cityComboBox.Items.Add(cities[i].city_name);
                }
            }
            else
            {
                cityComboBox.SelectedItem = null;
                cityComboBox.IsEnabled = false;
            }
        }

        private void OnSelChangedCity(object sender, SelectionChangedEventArgs e)
        {
            if (cityComboBox.SelectedItem != null)
            {
                if (!commonQueries.GetAllCityDistricts(cities[cityComboBox.SelectedIndex].city_id, ref districts))
                    return;

                districtComboBox.IsEnabled = true;
                districtComboBox.Items.Clear();

                for (int i = 0; i < districts.Count; i++)
                {
                    if (cityComboBox.SelectedItem != null && districts[i].district_id / 100 == cities[cityComboBox.SelectedIndex].city_id)
                        districtComboBox.Items.Add(districts[i].district_name);
                }
            }
            else
            {
                districtComboBox.IsEnabled = false;
                districtComboBox.SelectedItem = null;
            }
        }

        private void OnSelChangedDistrict(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnSelChangedProjectName(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {

        }
        private bool CheckProjectNameEditBox()
        {
            if (countryComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Project Name must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


                return true;
        }
        private bool CheckCountryComboBox()
        {
            if (countryComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Country must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

           // company.SetCompanyCountry(countries[countryComboBox.SelectedIndex].country_id, countryComboBox.SelectedItem.ToString());

            return true;
        }

        private bool CheckStateComboBox()
        {
            if (stateComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("State must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            //company.SetCompanyState(states[stateComboBox.SelectedIndex].state_id, stateComboBox.SelectedItem.ToString());

            return true;
        }

        private bool CheckCityComboBox()
        {
            if (cityComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("City must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

           // company.SetCompanyCity(cities[cityComboBox.SelectedIndex].city_id, cityComboBox.SelectedItem.ToString());

            return true;
        }

        private bool CheckDistrictComboBox()
        {
            if (districtComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("District must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            //company.SetCompanyDistrict(districts[districtComboBox.SelectedIndex].district_id, districtComboBox.SelectedItem.ToString());

            return true;
        }
    }
}
