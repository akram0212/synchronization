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
    /// Interaction logic for MaintContractsPaymentAndDeliveryPage.xaml
    /// </summary>
    public partial class MaintContractsPaymentAndDeliveryPage : Page
    {
        Employee loggedInUser;
        MaintenanceContract maintContracts;

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private IntegrityChecks IntegrityChecks = new IntegrityChecks();

        private List<BASIC_STRUCTS.CURRENCY_STRUCT> currencies = new List<BASIC_STRUCTS.CURRENCY_STRUCT>();
        private List<BASIC_STRUCTS.TIMEUNIT_STRUCT> timeUnits = new List<BASIC_STRUCTS.TIMEUNIT_STRUCT>();

        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> vatConditions = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> conditionStartDates = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> shipmentTypes = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
        private List<BASIC_STRUCTS.CONTRACT_STRUCT> contractTypes = new List<BASIC_STRUCTS.CONTRACT_STRUCT>();

        private int viewAddCondition;

        private Decimal totalPrice;

        public MaintContractsBasicInfoPage maintContractsBasicInfoPage;
        public MaintContractsProductsPage maintContractsProductsPage;
        public MaintContractsProjectsPage maintContractsProjectsPage;
        public MaintContractsAdditionalInfoPage maintContractsAdditionalInfoPage;
        public MaintContractsUploadFilesPage maintContractsUploadFilesPage;

        public MaintContractsPaymentAndDeliveryPage(ref Employee mLoggedInUser, ref MaintenanceContract mMaintOffer, int mViewAddCondition, ref MaintContractsAdditionalInfoPage mMaintOfferAdditionalInfoPage)
        {
            maintContractsAdditionalInfoPage = mMaintOfferAdditionalInfoPage;

            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            maintContracts = mMaintOffer;

            totalPrice = 0;

            InitializeComponent();

            if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_ADD_CONDITION)
            {
                InitializeTotalPriceCurrencyComboBox();
                InitializeTotalPriceVATCombo();
                InitializeContractType();

                DisableTotalPriceComboAndTextBox();
                SetTotalPriceCurrencyComboBox();
                SetTotalPriceTextBox();
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                InitializeTotalPriceCurrencyComboBox();
                InitializeTotalPriceVATCombo();
                InitializeContractType();

                ConfigureViewUIElements();
                SetContractTypeValue();
                SetPriceValues();
                SetConditionsCheckBoxes();
                SetFrequenciesValue();
                DisableFrequenciesTextBoxes();
                DisableCheckBoxes();

                cancelButton.IsEnabled = false;
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_REVISE_CONDITION)
            {
                InitializeTotalPriceCurrencyComboBox();
                InitializeTotalPriceVATCombo();
                InitializeContractType();

                SetContractTypeValue();
                SetPriceValues();
                SetConditionsCheckBoxes();
                SetFrequenciesValue();
                DisableTotalPriceComboAndTextBox();
            }
            else
            {
                InitializeContractType();
                InitializeTotalPriceCurrencyComboBox();
                InitializeTotalPriceVATCombo();

                DisableTotalPriceComboAndTextBox();
                SetContractTypeValue();
                SetTotalPriceCurrencyComboBox();
                SetTotalPriceTextBox();
            }
        }
        public MaintContractsPaymentAndDeliveryPage(ref MaintenanceContract mMaintOffer)
        {
            maintContracts = mMaintOffer;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        ///CONFIGURE UI ELEMENTS
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        private void ConfigureViewUIElements()
        {
            contractTypeComboBox.IsEnabled = false;
            totalPriceCombo.IsEnabled = false;
            totalPriceTextBox.IsEnabled = false;
        }

        private void DisableTotalPriceComboAndTextBox()
        {
            totalPriceCombo.IsEnabled = false;
            totalPriceTextBox.IsEnabled = false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        ///INITIALIZATION FUNCTIONS
        ///////////////////////////////////////////////////////////////////////////////////////////////////

        private bool InitializeContractType()
        {
            if (!commonQueriesObject.GetContractTypes(ref contractTypes))
                return false;
            for (int i = 0; i < contractTypes.Count; i++)
                contractTypeComboBox.Items.Add(contractTypes[i].contractName);

            return true;
        }

        private void InitializeTotalPriceCurrencyComboBox()
        {
            commonQueriesObject.GetCurrencyTypes(ref currencies);
            for (int i = 0; i < currencies.Count; i++)
                totalPriceCombo.Items.Add(currencies[i].currencyName);

        }

        private bool InitializeTotalPriceVATCombo()
        {
            if (!commonQueriesObject.GetVATConditions(ref vatConditions))
                return false;

            return true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        ///SET FUNCTIONS
        ///////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetContractTypeValue()
        {
            contractTypeComboBox.Text = maintContracts.GetMaintOfferContractType();
        }
        public void SetFrequenciesValue()
        {
            paymentsFrequencyTextBox.Text = maintContracts.GetPaymentsFrequency().ToString();
            visitsFrequencyTextBox.Text = maintContracts.GetVisitsFrequency().ToString();
            emergenciesFrequencyTextBox.Text = maintContracts.GetEmergenciesFrequency().ToString();
        }
        public void DisableFrequenciesTextBoxes()
        {
            paymentsFrequencyTextBox.IsEnabled = false;
            visitsFrequencyTextBox.IsEnabled = false;
            emergenciesFrequencyTextBox.IsEnabled = false;
        }
        public void DisableCheckBoxes()
        {
            VatConditionCheckBox.IsEnabled = false;
            SparePartsCheckBox.IsEnabled = false;
            BatteriesCheckBox.IsEnabled = false;
        }
        public void SetTotalPriceCurrencyComboBox()
        {
            totalPriceCombo.SelectedItem = maintContracts.GetCurrency();
        }

        public void SetTotalPriceTextBox()
        {
            totalPrice = 0;

            for (int i = 1; i <= COMPANY_WORK_MACROS.MAX_OUTGOING_QUOTATION_PRODUCTS; i++)
                totalPrice += maintContracts.GetProductPriceValue(i) * maintContracts.GetMaintOfferProductQuantity(i);

            totalPriceTextBox.Text = totalPrice.ToString();
            maintContracts.SetTotalValues();
        }

        private void SetPriceValues()
        {
            if (maintContracts.GetCurrency() != null)
            {
                totalPriceCombo.Text = maintContracts.GetCurrency().ToString();
                totalPriceTextBox.Text = maintContracts.GetTotalPriceValue().ToString();

                totalPrice = maintContracts.GetTotalPriceValue();
            }
        }
        private void SetConditionsCheckBoxes()
        {
            if (maintContracts.GetMaintOfferVATConditionID())
                VatConditionCheckBox.IsChecked = true;
            else
                VatConditionCheckBox.IsChecked = false;

            if (maintContracts.GetMaintOfferSparePartsConditionID())
                SparePartsCheckBox.IsChecked = true;
            else
                SparePartsCheckBox.IsChecked = false;

            if (maintContracts.GetMaintOfferBatteriesConditionID())
                BatteriesCheckBox.IsChecked = true;
            else
                BatteriesCheckBox.IsChecked = false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///TEXT CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnTextChangedVisitsFrequencyTextBox(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(visitsFrequencyTextBox.Text, BASIC_MACROS.PHONE_STRING) && visitsFrequencyTextBox.Text != "")
                maintContracts.SetVisitsFrequency(Int32.Parse(visitsFrequencyTextBox.Text));
            else
            {
                maintContracts.SetVisitsFrequency(0);
                visitsFrequencyTextBox.Text = null;
            }
        }
        private void OnTextChangedPaymentsFrequencyTextBox(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(paymentsFrequencyTextBox.Text, BASIC_MACROS.PHONE_STRING) && paymentsFrequencyTextBox.Text != "")
                maintContracts.SetPaymentsFrequency(Int32.Parse(paymentsFrequencyTextBox.Text));
            else
            {
                maintContracts.SetPaymentsFrequency(0);
                paymentsFrequencyTextBox.Text = null;
            }
        }
        private void OnTextChangedEmergenciesFrequencyTextBox(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(emergenciesFrequencyTextBox.Text, BASIC_MACROS.PHONE_STRING) && emergenciesFrequencyTextBox.Text != "")
                maintContracts.SetEmergenciesFrequency(Int32.Parse(emergenciesFrequencyTextBox.Text));
            else
            {
                maintContracts.SetEmergenciesFrequency(0);
                emergenciesFrequencyTextBox.Text = null;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ContractTypeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            maintContracts.SetMaintOfferContractType(contractTypes[contractTypeComboBox.SelectedIndex].contractId, contractTypes[contractTypeComboBox.SelectedIndex].contractName);

            if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                maintContractsAdditionalInfoPage.warrantyPeriodTextBox.IsEnabled = false;
                maintContractsAdditionalInfoPage.warrantyPeriodCombo.IsEnabled = false;
                maintContractsAdditionalInfoPage.warrantyPeriodFromWhenCombo.IsEnabled = false;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            maintContractsBasicInfoPage.maintContractsProductsPage = maintContractsProductsPage;
            maintContractsBasicInfoPage.maintContractsPaymentAndDeliveryPage = this;
            maintContractsBasicInfoPage.maintContractsProjectInfoPage = maintContractsProjectsPage;
            maintContractsBasicInfoPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
            maintContractsBasicInfoPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;

            NavigationService.Navigate(maintContractsBasicInfoPage);
        }
        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            maintContractsProductsPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
            maintContractsProductsPage.maintContractsProjectsPage = maintContractsProjectsPage;
            maintContractsProductsPage.maintContractsPaymentAndDeliveryPage = this;
            maintContractsProductsPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
            maintContractsProductsPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;

            NavigationService.Navigate(maintContractsProductsPage);


        }
        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {

        }
        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            maintContractsAdditionalInfoPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
            maintContractsAdditionalInfoPage.maintContractsProductsPage = maintContractsProductsPage;
            maintContractsAdditionalInfoPage.maintContractsProjectsPage = maintContractsProjectsPage;
            maintContractsAdditionalInfoPage.maintContractsPaymentAndDeliveryPage = this;
            maintContractsAdditionalInfoPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;

            NavigationService.Navigate(maintContractsAdditionalInfoPage);

        }
        private void OnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                maintContractsUploadFilesPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
                maintContractsUploadFilesPage.maintContractsProjectsPage = maintContractsProjectsPage;
                maintContractsUploadFilesPage.maintContractsProductsPage = maintContractsProductsPage;
                maintContractsUploadFilesPage.maintContractsPaymentAndDeliveryPage = this;
                maintContractsUploadFilesPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;

                NavigationService.Navigate(maintContractsUploadFilesPage);
            }
        }

        private void OnClickNextButton(object sender, RoutedEventArgs e)
        {
            maintContractsAdditionalInfoPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
            maintContractsAdditionalInfoPage.maintContractsProjectsPage = maintContractsProjectsPage;
            maintContractsAdditionalInfoPage.maintContractsProductsPage = maintContractsProductsPage;
            maintContractsAdditionalInfoPage.maintContractsPaymentAndDeliveryPage = this;
            maintContractsAdditionalInfoPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;

            NavigationService.Navigate(maintContractsAdditionalInfoPage);
        }
        private void OnClickProjectInfo(object sender, MouseButtonEventArgs e)
        {
           
            maintContractsProjectsPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
            maintContractsProjectsPage.maintContractsProductsPage = maintContractsProductsPage;
            maintContractsProjectsPage.maintContractsPaymentAndDeliveryPage = this;
            maintContractsProjectsPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
            maintContractsProjectsPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;

            NavigationService.Navigate(maintContractsProjectsPage);
        }

        private void OnClickBackButton(object sender, RoutedEventArgs e)
        {
            maintContractsProductsPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
            maintContractsProductsPage.maintContractsProjectsPage = maintContractsProjectsPage;
            maintContractsProductsPage.maintContractsPaymentAndDeliveryPage = this;
            maintContractsProductsPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
            maintContractsProductsPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;

            NavigationService.Navigate(maintContractsProductsPage);
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

        private void OnCheckSparePartsCheckBox(object sender, RoutedEventArgs e)
        {
            maintContracts.SetMaintOfferSparePartsCondition(true);
        }
        private void OnCheckVatCheckBox(object sender, RoutedEventArgs e)
        {
            maintContracts.SetMaintOfferVATCondition(true);
        }
        private void OnCheckBatteriesCheckBox(object sender, RoutedEventArgs e)
        {
            maintContracts.SetMaintOfferBatteriesCondition(true);
        }

        private void OnUnCheckSparePartsCheckBox(object sender, RoutedEventArgs e)
        {
            maintContracts.SetMaintOfferSparePartsCondition(false);
        }
        private void OnUnCheckVatCheckBox(object sender, RoutedEventArgs e)
        {
            maintContracts.SetMaintOfferVATCondition(false);
        }
        private void OnUnCheckBatteriesCheckBox(object sender, RoutedEventArgs e)
        {
            maintContracts.SetMaintOfferBatteriesCondition(false);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///CHECK/UNCHECK HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

    }
}
