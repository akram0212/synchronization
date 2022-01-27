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
        OutgoingQuotation tmpWorkOffer;


        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;

        private List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> companiesList = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> companiesAddedToComboList = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> branchInfo = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT> contactInfo = new List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> employeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> techOfficeList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT> outgoingQuotationsList = new List<COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT> offersAddedToComboList = new List<COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT>();

        private int viewAddCondition;
        private int salesPersonID;
        private int salesPersonTeamID;

        public WorkOrderProductsPage workOrderProductsPage;
        public WorkOrderPaymentAndDeliveryPage workOrderPaymentAndDeliveryPage;
        public WorkOrderAdditionalInfoPage workOrderAdditionalInfoPage;
        public WorkOrderUploadFilesPage workOrderUploadFilesPage;
        public WorkOrderProjectInfoPage workOrderProjectInfoPage;
        public WorkOrderBasicInfoPage(ref Employee mLoggedInUser, ref WorkOrder mWorkOrder, int mViewAddCondition, ref WorkOrderProjectInfoPage mWorkOrderProjectInfoPage, ref WorkOrderProductsPage mWorkOrderProductsPage)
        {
            workOrderProjectInfoPage = mWorkOrderProjectInfoPage;
            mWorkOrderProductsPage = workOrderProductsPage;

            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            workOrder = mWorkOrder;
            //tmpWorkOffer = new OutgoingQuotation();

            InitializeComponent();

            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_ADD_CONDITION)
            {
                FillOffersList();
                ConfigureUIElemenetsForAdd();
                InitializeSalesPersonCombo();
                //InitializeAssignedSalesPersonCombo();
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
            else if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION)
            {
                FillOffersList();
                ConfigureUIElemenetsForAdd();
                InitializeSalesPersonCombo();
                SetSalesPersonComboValue();

                //CHANGE CONDITIONS
                if (workOrder.GetOfferID() != null)
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
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INITIALIZE FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool InitializeSalesPersonCombo()
        {
            if (!commonQueriesObject.GetTeamEmployees(COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID, ref employeesList))
                return false;
            if (!commonQueriesObject.GetTeamEmployees(COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID, ref techOfficeList))
                return false;

            for (int i = 0; i < employeesList.Count(); i++)
            {
                salesPersonCombo.Items.Add(employeesList[i].employee_name);
            }
            for (int i = 0; i < techOfficeList.Count(); i++)
            {
                salesPersonCombo.Items.Add(techOfficeList[i].employee_name);
            }

            salesPersonCombo.Items.Add("Ahmed Ayman Farid");

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

        private void InitializeOfferSerialCombo()
        {

            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_ADD_CONDITION)
            {
                FillOfferSerialCombo();
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION)
            {
                COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT tmp = outgoingQuotationsList.Find(x => x.offer_serial == workOrder.GetOfferSerial());

                OfferSerialCombo.Items.Add(workOrder.GetOfferID());
                offersAddedToComboList.Add(tmp);

                OfferCheckBox.IsEnabled = false;
                OfferCheckBox.IsChecked = true;
                OfferSerialCombo.IsEnabled = false;
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
                if (salesPersonCombo.SelectedIndex < employeesList.Count())
                {
                    for (int i = 0; i < outgoingQuotationsList.Count(); i++)
                    {
                        if (outgoingQuotationsList[i].sales_person_id == employeesList[salesPersonCombo.SelectedIndex].employee_id && outgoingQuotationsList[i].offer_proposer_id == loggedInUser.GetEmployeeId())
                        {
                            OfferSerialCombo.Items.Add(outgoingQuotationsList[i].offer_id);
                            offersAddedToComboList.Add(outgoingQuotationsList[i]);
                        }
                    }
                }

                else if (salesPersonCombo.SelectedIndex < employeesList.Count() + techOfficeList.Count())
                {
                    for (int i = 0; i < outgoingQuotationsList.Count(); i++)
                    {
                        if (outgoingQuotationsList[i].sales_person_id == loggedInUser.GetEmployeeId() && outgoingQuotationsList[i].offer_proposer_id == loggedInUser.GetEmployeeId())
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
                        if (outgoingQuotationsList[i].sales_person_id == 3 && outgoingQuotationsList[i].offer_proposer_id == loggedInUser.GetEmployeeId())
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

            if (salesPersonCombo.SelectedIndex < employeesList.Count())
            {
                salesPersonID = employeesList[salesPersonCombo.SelectedIndex].employee_id;
                salesPersonTeamID = employeesList[salesPersonCombo.SelectedIndex].team.team_id;
                InitializeCompanyNameCombo();
                companyNameCombo.IsEnabled = false;
                OfferCheckBox.IsChecked = true;
            }
            else if (salesPersonCombo.SelectedIndex < employeesList.Count() + techOfficeList.Count())
            {
                salesPersonID = loggedInUser.GetEmployeeId();
                salesPersonTeamID = COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID;

                InitializeCompanyNameCombo();
                //OfferCheckBox.IsChecked = false;
                //SetOfferSerialComboValue();
            }
            else
            {
                salesPersonID = 3;
                salesPersonTeamID = COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID;

                InitializeCompanyNameCombo();
                companyNameCombo.IsEnabled = false;
                OfferCheckBox.IsChecked = true;
            }

            if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_ADD_CONDITION)
                workOrder.ResetWorkOrderInfo(salesPersonTeamID);

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
                    workOrder.InitializeContactInfo(contactInfo[contactPersonNameCombo.SelectedIndex].contact_id);
            }

        }

        private void OnSelChangedQuotationSerialCombo(object sender, SelectionChangedEventArgs e)
        {
            if (OfferSerialCombo.SelectedItem != null)
            {
                if (salesPersonCombo.SelectedIndex != employeesList.Count())
                {
                    workOrder.InitializeSalesWorkOfferInfo(offersAddedToComboList[OfferSerialCombo.SelectedIndex].offer_serial, offersAddedToComboList[OfferSerialCombo.SelectedIndex].offer_version, loggedInUser.GetEmployeeId());
                }
                else
                {
                    workOrder.InitializeTechnicalOfficeWorkOfferInfo(offersAddedToComboList[OfferSerialCombo.SelectedIndex].offer_serial, offersAddedToComboList[OfferSerialCombo.SelectedIndex].offer_version);
                }

                SetCompanyNameAddressContactFromOffer();

                if (workOrder.GetprojectSerial() != 0)
                    workOrderProjectInfoPage.SetProjectComboBox();

                if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                {
                    workOrderProjectInfoPage.workOrderProductsPage.SetCategoryComboBoxes();
                    workOrderProjectInfoPage.workOrderProductsPage.SetTypeComboBoxes();
                    workOrderProjectInfoPage.workOrderProductsPage.SetBrandComboBoxes();
                    workOrderProjectInfoPage.workOrderProductsPage.SetModelComboBoxes();
                    workOrderProjectInfoPage.workOrderProductsPage.SetQuantityTextBoxes();
                    workOrderProjectInfoPage.workOrderProductsPage.SetPriceTextBoxes();
                    workOrderProjectInfoPage.workOrderProductsPage.SetPriceComboBoxes();

                }
                else
                {
                    workOrderProjectInfoPage.workOrderProductsPage.SetCategoryLabels();
                    workOrderProjectInfoPage.workOrderProductsPage.SetTypeLabels();
                    workOrderProjectInfoPage.workOrderProductsPage.SetBrandLabels();
                    workOrderProjectInfoPage.workOrderProductsPage.SetModelLabels();
                    workOrderProjectInfoPage.workOrderProductsPage.SetQuantityTextBoxes();
                    workOrderProjectInfoPage.workOrderProductsPage.SetPriceTextBoxes();
                    workOrderProjectInfoPage.workOrderProductsPage.SetPriceComboBoxes();
                    
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

            companyNameCombo.SelectedItem = null;
            companyAddressCombo.SelectedItem = null;
            contactPersonNameCombo.SelectedItem = null;

            companyNameCombo.IsEnabled = false;
            companyAddressCombo.IsEnabled = false;
            contactPersonNameCombo.IsEnabled = false;

            workOrderProjectInfoPage.projectCheckBox.IsEnabled = false;
            workOrderProjectInfoPage.checkAllCheckBox.IsEnabled = false;
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
