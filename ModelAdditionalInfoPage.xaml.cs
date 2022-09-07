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
    /// Interaction logic for ModelAdditionalInfoPage.xaml
    /// </summary>
    public partial class ModelAdditionalInfoPage : Page
    {

        Employee loggedInUser;
        Product product;
        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private int viewAddCondition;

        public ModelBasicInfoPage modelBasicInfoPage;
        public ModelUploadFilesPage modelUploadFilesPage;

        public ModelAdditionalInfoPage(ref Employee mLoggedInUser, ref Product mPrduct, int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            sqlDatabase = new SQLServer();

            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            product = mPrduct;
            InitializeComponent();

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
            {

            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {

            }
        }

        private void OnBtnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            modelBasicInfoPage.modelAdditionalInfoPage = this;

            if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
                modelBasicInfoPage.modelUploadFilesPage = modelUploadFilesPage;

            NavigationService.Navigate(modelBasicInfoPage);
        }

        private void OnBtnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
            {
                modelUploadFilesPage.modelBasicInfoPage = modelBasicInfoPage;
                modelUploadFilesPage.modelAdditionalInfoPage = this;

                NavigationService.Navigate(modelUploadFilesPage);
            }
        }
    }
}
