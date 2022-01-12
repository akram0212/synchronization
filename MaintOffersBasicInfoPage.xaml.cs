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
        private List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> companiesAddedToComboList = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> branchInfo = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT> contactInfo = new List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> employeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsList = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsAddedToComboList = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();

        private List<BASIC_STRUCTS.PROJECT_STRUCT> projects = new List<BASIC_STRUCTS.PROJECT_STRUCT>();


        private int viewAddCondition;
        private int salesPersonID;
        private int salesPersonTeamID;

        public MaintOffersProductsPage maintOffersProductsPage;
        public MaintOffersPaymentAndDeliveryPage maintOffersPaymentAndDeliveryPage;
        public MaintOffersAdditionalInfoPage maintOffersAdditionalInfoPage;
        public MaintOffersUploadFilesPage maintOffersUploadFilesPage;

        public MaintOffersBasicInfoPage(ref Employee mLoggedInUser, ref MaintenanceOffer mMaintOffers, int mViewAddCondition, ref MaintOffersProductsPage mMaintOffersProductsPage)
        {
            maintOffersProductsPage = mMaintOffersProductsPage;

            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            maintOffer = mMaintOffers;

            InitializeComponent();

            //if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_ADD_CONDITION || viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_RESOLVE_CONDITION)
            //{
            //    maintOffer.InitializeOfferProposerInfo(loggedInUser.GetEmployeeId(), loggedInUser.GetEmployeeTeamId());
            //    if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_REVISE_CONDITION)
            //        if (!maintOffer.GetNewOfferSerial())
            //            return;
            //    if(!maintOffer.GetNewOfferVersion())
            //        return;
            //    maintOffer.SetOfferIssueDateToToday();
            //    maintOffer.GetNewOfferID();
            //    
            //}
            //
            //if(viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_REVISE_CONDITION)
            //{
            //    if (!maintOffer.GetNewOfferVersion())
            //        return;
            //    maintOffer.SetOfferIssueDateToToday();
            //    maintOffer.GetNewOfferID();
            //}

            if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_ADD_CONDITION)
            {
                FillrfqsList();
                ConfigureUIElemenetsForAdd();
                InitializeSalesPersonCombo();
                InitializeProjectCombo();
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                FillrfqsList();
                ConfigureUIElementsForView();

                SetSalesPersonLabel();
                SetRFQSerialLabel();
                SetCompanyNameLabel();
                SetCompanyAddressLabel();
                SetContactPersonLabel();
                SetProjectLabel();


                maintOffersUploadFilesPage = new MaintOffersUploadFilesPage(ref loggedInUser, ref maintOffer, viewAddCondition);
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_REVISE_CONDITION)
            {
                FillrfqsList();
                ConfigureUIElemenetsForAdd();
                InitializeSalesPersonCombo();
                InitializeProjectCombo();

                SetSalesPersonComboValue();
                SetProjectCombo();
                projectSerialCombo.IsEnabled = false;

                //CHANGE CONDITIONS
                if (RFQSerialCombo.IsEnabled == true)
                    SetRFQSerialComboValue();
                else
                {
                    SetCompanyNameComboValue();
                    SetContactPersonComboValue();
                }
                DisableSalesPersonAndRFQCombo();
            }
            else
            {
                FillrfqsList();
                ConfigureUIElemenetsForAdd();
                InitializeSalesPersonCombo();
                InitializeProjectCombo();

                SetSalesPersonComboValue();
                //InitializeRFQSerialCombo();
                SetRFQSerialComboValue();
                SetProjectCombo();
                projectSerialCombo.IsEnabled = false;

                DisableSalesPersonAndRFQCombo();
            }

        }

        public MaintOffersBasicInfoPage(ref MaintenanceOffer mMaintOffers)
        {
            maintOffer = mMaintOffers;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////INITIALIZE FUNCTIONS///////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool InitializeSalesPersonCombo()
        {
            if (!commonQueriesObject.GetTeamEmployees(COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID, ref employeesList))
                return false;

            for (int i = 0; i < employeesList.Count(); i++)
            {
                salesPersonCombo.Items.Add(employeesList[i].employee_name);
            }
            salesPersonCombo.Items.Add(loggedInUser.GetEmployeeName());
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
            for (int i = 0; i < rfqsList.Count; i++)
            {/////////////////////
             ////THE NUMBER 5 IS FOR CONTRACT TYPE MAINTENANCE ON REQUEST (SAMEH)
             /////////////////////
                if (rfqsList[i].sales_person_id == salesPersonID && rfqsList[i].assignee_id == loggedInUser.GetEmployeeId() && rfqsList[i].contract_type_id == 5)
                {
                    RFQSerialCombo.Items.Add(rfqsList[i].rfq_id);
                    rfqsAddedToComboList.Add(rfqsList[i]);
                }
            }
            RFQSerialCombo.IsEnabled = true;
        }

        private void InitializeCompanyNameCombo()
        {
            if (!commonQueriesObject.GetEmployeeCompanies(salesPersonID, ref companiesList))
                return;

            for (int i = 0; i < companiesList.Count; i++)
            {
                companyNameCombo.Items.Add(companiesList[i].company_name);
                companiesAddedToComboList.Add(companiesList[i]);
            }

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

            maintOffer.InitializeBranchInfo(branchInfo[0].address_serial);

            companyAddressCombo.SelectedIndex = 0;

            companyAddressCombo.IsEnabled = true;
        }

        private void InitializeCompanyContactCombo()
        {
            int addressSerial = branchInfo[companyAddressCombo.SelectedIndex].address_serial;

            if (!commonQueriesObject.GetCompanyContacts(salesPersonID, addressSerial, ref contactInfo))
                return;

            for (int i = 0; i < contactInfo.Count(); i++)
                contactPersonNameCombo.Items.Add(contactInfo[i].contact_name);

            if (contactInfo.Count > 0)
                contactPersonNameCombo.SelectedIndex = 0;

            contactPersonNameCombo.IsEnabled = true;
        }

        private bool InitializeProjectCombo()
        {

            if (!commonQueriesObject.GetClientProjects(ref projects))
                return false;

            projectSerialCombo.Items.Clear();

            for (int i = 0; i < projects.Count; i++)
            {
                projectSerialCombo.Items.Add(projects[i].project_name);
            }

            return true;
        }



        ///////////CONFIGURE UI ELEMENTS////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ConfigureUIElemenetsForAdd()
        {
            salesPersonLabel.Visibility = Visibility.Collapsed;
            RFQSerialLabel.Visibility = Visibility.Collapsed;
            companyNameLabel.Visibility = Visibility.Collapsed;
            companyAddressLabel.Visibility = Visibility.Collapsed;
            contactPersonNameLabel.Visibility = Visibility.Collapsed;
            projectSerialLabel.Visibility = Visibility.Collapsed;

            salesPersonCombo.Visibility = Visibility.Visible;
            RFQSerialCombo.Visibility = Visibility.Visible;
            companyNameCombo.Visibility = Visibility.Visible;
            companyAddressCombo.Visibility = Visibility.Visible;
            contactPersonNameCombo.Visibility = Visibility.Visible;
            projectSerialCombo.Visibility = Visibility.Visible;

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
            projectSerialLabel.Visibility = Visibility.Visible;

            salesPersonCombo.Visibility = Visibility.Collapsed;
            RFQSerialCombo.Visibility = Visibility.Collapsed;
            companyNameCombo.Visibility = Visibility.Collapsed;
            companyAddressCombo.Visibility = Visibility.Collapsed;
            contactPersonNameCombo.Visibility = Visibility.Collapsed;
            projectSerialCombo.Visibility = Visibility.Collapsed;


        }
        private void DisableSalesPersonAndRFQCombo()
        {
            salesPersonCombo.IsEnabled = false;
            RFQSerialCombo.IsEnabled = false;
        }
        private void EnableSalesPersonAndRFQCombo()
        {
            salesPersonCombo.IsEnabled = true;
            RFQSerialCombo.IsEnabled = true;

        }
        private void DisableCompanyNameaddressContactCombos()
        {
            companyNameCombo.IsEnabled = false;
            companyAddressCombo.IsEnabled = false;
            contactPersonNameCombo.IsEnabled = false;
        }

        private void EnableCompanyNameAddressContactCombos()
        {
            companyNameCombo.IsEnabled = true;
            companyAddressCombo.IsEnabled = true;
            contactPersonNameCombo.IsEnabled = true;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////SET FUNCTIONS////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SetSalesPersonLabel()
        {
            salesPersonLabel.Content = maintOffer.GetSalesPersonName();
        }

        private void SetSalesPersonComboValue()
        {
            salesPersonCombo.SelectedItem = maintOffer.GetSalesPersonName();
        }

        private void SetRFQSerialLabel()
        {
            RFQSerialLabel.Content = maintOffer.GetRFQID();
        }

        private void SetRFQSerialComboValue()
        {
            RFQSerialCombo.SelectedItem = maintOffer.GetRFQID();
        }

        private void SetCompanyNameLabel()
        {
            companyNameLabel.Content = maintOffer.GetCompanyName();
        }

        private void SetCompanyNameComboValue()
        {
            companyNameCombo.Text = maintOffer.GetCompanyName();
        }

        private void SetCompanyAddressLabel()
        {
            if (!commonQueriesObject.GetCompanyAddresses(maintOffer.GetCompanySerial(), ref branchInfo))
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
            contactPersonNameLabel.Content = maintOffer.GetContactName();
        }

        private void SetContactPersonComboValue()
        {
            contactPersonNameCombo.Text = maintOffer.GetContactName();
        }

        private void SetCompanyNameAddressContactFromRFQ()
        {

            InitializeCompanyNameCombo();


            companyNameCombo.SelectedItem = rfqsAddedToComboList[RFQSerialCombo.SelectedIndex].company_name;


            if (!commonQueriesObject.GetCompanyAddresses(companiesList[companyNameCombo.SelectedIndex].company_serial, ref branchInfo))
                return;

            for (int i = 0; i < branchInfo.Count; i++)
            {
                string address;
                address = branchInfo[i].district + ", " + branchInfo[i].city + ", " + branchInfo[i].state_governorate + ", " + branchInfo[i].country + ".";
                companyAddressCombo.Items.Add(address);
                companyAddressCombo.SelectedIndex = 0;
            }

            //InitializeCompanyContactCombo();

            contactPersonNameCombo.Items.Add(rfqsAddedToComboList[RFQSerialCombo.SelectedIndex].contact_name);
            contactPersonNameCombo.SelectedItem = rfqsAddedToComboList[RFQSerialCombo.SelectedIndex].contact_name;

            DisableCompanyNameaddressContactCombos();
        }

        private void SetProjectCombo()
        {
            projectSerialCombo.SelectedItem = maintOffer.GetprojectName();
        }

        private void SetProjectLabel()
        {
            projectSerialLabel.Content = maintOffer.GetprojectName();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////SELECTION CHANGED HANDLERS///////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void OnSelChangedSalesPersonCombo(object sender, SelectionChangedEventArgs e)
        {
            RFQSerialCombo.Items.Clear();
            companyNameCombo.Items.Clear();
            rfqsAddedToComboList.Clear();
            RFQSerialCombo.SelectedIndex = -1;
            companyNameCombo.SelectedIndex = -1;

            if (salesPersonCombo.SelectedIndex != employeesList.Count())
            {
                salesPersonID = employeesList[salesPersonCombo.SelectedIndex].employee_id;
                salesPersonTeamID = employeesList[salesPersonCombo.SelectedIndex].team.team_id;
            }
            else
            {
                salesPersonID = loggedInUser.GetEmployeeId();
                salesPersonTeamID = COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID;
            }

            if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_ADD_CONDITION)
                maintOffer.ResetMaintOfferInfo(salesPersonTeamID);

            maintOffer.InitializeMaintOfferProposerInfo(loggedInUser.GetEmployeeId(), salesPersonTeamID);

            if (salesPersonTeamID == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
            {
                maintOffer.InitializeSalesPersonInfo(salesPersonID);
                InitializeRFQSerialCombo();
                projectSerialCombo.IsEnabled = false;
                companyNameCombo.SelectedItem = null;
            }
            else
            {
                maintOffer.InitializeSalesPersonInfo(loggedInUser.GetEmployeeId());
                InitializeCompanyNameCombo();
                RFQSerialCombo.SelectedItem = null;
                RFQSerialCombo.IsEnabled = false;
            }
        }
        private void OnSelChangedRFQSerialCombo(object sender, SelectionChangedEventArgs e)
        {
            if (RFQSerialCombo.SelectedItem != null)
            {
                maintOffer.InitializeRFQInfo(rfqsAddedToComboList[RFQSerialCombo.SelectedIndex].rfq_serial, rfqsAddedToComboList[RFQSerialCombo.SelectedIndex].rfq_version);
                if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_REVISE_CONDITION)
                    maintOffer.LinkRFQInfo();

                SetCompanyNameAddressContactFromRFQ();

                if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
                {
                    maintOffersProductsPage.SetCategoryComboBoxes();
                    maintOffersProductsPage.SetTypeComboBoxes();
                    maintOffersProductsPage.SetBrandComboBoxes();
                    maintOffersProductsPage.SetModelComboBoxes();
                    maintOffersProductsPage.SetQuantityTextBoxes();
                }
                else
                {
                    maintOffersProductsPage.SetCategoryLabels();
                    maintOffersProductsPage.SetTypeLabels();
                    maintOffersProductsPage.SetBrandLabels();
                    maintOffersProductsPage.SetModelLabels();
                    maintOffersProductsPage.SetQuantityTextBoxes();
                }

                SetProjectCombo();
                projectSerialCombo.IsEnabled = false;
            }
        }
        private void OnSelChangedCompanyNameCombo(object sender, SelectionChangedEventArgs e)
        {
            companyAddressCombo.Items.Clear();

            if (companyNameCombo.SelectedItem != null)
            {
                InitializeCompanyAddressCombo();
                maintOffer.InitializeCompanyInfo(companiesAddedToComboList[companyNameCombo.SelectedIndex].company_serial);
            }

            if (companyNameCombo.SelectedItem == null)
            {
                companyAddressCombo.SelectedItem = null;
                companyAddressCombo.IsEnabled = false;
            }


        }

        private void OnSelChangedCompanyAddressCombo(object sender, SelectionChangedEventArgs e)
        {
            contactPersonNameCombo.Items.Clear();

            if (companyAddressCombo.SelectedItem == null)
            {
                contactPersonNameCombo.SelectedItem = null;
                contactPersonNameCombo.IsEnabled = false;
            }
            else
                InitializeCompanyContactCombo();

        }

        private void OnSelChangedContactPersonCombo(object sender, SelectionChangedEventArgs e)
        {
            if (contactPersonNameCombo.SelectedItem != null)
                maintOffer.InitializeContactInfo(contactInfo[contactPersonNameCombo.SelectedIndex].contact_id);

        }

        private void OnSelChangedProjectSerialCombo(object sender, SelectionChangedEventArgs e)
        {
            if (projectSerialCombo.SelectedItem != null)
                maintOffer.InitializeProjectInfo(projects[projectSerialCombo.SelectedIndex].project_serial);
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {

        }
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

        private void OnBtnClickNext(object sender, RoutedEventArgs e)
        {
            maintOffersProductsPage.maintOffersBasicInfoPage = this;
            maintOffersProductsPage.maintOffersPaymentAndDeliveryPage = maintOffersPaymentAndDeliveryPage;
            maintOffersProductsPage.maintOffersAdditionalInfoPage = maintOffersAdditionalInfoPage;
            maintOffersProductsPage.maintOffersUploadFilesPage = maintOffersUploadFilesPage;


            NavigationService.Navigate(maintOffersProductsPage);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///BUTTON CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnButtonClickAutomateMaintOffers(object sender, RoutedEventArgs e)
        {

        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;

            currentWindow.Close();
        }


    }
}
