using _01electronics_library;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for AddCollectionWindow.xaml
    /// </summary>
    public partial class AddCollectionWindow : Window
    {
        WorkOrder workOrder;
        SQLServer sqlServer;
        CommonQueries commonQueries;

        List<COMPANY_WORK_MACROS.WORK_ORDER_MAX_STRUCT> workOrders = new List<COMPANY_WORK_MACROS.WORK_ORDER_MAX_STRUCT>();
        public AddCollectionWindow(WorkOrder mWorkOrder)
        {
            sqlServer = new SQLServer();
            commonQueries = new CommonQueries(sqlServer);
            workOrder = mWorkOrder;

            InitializeComponent();

            InitializeOrderSerialCombo();
            SetOrderSerialCombo();
        }

        private bool InitializeOrderSerialCombo()
        {
            if (!commonQueries.GetWorkOrders(ref workOrders))
                return false;

            for (int i = 0; i < workOrders.Count; i++)
                orderSerialCombo.Items.Add(workOrders[i].order_id);

            return true;
        }

        private void InitializeOrderGrid()
        {
            salesPersonLabel.Content = workOrder.GetSalesPersonName();
            companyNameLabel.Content = workOrder.GetCompanyName();
            contractTypeLabel.Content = workOrder.GetOrderContractType();
            totalAmountLabel.Content = workOrder.GetOrderTotalPriceValue().ToString();

            Label collectionHistoryLabel = new Label();
            collectionHistoryLabel.Style = (Style)FindResource("labelStyle");
            collectionHistoryLabel.Content = "No History";
            collectionHistoryLabel.HorizontalAlignment = HorizontalAlignment.Center;

            collectionHistoryStackPanel.Children.Add(collectionHistoryLabel);

            remainingAmountLabel.Content = workOrder.GetOrderTotalPriceValue().ToString();
        }

        private void SetOrderSerialCombo()
        {
            orderSerialCombo.SelectedItem = workOrder.GetOrderID();
            if (orderSerialCombo.SelectedItem != null)
                orderSerialCombo.IsEnabled = false;
        }

        private bool GetCollectionHistory()
        {
            return true;
        }

        private void OnSelChangedOrderSerialCombo(object sender, SelectionChangedEventArgs e)
        {
            if (workOrders[orderSerialCombo.SelectedIndex].order_serial != workOrder.GetOrderSerial())
                workOrder.InitializeWorkOrderInfo(workOrders[orderSerialCombo.SelectedIndex].order_serial);

            InitializeOrderGrid();
        }

        private void OnSelChangedCollectionTypeCombo(object sender, SelectionChangedEventArgs e)
        {

        }
        private void OnTextChangedCollectionAmountTextBox(object sender, TextChangedEventArgs e)
        {

        }

        private void OnBtnClickSaveChanges(object sender, RoutedEventArgs e)
        {

        }


    }
}
