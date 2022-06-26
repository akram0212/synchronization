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
using _01electronics_library;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for WorkOfferPaymentAndDeliveryPage.xaml
    /// </summary>
    public partial class WorkOfferPaymentAndDeliveryPage : Page
    {
        Employee loggedInUser;
        Quotation quotation;

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private IntegrityChecks IntegrityChecks = new IntegrityChecks();

        private List<BASIC_STRUCTS.CURRENCY_STRUCT> currencies = new List<BASIC_STRUCTS.CURRENCY_STRUCT>();
        private List<BASIC_STRUCTS.TIMEUNIT_STRUCT> timeUnits = new List<BASIC_STRUCTS.TIMEUNIT_STRUCT>();
        private List<BASIC_STRUCTS.DELIVERY_POINT_STRUCT> deliveryPoints = new List<BASIC_STRUCTS.DELIVERY_POINT_STRUCT>();

        private List<BASIC_STRUCTS.VAT_CONDITION_STRUCT> vatConditions = new List<BASIC_STRUCTS.VAT_CONDITION_STRUCT>();
        private List<BASIC_STRUCTS.CONDITION_START_DATES_STRUCT> conditionStartDates = new List<BASIC_STRUCTS.CONDITION_START_DATES_STRUCT>();
        private List<BASIC_STRUCTS.SHIPMENT_TYPE_STRUCT> shipmentTypes = new List<BASIC_STRUCTS.SHIPMENT_TYPE_STRUCT>();
        private List<BASIC_STRUCTS.CONTRACT_STRUCT> contractTypes = new List<BASIC_STRUCTS.CONTRACT_STRUCT>();

        private int viewAddCondition;
        private int downPaymentPercentage;
        private int onDeliveryPercentage;
        private int onInstallationPercentage;

        private Decimal totalPrice;

        public WorkOfferBasicInfoPage workOfferBasicInfoPage;
        public WorkOfferProductsPage workOfferProductsPage;
        public WorkOfferAdditionalInfoPage workOfferAdditionalInfoPage;
        public WorkOfferUploadFilesPage workOfferUploadFilesPage;

        public WorkOfferPaymentAndDeliveryPage(ref Employee mLoggedInUser, ref Quotation mWorkOffer, int mViewAddCondition, ref WorkOfferAdditionalInfoPage mWorkOfferAdditionalInfoPage)
        {
            workOfferAdditionalInfoPage = mWorkOfferAdditionalInfoPage;

            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;
            
            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();
            
            quotation = mWorkOffer;

            totalPrice = 0;

            InitializeComponent();

            if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_ADD_CONDITION)
            {
                InitializeTotalPriceCurrencyComboBox();
                InitializeDeliveryTimeComboBox();
                InitializeDeliveryPointComboBox();
                InitializeTotalPriceVATCombo();
                InitializeDeliveryTimeFromWhenCombo();
                InitializeContractType();

                DisableTotalPriceComboAndTextBox();
                SetTotalPriceCurrencyComboBox();
                SetTotalPriceTextBox();
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                InitializeTotalPriceCurrencyComboBox();
                InitializeDeliveryTimeComboBox();
                InitializeDeliveryPointComboBox();
                InitializeTotalPriceVATCombo();
                InitializeDeliveryTimeFromWhenCombo();
                InitializeContractType();

                ConfigureViewUIElements();
                SetContractTypeValue();
                SetPriceValues();
                SetDownPaymentValues();
                SetOnDeliveryValues();
                SetOnInstallationValues();
                SetDeliveryTimeValues();
                SetDeliveryPointValue();

                cancelButton.IsEnabled = false;
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_REVISE_CONDITION)
            {
                InitializeTotalPriceCurrencyComboBox();
                InitializeDeliveryTimeComboBox();
                InitializeDeliveryPointComboBox();
                InitializeTotalPriceVATCombo();
                InitializeDeliveryTimeFromWhenCombo();
                InitializeContractType();

                SetContractTypeValue();
                SetPriceValues();
                SetDownPaymentValues();
                SetOnDeliveryValues();
                SetOnInstallationValues();
                SetDeliveryTimeValues();
                SetDeliveryPointValue();
                DisableTotalPriceComboAndTextBox();
            }
            else
            {
                InitializeContractType();
                InitializeTotalPriceCurrencyComboBox();
                InitializeDeliveryTimeComboBox();
                InitializeDeliveryPointComboBox();
                InitializeTotalPriceVATCombo();
                InitializeDeliveryTimeFromWhenCombo();

                DisableTotalPriceComboAndTextBox();
                SetContractTypeValue();
                SetTotalPriceCurrencyComboBox();
                SetTotalPriceTextBox();
            }
        }
        public WorkOfferPaymentAndDeliveryPage(ref Quotation mWorkOffer)
        {
            quotation = mWorkOffer;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        ///CONFIGURE UI ELEMENTS
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        private void ConfigureViewUIElements()
        {
            contractTypeComboBox.IsEnabled = false;
            totalPriceCombo.IsEnabled = false;
            totalPriceTextBox.IsEnabled = false;
            downPaymentPercentageTextBox.IsEnabled = false;
            downPaymentActualTextBox.IsEnabled = false;
            onDeliveryPercentageTextBox.IsEnabled = false;
            onDeliveryActualTextBox.IsEnabled = false;
            onInstallationPercentageTextBox.IsEnabled = false;
            onInstallationActualTextBox.IsEnabled = false;
            deliveryTimeCombo.IsEnabled = false;
            deliveryTimeTextBoxFrom.IsEnabled = false;
            deliveryTimeTextBoxTo.IsEnabled = false;
            deliveryPointCombo.IsEnabled = false;
            totalPriceVATCombo.IsEnabled = false;
            deliveryTimeFromWhenCombo.IsEnabled = false;
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
            contractTypes.Remove(contractTypes.Find(s1 => s1.contractId == 5));
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
        private bool InitializeDeliveryTimeComboBox()
        {
            if (!commonQueriesObject.GetTimeUnits(ref timeUnits))
                return false;
            for (int i = 0; i < timeUnits.Count(); i++)
                deliveryTimeCombo.Items.Add(timeUnits[i].timeUnit);
            return true;
        }

        private bool InitializeDeliveryPointComboBox()
        {
            if (!commonQueriesObject.GetDeliveryPoints(ref deliveryPoints))
                return false;

            for (int i = 0; i < deliveryPoints.Count; i++)
                deliveryPointCombo.Items.Add(deliveryPoints[i].pointName);
            return true;
        }

        private bool InitializeTotalPriceVATCombo()
        {
            if (!commonQueriesObject.GetVATConditions(ref vatConditions))
                return false;

            for (int i = 0; i < vatConditions.Count; i++)
                totalPriceVATCombo.Items.Add(vatConditions[i].condition_type);
            return true;
        }
        private bool InitializeDeliveryTimeFromWhenCombo()
        {
            if (!commonQueriesObject.GetConditionStartDates(ref conditionStartDates))
                return false;

            for (int i = 0; i < conditionStartDates.Count; i++)
                deliveryTimeFromWhenCombo.Items.Add(conditionStartDates[i].condition_type);
            return true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        ///SET FUNCTIONS
        ///////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetContractTypeValue()
        {
            contractTypeComboBox.Text = quotation.GetOfferContractType();
        }
        public void SetTotalPriceCurrencyComboBox()
        {
            totalPriceCombo.SelectedItem = quotation.GetCurrency();
        }

        public void SetTotalPriceTextBox()
        {
            totalPrice = 0;

            for(int i = 1; i <= COMPANY_WORK_MACROS.MAX_OUTGOING_QUOTATION_PRODUCTS; i++)
                totalPrice += quotation.GetProductPriceValue(i) * quotation.GetOfferProductQuantity(i);

            totalPriceTextBox.Text = totalPrice.ToString();
            quotation.SetTotalValues();
        }

        private void SetPercentAndValuesInWorkOffer()
        {
            quotation.SetPercentValues();
        }

        private void SetPriceValues()
        {
            if (quotation.GetCurrency() != null)
            {
                totalPriceCombo.Text = quotation.GetCurrency().ToString();
                totalPriceTextBox.Text = quotation.GetTotalPriceValue().ToString();

                if (quotation.GetOfferVATCondition() != "")
                    totalPriceVATCombo.SelectedItem = quotation.GetOfferVATCondition();
                else
                    totalPriceVATCombo.SelectedIndex = -1;

                totalPrice = quotation.GetTotalPriceValue();
            }
        }

        public void SetDownPaymentValues()
        {
            if (quotation.GetPercentDownPayment() != 0)
            {
                downPaymentPercentageTextBox.Text = quotation.GetPercentDownPayment().ToString();
                if (downPaymentPercentageTextBox.Text != "")
                {
                    downPaymentPercentage = int.Parse(downPaymentPercentageTextBox.Text);
                    downPaymentActualTextBox.Text = GetPercentage(downPaymentPercentage, totalPrice).ToString();
                }
            }

        }

        public void SetOnDeliveryValues()
        {
            if (quotation.GetPercentOnDelivery() != 0)
            {
                onDeliveryPercentageTextBox.Text = quotation.GetPercentOnDelivery().ToString();
                if (onDeliveryPercentageTextBox.Text != "")
                {
                    onDeliveryPercentage = int.Parse(onDeliveryPercentageTextBox.Text);
                    onDeliveryActualTextBox.Text = GetPercentage(onDeliveryPercentage, totalPrice).ToString();
                }
            }
        }

        public void SetOnInstallationValues()
        {
            if (quotation.GetPercentOnInstallation() != 0)
            {
                onInstallationPercentageTextBox.Text = quotation.GetPercentOnInstallation().ToString();
                if (onInstallationPercentageTextBox.Text != "")
                {
                    onDeliveryPercentage = int.Parse(onInstallationPercentageTextBox.Text.ToString());
                    onInstallationActualTextBox.Text = GetPercentage(onInstallationPercentage, totalPrice).ToString();
                }
            }
        }

        private void SetDeliveryTimeValues()
        {
            ////////////Added by me ama get awareeh
            if (quotation.GetDeliveryTimeMinimum() != 0)
            {
                deliveryTimeTextBoxFrom.Text = quotation.GetDeliveryTimeMinimum().ToString();
                deliveryTimeTextBoxTo.Text = quotation.GetDeliveryTimeMaximum().ToString();
            }

            if(quotation.GetDeliveryTimeUnit() != null)
                deliveryTimeCombo.Text = quotation.GetDeliveryTimeUnit().ToString();

            if (quotation.GetOfferDeliveryTimeCondition() != "")
                deliveryTimeFromWhenCombo.Text = quotation.GetOfferDeliveryTimeCondition();
            else
                deliveryTimeFromWhenCombo.SelectedIndex = deliveryTimeFromWhenCombo.Items.Count - 1;
        }

        private void SetDeliveryPointValue()
        {
            deliveryPointCombo.SelectedItem = quotation.GetDeliveryPoint();
        }

        public void SetDownPaymentPercentage()
        {
            downPaymentActualTextBox.Text = GetPercentage(downPaymentPercentage, totalPrice).ToString();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///GET FUNCTIONS
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        private double GetPercentage(Decimal percentage, Decimal total)
        {
            if (percentage != 0)
            {
                var value = (percentage / 100) * total;
                var percentage1 = Convert.ToInt32(Math.Round(value, 0));
                return percentage1;
            }
            else
                return 0;
            
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///TEXT CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnTextChangedDownPaymentPercentageTextBox(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(downPaymentPercentageTextBox.Text, BASIC_MACROS.PHONE_STRING) && downPaymentPercentageTextBox.Text != "")
                downPaymentPercentage = int.Parse(downPaymentPercentageTextBox.Text);
            else
            {
                downPaymentPercentage = 0;
                downPaymentPercentageTextBox.Text = null;
            }
            if ((onInstallationPercentage + onDeliveryPercentage + downPaymentPercentage) > 100)
            {
                downPaymentPercentage = 0;
                downPaymentActualTextBox.Text = "";
                MessageBox.Show("You can't exceed 100%");
            }
            else
            {
                downPaymentActualTextBox.Text = GetPercentage(downPaymentPercentage, totalPrice).ToString();
                quotation.SetPercentDownPayment(downPaymentPercentage);
            }
        }
        private void OnTextChangedDeliveryPercentageTextBox(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(onDeliveryPercentageTextBox.Text, BASIC_MACROS.PHONE_STRING) && onDeliveryPercentageTextBox.Text != "")
                onDeliveryPercentage = int.Parse(onDeliveryPercentageTextBox.Text);
            else
            {
                onDeliveryPercentage = 0;
                onDeliveryPercentageTextBox.Text = null;
            }
            if ((onInstallationPercentage + onDeliveryPercentage + downPaymentPercentage) > 100)
            {
                onDeliveryPercentage = 0;
                onDeliveryPercentageTextBox.Text = "";
                MessageBox.Show("You can't exceed 100%");
            }
            else
            {
                onDeliveryActualTextBox.Text = GetPercentage(onDeliveryPercentage, totalPrice).ToString();
                quotation.SetPercentOnDelivery(onDeliveryPercentage);
            }
        }
        private void OnTextChangedInstallationPercentageTextBox(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(onInstallationPercentageTextBox.Text, BASIC_MACROS.PHONE_STRING) && onInstallationPercentageTextBox.Text != "")
                onInstallationPercentage = int.Parse(onInstallationPercentageTextBox.Text);
            else
            {
                onInstallationPercentage = 0;
                onInstallationPercentageTextBox.Text = null;
            }
            if ((onInstallationPercentage + onDeliveryPercentage + downPaymentPercentage) > 100)
            {
                onInstallationPercentage = 0;
                onInstallationPercentageTextBox.Text = "";
                MessageBox.Show("You can't exceed 100%");
            }
            else
            {
                onInstallationActualTextBox.Text = GetPercentage(onInstallationPercentage, totalPrice).ToString();
                quotation.SetPercentOnInstallation(onInstallationPercentage);
            }
        }
        private void OnTextChangedDeliveryTimeFromTextBox(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(deliveryTimeTextBoxFrom.Text, BASIC_MACROS.PHONE_STRING) && deliveryTimeTextBoxFrom.Text != "")
                quotation.SetDeliveryTimeMinimum(int.Parse(deliveryTimeTextBoxFrom.Text));
            else
                deliveryTimeTextBoxFrom.Text = null;
        }
        private void OnTextChangedDeliveryTimeToTextBox(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(deliveryTimeTextBoxTo.Text, BASIC_MACROS.PHONE_STRING) && deliveryTimeTextBoxTo.Text != "")
                quotation.SetDeliveryTimeMaximum(int.Parse(deliveryTimeTextBoxTo.Text));
            else
                deliveryTimeTextBoxTo.Text = null;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ContractTypeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(contractTypeComboBox.SelectedIndex != -1)
                quotation.SetOfferContractType(contractTypes[contractTypeComboBox.SelectedIndex].contractId, contractTypes[contractTypeComboBox.SelectedIndex].contractName);

            if (contractTypeComboBox.SelectedIndex > 1)
            {
                deliveryPointCheckBox.IsChecked = false;
                deliveryTimeCheckBox.IsChecked = false;
                workOfferAdditionalInfoPage.warrantyPeriodCheckBox.IsChecked = false;
            }
            else
            {
                deliveryPointCheckBox.IsChecked = true;
                deliveryTimeCheckBox.IsChecked = true;
                workOfferAdditionalInfoPage.warrantyPeriodCheckBox.IsChecked = true;

                if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
                {
                    deliveryTimeTextBoxFrom.IsEnabled = false;
                    deliveryTimeTextBoxTo.IsEnabled = false;
                    deliveryTimeCombo.IsEnabled = false;
                    deliveryTimeFromWhenCombo.IsEnabled = false;


                    deliveryPointCombo.IsEnabled = false;


                    workOfferAdditionalInfoPage.warrantyPeriodTextBox.IsEnabled = false;
                    workOfferAdditionalInfoPage.warrantyPeriodCombo.IsEnabled = false;
                    workOfferAdditionalInfoPage.warrantyPeriodFromWhenCombo.IsEnabled = false;
                }
            }
        }

        private void OnSelChangedDeliveryPointCombo(object sender, SelectionChangedEventArgs e)
        {
            if(deliveryPointCombo.SelectedItem != null)
                quotation.SetDeliveryPoint(deliveryPoints[deliveryPointCombo.SelectedIndex].pointId, deliveryPoints[deliveryPointCombo.SelectedIndex].pointName);
        }
        private void OnSelChangedDeliveryTimeCombo(object sender, SelectionChangedEventArgs e)
        {
            if(deliveryTimeCombo.SelectedItem != null)
                quotation.SetDeliveryTimeUnit(timeUnits[deliveryTimeCombo.SelectedIndex].timeUnitId, timeUnits[deliveryTimeCombo.SelectedIndex].timeUnit);
        }
        private void OnSelChangedDeliveryTimeFromWhenCombo(object sender, SelectionChangedEventArgs e)
        {
            if(deliveryTimeFromWhenCombo.SelectedItem != null)
                quotation.SetOfferDeliveryTimeCondition(conditionStartDates[deliveryTimeFromWhenCombo.SelectedIndex].condition_id, conditionStartDates[deliveryTimeFromWhenCombo.SelectedIndex].condition_type);
        }
        private void OnSelChangedTotalPriceVATCombo(object sender, SelectionChangedEventArgs e)
        {
            bool tmp;
            if (totalPriceVATCombo.SelectedIndex == 0)
            {
                tmp = true;
                quotation.SetOfferVATCondition(tmp, vatConditions[totalPriceVATCombo.SelectedIndex].condition_type);
            }
            else
            {
                tmp = false;
                quotation.SetOfferVATCondition(tmp, vatConditions[totalPriceVATCombo.SelectedIndex].condition_type);
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                SetPercentAndValuesInWorkOffer();
            }
            workOfferBasicInfoPage.workOfferProductsPage = workOfferProductsPage;
            workOfferBasicInfoPage.workOfferPaymentAndDeliveryPage = this;
            workOfferBasicInfoPage.workOfferAdditionalInfoPage = workOfferAdditionalInfoPage;
            workOfferBasicInfoPage.workOfferUploadFilesPage = workOfferUploadFilesPage;

            NavigationService.Navigate(workOfferBasicInfoPage);
        }
        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                SetPercentAndValuesInWorkOffer();
            }

            workOfferProductsPage.workOfferBasicInfoPage = workOfferBasicInfoPage;
            workOfferProductsPage.workOfferPaymentAndDeliveryPage = this;
            workOfferProductsPage.workOfferAdditionalInfoPage = workOfferAdditionalInfoPage;
            workOfferProductsPage.workOfferUploadFilesPage = workOfferUploadFilesPage;

            NavigationService.Navigate(workOfferProductsPage);


        }
        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {

        }
        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                SetPercentAndValuesInWorkOffer();
            }
            

            workOfferAdditionalInfoPage.workOfferBasicInfoPage = workOfferBasicInfoPage;
            workOfferAdditionalInfoPage.workOfferProductsPage = workOfferProductsPage;
            workOfferAdditionalInfoPage.workOfferPaymentAndDeliveryPage = this;
            workOfferAdditionalInfoPage.workOfferUploadFilesPage = workOfferUploadFilesPage;

            NavigationService.Navigate(workOfferAdditionalInfoPage);

        }
        private void OnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                workOfferUploadFilesPage.workOfferBasicInfoPage = workOfferBasicInfoPage;
                workOfferUploadFilesPage.workOfferProductsPage = workOfferProductsPage;
                workOfferUploadFilesPage.workOfferPaymentAndDeliveryPage = this;
                workOfferUploadFilesPage.workOfferAdditionalInfoPage = workOfferAdditionalInfoPage;

                NavigationService.Navigate(workOfferUploadFilesPage);
            }
        }

        private void OnClickNextButton(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                SetPercentAndValuesInWorkOffer();
            }

            workOfferAdditionalInfoPage.workOfferBasicInfoPage = workOfferBasicInfoPage;
            workOfferAdditionalInfoPage.workOfferProductsPage = workOfferProductsPage;
            workOfferAdditionalInfoPage.workOfferPaymentAndDeliveryPage = this;
            workOfferAdditionalInfoPage.workOfferUploadFilesPage = workOfferUploadFilesPage;

            NavigationService.Navigate(workOfferAdditionalInfoPage);
        }

        private void OnClickBackButton(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                SetPercentAndValuesInWorkOffer();
            }

            workOfferProductsPage.workOfferBasicInfoPage = workOfferBasicInfoPage;
            workOfferProductsPage.workOfferPaymentAndDeliveryPage = this;
            workOfferProductsPage.workOfferAdditionalInfoPage = workOfferAdditionalInfoPage;
            workOfferProductsPage.workOfferUploadFilesPage = workOfferUploadFilesPage;

            NavigationService.Navigate(workOfferProductsPage);
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
        private void OnCheckDeliveryTime(object sender, RoutedEventArgs e)
        {
            deliveryTimeCombo.IsEnabled = true;
            deliveryTimeTextBoxFrom.IsEnabled = true;
            deliveryTimeTextBoxTo.IsEnabled = true;
            deliveryTimeFromWhenCombo.IsEnabled = true;

            deliveryTimeCombo.SelectedItem= null;
            deliveryTimeTextBoxFrom.Text = null;
            deliveryTimeTextBoxTo.Text = null;
            deliveryTimeFromWhenCombo.SelectedItem = null;
          
        }

        private void OnUnCheckDeliveryTime(object sender, RoutedEventArgs e)
        {
            deliveryTimeCombo.IsEnabled = false;
            deliveryTimeTextBoxFrom.IsEnabled = false;
            deliveryTimeTextBoxTo.IsEnabled = false;
            deliveryTimeFromWhenCombo.IsEnabled = false;

            deliveryTimeCombo.SelectedIndex = deliveryTimeCombo.Items.Count - 1;
            deliveryTimeTextBoxFrom.Text = null;
            deliveryTimeTextBoxTo.Text = null;
            deliveryTimeFromWhenCombo.SelectedIndex = deliveryTimeFromWhenCombo.Items.Count -1;
        }

        private void OnCheckDeliveryPoint(object sender, RoutedEventArgs e)
        {
            deliveryPointCombo.IsEnabled = true;

            deliveryPointCombo.SelectedItem = null;
        }

        private void OnUnCheckDeliveryPoint(object sender, RoutedEventArgs e)
        {
            deliveryPointCombo.IsEnabled = false;

            deliveryPointCombo.SelectedIndex = deliveryPointCombo.Items.Count - 1;
        }
    }
}
