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

        private int standardFeatureId = 0;
        private int benefitId = 1;
        private int applicationId = 2;

        private List<String> modelStandardFeatures;
        private List<String> modelBenefits;
        private List<String> modelApplications;
        private List<String> modelSummeryPoints;
        public ModelAdditionalInfoPage(ref Employee mLoggedInUser, ref Product mPrduct, int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            sqlDatabase = new SQLServer();

            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();
            modelStandardFeatures = new List<String>();
            modelBenefits = new List<String>();
            modelApplications = new List<String>();


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
            modelStandardFeatures.Clear();
            modelBenefits.Clear();
            modelApplications.Clear();

            for (int i = 0; i < standardFeaturesGrid.Children.Count; i ++)
            {
                Grid innerGrid = standardFeaturesGrid.Children[i] as Grid;
                TextBox feature = innerGrid.Children[1] as TextBox;
                if(feature.Text.ToString() != String.Empty)
                    modelStandardFeatures.Add(feature.Text.ToString());
            }
            for (int i = 0; i < benefitsGrid.Children.Count; i++)
            {
                Grid innerGrid = benefitsGrid.Children[i] as Grid;
                TextBox feature = innerGrid.Children[1] as TextBox;
                if (feature.Text.ToString() != String.Empty)
                    modelBenefits.Add(feature.Text.ToString());
            }
            for (int i = 0; i < applicationsGrid.Children.Count; i++)
            {
                Grid innerGrid = applicationsGrid.Children[i] as Grid;
                TextBox feature = innerGrid.Children[1] as TextBox;
                if (feature.Text.ToString() != String.Empty)
                    modelApplications.Add(feature.Text.ToString());
            }
            if (product.GetModelName() == null)
                System.Windows.Forms.MessageBox.Show("Model Name must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else if (product.GetModelSummaryPoints().Count() == 0)
                System.Windows.Forms.MessageBox.Show("Model Summary Points must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            //else if (product.GetUPSSpecs().Count() == 0)
            //    System.Windows.Forms.MessageBox.Show("Model Specs must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else if (product.GetUPSSpecs()[0].io_phase == null)
                System.Windows.Forms.MessageBox.Show("Model IO Phase must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else if (product.GetUPSSpecs()[0].rated_power == null)
                System.Windows.Forms.MessageBox.Show("Model rated power must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else if (product.GetUPSSpecs()[0].rating_id == null)
                System.Windows.Forms.MessageBox.Show("Model rating must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else if (product.GetUPSSpecs()[0].backup_time_50 == null)
                System.Windows.Forms.MessageBox.Show("Model backup_time_50 must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else if (product.GetUPSSpecs()[0].backup_time_70 == null)
                System.Windows.Forms.MessageBox.Show("Model backup_time_70 must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else if (product.GetUPSSpecs()[0].backup_time_100 == null)
                System.Windows.Forms.MessageBox.Show("Model backup_time_100 must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else if (product.GetUPSSpecs()[0].valid_until == null)
                System.Windows.Forms.MessageBox.Show("Model Valid Until must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else if (modelStandardFeatures.Count() == 0)
                System.Windows.Forms.MessageBox.Show("Model Standard Features must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else if (modelBenefits.Count() == 0)
                System.Windows.Forms.MessageBox.Show("Model Benefits must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else if (modelApplications.Count() == 0)
                System.Windows.Forms.MessageBox.Show("Model Applications must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            else
            {
                if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
                {
                    modelBasicInfoPage.uploadBackground.RunWorkerAsync();

                   // if (!product.IssueNewModel(ref modelApplications, ref modelBenefits, ref modelStandardFeatures))
                   //     return;

                    if (viewAddCondition != COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                    {
                        viewAddCondition = COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION;

                        ModelsWindow viewproduct = new ModelsWindow(ref loggedInUser, ref product, viewAddCondition, true);

                        viewproduct.Show();
                    }

                }

                NavigationWindow currentWindow = (NavigationWindow)this.Parent;
                currentWindow.Close();

            }
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

        private void onClickHandler(object sender, MouseButtonEventArgs e)
        {
            Image addImage = sender as Image;
            Grid parentGrid = addImage.Parent as Grid;
            parentGrid.Tag = standardFeatureId;
            int index = standardFeaturesGrid.Children.IndexOf(parentGrid);

            AddNewStandardFeature(index, "Feature #", standardFeaturesGrid, parentGrid, standardFeatureId, onClickHandler);

        }

        private void OnClickBenefitsImage(object sender, MouseButtonEventArgs e)
        {
            Image addImage = sender as Image;
            Grid parentGrid = addImage.Parent as Grid;
            parentGrid.Tag = benefitId;
            int index = benefitsGrid.Children.IndexOf(parentGrid);

            AddNewStandardFeature(index, "Benefit #", benefitsGrid, parentGrid, benefitId, OnClickBenefitsImage);

        }

        private void OnClickApplicationsImage(object sender, MouseButtonEventArgs e)
        {
            Image addImage = sender as Image;
            Grid parentGrid = addImage.Parent as Grid;
            parentGrid.Tag = applicationId;
            int index = applicationsGrid.Children.IndexOf(parentGrid);

            AddNewStandardFeature(index, "Application #", applicationsGrid, parentGrid, applicationId, OnClickApplicationsImage);

        }
        private void AddNewStandardFeature(int index, String labelContent, Grid mainGrid, Grid parentGrid, int selectedGridiD, MouseButtonEventHandler onClickHandler)
        {
            if(index != 0 && index != -1)
                 parentGrid.Children.RemoveAt(3);
            else if(index == 0)
                parentGrid.Children.RemoveAt(2);

            parentGrid.Tag = selectedGridiD;
            Grid.SetRow(parentGrid, mainGrid.RowDefinitions.Count-1);
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
            mainGrid.RowDefinitions.Add(row);

            /////NEW FEATURE GRID
            Grid gridI = new Grid();
            Grid.SetRow(gridI, index + 1);
            gridI.Tag = selectedGridiD;

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
            featureIdLabel.Width = 200;
            featureIdLabel.HorizontalAlignment = HorizontalAlignment.Left;
            featureIdLabel.Style = (Style)FindResource("labelStyle");
            featureIdLabel.Content = labelContent + (index + 2).ToString();
            //featureIdLabel.Content = "Feature #" + (index + 2).ToString();
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
            addIcon.MouseLeftButtonDown += onClickHandler;
            //addIcon.Tag = selectedGridiD;
            //addIcon.MouseLeftButtonDown += OnClickStandardFeaturesImage;
            Grid.SetColumn(addIcon, 2);

            //Label featureNameLabel = new Label();
            //featureNameLabel.Margin = new Thickness(30, 0, 0, 0);
            //featureNameLabel.Width = 200;
            //featureNameLabel.HorizontalAlignment = HorizontalAlignment.Left;
            //featureNameLabel.Style = (Style)FindResource("labelStyle");
            //featureNameLabel.Content = labelContent + (index + 2).ToString();
            ////featureIdLabel.Content = "Feature #" + (index + 2).ToString();
            //featureNameLabel.Visibility = Visibility.Collapsed;
            //Grid.SetColumn(featureNameLabel, 1);

            gridI.Children.Add(featureIdLabel);
            gridI.Children.Add(featureTextBox);
            gridI.Children.Add(deleteIcon);
            gridI.Children.Add(addIcon);

            //if (index != 0)
            //{ 
            //}

            /////////////////////////////////////////////

            //standardFeaturesGrid.Children.Add(gridI);
            mainGrid.Children.Add(gridI);
        }

        private void OnClickRemoveFeature(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            Grid innerGrid = (Grid)currentImage.Parent;
            Grid outerGrid = (Grid)innerGrid.Parent;
            int tagID = int.Parse(innerGrid.Tag.ToString());

            int index = outerGrid.Children.IndexOf(innerGrid);

            if (outerGrid.Children.Count == 2)
            {
                innerGrid.Children.Clear();
                Image addVendorImage = new Image();
                addVendorImage.Source = new BitmapImage(new Uri(@"Icons\plus_icon.jpg", UriKind.Relative));
                addVendorImage.Height = 20;
                addVendorImage.Width = 20;

                if(tagID == standardFeatureId)
                    addVendorImage.MouseLeftButtonDown += onClickHandler;
                else if (tagID == benefitId)
                    addVendorImage.MouseLeftButtonDown += OnClickBenefitsImage;
                else
                    addVendorImage.MouseLeftButtonDown += OnClickApplicationsImage;

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

            String Content;
            if (tagID == standardFeatureId)
            {
                Content = "Feature #";
            }
            else if (tagID == benefitId)
            {
                Content = "Benefit #";
            }
            else
            {
                Content = "Application #";
            }
            updateLabelIds(Content, outerGrid, index);
        }
        void updateLabelIds(String content, Grid currentGrid, int index)
        {
            for(int i = index; i < currentGrid.Children.Count; i++)
            {
                Grid innerGrid = currentGrid.Children[i] as Grid;
                Label header = innerGrid.Children[0] as Label;
                header.Content = content + (i + 1).ToString();
            }
        }
    }
}
