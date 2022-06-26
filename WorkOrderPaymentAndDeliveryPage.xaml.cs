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
    /// Interaction logic for WorkOrderPaymentAndDeliveryPage.xaml
    /// </summary>
    public partial class WorkOrderPaymentAndDeliveryPage : Page
    {
        Employee loggedInUser;
        WorkOrder workOrder;

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

        private int viewAddCondition;
        private int downPaymentPercentage;
        private int onDeliveryPercentage;
        private int onInstallationPercentage;

        private Decimal totalPrice;

        public WorkOrderBasicInfoPage workOrderBasicInfoPage;
        public WorkOrderProductsPage workOrderProductsPage;
        public WorkOrderAdditionalInfoPage workOrderAdditionalInfoPage;
        public WorkOrderUploadFilesPage workOrderUploadFilesPage;
        public WorkOrderProjectInfoPage workOrderProjectInfoPage;

        public WorkOrderPaymentAndDeliveryPage(ref Employee mLoggedInUser, ref WorkOrder mWorkOrder, int mViewAddCondition, ref WorkOrderAdditionalInfoPage mWorkOrderAdditionalInfoPage)
        {
            workOrderAdditionalInfoPage = mWorkOrderAdditionalInfoPage;

            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            workOrder = mWorkOrder;

            totalPrice = 0;

            InitializeComponent();

            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_ADD_CONDITION)
            {
                InitializeTotalPriceCurrencyComboBox();
                InitializeDeliveryTimeComboBox();
                InitializeDeliveryPointComboBox();
                InitializeTotalPriceVATCombo();
                InitializeDeliveryTimeFromWhenCombo();

                DisableTotalPriceComboAndTextBox();
                //SetTotalPriceCurrencyComboBox();
                //SetTotalPriceTextBox();
                //SetDownPaymentValues();
                //SetOnDeliveryValues();
                //SetOnInstallationValues();
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                InitializeTotalPriceCurrencyComboBox();
                InitializeDeliveryTimeComboBox();
                InitializeDeliveryPointComboBox();
                InitializeTotalPriceVATCombo();
                InitializeDeliveryTimeFromWhenCombo();

                ConfigureViewUIElements();
                SetPriceValues();
                SetTotalPriceCondition();
                SetDownPaymentValues();
                SetOnDeliveryValues();
                SetOnInstallationValues();
                SetDeliveryTimeValues();
                SetDeliveryPointValue();

                cancelButton.IsEnabled = false;
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION)
            {
                InitializeTotalPriceCurrencyComboBox();
                InitializeDeliveryTimeComboBox();
                InitializeDeliveryPointComboBox();
                InitializeTotalPriceVATCombo();
                InitializeDeliveryTimeFromWhenCombo();

                SetTotalPriceCurrencyComboBox();
                SetTotalPriceTextBox();
                SetTotalPriceCondition();
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
                InitializeTotalPriceVATCombo();
                InitializeDeliveryTimeFromWhenCombo();

                //DisableTotalPriceComboAndTextBox();
                //SetTotalPriceCurrencyComboBox();
                //SetTotalPriceTextBox();
                //SetTotalPriceCondition();
                //SetDownPaymentValues();
                //SetOnDeliveryValues();
                //SetOnInstallationValues();
                //SetDeliveryTimeValues();
                //SetDeliveryPointValue();
                //SetNullsToZeros();
            }
        }
        public WorkOrderPaymentAndDeliveryPage(ref WorkOrder mWorkOrder)
        {
            workOrder = mWorkOrder;
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
        public void SetTotalPriceCurrencyComboBox()
        {

            totalPriceCombo.SelectedItem = workOrder.GetOrderCurrency();
        }

        public void SetTotalPriceTextBox()
        {
            totalPrice = 0;

            for (int i = 1; i <= COMPANY_WORK_MACROS.MAX_ORDER_PRODUCTS; i++)
            {
                if(workOrder.GetOfferID() != null)
                    totalPrice += workOrder.GetProductPriceValue(i) * workOrder.GetOfferProductQuantity(i);
                else
                    totalPrice += workOrder.GetOrderProductPriceValue(i) * workOrder.GetOrderProductQuantity(i);
            }

            totalPriceTextBox.Text = totalPrice.ToString();
            workOrder.SetOrderTotalValues();
        }

        public void SetTotalPriceCondition()
        {

            totalPriceVATCombo.SelectedItem = workOrder.GetOrderVATCondition();
        }

        private void SetPercentAndValuesInWorkOrder()
        {
            workOrder.SetPercentValues();
        }

        private void SetPriceValues()
        {

            if (workOrder.GetOfferID() != null && viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION || workOrderBasicInfoPage.InitializationComplete == true)
                {
                    totalPriceCombo.Text = workOrder.GetCurrency().ToString();
                    totalPriceTextBox.Text = workOrder.GetTotalPriceValue().ToString();
                    totalPrice = workOrder.GetTotalPriceValue();
                }
            }
            else
            {
                if (workOrder.GetOrderCurrencyId() != 0)
                {
                    totalPriceCombo.SelectedItem = workOrder.GetOrderCurrency().ToString();
                    totalPriceTextBox.Text = workOrder.GetOrderTotalPriceValue().ToString();
                    totalPrice = workOrder.GetOrderTotalPriceValue();
                }
            }

        }

        public void SetDownPaymentValues()
        {

            if (workOrder.GetOrderPercentDownPayment() != 0)
            {
                downPaymentPercentageTextBox.Text = workOrder.GetOrderPercentDownPayment().ToString();
                downPaymentPercentage = int.Parse(downPaymentPercentageTextBox.Text);
                downPaymentActualTextBox.Text = GetPercentage(downPaymentPercentage, totalPrice).ToString();
            }


        }

        public void SetOnDeliveryValues()
        {


            if (workOrder.GetOrderPercentOnDelivery() != 0)
            {
                onDeliveryPercentageTextBox.Text = workOrder.GetOrderPercentOnDelivery().ToString();
                if (onDeliveryPercentageTextBox.Text != "")
                {
                    onDeliveryPercentage = int.Parse(onDeliveryPercentageTextBox.Text);
                    onDeliveryActualTextBox.Text = GetPercentage(onDeliveryPercentage, totalPrice).ToString();
                }
            }

        }

        public void SetOnInstallationValues()
        {


            if (workOrder.GetOrderPercentOnInstallation() != 0)
            {
                onInstallationPercentageTextBox.Text = workOrder.GetOrderPercentOnInstallation().ToString();
                if (onDeliveryPercentageTextBox.Text != "")
                {
                    onDeliveryPercentage = int.Parse(onDeliveryPercentageTextBox.Text);
                    onInstallationActualTextBox.Text = GetPercentage(onInstallationPercentage, totalPrice).ToString();
                }
            }

        }

        public void SetDeliveryTimeValues()
        {
            
            deliveryTimeTextBoxFrom.Text = workOrder.GetOrderDeliveryTimeMinimum().ToString();
            deliveryTimeTextBoxTo.Text = workOrder.GetOrderDeliveryTimeMaximum().ToString();

            if (workOrder.GetOrderDeliveryTimeUnit() != null)
                deliveryTimeCombo.Text = workOrder.GetOrderDeliveryTimeUnit().ToString();

            deliveryTimeFromWhenCombo.SelectedItem = workOrder.GetOrderDeliveryTimeCondition();

        }

        public void SetDeliveryPointValue()
        {


            deliveryPointCombo.SelectedItem = workOrder.GetOrderDeliveryPoint();

        }

        public void SetDownPaymentPercentage()
        {
            downPaymentActualTextBox.Text = GetPercentage(downPaymentPercentage, totalPrice).ToString();
        }

        public void SetNullsToZeros()
        {
            if (downPaymentActualTextBox.Text == "0")
                downPaymentPercentageTextBox.Text = "0";

            if (onDeliveryActualTextBox.Text == "0")
                onDeliveryPercentageTextBox.Text = "0";

            if (onInstallationActualTextBox.Text == "0")
                onInstallationPercentageTextBox.Text = "0";

            if (deliveryTimeTextBoxFrom.Text == null)
                deliveryTimeTextBoxFrom.Text = "0";

            if (deliveryTimeTextBoxTo.Text == null)
                deliveryTimeTextBoxTo.Text = "0";
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
                workOrder.SetOrderPercentDownPayment(downPaymentPercentage);
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
                workOrder.SetOrderPercentOnDelivery(onDeliveryPercentage);
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
                workOrder.SetOrderPercentOnInstallation(onInstallationPercentage);
            }
        }
        private void OnTextChangedDeliveryTimeFromTextBox(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(deliveryTimeTextBoxFrom.Text, BASIC_MACROS.PHONE_STRING) && deliveryTimeTextBoxFrom.Text != "")
                workOrder.SetOrderDeliveryTimeMinimum(int.Parse(deliveryTimeTextBoxFrom.Text));
            else
                deliveryTimeTextBoxFrom.Text = null;
        }
        private void OnTextChangedDeliveryTimeToTextBox(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(deliveryTimeTextBoxTo.Text, BASIC_MACROS.PHONE_STRING) && deliveryTimeTextBoxTo.Text != "")
                workOrder.SetOrderDeliveryTimeMaximum(int.Parse(deliveryTimeTextBoxTo.Text));
            else
                deliveryTimeTextBoxTo.Text = null;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnSelChangedDeliveryPointCombo(object sender, SelectionChangedEventArgs e)
        {
            if(deliveryPointCombo.SelectedIndex != -1)
                workOrder.SetOrderDeliveryPoint(deliveryPoints[deliveryPointCombo.SelectedIndex].pointId, deliveryPoints[deliveryPointCombo.SelectedIndex].pointName);
        }
        private void OnSelChangedDeliveryTimeCombo(object sender, SelectionChangedEventArgs e)
        {
            if (deliveryTimeCombo.SelectedIndex != -1)
                workOrder.SetOrderDeliveryTimeUnit(timeUnits[deliveryTimeCombo.SelectedIndex].timeUnitId, timeUnits[deliveryTimeCombo.SelectedIndex].timeUnit);
        }
        private void OnSelChangedDeliveryTimeFromWhenCombo(object sender, SelectionChangedEventArgs e)
        {
            if (deliveryTimeFromWhenCombo.SelectedIndex != -1)
                workOrder.SetOrderDeliveryTimeCondition(conditionStartDates[deliveryTimeFromWhenCombo.SelectedIndex].condition_id, conditionStartDates[deliveryTimeFromWhenCombo.SelectedIndex].condition_type);
        }
        private void OnSelChangedTotalPriceVATCombo(object sender, SelectionChangedEventArgs e)
        {
            bool tmp;
            if (totalPriceVATCombo.SelectedIndex == 0)
            {
                tmp = true;
                workOrder.SetOrderVATCondition(tmp, vatConditions[totalPriceVATCombo.SelectedIndex].condition_type);
            }
            else
            {
                tmp = false;
                workOrder.SetOrderVATCondition(tmp, vatConditions[totalPriceVATCombo.SelectedIndex].condition_type);
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                SetPercentAndValuesInWorkOrder();
            }
            workOrderBasicInfoPage.workOrderProductsPage = workOrderProductsPage;
            workOrderBasicInfoPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
            workOrderBasicInfoPage.workOrderPaymentAndDeliveryPage = this;
            workOrderBasicInfoPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderBasicInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderBasicInfoPage);
        }
        private void OnClickProjectInfo(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                SetPercentAndValuesInWorkOrder();
            }
            workOrderProjectInfoPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderProjectInfoPage.workOrderProductsPage = workOrderProductsPage;
            workOrderProjectInfoPage.workOrderPaymentAndDeliveryPage = this;
            workOrderProjectInfoPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderProjectInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderProjectInfoPage);
        }
        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                SetPercentAndValuesInWorkOrder();
            }

            workOrderProductsPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderProductsPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
            workOrderProductsPage.workOrderPaymentAndDeliveryPage = this;
            workOrderProductsPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderProductsPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderProductsPage);


        }
        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {

        }
        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                SetPercentAndValuesInWorkOrder();
            }

            workOrderAdditionalInfoPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderAdditionalInfoPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
            workOrderAdditionalInfoPage.workOrderProductsPage = workOrderProductsPage;
            workOrderAdditionalInfoPage.workOrderPaymentAndDeliveryPage = this;
            workOrderAdditionalInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderAdditionalInfoPage);

        }
        private void OnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                workOrderUploadFilesPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
                workOrderUploadFilesPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
                workOrderUploadFilesPage.workOrderProductsPage = workOrderProductsPage;
                workOrderUploadFilesPage.workOrderPaymentAndDeliveryPage = this;
                workOrderUploadFilesPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;

                NavigationService.Navigate(workOrderUploadFilesPage);
            }
        }

        private void OnClickNextButton(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                SetPercentAndValuesInWorkOrder();
            }
            
            workOrderAdditionalInfoPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderAdditionalInfoPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
            workOrderAdditionalInfoPage.workOrderProductsPage = workOrderProductsPage;
            workOrderAdditionalInfoPage.workOrderPaymentAndDeliveryPage = this;
            workOrderAdditionalInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderAdditionalInfoPage);
        }

        private void OnClickBackButton(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                SetPercentAndValuesInWorkOrder();
            }

            workOrderProductsPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderProductsPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
            workOrderProductsPage.workOrderPaymentAndDeliveryPage = this;
            workOrderProductsPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderProductsPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderProductsPage);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///BUTTON CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnButtonClickAutomateWorkOrder(object sender, RoutedEventArgs e)
        {
        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;

            currentWindow.Close();
        }

        private void OnTextChangedTotalPrice(object sender, TextChangedEventArgs e)
        {
            SetTotalPriceTextBox();
            SetTotalPriceCurrencyComboBox();
            SetDownPaymentValues();
            SetOnDeliveryValues();
            SetOnInstallationValues();

            downPaymentActualTextBox.Text = GetPercentage(downPaymentPercentage, totalPrice).ToString();
            onDeliveryActualTextBox.Text = GetPercentage(onDeliveryPercentage, totalPrice).ToString();
            onInstallationActualTextBox.Text = GetPercentage(onInstallationPercentage, totalPrice).ToString();
        }

        
    }
}
