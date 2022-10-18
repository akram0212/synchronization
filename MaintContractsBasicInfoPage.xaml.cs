using _01electronics_library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for MaintContractsBasicInfoPage.xaml
    /// </summary>
    public partial class MaintContractsBasicInfoPage : Page
    {
        Employee loggedInUser;
        MaintenanceContract maintContract;
        public MaintenanceContract oldMaintContract;
        MaintenanceOffer tmpMaintOffer;

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;

        private List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> companiesList = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> companiesAddedToComboList = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> branchInfo = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT> contactInfo = new List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> employeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT> outgoingQuotationsList = new List<COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT> offersAddedToComboList = new List<COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.STATUS_STRUCT> contractStatuses = new List<COMPANY_WORK_MACROS.STATUS_STRUCT>();

        private int viewAddCondition;
        private int salesPersonID;
        private int salesPersonTeamID;

        public MaintContractsProductsPage maintContractsProductsPage;
        public MaintContractsPaymentAndDeliveryPage maintContractsPaymentAndDeliveryPage;
        public MaintContractsAdditionalInfoPage maintContractsAdditionalInfoPage;
        public MaintContractsUploadFilesPage maintContractsUploadFilesPage;
        public MaintContractsProjectsPage maintContractsProjectInfoPage;
        public MaintContractsBasicInfoPage(ref Employee mLoggedInUser, ref MaintenanceContract mMaintContracts, int mViewAddCondition, ref MaintContractsProjectsPage mMaintContractsProjectsPage)
        {
            maintContractsProjectInfoPage = mMaintContractsProjectsPage;

            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            maintContract = mMaintContracts;
            oldMaintContract = new MaintenanceContract();
            //tmpMaintOffer = new MaintenanceOffer();

            InitializeComponent();

            if (viewAddCondition == COMPANY_WORK_MACROS.CONTRACT_ADD_CONDITION)
            {
                FillOffersList();
                ConfigureUIElemenetsForAdd();
                InitializeSalesPersonCombo();
                //InitializeAssignedSalesPersonCombo();
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.CONTRACT_VIEW_CONDITION)
            {
                FillOffersList();
                ConfigureUIElementsForView();
                SetSalesPersonLabel();
                SetOfferSerialLabel();
                SetCompanyNameLabel();
                SetCompanyAddressLabel();
                SetContactPersonLabel();
                SetStatusLabel();
                SetIssueDate();
                issueDatePicker.IsEnabled = false;

                maintContractsUploadFilesPage = new MaintContractsUploadFilesPage(ref loggedInUser, ref maintContract, viewAddCondition);
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.CONTRACT_EDIT_CONDITION)
            {
                oldMaintContract.CopyMaintenanceContract(maintContract);

                FillOffersList();
                ConfigureUIElemenetsForAdd();
                InitializeSalesPersonCombo();
                SetSalesPersonComboValue();
                InitializeStatusComboBox();
                SetIssueDate();

                contractStatusWrapPanel.Visibility = Visibility.Visible;

                if (maintContract.GetMaintOfferID() != null)
                {
                    SetOfferSerialComboValue();
                    OfferSerialCombo.IsEnabled = true;
                }
                else
                {
                    SetCompanyNameComboValue();
                    SetContactPersonComboValue();

                    companyNameCombo.IsEnabled = true;
                    companyAddressCombo.IsEnabled = true;
                    contactPersonNameCombo.IsEnabled = true;
                }

                DisableSalesPersonAndOfferCombo();

            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.CONTRACT_RENEW_CONDITION)
            {
                FillOffersList();
                ConfigureUIElemenetsForAdd();
                InitializeSalesPersonCombo();
                SetSalesPersonComboValue();
                InitializeStatusComboBox();
                maintContract.SetMaintContractIssueDateToToday();
                SetIssueDate();
                //contractStatusWrapPanel.Visibility = Visibility.Visible;

                if (maintContract.GetMaintOfferID() != null)
                {
                    SetOfferSerialComboValue();
                    OfferSerialCombo.IsEnabled = true;
                }
                else
                {
                    SetCompanyNameComboValue();
                    SetContactPersonComboValue();

                    companyNameCombo.IsEnabled = true;
                    companyAddressCombo.IsEnabled = true;
                    contactPersonNameCombo.IsEnabled = true;
                }

                DisableSalesPersonAndOfferCombo();
                DisableCompanyNameaddressContactCombos();
                OfferCheckBox.IsEnabled = false;


            }
            else
            {
                FillOffersList();
                ConfigureUIElemenetsForAdd();
                InitializeSalesPersonCombo();
                SetSalesPersonComboValue();

                if (maintContract.GetMaintOfferID() != null)
                    SetOfferSerialComboValue();
                else
                {
                    SetCompanyNameComboValue();
                    SetContactPersonComboValue();
                }

                DisableSalesPersonAndOfferCombo();
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///CONFIGURATION FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ConfigureUIElementsForView()
        {
            salesPersonLabel.Visibility = Visibility.Visible;
            OfferSerialLabel.Visibility = Visibility.Visible;
            companyNameLabel.Visibility = Visibility.Visible;
            companyAddressLabel.Visibility = Visibility.Visible;
            contactPersonNameLabel.Visibility = Visibility.Visible;
            statusLabel.Visibility = Visibility.Visible;

            salesPersonCombo.Visibility = Visibility.Collapsed;
            OfferSerialCombo.Visibility = Visibility.Collapsed;
            companyNameCombo.Visibility = Visibility.Collapsed;
            companyAddressCombo.Visibility = Visibility.Collapsed;
            contactPersonNameCombo.Visibility = Visibility.Collapsed;
            statusComboBox.Visibility = Visibility.Collapsed;

            OfferCheckBox.IsEnabled = false;

            contractStatusWrapPanel.Visibility = Visibility.Visible;

        }
        private void ConfigureUIElemenetsForAdd()
        {
            salesPersonLabel.Visibility = Visibility.Collapsed;
            companyNameLabel.Visibility = Visibility.Collapsed;
            companyAddressLabel.Visibility = Visibility.Collapsed;
            contactPersonNameLabel.Visibility = Visibility.Collapsed;
            OfferSerialLabel.Visibility = Visibility.Collapsed;
            statusLabel.Visibility = Visibility.Collapsed;

            salesPersonCombo.Visibility = Visibility.Visible;
            companyNameCombo.Visibility = Visibility.Visible;
            companyAddressCombo.Visibility = Visibility.Visible;
            contactPersonNameCombo.Visibility = Visibility.Visible;
            OfferSerialCombo.Visibility = Visibility.Visible;
            statusComboBox.Visibility = Visibility.Visible;

            OfferCheckBox.IsEnabled = true;
            OfferSerialCombo.IsEnabled = false;
            companyNameCombo.IsEnabled = false;
            companyAddressCombo.IsEnabled = false;
            contactPersonNameCombo.IsEnabled = false;

            // assignedSalesCombo.IsEnabled = false;
        }

        private void DisableSalesPersonAndOfferCombo()
        {
            salesPersonCombo.IsEnabled = false;
            OfferSerialCombo.IsEnabled = false;
        }
        private void DisableCompanyNameaddressContactCombos()
        {
            companyNameCombo.IsEnabled = false;
            companyAddressCombo.IsEnabled = false;
            contactPersonNameCombo.IsEnabled = false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INITIALIZE FUNCTIONS
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

        //private bool InitializeAssignedSalesPersonCombo()
        //{
        //    for (int i = 0; i < employeesList.Count(); i++)
        //    {
        //        assignedSalesCombo.Items.Add(employeesList[i].employee_name);
        //    }
        //    assignedSalesCombo.Items.Add(loggedInUser.GetEmployeeName());
        //    return true;
        //}

        private void InitializeCompanyNameCombo()
        {
            companyNameCombo.Items.Clear();
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

            maintContract.InitializeBranchInfo(branchInfo[0].address_serial);

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


            contactPersonNameCombo.SelectedIndex = 0;

            contactPersonNameCombo.IsEnabled = true;
        }

        private void InitializeOfferSerialCombo()
        {

            if (viewAddCondition == COMPANY_WORK_MACROS.CONTRACT_ADD_CONDITION)
            {
                FillOfferSerialCombo();
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.CONTRACT_EDIT_CONDITION || viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_RESOLVE_CONDITION)
            {
                COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT tmp = outgoingQuotationsList.Find(x => x.offer_serial == maintContract.GetMaintOfferSerial());

                OfferSerialCombo.Items.Add(maintContract.GetMaintOfferID());
                offersAddedToComboList.Add(tmp);

                OfferCheckBox.IsEnabled = false;
                OfferCheckBox.IsChecked = true;
                OfferSerialCombo.IsEnabled = false;
            }

        }

        private bool FillOffersList()
        {
            if (!commonQueriesObject.GetMaintenanceOffers(ref outgoingQuotationsList))
                return false;
            return true;
        }

        private void FillOfferSerialCombo()
        {
            offersAddedToComboList.Clear();
            OfferSerialCombo.Items.Clear();

            if (salesPersonCombo.SelectedItem != null)
            {
                for (int i = 0; i < outgoingQuotationsList.Count(); i++)
                {
                    if (loggedInUser.GetEmployeePositionId() <= COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION)
                    {
                        if (outgoingQuotationsList[i].sales_person_id == employeesList[salesPersonCombo.SelectedIndex].employee_id && outgoingQuotationsList[i].offer_status_id == COMPANY_WORK_MACROS.PENDING_OUTGOING_QUOTATION)
                        {
                            OfferSerialCombo.Items.Add(outgoingQuotationsList[i].offer_id);
                            offersAddedToComboList.Add(outgoingQuotationsList[i]);
                        }
                    }
                    else
                    {
                        if (outgoingQuotationsList[i].sales_person_id == employeesList[salesPersonCombo.SelectedIndex].employee_id && outgoingQuotationsList[i].offer_proposer_id == loggedInUser.GetEmployeeId() && outgoingQuotationsList[i].offer_status_id == COMPANY_WORK_MACROS.PENDING_OUTGOING_QUOTATION)
                        {
                            OfferSerialCombo.Items.Add(outgoingQuotationsList[i].offer_id);
                            offersAddedToComboList.Add(outgoingQuotationsList[i]);
                        }
                    }
                }
            }
        }

        private void InitializeStatusComboBox()
        {
            if (!commonQueriesObject.GetMaintenanceContractsStatus(ref contractStatuses))
                return;

            for (int i = 0; i < contractStatuses.Count; i++)
            {
                statusComboBox.Items.Add(contractStatuses[i].status_name);
            }

            statusComboBox.SelectedIndex = contractStatuses.FindIndex(contractID => contractID.status_id == maintContract.GetMaintContractStatusId());
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///SET FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SetSalesPersonLabel()
        {
            salesPersonLabel.Content = maintContract.GetSalesPersonName();
        }

        private void SetSalesPersonComboValue()
        {
            salesPersonCombo.SelectedItem = maintContract.GetSalesPersonName();
        }

        private void SetOfferSerialLabel()
        {
            OfferSerialLabel.Content = maintContract.GetMaintOfferID();
        }
        private void SetIssueDate()
        {
            issueDatePicker.SelectedDate = maintContract.GetMaintContractIssueDate();
        }
        private void SetOfferSerialComboValue()
        {
            OfferCheckBox.IsChecked = true;
            OfferSerialCombo.SelectedItem = maintContract.GetMaintOfferID();
            OfferSerialCombo.IsEnabled = false;
        }


        private void SetCompanyNameLabel()
        {
            companyNameLabel.Content = maintContract.GetCompanyName();
        }

        private void SetCompanyNameComboValue()
        {
            companyNameCombo.Text = maintContract.GetCompanyName();
        }

        private void SetCompanyAddressLabel()
        {
            if (!commonQueriesObject.GetCompanyAddresses(maintContract.GetCompanySerial(), ref branchInfo))
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
            contactPersonNameLabel.Content = maintContract.GetContactName();
        }
        
        private void SetStatusLabel()
        {
            statusLabel.Content = maintContract.GetMaintContractStatus();
        }

        private void SetContactPersonComboValue()
        {
            contactPersonNameCombo.Text = maintContract.GetContactName();
        }

        private void SetCompanyNameAddressContactFromOffer()
        {
            companyNameCombo.Items.Clear();
            companyAddressCombo.Items.Clear();
            contactPersonNameCombo.Items.Clear();
            companyNameCombo.Items.Add(offersAddedToComboList[OfferSerialCombo.SelectedIndex].company_name);
            companyNameCombo.SelectedIndex = 0;



            if (!commonQueriesObject.GetCompanyAddresses(offersAddedToComboList[OfferSerialCombo.SelectedIndex].company_serial, ref branchInfo))
                return;

            for (int i = 0; i < branchInfo.Count; i++)
            {
                string address;
                address = branchInfo[i].district + ", " + branchInfo[i].city + ", " + branchInfo[i].state_governorate + ", " + branchInfo[i].country + ".";
                companyAddressCombo.Items.Add(address);
                companyAddressCombo.SelectedIndex = 0;
            }

            InitializeCompanyContactCombo();

            contactPersonNameCombo.Items.Add(offersAddedToComboList[OfferSerialCombo.SelectedIndex].contact_name);
            contactPersonNameCombo.SelectedItem = offersAddedToComboList[OfferSerialCombo.SelectedIndex].contact_name;

            DisableCompanyNameaddressContactCombos();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////SELECTION CHANGED HANDLERS///////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnSelChangedSalesPersonCombo(object sender, SelectionChangedEventArgs e)
        {
            companyNameCombo.Items.Clear();
            companyNameCombo.SelectedIndex = -1;
            OfferCheckBox.IsChecked = false;

            companyNameCombo.IsEnabled = false;

            salesPersonID = employeesList[salesPersonCombo.SelectedIndex].employee_id;
            commonQueriesObject.GetEmployeeTeam(salesPersonID, ref salesPersonTeamID);

            if (salesPersonTeamID != COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID)
            {
                InitializeCompanyNameCombo();
                if (viewAddCondition == COMPANY_WORK_MACROS.CONTRACT_ADD_CONDITION || maintContract.GetMaintOfferID() != null)
                    OfferCheckBox.IsChecked = true;
            }
            else
            {
                InitializeCompanyNameCombo();
            }

            if (viewAddCondition == COMPANY_WORK_MACROS.CONTRACT_ADD_CONDITION)
                maintContract.InitializeMaintContractProposerInfo(loggedInUser.GetEmployeeId());
            else if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_RESOLVE_CONDITION)
                maintContract.InitializeMaintContractProposerInfo(maintContract.GetMaintOfferProposerId());


            if (salesPersonTeamID == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
            {
                maintContract.InitializeSalesPersonInfo(salesPersonID);
            }
            else
            {
                maintContract.InitializeSalesPersonInfo(salesPersonID);

                if (OfferCheckBox.IsChecked == false)
                {
                    companyNameCombo.IsEnabled = true;
                }
            }

        }

        private void OnSelChangedCompanyNameCombo(object sender, SelectionChangedEventArgs e)
        {
            companyAddressCombo.Items.Clear();

            if (companyNameCombo.SelectedItem != null && OfferSerialCombo.SelectedItem == null)
            {
                InitializeCompanyAddressCombo();

                if (OfferCheckBox.IsChecked != true)
                {
                    maintContract.InitializeCompanyInfo(companiesAddedToComboList[companyNameCombo.SelectedIndex].company_serial);
                }
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
            {
                if (OfferCheckBox.IsChecked != true)
                {
                    InitializeCompanyContactCombo();
                }
            }

        }

        private void OnSelChangedContactPersonCombo(object sender, SelectionChangedEventArgs e)
        {
            if (OfferCheckBox.IsChecked != true)
            {
                if (contactPersonNameCombo.SelectedItem != null)
                    maintContract.InitializeContactInfo(contactInfo[contactPersonNameCombo.SelectedIndex].contact_id);
            }

        }

        private void OnSelChangedQuotationSerialCombo(object sender, SelectionChangedEventArgs e)
        {
            if (OfferSerialCombo.SelectedItem != null)
            {
                if (viewAddCondition != COMPANY_WORK_MACROS.CONTRACT_EDIT_CONDITION)
                {
                    if (maintContract.GetMaintOfferSerial() == 0)
                        maintContract.InitializeMaintOfferInfo(offersAddedToComboList[OfferSerialCombo.SelectedIndex].offer_serial, offersAddedToComboList[OfferSerialCombo.SelectedIndex].offer_version, offersAddedToComboList[OfferSerialCombo.SelectedIndex].offer_proposer_id);

                    if (maintContract.GetprojectSerial() != 0)
                    {
                        maintContractsProjectInfoPage.SetProjectComboBox();
                        maintContractsProjectInfoPage.projectCheckBox.IsEnabled = true;
                        maintContractsProjectInfoPage.projectCheckBox.IsChecked = true;
                    }

                    maintContractsProjectInfoPage.maintContractsProductsPage.SetCategoryComboBoxesFromOffer();
                    maintContractsProjectInfoPage.maintContractsProductsPage.SetTypeComboBoxesFromOffer();
                    maintContractsProjectInfoPage.maintContractsProductsPage.SetBrandComboBoxesFromOffer();
                    maintContractsProjectInfoPage.maintContractsProductsPage.SetModelComboBoxesFromOffer();
                    maintContractsProjectInfoPage.maintContractsProductsPage.SetQuantityTextBoxesFromOffer();
                    maintContractsProjectInfoPage.maintContractsProductsPage.SetPriceTextBoxesFromOffer();
                    maintContractsProjectInfoPage.maintContractsProductsPage.SetPriceComboBoxesFromOffer();

                    maintContractsProjectInfoPage.maintContractsProductsPage.maintContractsPaymentAndDeliveryPage.SetTotalPriceTextBoxFromOffer();
                    maintContractsProjectInfoPage.maintContractsProductsPage.maintContractsPaymentAndDeliveryPage.SetTotalPriceCurrencyComboBoxFromOffer();
                    maintContractsProjectInfoPage.maintContractsProductsPage.maintContractsPaymentAndDeliveryPage.SetConditionsCheckBoxesFromOffer();
                    maintContractsProjectInfoPage.maintContractsProductsPage.maintContractsPaymentAndDeliveryPage.SetFrequenciesValueFromOffer();


                    maintContractsProjectInfoPage.maintContractsProductsPage.maintContractsPaymentAndDeliveryPage.maintContractsAdditionalInfoPage.SetWarrantyPeriodValuesFromOffer();
                    maintContractsProjectInfoPage.maintContractsProductsPage.maintContractsPaymentAndDeliveryPage.maintContractsAdditionalInfoPage.SetAdditionalDescriptionValueFromOffer();
                    maintContractsProjectInfoPage.maintContractsProductsPage.maintContractsPaymentAndDeliveryPage.maintContractsAdditionalInfoPage.SetFrequenciesValueFromOffer();
                }

                SetCompanyNameAddressContactFromOffer();
            }
        }
        private void OnSelChangedIssuedatePicker(object sender, SelectionChangedEventArgs e)
        {
            maintContract.SetMaintContractIssueDate(DateTime.Parse(issueDatePicker.Text.ToString()));
        }
        private void OnSelChangedStatusCombo(object sender, SelectionChangedEventArgs e)
        {
            if(statusComboBox.SelectedIndex != -1)
            {
                maintContract.SetMaintContractStatus(contractStatuses[statusComboBox.SelectedIndex].status_id, contractStatuses[statusComboBox.SelectedIndex].status_name);
            }
        }

        //private void OnSelChangedAssignedSalesCombo(object sender, SelectionChangedEventArgs e)
        //{
        //    if (viewAddCondition != COMPANY_WORK_MACROS.CONTRACT_VIEW_CONDITION && assignedSalesCombo.SelectedItem != null)
        //    {
        //        if (assignedSalesCombo.SelectedIndex != employeesList.Count())
        //        {
        //            maintContract.SetOrderAssignedSalesID(employeesList[assignedSalesCombo.SelectedIndex].employee_id);
        //        }
        //        else
        //            maintContract.SetOrderAssignedSalesID(loggedInUser.GetEmployeeId());
        //    }
        //    else
        //    {

        //    }
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {

        }
        private void OnClickProjectInfo(object sender, MouseButtonEventArgs e)
        {
            maintContractsProjectInfoPage.maintContractsBasicInfoPage = this;
            maintContractsProjectInfoPage.maintContractsProductsPage = maintContractsProductsPage;
            maintContractsProjectInfoPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
            maintContractsProjectInfoPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
            maintContractsProjectInfoPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;


            NavigationService.Navigate(maintContractsProjectInfoPage);

        }
        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            maintContractsProductsPage.maintContractsBasicInfoPage = this;
            maintContractsProductsPage.maintContractsProjectsPage = maintContractsProjectInfoPage;
            maintContractsProductsPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
            maintContractsProductsPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
            maintContractsProductsPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;


            NavigationService.Navigate(maintContractsProductsPage);
        }
        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {
            maintContractsPaymentAndDeliveryPage.maintContractsBasicInfoPage = this;
            maintContractsPaymentAndDeliveryPage.maintContractsProjectsPage = maintContractsProjectInfoPage;
            maintContractsPaymentAndDeliveryPage.maintContractsProductsPage = maintContractsProductsPage;
            maintContractsPaymentAndDeliveryPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
            maintContractsPaymentAndDeliveryPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;

            NavigationService.Navigate(maintContractsPaymentAndDeliveryPage);
        }
        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            maintContractsAdditionalInfoPage.maintContractsBasicInfoPage = this;
            maintContractsAdditionalInfoPage.maintContractsProjectsPage = maintContractsProjectInfoPage;
            maintContractsAdditionalInfoPage.maintContractsProductsPage = maintContractsProductsPage;
            maintContractsAdditionalInfoPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
            maintContractsAdditionalInfoPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;

            NavigationService.Navigate(maintContractsAdditionalInfoPage);
        }
        private void OnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.CONTRACT_VIEW_CONDITION)
            {
                maintContractsUploadFilesPage.maintContractsBasicInfoPage = this;
                maintContractsUploadFilesPage.maintContractsProjectsPage = maintContractsProjectInfoPage;
                maintContractsUploadFilesPage.maintContractsProductsPage = maintContractsProductsPage;
                maintContractsUploadFilesPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
                maintContractsUploadFilesPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;

                NavigationService.Navigate(maintContractsUploadFilesPage);
            }
        }

        private void OnBtnClickNext(object sender, RoutedEventArgs e)
        {
            maintContractsProjectInfoPage.maintContractsBasicInfoPage = this;
            maintContractsProjectInfoPage.maintContractsProductsPage = maintContractsProductsPage;
            maintContractsProjectInfoPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
            maintContractsProjectInfoPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
            maintContractsProjectInfoPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;


            NavigationService.Navigate(maintContractsProjectInfoPage);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///BUTTON CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnButtonClickAutomateMaintContracts(object sender, RoutedEventArgs e)
        {

        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;

            currentWindow.Close();
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///CHECK HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnCheckOfferCheckBox(object sender, RoutedEventArgs e)
        {
            OfferSerialCombo.IsEnabled = true;

            InitializeOfferSerialCombo();

            companyNameCombo.SelectedItem = null;
            companyAddressCombo.SelectedItem = null;
            contactPersonNameCombo.SelectedItem = null;

            companyNameCombo.IsEnabled = false;
            companyAddressCombo.IsEnabled = false;
            contactPersonNameCombo.IsEnabled = false;

            //maintContractsProjectInfoPage.checkAllCheckBox.IsEnabled = false;
        }

        private void OnUnCheckOfferCheckBox(object sender, RoutedEventArgs e)
        {
            OfferSerialCombo.IsEnabled = false;
            OfferSerialCombo.SelectedItem = null;

            companyNameCombo.SelectedItem = null;
            companyAddressCombo.SelectedItem = null;
            contactPersonNameCombo.SelectedItem = null;


            //if (salesPersonCombo.SelectedItem == loggedInUser.GetEmployeeName())
            //{
            companyNameCombo.IsEnabled = true;
            InitializeCompanyNameCombo();
            //}
        }

    }
}
