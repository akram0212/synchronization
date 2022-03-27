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
using _01electronics_library;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for ViewCompanyWindow.xaml
    /// </summary>
    public partial class ViewCompanyWindow : Window
    {
        protected Employee loggedInUser;

        protected CommonQueries commonQueries;

        protected Company company;

        protected int phonesCount;
        protected int faxesCount;

        Label currentSelectedItem;
        Label previousSelectedItem;

        List<BASIC_STRUCTS.PRIMARY_FIELD_STRUCT> primaryWorkFields;
        List<BASIC_STRUCTS.SECONDARY_FIELD_STRUCT> secondaryWorkFields;

        List<KeyValuePair<int, int>> updatedObjects;
        int gridRowsCounter;
        bool isManager;

        int TELEPHONE_INDEX = 1;
        int FAX_INDEX = 2;

        protected List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> branchesList;
        public ViewCompanyWindow(ref Employee mLoggedInUser, ref Company mCompany)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;
            company = mCompany;

            commonQueries = new CommonQueries();

            secondaryWorkFields = new List<BASIC_STRUCTS.SECONDARY_FIELD_STRUCT>();
            primaryWorkFields = new List<BASIC_STRUCTS.PRIMARY_FIELD_STRUCT>();
            branchesList = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
            updatedObjects = new List<KeyValuePair<int, int>>();
            gridRowsCounter = 4;
            isManager = false;

            currentSelectedItem = new Label();
            previousSelectedItem = new Label();
            currentSelectedItem = null;
            previousSelectedItem = null;

            companyNameLabel.Content = company.GetCompanyName();
            companyNameLabel.Tag = company.GetCompanySerial();
            companyNameTextBox.Tag = company.GetCompanySerial();

            primaryWorkFieldLabel.Content = company.GetCompanyPrimaryField();

            secondaryWorkFieldLabel.Content = company.GetCompanySecondaryField();

            if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)
            {
                isManager = true;

                mergeCompanyButton.IsEnabled = true;

                initializePrimaryWorkFieldCombo();
                initializeSecondaryWorkFieldCombo(company.GetCompanyPrimaryFieldId());

                secondaryWorkFieldLabel.Visibility = Visibility.Visible;
                secondaryWorkFieldComboBox.Visibility = Visibility.Collapsed;

                primaryWorkFieldLabel.Visibility = Visibility.Visible;
                primaryWorkFieldComboBox.Visibility = Visibility.Collapsed;

            }

            if (!InitializeBranchesComboBox())
            return;

        }
        private void InitializeCompanyInfo()
        {
            companyNameTextBox.IsEnabled = false;
            primaryWorkFieldComboBox.IsEnabled = false;
            secondaryWorkFieldComboBox.IsEnabled = false;

            companyNameLabel.Content = company.GetCompanyName();
            primaryWorkFieldLabel.Content = company.GetCompanyPrimaryField();
            secondaryWorkFieldLabel.Content = company.GetCompanySecondaryField();
            //primaryWorkFieldComboBox.Text = company.GetCompanyPrimaryField();
            // secondaryWorkFieldComboBox.Text = company.GetCompanySecondaryField();

            if (!InitializeBranchesComboBox())
                return;

            if (!InitializeBranchPhones())
                return;
        }
        private bool InitializeBranchesComboBox()
        {
            branchComboBox.Items.Clear();

            if (!commonQueries.GetCompanyAddresses(company.GetCompanySerial(), ref branchesList))
                return false;

            for (int i = 0; i < branchesList.Count; i++)
                branchComboBox.Items.Add(branchesList[i].district + ", " + branchesList[i].city + ", " + branchesList[i].state_governorate + ", " + branchesList[i].country);

            branchComboBox.SelectedIndex = 0;

            return true;
        }
        private bool InitializeBranchPhones()
        {
            company.SetAddressSerial(branchesList[branchComboBox.SelectedIndex].address_serial);

            if (!company.QueryCompanyPhones())
                return false;

            AddPhone();

            return true;
        }
        private bool InitializeBranchFaxes()
        {
            company.SetAddressSerial(branchesList[branchComboBox.SelectedIndex].address_serial);

            if (!company.QueryCompanyFaxes())
                return false;

            AddFax();

            return true;
        }
        private bool initializePrimaryWorkFieldCombo()
        {
            if (!commonQueries.GetPrimaryWorkFields(ref primaryWorkFields))
                return false;

            for(int i = 0; i < primaryWorkFields.Count; i++)
            {
                primaryWorkFieldComboBox.Items.Add(primaryWorkFields[i].field_name);
            }

            return true;
        } 
        private bool initializeSecondaryWorkFieldCombo(int primaryFieldID)
        {
            secondaryWorkFields.Clear();

            if (!commonQueries.GetSecondaryWorkFields(primaryFieldID, ref secondaryWorkFields))
                return false;

            for (int i = 0; i < secondaryWorkFields.Count; i++)
            {
                secondaryWorkFieldComboBox.Items.Add(secondaryWorkFields[i].field_name);
            }

            return true;
        }

        private void AddPhone()
        {
            for (int i = 0; i < company.GetCompanyPhones().Count(); i++)
            {
                if (company.GetCompanyPhones()[i] != String.Empty)
                {
                    gridRowsCounter++;

                    RowDefinition phoneRow = new RowDefinition();

                    ContactGrid.RowDefinitions.Add(phoneRow);

                    WrapPanel PhoneWrapPanel = new WrapPanel();

                    Label PhoneLabel = new Label();
                    PhoneLabel.Style = (Style)FindResource("tableItemLabel");
                    PhoneLabel.Content = "Telephone";

                    Label telephoneLabel = new Label();
                    telephoneLabel.Style = (Style)FindResource("tableItemValue");
                    telephoneLabel.Content = company.GetCompanyPhones()[i];
                    telephoneLabel.MouseDoubleClick += OnDoubleClickLabel;
                    telephoneLabel.Tag = i;

                    TextBox telephoneTextBox = new TextBox();
                    telephoneTextBox.IsEnabled = false;
                    telephoneTextBox.Style = (Style)FindResource("textBoxStyle");
                    telephoneTextBox.Text = company.GetCompanyPhones()[i];
                    telephoneTextBox.Visibility = Visibility.Collapsed;
                    telephoneTextBox.Tag = TELEPHONE_INDEX;

                    PhoneWrapPanel.Children.Add(PhoneLabel);
                    PhoneWrapPanel.Children.Add(telephoneLabel);
                    PhoneWrapPanel.Children.Add(telephoneTextBox);
                    Grid.SetRow(PhoneWrapPanel, gridRowsCounter-1);
                    ContactGrid.Children.Add(PhoneWrapPanel);
                }
            }
        }
        private void AddFax()
        {
            for (int i = 0; i < company.GetCompanyFaxes().Count(); i++)
            {
                if (company.GetCompanyFaxes()[i] != String.Empty)
                {
                    gridRowsCounter++;

                    RowDefinition phoneRow = new RowDefinition();

                    ContactGrid.RowDefinitions.Add(phoneRow);

                    WrapPanel FaxWrapPanel = new WrapPanel();

                    Label FaxHeader = new Label();
                    FaxHeader.Style = (Style)FindResource("tableItemLabel");
                    FaxHeader.Content = "Fax";

                    Label faxLabel = new Label();
                    faxLabel.Style = (Style)FindResource("tableItemValue");
                    faxLabel.Content = company.GetCompanyFaxes()[i];
                    faxLabel.Tag = i;
                    faxLabel.MouseDoubleClick +=  OnDoubleClickLabel;

                    TextBox FaxTextBox = new TextBox();
                    FaxTextBox.IsEnabled = false;
                    FaxTextBox.Style = (Style)FindResource("textBoxStyle");
                    FaxTextBox.Text = company.GetCompanyFaxes()[i];
                    FaxTextBox.Tag = FAX_INDEX;
                    FaxTextBox.Visibility = Visibility.Collapsed;

                    FaxWrapPanel.Children.Add(FaxHeader);
                    FaxWrapPanel.Children.Add(faxLabel);
                    FaxWrapPanel.Children.Add(FaxTextBox); 
                    Grid.SetRow(FaxWrapPanel, gridRowsCounter - 1);
                    ContactGrid.Children.Add(FaxWrapPanel);
                }
            }
        }

        private void setUIEelements()
        {
            WrapPanel currentWrapPanel = (WrapPanel)currentSelectedItem.Parent;

            if (isManager == true)
            {
                saveChangesButton.IsEnabled = true;

                if(previousSelectedItem != null)
                {
                    WrapPanel previousWrapPanel = (WrapPanel)previousSelectedItem.Parent;

                    if (previousWrapPanel.Children[2].GetType() == typeof(TextBox))
                    {
                        setCollapsedTextBoxes(previousWrapPanel);
                    }
                    else
                    {
                        setCollapsedComboBoxes(previousWrapPanel);
                    }
                    
                }
                if (currentWrapPanel.Children[2].GetType() == typeof(TextBox))
                {
                    setTextBoxes(currentWrapPanel);
                }
                else
                {
                    setComboBoxes(currentWrapPanel);
                }
            }
        }

        private void OnClosedAddBranchWindow(object sender, EventArgs e)
        {
            InitializeBranchesComboBox();
        }
        private void OnClosedAddCompanyDetailsWindow(object sender, EventArgs e)
        {
            InitializeCompanyInfo();
        }
        private void OnClosedMergeCompaniesWindow(object sender, EventArgs e)
        {
            InitializeCompanyInfo();
            for (int i = ContactGrid.Children.Count - 1; i >= 4; i--)
            {
                ContactGrid.Children.RemoveAt(i);
                ContactGrid.RowDefinitions.RemoveAt(i);
                gridRowsCounter--;
            }

            if (branchComboBox.SelectedItem != null)
            {
                InitializeBranchPhones();
                InitializeBranchFaxes();

            }
        }

        private void OnDoubleClickLabel(object sender, MouseButtonEventArgs e)
        {
            previousSelectedItem = currentSelectedItem;
            currentSelectedItem = (Label) sender;
            setUIEelements();
        }

        private void OnBtnClkAddBranch(object sender, RoutedEventArgs e)
        {
            AddBranchWindow addBranchWindow = new AddBranchWindow(ref loggedInUser, ref company);
            addBranchWindow.Closed += OnClosedAddBranchWindow;
            addBranchWindow.Show();
        }
        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {
            try
            {
                setCollapsedTextBoxes((WrapPanel)currentSelectedItem.Parent);
            }
            catch
            {
                try
                {
                    setCollapsedComboBoxes((WrapPanel)currentSelectedItem.Parent);
                }
                catch
                {

                }
            }
            for (int i = 0; i < updatedObjects.Count; i++)
            {
                if (updatedObjects[i].Key == 0)
                {
                    company.SetCompanyName(companyNameLabel.Content.ToString());

                   if (!company.UpdateCompanyName())
                        return;
                }
                else if (updatedObjects[i].Key == 1 || updatedObjects[i].Key == 2)
                {
                    company.SetCompanySecondaryField(updatedObjects[i].Value, secondaryWorkFieldLabel.Content.ToString());

                    if (!company.UpdateCompanyWorkField())
                        return;
                }
                else
                {
                    if(updatedObjects[i].Value == TELEPHONE_INDEX)
                    {
                        WrapPanel currentTelephone = (WrapPanel)ContactGrid.Children[updatedObjects[i].Key];
                        Label currentTelephoneLabel = (Label) currentTelephone.Children[1];

                        if (!company.UpdateCompanyTelephone(branchesList[branchComboBox.SelectedIndex].address_serial, company.GetCompanyPhones()[int.Parse(currentTelephoneLabel.Tag.ToString())] , currentTelephoneLabel.Content.ToString()))
                            return;
                    }
                    else
                    {
                        WrapPanel currentFax = (WrapPanel)ContactGrid.Children[updatedObjects[i].Key];
                        Label currentFaxLabel = (Label)currentFax.Children[1];

                        if (!company.UpdateCompanyFax(branchesList[branchComboBox.SelectedIndex].address_serial, company.GetCompanyFaxes()[int.Parse(currentFaxLabel.Tag.ToString())], currentFaxLabel.Content.ToString()))
                            return;
                    }
                }
            }
            this.Close();
        }
        private void OnBtnClkAddDetails(object sender, RoutedEventArgs e)
        {
            AddCompanyDetailsWindow addComapnyDetailsWindow = new AddCompanyDetailsWindow(ref loggedInUser, ref company, branchComboBox.SelectedItem.ToString());
            addComapnyDetailsWindow.Closed += OnClosedAddBranchWindow;
            addComapnyDetailsWindow.Show();
        }
        private void OnBtnClkMergeCompany(object sender, RoutedEventArgs e)
        {
            MergeCompaniesWindow mergeCompaniesWindow = new MergeCompaniesWindow(ref loggedInUser, ref company);
            mergeCompaniesWindow.Closed += OnClosedMergeCompaniesWindow;
            mergeCompaniesWindow.Show();
        }

        private void setCollapsedComboBoxes(WrapPanel mWrapPanel)
        {
            ComboBox currentComboBox = (ComboBox)mWrapPanel.Children[2];
            Label currentLabel = (Label)mWrapPanel.Children[1];

            currentComboBox.Visibility = Visibility.Collapsed;
            currentLabel.Visibility = Visibility.Visible;

            if (currentLabel.Content.ToString() != currentComboBox.Text.ToString())
            {
                currentLabel.Foreground = Brushes.Red;
                currentLabel.Content = currentComboBox.Text;

                try
                {
                    currentComboBox.Tag = secondaryWorkFields[secondaryWorkFieldComboBox.SelectedIndex].field_id;
                }
                catch
                {

                }

                if (!updatedObjects.Exists(wrapPanelIndex => wrapPanelIndex.Key == ContactGrid.Children.IndexOf(mWrapPanel)) && currentComboBox.Tag != null)
                {
                    updatedObjects.Add(new KeyValuePair<int, int>(ContactGrid.Children.IndexOf(mWrapPanel), int.Parse(currentComboBox.Tag.ToString())));
                }
            }    

        }
        private void setCollapsedTextBoxes(WrapPanel mWrapPanel)
        {
            TextBox currentTextBox = (TextBox)mWrapPanel.Children[2];
            Label currentLabel = (Label)mWrapPanel.Children[1];

            currentTextBox.Visibility = Visibility.Collapsed;
            currentLabel.Visibility = Visibility.Visible;

            if (currentLabel.Content.ToString() != currentTextBox.Text.ToString())
            {
                currentLabel.Foreground = Brushes.Red;
                currentLabel.Content = currentTextBox.Text;
                if (!updatedObjects.Exists(wrapPanelIndex => wrapPanelIndex.Key == ContactGrid.Children.IndexOf(mWrapPanel)) && currentTextBox.Tag != null)
                {
                    updatedObjects.Add(new KeyValuePair<int, int>(ContactGrid.Children.IndexOf(mWrapPanel), int.Parse(currentTextBox.Tag.ToString())));
                }

            }

        }
        private void setTextBoxes(WrapPanel mWrapPanel)
        {
            TextBox currentTextBox = (TextBox)mWrapPanel.Children[2];
            currentTextBox.IsEnabled = true;
            Label currentLabel = (Label)mWrapPanel.Children[1];
            currentTextBox.Text = currentLabel.Content.ToString();

            currentTextBox.Visibility = Visibility.Visible;
            currentLabel.Visibility = Visibility.Collapsed;

        }
        private void setComboBoxes(WrapPanel mWrapPanel)
        {
            ComboBox currentComboBox = (ComboBox)mWrapPanel.Children[2];
            currentComboBox.IsEnabled = true;
            Label currentLabel = (Label)mWrapPanel.Children[1];
            currentComboBox.Text = currentLabel.Content.ToString();

            currentComboBox.Visibility = Visibility.Visible;
            currentLabel.Visibility = Visibility.Collapsed;

        }

        private void OnTextChangedCompanyName(object sender, TextChangedEventArgs e)
        {

        }
        private void OnSelChangedBranch(object sender, SelectionChangedEventArgs e)
        {
            for (int i = ContactGrid.Children.Count -1; i >= 4; i--)
            {
                ContactGrid.Children.RemoveAt(i);
                ContactGrid.RowDefinitions.RemoveAt(i);
                gridRowsCounter--;
            }

            if (branchComboBox.SelectedItem != null)
            {
                InitializeBranchPhones();
                InitializeBranchFaxes();

            }
        }
        private void OnSelprimaryWorkFieldComboBox(object sender, SelectionChangedEventArgs e)
        {
            secondaryWorkFieldComboBox.Items.Clear();
            initializeSecondaryWorkFieldCombo(primaryWorkFields[primaryWorkFieldComboBox.SelectedIndex].field_id);
            secondaryWorkFieldLabel.Content = "Others";
            secondaryWorkFieldLabel.Foreground = Brushes.Red;

        }
        private void OnSelSecondaryWorkFieldComboBox(object sender, SelectionChangedEventArgs e)
        {
            ComboBox currentComboBox = (ComboBox)sender;
            WrapPanel parent = (WrapPanel)currentComboBox.Parent;
            Label currentLabel = (Label)parent.Children[1];

            try
            {
                if (!updatedObjects.Exists(wrapPanelIndex => wrapPanelIndex.Key == ContactGrid.Children.IndexOf(parent)))
                {
                    updatedObjects.Add(new KeyValuePair<int, int>(ContactGrid.Children.IndexOf(parent), int.Parse(currentLabel.Tag.ToString())));
                }
            }
            catch
            {

            }
        }

    }
}
