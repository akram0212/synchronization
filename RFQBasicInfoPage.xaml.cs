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
        Employee loggedInUser;
        RFQ rfq;

        //NO NEED FOR THESE OBJECTS, RFQ OBJECT ALREADY CONTAINS A CONTACT OBJECT WHICH INHERETS COMPANY CLASS
        //Company company;
        //Contact contact;

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;

        //I MADE A STRUCT FOR THIS LIST
        //AND A QUERY FUNCTION IN COMMONQUERIES
        private List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> companiesList = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> branchInfo = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT> contactInfo = new List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> preSalesEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> salesEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsList;

        String[] contactPhones = new String[COMPANY_ORGANISATION_MACROS.MAX_TELEPHONES_PER_CONTACT];

        private int viewAddCondition;

        //NICE WORK HERE, I LIKE YOUR WORK
        //YOUR CODING STYLE IS GETTING BETTER

        //PLEASE DONT CHANGE/ADD STYLES, WE SHALL STICK TO THE STYLES USED BY ALL THE ERP SYSTEM
        public RFQBasicInfoPage(ref Employee mLoggedInUser, ref RFQ mRFQ, int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            InitializeComponent();

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();
            rfq = new RFQ(sqlDatabase);
            rfq = mRFQ;

            if (viewAddCondition == 1)
            {
                ConfigureAddRFQUIElements();

                InitializeSalesPersonCombo();
                InitializeCompanyNameCombo();
                InitializeAssigneeCombo();

                SetSalesPerson();

                if (rfq.GetSalesPersonName() != null)
                    salesPersonCombo.Text = rfq.GetSalesPersonName();
                if (rfq.GetAssigneeName() != null)
                    assigneeCombo.Text = rfq.GetAssigneeName();
                if (rfq.GetRFQCompany().GetCompanyName() != null)
                    companyNameCombo.Text = rfq.GetRFQCompany().GetCompanyName();

                
            }
            else
            {
                InitializeCompanyInfo();
                InitializeContactInfo();

                ConfigureViewRFQUIElements();

                SetSalesPersonLabel();
                SetAssigneeLabel();
                SetCompanyNameLabel();
                SetCompanyAddressLabel();
                SetContactPersonLabel();
                SetContactNumberLabel();
            }  
        }

        /////////////////UI ELEMENTS CINFIGURATION//////////////
        ////////////////////////////////////////////////////////
        private void ConfigureAddRFQUIElements()
        {
            //USE COLLAPSED VISIBILITY NOT HIDDEN VISIBILITY
            offerProposerLabel.Visibility = Visibility.Collapsed;
            companyNameLabel.Visibility = Visibility.Collapsed;
            companyAddressLabel.Visibility = Visibility.Collapsed;
            contactPersonNameLabel.Visibility = Visibility.Collapsed;
            contactPersonPhoneLabel.Visibility = Visibility.Collapsed;
            salesPersonLabel.Visibility = Visibility.Collapsed;

            //YOU SHALL MAKE SURE THAT LABELS ARE COLLAPSED AND COMBOS ARE VISIBLE EVERY TIME
            assigneeCombo.Visibility = Visibility.Visible;
            companyNameCombo.Visibility = Visibility.Visible;
            companyAddressCombo.Visibility = Visibility.Visible;
            contactPersonCombo.Visibility = Visibility.Visible;
            contactPersonPhoneCombo.Visibility = Visibility.Visible;
            salesPersonCombo.Visibility = Visibility.Visible;

            companyAddressCombo.IsEnabled = false;
            contactPersonCombo.IsEnabled = false;
            contactPersonPhoneCombo.IsEnabled = false;
        }

        private void ConfigureViewRFQUIElements()
        {
            assigneeCombo.Visibility = Visibility.Collapsed;
            companyNameCombo.Visibility = Visibility.Collapsed;
            companyAddressCombo.Visibility = Visibility.Collapsed;
            contactPersonCombo.Visibility = Visibility.Collapsed;
            contactPersonPhoneCombo.Visibility = Visibility.Collapsed;
            salesPersonCombo.Visibility = Visibility.Collapsed;

            offerProposerLabel.Visibility = Visibility.Visible;
            companyNameLabel.Visibility = Visibility.Visible;
            companyAddressLabel.Visibility = Visibility.Visible;
            contactPersonNameLabel.Visibility = Visibility.Visible;
            contactPersonPhoneLabel.Visibility = Visibility.Visible;
            salesPersonLabel.Visibility = Visibility.Visible;
        }

        ///////////////INITIALIZE FUNCTIONS///////////////
        //////////////////////////////////////////////////
        
        private void InitializeCompanyInfo()
        {
            int companySerial = rfq.GetCompanySerial();
            

            if (!commonQueriesObject.GetCompanyAddresses(companySerial, ref branchInfo))
                return;
        }
        private void InitializeContactInfo()
        {
            if (!commonQueriesObject.GetCompanyContacts(loggedInUser.GetEmployeeId(),rfq.GetAddressSerial(), ref contactInfo))
                return;
        }
        

        private bool InitializeSalesPersonCombo()
        {
            if (!commonQueriesObject.GetTeamEmployees(COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID, ref salesEmployees))
                return false;
            for (int i = 0; i < salesEmployees.Count(); i++)
                salesPersonCombo.Items.Add(salesEmployees[i].employee_name);

            return true;
        }

        //ANY FUNCTION THAT ACCESS A DATABASE MUST BE BOOL NOT VOID
        private bool InitializeCompanyNameCombo()
        {
            if (!commonQueriesObject.GetEmployeeCompanies(loggedInUser.GetEmployeeId(), ref companiesList))
                return false;

            //NO NEED FOR TEMP VARIABLES
            for (int i = 0; i < companiesList.Count(); i++)
                companyNameCombo.Items.Add(companiesList[i].company_name);

            return true;
        }

        private bool InitializeAssigneeCombo()
        {
            if (!commonQueriesObject.GetTeamEmployees(COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID, ref preSalesEmployees))
                return false;

            //NO NEED FOR TEMP VARIABLES
            for (int i = 0; i < preSalesEmployees.Count(); i++)
                assigneeCombo.Items.Add(preSalesEmployees[i].employee_name);

            return true;
        }

        /////////////SET FUNCTIONS////////////////
        //////////////////////////////////////////

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
            //YOU SHALL ACCESS THE COMPANY INFO LIKE THIS
            companyNameLabel.Content = rfq.GetRFQContact().GetCompanyName();
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
            contactPersonNameLabel.Content = null;

            for (int i = 0; i < contactInfo.Count(); i++)
                contactPersonNameLabel.Content += contactInfo[i].contact_name + "\n";
        }

        private void SetContactNumberLabel()
        {
            //for (int i = 0; i < contactPhones.Count(); i++)
              //contactPersonPhoneLabel.Content += contactPhones[i] + "\n";
              if(rfq.GetRFQContact().GetContactPhones()[0] != null)
                contactPersonPhoneLabel.Content = rfq.GetRFQContact().GetContactPhones()[0].ToString();
        }

        /////////////SELECTION CHANGED//////////////
        ////////////////////////////////////////////
        private void salesPersonComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
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
                    contactPersonCombo.Items.Add(contactInfo[i].contact_name);
                

                if (contactInfo.Count() == 1)
                    contactPersonCombo.Items.GetItemAt(0);
                rfq.InitializeBranchInfo(addressSerial);
            }
        }

        private void ContactPersonComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contactPersonCombo.SelectedItem != null)
            {
                contactPersonPhoneCombo.IsEnabled = true;
                rfq.InitializeContactInfo(contactInfo[contactPersonCombo.SelectedIndex].contact_id);
            }

            //YOU SHALL ACCESS THE CONTACT PHONE LIKE THIS
            for (int i = 0; i < rfq.GetRFQContact().GetNumberOfSavedContactPhones(); i++)
            {
                contactPersonPhoneCombo.Items.Add(rfq.GetRFQContact().GetContactPhones()[i]);
                contactPhones[i] = rfq.GetRFQContact().GetContactPhones()[i];
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
            RFQProductsPage productsPage = new RFQProductsPage(ref loggedInUser, ref rfq, viewAddCondition);
            NavigationService.Navigate(productsPage);
        }

        private void OnClickAdditionalInfo(object sender, RoutedEventArgs e)
        {
            RFQAdditionalInfoPage additionalInfoPage = new RFQAdditionalInfoPage(ref loggedInUser, ref rfq, viewAddCondition);
            NavigationService.Navigate(additionalInfoPage);
        }

    }
}
