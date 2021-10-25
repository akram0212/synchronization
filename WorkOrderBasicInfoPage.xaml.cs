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
    /// Interaction logic for WorkOrderBasicInfoPage.xaml
    /// </summary>
    public partial class WorkOrderBasicInfoPage : Page
    {
        Employee loggedInUser;
        WorkOrder workOrder;
        WorkOffer tmpWorkOffer;


        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;

        private List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> companiesList = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> companiesAddedToComboList = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> branchInfo = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT> contactInfo = new List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> employeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_WORK_MACROS.WORK_OFFER_MAX_STRUCT> offersList = new List<COMPANY_WORK_MACROS.WORK_OFFER_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.WORK_OFFER_MAX_STRUCT> offersAddedToComboList = new List<COMPANY_WORK_MACROS.WORK_OFFER_MAX_STRUCT>();

        private int viewAddCondition;
        private int salesPersonID;
        private int salesPersonTeamID;

        public WorkOrderProductsPage workOrderProductsPage;
        public WorkOrderPaymentAndDeliveryPage workOrderPaymentAndDeliveryPage;
        public WorkOrderAdditionalInfoPage workOrderAdditionalInfoPage;
        public WorkOrderUploadFilesPage workOrderUploadFilesPage;
        public WorkOrderBasicInfoPage(ref Employee mLoggedInUser, ref WorkOrder mWorkOrder, int mViewAddCondition, ref WorkOrderProductsPage mWorkOrderProductsPage)
        {
            workOrderProductsPage = mWorkOrderProductsPage;

            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            workOrder = mWorkOrder;
            tmpWorkOffer = new WorkOffer();

            InitializeComponent();
            if (viewAddCondition == COMPANY_WORK_MACROS.OFFER_ADD_CONDITION)
            {
                FillOffersList();
                ConfigureUIElemenetsForAdd();
                InitializeSalesPersonCombo();
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                FillOffersList();
                ConfigureUIElementsForView();
                SetSalesPersonLabel();
                SetOfferSerialLabel();
                SetCompanyNameLabel();
                SetCompanyAddressLabel();
                SetContactPersonLabel();

                workOrderUploadFilesPage = new WorkOrderUploadFilesPage(ref loggedInUser, ref workOrder, viewAddCondition);
            }
        }
        private void ConfigureUIElementsForView()
        {
            salesPersonLabel.Visibility = Visibility.Visible;
            OfferSerialLabel.Visibility = Visibility.Visible;
            companyNameLabel.Visibility = Visibility.Visible;
            companyAddressLabel.Visibility = Visibility.Visible;
            contactPersonNameLabel.Visibility = Visibility.Visible;

            salesPersonCombo.Visibility = Visibility.Collapsed;
            OfferSerialCombo.Visibility = Visibility.Collapsed;
            companyNameCombo.Visibility = Visibility.Collapsed;
            companyAddressCombo.Visibility = Visibility.Collapsed;
            contactPersonNameCombo.Visibility = Visibility.Collapsed;

        }
        private void ConfigureUIElemenetsForAdd()
        {
            salesPersonLabel.Visibility = Visibility.Collapsed;
            companyNameLabel.Visibility = Visibility.Collapsed;
            companyAddressLabel.Visibility = Visibility.Collapsed;
            contactPersonNameLabel.Visibility = Visibility.Collapsed;

            salesPersonCombo.Visibility = Visibility.Visible;
            companyNameCombo.Visibility = Visibility.Visible;
            companyAddressCombo.Visibility = Visibility.Visible;
            contactPersonNameCombo.Visibility = Visibility.Visible;

            OfferCheckBox.IsChecked = false;
            OfferSerialCombo.IsEnabled = false;
            companyNameCombo.IsEnabled = false;
            companyAddressCombo.IsEnabled = false;
            contactPersonNameCombo.IsEnabled = false;
        }
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
        private void DisableSalesPersonAndOfferCombo()
        {
            salesPersonCombo.IsEnabled = false;
        }
        private void DisableCompanyNameaddressContactCombos()
        {
            companyNameCombo.IsEnabled = false;
            companyAddressCombo.IsEnabled = false;
            contactPersonNameCombo.IsEnabled = false;
        }
        private void SetSalesPersonLabel()
        {
            salesPersonLabel.Content = workOrder.GetSalesPersonName();
        }

        private void SetSalesPersonComboValue()
        {
            salesPersonCombo.SelectedItem = workOrder.GetSalesPersonName();
        }

        private void SetOfferSerialLabel()
        {
            OfferSerialLabel.Content = workOrder.GetRFQID();
        }
        private void SetCompanyNameLabel()
        {
            companyNameLabel.Content = workOrder.GetCompanyName();
        }

        private void SetCompanyNameComboValue()
        {
            companyNameCombo.Text = workOrder.GetCompanyName();
        }

        private void SetCompanyAddressLabel()
        {
            if (!commonQueriesObject.GetCompanyAddresses(workOrder.GetCompanySerial(), ref branchInfo))
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
            contactPersonNameLabel.Content = workOrder.GetContactName();
        }

        private void SetContactPersonComboValue()
        {
            contactPersonNameCombo.Text = workOrder.GetContactName();
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////SELECTION CHANGED HANDLERS///////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void OnSelChangedSalesPersonCombo(object sender, SelectionChangedEventArgs e)
        {
            companyNameCombo.Items.Clear();
            OfferSerialCombo.Items.Clear();
            companyNameCombo.SelectedIndex = -1;
            OfferSerialCombo.SelectedIndex = -1;

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

            if (viewAddCondition == COMPANY_WORK_MACROS.OFFER_ADD_CONDITION)
                workOrder.ResetWorkOrderInfo(salesPersonTeamID);

            workOrder.InitializeOfferProposerInfo(loggedInUser.GetEmployeeId(), salesPersonTeamID);
                InitializeCompanyNameCombo();

            if (salesPersonTeamID == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
            {
                workOrder.InitializeSalesPersonInfo(salesPersonID);
                InitializeOfferSerialCombo();

                if (OfferCheckBox.IsChecked == false)
                {
                    companyNameCombo.IsEnabled = true;
                }
            }
            else
            {
                workOrder.InitializeSalesPersonInfo(loggedInUser.GetEmployeeId());
                InitializeOfferSerialCombo();

                if (OfferCheckBox.IsChecked == false)
                {
                    companyNameCombo.IsEnabled = true;
                }
            }

        }
       
        private void OnSelChangedCompanyNameCombo(object sender, SelectionChangedEventArgs e)
        {
            companyAddressCombo.Items.Clear();

            if (companyNameCombo.SelectedItem != null)
            {
                InitializeCompanyAddressCombo();
                workOrder.InitializeCompanyInfo(companiesAddedToComboList[companyNameCombo.SelectedIndex].company_serial);
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
                workOrder.InitializeContactInfo(contactInfo[contactPersonNameCombo.SelectedIndex].contact_id);

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {

        }
        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderProductsPage.workOrderBasicInfoPage = this;
            workOrderProductsPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderProductsPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderProductsPage.workOrderUploadFilesPage = workOrderUploadFilesPage;


            NavigationService.Navigate(workOrderProductsPage);
        }
        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderPaymentAndDeliveryPage.workOrderBasicInfoPage = this;
            workOrderPaymentAndDeliveryPage.workOrderProductsPage = workOrderProductsPage;
            workOrderPaymentAndDeliveryPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderPaymentAndDeliveryPage);
        }
        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderAdditionalInfoPage.workOrderBasicInfoPage = this;
            workOrderAdditionalInfoPage.workOrderProductsPage = workOrderProductsPage;
            workOrderAdditionalInfoPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderAdditionalInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderAdditionalInfoPage);
        }
        private void OnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                workOrderUploadFilesPage.workOrderBasicInfoPage = this;
                workOrderUploadFilesPage.workOrderProductsPage = workOrderProductsPage;
                workOrderUploadFilesPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
                workOrderUploadFilesPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;

                NavigationService.Navigate(workOrderUploadFilesPage);
            }
        }

        private void OnBtnClickNext(object sender, RoutedEventArgs e)
        {
            workOrderProductsPage.workOrderBasicInfoPage = this;
            workOrderProductsPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderProductsPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderProductsPage.workOrderUploadFilesPage = workOrderUploadFilesPage;


            NavigationService.Navigate(workOrderProductsPage);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///BUTTON CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnButtonClickAutomateWorkOrder(object sender, RoutedEventArgs e)
        {

        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;

            currentWindow.Close();
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

            workOrder.InitializeBranchInfo(branchInfo[0].address_serial);

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
        private bool FillOffersList()
        {
            if (!commonQueriesObject.GetWorkOffers(ref offersList))
                return false;
            return true;
        }
        private void InitializeOfferSerialCombo()
        {
            for (int i = 0; i < offersList.Count; i++)
            {
                if (offersList[i].sales_person_id == salesPersonID && offersList[i].offer_proposer_id == loggedInUser.GetEmployeeId())
                {
                    OfferSerialCombo.Items.Add(offersList[i].offer_id);
                    offersAddedToComboList.Add(offersList[i]);
                }
            }
           // OfferSerialCombo.IsEnabled = true;
        }
        private void OnSelChangedOFFERSerialCombo(object sender, SelectionChangedEventArgs e)
        {
            if (OfferSerialCombo.SelectedItem != null)
            {
                if (salesPersonCombo.SelectedIndex != employeesList.Count())
                {
                    workOrder.InitializeSalesWorkOfferInfo(offersAddedToComboList[OfferSerialCombo.SelectedIndex].offer_serial, offersAddedToComboList[OfferSerialCombo.SelectedIndex].offer_version);
                }
                else
                {
                    workOrder.InitializeTechnicalOfficeWorkOfferInfo(offersAddedToComboList[OfferSerialCombo.SelectedIndex].offer_serial, offersAddedToComboList[OfferSerialCombo.SelectedIndex].offer_version);
                }
                
                //if (viewAddCondition != COMPANY_WORK_MACROS.OFFER_REVISE_CONDITION)
                //    workOrder.CopyWorkOffer(tmpWorkOffer);
                SetCompanyNameAddressContactFromOffer();

                if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                {
                    workOrderProductsPage.SetTypeComboBoxes();
                    workOrderProductsPage.SetBrandComboBoxes();
                    workOrderProductsPage.SetModelComboBoxes();
                    workOrderProductsPage.SetQuantityTextBoxes();
                    workOrderProductsPage.SetPriceTextBoxes();
                    workOrderProductsPage.SetPriceComboBoxes();
                    workOrderPaymentAndDeliveryPage.SetDeliveryTimeValues();
                    workOrderPaymentAndDeliveryPage.SetDeliveryPointValue();
                    workOrderAdditionalInfoPage.SetContractTypeValue();
                    workOrderAdditionalInfoPage.SetWarrantyPeriodValues();
                    workOrderAdditionalInfoPage.SetAdditionalDescriptionValue();
                }
                else
                {
                    workOrderProductsPage.SetTypeLabels();
                    workOrderProductsPage.SetBrandLabels();
                    workOrderProductsPage.SetModelLabels();
                    workOrderProductsPage.SetQuantityTextBoxes();
                    workOrderProductsPage.SetPriceTextBoxes();
                    workOrderProductsPage.SetPriceComboBoxes();
                    workOrderPaymentAndDeliveryPage.SetDeliveryTimeValues();
                    workOrderPaymentAndDeliveryPage.SetDeliveryPointValue();
                    workOrderAdditionalInfoPage.SetContractTypeValue();
                    workOrderAdditionalInfoPage.SetWarrantyPeriodValues();
                    workOrderAdditionalInfoPage.SetAdditionalDescriptionValue();
                }
            }
        }
        private void SetCompanyNameAddressContactFromOffer()
        {

            companyNameCombo.SelectedItem = offersAddedToComboList[OfferSerialCombo.SelectedIndex].company_name;


            if (!commonQueriesObject.GetCompanyAddresses(companiesList[companyNameCombo.SelectedIndex].company_serial, ref branchInfo))
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
        private void OnCheckOfferCheckBox(object sender, RoutedEventArgs e)
        {
            OfferSerialCombo.IsEnabled = true;

            companyNameCombo.SelectedItem = null;
            companyAddressCombo.SelectedItem = null;
            contactPersonNameCombo.SelectedItem = null;

            companyNameCombo.IsEnabled = false;
            companyAddressCombo.IsEnabled = false;
            contactPersonNameCombo.IsEnabled = false;
        }

        private void OnUnCheckOfferCheckBox(object sender, RoutedEventArgs e)
        {
            OfferSerialCombo.IsEnabled = false;
            OfferSerialCombo.SelectedItem = null;

            companyNameCombo.SelectedItem = null;
            companyAddressCombo.SelectedItem = null;
            contactPersonNameCombo.SelectedItem = null;


            companyNameCombo.IsEnabled = true;
        }
    }
}
