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
        public ModelUpsSpecsPage modelUpsSpecsPage;
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
                cancelButton.IsEnabled = false;
                finishButton.IsEnabled = false;
                nextButton.IsEnabled = true;
            }

            if (viewAddCondition != COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                nextButton.IsEnabled = false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        //////////BUTTON CLICKED/////////////
        /////////////////////////////////////////////////////////////////////////////////////////////

        private void OnBtnClickFinish(object sender, RoutedEventArgs e)
        {
            //YOUR MESSAGE MUST BE SPECIFIC
            //YOU SHALL CHECK UI ELEMENTS IN ORDER AND THEN WRITE A MESSAGE IF ERROR IS TO BE FOUND
           // if (product.GetSalesPersonId() == 0)
           //     System.Windows.Forms.MessageBox.Show("Sales person is not specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
           // else if (product.GetAssigneeId() == 0)
           //     System.Windows.Forms.MessageBox.Show("Assignee is not specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
           // else if (product.GetAddressSerial() == 0)
           //     System.Windows.Forms.MessageBox.Show("Company Address is not specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
           // else if (product.GetContactId() == 0)
           //     System.Windows.Forms.MessageBox.Show("Contact is not specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
           // else if (product.GetproductProduct1TypeId() != 0 && product.GetproductProduct1Quantity() == 0)
           //     System.Windows.Forms.MessageBox.Show("Quantity is not specified for product 1!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
           // else if (product.GetproductProduct2TypeId() != 0 && product.GetproductProduct2Quantity() == 0)
           //     System.Windows.Forms.MessageBox.Show("Quantity is not specified for product 2!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
           // else if (product.GetproductProduct3TypeId() != 0 && product.GetproductProduct3Quantity() == 0)
           //     System.Windows.Forms.MessageBox.Show("Quantity is not specified for product 3!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
           // else if (product.GetproductProduct4TypeId() != 0 && product.GetproductProduct4Quantity() == 0)
           //     System.Windows.Forms.MessageBox.Show("Quantity is not specified for product 4!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
           // else if (product.GetproductContractTypeId() == 0)
           //     System.Windows.Forms.MessageBox.Show("Contract type is not specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
           // else if (product.GetproductStatusId() == 0)
           //     System.Windows.Forms.MessageBox.Show("Status ID can't be 0 for an product! Contact your system administrator!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
           // else
           // {
           //     if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
           //     {
           //         //if (!product.IssueNewproduct())
           //         //    return;
           //
           //         if (viewAddCondition != COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
           //         {
           //             viewAddCondition = COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION;
           //
           //             ModelsWindow viewproduct = new ModelsWindow(ref loggedInUser, ref product, viewAddCondition, true);
           //
           //             viewproduct.Show();
           //         }
           //
           //     }
           //
           //     NavigationWindow currentWindow = (NavigationWindow)this.Parent;
           //     currentWindow.Close();
           // }
        }

        private void OnBtnClickNext(object sender, RoutedEventArgs e)
        {
            modelUploadFilesPage.modelBasicInfoPage = modelBasicInfoPage;
            modelUploadFilesPage.modelUpsSpecsPage = modelUpsSpecsPage;
            modelUploadFilesPage.modelAdditionalInfoPage = this;

            NavigationService.Navigate(modelUploadFilesPage);
        }

        private void OnBtnClickBack(object sender, RoutedEventArgs e)
        {

            modelUpsSpecsPage.modelAdditionalInfoPage = this;
            modelUpsSpecsPage.modelBasicInfoPage = modelBasicInfoPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                modelUpsSpecsPage.modelUploadFilesPage = modelUploadFilesPage;

            NavigationService.Navigate(modelUpsSpecsPage);

            //modelBasicInfoPage.modelAdditionalInfoPage = this;
            //
            //if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            //    modelBasicInfoPage.modelUploadFilesPage = modelUploadFilesPage;
            //
            //NavigationService.Navigate(modelBasicInfoPage);
        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;
            currentWindow.Close();
        }
        private void OnBtnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            modelBasicInfoPage.modelUpsSpecsPage = modelUpsSpecsPage;
            modelBasicInfoPage.modelAdditionalInfoPage = this;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                modelBasicInfoPage.modelUploadFilesPage = modelUploadFilesPage;

            NavigationService.Navigate(modelBasicInfoPage);
        }
        private void OnBtnClickUpsSpecs(object sender, MouseButtonEventArgs e)
        {
            modelUpsSpecsPage.modelAdditionalInfoPage = this;
            modelUpsSpecsPage.modelBasicInfoPage = modelBasicInfoPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                modelUpsSpecsPage.modelUploadFilesPage = modelUploadFilesPage;

            NavigationService.Navigate(modelUpsSpecsPage);
        }

        private void OnBtnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                modelUploadFilesPage.modelBasicInfoPage = modelBasicInfoPage; 
                modelUploadFilesPage.modelUpsSpecsPage = modelUpsSpecsPage;
                modelUploadFilesPage.modelAdditionalInfoPage = this;

                NavigationService.Navigate(modelUploadFilesPage);
            }
        }

        private void OnClickStandardFeaturesImage(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
