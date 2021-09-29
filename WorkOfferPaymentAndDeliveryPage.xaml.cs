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
        WorkOffer workOffer;

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private IntegrityChecks IntegrityChecks = new IntegrityChecks();

        private List<BASIC_STRUCTS.CURRENCY_STRUCT> currencies = new List<BASIC_STRUCTS.CURRENCY_STRUCT>();
        private List<BASIC_STRUCTS.TIMEUNIT_STRUCT> timeUnits = new List<BASIC_STRUCTS.TIMEUNIT_STRUCT>();
        private List<BASIC_STRUCTS.DELIVERY_POINT_STRUCT> deliveryPoints = new List<BASIC_STRUCTS.DELIVERY_POINT_STRUCT>();

        private int viewAddCondition;
        private int downPaymentPercentage;
        private int onDeliveryPercentage;
        private int onInstallationPercentage;

        private Decimal totalPrice;

        public WorkOfferBasicInfoPage workOfferBasicInfoPage;
        public WorkOfferProductsPage workOfferProductsPage;
        public WorkOfferAdditionalInfoPage workOfferAdditionalInfoPage;
        public WorkOfferUploadFilesPage workOfferUploadFilesPage;

        public WorkOfferPaymentAndDeliveryPage(ref Employee mLoggedInUser, ref WorkOffer mWorkOffer, int mViewAddCondition, ref WorkOfferAdditionalInfoPage mWorkOfferAdditionalInfoPage)
        {
            workOfferAdditionalInfoPage = mWorkOfferAdditionalInfoPage;

            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;
            
            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();
            
            workOffer = mWorkOffer;

            totalPrice = 0;

            InitializeComponent();

            if (viewAddCondition == COMPANY_WORK_MACROS.OFFER_ADD_CONDITION)
            {
                InitializeTotalPriceCurrencyComboBox();
                InitializeDeliveryTimeComboBox();
                InitializeDeliveryPointComboBox();

                DisableTotalPriceComboAndTextBox();
                SetTotalPriceCurrencyComboBox();
                SetTotalPriceTextBox();
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.OFFER_VIEW_CONDITION)
            {
                InitializeTotalPriceCurrencyComboBox();
                InitializeDeliveryTimeComboBox();
                InitializeDeliveryPointComboBox();

                ConfigureViewUIElements();
                SetPriceValues();
                SetDownPaymentValues();
                SetOnDeliveryValues();
                SetOnInstallationValues();
                SetDeliveryTimeValues();
                SetDeliveryPointValue();

                cancelButton.IsEnabled = false;
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.OFFER_REVISE_CONDITION)
            {
                InitializeTotalPriceCurrencyComboBox();
                InitializeDeliveryTimeComboBox();
                InitializeDeliveryPointComboBox();
                SetTotalPriceCurrencyComboBox();
                SetTotalPriceTextBox();
                SetDownPaymentValues();
                SetOnDeliveryValues();
                SetOnInstallationValues();
                SetDeliveryTimeValues();
                SetDeliveryPointValue();
                DisableTotalPriceComboAndTextBox();
            }
            else
            {
                InitializeTotalPriceCurrencyComboBox();
                InitializeDeliveryTimeComboBox();
                InitializeDeliveryPointComboBox();

                DisableTotalPriceComboAndTextBox();
                SetTotalPriceCurrencyComboBox();
                SetTotalPriceTextBox();
            }
        }
        public WorkOfferPaymentAndDeliveryPage(ref WorkOffer mWorkOffer)
        {
            workOffer = mWorkOffer;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        ///CONFIGURE UI ELEMENTS
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        private void ConfigureViewUIElements()
        {
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

        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        ///SET FUNCTIONS
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetTotalPriceCurrencyComboBox()
        {
            totalPriceCombo.SelectedItem = workOffer.GetCurrency();
        }

        public void SetTotalPriceTextBox()
        {
            totalPrice = 0;

            for(int i = 1; i <= COMPANY_WORK_MACROS.MAX_OFFER_PRODUCTS; i++)
                totalPrice += workOffer.GetProductPriceValue(i) * workOffer.GetOfferProductQuantity(i);

            totalPriceTextBox.Text = totalPrice.ToString();
            workOffer.SetTotalValues();
        }

        private void SetPercentAndValuesInWorkOffer()
        {
            workOffer.SetPercentValues();
        }

        private void SetPriceValues()
        {
            if (workOffer.GetCurrency() != null)
            {
                totalPriceCombo.Text = workOffer.GetCurrency().ToString();
                totalPriceTextBox.Text = workOffer.GetTotalPriceValue().ToString();
                totalPrice = workOffer.GetTotalPriceValue();
            }
        }

        public void SetDownPaymentValues()
        {
            if (workOffer.GetPercentDownPayment() != 0)
            {
                downPaymentPercentageTextBox.Text = workOffer.GetPercentDownPayment().ToString();
                downPaymentPercentage = int.Parse(downPaymentPercentageTextBox.Text);
                downPaymentActualTextBox.Text = GetPercentage(downPaymentPercentage, totalPrice).ToString();
            }

        }

        public void SetOnDeliveryValues()
        {
            if (workOffer.GetPercentOnDelivery() != 0)
            {
                onDeliveryPercentageTextBox.Text = workOffer.GetPercentOnDelivery().ToString();
                onDeliveryPercentage = int.Parse(onDeliveryPercentageTextBox.Text);
                onDeliveryActualTextBox.Text = GetPercentage(onDeliveryPercentage, totalPrice).ToString();
            }
        }

        public void SetOnInstallationValues()
        {
            if (workOffer.GetPercentOnInstallation() != 0)
            {
                onInstallationPercentageTextBox.Text = workOffer.GetPercentOnInstallation().ToString();
                onDeliveryPercentage = int.Parse(onDeliveryPercentageTextBox.Text);
                onInstallationActualTextBox.Text = GetPercentage(onInstallationPercentage, totalPrice).ToString();
            }
        }

        private void SetDeliveryTimeValues()
        {
            ////////////Added by me ama get awareeh
            if (workOffer.GetDeliveryTimeMinimum() != 0)
            {
                deliveryTimeTextBoxFrom.Text = workOffer.GetDeliveryTimeMinimum().ToString();
                deliveryTimeTextBoxTo.Text = workOffer.GetDeliveryTimeMaximum().ToString();
            }
            if(workOffer.GetDeliveryTimeUnit() != null)
                deliveryTimeCombo.Text = workOffer.GetDeliveryTimeUnit().ToString();
        }

        private void SetDeliveryPointValue()
        {
            deliveryPointCombo.SelectedItem = workOffer.GetDeliveryPoint();
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
                workOffer.SetPercentDownPayment(downPaymentPercentage);
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
                workOffer.SetPercentOnDelivery(onDeliveryPercentage);
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
                workOffer.SetPercentOnInstallation(onInstallationPercentage);
            }
        }
        private void OnTextChangedDeliveryTimeFromTextBox(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(deliveryTimeTextBoxFrom.Text, BASIC_MACROS.PHONE_STRING) && deliveryTimeTextBoxFrom.Text != "")
                workOffer.SetDeliveryTimeMinimum(int.Parse(deliveryTimeTextBoxFrom.Text));
            else
                deliveryTimeTextBoxFrom.Text = null;
        }
        private void OnTextChangedDeliveryTimeToTextBox(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(deliveryTimeTextBoxTo.Text, BASIC_MACROS.PHONE_STRING) && deliveryTimeTextBoxTo.Text != "")
                workOffer.SetDeliveryTimeMaximum(int.Parse(deliveryTimeTextBoxTo.Text));
            else
                deliveryTimeTextBoxTo.Text = null;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnSelChangedDeliveryPointCombo(object sender, SelectionChangedEventArgs e)
        {
            workOffer.SetDeliveryPoint(deliveryPoints[deliveryPointCombo.SelectedIndex].pointId, deliveryPoints[deliveryPointCombo.SelectedIndex].pointName);
        }
        private void OnSelChangedDeliveryTimeCombo(object sender, SelectionChangedEventArgs e)
        {
            workOffer.SetDeliveryTimeUnit(timeUnits[deliveryTimeCombo.SelectedIndex].timeUnitId, timeUnits[deliveryTimeCombo.SelectedIndex].timeUnit);
        }
        private void OnSelChangedDeliveryTimeFromWhenCombo(object sender, SelectionChangedEventArgs e)
        {

        }
        private void OnSelChangedDeliveryPointPortCombo(object sender, SelectionChangedEventArgs e)
        {

        }
        private void OnSelChangedTotalPriceVATCombo(object sender, SelectionChangedEventArgs e)
        {

        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.OFFER_VIEW_CONDITION)
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
            if (viewAddCondition != COMPANY_WORK_MACROS.OFFER_VIEW_CONDITION)
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
            if (viewAddCondition != COMPANY_WORK_MACROS.OFFER_VIEW_CONDITION)
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
            if (viewAddCondition == COMPANY_WORK_MACROS.OFFER_VIEW_CONDITION)
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
            if (viewAddCondition != COMPANY_WORK_MACROS.OFFER_VIEW_CONDITION)
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
            if (viewAddCondition != COMPANY_WORK_MACROS.OFFER_VIEW_CONDITION)
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
    }
}
