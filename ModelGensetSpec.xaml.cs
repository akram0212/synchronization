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
    /// Interaction logic for ModelGensetSpec.xaml
    /// </summary>
    public partial class ModelGensetSpec : Page
    {
        public ModelGensetSpec()
        {
            InitializeComponent();
            InitializeCard();


        }

        private void InitializeCard()
        {

            Home.RowDefinitions.Add(new RowDefinition());

            Grid Card = new Grid() { VerticalAlignment = VerticalAlignment.Top };

            Card.Background = new SolidColorBrush(Colors.White);
            Card.Margin = new Thickness(20);

            Grid.SetRow(Card, 0);

            Grid Header = new Grid() { Height = 50 };
            Header.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF105A97");

            Label header = new Label() { Content = "SPEC "+(Home.Children.Count+1), Style = (Style)FindResource("tableHeaderItem") };
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

            TextBox RatedPowerText = new TextBox() { Style = (Style)FindResource("textBoxStyle"),Width= 253 };

            ComboBox ratedPowercombo = new ComboBox() { Style = (Style)FindResource("comboBoxStyle"),Width = 90 };

            WrapPanel ratedPowerPanel = new WrapPanel();

            ratedPowerPanel.Children.Add(ratedPowerLable);
            ratedPowerPanel.Children.Add(RatedPowerText);
            ratedPowerPanel.Children.Add(ratedPowercombo);

            Grid.SetRow(ratedPowerPanel, 1);

            WrapPanel EnginePanel = new WrapPanel();

            Label engineLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            engineLabel.Content = "Engine";

            ComboBox engineCombo = new ComboBox() { Style = (Style)FindResource("comboBoxStyle")};

            EnginePanel.Children.Add(engineLabel);
            EnginePanel.Children.Add(engineCombo);

            Grid.SetRow(EnginePanel, 2);


            WrapPanel modelPanel = new WrapPanel();

            Label ModelLabel = new Label() {Style = (Style)FindResource("labelStyle") };
            ModelLabel.Content = "Model";

            TextBox ModelText = new TextBox() {Style=(Style)FindResource("textBoxStyle")};

            modelPanel.Children.Add(ModelLabel);
            modelPanel.Children.Add(ModelText);

            Grid.SetRow(modelPanel, 3);


            WrapPanel LtbKva50Panel = new WrapPanel();

            Label ltbKva50Label = new Label() { Style = (Style)FindResource("labelStyle") };
            ltbKva50Label.Content = "LTB Kva 50";

            TextBox kva50TextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle") };

            LtbKva50Panel.Children.Add(ltbKva50Label);
            LtbKva50Panel.Children.Add(kva50TextBox);

            Grid.SetRow(LtbKva50Panel, 4);


            WrapPanel LTPkVA60HZPanel = new WrapPanel();

            Label ltbKva60Label = new Label() { Style = (Style)FindResource("labelStyle") };
            ltbKva60Label.Content = "LTB Kva 60";

            TextBox kva60TextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle") };

            LTPkVA60HZPanel.Children.Add(ltbKva60Label);
            LTPkVA60HZPanel.Children.Add(kva60TextBox);

            Grid.SetRow(LTPkVA60HZPanel, 5);


            WrapPanel PRPkVA50HZPanel = new WrapPanel();

            Label prpKva50Label = new Label() { Style = (Style)FindResource("labelStyle") };
            prpKva50Label.Content = "PRP Kva 50";

            TextBox prpkva50TextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle") };

            PRPkVA50HZPanel.Children.Add(prpKva50Label);
            PRPkVA50HZPanel.Children.Add(prpkva50TextBox);

            Grid.SetRow(PRPkVA50HZPanel, 6);


            WrapPanel PRPkVA60HZPanel = new WrapPanel();

            Label prpKva60Label = new Label() { Style = (Style)FindResource("labelStyle") };
            prpKva60Label.Content = "PRP Kva 60";

            TextBox prpkva60TextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle") };

            PRPkVA60HZPanel.Children.Add(prpKva60Label);
            PRPkVA60HZPanel.Children.Add(prpkva60TextBox);

            Grid.SetRow(PRPkVA60HZPanel, 7);



            WrapPanel MinPanel = new WrapPanel();

            Label MinLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            MinLabel.Content = "Min";

            TextBox MinTextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle") };

            MinPanel.Children.Add(MinLabel);
            MinPanel.Children.Add(MinTextBox);

            Grid.SetRow(MinPanel, 8);



            WrapPanel MidPanel = new WrapPanel();

            Label MidLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            MidLabel.Content = "AVG";

            TextBox MidTextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle") };

            MidPanel.Children.Add(MidLabel);
            MidPanel.Children.Add(MidTextBox);

            Grid.SetRow(MidPanel, 9);


            WrapPanel MaxPanel = new WrapPanel();

            Label MaxLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            MaxLabel.Content = "Max";

            TextBox MaxTextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle") };

            MaxPanel.Children.Add(MaxLabel);
            MaxPanel.Children.Add(MaxTextBox);

            Grid.SetRow(MaxPanel, 10);


            WrapPanel CoolingPanel = new WrapPanel();

            Label CoolingLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            CoolingLabel.Content = "Cooling";

            TextBox coolingTextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle") };

            CoolingPanel.Children.Add(CoolingLabel);
            CoolingPanel.Children.Add(coolingTextBox);

            Grid.SetRow(CoolingPanel, 11);


            WrapPanel TankPanel = new WrapPanel();

            Label TankLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            TankLabel.Content = "TANK";

            TextBox TankTextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle") };

            TankPanel.Children.Add(TankLabel);
            TankPanel.Children.Add(TankTextBox);

            Grid.SetRow(TankPanel, 12);


            WrapPanel LOADPanel = new WrapPanel();

            Label LoadLabel = new Label() { Style = (Style)FindResource("labelStyle") };
            LoadLabel.Content = "LOAD";

            TextBox loadTextBox = new TextBox() { Style = (Style)FindResource("textBoxStyle") };

            LOADPanel.Children.Add(LoadLabel);
            LOADPanel.Children.Add(loadTextBox);

            Grid.SetRow(LOADPanel, 13);

            WrapPanel MainFeaturesPanel = new WrapPanel();

            Label MainFeatureLabel = new Label() { Style = (Style)FindResource("labelStyle")};
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

            Label AlternatorFeaturesLable = new Label() { Style = (Style)FindResource("labelStyle")};
            AlternatorFeaturesLable.Content = "Alternator"+'\n'+"Features";

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

            Grid.SetRow(BatteriesPanel,19);


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
            Home.Children.Add(Card);

        }





    }
}
