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
    /// Interaction logic for RFQWindow.xaml
    /// </summary>
    public partial class RFQWindow : NavigationWindow
    {
        public RFQBasicInfoPage rfqBasicInfoPage;
        public RFQProductsPage rfqProductsPage;
        public RFQAdditionalInfoPage rfqAdditionalInfoPage;
        public RFQUploadFilesPage rfqUploadFilesPage;

        public RFQWindow(ref Employee mLoggedInUser, ref RFQ mRFQ, int mViewAddCondition, bool openFilesPage)
        {
            InitializeComponent();

            rfqBasicInfoPage = new RFQBasicInfoPage(ref mLoggedInUser, ref mRFQ, mViewAddCondition);
            rfqProductsPage = new RFQProductsPage(ref mLoggedInUser, ref mRFQ, mViewAddCondition);
            rfqAdditionalInfoPage = new RFQAdditionalInfoPage(ref mLoggedInUser, ref mRFQ, mViewAddCondition);
            rfqUploadFilesPage = new RFQUploadFilesPage(ref mLoggedInUser, ref mRFQ, mViewAddCondition);

            if (openFilesPage)
            {
                rfqUploadFilesPage.rfqBasicInfoPage = rfqBasicInfoPage;
                rfqUploadFilesPage.rfqProductsPage = rfqProductsPage;
                rfqUploadFilesPage.rfqAdditionalInfoPage = rfqAdditionalInfoPage;

                this.NavigationService.Navigate(rfqUploadFilesPage);
            }
            else
            {
                rfqBasicInfoPage.rfqProductsPage = rfqProductsPage;
                rfqBasicInfoPage.rfqAdditionalInfoPage = rfqAdditionalInfoPage;
                rfqBasicInfoPage.rfqUploadFilesPage = rfqUploadFilesPage;

                this.NavigationService.Navigate(rfqBasicInfoPage);
            }
        }

    }
}
