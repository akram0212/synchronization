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
    
    public partial class WorkOffersFilteredWithRFQSerialWindow : Window
    {
        Employee loggedInUser;
        SQLServer sqlServer;
        OutgoingQuotation outgoingQuotation;
        CommonQueries CommonQueries;

        private List<COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT> workOffers = new List<COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT> workOffersAfterFiltering = new List<COMPANY_WORK_MACROS.OUTGOING_QUOTATION_MAX_STRUCT>();

        private int salesPersonTeam;
        private int viewAddCondition;

        private Grid currentGrid;
        private Expander currentExpander;
        private Expander previousExpander;

        public WorkOffersFilteredWithRFQSerialWindow(int rfqSerial, int rfqVersion, int salesPersonId, ref Employee mLoggedInUser)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;

            sqlServer = new SQLServer();
            CommonQueries = new CommonQueries(sqlServer);
            workOffer = new WorkOffer(sqlServer);

            if (!CommonQueries.GetOutgoingQuotations(ref workOffers, rfqSerial, rfqVersion, salesPersonId))
                return;

            SetWorkOffersStackPanel();

        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool SetWorkOffersStackPanel()
        {
            workOffersStackPanel.Children.Clear();

            workOffersAfterFiltering.Clear();

            for (int i = 0; i < workOffers.Count; i++)
            {
               
                StackPanel fullStackPanel = new StackPanel();
                fullStackPanel.Orientation = Orientation.Vertical;
                //fullStackPanel.Height = 90;

                workOffersAfterFiltering.Add(workOffers[i]);

                Label offerIdLabel = new Label();
                offerIdLabel.Content = workOffers[i].offer_id;
                offerIdLabel.Style = (Style)FindResource("stackPanelItemHeader");

                Label salesLabel = new Label();
                salesLabel.Content = workOffers[i].sales_person_name;
                salesLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label preSalesLabel = new Label();
                preSalesLabel.Content = workOffers[i].offer_proposer_name;
                preSalesLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label companyAndContactLabel = new Label();
                companyAndContactLabel.Content = workOffers[i].company_name + " -" + workOffers[i].contact_name;
                companyAndContactLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label productTypeAndBrandLabel = new Label();
                List<COMPANY_WORK_MACROS.OUTGOING_QUOTATION_PRODUCT_STRUCT> temp = workOffers[i].products;

                for (int j = 0; j < temp.Count(); j++)
                {
                    COMPANY_WORK_MACROS.PRODUCT_STRUCT tempType1 = temp[j].productType;
                    COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand1 = temp[j].productBrand;

                    productTypeAndBrandLabel.Content += tempType1.typeName + " -" + tempBrand1.brandName;

                    if (j != temp.Count() - 1)
                        productTypeAndBrandLabel.Content += ", ";
                }
                productTypeAndBrandLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label contractTypeLabel = new Label();
                contractTypeLabel.Content = workOffers[i].contract_type;
                contractTypeLabel.Style = (Style)FindResource("stackPanelItemBody");

                Border borderIcon = new Border();
                borderIcon.Style = (Style)FindResource("BorderIcon");

                Label statusLabel = new Label();
                statusLabel.Content = workOffers[i].offer_status;
                statusLabel.Style = (Style)FindResource("BorderIconTextLabel");

                if (workOffers[i].offer_status_id == COMPANY_WORK_MACROS.PENDING_WORK_OFFER)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500"));
                }
                else if (workOffers[i].offer_status_id == COMPANY_WORK_MACROS.CONFIRMED_WORK_OFFER)
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008000"));
                }
                else
                {
                    borderIcon.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                }

                borderIcon.Child = statusLabel;

                Expander expander = new Expander();
                expander.ExpandDirection = ExpandDirection.Down;
                expander.VerticalAlignment = VerticalAlignment.Center;
                expander.HorizontalAlignment = HorizontalAlignment.Center;
                expander.HorizontalContentAlignment = HorizontalAlignment.Center;
                expander.Expanded += new RoutedEventHandler(OnExpandExpander);
                expander.Collapsed += new RoutedEventHandler(OnCollapseExpander);

                ListBox listBox = new ListBox();
                listBox.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                listBox.SelectionChanged += new SelectionChangedEventHandler(OnSelChangedListBox);

                ListBoxItem viewButton = new ListBoxItem();
                viewButton.Content = "View";
                viewButton.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));

                ListBoxItem reviseButton = new ListBoxItem();
                reviseButton.Content = "Revise Offer";
                reviseButton.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));

                listBox.Items.Add(viewButton);

                if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID)
                    listBox.Items.Add(reviseButton);

                expander.Content = listBox;

                fullStackPanel.Children.Add(offerIdLabel);
                fullStackPanel.Children.Add(salesLabel);
                fullStackPanel.Children.Add(preSalesLabel);
                fullStackPanel.Children.Add(companyAndContactLabel);
                fullStackPanel.Children.Add(productTypeAndBrandLabel);
                fullStackPanel.Children.Add(contractTypeLabel);

                Grid grid = new Grid();
                ColumnDefinition column1 = new ColumnDefinition();
                ColumnDefinition column2 = new ColumnDefinition();
                ColumnDefinition column3 = new ColumnDefinition();
                column2.MaxWidth = 95;
                column3.MaxWidth = 50;

                grid.ColumnDefinitions.Add(column1);
                grid.ColumnDefinitions.Add(column2);
                grid.ColumnDefinitions.Add(column3);

                grid.Children.Add(fullStackPanel);
                grid.Children.Add(borderIcon);
                grid.Children.Add(expander);

                Grid.SetColumn(fullStackPanel, 0);
                Grid.SetColumn(borderIcon, 1);
                Grid.SetColumn(expander, 2);

                workOffersStackPanel.Children.Add(grid);
            }

            return true;
        }

        private void OnExpandExpander(object sender, RoutedEventArgs e)
        {
            if (currentExpander != null)
                previousExpander = currentExpander;

            currentExpander = (Expander)sender;

            if (previousExpander != currentExpander && previousExpander != null)
                previousExpander.IsExpanded = false;

            Grid currentGrid = (Grid)currentExpander.Parent;
            ColumnDefinition expanderColumn = currentGrid.ColumnDefinitions[2];
            //expanderColumn.Width = new GridLength(Width = 120);
            currentExpander.VerticalAlignment = VerticalAlignment.Top;
            expanderColumn.MaxWidth = 120;
        }

        private void OnCollapseExpander(object sender, RoutedEventArgs e)
        {
            Expander currentExpander = (Expander)sender;
            Grid currentGrid = (Grid)currentExpander.Parent;
            ColumnDefinition expanderColumn = currentGrid.ColumnDefinitions[2];
            currentExpander.VerticalAlignment = VerticalAlignment.Center;
            expanderColumn.MaxWidth = 50;
        }

        private void OnSelChangedListBox(object sender, SelectionChangedEventArgs e)
        {
            ListBox tempListBox = (ListBox)sender;
            ListBoxItem currentItem = (ListBoxItem)tempListBox.Items[tempListBox.SelectedIndex];
            Expander currentExpander = (Expander)tempListBox.Parent;

            currentGrid = (Grid)currentExpander.Parent;

            if (currentItem.Content.ToString() == "View")
            {
                OnBtnClickView();
            }
            else if (currentItem.Content.ToString() == "Revise Offer")
            {
                OnBtnClickReviseOffer();
            }
        }

        private void OnBtnClickView()
        {
            viewAddCondition = COMPANY_WORK_MACROS.OUTGOING_QUOTATION_VIEW_CONDITION;

            CommonQueries.GetEmployeeTeam(workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].sales_person_id, ref salesPersonTeam);


            if (salesPersonTeam == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
            {
                outgoingQuotation.InitializeSalesWorkOfferInfo(workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_serial,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_version,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_proposer_id);
            }
            else
            {
                outgoingQuotation.InitializeTechnicalOfficeWorkOfferInfo(workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_serial,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_version,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_proposer_id);
            }


            WorkOfferWindow viewOffer = new WorkOfferWindow(ref loggedInUser, ref outgoingQuotation, viewAddCondition, false);
            viewOffer.Show();
        }

        private void OnBtnClickReviseOffer()
        {
            viewAddCondition = COMPANY_WORK_MACROS.OUTGOING_QUOTATION_REVISE_CONDITION;

            CommonQueries.GetEmployeeTeam(workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].sales_person_id, ref salesPersonTeam);

            if (salesPersonTeam == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
            {
                outgoingQuotation.InitializeSalesWorkOfferInfo(workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_serial,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_version,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_proposer_id);
            }
            else
            {
                outgoingQuotation.InitializeTechnicalOfficeWorkOfferInfo(workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_serial,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_version,
                                                                workOffersAfterFiltering[workOffersStackPanel.Children.IndexOf(currentGrid)].offer_proposer_id);
            }


            WorkOfferWindow reviseOffer = new WorkOfferWindow(ref loggedInUser, ref outgoingQuotation, viewAddCondition, false);
            reviseOffer.Closed += OnClosedReviseOffer;
            reviseOffer.Show();
        }

        private void OnClosedReviseOffer(object sender, EventArgs e)
        {
            SetWorkOffersStackPanel();
        }
    }

}
