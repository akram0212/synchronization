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
        private IntegrityChecks IntegrityChecks = new IntegrityChecks();

        private List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> products = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
        private List<COMPANY_WORK_MACROS.BRAND_STRUCT> brands = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();
        private List<COMPANY_WORK_MACROS.MODEL_STRUCT> models = new List<COMPANY_WORK_MACROS.MODEL_STRUCT>();

        private int viewAddCondition;
        private int totalPrice;
        private int numberOfProductsAdded;
        private int typeId;
        private int brandId;
        private int modelId;
        private int quantity;
        private decimal priceQuantity;

        ///////////ADD WORKOFFER CONSTRUCTOR///////////////
        ///////////////////////////////////////////////////
        public WorkOfferProductsPage(ref Employee mLoggedInUser, ref WorkOffer mWorkOffer, int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;
            InitializeComponent();

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            workOffer = new WorkOffer(sqlDatabase);
            workOffer = mWorkOffer;

            numberOfProductsAdded = 0;

            //////////////////////////////
            ///ADD CONDITION
            //////////////////////////////
            if (viewAddCondition == 1)
            {
                InitializeProducts();
                InitializeBrandCombo();
                SetUpPageUIElements();
               
            }
            /////////////////////////////
            ///VIEW CONDITION
            /////////////////////////////
            else if (viewAddCondition == 0)
            {
                SetUpPageUIElements();
                SetTypeLabels();
                SetBrandLabels();
                SetModelLabels();
                SetQuantityTextBoxes();
                SetPriceTextBoxes();
            }
            //////////////////////////////
            ///REVISE
            //////////////////////////////
            else if (viewAddCondition == 2)
            {
                InitializeProducts();
                InitializeBrandCombo();
                SetUpPageUIElements();
                SetTypeComboBoxes();
                SetBrandComboBoxes();
                SetModelComboBoxes();
                SetQuantityTextBoxes();
                SetPriceTextBoxes();
            }
            /////////////////////////////
            ///RESOLVE RFQ
            /////////////////////////////
            else
            {
                InitializeProducts();
                InitializeBrandCombo();
                SetUpPageUIElements();
                SetTypeComboBoxesResolve();
                SetBrandComboBoxesResolve();
                SetModelComboBoxesResolve();
                SetQuantityTextBoxesResolve();
                SetPriceTextBoxes();
            }


        }

        //////////////CONFIGURE UI ELEMENTS////////////////
        //////////////////////////////////////////////////

        //////////////////////////////////////////////////
        ///INITIALIZATION FUNCTIONS
        //////////////////////////////////////////////////

        private void InitializeProducts()
        {
            if (!commonQueriesObject.GetCompanyProducts(ref products))
                return;
        }

        private void InitializeBrandCombo()
        {
            if (!commonQueriesObject.GetCompanyBrands(ref brands))
                return;
        }

        //////////////////////////////////////////////////
        ///SET FUNCTIONS
        //////////////////////////////////////////////////

        private void SetTypeComboBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentTypeWrapPanel = (WrapPanel)currentProductGrid.Children[1];
                ComboBox CurrentTypeComboBox = (ComboBox)currentTypeWrapPanel.Children[1];
                CurrentTypeComboBox.Text = workOffer.GetOfferProductType(i + 1);
            }
        }
        private void SetTypeComboBoxesResolve()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentTypeWrapPanel = (WrapPanel)currentProductGrid.Children[1];
                ComboBox CurrentTypeComboBox = (ComboBox)currentTypeWrapPanel.Children[1];
                CurrentTypeComboBox.Text = workOffer.GetRFQProductType(i + 1);
            }
        }
        private void SetBrandComboBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentBrandWrapPanel = (WrapPanel)currentProductGrid.Children[2];
                ComboBox currentBrandComboBox = (ComboBox)currentBrandWrapPanel.Children[1];
                currentBrandComboBox.Text = workOffer.GetOfferProductBrand(i + 1);
            }
        }

        private void SetBrandComboBoxesResolve()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentBrandWrapPanel = (WrapPanel)currentProductGrid.Children[2];
                ComboBox currentBrandComboBox = (ComboBox)currentBrandWrapPanel.Children[1];
                currentBrandComboBox.Text = workOffer.GetRFQProductBrand(i + 1);
            }
        }
        private void SetModelComboBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentModelWrapPanel = (WrapPanel)currentProductGrid.Children[3];
                ComboBox currentModelComboBox = (ComboBox)currentModelWrapPanel.Children[1];
                currentModelComboBox.SelectedItem = workOffer.GetOfferProductModel(i + 1);
            }
        }

        private void SetModelComboBoxesResolve()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentModelWrapPanel = (WrapPanel)currentProductGrid.Children[3];
                ComboBox currentModelComboBox = (ComboBox)currentModelWrapPanel.Children[1];
                currentModelComboBox.SelectedItem = workOffer.GetRFQProductModel(i + 1);
            }
        }
        private void SetTypeLabels()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentTypeWrapPanel = (WrapPanel)currentProductGrid.Children[1];
                Label currentTypeLabelValue = (Label)currentTypeWrapPanel.Children[1];
                currentTypeLabelValue.Content = workOffer.GetOfferProductType(i + 1);
            }
        }

        private void SetBrandLabels()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentBrandWrapPanel = (WrapPanel)currentProductGrid.Children[2];
                Label currentBrandLabelValue = (Label)currentBrandWrapPanel.Children[1];
                currentBrandLabelValue.Content = workOffer.GetOfferProductBrand(i + 1);
            }
        }

        private void SetModelLabels()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentModelWrapPanel = (WrapPanel)currentProductGrid.Children[3];
                Label currentModelLabelValue = (Label)currentModelWrapPanel.Children[1];
                currentModelLabelValue.Content = workOffer.GetOfferProductModel(i + 1);
            }
        }

        private void SetQuantityTextBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentQuantityWrapPanel = (WrapPanel)currentProductGrid.Children[4];
                TextBox currentQuantityTextBoxValue = (TextBox)currentQuantityWrapPanel.Children[1];
                currentQuantityTextBoxValue.Text = workOffer.GetOfferProductQuantity(i + 1).ToString();
            }
        }
        private void SetQuantityTextBoxesResolve()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentQuantityWrapPanel = (WrapPanel)currentProductGrid.Children[4];
                TextBox currentQuantityTextBoxValue = (TextBox)currentQuantityWrapPanel.Children[1];
                currentQuantityTextBoxValue.Text = workOffer.GetRFQProductQuantity(i + 1).ToString();
            }
        }

        private void SetPriceTextBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentPriceWrapPanel = (WrapPanel)currentProductGrid.Children[5];
                TextBox currentPriceTextBoxValue = (TextBox)currentPriceWrapPanel.Children[1];
                decimal price = workOffer.GetProductPriceValue(i + 1);
                currentPriceTextBoxValue.Text = price.ToString();
                            }
        }

        public void SetUpPageUIElements()
        {

            for (int i = 0; i < COMPANY_WORK_MACROS.MAX_OFFER_PRODUCTS; i++)
            {
                if (viewAddCondition == 0 && workOffer.GetOfferProductTypeId(i + 1) == 0)
                    continue;
                //if (viewAddCondition == 2 && workOffer.GetworkOfferProductTypeId(i + 1) == 0)
                //  continue;

                Grid currentProductGrid = new Grid();
                currentProductGrid.Margin = new Thickness(24);

                RowDefinition row1 = new RowDefinition();
                RowDefinition row2 = new RowDefinition();
                RowDefinition row3 = new RowDefinition();
                RowDefinition row4 = new RowDefinition();
                RowDefinition row5 = new RowDefinition();
                RowDefinition row6 = new RowDefinition();

                currentProductGrid.RowDefinitions.Add(row1);
                currentProductGrid.RowDefinitions.Add(row2);
                currentProductGrid.RowDefinitions.Add(row3);
                currentProductGrid.RowDefinitions.Add(row4);
                currentProductGrid.RowDefinitions.Add(row5);
                currentProductGrid.RowDefinitions.Add(row6);

                Label mainLabel = new Label();
                int productNumber = i + 1;
                mainLabel.Content = "Product " + productNumber;
                mainLabel.Style = (Style)FindResource("tableHeaderItem");
                currentProductGrid.Children.Add(mainLabel);
                Grid.SetRow(mainLabel, 0);

                /////////TYPE WRAPPANEL////////////////
                ////////////////////////////////////////
                WrapPanel productTypeWrapPanel = new WrapPanel();

                Label currentTypeLabel = new Label();
                currentTypeLabel.Content = "Type";
                currentTypeLabel.Style = (Style)FindResource("labelStyle");
                productTypeWrapPanel.Children.Add(currentTypeLabel);

                if (viewAddCondition == 0)
                {
                    Label currentTypeLabelValue = new Label();
                    currentTypeLabelValue.Style = (Style)FindResource("labelStyle");
                    //currentTypeLabelValue.Margin = new Thickness(-300, 12, 12, 12);
                    currentTypeLabelValue.Content = workOffer.GetOfferProductType(i + 1);
                    productTypeWrapPanel.Children.Add(currentTypeLabelValue);
                }
                else
                {
                    ComboBox currentTypeCombo = new ComboBox();
                    currentTypeCombo.Style = (Style)FindResource("comboBoxStyle");
                    //currentTypeCombo.Margin = new Thickness(-300, 12, 12, 12);
                    currentTypeCombo.SelectionChanged += new SelectionChangedEventHandler(TypeComboBoxesSelectionChanged);
                    //if (viewAddCondition == 0)
                    //  currentTypeCombo.Visibility = Visibility.Collapsed;
                    for (int j = 0; j < products.Count(); j++)
                        currentTypeCombo.Items.Add(products[j].typeName);
                    productTypeWrapPanel.Children.Add(currentTypeCombo);
                }

                currentProductGrid.Children.Add(productTypeWrapPanel);
                Grid.SetRow(productTypeWrapPanel, 1);

                ////////BRAND WRAPPANEL////////////////
                ////////////////////////////////////////
                WrapPanel productBrandWrapPanel = new WrapPanel();

                Label currentBrandLabel = new Label();
                currentBrandLabel.Content = "Brand";
                currentBrandLabel.Style = (Style)FindResource("labelStyle");
                productBrandWrapPanel.Children.Add(currentBrandLabel);

                if (viewAddCondition == 0)
                {
                    Label currentBrandLabelValue = new Label();
                    currentBrandLabelValue.Style = (Style)FindResource("labelStyle");
                    //currentBrandLabelValue.Margin = new Thickness(-300, 12, 12, 12);
                    currentBrandLabelValue.Content = workOffer.GetOfferProductBrand(i + 1);
                    productBrandWrapPanel.Children.Add(currentBrandLabelValue);
                }

                else
                {
                    ComboBox currentBrandCombo = new ComboBox();
                    currentBrandCombo.Style = (Style)FindResource("comboBoxStyle");
                    //currentBrandCombo.Margin = new Thickness(-300, 12, 12, 12);
                    currentBrandCombo.SelectionChanged += new SelectionChangedEventHandler(BrandComboBoxesSelectionChanged);
                    for (int j = 0; j < brands.Count(); j++)
                        currentBrandCombo.Items.Add(brands[j].brandName);
                    productBrandWrapPanel.Children.Add(currentBrandCombo);
                }

                currentProductGrid.Children.Add(productBrandWrapPanel);
                Grid.SetRow(productBrandWrapPanel, 2);

                //////////MODEL WRAPPANEL/////////////////////////
                //////////////////////////////////////////////////
                WrapPanel productModelWrapPanel = new WrapPanel();

                Label currentModelLabel = new Label();
                currentModelLabel.Content = "Model";
                currentModelLabel.Style = (Style)FindResource("labelStyle");
                productModelWrapPanel.Children.Add(currentModelLabel);

                if (viewAddCondition == 0)
                {
                    Label currentModelLabelValue = new Label();
                    currentModelLabelValue.Style = (Style)FindResource("labelStyle");
                    //currentModelLabelValue.Margin = new Thickness(-300, 12, 12, 12);
                    currentModelLabelValue.Content = workOffer.GetOfferProductModel(i + 1);
                    productModelWrapPanel.Children.Add(currentModelLabelValue);
                }
                else
                {
                    ComboBox currentModelCombo = new ComboBox();
                    currentModelCombo.Style = (Style)FindResource("comboBoxStyle");
                    //currentModelCombo.Margin = new Thickness(-300, 12, 12, 12);
                    currentModelCombo.SelectionChanged += new SelectionChangedEventHandler(ModelComboBoxesSelectionChanged);
                    productModelWrapPanel.Children.Add(currentModelCombo);
                }
                currentProductGrid.Children.Add(productModelWrapPanel);
                Grid.SetRow(productModelWrapPanel, 3);

                /////////////QUANTITY WRAPPANEL///////////////////////
                //////////////////////////////////////////////////////
                WrapPanel productQuantityWrapPanel = new WrapPanel();

                Label currentQuantityLabel = new Label();
                currentQuantityLabel.Content = "Quantity";
                currentQuantityLabel.Style = (Style)FindResource("labelStyle");
                productQuantityWrapPanel.Children.Add(currentQuantityLabel);

                TextBox currentQuantityTextBox = new TextBox();
                currentQuantityTextBox.Style = (Style)FindResource("textBoxStyle");
                currentQuantityTextBox.TextChanged += new TextChangedEventHandler(QuantityTextBoxesTextChanged);
                //currentQuantityTextBox.Margin = new Thickness(-300, 12, 12, 12);
                productQuantityWrapPanel.Children.Add(currentQuantityTextBox);

                //currentQuantityTextBox.Text = workOffer.GetworkOfferProductQuantity(i + 1).ToString();

                if (viewAddCondition == 0)
                    currentQuantityTextBox.IsEnabled = false;

                currentProductGrid.Children.Add(productQuantityWrapPanel);
                Grid.SetRow(productQuantityWrapPanel, 4);

                /////////////PRICE WRAPPANEL//////////////////
                /////////////////////////////////////////////
                WrapPanel productPriceWrapPanel = new WrapPanel();

                Label currentPriceLabel = new Label();
                currentPriceLabel.Content = "Price";
                currentPriceLabel.Style = (Style)FindResource("labelStyle");
                productPriceWrapPanel.Children.Add(currentPriceLabel);

                TextBox currentPriceTextBox = new TextBox();
                currentPriceTextBox.Style = (Style)FindResource("textBoxStyle");
                currentPriceTextBox.TextChanged += new TextChangedEventHandler(PriceTextBoxesTextChanged);
                productPriceWrapPanel.Children.Add(currentPriceTextBox);

                ComboBox currentPriceComboBox = new ComboBox();
                currentPriceComboBox.Style = (Style)FindResource("comboBoxStyle");
                currentPriceComboBox.SelectionChanged += new SelectionChangedEventHandler(PriceComboBoxesSelectionChanged);
                currentPriceComboBox.Items.Add("EGP");

                if (viewAddCondition == 0)
                    currentPriceTextBox.IsEnabled = false;


                currentProductGrid.Children.Add(productPriceWrapPanel);
                Grid.SetRow(productPriceWrapPanel, 5);


                mainWrapPanel.Children.Add(currentProductGrid);

                numberOfProductsAdded += 1;
            }
        }

        ///////////////SELECTION CHANGED HANDLERS///////////
        ////////////////////////////////////////////////////

        private void TypeComboBoxesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBox currentTypeComboBox = (ComboBox)sender;
            WrapPanel currentTypeWrapPanel = (WrapPanel)currentTypeComboBox.Parent;
            Grid currentProductGrid = (Grid)currentTypeWrapPanel.Parent;

            WrapPanel currentBrandWrapPanel = (WrapPanel)currentProductGrid.Children[2];
            ComboBox currentBrandComboBox = (ComboBox)currentBrandWrapPanel.Children[1];

            WrapPanel currentModelWrapPanel = (WrapPanel)currentProductGrid.Children[3];
            ComboBox currentModelComboBox = (ComboBox)currentModelWrapPanel.Children[1];


            currentModelComboBox.Items.Clear();

            if (currentTypeComboBox.SelectedItem != null)
            {
                if (currentBrandComboBox.SelectedItem != null)
                {
                    if (!commonQueriesObject.GetCompanyModels(products[currentTypeComboBox.SelectedIndex], brands[currentBrandComboBox.SelectedIndex], ref models))
                        return;

                    for (int i = 0; i < models.Count(); i++)
                    {
                        currentModelComboBox.Items.Add(models[i].modelName);
                    }
                }
                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    if (currentProductGrid == mainWrapPanel.Children[k])
                    {
                        for (int i = 0; i < products.Count; i++)
                        {
                            if (currentTypeComboBox.SelectedItem.ToString() == products[i].typeName)
                                typeId = products[i].typeId;
                        }
                        workOffer.SetOfferProductType(k + 1, typeId, currentTypeComboBox.SelectedItem.ToString());
                    }
                }

            }
        }

        private void BrandComboBoxesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox currentBrandComboBox = (ComboBox)sender;
            WrapPanel currentBrandWrapPanel = (WrapPanel)currentBrandComboBox.Parent;
            Grid currentProductGrid = (Grid)currentBrandWrapPanel.Parent;

            WrapPanel currentTypeWrapPanel = (WrapPanel)currentProductGrid.Children[1];
            ComboBox currentTypeComboBox = (ComboBox)currentTypeWrapPanel.Children[1];

            WrapPanel currentModelWrapPanel = (WrapPanel)currentProductGrid.Children[3];
            ComboBox currentModelComboBox = (ComboBox)currentModelWrapPanel.Children[1];

            currentModelComboBox.Items.Clear();

            if (currentBrandComboBox.SelectedItem != null)
            {
                if (currentTypeComboBox.SelectedItem != null)
                {
                    COMPANY_WORK_MACROS.PRODUCT_STRUCT tempProduct = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
                    COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand = new COMPANY_WORK_MACROS.BRAND_STRUCT();

                    tempProduct = products[currentTypeComboBox.SelectedIndex];
                    tempBrand = brands[currentBrandComboBox.SelectedIndex];

                    if (!commonQueriesObject.GetCompanyModels(tempProduct, tempBrand, ref models))
                        return;

                    for (int i = 0; i < models.Count(); i++)
                    {
                        COMPANY_WORK_MACROS.MODEL_STRUCT temp = models[i];
                        currentModelComboBox.Items.Add(temp.modelName);
                    }
                }
                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    for (int i = 0; i < brands.Count; i++)
                    {
                        if (currentBrandComboBox.SelectedItem.ToString() == brands[i].brandName)
                            brandId = brands[i].brandId;
                    }
                    if (currentProductGrid == mainWrapPanel.Children[k])
                        workOffer.SetOfferProductBrand(k + 1, brands[currentBrandComboBox.SelectedIndex].brandId, currentBrandComboBox.SelectedItem.ToString());
                }

            }
        }

        private void ModelComboBoxesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox currentModelComboBox = (ComboBox)sender;
            WrapPanel currentModelWrapPanel = (WrapPanel)currentModelComboBox.Parent;
            Grid currentProductGrid = (Grid)currentModelWrapPanel.Parent;

            if (currentModelComboBox.SelectedItem != null)
            {
                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    for (int i = 0; i < models.Count; i++)
                    {
                        if (currentModelComboBox.SelectedItem.ToString() == models[i].modelName)
                            modelId = models[i].modelId;
                    }
                    if (currentProductGrid == mainWrapPanel.Children[k])
                        workOffer.SetOfferProductModel(k + 1, modelId, currentModelComboBox.SelectedItem.ToString());
                }

            }

        }

        private void QuantityTextBoxesTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox currentQuantityTextBox = (TextBox)sender;
            WrapPanel currentQuantityWrapPanel = (WrapPanel)currentQuantityTextBox.Parent;
            Grid currentProductGrid = (Grid)currentQuantityWrapPanel.Parent;

            if (IntegrityChecks.CheckInvalidCharacters(currentQuantityTextBox.Text, BASIC_MACROS.PHONE_STRING) && currentQuantityTextBox.Text != "")
            {
                quantity = int.Parse(currentQuantityTextBox.Text);
                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    if (currentProductGrid == mainWrapPanel.Children[k])
                        workOffer.SetOfferProductQuantity(k + 1, quantity);
                }
            }
            else
            {
                quantity = 0;
                currentQuantityTextBox.Text = null;
            }
        }
        private void PriceTextBoxesTextChanged(Object sender, TextChangedEventArgs e)
        {
            TextBox currentPriceTextBox = (TextBox)sender;
            WrapPanel currentPriceWrapPanel = (WrapPanel)currentPriceTextBox.Parent;
            Grid currentProductGrid = (Grid)currentPriceWrapPanel.Parent;

            if (viewAddCondition != 0)
            {
                if (IntegrityChecks.CheckInvalidCharacters(currentPriceTextBox.Text.ToString(), BASIC_MACROS.MONETARY_STRING) && currentPriceTextBox.Text != "")
                {
                    priceQuantity = Decimal.Parse(currentPriceTextBox.Text.ToString());
                    for (int k = 0; k < numberOfProductsAdded; k++)
                    {
                        if (currentProductGrid == mainWrapPanel.Children[k] && viewAddCondition == 1)
                            workOffer.SetOfferProductPriceValue(k + 1,int.Parse(priceQuantity.ToString()));
                    }
                }
                else
                {
                    priceQuantity = 0;
                   // if(viewAddCondition == 1)
                        currentPriceTextBox.Text = null;
                }
            }
        }

        private void PriceComboBoxesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox currentPriceComboBox = (ComboBox)sender;
            WrapPanel currentPriceWrapPanel = (WrapPanel)currentPriceComboBox.Parent;
            Grid currentProductGrid = (Grid)currentPriceWrapPanel.Parent;

            // if (currentPriceComboBox.SelectedItem != null)
            //{
            //  for (int k = 0; k < numberOfProductsAdded; k++)
            // {
            //   if (currentProductGrid == mainWrapPanel.Children[k])
            //     workOffer.SetOffer(k + 1, modelId, currentModelComboBox.SelectedItem.ToString());
            //}

            //}
        }

        ///////////////CHECKBOX HANDLERS////////////////////
        ///////////////////////////////////////////////////
        /*private void Product2CheckBoxChecked(object sender, RoutedEventArgs e)
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
        }*/
        //////////BUTTON CLICK HANDLERS//////////////////
        /////////////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            WorkOfferBasicInfoPage basicInfoPage = new WorkOfferBasicInfoPage(ref loggedInUser, ref workOffer, viewAddCondition);
            NavigationService.Navigate(basicInfoPage);
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
            WorkOfferAdditionalInfoPage offerAdditionalInfoPage = new WorkOfferAdditionalInfoPage(ref loggedInUser, ref workOffer, viewAddCondition);
            NavigationService.Navigate(offerAdditionalInfoPage);

        }

        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {
            WorkOfferPaymentAndDeliveryPage paymentAndDeliveryPage = new WorkOfferPaymentAndDeliveryPage(ref loggedInUser, ref workOffer, viewAddCondition);
            NavigationService.Navigate(paymentAndDeliveryPage);
        }
    }
}
