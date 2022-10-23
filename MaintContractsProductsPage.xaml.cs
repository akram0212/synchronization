using _01electronics_library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for MaintContractsProductsPage.xaml
    /// </summary>
    public partial class MaintContractsProductsPage : Page
    {
        Employee loggedInUser;
        MaintenanceContract maintContracts;

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private IntegrityChecks IntegrityChecks = new IntegrityChecks();

        private List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT> categories = new List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT>();
        private List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> products = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
        private List<COMPANY_WORK_MACROS.BRAND_STRUCT> brands = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();
        private List<COMPANY_WORK_MACROS.MODEL_STRUCT> models = new List<COMPANY_WORK_MACROS.MODEL_STRUCT>();
        private List<COMPANY_WORK_MACROS.SPEC_STRUCT> modelSpecs = new List<COMPANY_WORK_MACROS.SPEC_STRUCT>();

        //private List<COMPANY_WORK_MACROS.MaintContract_PRODUCT_STRUCT> MaintContractProduct1 = new List<COMPANY_WORK_MACROS.MaintContract_PRODUCT_STRUCT>();

        private List<BASIC_STRUCTS.CURRENCY_STRUCT> currencies = new List<BASIC_STRUCTS.CURRENCY_STRUCT>();

        private int viewAddCondition;
        //private int previousViewAddCondition;
        private int numberOfProductsAdded;
        private int quantity;
        private decimal priceQuantity;

        public MaintContractsBasicInfoPage maintContractsBasicInfoPage;
        public MaintContractsPaymentAndDeliveryPage maintContractsPaymentAndDeliveryPage;
        public MaintContractsAdditionalInfoPage maintContractsAdditionalInfoPage;
        public MaintContractsUploadFilesPage maintContractsUploadFilesPage;
        public MaintContractsProjectsPage maintContractsProjectsPage;

        public MaintContractsProductsPage(ref Employee mLoggedInUser, ref MaintenanceContract mMaintContracts, int mViewAddCondition, ref MaintContractsPaymentAndDeliveryPage mMaintContractsPaymentAndDeliveryPage, ref MaintContractsAdditionalInfoPage mMaintContractsAdditionalInfoPage)
        {
            maintContractsPaymentAndDeliveryPage = mMaintContractsPaymentAndDeliveryPage;
            maintContractsAdditionalInfoPage = mMaintContractsAdditionalInfoPage;

            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            maintContracts = mMaintContracts;

            numberOfProductsAdded = 0;

            InitializeComponent();
            //previousViewAddCondition = viewAddCondition;

            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_ADD_CONDITION)
            {
                InitializeCategories();
                InitializePriceCurrencyComboBoxes();
                SetUpPageUIElements();
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                //viewAddCondition = COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION;
                InitializePriceCurrencyComboBoxes();
                SetUpPageUIElements();
                SetCategoryLabels();
                SetTypeLabels();
                SetBrandLabels();
                SetModelLabels();
                SetQuantityTextBoxes();
                SetPriceTextBoxes();
                SetPriceComboBoxes();

                cancelButton.IsEnabled = false;
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION || viewAddCondition == COMPANY_WORK_MACROS.CONTRACT_RENEW_CONDITION)
            {
                InitializeCategories();
                InitializePriceCurrencyComboBoxes();
                SetUpPageUIElements();
                SetCategoryComboBoxes();
                SetTypeComboBoxes();
                SetBrandComboBoxes();
                SetModelComboBoxes();
                SetQuantityTextBoxes();
                SetPriceTextBoxes();
                SetPriceComboBoxes();
            }
            else
            {
                InitializeCategories();
                InitializePriceCurrencyComboBoxes();
                SetUpPageUIElements();
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INITIALIZATION FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetUpPageUIElements()
        {

            for (int i = 0; i < COMPANY_WORK_MACROS.MAX_ORDER_PRODUCTS; i++)
            {
                if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION && maintContracts.GetMaintContractProductTypeId(i + 1) == 0)
                    continue;
                //if (viewAddCondition == 2 && maintContracts.GetmaintContractsProductTypeId(i + 1) == 0)
                //  continue;

                Grid currentProductGrid = new Grid();
                currentProductGrid.Margin = new Thickness(24);

                RowDefinition row1 = new RowDefinition();
                RowDefinition row2 = new RowDefinition();
                RowDefinition row3 = new RowDefinition();
                RowDefinition row4 = new RowDefinition();
                RowDefinition row5 = new RowDefinition();
                RowDefinition row6 = new RowDefinition();
                RowDefinition row7 = new RowDefinition();
                RowDefinition row8 = new RowDefinition();

                currentProductGrid.RowDefinitions.Add(row1);
                currentProductGrid.RowDefinitions.Add(row2);
                currentProductGrid.RowDefinitions.Add(row3);
                currentProductGrid.RowDefinitions.Add(row4);
                currentProductGrid.RowDefinitions.Add(row5);
                currentProductGrid.RowDefinitions.Add(row6);
                currentProductGrid.RowDefinitions.Add(row7);
                currentProductGrid.RowDefinitions.Add(row8);

                Grid backgroundColour = new Grid();
                RowDefinition firstRow = new RowDefinition();
                backgroundColour.RowDefinitions.Add(firstRow);
                backgroundColour.Background = new SolidColorBrush(Color.FromRgb(16, 90, 151));

                CheckBox mainLabelCheckBox = new CheckBox();
                int productNumber = i + 1;
                mainLabelCheckBox.Content = "Product " + productNumber;
                mainLabelCheckBox.Style = (Style)FindResource("checkBoxStyle");
                mainLabelCheckBox.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                mainLabelCheckBox.Checked += new RoutedEventHandler(OnCheckMainLabelCheckBox);
                mainLabelCheckBox.Unchecked += new RoutedEventHandler(OnUnCheckMainLabelCheckBox);

                backgroundColour.Children.Add(mainLabelCheckBox);
                Grid.SetRow(mainLabelCheckBox, 0);


                if (i == 0 || viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                    mainLabelCheckBox.IsEnabled = false;
                else if (i == 1)
                    mainLabelCheckBox.IsEnabled = true;
                else
                    mainLabelCheckBox.IsEnabled = false;

                currentProductGrid.Children.Add(backgroundColour);
                Grid.SetRow(backgroundColour, 0);

                /////////CATEGORY WRAPPANEL////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////////
                WrapPanel productCategoryWrapPanel = new WrapPanel();

                Label currentCategoryLabel = new Label();
                currentCategoryLabel.Content = "Category*";
                currentCategoryLabel.Style = (Style)FindResource("labelStyle");
                productCategoryWrapPanel.Children.Add(currentCategoryLabel);

                if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                {
                    Label currentCategoryLabelValue = new Label();
                    currentCategoryLabelValue.Style = (Style)FindResource("labelStyle");
                    currentCategoryLabelValue.Width = 150.00;
                    currentCategoryLabelValue.Content = maintContracts.GetMaintContractProductCategory(i + 1);
                    productCategoryWrapPanel.Children.Add(currentCategoryLabelValue);
                }
                else
                {
                    ComboBox currentCategoryCombo = new ComboBox();
                    currentCategoryCombo.Style = (Style)FindResource("comboBoxStyle");
                    currentCategoryCombo.SelectionChanged += new SelectionChangedEventHandler(CategoryComboBoxesSelectionChanged);
                    if (i != 0)
                        currentCategoryCombo.IsEnabled = false;
                    for (int j = 0; j < categories.Count(); j++)
                        currentCategoryCombo.Items.Add(categories[j].category);
                    productCategoryWrapPanel.Children.Add(currentCategoryCombo);
                }

                currentProductGrid.Children.Add(productCategoryWrapPanel);
                Grid.SetRow(productCategoryWrapPanel, 1);

                /////////TYPE WRAPPANEL////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////////
                WrapPanel productTypeWrapPanel = new WrapPanel();

                Label currentTypeLabel = new Label();
                currentTypeLabel.Content = "Type*";
                currentTypeLabel.Style = (Style)FindResource("labelStyle");
                productTypeWrapPanel.Children.Add(currentTypeLabel);

                if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                {
                    Label currentTypeLabelValue = new Label();
                    currentTypeLabelValue.Style = (Style)FindResource("labelStyle");
                    currentTypeLabelValue.Width = 150.00;
                    currentTypeLabelValue.Content = maintContracts.GetMaintContractProductType(i + 1);
                    productTypeWrapPanel.Children.Add(currentTypeLabelValue);
                }
                else
                {
                    ComboBox currentTypeCombo = new ComboBox();
                    currentTypeCombo.Style = (Style)FindResource("comboBoxStyle");
                    currentTypeCombo.SelectionChanged += new SelectionChangedEventHandler(TypeComboBoxesSelectionChanged);
                    currentTypeCombo.IsEnabled = false;
                    for (int j = 0; j < products.Count(); j++)
                        currentTypeCombo.Items.Add(products[j].typeName);
                    productTypeWrapPanel.Children.Add(currentTypeCombo);
                    currentTypeCombo.IsEnabled = false;
                }

                currentProductGrid.Children.Add(productTypeWrapPanel);
                Grid.SetRow(productTypeWrapPanel, 2);

                ////////BRAND WRAPPANEL////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////////
                WrapPanel productBrandWrapPanel = new WrapPanel();

                Label currentBrandLabel = new Label();
                currentBrandLabel.Content = "Brand";
                currentBrandLabel.Style = (Style)FindResource("labelStyle");
                productBrandWrapPanel.Children.Add(currentBrandLabel);

                if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                {
                    Label currentBrandLabelValue = new Label();
                    currentBrandLabelValue.Style = (Style)FindResource("labelStyle");
                    currentBrandLabelValue.Width = 150.00;
                    currentBrandLabelValue.Content = maintContracts.GetMaintContractProductBrand(i + 1);
                    productBrandWrapPanel.Children.Add(currentBrandLabelValue);
                }

                else
                {
                    ComboBox currentBrandCombo = new ComboBox();
                    currentBrandCombo.Style = (Style)FindResource("comboBoxStyle");
                    currentBrandCombo.SelectionChanged += new SelectionChangedEventHandler(BrandComboBoxesSelectionChanged);
                    currentBrandCombo.IsEnabled = false;
                    for (int j = 0; j < brands.Count(); j++)
                        currentBrandCombo.Items.Add(brands[j].brandName);
                    productBrandWrapPanel.Children.Add(currentBrandCombo);
                }

                currentProductGrid.Children.Add(productBrandWrapPanel);
                Grid.SetRow(productBrandWrapPanel, 3);

                //////////MODEL WRAPPANEL/////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////////////////////
                WrapPanel productModelWrapPanel = new WrapPanel();

                Label currentModelLabel = new Label();
                currentModelLabel.Content = "Model";
                currentModelLabel.Style = (Style)FindResource("labelStyle");
                productModelWrapPanel.Children.Add(currentModelLabel);

                if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                {
                    Label currentModelLabelValue = new Label();
                    currentModelLabelValue.Style = (Style)FindResource("labelStyle");
                    currentModelLabelValue.Width = 150.00;
                    currentModelLabelValue.Content = maintContracts.GetMaintContractProductModel(i + 1);
                    productModelWrapPanel.Children.Add(currentModelLabelValue);
                }
                else
                {
                    ComboBox currentModelCombo = new ComboBox();
                    currentModelCombo.Style = (Style)FindResource("comboBoxStyle");
                    currentModelCombo.SelectionChanged += new SelectionChangedEventHandler(ModelComboBoxesSelectionChanged);
                    currentModelCombo.IsEnabled = false;
                    productModelWrapPanel.Children.Add(currentModelCombo);
                }
                currentProductGrid.Children.Add(productModelWrapPanel);
                Grid.SetRow(productModelWrapPanel, 4);


                //////////SPEC NAME WRAPPANEL/////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////////////////////
                WrapPanel specNameWrapPanel = new WrapPanel();

                Label currentSpecNameLabel = new Label();
                currentSpecNameLabel.Content = "Spec Name";
                currentSpecNameLabel.Style = (Style)FindResource("labelStyle");
                specNameWrapPanel.Children.Add(currentSpecNameLabel);

                if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                {
                    Label currentSpecNameLabelValue = new Label();
                    currentSpecNameLabelValue.Style = (Style)FindResource("labelStyle");
                    currentSpecNameLabelValue.Width = 150.00;
                    currentSpecNameLabelValue.Content = maintContracts.GetMaintContractProductModel(i + 1);
                    specNameWrapPanel.Children.Add(currentSpecNameLabelValue);
                }
                else
                {
                    ComboBox currentSpecNameCombo = new ComboBox();
                    currentSpecNameCombo.Style = (Style)FindResource("comboBoxStyle");
                    //currentSpecNameCombo.SelectionChanged += new SelectionChangedEventHandler(SpecNameComboBoxesSelectionChanged);
                    currentSpecNameCombo.IsEnabled = true;
                    specNameWrapPanel.Children.Add(currentSpecNameCombo);
                }
                currentProductGrid.Children.Add(specNameWrapPanel);
                Grid.SetRow(specNameWrapPanel, 5);

                specNameWrapPanel.IsEnabled = false;

                /////////////QUANTITY WRAPPANEL///////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                WrapPanel productQuantityWrapPanel = new WrapPanel();

                Label currentQuantityLabel = new Label();
                currentQuantityLabel.Content = "Quantity";
                currentQuantityLabel.Style = (Style)FindResource("labelStyle");
                productQuantityWrapPanel.Children.Add(currentQuantityLabel);

                TextBox currentQuantityTextBox = new TextBox();
                currentQuantityTextBox.Style = (Style)FindResource("textBoxStyle");
                currentQuantityTextBox.TextChanged += new TextChangedEventHandler(QuantityTextBoxesTextChanged);
                if (i != 0)
                    currentQuantityTextBox.IsEnabled = false;
                productQuantityWrapPanel.Children.Add(currentQuantityTextBox);

                if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION || viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                    currentQuantityTextBox.IsEnabled = false;

                currentProductGrid.Children.Add(productQuantityWrapPanel);
                Grid.SetRow(productQuantityWrapPanel, 6);

                /////////////PRICE WRAPPANEL//////////////////
                /////////////////////////////////////////////////////////////////////////////////////////////////////
                WrapPanel productPriceWrapPanel = new WrapPanel();

                Label currentPriceLabel = new Label();
                currentPriceLabel.Content = "Price";
                currentPriceLabel.Style = (Style)FindResource("labelStyle");
                productPriceWrapPanel.Children.Add(currentPriceLabel);

                TextBox currentPriceTextBox = new TextBox();
                currentPriceTextBox.Style = (Style)FindResource("miniTextBoxStyle");
                currentPriceTextBox.Margin = new System.Windows.Thickness(30, 12, 42, 12);
                currentPriceTextBox.TextChanged += new TextChangedEventHandler(PriceTextBoxesTextChanged);

                productPriceWrapPanel.Children.Add(currentPriceTextBox);

                ComboBox currentPriceComboBox = new ComboBox();
                currentPriceComboBox.Style = (Style)FindResource("miniComboBoxStyle");
                currentPriceComboBox.Margin = new System.Windows.Thickness(42, 12, 12, 12);
                currentPriceComboBox.SelectionChanged += new SelectionChangedEventHandler(PriceComboBoxesSelectionChanged);
                for (int j = 0; j < currencies.Count; j++)
                {
                    currentPriceComboBox.Items.Add(currencies[j].currencyName);
                }
                if (i != 0)
                {
                    currentPriceTextBox.IsEnabled = false;
                    currentPriceComboBox.IsEnabled = false;
                }
                productPriceWrapPanel.Children.Add(currentPriceComboBox);

                if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                {
                    currentPriceTextBox.IsEnabled = false;
                    currentPriceComboBox.IsEnabled = false;
                }

                currentProductGrid.Children.Add(productPriceWrapPanel);
                Grid.SetRow(productPriceWrapPanel, 7);


                mainWrapPanel.Children.Add(currentProductGrid);

                numberOfProductsAdded += 1;
            }
        }

        private bool InitializeCategories()
        {
            if (!commonQueriesObject.GetProductCategories(ref categories))
                return false;

            return true;
        }
        private void InitializeProducts()
        {
            if (!commonQueriesObject.GetCompanyProducts(ref products))
                return;
        }

        private void InitializeBrandCombo(int productId)
        {
            if (!commonQueriesObject.GetProductBrands(productId, ref brands))
                return;
        }

        private void InitializePriceCurrencyComboBoxes()
        {
            if (!commonQueriesObject.GetCurrencyTypes(ref currencies))
                return;

        }
        
        private void InitializeSpecNameCombo(ComboBox currentSpecNameComboBox)
        {
            currentSpecNameComboBox.Items.Clear();
            for (int i = 0; i < modelSpecs.Count(); i++)
            {
                currentSpecNameComboBox.Items.Add(modelSpecs[i].spec_name);
            }
            if (modelSpecs.Count() != 0)
                currentSpecNameComboBox.SelectedIndex = 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///SET FUNCTIONS
        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetCategoryComboBoxesFromOffer()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                if (maintContracts.GetMaintOfferProductTypeId(i + 1) != 0)
                {
                    Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                    WrapPanel currentCategoryWrapPanel = (WrapPanel)currentProductGrid.Children[1];
                    ComboBox CurrentCategoryComboBox = (ComboBox)currentCategoryWrapPanel.Children[1];
                    CurrentCategoryComboBox.SelectedItem = maintContracts.GetMaintOfferProductCategory(i + 1);
                }
            }
        }
        public void SetTypeComboBoxesFromOffer()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                if (maintContracts.GetMaintOfferProductTypeId(i + 1) != 0)
                {
                    Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                    WrapPanel currentTypeWrapPanel = (WrapPanel)currentProductGrid.Children[2];
                    ComboBox CurrentTypeComboBox = (ComboBox)currentTypeWrapPanel.Children[1];
                    if (maintContracts.GetMaintOfferProductTypeId(i + 1) != 0)
                        CurrentTypeComboBox.SelectedItem = maintContracts.GetMaintOfferProductType(i + 1);
                }
            }
        }
        public void SetBrandComboBoxesFromOffer()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentBrandWrapPanel = (WrapPanel)currentProductGrid.Children[3];
                ComboBox currentBrandComboBox = (ComboBox)currentBrandWrapPanel.Children[1];

                currentBrandComboBox.SelectedItem = maintContracts.GetMaintOfferProductBrand(i + 1);
            }
        }

        public void SetModelComboBoxesFromOffer()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentModelWrapPanel = (WrapPanel)currentProductGrid.Children[4];
                ComboBox currentModelComboBox = (ComboBox)currentModelWrapPanel.Children[1];

                //if (maintContracts.GetMaintOfferProductModelId(i + 1) == 0)
                //    continue;
                // else
                currentModelComboBox.SelectedItem = maintContracts.GetMaintOfferProductModel(i + 1);
            }
        }

        private void SetCategoryComboBoxes()
        {
            for (int i = 0; i < maintContracts.GetNoOfMaintContractSavedProducts(); i++)
            {

                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];

                if (maintContracts.GetMaintContractProductCategory(i + 1) != "" && i != 0)
                {
                    Grid backgroundColorGrid = (Grid)currentProductGrid.Children[0];
                    CheckBox mainCheckBox = (CheckBox)backgroundColorGrid.Children[0];

                    mainCheckBox.IsEnabled = true;
                    mainCheckBox.IsChecked = true;
                }

                WrapPanel currentCategoryWrapPanel = (WrapPanel)currentProductGrid.Children[1];
                ComboBox CurrentCategoryComboBox = (ComboBox)currentCategoryWrapPanel.Children[1];
                CurrentCategoryComboBox.SelectedItem = maintContracts.GetMaintContractProductCategory(i + 1);


            }
        }
        private void SetTypeComboBoxes()
        {
            for (int i = 0; i < maintContracts.GetNoOfMaintContractSavedProducts(); i++)
            {

                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentTypeWrapPanel = (WrapPanel)currentProductGrid.Children[2];
                ComboBox CurrentTypeComboBox = (ComboBox)currentTypeWrapPanel.Children[1];
                CurrentTypeComboBox.SelectedItem = maintContracts.GetMaintContractProductType(i + 1);

            }
        }
        private void SetBrandComboBoxes()
        {
            for (int i = 0; i < maintContracts.GetNoOfMaintContractSavedProducts(); i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentBrandWrapPanel = (WrapPanel)currentProductGrid.Children[3];
                ComboBox currentBrandComboBox = (ComboBox)currentBrandWrapPanel.Children[1];

                currentBrandComboBox.SelectedItem = maintContracts.GetMaintContractProductBrand(i + 1);
            }
        }

        private void SetModelComboBoxes()
        {
            for (int i = 0; i < maintContracts.GetNoOfMaintContractSavedProducts(); i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentModelWrapPanel = (WrapPanel)currentProductGrid.Children[4];
                ComboBox currentModelComboBox = (ComboBox)currentModelWrapPanel.Children[1];

                currentModelComboBox.SelectedItem = maintContracts.GetMaintContractProductModel(i + 1);
            }
        }

        public void SetCategoryLabels()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentCategoryWrapPanel = (WrapPanel)currentProductGrid.Children[1];
                Label currentCategoryLabelValue = (Label)currentCategoryWrapPanel.Children[1];
                currentCategoryLabelValue.Content = maintContracts.GetMaintContractProductCategory(i + 1);
            }
        }
        public void SetTypeLabels()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentTypeWrapPanel = (WrapPanel)currentProductGrid.Children[2];
                Label currentTypeLabelValue = (Label)currentTypeWrapPanel.Children[1];
                currentTypeLabelValue.Content = maintContracts.GetMaintContractProductType(i + 1);
            }
        }

        public void SetBrandLabels()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentBrandWrapPanel = (WrapPanel)currentProductGrid.Children[3];
                Label currentBrandLabelValue = (Label)currentBrandWrapPanel.Children[1];
                currentBrandLabelValue.Content = maintContracts.GetMaintContractProductBrand(i + 1);
            }
        }

        public void SetModelLabels()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentModelWrapPanel = (WrapPanel)currentProductGrid.Children[4];
                Label currentModelLabelValue = (Label)currentModelWrapPanel.Children[1];
                currentModelLabelValue.Content = maintContracts.GetMaintContractProductModel(i + 1);
            }
        }

        public void SetQuantityTextBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                {
                    Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                    WrapPanel currentQuantityWrapPanel = (WrapPanel)currentProductGrid.Children[6];
                    TextBox currentQuantityTextBoxValue = (TextBox)currentQuantityWrapPanel.Children[1];
                    if(maintContracts.GetMaintOfferProductQuantity(i + 1) != 0)
                        currentQuantityTextBoxValue.Text = maintContracts.GetMaintOfferProductQuantity(i + 1).ToString();
                    else
                        currentQuantityTextBoxValue.Text = maintContracts.GetMaintContractProductQuantity(i + 1).ToString();

                }
                else
                {
                    Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                    WrapPanel currentQuantityWrapPanel = (WrapPanel)currentProductGrid.Children[6];
                    TextBox currentQuantityTextBoxValue = (TextBox)currentQuantityWrapPanel.Children[1];
                    currentQuantityTextBoxValue.Text = maintContracts.GetMaintContractProductQuantity(i + 1).ToString();

                }
            }
        }
        public void SetPriceTextBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                {
                    Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                    WrapPanel currentPriceWrapPanel = (WrapPanel)currentProductGrid.Children[7];
                    TextBox currentPriceTextBoxValue = (TextBox)currentPriceWrapPanel.Children[1];
                    int price = (int)maintContracts.GetProductPriceValue(i + 1);

                    if (maintContracts.GetProductPriceValue(i + 1) == 0)
                        price = (int)maintContracts.GetMaintContractProductPriceValue(i + 1);

                    currentPriceTextBoxValue.Text = price.ToString();

                }
                else
                {
                    Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                    WrapPanel currentPriceWrapPanel = (WrapPanel)currentProductGrid.Children[7];
                    TextBox currentPriceTextBoxValue = (TextBox)currentPriceWrapPanel.Children[1];
                    int price = (int)maintContracts.GetMaintContractProductPriceValue(i + 1);
                    currentPriceTextBoxValue.Text = price.ToString();

                }
            }
        }
        public void SetPriceComboBoxes()
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION && mainWrapPanel.Children.Count != 0)
            {

                Grid currentPriceGrid = (Grid)mainWrapPanel.Children[0];
                WrapPanel currentProductWrapPanel = (WrapPanel)currentPriceGrid.Children[7];
                ComboBox currentPriceComboBox = (ComboBox)currentProductWrapPanel.Children[2];

                if(maintContracts.GetCurrencyId() != 0)
                    currentPriceComboBox.SelectedItem = maintContracts.GetCurrency();
                else
                    currentPriceComboBox.SelectedItem = maintContracts.GetMaintContractCurrency();


            }
            else if (mainWrapPanel.Children.Count != 0)
            {
                Grid currentPriceGrid = (Grid)mainWrapPanel.Children[0];
                WrapPanel currentProductWrapPanel = (WrapPanel)currentPriceGrid.Children[7];
                ComboBox currentPriceComboBox = (ComboBox)currentProductWrapPanel.Children[2];
                currentPriceComboBox.SelectedItem = maintContracts.GetMaintContractCurrency();

            }
        }

        public void SetQuantityTextBoxesFromOffer()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentQuantityWrapPanel = (WrapPanel)currentProductGrid.Children[6];
                TextBox currentQuantityTextBoxValue = (TextBox)currentQuantityWrapPanel.Children[1];
                currentQuantityTextBoxValue.Text = maintContracts.GetMaintOfferProductQuantity(i + 1).ToString();
            }
        }
        public void SetPriceTextBoxesFromOffer()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {

                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentPriceWrapPanel = (WrapPanel)currentProductGrid.Children[7];
                TextBox currentPriceTextBoxValue = (TextBox)currentPriceWrapPanel.Children[1];
                int price = (int)maintContracts.GetProductPriceValue(i + 1);
                currentPriceTextBoxValue.Text = price.ToString();


            }
        }
        public void SetPriceComboBoxesFromOffer()
        {
            Grid currentPriceGrid = (Grid)mainWrapPanel.Children[0];
            WrapPanel currentProductWrapPanel = (WrapPanel)currentPriceGrid.Children[7];
            ComboBox currentPriceComboBox = (ComboBox)currentProductWrapPanel.Children[2];
            currentPriceComboBox.SelectedItem = maintContracts.GetCurrency();

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////SELECTION CHANGED HANDLERS///////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CategoryComboBoxesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBox currentCategoryComboBox = (ComboBox)sender;
            WrapPanel currentCategoryWrapPanel = (WrapPanel)currentCategoryComboBox.Parent;
            Grid currentProductGrid = (Grid)currentCategoryWrapPanel.Parent;

            WrapPanel currentTypeWrapPanel = (WrapPanel)currentProductGrid.Children[2];
            ComboBox currentTypeComboBox = (ComboBox)currentTypeWrapPanel.Children[1];

            Grid checkBoxGrid = (Grid)currentProductGrid.Children[0];
            CheckBox currentProductCheckBox = (CheckBox)checkBoxGrid.Children[0];

            products.Clear();
            currentTypeComboBox.Items.Clear();
            currentTypeComboBox.IsEnabled = false;

            if (currentCategoryComboBox.SelectedItem != null)
            {

                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    if (currentProductGrid == mainWrapPanel.Children[k])
                    {
                        if (k != 0)
                            currentProductCheckBox.IsChecked = true;

                        maintContracts.SetMaintContractProductCategory(k + 1, categories[currentCategoryComboBox.SelectedIndex].categoryId, categories[currentCategoryComboBox.SelectedIndex].category);

                        if (!commonQueriesObject.GetCompanyProducts(ref products, categories[currentCategoryComboBox.SelectedIndex].categoryId))
                            return;
                        for (int i = 0; i < products.Count; i++)
                            currentTypeComboBox.Items.Add(products[i].typeName);
                    }
                }

                currentTypeComboBox.IsEnabled = true;
            }
            else
            {
                currentTypeComboBox.IsEnabled = false;
                currentTypeComboBox.SelectedItem = null;

                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    if (k != 0)
                        currentProductCheckBox.IsChecked = false;

                    if (currentProductGrid == mainWrapPanel.Children[k])
                        maintContracts.SetMaintContractProductCategory(k + 1, 0, "Others");
                }
            }
        }

        private void TypeComboBoxesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBox currentTypeComboBox = (ComboBox)sender;
            WrapPanel currentTypeWrapPanel = (WrapPanel)currentTypeComboBox.Parent;
            Grid currentProductGrid = (Grid)currentTypeWrapPanel.Parent;

            WrapPanel currentCategoryWrapPanel = (WrapPanel)currentProductGrid.Children[1];
            ComboBox currentCategoryComboBox = (ComboBox)currentCategoryWrapPanel.Children[1];

            WrapPanel currentBrandWrapPanel = (WrapPanel)currentProductGrid.Children[3];
            ComboBox currentBrandComboBox = (ComboBox)currentBrandWrapPanel.Children[1];

            WrapPanel currentModelWrapPanel = (WrapPanel)currentProductGrid.Children[4];
            ComboBox currentModelComboBox = (ComboBox)currentModelWrapPanel.Children[1];

            Grid checkBoxGrid = (Grid)currentProductGrid.Children[0];
            CheckBox currentProductCheckBox = (CheckBox)checkBoxGrid.Children[0];


            currentModelComboBox.Items.Clear();

            if (currentTypeComboBox.SelectedItem != null)
            {
                if (!commonQueriesObject.GetCompanyProducts(ref products, categories[currentCategoryComboBox.SelectedIndex].categoryId))
                    return;
                currentBrandComboBox.IsEnabled = true;
                InitializeBrandCombo(products[currentTypeComboBox.SelectedIndex].typeId);
                currentBrandComboBox.Items.Clear();
                for (int m = 0; m < brands.Count; m++)
                {
                    currentBrandComboBox.Items.Add(brands[m].brandName);
                }

                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    if (currentProductGrid == mainWrapPanel.Children[k])
                    {
                        if (k != 0)
                            currentProductCheckBox.IsChecked = true;

                        maintContracts.SetMaintContractProductType(k + 1, products[currentTypeComboBox.SelectedIndex].typeId, products[currentTypeComboBox.SelectedIndex].typeName);
                    }
                }
            }
            else
            {
                currentBrandComboBox.IsEnabled = false;
                currentBrandComboBox.SelectedItem = null;

                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    if (k != 0)
                        currentProductCheckBox.IsChecked = false;

                    if (currentProductGrid == mainWrapPanel.Children[k])
                        maintContracts.SetMaintContractProductType(k + 1, 0, "Others");
                }
            }
        }

        private void BrandComboBoxesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox currentBrandComboBox = (ComboBox)sender;
            WrapPanel currentBrandWrapPanel = (WrapPanel)currentBrandComboBox.Parent;
            Grid currentProductGrid = (Grid)currentBrandWrapPanel.Parent;

            WrapPanel currentCategoryWrapPanel = (WrapPanel)currentProductGrid.Children[1];
            ComboBox currentCategoryComboBox = (ComboBox)currentCategoryWrapPanel.Children[1];

            WrapPanel currentTypeWrapPanel = (WrapPanel)currentProductGrid.Children[2];
            ComboBox currentTypeComboBox = (ComboBox)currentTypeWrapPanel.Children[1];

            WrapPanel currentModelWrapPanel = (WrapPanel)currentProductGrid.Children[4];
            ComboBox currentModelComboBox = (ComboBox)currentModelWrapPanel.Children[1];

            currentModelComboBox.Items.Clear();

            if (currentBrandComboBox.SelectedItem != null)
            {
                if (currentTypeComboBox.SelectedItem != null)
                {
                    if (!commonQueriesObject.GetCompanyProducts(ref products, categories[currentCategoryComboBox.SelectedIndex].categoryId))
                        return;
                    InitializeBrandCombo(products[currentTypeComboBox.SelectedIndex].typeId);

                    currentModelComboBox.IsEnabled = true;
                    if (!commonQueriesObject.GetCompanyModels(products[currentTypeComboBox.SelectedIndex], brands[currentBrandComboBox.SelectedIndex], ref models))
                        return;

                    for (int i = 0; i < models.Count(); i++)
                    {
                        COMPANY_WORK_MACROS.MODEL_STRUCT temp = models[i];
                        currentModelComboBox.Items.Add(temp.modelName);
                    }
                }
                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    if (currentProductGrid == mainWrapPanel.Children[k])
                    {
                        maintContracts.SetMaintContractProductBrand(k + 1, brands[currentBrandComboBox.SelectedIndex].brandId, currentBrandComboBox.SelectedItem.ToString());
                    }
                }
            }
            else
            {
                currentModelComboBox.IsEnabled = false;
                currentModelComboBox.SelectedItem = null;

                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    if (currentProductGrid == mainWrapPanel.Children[k])
                        maintContracts.SetMaintContractProductBrand(k + 1, 0, "Others");
                }
            }
        }

        private void ModelComboBoxesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox currentModelComboBox = (ComboBox)sender;
            WrapPanel currentModelWrapPanel = (WrapPanel)currentModelComboBox.Parent;
            Grid currentProductGrid = (Grid)currentModelWrapPanel.Parent;

            WrapPanel currentCategoryWrapPanel = (WrapPanel)currentProductGrid.Children[1];
            ComboBox currentCategoryComboBox = (ComboBox)currentCategoryWrapPanel.Children[1];

            WrapPanel currentTypeWrapPanel = (WrapPanel)currentProductGrid.Children[2];
            ComboBox currentTypeComboBox = (ComboBox)currentTypeWrapPanel.Children[1];

            WrapPanel currentBrandWrapPanel = (WrapPanel)currentProductGrid.Children[3];
            ComboBox currentBrandComboBox = (ComboBox)currentBrandWrapPanel.Children[1];

            WrapPanel currentSpecNameWrapPanel = (WrapPanel)currentProductGrid.Children[5];
            ComboBox currentSpecNameComboBox = (ComboBox)currentSpecNameWrapPanel.Children[1];


            if (currentModelComboBox.SelectedItem != null)
            {
                if (!commonQueriesObject.GetCompanyProducts(ref products, categories[currentCategoryComboBox.SelectedIndex].categoryId))
                    return;
                InitializeBrandCombo(products[currentTypeComboBox.SelectedIndex].typeId);

                if (!commonQueriesObject.GetCompanyModels(products[currentTypeComboBox.SelectedIndex], brands[currentBrandComboBox.SelectedIndex], ref models))
                    return;

                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    if (currentProductGrid == mainWrapPanel.Children[k])
                        maintContracts.SetMaintContractProductModel(k + 1, models[currentModelComboBox.SelectedIndex].modelId, models[currentModelComboBox.SelectedIndex].modelName);
                }

                if (categories[currentCategoryComboBox.SelectedIndex].categoryId == COMPANY_WORK_MACROS.UPS_CATEGORY_ID)
                {
                    if (!commonQueriesObject.GetModelSpecsNames(categories[currentCategoryComboBox.SelectedIndex].categoryId,products[currentTypeComboBox.SelectedIndex].typeId, brands[currentBrandComboBox.SelectedIndex].brandId, models[currentModelComboBox.SelectedIndex].modelId, ref modelSpecs))
                        return;
                    InitializeSpecNameCombo(currentSpecNameComboBox);

                    currentSpecNameWrapPanel.IsEnabled = true;

                }
                else if (categories[currentCategoryComboBox.SelectedIndex].categoryId == COMPANY_WORK_MACROS.GENSET_CATEGORY_ID)
                {
                    if (!commonQueriesObject.GetModelSpecsNames(categories[currentCategoryComboBox.SelectedIndex].categoryId,products[currentTypeComboBox.SelectedIndex].typeId, brands[currentBrandComboBox.SelectedIndex].brandId, models[currentModelComboBox.SelectedIndex].modelId, ref modelSpecs))
                        return;
                    InitializeSpecNameCombo(currentSpecNameComboBox);

                    currentSpecNameWrapPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    currentSpecNameWrapPanel.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    if (currentProductGrid == mainWrapPanel.Children[k])
                        maintContracts.SetMaintContractProductModel(k + 1, 0, "Others");
                }
                currentSpecNameWrapPanel.Visibility = Visibility.Collapsed;
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
                    {
                        maintContracts.SetMaintContractProductQuantity(k + 1, quantity);
                        maintContractsAdditionalInfoPage.ShowModelsSerialsGrid(k, quantity);
                    }

                    maintContractsPaymentAndDeliveryPage.SetTotalPriceTextBox();
                    maintContracts.SetMaintContractTotalValues();
                }
            }
            else
            {
                quantity = 0;
                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    if (currentProductGrid == mainWrapPanel.Children[k])
                    {
                        maintContracts.SetMaintContractProductQuantity(k + 1, quantity);
                        maintContractsAdditionalInfoPage.ShowModelsSerialsGrid(k, quantity);
                    }
                }
                currentQuantityTextBox.Text = null;

            }
        }
        private void PriceTextBoxesTextChanged(Object sender, TextChangedEventArgs e)
        {
            TextBox currentPriceTextBox = (TextBox)sender;
            WrapPanel currentPriceWrapPanel = (WrapPanel)currentPriceTextBox.Parent;
            Grid currentProductGrid = (Grid)currentPriceWrapPanel.Parent;

            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                if (IntegrityChecks.CheckInvalidCharacters(currentPriceTextBox.Text.ToString(), BASIC_MACROS.MONETARY_STRING) && currentPriceTextBox.Text != "")
                {
                    priceQuantity = Decimal.Parse(currentPriceTextBox.Text.ToString());

                    for (int k = 0; k < numberOfProductsAdded; k++)
                    {
                        if (currentProductGrid == mainWrapPanel.Children[k])
                        {
                            maintContracts.SetMaintContractProductPriceValue(k + 1, (int)priceQuantity);

                            maintContractsPaymentAndDeliveryPage.SetTotalPriceTextBox();
                            maintContracts.SetMaintContractTotalValues();

                        }
                    }
                }
                else
                {
                    priceQuantity = 0;
                    for (int k = 0; k < numberOfProductsAdded; k++)
                    {
                        if (currentProductGrid == mainWrapPanel.Children[k])
                            maintContracts.SetMaintContractProductPriceValue(k + 1, (int)priceQuantity);
                    }
                    currentPriceTextBox.Text = null;

                }
            }

        }

        private void PriceComboBoxesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox currentPriceComboBox = (ComboBox)sender;
            //WrapPanel currentPriceWrapPanel = (WrapPanel)currentPriceComboBox.Parent;
            //Grid currentProductGrid = (Grid)currentPriceWrapPanel.Parent;

            if (currentPriceComboBox.SelectedItem != null)
            {
                maintContracts.SetMaintContractCurrency(currencies[currentPriceComboBox.SelectedIndex].currencyId, currencies[currentPriceComboBox.SelectedIndex].currencyName);


                for (int i = 0; i < maintContracts.GetNoOfMaintContractSavedProducts(); i++)
                {
                    Grid productGrid = (Grid)mainWrapPanel.Children[i];
                    WrapPanel priceWrapPanel = (WrapPanel)productGrid.Children[7];
                    ComboBox currencyComboBox = (ComboBox)priceWrapPanel.Children[2];
                    currencyComboBox.SelectedItem = currentPriceComboBox.SelectedItem;
                }

                maintContractsPaymentAndDeliveryPage.SetTotalPriceCurrencyComboBox();

            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////CHECKBOX HANDLERS////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnCheckMainLabelCheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            Grid checkBoxColorGrid = (Grid)currentCheckBox.Parent;
            Grid currentProductGrid = (Grid)checkBoxColorGrid.Parent;

            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                if (currentProductGrid == mainWrapPanel.Children[i] && i > 0 && i < numberOfProductsAdded - 1)
                {
                    Grid nextProductGrid = (Grid)mainWrapPanel.Children[i + 1];
                    Grid nextCheckBoxColorGrid = (Grid)nextProductGrid.Children[0];
                    CheckBox nextCheckBox = (CheckBox)nextCheckBoxColorGrid.Children[0];
                    nextCheckBox.IsEnabled = true;
                }
            }

            WrapPanel currentCategoryWrapPanel = (WrapPanel)currentProductGrid.Children[1];
            ComboBox currentCategoryComboBox = (ComboBox)currentCategoryWrapPanel.Children[1];
            currentCategoryComboBox.IsEnabled = true;

            WrapPanel currentTypeWrapPanel = (WrapPanel)currentProductGrid.Children[2];
            ComboBox currentTypeComboBox = (ComboBox)currentTypeWrapPanel.Children[1];
            currentTypeComboBox.IsEnabled = false;

            WrapPanel currentBrandWrapPanel = (WrapPanel)currentProductGrid.Children[3];
            ComboBox currentBrandComboBox = (ComboBox)currentBrandWrapPanel.Children[1];
            currentBrandComboBox.IsEnabled = false;

            WrapPanel currentModelWrapPanel = (WrapPanel)currentProductGrid.Children[4];
            ComboBox currentModelComboBox = (ComboBox)currentModelWrapPanel.Children[1];
            currentModelComboBox.IsEnabled = false;
            
            WrapPanel currentSpecNameWrapPanel = (WrapPanel)currentProductGrid.Children[5];
            ComboBox currentSpecNameComboBox = (ComboBox)currentModelWrapPanel.Children[1];
            currentSpecNameComboBox.IsEnabled = false;

            WrapPanel currentQuantitWrapPanel = (WrapPanel)currentProductGrid.Children[6];
            TextBox currentQuantityTextBox = (TextBox)currentQuantitWrapPanel.Children[1];
            if(viewAddCondition != COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION)
                currentQuantityTextBox.IsEnabled = true;

            WrapPanel currentPriceWrapPanel = (WrapPanel)currentProductGrid.Children[7];
            TextBox currentPriceTextBox = (TextBox)currentPriceWrapPanel.Children[1];
            currentPriceTextBox.IsEnabled = true;
            ComboBox currentPriceCurrencyComboBox = (ComboBox)currentPriceWrapPanel.Children[2];
            currentPriceCurrencyComboBox.IsEnabled = true;
        }

        private void OnUnCheckMainLabelCheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            Grid checkBoxColorGrid = (Grid)currentCheckBox.Parent;
            Grid currentProductGrid = (Grid)checkBoxColorGrid.Parent;

            WrapPanel currentCategoryWrapPanel = (WrapPanel)currentProductGrid.Children[1];
            ComboBox currentCategoryComboBox = (ComboBox)currentCategoryWrapPanel.Children[1];

            WrapPanel currentTypeWrapPanel = (WrapPanel)currentProductGrid.Children[2];
            ComboBox currentTypeComboBox = (ComboBox)currentTypeWrapPanel.Children[1];

            WrapPanel currentBrandWrapPanel = (WrapPanel)currentProductGrid.Children[3];
            ComboBox currentBrandComboBox = (ComboBox)currentBrandWrapPanel.Children[1];

            WrapPanel currentModelWrapPanel = (WrapPanel)currentProductGrid.Children[4];
            ComboBox currentModelComboBox = (ComboBox)currentModelWrapPanel.Children[1];

            WrapPanel currentSpecNameWrapPanel = (WrapPanel)currentProductGrid.Children[5];
            ComboBox currentSpecNameComboBox = (ComboBox)currentSpecNameWrapPanel.Children[1];
            
            WrapPanel currentQuantityWrapPanel = (WrapPanel)currentProductGrid.Children[6];
            TextBox currentQuantityTextBox = (TextBox)currentQuantityWrapPanel.Children[1];

            WrapPanel currentPriceWrapPanel = (WrapPanel)currentProductGrid.Children[7];
            TextBox currentPriceTextBox = (TextBox)currentPriceWrapPanel.Children[1];

            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                if (currentProductGrid == mainWrapPanel.Children[i])
                {
                    if (i > 0 && i < numberOfProductsAdded - 1)
                    {
                        Grid nextProductGrid = (Grid)mainWrapPanel.Children[i + 1];
                        Grid nextCheckBoxColorGrid = (Grid)nextProductGrid.Children[0];
                        CheckBox nextCheckBox = (CheckBox)nextCheckBoxColorGrid.Children[0];

                        if (nextCheckBox.IsChecked == true)
                        {
                            WrapPanel nextCategoryWrapPanel = (WrapPanel)nextProductGrid.Children[1];
                            ComboBox nextCategoryCombo = (ComboBox)nextCategoryWrapPanel.Children[1];

                            WrapPanel nextTypeWrapPanel = (WrapPanel)nextProductGrid.Children[2];
                            ComboBox nextTypeCombo = (ComboBox)nextTypeWrapPanel.Children[1];

                            WrapPanel nextBrandWrapPanel = (WrapPanel)nextProductGrid.Children[3];
                            ComboBox nextBrandCombo = (ComboBox)nextBrandWrapPanel.Children[1];

                            WrapPanel nextModelWrapPanel = (WrapPanel)nextProductGrid.Children[4];
                            ComboBox nextModelCombo = (ComboBox)nextModelWrapPanel.Children[1];
                            
                            WrapPanel nextSpecNameWrapPanel = (WrapPanel)nextProductGrid.Children[5];
                            ComboBox nextSpecNameCombo = (ComboBox)nextSpecNameWrapPanel.Children[1];

                            WrapPanel nextQuantityWrapPanel = (WrapPanel)nextProductGrid.Children[6];
                            TextBox nextQuantityTextBox = (TextBox)nextQuantityWrapPanel.Children[1];

                            WrapPanel nextPriceWrapPanel = (WrapPanel)nextProductGrid.Children[7];
                            TextBox nextPriceTextBox = (TextBox)nextPriceWrapPanel.Children[1];

                            currentCategoryComboBox.SelectedItem = nextCategoryCombo.SelectedItem;
                            currentTypeComboBox.SelectedItem = nextTypeCombo.SelectedItem;
                            currentBrandComboBox.SelectedItem = nextBrandCombo.SelectedItem;
                            currentModelComboBox.SelectedItem = nextModelCombo.SelectedItem;
                            currentSpecNameComboBox.SelectedItem = nextSpecNameCombo.SelectedItem;
                            currentQuantityTextBox.Text = nextQuantityTextBox.Text;
                            currentPriceTextBox.Text = nextPriceTextBox.Text;

                            nextCheckBox.IsChecked = false;
                            currentCheckBox.IsChecked = true;
                        }
                        else
                        {
                            currentCategoryComboBox.SelectedItem = null;
                            currentCategoryComboBox.IsEnabled = false;

                            currentTypeComboBox.SelectedItem = null;
                            currentTypeComboBox.IsEnabled = false;


                            currentBrandComboBox.SelectedItem = null;
                            currentBrandComboBox.IsEnabled = false;


                            currentModelComboBox.SelectedItem = null;
                            currentModelComboBox.IsEnabled = false;


                            currentSpecNameComboBox.SelectedItem = null;
                            currentSpecNameComboBox.IsEnabled = false;

                            currentQuantityTextBox.Text = "0";
                            currentQuantityTextBox.IsEnabled = false;


                            currentPriceTextBox.Text = "0";
                            currentPriceTextBox.IsEnabled = false;
                            ComboBox currentPriceCurrencyComboBox = (ComboBox)currentPriceWrapPanel.Children[2];
                            currentPriceCurrencyComboBox.IsEnabled = false;

                            nextCheckBox.IsEnabled = false;
                        }
                    }
                    else
                    {
                        currentCategoryComboBox.SelectedItem = null;
                        currentCategoryComboBox.IsEnabled = false;

                        currentTypeComboBox.SelectedItem = null;
                        currentTypeComboBox.IsEnabled = false;


                        currentBrandComboBox.SelectedItem = null;
                        currentBrandComboBox.IsEnabled = false;


                        currentModelComboBox.SelectedItem = null;
                        currentModelComboBox.IsEnabled = false;


                        currentSpecNameComboBox.SelectedItem = null;
                        currentSpecNameComboBox.IsEnabled = false;

                        currentQuantityTextBox.Text = "0";
                        currentQuantityTextBox.IsEnabled = false;


                        currentPriceTextBox.Text = "0";
                        currentPriceTextBox.IsEnabled = false;
                        ComboBox currentPriceCurrencyComboBox = (ComboBox)currentPriceWrapPanel.Children[2];
                        currentPriceCurrencyComboBox.IsEnabled = false;
                    }
                }
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            //viewAddCondition = previousViewAddCondition;

            maintContractsBasicInfoPage.maintContractsProductsPage = this;
            maintContractsBasicInfoPage.maintContractsProjectInfoPage = maintContractsProjectsPage;
            maintContractsBasicInfoPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
            maintContractsBasicInfoPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
            maintContractsBasicInfoPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;

            NavigationService.Navigate(maintContractsBasicInfoPage);
        }
        private void OnClickProjectInfo(object sender, MouseButtonEventArgs e)
        {
            //viewAddCondition = previousViewAddCondition;

            maintContractsProjectsPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
            maintContractsProjectsPage.maintContractsProductsPage = this;
            maintContractsProjectsPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
            maintContractsProjectsPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
            maintContractsProjectsPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;

            NavigationService.Navigate(maintContractsProjectsPage);
        }
        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {

        }
        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {
            //viewAddCondition = previousViewAddCondition;

            maintContractsPaymentAndDeliveryPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
            maintContractsPaymentAndDeliveryPage.maintContractsProjectsPage = maintContractsProjectsPage;
            maintContractsPaymentAndDeliveryPage.maintContractsProductsPage = this;
            maintContractsPaymentAndDeliveryPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
            maintContractsPaymentAndDeliveryPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;

            NavigationService.Navigate(maintContractsPaymentAndDeliveryPage);
        }
        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            //viewAddCondition = previousViewAddCondition;

            maintContractsAdditionalInfoPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
            maintContractsAdditionalInfoPage.maintContractsProjectsPage = maintContractsProjectsPage;
            maintContractsAdditionalInfoPage.maintContractsProductsPage = this;
            maintContractsAdditionalInfoPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
            maintContractsAdditionalInfoPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;

            NavigationService.Navigate(maintContractsAdditionalInfoPage);

        }
        private void OnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            //viewAddCondition = previousViewAddCondition;

            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                maintContractsUploadFilesPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
                maintContractsUploadFilesPage.maintContractsProjectsPage = maintContractsProjectsPage;
                maintContractsUploadFilesPage.maintContractsProductsPage = this;
                maintContractsUploadFilesPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
                maintContractsUploadFilesPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;

                NavigationService.Navigate(maintContractsUploadFilesPage);
            }
        }

        private void OnClickNextButton(object sender, RoutedEventArgs e)
        {
            //viewAddCondition = previousViewAddCondition;

            maintContractsPaymentAndDeliveryPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
            maintContractsPaymentAndDeliveryPage.maintContractsProjectsPage = maintContractsProjectsPage;
            maintContractsPaymentAndDeliveryPage.maintContractsProductsPage = this;
            maintContractsPaymentAndDeliveryPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
            maintContractsPaymentAndDeliveryPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;

            NavigationService.Navigate(maintContractsPaymentAndDeliveryPage);
        }

        private void OnClickBackButton(object sender, RoutedEventArgs e)
        {
            //viewAddCondition = previousViewAddCondition;

            maintContractsProjectsPage.maintContractsBasicInfoPage = maintContractsBasicInfoPage;
            maintContractsProjectsPage.maintContractsProductsPage = this;
            maintContractsProjectsPage.maintContractsPaymentAndDeliveryPage = maintContractsPaymentAndDeliveryPage;
            maintContractsProjectsPage.maintContractsAdditionalInfoPage = maintContractsAdditionalInfoPage;
            maintContractsProjectsPage.maintContractsUploadFilesPage = maintContractsUploadFilesPage;

            NavigationService.Navigate(maintContractsProjectsPage);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///BUTTON CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void OnButtonClickAutomateMaintContracts(object sender, RoutedEventArgs e)
        {

        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;

            currentWindow.Close();
        }

    }
}
