using _01electronics_library;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using ComboBox = System.Windows.Controls.ComboBox;
using Label = System.Windows.Controls.Label;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using TextBox = System.Windows.Controls.TextBox;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for ViewContactWindow.xaml
    /// </summary>
    public partial class ViewContactWindow : Window
    {
        protected Employee loggedInUser;

        protected CommonQueries commonQueries;
        protected IntegrityChecks integrityChecks;

        List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> companies;
        List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> companyAddresses;
        List<COMPANY_ORGANISATION_MACROS.DEPARTMENT_STRUCT> departments;
        List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> employees;

        private List<BASIC_STRUCTS.COUNTRY_CODES_STRUCT> countryCodes;

        Contact contact;

        int contactEmployeeId;

        int assigneeId = 0;

        bool initializationComplete;

        private String errorMessage;

        private int countryCodeIndex;
        private String countryCode;

        public ViewContactWindow(ref Employee mLoggedInUser, ref Contact mContact)
        {
            initializationComplete = false;

            InitializeComponent();

            commonQueries = new CommonQueries();
            loggedInUser = mLoggedInUser;
            contact = mContact;
            integrityChecks = new IntegrityChecks();

            companies = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
            companyAddresses = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
            departments = new List<COMPANY_ORGANISATION_MACROS.DEPARTMENT_STRUCT>();
            employees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
            countryCodes = new List<BASIC_STRUCTS.COUNTRY_CODES_STRUCT>();

            InitializeCountryCodes();
            InitializeCompanyNameCombo();
            InitializeCompanyAddressCombo();
            InitializeGenderCombo();
            InitializeDepartmentsCombo();
            InitializeAssigneeCombo();

            countryCodeIndex = countryCodes.FindIndex(x1 => x1.country_id == companyAddresses[companyBranchCombo.SelectedIndex].address / 1000000);
            countryCode = countryCodes[countryCodeIndex].phone_code;


            InitializeContactInfo();


            if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION || loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)
            {
                saveChangesButton.Visibility = Visibility.Visible;
            }

            contactEmployeeId = contact.GetSalesPersonId();

            initializationComplete = true;
        }

        private void SetUpUIElementsForSaveChanges()
        {
            employeeFirstNameTextBox.Visibility = Visibility.Collapsed;
            employeeFirstNameLabel.Visibility = Visibility.Visible;
            employeeFirstNameLabel.Content = employeeFirstNameTextBox.Text;

            contactGenderCombo.Visibility = Visibility.Collapsed;
            contactGenderLabel.Visibility = Visibility.Visible;
            contactGenderLabel.Content = contactGenderCombo.Text;

            companyNameCombo.Visibility = Visibility.Collapsed;
            companyNameLabel.Visibility = Visibility.Visible;
            companyNameLabel.Content = companyNameCombo.Text;

            companyNameCombo.Visibility = Visibility.Collapsed;
            companyNameLabel.Visibility = Visibility.Visible;
            companyNameLabel.Content = companyNameCombo.Text;

            companyBranchCombo.Visibility = Visibility.Collapsed;
            companyBranchLabel.Visibility = Visibility.Visible;
            companyBranchLabel.Content = companyBranchCombo.Text;


        }

        private bool InitializeCountryCodes()
        {
            if (!commonQueries.GetCountryCodes(ref countryCodes))
                return false;

            return true;
        }
        private void InitializeCompanyNameCombo()
        {
            ///////If Assignee is changed we get selected assignee companies
            if (assigneeId == 0)
            {
                if (!commonQueries.GetEmployeeCompanies(contact.GetSalesPersonId(), ref companies))
                    return;
            }
            else
            {
                if (!commonQueries.GetEmployeeCompanies(assigneeId, ref companies))
                    return;
            }

            companyNameCombo.Items.Clear();

            for (int i = 0; i < companies.Count; i++)
            {
                companyNameCombo.Items.Add(companies[i].company_name);
            }

            companyNameCombo.Text = contact.GetCompanyName();
        }

        private void InitializeCompanyAddressCombo()
        {
            if (!commonQueries.GetCompanyAddresses(contact.GetCompanySerial(), ref companyAddresses))
                return;

            companyBranchCombo.Items.Clear();
            companyBranchCombo.IsEnabled = true;
            for (int i = 0; i < companyAddresses.Count; i++)
            {
                companyBranchCombo.Items.Add(companyAddresses[i].country + ", " + companyAddresses[i].state_governorate + ", " + companyAddresses[i].city + ", " + companyAddresses[i].district);
            }

            companyBranchCombo.Text = contact.GetCompanyCountry() + ", " + contact.GetCompanyState() + ", " + contact.GetCompanyCity() + ", " + contact.GetCompanyDistrict();
        }

        private void InitializeGenderCombo()
        {
            contactGenderCombo.Items.Add("Male");
            contactGenderCombo.Items.Add("Female");

            contactGenderCombo.Text = contact.GetContactGender();
        }

        private void InitializeDepartmentsCombo()
        {
            if (!commonQueries.GetContactTitles(ref departments))
                return;
            for (int i = 0; i < departments.Count; i++)
            {
                departmentComboBox.Items.Add(departments[i].department_name);
            }

            departmentComboBox.Text = contact.GetContactDepartment();
        }

        private void InitializeAssigneeCombo()
        {

            List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> tempEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();

            if (!commonQueries.GetAllTeamEmployees(COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID, ref tempEmployeesList))
                return;

            for (int i = 0; i < tempEmployeesList.Count; i++)
            {
                employees.Add(tempEmployeesList[i]);
            }


            if (!commonQueries.GetAllTeamEmployees(COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID, ref tempEmployeesList))
                return;

            for (int i = 0; i < tempEmployeesList.Count; i++)
            {
                employees.Add(tempEmployeesList[i]);
            }

            if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)
            {
                COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT temp = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT();
                temp.employee_id = loggedInUser.GetEmployeeId();
                temp.employee_name = loggedInUser.GetEmployeeName();

                employees.Add(temp);
            }

            for (int i = 0; i < employees.Count; i++)
            {

                if (employees[i].employement_status_id >= 4)
                {
                    ComboBoxItem comboBoxItem = new ComboBoxItem();
                    comboBoxItem.Content = employees[i].employee_name;
                    comboBoxItem.Foreground = Brushes.Red;
                    assigneeComboBox.Items.Add(comboBoxItem);
                }
                else
                    assigneeComboBox.Items.Add(employees[i].employee_name);

            }

            assigneeComboBox.Text = contact.GetSalesPerson().GetEmployeeName();
        }

        private void InitializeContactInfo()
        {
            employeeFirstNameLabel.Content = contact.GetContactName();

            contactGenderLabel.Content = contact.GetContactGender();
            companyNameLabel.Content = contact.GetCompanyName();

            companyBranchLabel.Content = contact.GetCompanyCountry() + ", " + contact.GetCompanyState() + ", " + contact.GetCompanyCity() + ", " + contact.GetCompanyDistrict();
            departmentLabel.Content = contact.GetContactDepartment();

            assigneeLabel.Content = contact.GetSalesPerson().GetEmployeeName();

            businessEmailWrapPanel.Children.Clear();
            contactPhoneWrapPanel.Children.Clear();
            personalEmailWrapPanel.Children.Clear();

            AddBusinessEmail();

            for (int i = 0; i < contact.GetNumberOfSavedContactEmails(); i++)
            {
                if (contact.GetContactPersonalEmails()[i] != null)
                    AddPersonalEmail(contact.GetContactPersonalEmails()[i]);
            }

            if (contact.GetContactPhones()[0] != null)
                AddBusinessPhone();

            for (int i = 1; i < contact.GetNumberOfSavedContactPhones(); i++)
            {
                if (contact.GetContactPhones()[i] != null)
                    AddPersonalPhone(contact.GetContactPhones()[i]);
            }

        }
        private void AddPersonalPhone(String Phone)
        {
            WrapPanel ContactPersonalPhoneWrapPanel = new WrapPanel();
            contactPersonalPhoneWrapPanel.Tag = 1;

            Label PersonalPhoneLabel = new Label();
            PersonalPhoneLabel.Style = (Style)FindResource("tableItemLabel");
            PersonalPhoneLabel.Content = "Personal Phone";

            Label PersonalPhoneLabelValue = new Label();
            PersonalPhoneLabelValue.Style = (Style)FindResource("tableItemValue");
            PersonalPhoneLabelValue.Width = 150;
            PersonalPhoneLabelValue.Content = countryCode + Phone;
            PersonalPhoneLabelValue.MouseDoubleClick += OnClickTextBoxLabels;

            ComboBox countryCodeCombo = new ComboBox();
            countryCodeCombo.Style = (Style)FindResource("miniComboBoxStyle");
            countryCodeCombo.Items.Add(countryCodes[countryCodeIndex].iso3 + "   " + countryCodes[countryCodeIndex].phone_code);
            countryCodeCombo.SelectedIndex = 0;
            countryCodeCombo.IsEnabled = false;
            countryCodeCombo.Visibility = Visibility.Collapsed;

            TextBox employeePersonalPhoneTextBox = new TextBox();
            employeePersonalPhoneTextBox.Style = (Style)FindResource("miniTextBoxStyle");
            employeePersonalPhoneTextBox.Text = Phone;
            employeePersonalPhoneTextBox.Visibility = Visibility.Collapsed;
            employeePersonalPhoneTextBox.MouseLeave += TextBoxesMouseLeave;

            ContactPersonalPhoneWrapPanel.Children.Add(PersonalPhoneLabel);
            ContactPersonalPhoneWrapPanel.Children.Add(PersonalPhoneLabelValue);
            ContactPersonalPhoneWrapPanel.Children.Add(countryCodeCombo);
            ContactPersonalPhoneWrapPanel.Children.Add(employeePersonalPhoneTextBox);
            contactPersonalPhoneWrapPanel.Children.Add(ContactPersonalPhoneWrapPanel);

        }

        private void AddBusinessPhone()
        {
            WrapPanel ContactBusinessPhoneWrapPanel = new WrapPanel();
            ContactBusinessPhoneWrapPanel.Tag = 1;

            Label BusinessPhoneLabel = new Label();
            BusinessPhoneLabel.Style = (Style)FindResource("tableItemLabel");
            BusinessPhoneLabel.Content = "Business Phone";

            Label BusinessPhoneLabelValue = new Label();
            BusinessPhoneLabelValue.Width = 150;
            BusinessPhoneLabelValue.Content = countryCode + contact.GetContactPhones()[0];
            BusinessPhoneLabelValue.Style = (Style)FindResource("tableItemValue");
            BusinessPhoneLabelValue.MouseDoubleClick += OnClickTextBoxLabels;

            ComboBox countryCodeCombo = new ComboBox();
            countryCodeCombo.Style = (Style)FindResource("miniComboBoxStyle");
            countryCodeCombo.Items.Add(countryCodes[countryCodeIndex].iso3 + "   " + countryCodes[countryCodeIndex].phone_code);
            countryCodeCombo.SelectedIndex = 0;
            countryCodeCombo.IsEnabled = false;
            countryCodeCombo.Visibility = Visibility.Collapsed;

            TextBox employeeBusinessPhoneTextBox = new TextBox();
            employeeBusinessPhoneTextBox.Style = (Style)FindResource("miniTextBoxStyle");
            employeeBusinessPhoneTextBox.Text = contact.GetContactPhones()[0];
            employeeBusinessPhoneTextBox.Visibility = Visibility.Collapsed;
            employeeBusinessPhoneTextBox.MouseLeave += TextBoxesMouseLeave;

            ContactBusinessPhoneWrapPanel.Children.Add(BusinessPhoneLabel);
            ContactBusinessPhoneWrapPanel.Children.Add(BusinessPhoneLabelValue);
            ContactBusinessPhoneWrapPanel.Children.Add(countryCodeCombo);
            ContactBusinessPhoneWrapPanel.Children.Add(employeeBusinessPhoneTextBox);
            contactPhoneWrapPanel.Children.Add(ContactBusinessPhoneWrapPanel);

        }
        private void AddBusinessEmail()
        {
            WrapPanel ContactBusinessEmailWrapPanel = new WrapPanel();

            Label BusinessEmailLabel = new Label();
            BusinessEmailLabel.Style = (Style)FindResource("tableItemLabel");
            BusinessEmailLabel.Content = "Business Email";

            Label BusinessEmailLabelValue = new Label();
            BusinessEmailLabelValue.Content = contact.GetContactBusinessEmail();
            BusinessEmailLabelValue.Style = (Style)FindResource("tableItemValue");
            BusinessEmailLabelValue.MouseDoubleClick += OnClickTextBoxLabels;

            TextBox employeeBusinessEmailTextBox = new TextBox();
            employeeBusinessEmailTextBox.Style = (Style)FindResource("textBoxStyle");
            employeeBusinessEmailTextBox.Text = contact.GetContactBusinessEmail();
            employeeBusinessEmailTextBox.Visibility = Visibility.Collapsed;
            employeeBusinessEmailTextBox.MouseLeave += TextBoxesMouseLeave;

            ContactBusinessEmailWrapPanel.Children.Add(BusinessEmailLabel);
            ContactBusinessEmailWrapPanel.Children.Add(BusinessEmailLabelValue);
            ContactBusinessEmailWrapPanel.Children.Add(employeeBusinessEmailTextBox);

            businessEmailWrapPanel.Children.Add(ContactBusinessEmailWrapPanel);
        }
        private void AddPersonalEmail(String Email)
        {
            WrapPanel ContactPersonalEmailWrapPanel = new WrapPanel();

            Label PersonalEmailLabel = new Label();
            PersonalEmailLabel.Style = (Style)FindResource("tableItemLabel");
            PersonalEmailLabel.Content = "Personal Email";

            Label PersonalEmailLabelValue = new Label();
            PersonalEmailLabelValue.Content = Email;
            PersonalEmailLabelValue.Style = (Style)FindResource("tableItemValue");
            PersonalEmailLabelValue.MouseDoubleClick += OnClickTextBoxLabels;

            TextBox employeePersonalEmailTextBox = new TextBox();
            employeePersonalEmailTextBox.Style = (Style)FindResource("textBoxStyle");
            employeePersonalEmailTextBox.Text = Email;
            employeePersonalEmailTextBox.Visibility = Visibility.Collapsed;
            employeePersonalEmailTextBox.MouseLeave += TextBoxesMouseLeave;

            ContactPersonalEmailWrapPanel.Children.Add(PersonalEmailLabel);
            ContactPersonalEmailWrapPanel.Children.Add(PersonalEmailLabelValue);
            ContactPersonalEmailWrapPanel.Children.Add(employeePersonalEmailTextBox);

            personalEmailWrapPanel.Children.Add(ContactPersonalEmailWrapPanel);
        }

        private void OnBtnClkAddDetails(object sender, RoutedEventArgs e)
        {
            AddContactDetailsWindow addContactDetailsWindow = new AddContactDetailsWindow(ref loggedInUser, ref contact);
            addContactDetailsWindow.Closed += OnClosedAddContactDetailsWindow;
            addContactDetailsWindow.Show();
        }
        private void OnClosedAddContactDetailsWindow(object sender, EventArgs e)
        {
            InitializeContactInfo();
        }




        private void OnClickTextBoxLabels(object sender, RoutedEventArgs e)
        {
            Label currentLabel = (Label)sender;
            WrapPanel currentWrapPanel = (WrapPanel)currentLabel.Parent;

            if (currentWrapPanel.Tag == null)
            {
                TextBox currentTextBox = (TextBox)currentWrapPanel.Children[2];

                if ((loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION || loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION) && currentTextBox.Visibility == Visibility.Collapsed)
                {
                    currentTextBox.Visibility = Visibility.Visible;
                    currentLabel.Visibility = Visibility.Collapsed;

                    currentTextBox.Text = currentLabel.Content.ToString();
                }
            }
            else
            {
                ComboBox currentComboBox = (ComboBox)currentWrapPanel.Children[2];
                TextBox currentTextBox = (TextBox)currentWrapPanel.Children[3];

                if ((loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION || loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION) && currentTextBox.Visibility == Visibility.Collapsed)
                {
                    currentComboBox.Visibility = Visibility.Visible;
                    currentTextBox.Visibility = Visibility.Visible;
                    currentLabel.Visibility = Visibility.Collapsed;

                    String tempPhone = string.Empty;

                    for (int i = 2; i < currentLabel.Content.ToString().Length; i++)
                    {
                        tempPhone += currentLabel.Content.ToString()[i];
                    }

                    currentTextBox.Text = tempPhone;
                }
            }
        }

        private void OnClickComboBoxLabels(object sender, RoutedEventArgs e)
        {
            Label currentLabel = (Label)sender;
            WrapPanel currentWrapPanel = (WrapPanel)currentLabel.Parent;
            ComboBox currentComboBox = (ComboBox)currentWrapPanel.Children[2];

            if ((loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION || loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION) && currentComboBox.Visibility == Visibility.Collapsed)
            {

                currentComboBox.Visibility = Visibility.Visible;
                currentLabel.Visibility = Visibility.Collapsed;

                currentComboBox.SelectedItem = currentLabel.Content.ToString();

                if (currentComboBox.Name == "companyNameCombo")
                {
                    companyBranchCombo.Visibility = Visibility.Visible;
                    companyBranchLabel.Visibility = Visibility.Collapsed;

                    if (companyNameCombo.SelectedItem != null)
                    {
                        if (!commonQueries.GetCompanyAddresses(companies[companyNameCombo.SelectedIndex].company_serial, ref companyAddresses))
                            return;

                        companyBranchCombo.Items.Clear();
                        companyBranchCombo.IsEnabled = true;
                        for (int i = 0; i < companyAddresses.Count; i++)
                        {
                            companyBranchCombo.Items.Add(companyAddresses[i].country + ", " + companyAddresses[i].state_governorate + ", " + companyAddresses[i].city + ", " + companyAddresses[i].district);
                        }
                        companyBranchCombo.SelectedIndex = companyAddresses.FindIndex(x => x.address_serial == contact.GetAddressSerial());
                    }
                    else
                    {
                        companyBranchCombo.IsEnabled = true;
                        companyBranchCombo.SelectedItem = null;
                    }
                }
            }
        }


        private void OnTextChangedEditTextBoxes(object sender, TextChangedEventArgs e)
        {
        }

        private void OnSelChangedEditComboBoxes(object sender, SelectionChangedEventArgs e)
        {
        }

        private void OnSelChangedAssigneeCombo(object sender, SelectionChangedEventArgs e)
        {
            if (initializationComplete == true)
            {
                ComboBox currentComboBox = (ComboBox)sender;

                Employee selectedEmployee = new Employee();

                if (currentComboBox.SelectedIndex != -1)
                {
                    selectedEmployee.InitializeEmployeeInfo(employees[currentComboBox.SelectedIndex].employee_id);

                    assigneeId = selectedEmployee.GetEmployeeId();
                }
            }
        }

        private void OnSelChangedCompanyName(object sender, SelectionChangedEventArgs e)
        {
            if (initializationComplete == true)
            {
                saveChangesButton.IsEnabled = true;

                if (companyNameCombo.SelectedItem != null)
                {
                    if (!commonQueries.GetCompanyAddresses(companies[companyNameCombo.SelectedIndex].company_serial, ref companyAddresses))
                        return;

                    companyBranchCombo.Items.Clear();
                    companyBranchCombo.IsEnabled = true;
                    for (int i = 0; i < companyAddresses.Count; i++)
                    {
                        companyBranchCombo.Items.Add(companyAddresses[i].country + ", " + companyAddresses[i].state_governorate + ", " + companyAddresses[i].city + ", " + companyAddresses[i].district);
                    }
                    companyBranchCombo.SelectedIndex = 0;
                }
                else
                {
                    companyBranchCombo.IsEnabled = true;
                    companyBranchCombo.SelectedItem = null;
                }
            }
        }




        private void TextBoxesMouseLeave(object sender, MouseEventArgs e)
        {
            saveChangesButton.IsEnabled = true;

            TextBox currentTextBox = (TextBox)sender;
            WrapPanel currentWrapPanel = (WrapPanel)currentTextBox.Parent;

            Label currentLabel = (Label)currentWrapPanel.Children[1];

            WrapPanel parentWrapPanel = (WrapPanel)currentWrapPanel.Parent;

            currentTextBox.Visibility = Visibility.Collapsed;
            currentLabel.Visibility = Visibility.Visible;

            BrushConverter brush = new BrushConverter();

            currentLabel.Content = currentTextBox.Text;

            if (currentLabel.Name == "employeeFirstNameLabel")
            {
                if (currentLabel.Content.ToString() != contact.GetContactName())
                    currentLabel.Foreground = Brushes.Red;
                else
                    currentLabel.Foreground = (Brush)brush.ConvertFrom("#105A97");
            }
            else if (parentWrapPanel.Name == "contactPhoneWrapPanel")
            {
                ComboBox currentComboBox = (ComboBox)currentWrapPanel.Children[2];
                currentComboBox.Visibility = Visibility.Collapsed;

                currentLabel.Content = countryCode + currentTextBox.Text;

                if (currentTextBox.Text.ToString() != contact.GetContactPhones()[0])
                    currentLabel.Foreground = Brushes.Red;
                else
                    currentLabel.Foreground = (Brush)brush.ConvertFrom("#105A97");
            }
            else if (parentWrapPanel.Name == "businessEmailWrapPanel")
            {
                if (currentLabel.Content.ToString() != contact.GetContactBusinessEmail())
                    currentLabel.Foreground = Brushes.Red;
                else
                    currentLabel.Foreground = (Brush)brush.ConvertFrom("#105A97");
            }
            else if (parentWrapPanel.Name == "contactPersonalPhoneWrapPanel")
            {
                ComboBox currentComboBox = (ComboBox)currentWrapPanel.Children[2];
                currentComboBox.Visibility = Visibility.Collapsed;

                currentLabel.Content = countryCode + currentTextBox.Text;

                int index = 0;
                for (int i = 0; i < contactPersonalPhoneWrapPanel.Children.Count; i++)
                {
                    WrapPanel selectedWrapPanel = (WrapPanel)contactPersonalPhoneWrapPanel.Children[i];
                    if (selectedWrapPanel == currentWrapPanel)
                        index = i + 1;
                }
                if (currentTextBox.Text.ToString() != contact.GetContactPhones()[index].ToString())
                    currentLabel.Foreground = Brushes.Red;
                else
                    currentLabel.Foreground = (Brush)brush.ConvertFrom("#105A97");
            }
            else if (parentWrapPanel.Name == "personalEmailWrapPanel")
            {
                int index = 0;
                for (int i = 0; i < personalEmailWrapPanel.Children.Count; i++)
                {
                    WrapPanel selectedWrapPanel = (WrapPanel)personalEmailWrapPanel.Children[i];
                    if (selectedWrapPanel == currentWrapPanel)
                        index = i;
                }
                if (currentTextBox.Text.ToString() != contact.GetContactPersonalEmails()[index].ToString())
                    currentLabel.Foreground = Brushes.Red;
                else
                    currentLabel.Foreground = (Brush)brush.ConvertFrom("#105A97");
            }

        }

        private void ComboBoxesMouseLeave(object sender, MouseEventArgs e)
        {
            saveChangesButton.IsEnabled = true;

            ComboBox currentComboBox = (ComboBox)sender;
            WrapPanel currentWrapPanel = (WrapPanel)currentComboBox.Parent;
            Label currentLabel = (Label)currentWrapPanel.Children[1];

            BrushConverter brush = new BrushConverter();

            currentComboBox.Visibility = Visibility.Collapsed;
            currentLabel.Visibility = Visibility.Visible;

            currentLabel.Content = currentComboBox.Text;

            if (currentLabel.Name == "contactGenderLabel")
            {
                if (currentLabel.Content.ToString() != contact.GetContactGender())
                    currentLabel.Foreground = Brushes.Red;
                else
                    currentLabel.Foreground = (Brush)brush.ConvertFrom("#105A97");
            }
            else if (currentLabel.Name == "companyNameLabel")
            {
                if (currentLabel.Content.ToString() != contact.GetCompanyName())
                    currentLabel.Foreground = Brushes.Red;
                else
                    currentLabel.Foreground = (Brush)brush.ConvertFrom("#105A97");
            }
            else if (currentLabel.Name == "companyBranchLabel")
            {
                if (currentLabel.Content.ToString() != contact.GetCompanyCountry() + ", " + contact.GetCompanyState() + ", " + contact.GetCompanyCity() + ", " + contact.GetCompanyDistrict())
                {
                    currentLabel.Foreground = Brushes.Red;
                    countryCodeIndex = countryCodes.FindIndex(x1 => x1.country_id == companyAddresses[companyBranchCombo.SelectedIndex].address / 1000000);
                    countryCode = countryCodes[countryCodeIndex].phone_code;
                }
                else
                    currentLabel.Foreground = (Brush)brush.ConvertFrom("#105A97");
            }
            else if (currentLabel.Name == "departmentLabel")
            {
                if (currentLabel.Content.ToString() != contact.GetContactDepartment())
                    currentLabel.Foreground = Brushes.Red;
                else
                    currentLabel.Foreground = (Brush)brush.ConvertFrom("#105A97");
            }
            else if (currentLabel.Name == "assigneeLabel")
            {
                if (currentComboBox.SelectedIndex != -1 && employees[currentComboBox.SelectedIndex].employee_id != contactEmployeeId)
                    currentLabel.Foreground = Brushes.Red;
                else if (currentComboBox.SelectedIndex != -1)
                    currentLabel.Foreground = (Brush)brush.ConvertFrom("#105A97");
            }
        }


        private void OnBtnClickSaveChanges(object sender, RoutedEventArgs e)
        {
            Contact newContact = new Contact();
            bool assigneeChanged = false;

            bool companyChanged = false;

            newContact.SetContactName(employeeFirstNameLabel.Content.ToString());
            newContact.SetContactGender(contactGenderLabel.Content.ToString());

            newContact.InitializeSalesPersonInfo(employees[assigneeComboBox.SelectedIndex].employee_id);

            if (assigneeLabel.Foreground == Brushes.Red)
            {
                assigneeChanged = true;
            }
            newContact.InitializeCompanyInfo(companies[companyNameCombo.SelectedIndex].company_serial);
            newContact.InitializeBranchInfo(companyAddresses[companyBranchCombo.SelectedIndex].address_serial);
            newContact.SetContactDepartment(departments[departmentComboBox.SelectedIndex].department_id, departments[departmentComboBox.SelectedIndex].department_name);

            if(newContact.GetCompanySerial()!=contact.GetCompanySerial())
                companyChanged = true;

            if(contactPhoneWrapPanel.Children.Count !=0)
            {
                WrapPanel businessPhoneWrap = (WrapPanel)contactPhoneWrapPanel.Children[0];
                TextBox businessPhone = (TextBox)businessPhoneWrap.Children[3];
    
                String inputString = businessPhone.Text.ToString();
                String outputString = businessPhone.Text.ToString();
    
                if (!integrityChecks.CheckContactPhoneEditBox(inputString, ref outputString, companyAddresses[companyBranchCombo.SelectedIndex].address / 1000000, true, ref errorMessage))
                {
                    System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    newContact.AddNewContactPhone(outputString);
                }
    
                if (contactPersonalPhoneWrapPanel.Children.Count > 0)
                {
                    for (int i = 0; i < contactPersonalPhoneWrapPanel.Children.Count; i++)
                    {
    
                        WrapPanel personalPhoneWrapPanel = (WrapPanel)contactPersonalPhoneWrapPanel.Children[i];
                        TextBox personalPhone = (TextBox)personalPhoneWrapPanel.Children[3];
    
                        inputString = personalPhone.Text.ToString();
                        outputString = personalPhone.Text.ToString();
    
                        if (!integrityChecks.CheckContactPhoneEditBox(inputString, ref outputString, companyAddresses[companyBranchCombo.SelectedIndex].address / 1000000, true, ref errorMessage))
                        {
                            System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        else
                        {
                            newContact.AddNewContactPhone(outputString);
                        }
                    }
                }

            }

            if (businessEmailWrapPanel.Children.Count != 0)
            {
                WrapPanel businessEmailWrap = (WrapPanel)businessEmailWrapPanel.Children[0];
                Label businessEmail = (Label)businessEmailWrap.Children[1];
                newContact.SetContactBusinessEmail(businessEmail.Content.ToString());

                if (personalEmailWrapPanel.Children.Count > 0)
                {
                    for (int i = 0; i < personalEmailWrapPanel.Children.Count; i++)
                    {
                        WrapPanel personalEmailWrap = (WrapPanel)personalEmailWrapPanel.Children[i];
                        Label personalEmail = (Label)personalEmailWrap.Children[1];
                        newContact.AddNewContactEmail(personalEmail.Content.ToString());
                    }
                }
            }

            contact.EditContactInfo(newContact, assigneeChanged,companyChanged);
            this.Close();
        }

    }
}
