﻿using _01electronics_library;
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
       
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> vatConditions = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> conditionStartDates = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> shipmentTypes = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
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
        public MaintOffersPaymentAndDeliveryPage(ref MaintenanceOffer mMaintOffer)
        {
            maintOffers = mMaintOffer;
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
            contractTypeComboBox.Text = maintOffers.GetMaintOfferContractType();
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

        private void OnTextChangedVisitsFrequencyTextBox(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(visitsFrequencyTextBox.Text, BASIC_MACROS.PHONE_STRING) && visitsFrequencyTextBox.Text != "")
                maintOffers.SetVisitsFrequency(Int32.Parse(visitsFrequencyTextBox.Text));
            else
            {
                maintOffers.SetVisitsFrequency(0);
                visitsFrequencyTextBox.Text = null;
            }
        }
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

        private void OnTextChangedEmergenciesFrequencyTextBox(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(emergenciesFrequencyTextBox.Text, BASIC_MACROS.PHONE_STRING) && emergenciesFrequencyTextBox.Text != "")
                maintOffers.SetEmergenciesFrequency(Int32.Parse(emergenciesFrequencyTextBox.Text));
            else
            {
                maintOffers.SetEmergenciesFrequency(0);
                emergenciesFrequencyTextBox.Text = null;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ContractTypeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            maintOffers.SetMaintOfferContractType(contractTypes[contractTypeComboBox.SelectedIndex].contractId, contractTypes[contractTypeComboBox.SelectedIndex].contractName);

            if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                maintOffersAdditionalInfoPage.warrantyPeriodTextBox.IsEnabled = false;
                maintOffersAdditionalInfoPage.warrantyPeriodCombo.IsEnabled = false;
                maintOffersAdditionalInfoPage.warrantyPeriodFromWhenCombo.IsEnabled = false;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                maintOffers.SetPaymentsFrequency(Int32.Parse(paymentsFrequencyTextBox.Text.ToString()));
                maintOffers.SetVisitsFrequency(Int32.Parse(visitsFrequencyTextBox.Text.ToString()));
                maintOffers.SetEmergenciesFrequency(Int32.Parse(emergenciesFrequencyTextBox.Text.ToString()));
            }
            maintOffersBasicInfoPage.maintOffersProductsPage = maintOffersProductsPage;
            maintOffersBasicInfoPage.maintOffersPaymentAndDeliveryPage = this;
            maintOffersBasicInfoPage.maintOffersAdditionalInfoPage = maintOffersAdditionalInfoPage;
            maintOffersBasicInfoPage.maintOffersUploadFilesPage = maintOffersUploadFilesPage;

            NavigationService.Navigate(maintOffersBasicInfoPage);
        }
        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                maintOffers.SetPaymentsFrequency(Int32.Parse(paymentsFrequencyTextBox.Text.ToString()));
                maintOffers.SetVisitsFrequency(Int32.Parse(visitsFrequencyTextBox.Text.ToString()));
                maintOffers.SetEmergenciesFrequency(Int32.Parse(emergenciesFrequencyTextBox.Text.ToString()));
            }
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
            if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                maintOffers.SetPaymentsFrequency(Int32.Parse(paymentsFrequencyTextBox.Text.ToString()));
                maintOffers.SetVisitsFrequency(Int32.Parse(visitsFrequencyTextBox.Text.ToString()));
                maintOffers.SetEmergenciesFrequency(Int32.Parse(emergenciesFrequencyTextBox.Text.ToString()));
            }

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
            if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                maintOffers.SetPaymentsFrequency(Int32.Parse(paymentsFrequencyTextBox.Text.ToString()));
                maintOffers.SetVisitsFrequency(Int32.Parse(visitsFrequencyTextBox.Text.ToString()));
                maintOffers.SetEmergenciesFrequency(Int32.Parse(emergenciesFrequencyTextBox.Text.ToString()));
            }
            maintOffersAdditionalInfoPage.maintOffersBasicInfoPage = maintOffersBasicInfoPage;
            maintOffersAdditionalInfoPage.maintOffersProductsPage = maintOffersProductsPage;
            maintOffersAdditionalInfoPage.maintOffersPaymentAndDeliveryPage = this;
            maintOffersAdditionalInfoPage.maintOffersUploadFilesPage = maintOffersUploadFilesPage;

            NavigationService.Navigate(maintOffersAdditionalInfoPage);
        }

        private void OnClickBackButton(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                maintOffers.SetPaymentsFrequency(Int32.Parse(paymentsFrequencyTextBox.Text.ToString()));
                maintOffers.SetVisitsFrequency(Int32.Parse(visitsFrequencyTextBox.Text.ToString()));
                maintOffers.SetEmergenciesFrequency(Int32.Parse(emergenciesFrequencyTextBox.Text.ToString()));
            }
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

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///CHECK/UNCHECK HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        
    }
}