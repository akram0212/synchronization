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

        protected List<BASIC_STRUCTS.COUNTRY_STRUCT> countries;
        protected List<BASIC_STRUCTS.STATE_STRUCT> states;
        protected List<BASIC_STRUCTS.CITY_STRUCT> cities;
        protected List<BASIC_STRUCTS.DISTRICT_STRUCT> districts;

        protected List<PROJECT_MACROS.PROJECT_STRUCT> projects;
        protected int projectLocationId;
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

            projects = new List<PROJECT_MACROS.PROJECT_STRUCT>();

            InitializeProjects();
            InitializeCountries();

            stateComboBox.IsEnabled = false;
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;
        }
        private bool InitializeProjects()
        {
            if (!commonQueries.GetClientProjects(ref projects))
                return false;

            for (int i = 0; i < projects.Count; i++)
                ProjectNameComboBox.Items.Add(projects[i].project_name);

            return true;
        }
        private bool InitializeCountries()
        {
            if (!commonQueries.GetAllCountries(ref countries))
                return false;

            for (int i = 0; i < countries.Count; i++)
                countryComboBox.Items.Add(countries[i].country_name);

            return true;
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
        private void OnSelChangedProjectName(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {
            if (!CheckProjectNameEditBox())
                return;
            if (!CheckCountryComboBox())
                return;
            if (!CheckStateComboBox())
                return;
            if (!CheckCityComboBox())
                return;
            if (!CheckDistrictComboBox())
                return;
            if (!GetProjectLocationsCount())
                return;
            if (!InsertIntoProjectLocations())
                return;

            this.Close();
        }

        private bool CheckProjectNameEditBox()
        {
            if (ProjectNameComboBox.SelectedItem == null)
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

            return true;
        }
        private bool CheckStateComboBox()
        {
            if (stateComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("State must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
        private bool CheckCityComboBox()
        {
            if (cityComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("City must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
        private bool CheckDistrictComboBox()
        {
            if (districtComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("District must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool GetProjectLocationsCount()
        {
            String sqlQueryPart1 = @" select max(client_project_locations.location_id)
                                      from erp_system.dbo.client_project_locations
                                      where project_serial = ";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += projects[ProjectNameComboBox.SelectedIndex].project_serial;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlServer.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            projectLocationId = sqlServer.rows[0].sql_int[0] + 1;

            return true;
        }
        private bool InsertIntoProjectLocations()
        {
            String sqlQueryPart1 = @"insert into erp_system.dbo.client_project_locations
                                     values( ";
            String sqlQueryPart2 = " GETDATE());";
            String comma = ", ";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += projects[ProjectNameComboBox.SelectedIndex].project_serial;
            sqlQuery += comma;
            sqlQuery += projectLocationId;
            sqlQuery += comma;
            sqlQuery += districts[districtComboBox.SelectedIndex].district_id;
            sqlQuery += comma;
            sqlQuery += sqlQueryPart2;

            if (!sqlServer.InsertRows(sqlQuery))
                return false;

            System.Windows.Forms.MessageBox.Show("Added");

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
