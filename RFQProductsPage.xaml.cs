using _01electronics_library;
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
    /// Interaction logic for RFQProductsPage.xaml
    /// </summary>
    public partial class RFQProductsPage : Page
    {
        protected Employee loggedInUser;
        protected RFQ rfq;

        protected SQLServer sqlDatabase;

        protected CommonQueries commonQueriesObject;
        protected CommonFunctions commonFunctionsObject;

        protected IntegrityChecks IntegrityChecks = new IntegrityChecks();

        protected List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT> categories = new List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT>();
        protected List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> products = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
        protected List<COMPANY_WORK_MACROS.BRAND_STRUCT> brands = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();
        protected List<COMPANY_WORK_MACROS.MODEL_STRUCT> models = new List<COMPANY_WORK_MACROS.MODEL_STRUCT>();

        protected int quantity1;

        protected int viewAddCondition;
        protected int numberOfProductsAdded;

        public RFQBasicInfoPage rfqBasicInfoPage;
        public RFQAdditionalInfoPage rfqAdditionalInfoPage;
        public RFQUploadFilesPage rfqUploadFilesPage;

        public RFQProductsPage(ref Employee mLoggedInUser, ref RFQ mRFQ, int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();
            sqlDatabase = new SQLServer();
            rfq = new RFQ(sqlDatabase);
            rfq = mRFQ;

            InitializeComponent();

            numberOfProductsAdded = 0;

            if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_ADD_CONDITION)
            {
                InitializeCategories();
                SetUpPageUIElements();
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
            {
                SetUpPageUIElements();
                //SetCategoryLabels();
                //SetTypeLabels();
                //SetBrandLabels();
                //SetModelLabels();
                //SetQuantityTextBoxes();
                //
                cancelButton.IsEnabled = false;
            }
            else
            {
                InitializeCategories();
                SetUpPageUIElements();
                SetCategoryComboBoxes();
                SetTypeComboBoxes();
                SetBrandComboBoxes();
                SetModelComboBoxes();
                SetQuantityTextBoxes();
            }
        }
        //////////INITIALIZE FUNCTIONS//////////
        ////////////////////////////////////////////////////////////////////////////////////////////////

        private bool InitializeCategories()
        {
            if (!commonQueriesObject.GetProductCategories(ref categories))
                return false;

            return true;
        }

        private bool InitializeProducts()
        {
            if (!commonQueriesObject.GetCompanyProducts(ref products))
                return false;

            return true;
        }

        private bool InitializeBrandCombo()
        {
            if (!commonQueriesObject.GetCompanyBrands(ref brands))
                return false;
            return true;
        }

        ////////SET FUNCTIONS////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetCategoryComboBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentCategoryWrapPanel = (WrapPanel)currentProductGrid.Children[1];
                ComboBox CurrentCategoryComboBox = (ComboBox)currentCategoryWrapPanel.Children[1];
                if (rfq.GetRFQProductCategoryId(i + 1) != 0)
                    CurrentCategoryComboBox.Text = rfq.GetRFQProductCategory(i + 1);
            }
        }

        private void SetTypeComboBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentTypeWrapPanel = (WrapPanel)currentProductGrid.Children[2];
                ComboBox CurrentTypeComboBox = (ComboBox)currentTypeWrapPanel.Children[1];
                if (rfq.GetRFQProductTypeId(i + 1) != 0)
                    CurrentTypeComboBox.Text = rfq.GetRFQProductType(i + 1);
            }
        }

        private void SetBrandComboBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentBrandWrapPanel = (WrapPanel)currentProductGrid.Children[3];
                ComboBox currentBrandComboBox = (ComboBox)currentBrandWrapPanel.Children[1];

                currentBrandComboBox.Text = rfq.GetRFQProductBrand(i + 1);
            }
        }
        private void SetModelComboBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentModelWrapPanel = (WrapPanel)currentProductGrid.Children[4];
                ComboBox currentModelComboBox = (ComboBox)currentModelWrapPanel.Children[1];

                currentModelComboBox.SelectedItem = rfq.GetRFQProductModel(i + 1);
            }
        }

        private void SetCategoryLabels()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentCategoryWrapPanel = (WrapPanel)currentProductGrid.Children[1];
                Label currentCategoryLabelValue = (Label)currentCategoryWrapPanel.Children[1];
                currentCategoryLabelValue.Content = rfq.GetRFQProductCategory(i + 1);
            }
        }
        private void SetTypeLabels()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentTypeWrapPanel = (WrapPanel)currentProductGrid.Children[2];
                Label currentTypeLabelValue = (Label)currentTypeWrapPanel.Children[1];
                currentTypeLabelValue.Content = rfq.GetRFQProductType(i + 1);
            }
        }

        private void SetBrandLabels()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentBrandWrapPanel = (WrapPanel)currentProductGrid.Children[3];
                Label currentBrandLabelValue = (Label)currentBrandWrapPanel.Children[1];
                currentBrandLabelValue.Content = rfq.GetRFQProductBrand(i + 1);
            }
        }

        private void SetModelLabels()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentModelWrapPanel = (WrapPanel)currentProductGrid.Children[4];
                Label currentModelLabelValue = (Label)currentModelWrapPanel.Children[1];
                currentModelLabelValue.Content = rfq.GetRFQProductModel(i + 1);
            }
        }

        private void SetQuantityTextBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentQuantityWrapPanel = (WrapPanel)currentProductGrid.Children[5];
                TextBox currentQuantityTextBoxValue = (TextBox)currentQuantityWrapPanel.Children[1];
                currentQuantityTextBoxValue.Text = rfq.GetRFQProductQuantity(i + 1).ToString();
            }
        }
        public void SetUpPageUIElements()
        {

            for (int i = 0; i < COMPANY_WORK_MACROS.MAX_RFQ_PRODUCTS; i++)
            {
                if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION && rfq.GetRFQProductTypeId(i + 1) == 0)
                    continue;
                //if (viewAddCondition == 2 && rfq.GetRFQProductTypeId(i + 1) == 0)
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


                if (i == 0 || viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
                    mainLabelCheckBox.IsEnabled = false;
                else if (i == 1 && viewAddCondition != COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
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
                currentCategoryLabel.Style = (Style)FindResource("tableItemLabel");
                productCategoryWrapPanel.Children.Add(currentCategoryLabel);

                if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
                {
                    Label currentCategoryLabelValue = new Label();
                    currentCategoryLabelValue.Style = (Style)FindResource("tableItemValue");
                    currentCategoryLabelValue.Width = 150;
                    currentCategoryLabelValue.Content = rfq.GetRFQProductCategory(i + 1);
                    productCategoryWrapPanel.Children.Add(currentCategoryLabelValue);
                }
                else
                {
                    ComboBox currentCategoryCombo = new ComboBox();
                    currentCategoryCombo.Style = (Style)FindResource("comboBoxStyle");
                    currentCategoryCombo.SelectionChanged += new SelectionChangedEventHandler(CategoryComboBoxesSelectionChanged);

                    if (i != 0)
                    {
                        currentCategoryCombo.IsEnabled = false;
                    }

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
                currentTypeLabel.Style = (Style)FindResource("tableItemLabel");
                productTypeWrapPanel.Children.Add(currentTypeLabel);

                if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
                {
                    Label currentTypeLabelValue = new Label();
                    currentTypeLabelValue.Style = (Style)FindResource("tableItemValue");
                    currentTypeLabelValue.Width = 150;
                    currentTypeLabelValue.Content = rfq.GetRFQProductType(i + 1);
                    productTypeWrapPanel.Children.Add(currentTypeLabelValue);
                }
                else
                {
                    ComboBox currentTypeCombo = new ComboBox();
                    currentTypeCombo.Style = (Style)FindResource("comboBoxStyle");
                    currentTypeCombo.SelectionChanged += new SelectionChangedEventHandler(TypeComboBoxesSelectionChanged);
                    currentTypeCombo.IsEnabled = false;

                    if (i != 0)
                    {
                        currentTypeCombo.IsEnabled = false;
                    }

                    for (int j = 0; j < products.Count(); j++)
                        currentTypeCombo.Items.Add(products[j].typeName);
                    productTypeWrapPanel.Children.Add(currentTypeCombo);
                }

                currentProductGrid.Children.Add(productTypeWrapPanel);
                Grid.SetRow(productTypeWrapPanel, 2);

                ////////BRAND WRAPPANEL////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////////
                WrapPanel productBrandWrapPanel = new WrapPanel();

                Label currentBrandLabel = new Label();
                currentBrandLabel.Content = "Brand";
                currentBrandLabel.Style = (Style)FindResource("tableItemLabel");
                productBrandWrapPanel.Children.Add(currentBrandLabel);

                if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
                {
                    Label currentBrandLabelValue = new Label();
                    currentBrandLabelValue.Style = (Style)FindResource("tableItemValue");
                    currentBrandLabelValue.Width = 150;
                    currentBrandLabelValue.Content = rfq.GetRFQProductBrand(i + 1);
                    productBrandWrapPanel.Children.Add(currentBrandLabelValue);
                }

                else
                {
                    ComboBox currentBrandCombo = new ComboBox();
                    currentBrandCombo.Style = (Style)FindResource("comboBoxStyle");
                    currentBrandCombo.SelectionChanged += new SelectionChangedEventHandler(BrandComboBoxesSelectionChanged);
                    currentBrandCombo.IsEnabled = false;
                    if (i != 0)
                    {
                        currentBrandCombo.IsEnabled = false;
                    }

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
                currentModelLabel.Style = (Style)FindResource("tableItemLabel");
                productModelWrapPanel.Children.Add(currentModelLabel);

                if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
                {
                    Label currentModelLabelValue = new Label();
                    currentModelLabelValue.Style = (Style)FindResource("tableItemValue");
                    currentModelLabelValue.Width = 150;
                    currentModelLabelValue.Content = rfq.GetRFQProductModel(i + 1);
                    productModelWrapPanel.Children.Add(currentModelLabelValue);
                }
                else
                {
                    ComboBox currentModelCombo = new ComboBox();
                    currentModelCombo.Style = (Style)FindResource("comboBoxStyle");
                    currentModelCombo.IsEnabled = false;

                    currentModelCombo.SelectionChanged += new SelectionChangedEventHandler(ModelComboBoxesSelectionChanged);
                    productModelWrapPanel.Children.Add(currentModelCombo);
                }
                currentProductGrid.Children.Add(productModelWrapPanel);
                Grid.SetRow(productModelWrapPanel, 4);

                /////////////QUANTITY WRAPPANEL///////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                WrapPanel productQuantityWrapPanel = new WrapPanel();

                Label currentQuantityLabel = new Label();
                currentQuantityLabel.Content = "Quantity*";
                currentQuantityLabel.Style = (Style)FindResource("tableItemLabel");
                productQuantityWrapPanel.Children.Add(currentQuantityLabel);

                TextBox currentQuantityTextBox = new TextBox();
                currentQuantityTextBox.Style = (Style)FindResource("textBoxStyle");
                currentQuantityTextBox.TextChanged += new TextChangedEventHandler(QuantityTextBoxesTextChanged);
                //currentQuantityTextBox.Margin = new Thickness(-300, 12, 12, 12);
                productQuantityWrapPanel.Children.Add(currentQuantityTextBox);

                //currentQuantityTextBox.Text = rfq.GetRFQProductQuantity(i + 1).ToString();

                if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
                {
                    currentQuantityTextBox.IsEnabled = false;
                    currentQuantityTextBox.Text = rfq.GetRFQProductQuantity(i + 1).ToString();
                }
                currentProductGrid.Children.Add(productQuantityWrapPanel);
                Grid.SetRow(productQuantityWrapPanel, 5);

                mainWrapPanel.Children.Add(currentProductGrid);

                numberOfProductsAdded += 1;
            }
        }

        //////////////SELECTION CHANGED HANDLERS///////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CategoryComboBoxesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBox currentCategoryComboBox = (ComboBox)sender;
            WrapPanel currentCategoryWrapPanel = (WrapPanel)currentCategoryComboBox.Parent;
            Grid currentProductGrid = (Grid)currentCategoryWrapPanel.Parent;

            WrapPanel currentTypeWrapPanel = (WrapPanel)currentProductGrid.Children[2];
            ComboBox currentTypeComboBox = (ComboBox)currentTypeWrapPanel.Children[1];

            Grid checkBoxGrid = (Grid)currentProductGrid.Children[0];
            CheckBox currentProductCheckBox = (CheckBox)checkBoxGrid.Children[0];

            currentTypeComboBox.Items.Clear();
            currentTypeComboBox.SelectedItem = null;
            currentTypeComboBox.IsEnabled = false;

            if (currentCategoryComboBox.SelectedItem != null)
            {
                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    if (currentProductGrid == mainWrapPanel.Children[k])
                    {
                        if (k != 0)
                            currentProductCheckBox.IsChecked = true;

                        rfq.SetRFQProductCategory(k + 1, categories[currentCategoryComboBox.SelectedIndex].categoryId, categories[currentCategoryComboBox.SelectedIndex].category);

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
                    if (currentProductGrid == mainWrapPanel.Children[k])
                        rfq.SetRFQProductCategory(k + 1, 0, "Others");
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
                if (!commonQueriesObject.GetProductBrands(products[currentTypeComboBox.SelectedIndex].typeId, ref brands))
                    return;
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

                        rfq.SetRFQProductType(k + 1, products[currentTypeComboBox.SelectedIndex].typeId, products[currentTypeComboBox.SelectedIndex].typeName);
                    }
                }
            }
            else
            {
                currentBrandComboBox.IsEnabled = false;
                currentBrandComboBox.SelectedItem = null;

                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    if (currentProductGrid == mainWrapPanel.Children[k])
                        rfq.SetRFQProductType(k + 1, 0, "Others");
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

                    if (!commonQueriesObject.GetProductBrands(products[currentTypeComboBox.SelectedIndex].typeId, ref brands))
                        return;

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
                        rfq.SetRFQProductBrand(k + 1, brands[currentBrandComboBox.SelectedIndex].brandId, currentBrandComboBox.SelectedItem.ToString());

                        //currentModelComboBox.SelectedItem = rfq.GetRFQProductModel(k + 1);
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
                        rfq.SetRFQProductBrand(k + 1, 0, "Others");
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
                        rfq.SetRFQProductModel(k + 1, models[currentModelComboBox.SelectedIndex].modelId, models[currentModelComboBox.SelectedIndex].modelName);
                }
            }
            else
            {
                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    if (currentProductGrid == mainWrapPanel.Children[k])
                        rfq.SetRFQProductModel(k + 1, 0, "Others");
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
                quantity1 = int.Parse(currentQuantityTextBox.Text);
                for (int k = 0; k < numberOfProductsAdded; k++)
                {
                    if (currentProductGrid == mainWrapPanel.Children[k])
                        rfq.SetRFQProductQuantity(k + 1, quantity1);
                }
            }
            else
            {
                quantity1 = 0;
                currentQuantityTextBox.Text = null;
            }
        }
        ////////////CHECK BOXES CHECKED HANDLERS////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////

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

        }

        ///////////CHECK BOXES UNCHECKED EVENT HANDLERS//
        /////////////////////////////////////////////////////////////////////////////////////////////////////////

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

            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                if (currentProductGrid == mainWrapPanel.Children[i])
                {
                    if (i > 0 && i < COMPANY_WORK_MACROS.MAX_OUTGOING_QUOTATION_PRODUCTS - 1)
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

                            currentCategoryComboBox.SelectedItem = nextCategoryCombo.SelectedItem;
                            currentTypeComboBox.SelectedItem = nextTypeCombo.SelectedItem;
                            currentBrandComboBox.SelectedItem = nextBrandCombo.SelectedItem;
                            currentModelComboBox.SelectedItem = nextModelCombo.SelectedItem;
                            currentQuantityTextBox.Text = nextQuantityTextBox.Text;

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
                    }
                }
            }
        }

        ////////////BUTTON CLICKS///////////
        ////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void OnBtnClickBack(object sender, RoutedEventArgs e)
        {
            rfqBasicInfoPage.rfqProductsPage = this;
            rfqBasicInfoPage.rfqAdditionalInfoPage = rfqAdditionalInfoPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
                rfqBasicInfoPage.rfqUploadFilesPage = rfqUploadFilesPage;

            NavigationService.Navigate(rfqBasicInfoPage);
        }

        private void OnBtnClickNext(object sender, RoutedEventArgs e)
        {
            rfqAdditionalInfoPage.rfqBasicInfoPage = rfqBasicInfoPage;
            rfqAdditionalInfoPage.rfqProductsPage = this;

            if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
                rfqAdditionalInfoPage.rfqUploadFilesPage = rfqUploadFilesPage;

            NavigationService.Navigate(rfqAdditionalInfoPage);
        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;
            currentWindow.Close();
        }

        private void OnBtnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            rfqBasicInfoPage.rfqProductsPage = this;
            rfqBasicInfoPage.rfqAdditionalInfoPage = rfqAdditionalInfoPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
                rfqBasicInfoPage.rfqUploadFilesPage = rfqUploadFilesPage;

            NavigationService.Navigate(rfqBasicInfoPage);
        }

        private void OnBtnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            //rfq.SetNoOfSavedRFQProducts();

            rfqAdditionalInfoPage.rfqBasicInfoPage = rfqBasicInfoPage;
            rfqAdditionalInfoPage.rfqProductsPage = this;

            if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
                rfqAdditionalInfoPage.rfqUploadFilesPage = rfqUploadFilesPage;

            NavigationService.Navigate(rfqAdditionalInfoPage);
        }

        private void OnBtnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
            {
                rfqUploadFilesPage.rfqBasicInfoPage = rfqBasicInfoPage;
                rfqUploadFilesPage.rfqProductsPage = this;
                rfqUploadFilesPage.rfqAdditionalInfoPage = rfqAdditionalInfoPage;

                NavigationService.Navigate(rfqUploadFilesPage);
            }
        }
    }
}
