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
        private List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> companiesAddedToComboList = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> branchInfo = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT> contactInfo = new List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> employeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsList = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsAddedToComboList = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();

        private int viewAddCondition;
        private int salesPersonID;
        private int salesPersonTeamID;
        
        //////////////ADD CONSTRUCTOR////////////////
        /////////////////////////////////////////////
        public WorkOfferBasicInfoPage(ref Employee mLoggedInUser, ref WorkOffer mWorkOffer,int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;
            InitializeComponent();

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            workOffer = new WorkOffer(sqlDatabase);
            workOffer = mWorkOffer;
            
            ///////////////////////////
            ////ADD
            ///////////////////////////
            if (viewAddCondition == 1)
            {
                FillrfqsList(); 
                ConfigureUIElemenetsForAdd();
                InitializeSalesPersonCombo();
                InitializeOfferProposer();
            }
            ////////////////////////////
            ///VIEW
            ////////////////////////////
            else if (viewAddCondition == 0)
            {
                FillrfqsList();
                ConfigureUIElementsForView();
                SetSalesPersonLabel();
                SetRFQSerialLabel();
                SetCompanyNameLabel();
                SetCompanyAddressLabel();
                SetContactPersonLabel();
            }
            /////////////////////////////
            ///REVISE
            /////////////////////////////
            else if(viewAddCondition == 2)
            {
                FillrfqsList();
                ConfigureUIElemenetsForAdd();
                InitializeSalesPersonCombo();
                InitializeOfferProposer();
                SetSalesPersonComboValue();
                if (RFQSerialCombo.IsEnabled == true)
                    SetRFQSerialComboValue();
                else 
                { 
                    SetCompanyNameComboValue();
                    SetContactPersonComboValue();
                }
                
            }
            //////////////////////////
            ///RESOLVE RFQ
            //////////////////////////
            else
            {
                FillrfqsList();
                ConfigureUIElemenetsForAdd();
                InitializeSalesPersonCombo();
                InitializeOfferProposer();
                SetSalesPersonComboValue();
                if (RFQSerialCombo.IsEnabled == true)
                    SetRFQSerialComboValue();
                else
                {
                    SetCompanyNameComboValue();
                    SetContactPersonComboValue();
                }
            }
        }

        //////////////////////////////////////////////////
        ///////////////INITIALIZE FUNCTIONS///////////////
        //////////////////////////////////////////////////

        private void InitializeOfferProposer()
        {
            workOffer.InitializeOfferProposerInfo(loggedInUser.GetEmployeeId(), loggedInUser.GetEmployeeTeamId());
        }
        private bool InitializeSalesPersonCombo()
        {
            if (!commonQueriesObject.GetDepartmentEmployees(COMPANY_ORGANISATION_MACROS.MARKETING_AND_SALES_DEPARTMENT_ID, ref employeesList))
                return false;

            for (int i = 0; i < employeesList.Count(); i++)
            {
                salesPersonCombo.Items.Add(employeesList[i].employee_name);
            }
            return true;
        }

        private bool FillrfqsList()
        {
            if (!commonQueriesObject.GetRFQs(ref rfqsList))
                return false;
            return true;
        }
        private void InitializeRFQSerialCombo()
        {
            for (int i = rfqsList.Count - 1; i >= 0; i--)
            {
                if (rfqsList[i].sales_person_id == salesPersonID)
                {
                    RFQSerialCombo.Items.Add(rfqsList[i].rfq_id);
                    rfqsAddedToComboList.Add(rfqsList[i]);
                }
            }
            RFQSerialCombo.IsEnabled = true;
        }

        private void InitializeCompanyNameCombo()
        {
            if (!GetCompaniesQuery(salesPersonID, ref companiesList))
                return;

            for (int i = 0; i < companiesList.Count; i++)
            {
                companyNameCombo.Items.Add(companiesList[i].company_name);
                companiesAddedToComboList.Add(companiesList[i]);
            }

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
           
            if (!commonQueriesObject.GetCompanyAddresses(companiesList[companyNameCombo.SelectedIndex].company_serial, ref branchInfo))
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

        ///////////////////////////////////////////////////
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
        private void ConfigureUIElementsForView()
        {
            salesPersonLabel.Visibility = Visibility.Visible;
            RFQSerialLabel.Visibility = Visibility.Visible;
            companyNameLabel.Visibility = Visibility.Visible;
            companyAddressLabel.Visibility = Visibility.Visible;
            contactPersonNameLabel.Visibility = Visibility.Visible;

            salesPersonCombo.Visibility = Visibility.Collapsed;
            RFQSerialCombo.Visibility = Visibility.Collapsed;
            companyNameCombo.Visibility = Visibility.Collapsed;
            companyAddressCombo.Visibility = Visibility.Collapsed;
            contactPersonNameCombo.Visibility = Visibility.Collapsed;

        }

        ////////////////////////////////////////////////////
        ///////////SET FUNCTIONS////////////////////////////
        ////////////////////////////////////////////////////
        private void SetSalesPersonLabel()
        {
            salesPersonLabel.Content = workOffer.GetSalesPersonName();
        }

        private void SetSalesPersonComboValue()
        {
            salesPersonCombo.Text = workOffer.GetSalesPersonName();
        }
        private void SetRFQSerialLabel()
        {
            RFQSerialLabel.Content = workOffer.GetRFQID();
        }

        private void SetRFQSerialComboValue()
        {
            RFQSerialCombo.Text = workOffer.GetRFQID();
        }

        private void SetCompanyNameLabel()
        {
            companyNameLabel.Content = workOffer.GetCompanyName();
        }

        private void SetCompanyNameComboValue()
        {
            companyNameCombo.Text = workOffer.GetCompanyName();
        }

        private void SetCompanyAddressLabel()
        {
            if (!commonQueriesObject.GetCompanyAddresses(workOffer.GetCompanySerial(), ref branchInfo))
                return;

            string address;
            if (branchInfo.Count != 0)
            {
                address = branchInfo[0].district + ", " + branchInfo[0].city + ", " + branchInfo[0].state_governorate + ", " + branchInfo[0].country + ".";
                companyAddressLabel.Content = address;
            }
        }

        private void SetContactPersonLabel()
        {
            contactPersonNameLabel.Content = workOffer.GetContactName();
        }

        private void SetContactPersonComboValue()
        {
            contactPersonNameCombo.Text= workOffer.GetContactName();
        }

        ////////////////////////////////////////////////////
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

        ////////////////////////////////////////////////////
        ///////////SELECTION CHANGED HANDLERS///////////////
        ////////////////////////////////////////////////////
        private void OnSelChangedSalesPersonCombo(object sender, SelectionChangedEventArgs e)
        {
            RFQSerialCombo.Items.Clear();
            companyNameCombo.Items.Clear();
            salesPersonID = employeesList[salesPersonCombo.SelectedIndex].employee_id;
            salesPersonTeamID = employeesList[salesPersonCombo.SelectedIndex].team.team_id;

            if (salesPersonTeamID == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
            {
                workOffer.InitializeSalesPersonInfo(salesPersonID);
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
        private void OnSelChangedRFQSerialCombo(object sender, SelectionChangedEventArgs e)
        {
            if(RFQSerialCombo.SelectedItem != null)
                workOffer.InitializeRFQInfo(rfqsAddedToComboList[RFQSerialCombo.SelectedIndex].rfq_serial, rfqsAddedToComboList[RFQSerialCombo.SelectedIndex].rfq_version);
        }

        private void OnSelChangedCompanyNameCombo(object sender, SelectionChangedEventArgs e)
        {
            companyAddressCombo.Items.Clear();

            if (companyNameCombo.SelectedItem != null)
                InitializeCompanyAddressCombo();

            if (companyNameCombo.SelectedItem == null)
            {
                companyAddressCombo.SelectedItem = null;
                companyAddressCombo.IsEnabled = false;
            }
            
            workOffer.InitializeCompanyInfo(companiesAddedToComboList[companyNameCombo.SelectedIndex].company_serial);
        }

        private void OnSelChangedCompanyAddressCombo(object sender, SelectionChangedEventArgs e)
        {
            contactPersonNameCombo.Items.Clear();

            if (companyAddressCombo.SelectedItem != null)
                InitializeCompanyContactCombo();
        }

        private void OnSelChangedContactPersonCombo(object sender, SelectionChangedEventArgs e)
        {

        }

        /////////////////////////////////////////
        ///////////BUTTON CLICK HANDLERS/////////
        /////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            WorkOfferProductsPage workOfferProductsPage = new WorkOfferProductsPage(ref loggedInUser, ref workOffer, viewAddCondition);
            NavigationService.Navigate(workOfferProductsPage);
        }
        private void OnClickPaymentAndDelivery(object sender, MouseButtonEventArgs e)
        {
             WorkOfferPaymentAndDeliveryPage paymentAndDeliveryPage = new WorkOfferPaymentAndDeliveryPage(ref loggedInUser, ref workOffer, viewAddCondition);
            NavigationService.Navigate(paymentAndDeliveryPage);
        }

        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            WorkOfferAdditionalInfoPage offerAdditionalInfoPage = new WorkOfferAdditionalInfoPage(ref loggedInUser, ref workOffer, viewAddCondition);
            NavigationService.Navigate(offerAdditionalInfoPage);
        }

       
    }
}
