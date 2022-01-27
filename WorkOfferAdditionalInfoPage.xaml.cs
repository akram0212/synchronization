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
using Microsoft.Win32;
using _01electronics_library;
using System.ComponentModel;
using System.IO;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for WorkOfferAdditionalInfoPage.xaml
    /// </summary>
    public partial class WorkOfferAdditionalInfoPage : Page
    {
        Employee loggedInUser;
        OutgoingQuotation outgoingQuotation;
        WordAutomation wordAutomation;

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private IntegrityChecks integrityChecks;
        protected FTPServer fTPObject;

        
        private List<BASIC_STRUCTS.TIMEUNIT_STRUCT> timeUnits = new List<BASIC_STRUCTS.TIMEUNIT_STRUCT>();
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> conditionStartDates = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

        private int viewAddCondition;
        private int warrantyPeriod = 0;
        private int offerValidityPeriod = 0;
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

        public WorkOfferBasicInfoPage workOfferBasicInfoPage;
        public WorkOfferProductsPage workOfferProductsPage;
        public WorkOfferPaymentAndDeliveryPage workOfferPaymentAndDeliveryPage;
        public WorkOfferUploadFilesPage workOfferUploadFilesPage;

        public WorkOfferAdditionalInfoPage(ref Employee mLoggedInUser, ref OutgoingQuotation mWorkOffer, int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;
            outgoingQuotation = mWorkOffer;

            sqlDatabase = new SQLServer();
            fTPObject = new FTPServer();

            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();
            integrityChecks = new IntegrityChecks();

            wordAutomation = new WordAutomation();

            InitializeComponent();

            outgoingQuotation.SetHasDrawings(false);
            ConfigureDrawingSubmissionUIElements();


            if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_ADD_CONDITION)
            {
                
                InitializeTimeUnitComboBoxes();
                InitializeDrawingDeadlineDateFromWhenComboBox();
                InitializeWarrantyPeriodFromWhenCombo();
               
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                
                InitializeTimeUnitComboBoxes();
                InitializeDrawingDeadlineDateFromWhenComboBox();
                InitializeWarrantyPeriodFromWhenCombo();


                if (outgoingQuotation.GetDrawingSubmissionDeadlineTimeUnitId() != 0)
                    drawingSubmissionCheckBox.IsChecked = true;
                
                drawingSubmissionCheckBox.IsEnabled = false;

                ConfigureUIElementsView();
                SetDrawingSubmissionValues();
                SetWarrantyPeriodValues();
                SetValidityPeriodValues();
                SetAdditionalDescriptionValue();

                nextButton.IsEnabled = true;
                finishButton.IsEnabled = false;
                cancelButton.IsEnabled = false;
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_REVISE_CONDITION)
            {
                
                InitializeTimeUnitComboBoxes();
                InitializeDrawingDeadlineDateFromWhenComboBox();
                InitializeWarrantyPeriodFromWhenCombo();

                SetDrawingSubmissionValues();
                SetWarrantyPeriodValues();
                SetValidityPeriodValues();
                SetAdditionalDescriptionValue();

                if (outgoingQuotation.GetDrawingSubmissionDeadlineMinimum() != 0)
                    drawingSubmissionCheckBox.IsChecked = true;

            }
            else
            {
                InitializeTimeUnitComboBoxes();
                InitializeDrawingDeadlineDateFromWhenComboBox();
                InitializeWarrantyPeriodFromWhenCombo();
            }
        }
        public WorkOfferAdditionalInfoPage(ref OutgoingQuotation mWorkOffer)
        {
            outgoingQuotation = mWorkOffer;
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
        }

        private void EnableDrawingSubmissionUIElements()
        {
            drawingDeadlineFromTextBox.IsEnabled = true;
            drawingDeadlineToTextBox.IsEnabled = true;
            drawingDeadlineDateComboBox.IsEnabled = true;
            drawingDeadlineDateFromWhenComboBox.IsEnabled = true;
        }

        private void ConfigureUIElementsView()
        {
            //drawingConditionsCheckBox.IsEnabled = false;
            drawingDeadlineFromTextBox.IsEnabled = false;
            drawingDeadlineToTextBox.IsEnabled = false;
            drawingDeadlineDateComboBox.IsEnabled = false;
            warrantyPeriodTextBox.IsEnabled = false;
            warrantyPeriodCombo.IsEnabled = false;
            offerValidityCombo.IsEnabled = false;
            offerValidityTextBox.IsEnabled = false;
            additionalDescriptionTextBox.IsEnabled = false;
            drawingDeadlineDateFromWhenComboBox.IsEnabled = false;
            warrantyPeriodFromWhenCombo.IsEnabled = false;
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        ///INITIALIZATION FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////
        

        private bool InitializeTimeUnitComboBoxes()
        {
            if (!commonQueriesObject.GetTimeUnits(ref timeUnits))
                return false;
            for (int i = 0; i < timeUnits.Count(); i++)
            {
                warrantyPeriodCombo.Items.Add(timeUnits[i].timeUnit);
                offerValidityCombo.Items.Add(timeUnits[i].timeUnit);
                drawingDeadlineDateComboBox.Items.Add(timeUnits[i].timeUnit);
            }
           
            drawingDeadlineDateComboBox.SelectedIndex = drawingDeadlineDateComboBox.Items.Count - 1;
            

            return true;
        }

        private bool InitializeDrawingDeadlineDateFromWhenComboBox()
        {
            if (!commonQueriesObject.GetConditionStartDates(ref conditionStartDates))
                return false;

            for (int i = 0; i < conditionStartDates.Count; i++)
                drawingDeadlineDateFromWhenComboBox.Items.Add(conditionStartDates[i].value);

            drawingDeadlineDateFromWhenComboBox.SelectedIndex = drawingDeadlineDateFromWhenComboBox.Items.Count - 1;

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
        private void SetDrawingSubmissionValues()
        {
            drawingDeadlineFromTextBox.Text = outgoingQuotation.GetDrawingSubmissionDeadlineMinimum().ToString();
            drawingDeadlineToTextBox.Text = outgoingQuotation.GetDrawingSubmissionDeadlineMaximum().ToString();
            drawingDeadlineDateComboBox.Text = outgoingQuotation.GetDrawingDeadlineTimeUnit();
            drawingDeadlineDateFromWhenComboBox.SelectedItem = outgoingQuotation.GetOfferDrawingSubmissionDeadlineCondition();
        }

        

        private void SetWarrantyPeriodValues()
        {
           warrantyPeriodTextBox.Text = outgoingQuotation.GetWarrantyPeriod().ToString();

                if (outgoingQuotation.GetWarrantyPeriodTimeUnit() != "")
                    warrantyPeriodCombo.SelectedItem = outgoingQuotation.GetWarrantyPeriodTimeUnit();
                else
                    warrantyPeriodCombo.SelectedIndex = warrantyPeriodCombo.Items.Count - 1;

                if (outgoingQuotation.GetOfferWarrantyPeriodCondition() != "")
                    warrantyPeriodFromWhenCombo.SelectedItem = outgoingQuotation.GetOfferWarrantyPeriodCondition();
                else
                    warrantyPeriodFromWhenCombo.SelectedIndex = warrantyPeriodFromWhenCombo.Items.Count - 1;
           
        }

        private void SetValidityPeriodValues()
        {
            if (outgoingQuotation.GetOfferValidityPeriod() != 0)
            {
                offerValidityTextBox.Text = outgoingQuotation.GetOfferValidityPeriod().ToString();
                offerValidityCombo.Text = outgoingQuotation.GetOfferValidityTimeUnit();
            }
        }

        private void SetAdditionalDescriptionValue()
        {
            additionalDescriptionTextBox.Text = outgoingQuotation.GetOfferNotes();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        

        private void WarrantyPeriodComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (warrantyPeriodCombo.SelectedItem != null)
                outgoingQuotation.SetWarrantyPeriodTimeUnit(timeUnits[warrantyPeriodCombo.SelectedIndex].timeUnitId, timeUnits[warrantyPeriodCombo.SelectedIndex].timeUnit);
        }

        private void OfferValidityComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            outgoingQuotation.SetOfferValidityTimeUnit(timeUnits[offerValidityCombo.SelectedIndex].timeUnitId, timeUnits[offerValidityCombo.SelectedIndex].timeUnit);
        }

        private void DrawingDeadlineDateFromWhenComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (drawingDeadlineDateFromWhenComboBox.SelectedIndex != -1)
            {
                outgoingQuotation.SetOfferDrawingSubmissionDeadlineCondition(conditionStartDates[drawingDeadlineDateFromWhenComboBox.SelectedIndex].key, conditionStartDates[drawingDeadlineDateFromWhenComboBox.SelectedIndex].value);
            }
        }

        private void WarrantyPeriodFromWhenComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (warrantyPeriodFromWhenCombo.SelectedIndex != -1)
            {
                outgoingQuotation.SetOfferWarrantyPeriodCondition(conditionStartDates[warrantyPeriodFromWhenCombo.SelectedIndex].key, conditionStartDates[warrantyPeriodFromWhenCombo.SelectedIndex].value);
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
                outgoingQuotation.SetWarrantyPeriod(warrantyPeriod);
            }
            else
            {
                warrantyPeriod = 0;
                warrantyPeriodTextBox.Text = null;
            }
        }

        private void OfferValidityTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(offerValidityTextBox.Text, BASIC_MACROS.PHONE_STRING) && offerValidityTextBox.Text != "")
            {
                offerValidityPeriod = int.Parse(offerValidityTextBox.Text);
                outgoingQuotation.SetOfferValidityPeriod(offerValidityPeriod);
            }
            else
            {
                offerValidityPeriod = 0;
                warrantyPeriodTextBox.Text = null;
            }
        }
        private void AdditionalDescriptionTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            //additionalDescription = additionalDescriptionTextBox.Text;
            outgoingQuotation.SetOfferNotes(additionalDescriptionTextBox.Text);
        }
        private void DrawingDeadlineFromTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(drawingDeadlineFromTextBox.Text, BASIC_MACROS.PHONE_STRING) && drawingDeadlineFromTextBox.Text != "")
            {
                drawingDeadlineFrom = int.Parse(drawingDeadlineFromTextBox.Text);
                outgoingQuotation.SetDrawingSubmissionDeadlineMinimum(drawingDeadlineFrom);
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
                outgoingQuotation.SetDrawingSubmissionDeadlineMaximum(drawingDeadlineTo);
            }
            else
            {
                drawingDeadlineTo = 0;
                drawingDeadlineToTextBox.Text = null;
            }
        }
        private void DrawingDeadlineDateComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (drawingDeadlineDateComboBox.SelectedItem != null && drawingSubmissionCheckBox.IsChecked == true)
            {
                outgoingQuotation.SetHasDrawings(true);
                outgoingQuotation.SetDrawingSubmissionDeadlineTimeUnit(timeUnits[drawingDeadlineDateComboBox.SelectedIndex].timeUnitId, timeUnits[drawingDeadlineDateComboBox.SelectedIndex].timeUnit);
            }
            else
            {
                outgoingQuotation.SetHasDrawings(false);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///CHECK BOXES EVENT HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnCheckDrawingSubmission(object sender, RoutedEventArgs e)
        {
            EnableDrawingSubmissionUIElements();
            isDrawing = 1;
            outgoingQuotation.SetHasDrawings(true);
        }

        private void OnUnCheckDrawingSubmission(object sender, RoutedEventArgs e)
        {
            ConfigureDrawingSubmissionUIElements();

            drawingDeadlineFromTextBox.Text = null;
            drawingDeadlineToTextBox.Text = null;
            drawingDeadlineDateComboBox.Text = null;
            drawingDeadlineDateFromWhenComboBox.SelectedIndex = drawingDeadlineDateFromWhenComboBox.Items.Count - 1;
            isDrawing = 0;
            outgoingQuotation.SetHasDrawings(false);
        }

        private void OnCheckWarrantyPeriod(object sender, RoutedEventArgs e)
        {
            warrantyPeriodTextBox.IsEnabled = true;
            warrantyPeriodCombo.IsEnabled = true;
            warrantyPeriodFromWhenCombo.IsEnabled = true;
        }

        private void OnUnCheckWarrantyPeriod(object sender, RoutedEventArgs e)
        {
            warrantyPeriodTextBox.IsEnabled = false;
            warrantyPeriodCombo.IsEnabled = false;
            warrantyPeriodFromWhenCombo.IsEnabled = false;

            warrantyPeriodTextBox.Text = null;
            warrantyPeriodCombo.SelectedIndex = warrantyPeriodCombo.Items.Count - 1;
            warrantyPeriodFromWhenCombo.SelectedIndex = warrantyPeriodFromWhenCombo.Items.Count - 1;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            workOfferBasicInfoPage.workOfferProductsPage = workOfferProductsPage;
            workOfferBasicInfoPage.workOfferPaymentAndDeliveryPage = workOfferPaymentAndDeliveryPage;
            workOfferBasicInfoPage.workOfferAdditionalInfoPage = this;
            workOfferBasicInfoPage.workOfferUploadFilesPage = workOfferUploadFilesPage;

            NavigationService.Navigate(workOfferBasicInfoPage);
        }
        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            workOfferProductsPage.workOfferBasicInfoPage = workOfferBasicInfoPage;
            workOfferProductsPage.workOfferPaymentAndDeliveryPage = workOfferPaymentAndDeliveryPage;
            workOfferProductsPage.workOfferAdditionalInfoPage = this;
            workOfferProductsPage.workOfferUploadFilesPage = workOfferUploadFilesPage;

            NavigationService.Navigate(workOfferProductsPage);
        }
        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {
            workOfferPaymentAndDeliveryPage.workOfferBasicInfoPage = workOfferBasicInfoPage;
            workOfferPaymentAndDeliveryPage.workOfferProductsPage = workOfferProductsPage;
            workOfferPaymentAndDeliveryPage.workOfferAdditionalInfoPage = this;
            workOfferPaymentAndDeliveryPage.workOfferUploadFilesPage = workOfferUploadFilesPage;

            NavigationService.Navigate(workOfferPaymentAndDeliveryPage);
        }
        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {

        }
        private void OnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                workOfferUploadFilesPage.workOfferBasicInfoPage = workOfferBasicInfoPage;
                workOfferUploadFilesPage.workOfferProductsPage = workOfferProductsPage;
                workOfferUploadFilesPage.workOfferPaymentAndDeliveryPage = workOfferPaymentAndDeliveryPage;
                workOfferUploadFilesPage.workOfferAdditionalInfoPage = this;

                NavigationService.Navigate(workOfferUploadFilesPage);
            }
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///BUTTON CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnClickNextButton(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                workOfferUploadFilesPage.workOfferBasicInfoPage = workOfferBasicInfoPage;
                workOfferUploadFilesPage.workOfferProductsPage = workOfferProductsPage;
                workOfferUploadFilesPage.workOfferPaymentAndDeliveryPage = workOfferPaymentAndDeliveryPage;
                workOfferUploadFilesPage.workOfferAdditionalInfoPage = this;

                NavigationService.Navigate(workOfferUploadFilesPage);
            }
        }

        private void OnClickBackButton(object sender, RoutedEventArgs e)
        {
            workOfferPaymentAndDeliveryPage.workOfferBasicInfoPage = workOfferBasicInfoPage;
            workOfferPaymentAndDeliveryPage.workOfferProductsPage = workOfferProductsPage;
            workOfferPaymentAndDeliveryPage.workOfferAdditionalInfoPage = this;
            workOfferPaymentAndDeliveryPage.workOfferUploadFilesPage = workOfferUploadFilesPage;

            NavigationService.Navigate(workOfferPaymentAndDeliveryPage);
        }

        private void OnButtonClickAutomateWorkOffer(object sender, RoutedEventArgs e)
        {
            wordAutomation.AutomateWorkOffer(outgoingQuotation);
        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;

            currentWindow.Close();
        }

        private void OnBtnClickFinish(object sender, RoutedEventArgs e)
        {
            //PLEASE CHANGE THESE MESSAGE
            //AN MAKE IT POP UP AS AN ERROR NOT MESSAGE
            if (outgoingQuotation.GetSalesPersonId() == 0)
                MessageBox.Show("You need to choose sales person before adding a work offer!");
            else if (outgoingQuotation.GetCompanyName() == null)
                MessageBox.Show("You need to choose a company before adding a work offer!");
            else if (outgoingQuotation.GetAddressSerial() == 0)
                MessageBox.Show("You need to choose company address before adding a work offer!");
            else if (outgoingQuotation.GetContactId() == 0)
                MessageBox.Show("You need to choose a contact before adding a work offer!");
            else if (outgoingQuotation.GetOfferProduct1TypeId() != 0 && outgoingQuotation.GetProduct1PriceValue() == 0)
                MessageBox.Show("You need to add a price for product 1 before adding a work offer!");
            else if (outgoingQuotation.GetOfferProduct2TypeId() != 0 && outgoingQuotation.GetProduct2PriceValue() == 0)
                MessageBox.Show("You need to add a price for product 2 before adding a work offer!");
            else if (outgoingQuotation.GetOfferProduct3TypeId() != 0 && outgoingQuotation.GetProduct3PriceValue() == 0)
                MessageBox.Show("You need to add a price for product 3 before adding a work offer!");
            else if (outgoingQuotation.GetOfferProduct4TypeId() != 0 && outgoingQuotation.GetProduct4PriceValue() == 0)
                MessageBox.Show("You need to add a price for product 4 before adding a work offer!");
            else if (outgoingQuotation.GetPercentDownPayment() + outgoingQuotation.GetPercentOnDelivery() + outgoingQuotation.GetPercentOnInstallation() != 100)
                MessageBox.Show("Down payement, on delivery and on installation percentages total is less than 100%!!");
            else if ((workOfferPaymentAndDeliveryPage.deliveryTimeCheckBox.IsChecked == true && outgoingQuotation.GetDeliveryTimeMinimum() == 0) || (workOfferPaymentAndDeliveryPage.deliveryTimeCheckBox.IsChecked == true && outgoingQuotation.GetDeliveryTimeMaximum() == 0))
                MessageBox.Show("You need to set delivery time min and max before adding a work offer!");
            else if (workOfferPaymentAndDeliveryPage.deliveryPointCheckBox.IsChecked == true && outgoingQuotation.GetDeliveryPointId() == 0)
                MessageBox.Show("You need to set delivery point before adding a work offer!");
            else if (outgoingQuotation.GetOfferContractTypeId() == 0)
                MessageBox.Show("You need to set contract type before adding a work offer!");
            else if ((warrantyPeriodCheckBox.IsChecked == true && outgoingQuotation.GetWarrantyPeriod() == 0) || (warrantyPeriodCheckBox.IsChecked == true && outgoingQuotation.GetWarrantyPeriodTimeUnitId() == 0))
                MessageBox.Show("You need to set warranty period before adding a work offer!");
            else if (outgoingQuotation.GetOfferValidityPeriod() == 0 || outgoingQuotation.GetOfferValidityTimeUnitId() == 0)
                MessageBox.Show("You need to set validity period before adding a work offer!");
            else
            {
                if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_ADD_CONDITION || viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_RESOLVE_CONDITION)
                {
                    if (!outgoingQuotation.IssueNewOffer())
                        return;

                     //if (outgoingQuotation.GetRFQID() != null)
                       // if (!outgoingQuotation.ConfirmRFQ())
                         //   return;
                    if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
                    {
                        viewAddCondition = COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION;

                        WorkOfferWindow viewOffer = new WorkOfferWindow(ref loggedInUser, ref outgoingQuotation, viewAddCondition, true);

                        NavigationWindow currentWindow = (NavigationWindow)this.Parent;
                        currentWindow.Close();

                        viewOffer.Show();
                    }
                }
                else if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_REVISE_CONDITION)
                {
                    if (!outgoingQuotation.ReviseOffer())
                        return;

                    if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
                    {
                        viewAddCondition = COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION;

                        WorkOfferWindow viewOffer = new WorkOfferWindow(ref loggedInUser, ref outgoingQuotation, viewAddCondition, true);

                        NavigationWindow currentWindow = (NavigationWindow)this.Parent;
                        currentWindow.Close();

                        viewOffer.Show();
                    }
                }
            }
        }

        

        
    }
}
