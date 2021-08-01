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
using System.Windows.Navigation;
using System.Windows.Shapes;
using _01electronics_library;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for WorkOfferBasicInfoPage.xaml
    /// </summary>
    public partial class WorkOfferBasicInfoPage : Page
    {
        Employee loggedInUser;
        WorkOffer workOffer;

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;

        private List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> companiesList = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> branchInfo = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT> contactInfo = new List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> employeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsList = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();

        private int viewAddCondition;
        private int salesPersonID;
        private int salesPersonTeamID;

        //////////////ADD CONSTRUCTOR////////////////
        /////////////////////////////////////////////
        public WorkOfferBasicInfoPage(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            workOffer = new WorkOffer(sqlDatabase);

            ConfigureUIElemenetsForAdd();
            InitializeSalesPersonCombo();

            viewAddCondition = 1;
        }
        //////////////VIEW CONSTRUCTOR///////////////
        /////////////////////////////////////////////
        public WorkOfferBasicInfoPage(ref Employee mLoggedInUser, ref WorkOffer mWorkOffer)
        {
            loggedInUser = mLoggedInUser;
            workOffer = mWorkOffer;

            InitializeComponent();

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            workOffer = new WorkOffer(sqlDatabase);

            ConfigureUIElemenetsForAdd();
            InitializeSalesPersonCombo();

            viewAddCondition = 0;
        }

        ///////////////INITIALIZE FUNCTIONS///////////////
        //////////////////////////////////////////////////
        private bool InitializeSalesPersonCombo()
        {
            if (!commonQueriesObject.GetDepartmentEmployees(COMPANY_ORGANISATION_MACROS.MARKETING_AND_SALES_DEPARTMENT_ID, ref employeesList))
                return false;

            for (int i = 0; i < employeesList.Count(); i++)
            {
                string temp = employeesList[i].employee_name;
                salesPersonCombo.Items.Add(temp);
            }
            return true;
        }

        private bool InitializeRFQSerialCombo()
        {
            if (!commonQueriesObject.GetRFQs(ref rfqsList))
                return false;

            for (int i = rfqsList.Count - 1; i >= 0; i--)
            {
                if (rfqsList[i].sales_person_id == salesPersonID)
                    RFQSerialCombo.Items.Add(rfqsList[i].rfq_id);
            }
            RFQSerialCombo.IsEnabled = true;

            return true;
        }

        private void InitializeCompanyNameCombo()
        {
            if (!GetCompaniesQuery(salesPersonID, ref companiesList))
                return;

            for (int i = 0; i < companiesList.Count; i++)
                companyNameCombo.Items.Add(companiesList[i].company_name);

            if (companyNameCombo.SelectedItem == null)
            {
                companyAddressCombo.SelectedItem = null;
                companyAddressCombo.IsEnabled = false;
            }
            else
                InitializeCompanyAddressCombo();

            companyNameCombo.IsEnabled = true;
        }

        private void InitializeCompanyAddressCombo()
        {
           
            if (!commonQueriesObject.GetCompanyAddresses(companyNameCombo.SelectedIndex + 1, ref branchInfo))
                return;

            for (int i = 0; i < branchInfo.Count; i++)
            {
                string address;
                address = branchInfo[i].district + ", " + branchInfo[i].city + ", " + branchInfo[i].state_governorate + ", " + branchInfo[i].country + ".";
                companyAddressCombo.Items.Add(address);
            }

            if (branchInfo.Count == 1)
                companyAddressCombo.SelectedItem = companyAddressCombo.Items.GetItemAt(0);
            
            if(companyAddressCombo.SelectedItem == null)
            {
                contactPersonNameCombo.SelectedItem = null;
                contactPersonNameCombo.IsEnabled = false;
            }
            companyAddressCombo.IsEnabled = true;
        }

        private void InitializeCompanyContactCombo()
        {
            int addressSerial = branchInfo[companyAddressCombo.SelectedIndex].address_serial;

            if (!commonQueriesObject.GetCompanyContacts(loggedInUser.GetEmployeeId(), addressSerial, ref contactInfo))
                return;

            for (int i = 0; i < contactInfo.Count(); i++)
                contactPersonNameCombo.Items.Add(contactInfo[i].contact_name);
        
            if (contactInfo.Count() == 1)
                contactPersonNameCombo.Items.GetItemAt(0);

            contactPersonNameCombo.IsEnabled = true;
        }
        ///////////CONFIGURE UI ELEMENTS////////////////////
        ///////////////////////////////////////////////////
        private void ConfigureUIElemenetsForAdd()
        {
            salesPersonLabel.Visibility = Visibility.Collapsed;
            RFQSerialLabel.Visibility = Visibility.Collapsed;
            companyNameLabel.Visibility = Visibility.Collapsed;
            companyAddressLabel.Visibility = Visibility.Collapsed;
            contactPersonNameLabel.Visibility = Visibility.Collapsed;

            salesPersonCombo.Visibility = Visibility.Visible;
            RFQSerialCombo.Visibility = Visibility.Visible;
            companyNameCombo.Visibility = Visibility.Visible;
            companyAddressCombo.Visibility = Visibility.Visible;
            contactPersonNameCombo.Visibility = Visibility.Visible;

            RFQSerialCombo.IsEnabled = false;
            companyNameCombo.IsEnabled = false;
            companyAddressCombo.IsEnabled = false;
            contactPersonNameCombo.IsEnabled = false;
        }

        ///////////GET FUNCTIONS////////////////////////////
        ////////////////////////////////////////////////////

        private bool GetCompaniesQuery(int mEmployeeSerial, ref List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> returnVector)
        {
            returnVector.Clear();

            string sqlQuery = "SELECT company_serial,company_name FROM erp_system.dbo.company_name WHERE added_by = " + mEmployeeSerial;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT temp = new COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT();

                temp.company_serial = sqlDatabase.rows[i].sql_int[0];
                temp.company_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(temp);
            }
            return true;
        }

        ///////////SELECTION CHANGED HANDLERS///////////////
        ////////////////////////////////////////////////////
        private void SalesPersonComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RFQSerialCombo.Items.Clear();
            companyNameCombo.Items.Clear();
            salesPersonID = employeesList[salesPersonCombo.SelectedIndex].employee_id;

            if (!commonQueriesObject.GetEmployeeTeam(salesPersonID, ref salesPersonTeamID))
                return;

            if (salesPersonTeamID == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
            {
                InitializeRFQSerialCombo();
                companyNameCombo.SelectedItem = null;
                companyNameCombo.IsEnabled = false;
            }
            else
            {
                InitializeCompanyNameCombo();
                RFQSerialCombo.SelectedItem = null;
                RFQSerialCombo.IsEnabled = false;
            }
        }
        private void RFQSerialComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CompanyNameComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            companyAddressCombo.Items.Clear();

            if (companyNameCombo.SelectedItem != null)
                InitializeCompanyAddressCombo();
        }

        private void CompanyAddressComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            contactPersonNameCombo.Items.Clear();

            if (companyAddressCombo.SelectedItem != null)
                InitializeCompanyContactCombo();
        }

        private void ContactPersonComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        ///////////BUTTON CLICK HANDLERS/////////
        /////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            if(viewAddCondition == 0)
            {
                WorkOfferProductsPage workOfferProductsPage = new WorkOfferProductsPage(ref loggedInUser, ref workOffer);
                NavigationService.Navigate(workOfferProductsPage);
            }
            else
            {
                WorkOfferProductsPage workOfferProductsPage = new WorkOfferProductsPage(ref loggedInUser);
                NavigationService.Navigate(workOfferProductsPage);
            }
        }
        private void OnClickPaymentAndDelivery(object sender, MouseButtonEventArgs e)
        {
            WorkOfferPaymentAndDeliveryPage workOfferPaymentAndDeliveryPage = new WorkOfferPaymentAndDeliveryPage();
            NavigationService.Navigate(workOfferPaymentAndDeliveryPage);
        }

        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            /*
            if (viewAddCondition == 0)
            {
                RFQAdditionalInfoPage additionalInfoPage = new RFQAdditionalInfoPage(ref loggedInUser, ref rfq);
                NavigationService.Navigate(additionalInfoPage);
            }
            else
            {
                RFQAdditionalInfoPage additionalInfoPage = new RFQAdditionalInfoPage(ref loggedInUser);
                NavigationService.Navigate(additionalInfoPage);
            }*/
        }

       
    }
}
