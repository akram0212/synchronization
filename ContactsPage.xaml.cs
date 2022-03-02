﻿using _01electronics_library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        private CommonQueries commonQueries;
        private Employee loggedInUser;
        private int selectedEmployee;

        private StackPanel previousSelectedContactItem;
        private StackPanel currentSelectedContactItem;

        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> listOfEmployees;

        private List<BASIC_STRUCTS.PRIMARY_FIELD_STRUCT> primaryFieldsList;
        private List<BASIC_STRUCTS.SECONDARY_FIELD_STRUCT> secondaryFieldsList;

        private List<KeyValuePair<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT, List<COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT>>> employeesContacts;
        
        private List<KeyValuePair<int, TreeViewItem>> companiesTreeArray;
        private List<KeyValuePair<int, StackPanel>> companiesStackArray;

        private List<KeyValuePair<COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT, TreeViewItem>> contactsTreeArray;
        private List<KeyValuePair<COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT, StackPanel>> contactsStackArray;

        public ContactsPage(ref Employee mLoggedInUser)
        {
            initializationObject = new SQLServer();

            listOfEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();

            primaryFieldsList = new List<BASIC_STRUCTS.PRIMARY_FIELD_STRUCT>();
            secondaryFieldsList = new List<BASIC_STRUCTS.SECONDARY_FIELD_STRUCT>();

            employeesContacts = new List<KeyValuePair<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT, List<COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT>>>();
            
            companiesTreeArray = new List<KeyValuePair<int, TreeViewItem>>();
            companiesStackArray = new List<KeyValuePair<int, StackPanel>>();

            contactsTreeArray = new List<KeyValuePair<COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT, TreeViewItem>>();
            contactsStackArray = new List<KeyValuePair<COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT, StackPanel>>();

            commonQueries = new CommonQueries();
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            companyNameTextBox.IsEnabled = false;
            contactNameTextBox.IsEnabled = false;
            primaryFieldComboBox.IsEnabled = false;
            secondaryFieldComboBox.IsEnabled = false;
            salesPersonComboBox.IsEnabled = false;

            secondaryFieldCheckBox.IsEnabled = false;
            salesPersonCheckBox.IsEnabled = true;

            ViewBtn.IsEnabled = false;

            InitializePrimaryFieldComboBox();

            if (!CheckEmployeePosition())
                return;
            if (!InitializeSalesPersonComboBox())
                return;

            GetAllContacts();

            SetDefaultSettings();

            InitializeContactsTree();
            InitializeContactsStackPanel();

        }
        private void SetDefaultSettings()
        {
            if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION || loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION)
            {
                salesPersonCheckBox.IsChecked = false;
                salesPersonCheckBox.IsEnabled = true;
                salesPersonComboBox.IsEnabled = false;
            }
            else
            {
                salesPersonCheckBox.IsChecked = true;
                salesPersonCheckBox.IsEnabled = false;
                salesPersonComboBox.IsEnabled = false;
            }
        }
        private bool InitializeSalesPersonComboBox()
        {
            salesPersonComboBox.Items.Clear();

            for (int i = 0; i < listOfEmployees.Count; i++)
                salesPersonComboBox.Items.Add(listOfEmployees[i].employee_name);

            return true;
        }
        
        private bool CheckEmployeePosition()
        {
            listOfEmployees.Clear();

            if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)
            {
                if (!commonQueries.GetDepartmentEmployees(loggedInUser.GetEmployeeDepartmentId(), ref listOfEmployees))
                    return false;
            }
            else
            {
                if (!commonQueries.GetTeamEmployees(loggedInUser.GetEmployeeTeamId(), ref listOfEmployees))
                    return false;
            }

            InitializeSalesPersonComboBox();

            return true;
        }
        
        public bool GetAllContacts()
        {
            employeesContacts.Clear();

            for (int i = 0; i < listOfEmployees.Count; i++)
            {
                List<COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT> employeeContactList = new List<COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT>();
                commonQueries.GetEmployeeContacts(listOfEmployees[i].employee_id, ref employeeContactList);

                 employeesContacts.Add(new KeyValuePair<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT, List<COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT>>(listOfEmployees[i], employeeContactList));
                
            }

            return true;
        }
        private void SetEmployeeComboBox()
        {
            //salesPersonComboBox.SelectedIndex = 0;

            for (int i = 0; i < listOfEmployees.Count; i++)
                if (loggedInUser.GetEmployeeId() == listOfEmployees[i].employee_id)
                    salesPersonComboBox.SelectedIndex = i;
        }

    
        public bool InitializePrimaryFieldComboBox()
        {
            primaryFieldComboBox.Items.Clear();

            if (!commonQueries.GetPrimaryWorkFields(ref primaryFieldsList))
                return false;

            for (int i = 0; i < primaryFieldsList.Count; i++)
                primaryFieldComboBox.Items.Add(primaryFieldsList[i].field_name);

            return true;
        }
        public bool InitializeSecondaryFieldComboBox()
        {
            secondaryFieldComboBox.Items.Clear();
            if (primaryFieldComboBox.SelectedItem != null)
            {
                if (!commonQueries.GetSecondaryWorkFields(primaryFieldsList[primaryFieldComboBox.SelectedIndex].field_id, ref secondaryFieldsList))
                    return false;

                for (int i = 0; i < secondaryFieldsList.Count; i++)
                    secondaryFieldComboBox.Items.Add(secondaryFieldsList[i].field_name);
            }

            return true;
        }

   
        public bool InitializeContactsTree()
        {
            contactsTreeArray.Clear();

            for (int i = 0; i < employeesContacts.Count(); i++)
            {
                if (salesPersonComboBox.SelectedItem != null && listOfEmployees[salesPersonComboBox.SelectedIndex].employee_id != employeesContacts[i].Key.employee_id)
                    continue;

                TreeViewItem salesPersonItem = new TreeViewItem();

                salesPersonItem.Header = employeesContacts[i].Key.employee_name;
                salesPersonItem.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                salesPersonItem.FontSize = 14;
                salesPersonItem.FontWeight = FontWeights.SemiBold;
                salesPersonItem.FontFamily = new FontFamily("Sans Serif");
                salesPersonItem.Tag = employeesContacts[i].Key.employee_id;

                contactTreeView.Items.Add(salesPersonItem);

                for (int j = 0; j < employeesContacts[i].Value.Count; j++)
                {
                    TreeViewItem companyTreeItem = new TreeViewItem();

                    bool containsCompanyName = employeesContacts[i].Value[j].company.company_name.IndexOf(companyNameTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (companyNameCheckBox.IsChecked == true && companyNameTextBox.Text != null && !containsCompanyName)
                        continue;

                    if (!companiesTreeArray.Exists(company_item => company_item.Key == employeesContacts[i].Value[j].company.company_serial))
                    {
                        companyTreeItem.Header = employeesContacts[i].Value[j].company.company_name;
                        companyTreeItem.Tag = employeesContacts[i].Value[j].company.company_serial;
                        companyTreeItem.FontSize = 13;
                        companyTreeItem.FontWeight = FontWeights.SemiBold;

                        salesPersonItem.Items.Add(companyTreeItem);

                        companiesTreeArray.Add(new KeyValuePair<int, TreeViewItem>(employeesContacts[i].Value[j].company.company_serial, companyTreeItem));
                    }

                    bool containsContactName = employeesContacts[i].Value[j].contact.contact_name.IndexOf(contactNameTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (contactNameCheckBox.IsChecked == true && contactNameTextBox.Text != null && !containsContactName)
                        continue;

                    TreeViewItem contactTreeItem = new TreeViewItem();

                    contactTreeItem.Header = employeesContacts[i].Value[j].contact.contact_name;
                    contactTreeItem.Tag = employeesContacts[i].Value[j].contact.contact_id;
                    contactTreeItem.FontSize = 12;
                    contactTreeItem.FontWeight = FontWeights.Normal;

                    companyTreeItem = companiesTreeArray.Find(company_item => company_item.Key == employeesContacts[i].Value[j].company.company_serial).Value;

                    companyTreeItem.Items.Add(contactTreeItem);

                    contactsTreeArray.Add(new KeyValuePair<COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT, TreeViewItem>(employeesContacts[i].Value[j], contactTreeItem));

                }

            }

            return true;
        }
        public void InitializeContactsStackPanel()
        {
            contactStackView.Children.Clear();

            currentSelectedContactItem = null;
            previousSelectedContactItem = null;

            contactsStackArray.Clear();

            for (int i = 0; i < listOfEmployees.Count(); i++)
            {
                if (salesPersonComboBox.SelectedItem != null && listOfEmployees[salesPersonComboBox.SelectedIndex].employee_id != listOfEmployees[i].employee_id)
                    continue;

                StackPanel employeeStackPanel = new StackPanel();
                employeeStackPanel.Orientation = Orientation.Vertical;

                Label employeeNameLabel = new Label();

                employeeNameLabel.Content = listOfEmployees[i].employee_name;
                employeeNameLabel.Style = (Style)FindResource("stackPanelItemHeader");

                employeeStackPanel.Children.Add(employeeNameLabel);
                contactStackView.Children.Add(employeeStackPanel);

                for (int j = 0; j < employeesContacts[i].Value.Count; j++)
                {
                    StackPanel companyStackPanel = new StackPanel();
                    companyStackPanel.Margin = new Thickness(12, 0, 0, 0);

                    bool containsCompanyName = employeesContacts[i].Value[j].company.company_name.IndexOf(companyNameTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (companyNameCheckBox.IsChecked == true && companyNameTextBox.Text != null && !containsCompanyName)
                        continue;

                    if (!companiesStackArray.Exists(company_item => company_item.Key == employeesContacts[i].Value[j].company.company_serial))
                    {
                        StackPanel companyDetailsStackPanel = new StackPanel();
                        companyDetailsStackPanel.Orientation = Orientation.Vertical;
                        companyDetailsStackPanel.MouseDown += OnMouseDownCompanyStackPanel;

                        Grid companyDetailsGrid = new Grid();

                        ColumnDefinition companyGridIconColumn = new ColumnDefinition();
                        ColumnDefinition companyGridDetailColumn = new ColumnDefinition();

                        companyGridIconColumn.MaxWidth = 30;

                        companyDetailsGrid.ColumnDefinitions.Add(companyGridIconColumn);
                        companyDetailsGrid.ColumnDefinitions.Add(companyGridDetailColumn);

                        Label companyNameLabel = new Label();
                        companyNameLabel.Content = employeesContacts[i].Value[j].company.company_name;
                        companyNameLabel.Style = (Style)FindResource("stackPanelItemHeader");
                        companyDetailsStackPanel.Children.Add(companyNameLabel);

                        companyDetailsStackPanel.Children.Add(companyDetailsGrid);

                        Grid companyGridItem = new Grid();

                        ColumnDefinition companyIconColumn = new ColumnDefinition();
                        ColumnDefinition companyStackColumn = new ColumnDefinition();

                        companyIconColumn.MaxWidth = 50;

                        companyGridItem.ColumnDefinitions.Add(companyIconColumn);
                        companyGridItem.ColumnDefinitions.Add(companyStackColumn);

                        Image companyIcon = new Image { Source = new BitmapImage(new Uri(@"icons\company_icon.png", UriKind.Relative)) };
                        ResizeImage(ref companyIcon, 40, 40);

                        companyGridItem.Children.Add(companyIcon);
                        Grid.SetRow(companyIcon, 0);

                        companyGridItem.Children.Add(companyDetailsStackPanel);
                        Grid.SetColumn(companyDetailsStackPanel, 1);

                        companyStackPanel.Children.Add(companyGridItem);
                        employeeStackPanel.Children.Add(companyStackPanel);

                        companiesStackArray.Add(new KeyValuePair<int, StackPanel>(employeesContacts[i].Value[j].company.company_serial, companyStackPanel));
                    }

                    bool containsContactName = employeesContacts[i].Value[j].contact.contact_name.IndexOf(contactNameTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (contactNameCheckBox.IsChecked == true && contactNameTextBox.Text != null && !containsContactName)
                        continue;

                    StackPanel contactStackPanel = new StackPanel();
                    contactStackPanel.Margin = new Thickness(36,0,0,0);

                    companyStackPanel = companiesStackArray.Find(company_item => company_item.Key == employeesContacts[i].Value[j].company.company_serial).Value;

                    StackPanel contactDetailsStackPanel = new StackPanel();
                    contactDetailsStackPanel.Orientation = Orientation.Vertical;
                    contactDetailsStackPanel.MouseDown += OnMouseDownContactStackPanel;


                    Grid contactDetailsGrid = new Grid();

                    ColumnDefinition gridIconColumn = new ColumnDefinition();
                    ColumnDefinition gridDetailColumn = new ColumnDefinition();

                    gridIconColumn.MaxWidth = 30;

                    contactDetailsGrid.ColumnDefinitions.Add(gridIconColumn);
                    contactDetailsGrid.ColumnDefinitions.Add(gridDetailColumn);

                    Label contactNameLabel = new Label();
                    contactNameLabel.Content = employeesContacts[i].Value[j].contact.contact_name;
                    contactNameLabel.Style = (Style)FindResource("stackPanelItemHeader");
                    contactDetailsStackPanel.Children.Add(contactNameLabel);

                    for (int k = 0; k < employeesContacts[i].Value[j].contact_phones.Count; k++)
                        SetContactPhoneRow(k, employeesContacts[i].Value[j].contact_phones[k], ref contactDetailsGrid);

                    for (int k = 0; k < employeesContacts[i].Value[j].contact_emails.Count; k++)
                        SetContactEmailRow(employeesContacts[i].Value[j].contact_phones.Count + k, employeesContacts[i].Value[j].contact_emails[k], ref contactDetailsGrid);


                    contactDetailsStackPanel.Children.Add(contactDetailsGrid);

                    Grid contactGridItem = new Grid();

                    ColumnDefinition iconColumn = new ColumnDefinition();
                    ColumnDefinition stackColumn = new ColumnDefinition();

                    iconColumn.MaxWidth = 50;

                    contactGridItem.ColumnDefinitions.Add(iconColumn);
                    contactGridItem.ColumnDefinitions.Add(stackColumn);

                    Image contactIcon = new Image { Source = new BitmapImage(new Uri(@"icons\contact_icon.png", UriKind.Relative)) };
                    ResizeImage(ref contactIcon, 40, 40);

                    contactGridItem.Children.Add(contactIcon);
                    Grid.SetRow(contactIcon, 0);

                    contactGridItem.Children.Add(contactDetailsStackPanel);
                    Grid.SetColumn(contactDetailsStackPanel, 1);

                    contactStackPanel.Children.Add(contactGridItem);
                    companyStackPanel.Children.Add(contactStackPanel);

                    contactsStackArray.Add(new KeyValuePair<COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT, StackPanel>(employeesContacts[i].Value[j], contactStackPanel));

                }

            }
        }

        public void SetCompanyFieldRow(int row, String companyField, ref Grid companyGrid)
        {
            RowDefinition contactPhoneRow = new RowDefinition();
            companyGrid.RowDefinitions.Add(contactPhoneRow);


            Image phoneIcon = new Image { Source = new BitmapImage(new Uri(@"icons\field_icon.png", UriKind.Relative)) };
            ResizeImage(ref phoneIcon, 25, 25);

            companyGrid.Children.Add(phoneIcon);
            Grid.SetRow(phoneIcon, row);
            Grid.SetColumn(phoneIcon, 0);


            Label companyPhoneLabel = new Label();
            companyPhoneLabel.Content = companyField;
            companyPhoneLabel.Style = (Style)FindResource("stackPanelItemBody");

            companyGrid.Children.Add(companyPhoneLabel);
            Grid.SetRow(companyPhoneLabel, row);
            Grid.SetColumn(companyPhoneLabel, 1);
        }
        public void SetCompanyPhoneRow(int row, String companyPhone, ref Grid companyGrid)
        {
            RowDefinition contactPhoneRow = new RowDefinition();
            companyGrid.RowDefinitions.Add(contactPhoneRow);


            Image phoneIcon = new Image { Source = new BitmapImage(new Uri(@"icons\phone_icon.png", UriKind.Relative)) };
            ResizeImage(ref phoneIcon, 25, 25);

            companyGrid.Children.Add(phoneIcon);
            Grid.SetRow(phoneIcon, row);
            Grid.SetColumn(phoneIcon, 0);


            Label companyPhoneLabel = new Label();
            companyPhoneLabel.Content = companyPhone;
            companyPhoneLabel.Style = (Style)FindResource("stackPanelItemBody");

            companyGrid.Children.Add(companyPhoneLabel);
            Grid.SetRow(companyPhoneLabel, row);
            Grid.SetColumn(companyPhoneLabel, 1);
        }

        public void SetContactPhoneRow(int row, String contactPhone, ref Grid contactGrid)
        {
            RowDefinition contactPhoneRow = new RowDefinition();
            contactGrid.RowDefinitions.Add(contactPhoneRow);


            Image phoneIcon = new Image { Source = new BitmapImage(new Uri(@"icons\phone_icon.png", UriKind.Relative)) };
            ResizeImage(ref phoneIcon, 25, 25);

            contactGrid.Children.Add(phoneIcon);
            Grid.SetRow(phoneIcon, row);
            Grid.SetColumn(phoneIcon, 0);


            Label contactPhoneLabel = new Label();
            contactPhoneLabel.Content = contactPhone;
            contactPhoneLabel.Style = (Style)FindResource("stackPanelItemBody");

            contactGrid.Children.Add(contactPhoneLabel);
            Grid.SetRow(contactPhoneLabel, row);
            Grid.SetColumn(contactPhoneLabel, 1);
        }

        public void SetContactEmailRow(int row, String contactEmail, ref Grid contactGrid)
        {
            RowDefinition contactStatusRow = new RowDefinition();
            contactGrid.RowDefinitions.Add(contactStatusRow);

            Image statusIcon = new Image { Source = new BitmapImage(new Uri(@"icons\email_icon.png", UriKind.Relative)) };
            ResizeImage(ref statusIcon, 25, 25);

            contactGrid.Children.Add(statusIcon);
            Grid.SetRow(statusIcon, row);
            Grid.SetColumn(statusIcon, 0);

            Label contactStatusLabel = new Label();
            contactStatusLabel.Content = contactEmail;
            contactStatusLabel.Style = (Style)FindResource("stackPanelItemBody");

            contactGrid.Children.Add(contactStatusLabel);
            Grid.SetRow(contactStatusLabel, row);
            Grid.SetColumn(contactStatusLabel, 1);
        }
        public void SetContactBudgetRow(int row, String contactBudget, ref Grid contactGrid)
        {
            RowDefinition contactStatusRow = new RowDefinition();
            contactGrid.RowDefinitions.Add(contactStatusRow);


            Image budgetIcon = new Image { Source = new BitmapImage(new Uri(@"icons\budget_icon.jpg", UriKind.Relative)) };
            ResizeImage(ref budgetIcon, 25, 25);

            contactGrid.Children.Add(budgetIcon);
            Grid.SetRow(budgetIcon, row);
            Grid.SetColumn(budgetIcon, 0);


            Label contactBudgeRangeLabel = new Label();
            contactBudgeRangeLabel.Content = contactBudget;
            contactBudgeRangeLabel.Style = (Style)FindResource("stackPanelItemBody");

            contactGrid.Children.Add(contactBudgeRangeLabel);
            Grid.SetRow(contactBudgeRangeLabel, row);
            Grid.SetColumn(contactBudgeRangeLabel, 1);
        }

        public void ResizeImage(ref Image imgToResize, int width, int height)
        {
            imgToResize.Width = width;
            imgToResize.Height = height;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON CHECK HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnCheckedCompanyNameCheckBox(object sender, RoutedEventArgs e)
        {
            companyNameTextBox.IsEnabled = true;
        }


        private void OnCheckedContactNameCheckBox(object sender, RoutedEventArgs e)
        {
            contactNameTextBox.IsEnabled = true;
        }


        private void OnCheckedPrimaryFieldCheckBox(object sender, RoutedEventArgs e)
        {
            primaryFieldComboBox.IsEnabled = true;
            primaryFieldComboBox.SelectedIndex = 0;
        }


        private void OnCheckedSecondaryFieldCheckBox(object sender, RoutedEventArgs e)
        {
            secondaryFieldComboBox.IsEnabled = true;
            secondaryFieldComboBox.SelectedIndex = 0;
        }
        private void OnCheckedSalesPersonCheckBox(object sender, RoutedEventArgs e)
        {
            salesPersonComboBox.IsEnabled = true;

            SetEmployeeComboBox();
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void OnTextChangedCompanyName(object sender, TextChangedEventArgs e)
        {
            InitializeContactsTree();
            InitializeContactsStackPanel();
        }

        private void OnTextChangedContactName(object sender, TextChangedEventArgs e)
        {
            InitializeContactsTree();
            InitializeContactsStackPanel();
        }

        private void OnSelChangedPrimaryFieldComboBox(object sender, SelectionChangedEventArgs e)
        {
            secondaryFieldCheckBox.IsEnabled = true;
            secondaryFieldCheckBox.IsChecked = false;
            secondaryFieldComboBox.IsEnabled = false;
            ViewBtn.IsEnabled = false;

            if (!InitializeSecondaryFieldComboBox())
                return;

            InitializeContactsTree();
            InitializeContactsStackPanel();
        }

        private void OnSelChangedSecondaryFieldComboBox(object sender, SelectionChangedEventArgs e)
        {
            ViewBtn.IsEnabled = false;

            InitializeContactsTree();
            InitializeContactsStackPanel();
        }
        private void OnSelChangedSalesPersonComboBox(object sender, SelectionChangedEventArgs e)
        {
            ViewBtn.IsEnabled = false;

            if (salesPersonCheckBox.IsChecked == true && salesPersonComboBox.SelectedItem != null)
                selectedEmployee = listOfEmployees[salesPersonComboBox.SelectedIndex].employee_id;
            else
                selectedEmployee = 0;

            InitializeContactsTree();
            InitializeContactsStackPanel();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //ON SELECTED ITEM CHANGED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnMouseDownCompanyStackPanel(object sender, RoutedEventArgs e)
        {

        }
        private void OnMouseDownContactStackPanel(object sender, RoutedEventArgs e)
        {
            previousSelectedContactItem = currentSelectedContactItem;
            currentSelectedContactItem = (StackPanel)sender;

            BrushConverter brush = new BrushConverter();

            if (previousSelectedContactItem != null)
            {
                Grid previousParentGrid = (Grid)previousSelectedContactItem.Parent;
                previousParentGrid.Background = (Brush)brush.ConvertFrom("#FFFFFF"); ;

                StackPanel prevoiusParentStackPanel = (StackPanel)previousParentGrid.Parent;
                COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT previousContactItem = contactsStackArray.Find(contact_item => contact_item.Value == prevoiusParentStackPanel).Key;

                Image previousSontactIcon = (Image)previousParentGrid.Children[0];
                previousSontactIcon.Source = new BitmapImage(new Uri(@"icons\contact_icon.png", UriKind.Relative));

                Label previousSelectedContactLabel = (Label)previousSelectedContactItem.Children[0];
                previousSelectedContactLabel.Foreground = (Brush)brush.ConvertFrom("#105A97");
                previousSelectedContactLabel.Background = (Brush)brush.ConvertFrom("#FFFFFF");

                Label previouscontactNameLabel = (Label)previousSelectedContactItem.Children[0];

                previouscontactNameLabel.Foreground = (Brush)brush.ConvertFrom("#105A97");
                previouscontactNameLabel.Background = (Brush)brush.ConvertFrom("#FFFFFF");

                Grid previousContactDetailsGrid = (Grid)previousSelectedContactItem.Children[1];


                for (int i = 1; i < previousContactDetailsGrid.Children.Count; i += 2)
                {
                    Label currentLabel = (Label)previousContactDetailsGrid.Children[i];

                    currentLabel.Foreground = (Brush)brush.ConvertFrom("#105A97");
                    currentLabel.Background = (Brush)brush.ConvertFrom("#FFFFFF");
                }

                int previousContactDetailsCount = 0;
                for (int i = 0; i < previousContactDetailsGrid.Children.Count; i += 2)
                {
                    Image currentIcon = (Image)previousContactDetailsGrid.Children[i];

                    if (previousContactDetailsCount++ < previousContactItem.contact_phones.Count)
                        currentIcon.Source = new BitmapImage(new Uri(@"icons\phone_icon.png", UriKind.Relative));
                    else
                        currentIcon.Source = new BitmapImage(new Uri(@"icons\email_icon.png", UriKind.Relative));
                }
            }

            Grid parentGrid = (Grid)currentSelectedContactItem.Parent;
            parentGrid.Background = (Brush)brush.ConvertFrom("#105A97"); ;

            StackPanel parentStackPanel = (StackPanel)parentGrid.Parent;
            COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT currentContactItem = contactsStackArray.Find(contact_item => contact_item.Value == parentStackPanel).Key;

            Image contactIcon = (Image)parentGrid.Children[0];
            contactIcon.Source = new BitmapImage(new Uri(@"icons\contact_icon_blue.png", UriKind.Relative));

            Label currentSelectedContactLabel = (Label)currentSelectedContactItem.Children[0];
            currentSelectedContactLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");
            currentSelectedContactLabel.Background = (Brush)brush.ConvertFrom("#105A97");

            Label contactNameLabel = (Label)currentSelectedContactItem.Children[0];

            contactNameLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");
            contactNameLabel.Background = (Brush)brush.ConvertFrom("#105A97");

            Grid contactDetailsGrid = (Grid)currentSelectedContactItem.Children[1];


            for (int i = 1; i < contactDetailsGrid.Children.Count; i += 2)
            {
                Label currentLabel = (Label)contactDetailsGrid.Children[i];

                currentLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");
                currentLabel.Background = (Brush)brush.ConvertFrom("#105A97");
            }

            int contactDetailsCount = 0;
            for (int i = 0; i < contactDetailsGrid.Children.Count; i += 2)
            {
                Image currentIcon = (Image)contactDetailsGrid.Children[i];

                if (contactDetailsCount++ < currentContactItem.contact_phones.Count) 
                    currentIcon.Source = new BitmapImage(new Uri(@"icons\phone_icon_blue.png", UriKind.Relative));
                else
                    currentIcon.Source = new BitmapImage(new Uri(@"icons\email_icon_blue.png", UriKind.Relative));
                    
            }

            ViewBtn.IsEnabled = true;

        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON UNCHECKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnUncheckedCompanyNameCheckBox(object sender, RoutedEventArgs e)
        {
            companyNameTextBox.IsEnabled = false;
            companyNameTextBox.Text = null;

            ViewBtn.IsEnabled = false;

            InitializeContactsTree();
            InitializeContactsStackPanel();
        }

        private void OnUncheckedContactNameCheckBox(object sender, RoutedEventArgs e)
        {
            contactNameTextBox.IsEnabled = false;
            contactNameTextBox.Text = null;

            ViewBtn.IsEnabled = false;

            InitializeContactsTree();
            InitializeContactsStackPanel();
        }
        private void OnUncheckedPrimaryFieldCheckBox(object sender, RoutedEventArgs e)
        {
            primaryFieldComboBox.IsEnabled = false;
            secondaryFieldComboBox.IsEnabled = false;
            ViewBtn.IsEnabled = false;

            secondaryFieldCheckBox.IsEnabled = false;

            primaryFieldComboBox.SelectedItem = null;
            secondaryFieldComboBox.SelectedItem = null;

            primaryFieldCheckBox.IsChecked = false;
            secondaryFieldCheckBox.IsChecked = false;

            InitializeContactsTree();
            InitializeContactsStackPanel();
        }

        private void OnUncheckedSecondaryFieldCheckBox(object sender, RoutedEventArgs e)
        {
            secondaryFieldComboBox.SelectedItem = null;

            secondaryFieldComboBox.IsEnabled = false;
            ViewBtn.IsEnabled = false;

            InitializeContactsTree();
            InitializeContactsStackPanel();
        }
        private void OnUncheckedSalesPersonCheckBox(object sender, RoutedEventArgs e)
        {
            salesPersonComboBox.IsEnabled = false;
            salesPersonComboBox.SelectedItem = null;

            InitializeContactsTree();
            InitializeContactsStackPanel();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON BTN CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnBtnClickAddCompany(object sender, RoutedEventArgs e)
        {
            ViewBtn.IsEnabled = false;
            AddCompanyWindow addCompanyWindow = new AddCompanyWindow(ref loggedInUser);
            addCompanyWindow.Closed += OnClosedAddCompanyWindow;
            addCompanyWindow.Show();
        }
        private void OnBtnClickAddContact(object sender, RoutedEventArgs e)
        {
            ViewBtn.IsEnabled = false;

            AddContactWindow addContactWindow = new AddContactWindow(ref loggedInUser);
            addContactWindow.Closed += OnClosedAddContactWindow;
            addContactWindow.Show();
        }

        private void OnClosedAddCompanyWindow(object sender, EventArgs e)
        {
            employeesContacts.Clear();

            GetAllContacts();

            SetDefaultSettings();

            InitializeContactsTree();
            InitializeContactsStackPanel();
        }
        private void OnClosedAddContactWindow(object sender, EventArgs e)
        {
            employeesContacts.Clear();

            GetAllContacts();

            SetDefaultSettings();

            InitializeContactsTree();
            InitializeContactsStackPanel();
        }
        private void OnSelectedItemChangedTreeViewItem(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ViewBtn.IsEnabled = false;
            TreeViewItem selectedItem = (TreeViewItem)contactTreeView.SelectedItem;

            if (selectedItem != null)
                ViewBtn.IsEnabled = true;
        }

        private void OnBtnClickView(object sender, RoutedEventArgs e)
        {
            if (contactTreeScrollViewer.Visibility == Visibility.Visible)
            {
                TreeViewItem selectedItem = (TreeViewItem)contactTreeView.SelectedItem;

                if (!selectedItem.HasItems)
                {
                    COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT currentContactStruct = new COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT();
                    currentContactStruct = contactsTreeArray.Find(current_item => current_item.Value == selectedItem).Key;

                    Contact selectedContact = new Contact();
                    selectedContact.InitializeContactInfo(currentContactStruct.sales_person_id, currentContactStruct.company.address_serial, currentContactStruct.contact.contact_id);

                    ViewContactWindow viewContactWindow = new ViewContactWindow(ref loggedInUser, ref selectedContact);
                    viewContactWindow.Show();

                }
                else if(selectedItem.Parent != null)
                {
                    int selectedCompanySerial = companiesTreeArray.Find(current_item => current_item.Value == selectedItem).Key; 

                    Company selectedCompany = new Company();
                    selectedCompany.InitializeCompanyInfo(selectedCompanySerial);

                    ViewCompanyWindow viewCompanyWindow = new ViewCompanyWindow(ref loggedInUser, ref selectedCompany);
                    viewCompanyWindow.Show();
                }
            }
            else if (contactStackScrollViewer.Visibility == Visibility.Visible)
            {
                COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT currentContactStruct = new COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT();
                currentContactStruct = contactsStackArray.Find(current_item => current_item.Value == currentSelectedContactItem).Key;

                Contact selectedContact = new Contact();
                selectedContact.InitializeContactInfo(currentContactStruct.sales_person_id, currentContactStruct.contact.contact_id);

                ViewContactWindow viewContactWindow = new ViewContactWindow(ref loggedInUser, ref selectedContact);
                viewContactWindow.Show();
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //VIEWING TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnClickListView(object sender, MouseButtonEventArgs e)
        {
            listViewLabel.Style = (Style)FindResource("selectedMainTabLabelItem");
            treeViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");

            contactStackScrollViewer.Visibility = Visibility.Visible;
            contactTreeScrollViewer.Visibility = Visibility.Collapsed;
        }

        private void OnClickTreeView(object sender, MouseButtonEventArgs e)
        {
            listViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");
            treeViewLabel.Style = (Style)FindResource("selectedMainTabLabelItem");

            contactStackScrollViewer.Visibility = Visibility.Collapsed;
            contactTreeScrollViewer.Visibility = Visibility.Visible;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //EXTERNAL TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnButtonClickedMyProfile(object sender, RoutedEventArgs e)
        {
            UserPortalPage userPortal = new UserPortalPage(ref loggedInUser);
            this.NavigationService.Navigate(userPortal);
        }
        private void OnButtonClickedContacts(object sender, RoutedEventArgs e)
        {
            ContactsPage contacts = new ContactsPage(ref loggedInUser);
            this.NavigationService.Navigate(contacts);
        }
        private void OnButtonClickedProducts(object sender, MouseButtonEventArgs e)
        {
            ProductsPage productsPage = new ProductsPage(ref loggedInUser);
            this.NavigationService.Navigate(productsPage);
        }
        private void OnButtonClickedWorkOrders(object sender, RoutedEventArgs e)
        {
            WorkOrdersPage workOrders = new WorkOrdersPage(ref loggedInUser);
            this.NavigationService.Navigate(workOrders);
        }
        private void OnButtonClickedWorkOffers(object sender, RoutedEventArgs e)
        {
            QuotationsPage workOffers = new QuotationsPage(ref loggedInUser);
            this.NavigationService.Navigate(workOffers);
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

        }
        private void OnButtonClickedProjects(object sender, MouseButtonEventArgs e)
        {
            ProjectsPage projectsPage = new ProjectsPage(ref loggedInUser);
            this.NavigationService.Navigate(projectsPage);
        }
        private void OnButtonClickedMaintenanceContracts(object sender, MouseButtonEventArgs e)
        {

        }
        private void OnButtonClickedMaintenanceOffer(object sender, MouseButtonEventArgs e)
        {
            MaintenanceOffersPage maintenanceOffersPage = new MaintenanceOffersPage(ref loggedInUser);
            this.NavigationService.Navigate(maintenanceOffersPage);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //BTN CLICKED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }


}
