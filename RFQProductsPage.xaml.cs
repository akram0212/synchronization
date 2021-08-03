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
        private int typeId;
        private int brandId;
        private int modelId;

        private int viewAddCondition;
        private int numberOfProductsAdded;
        

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

            if (viewAddCondition == 1)
            {
                InitializeProducts();
                InitializeBrandCombo();
                SetUpPageUIElements();
            }
            else if (viewAddCondition == 0)
            {
                SetUpPageUIElements();
                SetTypeLabels();
                SetBrandLabels();
                SetModelLabels();
                SetQuantityTextBoxes();
            }
            else
            {
                InitializeProducts();
                InitializeBrandCombo();
                SetUpPageUIElements();
                SetTypeComboBoxes();
                SetBrandComboBoxes();
                SetModelComboBoxes();
                SetQuantityTextBoxes();
            }
        }
        //////////INITIALIZE FUNCTIONS//////////
        ////////////////////////////////////////
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

        ////////SET FUNCTIONS////////////////////
        /////////////////////////////////////////
        
        private void SetTypeComboBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentTypeWrapPanel = (WrapPanel)currentProductGrid.Children[1];
                ComboBox CurrentTypeComboBox = (ComboBox)currentTypeWrapPanel.Children[1];
                CurrentTypeComboBox.Text = rfq.GetRFQProductType(i + 1);
            }
        }

        private void SetBrandComboBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentBrandWrapPanel = (WrapPanel)currentProductGrid.Children[2];
                ComboBox currentBrandComboBox = (ComboBox)currentBrandWrapPanel.Children[1];
                currentBrandComboBox.Text = rfq.GetRFQProductBrand(i + 1);
            }
        }
        private void SetModelComboBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentModelWrapPanel = (WrapPanel)currentProductGrid.Children[3];
                ComboBox currentModelComboBox = (ComboBox)currentModelWrapPanel.Children[1];
                currentModelComboBox.SelectedItem = rfq.GetRFQProductModel(i + 1);
            }
        }
        private void SetTypeLabels()
        {
            for(int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentTypeWrapPanel = (WrapPanel)currentProductGrid.Children[1];
                Label currentTypeLabelValue = (Label)currentTypeWrapPanel.Children[1];
                currentTypeLabelValue.Content = rfq.GetRFQProductType(i + 1);
            }
        }

        private void SetBrandLabels()
        {
            for(int i = 0; i< numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentBrandWrapPanel = (WrapPanel)currentProductGrid.Children[2];
                Label currentBrandLabelValue = (Label)currentBrandWrapPanel.Children[1];
                currentBrandLabelValue.Content = rfq.GetRFQProductBrand(i + 1);
            }
        }

        private void SetModelLabels()
        {
            for(int i = 0; i< numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentModelWrapPanel = (WrapPanel)currentProductGrid.Children[3];
                Label currentModelLabelValue = (Label)currentModelWrapPanel.Children[1];
                currentModelLabelValue.Content = rfq.GetRFQProductModel(i + 1);
            }
        }

        private void SetQuantityTextBoxes()
        {
            for (int i = 0; i < numberOfProductsAdded; i++)
            {
                Grid currentProductGrid = (Grid)mainWrapPanel.Children[i];
                WrapPanel currentQuantityWrapPanel = (WrapPanel)currentProductGrid.Children[4];
                TextBox currentQuantityTextBoxValue = (TextBox)currentQuantityWrapPanel.Children[1];
                currentQuantityTextBoxValue.Text= rfq.GetRFQProductQuantity(i + 1).ToString();
            }
        }
        public void SetUpPageUIElements()
        {

            for (int i = 0; i < COMPANY_WORK_MACROS.MAX_RFQ_PRODUCTS; i++)
            {
                if (viewAddCondition == 0 && rfq.GetRFQProductTypeId(i + 1) == 0)
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

                currentProductGrid.RowDefinitions.Add(row1);
                currentProductGrid.RowDefinitions.Add(row2);
                currentProductGrid.RowDefinitions.Add(row3);
                currentProductGrid.RowDefinitions.Add(row4);
                currentProductGrid.RowDefinitions.Add(row5);

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
                currentTypeLabel.Style = (Style)FindResource("tableItemLabel");
                productTypeWrapPanel.Children.Add(currentTypeLabel);

                if (viewAddCondition == 0)
                {
                    Label currentTypeLabelValue = new Label();
                    currentTypeLabelValue.Style = (Style)FindResource("tableItemValue");
                    //currentTypeLabelValue.Margin = new Thickness(-300, 12, 12, 12);
                    currentTypeLabelValue.Content = rfq.GetRFQProductType(i + 1);
                    productTypeWrapPanel.Children.Add(currentTypeLabelValue);
                }
                else
                {
                    ComboBox currentTypeCombo = new ComboBox();
                    currentTypeCombo.Style = (Style)FindResource("comboBoxStyle");
                    //currentTypeCombo.Margin = new Thickness(-300, 12, 12, 12);
                    currentTypeCombo.SelectionChanged += new SelectionChangedEventHandler(TypeComboBoxesSelectionChanged);
                    if (viewAddCondition == 0)
                        currentTypeCombo.Visibility = Visibility.Collapsed;
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
                currentBrandLabel.Style = (Style)FindResource("tableItemLabel");
                productBrandWrapPanel.Children.Add(currentBrandLabel);

                if (viewAddCondition == 0)
                {
                    Label currentBrandLabelValue = new Label();
                    currentBrandLabelValue.Style = (Style)FindResource("tableItemValue");
                    //currentBrandLabelValue.Margin = new Thickness(-300, 12, 12, 12);
                    currentBrandLabelValue.Content = rfq.GetRFQProductBrand(i + 1);
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
                currentModelLabel.Style = (Style)FindResource("tableItemLabel");
                productModelWrapPanel.Children.Add(currentModelLabel);

                if (viewAddCondition == 0)
                {
                    Label currentModelLabelValue = new Label();
                    currentModelLabelValue.Style = (Style)FindResource("tableItemValue");
                    //currentModelLabelValue.Margin = new Thickness(-300, 12, 12, 12);
                    currentModelLabelValue.Content = rfq.GetRFQProductModel(i + 1);
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
                currentQuantityLabel.Style = (Style)FindResource("tableItemLabel");
                productQuantityWrapPanel.Children.Add(currentQuantityLabel);

                TextBox currentQuantityTextBox = new TextBox();
                currentQuantityTextBox.Style = (Style)FindResource("textBoxStyle");
                currentQuantityTextBox.TextChanged += new TextChangedEventHandler(QuantityTextBoxesTextChanged);
                //currentQuantityTextBox.Margin = new Thickness(-300, 12, 12, 12);
                productQuantityWrapPanel.Children.Add(currentQuantityTextBox);

                //currentQuantityTextBox.Text = rfq.GetRFQProductQuantity(i + 1).ToString();

                if (viewAddCondition == 0)
                    currentQuantityTextBox.IsEnabled = false;

                currentProductGrid.Children.Add(productQuantityWrapPanel);
                Grid.SetRow(productQuantityWrapPanel, 4);

                mainWrapPanel.Children.Add(currentProductGrid);

                numberOfProductsAdded += 1;
            }
        }

            //////////////SELECTION CHANGED HANDLERS///////////
            ///////////////////////////////////////////////////

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
                        rfq.SetRFQProductType(k + 1, typeId, currentTypeComboBox.SelectedItem.ToString());
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
                        rfq.SetRFQProductBrand(k+1 ,brands[currentBrandComboBox.SelectedIndex].brandId, currentBrandComboBox.SelectedItem.ToString());
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
                for(int k = 0; k < numberOfProductsAdded; k++)
                {
                    for(int i = 0; i < models.Count; i++)
                    {
                        if (currentModelComboBox.SelectedItem.ToString() == models[i].modelName)
                            modelId = models[i].modelId;
                    }
                    if (currentProductGrid == mainWrapPanel.Children[k])
                        rfq.SetRFQProductModel(k+1 , modelId, currentModelComboBox.SelectedItem.ToString());
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
                for(int k = 0; k< numberOfProductsAdded; k++)
                {
                    if(currentProductGrid == mainWrapPanel.Children[k])
                        rfq.SetRFQProductQuantity(k+1 ,quantity1);
                }
            }
            else
            {
                quantity1 = 0;
                currentQuantityTextBox.Text = null;
            }
        }

        ////////////BUTTON CLICKS///////////
        ////////////////////////////////////
        private void OnClickBasicInfo(object sender, RoutedEventArgs e)
        {
            RFQBasicInfoPage basicInfoPage = new RFQBasicInfoPage(ref loggedInUser, ref rfq, viewAddCondition);
            NavigationService.Navigate(basicInfoPage);
        }
        

        private void OnClickProductsInfo(object sender, RoutedEventArgs e)
        {
            //RFQProductsPage productsPage = new RFQProductsPage(ref loggedInUser, ref rfq);
            //NavigationService.Navigate(productsPage);
        }

        private void OnClickAdditionalInfo(object sender, RoutedEventArgs e)
        {
            RFQAdditionalInfoPage additionalInfoPage = new RFQAdditionalInfoPage(ref loggedInUser, ref rfq, viewAddCondition);
            NavigationService.Navigate(additionalInfoPage);
        }    
       
        
    }
}
