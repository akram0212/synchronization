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

        List<BASIC_STRUCTS.PRIMARY_FIELD_STRUCT> primaryWorkFields;
        List<BASIC_STRUCTS.SECONDARY_FIELD_STRUCT> secondaryWorkFields;

        List<BASIC_STRUCTS.COUNTRY_STRUCT> countries;
        List<BASIC_STRUCTS.STATE_STRUCT> states;
        List<BASIC_STRUCTS.CITY_STRUCT> cities;
        List<BASIC_STRUCTS.DISTRICT_STRUCT> districts;

        int employeeID;
        int branch_serial;
        int companySerial;
        int address;
        string companyName;
        string companyTelephone;
        string companyFax;

        public AddCompanyWindow(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;
            employeeID = loggedInUser.GetEmployeeId();
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

            commonQueries.GetPrimaryWorkFields(ref primaryWorkFields);
            for(int i = 0; i < primaryWorkFields.Count; i++)
            {
                primaryWorkFieldComboBox.Items.Add(primaryWorkFields[i].field_name);
            }

            commonQueries.GetAllCountries(ref countries);
            for (int i = 0; i < countries.Count; i++)
            {
                countryComboBox.Items.Add(countries[i].country_name);
            }
            stateComboBox.IsEnabled = false;
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;


        }

        private void OnTextChangedCompanyName(object sender, TextChangedEventArgs e)
        {

        }

        private void OnSelChangedPrimaryWorkField(object sender, SelectionChangedEventArgs e)
        {
            secondaryWorkField.Items.Clear();
            if (primaryWorkFieldComboBox.SelectedIndex != null)
                commonQueries.GetSecondaryWorkFields(primaryWorkFields[primaryWorkFieldComboBox.SelectedIndex].field_id, secondaryWorkFields);
            for (int i = 0; i < secondaryWorkFields.Count; i++)
            {
                secondaryWorkField.Items.Add(secondaryWorkFields[i].field_name);
            }
        }

        private void OnSelChangedSecondaryWorkField(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnSelChangedCountry(object sender, SelectionChangedEventArgs e)
        {
            stateComboBox.IsEnabled = true;
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;

            if (countryComboBox.SelectedIndex != null)
                commonQueries.GetAllCountryStates(countries[countryComboBox.SelectedIndex].country_id, ref states);
            stateComboBox.Items.Clear();
            for (int i = 0; i < states.Count(); i++)
            {
                if (countryComboBox.SelectedIndex != null && states[i].state_id / 100 == countries[countryComboBox.SelectedIndex].country_id)
                    stateComboBox.Items.Add(states[i].state_name);
            }
        }

        private void OnSelChangedState(object sender, SelectionChangedEventArgs e)
        {
            cityComboBox.IsEnabled = true;
            districtComboBox.IsEnabled = false;

            if (stateComboBox.SelectedIndex != null)
                commonQueries.GetAllStateCities(states[stateComboBox.SelectedIndex].state_id, ref cities);
            cityComboBox.Items.Clear();

            for (int i = 0; i < cities.Count; i++)
            {
                if (stateComboBox.SelectedIndex != null && cities[i].city_id / 100 == states[stateComboBox.SelectedIndex].state_id)
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

        private bool CheckCompanyNameEditBox()
        {
            string inputString = companyNameTextBox.Text;
            string outputString = companyNameTextBox.Text;

            if (!integrityChecker.CheckCompanyNameEditBox(inputString, ref outputString, true))
                return false;

            companyName = outputString;
            company.SetCompanyName(companyName);
            companyNameTextBox.Text = companyName;

            return true;
        }

        private bool CheckCompanyPhoneEditBox()
        {
            string inputString = telephoneTextBox.Text;
            string outputString = telephoneTextBox.Text;

            if (!integrityChecker.CheckCompanyPhoneEditBox(inputString, ref outputString, false))
                return false;

            company.AddCompanyPhone(outputString);
            telephoneTextBox.Text = companyTelephone;

            return true;
        } 
        
        private bool CheckCompanyFaxEditBox()
        {
            string inputString = faxTextBox.Text;
            string outputString = faxTextBox.Text;

            if (!integrityChecker.CheckCompanyFaxEditBox(inputString, ref outputString, false))
                return false;

            companyFax = outputString;
            faxTextBox.Text = companyFax;

            return true;
        }

        private bool CheckPrimaryWorkFieldComboBox()
        {
            if (primaryWorkFieldComboBox.SelectedItem == null)
            {
                MessageBox.Show("Company's Primary Work Field must be specified.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            company.SetCompanyPrimaryField(primaryWorkFields[primaryWorkFieldComboBox.SelectedIndex].field_id, primaryWorkFieldComboBox.SelectedItem.ToString());
           
            return true;
        }

        private bool CheckSecondaryWorkFieldComboBox()
        {
            if (secondaryWorkField.SelectedItem == null)
            {
                MessageBox.Show("Company's Secondary Work Field must be specified.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            company.SetCompanySecondaryField(secondaryWorkFields[secondaryWorkField.SelectedIndex].field_id, secondaryWorkField.SelectedItem.ToString());

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

            address = districts[districtComboBox.SelectedIndex].district_id;
            
            QueryGetMaxCompanySerial();

            if (telephoneTextBox.Text != "")
            {
                QueryAddCompanyTelephone();
            }

            if (faxTextBox.Text != "")
            {
                QueryAddCompanyFax();
            }

            QueryAddCompanyName();
            QueryAddCompanyAddress();
            QueryAddCompanyWorkField();
            MessageBox.Show("Company Added Successfully");
            this.Hide();

        }
        private bool QueryAddCompanyName()
        {
            string sqlQueryPart1 = @"insert into erp_system.dbo.company_name values(";
            string sqlQueryPart2 = ", ";
            string sqlQueryPart3 = "getdate()) ;";

            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += companySerial;
            sqlQuery += sqlQueryPart2;
            sqlQuery += "'" + company.GetCompanyName() + "'";
            sqlQuery += sqlQueryPart2;
            sqlQuery += employeeID;
            sqlQuery += sqlQueryPart2;
            sqlQuery += sqlQueryPart3;

            if (!sqlServer.InsertRows(sqlQuery))
                return false;

            return true;
        }
        private bool QueryAddCompanyAddress()
        {
            string sqlQueryPart1 = @"insert into erp_system.dbo.company_address values(";
            string sqlQueryPart2 = ", ";
            string sqlQueryPart3 = "getdate()) ;";

            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += company.GetCompanySerial();
            sqlQuery += sqlQueryPart2;
            sqlQuery += companySerial;
            sqlQuery += sqlQueryPart2;
            sqlQuery += address;
            sqlQuery += sqlQueryPart2;
            sqlQuery += employeeID;
            sqlQuery += sqlQueryPart2;
            sqlQuery += sqlQueryPart3;

            if (!sqlServer.InsertRows(sqlQuery))
                return false;

            return true;
        } 
        private bool QueryAddCompanyWorkField()
        {
            string sqlQueryPart1 = @"insert into erp_system.dbo.company_field_of_work values(";
            string sqlQueryPart2 = ", ";
            string sqlQueryPart3 = "getdate()) ;";

            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += companySerial;
            sqlQuery += sqlQueryPart2;
            sqlQuery += company.GetCompanySecondaryFieldId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += employeeID;
            sqlQuery += sqlQueryPart2;
            sqlQuery += sqlQueryPart3;

            if (!sqlServer.InsertRows(sqlQuery))
                return false;

            return true;
        } 
        private bool QueryAddCompanyFax()
        {
            string sqlQueryPart1 = @"insert into erp_system.dbo.company_fax values(";
            string sqlQueryPart2 = ", ";
            string sqlQueryPart3 = "getdate()) ;";

            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += companySerial;
            sqlQuery += sqlQueryPart2;
            sqlQuery += company.GetCompanyFaxes()[0];
            sqlQuery += sqlQueryPart2;
            sqlQuery += employeeID;
            sqlQuery += sqlQueryPart2;
            sqlQuery += sqlQueryPart3;

            if (!sqlServer.InsertRows(sqlQuery))
                return false;

            return true;
        }
        private bool QueryAddCompanyTelephone()
        {
            string sqlQueryPart1 = @"insert into erp_system.dbo.company_telephone values(";
            string sqlQueryPart2 = ", ";
            string sqlQueryPart3 = "getdate()) ;";

            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += companySerial;
            sqlQuery += sqlQueryPart2;
            sqlQuery += company.GetCompanyPhones()[0];
            sqlQuery += sqlQueryPart2;
            sqlQuery += employeeID;
            sqlQuery += sqlQueryPart2;
            sqlQuery += sqlQueryPart3;

            if (!sqlServer.InsertRows(sqlQuery))
                return false;

            return true;
        }
        private bool QueryGetMaxCompanySerial()
        {
            string sqlQueryPart1 = "select max(company_serial) from erp_system.dbo.company_name ";
            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 0;

            if (!sqlServer.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            companySerial = 1 + sqlServer.rows[0].sql_int[0];

            return true;
        }
        }
}
