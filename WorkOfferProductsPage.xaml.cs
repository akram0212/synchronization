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
    /// Interaction logic for WorkOfferProductsPage.xaml
    /// </summary>
    public partial class WorkOfferProductsPage : Page
    {
        Employee loggedInUser;
        WorkOffer workOffer;

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;

        private List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> products = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
        private List<COMPANY_WORK_MACROS.BRAND_STRUCT> brands = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();
        private List<COMPANY_WORK_MACROS.MODEL_STRUCT> models = new List<COMPANY_WORK_MACROS.MODEL_STRUCT>();

        private int viewAddCondition;
        private int totalPrice;

        ///////////ADD WORKOFFER CONSTRUCTOR///////////////
        ///////////////////////////////////////////////////
        public WorkOfferProductsPage(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;

            InitializeComponent();
        
            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            workOffer = new WorkOffer(sqlDatabase);

            viewAddCondition = 1;

            ConfigureAddUIElements();
        }
        //////////////VIEW WORKOFFER CONSTRUCTOR//////////
        //////////////////////////////////////////////////
        public WorkOfferProductsPage(ref Employee mLoggedInUser, ref WorkOffer mWorkOffer)
        {
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            workOffer = new WorkOffer(sqlDatabase);

            viewAddCondition = 1;

            ConfigureViewUIElements();
        }
        //////////////CONFIGURE UI ELEMENTS////////////////
        //////////////////////////////////////////////////

        private void ConfigureAddUIElements()
        {
            product1TypeLabel.Visibility = Visibility.Collapsed;
            product1BrandLabel.Visibility = Visibility.Collapsed;
            product1ModelLabel.Visibility = Visibility.Collapsed;
            product1QuantityLabel.Visibility = Visibility.Collapsed;

            product2TypeLabel.Visibility = Visibility.Collapsed;
            product2BrandLabel.Visibility = Visibility.Collapsed;
            product2ModelLabel.Visibility = Visibility.Collapsed;
            product2QuantityLabel.Visibility = Visibility.Collapsed;

            product3TypeLabel.Visibility = Visibility.Collapsed;
            product3BrandLabel.Visibility = Visibility.Collapsed;
            product3ModelLabel.Visibility = Visibility.Collapsed;
            product3QuantityLabel.Visibility = Visibility.Collapsed;

            product4TypeLabel.Visibility = Visibility.Collapsed;
            product4BrandLabel.Visibility = Visibility.Collapsed;
            product4ModelLabel.Visibility = Visibility.Collapsed;
            product4QuantityLabel.Visibility = Visibility.Collapsed;

            product1TypeCombo.Visibility = Visibility.Visible;
            product1BrandCombo.Visibility = Visibility.Visible;
            product1ModelCombo.Visibility = Visibility.Visible;
            product1QuantityTextBox.Visibility = Visibility.Visible;

            product2TypeCombo.Visibility = Visibility.Visible;
            product2BrandCombo.Visibility = Visibility.Visible;
            product2ModelCombo.Visibility = Visibility.Visible;
            product2QuantityTextBox.Visibility = Visibility.Visible;

            product3TypeCombo.Visibility = Visibility.Visible;
            product3BrandCombo.Visibility = Visibility.Visible;
            product3ModelCombo.Visibility = Visibility.Visible;
            product3QuantityTextBox.Visibility = Visibility.Visible;

            product4TypeCombo.Visibility = Visibility.Visible;
            product4BrandCombo.Visibility = Visibility.Visible;
            product4ModelCombo.Visibility = Visibility.Visible;
            product4QuantityTextBox.Visibility = Visibility.Visible;

            product2TypeCombo.IsEnabled = false;
            product2BrandCombo.IsEnabled = false;
            product2ModelCombo.IsEnabled = false;
            product2QuantityTextBox.IsEnabled = false;

            product3TypeCombo.IsEnabled = false;
            product3BrandCombo.IsEnabled = false;
            product3ModelCombo.IsEnabled = false;
            product3QuantityTextBox.IsEnabled = false;

            product4TypeCombo.IsEnabled = false;
            product4BrandCombo.IsEnabled = false;
            product4ModelCombo.IsEnabled = false;
            product4QuantityTextBox.IsEnabled = false;
        }

        private void ConfigureViewUIElements()
        {
            product1TypeCombo.Visibility = Visibility.Collapsed;
            product1BrandCombo.Visibility = Visibility.Collapsed;
            product1ModelCombo.Visibility = Visibility.Collapsed;
            product1QuantityTextBox.Visibility = Visibility.Collapsed;

            product2TypeCombo.Visibility = Visibility.Collapsed;
            product2BrandCombo.Visibility = Visibility.Collapsed;
            product2ModelCombo.Visibility = Visibility.Collapsed;
            product2QuantityTextBox.Visibility = Visibility.Collapsed;

            product3TypeCombo.Visibility = Visibility.Collapsed;
            product3BrandCombo.Visibility = Visibility.Collapsed;
            product3ModelCombo.Visibility = Visibility.Collapsed;
            product3QuantityTextBox.Visibility = Visibility.Collapsed;

            product4TypeCombo.Visibility = Visibility.Collapsed;
            product4BrandCombo.Visibility = Visibility.Collapsed;
            product4ModelCombo.Visibility = Visibility.Collapsed;
            product4QuantityTextBox.Visibility = Visibility.Collapsed;
        }
        ///////////////SELECTION CHANGED HANDLERS///////////
        ////////////////////////////////////////////////////
        private void Product1TypeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void Product1BrandComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
        private void Product1ModelComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }
        private void Product1QuantityTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void Product2TypeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Product2BrandComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void Product2ModelComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void Product2QuantityTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void Product3TypeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Product3BrandComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void Product3ModelComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void Product3QuantityTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void Product4TypeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Product4BrandComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void Product4ModelComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void Product4QuantityTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {

        }
        ///////////////CHECKBOX HANDLERS////////////////////
        ///////////////////////////////////////////////////
        private void Product2CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            product2TypeCombo.IsEnabled = true;
            product2ModelCombo.IsEnabled = true;
            product2ModelCombo.IsEnabled = true;
            product2QuantityTextBox.IsEnabled = true;
        }

        private void Product2CheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            product2TypeCombo.IsEnabled = false;
            product2ModelCombo.IsEnabled = false;
            product2ModelCombo.IsEnabled = false;
            product2QuantityTextBox.IsEnabled = false;
        }
        private void Product3CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            product3TypeCombo.IsEnabled = true;
            product3ModelCombo.IsEnabled = true;
            product3ModelCombo.IsEnabled = true;
            product3QuantityTextBox.IsEnabled = true;
        }

        private void Product3CheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            product3TypeCombo.IsEnabled = false;
            product3ModelCombo.IsEnabled = false;
            product3ModelCombo.IsEnabled = false;
            product3QuantityTextBox.IsEnabled = false;
        }
        private void Product4CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            product4TypeCombo.IsEnabled = true;
            product4ModelCombo.IsEnabled = true;
            product4ModelCombo.IsEnabled = true;
            product4QuantityTextBox.IsEnabled = true;
        }

        private void Product4CheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            product4TypeCombo.IsEnabled = false;
            product4ModelCombo.IsEnabled = false;
            product4ModelCombo.IsEnabled = false;
            product4QuantityTextBox.IsEnabled = false;
        }
        //////////BUTTON CLICK HANDLERS//////////////////
        /////////////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            /*
            if (viewAddCondition == 0)
            {
                WorkOfferProductsPage workOfferProductsPage = new WorkOfferProductsPage(ref loggedInUser, ref workOffer);
                NavigationService.Navigate(workOfferProductsPage);
            }
            else
            {
                WorkOfferProductsPage workOfferProductsPage = new WorkOfferProductsPage(ref loggedInUser);
                NavigationService.Navigate(workOfferProductsPage);
            }*/
        }


        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            /*
            if (viewAddCondition == 0)
            {
                RFQAdditionalInfoPage additionalInfoPage = new RFQAdditionalInfoPage(ref loggedInUser, ref rfq);
                NavigationService.Navigate(additionalInfoPage);
            }
            else
            {
                RFQAdditionalInfoPage additionalInfoPage = new RFQAdditionalInfoPage(ref loggedInUser);
                NavigationService.Navigate(additionalInfoPage);
            }*/
        }

        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
