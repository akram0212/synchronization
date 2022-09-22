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
    /// Interaction logic for ModelAdditionalInfoPage.xaml
    /// </summary>
    public partial class ModelAdditionalInfoPage : Page
    {

        Employee loggedInUser;
        Product product;
        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private int viewAddCondition;

        public ModelBasicInfoPage modelBasicInfoPage;
        public ModelUpsSpecsPage modelUpsSpecsPage;
        public ModelUploadFilesPage modelUploadFilesPage;
        
        public ModelAdditionalInfoPage(ref Employee mLoggedInUser, ref Product mPrduct, int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            sqlDatabase = new SQLServer();

            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            product = mPrduct;
            InitializeComponent();

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
            {

            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                cancelButton.IsEnabled = false;
                finishButton.IsEnabled = false;
                nextButton.IsEnabled = true;
            }

            if (viewAddCondition != COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                nextButton.IsEnabled = false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        //////////BUTTON CLICKED/////////////
        /////////////////////////////////////////////////////////////////////////////////////////////

        private void OnBtnClickFinish(object sender, RoutedEventArgs e)
        {
            //YOUR MESSAGE MUST BE SPECIFIC
            //YOU SHALL CHECK UI ELEMENTS IN ORDER AND THEN WRITE A MESSAGE IF ERROR IS TO BE FOUND
            // if (product.GetSalesPersonId() == 0)
            //     System.Windows.Forms.MessageBox.Show("Sales person is not specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            // else if (product.GetAssigneeId() == 0)
            //     System.Windows.Forms.MessageBox.Show("Assignee is not specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            // else if (product.GetAddressSerial() == 0)
            //     System.Windows.Forms.MessageBox.Show("Company Address is not specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            // else if (product.GetContactId() == 0)
            //     System.Windows.Forms.MessageBox.Show("Contact is not specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            // else if (product.GetproductProduct1TypeId() != 0 && product.GetproductProduct1Quantity() == 0)
            //     System.Windows.Forms.MessageBox.Show("Quantity is not specified for product 1!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            // else if (product.GetproductProduct2TypeId() != 0 && product.GetproductProduct2Quantity() == 0)
            //     System.Windows.Forms.MessageBox.Show("Quantity is not specified for product 2!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            // else if (product.GetproductProduct3TypeId() != 0 && product.GetproductProduct3Quantity() == 0)
            //     System.Windows.Forms.MessageBox.Show("Quantity is not specified for product 3!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            // else if (product.GetproductProduct4TypeId() != 0 && product.GetproductProduct4Quantity() == 0)
            //     System.Windows.Forms.MessageBox.Show("Quantity is not specified for product 4!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            // else if (product.GetproductContractTypeId() == 0)
            //     System.Windows.Forms.MessageBox.Show("Contract type is not specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            // else if (product.GetproductStatusId() == 0)
            //     System.Windows.Forms.MessageBox.Show("Status ID can't be 0 for an product! Contact your system administrator!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            // else
            // {
            //     if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
            //     {
            //         //if (!product.IssueNewproduct())
            //         //    return;
            //
            //         if (viewAddCondition != COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            //         {
            //             viewAddCondition = COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION;
            //
            //             ModelsWindow viewproduct = new ModelsWindow(ref loggedInUser, ref product, viewAddCondition, true);
            //
            //             viewproduct.Show();
            //         }
            //
            //     }
            //
            //     NavigationWindow currentWindow = (NavigationWindow)this.Parent;
            //     currentWindow.Close();
            // }

            product.GetNewModelID();
            product.InsertIntoBrandModels();
            product.InsertIntoUPSSpecs();
            
        }

        private void OnBtnClickNext(object sender, RoutedEventArgs e)
        {
            modelUploadFilesPage.modelBasicInfoPage = modelBasicInfoPage;
            modelUploadFilesPage.modelUpsSpecsPage = modelUpsSpecsPage;
            modelUploadFilesPage.modelAdditionalInfoPage = this;

            NavigationService.Navigate(modelUploadFilesPage);
        }

        private void OnBtnClickBack(object sender, RoutedEventArgs e)
        {

            modelUpsSpecsPage.modelAdditionalInfoPage = this;
            modelUpsSpecsPage.modelBasicInfoPage = modelBasicInfoPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                modelUpsSpecsPage.modelUploadFilesPage = modelUploadFilesPage;

            NavigationService.Navigate(modelUpsSpecsPage);

            //modelBasicInfoPage.modelAdditionalInfoPage = this;
            //
            //if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            //    modelBasicInfoPage.modelUploadFilesPage = modelUploadFilesPage;
            //
            //NavigationService.Navigate(modelBasicInfoPage);
        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;
            currentWindow.Close();
        }
        private void OnBtnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            modelBasicInfoPage.modelUpsSpecsPage = modelUpsSpecsPage;
            modelBasicInfoPage.modelAdditionalInfoPage = this;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                modelBasicInfoPage.modelUploadFilesPage = modelUploadFilesPage;

            NavigationService.Navigate(modelBasicInfoPage);
        }
        private void OnBtnClickUpsSpecs(object sender, MouseButtonEventArgs e)
        {
            modelUpsSpecsPage.modelAdditionalInfoPage = this;
            modelUpsSpecsPage.modelBasicInfoPage = modelBasicInfoPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                modelUpsSpecsPage.modelUploadFilesPage = modelUploadFilesPage;

            NavigationService.Navigate(modelUpsSpecsPage);
        }

        private void OnBtnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                modelUploadFilesPage.modelBasicInfoPage = modelBasicInfoPage; 
                modelUploadFilesPage.modelUpsSpecsPage = modelUpsSpecsPage;
                modelUploadFilesPage.modelAdditionalInfoPage = this;

                NavigationService.Navigate(modelUploadFilesPage);
            }
        }

        private void OnClickStandardFeaturesImage(object sender, MouseButtonEventArgs e)
        {
            Image addImage = sender as Image;
            Grid parentGrid = addImage.Parent as Grid;
            int index = standardFeaturesGrid.Children.IndexOf(parentGrid);

            AddNewStandardFeature(index, parentGrid);



        }

        private void OnClickBenefitsImage(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnClickApplicationsImage(object sender, MouseButtonEventArgs e)
        {

        }
        private void AddNewStandardFeature(int index, Grid parentGrid)
        {
            if(index != 0)
                 parentGrid.Children.RemoveAt(3);
            else
                parentGrid.Children.RemoveAt(2);

            //Image removeIcon = new Image();
            //removeIcon.Source = new BitmapImage(new Uri(@"Icons\red_cross_icon.jpg", UriKind.Relative));
            //removeIcon.Width = 20;
            //removeIcon.Height = 20;
            //removeIcon.Margin = new Thickness(10, 0, 10, 0);
            //removeIcon.MouseLeftButtonDown += OnClickRemoveFeature;
            //Grid.SetColumn(removeIcon, 2);
            //Grid.SetRow(removeIcon, index);
            //
            //parentGrid.Children.Add(removeIcon);
            //}

            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(75);
            standardFeaturesGrid.RowDefinitions.Add(row);

            /////NEW FEATURE GRID
            Grid gridI = new Grid();
            Grid.SetRow(gridI, index + 1);

            ColumnDefinition labelColumn = new ColumnDefinition();
            labelColumn.Width = new GridLength(250);

            ColumnDefinition featureColumn = new ColumnDefinition();

            ColumnDefinition imageColumn = new ColumnDefinition();
            imageColumn.Width = new GridLength(90);

            gridI.ColumnDefinitions.Add(labelColumn);
            gridI.ColumnDefinitions.Add(featureColumn);
            gridI.ColumnDefinitions.Add(imageColumn);

            Label featureIdLabel = new Label();
            featureIdLabel.Margin = new Thickness(30, 0, 0, 0);
            featureIdLabel.HorizontalAlignment = HorizontalAlignment.Left;
            featureIdLabel.Style = (Style)FindResource("labelStyle");
            featureIdLabel.Content = "Feature #" + (index + 2).ToString();
            Grid.SetColumn(featureIdLabel, 0);

            TextBox featureTextBox = new TextBox();
            featureTextBox.Style = (Style)FindResource("textBoxStyle");
            featureTextBox.TextWrapping = TextWrapping.Wrap;
            Grid.SetColumn(featureTextBox, 1);

            Image deleteIcon = new Image();
            deleteIcon.Source = new BitmapImage(new Uri(@"Icons\red_cross_icon.jpg", UriKind.Relative));
            deleteIcon.Width = 20;
            deleteIcon.Height = 20;
            //deleteIcon.Margin = new Thickness(0, 0, 10, 0);
            deleteIcon.MouseLeftButtonDown += OnClickRemoveFeature;
            Grid.SetColumn(deleteIcon, 2);

            Image addIcon = new Image();
            addIcon.Source = new BitmapImage(new Uri(@"Icons\plus_icon.jpg", UriKind.Relative));
            addIcon.Width = 20;
            addIcon.Height = 20;
            addIcon.Margin = new Thickness(55, 0, 10, 0);
            addIcon.MouseLeftButtonDown += OnClickStandardFeaturesImage;
            Grid.SetColumn(addIcon, 2);

            gridI.Children.Add(featureIdLabel);
            gridI.Children.Add(featureTextBox);
            gridI.Children.Add(deleteIcon);
            gridI.Children.Add(addIcon);

            //if (index != 0)
            //{ 
            //}

            /////////////////////////////////////////////

            standardFeaturesGrid.Children.Add(gridI);
        }

        private void OnClickRemoveFeature(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            Grid innerGrid = (Grid)currentImage.Parent;
            Grid outerGrid = (Grid)innerGrid.Parent;

            int index = outerGrid.Children.IndexOf(innerGrid);

            if (outerGrid.Children.Count == 2)
            {
                innerGrid.Children.Clear();
                Image addVendorImage = new Image();
                addVendorImage.Source = new BitmapImage(new Uri(@"Icons\plus_icon.jpg", UriKind.Relative));
                addVendorImage.Height = 20;
                addVendorImage.Width = 20;
                addVendorImage.MouseLeftButtonDown += OnClickStandardFeaturesImage;
                Grid previousGrid = outerGrid.Children[0] as Grid;

                previousGrid.Children.Add(addVendorImage);
                Grid.SetColumn(addVendorImage, 2);
                outerGrid.Children.Remove(innerGrid);
                outerGrid.RowDefinitions.RemoveAt(outerGrid.RowDefinitions.Count - 1);
            }
            else if (index == outerGrid.Children.Count - 1 && outerGrid.Children.Count != 0)
            {
                outerGrid.Children.Remove(innerGrid);
                outerGrid.RowDefinitions.RemoveAt(outerGrid.RowDefinitions.Count - 1);

                Grid previousInnerGrid = (Grid)outerGrid.Children[index - 1];
                Image plusIcon = (Image)innerGrid.Children[3];
                innerGrid.Children.Remove(plusIcon);
                Grid.SetColumn(plusIcon, 2);
                previousInnerGrid.Children.Add(plusIcon);

            }
            else
            {
                for (int i = index; i < outerGrid.Children.Count - 1; i++)
                {
                    if (i == index)
                        Grid.SetRow(innerGrid, outerGrid.RowDefinitions.Count - 1);
                    else
                        Grid.SetRow(outerGrid.Children[i], i - 1);

                }

                outerGrid.Children.Remove(innerGrid);

                outerGrid.RowDefinitions.RemoveAt(outerGrid.RowDefinitions.Count - 1);
            }
        }
    }
}
