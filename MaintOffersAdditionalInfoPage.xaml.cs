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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for MaintOffersAdditionalInfoPage.xaml
    /// </summary>
    public partial class MaintOffersAdditionalInfoPage : Page
    {
        Employee loggedInUser;
        MaintenanceOffer maintenanceOffer;
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

        protected String serverFolderPath;
        protected String serverFileName;

        protected String localFolderPath;
        protected String localFileName;

        protected BackgroundWorker uploadBackground;
        protected BackgroundWorker downloadBackground;

        protected String additionalDescription;

        public MaintOffersBasicInfoPage maintOffersBasicInfoPage;
        public MaintOffersProductsPage maintOffersProductsPage;
        public MaintOffersPaymentAndDeliveryPage maintOffersPaymentAndDeliveryPage;
        public MaintOffersUploadFilesPage maintOffersUploadFilesPage;

        public MaintOffersAdditionalInfoPage(ref Employee mLoggedInUser, ref MaintenanceOffer mMaintOffers, int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;
            maintenanceOffer = mMaintOffers;

            sqlDatabase = new SQLServer();
            fTPObject = new FTPServer();

            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();
            integrityChecks = new IntegrityChecks();

            wordAutomation = new WordAutomation();

            InitializeComponent();


            if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_ADD_CONDITION)
            {
                InitializeTimeUnitComboBoxes();
                InitializeWarrantyPeriodFromWhenCombo();
               
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                
                InitializeTimeUnitComboBoxes();
                InitializeWarrantyPeriodFromWhenCombo();

                ConfigureUIElementsView();
                SetWarrantyPeriodValues();
                SetAdditionalDescriptionValue();
                SetFrequenciesValue();
                DisableFrequenciesTextBoxes();

                nextButton.IsEnabled = true;
                finishButton.IsEnabled = false;
                cancelButton.IsEnabled = false;
                remainingCharactersWrapPanel.Visibility = Visibility.Collapsed;
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_REVISE_CONDITION)
            {
                InitializeTimeUnitComboBoxes();
                InitializeWarrantyPeriodFromWhenCombo();

                SetWarrantyPeriodValues();
                SetAdditionalDescriptionValue();
                SetFrequenciesValue();

            }
            else
            {
                
                InitializeTimeUnitComboBoxes();
                InitializeWarrantyPeriodFromWhenCombo();
            }
        }
        public MaintOffersAdditionalInfoPage(ref MaintenanceOffer mMaintOffers)
        {
            maintenanceOffer = mMaintOffers;
        }
        /////////////////////////////////////////////////////////////////////////////////////////
        ///CONFIGURE UI ELEMENTS FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////

        private void ConfigureUIElementsView()
        {
            warrantyPeriodTextBox.IsEnabled = false;
            warrantyPeriodCombo.IsEnabled = false;
            additionalDescriptionTextBox.IsEnabled = false;
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
            }

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
        private void SetWarrantyPeriodValues()
        {
            warrantyPeriodTextBox.Text = maintenanceOffer.GetWarrantyPeriod().ToString();

            if (maintenanceOffer.GetWarrantyPeriodTimeUnit() != "")
                warrantyPeriodCombo.SelectedItem = maintenanceOffer.GetWarrantyPeriodTimeUnit();
            else
                warrantyPeriodCombo.SelectedIndex = warrantyPeriodCombo.Items.Count - 1;

            if (maintenanceOffer.GetMaintOfferWarrantyPeriodCondition() != "")
                warrantyPeriodFromWhenCombo.SelectedItem = maintenanceOffer.GetMaintOfferWarrantyPeriodCondition();
            else
                warrantyPeriodFromWhenCombo.SelectedIndex = warrantyPeriodFromWhenCombo.Items.Count - 1;

        }
        private void SetAdditionalDescriptionValue()
        {
            additionalDescriptionTextBox.Text = maintenanceOffer.GetMaintOfferNotes();
        }
        public void SetFrequenciesValue()
        {
            visitsFrequencyTextBox.Text = maintenanceOffer.GetVisitsFrequency().ToString();
            emergenciesFrequencyTextBox.Text = maintenanceOffer.GetEmergenciesFrequency().ToString();
        }
        public void DisableFrequenciesTextBoxes()
        {
            visitsFrequencyTextBox.IsEnabled = false;
            emergenciesFrequencyTextBox.IsEnabled = false;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnTextChangedVisitsFrequencyTextBox(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(visitsFrequencyTextBox.Text, BASIC_MACROS.PHONE_STRING) && visitsFrequencyTextBox.Text != "")
                maintenanceOffer.SetVisitsFrequency(Int32.Parse(visitsFrequencyTextBox.Text));
            else
            {
                maintenanceOffer.SetVisitsFrequency(0);
                visitsFrequencyTextBox.Text = null;
            }
        }

        private void OnTextChangedEmergenciesFrequencyTextBox(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(emergenciesFrequencyTextBox.Text, BASIC_MACROS.PHONE_STRING) && emergenciesFrequencyTextBox.Text != "")
                maintenanceOffer.SetEmergenciesFrequency(Int32.Parse(emergenciesFrequencyTextBox.Text));
            else
            {
                maintenanceOffer.SetEmergenciesFrequency(0);
                emergenciesFrequencyTextBox.Text = null;
            }
        }
        private void WarrantyPeriodComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (warrantyPeriodCombo.SelectedItem != null)
                maintenanceOffer.SetWarrantyPeriodTimeUnit(timeUnits[warrantyPeriodCombo.SelectedIndex].timeUnitId, timeUnits[warrantyPeriodCombo.SelectedIndex].timeUnit);
        }

        private void WarrantyPeriodFromWhenComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (warrantyPeriodFromWhenCombo.SelectedIndex != -1)
            {
                maintenanceOffer.SetMaintOfferWarrantyPeriodCondition(conditionStartDates[warrantyPeriodFromWhenCombo.SelectedIndex].condition_id, conditionStartDates[warrantyPeriodFromWhenCombo.SelectedIndex].condition_type);
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
                maintenanceOffer.SetWarrantyPeriod(warrantyPeriod);
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
            maintenanceOffer.SetMaintOfferNotes(additionalDescriptionTextBox.Text);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///CHECK BOXES EVENT HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

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
            maintOffersBasicInfoPage.maintOffersProductsPage = maintOffersProductsPage;
            maintOffersBasicInfoPage.maintOffersPaymentAndDeliveryPage = maintOffersPaymentAndDeliveryPage;
            maintOffersBasicInfoPage.maintOffersAdditionalInfoPage = this;
            maintOffersBasicInfoPage.maintOffersUploadFilesPage = maintOffersUploadFilesPage;

            NavigationService.Navigate(maintOffersBasicInfoPage);
        }
        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            maintOffersProductsPage.maintOffersBasicInfoPage = maintOffersBasicInfoPage;
            maintOffersProductsPage.maintOffersPaymentAndDeliveryPage = maintOffersPaymentAndDeliveryPage;
            maintOffersProductsPage.maintOffersAdditionalInfoPage = this;
            maintOffersProductsPage.maintOffersUploadFilesPage = maintOffersUploadFilesPage;

            NavigationService.Navigate(maintOffersProductsPage);
        }
        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {
            maintOffersPaymentAndDeliveryPage.maintOffersBasicInfoPage = maintOffersBasicInfoPage;
            maintOffersPaymentAndDeliveryPage.maintOffersProductsPage = maintOffersProductsPage;
            maintOffersPaymentAndDeliveryPage.maintOffersAdditionalInfoPage = this;
            maintOffersPaymentAndDeliveryPage.maintOffersUploadFilesPage = maintOffersUploadFilesPage;

            NavigationService.Navigate(maintOffersPaymentAndDeliveryPage);
        }
        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {

        }
        private void OnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                maintOffersUploadFilesPage.maintOffersBasicInfoPage = maintOffersBasicInfoPage;
                maintOffersUploadFilesPage.maintOffersProductsPage = maintOffersProductsPage;
                maintOffersUploadFilesPage.maintOffersPaymentAndDeliveryPage = maintOffersPaymentAndDeliveryPage;
                maintOffersUploadFilesPage.maintOffersAdditionalInfoPage = this;

                NavigationService.Navigate(maintOffersUploadFilesPage);
            }
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///BUTTON CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnClickNextButton(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                maintOffersUploadFilesPage.maintOffersBasicInfoPage = maintOffersBasicInfoPage;
                maintOffersUploadFilesPage.maintOffersProductsPage = maintOffersProductsPage;
                maintOffersUploadFilesPage.maintOffersPaymentAndDeliveryPage = maintOffersPaymentAndDeliveryPage;
                maintOffersUploadFilesPage.maintOffersAdditionalInfoPage = this;

                NavigationService.Navigate(maintOffersUploadFilesPage);
            }
        }

        private void OnClickBackButton(object sender, RoutedEventArgs e)
        {
            maintOffersPaymentAndDeliveryPage.maintOffersBasicInfoPage = maintOffersBasicInfoPage;
            maintOffersPaymentAndDeliveryPage.maintOffersProductsPage = maintOffersProductsPage;
            maintOffersPaymentAndDeliveryPage.maintOffersAdditionalInfoPage = this;
            maintOffersPaymentAndDeliveryPage.maintOffersUploadFilesPage = maintOffersUploadFilesPage;

            NavigationService.Navigate(maintOffersPaymentAndDeliveryPage);
        }

        private void OnButtonClickAutomateMaintOffer(object sender, RoutedEventArgs e)
        {
           // wordAutomation.AutomateWorkOffer(maintenanceOffer);
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
            if (maintenanceOffer.GetMaintOfferProposerId() == 0)
                MessageBox.Show("You need to choose sales person before adding a work offer!");
            //else if (maintenanceOffer.GetCompanyName() == null)
            //    MessageBox.Show("You need to choose a company before adding a work offer!");
            //else if (maintenanceOffer.GetAddressSerial() == 0)
            //    MessageBox.Show("You need to choose company address before adding a work offer!");
            //else if (maintenanceOffer.GetContactId() == 0)
            //    MessageBox.Show("You need to choose a contact before adding a work offer!");
            else if (maintenanceOffer.GetMaintOfferProduct1TypeId() != 0 && maintenanceOffer.GetProduct1PriceValue() == 0)
                MessageBox.Show("You need to add a price for product 1 before adding a work offer!");
            else if (maintenanceOffer.GetMaintOfferProduct2TypeId() != 0 && maintenanceOffer.GetProduct2PriceValue() == 0)
                MessageBox.Show("You need to add a price for product 2 before adding a work offer!");
            else if (maintenanceOffer.GetMaintOfferProduct3TypeId() != 0 && maintenanceOffer.GetProduct3PriceValue() == 0)
                MessageBox.Show("You need to add a price for product 3 before adding a work offer!");
            else if (maintenanceOffer.GetMaintOfferProduct4TypeId() != 0 && maintenanceOffer.GetProduct4PriceValue() == 0)
                MessageBox.Show("You need to add a price for product 4 before adding a work offer!");
            else
            {
                if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_ADD_CONDITION || viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_RESOLVE_CONDITION)
                {
                   if (!maintenanceOffer.IssueNewMaintOffer())
                        return;

                    if (maintenanceOffer.GetRFQID() != null)
                        if (!maintenanceOffer.ConfirmRFQ())
                            return;
                    if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
                    {
                        viewAddCondition = COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION;

                        MaintenanceOffersWindow viewOffer = new MaintenanceOffersWindow(ref loggedInUser, ref maintenanceOffer, viewAddCondition, true);

                        NavigationWindow currentWindow = (NavigationWindow)this.Parent;
                        currentWindow.Close();

                        viewOffer.Show();
                    }
                }
                else if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_REVISE_CONDITION)
                {
                    if (!maintenanceOffer.ReviseMaintOffer())
                        return;

                    if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
                    {
                        viewAddCondition = COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION;

                        MaintenanceOffersWindow viewOffer = new MaintenanceOffersWindow(ref loggedInUser, ref maintenanceOffer, viewAddCondition, true);

                        NavigationWindow currentWindow = (NavigationWindow)this.Parent;
                        currentWindow.Close();

                        viewOffer.Show();
                    }
                }
            }
        }

    }
}
