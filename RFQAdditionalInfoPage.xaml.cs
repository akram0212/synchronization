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
using _01electronics_erp;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for RFQAdditionalInfoPage.xaml
    /// </summary>
    public partial class RFQAdditionalInfoPage : Page
    {
        Employee loggedInUser;
        RFQ rfq;
        public RFQAdditionalInfoPage(ref Employee mLoggedInUser, ref RFQ mRFQ)
        {
            loggedInUser = mLoggedInUser;
            rfq = mRFQ;

            InitializeComponent();
        }

        private void OnClickBasicInfo(object sender, RoutedEventArgs e)
        {
            RFQBasicInfoPage basicInfoPage = new RFQBasicInfoPage(ref loggedInUser, ref rfq);
            NavigationService.Navigate(basicInfoPage);
        }

        private void OnClickProductsInfo(object sender, RoutedEventArgs e)
        {
            RFQProductsPage productsPage = new RFQProductsPage(ref loggedInUser, ref rfq);
            NavigationService.Navigate(productsPage);
        }

        private void OnClickAdditionalInfo(object sender, RoutedEventArgs e)
        {
            RFQAdditionalInfoPage additionalInfoPage = new RFQAdditionalInfoPage(ref loggedInUser, ref rfq);
            NavigationService.Navigate(additionalInfoPage);
        }
    }
}
