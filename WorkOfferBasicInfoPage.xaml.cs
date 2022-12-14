using _01electronics_library;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;


namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for WorkOfferBasicInfoPage.xaml
    /// </summary>
    public partial class WorkOfferBasicInfoPage : Page
    {
        Employee loggedInUser;
        Quotation quotation;


        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;

        private List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> companiesList = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> companiesAddedToComboList = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> branchInfo = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT> contactInfo = new List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> employeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        //private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> techOfficeList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsList = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsAddedToComboList = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();

        private List<PROJECT_MACROS.PROJECT_STRUCT> projects = new List<PROJECT_MACROS.PROJECT_STRUCT>();


        private int viewAddCondition;
        private int salesPersonID;
        private int salesPersonTeamID;


        public WorkOfferProductsPage workOfferProductsPage;
        public WorkOfferPaymentAndDeliveryPage workOfferPaymentAndDeliveryPage;
        public WorkOfferAdditionalInfoPage workOfferAdditionalInfoPage;
        public WorkOfferUploadFilesPage workOfferUploadFilesPage;

        public WorkOfferBasicInfoPage(ref Employee mLoggedInUser, ref Quotation mWorkOffer, int mViewAddCondition, ref WorkOfferProductsPage mWorkOfferProductsPage)
        {
            workOfferProductsPage = mWorkOfferProductsPage;

            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;


            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            quotation = mWorkOffer;

            InitializeComponent();

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


                workOfferUploadFilesPage = new WorkOfferUploadFilesPage(ref loggedInUser, ref quotation, viewAddCondition);
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

                projectSerialCombo.IsEnabled = false;
            }
            else
            {
                FillrfqsList();
                ConfigureUIElemenetsForAdd();
                InitializeSalesPersonCombo();
                InitializeProjectCombo();

                SetSalesPersonComboValue();
                InitializeRFQSerialCombo();
                SetRFQSerialComboValue();
                SetProjectCombo();

                DisableSalesPersonAndRFQCombo();
                projectSerialCombo.IsEnabled = false;
            }

        }

        public WorkOfferBasicInfoPage(ref Quotation mWorkOffer)
        {
            quotation = mWorkOffer;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////INITIALIZE FUNCTIONS///////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool InitializeSalesPersonCombo()
        {
            List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> tempEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();

            if (!commonQueriesObject.GetTeamEmployees(COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID, ref tempEmployeesList))
                return false;

            for (int i = 0; i < tempEmployeesList.Count; i++)
                employeesList.Add(tempEmployeesList[i]);

            if (!commonQueriesObject.GetTeamEmployees(COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID, ref tempEmployeesList))
                return false;

            for (int i = 0; i < tempEmployeesList.Count; i++)
                employeesList.Add(tempEmployeesList[i]);

            if (!commonQueriesObject.GetManagerEmployees(ref tempEmployeesList))
                return false;

            for (int i = 0; i < tempEmployeesList.Count; i++)
                employeesList.Add(tempEmployeesList[i]);

            employeesList.Sort();

            for (int i = 0; i < employeesList.Count(); i++)
                salesPersonCombo.Items.Add(employeesList[i].employee_name);

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
            {
                ////////////////////////////
                //// THE NUMBER 5 IS FOR CONTRACT TYPE MAINTENANCE ON REQUEST (SAMEH)
                ////////////////////////////
                if (rfqsList[i].sales_person_id == salesPersonID && rfqsList[i].assignee_id == loggedInUser.GetEmployeeId() && rfqsList[i].contract_type_id != 5 && rfqsList[i].rfq_status_id == 1)
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

            quotation.InitializeBranchInfo(branchInfo[0].address_serial);

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
            salesPersonLabel.Content = quotation.GetSalesPersonName();
        }

        private void SetSalesPersonComboValue()
        {
            salesPersonCombo.SelectedItem = quotation.GetSalesPersonName();
        }

        private void SetRFQSerialLabel()
        {
            RFQSerialLabel.Content = quotation.GetRFQID();
        }

        private void SetRFQSerialComboValue()
        {
            RFQSerialCombo.SelectedItem = quotation.GetRFQID();
        }

        private void SetCompanyNameLabel()
        {
            companyNameLabel.Content = quotation.GetCompanyName();
        }

        private void SetCompanyNameComboValue()
        {
            companyNameCombo.Text = quotation.GetCompanyName();
        }

        private void SetCompanyAddressLabel()
        {
            if (!commonQueriesObject.GetCompanyAddresses(quotation.GetCompanySerial(), ref branchInfo))
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
            contactPersonNameLabel.Content = quotation.GetContactName();
        }

        private void SetContactPersonComboValue()
        {
            contactPersonNameCombo.Text = quotation.GetContactName();
        }

        private void SetCompanyNameAddressContactFromRFQ()
        {

            InitializeCompanyNameCombo();


            companyNameCombo.SelectedIndex = companiesList.FindIndex(s1 => s1.company_serial == rfqsAddedToComboList[RFQSerialCombo.SelectedIndex].company_serial);


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

        public void SetContractTypeValueFromRFQ()
        {
            workOfferProductsPage.workOfferPaymentAndDeliveryPage.contractTypeComboBox.Text = quotation.GetRFQContractType();
        }

        private void SetProjectCombo()
        {
            projectSerialCombo.SelectedItem = quotation.GetprojectName();
        }

        private void SetProjectLabel()
        {
            projectSerialLabel.Content = quotation.GetprojectName();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////SELECTION CHANGED HANDLERS///////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void OnSelChangedSalesPersonCombo(object sender, SelectionChangedEventArgs e)
        {
            RFQSerialCombo.Items.Clear();
            companyNameCombo.Items.Clear();
            RFQSerialCombo.SelectedIndex = -1;
            companyNameCombo.SelectedIndex = -1;
            rfqsAddedToComboList.Clear();

            salesPersonID = employeesList[salesPersonCombo.SelectedIndex].employee_id;
            commonQueriesObject.GetEmployeeTeam(salesPersonID, ref salesPersonTeamID);

            //if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_ADD_CONDITION)
            //    quotation.ResetWorkOfferInfo();

            quotation.InitializeOfferProposerInfo(loggedInUser.GetEmployeeId());
            quotation.InitializeSalesPersonInfo(salesPersonID);

            if (salesPersonTeamID != COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID)
            {
                InitializeRFQSerialCombo();
                projectSerialCombo.IsEnabled = false;
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
            if (RFQSerialCombo.SelectedItem != null)
            {
                quotation.InitializeRFQInfo(rfqsAddedToComboList[RFQSerialCombo.SelectedIndex].rfq_serial, rfqsAddedToComboList[RFQSerialCombo.SelectedIndex].rfq_version);
                if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_REVISE_CONDITION)
                    quotation.LinkRFQInfo();

                SetCompanyNameAddressContactFromRFQ();
                if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_REVISE_CONDITION)
                    SetContractTypeValueFromRFQ();

                if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
                {
                    workOfferProductsPage.SetCategoryComboBoxesResolve();
                    workOfferProductsPage.SetTypeComboBoxesResolve();
                    workOfferProductsPage.SetBrandComboBoxesResolve();
                    workOfferProductsPage.SetModelComboBoxesResolve();
                    workOfferProductsPage.SetQuantityTextBoxes();
                }
                else
                {
                    workOfferProductsPage.SetCategoryLabels();
                    workOfferProductsPage.SetTypeLabels();
                    workOfferProductsPage.SetBrandLabels();
                    workOfferProductsPage.SetModelLabels();
                    workOfferProductsPage.SetQuantityTextBoxes();
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
                quotation.InitializeCompanyInfo(companiesAddedToComboList[companyNameCombo.SelectedIndex].company_serial);
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
                quotation.InitializeContactInfo(contactInfo[contactPersonNameCombo.SelectedIndex].contact_id);

        }

        private void OnSelChangedProjectSerialCombo(object sender, SelectionChangedEventArgs e)
        {
            if (projectSerialCombo.SelectedItem != null)
                quotation.InitializeProjectInfo(projects[projectSerialCombo.SelectedIndex].project_serial);
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {

        }
        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            workOfferProductsPage.workOfferBasicInfoPage = this;
            workOfferProductsPage.workOfferPaymentAndDeliveryPage = workOfferPaymentAndDeliveryPage;
            workOfferProductsPage.workOfferAdditionalInfoPage = workOfferAdditionalInfoPage;
            workOfferProductsPage.workOfferUploadFilesPage = workOfferUploadFilesPage;


            NavigationService.Navigate(workOfferProductsPage);
        }
        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {
            workOfferPaymentAndDeliveryPage.workOfferBasicInfoPage = this;
            workOfferPaymentAndDeliveryPage.workOfferProductsPage = workOfferProductsPage;
            workOfferPaymentAndDeliveryPage.workOfferAdditionalInfoPage = workOfferAdditionalInfoPage;
            workOfferPaymentAndDeliveryPage.workOfferUploadFilesPage = workOfferUploadFilesPage;

            NavigationService.Navigate(workOfferPaymentAndDeliveryPage);
        }
        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            workOfferAdditionalInfoPage.workOfferBasicInfoPage = this;
            workOfferAdditionalInfoPage.workOfferProductsPage = workOfferProductsPage;
            workOfferAdditionalInfoPage.workOfferPaymentAndDeliveryPage = workOfferPaymentAndDeliveryPage;
            workOfferAdditionalInfoPage.workOfferUploadFilesPage = workOfferUploadFilesPage;

            NavigationService.Navigate(workOfferAdditionalInfoPage);
        }
        private void OnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
            {
                workOfferUploadFilesPage.workOfferBasicInfoPage = this;
                workOfferUploadFilesPage.workOfferProductsPage = workOfferProductsPage;
                workOfferUploadFilesPage.workOfferPaymentAndDeliveryPage = workOfferPaymentAndDeliveryPage;
                workOfferUploadFilesPage.workOfferAdditionalInfoPage = workOfferAdditionalInfoPage;

                NavigationService.Navigate(workOfferUploadFilesPage);
            }
        }

        private void OnBtnClickNext(object sender, RoutedEventArgs e)
        {
            workOfferProductsPage.workOfferBasicInfoPage = this;
            workOfferProductsPage.workOfferPaymentAndDeliveryPage = workOfferPaymentAndDeliveryPage;
            workOfferProductsPage.workOfferAdditionalInfoPage = workOfferAdditionalInfoPage;
            workOfferProductsPage.workOfferUploadFilesPage = workOfferUploadFilesPage;


            NavigationService.Navigate(workOfferProductsPage);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///BUTTON CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnButtonClickAutomateWorkOffer(object sender, RoutedEventArgs e)
        {

        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;

            currentWindow.Close();
        }


    }
}
