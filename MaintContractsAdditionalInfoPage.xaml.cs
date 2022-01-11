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
    /// Interaction logic for MaintContractsAdditionalInfoPage.xaml
    /// </summary>
    public partial class MaintContractsAdditionalInfoPage : Page
    {
        Employee loggedInUser;
        MaintenanceContract maintenanceContract;
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

        protected String serverFolderPath;
        protected String serverFileName;

        protected String localFolderPath;
        protected String localFileName;

        protected BackgroundWorker uploadBackground;
        protected BackgroundWorker downloadBackground;

        public MaintContractsBasicInfoPage maintContractsBasicInfoPage;
        public MaintContractsProductsPage maintContractsProductsPage;
        public MaintContractsProjectsPage maintContractsProjectsPage;
        public MaintContractsPaymentAndDeliveryPage maintContractsPaymentAndDeliveryPage;
        public MaintContractsUploadFilesPage maintContractsUploadFilesPage;

        public MaintContractsAdditionalInfoPage(ref Employee mLoggedInUser, ref MaintenanceContract mMaintContracts, int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;
            maintenanceContract = mMaintContracts;

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

                nextButton.IsEnabled = true;
                finishButton.IsEnabled = false;
                cancelButton.IsEnabled = false;
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_REVISE_CONDITION)
            {
                InitializeTimeUnitComboBoxes();
                InitializeWarrantyPeriodFromWhenCombo();

                SetWarrantyPeriodValues();
                SetAdditionalDescriptionValue();

            }
            else
            {

                InitializeTimeUnitComboBoxes();
                InitializeWarrantyPeriodFromWhenCombo();
            }
        }
        public MaintContractsAdditionalInfoPage(ref MaintenanceContract mMaintContracts)
        {
            maintenanceContract = mMaintContracts;
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
                warrantyPeriodFromWhenCombo.Items.Add(conditionStartDates[i].value);
            return true;
        }

        //////////////////////////////
        ///SET FUNCTIONS
        //////////////////////////////
        private void SetWarrantyPeriodValues()
        {
            warrantyPeriodTextBox.Text = maintenanceContract.GetWarrantyPeriod().ToString();

            if (maintenanceContract.GetWarrantyPeriodTimeUnit() != "")
                warrantyPeriodCombo.SelectedItem = maintenanceContract.GetWarrantyPeriodTimeUnit();
            else
                warrantyPeriodCombo.SelectedIndex = warrantyPeriodCombo.Items.Count - 1;

            if (maintenanceContract.GetMaintContractWarrantyPeriodCondition() != "")
                warrantyPeriodFromWhenCombo.SelectedItem = maintenanceContract.GetMaintContractWarrantyPeriodCondition();
            else
                warrantyPeriodFromWhenCombo.SelectedIndex = warrantyPeriodFromWhenCombo.Items.Count - 1;

        }
        private void SetAdditionalDescriptionValue()
        {
            additionalDescriptionTextBox.Text = maintenanceContract.GetMaintContractNotes();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////



        private void WarrantyPeriodComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (warrantyPeriodCombo.SelectedItem != null)
                maintenanceContract.SetWarrantyPeriodTimeUnit(timeUnits[warrantyPeriodCombo.SelectedIndex].timeUnitId, timeUnits[warrantyPeriodCombo.SelectedIndex].timeUnit);
        }

        private void WarrantyPeriodFromWhenComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (warrantyPeriodFromWhenCombo.SelectedIndex != -1)
            {
                maintenanceContract.SetMaintContractWarrantyPeriodCondition(conditionStartDates[warrantyPeriodFromWhenCombo.SelectedIndex].key, conditionStartDates[warrantyPeriodFromWhenCombo.SelectedIndex].value);
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
                maintenanceContract.SetWarrantyPeriod(warrantyPeriod);
            }
            else
            {
                warrantyPeriod = 0;
                warrantyPeriodTextBox.Text = null;
            }
        }

        private void AdditionalDescriptionTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            maintenanceContract.SetMaintContractNotes(additionalDescriptionTextBox.Text);
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
            maintContractsBasicInfoPage.maintContractsProductsPage = maintContractsProductsPage;
            maintContractsBasicInfoPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
            maintContractsBasicInfoPage.maintContractsAdditionalInfoPage = this;
            maintContractsBasicInfoPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;

            NavigationService.Navigate(maintContractsBasicInfoPage);
        }
        private void OnClickProjectInfo(object sender, MouseButtonEventArgs e)
        {
            maintContractsProjectsPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
            maintContractsProjectsPage.maintContractsProductsPage = maintContractsProductsPage;
            maintContractsProjectsPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
            maintContractsProjectsPage.maintContractsAdditionalInfoPage = this;
            maintContractsProjectsPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;

            NavigationService.Navigate(maintContractsProjectsPage);
        }
        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            maintContractsProductsPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
            maintContractsProductsPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
            maintContractsProductsPage.maintContractsAdditionalInfoPage = this;
            maintContractsProductsPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;

            NavigationService.Navigate(maintContractsProductsPage);
        }
        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {
            maintContractsPaymentAndDeliveryPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
            maintContractsPaymentAndDeliveryPage.maintContractsProductsPage = maintContractsProductsPage;
            maintContractsPaymentAndDeliveryPage.maintContractsAdditionalInfoPage = this;
            maintContractsPaymentAndDeliveryPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;

            NavigationService.Navigate(maintContractsPaymentAndDeliveryPage);
        }
        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {

        }
        private void OnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                maintContractsUploadFilesPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
                maintContractsUploadFilesPage.maintContractsProductsPage = maintContractsProductsPage;
                maintContractsUploadFilesPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
                maintContractsUploadFilesPage.maintContractsAdditionalInfoPage = this;

                NavigationService.Navigate(maintContractsUploadFilesPage);
            }
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///BUTTON CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnClickNextButton(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                maintContractsUploadFilesPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
                maintContractsUploadFilesPage.maintContractsProductsPage = maintContractsProductsPage;
                maintContractsUploadFilesPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
                maintContractsUploadFilesPage.maintContractsAdditionalInfoPage = this;

                NavigationService.Navigate(maintContractsUploadFilesPage);
            }
        }

        private void OnClickBackButton(object sender, RoutedEventArgs e)
        {
            maintContractsPaymentAndDeliveryPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
            maintContractsPaymentAndDeliveryPage.maintContractsProductsPage = maintContractsProductsPage;
            maintContractsPaymentAndDeliveryPage.maintContractsAdditionalInfoPage = this;
            maintContractsPaymentAndDeliveryPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;

            NavigationService.Navigate(maintContractsPaymentAndDeliveryPage);
        }

        private void OnButtonClickAutomateMaintContract(object sender, RoutedEventArgs e)
        {
            // wordAutomation.AutomateWorkOffer(maintenanceContract);
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
            if (maintenanceContract.GetMaintContractProposerId() == 0)
                MessageBox.Show("You need to choose sales person before adding a work offer!");
            //else if (maintenanceContract.GetCompanyName() == null)
            //    MessageBox.Show("You need to choose a company before adding a work offer!");
            //else if (maintenanceContract.GetAddressSerial() == 0)
            //    MessageBox.Show("You need to choose company address before adding a work offer!");
            //else if (maintenanceContract.GetContactId() == 0)
            //    MessageBox.Show("You need to choose a contact before adding a work offer!");
            else if (maintenanceContract.GetMaintContractProduct1TypeId() != 0 && maintenanceContract.GetProduct1PriceValue() == 0)
                MessageBox.Show("You need to add a price for product 1 before adding a work offer!");
            else if (maintenanceContract.GetMaintContractProduct2TypeId() != 0 && maintenanceContract.GetProduct2PriceValue() == 0)
                MessageBox.Show("You need to add a price for product 2 before adding a work offer!");
            else if (maintenanceContract.GetMaintContractProduct3TypeId() != 0 && maintenanceContract.GetProduct3PriceValue() == 0)
                MessageBox.Show("You need to add a price for product 3 before adding a work offer!");
            else if (maintenanceContract.GetMaintContractProduct4TypeId() != 0 && maintenanceContract.GetProduct4PriceValue() == 0)
                MessageBox.Show("You need to add a price for product 4 before adding a work offer!");
            else if (maintenanceContract.GetMaintContractContractTypeId() == 0)
                MessageBox.Show("You need to set contract type before adding a work offer!");
            else
            {
                if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_ADD_CONDITION || viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_RESOLVE_CONDITION)
                {
                    if (!maintenanceContract.IssueNewMaintContract())
                        return;

                    //if (maintenanceContract.GetRFQID() != null)
                    //  if (!maintenanceContract.ConfirmRFQ())
                    //    return;
                    if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
                    {
                        viewAddCondition = COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION;

                        MaintenanceContractsWindow viewOffer = new MaintenanceContractsWindow(ref loggedInUser, ref maintenanceContract, viewAddCondition, true);

                        NavigationWindow currentWindow = (NavigationWindow)this.Parent;
                        currentWindow.Close();

                        viewOffer.Show();
                    }
                }
                else if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_REVISE_CONDITION)
                {
                    if (!maintenanceContract.ReviseMaintContract())
                        return;

                    if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
                    {
                        viewAddCondition = COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION;

                        MaintenanceContractsWindow viewOffer = new MaintenanceContractsWindow(ref loggedInUser, ref maintenanceContract, viewAddCondition, true);

                        NavigationWindow currentWindow = (NavigationWindow)this.Parent;
                        currentWindow.Close();

                        viewOffer.Show();
                    }
                }
            }
        }

        private void OnButtonClickAutomateMaintOffer(object sender, RoutedEventArgs e)
        {

        }
    }
}
