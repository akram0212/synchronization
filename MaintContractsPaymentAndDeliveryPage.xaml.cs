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

            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_ADD_CONDITION)
            {
                InitializeTotalPriceCurrencyComboBox();
                InitializeTotalPriceVATCombo();

                DisableTotalPriceComboAndTextBox();
                SetTotalPriceCurrencyComboBox();
                SetTotalPriceTextBox();
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                InitializeTotalPriceCurrencyComboBox();
                InitializeTotalPriceVATCombo();

                ConfigureViewUIElements();
                SetPriceValues();
                SetConditionsCheckBoxes();
                SetFrequenciesValue();
                DisableFrequenciesTextBoxes();
                DisableCheckBoxes();

                cancelButton.IsEnabled = false;
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION)
            {
                InitializeTotalPriceCurrencyComboBox();
                InitializeTotalPriceVATCombo();

                SetPriceValues();
                SetConditionsCheckBoxes();
                SetFrequenciesValue();
                DisableTotalPriceComboAndTextBox();
            }
            else
            {
                InitializeTotalPriceCurrencyComboBox();
                InitializeTotalPriceVATCombo();

                DisableTotalPriceComboAndTextBox();
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

        private void SetFrequenciesValue()
        {
            paymentsFrequencyTextBox.Text = maintContracts.GetMaintContractPaymentsFrequency().ToString();
        }
        public void SetFrequenciesValueFromOffer()
        {
            paymentsFrequencyTextBox.Text = maintContracts.GetPaymentsFrequency().ToString();
        }
        public void DisableFrequenciesTextBoxes()
        {
            paymentsFrequencyTextBox.IsEnabled = false;
        }
        public void DisableCheckBoxes()
        {
            VatConditionCheckBox.IsEnabled = false;
            SparePartsCheckBox.IsEnabled = false;
            BatteriesCheckBox.IsEnabled = false;
        }
        public void SetTotalPriceCurrencyComboBox()
        {
            totalPriceCombo.SelectedItem = maintContracts.GetMaintContractCurrency();
        }

        public void SetTotalPriceCurrencyComboBoxFromOffer()
        {
            totalPriceCombo.SelectedItem = maintContracts.GetCurrency();
        }

        public void SetTotalPriceTextBox()
        {
            totalPrice = 0;

            for (int i = 1; i <= COMPANY_WORK_MACROS.MAX_OUTGOING_QUOTATION_PRODUCTS; i++)
                totalPrice += maintContracts.GetMaintContractProductPriceValue(i) * maintContracts.GetMaintContractProductQuantity(i);

            totalPriceTextBox.Text = totalPrice.ToString();
            maintContracts.SetTotalValues();
        }

        public void SetTotalPriceTextBoxFromOffer()
        {
            totalPrice = 0;

            for (int i = 1; i <= COMPANY_WORK_MACROS.MAX_OUTGOING_QUOTATION_PRODUCTS; i++)
                totalPrice += maintContracts.GetProductPriceValue(i) * maintContracts.GetMaintOfferProductQuantity(i);

            totalPriceTextBox.Text = totalPrice.ToString();
            maintContracts.SetTotalValues();
        }

        public void SetPriceValues()
        {
            if (maintContracts.GetCurrency() != null)
            {
                totalPriceCombo.Text = maintContracts.GetMaintContractCurrency().ToString();
                totalPriceTextBox.Text = maintContracts.GetMaintContractTotalPriceValue().ToString();

                totalPrice = maintContracts.GetMaintContractTotalPriceValue();
            }
        }
        private void SetConditionsCheckBoxes()
        {
            if (maintContracts.GetMaintContractVATConditionID())
                VatConditionCheckBox.IsChecked = true;
            else
                VatConditionCheckBox.IsChecked = false;

            if (maintContracts.GetMaintContractSparePartsConditionID())
                SparePartsCheckBox.IsChecked = true;
            else
                SparePartsCheckBox.IsChecked = false;

            if (maintContracts.GetMaintContractBatteriesConditionID())
                BatteriesCheckBox.IsChecked = true;
            else
                BatteriesCheckBox.IsChecked = false;
        }

        public void SetConditionsCheckBoxesFromOffer()
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

        private void OnTextChangedPaymentsFrequencyTextBox(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(paymentsFrequencyTextBox.Text, BASIC_MACROS.PHONE_STRING) && paymentsFrequencyTextBox.Text != "")
                maintContracts.SetMaintContractPaymentsFrequency(Int32.Parse(paymentsFrequencyTextBox.Text));
            else
            {
                maintContracts.SetMaintContractPaymentsFrequency(0);
                paymentsFrequencyTextBox.Text = null;
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
        ///CHECK/UNCHECK HANDLERS
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
            maintContracts.SetMaintContractSparePartsCondition(true);
        }
        private void OnCheckVatCheckBox(object sender, RoutedEventArgs e)
        {
            maintContracts.SetMaintContractVATCondition(true);
        }
        private void OnCheckBatteriesCheckBox(object sender, RoutedEventArgs e)
        {
            maintContracts.SetMaintContractBatteriesCondition(true);
        }

        private void OnUnCheckSparePartsCheckBox(object sender, RoutedEventArgs e)
        {
            maintContracts.SetMaintContractSparePartsCondition(false);
        }
        private void OnUnCheckVatCheckBox(object sender, RoutedEventArgs e)
        {
            maintContracts.SetMaintContractVATCondition(false);
        }
        private void OnUnCheckBatteriesCheckBox(object sender, RoutedEventArgs e)
        {
            maintContracts.SetMaintContractBatteriesCondition(false);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///CHECK/UNCHECK HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

    }
}
    