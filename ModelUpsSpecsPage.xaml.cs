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
{/// <summary>
    /// Interaction logic for ModelUpsSpecsPage.xaml
    /// </summary>
    
    public partial class ModelUpsSpecsPage : Page
    {
        IntegrityChecks IntegrityChecks;
        Employee loggedInUser;
        Model product;
        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private int viewAddCondition;

        public ModelAdditionalInfoPage modelAdditionalInfoPage;
        public ModelBasicInfoPage modelBasicInfoPage;
        public ModelUploadFilesPage modelUploadFilesPage;
        protected List<BASIC_STRUCTS.UPS_SPECS_STRUCT> UPSSpecs;
        List<PROCUREMENT_STRUCTS.MEASURE_UNITS_STRUCT> rating;
        protected int index = 0;
        int cardCountGenset = 0;


        public ModelUpsSpecsPage(ref Employee mLoggedInUser, ref Model mPrduct, int mViewAddCondition)
        {
            InitializeComponent();

            IntegrityChecks = new IntegrityChecks();
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
                if (product.GetCategoryID() == COMPANY_WORK_MACROS.GENSET_CATEGORY_ID)
                {
                    SpecsLable.Content = "Genset Specs";
                    InitializeNewCardGenset();
                }


            }

            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                if (COMPANY_WORK_MACROS.GENSET_CATEGORY_ID == product.GetCategoryID()) {

                    for(int i=0;i<product.GetGensetSpecs().Count;i++)
                    InitializeNewCardGenset(); 
                }
                else
             InitializeUIElements();
            }


            if (product.GetCategoryID() == COMPANY_WORK_MACROS.GENSET_CATEGORY_ID)
            {
                SpecsLable.Content = "Genset Specs";
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
            mainGrid.Children.Clear();
            mainGrid.RowDefinitions.Clear();
            mainGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < product.GetUPSSpecs().Count; i++)
            {
                index = i;
                InitializeNewCard();
                
            }
        }

        private void InitializeNewCardGenset() {

            if (cardCountGenset == 0) {

                mainGrid.Children.Clear();
                mainGrid.RowDefinitions.Clear();
            }

            cardCountGenset++;

            mainGrid.RowDefinitions.Add(new RowDefinition());

            Grid Card = new Grid() { VerticalAlignment = VerticalAlignment.Top };

            Card.Background = new SolidColorBrush(Colors.White);
            Card.Margin = new Thickness(20);

            Grid.SetRow(Card, cardCountGenset-1);

            Grid Header = new Grid() { Height = 50 };
            Header.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF105A97");

            Label header = new Label() { Content = "SPEC " + (mainGrid.Children.Count + 1), Style = (Style)FindResource("tableHeaderItem") };
            header.HorizontalAlignment = HorizontalAlignment.Left;

            Card.Tag = cardCountGenset;

            if (cardCountGenset != 1)
            {
                Image delete = new Image() { Margin= new Thickness(10, 0, 10, 0) };
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri("Icons\\cross_icon.jpg", UriKind.Relative);
                bi3.EndInit();
                delete.Source = bi3;
                delete.Width = 20;
                delete.Height = 20;
                delete.HorizontalAlignment = HorizontalAlignment.Right;
                delete.Tag = cardCountGenset;
                delete.MouseLeftButtonDown += MouseLeftButtonDownGenset;

                Header.Children.Add(delete);

            }

            Header.Children.Add(header);

            Card.RowDefinitions.Add(new RowDefinition());
            Card.RowDefinitions.Add(new RowDefinition());
            Card.RowDefinitions.Add(new RowDefinition());
            Card.RowDefinitions.Add(new RowDefinition());
            Card.RowDefinitions.Add(new RowDefinition());
            Card.RowDefinitions.Add(new RowDefinition());
            Card.RowDefinitions.Add(new RowDefinition());



            WrapPanel wrapPanel1 = new WrapPanel();
            WrapPanel wrapPanel2 = new WrapPanel();
            WrapPanel wrapPanel3 = new WrapPanel();
            WrapPanel wrapPanel4 = new WrapPanel();
            WrapPanel wrapPanel5 = new WrapPanel();
            WrapPanel wrapPanel6 = new WrapPanel();

            Grid.SetRow(wrapPanel1, 1);
            Grid.SetRow(wrapPanel2, 2);
            Grid.SetRow(wrapPanel3, 3);
            Grid.SetRow(wrapPanel4, 4);
            Grid.SetRow(wrapPanel5, 5);
            Grid.SetRow(wrapPanel6, 6);




            Grid.SetRow(Header, 0);

            Label ratedPowerLable = new Label() { Style = (Style)FindResource("labelStyle")};
            ratedPowerLable.Content = "Rated Power";
            Label ratedPowerlabelInvisible = new Label() { Style = (Style)FindResource("labelStyle")};
            ratedPowerlabelInvisible.Visibility = Visibility.Collapsed;

            TextBox RatedPowerText = new TextBox() { Style = (Style)FindResource("textBoxStyle"), Width = 235, TextWrapping=TextWrapping.Wrap };
            RatedPowerText.TextChanged += RatedPower_TextChanged;

            ComboBox ratedPowercombo = new ComboBox() { Style = (Style)FindResource("comboBoxStyle"), Width = 90};
            rating.ForEach(a => ratedPowercombo.Items.Add(a.measure_unit));
            ratedPowercombo.SelectedItem = "KVA";

            WrapPanel ratedPowerPanel = new WrapPanel();

            ratedPowerPanel.Children.Add(ratedPowerLable);
            ratedPowerPanel.Children.Add(RatedPowerText);
            ratedPowerPanel.Children.Add(ratedPowerlabelInvisible);
            ratedPowerPanel.Children.Add(ratedPowercombo);


            WrapPanel modelPanel = new WrapPanel();

            Label ModelLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            ModelLabel.Content = "SpecName";

            TextBox ModelText = new TextBox() { Style = (Style)FindResource("textBoxStyle"),TextWrapping=TextWrapping.Wrap, Width = 235 };

             
            Label ModellabelInvisible = new Label() { Style = (Style)FindResource("labelStyle") };
            ModellabelInvisible.Visibility = Visibility.Collapsed;

            modelPanel.Children.Add(ModelLabel);
            modelPanel.Children.Add(ModelText);
            modelPanel.Children.Add(ModellabelInvisible);


            wrapPanel1.Children.Add(ratedPowerPanel);
            wrapPanel1.Children.Add(modelPanel);


            WrapPanel LtbKva50Panel = new WrapPanel();

            Label ltbKva50Label = new Label() { Style = (Style)FindResource("labelStyle") };
            ltbKva50Label.Content = "LTB 50HZ";

            TextBox kva50TextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle"), TextWrapping = TextWrapping.Wrap, Width = 235 };
            kva50TextBox.TextChanged += RatedPower_TextChanged;
            ComboBox Ltb50HzComboBox = new ComboBox() { Style = (Style)FindResource("comboBoxStyle"), Width = 90 };
            rating.ForEach(a => Ltb50HzComboBox.Items.Add(a.measure_unit));

            Ltb50HzComboBox.SelectedItem = "KVA";

            Label Kva50labelInvisible = new Label() { Style = (Style)FindResource("labelStyle") };
            Kva50labelInvisible.Visibility = Visibility.Collapsed;

            LtbKva50Panel.Children.Add(ltbKva50Label);
            LtbKva50Panel.Children.Add(kva50TextBox);
            LtbKva50Panel.Children.Add(Kva50labelInvisible);
            LtbKva50Panel.Children.Add(Ltb50HzComboBox);

            WrapPanel LTPkVA60HZPanel = new WrapPanel();

            Label ltbKva60Label = new Label() { Style = (Style)FindResource("labelStyle") };
            ltbKva60Label.Content = "LTB 60HZ";

            TextBox kva60TextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle"), TextWrapping = TextWrapping.Wrap, Width = 235 };
            kva60TextBox.TextChanged += RatedPower_TextChanged;
            ComboBox Ltb60HzComboBox = new ComboBox() { Style = (Style)FindResource("comboBoxStyle"), Width = 90 };
            rating.ForEach(a => Ltb60HzComboBox.Items.Add(a.measure_unit));
            Ltb60HzComboBox.SelectedItem = "KVA";


            Label Kva60labelInvisible = new Label() { Style = (Style)FindResource("labelStyle") };
            Kva60labelInvisible.Visibility = Visibility.Collapsed;


            LTPkVA60HZPanel.Children.Add(ltbKva60Label);
            LTPkVA60HZPanel.Children.Add(kva60TextBox);
            LTPkVA60HZPanel.Children.Add(Kva60labelInvisible);
            LTPkVA60HZPanel.Children.Add(Ltb60HzComboBox);

            wrapPanel2.Children.Add(LtbKva50Panel);
            wrapPanel2.Children.Add(LTPkVA60HZPanel);


            WrapPanel PRPkVA50HZPanel = new WrapPanel();

            Label prpKva50Label = new Label() { Style = (Style)FindResource("labelStyle") };
            prpKva50Label.Content = "PRP 50HZ";

            TextBox prpkva50TextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle"),TextWrapping = TextWrapping.Wrap, Width = 235 };

            prpkva50TextBox.TextChanged += RatedPower_TextChanged;

            ComboBox Prp50HzComboBox = new ComboBox() { Style = (Style)FindResource("comboBoxStyle"), Width = 90 };
            rating.ForEach(a =>Prp50HzComboBox.Items.Add(a.measure_unit));
            Prp50HzComboBox.SelectedItem = "KVA";



            Label prpKva50labelInvisible = new Label() { Style = (Style)FindResource("labelStyle")};
            prpKva50labelInvisible.Visibility = Visibility.Collapsed;

            PRPkVA50HZPanel.Children.Add(prpKva50Label);
            PRPkVA50HZPanel.Children.Add(prpkva50TextBox);
            PRPkVA50HZPanel.Children.Add(prpKva50labelInvisible);
            PRPkVA50HZPanel.Children.Add(Prp50HzComboBox);

            WrapPanel PRPkVA60HZPanel = new WrapPanel();

            Label prpKva60Label = new Label() { Style = (Style)FindResource("labelStyle")};
            prpKva60Label.Content = "PRP 60HZ";

            TextBox prpkva60TextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle"), TextWrapping = TextWrapping.Wrap, Width = 235 };
            prpkva60TextBox.TextChanged += RatedPower_TextChanged;
            ComboBox Prp60HzComboBox = new ComboBox() { Style = (Style)FindResource("comboBoxStyle"), Width = 90 };
            rating.ForEach(a => Prp60HzComboBox.Items.Add(a.measure_unit));
            Prp60HzComboBox.SelectedItem = "KVA";



            Label prpKva60labelInvisible = new Label() { Style = (Style)FindResource("labelStyle")};
            prpKva60labelInvisible.Visibility = Visibility.Collapsed;

            PRPkVA60HZPanel.Children.Add(prpKva60Label);
            PRPkVA60HZPanel.Children.Add(prpkva60TextBox);
            PRPkVA60HZPanel.Children.Add(prpKva60labelInvisible);
            PRPkVA60HZPanel.Children.Add(Prp60HzComboBox);


            wrapPanel3.Children.Add(PRPkVA50HZPanel);
            wrapPanel3.Children.Add(PRPkVA60HZPanel);



            WrapPanel CoolingPanel = new WrapPanel();

            Label CoolingLabel = new Label() { Style = (Style)FindResource("labelStyle")};
            CoolingLabel.Content = "Cooling";

            TextBox coolingTextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle"), TextWrapping = TextWrapping.Wrap, Width = 235 };

            Label coolinglabelInvisible = new Label() { Style = (Style)FindResource("labelStyle") };
            coolinglabelInvisible.Visibility = Visibility.Collapsed;


            CoolingPanel.Children.Add(CoolingLabel);
            CoolingPanel.Children.Add(coolingTextBox);
            CoolingPanel.Children.Add(coolinglabelInvisible);

            WrapPanel TankPanel = new WrapPanel();

            Label TankLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            TankLabel.Content = "TANK";

            TextBox TankTextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle"), TextWrapping = TextWrapping.Wrap, Width = 235 };


            Label tanklabelInvisible = new Label() { Style = (Style)FindResource("labelStyle") };
            tanklabelInvisible.Visibility = Visibility.Collapsed;

            TankPanel.Children.Add(TankLabel);
            TankPanel.Children.Add(TankTextBox);
            TankPanel.Children.Add(tanklabelInvisible);


            wrapPanel4.Children.Add(CoolingPanel);
            wrapPanel4.Children.Add(TankPanel);


            WrapPanel LOADPanel = new WrapPanel();

            Label LoadLabel = new Label() { Style = (Style)FindResource("labelStyle")};
            LoadLabel.Content = "Load";

            TextBox loadTextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle"), TextWrapping = TextWrapping.Wrap, Width = 235 };


            Label loadlabelInvisible = new Label() { Style = (Style)FindResource("labelStyle") };
            loadlabelInvisible.Visibility = Visibility.Collapsed;

            LOADPanel.Children.Add(LoadLabel);
            LOADPanel.Children.Add(loadTextBox);
            LOADPanel.Children.Add(loadlabelInvisible);
  
            WrapPanel AlternatorPanel = new WrapPanel();

            Label AlternatorLable = new Label() { Style = (Style)FindResource("labelStyle") };
            AlternatorLable.Content = "Alternator";

            TextBox AlternatorTextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle"), TextWrapping = TextWrapping.Wrap, Width = 235 };

            Label alternatorLabelInvisible = new Label() { Style = (Style)FindResource("labelStyle") };
            alternatorLabelInvisible.Visibility = Visibility.Collapsed;

            AlternatorPanel.Children.Add(AlternatorLable);
            AlternatorPanel.Children.Add(AlternatorTextBox);
            AlternatorPanel.Children.Add(alternatorLabelInvisible);

            wrapPanel5.Children.Add(LOADPanel);
            wrapPanel5.Children.Add(AlternatorPanel);

            WrapPanel datePanel = new WrapPanel();

            Label dateLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            dateLabel.Content = "Valid Until";
            DatePicker dateField = new DatePicker() { Style = (Style)FindResource("datePickerStyle"),Width = 253,SelectedDate= new DateTime(2030,1,1)};


            Label dateLabelInvisible = new Label() { Style = (Style)FindResource("labelStyle") };
            dateLabelInvisible.Visibility = Visibility.Collapsed;

            datePanel.Children.Add(dateLabel);
            datePanel.Children.Add(dateField);
            datePanel.Children.Add(dateLabelInvisible);

            wrapPanel6.Children.Add(datePanel);


            Card.Children.Add(Header);

            Card.Children.Add(wrapPanel1);

            Card.Children.Add(wrapPanel2);

            Card.Children.Add(wrapPanel3);

            Card.Children.Add(wrapPanel4);

            Card.Children.Add(wrapPanel5);

            Card.Children.Add(wrapPanel6);

            mainGrid.Children.Add(Card);


            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {

                ///to continue edit genset here
                    RatedPowerText.Visibility = Visibility.Collapsed;
                    ratedPowerlabelInvisible.Visibility = Visibility.Visible;
                    ratedPowerlabelInvisible.Foreground = Brushes.Black;
                ratedPowerlabelInvisible.MouseDoubleClick += RatedPowerlabelInvisible_MouseDoubleClick;
                    ratedPowercombo.IsEnabled = false;
                    ratedPowerlabelInvisible.Content = product.GetGensetSpecs()[cardCountGenset-1].RatedPower;

                    ModelText.Visibility = Visibility.Collapsed;
                    ModellabelInvisible.Visibility = Visibility.Visible;
                    ModellabelInvisible.Foreground = Brushes.Black;
                    ModellabelInvisible.Content = product.GetGensetSpecs()[cardCountGenset - 1].spec_name;

                    kva50TextBox.Visibility = Visibility.Collapsed;
                    Kva50labelInvisible.Visibility = Visibility.Visible;
                    Kva50labelInvisible.Content = product.GetGensetSpecs()[cardCountGenset - 1].ltb_50;
                    Kva50labelInvisible.Foreground = Brushes.Black;
                    Ltb50HzComboBox.IsEnabled = false;
                    Ltb50HzComboBox.SelectedItem = product.GetGensetSpecs()[cardCountGenset - 1].ltb_50_unit_name;




                    kva60TextBox.Visibility = Visibility.Collapsed;
                    Kva60labelInvisible.Visibility = Visibility.Visible;
                    Kva60labelInvisible.Content = product.GetGensetSpecs()[cardCountGenset-1].ltb_60;
                    Kva60labelInvisible.Foreground = Brushes.Black;
                    Ltb60HzComboBox.IsEnabled=false;
                    Ltb60HzComboBox.SelectedItem = product.GetGensetSpecs()[cardCountGenset - 1].ltb_60_unit_name;



                    prpkva50TextBox.Visibility = Visibility.Collapsed;
                    prpKva50labelInvisible.Visibility = Visibility.Visible;
                    prpKva50labelInvisible.Content = product.GetGensetSpecs()[cardCountGenset - 1].prp_50;
                    prpKva50labelInvisible.Foreground= Brushes.Black;
                    Prp50HzComboBox.IsEnabled = false;
                    Prp50HzComboBox.SelectedItem = product.GetGensetSpecs()[cardCountGenset - 1].prp_50_unit_name;



                    prpkva60TextBox.Visibility = Visibility.Collapsed;
                    prpKva60labelInvisible.Visibility = Visibility.Visible;
                    prpKva60labelInvisible.Content = product.GetGensetSpecs()[cardCountGenset - 1].prp_60;
                    prpKva60labelInvisible.Foreground=Brushes.Black;
                    Prp60HzComboBox.IsEnabled = false;
                    Prp60HzComboBox.SelectedItem = product.GetGensetSpecs()[cardCountGenset - 1].prp_60_unit_name;




                    coolingTextBox.Visibility = Visibility.Collapsed;
                    coolinglabelInvisible.Visibility = Visibility.Visible;
                    coolinglabelInvisible.Content = product.GetGensetSpecs()[cardCountGenset - 1].cooling;
                    coolinglabelInvisible.Foreground = Brushes.Black;



                    TankTextBox.Visibility = Visibility.Collapsed;
                    tanklabelInvisible.Visibility = Visibility.Visible;
                    tanklabelInvisible.Content = product.GetGensetSpecs()[cardCountGenset-1].tank;
                    tanklabelInvisible.Foreground = Brushes.Black;


                    loadTextBox.Visibility = Visibility.Collapsed;
                    loadlabelInvisible.Visibility = Visibility.Visible;
                    loadlabelInvisible.Content = product.GetGensetSpecs()[cardCountGenset - 1].load_percentage;
                    loadlabelInvisible.Foreground = Brushes.Black;



                    AlternatorTextBox.Visibility = Visibility.Collapsed;
                    alternatorLabelInvisible.Visibility = Visibility.Visible;
                    alternatorLabelInvisible.Content = product.GetGensetSpecs()[cardCountGenset-1].alternator;
                    alternatorLabelInvisible.Foreground = Brushes.Black;



                    dateField.Visibility = Visibility.Collapsed;
                    dateLabelInvisible.Visibility = Visibility.Visible;
                    dateLabelInvisible.Content = product.GetGensetSpecs()[cardCountGenset-1].valid_Until;
                    dateLabelInvisible.Foreground = Brushes.Black;
            }

        }

        private void RatedPowerlabelInvisible_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            Label ratedLabel = sender as Label;
  
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
            IOPhaseLabel.Style = (Style)FindResource("tableItemLabel");
            IOPhaseLabel.Visibility = Visibility.Collapsed;
            IOPhaseLabel.Width = 384;



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
            ratedPowerLabel.Style = (Style)FindResource("tableItemLabel");
            ratedPowerLabel.Visibility = Visibility.Collapsed;
            ratedPowerLabel.Width = 250;


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
            ratingLabel.Style = (Style)FindResource("tableItemLabel");
            ratingLabel.Visibility = Visibility.Collapsed;
            ratingLabel.Width = 90;


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
            backupTime50Label.Style = (Style)FindResource("tableItemLabel");
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
            backupTime70Label.Style = (Style)FindResource("tableItemLabel");
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
            backupTime100Label.Style = (Style)FindResource("tableItemLabel");
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
            inputPowerFactorPhaseLabel.Style = (Style)FindResource("tableItemLabel");
            inputPowerFactorPhaseLabel.Visibility = Visibility.Collapsed;
            inputPowerFactorPhaseLabel.Width = 384;



            Label THDI = new Label();
            THDI.Content = "THDI";
            THDI.Style = (Style)FindResource("labelStyle");
            THDI.Width = 210;

            TextBox THDITextBox = new TextBox();
            THDITextBox.Style = (Style)FindResource("textBoxStyle");
            THDITextBox.TextWrapping = TextWrapping.Wrap;

            Label THDILabel = new Label();
            THDILabel.Style = (Style)FindResource("tableItemLabel");
            THDILabel.Visibility = Visibility.Collapsed;
            THDILabel.Width = 384;



            Label inputNominalVoltage = new Label();
            inputNominalVoltage.Content = "Input Voltage";
            inputNominalVoltage.Style = (Style)FindResource("labelStyle");
            inputNominalVoltage.Width = 210;

            TextBox inputNominalVoltageTextBox = new TextBox();
            inputNominalVoltageTextBox.Style = (Style)FindResource("textBoxStyle");
            inputNominalVoltageTextBox.TextWrapping = TextWrapping.Wrap;

            Label inputNominalVoltageLabel = new Label();
            inputNominalVoltageLabel.Style = (Style)FindResource("tableItemLabel");
            inputNominalVoltageLabel.Visibility = Visibility.Collapsed;
            inputNominalVoltageLabel.Width = 384;


            Label inputVoltage = new Label();
            inputVoltage.Content = "Input Nominal Voltage";
            inputVoltage.Style = (Style)FindResource("labelStyle");
            inputVoltage.Width = 210;

            TextBox inputVoltageTextBox = new TextBox();
            inputVoltageTextBox.Style = (Style)FindResource("textBoxStyle");
            inputVoltageTextBox.TextWrapping = TextWrapping.Wrap;

            Label inputVoltageLabel = new Label();
            inputVoltageLabel.Style = (Style)FindResource("tableItemLabel");
            inputVoltageLabel.Visibility = Visibility.Collapsed;
            inputVoltageLabel.Width = 384;


            Label voltageTolerance = new Label();
            voltageTolerance.Content = "Voltage Tolerance";
            voltageTolerance.Style = (Style)FindResource("labelStyle");
            voltageTolerance.Width = 210;

            TextBox voltageToleranceTextBox = new TextBox();
            voltageToleranceTextBox.Style = (Style)FindResource("textBoxStyle");
            voltageToleranceTextBox.TextWrapping = TextWrapping.Wrap;

            Label voltageToleranceLabel = new Label();
            voltageToleranceLabel.Style = (Style)FindResource("tableItemLabel");
            voltageToleranceLabel.Visibility = Visibility.Collapsed;
            voltageToleranceLabel.Width = 384;




            Label outputPowerFactor = new Label();
            outputPowerFactor.Content = "Output Power Factor";
            outputPowerFactor.Style = (Style)FindResource("labelStyle");
            outputPowerFactor.Width = 210;

            TextBox outputPowerFactorTextBox = new TextBox();
            outputPowerFactorTextBox.Style = (Style)FindResource("textBoxStyle");
            outputPowerFactorTextBox.TextWrapping = TextWrapping.Wrap;

            Label outputPowerFactorLabel = new Label();
            outputPowerFactorLabel.Style = (Style)FindResource("tableItemLabel");
            outputPowerFactorLabel.Visibility = Visibility.Collapsed;
            outputPowerFactorLabel.Width = 384;


            Label THDV = new Label();
            THDV.Content = "THDV";
            THDV.Style = (Style)FindResource("labelStyle");
            THDV.Width = 210;

            TextBox THDVTextBox = new TextBox();
            THDVTextBox.Style = (Style)FindResource("textBoxStyle");
            THDVTextBox.TextWrapping = TextWrapping.Wrap;

            Label THDVLabel = new Label();
            THDVLabel.Style = (Style)FindResource("tableItemLabel");
            THDVLabel.Visibility = Visibility.Collapsed;
            THDVLabel.Width = 384;



            Label outputNominalVoltage = new Label();
            outputNominalVoltage.Content = "Output Nominal Voltage";
            outputNominalVoltage.Style = (Style)FindResource("labelStyle");
            outputNominalVoltage.Width = 210;

            TextBox outputNominalVoltageTextBox = new TextBox();
            outputNominalVoltageTextBox.Style = (Style)FindResource("textBoxStyle");
            outputNominalVoltageTextBox.TextWrapping = TextWrapping.Wrap;

            Label outputNominalVoltageLabel = new Label();
            outputNominalVoltageLabel.Style = (Style)FindResource("tableItemLabel");
            outputNominalVoltageLabel.Visibility = Visibility.Collapsed;
            outputNominalVoltageLabel.Width = 384;


            Label outputDCVoltageRange = new Label();
            outputDCVoltageRange.Content = "Output DC Voltage Range";
            outputDCVoltageRange.Style = (Style)FindResource("labelStyle");
            outputDCVoltageRange.Width = 210;

            TextBox outputDCVoltageRangeTextBox = new TextBox();
            outputDCVoltageRangeTextBox.Style = (Style)FindResource("textBoxStyle");
            outputDCVoltageRangeTextBox.TextWrapping = TextWrapping.Wrap;

            Label outputDCVoltageRangeLabel = new Label();
            outputDCVoltageRangeLabel.Style = (Style)FindResource("tableItemLabel");
            outputDCVoltageRangeLabel.Visibility = Visibility.Collapsed;
            outputDCVoltageRangeLabel.Width = 384;


            Label overloadCapability = new Label();
            overloadCapability.Content = "Overload Capability";
            overloadCapability.Style = (Style)FindResource("labelStyle");
            overloadCapability.Width = 210;

            TextBox overloadCapabilityTextBox = new TextBox();
            overloadCapabilityTextBox.Style = (Style)FindResource("textBoxStyle");
            overloadCapabilityTextBox.TextWrapping = TextWrapping.Wrap;

            Label overloadCapabilityLabel = new Label();
            overloadCapabilityLabel.Style = (Style)FindResource("tableItemLabel");
            overloadCapabilityLabel.Visibility = Visibility.Collapsed;
            overloadCapabilityLabel.Width = 384;


            Label efficiency = new Label();
            efficiency.Content = "Efficiency";
            efficiency.Style = (Style)FindResource("labelStyle");
            efficiency.Width = 210;

            TextBox efficiencyTextBox = new TextBox();
            efficiencyTextBox.Style = (Style)FindResource("textBoxStyle");
            efficiencyTextBox.TextWrapping = TextWrapping.Wrap;

            Label efficiencyLabel = new Label();
            efficiencyLabel.Style = (Style)FindResource("tableItemLabel");
            efficiencyLabel.Visibility = Visibility.Collapsed;
            efficiencyLabel.Width = 384;


            Label inputConnectionType = new Label();
            inputConnectionType.Content = "Input Connection Type";
            inputConnectionType.Style = (Style)FindResource("labelStyle");
            inputConnectionType.Width = 210;

            TextBox inputConnectionTypeTextBox = new TextBox();
            inputConnectionTypeTextBox.Style = (Style)FindResource("textBoxStyle");
            inputConnectionTypeTextBox.TextWrapping = TextWrapping.Wrap;

            Label inputConnectionTypeLabel = new Label();
            inputConnectionTypeLabel.Style = (Style)FindResource("tableItemLabel");
            inputConnectionTypeLabel.Visibility = Visibility.Collapsed;
            inputConnectionTypeLabel.Width = 384;


            Label frontPanel = new Label();
            frontPanel.Content = "Front Panel";
            frontPanel.Style = (Style)FindResource("labelStyle");
            frontPanel.Width = 210;

            TextBox frontPanelTextBox = new TextBox();
            frontPanelTextBox.Style = (Style)FindResource("textBoxStyle");
            frontPanelTextBox.TextWrapping = TextWrapping.Wrap;

            Label frontPanelLabel = new Label();
            frontPanelLabel.Style = (Style)FindResource("tableItemLabel");
            frontPanelLabel.Visibility = Visibility.Collapsed;
            frontPanelLabel.Width = 384;


            Label maxPower = new Label();
            maxPower.Content = "Max Power";
            maxPower.Style = (Style)FindResource("labelStyle");
            maxPower.Width = 210;

            TextBox maxPowerTextBox = new TextBox();
            maxPowerTextBox.Style = (Style)FindResource("textBoxStyle");
            maxPowerTextBox.TextWrapping = TextWrapping.Wrap;

            Label maxPowerLabel = new Label();
            maxPowerLabel.Style = (Style)FindResource("tableItemLabel");
            maxPowerLabel.Visibility = Visibility.Collapsed;
            maxPowerLabel.Width = 384;


            Label certificates = new Label();
            certificates.Content = "Certificates";
            certificates.Style = (Style)FindResource("labelStyle");
            certificates.Width = 210;

            TextBox certificatesTextBox = new TextBox();
            certificatesTextBox.Style = (Style)FindResource("textBoxStyle");
            certificatesTextBox.TextWrapping = TextWrapping.Wrap;

            Label certificatesLabel = new Label();
            certificatesLabel.Style = (Style)FindResource("tableItemLabel");
            certificatesLabel.Visibility = Visibility.Collapsed;
            certificatesLabel.Width = 384;


            Label safety = new Label();
            safety.Content = "Safety";
            safety.Style = (Style)FindResource("labelStyle");
            safety.Width = 210;

            TextBox safetyTextBox = new TextBox();
            safetyTextBox.Style = (Style)FindResource("textBoxStyle");
            safetyTextBox.TextWrapping = TextWrapping.Wrap;

            Label safetyLabel = new Label();
            safetyLabel.Style = (Style)FindResource("tableItemLabel");
            safetyLabel.Visibility = Visibility.Collapsed;
            safetyLabel.Width = 384;


            Label EMC = new Label();
            EMC.Content = "EMC";
            EMC.Style = (Style)FindResource("labelStyle");
            EMC.Width = 210;

            TextBox EMCTextBox = new TextBox();
            EMCTextBox.Style = (Style)FindResource("textBoxStyle");
            EMCTextBox.TextWrapping = TextWrapping.Wrap;

            Label EMCLabel = new Label();
            EMCLabel.Style = (Style)FindResource("tableItemLabel");
            EMCLabel.Visibility = Visibility.Collapsed;
            EMCLabel.Width = 384;


            Label environmentalAspects = new Label();
            environmentalAspects.Content = "Environmental Aspects";
            environmentalAspects.Style = (Style)FindResource("labelStyle");
            environmentalAspects.Width = 210;

            TextBox environmentalAspectsTextBox = new TextBox();
            environmentalAspectsTextBox.Style = (Style)FindResource("textBoxStyle");
            environmentalAspectsTextBox.TextWrapping = TextWrapping.Wrap;

            Label environmentalAspectsLabel = new Label();
            environmentalAspectsLabel.Style = (Style)FindResource("tableItemLabel");
            environmentalAspectsLabel.Visibility = Visibility.Collapsed;
            environmentalAspectsLabel.Width = 384;


            Label testPerformance = new Label();
            testPerformance.Content = "Test Performance";
            testPerformance.Style = (Style)FindResource("labelStyle");
            testPerformance.Width = 210;

            TextBox testPerformanceTextBox = new TextBox();
            testPerformanceTextBox.Style = (Style)FindResource("textBoxStyle");
            testPerformanceTextBox.TextWrapping = TextWrapping.Wrap;

            Label testPerformanceLabel = new Label();
            testPerformanceLabel.Style = (Style)FindResource("tableItemLabel");
            testPerformanceLabel.Visibility = Visibility.Collapsed;
            testPerformanceLabel.Width = 384;


            Label protectionDegree = new Label();
            protectionDegree.Content = "Protection Degree";
            protectionDegree.Style = (Style)FindResource("labelStyle");
            protectionDegree.Width = 210;

            TextBox protectionDegreeTextBox = new TextBox();
            protectionDegreeTextBox.Style = (Style)FindResource("textBoxStyle");
            protectionDegreeTextBox.TextWrapping = TextWrapping.Wrap;

            Label protectionDegreeLabel = new Label();
            protectionDegreeLabel.Style = (Style)FindResource("tableItemLabel");
            protectionDegreeLabel.Visibility = Visibility.Collapsed;
            protectionDegreeLabel.Width = 384;


            Label transferVoltageLimit = new Label();
            transferVoltageLimit.Content = "Transfer Voltage Limit";
            transferVoltageLimit.Style = (Style)FindResource("labelStyle");
            transferVoltageLimit.Width = 210;

            TextBox transferVoltageLimitTextBox = new TextBox();
            transferVoltageLimitTextBox.Style = (Style)FindResource("textBoxStyle");
            transferVoltageLimitTextBox.TextWrapping = TextWrapping.Wrap;

            Label transferVoltageLimitLabel = new Label();
            transferVoltageLimitLabel.Style = (Style)FindResource("tableItemLabel");
            transferVoltageLimitLabel.Visibility = Visibility.Collapsed;
            transferVoltageLimitLabel.Width = 384;


            Label marking = new Label();
            marking.Content = "Marking";
            marking.Style = (Style)FindResource("labelStyle");
            marking.Width = 210;

            TextBox markingTextBox = new TextBox();
            markingTextBox.Style = (Style)FindResource("textBoxStyle");
            markingTextBox.TextWrapping = TextWrapping.Wrap;

            Label markingLabel = new Label();
            markingLabel.Style = (Style)FindResource("tableItemLabel");
            markingLabel.Visibility = Visibility.Collapsed;
            markingLabel.Width = 384;


            Label validUntil = new Label();
            validUntil.Content = "Valid Until*";
            validUntil.Style = (Style)FindResource("labelStyle");
            validUntil.Width = 210;

            DatePicker validUntilDatePicker = new DatePicker();
            validUntilDatePicker.Style = (Style)FindResource("datePickerStyle");
            validUntilDatePicker.Width = 384;
            validUntilDatePicker.SelectedDate = new DateTime(2030,1,1);


            Label validUntilLabel = new Label();
            validUntilLabel.Style = (Style)FindResource("tableItemLabel");
            validUntilLabel.Visibility = Visibility.Collapsed;
            validUntilLabel.Width = 384;



            //wrapPanelRow1 .Children.Add(Spec);

            wrapPanelRow2.Children.Add(IOPhase);
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
                IOPhaseLabel.Content = product.GetUPSSpecs()[index].io_phase;

                ratedPowerTextBox.Visibility = Visibility.Collapsed;
                ratedPowerLabel.Visibility = Visibility.Visible;
                ratedPowerLabel.Content = product.GetUPSSpecs()[index].rated_power;

                ratingComboBox.Visibility = Visibility.Collapsed;
                ratingLabel.Visibility = Visibility.Visible;
                ratingLabel.Content = product.GetUPSSpecs()[index].rating;


                backupTime50TextBox.Visibility = Visibility.Collapsed;
                backupTime50Label.Visibility = Visibility.Visible;
                backupTime50Label.Content = product.GetUPSSpecs()[index].backup_time_50;


                backupTime70TextBox.Visibility = Visibility.Collapsed;
                backupTime70Label.Visibility = Visibility.Visible;
                backupTime70Label.Content = product.GetUPSSpecs()[index].backup_time_70;


                backupTime100TextBox.Visibility = Visibility.Collapsed;
                backupTime100Label.Visibility = Visibility.Visible;
                backupTime100Label.Content = product.GetUPSSpecs()[index].backup_time_100;


                inputPowerFactorPhaseTextBox.Visibility = Visibility.Collapsed;
                inputPowerFactorPhaseLabel.Visibility = Visibility.Visible;
                inputPowerFactorPhaseLabel.Content = product.GetUPSSpecs()[index].input_power_factor;


                THDITextBox.Visibility = Visibility.Collapsed;
                THDILabel.Visibility = Visibility.Visible;
                THDILabel.Content = product.GetUPSSpecs()[index].thdi;


                inputNominalVoltageTextBox.Visibility = Visibility.Collapsed;
                inputNominalVoltageLabel.Visibility = Visibility.Visible;
                inputNominalVoltageLabel.Content = product.GetUPSSpecs()[index].input_nominal_voltage;


                inputVoltageTextBox.Visibility = Visibility.Collapsed;
                inputVoltageLabel.Visibility = Visibility.Visible;
                inputVoltageLabel.Content = product.GetUPSSpecs()[index].input_voltage;


                voltageToleranceTextBox.Visibility = Visibility.Collapsed;
                voltageToleranceLabel.Visibility = Visibility.Visible;
                voltageToleranceLabel.Content = product.GetUPSSpecs()[index].voltage_tolerance;


                outputPowerFactorTextBox.Visibility = Visibility.Collapsed;
                outputPowerFactorLabel.Visibility = Visibility.Visible;
                outputPowerFactorLabel.Content = product.GetUPSSpecs()[index].output_power_factor;


                THDVTextBox.Visibility = Visibility.Collapsed;
                THDVLabel.Visibility = Visibility.Visible;
                THDVLabel.Content = product.GetUPSSpecs()[index].thdv;


                outputNominalVoltageTextBox.Visibility = Visibility.Collapsed;
                outputNominalVoltageLabel.Visibility = Visibility.Visible;
                outputNominalVoltageLabel.Content = product.GetUPSSpecs()[index].output_nominal_voltage;


                outputDCVoltageRangeTextBox.Visibility = Visibility.Collapsed;
                outputDCVoltageRangeLabel.Visibility = Visibility.Visible;
                outputDCVoltageRangeLabel.Content = product.GetUPSSpecs()[index].output_dc_voltage_range;


                overloadCapabilityTextBox.Visibility = Visibility.Collapsed;
                overloadCapabilityLabel.Visibility = Visibility.Visible;
                overloadCapabilityLabel.Content = product.GetUPSSpecs()[index].overload_capability;


                efficiencyTextBox.Visibility = Visibility.Collapsed;
                efficiencyLabel.Visibility = Visibility.Visible;
                efficiencyLabel.Content = product.GetUPSSpecs()[index].efficiency;


                inputConnectionTypeTextBox.Visibility = Visibility.Collapsed;
                inputConnectionTypeLabel.Visibility = Visibility.Visible;
                inputConnectionTypeLabel.Content = product.GetUPSSpecs()[index].input_connection_type;


                frontPanelTextBox.Visibility = Visibility.Collapsed;
                frontPanelLabel.Visibility = Visibility.Visible;
                frontPanelLabel.Content = product.GetUPSSpecs()[index].front_panel;


                maxPowerTextBox.Visibility = Visibility.Collapsed;
                maxPowerLabel.Visibility = Visibility.Visible;
                maxPowerLabel.Content = product.GetUPSSpecs()[index].max_power;


                certificatesTextBox.Visibility = Visibility.Collapsed;
                certificatesLabel.Visibility = Visibility.Visible;
                certificatesLabel.Content = product.GetUPSSpecs()[index].certificates;


                safetyTextBox.Visibility = Visibility.Collapsed;
                safetyLabel.Visibility = Visibility.Visible;
                safetyLabel.Content = product.GetUPSSpecs()[index].safety;


                EMCTextBox.Visibility = Visibility.Collapsed;
                EMCLabel.Visibility = Visibility.Visible;
                EMCLabel.Content = product.GetUPSSpecs()[index].emc;


                environmentalAspectsTextBox.Visibility = Visibility.Collapsed;
                environmentalAspectsLabel.Visibility = Visibility.Visible;
                environmentalAspectsLabel.Content = product.GetUPSSpecs()[index].environmental_aspects;


                testPerformanceTextBox.Visibility = Visibility.Collapsed;
                testPerformanceLabel.Visibility = Visibility.Visible;
                testPerformanceLabel.Content = product.GetUPSSpecs()[index].test_performance;



                protectionDegreeTextBox.Visibility = Visibility.Collapsed;
                protectionDegreeLabel.Visibility = Visibility.Visible;
                protectionDegreeLabel.Content = product.GetUPSSpecs()[index].protection_degree;


                transferVoltageLimitTextBox.Visibility = Visibility.Collapsed;
                transferVoltageLimitLabel.Visibility = Visibility.Visible;
                transferVoltageLimitLabel.Content = product.GetUPSSpecs()[index].transfer_voltage_limit;


                markingTextBox.Visibility = Visibility.Collapsed;
                markingLabel.Visibility = Visibility.Visible;
                markingLabel.Content = product.GetUPSSpecs()[index].marking;


                validUntilDatePicker.Visibility = Visibility.Collapsed;
                validUntilLabel.Visibility = Visibility.Visible;
                validUntilLabel.Content = product.GetUPSSpecs()[index].valid_until;



            }





        }

        private void ratingComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void OnBtnClickNext(object sender, RoutedEventArgs e)
        {

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
            {

                if (product.GetCategoryID() == COMPANY_WORK_MACROS.GENSET_CATEGORY_ID)
                {
                    List<BASIC_STRUCTS.GENSET_SPEC> GensetSpecs = new List<BASIC_STRUCTS.GENSET_SPEC>();


                    for (int i = 0; i < mainGrid.Children.Count; i++)
                    {

                        BASIC_STRUCTS.GENSET_SPEC gensetSpec = new BASIC_STRUCTS.GENSET_SPEC();

                        Grid card = mainGrid.Children[i] as Grid;


                        WrapPanel wrap1 = card.Children[1] as WrapPanel;

                        WrapPanel ratedPanel = wrap1.Children[0] as WrapPanel;

                        TextBox ratedPower = ratedPanel.Children[1] as TextBox;
                        if (ratedPower.Text == "")
                            gensetSpec.RatedPower = null;
                        else
                            gensetSpec.RatedPower = Convert.ToDecimal(ratedPower.Text);

                        ComboBox ratedPowerCombo = ratedPanel.Children[2] as ComboBox;

                        gensetSpec.rating_unit_id = rating[ratedPowerCombo.SelectedIndex].measure_unit_id;

                        WrapPanel modelPanel = wrap1.Children[1] as WrapPanel;

                        TextBox modelTextBox = modelPanel.Children[1] as TextBox;
                        gensetSpec.spec_name = modelTextBox.Text;
                        gensetSpec.spec_id = int.Parse(card.Tag.ToString());



                        WrapPanel wrap2 = card.Children[2] as WrapPanel;

                        WrapPanel ltbKva50Panel = wrap2.Children[0] as WrapPanel;

                        TextBox ltbKva50TextBox = ltbKva50Panel.Children[1] as TextBox;

                        if (ltbKva50TextBox.Text == "")
                            gensetSpec.ltb_50 = 0;
                        else
                            gensetSpec.ltb_50 = decimal.Parse(ltbKva50TextBox.Text);

                        ComboBox ltbKva50ComboBox = ltbKva50Panel.Children[2] as ComboBox;

                        gensetSpec.ltb_50_unit = rating[ltbKva50ComboBox.SelectedIndex].measure_unit_id;


                        WrapPanel ltbKva60Panel = wrap2.Children[1] as WrapPanel;

                        TextBox ltbKva60TextBox = ltbKva60Panel.Children[1] as TextBox;

                        if (ltbKva60TextBox.Text == "")
                            gensetSpec.ltb_60 = 0;
                        else
                            gensetSpec.ltb_60 = decimal.Parse(ltbKva60TextBox.Text);

                        ComboBox ltbKva60ComboBox = ltbKva60Panel.Children[2] as ComboBox;

                        gensetSpec.ltb_60_unit = rating[ltbKva60ComboBox.SelectedIndex].measure_unit_id;



                        WrapPanel wrap3 = card.Children[3] as WrapPanel;

                        WrapPanel prpKva50Panel = wrap3.Children[0] as WrapPanel;

                        TextBox prpKva50TextBox = prpKva50Panel.Children[1] as TextBox;

                        if (prpKva50TextBox.Text == "")
                            gensetSpec.prp_50 = 0;
                        else
                            gensetSpec.prp_50 = decimal.Parse(prpKva50TextBox.Text);

                        ComboBox prpKva50ComboBox = ltbKva50Panel.Children[2] as ComboBox;

                        gensetSpec.prp_50_unit = rating[prpKva50ComboBox.SelectedIndex].measure_unit_id;


                        WrapPanel prpKva60Panel = wrap3.Children[1] as WrapPanel;

                        TextBox prpKva60TextBox = prpKva60Panel.Children[1] as TextBox;

                        if (prpKva60TextBox.Text == "")
                            gensetSpec.prp_60 = 0;
                        else
                            gensetSpec.prp_60 = decimal.Parse(prpKva60TextBox.Text);

                        ComboBox prpKva60ComboBox = ltbKva50Panel.Children[2] as ComboBox;

                        gensetSpec.prp_60_unit = rating[prpKva60ComboBox.SelectedIndex].measure_unit_id;

                        WrapPanel wrap4 = card.Children[4] as WrapPanel;

                        WrapPanel CoolingPanel = wrap4.Children[0] as WrapPanel;

                        TextBox coolingTextBox = CoolingPanel.Children[1] as TextBox;

                        gensetSpec.cooling = coolingTextBox.Text;


                        WrapPanel tankPanel = wrap4.Children[1] as WrapPanel;

                        TextBox tankTextBox = tankPanel.Children[1] as TextBox;

                        gensetSpec.tank = tankTextBox.Text;


                        WrapPanel wrap5 = card.Children[5] as WrapPanel;

                        WrapPanel loadPanel = wrap5.Children[0] as WrapPanel;

                        TextBox loadTextBox = loadPanel.Children[1] as TextBox;

                        gensetSpec.load_percentage = loadTextBox.Text;


                        WrapPanel alternatorPanel = wrap5.Children[1] as WrapPanel;

                        TextBox alternatorTextBox = alternatorPanel.Children[1] as TextBox;

                        gensetSpec.alternator = alternatorTextBox.Text;

                        WrapPanel wrap6 = card.Children[6] as WrapPanel;

                        WrapPanel datePanel = wrap6.Children[0] as WrapPanel;

                        DatePicker dateTextBox = datePanel.Children[1] as DatePicker;

                        gensetSpec.valid_Until = (DateTime)dateTextBox.SelectedDate;
                        gensetSpec.is_valid = true;

                        GensetSpecs.Add(gensetSpec);
                    }

                    product.SetGensetSpec(GensetSpecs);

                }

                else
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

                if (product.GetCategoryID() == COMPANY_WORK_MACROS.GENSET_CATEGORY_ID)
                {
                    List<BASIC_STRUCTS.GENSET_SPEC> GensetSpecs=new List<BASIC_STRUCTS.GENSET_SPEC>();


                    for (int i = 0; i < mainGrid.Children.Count; i++)
                    {

                        BASIC_STRUCTS.GENSET_SPEC gensetSpec=new BASIC_STRUCTS.GENSET_SPEC();

                        Grid card = mainGrid.Children[i] as Grid;


                        WrapPanel wrap1 = card.Children[1] as WrapPanel;

                        WrapPanel ratedPanel = wrap1.Children[0] as WrapPanel;

                        TextBox ratedPower = ratedPanel.Children[1] as TextBox;
                        if (ratedPower.Text == "")
                            gensetSpec.RatedPower = null;
                        else
                        gensetSpec.RatedPower = Convert.ToDecimal(ratedPower.Text);

                        ComboBox ratedPowerCombo = ratedPanel.Children[2] as ComboBox;

                       gensetSpec.rating_unit_id=rating[ratedPowerCombo.SelectedIndex].measure_unit_id;



                        WrapPanel modelPanel = wrap1.Children[1] as WrapPanel;

                        TextBox modelTextBox = modelPanel.Children[1] as TextBox;
                        gensetSpec.spec_name = modelTextBox.Text;
                        gensetSpec.spec_id =  int.Parse(card.Tag.ToString());



                        WrapPanel wrap2= card.Children[2] as WrapPanel;

                        WrapPanel ltbKva50Panel = wrap2.Children[0] as WrapPanel;

                        TextBox ltbKva50TextBox=ltbKva50Panel.Children[1] as TextBox;

                        if (ltbKva50TextBox.Text == "")
                            gensetSpec.ltb_50 = 0;
                        else
                        gensetSpec.ltb_50 = decimal.Parse(ltbKva50TextBox.Text);

                        ComboBox ltbKva50ComboBox = ltbKva50Panel.Children[2] as ComboBox;

                        gensetSpec.ltb_50_unit = rating[ltbKva50ComboBox.SelectedIndex].measure_unit_id;


                        WrapPanel ltbKva60Panel = wrap2.Children[1] as WrapPanel;

                        TextBox ltbKva60TextBox = ltbKva60Panel.Children[1] as TextBox;

                        if (ltbKva60TextBox.Text == "")
                            gensetSpec.ltb_60 = 0;
                        else
                            gensetSpec.ltb_60 = decimal.Parse(ltbKva60TextBox.Text);

                        ComboBox ltbKva60ComboBox = ltbKva60Panel.Children[2] as ComboBox;

                        gensetSpec.ltb_60_unit = rating[ltbKva60ComboBox.SelectedIndex].measure_unit_id;



                        WrapPanel wrap3 = card.Children[3] as WrapPanel;

                        WrapPanel prpKva50Panel = wrap3.Children[0] as WrapPanel;

                        TextBox prpKva50TextBox = prpKva50Panel.Children[1] as TextBox;

                        if (prpKva50TextBox.Text == "")
                            gensetSpec.prp_50 = 0;
                        else
                            gensetSpec.prp_50 = decimal.Parse(prpKva50TextBox.Text);

                        ComboBox prpKva50ComboBox = ltbKva50Panel.Children[2] as ComboBox;

                        gensetSpec.prp_50_unit = rating[prpKva50ComboBox.SelectedIndex].measure_unit_id;


                        WrapPanel prpKva60Panel = wrap3.Children[1] as WrapPanel;

                        TextBox prpKva60TextBox = prpKva60Panel.Children[1] as TextBox;

                        if (prpKva60TextBox.Text == "")
                            gensetSpec.prp_60 = 0;
                        else
                            gensetSpec.prp_60 = decimal.Parse(prpKva60TextBox.Text);

                        ComboBox prpKva60ComboBox = ltbKva50Panel.Children[2] as ComboBox;

                        gensetSpec.prp_60_unit = rating[prpKva60ComboBox.SelectedIndex].measure_unit_id;

                        WrapPanel wrap4 = card.Children[4] as WrapPanel;

                        WrapPanel CoolingPanel = wrap4.Children[0] as WrapPanel;

                        TextBox coolingTextBox = CoolingPanel.Children[1] as TextBox;

                        gensetSpec.cooling = coolingTextBox.Text;


                        WrapPanel tankPanel = wrap4.Children[1] as WrapPanel;

                        TextBox tankTextBox = tankPanel.Children[1] as TextBox;

                        gensetSpec.tank = tankTextBox.Text;


                        WrapPanel wrap5 = card.Children[5] as WrapPanel;

                        WrapPanel loadPanel = wrap5.Children[0] as WrapPanel;

                        TextBox loadTextBox = loadPanel.Children[1] as TextBox;

                        gensetSpec.load_percentage = loadTextBox.Text;


                        WrapPanel alternatorPanel = wrap5.Children[1] as WrapPanel;

                        TextBox alternatorTextBox = alternatorPanel.Children[1] as TextBox;

                        gensetSpec.alternator = alternatorTextBox.Text;

                        WrapPanel wrap6 = card.Children[6] as WrapPanel;

                        WrapPanel datePanel = wrap6.Children[0] as WrapPanel;

                        DatePicker dateTextBox = datePanel.Children[1] as DatePicker;

                        gensetSpec.valid_Until = (DateTime)dateTextBox.SelectedDate;
                        gensetSpec.is_valid = true;

                        GensetSpecs.Add(gensetSpec);
                    }

                    product.SetGensetSpec(GensetSpecs);


                }

                else
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

                            if (ratedPowerTextBox.Text != "" || (product.GetUPSSpecs().Count > 0 /*&& product.GetUPSSpecs()[0].rated_power != null*/))
                            {
                                tempUPSSpecs.rated_power = decimal.Parse(ratedPowerTextBox.Text.ToString());
                            }

                            if (ratingComboBox.SelectedIndex != -1 || (product.GetUPSSpecs().Count > 0 && product.GetUPSSpecs()[0].rating != null))
                            {
                                tempUPSSpecs.rating = ratingComboBox.SelectedItem.ToString();
                                tempUPSSpecs.rating_id = ratingComboBox.SelectedIndex + 1;
                            }


                            if (backupTime50TextBox.Text != "" || (product.GetUPSSpecs().Count > 0 /*&& product.GetUPSSpecs()[0].backup_time_50 != null*/))
                            {
                                tempUPSSpecs.backup_time_50 = int.Parse(backupTime50TextBox.Text.ToString());
                            }


                            if (backupTime70TextBox.Text != "" || (product.GetUPSSpecs().Count > 0 /*&& product.GetUPSSpecs()[0].backup_time_70 != null*/))
                            {
                                tempUPSSpecs.backup_time_70 = int.Parse(backupTime70TextBox.Text.ToString());
                            }


                            if (backupTime100TextBox.Text != "" || (product.GetUPSSpecs().Count > 0 /*&& product.GetUPSSpecs()[0].backup_time_100 != null*/))
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

        private void RatedPower_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox ratedText = sender as TextBox;

            for (int i = 0; i < ratedText.Text.Length; i++) {


                if (char.IsDigit(ratedText.Text[i]) != true) {
                    ratedText.Text = "";

                    break;
                }

            }   
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


        private void MouseLeftButtonDownGenset(object sender, MouseEventArgs e)
        {

            Image b1 = sender as Image;
            int tag = int.Parse(b1.Tag.ToString());

            if (tag < mainGrid.Children.Count) {
                for (int i = tag; i < mainGrid.Children.Count; i++) {

                    Grid card = mainGrid.Children[i] as Grid;

                    Grid header = card.Children[0] as Grid;
                    Label head = header.Children[1] as Label;
                    Image image = header.Children[0] as Image;
                    image.Tag = i;
                    head.Content = $"SPEC {i}";
                }      
            }
            mainGrid.Children.Remove(mainGrid.Children[tag - 1]);
            cardCountGenset--;
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
