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
    /// Interaction logic for ModelBasicInfoPage.xaml
    /// </summary>
    public partial class ModelBasicInfoPage : Page
    {
        Employee loggedInUser;
        Product product;
        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private int viewAddCondition;

        public ModelAdditionalInfoPage modelAdditionalInfoPage;
        public ModelUpsSpecsPage modelUpsSpecsPage;
        public ModelUploadFilesPage modelUploadFilesPage;

        public ModelBasicInfoPage(ref Employee mLoggedInUser, ref Product mPrduct, int mViewAddCondition)
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
               
                NameTextBox.Visibility = Visibility.Visible;
                summeryPointsTextBox.Visibility = Visibility.Visible;

            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {

                NameLabel.Visibility = Visibility.Visible;
                summeryPointsTextBlock.Visibility = Visibility.Visible;
                InitializeInfo();

            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //////////BUTTON CLICKED///////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnClickNextButton(object sender, RoutedEventArgs e)
        {
            product.SetModelName(NameTextBox.Text.ToString());
            product.SetsummaryPoints(summeryPointsTextBox.Text.ToString());
            modelUpsSpecsPage.modelBasicInfoPage = this;
            modelUpsSpecsPage.modelAdditionalInfoPage = modelAdditionalInfoPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                modelUpsSpecsPage.modelUploadFilesPage = modelUploadFilesPage;

            NavigationService.Navigate(modelUpsSpecsPage);
            
            //modelAdditionalInfoPage.modelBasicInfoPage = this;
            //
            //if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            //    modelAdditionalInfoPage.modelUploadFilesPage = modelUploadFilesPage;
            //
            //NavigationService.Navigate(modelAdditionalInfoPage);
        }
        private void OnBtnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            modelAdditionalInfoPage.modelBasicInfoPage = this;
            modelAdditionalInfoPage.modelUpsSpecsPage = modelUpsSpecsPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                modelAdditionalInfoPage.modelUploadFilesPage = modelUploadFilesPage;

            NavigationService.Navigate(modelAdditionalInfoPage);
        }
        private void OnBtnClickUpsSpecs(object sender, MouseButtonEventArgs e)
        {
            modelUpsSpecsPage.modelBasicInfoPage = this;
            modelUpsSpecsPage.modelAdditionalInfoPage = modelAdditionalInfoPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                modelUpsSpecsPage.modelUploadFilesPage = modelUploadFilesPage;

            NavigationService.Navigate(modelUpsSpecsPage);
        }

        private void OnBtnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                modelUploadFilesPage.modelBasicInfoPage = this;
                modelUploadFilesPage.modelUpsSpecsPage = modelUpsSpecsPage;
                modelUploadFilesPage.modelAdditionalInfoPage = modelAdditionalInfoPage;

                NavigationService.Navigate(modelUploadFilesPage);
            }
        }
        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;
            currentWindow.Close();
        }

        private void OnDropUploadFilesStackPanel(object sender, DragEventArgs e)
        {

        }
        private void InitializeInfo()
        {
            
        }


    }
}
