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
        private int totalPrice;
        private int downPaymentPercentage;
        private int onDeliveryPercentage;
        private int onInstallationPercentage;

        public WorkOfferPaymentAndDeliveryPage(ref Employee mLoggedInUser, ref WorkOffer mWorkOffer, int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;
            InitializeComponent();

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            workOffer = new WorkOffer(sqlDatabase);
            workOffer = mWorkOffer;

            totalPrice = 0;

            if(viewAddCondition == 1)
            {
                InitializeTotalPriceCurrencyComboBox();
                InitializeDeliveryTimeComboBox();
                InitializeDeliveryPointComboBox();

                SetTotalPriceCurrencyComboBox();
                SetTotalPriceTextBox();
            }
        }

        private void InitializeTotalPriceCurrencyComboBox()
        {
            commonQueriesObject.GetCurrencyTypes(ref currencies);
            for (int i = 0; i < currencies.Count; i++)
                totalPriceCombo.Items.Add(currencies[i].currencyName);
            
        }

        private void SetTotalPriceCurrencyComboBox()
        {
            totalPriceCombo.Text = workOffer.GetCurrency();
        }

        private void SetTotalPriceTextBox()
        {
            for(int i = 1; i <= COMPANY_WORK_MACROS.MAX_OFFER_PRODUCTS; i++)
                totalPrice += workOffer.GetProductPriceValue(i) * workOffer.GetOfferProductQuantity(i);

            totalPriceTextBox.Text = totalPrice.ToString();
            workOffer.SetTotalValues();
        }

        private void InitializeDeliveryTimeComboBox()
        {
            commonQueriesObject.GetTimeUnits(ref timeUnits);
        }

        private void InitializeDeliveryPointComboBox()
        {
            commonQueriesObject.GetDeliveryPoints(ref deliveryPoints);
        }
        private double GetPercentage(int percentage, int total)
        {
            if (percentage != 0)
            {
                var value = ((double)percentage / 100) * total;
                var percentage1 = Convert.ToInt32(Math.Round(value, 0));
                return percentage1;
            }
            else
                return 0;
            
        }
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            WorkOfferBasicInfoPage basicInfoPage = new WorkOfferBasicInfoPage(ref loggedInUser, ref workOffer, viewAddCondition);
            NavigationService.Navigate(basicInfoPage);
        }

        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            WorkOfferProductsPage workOfferProductsPage = new WorkOfferProductsPage(ref loggedInUser, ref workOffer, viewAddCondition);
            NavigationService.Navigate(workOfferProductsPage);
        }

        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {

        }

        private void DownPaymentPercentageTextBoxTextChanged(object sender, TextChangedEventArgs e)
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
                downPaymentActualTextBox.Text = "0";
                MessageBox.Show("You can't exceed 100%");
            }
            else
                downPaymentActualTextBox.Text = GetPercentage(downPaymentPercentage, totalPrice).ToString();
            
        }

        private void OnDeliveryPercentageTextBoxTextChanged(object sender, TextChangedEventArgs e)
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
                onDeliveryPercentageTextBox.Text = "0";
                MessageBox.Show("You can't exceed 100%");
            }
            else
            {

                onDeliveryActualTextBox.Text = GetPercentage(onDeliveryPercentage, totalPrice).ToString();
            }
        }

        private void OnInstallationPercentageTextBoxTextChanged(object sender, TextChangedEventArgs e)
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
                onInstallationPercentageTextBox.Text = "0";
                MessageBox.Show("You can't exceed 100%");
            }
            else
                onInstallationActualTextBox.Text = GetPercentage(onInstallationPercentage, totalPrice).ToString();
            
        }

        private void DeliveryPointComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            workOffer.SetDeliveryPoint(deliveryPoints[deliveryPointCombo.SelectedIndex].pointId, deliveryPoints[deliveryPointCombo.SelectedIndex].pointName);
        }

        private void DeliveryTimeTextBoxFromTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(deliveryTimeTextBoxFrom.Text, BASIC_MACROS.PHONE_STRING) && deliveryTimeTextBoxFrom.Text != "")
                workOffer.SetDeliveryTimeMinimum(int.Parse(deliveryTimeTextBoxFrom.Text));
            else
                deliveryTimeTextBoxFrom.Text = null;
        }

        private void DeliveryTimeTextBoxToTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(deliveryTimeTextBoxTo.Text, BASIC_MACROS.PHONE_STRING) && deliveryTimeTextBoxTo.Text != "")
                workOffer.SetDeliveryTimeMaximum(int.Parse(deliveryTimeTextBoxTo.Text));
            else
                deliveryTimeTextBoxTo.Text = null;
        }

        private void DeliveryTimeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            workOffer.SetDeliveryTimeUnit(timeUnits[deliveryTimeCombo.SelectedIndex].timeUnitId, timeUnits[deliveryTimeCombo.SelectedIndex].timeUnit);
        }
    }
}
