using _01electronics_erp;
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
    /// Interaction logic for ContactsPage.xaml
    /// </summary>
    public partial class ContactsPage : Page
    {
        private String sqlQuery;

        private SQLServer initializationObject;

        TextBlock selectedTextBlock;
        private List<TextBlock> AllTextBlocks = new List<TextBlock>();

        CommonQueries commonQueries;
        private Employee loggedInUser;
        private int employeeID;
        List<COMPANY_ORGANISATION_MACROS.CONTACT_PRO_STRUCT> employeeContacts;
        List<KeyValuePair<long, string>> company_name;

        public struct Contact_struct
        {
            public int salesPersonID;
            public long contactID;
            public long branchID;
            public string contact_name;
        };

        List<Contact_struct> contactInfo = new List<Contact_struct>();
        
        public ContactsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();
            initializationObject = new SQLServer();
            employeeContacts = new List<COMPANY_ORGANISATION_MACROS.CONTACT_PRO_STRUCT>();
            company_name = new List<KeyValuePair<long, string>>();
            commonQueries = new CommonQueries();
            loggedInUser = mLoggedInUser;
            employeeID = mLoggedInUser.GetEmployeeId();

            commonQueries.GetEmployeeCompanies(employeeID, ref company_name);
          
            CompanyNameCombo.Items.Clear();
            for (int i = company_name.Count - 1; i >= 0; i--)
            {
                if (i > 0 && company_name[i].Key == company_name[i - 1].Key)
                {
                    company_name.Remove(new KeyValuePair<long, string>(company_name[i].Key, company_name[i].Value));
                }
            }

            for (int i = 0; i < company_name.Count; i++)
            {
                CompanyNameCombo.Items.Add(company_name[i].Value);
            }
            commonQueries.GetCompanyContacts(employeeID, ref employeeContacts);
            //    contact_name.Clear();
            //    for (int i = 0; i < initializationObject.rows.Count; i++)
            //    {
            //        contact_name.Add(new KeyValuePair<long, string>(initializationObject.rows[i].sql_int[1], initializationObject.rows[i].sql_string[1]));
            //    }

            //    for (int i = 0; i < contact_name.Count(); i++)
            //    {
            //        var getcontactInfo = new Contact_struct();
            //        getcontactInfo.salesPersonID = employeeID;
            //        getcontactInfo.contactID = contact_name[i].Key;
            //        getcontactInfo.branchID = company_name[i].Key;
            //        getcontactInfo.contact_name = contact_name[i].Value;
            //        contactInfo.Add(getcontactInfo);
            //    }

            //for (int i = 0; i < employeeContacts.Count; i++)
            //{
            //    RowDefinition rowDef = new RowDefinition();
            //    rowDef.Height = new GridLength(30);

            //    NameGrid.RowDefinitions.Insert(i, rowDef);
            //    TextBlock TitleTextblock = new TextBlock();
            //    TitleTextblock.Name = "company_nameText";

            //    if (employeeContacts[i].company.company_name == " ")
            //    {
            //        TitleTextblock.Text = "NONE";
            //    }
            //    else if (i>0 && employeeContacts[i-1].company.company_name == employeeContacts[i].company.company_name)
            //    {
            //        TitleTextblock.Text = " ";
            //    }
            //    else
            //    {
            //        TitleTextblock.Text = employeeContacts[i].company.company_name;
            //        AllTextBlocks.Add(TitleTextblock);
            //    }
            //    TitleTextblock.Margin = new Thickness(10, 5, 5, 0);
            //    TitleTextblock.Padding = new Thickness(40, 5, 5, 0);
            //    TitleTextblock.FontSize = 14;
            //    TitleTextblock.MouseDown += handlerfunction;
            //    TitleTextblock.TextAlignment = TextAlignment.Center;
            //    TitleTextblock.TextWrapping = TextWrapping.Wrap;
            //    TitleTextblock.FontWeight = FontWeights.Bold;
            //    Grid.SetColumn(TitleTextblock, 0);
            //    Grid.SetRow(TitleTextblock, i);
            //    NameGrid.Children.Add(TitleTextblock);

            //    RowDefinition rowDef2 = new RowDefinition();
            //    rowDef2.Height = new GridLength(30);

            //    NameGrid.RowDefinitions.Insert(i, rowDef2);
            //    TextBlock TitleTextblock2 = new TextBlock();
            //    TitleTextblock2.Name = "contact_nameText";
            //    if (employeeContacts[i].contact_name == " ")
            //    {
            //        TitleTextblock2.Text = "NONE";
            //    }
            //    else
            //    {
            //        TitleTextblock2.Text = employeeContacts[i].contact_name;
            //        AllTextBlocks.Add(TitleTextblock2);
            //    }
            //    TitleTextblock2.MouseDown += handlerfunction;
            //    TitleTextblock2.Margin = new Thickness(10, 5, 5, 0);
            //    TitleTextblock2.Padding = new Thickness(40, 5, 5, 0);
            //    TitleTextblock2.FontSize = 14;
            //    TitleTextblock2.TextAlignment = TextAlignment.Center;
            //    TitleTextblock2.TextWrapping = TextWrapping.Wrap;
            //    Grid.SetColumn(TitleTextblock2, 1);
            //    Grid.SetRow(TitleTextblock2, i);
            //    NameGrid.Children.Add(TitleTextblock2);

            //if (contact_name.Count > company_name.Count)
            //{
            //for (int j = 1; j < employeeContacts.Count; j++)
            //{
            //    RowDefinition rowDef3 = new RowDefinition();
            //    rowDef2.Height = new GridLength(30);

            //    TextBlock TitleTextblock3 = new TextBlock();
            //    TitleTextblock3.Name = "contact_nameText";

            //    if (employeeContacts[j].contact_name == " ")
            //    {
            //        TitleTextblock3.Text = "NONE";
            //    }
            //    else
            //    {
            //        TitleTextblock3.Text = employeeContacts[j].contact_name;
            //        AllTextBlocks.Add(TitleTextblock3);
            //    }

            //    TitleTextblock3.Margin = new Thickness(10, 5, 5, 0);
            //    TitleTextblock3.Padding = new Thickness(40, 5, 5, 0);
            //    TitleTextblock3.FontSize = 14;
            //    TitleTextblock3.TextAlignment = TextAlignment.Center;
            //    TitleTextblock3.TextWrapping = TextWrapping.Wrap;
            //    Grid.SetColumn(TitleTextblock3, 1);
            //    Grid.SetRow(TitleTextblock3, j);
            //    NameGrid.Children.Add(TitleTextblock3);

            //    RowDefinition rowDef4 = new RowDefinition();
            //    rowDef4.Height = new GridLength(30);
            //    NameGrid.RowDefinitions.Insert(j, rowDef4);
            //    TextBlock TitleTextblock4 = new TextBlock();
            //    TitleTextblock4.Text = "";
            //    Grid.SetColumn(TitleTextblock4, 0);
            //    Grid.SetRow(TitleTextblock4, j);
            //    NameGrid.Children.Add(TitleTextblock4);
            //}

            //  }

            //}

            //}

        }

        public void handlerfunction(object sender, RoutedEventArgs e)
        {
            selectedTextBlock = (TextBlock)sender;
            for (int i = 0; i < AllTextBlocks.Count; i++)
            {
                BrushConverter brush = new BrushConverter();
                AllTextBlocks[i].Foreground = (Brush)brush.ConvertFrom("Black");
            }
            BrushConverter brushStyle = new BrushConverter();
            selectedTextBlock.Foreground = (Brush)brushStyle.ConvertFrom("#FF105A97");
        }

        private void OnButtonClickedOrders(object sender, RoutedEventArgs e)
        {
            WorkOrdersPage workOrdersPage = new WorkOrdersPage(ref loggedInUser);
            this.NavigationService.Navigate(workOrdersPage);
        }
        private void OnButtonClickedOffers(object sender, RoutedEventArgs e)
        {
            WorkOffersPage workOffersPage = new WorkOffersPage(ref loggedInUser);
            this.NavigationService.Navigate(workOffersPage);
        }
        private void OnButtonClickedRFQs(object sender, RoutedEventArgs e)
        {
            RFQsPage rFQsPage = new RFQsPage(ref loggedInUser);
            this.NavigationService.Navigate(rFQsPage);
        }
        private void OnButtonClickedVisits(object sender, RoutedEventArgs e)
        {
            ClientVisitsPage clientVisitsPage = new ClientVisitsPage(ref loggedInUser);
            this.NavigationService.Navigate(clientVisitsPage);
        }
        private void OnButtonClickedCalls(object sender, RoutedEventArgs e)
        {
            ClientCallsPage clientCallsPage = new ClientCallsPage(ref loggedInUser);
            this.NavigationService.Navigate(clientCallsPage);
        }
        private void OnButtonClickedMeetings(object sender, RoutedEventArgs e)
        {
            OfficeMeetingsPage officeMeetingsPage = new OfficeMeetingsPage(ref loggedInUser);
            this.NavigationService.Navigate(officeMeetingsPage);
        }
        private void OnButtonClickedStatistics(object sender, RoutedEventArgs e)
        {
            StatisticsPage statisticsPage = new StatisticsPage(ref loggedInUser);
            this.NavigationService.Navigate(statisticsPage);
        }

        private void OnButtonClickedMyProfile(object sender, MouseButtonEventArgs e)
        {
            UserPortalPage userPortal = new UserPortalPage(ref loggedInUser);
            this.NavigationService.Navigate(userPortal);
        }

        private void OnButtonClickedContacts(object sender, MouseButtonEventArgs e)
        {
            ContactsPage contactsPage = new ContactsPage(ref loggedInUser);
            this.NavigationService.Navigate(contactsPage);
        }

        private void ViewClick(object sender, RoutedEventArgs e)
        {

            //for (int i = 0; i < AllTextBlocks.Count; i++)
            //{
            //    BrushConverter brush = new BrushConverter();
            //    AllTextBlocks[i].Foreground = (Brush)brush.ConvertFrom("Black");

            //}

            //if (selectedTextBlock == null)
            //{
            //    MessageBox.Show("Please select a Contact to view/modify his Info!", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Information);
            //}

            //else
            //{
            //    for(int i = company_name.Count - 1; i >= 0 ; i--)
            //    {
            //        if(i > 0 && company_name[i].Key == company_name[i-1].Key)
            //        {
            //            company_name.Remove(new KeyValuePair<long, string>(company_name[i].Key, company_name[i].Value));
            //        }
            //    }

            //    long companyID, contactID;
            //    string SelectedItem;
            //    SelectedItem = selectedTextBlock.Text;

            //    if (selectedTextBlock == null)
            //    {
            //        MessageBox.Show("Please select a Contact to view/modify his Info!", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Information);
            //    }

            //    else 
            //    {
            //        KeyValuePair<long, string> company = company_name.SingleOrDefault(x => x.Value == SelectedItem);

            //        companyID = company.Key;
            //        if (company.Value != null)
            //        {

            //            ViewCompanyInfoWindow dialog = new ViewCompanyInfoWindow(companyID);
            //            dialog.ShowDialog();

            //        }
            //        else
            //        {
            //            var getcontactInfo = new Contact_struct();
            //            foreach (var kvp in contactInfo)
            //            {
            //                if (kvp.contact_name == SelectedItem)
            //                {
            //                    companyID = kvp.branchID;
            //                    contactID = kvp.contactID;
            //                    getcontactInfo.salesPersonID = employeeID;
            //                    getcontactInfo.contactID = contactID;
            //                    getcontactInfo.branchID = companyID;
            //                    ViewContactInfoWindow dialog = new ViewContactInfoWindow(contactID, loggedInUser, companyID);
            //                    dialog.ShowDialog();
            //                }
    
            //            }
            //        }
            //    }
            //}
        }

        private void CompanyNameCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i < NameGrid.RowDefinitions.Count; i++)
            {
                NameGrid.RowDefinitions.RemoveAt(i);
            }
            NameGrid.Children.Clear();

            string companyName = (string)CompanyNameCombo.SelectedItem;
            long companyID = 0;

            for(int i = 0; i < company_name.Count; i++)
            {
                if(company_name[i].Value == companyName)
                {
                    companyID = company_name[i].Key;
                }
            }

            commonQueries.GetCompanyContacts((int)companyID, ref employeeContacts);

            for (int i = 0; i < company_name.Count; i++)
            {
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(30);

                NameGrid.RowDefinitions.Insert(i, rowDef);
                TextBlock TitleTextblock = new TextBlock();
                TitleTextblock.Name = "CompanyNameText";

                TitleTextblock.Text = companyName;
                
                TitleTextblock.Margin = new Thickness(10, 5, 5, 0);
                TitleTextblock.Padding = new Thickness(40, 5, 5, 0);
                TitleTextblock.FontSize = 14;
                TitleTextblock.TextAlignment = TextAlignment.Center;
                TitleTextblock.TextWrapping = TextWrapping.Wrap;
                Grid.SetColumn(TitleTextblock, 0);
                Grid.SetRow(TitleTextblock, i);
                NameGrid.Children.Add(TitleTextblock);

                RowDefinition rowDef2 = new RowDefinition();
                rowDef2.Height = new GridLength(30);

                NameGrid.RowDefinitions.Insert(i, rowDef2);
                TextBlock TitleTextblock2 = new TextBlock();
                TitleTextblock2.Name = "ContactNameText";
                if (employeeContacts[i].contact_name == " ")
                {
                    TitleTextblock2.Text = "NONE";
                }
                else
                {
                    TitleTextblock2.Text = employeeContacts[i].contact_name;
                
                TitleTextblock2.Margin = new Thickness(10, 5, 5, 0);
                TitleTextblock2.Padding = new Thickness(40, 5, 5, 0);
                TitleTextblock2.FontSize = 14;
                TitleTextblock2.TextAlignment = TextAlignment.Center;
                TitleTextblock2.TextWrapping = TextWrapping.Wrap;
                Grid.SetColumn(TitleTextblock2, 1);
                Grid.SetRow(TitleTextblock2, i);
                NameGrid.Children.Add(TitleTextblock2);
                    if (employeeContacts.Count > company_name.Count)
                    {
                        for (int j = 1; j < employeeContacts.Count; j++)
                        {
                            RowDefinition rowDef3 = new RowDefinition();
                            rowDef2.Height = new GridLength(30);

                            TextBlock TitleTextblock3 = new TextBlock();
                            TitleTextblock3.Name = "ContactNameText";

                            if (employeeContacts[j].contact_name == " ")
                            {
                                TitleTextblock3.Text = "NONE";
                            }
                            else if (employeeContacts[j].contact_name != employeeContacts[j - 1].contact_name)
                            {
                                TitleTextblock3.Text = employeeContacts[j].contact_name;

                            }
                            else
                            {
                                continue;
                            }

                            TitleTextblock3.Margin = new Thickness(10, 5, 5, 0);
                            TitleTextblock3.Padding = new Thickness(40, 5, 5, 0);
                            TitleTextblock3.FontSize = 14;
                            TitleTextblock3.TextAlignment = TextAlignment.Center;
                            TitleTextblock3.TextWrapping = TextWrapping.Wrap;
                            Grid.SetColumn(TitleTextblock3, 1);
                            Grid.SetRow(TitleTextblock3, j);
                            NameGrid.Children.Add(TitleTextblock3);

                            RowDefinition rowDef4 = new RowDefinition();
                            rowDef4.Height = new GridLength(30);
                            NameGrid.RowDefinitions.Insert(j, rowDef4);
                            TextBlock TitleTextblock4 = new TextBlock();
                            TitleTextblock4.Text = "";
                            Grid.SetColumn(TitleTextblock4, 0);
                            Grid.SetRow(TitleTextblock4, j);
                            NameGrid.Children.Add(TitleTextblock4);
                        }

                    }

                }

            }
        }
    }
}
