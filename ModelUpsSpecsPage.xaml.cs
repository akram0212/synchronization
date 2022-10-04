using _01electronics_library;
using _01electronics_windows_library;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for ModelUpsSpecsPage.xaml
    /// </summary>
    public partial class ModelUpsSpecsPage : Page
    {
        Employee loggedInUser;
        Product product;
        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private int viewAddCondition;

        public ModelAdditionalInfoPage modelAdditionalInfoPage;
        public ModelBasicInfoPage modelBasicInfoPage;
        public ModelUploadFilesPage modelUploadFilesPage;

        protected List<BASIC_STRUCTS.UPS_SPECS_STRUCT> UPSSpecs;
        List<PROCUREMENT_STRUCTS.MEASURE_UNITS_STRUCT> rating;

        public ModelUpsSpecsPage(ref Employee mLoggedInUser, ref Product mPrduct, int mViewAddCondition)
        {
            InitializeComponent();

            product = mPrduct;

            loggedInUser = mLoggedInUser;
            
            viewAddCondition = mViewAddCondition;

            sqlDatabase = new SQLServer();
            UPSSpecs = new List<BASIC_STRUCTS.UPS_SPECS_STRUCT>();
            rating = new List<PROCUREMENT_STRUCTS.MEASURE_UNITS_STRUCT>();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

          
            rating.Clear();
            if (!commonQueriesObject.GetMeasureUnits(ref rating))
                return;
            initializeRatingCombobox();
            

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
            {
                

            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                
            }

            //
            if (product.GetCategoryID() == COMPANY_WORK_MACROS.GENSET_CATEGORY_ID) {
                SpecsLable.Content = "Genset Specs";
                InitializeNewCardGenset();



            }



        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //////////BUTTON CLICKED///////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnBtnClickBack(object sender, RoutedEventArgs e)
        {
            modelBasicInfoPage.modelUpsSpecsPage = this;
            modelBasicInfoPage.modelAdditionalInfoPage = modelAdditionalInfoPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                modelBasicInfoPage.modelUploadFilesPage = modelUploadFilesPage;

            NavigationService.Navigate(modelBasicInfoPage);
        }

        private void InitializeUIElements()
        {
            for(int i = 0; i < product.GetUPSSpecs().Count; i++)
            {
                if (i == 0)
                {
                    iOPhaseTextBox.Visibility= Visibility.Collapsed;
                    
                }
            }
        }

        private void InitializeNewCardGenset() {

            mainGrid.Children.Clear();
            mainGrid.RowDefinitions.Clear();
            mainGrid.RowDefinitions.Add(new RowDefinition());

            Grid Card = new Grid() { VerticalAlignment = VerticalAlignment.Top };

            Card.Background = new SolidColorBrush(Colors.White);
            Card.Margin = new Thickness(20);

            Grid.SetRow(Card, 0);

            Grid Header = new Grid() { Height = 50 };
            Header.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF105A97");

            Label header = new Label() { Content = "SPEC " + (mainGrid.Children.Count + 1), Style = (Style)FindResource("tableHeaderItem") };
            header.HorizontalAlignment = HorizontalAlignment.Left;

            Header.Children.Add(header);


            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(45, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(80, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(80, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Pixel) });
            Card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Pixel) });



            Grid.SetRow(Header, 0);

            Label ratedPowerLable = new Label() { Style = (Style)FindResource("labelStyle") };
            ratedPowerLable.Content = "Rated Power";
            Label ratedPowerlabelInvisible = new Label() { Style = (Style)FindResource("labelStyle") };
            ratedPowerlabelInvisible.Visibility = Visibility.Collapsed;

            TextBox RatedPowerText = new TextBox() { Style = (Style)FindResource("textBoxStyle"), Width = 253 };

            ComboBox ratedPowercombo = new ComboBox() { Style = (Style)FindResource("comboBoxStyle"), Width = 90 };

            WrapPanel ratedPowerPanel = new WrapPanel();

            ratedPowerPanel.Children.Add(ratedPowerLable);
            ratedPowerPanel.Children.Add(RatedPowerText);
            ratedPowerPanel.Children.Add(ratedPowercombo);
            ratedPowerPanel.Children.Add(ratedPowerlabelInvisible);

            Grid.SetRow(ratedPowerPanel, 1);

            WrapPanel EnginePanel = new WrapPanel();

            Label engineLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            engineLabel.Content = "Engine";

            ComboBox engineCombo = new ComboBox() { Style = (Style)FindResource("comboBoxStyle") };

            EnginePanel.Children.Add(engineLabel);
            EnginePanel.Children.Add(engineCombo);

            Grid.SetRow(EnginePanel, 2);


            WrapPanel modelPanel = new WrapPanel();

            Label ModelLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            ModelLabel.Content = "Model";

            TextBox ModelText = new TextBox() { Style = (Style)FindResource("textBoxStyle") };

            Label ModellabelInvisible = new Label() { Style = (Style)FindResource("labelStyle") };
            ModellabelInvisible.Visibility = Visibility.Collapsed;

            modelPanel.Children.Add(ModelLabel);
            modelPanel.Children.Add(ModelText);
            modelPanel.Children.Add(ModellabelInvisible);
            Grid.SetRow(modelPanel, 3);


            WrapPanel LtbKva50Panel = new WrapPanel();

            Label ltbKva50Label = new Label() { Style = (Style)FindResource("labelStyle") };
            ltbKva50Label.Content = "LTB Kva 50";

            TextBox kva50TextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle") };

            Label Kva50labelInvisible = new Label() { Style = (Style)FindResource("labelStyle") };
            Kva50labelInvisible.Visibility = Visibility.Collapsed;

            LtbKva50Panel.Children.Add(ltbKva50Label);
            LtbKva50Panel.Children.Add(kva50TextBox);
            LtbKva50Panel.Children.Add(Kva50labelInvisible);

            Grid.SetRow(LtbKva50Panel, 4);


            WrapPanel LTPkVA60HZPanel = new WrapPanel();

            Label ltbKva60Label = new Label() { Style = (Style)FindResource("labelStyle") };
            ltbKva60Label.Content = "LTB Kva 60";

            TextBox kva60TextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle") };

            Label Kva60labelInvisible = new Label() { Style = (Style)FindResource("labelStyle") };
            Kva60labelInvisible.Visibility = Visibility.Collapsed;


            LTPkVA60HZPanel.Children.Add(ltbKva60Label);
            LTPkVA60HZPanel.Children.Add(kva60TextBox);
            LTPkVA60HZPanel.Children.Add(Kva60labelInvisible);

            Grid.SetRow(LTPkVA60HZPanel, 5);


            WrapPanel PRPkVA50HZPanel = new WrapPanel();

            Label prpKva50Label = new Label() { Style = (Style)FindResource("labelStyle") };
            prpKva50Label.Content = "PRP Kva 50";

            TextBox prpkva50TextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle") };

            Label prpKva50labelInvisible = new Label() { Style = (Style)FindResource("labelStyle") };
            prpKva50labelInvisible.Visibility = Visibility.Collapsed;

            PRPkVA50HZPanel.Children.Add(prpKva50Label);
            PRPkVA50HZPanel.Children.Add(prpkva50TextBox);
            PRPkVA50HZPanel.Children.Add(prpKva50labelInvisible);

            Grid.SetRow(PRPkVA50HZPanel, 6);


            WrapPanel PRPkVA60HZPanel = new WrapPanel();

            Label prpKva60Label = new Label() { Style = (Style)FindResource("labelStyle") };
            prpKva60Label.Content = "PRP Kva 60";

            TextBox prpkva60TextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle") };


            Label prpKva60labelInvisible = new Label() { Style = (Style)FindResource("labelStyle") };
            prpKva60labelInvisible.Visibility = Visibility.Collapsed;

            PRPkVA60HZPanel.Children.Add(prpKva60Label);
            PRPkVA60HZPanel.Children.Add(prpkva60TextBox);
            PRPkVA60HZPanel.Children.Add(prpKva60labelInvisible);

            Grid.SetRow(PRPkVA60HZPanel, 7);



            WrapPanel MinPanel = new WrapPanel();

            Label MinLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            MinLabel.Content = "Min";

            TextBox MinTextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle") };

              Label minlabelInvisible = new Label() { Style = (Style)FindResource("labelStyle") };
            minlabelInvisible.Visibility = Visibility.Collapsed;

            MinPanel.Children.Add(MinLabel);
            MinPanel.Children.Add(MinTextBox);
            MinPanel.Children.Add(minlabelInvisible);

            Grid.SetRow(MinPanel, 8);



            WrapPanel MidPanel = new WrapPanel();

            Label MidLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            MidLabel.Content = "AVG";

            TextBox MidTextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle") };

            Label midlabelInvisible = new Label() { Style = (Style)FindResource("labelStyle") };
            midlabelInvisible.Visibility = Visibility.Collapsed;

            MidPanel.Children.Add(MidLabel);
            MidPanel.Children.Add(MidTextBox);
            MidPanel.Children.Add(midlabelInvisible);

            Grid.SetRow(MidPanel, 9);


            WrapPanel MaxPanel = new WrapPanel();

            Label MaxLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            MaxLabel.Content = "Max";

            TextBox MaxTextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle") };


            Label maxlabelInvisible = new Label() { Style = (Style)FindResource("labelStyle") };
            maxlabelInvisible.Visibility = Visibility.Collapsed;

            MaxPanel.Children.Add(MaxLabel);
            MaxPanel.Children.Add(MaxTextBox);
            MaxPanel.Children.Add(maxlabelInvisible);

            Grid.SetRow(MaxPanel, 10);


            WrapPanel CoolingPanel = new WrapPanel();

            Label CoolingLabel = new Label() { Style = (Style)FindResource("labelStyle")};
            CoolingLabel.Content = "Cooling";

            TextBox coolingTextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle")};

            Label coolinglabelInvisible = new Label() { Style = (Style)FindResource("labelStyle") };
            coolinglabelInvisible.Visibility = Visibility.Collapsed;


            CoolingPanel.Children.Add(CoolingLabel);
            CoolingPanel.Children.Add(coolingTextBox);
            CoolingPanel.Children.Add(coolinglabelInvisible);

            Grid.SetRow(CoolingPanel, 11);


            WrapPanel TankPanel = new WrapPanel();

            Label TankLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            TankLabel.Content = "TANK";

            TextBox TankTextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle") };


            Label tanklabelInvisible = new Label() { Style = (Style)FindResource("labelStyle") };
            tanklabelInvisible.Visibility = Visibility.Collapsed;

            TankPanel.Children.Add(TankLabel);
            TankPanel.Children.Add(TankTextBox);
            TankPanel.Children.Add(tanklabelInvisible);

            Grid.SetRow(TankPanel, 12);


            WrapPanel LOADPanel = new WrapPanel();

            Label LoadLabel = new Label() { Style = (Style)FindResource("labelStyle")};
            LoadLabel.Content = "LOAD";

            TextBox loadTextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle")};


            Label loadlabelInvisible = new Label() { Style = (Style)FindResource("labelStyle") };
            loadlabelInvisible.Visibility = Visibility.Collapsed;

            LOADPanel.Children.Add(LoadLabel);
            LOADPanel.Children.Add(loadTextBox);
            LOADPanel.Children.Add(loadlabelInvisible);

            Grid.SetRow(LOADPanel, 13);

            WrapPanel MainFeaturesPanel = new WrapPanel();

            Label MainFeatureLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            MainFeatureLabel.Content = $"Main{'\n'}Feature";

            ComboBox MainComboBox = new ComboBox() { Style = (Style)FindResource("comboBoxStyle") };

            MainFeaturesPanel.Children.Add(MainFeatureLabel);
            MainFeaturesPanel.Children.Add(MainComboBox);

            Grid.SetRow(MainFeaturesPanel, 14);


            WrapPanel ApplicationsPanel = new WrapPanel();

            Label applicationLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            applicationLabel.Content = "Applications";

            ComboBox ApplicationComboBox = new ComboBox() { Style = (Style)FindResource("comboBoxStyle") };

            ApplicationsPanel.Children.Add(applicationLabel);
            ApplicationsPanel.Children.Add(ApplicationComboBox);

            Grid.SetRow(ApplicationsPanel, 15);


            WrapPanel EnginePanell = new WrapPanel();

            Label EngineLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            EngineLabel.Content = "Engine";

            ComboBox engineComboBox = new ComboBox() { Style = (Style)FindResource("comboBoxStyle") };

            EnginePanell.Children.Add(EngineLabel);
            EnginePanell.Children.Add(engineComboBox);

            Grid.SetRow(EnginePanell, 16);


            WrapPanel AlternatorPanel = new WrapPanel();

            Label AlternatorLable = new Label() { Style = (Style)FindResource("labelStyle") };
            AlternatorLable.Content = "Alternator";

            ComboBox AlternatorComboBox = new ComboBox() { Style = (Style)FindResource("comboBoxStyle") };

            AlternatorPanel.Children.Add(AlternatorLable);
            AlternatorPanel.Children.Add(AlternatorComboBox);

            Grid.SetRow(AlternatorPanel, 17);


            WrapPanel AlternatorFeaturesPanel = new WrapPanel();

            Label AlternatorFeaturesLable = new Label() { Style = (Style)FindResource("labelStyle") };
            AlternatorFeaturesLable.Content = "Alternator" + '\n' + "Features";

            ComboBox AlternatorFeatureComboBox = new ComboBox() { Style = (Style)FindResource("comboBoxStyle") };

            AlternatorFeaturesPanel.Children.Add(AlternatorFeaturesLable);
            AlternatorFeaturesPanel.Children.Add(AlternatorFeatureComboBox);

            Grid.SetRow(AlternatorFeaturesPanel, 18);



            WrapPanel BatteriesPanel = new WrapPanel();

            Label BatteriesLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            BatteriesLabel.Content = "Batteries";

            ComboBox BatteriesComboBox = new ComboBox() { Style = (Style)FindResource("comboBoxStyle") };

            BatteriesPanel.Children.Add(BatteriesLabel);
            BatteriesPanel.Children.Add(BatteriesComboBox);

            Grid.SetRow(BatteriesPanel, 19);


            WrapPanel ExhaustPanel = new WrapPanel();

            Label ExhaustLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            ExhaustLabel.Content = "Exhaust";

            ComboBox ExhaustComboBox = new ComboBox() { Style = (Style)FindResource("comboBoxStyle") };

            ExhaustPanel.Children.Add(ExhaustLabel);
            ExhaustPanel.Children.Add(ExhaustComboBox);

            Grid.SetRow(ExhaustPanel, 20);


            WrapPanel TanksPanel = new WrapPanel();

            Label TanksLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            TanksLabel.Content = "Tanks";

            ComboBox TanksComboBox = new ComboBox() { Style = (Style)FindResource("comboBoxStyle") };

            TanksPanel.Children.Add(TanksLabel);
            TanksPanel.Children.Add(TanksComboBox);

            Grid.SetRow(TanksPanel, 21);

            WrapPanel ContainterPanel = new WrapPanel();

            Label ContainterLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            ContainterLabel.Content = "Containter";

            ComboBox ContainterComboBox = new ComboBox() { Style = (Style)FindResource("comboBoxStyle") };

            ContainterPanel.Children.Add(ContainterLabel);
            ContainterPanel.Children.Add(ContainterComboBox);

            Grid.SetRow(ContainterPanel, 22);
            Card.Children.Add(Header);
            Card.Children.Add(ratedPowerPanel);
            Card.Children.Add(EnginePanel);
            Card.Children.Add(modelPanel);
            Card.Children.Add(LtbKva50Panel);
            Card.Children.Add(LTPkVA60HZPanel);
            Card.Children.Add(PRPkVA50HZPanel);
            Card.Children.Add(PRPkVA60HZPanel);
            Card.Children.Add(MinPanel);
            Card.Children.Add(MidPanel);
            Card.Children.Add(MaxPanel);
            Card.Children.Add(CoolingPanel);
            Card.Children.Add(TankPanel);
            Card.Children.Add(LOADPanel);
            Card.Children.Add(MainFeaturesPanel);
            Card.Children.Add(ApplicationsPanel);
            Card.Children.Add(EnginePanell);
            Card.Children.Add(AlternatorPanel);
            Card.Children.Add(AlternatorFeaturesPanel);
            Card.Children.Add(BatteriesPanel);
            Card.Children.Add(ExhaustPanel);
            Card.Children.Add(TanksPanel);
            Card.Children.Add(ContainterPanel);
            mainGrid.Children.Add(Card);
        }
        
        private void InitializeNewCard()
        {
            RowDefinition row = new RowDefinition();
            mainGrid.RowDefinitions.Add(row);
            
            Grid Card =new Grid();

            Card.Background = new SolidColorBrush(Colors.White);
            Card.Margin = new Thickness(20);
            
            Grid.SetRow(Card, mainGrid.RowDefinitions.Count()-1);

            RowDefinition CardRow1 = new RowDefinition();
            RowDefinition CardRow2 = new RowDefinition();

            CardRow1.Height = new GridLength(50, GridUnitType.Pixel); 

            Grid Header = new Grid();
            Header.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#105A97");

            Grid Content = new Grid();
            
            Grid.SetRow(Header, 0);
            Grid.SetRow(Content, 1);

            Grid.SetColumn(Header, 0);
            Grid.SetColumn(Content, 0);

            Label CardLabel = new Label();
            CardLabel.Style = (Style)FindResource("tableHeaderItem");
            CardLabel.Margin = new Thickness(20, 0, 0, 0);
            CardLabel.HorizontalAlignment = HorizontalAlignment.Left;
            CardLabel.Content = "SPEC " + (mainGrid.Children.Count+1);

            Image delete = new Image();
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("Icons\\cross_icon.jpg", UriKind.Relative);
            bi3.EndInit();
            delete.Source = bi3;
            delete.Width = 20;
            delete.Height = 20;
            delete.Margin = new Thickness(10, 0, 10, 0);
            delete.Tag = mainGrid.RowDefinitions.Count - 1;
            delete.MouseLeftButtonDown += Delete_MouseLeftButtonDown;
            


            ColumnDefinition Col = new ColumnDefinition();
            ColumnDefinition Col2 = new ColumnDefinition();

            Col.Width = new GridLength(2, GridUnitType.Star);
            Col2.Width = new GridLength(0.10, GridUnitType.Star);

            Grid.SetRow(CardLabel, 0);
            Grid.SetRow(delete, 0);

            Grid.SetColumn(CardLabel, 0);
            Grid.SetColumn(delete, 1);

            Header.ColumnDefinitions.Add(Col);
            Header.ColumnDefinitions.Add(Col2);

            Header.Children.Add(CardLabel);
            Header.Children.Add(delete);

            Card.RowDefinitions.Add(CardRow1);
            Card.RowDefinitions.Add(CardRow2);

            Grid grid = new Grid();

            Card.Tag = mainGrid.RowDefinitions.Count()-1 ;

            Grid.SetRow(grid,mainGrid.RowDefinitions.Count() - 1);
            
            RowDefinition gridRow1  = new RowDefinition();
            RowDefinition gridRow2  = new RowDefinition();
            RowDefinition gridRow3  = new RowDefinition();
            RowDefinition gridRow4  = new RowDefinition();
            RowDefinition gridRow5  = new RowDefinition();
            RowDefinition gridRow6  = new RowDefinition();
            RowDefinition gridRow7  = new RowDefinition();
            RowDefinition gridRow8  = new RowDefinition();
            RowDefinition gridRow9  = new RowDefinition();
            RowDefinition gridRow10 = new RowDefinition();
            RowDefinition gridRow11 = new RowDefinition();
            RowDefinition gridRow12 = new RowDefinition();
            RowDefinition gridRow13 = new RowDefinition();
            RowDefinition gridRow14 = new RowDefinition();

            grid.RowDefinitions.Add(gridRow1 );
            grid.RowDefinitions.Add(gridRow2 );
            grid.RowDefinitions.Add(gridRow3 );
            grid.RowDefinitions.Add(gridRow4 );
            grid.RowDefinitions.Add(gridRow5 );
            grid.RowDefinitions.Add(gridRow6 );
            grid.RowDefinitions.Add(gridRow7 );
            grid.RowDefinitions.Add(gridRow8 );
            grid.RowDefinitions.Add(gridRow9 );
            grid.RowDefinitions.Add(gridRow10);
            grid.RowDefinitions.Add(gridRow11);
            grid.RowDefinitions.Add(gridRow12);
            grid.RowDefinitions.Add(gridRow13);
            grid.RowDefinitions.Add(gridRow14);

            WrapPanel wrapPanel1  = new WrapPanel();
            WrapPanel wrapPanel2  = new WrapPanel();
            WrapPanel wrapPanel3  = new WrapPanel();
            WrapPanel wrapPanel4  = new WrapPanel();
            WrapPanel wrapPanel5  = new WrapPanel();
            WrapPanel wrapPanel6  = new WrapPanel();
            WrapPanel wrapPanel7  = new WrapPanel();
            WrapPanel wrapPanel8  = new WrapPanel();
            WrapPanel wrapPanel9  = new WrapPanel();
            WrapPanel wrapPanel10 = new WrapPanel();
            WrapPanel wrapPanel11 = new WrapPanel();
            WrapPanel wrapPanel12 = new WrapPanel();
            WrapPanel wrapPanel13 = new WrapPanel();
            WrapPanel wrapPanel14 = new WrapPanel();
             
            Grid.SetRow(wrapPanel1 ,0 );
            Grid.SetRow(wrapPanel2 ,1 );
            Grid.SetRow(wrapPanel3 ,2 );
            Grid.SetRow(wrapPanel4 ,3 );
            Grid.SetRow(wrapPanel5 ,4 );
            Grid.SetRow(wrapPanel6 ,5 );
            Grid.SetRow(wrapPanel7 ,6 );
            Grid.SetRow(wrapPanel8 ,7 );
            Grid.SetRow(wrapPanel9 ,8 );
            Grid.SetRow(wrapPanel10,9 );
            Grid.SetRow(wrapPanel11,10);
            Grid.SetRow(wrapPanel12,11);
            Grid.SetRow(wrapPanel13,12);
            Grid.SetRow(wrapPanel14,13);

            Grid.SetColumn(wrapPanel1 , 0);
            Grid.SetColumn(wrapPanel2 , 0);
            Grid.SetColumn(wrapPanel3 , 0);
            Grid.SetColumn(wrapPanel4 , 0);
            Grid.SetColumn(wrapPanel5 , 0);
            Grid.SetColumn(wrapPanel6 , 0);
            Grid.SetColumn(wrapPanel7 , 0);
            Grid.SetColumn(wrapPanel8 , 0);
            Grid.SetColumn(wrapPanel9 , 0);
            Grid.SetColumn(wrapPanel10, 0);
            Grid.SetColumn(wrapPanel11, 0);
            Grid.SetColumn(wrapPanel12, 0);
            Grid.SetColumn(wrapPanel13, 0);
            Grid.SetColumn(wrapPanel14, 0);

            WrapPanel wrapPanelRow1  = new WrapPanel();
            WrapPanel wrapPanelRow2  = new WrapPanel();
            WrapPanel wrapPanelRow3  = new WrapPanel();
            WrapPanel wrapPanelRow4  = new WrapPanel();
            WrapPanel wrapPanelRow5  = new WrapPanel();
            WrapPanel wrapPanelRow6  = new WrapPanel();
            WrapPanel wrapPanelRow7  = new WrapPanel();
            WrapPanel wrapPanelRow8  = new WrapPanel();
            WrapPanel wrapPanelRow9  = new WrapPanel();
            WrapPanel wrapPanelRow10 = new WrapPanel();
            WrapPanel wrapPanelRow11 = new WrapPanel();
            WrapPanel wrapPanelRow12 = new WrapPanel();
            WrapPanel wrapPanelRow13 = new WrapPanel();
            WrapPanel wrapPanelRow14 = new WrapPanel();
            WrapPanel wrapPanelRow15 = new WrapPanel();
            WrapPanel wrapPanelRow16 = new WrapPanel();
            WrapPanel wrapPanelRow17 = new WrapPanel();
            WrapPanel wrapPanelRow18 = new WrapPanel();
            WrapPanel wrapPanelRow19 = new WrapPanel();
            WrapPanel wrapPanelRow20 = new WrapPanel();
            WrapPanel wrapPanelRow21 = new WrapPanel();
            WrapPanel wrapPanelRow22 = new WrapPanel();
            WrapPanel wrapPanelRow23 = new WrapPanel();
            WrapPanel wrapPanelRow24 = new WrapPanel();
            WrapPanel wrapPanelRow25 = new WrapPanel();
            WrapPanel wrapPanelRow26 = new WrapPanel();
            WrapPanel wrapPanelRow27 = new WrapPanel();
            WrapPanel wrapPanelRow28 = new WrapPanel();

            //Label Spec = new Label();
            //Spec.Content = "Spec " + mainGrid.RowDefinitions.Count();
            //Spec.Style = (Style)FindResource("largeLabelStyle");
            //Spec.Width = 210;
            //Spec.Padding = new Thickness(0, 0, 0, 25);
            //Spec.Margin = new Thickness(-65, 25, 0, 0);
            //Spec.FontWeight = FontWeights.Bold;
            //Spec.FontSize = 20;

            Label IOPhase = new Label();
            IOPhase.Content = "IO Phase*";
            IOPhase.Style= (Style)FindResource("labelStyle");
            IOPhase.Width = 210;

            TextBox IOPhaseTextBox = new TextBox();
            IOPhaseTextBox.Style = (Style)FindResource("textBoxStyle");
            IOPhaseTextBox.TextWrapping = TextWrapping.Wrap;

            Label IOPhaseLabel = new Label();
            IOPhaseLabel.Style = (Style)FindResource("labelStyle");
            IOPhaseLabel.Visibility = Visibility.Collapsed;



            Label ratedPower = new Label();
            ratedPower.Content = "rated Power*";
            ratedPower.Style = (Style)FindResource("labelStyle");
            ratedPower.Width = 210;

            TextBox ratedPowerTextBox = new TextBox();
            ratedPowerTextBox.Style = (Style)FindResource("textBoxStyle");
            ratedPowerTextBox.TextWrapping = TextWrapping.Wrap;
            ratedPowerTextBox.Text = "0.0";
            ratedPowerTextBox.Width = 250;

            Label ratedPowerLabel = new Label();
            ratedPowerLabel.Style = (Style)FindResource("labelStyle");
            ratedPowerLabel.Visibility = Visibility.Collapsed;

            ComboBox ratingComboBox = new ComboBox();
            ratingComboBox.Style = (Style)FindResource("comboBoxStyle");
            ratingComboBox.SelectionChanged += ratingComboBoxSelectionChanged;
            ratingComboBox.Width = 90;

            ratingComboBox.Items.Clear();
            for (int i = 0; i < rating.Count; i++)
            {
                ratingComboBox.Items.Add(rating[i].measure_unit);
            }
            ratingComboBox.SelectedIndex = 0;
            
            Label ratingLabel = new Label();
            ratingLabel.Style = (Style)FindResource("labelStyle");
            ratingLabel.Visibility = Visibility.Collapsed;

            Label BackupLabel = new Label();
            BackupLabel.Content = "Backup time (in min)*";
            BackupLabel.Style = (Style)FindResource("labelStyle");
            BackupLabel.Width = 210;



            Grid backupGrid = new Grid();

            ColumnDefinition BackupColumnDefinition1 = new ColumnDefinition();
            ColumnDefinition BackupColumnDefinition2 = new ColumnDefinition();
            ColumnDefinition BackupColumnDefinition3 = new ColumnDefinition();
            ColumnDefinition BackupColumnDefinition4 = new ColumnDefinition();

            
            backupGrid.ColumnDefinitions.Add(BackupColumnDefinition1);
            backupGrid.ColumnDefinitions.Add(BackupColumnDefinition2);
            backupGrid.ColumnDefinitions.Add(BackupColumnDefinition3);
            backupGrid.ColumnDefinitions.Add(BackupColumnDefinition4);

            WrapPanel backupTime50WrapPanel  = new WrapPanel();
            WrapPanel backupTime70WrapPanel  = new WrapPanel();
            WrapPanel backupTime100WrapPanel = new WrapPanel();

            backupTime50WrapPanel.Margin=new Thickness(12, 0, 0, 0);
            

            Grid.SetColumn(backupTime50WrapPanel, 1);
            Grid.SetColumn(backupTime70WrapPanel, 2);
            Grid.SetColumn(backupTime100WrapPanel,3);

            Grid.SetRow(backupTime50WrapPanel, 0);
            Grid.SetRow(backupTime70WrapPanel, 0);
            Grid.SetRow(backupTime100WrapPanel,0);

            Label m50Label  = new Label();
            m50Label.Style = (Style)FindResource("labelStyle");
            m50Label.Content = "50%";
            m50Label.Width = 40;
            m50Label.Margin = new Thickness(5, 0, 0, 0);

            TextBox backupTime50TextBox = new TextBox();
            backupTime50TextBox.Style = (Style)FindResource("textBoxStyle");
            backupTime50TextBox.Width = 30;
            backupTime50TextBox.Text ="0";
            backupTime50TextBox.Margin = new Thickness(12);

            Label backupTime50Label = new Label();
            backupTime50Label.Style = (Style)FindResource("labelStyle");
            backupTime50Label.Visibility = Visibility.Collapsed;
            backupTime50Label.Width = 30;
            backupTime50Label.Margin = new Thickness(12);

            Label backupTime50LabelMin = new Label();
            backupTime50LabelMin.Style = (Style)FindResource("labelStyle");
            backupTime50LabelMin.Content = "min";
            backupTime50LabelMin.Width = 50;
            backupTime50LabelMin.Margin = new Thickness(-15, 0, 0, 0);

            Label m70Label = new Label();
            m70Label.Style = (Style)FindResource("labelStyle");
            m70Label.Content = "70%";
            m70Label.Width = 40;
            m70Label.Margin = new Thickness(5, 0, 0, 0);

            TextBox backupTime70TextBox = new TextBox();
            backupTime70TextBox.Style = (Style)FindResource("textBoxStyle");
            backupTime70TextBox.Width = 30;
            backupTime70TextBox.Text = "0";
            backupTime70TextBox.Margin = new Thickness(12);

            Label backupTime70Label = new Label();
            backupTime70Label.Style = (Style)FindResource("labelStyle");
            backupTime70Label.Visibility = Visibility.Collapsed;
            backupTime70Label.Width = 30;
            backupTime70Label.Margin = new Thickness(12);

            Label backupTime70LabelMin = new Label();
            backupTime70LabelMin.Style = (Style)FindResource("labelStyle");
            backupTime70LabelMin.Content = "min";
            backupTime70LabelMin.Width = 50;
            backupTime70LabelMin.Margin = new Thickness(-15, 0, 0, 0);

            Label m100Label = new Label();
            m100Label.Style = (Style)FindResource("labelStyle");
            m100Label.Content = "100%";
            m100Label.Width = 50;
            m100Label.Margin = new Thickness(5, 0, 0, 0);

            TextBox backupTime100TextBox = new TextBox();
            backupTime100TextBox.Style = (Style)FindResource("textBoxStyle");
            backupTime100TextBox.Width = 30;
            backupTime100TextBox.Text = "0";
            backupTime100TextBox.Margin = new Thickness(12);

            Label backupTime100Label = new Label();
            backupTime100Label.Style = (Style)FindResource("labelStyle");
            backupTime100Label.Visibility = Visibility.Collapsed;
            backupTime100Label.Width = 30;
            backupTime100Label.Margin = new Thickness(12);

            Label backupTime100LabelMin = new Label();
            backupTime100LabelMin.Style = (Style)FindResource("labelStyle");
            backupTime100LabelMin.Content = "min";
            backupTime100LabelMin.Width = 50;
            backupTime100LabelMin.Margin = new Thickness(-15, 0, 0, 0);

            backupTime50WrapPanel.Children.Add(m50Label);
            backupTime50WrapPanel.Children.Add(backupTime50TextBox);
            backupTime50WrapPanel.Children.Add(backupTime50Label);
            backupTime50WrapPanel.Children.Add(backupTime50LabelMin);
            backupTime70WrapPanel.Children.Add(m70Label);
            backupTime70WrapPanel.Children.Add(backupTime70TextBox);
            backupTime70WrapPanel.Children.Add(backupTime70Label);
            backupTime70WrapPanel.Children.Add(backupTime70LabelMin);
            backupTime100WrapPanel.Children.Add(m100Label);
            backupTime100WrapPanel.Children.Add(backupTime100TextBox);
            backupTime100WrapPanel.Children.Add(backupTime100Label);
            backupTime100WrapPanel.Children.Add(backupTime100LabelMin);

            backupGrid.Children.Add(backupTime50WrapPanel);
            backupGrid.Children.Add(backupTime70WrapPanel);
            backupGrid.Children.Add(backupTime100WrapPanel);


            Label inputPowerFactorPhase = new Label();
            inputPowerFactorPhase.Content = "Input Power Factor";
            inputPowerFactorPhase.Style = (Style)FindResource("labelStyle");
            inputPowerFactorPhase.Width = 210;

            TextBox inputPowerFactorPhaseTextBox = new TextBox();
            inputPowerFactorPhaseTextBox.Style = (Style)FindResource("textBoxStyle");
            inputPowerFactorPhaseTextBox.TextWrapping = TextWrapping.Wrap;

            Label inputPowerFactorPhaseLabel = new Label();
            inputPowerFactorPhaseLabel.Style = (Style)FindResource("labelStyle");
            inputPowerFactorPhaseLabel.Visibility = Visibility.Collapsed;


            Label THDI = new Label();
            THDI.Content = "THDI";
            THDI.Style = (Style)FindResource("labelStyle");
            THDI.Width = 210;

            TextBox THDITextBox = new TextBox();
            THDITextBox.Style = (Style)FindResource("textBoxStyle");
            THDITextBox.TextWrapping = TextWrapping.Wrap;

            Label THDILabel = new Label();
            THDILabel.Style = (Style)FindResource("labelStyle");
            THDILabel.Visibility = Visibility.Collapsed;


            Label inputNominalVoltage = new Label();
            inputNominalVoltage.Content = "Input Voltage";
            inputNominalVoltage.Style = (Style)FindResource("labelStyle");
            inputNominalVoltage.Width = 210;

            TextBox inputNominalVoltageTextBox = new TextBox();
            inputNominalVoltageTextBox.Style = (Style)FindResource("textBoxStyle");
            inputNominalVoltageTextBox.TextWrapping = TextWrapping.Wrap;

            Label inputNominalVoltageLabel = new Label();
            inputNominalVoltageLabel.Style = (Style)FindResource("labelStyle");
            inputNominalVoltageLabel.Visibility = Visibility.Collapsed;


            Label inputVoltage = new Label();
            inputVoltage.Content = "Input Nominal Voltage";
            inputVoltage.Style = (Style)FindResource("labelStyle");
            inputVoltage.Width = 210;

            TextBox inputVoltageTextBox = new TextBox();
            inputVoltageTextBox.Style = (Style)FindResource("textBoxStyle");
            inputVoltageTextBox.TextWrapping = TextWrapping.Wrap;

            Label inputVoltageLabel = new Label();
            inputVoltageLabel.Style = (Style)FindResource("labelStyle");
            inputVoltageLabel.Visibility = Visibility.Collapsed;

            Label voltageTolerance = new Label();
            voltageTolerance.Content = "Voltage Tolerance";
            voltageTolerance.Style = (Style)FindResource("labelStyle");
            voltageTolerance.Width = 210;

            TextBox voltageToleranceTextBox = new TextBox();
            voltageToleranceTextBox.Style = (Style)FindResource("textBoxStyle");
            voltageToleranceTextBox.TextWrapping = TextWrapping.Wrap;

            Label voltageToleranceLabel = new Label();
            voltageToleranceLabel.Style = (Style)FindResource("labelStyle");
            voltageToleranceLabel.Visibility = Visibility.Collapsed;



            Label outputPowerFactor = new Label();
            outputPowerFactor.Content = "Output Power Factor";
            outputPowerFactor.Style = (Style)FindResource("labelStyle");
            outputPowerFactor.Width = 210;

            TextBox outputPowerFactorTextBox = new TextBox();
            outputPowerFactorTextBox.Style = (Style)FindResource("textBoxStyle");
            outputPowerFactorTextBox.TextWrapping = TextWrapping.Wrap;

            Label outputPowerFactorLabel = new Label();
            outputPowerFactorLabel.Style = (Style)FindResource("labelStyle");
            outputPowerFactorLabel.Visibility = Visibility.Collapsed;

            Label THDV = new Label();
            THDV.Content = "THDV";
            THDV.Style = (Style)FindResource("labelStyle");
            THDV.Width = 210;

            TextBox THDVTextBox = new TextBox();
            THDVTextBox.Style = (Style)FindResource("textBoxStyle");
            THDVTextBox.TextWrapping = TextWrapping.Wrap;

            Label THDVLabel = new Label();
            THDVLabel.Style = (Style)FindResource("labelStyle");
            THDVLabel.Visibility = Visibility.Collapsed;

            Label outputNominalVoltage = new Label();
            outputNominalVoltage.Content = "Output Nominal Voltage";
            outputNominalVoltage.Style = (Style)FindResource("labelStyle");
            outputNominalVoltage.Width = 210;

            TextBox outputNominalVoltageTextBox = new TextBox();
            outputNominalVoltageTextBox.Style = (Style)FindResource("textBoxStyle");
            outputNominalVoltageTextBox.TextWrapping = TextWrapping.Wrap;

            Label outputNominalVoltageLabel = new Label();
            outputNominalVoltageLabel.Style = (Style)FindResource("labelStyle");
            outputNominalVoltageLabel.Visibility = Visibility.Collapsed;

            Label outputDCVoltageRange = new Label();
            outputDCVoltageRange.Content = "Output DC Voltage Range";
            outputDCVoltageRange.Style = (Style)FindResource("labelStyle");
            outputDCVoltageRange.Width = 210;

            TextBox outputDCVoltageRangeTextBox = new TextBox();
            outputDCVoltageRangeTextBox.Style = (Style)FindResource("textBoxStyle");
            outputDCVoltageRangeTextBox.TextWrapping = TextWrapping.Wrap;

            Label outputDCVoltageRangeLabel = new Label();
            outputDCVoltageRangeLabel.Style = (Style)FindResource("labelStyle");
            outputDCVoltageRangeLabel.Visibility = Visibility.Collapsed;

            Label overloadCapability = new Label();
            overloadCapability.Content = "Overload Capability";
            overloadCapability.Style = (Style)FindResource("labelStyle");
            overloadCapability.Width = 210;

            TextBox overloadCapabilityTextBox = new TextBox();
            overloadCapabilityTextBox.Style = (Style)FindResource("textBoxStyle");
            overloadCapabilityTextBox.TextWrapping = TextWrapping.Wrap;

            Label overloadCapabilityLabel = new Label();
            overloadCapabilityLabel.Style = (Style)FindResource("labelStyle");
            overloadCapabilityLabel.Visibility = Visibility.Collapsed;

            Label efficiency = new Label();
            efficiency.Content = "Efficiency";
            efficiency.Style = (Style)FindResource("labelStyle");
            efficiency.Width = 210;

            TextBox efficiencyTextBox = new TextBox();
            efficiencyTextBox.Style = (Style)FindResource("textBoxStyle");
            efficiencyTextBox.TextWrapping = TextWrapping.Wrap;

            Label efficiencyLabel = new Label();
            efficiencyLabel.Style = (Style)FindResource("labelStyle");
            efficiencyLabel.Visibility = Visibility.Collapsed;

            Label inputConnectionType = new Label();
            inputConnectionType.Content = "Input Connection Type";
            inputConnectionType.Style = (Style)FindResource("labelStyle");
            inputConnectionType.Width = 210;

            TextBox inputConnectionTypeTextBox = new TextBox();
            inputConnectionTypeTextBox.Style = (Style)FindResource("textBoxStyle");
            inputConnectionTypeTextBox.TextWrapping = TextWrapping.Wrap;

            Label inputConnectionTypeLabel = new Label();
            inputConnectionTypeLabel.Style = (Style)FindResource("labelStyle");
            inputConnectionTypeLabel.Visibility = Visibility.Collapsed;

            Label frontPanel = new Label();
            frontPanel.Content = "Front Panel";
            frontPanel.Style = (Style)FindResource("labelStyle");
            frontPanel.Width = 210;

            TextBox frontPanelTextBox = new TextBox();
            frontPanelTextBox.Style = (Style)FindResource("textBoxStyle");
            frontPanelTextBox.TextWrapping = TextWrapping.Wrap;

            Label frontPanelLabel = new Label();
            frontPanelLabel.Style = (Style)FindResource("labelStyle");
            frontPanelLabel.Visibility = Visibility.Collapsed;

            Label maxPower = new Label();
            maxPower.Content = "Max Power";
            maxPower.Style = (Style)FindResource("labelStyle");
            maxPower.Width = 210;

            TextBox maxPowerTextBox = new TextBox();
            maxPowerTextBox.Style = (Style)FindResource("textBoxStyle");
            maxPowerTextBox.TextWrapping = TextWrapping.Wrap;

            Label maxPowerLabel = new Label();
            maxPowerLabel.Style = (Style)FindResource("labelStyle");
            maxPowerLabel.Visibility = Visibility.Collapsed;

            Label certificates = new Label();
            certificates.Content = "Certificates";
            certificates.Style = (Style)FindResource("labelStyle");
            certificates.Width = 210;

            TextBox certificatesTextBox = new TextBox();
            certificatesTextBox.Style = (Style)FindResource("textBoxStyle");
            certificatesTextBox.TextWrapping = TextWrapping.Wrap;

            Label certificatesLabel = new Label();
            certificatesLabel.Style = (Style)FindResource("labelStyle");
            certificatesLabel.Visibility = Visibility.Collapsed;

            Label safety = new Label();
            safety.Content = "Safety";
            safety.Style = (Style)FindResource("labelStyle");
            safety.Width = 210;

            TextBox safetyTextBox = new TextBox();
            safetyTextBox.Style = (Style)FindResource("textBoxStyle");
            safetyTextBox.TextWrapping = TextWrapping.Wrap;

            Label safetyLabel = new Label();
            safetyLabel.Style = (Style)FindResource("labelStyle");
            safetyLabel.Visibility = Visibility.Collapsed;

            Label EMC = new Label();
            EMC.Content = "EMC";
            EMC.Style = (Style)FindResource("labelStyle");
            EMC.Width = 210;

            TextBox EMCTextBox = new TextBox();
            EMCTextBox.Style = (Style)FindResource("textBoxStyle");
            EMCTextBox.TextWrapping = TextWrapping.Wrap;

            Label EMCLabel = new Label();
            EMCLabel.Style = (Style)FindResource("labelStyle");
            EMCLabel.Visibility = Visibility.Collapsed;

            Label environmentalAspects = new Label();
            environmentalAspects.Content = "Environmental Aspects";
            environmentalAspects.Style = (Style)FindResource("labelStyle");
            environmentalAspects.Width = 210;

            TextBox environmentalAspectsTextBox = new TextBox();
            environmentalAspectsTextBox.Style = (Style)FindResource("textBoxStyle");
            environmentalAspectsTextBox.TextWrapping = TextWrapping.Wrap;

            Label environmentalAspectsLabel = new Label();
            environmentalAspectsLabel.Style = (Style)FindResource("labelStyle");
            environmentalAspectsLabel.Visibility = Visibility.Collapsed;

            Label testPerformance = new Label();
            testPerformance.Content = "Test Performance";
            testPerformance.Style = (Style)FindResource("labelStyle");
            testPerformance.Width = 210;

            TextBox testPerformanceTextBox = new TextBox();
            testPerformanceTextBox.Style = (Style)FindResource("textBoxStyle");
            testPerformanceTextBox.TextWrapping = TextWrapping.Wrap;

            Label testPerformanceLabel = new Label();
            testPerformanceLabel.Style = (Style)FindResource("labelStyle");
            testPerformanceLabel.Visibility = Visibility.Collapsed;

            Label protectionDegree = new Label();
            protectionDegree.Content = "Protection Degree";
            protectionDegree.Style = (Style)FindResource("labelStyle");
            protectionDegree.Width = 210;

            TextBox protectionDegreeTextBox = new TextBox();
            protectionDegreeTextBox.Style = (Style)FindResource("textBoxStyle");
            protectionDegreeTextBox.TextWrapping = TextWrapping.Wrap;

            Label protectionDegreeLabel = new Label();
            protectionDegreeLabel.Style = (Style)FindResource("labelStyle");
            protectionDegreeLabel.Visibility = Visibility.Collapsed;

            Label transferVoltageLimit = new Label();
            transferVoltageLimit.Content = "Transfer Voltage Limit";
            transferVoltageLimit.Style = (Style)FindResource("labelStyle");
            transferVoltageLimit.Width = 210;

            TextBox transferVoltageLimitTextBox = new TextBox();
            transferVoltageLimitTextBox.Style = (Style)FindResource("textBoxStyle");
            transferVoltageLimitTextBox.TextWrapping = TextWrapping.Wrap;

            Label transferVoltageLimitLabel = new Label();
            transferVoltageLimitLabel.Style = (Style)FindResource("labelStyle");
            transferVoltageLimitLabel.Visibility = Visibility.Collapsed;

            Label marking = new Label();
            marking.Content = "Marking";
            marking.Style = (Style)FindResource("labelStyle");
            marking.Width = 210;

            TextBox markingTextBox = new TextBox();
            markingTextBox.Style = (Style)FindResource("textBoxStyle");
            markingTextBox.TextWrapping = TextWrapping.Wrap;

            Label markingLabel = new Label();
            markingLabel.Style = (Style)FindResource("labelStyle");
            markingLabel.Visibility = Visibility.Collapsed;

            Label validUntil = new Label();
            validUntil.Content = "Valid Until*";
            validUntil.Style = (Style)FindResource("labelStyle");
            validUntil.Width = 210;

            DatePicker validUntilDatePicker = new DatePicker();
            validUntilDatePicker.Style = (Style)FindResource("datePickerStyle");
            validUntilDatePicker.Width = 384;
            validUntilDatePicker.SelectedDate = new DateTime(2030,1,1);


            Label validUntilLabel = new Label();
            validUntilLabel.Style = (Style)FindResource("labelStyle");
            validUntilLabel.Visibility = Visibility.Collapsed;


            //wrapPanelRow1 .Children.Add(Spec);
            
            wrapPanelRow2 .Children.Add(IOPhase);
            wrapPanelRow2 .Children.Add(IOPhaseTextBox);
            wrapPanelRow2 .Children.Add(IOPhaseLabel);
            
            wrapPanelRow3 .Children.Add(ratedPower);
            wrapPanelRow3 .Children.Add(ratedPowerTextBox);
            wrapPanelRow3 .Children.Add(ratedPowerLabel);
            wrapPanelRow3 .Children.Add(ratingComboBox);
            wrapPanelRow3 .Children.Add(ratingLabel);
            
            wrapPanelRow4 .Children.Add(BackupLabel);
            wrapPanelRow4 .Children.Add(backupGrid);
            
            wrapPanelRow5 .Children.Add(inputPowerFactorPhase);
            wrapPanelRow5 .Children.Add(inputPowerFactorPhaseTextBox);
            wrapPanelRow5 .Children.Add(inputPowerFactorPhaseLabel);
            
            wrapPanelRow6 .Children.Add(THDI);
            wrapPanelRow6 .Children.Add(THDITextBox);
            wrapPanelRow6 .Children.Add(THDILabel);
            
            wrapPanelRow7 .Children.Add(inputNominalVoltage);
            wrapPanelRow7 .Children.Add(inputNominalVoltageTextBox);
            wrapPanelRow7 .Children.Add(inputNominalVoltageLabel);
            
            wrapPanelRow8 .Children.Add(inputVoltage);
            wrapPanelRow8 .Children.Add(inputVoltageTextBox);
            wrapPanelRow8 .Children.Add(inputVoltageLabel);
            
            wrapPanelRow9.Children.Add(voltageTolerance);
            wrapPanelRow9.Children.Add(voltageToleranceTextBox);
            wrapPanelRow9.Children.Add(voltageToleranceLabel);
            
            wrapPanelRow10.Children.Add(outputPowerFactor);
            wrapPanelRow10.Children.Add(outputPowerFactorTextBox);
            wrapPanelRow10.Children.Add(outputPowerFactorLabel);
            
            wrapPanelRow11.Children.Add(THDV);
            wrapPanelRow11.Children.Add(THDVTextBox);
            wrapPanelRow11.Children.Add(THDVLabel);
            
            wrapPanelRow12.Children.Add(outputNominalVoltage);
            wrapPanelRow12.Children.Add(outputNominalVoltageTextBox);
            wrapPanelRow12.Children.Add(outputNominalVoltageLabel);
            
            wrapPanelRow13.Children.Add(outputDCVoltageRange);
            wrapPanelRow13.Children.Add(outputDCVoltageRangeTextBox);
            wrapPanelRow13.Children.Add(outputDCVoltageRangeLabel);
            
            wrapPanelRow14.Children.Add(overloadCapability);
            wrapPanelRow14.Children.Add(overloadCapabilityTextBox);
            wrapPanelRow14.Children.Add(overloadCapabilityLabel);
            
            wrapPanelRow15.Children.Add(efficiency);
            wrapPanelRow15.Children.Add(efficiencyTextBox);
            wrapPanelRow15.Children.Add(efficiencyLabel);
            
            wrapPanelRow16.Children.Add(inputConnectionType);
            wrapPanelRow16.Children.Add(inputConnectionTypeTextBox);
            wrapPanelRow16.Children.Add(inputConnectionTypeLabel);
            
            wrapPanelRow17.Children.Add(frontPanel);
            wrapPanelRow17.Children.Add(frontPanelTextBox);
            wrapPanelRow17.Children.Add(frontPanelLabel);
            
            wrapPanelRow18.Children.Add(maxPower);
            wrapPanelRow18.Children.Add(maxPowerTextBox);
            wrapPanelRow18.Children.Add(maxPowerLabel);
            
            wrapPanelRow19.Children.Add(certificates);
            wrapPanelRow19.Children.Add(certificatesTextBox);
            wrapPanelRow19.Children.Add(certificatesLabel);
            
            wrapPanelRow20.Children.Add(safety);
            wrapPanelRow20.Children.Add(safetyTextBox);
            wrapPanelRow20.Children.Add(safetyLabel);
            
            wrapPanelRow21.Children.Add(EMC);
            wrapPanelRow21.Children.Add(EMCTextBox);
            wrapPanelRow21.Children.Add(EMCLabel);
            
            wrapPanelRow22.Children.Add(environmentalAspects);
            wrapPanelRow22.Children.Add(environmentalAspectsTextBox);
            wrapPanelRow22.Children.Add(environmentalAspectsLabel);
            
            wrapPanelRow23.Children.Add(testPerformance);
            wrapPanelRow23.Children.Add(testPerformanceTextBox);
            wrapPanelRow23.Children.Add(testPerformanceLabel);
            
            wrapPanelRow24.Children.Add(protectionDegree);
            wrapPanelRow24.Children.Add(protectionDegreeTextBox);
            wrapPanelRow24.Children.Add(protectionDegreeLabel);
            
            wrapPanelRow25.Children.Add(transferVoltageLimit);
            wrapPanelRow25.Children.Add(transferVoltageLimitTextBox);
            wrapPanelRow25.Children.Add(transferVoltageLimitLabel);

            wrapPanelRow26.Children.Add(marking);
            wrapPanelRow26.Children.Add(markingTextBox);
            wrapPanelRow26.Children.Add(markingLabel);

            wrapPanelRow27.Children.Add(validUntil);
            wrapPanelRow27.Children.Add(validUntilDatePicker);
            wrapPanelRow27.Children.Add(validUntilLabel);

            wrapPanel1.Children.Add(wrapPanelRow1 );
            
            wrapPanel2 .Children.Add(wrapPanelRow2 );
            wrapPanel2 .Children.Add(wrapPanelRow3 );
            
            wrapPanel3 .Children.Add(wrapPanelRow4 );
            wrapPanel3 .Children.Add(wrapPanelRow5 );
            
            wrapPanel4 .Children.Add(wrapPanelRow6 );
            wrapPanel4 .Children.Add(wrapPanelRow7 );
            
            wrapPanel5 .Children.Add(wrapPanelRow8 );
            wrapPanel5 .Children.Add(wrapPanelRow9 );
            
            wrapPanel6 .Children.Add(wrapPanelRow10);
            wrapPanel6 .Children.Add(wrapPanelRow11);
            
            wrapPanel7 .Children.Add(wrapPanelRow12);
            wrapPanel7 .Children.Add(wrapPanelRow13);
            
            wrapPanel8 .Children.Add(wrapPanelRow14);
            wrapPanel8 .Children.Add(wrapPanelRow15);
            
            wrapPanel9 .Children.Add(wrapPanelRow16);
            wrapPanel9 .Children.Add(wrapPanelRow17);
            
            wrapPanel10.Children.Add(wrapPanelRow18);
            wrapPanel10.Children.Add(wrapPanelRow19);
            
            wrapPanel11.Children.Add(wrapPanelRow20);
            wrapPanel11.Children.Add(wrapPanelRow21);
            
            wrapPanel12.Children.Add(wrapPanelRow22);
            wrapPanel12.Children.Add(wrapPanelRow23);
            
            wrapPanel13.Children.Add(wrapPanelRow24);
            wrapPanel13.Children.Add(wrapPanelRow25);
            
            wrapPanel14.Children.Add(wrapPanelRow26);
            wrapPanel14.Children.Add(wrapPanelRow27);

            grid.Children.Add(wrapPanel1 );
            grid.Children.Add(wrapPanel2 );
            grid.Children.Add(wrapPanel3 );
            grid.Children.Add(wrapPanel4 );
            grid.Children.Add(wrapPanel5 );
            grid.Children.Add(wrapPanel6 );
            grid.Children.Add(wrapPanel7 );
            grid.Children.Add(wrapPanel8 );
            grid.Children.Add(wrapPanel9 );
            grid.Children.Add(wrapPanel10);
            grid.Children.Add(wrapPanel11);
            grid.Children.Add(wrapPanel12);
            grid.Children.Add(wrapPanel13);
            grid.Children.Add(wrapPanel14);

            Content.Children.Add(grid);
            Card.Children.Add(Header);
            Card.Children.Add(Content);
            mainGrid.Children.Add(Card);

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                IOPhaseTextBox.Visibility = Visibility.Collapsed;
                IOPhaseLabel.Visibility = Visibility.Visible;

                ratedPowerTextBox.Visibility = Visibility.Collapsed;
                ratedPowerLabel.Visibility =Visibility.Visible;

                backupTime50TextBox.Visibility = Visibility.Collapsed;
                backupTime50Label.Visibility = Visibility.Visible;

                backupTime70TextBox.Visibility = Visibility.Collapsed;
                backupTime70Label.Visibility = Visibility.Visible;

                backupTime100TextBox.Visibility = Visibility.Collapsed;
                backupTime100Label.Visibility = Visibility.Visible;

                inputPowerFactorPhaseTextBox.Visibility = Visibility.Collapsed;
                inputPowerFactorPhaseLabel.Visibility = Visibility.Visible;

                THDITextBox.Visibility = Visibility.Collapsed;
                THDILabel.Visibility = Visibility.Visible;

                inputNominalVoltageTextBox.Visibility = Visibility.Collapsed;
                inputNominalVoltageLabel.Visibility = Visibility.Visible;





            }

            
        }

        private void ratingComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void OnBtnClickNext(object sender, RoutedEventArgs e)
        {

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
            {

                for (int i = 0; i < mainGrid.RowDefinitions.Count(); i++)
                {


                    if (i == 0)
                    {
                        BASIC_STRUCTS.UPS_SPECS_STRUCT tempUPSSpecs = new BASIC_STRUCTS.UPS_SPECS_STRUCT();

                        tempUPSSpecs.spec_id = 1;
                        if (iOPhaseTextBox.Text != "" || (product.GetUPSSpecs().Count > 0 && product.GetUPSSpecs()[0].io_phase != null))
                        {
                            tempUPSSpecs.io_phase = iOPhaseTextBox.Text.ToString();
                        }

                        if (ratedPowerTextBox.Text != "" || (product.GetUPSSpecs().Count > 0 && product.GetUPSSpecs()[0].rated_power != null))
                        {
                            tempUPSSpecs.rated_power = decimal.Parse(ratedPowerTextBox.Text.ToString());
                        }

                        if (ratingComboBox.SelectedIndex != -1 || (product.GetUPSSpecs().Count > 0 && product.GetUPSSpecs()[0].rating != null))
                        {
                            tempUPSSpecs.rating = ratingComboBox.SelectedItem.ToString();
                            tempUPSSpecs.rating_id = ratingComboBox.SelectedIndex + 1;
                        }


                        if (backupTime50TextBox.Text != "" || (product.GetUPSSpecs().Count > 0 && product.GetUPSSpecs()[0].backup_time_50 != null))
                        {
                            tempUPSSpecs.backup_time_50 = int.Parse(backupTime50TextBox.Text.ToString());
                        }


                        if (backupTime70TextBox.Text != "" || (product.GetUPSSpecs().Count > 0 && product.GetUPSSpecs()[0].backup_time_70 != null))
                        {
                            tempUPSSpecs.backup_time_70 = int.Parse(backupTime70TextBox.Text.ToString());
                        }


                        if (backupTime100TextBox.Text != "" || (product.GetUPSSpecs().Count > 0 && product.GetUPSSpecs()[0].backup_time_100 != null))
                        {
                            tempUPSSpecs.backup_time_100 = int.Parse(backupTime100TextBox.Text.ToString());
                        }


                        tempUPSSpecs.input_power_factor = inputPowerFactorTextBox.Text.ToString();
                        tempUPSSpecs.thdi = thdiTextBox.Text.ToString();
                        tempUPSSpecs.input_nominal_voltage = inputNominalVoltageTextBox.Text.ToString();
                        tempUPSSpecs.input_voltage = inputVoltageTextBox.Text.ToString();
                        tempUPSSpecs.voltage_tolerance = voltageToleranceTextBox.Text.ToString();
                        tempUPSSpecs.output_power_factor = outputPowerFactorTextBox.Text.ToString();
                        tempUPSSpecs.thdv = thdvTextBox.Text.ToString();
                        tempUPSSpecs.output_nominal_voltage = outputNominalVoltageTextBox.Text.ToString();
                        tempUPSSpecs.output_dc_voltage_range = outputDCVoltageRangeTextBox.Text.ToString();
                        tempUPSSpecs.overload_capability = overloadCapabilityTextBox.Text.ToString();
                        tempUPSSpecs.efficiency = efficiencyTextBox.Text.ToString();
                        tempUPSSpecs.input_connection_type = inputConnectionTypeTextBox.Text.ToString();
                        tempUPSSpecs.front_panel = frontPanelTextBox.Text.ToString();
                        tempUPSSpecs.max_power = maxPowerTextBox.Text.ToString();
                        tempUPSSpecs.certificates = certificatesTextBox.Text.ToString();
                        tempUPSSpecs.safety = safetyTextBox.Text.ToString();
                        tempUPSSpecs.emc = emcTextBox.Text.ToString();
                        tempUPSSpecs.environmental_aspects = environmentalAspectsTextBox.Text.ToString();
                        tempUPSSpecs.test_performance = testPerformanceTextBox.Text.ToString();
                        tempUPSSpecs.protection_degree = protectionDegreeTextBox.Text.ToString();
                        tempUPSSpecs.transfer_voltage_limit = transferVoltageLimitTextBox.Text.ToString();
                        tempUPSSpecs.marking = markingTextBox.Text.ToString();
                        tempUPSSpecs.is_valid = true;
                        if (validUntilDatePicker.SelectedDate != null || (product.GetUPSSpecs().Count > 0 && product.GetUPSSpecs()[0].valid_until != null))
                        {
                            tempUPSSpecs.valid_until = (DateTime)validUntilDatePicker.SelectedDate;
                        }
                        product.GetUPSSpecs().Clear();
                        product.SetUPSSpecs(tempUPSSpecs);
                    }
                    else
                    {
                        BASIC_STRUCTS.UPS_SPECS_STRUCT tempUPSSpecs = new BASIC_STRUCTS.UPS_SPECS_STRUCT();

                        tempUPSSpecs.spec_id = i + 1;


                        Grid cuurentGrid = new Grid();
                        cuurentGrid = (Grid)mainGrid.Children[i];

                        Grid specsGrid = new Grid();
                        specsGrid = (Grid)cuurentGrid.Children[1];

                        Grid Grid = new Grid();
                        Grid = (Grid)specsGrid.Children[0];


                        WrapPanel RowWrapPanel = new WrapPanel();
                        RowWrapPanel = (WrapPanel)Grid.Children[1];

                        WrapPanel IoPhaseWrapPanel = new WrapPanel();
                        IoPhaseWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox IoPhasetextBox = new TextBox();
                        IoPhasetextBox = (TextBox)IoPhaseWrapPanel.Children[1];

                        tempUPSSpecs.io_phase = IoPhasetextBox.Text;

                        WrapPanel ratedPowerWrapPanel = new WrapPanel();
                        ratedPowerWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox ratedPowerTextBox = new TextBox();
                        ratedPowerTextBox = (TextBox)ratedPowerWrapPanel.Children[1];

                        tempUPSSpecs.rated_power = decimal.Parse(ratedPowerTextBox.Text.ToString());

                        ComboBox ratingeComboBox = new ComboBox();
                        ratingeComboBox = (ComboBox)ratedPowerWrapPanel.Children[3];

                        tempUPSSpecs.rating = ratingeComboBox.SelectedItem.ToString();
                        tempUPSSpecs.rating_id = ratingeComboBox.SelectedIndex;



                        RowWrapPanel = (WrapPanel)Grid.Children[2];

                        WrapPanel BackupTimeWrapPanel = new WrapPanel();
                        BackupTimeWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        Grid BackupTimeGrid = new Grid();
                        BackupTimeGrid = (Grid)BackupTimeWrapPanel.Children[1];

                        WrapPanel BackupTime50wrapPanel = new WrapPanel();
                        BackupTime50wrapPanel = (WrapPanel)BackupTimeGrid.Children[0];

                        TextBox BackUp50TextBox = new TextBox();
                        BackUp50TextBox = (TextBox)BackupTime50wrapPanel.Children[1];
                        tempUPSSpecs.backup_time_50 = int.Parse(BackUp50TextBox.Text.ToString());

                        WrapPanel BackupTime70wrapPanel = new WrapPanel();
                        BackupTime70wrapPanel = (WrapPanel)BackupTimeGrid.Children[1];

                        TextBox BackUp70TextBox = new TextBox();
                        BackUp70TextBox = (TextBox)BackupTime70wrapPanel.Children[1];
                        tempUPSSpecs.backup_time_70 = int.Parse(BackUp70TextBox.Text.ToString());

                        WrapPanel BackupTime100wrapPanel = new WrapPanel();
                        BackupTime100wrapPanel = (WrapPanel)BackupTimeGrid.Children[2];

                        TextBox BackUp100TextBox = new TextBox();
                        BackUp100TextBox = (TextBox)BackupTime100wrapPanel.Children[1];
                        tempUPSSpecs.backup_time_100 = int.Parse(BackUp100TextBox.Text.ToString());

                        WrapPanel inputPowerFactorWrapPanel = new WrapPanel();
                        inputPowerFactorWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox inputPowerFactorTextBox = new TextBox();
                        inputPowerFactorTextBox = (TextBox)inputPowerFactorWrapPanel.Children[1];

                        tempUPSSpecs.input_power_factor = inputPowerFactorTextBox.Text;


                        RowWrapPanel = (WrapPanel)Grid.Children[3];

                        WrapPanel thdiWrapPanel = new WrapPanel();
                        thdiWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox thdiTextBox = new TextBox();
                        thdiTextBox = (TextBox)thdiWrapPanel.Children[1];

                        tempUPSSpecs.thdi = thdiTextBox.Text;

                        WrapPanel inputNominalVoltageWrapPanel = new WrapPanel();
                        inputNominalVoltageWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox inputNominalVoltageTextBox = new TextBox();
                        inputNominalVoltageTextBox = (TextBox)inputNominalVoltageWrapPanel.Children[1];

                        tempUPSSpecs.input_nominal_voltage = inputNominalVoltageTextBox.Text;


                        RowWrapPanel = (WrapPanel)Grid.Children[4];

                        WrapPanel inputVoltageWrapPanel = new WrapPanel();
                        inputVoltageWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox inputVoltageTextBox = new TextBox();
                        inputVoltageTextBox = (TextBox)inputVoltageWrapPanel.Children[1];

                        tempUPSSpecs.input_voltage = inputVoltageTextBox.Text;

                        WrapPanel voltageToleranceWrapPanel = new WrapPanel();
                        voltageToleranceWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox voltageToleranceTextBox = new TextBox();
                        voltageToleranceTextBox = (TextBox)voltageToleranceWrapPanel.Children[1];

                        tempUPSSpecs.voltage_tolerance = voltageToleranceTextBox.Text;

                        RowWrapPanel = (WrapPanel)Grid.Children[5];

                        WrapPanel outputPowerFactorWrapPanel = new WrapPanel();
                        outputPowerFactorWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox outputPowerFactorTextBox = new TextBox();
                        outputPowerFactorTextBox = (TextBox)outputPowerFactorWrapPanel.Children[1];

                        tempUPSSpecs.output_power_factor = outputPowerFactorTextBox.Text;

                        WrapPanel thdvWrapPanel = new WrapPanel();
                        thdvWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox thdvTextBox = new TextBox();
                        thdvTextBox = (TextBox)thdvWrapPanel.Children[1];

                        tempUPSSpecs.voltage_tolerance = thdvTextBox.Text;

                        RowWrapPanel = (WrapPanel)Grid.Children[6];

                        WrapPanel outputNominalVoltageWrapPanel = new WrapPanel();
                        outputNominalVoltageWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox outputNominalVoltageTextBox = new TextBox();
                        outputNominalVoltageTextBox = (TextBox)outputNominalVoltageWrapPanel.Children[1];

                        tempUPSSpecs.output_nominal_voltage = outputNominalVoltageTextBox.Text;

                        WrapPanel outputDCVoltageRangeWrapPanel = new WrapPanel();
                        outputDCVoltageRangeWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox outputDCVoltageRangeTextBox = new TextBox();
                        outputDCVoltageRangeTextBox = (TextBox)outputDCVoltageRangeWrapPanel.Children[1];

                        tempUPSSpecs.output_dc_voltage_range = outputDCVoltageRangeTextBox.Text;

                        RowWrapPanel = (WrapPanel)Grid.Children[7];

                        WrapPanel overloadCapabilityWrapPanel = new WrapPanel();
                        overloadCapabilityWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox overloadCapabilityTextBox = new TextBox();
                        overloadCapabilityTextBox = (TextBox)overloadCapabilityWrapPanel.Children[1];

                        tempUPSSpecs.overload_capability = overloadCapabilityTextBox.Text;

                        WrapPanel efficiencyWrapPanel = new WrapPanel();
                        efficiencyWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox efficiencyTextBox = new TextBox();
                        efficiencyTextBox = (TextBox)efficiencyWrapPanel.Children[1];

                        tempUPSSpecs.efficiency = efficiencyTextBox.Text;

                        RowWrapPanel = (WrapPanel)Grid.Children[8];

                        WrapPanel inputConnectionTypeWrapPanel = new WrapPanel();
                        inputConnectionTypeWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox inputConnectionTypeTextBox = new TextBox();
                        inputConnectionTypeTextBox = (TextBox)inputConnectionTypeWrapPanel.Children[1];

                        tempUPSSpecs.input_connection_type = inputConnectionTypeTextBox.Text;

                        WrapPanel frontPanelWrapPanel = new WrapPanel();
                        frontPanelWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox frontPanelTextBox = new TextBox();
                        frontPanelTextBox = (TextBox)frontPanelWrapPanel.Children[1];

                        tempUPSSpecs.front_panel = frontPanelTextBox.Text;

                        RowWrapPanel = (WrapPanel)Grid.Children[9];

                        WrapPanel maxPowerWrapPanel = new WrapPanel();
                        maxPowerWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox maxPowerTextBox = new TextBox();
                        maxPowerTextBox = (TextBox)maxPowerWrapPanel.Children[1];

                        tempUPSSpecs.max_power = maxPowerTextBox.Text;

                        WrapPanel certificatesWrapPanel = new WrapPanel();
                        certificatesWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox certificatesTextBox = new TextBox();
                        certificatesTextBox = (TextBox)certificatesWrapPanel.Children[1];

                        tempUPSSpecs.certificates = certificatesTextBox.Text;

                        RowWrapPanel = (WrapPanel)Grid.Children[10];

                        WrapPanel safetyWrapPanel = new WrapPanel();
                        safetyWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox safetyTextBox = new TextBox();
                        safetyTextBox = (TextBox)safetyWrapPanel.Children[1];

                        tempUPSSpecs.safety = safetyTextBox.Text;

                        WrapPanel emcWrapPanel = new WrapPanel();
                        emcWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox emcTextBox = new TextBox();
                        emcTextBox = (TextBox)emcWrapPanel.Children[1];

                        tempUPSSpecs.emc = emcTextBox.Text;

                        RowWrapPanel = (WrapPanel)Grid.Children[11];

                        WrapPanel environmentalAspectsWrapPanel = new WrapPanel();
                        environmentalAspectsWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox environmentalAspectsTextBox = new TextBox();
                        environmentalAspectsTextBox = (TextBox)environmentalAspectsWrapPanel.Children[1];

                        tempUPSSpecs.environmental_aspects = environmentalAspectsTextBox.Text;

                        WrapPanel testPerformanceWrapPanel = new WrapPanel();
                        testPerformanceWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox testPerformanceTextBox = new TextBox();
                        testPerformanceTextBox = (TextBox)testPerformanceWrapPanel.Children[1];

                        tempUPSSpecs.test_performance = testPerformanceTextBox.Text;

                        RowWrapPanel = (WrapPanel)Grid.Children[12];

                        WrapPanel protectionDegreeWrapPanel = new WrapPanel();
                        protectionDegreeWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox protectionDegreeTextBox = new TextBox();
                        protectionDegreeTextBox = (TextBox)protectionDegreeWrapPanel.Children[1];

                        tempUPSSpecs.protection_degree = protectionDegreeTextBox.Text;

                        WrapPanel transferVoltageLimitWrapPanel = new WrapPanel();
                        transferVoltageLimitWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox transferVoltageLimitTextBox = new TextBox();
                        transferVoltageLimitTextBox = (TextBox)transferVoltageLimitWrapPanel.Children[1];

                        tempUPSSpecs.transfer_voltage_limit = transferVoltageLimitTextBox.Text;

                        RowWrapPanel = (WrapPanel)Grid.Children[13];

                        WrapPanel markingWrapPanel = new WrapPanel();
                        markingWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox markingTextBox = new TextBox();
                        markingTextBox = (TextBox)markingWrapPanel.Children[1];

                        tempUPSSpecs.marking = markingTextBox.Text;

                        WrapPanel validUntilWrapPanel = new WrapPanel();
                        validUntilWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        DatePicker validUntilDatePicker = new DatePicker();
                        validUntilDatePicker = (DatePicker)validUntilWrapPanel.Children[1];

                        tempUPSSpecs.valid_until = (DateTime)validUntilDatePicker.SelectedDate;

                        product.SetUPSSpecs(tempUPSSpecs);
                    }
                }


            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {

            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_UPDATE_CONDITION)
            {

            }
            modelAdditionalInfoPage.modelBasicInfoPage = modelBasicInfoPage;
            modelAdditionalInfoPage.modelUpsSpecsPage = this;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                modelAdditionalInfoPage.modelUploadFilesPage = modelUploadFilesPage;

            NavigationService.Navigate(modelAdditionalInfoPage);
        }
        private void OnBtnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            modelBasicInfoPage.modelUpsSpecsPage = this;
            modelBasicInfoPage.modelAdditionalInfoPage = modelAdditionalInfoPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                modelBasicInfoPage.modelUploadFilesPage = modelUploadFilesPage;

            NavigationService.Navigate(modelBasicInfoPage);
        }
        private void OnBtnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
         
            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
            {

                for (int i = 0; i < mainGrid.RowDefinitions.Count(); i++)
                {


                    if (i == 0)
                    {
                        BASIC_STRUCTS.UPS_SPECS_STRUCT tempUPSSpecs = new BASIC_STRUCTS.UPS_SPECS_STRUCT();

                        tempUPSSpecs.spec_id = 1;
                        if (iOPhaseTextBox.Text != "" || (product.GetUPSSpecs().Count > 0 && product.GetUPSSpecs()[0].io_phase != null))
                        {
                            tempUPSSpecs.io_phase = iOPhaseTextBox.Text.ToString();
                        }

                        if (ratedPowerTextBox.Text != "" || (product.GetUPSSpecs().Count > 0 && product.GetUPSSpecs()[0].rated_power != null))
                        {
                            tempUPSSpecs.rated_power = decimal.Parse(ratedPowerTextBox.Text.ToString());
                        }

                        if (ratingComboBox.SelectedIndex != -1 || (product.GetUPSSpecs().Count > 0 && product.GetUPSSpecs()[0].rating != null))
                        {
                            tempUPSSpecs.rating = ratingComboBox.SelectedItem.ToString();
                            tempUPSSpecs.rating_id = ratingComboBox.SelectedIndex + 1;
                        }


                        if (backupTime50TextBox.Text != "" || (product.GetUPSSpecs().Count > 0 && product.GetUPSSpecs()[0].backup_time_50 != null))
                        {
                            tempUPSSpecs.backup_time_50 = int.Parse(backupTime50TextBox.Text.ToString());
                        }


                        if (backupTime70TextBox.Text != "" || (product.GetUPSSpecs().Count > 0 && product.GetUPSSpecs()[0].backup_time_70 != null))
                        {
                            tempUPSSpecs.backup_time_70 = int.Parse(backupTime70TextBox.Text.ToString());
                        }


                        if (backupTime100TextBox.Text != "" || (product.GetUPSSpecs().Count > 0 && product.GetUPSSpecs()[0].backup_time_100 != null))
                        {
                            tempUPSSpecs.backup_time_100 = int.Parse(backupTime100TextBox.Text.ToString());
                        }


                        tempUPSSpecs.input_power_factor = inputPowerFactorTextBox.Text.ToString();
                        tempUPSSpecs.thdi = thdiTextBox.Text.ToString();
                        tempUPSSpecs.input_nominal_voltage = inputNominalVoltageTextBox.Text.ToString();
                        tempUPSSpecs.input_voltage = inputVoltageTextBox.Text.ToString();
                        tempUPSSpecs.voltage_tolerance = voltageToleranceTextBox.Text.ToString();
                        tempUPSSpecs.output_power_factor = outputPowerFactorTextBox.Text.ToString();
                        tempUPSSpecs.thdv = thdvTextBox.Text.ToString();
                        tempUPSSpecs.output_nominal_voltage = outputNominalVoltageTextBox.Text.ToString();
                        tempUPSSpecs.output_dc_voltage_range = outputDCVoltageRangeTextBox.Text.ToString();
                        tempUPSSpecs.overload_capability = overloadCapabilityTextBox.Text.ToString();
                        tempUPSSpecs.efficiency = efficiencyTextBox.Text.ToString();
                        tempUPSSpecs.input_connection_type = inputConnectionTypeTextBox.Text.ToString();
                        tempUPSSpecs.front_panel = frontPanelTextBox.Text.ToString();
                        tempUPSSpecs.max_power = maxPowerTextBox.Text.ToString();
                        tempUPSSpecs.certificates = certificatesTextBox.Text.ToString();
                        tempUPSSpecs.safety = safetyTextBox.Text.ToString();
                        tempUPSSpecs.emc = emcTextBox.Text.ToString();
                        tempUPSSpecs.environmental_aspects = environmentalAspectsTextBox.Text.ToString();
                        tempUPSSpecs.test_performance = testPerformanceTextBox.Text.ToString();
                        tempUPSSpecs.protection_degree = protectionDegreeTextBox.Text.ToString();
                        tempUPSSpecs.transfer_voltage_limit = transferVoltageLimitTextBox.Text.ToString();
                        tempUPSSpecs.marking = markingTextBox.Text.ToString();
                        tempUPSSpecs.is_valid = true;
                        if (validUntilDatePicker.SelectedDate != null || (product.GetUPSSpecs().Count > 0 && product.GetUPSSpecs()[0].valid_until != null))
                        {
                            tempUPSSpecs.valid_until = (DateTime)validUntilDatePicker.SelectedDate;
                        }
                        product.GetUPSSpecs().Clear();
                        product.SetUPSSpecs(tempUPSSpecs);
                    }
                    else
                    {
                        BASIC_STRUCTS.UPS_SPECS_STRUCT tempUPSSpecs = new BASIC_STRUCTS.UPS_SPECS_STRUCT();

                        tempUPSSpecs.spec_id = i+1;


                        Grid cuurentGrid = new Grid();
                        cuurentGrid = (Grid)mainGrid.Children[i];

                        Grid specsGrid = new Grid();
                        specsGrid = (Grid)cuurentGrid.Children[1];

                        Grid Grid  = new Grid();
                        Grid = (Grid)specsGrid.Children[0];
                        

                        WrapPanel RowWrapPanel = new WrapPanel();
                        RowWrapPanel = (WrapPanel)Grid.Children[1];

                        WrapPanel IoPhaseWrapPanel = new WrapPanel();
                        IoPhaseWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox IoPhasetextBox = new TextBox();
                        IoPhasetextBox = (TextBox)IoPhaseWrapPanel.Children[1];

                        tempUPSSpecs.io_phase = IoPhasetextBox.Text;

                        WrapPanel ratedPowerWrapPanel = new WrapPanel();
                        ratedPowerWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox ratedPowerTextBox = new TextBox();
                        ratedPowerTextBox = (TextBox)ratedPowerWrapPanel.Children[1];

                        tempUPSSpecs.rated_power =decimal.Parse(ratedPowerTextBox.Text.ToString());

                        ComboBox ratingeComboBox = new ComboBox();
                        ratingeComboBox = (ComboBox)ratedPowerWrapPanel.Children[3];

                        tempUPSSpecs.rating = ratingeComboBox.SelectedItem.ToString();
                        tempUPSSpecs.rating_id = ratingeComboBox.SelectedIndex;

                       
                        
                        RowWrapPanel = (WrapPanel)Grid.Children[2];
                        
                        WrapPanel BackupTimeWrapPanel = new WrapPanel();
                        BackupTimeWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        Grid BackupTimeGrid = new Grid();
                        BackupTimeGrid = (Grid)BackupTimeWrapPanel.Children[1];

                        WrapPanel BackupTime50wrapPanel = new WrapPanel();
                        BackupTime50wrapPanel = (WrapPanel)BackupTimeGrid.Children[0];

                        TextBox BackUp50TextBox = new TextBox();
                        BackUp50TextBox = (TextBox)BackupTime50wrapPanel.Children[1];
                        tempUPSSpecs.backup_time_50 = int.Parse(BackUp50TextBox.Text.ToString());

                        WrapPanel BackupTime70wrapPanel = new WrapPanel();
                        BackupTime70wrapPanel = (WrapPanel)BackupTimeGrid.Children[1];

                        TextBox BackUp70TextBox = new TextBox();
                        BackUp70TextBox = (TextBox)BackupTime70wrapPanel.Children[1];
                        tempUPSSpecs.backup_time_70 = int.Parse(BackUp70TextBox.Text.ToString());

                        WrapPanel BackupTime100wrapPanel = new WrapPanel();
                        BackupTime100wrapPanel = (WrapPanel)BackupTimeGrid.Children[2];

                        TextBox BackUp100TextBox = new TextBox();
                        BackUp100TextBox = (TextBox)BackupTime100wrapPanel.Children[1];
                        tempUPSSpecs.backup_time_100 = int.Parse(BackUp100TextBox.Text.ToString());

                        WrapPanel inputPowerFactorWrapPanel = new WrapPanel();
                        inputPowerFactorWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox inputPowerFactorTextBox = new TextBox();
                        inputPowerFactorTextBox = (TextBox)inputPowerFactorWrapPanel.Children[1];

                        tempUPSSpecs.input_power_factor = inputPowerFactorTextBox.Text;


                        RowWrapPanel = (WrapPanel)Grid.Children[3];

                        WrapPanel thdiWrapPanel = new WrapPanel();
                        thdiWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox thdiTextBox = new TextBox();
                        thdiTextBox = (TextBox)thdiWrapPanel.Children[1];

                        tempUPSSpecs.thdi = thdiTextBox.Text;

                        WrapPanel inputNominalVoltageWrapPanel = new WrapPanel();
                        inputNominalVoltageWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox inputNominalVoltageTextBox = new TextBox();
                        inputNominalVoltageTextBox = (TextBox)inputNominalVoltageWrapPanel.Children[1];

                        tempUPSSpecs.input_nominal_voltage = inputNominalVoltageTextBox.Text;


                        RowWrapPanel = (WrapPanel)Grid.Children[4];

                        WrapPanel inputVoltageWrapPanel = new WrapPanel();
                        inputVoltageWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox inputVoltageTextBox = new TextBox();
                        inputVoltageTextBox = (TextBox)inputVoltageWrapPanel.Children[1];

                        tempUPSSpecs.input_voltage = inputVoltageTextBox.Text;

                        WrapPanel voltageToleranceWrapPanel = new WrapPanel();
                        voltageToleranceWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox voltageToleranceTextBox = new TextBox();
                        voltageToleranceTextBox = (TextBox)voltageToleranceWrapPanel.Children[1];

                        tempUPSSpecs.voltage_tolerance = voltageToleranceTextBox.Text;

                        RowWrapPanel = (WrapPanel)Grid.Children[5];

                        WrapPanel outputPowerFactorWrapPanel = new WrapPanel();
                        outputPowerFactorWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox outputPowerFactorTextBox = new TextBox();
                        outputPowerFactorTextBox = (TextBox)outputPowerFactorWrapPanel.Children[1];

                        tempUPSSpecs.output_power_factor = outputPowerFactorTextBox.Text;

                        WrapPanel thdvWrapPanel = new WrapPanel();
                        thdvWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox thdvTextBox = new TextBox();
                        thdvTextBox = (TextBox)thdvWrapPanel.Children[1];

                        tempUPSSpecs.voltage_tolerance = thdvTextBox.Text;

                        RowWrapPanel = (WrapPanel)Grid.Children[6];

                        WrapPanel outputNominalVoltageWrapPanel = new WrapPanel();
                        outputNominalVoltageWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox outputNominalVoltageTextBox = new TextBox();
                        outputNominalVoltageTextBox = (TextBox)outputNominalVoltageWrapPanel.Children[1];

                        tempUPSSpecs.output_nominal_voltage = outputNominalVoltageTextBox.Text;

                        WrapPanel outputDCVoltageRangeWrapPanel = new WrapPanel();
                        outputDCVoltageRangeWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox outputDCVoltageRangeTextBox = new TextBox();
                        outputDCVoltageRangeTextBox = (TextBox)outputDCVoltageRangeWrapPanel.Children[1];

                        tempUPSSpecs.output_dc_voltage_range = outputDCVoltageRangeTextBox.Text;

                        RowWrapPanel = (WrapPanel)Grid.Children[7];

                        WrapPanel overloadCapabilityWrapPanel = new WrapPanel();
                        overloadCapabilityWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox overloadCapabilityTextBox = new TextBox();
                        overloadCapabilityTextBox = (TextBox)overloadCapabilityWrapPanel.Children[1];

                        tempUPSSpecs.overload_capability = overloadCapabilityTextBox.Text;

                        WrapPanel efficiencyWrapPanel = new WrapPanel();
                        efficiencyWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox efficiencyTextBox = new TextBox();
                        efficiencyTextBox = (TextBox)efficiencyWrapPanel.Children[1];

                        tempUPSSpecs.efficiency = efficiencyTextBox.Text;

                        RowWrapPanel = (WrapPanel)Grid.Children[8];

                        WrapPanel inputConnectionTypeWrapPanel = new WrapPanel();
                        inputConnectionTypeWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox inputConnectionTypeTextBox = new TextBox();
                        inputConnectionTypeTextBox = (TextBox)inputConnectionTypeWrapPanel.Children[1];

                        tempUPSSpecs.input_connection_type = inputConnectionTypeTextBox.Text;

                        WrapPanel frontPanelWrapPanel = new WrapPanel();
                        frontPanelWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox frontPanelTextBox = new TextBox();
                        frontPanelTextBox = (TextBox)frontPanelWrapPanel.Children[1];

                        tempUPSSpecs.front_panel = frontPanelTextBox.Text;

                        RowWrapPanel = (WrapPanel)Grid.Children[9];

                        WrapPanel maxPowerWrapPanel = new WrapPanel();
                        maxPowerWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox maxPowerTextBox = new TextBox();
                        maxPowerTextBox = (TextBox)maxPowerWrapPanel.Children[1];

                        tempUPSSpecs.max_power = maxPowerTextBox.Text;

                        WrapPanel certificatesWrapPanel = new WrapPanel();
                        certificatesWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox certificatesTextBox = new TextBox();
                        certificatesTextBox = (TextBox)certificatesWrapPanel.Children[1];

                        tempUPSSpecs.certificates = certificatesTextBox.Text;

                        RowWrapPanel = (WrapPanel)Grid.Children[10];

                        WrapPanel safetyWrapPanel = new WrapPanel();
                        safetyWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox safetyTextBox = new TextBox();
                        safetyTextBox = (TextBox)safetyWrapPanel.Children[1];

                        tempUPSSpecs.safety = safetyTextBox.Text;

                        WrapPanel emcWrapPanel = new WrapPanel();
                        emcWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox emcTextBox = new TextBox();
                        emcTextBox = (TextBox)emcWrapPanel.Children[1];

                        tempUPSSpecs.emc = emcTextBox.Text;

                        RowWrapPanel = (WrapPanel)Grid.Children[11];

                        WrapPanel environmentalAspectsWrapPanel = new WrapPanel();
                        environmentalAspectsWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox environmentalAspectsTextBox = new TextBox();
                        environmentalAspectsTextBox = (TextBox)environmentalAspectsWrapPanel.Children[1];

                        tempUPSSpecs.environmental_aspects = environmentalAspectsTextBox.Text;

                        WrapPanel testPerformanceWrapPanel = new WrapPanel();
                        testPerformanceWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox testPerformanceTextBox = new TextBox();
                        testPerformanceTextBox = (TextBox)testPerformanceWrapPanel.Children[1];

                        tempUPSSpecs.test_performance = testPerformanceTextBox.Text;

                        RowWrapPanel = (WrapPanel)Grid.Children[12];

                        WrapPanel protectionDegreeWrapPanel = new WrapPanel();
                        protectionDegreeWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox protectionDegreeTextBox = new TextBox();
                        protectionDegreeTextBox = (TextBox)protectionDegreeWrapPanel.Children[1];

                        tempUPSSpecs.protection_degree = protectionDegreeTextBox.Text;

                        WrapPanel transferVoltageLimitWrapPanel = new WrapPanel();
                        transferVoltageLimitWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        TextBox transferVoltageLimitTextBox = new TextBox();
                        transferVoltageLimitTextBox = (TextBox)transferVoltageLimitWrapPanel.Children[1];

                        tempUPSSpecs.transfer_voltage_limit = transferVoltageLimitTextBox.Text;

                        RowWrapPanel = (WrapPanel)Grid.Children[13];

                        WrapPanel markingWrapPanel = new WrapPanel();
                        markingWrapPanel = (WrapPanel)RowWrapPanel.Children[0];

                        TextBox markingTextBox = new TextBox();
                        markingTextBox = (TextBox)markingWrapPanel.Children[1];

                        tempUPSSpecs.marking = markingTextBox.Text;

                        WrapPanel validUntilWrapPanel = new WrapPanel();
                        validUntilWrapPanel = (WrapPanel)RowWrapPanel.Children[1];

                        DatePicker validUntilDatePicker = new DatePicker();
                        validUntilDatePicker = (DatePicker)validUntilWrapPanel.Children[1];

                        tempUPSSpecs.valid_until = (DateTime)validUntilDatePicker.SelectedDate;

                        product.SetUPSSpecs(tempUPSSpecs);
                    }
                }


            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {

            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_UPDATE_CONDITION)
            {

            }
            modelAdditionalInfoPage.modelBasicInfoPage = modelBasicInfoPage;
            modelAdditionalInfoPage.modelUpsSpecsPage = this;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                modelAdditionalInfoPage.modelUploadFilesPage = modelUploadFilesPage;

            NavigationService.Navigate(modelAdditionalInfoPage);
        }
        private void OnBtnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                modelUploadFilesPage.modelBasicInfoPage = modelBasicInfoPage;
                modelUploadFilesPage.modelUpsSpecsPage = this;
                modelUploadFilesPage.modelAdditionalInfoPage = modelAdditionalInfoPage;

                NavigationService.Navigate(modelUploadFilesPage);
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

        private void OnSelChangedvalidUntilDate(object sender, SelectionChangedEventArgs e)
        {

        }
        //////////////////////////////////////////////////////////////////////
        ///INITILIZATIONS
        /////////////////////////////////////////////////////////////////////
        void initializeRatingCombobox()
        {
            ratingComboBox.Items.Clear();

            for (int i = 0; i < rating.Count; i++)
            {
                ratingComboBox.Items.Add(rating[i].measure_unit);
            }
            ratingComboBox.SelectedIndex = 0;

        }

        private void addBtnMouseEnter(object sender, MouseEventArgs e)
        {



            Storyboard storyboard = new Storyboard();
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 200);
            DoubleAnimation animation = new DoubleAnimation();

            animation.From = addBtn.Opacity;
            animation.To = 1.0;
            animation.Duration = new Duration(duration);

            Storyboard.SetTargetName(animation, addBtn.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));

            storyboard.Children.Add(animation);

            storyboard.Begin(this);
        }

        private void addBtnMouseLeave(object sender, MouseEventArgs e)
        {


            Storyboard storyboard = new Storyboard();
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 200);
            DoubleAnimation animation = new DoubleAnimation();

            animation.From = addBtn.Opacity;
            animation.To = 0.5;
            animation.Duration = new Duration(duration);

            Storyboard.SetTargetName(animation, addBtn.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));

            storyboard.Children.Add(animation);

            storyboard.Begin(this);

        }

        private void onBtnAddClick(object sender, MouseButtonEventArgs e)
        {
            if (product.GetCategoryID() == COMPANY_WORK_MACROS.GENSET_CATEGORY_ID)
            {
                InitializeNewCardGenset();

            }
            else
            InitializeNewCard();
        }

      
        private void Delete_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image currentCard = new Image();
            currentCard =(Image)sender;
            Grid Card = new Grid();
            Grid Header = new Grid();
            Header = (Grid)currentCard.Parent;
            Card = (Grid)Header.Parent;

            int tag = int.Parse(Card.Tag.ToString());
            mainGrid.RowDefinitions.RemoveAt(tag);
            mainGrid.Children.RemoveAt(tag);

            for(int i =tag; i< mainGrid.Children.Count;i++)
            {
                Grid temp = new Grid();
                Grid mCard = new Grid();
                Grid mHeader = new Grid();
                mCard = (Grid)mainGrid.Children[i];
                mHeader = (Grid)mCard.Children[0];
                Label spec = new Label();
                spec = (Label)mHeader.Children[0];
                spec.Content = "SPEC " + (i);

                temp =(Grid)mainGrid.Children[i];
                temp.Tag = i;
            }

        }
        void AraangeTags()
        {
           
        }
    }
}
