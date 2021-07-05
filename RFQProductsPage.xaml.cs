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
    /// Interaction logic for RFQProductsPage.xaml
    /// </summary>
    public partial class RFQProductsPage : Page
    {
        Employee loggedInUser;
        RFQ rfq;
        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private IntegrityChecks IntegrityChecks = new IntegrityChecks();

        private List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> products = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
        private List<COMPANY_WORK_MACROS.BRAND_STRUCT> brands = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();
        private List<COMPANY_WORK_MACROS.MODEL_STRUCT> models = new List<COMPANY_WORK_MACROS.MODEL_STRUCT>();

        private int quantity1;
        private int quantity2;
        private int quantity3;
        private int quantity4;

        private int viewAddCondition;

        ////////////ADD CONSTRUCTOR//////////////
        /////////////////////////////////////////
        public RFQProductsPage(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();
            sqlDatabase = new SQLServer();
            rfq = new RFQ(sqlDatabase);

            InitializeProducts();
            InitializeBrandCombo();

            /////////////VIEW AND ADD CONDITION
            viewAddCondition = 1;
        }
        ////////////VIEW CONSTRUCTOR//////////////////
        //////////////////////////////////////////////
        public RFQProductsPage(ref Employee mLoggedInUser, ref RFQ mRFQ)
        {
            loggedInUser = mLoggedInUser;
            rfq = mRFQ;

            InitializeComponent();

            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();
            sqlDatabase = new SQLServer();

            ConfigureViewRFQUIElements();
            viewAddCondition = 0;
        }
        ////////////////UI ELEMENTS CONFIGURATION///////////
        ////////////////////////////////////////////////////
        private void ConfigureViewRFQUIElements()
        {
            product1TypeCombo.Visibility = Visibility.Hidden;
            product1BrandCombo.Visibility = Visibility.Hidden;
            product1ModelCombo.Visibility = Visibility.Hidden;
            product1QuantityTextBox.Visibility = Visibility.Hidden;

            product2TypeCombo.Visibility = Visibility.Hidden;
            product2BrandCombo.Visibility = Visibility.Hidden;
            product2ModelCombo.Visibility = Visibility.Hidden;
            product2QuantityTextBox.Visibility = Visibility.Hidden;

            product3TypeCombo.Visibility = Visibility.Hidden;
            product3BrandCombo.Visibility = Visibility.Hidden;
            product3ModelCombo.Visibility = Visibility.Hidden;
            product3QuantityTextBox.Visibility = Visibility.Hidden;

            product4TypeCombo.Visibility = Visibility.Hidden;
            product4BrandCombo.Visibility = Visibility.Hidden;
            product4ModelCombo.Visibility = Visibility.Hidden;
            product4QuantityTextBox.Visibility = Visibility.Hidden;
        }
        //////////INITIALIZE FUNCTIONS//////////
        ////////////////////////////////////////
        private void InitializeProducts()
        {
            if (!commonQueriesObject.GetCompanyProducts(ref products))
                return;
            for (int i = 0; i < products.Count(); i++)
            {
                COMPANY_WORK_MACROS.PRODUCT_STRUCT tempType;
                tempType = products[i];
                product1TypeCombo.Items.Add(tempType.typeName);
                product2TypeCombo.Items.Add(tempType.typeName);
                product3TypeCombo.Items.Add(tempType.typeName);
                product4TypeCombo.Items.Add(tempType.typeName);
            }
        }

        private void InitializeBrandCombo()
        {
            if (!commonQueriesObject.GetCompanyBrands(ref brands))
                return;
            for (int i = 0; i < brands.Count(); i++)
            {
                COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand = brands[i];
                product1BrandCombo.Items.Add(tempBrand.brandName);
                product2BrandCombo.Items.Add(tempBrand.brandName);
                product3BrandCombo.Items.Add(tempBrand.brandName);
                product4BrandCombo.Items.Add(tempBrand.brandName);
            }
        }

       //////////////SELECTION CHANGED HANDLERS///////////
       ///////////////////////////////////////////////////

        private void Product1TypeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            product1ModelCombo.Items.Clear();

            if (product1TypeCombo.SelectedItem != null)
            {
                if (product1BrandCombo.SelectedItem != null)
                {
                    COMPANY_WORK_MACROS.PRODUCT_STRUCT tempProduct = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
                    COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand = new COMPANY_WORK_MACROS.BRAND_STRUCT();

                    tempProduct = products[product1TypeCombo.SelectedIndex];
                    tempBrand = brands[product1BrandCombo.SelectedIndex];

                    if (!commonQueriesObject.GetCompanyModels(tempProduct, tempBrand, ref models))
                        return;

                    for (int i = 0; i < models.Count(); i++)
                    {
                        COMPANY_WORK_MACROS.MODEL_STRUCT temp = models[i];
                        product1ModelCombo.Items.Add(temp.modelName);
                    }
                }
                rfq.SetRFQProduct1Type(product1TypeCombo.SelectedIndex + 1, product1TypeCombo.Text);
            }
        }

        private void Product1BrandComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            product1ModelCombo.Items.Clear();
            if (product1BrandCombo.SelectedItem != null)
            {
                if (product1TypeCombo.SelectedItem != null)
                {
                    COMPANY_WORK_MACROS.PRODUCT_STRUCT tempProduct = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
                    COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand = new COMPANY_WORK_MACROS.BRAND_STRUCT();

                    tempProduct = products[product1TypeCombo.SelectedIndex];
                    tempBrand = brands[product1BrandCombo.SelectedIndex];

                    if (!commonQueriesObject.GetCompanyModels(tempProduct, tempBrand, ref models))
                        return;

                    for (int i = 0; i < models.Count(); i++)
                    {
                        COMPANY_WORK_MACROS.MODEL_STRUCT temp = models[i];
                        product1ModelCombo.Items.Add(temp.modelName);
                    }
                }
                rfq.SetRFQProduct1Brand(product1BrandCombo.SelectedIndex + 1, product1BrandCombo.Text);
            }
        }

        private void Product1ModelComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (product1ModelCombo.SelectedItem != null)
                rfq.SetRFQProduct1Model(product1ModelCombo.SelectedIndex + 1, product1ModelCombo.Text);
        }

        private void Product1QuantityTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(product1QuantityTextBox.Text, BASIC_MACROS.PHONE_STRING) && product1QuantityTextBox.Text != "")
            {
                quantity1 = int.Parse(product1QuantityTextBox.Text);
                rfq.SetRFQProduct1Quantity(quantity1);
            }
            else
            {
                // //MessageBox.Show("Invalid Character Enterred");
                quantity1 = 0;
                product1QuantityTextBox.Text = null;
            }
        }

        private void Product2TypeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            product2ModelCombo.Items.Clear();

            if (product2TypeCombo.SelectedItem != null)
            {
                if (product2BrandCombo.SelectedItem != null)
                {
                    COMPANY_WORK_MACROS.PRODUCT_STRUCT tempProduct = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
                    COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand = new COMPANY_WORK_MACROS.BRAND_STRUCT();

                    tempProduct = products[product2TypeCombo.SelectedIndex];
                    tempBrand = brands[product2BrandCombo.SelectedIndex];

                    if (!commonQueriesObject.GetCompanyModels(tempProduct, tempBrand, ref models))
                        return;

                    for (int i = 0; i < models.Count(); i++)
                    {
                        COMPANY_WORK_MACROS.MODEL_STRUCT temp = models[i];
                        product2ModelCombo.Items.Add(temp.modelName);
                    }
                }
                rfq.SetRFQProduct2Type(product2TypeCombo.SelectedIndex + 1, product2TypeCombo.Text);
            }
        }

        private void Product2BrandComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            product2ModelCombo.Items.Clear();
            if (product2BrandCombo.SelectedItem != null)
            {
                if (product2TypeCombo.SelectedItem != null)
                {
                    COMPANY_WORK_MACROS.PRODUCT_STRUCT tempProduct = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
                    COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand = new COMPANY_WORK_MACROS.BRAND_STRUCT();

                    tempProduct = products[product2TypeCombo.SelectedIndex];
                    tempBrand = brands[product2BrandCombo.SelectedIndex];

                    if (!commonQueriesObject.GetCompanyModels(tempProduct, tempBrand, ref models))
                        return;

                    for (int i = 0; i < models.Count(); i++)
                    {
                        COMPANY_WORK_MACROS.MODEL_STRUCT temp = models[i];
                        product2ModelCombo.Items.Add(temp.modelName);
                    }
                }
                rfq.SetRFQProduct2Brand(product2BrandCombo.SelectedIndex + 1, product2BrandCombo.Text);
            }
        }

        private void Product2ModelComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (product2ModelCombo.SelectedItem != null)
                rfq.SetRFQProduct2Model(product2ModelCombo.SelectedIndex + 1, product2ModelCombo.Text);
        }

        private void Product2QuantityTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(product2QuantityTextBox.Text, BASIC_MACROS.PHONE_STRING) && product2QuantityTextBox.Text != "")
            { 
                quantity2 = int.Parse(product2QuantityTextBox.Text);
                rfq.SetRFQProduct2Quantity(quantity2);
            }
            else
            {
                // //MessageBox.Show("Invalid Character Enterred");
                quantity2 = 0;
                product2QuantityTextBox.Text = null;
            }
        }

        private void Product3TypeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            product3ModelCombo.Items.Clear();

            if (product3TypeCombo.SelectedItem != null)
            {
                if (product3BrandCombo.SelectedItem != null)
                {
                    COMPANY_WORK_MACROS.PRODUCT_STRUCT tempProduct = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
                    COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand = new COMPANY_WORK_MACROS.BRAND_STRUCT();

                    tempProduct = products[product3TypeCombo.SelectedIndex];
                    tempBrand = brands[product3BrandCombo.SelectedIndex];

                    if (!commonQueriesObject.GetCompanyModels(tempProduct, tempBrand, ref models))
                        return;

                    for (int i = 0; i < models.Count(); i++)
                    {
                        COMPANY_WORK_MACROS.MODEL_STRUCT temp = models[i];
                        product3ModelCombo.Items.Add(temp.modelName);
                    }
                }
                rfq.SetRFQProduct3Type(product3TypeCombo.SelectedIndex + 1, product3TypeCombo.Text);
            }
        }

        private void Product3BrandComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            product3ModelCombo.Items.Clear();
            if (product3BrandCombo.SelectedItem != null)
            {
                if (product3TypeCombo.SelectedItem != null)
                {
                    COMPANY_WORK_MACROS.PRODUCT_STRUCT tempProduct = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
                    COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand = new COMPANY_WORK_MACROS.BRAND_STRUCT();

                    tempProduct = products[product3TypeCombo.SelectedIndex];
                    tempBrand = brands[product3BrandCombo.SelectedIndex];

                    if (!commonQueriesObject.GetCompanyModels(tempProduct, tempBrand, ref models))
                        return;

                    for (int i = 0; i < models.Count(); i++)
                    {
                        COMPANY_WORK_MACROS.MODEL_STRUCT temp = models[i];
                        product3ModelCombo.Items.Add(temp.modelName);
                    }
                }
                rfq.SetRFQProduct3Brand(product3BrandCombo.SelectedIndex + 1, product3BrandCombo.Text);
            }
        }

        private void Product3ModelComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (product3ModelCombo.SelectedItem != null)
                rfq.SetRFQProduct3Model(product3ModelCombo.SelectedIndex + 1, product3ModelCombo.Text);
        }

        private void Product3QuantityTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(product3QuantityTextBox.Text, BASIC_MACROS.PHONE_STRING) && product3QuantityTextBox.Text != "")
            { 
                quantity3 = int.Parse(product3QuantityTextBox.Text);
                rfq.SetRFQProduct3Quantity(quantity3);
            }
            else
            {
                // //MessageBox.Show("Invalid Character Enterred");
                quantity3 = 0;
                product3QuantityTextBox.Text = null;
            }   
        }

        private void Product4TypeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            product4ModelCombo.Items.Clear();

            if (product4TypeCombo.SelectedItem != null)
            {
                if (product4BrandCombo.SelectedItem != null)
                {
                    COMPANY_WORK_MACROS.PRODUCT_STRUCT tempProduct = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
                    COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand = new COMPANY_WORK_MACROS.BRAND_STRUCT();

                    tempProduct = products[product4TypeCombo.SelectedIndex];
                    tempBrand = brands[product4BrandCombo.SelectedIndex];

                    if (!commonQueriesObject.GetCompanyModels(tempProduct, tempBrand, ref models))
                        return;

                    for (int i = 0; i < models.Count(); i++)
                    {
                        COMPANY_WORK_MACROS.MODEL_STRUCT temp = models[i];
                        product4ModelCombo.Items.Add(temp.modelName);
                    }
                }
                rfq.SetRFQProduct4Type(product4TypeCombo.SelectedIndex + 1, product4TypeCombo.Text);
            }
        }

        private void Product4BrandComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            product4ModelCombo.Items.Clear();
            if (product4BrandCombo.SelectedItem != null)
            {
                if (product4TypeCombo.SelectedItem != null)
                {
                    COMPANY_WORK_MACROS.PRODUCT_STRUCT tempProduct = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
                    COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand = new COMPANY_WORK_MACROS.BRAND_STRUCT();

                    tempProduct = products[product4TypeCombo.SelectedIndex];
                    tempBrand = brands[product4BrandCombo.SelectedIndex];

                    if (!commonQueriesObject.GetCompanyModels(tempProduct, tempBrand, ref models))
                        return;

                    for (int i = 0; i < models.Count(); i++)
                    {
                        COMPANY_WORK_MACROS.MODEL_STRUCT temp = models[i];
                        product4ModelCombo.Items.Add(temp.modelName);
                    }
                }
                rfq.SetRFQProduct4Brand(product4BrandCombo.SelectedIndex + 1, product4BrandCombo.Text);
            }
        }

        private void Product4ModelComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (product4ModelCombo.SelectedItem != null)
                rfq.SetRFQProduct4Model(product4ModelCombo.SelectedIndex + 1, product4ModelCombo.Text);
        }

        private void Product4QuantityTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(product4QuantityTextBox.Text, BASIC_MACROS.PHONE_STRING) && product4QuantityTextBox.Text != "")
            { 
                quantity4 = int.Parse(product4QuantityTextBox.Text);
                rfq.SetRFQProduct4Quantity(quantity4);
            }
            else
            {
                // //MessageBox.Show("Invalid Character Enterred");
                quantity4 = 0;
                product4QuantityTextBox.Text = null;
            }
        }
        ////////////BUTTON CLICKS///////////
        ////////////////////////////////////
        private void OnClickBasicInfo(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition == 0)
            {
                RFQBasicInfoPage basicInfoPage = new RFQBasicInfoPage(ref loggedInUser, ref rfq);
                NavigationService.Navigate(basicInfoPage);
            }
            else
            {
                RFQBasicInfoPage basicInfoPage = new RFQBasicInfoPage(ref loggedInUser);
                NavigationService.Navigate(basicInfoPage);
            }
        }

        private void OnClickProductsInfo(object sender, RoutedEventArgs e)
        {
            //RFQProductsPage productsPage = new RFQProductsPage(ref loggedInUser, ref rfq);
            //NavigationService.Navigate(productsPage);
        }

        private void OnClickAdditionalInfo(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition == 0)
            {
                RFQAdditionalInfoPage additionalInfoPage = new RFQAdditionalInfoPage(ref loggedInUser, ref rfq);
                NavigationService.Navigate(additionalInfoPage);
            }
            else
            {
                RFQAdditionalInfoPage additionalInfoPage = new RFQAdditionalInfoPage(ref loggedInUser);
                NavigationService.Navigate(additionalInfoPage);
            }
        }
    }
}
