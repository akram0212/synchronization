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
    /// Interaction logic for WorkOrderProjectInfoPage.xaml
    /// </summary>
    public partial class WorkOrderProjectInfoPage : Page
    {

        Employee loggedInUser;
        WorkOrder workOrder;

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private IntegrityChecks IntegrityChecks = new IntegrityChecks();

        private int viewAddCondition;

        private List<BASIC_STRUCTS.PROJECT_STRUCT> projects = new List<BASIC_STRUCTS.PROJECT_STRUCT>();

        public WorkOrderBasicInfoPage workOrderBasicInfoPage;
        public WorkOrderProductsPage workOrderProductsPage;
        public WorkOrderPaymentAndDeliveryPage workOrderPaymentAndDeliveryPage;
        public WorkOrderAdditionalInfoPage workOrderAdditionalInfoPage;
        public WorkOrderUploadFilesPage workOrderUploadFilesPage;

        public WorkOrderProjectInfoPage(ref Employee mLoggedInUser, ref WorkOrder mWorkOrder, int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            workOrder = mWorkOrder;

            InitializeComponent();
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////SELECTION CHANGED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnSelChangedProjectLocationCombo(object sender, SelectionChangedEventArgs e)
        {

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////CHECK/UNCHECK HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnCheckProjectLocation(object sender, RoutedEventArgs e)
        {

        }

        private void OnUnCheckProjectLocation(object sender, RoutedEventArgs e)
        {

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderBasicInfoPage.workOrderProductsPage = workOrderProductsPage;
            workOrderBasicInfoPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderBasicInfoPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderBasicInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;
            workOrderBasicInfoPage.workOrderProjectInfoPage = this;

            NavigationService.Navigate(workOrderBasicInfoPage);
        }
        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderProductsPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderProductsPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderProductsPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderProductsPage.workOrderUploadFilesPage = workOrderUploadFilesPage;
            workOrderProductsPage.workOrderProjectInfoPage = this;

            NavigationService.Navigate(workOrderProductsPage);
        }
        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderPaymentAndDeliveryPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderProductsPage = workOrderProductsPage;
            workOrderPaymentAndDeliveryPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderUploadFilesPage = workOrderUploadFilesPage;
            workOrderPaymentAndDeliveryPage.workOrderProjectInfoPage = this;

            NavigationService.Navigate(workOrderPaymentAndDeliveryPage);
        }
        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderAdditionalInfoPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderAdditionalInfoPage.workOrderProductsPage = workOrderProductsPage;
            workOrderAdditionalInfoPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderAdditionalInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;
            workOrderAdditionalInfoPage.workOrderProjectInfoPage = this;

            NavigationService.Navigate(workOrderAdditionalInfoPage);
        }
        private void OnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                workOrderUploadFilesPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
                workOrderUploadFilesPage.workOrderProductsPage = workOrderProductsPage;
                workOrderUploadFilesPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
                workOrderUploadFilesPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
                workOrderUploadFilesPage.workOrderProjectInfoPage = this;

                NavigationService.Navigate(workOrderUploadFilesPage);
            }
        }

        private void OnBtnClickNext(object sender, RoutedEventArgs e)
        {
            workOrderProductsPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderProductsPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderProductsPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderProductsPage.workOrderUploadFilesPage = workOrderUploadFilesPage;
            workOrderProductsPage.workOrderProjectInfoPage = this;

            NavigationService.Navigate(workOrderProductsPage);
        }

        private void OnBtnClickBack(object sender, RoutedEventArgs e)
        {
            workOrderBasicInfoPage.workOrderProductsPage = workOrderProductsPage;
            workOrderBasicInfoPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderBasicInfoPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderBasicInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;
            workOrderBasicInfoPage.workOrderProjectInfoPage = this;

            NavigationService.Navigate(workOrderBasicInfoPage);
        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;

            currentWindow.Close();
        }
    }
}
