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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for MaintOffersBasicInfoPage.xaml
    /// </summary>
    public partial class MaintOffersBasicInfoPage : Page
    {
        Employee loggedInUser;
        MaintenanceOffer maintOffer;


        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;


        private List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> companiesList = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> branchInfo = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT> contactInfo = new List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> preSalesEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> salesEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsList = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsAddedToComboList = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();
        private List<BASIC_STRUCTS.PROJECT_STRUCT> projects = new List<BASIC_STRUCTS.PROJECT_STRUCT>();

        String[] contactPhones = new String[COMPANY_ORGANISATION_MACROS.MAX_TELEPHONES_PER_CONTACT];

        private int viewAddCondition;
        private int companySerial;
        private int addressSerial;

        public MaintOffersProductsPage maintOffersProductsPage;
        public MaintOffersPaymentAndDeliveryPage maintOffersPaymentAndDeliveryPage;
        public MaintOffersAdditionalInfoPage maintOffersAdditionalInfoPage;
        public MaintOffersUploadFilesPage maintOffersUploadFilesPage;
        public MaintOffersBasicInfoPage(ref Employee mLoggedInUser, ref MaintenanceOffer mMaintOffer, int mViewAddCondition, ref MaintOffersProductsPage mMaintOffersProductsPage)
        {
            maintOffersProductsPage = mMaintOffersProductsPage;

            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            sqlDatabase = new SQLServer();

            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            maintOffer = mMaintOffer;

            InitializeComponent();

            if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_ADD_CONDITION)
            {
                ConfigureAddMaintOfferUIElements();
                SetOfferProposer();

                //InitializeOfferProposerCombo();
                InitializeProjectCombo();

                //offerProposerCombo.IsEnabled = false;

            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                InitializeCompanyInfo();
                InitializeContactInfo();

                ConfigureViewMaintOfferUIElements();

                SetCompanyNameLabel();
                SetCompanyAddressLabel();
                SetContactPersonLabel();
                SetContactNumberLabel();
                SetProjectLabel();

                cancelButton.IsEnabled = false;
            }
            else
            {

                ConfigureAddMaintOfferUIElements();
                InitializeProjectCombo();

                SetCompanyNameCombo();
                SetProjectCombo();

                //offerProposerCombo.IsEnabled = false;
                projectCombo.IsEnabled = false;
            }
        }
        private void ConfigureAddMaintOfferUIElements()
        {
            //USE COLLAPSED VISIBILITY NOT HIDDEN VISIBILITY
            //offerProposerLabel.Visibility = Visibility.Collapsed;
            salesPersonLabel.Visibility = Visibility.Collapsed;
            companyNameLabel.Visibility = Visibility.Collapsed;
            companyAddressLabel.Visibility = Visibility.Collapsed;
            contactPersonNameLabel.Visibility = Visibility.Collapsed;
            contactPersonPhoneLabel.Visibility = Visibility.Collapsed;
            projectLabel.Visibility = Visibility.Collapsed;

            //YOU SHALL MAKE SURE THAT LABELS ARE COLLAPSED AND COMBOS ARE VISIBLE EVERY TIME
            companyNameCombo.Visibility = Visibility.Visible;
            companyAddressCombo.Visibility = Visibility.Visible;
            contactPersonNameCombo.Visibility = Visibility.Visible;
            contactPersonPhoneCombo.Visibility = Visibility.Visible;
            //offerProposerCombo.Visibility = Visibility.Visible;
            salesPersonCombo.Visibility = Visibility.Visible;
            projectCombo.Visibility = Visibility.Visible;

            companyAddressCombo.IsEnabled = false;
            contactPersonNameCombo.IsEnabled = false;
            contactPersonPhoneCombo.IsEnabled = false;
        }

        private void ConfigureViewMaintOfferUIElements()
        {
            companyNameCombo.Visibility = Visibility.Collapsed;
            companyAddressCombo.Visibility = Visibility.Collapsed;
            contactPersonNameCombo.Visibility = Visibility.Collapsed;
            contactPersonPhoneCombo.Visibility = Visibility.Collapsed;
            //offerProposerCombo.Visibility = Visibility.Collapsed;
            salesPersonCombo.Visibility = Visibility.Collapsed;
            projectCombo.Visibility = Visibility.Collapsed;

            //offerProposerLabel.Visibility = Visibility.Visible;
            companyNameLabel.Visibility = Visibility.Visible;
            companyAddressLabel.Visibility = Visibility.Visible;
            contactPersonNameLabel.Visibility = Visibility.Visible;
            contactPersonPhoneLabel.Visibility = Visibility.Visible;
            salesPersonLabel.Visibility = Visibility.Visible;
            projectLabel.Visibility = Visibility.Visible;
        }
        ///////////////INITIALIZE FUNCTIONS///////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool FillrfqsList()
        {
            if (!commonQueriesObject.GetRFQs(ref rfqsList))
                return false;
            return true;
        }

        private void InitializeRFQSerialCombo()
        {
            for (int i = 0; i < rfqsList.Count; i++)
            {
                //if (rfqsList[i].sales_person_id == salesPersonID && rfqsList[i].assignee_id == loggedInUser.GetEmployeeId())
                //{
                  //  RFQSerialCombo.Items.Add(rfqsList[i].rfq_id);
                   // rfqsAddedToComboList.Add(rfqsList[i]);
                //}
            }
            RFQSerialCombo.IsEnabled = true;
        }
        private void InitializeCompanyInfo()
        {
            int companySerial = maintOffer.GetCompanySerial();


            if (!commonQueriesObject.GetCompanyAddresses(companySerial, ref branchInfo))
                return;
        }
        private bool InitializeContactInfo()
        {

            if (!commonQueriesObject.GetCompanyContacts(maintOffer.GetMaintOfferProposerId(), maintOffer.GetAddressSerial(), ref contactInfo))
                return false;

            return true;
        }

        private bool InitializeCompanyNameCombo()
        {
            if (!commonQueriesObject.GetEmployeeCompanies(maintOffer.GetMaintOfferProposerId(), ref companiesList))
                return false;

            for (int i = 0; i < companiesList.Count(); i++)
                companyNameCombo.Items.Add(companiesList[i].company_name);

            return true;
        }

        private bool InitializeSalesPersonCombo()
        {
            if (!commonQueriesObject.GetTeamEmployees(COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID, ref salesEmployees))
                return false;

            for (int i = 0; i < salesEmployees.Count(); i++)
            {
                salesPersonCombo.Items.Add(salesEmployees[i].employee_name);
            }
            salesPersonCombo.Items.Add(loggedInUser.GetEmployeeName());
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

        private void SetOfferProposer()
        {
            maintOffer.InitializeMaintOfferProposerInfo(loggedInUser.GetEmployeeId(), loggedInUser.GetEmployeeTeamId());
            SetOfferProposerCombo();
        }
        private void SetOfferProposerCombo()
        {
              //offerProposerCombo.SelectedItem = maintOffer.GetMaintOfferProposerName();
        }

        private void SetCompanyNameCombo()
        {
            companyNameCombo.SelectedItem = maintOffer.GetMaintOfferContact().GetCompanyName();
        }
        private void SetcontactPersonNameCombo()
        {
            contactPersonNameCombo.SelectedItem = maintOffer.GetMaintOfferContact().GetContactName().ToString();
            contactPersonNameCombo.Text = contactInfo[0].contact_name;
        }
        private void SetContactPhoneCombo()
        {
           if (maintOffer.GetMaintOfferContact().GetNumberOfSavedContactPhones() != 0)
               contactPersonPhoneCombo.SelectedItem = maintOffer.GetMaintOfferContact().GetContactPhones()[0].ToString();
        }

        private void SetProjectCombo()
        {
            projectCombo.SelectedItem = maintOffer.GetMaintOfferProjectName();
        }

        private void SetOfferProposerLabel()
        {
            //offerProposerLabel.Content = maintOffer.GetMaintOfferProposerName();
        }

        private void SetCompanyNameLabel()
        {
            //YOU SHALL ACCESS THE COMPANY INFO LIKE THIS
            companyNameLabel.Content = maintOffer.GetMaintOfferContact().GetCompanyName();
        }

        private void SetCompanyAddressLabel()
        {
            string address;

            address = maintOffer.GetMaintOfferContact().GetCompanyDistrict() + ", " + maintOffer.GetMaintOfferContact().GetCompanyCity() + ", " + maintOffer.GetMaintOfferContact().GetCompanyState() + ", " + maintOffer.GetMaintOfferContact().GetCompanyCountry();
            companyAddressLabel.Content += address;
        }

        private void SetContactPersonLabel()
        {
            //contactPersonNameLabel.Content = maintOffer.GetMaintOfferContact().GetContactName();


            //for (int i = 0; i < contactInfo.Count(); i++)
            //  contactPersonNameLabel.Content += contactInfo[i].contact_name + "\n";
        }

        private void SetContactNumberLabel()
        {
            //for (int i = 0; i < contactPhones.Count(); i++)
            //contactPersonPhoneLabel.Content += contactPhones[i] + "\n";
            //if (maintOffer.GetMaintOfferContact().GetContactPhones()[0] != null)
            //    contactPersonPhoneLabel.Content = maintOffer.GetMaintOfferContact().GetContactPhones()[0].ToString();
        }

        private void SetProjectLabel()
        {
            //projectLabel.Content = maintOffer.GetprojectName();
        }

        /////////////SELECTION CHANGED//////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnSelChangedOfferProposerCombo(object sender, SelectionChangedEventArgs e)
        {
            //if (loggedInUser.GetEmployeeTeamId() != COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
                //maintOffer.InitializeMaintOfferProposerInfo(salesEmployees[offerProposerCombo.SelectedIndex].employee_id, salesEmployees[offerProposerCombo.SelectedIndex].team.team_id);

            InitializeCompanyNameCombo();

        }

        private void OnSelChangedSalesPersonCombo(object sender, SelectionChangedEventArgs e)
        {

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

                maintOffer.InitializeCompanyInfo(companySerial);
            }

        }
        private void OnSelChangedCompanyAddressCombo(object sender, SelectionChangedEventArgs e)
        {
            contactPersonNameCombo.Items.Clear();

            if (companyAddressCombo.SelectedItem != null)
            {
                contactPersonNameCombo.IsEnabled = true;

                addressSerial = branchInfo[companyAddressCombo.SelectedIndex].address_serial;

                if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_ADD_CONDITION)
                {
                    if (!commonQueriesObject.GetCompanyContacts(loggedInUser.GetEmployeeId(), addressSerial, ref contactInfo))
                        return;
                }
                else
                {
                    if (!commonQueriesObject.GetCompanyContacts(maintOffer.GetMaintOfferProposerId(), addressSerial, ref contactInfo))
                        return;
                }

                for (int i = 0; i < contactInfo.Count(); i++)
                    contactPersonNameCombo.Items.Add(contactInfo[i].contact_name);

                maintOffer.InitializeBranchInfo(addressSerial);


                contactPersonNameCombo.SelectedIndex = 0;
            }
        }
        private void OnSelChangedContactPersonNameCombo(object sender, SelectionChangedEventArgs e)
        {
            contactPersonPhoneCombo.Items.Clear();
            if (contactPersonNameCombo.SelectedItem != null)
            {
                contactPersonPhoneCombo.IsEnabled = true;
                maintOffer.InitializeContactInfo(contactInfo[contactPersonNameCombo.SelectedIndex].contact_id);
            }

            //YOU SHALL ACCESS THE CONTACT PHONE LIKE THIS
            for (int i = 0; i < maintOffer.GetMaintOfferContact().GetNumberOfSavedContactPhones(); i++)
            {
                contactPersonPhoneCombo.Items.Add(maintOffer.GetMaintOfferContact().GetContactPhones()[i]);
                contactPhones[i] = maintOffer.GetMaintOfferContact().GetContactPhones()[i];
            }


            contactPersonPhoneCombo.SelectedIndex = 0;

        }
        private void OnSelChangedContactPersonPhoneCombo(object sender, SelectionChangedEventArgs e)
        {
            maintOffer.GetMaintOfferContact().AddNewContactPhone(contactPersonPhoneCombo.SelectedItem.ToString());
        }

        private void OnSelChangedProjectCombo(object sender, SelectionChangedEventArgs e)
        {
            maintOffer.InitializeProjectInfo(projects[projectCombo.SelectedIndex].project_serial);
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            maintOffersProductsPage.maintOffersBasicInfoPage = this;
            maintOffersProductsPage.maintOffersPaymentAndDeliveryPage = maintOffersPaymentAndDeliveryPage;
            maintOffersProductsPage.maintOffersAdditionalInfoPage = maintOffersAdditionalInfoPage;
            maintOffersProductsPage.maintOffersUploadFilesPage = maintOffersUploadFilesPage;


            NavigationService.Navigate(maintOffersProductsPage);
        }
        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {
            maintOffersPaymentAndDeliveryPage.maintOffersBasicInfoPage = this;
            maintOffersPaymentAndDeliveryPage.maintOffersProductsPage = maintOffersProductsPage;
            maintOffersPaymentAndDeliveryPage.maintOffersAdditionalInfoPage = maintOffersAdditionalInfoPage;
            maintOffersPaymentAndDeliveryPage.maintOffersUploadFilesPage = maintOffersUploadFilesPage;

            NavigationService.Navigate(maintOffersPaymentAndDeliveryPage);
        }
        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            maintOffersAdditionalInfoPage.maintOffersBasicInfoPage = this;
            maintOffersAdditionalInfoPage.maintOffersProductsPage = maintOffersProductsPage;
            maintOffersAdditionalInfoPage.maintOffersPaymentAndDeliveryPage = maintOffersPaymentAndDeliveryPage;
            maintOffersAdditionalInfoPage.maintOffersUploadFilesPage = maintOffersUploadFilesPage;

            NavigationService.Navigate(maintOffersAdditionalInfoPage);
        }
        private void OnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
            {
                maintOffersUploadFilesPage.maintOffersBasicInfoPage = this;
                maintOffersUploadFilesPage.maintOffersProductsPage = maintOffersProductsPage;
                maintOffersUploadFilesPage.maintOffersPaymentAndDeliveryPage = maintOffersPaymentAndDeliveryPage;
                maintOffersUploadFilesPage.maintOffersAdditionalInfoPage = maintOffersAdditionalInfoPage;

                NavigationService.Navigate(maintOffersUploadFilesPage);
            }
        }


        //////////BUTTON CLICKED///////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnBtnClickNext(object sender, RoutedEventArgs e)
        {
            maintOffersProductsPage.maintOffersBasicInfoPage = this;
            maintOffersProductsPage.maintOffersAdditionalInfoPage = maintOffersAdditionalInfoPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
                maintOffersProductsPage.maintOffersUploadFilesPage = maintOffersUploadFilesPage;

            NavigationService.Navigate(maintOffersProductsPage);
        }
        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;
            currentWindow.Close();
        }
        private void OnButtonClickAutomateMaintOffer(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
