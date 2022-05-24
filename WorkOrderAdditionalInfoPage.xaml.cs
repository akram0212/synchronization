using _01electronics_library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for WorkOrderAdditionalInfoPage.xaml
    /// </summary>
    public partial class WorkOrderAdditionalInfoPage : Page
    {
        Employee loggedInUser;
        WorkOrder workOrder;
        WordAutomation wordAutomation;

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private IntegrityChecks integrityChecks;
        protected FTPServer fTPObject;

        private List<BASIC_STRUCTS.CONTRACT_STRUCT> contractTypes = new List<BASIC_STRUCTS.CONTRACT_STRUCT>();
        private List<BASIC_STRUCTS.TIMEUNIT_STRUCT> timeUnits = new List<BASIC_STRUCTS.TIMEUNIT_STRUCT>();
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> conditionStartDates = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

        private int viewAddCondition;
        private int warrantyPeriod = 0;
        private int orderValidityPeriod = 0;
        private int drawingDeadlineFrom = 0;
        private int drawingDeadlineTo = 0;
        private int isDrawing = 0;

        private string additionalDescription;

        protected String serverFolderPath;
        protected String serverFileName;

        protected String localFolderPath;
        protected String localFileName;

        protected BackgroundWorker uploadBackground;
        protected BackgroundWorker downloadBackground;

        public WorkOrderBasicInfoPage workOrderBasicInfoPage;
        public WorkOrderProductsPage workOrderProductsPage;
        public WorkOrderPaymentAndDeliveryPage workOrderPaymentAndDeliveryPage;
        public WorkOrderUploadFilesPage workOrderUploadFilesPage;
        public WorkOrderProjectInfoPage workOrderProjectInfoPage;

        public WorkOrderAdditionalInfoPage(ref Employee mLoggedInUser, ref WorkOrder mWorkOrder, int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;
            workOrder = mWorkOrder;

            sqlDatabase = new SQLServer();
            fTPObject = new FTPServer();

            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();
            integrityChecks = new IntegrityChecks();

            wordAutomation = new WordAutomation();

            InitializeComponent();

            ConfigureDrawingSubmissionUIElements();


            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_ADD_CONDITION)
            {
                ConfigureDrawingSubmissionUIElements();
                InitializeContractType();
                InitializeTimeUnitComboBoxes();
                InitializeDrawingDeadlineDateFromWhenComboBox();
                InitializeWarrantyPeriodFromWhenCombo();
                SetContractTypeValue();
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                InitializeContractType();
                InitializeTimeUnitComboBoxes();
                InitializeDrawingDeadlineDateFromWhenComboBox();
                InitializeWarrantyPeriodFromWhenCombo();

                if (workOrder.GetDrawingSubmissionDeadlineTimeUnitId() != 0)
                    drawingSubmissionCheckBox.IsChecked = true;
                drawingSubmissionCheckBox.IsEnabled = false;

                ConfigureUIElementsView();
                SetDrawingSubmissionValues();
                SetContractTypeValue();
                SetWarrantyPeriodValues();
                SetAdditionalDescriptionValue();

                nextButton.IsEnabled = true;
                finishButton.IsEnabled = false;
                cancelButton.IsEnabled = false;
                remainingCharactersWrapPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                //ConfigureDrawingSubmissionUIElements();
                InitializeContractType();
                InitializeTimeUnitComboBoxes();
                InitializeDrawingDeadlineDateFromWhenComboBox();
                InitializeWarrantyPeriodFromWhenCombo();

                //SetDrawingSubmissionValues();
                SetContractTypeValue();
                SetWarrantyPeriodValues();
                SetAdditionalDescriptionValue();

            }
        }
        public WorkOrderAdditionalInfoPage(ref WorkOrder mWorkOrder)
        {
            workOrder = mWorkOrder;
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        ///CONFIGURE UI ELEMENTS FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////

        private void ConfigureDrawingSubmissionUIElements()
        {
            drawingDeadlineFromTextBox.IsEnabled = false;
            drawingDeadlineToTextBox.IsEnabled = false;
            drawingDeadlineDateComboBox.IsEnabled = false;
            drawingDeadlineDateFromWhenComboBox.IsEnabled = false;
            workOrder.SetOrderHasDrawings(false);
        }

        private void EnableDrawingSubmissionUIElements()
        {
            drawingDeadlineFromTextBox.IsEnabled = true;
            drawingDeadlineToTextBox.IsEnabled = true;
            drawingDeadlineDateComboBox.IsEnabled = true;
            drawingDeadlineDateFromWhenComboBox.IsEnabled = true;
            workOrder.SetOrderHasDrawings(true);
        }

        private void ConfigureUIElementsView()
        {
            //drawingConditionsCheckBox.IsEnabled = false;
            drawingDeadlineFromTextBox.IsEnabled = false;
            drawingDeadlineToTextBox.IsEnabled = false;
            drawingDeadlineDateComboBox.IsEnabled = false;
            contractTypeComboBox.IsEnabled = false;
            warrantyPeriodTextBox.IsEnabled = false;
            warrantyPeriodCombo.IsEnabled = false;
            additionalDescriptionTextBox.IsEnabled = false;
            drawingDeadlineDateFromWhenComboBox.IsEnabled = false;
            warrantyPeriodFromWhenCombo.IsEnabled = false;
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        ///INITIALIZATION FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////
        private bool InitializeContractType()
        {
            if (!commonQueriesObject.GetContractTypes(ref contractTypes))
                return false;
            contractTypes.Remove(contractTypes.Find(s1 => s1.contractId == 5));
            for (int i = 0; i < contractTypes.Count; i++)
                contractTypeComboBox.Items.Add(contractTypes[i].contractName);
            
            return true;
        }

        private bool InitializeTimeUnitComboBoxes()
        {
            if (!commonQueriesObject.GetTimeUnits(ref timeUnits))
                return false;
            for (int i = 0; i < timeUnits.Count(); i++)
            {
                warrantyPeriodCombo.Items.Add(timeUnits[i].timeUnit);
                drawingDeadlineDateComboBox.Items.Add(timeUnits[i].timeUnit);
            }
            return true;
        }
        private bool InitializeDrawingDeadlineDateFromWhenComboBox()
        {
            if (!commonQueriesObject.GetConditionStartDates(ref conditionStartDates))
                return false;

            for (int i = 0; i < conditionStartDates.Count; i++)
                drawingDeadlineDateFromWhenComboBox.Items.Add(conditionStartDates[i].value);
            return true;
        }
        private bool InitializeWarrantyPeriodFromWhenCombo()
        {
            if (!commonQueriesObject.GetConditionStartDates(ref conditionStartDates))
                return false;

            for (int i = 0; i < conditionStartDates.Count; i++)
                warrantyPeriodFromWhenCombo.Items.Add(conditionStartDates[i].value);
            return true;
        }

        //////////////////////////////
        ///SET FUNCTIONS
        //////////////////////////////
        public void SetDrawingSubmissionValues()
        {

            drawingDeadlineFromTextBox.Text = workOrder.GetOrderDrawingSubmissionDeadlineMinimum().ToString();
            drawingDeadlineToTextBox.Text = workOrder.GetOrderDrawingSubmissionDeadlineMaximum().ToString();
            drawingDeadlineDateComboBox.Text = workOrder.GetOrderDrawingDeadlineTimeUnit();
            drawingDeadlineDateFromWhenComboBox.SelectedItem = workOrder.GetOrderDrawingSubmissionDeadlineCondition();

        }

        public void SetContractTypeValue()
        {

            contractTypeComboBox.SelectedItem = workOrder.GetOrderContractType();


        }

        public void SetWarrantyPeriodValues()
        {

            warrantyPeriodTextBox.Text = workOrder.GetOrderWarrantyPeriod().ToString();
            warrantyPeriodCombo.SelectedItem = workOrder.GetOrderWarrantyPeriodTimeUnit();
            warrantyPeriodFromWhenCombo.SelectedItem = workOrder.GetOrderWarrantyPeriodCondition();

        }

        public void SetAdditionalDescriptionValue()
        {
            additionalDescriptionTextBox.Text = workOrder.GetOrderNotes();
        }

        public void SetNullsToZeros()
        {
            if (drawingDeadlineFromTextBox.Text == null)
                drawingDeadlineFromTextBox.Text = "0";

            if (drawingDeadlineToTextBox.Text == null)
                drawingDeadlineToTextBox.Text = "0";

            if (warrantyPeriodTextBox.Text == null)
                warrantyPeriodTextBox.Text = "0";
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////



        private void WarrantyPeriodComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (warrantyPeriodCombo.SelectedIndex != -1)
            {
                workOrder.SetOrderWarrantyPeriodTimeUnit(timeUnits[warrantyPeriodCombo.SelectedIndex].timeUnitId, timeUnits[warrantyPeriodCombo.SelectedIndex].timeUnit);
            }
        }

        private void ContractTypeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(contractTypeComboBox.SelectedIndex != -1)
            {
                workOrder.SetOrderContractType(contractTypes[contractTypeComboBox.SelectedIndex].contractId, contractTypes[contractTypeComboBox.SelectedIndex].contractName);
            }
          
        }

        private void DrawingDeadlineDateFromWhenComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (drawingDeadlineDateFromWhenComboBox.SelectedIndex != -1)
            {
                workOrder.SetOrderDrawingSubmissionDeadlineCondition(conditionStartDates[drawingDeadlineDateFromWhenComboBox.SelectedIndex].key, conditionStartDates[drawingDeadlineDateFromWhenComboBox.SelectedIndex].value);
            }
        }

        private void WarrantyPeriodFromWhenComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (warrantyPeriodFromWhenCombo.SelectedIndex != -1)
            {
                workOrder.SetOrderWarrantyPeriodCondition(conditionStartDates[warrantyPeriodFromWhenCombo.SelectedIndex].key, conditionStartDates[warrantyPeriodFromWhenCombo.SelectedIndex].value);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //TEXT CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void WarrantyPeriodTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(warrantyPeriodTextBox.Text, BASIC_MACROS.PHONE_STRING) && warrantyPeriodTextBox.Text != "")
            {
                warrantyPeriod = int.Parse(warrantyPeriodTextBox.Text);
                workOrder.SetOrderWarrantyPeriod(warrantyPeriod);
            }
            else
            {
                warrantyPeriod = 0;
                warrantyPeriodTextBox.Text = null;
            }
        }

        private void AdditionalDescriptionTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (additionalDescriptionTextBox.Text.Length <= COMPANY_WORK_MACROS.MAX_NOTES_TEXTBOX_CHAR_VALUE)
                additionalDescription = additionalDescriptionTextBox.Text;
            additionalDescriptionTextBox.Text = additionalDescription;
            additionalDescriptionTextBox.Select(additionalDescriptionTextBox.Text.Length, 0);
            counterLabel.Content = COMPANY_WORK_MACROS.MAX_NOTES_TEXTBOX_CHAR_VALUE - additionalDescriptionTextBox.Text.Length;
            workOrder.SetOrderNotes(additionalDescriptionTextBox.Text);
        }
        private void DrawingDeadlineFromTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(drawingDeadlineFromTextBox.Text, BASIC_MACROS.PHONE_STRING) && drawingDeadlineFromTextBox.Text != "")
            {
                drawingDeadlineFrom = int.Parse(drawingDeadlineFromTextBox.Text);
                workOrder.SetOrderDrawingSubmissionDeadlineMinimum(drawingDeadlineFrom);
            }
            else
            {
                drawingDeadlineFrom = 0;
                drawingDeadlineFromTextBox.Text = null;
            }
        }
        private void DrawingDeadlineToTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(drawingDeadlineToTextBox.Text, BASIC_MACROS.PHONE_STRING) && drawingDeadlineToTextBox.Text != "")
            {
                drawingDeadlineTo = int.Parse(drawingDeadlineToTextBox.Text);
                workOrder.SetOrderDrawingSubmissionDeadlineMaximum(drawingDeadlineTo);
            }
            else
            {
                drawingDeadlineTo = 0;
                drawingDeadlineToTextBox.Text = null;
            }
        }
        private void DrawingDeadlineDateComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (drawingDeadlineDateComboBox.SelectedItem != null)
            {
                workOrder.SetOrderHasDrawings(true);
                workOrder.SetOrderDrawingSubmissionDeadlineTimeUnit(timeUnits[drawingDeadlineDateComboBox.SelectedIndex].timeUnitId, timeUnits[drawingDeadlineDateComboBox.SelectedIndex].timeUnit);
            }
            else
            {
                workOrder.SetOrderHasDrawings(false);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///CHECK BOXES EVENT HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void DrawingConditionsCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            drawingDeadlineFromTextBox.IsEnabled = true;
            drawingDeadlineToTextBox.IsEnabled = true;
            drawingDeadlineDateComboBox.IsEnabled = true;
            isDrawing = 1;
        }

        private void DrawingConditionsCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            drawingDeadlineFromTextBox.IsEnabled = false;
            drawingDeadlineToTextBox.IsEnabled = false;
            drawingDeadlineDateComboBox.IsEnabled = false;
            isDrawing = 0;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderBasicInfoPage.workOrderProductsPage = workOrderProductsPage;
            workOrderBasicInfoPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
            workOrderBasicInfoPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderBasicInfoPage.workOrderAdditionalInfoPage = this;
            workOrderBasicInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderBasicInfoPage);
        }

        private void OnClickProjectInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderProjectInfoPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderProjectInfoPage.workOrderProductsPage = workOrderProductsPage;
            workOrderProjectInfoPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderProjectInfoPage.workOrderAdditionalInfoPage = this;
            workOrderProjectInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderProjectInfoPage);
        }

        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderProductsPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderProductsPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
            workOrderProductsPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderProductsPage.workOrderAdditionalInfoPage = this;
            workOrderProductsPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderProductsPage);
        }
        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderPaymentAndDeliveryPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderProductsPage = workOrderProductsPage;
            workOrderPaymentAndDeliveryPage.workOrderAdditionalInfoPage = this;
            workOrderPaymentAndDeliveryPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderPaymentAndDeliveryPage);
        }
        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {

        }
        private void OnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                workOrderUploadFilesPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
                workOrderUploadFilesPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
                workOrderUploadFilesPage.workOrderProductsPage = workOrderProductsPage;
                workOrderUploadFilesPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
                workOrderUploadFilesPage.workOrderAdditionalInfoPage = this;

                NavigationService.Navigate(workOrderUploadFilesPage);
            }
        }

        private void OnClickNextButton(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                workOrderUploadFilesPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
                workOrderUploadFilesPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
                workOrderUploadFilesPage.workOrderProductsPage = workOrderProductsPage;
                workOrderUploadFilesPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
                workOrderUploadFilesPage.workOrderAdditionalInfoPage = this;

                NavigationService.Navigate(workOrderUploadFilesPage);
            }
        }

        private void OnClickBackButton(object sender, RoutedEventArgs e)
        {
            workOrderPaymentAndDeliveryPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderProductsPage = workOrderProductsPage;
            workOrderPaymentAndDeliveryPage.workOrderAdditionalInfoPage = this;
            workOrderPaymentAndDeliveryPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderPaymentAndDeliveryPage);
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///BUTTON CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnButtonClickAutomateWorkOrder(object sender, RoutedEventArgs e)
        {
            wordAutomation.AutomateWorkOrder(workOrder);
        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;

            currentWindow.Close();
        }

        private void OnBtnClickFinish(object sender, RoutedEventArgs e)
        {
            //AN MAKE IT POP UP AS AN ERROR NOT MESSAGE
            if (workOrder.GetSalesPersonId() == 0)
                System.Windows.Forms.MessageBox.Show("Sales person must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (workOrder.GetCompanyName() == null)
                System.Windows.Forms.MessageBox.Show("Company must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (workOrder.GetAddressSerial() == 0)
                System.Windows.Forms.MessageBox.Show("Company address must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (workOrder.GetContactId() == 0)
                System.Windows.Forms.MessageBox.Show("Contact must be specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (workOrder.GetOrderProduct1TypeId() != 0 && workOrder.GetOrderProduct1Quantity() == 0)
                System.Windows.Forms.MessageBox.Show("Product 1 quantity must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (workOrder.GetOrderProduct2TypeId() != 0 && workOrder.GetOrderProduct2Quantity() == 0)
                System.Windows.Forms.MessageBox.Show("Product 2 quantity must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (workOrder.GetOrderProduct3TypeId() != 0 && workOrder.GetOrderProduct3Quantity() == 0)
                System.Windows.Forms.MessageBox.Show("Product 3 quantity must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (workOrder.GetOrderProduct4TypeId() != 0 && workOrder.GetOrderProduct4Quantity() == 0)
                System.Windows.Forms.MessageBox.Show("Product 4 quantity must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (workOrder.GetOrderPercentDownPayment() + workOrder.GetOrderPercentOnDelivery() + workOrder.GetOrderPercentOnInstallation() != 100)
                System.Windows.Forms.MessageBox.Show("Error in payment condition values", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (workOrder.GetOrderContractTypeId() == 0)
                System.Windows.Forms.MessageBox.Show("Contract type must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (workOrder.orderSerial == 0)
                System.Windows.Forms.MessageBox.Show("Work order serial must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (workOrder.GetOrderIssueDate().ToString().Contains("1/1/0001"))
                System.Windows.Forms.MessageBox.Show("Work order issue date must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            else
            {
                if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_ADD_CONDITION || viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_RESOLVE_CONDITION)
                {
                    if (!workOrder.IssueNewOrder())
                        return;
                    if (workOrder.GetOfferID() != null)
                        if (!workOrder.ConfirmOffer())
                            return;
                }
                else if(viewAddCondition == COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION)
                {
                    if (!workOrder.EditWorkOrder(workOrderBasicInfoPage.oldWorkOrder))
                        return;
                }

              viewAddCondition = COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION;

              WorkOrderWindow viewOffer = new WorkOrderWindow(ref loggedInUser, ref workOrder, viewAddCondition, true);
              viewOffer.Show();

              NavigationWindow currentWindow = (NavigationWindow)this.Parent;
              currentWindow.Close();


            }
        }

        private void OrderSerialTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void OnSelChangedOrderIssueDate(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OrderIDTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void OnCheckDrawingSubmission(object sender, RoutedEventArgs e)
        {
            EnableDrawingSubmissionUIElements();
        }

        private void OnUnCheckDrawingSubmission(object sender, RoutedEventArgs e)
        {
            ConfigureDrawingSubmissionUIElements();
        }

        
    }
}
