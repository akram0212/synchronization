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
        public WorkOrder oldWorkOrder;
        Quotation tmpWorkOffer;
        private IntegrityChecks integrityChecks = new IntegrityChecks();

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;

        private List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> companiesList = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> companiesAddedToComboList = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> branchInfo = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT> contactInfo = new List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> employeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        //private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> techOfficeList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT> outgoingQuotationsList = new List<COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT> offersAddedToComboList = new List<COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT>();

        private int viewAddCondition;
        private int salesPersonID;
        private int salesPersonTeamID;
        public bool InitializationComplete;

        public WorkOrderProductsPage workOrderProductsPage;
        public WorkOrderPaymentAndDeliveryPage workOrderPaymentAndDeliveryPage;
        public WorkOrderAdditionalInfoPage workOrderAdditionalInfoPage;
        public WorkOrderUploadFilesPage workOrderUploadFilesPage;
        public WorkOrderProjectInfoPage workOrderProjectInfoPage;
        public WorkOrderBasicInfoPage(ref Employee mLoggedInUser, ref WorkOrder mWorkOrder, int mViewAddCondition, ref WorkOrderProjectInfoPage mWorkOrderProjectInfoPage, ref WorkOrderProductsPage mWorkOrderProductsPage)
        {
            InitializationComplete = false;
            workOrderProjectInfoPage = mWorkOrderProjectInfoPage;
            mWorkOrderProductsPage = workOrderProductsPage;

            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            workOrder = mWorkOrder;
            
            //tmpWorkOffer = new Quotation();

            InitializeComponent();

            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_ADD_CONDITION)
            {
                FillOffersList();
                ConfigureUIElemenetsForAdd();
                InitializeSalesPersonCombo();
                SetSalesPersonComboValue();
                if (workOrder.GetOfferID() != null)
                    SetOfferSerialComboValue();
                if(salesPersonCombo.SelectedItem != null)
                    salesPersonCombo.IsEnabled = false;
                if(OfferSerialCombo.SelectedItem != null)
                {
                    OfferCheckBox.IsEnabled = false;
                    OfferSerialCombo.IsEnabled = false;
                }
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

                orderSerialTextBox.IsEnabled = false;
                orderSerialTextBox.Text = workOrder.orderSerial.ToString();
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION)
            {
                oldWorkOrder = new WorkOrder(sqlDatabase);
                oldWorkOrder.CopyWorkOrder(workOrder);

                FillOffersList();
                ConfigureUIElemenetsForAdd();
                InitializeSalesPersonCombo();
                SetSalesPersonComboValue();

                SetCompanyNameComboValue();
                SetContactPersonComboValue();

                if (workOrder.GetOfferID() != null)
                    SetOfferSerialComboValue();
                else
                    OfferCheckBox.IsEnabled = false;
                companyAddressCombo.IsEnabled = true;
                contactPersonNameCombo.IsEnabled = true;

                orderSerialTextBox.Text = workOrder.orderSerial.ToString();

                //DisableSalesPersonAndOfferCombo();
            }
            else
            {
                FillOffersList();
                ConfigureUIElemenetsForAdd();
                InitializeSalesPersonCombo();
                SetSalesPersonComboValue();

                if (workOrder.GetOfferID() != null)
                    SetOfferSerialComboValue();
                else
                {
                    SetCompanyNameComboValue();
                    SetContactPersonComboValue();
                }

                DisableSalesPersonAndOfferCombo();
            }

            InitializationComplete = true;
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

            salesPersonCombo.Visibility = Visibility.Collapsed;
            OfferSerialCombo.Visibility = Visibility.Collapsed;
            companyNameCombo.Visibility = Visibility.Collapsed;
            companyAddressCombo.Visibility = Visibility.Collapsed;
            contactPersonNameCombo.Visibility = Visibility.Collapsed;

            OfferCheckBox.IsEnabled = false;

        }
        private void ConfigureUIElemenetsForAdd()
        {
            salesPersonLabel.Visibility = Visibility.Collapsed;
            companyNameLabel.Visibility = Visibility.Collapsed;
            companyAddressLabel.Visibility = Visibility.Collapsed;
            contactPersonNameLabel.Visibility = Visibility.Collapsed;
            OfferSerialLabel.Visibility = Visibility.Collapsed;

            salesPersonCombo.Visibility = Visibility.Visible;
            companyNameCombo.Visibility = Visibility.Visible;
            companyAddressCombo.Visibility = Visibility.Visible;
            contactPersonNameCombo.Visibility = Visibility.Visible;
            OfferSerialCombo.Visibility = Visibility.Visible;

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
            salesPersonCombo.IsEnabled = false;
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

            if (OfferCheckBox.IsChecked == true)
                companyNameCombo.IsEnabled = false;

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

            if(viewAddCondition == COMPANY_WORK_MACROS.ORDER_ADD_CONDITION)
                companyAddressCombo.IsEnabled = true;
            if (OfferCheckBox.IsChecked == true)
                companyAddressCombo.IsEnabled = false;
        }

        private void InitializeCompanyContactCombo()
        {
            int addressSerial = branchInfo[companyAddressCombo.SelectedIndex].address_serial;

            if (!commonQueriesObject.GetCompanyContacts(salesPersonID, addressSerial, ref contactInfo))
                return;

            for (int i = 0; i < contactInfo.Count(); i++)
                contactPersonNameCombo.Items.Add(contactInfo[i].contact_name);


            contactPersonNameCombo.SelectedIndex = 0;
            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION)
                contactPersonNameCombo.IsEnabled = true;
            if (OfferCheckBox.IsChecked == true)
                contactPersonNameCombo.IsEnabled = false;
        }

        private void InitializeOfferSerialCombo()
        {

            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_ADD_CONDITION)
            {
                FillOfferSerialCombo();
            }
            else
            {
                FillOfferSerialCombo();
                OfferSerialCombo.SelectedItem = workOrder.GetOfferID();

            }

        }

        private bool FillOffersList()
        {
            if (!commonQueriesObject.GetOutgoingQuotations(ref outgoingQuotationsList))
                return false;
            return true;
        }
        
        private void FillOfferSerialCombo()
        {
            offersAddedToComboList.Clear();
            OfferSerialCombo.Items.Clear();

            if (salesPersonCombo.SelectedItem != null)
            {
                if (loggedInUser.GetEmployeePositionId() <= COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION)
                {
                    for (int i = 0; i < outgoingQuotationsList.Count(); i++)
                    {
                        if (outgoingQuotationsList[i].sales_person_id == salesPersonID && outgoingQuotationsList[i].offer_status_id == COMPANY_WORK_MACROS.PENDING_OUTGOING_QUOTATION)
                        {
                            OfferSerialCombo.Items.Add(outgoingQuotationsList[i].offer_id);
                            offersAddedToComboList.Add(outgoingQuotationsList[i]);
                        }
                    }
                }

                else 
                {
                    for (int i = 0; i < outgoingQuotationsList.Count(); i++)
                    {
                        if (outgoingQuotationsList[i].sales_person_id == salesPersonID && outgoingQuotationsList[i].offer_proposer_id == loggedInUser.GetEmployeeId() && outgoingQuotationsList[i].offer_status_id == COMPANY_WORK_MACROS.PENDING_OUTGOING_QUOTATION)
                        {
                            OfferSerialCombo.Items.Add(outgoingQuotationsList[i].offer_id);
                            offersAddedToComboList.Add(outgoingQuotationsList[i]);
                        }
                    }
                }

            }
        }
    

    

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///SET FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
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
            OfferSerialLabel.Content = workOrder.GetOfferID();
        }
        private void SetOfferSerialComboValue()
        {
            OfferCheckBox.IsChecked = true;
            OfferCheckBox.IsEnabled = false;
            OfferSerialCombo.SelectedItem = workOrder.GetOfferID();
            OfferSerialCombo.IsEnabled = false;
        }

       
        private void SetCompanyNameLabel()
        {
            companyNameLabel.Content = workOrder.GetCompanyName();
        }

        private void SetCompanyNameComboValue()
        {
            companyNameCombo.Text = workOrder.GetCompanyName();
            //companyNameCombo.IsEnabled = false;
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
            //contactPersonNameCombo.Text = workOrder.GetContactName();
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

            //InitializeCompanyContactCombo();

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

            //companyNameCombo.IsEnabled = false;

            salesPersonID = employeesList[salesPersonCombo.SelectedIndex].employee_id;
            commonQueriesObject.GetEmployeeTeam(salesPersonID, ref salesPersonTeamID);

            if (salesPersonTeamID != COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID)
            {
                InitializeCompanyNameCombo();
                if(viewAddCondition == COMPANY_WORK_MACROS.ORDER_ADD_CONDITION || workOrder.GetOfferID() != null)
                    OfferCheckBox.IsChecked = true;
            }
            else
            {
                InitializeCompanyNameCombo();
            }
          

            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_ADD_CONDITION)
                workOrder.ResetWorkOrderInfo(salesPersonTeamID);

            if(viewAddCondition != COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION)
                workOrder.SetPreSalesEngineer(loggedInUser.GetEmployeeId(), loggedInUser.GetEmployeeName());


            workOrder.InitializeSalesPersonInfo(salesPersonID);

            if (OfferCheckBox.IsChecked == false)
                companyNameCombo.IsEnabled = true;
        }
       
        private void OnSelChangedCompanyNameCombo(object sender, SelectionChangedEventArgs e)
        {
            companyAddressCombo.Items.Clear();

            if (companyNameCombo.SelectedItem != null && OfferSerialCombo.SelectedItem == null)
            {
                InitializeCompanyAddressCombo();

                if(OfferCheckBox.IsChecked != true)
                { 
                    workOrder.InitializeCompanyInfo(companiesAddedToComboList[companyNameCombo.SelectedIndex].company_serial);
                }
            }

            if (companyNameCombo.SelectedItem == null)
            {
                companyAddressCombo.SelectedItem = null;
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
                //if (OfferCheckBox.IsChecked != true)
                //{
                InitializeCompanyContactCombo();
                if(viewAddCondition == COMPANY_WORK_MACROS.ORDER_ADD_CONDITION && OfferSerialCombo.SelectedIndex == -1)
                    contactPersonNameCombo.IsEnabled = true;
                //}
            }

        }

        private void OnSelChangedContactPersonCombo(object sender, SelectionChangedEventArgs e)
        {
            if (OfferCheckBox.IsChecked != true)
            {
                if (contactPersonNameCombo.SelectedItem != null)
                    workOrder.InitializeContactInfo(contactInfo[contactPersonNameCombo.SelectedIndex].contact_id);
            }

        }

        private void OnSelChangedQuotationSerialCombo(object sender, SelectionChangedEventArgs e)
        {
            if (OfferSerialCombo.SelectedItem != null)
            {
                if (InitializationComplete == true || workOrder.GetOrderID() == null)
                {
                    if(workOrder.GetOfferSerial() == 0)
                        workOrder.InitializeWorkOfferInfo(offersAddedToComboList[OfferSerialCombo.SelectedIndex].offer_serial, offersAddedToComboList[OfferSerialCombo.SelectedIndex].offer_version, offersAddedToComboList[OfferSerialCombo.SelectedIndex].offer_proposer_id);

                    SetCompanyNameAddressContactFromOffer();

                    if (workOrder.GetprojectSerial() != 0)
                    {
                        workOrderProjectInfoPage.projectComboBox.SelectedItem = workOrder.GetprojectName();
                        workOrderProjectInfoPage.projectCheckBox.IsEnabled = true;
                        workOrderProjectInfoPage.projectCheckBox.IsChecked = true;

                    }

                    workOrderProjectInfoPage.workOrderProductsPage.SetCategoryComboBoxesFromOffer();
                    workOrderProjectInfoPage.workOrderProductsPage.SetTypeComboBoxesFromOffer();
                    workOrderProjectInfoPage.workOrderProductsPage.SetBrandComboBoxesFromOffer();
                    workOrderProjectInfoPage.workOrderProductsPage.SetModelComboBoxesFromOffer();
                    workOrderProjectInfoPage.workOrderProductsPage.SetQuantityTextBoxesFromOffer();
                    workOrderProjectInfoPage.workOrderProductsPage.SetPriceTextBoxesFromOffer();
                    workOrderProjectInfoPage.workOrderProductsPage.SetPriceComboBoxesFromOffer();

                    workOrderProjectInfoPage.workOrderProductsPage.workOrderPaymentAndDeliveryPage.totalPriceVATCombo.SelectedItem = workOrder.GetOfferVATCondition();

                    workOrderProjectInfoPage.workOrderProductsPage.workOrderPaymentAndDeliveryPage.downPaymentPercentageTextBox.Text = workOrder.GetPercentDownPayment().ToString();
                    workOrderProjectInfoPage.workOrderProductsPage.workOrderPaymentAndDeliveryPage.onDeliveryPercentageTextBox.Text = workOrder.GetPercentOnDelivery().ToString();
                    workOrderProjectInfoPage.workOrderProductsPage.workOrderPaymentAndDeliveryPage.onInstallationPercentageTextBox.Text = workOrder.GetPercentOnInstallation().ToString();
                    
                    workOrderProjectInfoPage.workOrderProductsPage.workOrderPaymentAndDeliveryPage.deliveryTimeTextBoxFrom.Text = workOrder.GetDeliveryTimeMinimum().ToString();
                    workOrderProjectInfoPage.workOrderProductsPage.workOrderPaymentAndDeliveryPage.deliveryTimeTextBoxTo.Text = workOrder.GetDeliveryTimeMaximum().ToString();
                    workOrderProjectInfoPage.workOrderProductsPage.workOrderPaymentAndDeliveryPage.deliveryTimeCombo.SelectedItem = workOrder.GetDeliveryTimeUnit();

                    workOrderProjectInfoPage.workOrderProductsPage.workOrderPaymentAndDeliveryPage.deliveryTimeFromWhenCombo.SelectedItem = workOrder.GetOfferDeliveryTimeCondition();

                    workOrderProjectInfoPage.workOrderProductsPage.workOrderPaymentAndDeliveryPage.deliveryPointCombo.SelectedItem = workOrder.GetDeliveryPoint();

                    workOrderProjectInfoPage.workOrderProductsPage.workOrderPaymentAndDeliveryPage.workOrderAdditionalInfoPage.contractTypeComboBox.SelectedItem = workOrder.GetOfferContractType();

                    workOrderProjectInfoPage.workOrderProductsPage.workOrderPaymentAndDeliveryPage.workOrderAdditionalInfoPage.warrantyPeriodTextBox.Text = workOrder.GetWarrantyPeriod().ToString();
                    workOrderProjectInfoPage.workOrderProductsPage.workOrderPaymentAndDeliveryPage.workOrderAdditionalInfoPage.warrantyPeriodCombo.SelectedItem = workOrder.GetWarrantyPeriodTimeUnit();
                    workOrderProjectInfoPage.workOrderProductsPage.workOrderPaymentAndDeliveryPage.workOrderAdditionalInfoPage.warrantyPeriodFromWhenCombo.SelectedItem = workOrder.GetOfferWarrantyPeriodCondition();

                    workOrderProjectInfoPage.workOrderProductsPage.workOrderPaymentAndDeliveryPage.workOrderAdditionalInfoPage.additionalDescriptionTextBox.Text = workOrder.GetOfferNotes();
                }

            }
        }

        //private void OnSelChangedAssignedSalesCombo(object sender, SelectionChangedEventArgs e)
        //{
        //    if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION && assignedSalesCombo.SelectedItem != null)
        //    {
        //        if (assignedSalesCombo.SelectedIndex != employeesList.Count())
        //        {
        //            workOrder.SetOrderAssignedSalesID(employeesList[assignedSalesCombo.SelectedIndex].employee_id);
        //        }
        //        else
        //            workOrder.SetOrderAssignedSalesID(loggedInUser.GetEmployeeId());
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
            workOrderProjectInfoPage.workOrderBasicInfoPage = this;
            workOrderProjectInfoPage.workOrderProductsPage = workOrderProductsPage;
            workOrderProjectInfoPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderProjectInfoPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderProjectInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;


            NavigationService.Navigate(workOrderProjectInfoPage);

        }
        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderProductsPage.workOrderBasicInfoPage = this;
            workOrderProductsPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
            workOrderProductsPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderProductsPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderProductsPage.workOrderUploadFilesPage = workOrderUploadFilesPage;


            NavigationService.Navigate(workOrderProductsPage);
        }
        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderPaymentAndDeliveryPage.workOrderBasicInfoPage = this;
            workOrderPaymentAndDeliveryPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderProductsPage = workOrderProductsPage;
            workOrderPaymentAndDeliveryPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderPaymentAndDeliveryPage);
        }
        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderAdditionalInfoPage.workOrderBasicInfoPage = this;
            workOrderAdditionalInfoPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
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
                workOrderUploadFilesPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
                workOrderUploadFilesPage.workOrderProductsPage = workOrderProductsPage;
                workOrderUploadFilesPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
                workOrderUploadFilesPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;

                NavigationService.Navigate(workOrderUploadFilesPage);
            }
        }

        private void OnBtnClickNext(object sender, RoutedEventArgs e)
        {
            workOrderProjectInfoPage.workOrderBasicInfoPage = this;
            workOrderProjectInfoPage.workOrderProductsPage = workOrderProductsPage;
            workOrderProjectInfoPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderProjectInfoPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderProjectInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;


            NavigationService.Navigate(workOrderProjectInfoPage);
        }

        public void NavigateToUploadFilesPage()
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                workOrderUploadFilesPage.workOrderBasicInfoPage = this;
                workOrderUploadFilesPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
                workOrderUploadFilesPage.workOrderProductsPage = workOrderProductsPage;
                workOrderUploadFilesPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
                workOrderUploadFilesPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;

                NavigationService.Navigate(workOrderUploadFilesPage);
            }
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



        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///CHECK HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnCheckOfferCheckBox(object sender, RoutedEventArgs e)
        {
            OfferSerialCombo.IsEnabled = true;

            InitializeOfferSerialCombo();

            //companyNameCombo.SelectedItem = null;
            //companyAddressCombo.SelectedItem = null;
            //contactPersonNameCombo.SelectedItem = null;
            //
            //companyNameCombo.IsEnabled = false;
            //companyAddressCombo.IsEnabled = false;
            //contactPersonNameCombo.IsEnabled = false;
            //
            //workOrderProjectInfoPage.projectCheckBox.IsEnabled = false;
            //workOrderProjectInfoPage.checkAllCheckBox.IsEnabled = false;
        }

        private void OnUnCheckOfferCheckBox(object sender, RoutedEventArgs e)
        {
            OfferSerialCombo.IsEnabled = false;
            OfferSerialCombo.SelectedItem = null;

            companyNameCombo.SelectedItem = null;
            companyAddressCombo.SelectedItem = null;
            contactPersonNameCombo.SelectedItem = null;

            salesPersonCombo.IsEnabled = true;

            //if (salesPersonCombo.SelectedItem == loggedInUser.GetEmployeeName())
            //{
                companyNameCombo.IsEnabled = true;
                InitializeCompanyNameCombo();
            //}
        }

        private void OnTextChangedOrderSerialTextBox(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(orderSerialTextBox.Text, BASIC_MACROS.PHONE_STRING) && orderSerialTextBox.Text != "")
            {
                workOrder.orderSerial = int.Parse(orderSerialTextBox.Text.ToString());
            }
        }
    }
}
