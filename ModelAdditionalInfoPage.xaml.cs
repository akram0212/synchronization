using System;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using _01electronics_library;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for ModelAdditionalInfoPage.xaml
    /// </summary>
    public partial class ModelAdditionalInfoPage : Page
    {
        Employee loggedInUser;
        Model product;
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
        public ModelAdditionalInfoPage(ref Employee mLoggedInUser, ref Model mPrduct, int mViewAddCondition, ModelUpsSpecsPage modelUpssSpecsPage = null, ModelBasicInfoPage ModelBasicInfo = null)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;
            modelBasicInfoPage = ModelBasicInfo;
            modelUpsSpecsPage = modelUpssSpecsPage;

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
                InitializeStandardFeature();
                InitializeBenifits();
                InitializeApplications();
                cancelButton.IsEnabled = false;
                finishButton.IsEnabled = false;
                nextButton.IsEnabled = true;
            }

            if (viewAddCondition != COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                nextButton.IsEnabled = false;

            if (product.GetCategoryID() == COMPANY_WORK_MACROS.GENSET_CATEGORY_ID)
            {

                SpecsType.Content = "Genset Specs";
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        //////////BUTTON CLICKED/////////////
        /////////////////////////////////////////////////////////////////////////////////////////////

        private void OnBtnClickFinish(object sender, RoutedEventArgs e)
        {

            //modelBasicInfoPage.uploadBackground.RunWorkerAsync();
            modelStandardFeatures.Clear();
            modelBenefits.Clear();
            modelApplications.Clear();


 
            for (int i = 0; i < standardFeaturesGrid.Children.Count; i++)
            {
                Grid innerGrid = standardFeaturesGrid.Children[i] as Grid;
                TextBox feature = innerGrid.Children[1] as TextBox;
                if (feature.Text.ToString() != String.Empty)
                    modelStandardFeatures.Add(feature.Text.ToString());
            }

            product.SetModelStandardFeatures(modelStandardFeatures);

            for (int i = 0; i < benefitsGrid.Children.Count; i++)
            {
                Grid innerGrid = benefitsGrid.Children[i] as Grid;
                TextBox feature = innerGrid.Children[1] as TextBox;
                if (feature.Text.ToString() != String.Empty)
                    modelBenefits.Add(feature.Text.ToString());
            }
            product.SetModelBenefits(modelBenefits);

            for (int i = 0; i < applicationsGrid.Children.Count; i++)
            {
                Grid innerGrid = applicationsGrid.Children[i] as Grid;
                TextBox feature = innerGrid.Children[1] as TextBox;
                if (feature.Text.ToString() != String.Empty)
                    modelApplications.Add(feature.Text.ToString());
            }
            product.SetModelApplications(modelApplications);

            if (product.GetCategoryID() == COMPANY_WORK_MACROS.GENSET_CATEGORY_ID)
            {

                if (product.GetModelName() == null)
                    System.Windows.Forms.MessageBox.Show("Model Name must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                else if (CheckGensetRequired() == false)
                    return;
                else if (product.GetModelSummaryPoints().Count() == 0)
                    System.Windows.Forms.MessageBox.Show("Model Summary Points must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                else
                {
                    if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
                    {
                        if (!product.IssueNewModel())
                            return;

                        modelBasicInfoPage.uploadBackground.RunWorkerAsync();

                        if (viewAddCondition != COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                        {
                            viewAddCondition = COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION;

                            ModelsWindow viewproduct = new ModelsWindow(ref loggedInUser, ref product, viewAddCondition, true);

                            viewproduct.Show();
                        }
                    }
                    else
                    {
                        NavigationWindow currentWindow = (NavigationWindow)this.Parent;
                        currentWindow.Close();
                    }
                }
            }

            else
            {
                if (product.GetModelName() == null)
                    System.Windows.Forms.MessageBox.Show("Model Name must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                else if (product.GetModelSummaryPoints().Count() == 0)
                    System.Windows.Forms.MessageBox.Show("Model Summary Points must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                //else if (product.GetUPSSpecs().Count() == 0)
                //    System.Windows.Forms.MessageBox.Show("Model Specs must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                else if (product.GetModelSpecs()[0].ups_io_phase == null)
                    System.Windows.Forms.MessageBox.Show("Model IO Phase must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                else if (product.GetModelSpecs()[0].ups_rated_power == null)
                    System.Windows.Forms.MessageBox.Show("Model rated power must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                else if (product.GetModelSpecs()[0].ups_rating == null)
                    System.Windows.Forms.MessageBox.Show("Model rating must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                else if (product.GetModelSpecs()[0].ups_backup_time_50 == null)
                    System.Windows.Forms.MessageBox.Show("Model backup_time_50 must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                else if (product.GetModelSpecs()[0].ups_backup_time_70 == null)
                    System.Windows.Forms.MessageBox.Show("Model backup_time_70 must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                else if (product.GetModelSpecs()[0].ups_backup_time_100 == null)
                    System.Windows.Forms.MessageBox.Show("Model backup_time_100 must be specified!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                else if (product.GetModelSpecs()[0].valid_until == null)
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


                        if (!product.IssueNewModel())
                            return;
                        modelBasicInfoPage.uploadBackground.RunWorkerAsync();

                        if (viewAddCondition != COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                        {
                            viewAddCondition = COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION;

                            ModelsWindow viewproduct = new ModelsWindow(ref loggedInUser, ref product, viewAddCondition, true);

                            viewproduct.Show();
                        }

                    }
                    else
                    {
                        NavigationWindow currentWindow = (NavigationWindow)this.Parent;
                        currentWindow.Close();
                    }


                }

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

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                AddNewStandardFeature(index+1, "Feature #", standardFeaturesGrid, parentGrid, standardFeatureId, onClickHandler);
            }
            else
            {
                AddNewStandardFeature(index, "Feature #", standardFeaturesGrid, parentGrid, standardFeatureId, onClickHandler);
            }
        }

        private void OnClickBenefitsImage(object sender, MouseButtonEventArgs e)
        {
            Image addImage = sender as Image;
            Grid parentGrid = addImage.Parent as Grid;
            parentGrid.Tag = benefitId;
            int index = benefitsGrid.Children.IndexOf(parentGrid);

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                AddNewStandardFeature(index+1, "Benefit #", benefitsGrid, parentGrid, benefitId, OnClickBenefitsImage);
            }
            else
            {
                AddNewStandardFeature(index, "Benefit #", benefitsGrid, parentGrid, benefitId, OnClickBenefitsImage);
            }
            

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
            if (index != 0 && index != -1)
                if (index == 1 && viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                {
                    parentGrid.Children.RemoveAt(2);
                }
                else
                {
                    parentGrid.Children.RemoveAt(3);
                }
            else if (index == 0)
            {
                if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                {
                    mainGrid.Children.Clear();
                }
                else
                {
                    parentGrid.Children.RemoveAt(2);
                }
            }

            parentGrid.Tag = selectedGridiD;
            Grid.SetRow(parentGrid, mainGrid.RowDefinitions.Count - 1);

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

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                Grid.SetRow(gridI, index);
            }
            else
            {
                Grid.SetRow(gridI, index + 1);
            }
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
            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                featureIdLabel.Content = labelContent + (index + 1).ToString();
            }
            else
            {
                featureIdLabel.Content = labelContent + (index + 2).ToString();
            }
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
            if (selectedGridiD == standardFeatureId)
            {
                addIcon.MouseLeftButtonDown += onClickHandler;
            }
            else if (selectedGridiD == benefitId)
            {
                addIcon.MouseLeftButtonDown += OnClickBenefitsImage;
            }
            else
            {
                addIcon.MouseLeftButtonDown += OnClickApplicationsImage;
            }
            //addIcon.Tag = selectedGridiD;
            //addIcon.MouseLeftButtonDown += OnClickStandardFeaturesImage;
            Grid.SetColumn(addIcon, 2);
            Label featureNameLabel = new Label();
            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {


                
                
                if ((index+1 > product.GetModelStandardFeatures().Count && selectedGridiD == standardFeatureId)|| (index+1 > product.GetModelBenefits().Count && selectedGridiD == benefitId) || (index+1 > product.GetModelApplications().Count && selectedGridiD == applicationId))


                featureNameLabel.Margin = new Thickness(30, 0, 0, 0);
                featureNameLabel.Width = 200;
                featureNameLabel.HorizontalAlignment = HorizontalAlignment.Left;
                featureNameLabel.Style = (Style)FindResource("labelStyle");
                if (selectedGridiD == standardFeatureId)

                {

                }
                else
                {

                    featureNameLabel.Margin = new Thickness(30, 0, 0, 0);
                    featureNameLabel.Width = 200;
                    featureNameLabel.HorizontalAlignment = HorizontalAlignment.Left;
                    featureNameLabel.Style = (Style)FindResource("labelStyle");
                    if (selectedGridiD == standardFeatureId)
                    {
                        featureNameLabel.Content = product.GetModelStandardFeatures()[index].ToString();
                    }
                    else if (selectedGridiD == benefitId)
                    {
                        featureNameLabel.Content = product.GetModelBenefits()[index].ToString();
                    }
                    else if(selectedGridiD == applicationId)
                    {
                        featureNameLabel.Content = product.GetModelApplications()[index].ToString();
                    }
                    featureNameLabel.Visibility = Visibility.Visible;
                    Grid.SetColumn(featureNameLabel, 1);
                    featureTextBox.Visibility = Visibility.Collapsed;
                }

            }



            gridI.Children.Add(featureIdLabel);
            gridI.Children.Add(featureTextBox);
            gridI.Children.Add(deleteIcon);
            gridI.Children.Add(addIcon);
            gridI.Children.Add(featureNameLabel);

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

                if (tagID == standardFeatureId)
                    addVendorImage.MouseLeftButtonDown += onClickHandler;
                else if (tagID == benefitId)
                    addVendorImage.MouseLeftButtonDown += OnClickBenefitsImage;
                else
                    addVendorImage.MouseLeftButtonDown += OnClickApplicationsImage;

                Grid previousGrid = outerGrid.Children[0] as Grid;

                Label viewLabel = previousGrid.Children[2] as Label;
                previousGrid.Children.Remove(viewLabel);
                previousGrid.Children.Add(addVendorImage);
                previousGrid.Children.Add(viewLabel);
                Grid.SetColumn(addVendorImage, 2);
                outerGrid.Children.Remove(innerGrid);
                if ((Int32)outerGrid.Tag == standardFeatureId && product.GetModelStandardFeatures().Count > 1)
                {
                    product.GetModelStandardFeatures().RemoveAt(index);
                }
                else if ((Int32)outerGrid.Tag == benefitId && product.GetModelBenefits().Count > 1)
                {
                    product.GetModelBenefits().RemoveAt(index);
                }
                else if ((Int32)outerGrid.Tag == applicationId && product.GetModelApplications().Count > 1)
                {
                    product.GetModelApplications().RemoveAt(index);
                }
                outerGrid.RowDefinitions.RemoveAt(outerGrid.RowDefinitions.Count - 1);
            }
            else if (index == outerGrid.Children.Count - 1 && outerGrid.Children.Count != 0)
            {
                outerGrid.Children.Remove(innerGrid);
                if ((Int32)outerGrid.Tag == standardFeatureId && product.GetModelStandardFeatures().Count > 1)
                {
                    product.GetModelStandardFeatures().RemoveAt(index);
                }
                else if ((Int32)outerGrid.Tag == benefitId && product.GetModelBenefits().Count > 1)
                {
                    product.GetModelBenefits().RemoveAt(index);
                }
                else if ((Int32)outerGrid.Tag == applicationId && product.GetModelApplications().Count > 1)
                {
                    product.GetModelApplications().RemoveAt(index);
                }
                outerGrid.RowDefinitions.RemoveAt(outerGrid.RowDefinitions.Count - 1);

                Grid previousInnerGrid = (Grid)outerGrid.Children[index - 1];
                Image plusIcon = (Image)innerGrid.Children[3];
                Label viewLabel = previousInnerGrid.Children[3] as Label;
                previousInnerGrid.Children.Remove(viewLabel);
                previousInnerGrid.Children.Remove(plusIcon);
                innerGrid.Children.Remove(viewLabel);
                innerGrid.Children.Remove(plusIcon);

                Grid.SetColumn(plusIcon, 2);
                previousInnerGrid.Children.Add(plusIcon);
                previousInnerGrid.Children.Add(viewLabel);

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
                if ((Int32)outerGrid.Tag == standardFeatureId && product.GetModelStandardFeatures().Count>1)
                {
                    product.GetModelStandardFeatures().RemoveAt(index);
                }
                else if ((Int32)outerGrid.Tag == benefitId && product.GetModelBenefits().Count > 1)
                {
                    product.GetModelBenefits().RemoveAt(index);
                }
                else if ((Int32)outerGrid.Tag == applicationId && product.GetModelApplications().Count > 1)
                {
                    product.GetModelApplications().RemoveAt(index);
                }

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
        public void updateLabelIds(String content, Grid currentGrid, int index)
        {
            for (int i = index; i < currentGrid.Children.Count; i++)
            {
                Grid innerGrid = currentGrid.Children[i] as Grid;
                Label header = innerGrid.Children[0] as Label;
                header.Content = content + (i + 1).ToString();
            }
        }



        public bool CheckGensetRequired()
        {

            for (int i = 0; i < modelUpsSpecsPage.mainGrid.Children.Count; i++)
            {

                Grid card = modelUpsSpecsPage.mainGrid.Children[i] as Grid;

                WrapPanel wrap1 = card.Children[1] as WrapPanel;

                WrapPanel ratedPanel = wrap1.Children[0] as WrapPanel;

                TextBox ratedPower = ratedPanel.Children[1] as TextBox;
                if (ratedPower.Text == "")
                {
                    System.Windows.Forms.MessageBox.Show("You have to enter the rated Power", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return false;
                }

                WrapPanel modelPanel = wrap1.Children[1] as WrapPanel;

                TextBox modelTextBox = modelPanel.Children[1] as TextBox;
                if (modelTextBox.Text == "")
                {

                    System.Windows.Forms.MessageBox.Show("You have to enter the Spec Name", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return false;

                }

            }
            return true;
        }

        public void InitializeStandardFeature()
        {

            try
            {
                standardFeaturesLabel1.Content = product.GetModelStandardFeatures()[0].ToString();
                standardFeaturesTextBox1.Visibility = Visibility.Collapsed;
                standardFeaturesLabel1.Visibility = Visibility.Visible;
             


                for (int i = 1; i < product.GetModelStandardFeatures().Count; i++)
                {

                    standardFeaturesGrid.Tag = standardFeatureId;
                    Grid Currentgrid = new Grid();
                    Currentgrid = (Grid)standardFeaturesGrid.Children[i - 1];
                    AddNewStandardFeature(i, "Feature #", standardFeaturesGrid, Currentgrid, standardFeatureId, onClickHandler);
                }
            }
            catch (Exception ex)

            {

            }
        }
        void InitializeBenifits()
        {
            try
            {
                benefitsLabel1.Content = product.GetModelBenefits()[0].ToString();
                benefitsTextBox1.Visibility = Visibility.Collapsed;
                benefitsLabel1.Visibility = Visibility.Visible;


                for (int i = 1; i < product.GetModelBenefits().Count; i++)
                {

                    benefitsGrid.Tag = benefitId;
                    Grid Currentgrid = new Grid();
                    Currentgrid = (Grid)benefitsGrid.Children[i - 1];
                    AddNewStandardFeature(i, "Benefit #", benefitsGrid, Currentgrid, benefitId, onClickHandler);
                }
            }
            catch (Exception ex)
            {

            }

        }

        void InitializeApplications()
        {
            try
            {
                applicationsLabel1.Content = product.GetModelApplications()[0].ToString();
                applicationsTextBox1.Visibility = Visibility.Collapsed;
                applicationsLabel1.Visibility = Visibility.Visible;


                for (int i = 1; i < product.GetModelApplications().Count; i++)
                {

                    applicationsGrid.Tag = applicationId;
                    Grid Currentgrid = new Grid();
                    Currentgrid = (Grid)applicationsGrid.Children[i - 1];
                    AddNewStandardFeature(i, "Application #", applicationsGrid, Currentgrid, applicationId, onClickHandler);
                }
        }
            catch (Exception ex)
            {

            }

}
    }
}
