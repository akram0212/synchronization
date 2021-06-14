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
using _01electronics_erp;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for RFQBasicInfoPage.xaml
    /// </summary>
    public partial class RFQBasicInfoPage : Page
    {
        Contact contact;
        Employee loggedInUser;
        RFQ rfq;
        Company company;
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsList;


        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;

        public struct Company_Struct
        {
            public int companySerial;
            public string companyName;
        };

        private List<Company_Struct> companyInfo = new List<Company_Struct>();
        private List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> branchInfo = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT> contactInfo = new List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> preSalesEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        String[] contactPhones = new String[COMPANY_ORGANISATION_MACROS.MAX_TELEPHONES_PER_CONTACT];

        private int viewAddCondition;

        /////////////ADD CONSTRUCTOR//////////////
        //////////////////////////////////////////
        public RFQBasicInfoPage(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            contact = new Contact(sqlDatabase);
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();
            sqlDatabase = new SQLServer();
            rfq = new RFQ(sqlDatabase);

            ConfigureAddRFQUIElements();

            InitializeIssueDate();
            InitializeSalesPersonLabel();
            InitializeCompanyNameCombo();
            InitializeAssigneeCombo();
            
            viewAddCondition = 1;

            SetIssueDate();
            SetSalesPerson();
        }
        ///////VIEW CONSTRUCTOR/////////////
        ////////////////////////////////////
        public RFQBasicInfoPage(ref Employee mLoggedInUser, ref RFQ mRFQ)
        {
            loggedInUser = mLoggedInUser;
            rfq = mRFQ;
            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            company = new Company(sqlDatabase);
            contact = new Contact(sqlDatabase);

            InitializeComponent();
            InitializeCompanyInfo();
            InitializeContactInfo();
            ConfigureViewRFQUIElements();

            InitializeIssueDate();

            SetSalesPersonLabel();
            SetAssigneeLabel();
            SetCompanyNameLabel();
            SetCompanyAddressLabel();
            SetContactPersonLabel();

            viewAddCondition = 0;
        }

        /////////////////UI ELEMENTS CINFIGURATION//////////////
        ////////////////////////////////////////////////////////
        private void ConfigureAddRFQUIElements()
        {
            offerProposerLabel.Visibility = Visibility.Hidden;
            companyNameLabel.Visibility = Visibility.Hidden;
            companyAddressLabel.Visibility = Visibility.Hidden;
            contactPersonNameLabel.Visibility = Visibility.Hidden;
            contactPersonPhoneLabel.Visibility = Visibility.Hidden;

            companyAddressCombo.IsEnabled = false;
            contactPersonCombo.IsEnabled = false;
            contactPersonPhoneCombo.IsEnabled = false;
            issueDateDatePicker.IsEnabled = false;
        }

        private void ConfigureViewRFQUIElements()
        {
            assigneeCombo.Visibility = Visibility.Hidden;
            companyNameCombo.Visibility = Visibility.Hidden;
            companyAddressCombo.Visibility = Visibility.Hidden;
            contactPersonCombo.Visibility = Visibility.Hidden;
            contactPersonPhoneCombo.Visibility = Visibility.Hidden;
        }

        ///////////////INITIALIZE FUNCTIONS///////////////
        //////////////////////////////////////////////////
        
        private void InitializeCompanyInfo()
        {
            int companySerial = rfq.GetCompanySerial();
            company.InitializeCompanyInfo(companySerial);

            if (!commonQueriesObject.GetCompanyAddresses(companySerial, ref branchInfo))
                return;
        }
        private void InitializeContactInfo()
        {
            if (!commonQueriesObject.GetCompanyContacts(loggedInUser.GetEmployeeId(),rfq.GetAddressSerial(), ref contactInfo))
                return;
        }
        private void InitializeIssueDate()
        {
            issueDateDatePicker.SelectedDate = DateTime.Today;
        }

        private void InitializeSalesPersonLabel()
        {
            salesPersonLabel.Content = loggedInUser.GetEmployeeName();
        }
        private void InitializeCompanyNameCombo()
        {
            if (!GetCompaniesQuery(loggedInUser.GetEmployeeId(), ref companyInfo))
                return;
            for (int i = 0; i < companyInfo.Count; i++)
            {
                string tempName = companyInfo[i].companyName;
                companyNameCombo.Items.Add(tempName);
            }
        }

        private void InitializeAssigneeCombo()
        {
            if (!commonQueriesObject.GetTeamEmployees(COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID, ref preSalesEmployees))
                return;

            for (int i = 0; i < preSalesEmployees.Count(); i++)
            {
                COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT temp = preSalesEmployees[i];
                assigneeCombo.Items.Add(temp.employee_name);
            }
        }

        //////////////GET FUNCTIONS///////////////////
        //////////////////////////////////////////////
        private bool GetCompaniesQuery(int mEmployeeSerial, ref List<Company_Struct> returnVector)
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
                Company_Struct temp = new Company_Struct();

                temp.companySerial = sqlDatabase.rows[i].sql_int[0];
                temp.companyName = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(temp);
            }
            return true;
        }
        /////////////SET FUNCTIONS////////////////
        //////////////////////////////////////////
        
        private void SetIssueDate()
        {
            rfq.SetRFQIssueDate(DateTime.Parse(issueDateDatePicker.SelectedDate.ToString()));
        }

        private void SetSalesPerson()
        {
            rfq.InitializeSalesPersonInfo(loggedInUser.GetEmployeeId());
        }

        private void SetSalesPersonLabel()
        {
            salesPersonLabel.Content = rfq.GetSalesPersonName();
        }

        private void SetAssigneeLabel()
        {
            offerProposerLabel.Content = rfq.GetAssigneeName();
        }

        private void SetCompanyNameLabel()
        {
            companyNameLabel.Content = company.GetCompanyName();
        }

        private void SetCompanyAddressLabel()
        {
            for (int i = 0; i < branchInfo.Count; i++)
            {
                string address;
                address = branchInfo[i].district + ", " + branchInfo[i].city + ", " + branchInfo[i].state_governorate + ", " + branchInfo[i].country + "\n";
                companyAddressLabel.Content += address;
            }
        }

        private void SetContactPersonLabel()
        {
            for (int i = 0; i < contactInfo.Count(); i++)
            {
                COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT tempContact = contactInfo[i];
                contactPersonNameLabel.Content += tempContact.contact_name + "\n";
            }
        }
        /////////////SELECTION CHANGED//////////////
        ////////////////////////////////////////////
        private void AssigneeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(assigneeCombo.SelectedItem != null)
                rfq.InitializeAssignedEngineerInfo(preSalesEmployees[assigneeCombo.SelectedIndex].employee_id);
        }

        private void CompanyNameComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            companyAddressCombo.Items.Clear();

            if (companyNameCombo.SelectedItem != null)
            {
                companyAddressCombo.IsEnabled = true;

                int companySerial = companyNameCombo.SelectedIndex + 1;

                if (!commonQueriesObject.GetCompanyAddresses(companySerial, ref branchInfo))
                    return;

                for (int i = 0; i < branchInfo.Count; i++)
                {
                    string address;
                    address = branchInfo[i].district + ", " + branchInfo[i].city + ", " + branchInfo[i].state_governorate + ", " + branchInfo[i].country + ".";
                    companyAddressCombo.Items.Add(address);
                }

                if (branchInfo.Count == 1)
                    companyAddressCombo.SelectedItem = companyAddressCombo.Items.GetItemAt(0);
                rfq.InitializeCompanyInfo(companySerial);
            }
            
        }

        private void CompanyAddressComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            contactPersonCombo.Items.Clear();

            if (companyAddressCombo.SelectedItem != null)
            {
                contactPersonCombo.IsEnabled = true;
                int addressSerial = branchInfo[companyAddressCombo.SelectedIndex].address_serial;

                if (!commonQueriesObject.GetCompanyContacts(loggedInUser.GetEmployeeId(), addressSerial, ref contactInfo))
                    return;
                
                for (int i = 0; i < contactInfo.Count(); i++)
                {
                    COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT tempContact = contactInfo[i];
                    contactPersonCombo.Items.Add(tempContact.contact_name);
                }

                if (contactInfo.Count() == 1)
                    contactPersonCombo.Items.GetItemAt(0);
                rfq.InitializeBranchInfo(addressSerial);
            }
        }

        private void ContactPersonComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(contactPersonCombo.SelectedItem != null)
            {
                contactPersonPhoneCombo.IsEnabled = true;
                rfq.InitializeContactInfo(contactInfo[contactPersonCombo.SelectedIndex].contact_id);
            }
            int numberOfPhones = contact.GetNumberOfSavedContactPhones();
            contactPhones = contact.GetContactPhones();

            for (int i = 0; i < numberOfPhones; i++)
            {
                contactPersonPhoneCombo.Items.Add(contactPhones[i]);
            }
        }

        private void ContactPersonPhoneComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //////////BUTTON CLICKED///////////////////
        ///////////////////////////////////////////
        private void OnClickBasicInfo(object sender, RoutedEventArgs e)
        {
           /* if(viewAddCondition == 0)
            {
                RFQBasicInfoPage basicInfoPage = new RFQBasicInfoPage(ref loggedInUser, ref rfq);
                NavigationService.Navigate(basicInfoPage);
            }
            else
            {
                RFQBasicInfoPage basicInfoPage = new RFQBasicInfoPage(ref loggedInUser);
                NavigationService.Navigate(basicInfoPage);
            }*/
        }

        private void OnClickProductsInfo(object sender, RoutedEventArgs e)
        {
            if(viewAddCondition == 0)
            {
                RFQProductsPage productsPage = new RFQProductsPage(ref loggedInUser, ref rfq);
                NavigationService.Navigate(productsPage);
            }
            else
            {
                RFQProductsPage productsPage = new RFQProductsPage(ref loggedInUser);
                NavigationService.Navigate(productsPage);
            }
        }

        private void OnClickAdditionalInfo(object sender, RoutedEventArgs e)
        {
            if(viewAddCondition == 0)
            {
                RFQAdditionalInfoPage additionalInfoPage = new RFQAdditionalInfoPage(ref loggedInUser, ref rfq);
                NavigationService.Navigate(additionalInfoPage);
            }
            else
            {
                RFQAdditionalInfoPage additionalInfoPage = new RFQAdditionalInfoPage(ref loggedInUser);
                NavigationService.Navigate(additionalInfoPage);
            }
        }

    }
}
