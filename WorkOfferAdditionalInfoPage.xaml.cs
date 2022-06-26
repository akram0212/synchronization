using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
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
        Quotation quotation;
        WordAutomation wordAutomation;

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private IntegrityChecks integrityChecks;
        protected FTPServer fTPObject;

        
        private List<BASIC_STRUCTS.TIMEUNIT_STRUCT> timeUnits = new List<BASIC_STRUCTS.TIMEUNIT_STRUCT>();
        private List<BASIC_STRUCTS.CONDITION_START_DATES_STRUCT> conditionStartDates = new List<BASIC_STRUCTS.CONDITION_START_DATES_STRUCT>();

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

        public WorkOfferAdditionalInfoPage(ref Employee mLoggedInUser, ref Quotation mWorkOffer, int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;
            quotation = mWorkOffer;

            sqlDatabase = new SQLServer();
            fTPObject = new FTPServer();

            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();
            integrityChecks = new IntegrityChecks();

            wordAutomation = new WordAutomation();

            InitializeComponent();

            quotation.SetHasDrawings(false);
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


                if (quotation.GetDrawingSubmissionDeadlineTimeUnitId() != 0)
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
                remainingCharactersWrapPanel.Visibility = Visibility.Collapsed;
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

                if (quotation.GetDrawingSubmissionDeadlineMinimum() != 0)
                    drawingSubmissionCheckBox.IsChecked = true;

            }
            else
            {
                InitializeTimeUnitComboBoxes();
                InitializeDrawingDeadlineDateFromWhenComboBox();
                InitializeWarrantyPeriodFromWhenCombo();
            }
        }
        public WorkOfferAdditionalInfoPage(ref Quotation mWorkOffer)
        {
            quotation = mWorkOffer;
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
                drawingDeadlineDateFromWhenComboBox.Items.Add(conditionStartDates[i].condition_type);

            drawingDeadlineDateFromWhenComboBox.SelectedIndex = drawingDeadlineDateFromWhenComboBox.Items.Count - 1;

            return true;
        }
        private bool InitializeWarrantyPeriodFromWhenCombo()
        {
            if (!commonQueriesObject.GetConditionStartDates(ref conditionStartDates))
                return false;

            for (int i = 0; i < conditionStartDates.Count; i++)
                warrantyPeriodFromWhenCombo.Items.Add(conditionStartDates[i].condition_type);
            return true;
        }

        //////////////////////////////
        ///SET FUNCTIONS
        //////////////////////////////
        private void SetDrawingSubmissionValues()
        {
            drawingDeadlineFromTextBox.Text = quotation.GetDrawingSubmissionDeadlineMinimum().ToString();
            drawingDeadlineToTextBox.Text = quotation.GetDrawingSubmissionDeadlineMaximum().ToString();
            drawingDeadlineDateComboBox.Text = quotation.GetDrawingDeadlineTimeUnit();
            drawingDeadlineDateFromWhenComboBox.SelectedItem = quotation.GetOfferDrawingSubmissionDeadlineCondition();
        }

        

        private void SetWarrantyPeriodValues()
        {
           warrantyPeriodTextBox.Text = quotation.GetWarrantyPeriod().ToString();

                if (quotation.GetWarrantyPeriodTimeUnit() != "")
                    warrantyPeriodCombo.SelectedItem = quotation.GetWarrantyPeriodTimeUnit();
                else
                    warrantyPeriodCombo.SelectedIndex = warrantyPeriodCombo.Items.Count - 1;

                if (quotation.GetOfferWarrantyPeriodCondition() != "")
                    warrantyPeriodFromWhenCombo.SelectedItem = quotation.GetOfferWarrantyPeriodCondition();
                else
                    warrantyPeriodFromWhenCombo.SelectedIndex = warrantyPeriodFromWhenCombo.Items.Count - 1;
           
        }

        private void SetValidityPeriodValues()
        {
            if (quotation.GetOfferValidityPeriod() != 0)
            {
                offerValidityTextBox.Text = quotation.GetOfferValidityPeriod().ToString();
                offerValidityCombo.Text = quotation.GetOfferValidityTimeUnit();
            }
        }

        private void SetAdditionalDescriptionValue()
        {
            additionalDescriptionTextBox.Text = quotation.GetOfferNotes();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        

        private void WarrantyPeriodComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (warrantyPeriodCombo.SelectedItem != null)
                quotation.SetWarrantyPeriodTimeUnit(timeUnits[warrantyPeriodCombo.SelectedIndex].timeUnitId, timeUnits[warrantyPeriodCombo.SelectedIndex].timeUnit);
        }

        private void OfferValidityComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            quotation.SetOfferValidityTimeUnit(timeUnits[offerValidityCombo.SelectedIndex].timeUnitId, timeUnits[offerValidityCombo.SelectedIndex].timeUnit);
        }

        private void DrawingDeadlineDateFromWhenComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (drawingDeadlineDateFromWhenComboBox.SelectedIndex != -1)
            {
                quotation.SetOfferDrawingSubmissionDeadlineCondition(conditionStartDates[drawingDeadlineDateFromWhenComboBox.SelectedIndex].condition_id, conditionStartDates[drawingDeadlineDateFromWhenComboBox.SelectedIndex].condition_type);
            }
        }

        private void WarrantyPeriodFromWhenComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (warrantyPeriodFromWhenCombo.SelectedIndex != -1)
            {
                quotation.SetOfferWarrantyPeriodCondition(conditionStartDates[warrantyPeriodFromWhenCombo.SelectedIndex].condition_id, conditionStartDates[warrantyPeriodFromWhenCombo.SelectedIndex].condition_type);
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
                quotation.SetWarrantyPeriod(warrantyPeriod);
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
                quotation.SetOfferValidityPeriod(offerValidityPeriod);
            }
            else
            {
                offerValidityPeriod = 0;
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
            quotation.SetOfferNotes(additionalDescriptionTextBox.Text);
        }
        private void DrawingDeadlineFromTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(drawingDeadlineFromTextBox.Text, BASIC_MACROS.PHONE_STRING) && drawingDeadlineFromTextBox.Text != "")
            {
                drawingDeadlineFrom = int.Parse(drawingDeadlineFromTextBox.Text);
                quotation.SetDrawingSubmissionDeadlineMinimum(drawingDeadlineFrom);
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
                quotation.SetDrawingSubmissionDeadlineMaximum(drawingDeadlineTo);
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
                quotation.SetHasDrawings(true);
                quotation.SetDrawingSubmissionDeadlineTimeUnit(timeUnits[drawingDeadlineDateComboBox.SelectedIndex].timeUnitId, timeUnits[drawingDeadlineDateComboBox.SelectedIndex].timeUnit);
            }
            else
            {
                quotation.SetHasDrawings(false);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///CHECK BOXES EVENT HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnCheckDrawingSubmission(object sender, RoutedEventArgs e)
        {
            EnableDrawingSubmissionUIElements();
            isDrawing = 1;
            quotation.SetHasDrawings(true);
        }

        private void OnUnCheckDrawingSubmission(object sender, RoutedEventArgs e)
        {
            ConfigureDrawingSubmissionUIElements();

            drawingDeadlineFromTextBox.Text = null;
            drawingDeadlineToTextBox.Text = null;
            drawingDeadlineDateComboBox.Text = null;
            drawingDeadlineDateFromWhenComboBox.SelectedIndex = drawingDeadlineDateFromWhenComboBox.Items.Count - 1;
            isDrawing = 0;
            quotation.SetHasDrawings(false);
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
            wordAutomation.AutomateWorkOffer(quotation);
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
            if (quotation.GetSalesPersonId() == 0)
                System.Windows.Forms.MessageBox.Show("Sales person must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (quotation.GetCompanyName() == null)
                System.Windows.Forms.MessageBox.Show("Company must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (quotation.GetAddressSerial() == 0)
                System.Windows.Forms.MessageBox.Show("Company address must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (quotation.GetContactId() == 0)
                System.Windows.Forms.MessageBox.Show("Contact must be specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (quotation.GetOfferProduct1TypeId() != 0 && quotation.GetProduct1PriceValue() == 0)
                System.Windows.Forms.MessageBox.Show("Product 1 price  must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (quotation.GetOfferProduct2TypeId() != 0 && quotation.GetProduct2PriceValue() == 0)
                System.Windows.Forms.MessageBox.Show("Product 2 price  must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (quotation.GetOfferProduct3TypeId() != 0 && quotation.GetProduct3PriceValue() == 0)
                System.Windows.Forms.MessageBox.Show("Product 3 price must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (quotation.GetOfferProduct4TypeId() != 0 && quotation.GetProduct4PriceValue() == 0)
                System.Windows.Forms.MessageBox.Show("Product 4 price must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (quotation.GetPercentDownPayment() + quotation.GetPercentOnDelivery() + quotation.GetPercentOnInstallation() != 100)
                System.Windows.Forms.MessageBox.Show("Error in payment condition values", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (quotation.GetOfferContractTypeId() == 0)
                System.Windows.Forms.MessageBox.Show("Contract type must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_ADD_CONDITION || viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_RESOLVE_CONDITION)
                {
                    if (!quotation.IssueNewOffer())
                        return;

                    if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
                    {
                        viewAddCondition = COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION;

                        WorkOfferWindow viewOffer = new WorkOfferWindow(ref loggedInUser, ref quotation, viewAddCondition, true);

                        NavigationWindow currentWindow = (NavigationWindow)this.Parent;
                        currentWindow.Close();

                        viewOffer.Show();
                    }
                }
                else if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_REVISE_CONDITION)
                {
                    if (!quotation.ReviseOffer())
                        return;

                    if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
                    {
                        viewAddCondition = COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION;

                        WorkOfferWindow viewOffer = new WorkOfferWindow(ref loggedInUser, ref quotation, viewAddCondition, true);

                        NavigationWindow currentWindow = (NavigationWindow)this.Parent;
                        currentWindow.Close();

                        viewOffer.Show();
                    }
                }
            }
        }

        

        
    }
}
