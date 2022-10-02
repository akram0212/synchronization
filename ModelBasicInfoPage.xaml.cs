using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using Color = System.Drawing.Color;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for ModelBasicInfoPage.xaml
    /// </summary>
    public partial class ModelBasicInfoPage : Page
    {
        Employee loggedInUser;
        Product product;
        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private int viewAddCondition;

        public ModelAdditionalInfoPage modelAdditionalInfoPage;
        public ModelUpsSpecsPage modelUpsSpecsPage;
        public ModelUploadFilesPage modelUploadFilesPage;

        public ModelBasicInfoPage(ref Employee mLoggedInUser, ref Product mPrduct, int mViewAddCondition)
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
               
                modelNameTextBox.Visibility = Visibility.Visible;
                summeryPointsTextBox.Visibility = Visibility.Visible;

            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {

                //NameLabel.Visibility = Visibility.Visible;
                //summeryPointsTextBlock.Visibility = Visibility.Visible;
                InitializeInfo();

            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //////////BUTTON CLICKED///////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnClickNextButton(object sender, RoutedEventArgs e)
        {
            product.SetModelName(modelNameTextBox.Text.ToString());
            modelUpsSpecsPage.modelBasicInfoPage = this;
            modelUpsSpecsPage.modelAdditionalInfoPage = modelAdditionalInfoPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                modelUpsSpecsPage.modelUploadFilesPage = modelUploadFilesPage;

            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
            {
                product.GetModelSummaryPoints().Clear();


                for (int i = 0; i < standardFeaturesGrid.Children.Count; i++)
                {
                    Grid innerGrid = standardFeaturesGrid.Children[i] as Grid;
                    TextBox feature = innerGrid.Children[1] as TextBox;
                    if (feature.Text.ToString() != String.Empty)
                        product.GetModelSummaryPoints().Add(feature.Text.ToString());
                }
            }

            NavigationService.Navigate(modelUpsSpecsPage);
        }
        private void OnBtnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            modelAdditionalInfoPage.modelBasicInfoPage = this;
            modelAdditionalInfoPage.modelUpsSpecsPage = modelUpsSpecsPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION) { 
                modelAdditionalInfoPage.modelUploadFilesPage = modelUploadFilesPage;
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
            {
                product.GetModelSummaryPoints().Clear();


                for (int i = 0; i < standardFeaturesGrid.Children.Count; i++)
                {
                    Grid innerGrid = standardFeaturesGrid.Children[i] as Grid;
                    TextBox feature = innerGrid.Children[1] as TextBox;
                    if (feature.Text.ToString() != String.Empty)
                        product.GetModelSummaryPoints().Add(feature.Text.ToString());
                }
            }
            NavigationService.Navigate(modelAdditionalInfoPage);

        }
        private void OnBtnClickUpsSpecs(object sender, MouseButtonEventArgs e)
        {
            modelUpsSpecsPage.modelBasicInfoPage = this;
            modelUpsSpecsPage.modelAdditionalInfoPage = modelAdditionalInfoPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION) { 
                modelUpsSpecsPage.modelUploadFilesPage = modelUploadFilesPage;
        }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
            {
                product.GetModelSummaryPoints().Clear();


                for (int i = 0; i < standardFeaturesGrid.Children.Count; i++)
                {
                    Grid innerGrid = standardFeaturesGrid.Children[i] as Grid;
                    TextBox feature = innerGrid.Children[1] as TextBox;
                    if (feature.Text.ToString() != String.Empty)
                        product.GetModelSummaryPoints().Add(feature.Text.ToString());
                }
            }
            NavigationService.Navigate(modelUpsSpecsPage);
        }

        private void OnBtnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                modelUploadFilesPage.modelBasicInfoPage = this;
                modelUploadFilesPage.modelUpsSpecsPage = modelUpsSpecsPage;
                modelUploadFilesPage.modelAdditionalInfoPage = modelAdditionalInfoPage;

                NavigationService.Navigate(modelUploadFilesPage);
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
            {
                product.GetModelSummaryPoints().Clear();


                for (int i = 0; i < standardFeaturesGrid.Children.Count; i++)
                {
                    Grid innerGrid = standardFeaturesGrid.Children[i] as Grid;
                    TextBox feature = innerGrid.Children[1] as TextBox;
                    if (feature.Text.ToString() != String.Empty)
                        product.GetModelSummaryPoints().Add(feature.Text.ToString());
                }
            }
        }
        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;
            currentWindow.Close();
        }

        private void OnDropUploadFilesStackPanel(object sender, DragEventArgs e)
        {

        }
        private void InitializeInfo()
        {
            modelNameTextBox.Visibility= Visibility.Collapsed;
            modelNameLabel.Visibility= Visibility.Visible;
            modelNameLabel.Content = product.GetModelName();
            summeryPointsTextBox.Visibility= Visibility.Collapsed;
            summeryPointsLabel.Visibility=Visibility.Visible;
            summeryPointsLabel.Text= product.GetModelSummaryPoints()[0];


            for (int i = 1; i < product.GetModelSummaryPoints().Count; i++)
            {
                AddNewSymmeryPoint(i-1, "Point #", product.GetModelSummaryPoints()[i], standardFeaturesGrid, standardFeaturesGrid.Children[i-1] as Grid, onClickHandler);

            }

        }

        private void onClickHandler(object sender, MouseButtonEventArgs e)
        {
            Image addImage = sender as Image;
            Grid parentGrid = addImage.Parent as Grid;
            int index = standardFeaturesGrid.Children.IndexOf(parentGrid);

            AddNewSymmeryPoint(index, "Point #", standardFeaturesGrid, parentGrid, onClickHandler);

        }

        private void AddNewSymmeryPoint(int index, String labelContent, Grid mainGrid, Grid parentGrid, MouseButtonEventHandler onClickHandler)
        {
            if (index != 0 && index != -1)
                parentGrid.Children.RemoveAt(3);
            else if (index == 0)
                parentGrid.Children.RemoveAt(2);

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
            //}A

            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(75);
            mainGrid.RowDefinitions.Add(row);

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
        
        private void AddNewSymmeryPoint(int index, String labelContent,String SummryPointContent, Grid mainGrid, Grid parentGrid, MouseButtonEventHandler onClickHandler)
        {
            if (index != 0 && index != -1)
                parentGrid.Children.RemoveAt(4);
            else if (index == 0)
                parentGrid.Children.RemoveAt(2);

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
            //}A

            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(75);
            mainGrid.RowDefinitions.Add(row);

            /////NEW FEATURE GRID
            Grid gridI = new Grid();
            Grid.SetRow(gridI, index + 1);

            ColumnDefinition labelColumn = new ColumnDefinition();
            labelColumn.Width = new GridLength(250);

            ColumnDefinition featureColumn = new ColumnDefinition();
            featureColumn.Width = new GridLength(500);

            ColumnDefinition imageColumn = new ColumnDefinition();
            imageColumn.Width = new GridLength(100);

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
            featureTextBox.Visibility= Visibility.Collapsed;
            featureTextBox.Margin= new Thickness(30, 0, 0, 0);
            featureTextBox.Margin= new Thickness(30, 12, 12, 12);
            featureTextBox.Width = 384;
            Grid.SetColumn(featureTextBox, 1);


            TextBlock featureTextBlock = new TextBlock();
            featureTextBlock.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(16, 90, 151));
            featureTextBlock.Text = SummryPointContent;
            featureTextBlock.TextWrapping = TextWrapping.Wrap;
            featureTextBlock.Visibility = Visibility.Visible;
            featureTextBlock.TextWrapping = TextWrapping.Wrap;
            featureTextBlock.FontSize = 16;
            featureTextBlock.MouseDown += OnClickEditFeature;
            featureTextBlock.FontWeight =FontWeights.DemiBold;
            featureTextBlock.Width = 384;
            featureTextBlock.Style = (Style)FindResource("cardTextBlockStyle");

            Grid.SetColumn(featureTextBlock, 1);
            //featureTextBlock.FontFamily = FontFamily.("Sans Serif");

            //TextBlock featureTextBlock = new TextBlock();
            //featureTextBlock.Style = (Style)FindResource("textBlockStyle");
            //featureTextBlock.Foreground = Brushes.Blue;
            //featureTextBlock.FontSize = 30;

            Image deleteIcon = new Image();
            deleteIcon.Source = new BitmapImage(new Uri(@"Icons\red_cross_icon.jpg", UriKind.Relative));
            deleteIcon.Width = 20;
            deleteIcon.Height = 20;
            //deleteIcon.Margin = new Thickness(0, 0, 10, 0);
            deleteIcon.MouseLeftButtonDown += OnClickRemoveFeature;
            Grid.SetColumn(deleteIcon, 2);
            deleteIcon.Visibility=Visibility.Collapsed; 


            Image addIcon = new Image();
            addIcon.Source = new BitmapImage(new Uri(@"Icons\plus_icon.jpg", UriKind.Relative));
            addIcon.Width = 20;
            addIcon.Height = 20;
            addIcon.Margin = new Thickness(55, 0, 10, 0);
            addIcon.MouseLeftButtonDown += onClickHandler;
            //addIcon.Tag = selectedGridiD;
            //addIcon.MouseLeftButtonDown += OnClickStandardFeaturesImage;
            Grid.SetColumn(addIcon, 2);

            gridI.Children.Add(featureIdLabel);
            gridI.Children.Add(featureTextBox);
            gridI.Children.Add(featureTextBlock);
            gridI.Children.Add(deleteIcon);
            
            gridI.Children.Add(addIcon);

            //if (index != 0)
            //{ 
            //}

            /////////////////////////////////////////////

            //standardFeaturesGrid.Children.Add(gridI);
            mainGrid.Children.Add(gridI);
        }

        private void summeryPointsLabel1MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            summeryPointsTextBox.Text  = summeryPointsLabel.Text.ToString();
            summeryPointsLabel.Visibility = Visibility.Collapsed;
            summeryPointsTextBox .Visibility = Visibility.Visible;


        }
        private void OnClickEditFeature(object sender, MouseButtonEventArgs e)
        {
            TextBlock currentLabel =(TextBlock) sender;
            Grid innerGrid = (Grid)currentLabel.Parent;
            Grid outerGrid = (Grid)innerGrid.Parent;
            Image deleteIcon =(Image) innerGrid.Children[3];
            TextBox TextBox = (TextBox) innerGrid.Children[1];
            TextBox.MouseLeave += SummeryPointMouseLeave; 

            TextBox.Text = currentLabel.Text.ToString();

            deleteIcon.Visibility = Visibility.Visible;
            TextBox.Visibility = Visibility.Visible;
            currentLabel.Visibility = Visibility.Collapsed;
        }

        private void SummaryPoint1MouseLeave(object sender, MouseEventArgs e)
        {
            summeryPointsLabel.Text = summeryPointsTextBox.Text;
            summeryPointsTextBox.Visibility = Visibility.Collapsed;
            summeryPointsLabel.Visibility = Visibility.Visible;

        }
        private void SummeryPointMouseLeave(object sender, MouseEventArgs e)
        {
            TextBox currentTextBox = (TextBox)sender;
            Grid innerGrid = (Grid)currentTextBox.Parent;
            Grid outerGrid = (Grid)innerGrid.Parent;
            Image deleteIcon = (Image)innerGrid.Children[3];
            TextBlock currentTextBlockl = (TextBlock)innerGrid.Children[2];

            currentTextBlockl.Text = currentTextBox.Text;

            deleteIcon.Visibility = Visibility.Collapsed;
            currentTextBox.Visibility = Visibility.Collapsed;
            currentTextBlockl.Visibility = Visibility.Visible;
        }

        private void OnClickRemoveFeature(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            Grid innerGrid = (Grid)currentImage.Parent;
            Grid outerGrid = (Grid)innerGrid.Parent;
            //int tagID = int.Parse(innerGrid.Tag.ToString());

            int index = outerGrid.Children.IndexOf(innerGrid);

            if (outerGrid.Children.Count == 2)
            {
                innerGrid.Children.Clear();
                Image addVendorImage = new Image();
                addVendorImage.Source = new BitmapImage(new Uri(@"Icons\plus_icon.jpg", UriKind.Relative));
                addVendorImage.Height = 20;
                addVendorImage.Width = 20;
                addVendorImage.MouseLeftButtonDown += onClickHandler;
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

            String Content = "Point #";
            updateLabelIds(Content, outerGrid, index);
        }
        void updateLabelIds(String content, Grid currentGrid, int index)
        {
            for (int i = index; i < currentGrid.Children.Count; i++)
            {
                Grid innerGrid = currentGrid.Children[i] as Grid;
                Label header = innerGrid.Children[0] as Label;
                header.Content = content + (i + 1).ToString();
            }
        }


    }
}
