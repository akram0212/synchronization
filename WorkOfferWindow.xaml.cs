using _01electronics_library;
using System.Windows.Navigation;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for WorkOfferWindow.xaml
    /// </summary>
    public partial class WorkOfferWindow : NavigationWindow
    {
        public WorkOfferBasicInfoPage workOfferBasicInfoPage;
        public WorkOfferProductsPage workOfferProductsPage;
        public WorkOfferPaymentAndDeliveryPage workOfferPaymentAndDeliveryPage;
        public WorkOfferAdditionalInfoPage workOfferAdditionalInfoPage;
        public WorkOfferUploadFilesPage workOfferUploadFilesPage;

        public WorkOfferWindow(ref Employee mLoggedInUser, ref Quotation mWorkOffer, int mViewAddCondition, bool openFilesPage)
        {
            InitializeComponent();

            workOfferAdditionalInfoPage = new WorkOfferAdditionalInfoPage(ref mLoggedInUser, ref mWorkOffer, mViewAddCondition);
            workOfferPaymentAndDeliveryPage = new WorkOfferPaymentAndDeliveryPage(ref mLoggedInUser, ref mWorkOffer, mViewAddCondition, ref workOfferAdditionalInfoPage);
            workOfferProductsPage = new WorkOfferProductsPage(ref mLoggedInUser, ref mWorkOffer, mViewAddCondition, ref workOfferPaymentAndDeliveryPage);
            workOfferBasicInfoPage = new WorkOfferBasicInfoPage(ref mLoggedInUser, ref mWorkOffer, mViewAddCondition, ref workOfferProductsPage);
            if (mViewAddCondition == COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION)
            {
                workOfferUploadFilesPage = new WorkOfferUploadFilesPage(ref mLoggedInUser, ref mWorkOffer, mViewAddCondition);
            }

            if (openFilesPage)
            {
                workOfferUploadFilesPage.workOfferBasicInfoPage = workOfferBasicInfoPage;
                workOfferUploadFilesPage.workOfferProductsPage = workOfferProductsPage;
                workOfferUploadFilesPage.workOfferPaymentAndDeliveryPage = workOfferPaymentAndDeliveryPage;
                workOfferUploadFilesPage.workOfferAdditionalInfoPage = workOfferAdditionalInfoPage;

                this.NavigationService.Navigate(workOfferUploadFilesPage);

            }
            else
            {
                workOfferBasicInfoPage.workOfferProductsPage = workOfferProductsPage;
                workOfferBasicInfoPage.workOfferAdditionalInfoPage = workOfferAdditionalInfoPage;
                workOfferBasicInfoPage.workOfferPaymentAndDeliveryPage = workOfferPaymentAndDeliveryPage;
                workOfferBasicInfoPage.workOfferUploadFilesPage = workOfferUploadFilesPage;

                this.NavigationService.Navigate(workOfferBasicInfoPage);
            }
        }
    }
}
