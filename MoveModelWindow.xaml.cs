using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using _01electronics_library;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for MoveModelWindow.xaml
    /// </summary>
    public partial class MoveModelWindow : Window
    {
        CommonQueries commonQueries;
        Model currentModell;
        List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT> categories=new List<COMPANY_WORK_MACROS.PRODUCT_CATEGORY_STRUCT>();
        List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> products = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
        List<COMPANY_WORK_MACROS.BRAND_STRUCT> brands = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();
        public MoveModelWindow(Model CurrentModel)
        {
            InitializeComponent();
            currentModell = CurrentModel;
            commonQueries = new CommonQueries();

            modelTextBox.Text = currentModell.GetModelName();
            modelTextBox.IsReadOnly = true;

            commonQueries.GetProductCategories(ref categories);

            categories.ForEach(a => CategoryCombo.Items.Add(a.category));

            ProductsCombo.IsEnabled = false;
            BrandComboBox.IsEnabled = false;
        }

        private void CategoryCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductsCombo.Items.Clear();
            products.Clear();
            commonQueries.GetCompanyProducts(ref products, categories[CategoryCombo.SelectedIndex].categoryId);
            products.ForEach(a => ProductsCombo.Items.Add(a.typeName));
            ProductsCombo.IsEnabled = true;
            ProductsCombo.SelectedItem = ProductsCombo.Items[0];
        }

        private void ProductsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductsCombo.Items.Count == 0)
                return;
            brands.Clear();
            BrandComboBox.Items.Clear();
            commonQueries.GetProductBrands(products[ProductsCombo.SelectedIndex].typeId, ref brands);
            brands.ForEach(a => BrandComboBox.Items.Add(a.brandName));
            BrandComboBox.IsEnabled = true;
            if (BrandComboBox.Items.Count == 0)
                return;
            BrandComboBox.SelectedItem = BrandComboBox.Items[0];
        }

        private void OnSaveChangesButtonClick(object sender, RoutedEventArgs e)
        {
            if (CategoryCombo.SelectedIndex == -1 || ProductsCombo.SelectedIndex == -1 || BrandComboBox.SelectedIndex == -1) {

                MessageBox.Show("You have to enter all of the data");
                return;
            }

            int productId = products[ProductsCombo.SelectedIndex].typeId;
            int brandId = brands[BrandComboBox.SelectedIndex].brandId;

            Model newModel = new Model();

            newModel.SetBrandID(brandId);
            newModel.SetProductID(productId);
            newModel.SetModelName(currentModell.GetModelName());

            newModel.SetCategoryID(categories[CategoryCombo.SelectedIndex].categoryId);

            newModel.SetModelsummaryPoints(currentModell.GetModelSummaryPoints());

            currentModell.GetUPSSpecs().ForEach(a => newModel.SetUPSSpecs(a));
            newModel.SetModelApplications(currentModell.GetModelApplications());

            newModel.SetModelBenefits(currentModell.GetModelBenefits());
            newModel.SetModelStandardFeatures(currentModell.GetModelStandardFeatures());
            //newModel.SetGensetSpec(currentModell.GetGensetSpecs());

            newModel.MoveModel(currentModell.GetProductID(),currentModell.GetBrandID(),currentModell.GetModelID(),currentModell.GetCategoryID());
            currentModell.DeleteModel();

            this.Close();


        }
    }
}
