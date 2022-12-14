using _01electronics_library;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for MaintOffersPaymentAndDeliveryPage.xaml
    /// </summary>
    public partial class MaintOffersPaymentAndDeliveryPage : Page
    {
        Employee loggedInUser;
        MaintenanceOffer maintOffers;

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private IntegrityChecks IntegrityChecks = new IntegrityChecks();

        private List<BASIC_STRUCTS.CURRENCY_STRUCT> currencies = new List<BASIC_STRUCTS.CURRENCY_STRUCT>();
        private List<BASIC_STRUCTS.TIMEUNIT_STRUCT> timeUnits = new List<BASIC_STRUCTS.TIMEUNIT_STRUCT>();

        private List<BASIC_STRUCTS.VAT_CONDITION_STRUCT> vatConditions = new List<BASIC_STRUCTS.VAT_CONDITION_STRUCT>();
        private List<BASIC_STRUCTS.CONDITION_START_DATES_STRUCT> conditionStartDates = new List<BASIC_STRUCTS.CONDITION_START_DATES_STRUCT>();
        private List<BASIC_STRUCTS.SHIPMENT_TYPE_STRUCT> shipmentTypes = new List<BASIC_STRUCTS.SHIPMENT_TYPE_STRUCT>();
        private List<BASIC_STRUCTS.CONTRACT_STRUCT> contractTypes = new List<BASIC_STRUCTS.CONTRACT_STRUCT>();

        private int viewAddCondition;

        private Decimal totalPrice;

        public MaintOffersBasicInfoPage maintOffersBasicInfoPage;
        public MaintOffersProductsPage maintOffersProductsPage;
        public MaintOffersAdditionalInfoPage maintOffersAdditionalInfoPage;
        public MaintOffersUploadFilesPage maintOffersUploadFilesPage;

        public MaintOffersPaymentAndDeliveryPage(ref Employee mLoggedInUser, ref MaintenanceOffer mMaintOffer, int mViewAddCondition, ref MaintOffersAdditionalInfoPage mMaintOfferAdditionalInfoPage)
        {
            maintOffersAdditionalInfoPage = mMaintOfferAdditionalInfoPage;

            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            maintOffers = mMaintOffer;

            totalPrice = 0;

            InitializeComponent();

            if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_ADD_CONDITION)
            {
                InitializeTotalPriceCurrencyComboBox();
                InitializeTotalPriceVATCombo();

                DisableTotalPriceComboAndTextBox();
                SetTotalPriceCurrencyComboBox();
                SetTotalPriceTextBox();
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
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
            else if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_REVISE_CONDITION)
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
        public MaintOffersPaymentAndDeliveryPage(ref MaintenanceOffer mMaintOffer)
        {
            maintOffers = mMaintOffer;
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

        public void SetFrequenciesValue()
        {
            paymentsFrequencyTextBox.Text = maintOffers.GetPaymentsFrequency().ToString();
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
            totalPriceCombo.SelectedItem = maintOffers.GetCurrency();
        }

        public void SetTotalPriceTextBox()
        {
            totalPrice = 0;

            for (int i = 1; i <= COMPANY_WORK_MACROS.MAX_OUTGOING_QUOTATION_PRODUCTS; i++)
                totalPrice += maintOffers.GetProductPriceValue(i) * maintOffers.GetMaintOfferProductQuantity(i);

            totalPriceTextBox.Text = totalPrice.ToString();
            maintOffers.SetTotalValues();
        }

        private void SetPriceValues()
        {
            if (maintOffers.GetCurrency() != null)
            {
                totalPriceCombo.Text = maintOffers.GetCurrency().ToString();
                totalPriceTextBox.Text = maintOffers.GetTotalPriceValue().ToString();

                totalPrice = maintOffers.GetTotalPriceValue();
            }
        }
        private void SetConditionsCheckBoxes()
        {
            if (maintOffers.GetMaintOfferVATConditionID())
                VatConditionCheckBox.IsChecked = true;
            else
                VatConditionCheckBox.IsChecked = false;

            if (maintOffers.GetMaintOfferSparePartsConditionID())
                SparePartsCheckBox.IsChecked = true;
            else
                SparePartsCheckBox.IsChecked = false;

            if (maintOffers.GetMaintOfferBatteriesConditionID())
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
                maintOffers.SetPaymentsFrequency(Int32.Parse(paymentsFrequencyTextBox.Text));
            else
            {
                maintOffers.SetPaymentsFrequency(0);
                paymentsFrequencyTextBox.Text = null;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            maintOffersBasicInfoPage.maintOffersProductsPage = maintOffersProductsPage;
            maintOffersBasicInfoPage.maintOffersPaymentAndDeliveryPage = this;
            maintOffersBasicInfoPage.maintOffersAdditionalInfoPage = maintOffersAdditionalInfoPage;
            maintOffersBasicInfoPage.maintOffersUploadFilesPage = maintOffersUploadFilesPage;

            NavigationService.Navigate(maintOffersBasicInfoPage);
        }
        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            maintOffersProductsPage.maintOffersBasicInfoPage = maintOffersBasicInfoPage;
            maintOffersProductsPage.maintOffersPaymentAndDeliveryPage = this;
            maintOffersProductsPage.maintOffersAdditionalInfoPage = maintOffersAdditionalInfoPage;
            maintOffersProductsPage.maintOffersUploadFilesPage = maintOffersUploadFilesPage;

            NavigationService.Navigate(maintOffersProductsPage);


        }
        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {

        }
        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            maintOffersAdditionalInfoPage.maintOffersBasicInfoPage = maintOffersBasicInfoPage;
            maintOffersAdditionalInfoPage.maintOffersProductsPage = maintOffersProductsPage;
            maintOffersAdditionalInfoPage.maintOffersPaymentAndDeliveryPage = this;
            maintOffersAdditionalInfoPage.maintOffersUploadFilesPage = maintOffersUploadFilesPage;

            NavigationService.Navigate(maintOffersAdditionalInfoPage);

        }
        private void OnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                maintOffersUploadFilesPage.maintOffersBasicInfoPage = maintOffersBasicInfoPage;
                maintOffersUploadFilesPage.maintOffersProductsPage = maintOffersProductsPage;
                maintOffersUploadFilesPage.maintOffersPaymentAndDeliveryPage = this;
                maintOffersUploadFilesPage.maintOffersAdditionalInfoPage = maintOffersAdditionalInfoPage;

                NavigationService.Navigate(maintOffersUploadFilesPage);
            }
        }

        private void OnClickNextButton(object sender, RoutedEventArgs e)
        {
            maintOffersAdditionalInfoPage.maintOffersBasicInfoPage = maintOffersBasicInfoPage;
            maintOffersAdditionalInfoPage.maintOffersProductsPage = maintOffersProductsPage;
            maintOffersAdditionalInfoPage.maintOffersPaymentAndDeliveryPage = this;
            maintOffersAdditionalInfoPage.maintOffersUploadFilesPage = maintOffersUploadFilesPage;

            NavigationService.Navigate(maintOffersAdditionalInfoPage);
        }

        private void OnClickBackButton(object sender, RoutedEventArgs e)
        {
            maintOffersProductsPage.maintOffersBasicInfoPage = maintOffersBasicInfoPage;
            maintOffersProductsPage.maintOffersPaymentAndDeliveryPage = this;
            maintOffersProductsPage.maintOffersAdditionalInfoPage = maintOffersAdditionalInfoPage;
            maintOffersProductsPage.maintOffersUploadFilesPage = maintOffersUploadFilesPage;

            NavigationService.Navigate(maintOffersProductsPage);
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
            maintOffers.SetMaintOfferSparePartsCondition(true);
        }
        private void OnCheckVatCheckBox(object sender, RoutedEventArgs e)
        {
            maintOffers.SetMaintOfferVATCondition(true);
        }
        private void OnCheckBatteriesCheckBox(object sender, RoutedEventArgs e)
        {
            maintOffers.SetMaintOfferBatteriesCondition(true);
        }

        private void OnUnCheckSparePartsCheckBox(object sender, RoutedEventArgs e)
        {
            maintOffers.SetMaintOfferSparePartsCondition(false);
        }
        private void OnUnCheckVatCheckBox(object sender, RoutedEventArgs e)
        {
            maintOffers.SetMaintOfferVATCondition(false);
        }
        private void OnUnCheckBatteriesCheckBox(object sender, RoutedEventArgs e)
        {
            maintOffers.SetMaintOfferBatteriesCondition(false);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///CHECK/UNCHECK HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

    }
}
