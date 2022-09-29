using _01electronics_library;
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
using System.Windows.Shapes;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for MaintContractHistoryWindow.xaml
    /// </summary>
    public partial class MaintContractHistoryWindow : Window
    {
        private Employee loggedInUser;
        private SQLServer sqlDatabase;
        private CommonQueries commonQueriesObject;

        private int selectedContractSerial;
        private List<COMPANY_WORK_MACROS.MAINTENANCE_CONTRACT_HISTORY_STRUCT> contractHistory;
        public MaintContractHistoryWindow(ref Employee mLoggedInUser, int ContractSerial)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;
            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();

            selectedContractSerial = ContractSerial;
            contractHistory = new List<COMPANY_WORK_MACROS.MAINTENANCE_CONTRACT_HISTORY_STRUCT>();

            if (!commonQueriesObject.GetMaintContractHistory(selectedContractSerial, ref contractHistory))
                return;

            SetMaintContractsGrid();
        }

        private bool SetMaintContractsGrid()
        {

            maintContractsGrid.Children.Clear();
            maintContractsGrid.RowDefinitions.Clear();
            maintContractsGrid.ColumnDefinitions.Clear();

            int currentRowNumber = 0;

            for (int i = 0; i < contractHistory.Count; i++)
            {
                RowDefinition currentRow = new RowDefinition();
                maintContractsGrid.RowDefinitions.Add(currentRow);

                Label issueDateLabel = new Label();
                issueDateLabel.Content = contractHistory[i].issue_date;
                issueDateLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(issueDateLabel, currentRowNumber);
                Grid.SetColumn(issueDateLabel, 0);
                maintContractsGrid.Children.Add(issueDateLabel);


                Label contractPeriodLabel = new Label();
                contractPeriodLabel.Content = contractHistory[i].contract_period.ToString() + "-" + contractHistory[i].contract_period_time_unit + "-" + contractHistory[i].contract_period_condition_unit;
                contractPeriodLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(contractPeriodLabel, currentRowNumber);
                Grid.SetColumn(contractPeriodLabel, 1);
                maintContractsGrid.Children.Add(contractPeriodLabel);

                Grid productGrid = new Grid();
                productGrid.ShowGridLines = true;

                productGrid.ColumnDefinitions.Add(new ColumnDefinition());
                productGrid.ColumnDefinitions.Add(new ColumnDefinition());
                productGrid.ColumnDefinitions.Add(new ColumnDefinition());
                productGrid.ColumnDefinitions.Add(new ColumnDefinition());

                productGrid.RowDefinitions.Add(new RowDefinition());


                Label rowColumnHeader = new Label();
                rowColumnHeader.Style = (Style)FindResource("tableSubHeaderItem");

                productGrid.Children.Add(rowColumnHeader);
                Grid.SetRow(rowColumnHeader, 0);
                Grid.SetColumn(rowColumnHeader, 0);

                Label typeHeader = new Label();
                typeHeader.Content = "Type";
                typeHeader.Style = (Style)FindResource("tableSubHeaderItem");

                productGrid.Children.Add(typeHeader);
                Grid.SetRow(typeHeader, 0);
                Grid.SetColumn(typeHeader, 1);


                Label brandHeader = new Label();
                brandHeader.Content = "Brand";
                brandHeader.Style = (Style)FindResource("tableSubHeaderItem");

                productGrid.Children.Add(brandHeader);
                Grid.SetRow(brandHeader, 0);
                Grid.SetColumn(brandHeader, 2);


                Label modelHeader = new Label();
                modelHeader.Content = "Model";
                modelHeader.Style = (Style)FindResource("tableSubHeaderItem");

                productGrid.Children.Add(modelHeader);
                Grid.SetRow(modelHeader, 0);
                Grid.SetColumn(modelHeader, 3);


                List<COMPANY_WORK_MACROS.ORDER_PRODUCT_STRUCT> temp = contractHistory[i].products_summary;

                for (int j = 0; j < temp.Count(); j++)
                {
                    COMPANY_WORK_MACROS.PRODUCT_STRUCT tempType1 = temp[j].productType;
                    COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand1 = temp[j].productBrand;
                    COMPANY_WORK_MACROS.MODEL_STRUCT tempModel1 = temp[j].productModel;

                    if (tempType1.typeId == 0)
                        continue;

                    productGrid.RowDefinitions.Add(new RowDefinition());

                    int tempNumber = j + 1;
                    Label productNumberHeader = new Label();
                    productNumberHeader.Content = "Product" + " " + tempNumber;
                    productNumberHeader.Style = (Style)FindResource("tableSubHeaderItem");

                    productGrid.Children.Add(productNumberHeader);
                    Grid.SetRow(productNumberHeader, j + 1);
                    Grid.SetColumn(productNumberHeader, 0);

                    Label type = new Label();
                    type.Content = tempType1.typeName;
                    type.Style = (Style)FindResource("tableSubItemLabel");

                    productGrid.Children.Add(type);
                    Grid.SetRow(type, j + 1);
                    Grid.SetColumn(type, 1);

                    Label brand = new Label();
                    brand.Content = tempBrand1.brandName;
                    brand.Style = (Style)FindResource("tableSubItemLabel");

                    productGrid.Children.Add(brand);
                    Grid.SetRow(brand, j + 1);
                    Grid.SetColumn(brand, 2);

                    Label model = new Label();
                    model.Content = tempModel1.modelName;
                    model.Style = (Style)FindResource("tableSubItemLabel");

                    productGrid.Children.Add(model);
                    Grid.SetRow(model, j + 1);
                    Grid.SetColumn(model, 3);
                }

                maintContractsGrid.Children.Add(productGrid);
                Grid.SetRow(productGrid, currentRowNumber);
                Grid.SetColumn(productGrid, 2);

                Label totalPriceLabel = new Label();
                totalPriceLabel.Content = contractHistory[i].total_Price.ToString();
                totalPriceLabel.Style = (Style)FindResource("tableSubItemLabel");

                maintContractsGrid.Children.Add(totalPriceLabel);
                Grid.SetRow(totalPriceLabel, currentRowNumber);
                Grid.SetColumn(totalPriceLabel, 3);

                currentRowNumber++;

            }

            return true;
        }

    }
}
