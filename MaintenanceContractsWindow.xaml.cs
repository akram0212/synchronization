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
using System.Windows.Shapes;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for MaintenanceContractsWindow.xaml
    /// </summary>
    public partial class MaintenanceContractsWindow : Window
    {
        public MaintContractsBasicInfoPage MaintContractsBasicInfoPage;
        public MaintContractsProductsPage MaintContractsProductsPage;
        public MaintContractsPaymentAndDeliveryPage MaintContractsPaymentAndDeliveryPage;
        public MaintContractsAdditionalInfoPage MaintContractsAdditionalInfoPage;
        public MaintContractsUploadFilesPage MaintContractsUploadFilesPage;

        public MaintenanceContractsWindow(ref Employee mLoggedInUser, ref MaintenanceContract mMaintContracts, int mViewAddCondition, bool openFilesPage)
        {
            InitializeComponent();

        //    MaintContractsAdditionalInfoPage = new MaintContractsAdditionalInfoPage(ref mLoggedInUser, ref mMaintContracts, mViewAddCondition);
        //    MaintContractsPaymentAndDeliveryPage = new MaintContractsPaymentAndDeliveryPage(ref mLoggedInUser, ref mMaintContracts, mViewAddCondition, ref MaintContractsAdditionalInfoPage);
        //    MaintContractsProductsPage = new MaintContractsProductsPage(ref mLoggedInUser, ref mMaintContracts, mViewAddCondition, ref MaintContractsPaymentAndDeliveryPage);
        //    MaintContractsBasicInfoPage = new MaintContractsBasicInfoPage(ref mLoggedInUser, ref mMaintContracts, mViewAddCondition, ref MaintContractsProductsPage);
        //    MaintContractsUploadFilesPage = new MaintContractsUploadFilesPage(ref mLoggedInUser, ref mMaintContracts, mViewAddCondition);

        //    if (openFilesPage)
        //    {
        //        MaintContractsUploadFilesPage.MaintContractsBasicInfoPage = MaintContractsBasicInfoPage;
        //        MaintContractsUploadFilesPage.MaintContractsProductsPage = MaintContractsProductsPage;
        //        MaintContractsUploadFilesPage.MaintContractsPaymentAndDeliveryPage = MaintContractsPaymentAndDeliveryPage;
        //        MaintContractsUploadFilesPage.MaintContractsAdditionalInfoPage = MaintContractsAdditionalInfoPage;

        //        this.NavigationService.Navigate(MaintContractsUploadFilesPage);

        //    }
        //    else
        //    {
        //        MaintContractsBasicInfoPage.MaintContractsProductsPage = MaintContractsProductsPage;
        //        MaintContractsBasicInfoPage.MaintContractsAdditionalInfoPage = MaintContractsAdditionalInfoPage;
        //        MaintContractsBasicInfoPage.MaintContractsPaymentAndDeliveryPage = MaintContractsPaymentAndDeliveryPage;
        //        MaintContractsBasicInfoPage.MaintContractsUploadFilesPage = MaintContractsUploadFilesPage;

        //        this.NavigationService.Navigate(MaintContractsBasicInfoPage);
        //    }
        }
    }
}
