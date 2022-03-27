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
using System.Windows.Shapes;
using System.Windows.Navigation;
using _01electronics_library;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for WorkOrderWindow.xaml
    /// </summary>
    public partial class WorkOrderWindow : NavigationWindow
    {
        public WorkOrderBasicInfoPage workOrderBasicInfoPage;
        public WorkOrderProjectInfoPage workOrderProjectInfoPage;
        public WorkOrderProductsPage workOrderProductsPage;
        public WorkOrderPaymentAndDeliveryPage workOrderPaymentAndDeliveryPage;
        public WorkOrderAdditionalInfoPage workOrderAdditionalInfoPage;
        public WorkOrderUploadFilesPage workOrderUploadFilesPage;

        public WorkOrderWindow(ref Employee mLoggedInUser, ref WorkOrder mWorkOrder, int mViewAddCondition, bool openFilesPage)
        {
            InitializeComponent();

            workOrderAdditionalInfoPage = new WorkOrderAdditionalInfoPage(ref mLoggedInUser, ref mWorkOrder, mViewAddCondition);
            workOrderPaymentAndDeliveryPage = new WorkOrderPaymentAndDeliveryPage(ref mLoggedInUser, ref mWorkOrder, mViewAddCondition, ref workOrderAdditionalInfoPage);
            workOrderProductsPage = new WorkOrderProductsPage(ref mLoggedInUser, ref mWorkOrder, mViewAddCondition, ref workOrderPaymentAndDeliveryPage);
            workOrderProjectInfoPage = new WorkOrderProjectInfoPage(ref mLoggedInUser, ref mWorkOrder, mViewAddCondition, ref workOrderProductsPage);
            workOrderBasicInfoPage = new WorkOrderBasicInfoPage(ref mLoggedInUser, ref mWorkOrder, mViewAddCondition, ref workOrderProjectInfoPage, ref workOrderProductsPage);
            if (openFilesPage)
            {
                workOrderUploadFilesPage = new WorkOrderUploadFilesPage(ref mLoggedInUser, ref mWorkOrder, mViewAddCondition);
            }
            if (openFilesPage)
            {
                
                workOrderUploadFilesPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
                workOrderUploadFilesPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
                workOrderUploadFilesPage.workOrderProductsPage = workOrderProjectInfoPage.workOrderProductsPage;
                workOrderUploadFilesPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
                workOrderUploadFilesPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;

                this.NavigationService.Navigate(workOrderUploadFilesPage);

            }
            else
            {
                workOrderBasicInfoPage.workOrderProductsPage = workOrderProjectInfoPage.workOrderProductsPage;
                workOrderBasicInfoPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
                workOrderBasicInfoPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
                workOrderBasicInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

                this.NavigationService.Navigate(workOrderBasicInfoPage);
            }
        }
    }
}
