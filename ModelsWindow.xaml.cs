using _01electronics_library;
using System.Windows.Navigation;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for ModelsWindow.xaml
    /// </summary>
    public partial class ModelsWindow : NavigationWindow
    {
        public ModelBasicInfoPage modelBasicInfoPage;
        public ModelUpsSpecsPage modelUpsSpecsPage;
        public ModelAdditionalInfoPage modelAdditionalInfoPage;
        public ModelUploadFilesPage modelUploadFilesPage;
        public ModelsWindow(ref Employee mLoggedInUser, ref Product mPrduct, int mViewAddCondition, bool openFilesPage)
        {
            InitializeComponent();

            modelBasicInfoPage = new ModelBasicInfoPage(ref mLoggedInUser, ref mPrduct, mViewAddCondition);
            modelUpsSpecsPage = new ModelUpsSpecsPage(ref mLoggedInUser, ref mPrduct, mViewAddCondition);
            modelAdditionalInfoPage = new ModelAdditionalInfoPage(ref mLoggedInUser, ref mPrduct, mViewAddCondition);
            modelUploadFilesPage = new ModelUploadFilesPage(ref mLoggedInUser, ref mPrduct, mViewAddCondition);

            if (openFilesPage)
            {
                modelUploadFilesPage.modelBasicInfoPage = modelBasicInfoPage;
                modelUploadFilesPage.modelUpsSpecsPage = modelUpsSpecsPage;
                modelUploadFilesPage.modelAdditionalInfoPage = modelAdditionalInfoPage;

                this.NavigationService.Navigate(modelUploadFilesPage);
            }
            else
            {
                modelBasicInfoPage.modelUpsSpecsPage = modelUpsSpecsPage;
                modelBasicInfoPage.modelAdditionalInfoPage = modelAdditionalInfoPage;
                modelBasicInfoPage.modelUploadFilesPage = modelUploadFilesPage;

                this.NavigationService.Navigate(modelBasicInfoPage);
            }
        }
    }
}
