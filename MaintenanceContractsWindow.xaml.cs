using _01electronics_library;
using System.Windows.Navigation;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for MaintenanceContractsWindow.xaml
    /// </summary>
    public partial class MaintenanceContractsWindow : NavigationWindow
    {
        public MaintContractsBasicInfoPage maintContractsBasicInfoPage;
        public MaintContractsProjectsPage maintContractsProjectInfoPage;
        public MaintContractsProductsPage maintContractsProductsPage;
        public MaintContractsPaymentAndDeliveryPage maintContractsPaymentAndDeliveryPage;
        public MaintContractsAdditionalInfoPage maintContractsAdditionalInfoPage;
        public MaintContractsUploadFilesPage maintContractsUploadFilesPage;

        public MaintenanceContractsWindow(ref Employee mLoggedInUser, ref MaintenanceContract mMaintContracts, int mViewAddCondition, bool openFilesPage)
        {
            InitializeComponent();

            maintContractsAdditionalInfoPage = new MaintContractsAdditionalInfoPage(ref mLoggedInUser, ref mMaintContracts, mViewAddCondition);
            maintContractsPaymentAndDeliveryPage = new MaintContractsPaymentAndDeliveryPage(ref mLoggedInUser, ref mMaintContracts, mViewAddCondition, ref maintContractsAdditionalInfoPage);
            maintContractsProductsPage = new MaintContractsProductsPage(ref mLoggedInUser, ref mMaintContracts, mViewAddCondition, ref maintContractsPaymentAndDeliveryPage);
            maintContractsProjectInfoPage = new MaintContractsProjectsPage(ref mLoggedInUser, ref mMaintContracts, mViewAddCondition, ref maintContractsProductsPage);
            maintContractsBasicInfoPage = new MaintContractsBasicInfoPage(ref mLoggedInUser, ref mMaintContracts, mViewAddCondition, ref maintContractsProjectInfoPage);
            if (mViewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                maintContractsUploadFilesPage = new MaintContractsUploadFilesPage(ref mLoggedInUser, ref mMaintContracts, mViewAddCondition);
            }
            if (openFilesPage)
            {
                maintContractsUploadFilesPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
                maintContractsUploadFilesPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
                maintContractsUploadFilesPage.maintContractsProductsPage = maintContractsProductsPage;
                maintContractsUploadFilesPage.maintContractsProjectsPage = maintContractsProjectInfoPage;
                maintContractsUploadFilesPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;

                this.NavigationService.Navigate(maintContractsUploadFilesPage);

            }
            else
            {
                maintContractsBasicInfoPage.maintContractsProjectInfoPage = maintContractsProjectInfoPage;
                maintContractsBasicInfoPage.maintContractsProductsPage = maintContractsProductsPage;
                maintContractsBasicInfoPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
                maintContractsBasicInfoPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
                maintContractsBasicInfoPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;

                this.NavigationService.Navigate(maintContractsBasicInfoPage);
            }
        }
    }
}
