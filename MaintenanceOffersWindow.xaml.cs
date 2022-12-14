using _01electronics_library;
using System.Windows.Navigation;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for MaintenanceOffersWindow.xaml
    /// </summary>
    public partial class MaintenanceOffersWindow : NavigationWindow
    {
        public MaintOffersBasicInfoPage maintOffersBasicInfoPage;
        public MaintOffersProductsPage maintOffersProductsPage;
        public MaintOffersPaymentAndDeliveryPage maintOffersPaymentAndDeliveryPage;
        public MaintOffersAdditionalInfoPage maintOffersAdditionalInfoPage;
        public MaintOffersUploadFilesPage maintOffersUploadFilesPage;

        public MaintenanceOffersWindow(ref Employee mLoggedInUser, ref MaintenanceOffer mMaintOffers, int mViewAddCondition, bool openFilesPage)
        {
            InitializeComponent();

            maintOffersAdditionalInfoPage = new MaintOffersAdditionalInfoPage(ref mLoggedInUser, ref mMaintOffers, mViewAddCondition);
            maintOffersPaymentAndDeliveryPage = new MaintOffersPaymentAndDeliveryPage(ref mLoggedInUser, ref mMaintOffers, mViewAddCondition, ref maintOffersAdditionalInfoPage);
            maintOffersProductsPage = new MaintOffersProductsPage(ref mLoggedInUser, ref mMaintOffers, mViewAddCondition, ref maintOffersPaymentAndDeliveryPage);
            maintOffersBasicInfoPage = new MaintOffersBasicInfoPage(ref mLoggedInUser, ref mMaintOffers, mViewAddCondition, ref maintOffersProductsPage);
            if (mViewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                maintOffersUploadFilesPage = new MaintOffersUploadFilesPage(ref mLoggedInUser, ref mMaintOffers, mViewAddCondition);
            }
            if (openFilesPage)
            {
                maintOffersUploadFilesPage.maintOffersBasicInfoPage = maintOffersBasicInfoPage;
                maintOffersUploadFilesPage.maintOffersProductsPage = maintOffersProductsPage;
                maintOffersUploadFilesPage.maintOffersPaymentAndDeliveryPage = maintOffersPaymentAndDeliveryPage;
                maintOffersUploadFilesPage.maintOffersAdditionalInfoPage = maintOffersAdditionalInfoPage;

                this.NavigationService.Navigate(maintOffersUploadFilesPage);

            }
            else
            {
                maintOffersBasicInfoPage.maintOffersProductsPage = maintOffersProductsPage;
                maintOffersBasicInfoPage.maintOffersAdditionalInfoPage = maintOffersAdditionalInfoPage;
                maintOffersBasicInfoPage.maintOffersPaymentAndDeliveryPage = maintOffersPaymentAndDeliveryPage;
                maintOffersBasicInfoPage.maintOffersUploadFilesPage = maintOffersUploadFilesPage;

                this.NavigationService.Navigate(maintOffersBasicInfoPage);
            }
        }

    }
}
