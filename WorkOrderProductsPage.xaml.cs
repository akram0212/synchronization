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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for WorkOrderProductsPage.xaml
    /// </summary>
    public partial class WorkOrderProductsPage : Page
    {
        Employee loggedInUser;
        WorkOrder workOrder;

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private IntegrityChecks IntegrityChecks = new IntegrityChecks();

        private List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT> categories = new List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT>();
        private List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> products = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
        private List<COMPANY_WORK_MACROS.BRAND_STRUCT> brands = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();
        private List<COMPANY_WORK_MACROS.MODEL_STRUCT> models = new List<COMPANY_WORK_MACROS.MODEL_STRUCT>();

        //private List<COMPANY_WORK_MACROS.Order_PRODUCT_STRUCT> OrderProduct1 = new List<COMPANY_WORK_MACROS.Order_PRODUCT_STRUCT>();

        private List<BASIC_STRUCTS.CURRENCY_STRUCT> currencies = new List<BASIC_STRUCTS.CURRENCY_STRUCT>();

        private int viewAddCondition;
        private int numberOfProductsAdded;
        private int quantity;
        private decimal priceQuantity;

        public WorkOrderBasicInfoPage workOrderBasicInfoPage;
        public WorkOrderPaymentAndDeliveryPage workOrderPaymentAndDeliveryPage;
        public WorkOrderAdditionalInfoPage workOrderAdditionalInfoPage;
        public WorkOrderUploadFilesPage workOrderUploadFilesPage;
        public WorkOrderProjectInfoPage workOrderProjectInfoPage;

        public WorkOrderProductsPage(ref Employee mLoggedInUser, ref WorkOrder mWorkOrder, int mViewAddCondition, ref WorkOrderPaymentAndDeliveryPage mWorkOrderPaymentAndDeliveryPage)
        {
            workOrderPaymentAndDeliveryPage = mWorkOrderPaymentAndDeliveryPage;

            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            workOrder = mWorkOrder;

            numberOfProductsAdded = 0;

            InitializeComponent();



            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_ADD_CONDITION)
            {
                InitializeCategories();
                InitializePriceCurrencyComboBoxes();
                SetUpPageUIElements();
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                InitializePriceCurrencyComboBoxes();
                SetUpPageUIElements();
                //SetCategoryLabels();
                //SetTypeLabels();
                //SetBrandLabels();
                //SetModelLabels();
                //SetQuantityTextBoxes();
                //SetPriceTextBoxes();
                SetPriceComboBoxes();

                cancelButton.IsEnabled = false;
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION)
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
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INITIALIZATION FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetUpPageUIElements()
        {

            for (int i = 0; i < COMPANY_WORK_MACROS.MAX_ORDER_PRODUCTS; i++)
            {
                if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION && workOrder.GetOrderProductTypeId(i + 1) == 0)
                    continue;
                //if (viewAddCondition == 2 && workOrder.GetworkOrderProductTypeId(i + 1) == 0)
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

                currentProductGrid.RowDefinitions.Add(row1);
                currentProductGrid.RowDefinitions.Add(row2);
                currentProductGrid.RowDefinitions.Add(row3);
                currentProductGrid.RowDefinitions.Add(row4);
                currentProductGrid.RowDefinitions.Add(row5);
                currentProductGrid.RowDefinitions.Add(row6);
                currentProductGrid.RowDefinitions.Add(row7);

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
                    currentCategoryLabelValue.Content = workOrder.GetOrderProductCategory(i + 1);
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
                    currentTypeLabelValue.Content = workOrder.GetOrderProductType(i + 1);
                    productTypeWrapPanel.Children.Add(currentTypeLabelValue);
                }
                else
                {
                    ComboBox currentTypeCombo = new ComboBox();
                    currentTypeCombo.Style = (Style)FindResource("comboBoxStyle");
                    currentTypeCombo.SelectionChanged += new SelectionChangedEventHandler(TypeComboBoxesSelectionChanged);
                    if (i != 0)
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
                    currentBrandLabelValue.Content = workOrder.GetOrderProductBrand(i + 1);
                    productBrandWrapPanel.Children.Add(currentBrandLabelValue);
                }

                else
                {
                    ComboBox currentBrandCombo = new ComboBox();
                    currentBrandCombo.Style = (Style)FindResource("comboBoxStyle");
                    currentBrandCombo.SelectionChanged += new SelectionChangedEventHandler(BrandComboBoxesSelectionChanged);
                    if (i != 0)
                        currentBrandCombo.IsEnabled = false;
                    for (int j = 0; j < brands.Count(); j++)
                        currentBrandCombo.Items.Add(brands[j].brandName);
                    productBrandWrapPanel.Children.Add(currentBrandCombo);
                    currentBrandCombo.IsEnabled = false;
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
                    currentModelLabelValue.Content = workOrder.GetOrderProductModel(i + 1);
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

                if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                {
                    currentQuantityTextBox.IsEnabled = false;
                    currentQuantityTextBox.Text = workOrder.GetOrderProductQuantity(i + 1).ToString();
                }
                currentProductGrid.Children.Add(productQuantityWrapPanel);
                Grid.SetRow(productQuantityWrapPanel, 5);

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
                    currentPriceTextBox.Text = workOrder.GetOrderProductPriceValue(i + 1).ToString();
                    //currentPriceComboBox.SelectedItem = workOrder.GetOrderCurrency();
                }

                currentProductGrid.Children.Add(productPriceWrapPanel);
                Grid.SetRow(productPriceWrapPanel, 6);


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

        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///SET FUNCTIONS
        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        
        public void SetCategoryComboBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentCategoryWrapPanel = (WrapPanel)currentProductGrid.Children[1];
                ComboBox CurrentCategoryComboBox = (ComboBox)currentCategoryWrapPanel.Children[1];
                if(workOrder.GetOfferID() != null && viewAddCondition != COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION)
                    CurrentCategoryComboBox.SelectedItem = workOrder.GetOfferProductCategory(i + 1);
                else
                    CurrentCategoryComboBox.SelectedItem = workOrder.GetOrderProductCategory(i + 1);
            }
        }
        public void SetTypeComboBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {

                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentTypeWrapPanel = (WrapPanel)currentProductGrid.Children[2];
                ComboBox CurrentTypeComboBox = (ComboBox)currentTypeWrapPanel.Children[1];
                if (workOrder.GetOfferID() != null && viewAddCondition != COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION)
                    CurrentTypeComboBox.SelectedItem = workOrder.GetOfferProductType(i + 1);
                else
                    CurrentTypeComboBox.SelectedItem = workOrder.GetOrderProductType(i + 1);
            }
        }
        public void SetBrandComboBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentBrandWrapPanel = (WrapPanel)currentProductGrid.Children[3];
                ComboBox currentBrandComboBox = (ComboBox)currentBrandWrapPanel.Children[1];
                if (workOrder.GetOfferID() != null && viewAddCondition != COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION)
                    currentBrandComboBox.SelectedItem = workOrder.GetOfferProductBrand(i + 1);
                else
                    currentBrandComboBox.SelectedItem = workOrder.GetOrderProductBrand(i + 1);
            }
        }

        public void SetModelComboBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentModelWrapPanel = (WrapPanel)currentProductGrid.Children[4];
                ComboBox currentModelComboBox = (ComboBox)currentModelWrapPanel.Children[1];
                if (workOrder.GetOfferID() != null && viewAddCondition != COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION)
                    currentModelComboBox.SelectedItem = workOrder.GetOfferProductModel(i + 1);
                else
                    currentModelComboBox.SelectedItem = workOrder.GetOrderProductModel(i + 1);
            }
        }

        public void SetCategoryLabels()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentCategoryWrapPanel = (WrapPanel)currentProductGrid.Children[1];
                Label currentCategoryLabelValue = (Label)currentCategoryWrapPanel.Children[1];
                currentCategoryLabelValue.Content = workOrder.GetOrderProductCategory(i + 1);
            }
        }
        public void SetTypeLabels()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentTypeWrapPanel = (WrapPanel)currentProductGrid.Children[2];
                Label currentTypeLabelValue = (Label)currentTypeWrapPanel.Children[1];
                currentTypeLabelValue.Content = workOrder.GetOrderProductType(i + 1);
            }
        }

        public void SetBrandLabels()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentBrandWrapPanel = (WrapPanel)currentProductGrid.Children[3];
                Label currentBrandLabelValue = (Label)currentBrandWrapPanel.Children[1];
                currentBrandLabelValue.Content = workOrder.GetOrderProductBrand(i + 1);
            }
        }

        public void SetModelLabels()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentModelWrapPanel = (WrapPanel)currentProductGrid.Children[4];
                Label currentModelLabelValue = (Label)currentModelWrapPanel.Children[1];
                currentModelLabelValue.Content = workOrder.GetOrderProductModel(i + 1);
            }
        }

        public void SetQuantityTextBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {

                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentQuantityWrapPanel = (WrapPanel)currentProductGrid.Children[5];
                TextBox currentQuantityTextBoxValue = (TextBox)currentQuantityWrapPanel.Children[1];
                if (workOrder.GetOfferID() != null && viewAddCondition != COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION)
                    currentQuantityTextBoxValue.Text = workOrder.GetOfferProductQuantity(i + 1).ToString();
                else
                    currentQuantityTextBoxValue.Text = workOrder.GetOrderProductQuantity(i + 1).ToString();

            }
        }
        public void SetPriceTextBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {

                int price = 0;
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentPriceWrapPanel = (WrapPanel)currentProductGrid.Children[6];
                TextBox currentPriceTextBoxValue = (TextBox)currentPriceWrapPanel.Children[1];
                if (workOrder.GetOfferID() != null && viewAddCondition != COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION)
                    price = (int)workOrder.GetProductPriceValue(i + 1);
                else
                    price = (int)workOrder.GetOrderProductPriceValue(i + 1);
                currentPriceTextBoxValue.Text = price.ToString();
            }
        }
        public void SetPriceComboBoxes()
        {

            Grid currentPriceGrid = (Grid)mainWrapPanel.Children[0];
            WrapPanel currentProductWrapPanel = (WrapPanel)currentPriceGrid.Children[6];
            ComboBox currentPriceComboBox = (ComboBox)currentProductWrapPanel.Children[2];
            if (workOrder.GetOfferID() != null && viewAddCondition != COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION)
                currentPriceComboBox.SelectedItem = workOrder.GetCurrency();
            else
                currentPriceComboBox.SelectedItem = workOrder.GetOrderCurrency();


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

            WrapPanel currentBrandWrapPanel = (WrapPanel)currentProductGrid.Children[3];
            ComboBox currentBrandComboBox = (ComboBox)currentBrandWrapPanel.Children[1];

            products.Clear();
            currentTypeComboBox.Items.Clear();
            currentTypeComboBox.IsEnabled = false;
            currentBrandComboBox.Items.Clear();
            currentBrandComboBox.IsEnabled = false;

            if (currentCategoryComboBox.SelectedItem != null)
            {

                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    if (currentProductGrid == mainWrapPanel.Children[k])
                    {
                        if (k != 0)
                            currentProductCheckBox.IsChecked = true;

                        workOrder.SetOrderProductCategory(k + 1, categories[currentCategoryComboBox.SelectedIndex].categoryId, categories[currentCategoryComboBox.SelectedIndex].category);

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
                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    if (k != 0)
                        currentProductCheckBox.IsChecked = false;

                    if (currentProductGrid == mainWrapPanel.Children[k])
                        workOrder.SetOrderProductCategory(k + 1, 0, "Others");
                }
                currentTypeComboBox.IsEnabled = false;
                currentTypeComboBox.SelectedItem = null;
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

                        workOrder.SetOrderProductType(k + 1, products[currentTypeComboBox.SelectedIndex].typeId, products[currentTypeComboBox.SelectedIndex].typeName);

                        if (workOrder.GetOfferProductModelId(k + 1) != 0)
                        {
                            currentModelComboBox.SelectedItem = workOrder.GetOfferProductModel(k + 1);
                            workOrder.SetNoOfSavedOrderProducts();
                        }
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
                        workOrder.SetOrderProductType(k + 1, 0, "Others");
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
                        workOrder.SetOrderProductBrand(k + 1, brands[currentBrandComboBox.SelectedIndex].brandId, currentBrandComboBox.SelectedItem.ToString());

                        //currentModelComboBox.SelectedItem = workOrder.GetOfferProductModel(k + 1);
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
                        workOrder.SetOrderProductBrand(k + 1, 0, "Others");
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


            if (currentModelComboBox.SelectedItem != null)
            {
                if (!commonQueriesObject.GetCompanyProducts(ref products, categories[currentCategoryComboBox.SelectedIndex].categoryId))
                    return;
                if (!commonQueriesObject.GetProductBrands(products[currentTypeComboBox.SelectedIndex].typeId, ref brands))
                    return;
                if (!commonQueriesObject.GetCompanyModels(products[currentTypeComboBox.SelectedIndex], brands[currentBrandComboBox.SelectedIndex], ref models))
                    return;

                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    if (currentProductGrid == mainWrapPanel.Children[k])
                        workOrder.SetOrderProductModel(k + 1, models[currentModelComboBox.SelectedIndex].modelId, models[currentModelComboBox.SelectedIndex].modelName);
                }
            }
            else
            {
                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    if (currentProductGrid == mainWrapPanel.Children[k])
                        workOrder.SetOrderProductModel(k + 1, 0, "Others");
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
                        workOrder.SetOrderProductQuantity(k + 1, quantity);
                }
            }
            else
            {
                quantity = 0;
                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    if (currentProductGrid == mainWrapPanel.Children[k])
                        workOrder.SetOrderProductQuantity(k + 1, quantity);
                }
                currentQuantityTextBox.Text = null;

                //CALL TOTAL PRICE HERE
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
                            workOrder.SetOrderProductPriceValue(k + 1, (int)priceQuantity);
                            //if(viewAddCondition == COMPANY_WORK_MACROS.Order_REVISE_CONDITION)
                            //    workOrderPaymentAndDeliveryPage = workOrderBasicInfoPage.workOrderPaymentAndDeliveryPage;
                            workOrderPaymentAndDeliveryPage.SetTotalPriceTextBox();

                            //REMOVE THIS FROM HERE, SHALL BE ADDED ON ANY SELECTION CHANGE FOR CURRENCY COMBO BOX
                           
                        }
                    }
                }
                else
                {
                    priceQuantity = 0;
                    for (int k = 0; k < numberOfProductsAdded; k++)
                    {
                        if (currentProductGrid == mainWrapPanel.Children[k])
                            workOrder.SetOrderProductPriceValue(k + 1, (int)priceQuantity);
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
                workOrder.SetOrderCurrency(currencies[currentPriceComboBox.SelectedIndex].currencyId, currencies[currentPriceComboBox.SelectedIndex].currencyName);

                if (workOrder.GetOfferID() != null)
                {
                    for (int i = 0; i < workOrder.GetNoOfOfferSavedProducts(); i++)
                    {
                        Grid productGrid = (Grid)mainWrapPanel.Children[i];
                        WrapPanel priceWrapPanel = (WrapPanel)productGrid.Children[6];
                        ComboBox currencyComboBox = (ComboBox)priceWrapPanel.Children[2];
                        currencyComboBox.SelectedItem = currentPriceComboBox.SelectedItem;
                    }
                }
                else
                {
                    for (int i = 0; i < workOrder.GetNoOfOrderSavedProducts(); i++)
                    {
                        Grid productGrid = (Grid)mainWrapPanel.Children[i];
                        WrapPanel priceWrapPanel = (WrapPanel)productGrid.Children[6];
                        ComboBox currencyComboBox = (ComboBox)priceWrapPanel.Children[2];
                        currencyComboBox.SelectedItem = currentPriceComboBox.SelectedItem;
                    }
                }
                workOrderPaymentAndDeliveryPage.SetTotalPriceCurrencyComboBox();
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

            WrapPanel currentQuantitWrapPanel = (WrapPanel)currentProductGrid.Children[5];
            TextBox currentQuantityTextBox = (TextBox)currentQuantitWrapPanel.Children[1];
            currentQuantityTextBox.IsEnabled = true;

            WrapPanel currentPriceWrapPanel = (WrapPanel)currentProductGrid.Children[6];
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

            WrapPanel currentQuantityWrapPanel = (WrapPanel)currentProductGrid.Children[5];
            TextBox currentQuantityTextBox = (TextBox)currentQuantityWrapPanel.Children[1];

            WrapPanel currentPriceWrapPanel = (WrapPanel)currentProductGrid.Children[6];
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

                            WrapPanel nextQuantityWrapPanel = (WrapPanel)nextProductGrid.Children[5];
                            TextBox nextQuantityTextBox = (TextBox)nextQuantityWrapPanel.Children[1];

                            WrapPanel nextPriceWrapPanel = (WrapPanel)nextProductGrid.Children[6];
                            TextBox nextPriceTextBox = (TextBox)nextPriceWrapPanel.Children[1];

                            currentCategoryComboBox.SelectedItem = nextCategoryCombo.SelectedItem;
                            currentTypeComboBox.SelectedItem = nextTypeCombo.SelectedItem;
                            currentBrandComboBox.SelectedItem = nextBrandCombo.SelectedItem;
                            currentModelComboBox.SelectedItem = nextModelCombo.SelectedItem;
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
            workOrderBasicInfoPage.workOrderProductsPage = this;
            workOrderBasicInfoPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
            workOrderBasicInfoPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderBasicInfoPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderBasicInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderBasicInfoPage);
        }
        private void OnClickProjectInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderProjectInfoPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderProjectInfoPage.workOrderProductsPage = this;
            workOrderProjectInfoPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderProjectInfoPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderProjectInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderProjectInfoPage);
        }
        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {

        }
        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {

            workOrderPaymentAndDeliveryPage.SetTotalPriceTextBox();
            workOrderPaymentAndDeliveryPage.SetTotalPriceCurrencyComboBox();
            workOrderPaymentAndDeliveryPage.SetDownPaymentValues();
            workOrderPaymentAndDeliveryPage.SetOnDeliveryValues();
            workOrderPaymentAndDeliveryPage.SetOnInstallationValues();
            workOrderPaymentAndDeliveryPage.SetDeliveryTimeValues();
            workOrderPaymentAndDeliveryPage.SetDeliveryPointValue();

            workOrderPaymentAndDeliveryPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderProductsPage = this;
            workOrderPaymentAndDeliveryPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderPaymentAndDeliveryPage);
        }
        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderAdditionalInfoPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderAdditionalInfoPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
            workOrderAdditionalInfoPage.workOrderProductsPage = this;
            workOrderAdditionalInfoPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderAdditionalInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderAdditionalInfoPage);

        }
        private void OnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                workOrderUploadFilesPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
                workOrderUploadFilesPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
                workOrderUploadFilesPage.workOrderProductsPage = this;
                workOrderUploadFilesPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
                workOrderUploadFilesPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;

                NavigationService.Navigate(workOrderUploadFilesPage);
            }
        }

        private void OnClickNextButton(object sender, RoutedEventArgs e)
        {
            workOrderPaymentAndDeliveryPage.SetTotalPriceTextBox();
            workOrderPaymentAndDeliveryPage.SetTotalPriceCurrencyComboBox();
            workOrderPaymentAndDeliveryPage.SetDownPaymentValues();
            workOrderPaymentAndDeliveryPage.SetOnDeliveryValues();
            workOrderPaymentAndDeliveryPage.SetOnInstallationValues();
            workOrderPaymentAndDeliveryPage.SetDeliveryTimeValues();
            workOrderPaymentAndDeliveryPage.SetDeliveryPointValue();

            workOrderPaymentAndDeliveryPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderProjectInfoPage = workOrderProjectInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderProductsPage = this;
            workOrderPaymentAndDeliveryPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderPaymentAndDeliveryPage);
        }

        private void OnClickBackButton(object sender, RoutedEventArgs e)
        {
            workOrderProjectInfoPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderProjectInfoPage.workOrderProductsPage = this;
            workOrderProjectInfoPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderProjectInfoPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderProjectInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;

            NavigationService.Navigate(workOrderProjectInfoPage);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///BUTTON CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void OnButtonClickAutomateWorkOrder(object sender, RoutedEventArgs e)
        {

        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;

            currentWindow.Close();
        }

        
    }
}
