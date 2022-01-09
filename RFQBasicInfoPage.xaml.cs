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
    /// Interaction logic for RFQBasicInfoPage.xaml
    /// </summary>
    public partial class RFQBasicInfoPage : Page
    {
        Employee loggedInUser;
        RFQ rfq;

        
        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;

        
        private List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> companiesList = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> branchInfo = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT> contactInfo = new List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> preSalesEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> salesEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<BASIC_STRUCTS.PROJECT_STRUCT> projects = new List<BASIC_STRUCTS.PROJECT_STRUCT>();

        String[] contactPhones = new String[COMPANY_ORGANISATION_MACROS.MAX_TELEPHONES_PER_CONTACT];

        private int viewAddCondition;
        private int companySerial;
        private int addressSerial;

        public RFQProductsPage rfqProductsPage;
        public RFQAdditionalInfoPage rfqAdditionalInfoPage;
        public RFQUploadFilesPage rfqUploadFilesPage;

        public RFQBasicInfoPage(ref Employee mLoggedInUser, ref RFQ mRFQ, int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            sqlDatabase = new SQLServer();

            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            rfq = mRFQ;

            InitializeComponent();

            if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_ADD_CONDITION)
            {
                ConfigureAddRFQUIElements();

                InitializeSalesPersonCombo();
                InitializeAssigneeCombo();
                InitializeProjectCombo();

                SetSalesPerson();
                salesPersonCombo.IsEnabled = false;
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
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
                SetProjectLabel();

                cancelButton.IsEnabled = false;
            }  
            else
            {
              
                ConfigureAddRFQUIElements();
                InitializeSalesPersonCombo();
                InitializeProjectCombo();

                InitializeAssigneeCombo();

                SetSalesPersonCombo();
                SetAssigneeCombo();
                SetCompanyNameCombo();
                SetProjectCombo();

                salesPersonCombo.IsEnabled = false;
                companyNameCombo.IsEnabled = false;
                companyAddressCombo.IsEnabled = false;
                contactPersonCombo.IsEnabled = false;
                contactPersonPhoneCombo.IsEnabled = false;
                projectCombo.IsEnabled = false;
            }

        }

        /////////////////UI ELEMENTS CINFIGURATION//////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ConfigureAddRFQUIElements()
        {
            //USE COLLAPSED VISIBILITY NOT HIDDEN VISIBILITY
            offerProposerLabel.Visibility = Visibility.Collapsed;
            companyNameLabel.Visibility = Visibility.Collapsed;
            companyAddressLabel.Visibility = Visibility.Collapsed;
            contactPersonNameLabel.Visibility = Visibility.Collapsed;
            contactPersonPhoneLabel.Visibility = Visibility.Collapsed;
            salesPersonLabel.Visibility = Visibility.Collapsed;
            projectLabel.Visibility = Visibility.Collapsed;

            //YOU SHALL MAKE SURE THAT LABELS ARE COLLAPSED AND COMBOS ARE VISIBLE EVERY TIME
            assigneeCombo.Visibility = Visibility.Visible;
            companyNameCombo.Visibility = Visibility.Visible;
            companyAddressCombo.Visibility = Visibility.Visible;
            contactPersonCombo.Visibility = Visibility.Visible;
            contactPersonPhoneCombo.Visibility = Visibility.Visible;
            salesPersonCombo.Visibility = Visibility.Visible;
            projectCombo.Visibility = Visibility.Visible;

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
            projectCombo.Visibility = Visibility.Collapsed;

            offerProposerLabel.Visibility = Visibility.Visible;
            companyNameLabel.Visibility = Visibility.Visible;
            companyAddressLabel.Visibility = Visibility.Visible;
            contactPersonNameLabel.Visibility = Visibility.Visible;
            contactPersonPhoneLabel.Visibility = Visibility.Visible;
            salesPersonLabel.Visibility = Visibility.Visible;
            projectLabel.Visibility = Visibility.Visible;
        }

        ///////////////INITIALIZE FUNCTIONS///////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        private void InitializeCompanyInfo()
        {
            int companySerial = rfq.GetCompanySerial();
            

            if (!commonQueriesObject.GetCompanyAddresses(companySerial, ref branchInfo))
                return;
        }
        private bool InitializeContactInfo()
        {

            if (!commonQueriesObject.GetCompanyContacts(rfq.GetSalesPersonId(), rfq.GetAddressSerial(), ref contactInfo))
                return false;

            return true;
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
            if (!commonQueriesObject.GetEmployeeCompanies(rfq.GetSalesPersonId(), ref companiesList))
                    return false;

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

        private bool InitializeProjectCombo()
        {
            if (!commonQueriesObject.GetClientProjects(ref projects))
                return false;

            for (int i = 0; i < projects.Count; i++)
                projectCombo.Items.Add(projects[i].project_name);

            return true;
        }

        /////////////SET FUNCTIONS////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetSalesPerson()
        {
            rfq.InitializeSalesPersonInfo(loggedInUser.GetEmployeeId());
            SetSalesPersonCombo();
        }
        private void SetSalesPersonCombo()
        {
            salesPersonCombo.SelectedItem = rfq.GetSalesPersonName();
        }
        private void SetAssigneeCombo()
        {
            assigneeCombo.SelectedItem = rfq.GetAssigneeName();
        }

        private void SetCompanyNameCombo()
        {
            companyNameCombo.SelectedItem = rfq.GetRFQCompany().GetCompanyName();
        }
        private void SetContactPersonCombo()
        {
            contactPersonCombo.SelectedItem = rfq.GetRFQContact().GetContactName().ToString();
            //contactPersonCombo.Text = contactInfo[0].contact_name;
        }
        private void SetContactPhoneCombo()
        {
            if(rfq.GetRFQContact().GetNumberOfSavedContactPhones() != 0)
                contactPersonPhoneCombo.SelectedItem = rfq.GetRFQContact().GetContactPhones()[0].ToString();
        }

        private void SetProjectCombo()
        {
            projectCombo.SelectedItem = rfq.GetprojectName();
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
            
            string address;
            
            address = rfq.GetRFQCompany().GetCompanyDistrict() + ", " + rfq.GetRFQCompany().GetCompanyCity() + ", " + rfq.GetRFQCompany().GetCompanyState() + ", " + rfq.GetRFQCompany().GetCompanyCountry();
            companyAddressLabel.Content += address;
            
        }

        private void SetContactPersonLabel()
        {
            contactPersonNameLabel.Content = rfq.GetRFQContact().GetContactName();


            //for (int i = 0; i < contactInfo.Count(); i++)
              //  contactPersonNameLabel.Content += contactInfo[i].contact_name + "\n";
        }

        private void SetContactNumberLabel()
        {
            //for (int i = 0; i < contactPhones.Count(); i++)
              //contactPersonPhoneLabel.Content += contactPhones[i] + "\n";
              if(rfq.GetRFQContact().GetContactPhones()[0] != null)
                contactPersonPhoneLabel.Content = rfq.GetRFQContact().GetContactPhones()[0].ToString();
        }

        private void SetProjectLabel()
        {
            projectLabel.Content = rfq.GetprojectName();
        }

        /////////////SELECTION CHANGED//////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnSelChangedSalesPersonCombo(object sender, SelectionChangedEventArgs e)
        {
            if (loggedInUser.GetEmployeeTeamId() != COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
                rfq.InitializeSalesPersonInfo(salesEmployees[salesPersonCombo.SelectedIndex].employee_id);

            InitializeCompanyNameCombo();

        }
        private void OnSelChangedAssigneeCombo(object sender, SelectionChangedEventArgs e)
        {
            if(assigneeCombo.SelectedItem != null)
                rfq.InitializeAssignedEngineerInfo(preSalesEmployees[assigneeCombo.SelectedIndex].employee_id);
        }
                     
        private void OnSelChangedCompanyNameCombo(object sender, SelectionChangedEventArgs e)
        {
            companyAddressCombo.Items.Clear();

            if (companyNameCombo.SelectedItem != null)
            {
                companyAddressCombo.IsEnabled = true;

                int companySerial = companiesList[companyNameCombo.SelectedIndex].company_serial;

                if (!commonQueriesObject.GetCompanyAddresses(companySerial, ref branchInfo))
                    return;

                for (int i = 0; i < branchInfo.Count; i++)
                {
                    string address;

                    address = branchInfo[i].district + ", " + branchInfo[i].city + ", " + branchInfo[i].state_governorate + ", " + branchInfo[i].country + ".";
                    companyAddressCombo.Items.Add(address);
                }

                companyAddressCombo.SelectedIndex = 0;
                
                rfq.InitializeCompanyInfo(companySerial);
            }
            
        }
        private void OnSelChangedCompanyAddressCombo(object sender, SelectionChangedEventArgs e)
        {
            contactPersonCombo.Items.Clear();

            if (companyAddressCombo.SelectedItem != null)
            {
                contactPersonCombo.IsEnabled = true;
                
                addressSerial = branchInfo[companyAddressCombo.SelectedIndex].address_serial;

                if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_ADD_CONDITION)
                {
                    if (!commonQueriesObject.GetCompanyContacts(loggedInUser.GetEmployeeId(), addressSerial, ref contactInfo))
                        return;
                }
                else
                {
                    if (!commonQueriesObject.GetCompanyContacts(rfq.GetSalesPersonId(), addressSerial, ref contactInfo))
                        return;
                }

                for (int i = 0; i < contactInfo.Count(); i++)
                    contactPersonCombo.Items.Add(contactInfo[i].contact_name);
                
                rfq.InitializeBranchInfo(addressSerial);

                
                contactPersonCombo.SelectedIndex = 0;
            }
        }
        private void OnSelChangedContactPersonCombo(object sender, SelectionChangedEventArgs e)
        {
            contactPersonPhoneCombo.Items.Clear();
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

            
            contactPersonPhoneCombo.SelectedIndex = 0;
            
        }
        private void OnSelChangedContactPersonPhoneCombo(object sender, SelectionChangedEventArgs e)
        {
            //rfq.GetRFQContact().AddNewContactPhone(contactPersonPhoneCombo.SelectedItem.ToString());
        }

        private void OnSelChangedProjectCombo(object sender, SelectionChangedEventArgs e)
        {
            rfq.InitializeProjectInfo(projects[projectCombo.SelectedIndex].project_serial);
        }

        //////////BUTTON CLICKED///////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnClickNextButton(object sender, RoutedEventArgs e)
        {
            rfqProductsPage.rfqBasicInfoPage = this;
            rfqProductsPage.rfqAdditionalInfoPage = rfqAdditionalInfoPage;
            
            if(viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
                rfqProductsPage.rfqUploadFilesPage = rfqUploadFilesPage;

            NavigationService.Navigate(rfqProductsPage);
        }

        private void OnBtnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            rfqProductsPage.rfqBasicInfoPage = this;
            rfqProductsPage.rfqAdditionalInfoPage = rfqAdditionalInfoPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
                rfqProductsPage.rfqUploadFilesPage = rfqUploadFilesPage;

            NavigationService.Navigate(rfqProductsPage);
        }

        private void OnBtnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            rfqAdditionalInfoPage.rfqBasicInfoPage = this;
            rfqAdditionalInfoPage.rfqProductsPage = rfqProductsPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
                rfqAdditionalInfoPage.rfqUploadFilesPage = rfqUploadFilesPage;

            NavigationService.Navigate(rfqAdditionalInfoPage);
        }

        private void OnBtnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
            {
                rfqUploadFilesPage.rfqBasicInfoPage = this;
                rfqUploadFilesPage.rfqProductsPage = rfqProductsPage;
                rfqUploadFilesPage.rfqAdditionalInfoPage = rfqAdditionalInfoPage;

                NavigationService.Navigate(rfqUploadFilesPage);
            }
        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;
            currentWindow.Close();
        }

        
    }
}
