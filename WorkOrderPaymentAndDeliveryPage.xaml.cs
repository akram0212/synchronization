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

        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> vatConditions = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> conditionStartDates = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> shipmentTypes = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

        private int viewAddCondition;
        private int downPaymentPercentage;
        private int onDeliveryPercentage;
        private int onInstallationPercentage;

        private Decimal totalPrice;

        public WorkOrderBasicInfoPage workOrderBasicInfoPage;
        public WorkOrderProductsPage workOrderProductsPage;
        public WorkOrderAdditionalInfoPage workOrderAdditionalInfoPage;
        public WorkOrderUploadFilesPage workOrderUploadFilesPage;

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
                InitializeDeliveryPointPortCombo();
                InitializeDeliveryTimeFromWhenCombo();

                DisableTotalPriceComboAndTextBox();
                SetTotalPriceCurrencyComboBox();
                SetTotalPriceTextBox();
                SetTotalPriceCurrencyComboBox();
                SetDownPaymentValues();
                SetOnDeliveryValues();
                SetOnInstallationValues();
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                InitializeTotalPriceCurrencyComboBox();
                InitializeDeliveryTimeComboBox();
                InitializeDeliveryPointComboBox();
                InitializeTotalPriceVATCombo();
                InitializeDeliveryPointPortCombo();
                InitializeDeliveryTimeFromWhenCombo();

                ConfigureViewUIElements();
                SetPriceValues();
                SetDownPaymentValues();
                SetOnDeliveryValues();
                SetOnInstallationValues();
                SetDeliveryTimeValues();
                SetDeliveryPointValue();

                cancelButton.IsEnabled = false;
            }
            //else if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION)
            //{
            //    InitializeTotalPriceCurrencyComboBox();
            //    InitializeDeliveryTimeComboBox();
            //    InitializeDeliveryPointComboBox();
            //    SetTotalPriceCurrencyComboBox();
            //    SetTotalPriceTextBox();
            //    SetDownPaymentValues();
            //    SetOnDeliveryValues();
            //    SetOnInstallationValues();
            //    SetDeliveryTimeValues();
            //    SetDeliveryPointValue();
            //    DisableTotalPriceComboAndTextBox();
            //}
            else
            {
                InitializeTotalPriceCurrencyComboBox();
                InitializeDeliveryTimeComboBox();
                InitializeDeliveryPointComboBox();
                InitializeTotalPriceVATCombo();
                InitializeDeliveryPointPortCombo();
                InitializeDeliveryTimeFromWhenCombo();

                DisableTotalPriceComboAndTextBox();
                SetTotalPriceCurrencyComboBox();
                SetTotalPriceTextBox();
                SetDeliveryTimeValues();
                SetDeliveryPointValue();
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
                totalPriceVATCombo.Items.Add(vatConditions[i].value);
            return true;
        } 
        private bool InitializeDeliveryTimeFromWhenCombo()
        {
            if (!commonQueriesObject.GetConditionStartDates(ref conditionStartDates))
                return false;

            for (int i = 0; i < conditionStartDates.Count; i++)
                deliveryTimeFromWhenCombo.Items.Add(conditionStartDates[i].value);
            return true;
        } 

        private bool InitializeDeliveryPointPortCombo()
        {
            if (!commonQueriesObject.GetShipmentTypes(ref shipmentTypes))
                return false;

            for (int i = 0; i < shipmentTypes.Count; i++)
                deliveryPointPortCombo.Items.Add(shipmentTypes[i].value);
            return true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        ///SET FUNCTIONS
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetTotalPriceCurrencyComboBox()
        {
            if (workOrder.GetOfferID() != null)
                totalPriceCombo.SelectedItem = workOrder.GetCurrency();
            else
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

        private void SetPercentAndValuesInWorkOrder()
        {
            workOrder.SetPercentValues();
        }

        private void SetPriceValues()
        {
            if (workOrder.GetCurrency() != null)
            {
                if (workOrder.GetOfferID() != null)
                {
                    totalPriceCombo.Text = workOrder.GetCurrency().ToString();
                    totalPriceTextBox.Text = workOrder.GetTotalPriceValue().ToString();
                    totalPrice = workOrder.GetTotalPriceValue();
                }
                else
                {
                    totalPriceCombo.Text = workOrder.GetOrderCurrency().ToString();
                    totalPriceTextBox.Text = workOrder.GetOrderTotalPriceValue().ToString();
                    totalPrice = workOrder.GetOrderTotalPriceValue();
                }
            }
        }

        public void SetDownPaymentValues()
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                if (workOrder.GetPercentDownPayment() != 0)
                {
                    downPaymentPercentageTextBox.Text = workOrder.GetPercentDownPayment().ToString();
                    downPaymentPercentage = int.Parse(downPaymentPercentageTextBox.Text);
                    downPaymentActualTextBox.Text = GetPercentage(downPaymentPercentage, totalPrice).ToString();
                }
            }
            else
            {
                if (workOrder.GetOrderPercentDownPayment() != 0)
                {
                    downPaymentPercentageTextBox.Text = workOrder.GetOrderPercentDownPayment().ToString();
                    downPaymentPercentage = int.Parse(downPaymentPercentageTextBox.Text);
                    downPaymentActualTextBox.Text = GetPercentage(downPaymentPercentage, totalPrice).ToString();
                }
            }

        }

        public void SetOnDeliveryValues()
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                if (workOrder.GetPercentOnDelivery() != 0)
                {
                    onDeliveryPercentageTextBox.Text = workOrder.GetPercentOnDelivery().ToString();
                    onDeliveryPercentage = int.Parse(onDeliveryPercentageTextBox.Text);
                    onDeliveryActualTextBox.Text = GetPercentage(onDeliveryPercentage, totalPrice).ToString();
                }
            }
            else
            {
                if (workOrder.GetOrderPercentOnDelivery() != 0)
                {
                    onDeliveryPercentageTextBox.Text = workOrder.GetOrderPercentOnDelivery().ToString();
                    onDeliveryPercentage = int.Parse(onDeliveryPercentageTextBox.Text);
                    onDeliveryActualTextBox.Text = GetPercentage(onDeliveryPercentage, totalPrice).ToString();
                }
            }
        }

        public void SetOnInstallationValues()
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                if (workOrder.GetPercentOnInstallation() != 0)
                {
                    onInstallationPercentageTextBox.Text = workOrder.GetPercentOnInstallation().ToString();
                    onDeliveryPercentage = int.Parse(onDeliveryPercentageTextBox.Text);
                    onInstallationActualTextBox.Text = GetPercentage(onInstallationPercentage, totalPrice).ToString();
                }
            }
            else
            {
                if (workOrder.GetOrderPercentOnInstallation() != 0)
                {
                    onInstallationPercentageTextBox.Text = workOrder.GetOrderPercentOnInstallation().ToString();
                    onDeliveryPercentage = int.Parse(onDeliveryPercentageTextBox.Text);
                    onInstallationActualTextBox.Text = GetPercentage(onInstallationPercentage, totalPrice).ToString();
                }
            }
        }

        public void SetDeliveryTimeValues()
        {
            ////////////Added by me ama get awareeh
            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                if (workOrder.GetDeliveryTimeMinimum() != 0)
                {
                    deliveryTimeTextBoxFrom.Text = workOrder.GetDeliveryTimeMinimum().ToString();
                    deliveryTimeTextBoxTo.Text = workOrder.GetDeliveryTimeMaximum().ToString();
                }
                if (workOrder.GetDeliveryTimeUnit() != null)
                    deliveryTimeCombo.Text = workOrder.GetDeliveryTimeUnit().ToString();
            }
            else
            {
                if (workOrder.GetOrderDeliveryTimeMinimum() != 0)
                {
                    deliveryTimeTextBoxFrom.Text = workOrder.GetOrderDeliveryTimeMinimum().ToString();
                    deliveryTimeTextBoxTo.Text = workOrder.GetOrderDeliveryTimeMaximum().ToString();
                }
                if (workOrder.GetOrderDeliveryTimeUnit() != null)
                    deliveryTimeCombo.Text = workOrder.GetOrderDeliveryTimeUnit().ToString();
            }
        }

        public void SetDeliveryPointValue()
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                deliveryPointCombo.SelectedItem = workOrder.GetDeliveryPoint();
            }
            else
            {
                deliveryPointCombo.SelectedItem = workOrder.GetOrderDeliveryPoint();
            }    
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
            workOrder.SetOrderDeliveryPoint(deliveryPoints[deliveryPointCombo.SelectedIndex].pointId, deliveryPoints[deliveryPointCombo.SelectedIndex].pointName);
        }
        private void OnSelChangedDeliveryTimeCombo(object sender, SelectionChangedEventArgs e)
        {
            workOrder.SetOrderDeliveryTimeUnit(timeUnits[deliveryTimeCombo.SelectedIndex].timeUnitId, timeUnits[deliveryTimeCombo.SelectedIndex].timeUnit);
        }
        private void OnSelChangedDeliveryTimeFromWhenCombo(object sender, SelectionChangedEventArgs e)
        {
            workOrder.SetOrderDeliveryTimeCondition(conditionStartDates[deliveryTimeFromWhenCombo.SelectedIndex].key, conditionStartDates[deliveryTimeFromWhenCombo.SelectedIndex].value);
        }
        private void OnSelChangedDeliveryPointPortCombo(object sender, SelectionChangedEventArgs e)
        {
            workOrder.SetOrderShipmentTypeCondition(shipmentTypes[deliveryPointPortCombo.SelectedIndex].key, shipmentTypes[deliveryPointPortCombo.SelectedIndex].value);
        }
        private void OnSelChangedTotalPriceVATCombo(object sender, SelectionChangedEventArgs e)
        {
            bool tmp;
            if (totalPriceVATCombo.SelectedIndex == 0)
            {
                tmp = true;
                workOrder.SetOrderVATCondition(tmp, vatConditions[totalPriceVATCombo.SelectedIndex].value);
            }
            else
            {
                tmp = false;
                workOrder.SetOrderVATCondition(tmp, vatConditions[totalPriceVATCombo.SelectedIndex].value);
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
            workOrderBasicInfoPage.workOrderPaymentAndDeliveryPage = this;
            workOrderBasicInfoPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderBasicInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderBasicInfoPage);
        }
        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                SetPercentAndValuesInWorkOrder();
            }

            workOrderProductsPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
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

            workOrderAdditionalInfoPage.SetContractTypeValue();
            workOrderAdditionalInfoPage.SetWarrantyPeriodValues();
            workOrderAdditionalInfoPage.SetAdditionalDescriptionValue();

            workOrderAdditionalInfoPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
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
            workOrderAdditionalInfoPage.SetContractTypeValue();
            workOrderAdditionalInfoPage.SetWarrantyPeriodValues();
            workOrderAdditionalInfoPage.SetAdditionalDescriptionValue();

            workOrderAdditionalInfoPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
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
