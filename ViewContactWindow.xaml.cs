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
    /// Interaction logic for ViewContactWindow.xaml
    /// </summary>
    public partial class ViewContactWindow : Window
    {
        protected Employee loggedInUser;

        protected CommonQueries commonQueries;

        List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> companies;
        List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> companyAddresses;
        List<COMPANY_ORGANISATION_MACROS.DEPARTMENT_STRUCT> departments;
        List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> employees;

        Contact contact;
        public ViewContactWindow(ref Employee mLoggedInUser, ref Contact mContact)
        {
            InitializeComponent();

            commonQueries = new CommonQueries();
            loggedInUser = mLoggedInUser;
            contact = mContact;

            companies = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
            companyAddresses = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
            departments = new List<COMPANY_ORGANISATION_MACROS.DEPARTMENT_STRUCT>();
            employees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();

            InitializeCompanyNameCombo();
            InitializeCompanyAddressCombo();
            InitializeGenderCombo();
            InitializeDepartmentsCombo();
            InitializeAssigneeCombo();
            
            InitializeContactInfo();

            if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION || loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)
            {
                saveChangesButton.Visibility = Visibility.Visible;
            }


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

        private void InitializeCompanyNameCombo()
        {
            if (!commonQueries.GetEmployeeCompanies(contact.GetSalesPersonId(), ref companies))
                return;
            for (int i = 0; i < companies.Count; i++)
            {
                companyNameCombo.Items.Add(companies[i].company_name);
            }
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
        }

        private void InitializeGenderCombo()
        {
            contactGenderCombo.Items.Add("Male");
            contactGenderCombo.Items.Add("Female");
        }

        private void InitializeDepartmentsCombo()
        {
            if (!commonQueries.GetDepartmentsType(ref departments))
                return;
            for (int i = 0; i < departments.Count; i++)
            {
                departmentComboBox.Items.Add(departments[i].department_name);
            }
        }

        private void InitializeAssigneeCombo()
        {

            List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> tempEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();

            if (!commonQueries.GetTeamEmployees(COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID, ref tempEmployeesList))
                return;

            for (int i = 0; i < tempEmployeesList.Count; i++)
            {
                employees.Add(tempEmployeesList[i]);
            }


            if (!commonQueries.GetTeamEmployees(COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID, ref tempEmployeesList))
                return;

            for (int i = 0; i < tempEmployeesList.Count; i++)
            {
                employees.Add(tempEmployeesList[i]);
            }

            for(int i = 0; i < employees.Count; i++)
            {
                assigneeComboBox.Items.Add(employees[i].employee_name);
            }
        }

        private void InitializeContactInfo()
        {
            employeeFirstNameLabel.Content = contact.GetContactName();

            contactGenderLabel.Content = contact.GetContactGender();
            companyNameLabel.Content = contact.GetCompanyName();

            companyBranchLabel.Content = contact.GetCompanyCountry() + ", " + contact.GetCompanyState() + ", " + contact.GetCompanyCity() + ", " + contact.GetCompanyDistrict();
            departmentLabel.Content = contact.GetContactDepartment();

            assigneeLabel.Content = contact.GetSalesPerson().GetEmployeeName();

            wrapPanel4.Children.Clear();
            wrapPanel3.Children.Clear();

            AddBusinessEmail();

            for(int i = 0; i < contact.GetNumberOfSavedContactEmails(); i++)
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

             Label PersonalPhoneLabel = new Label();
             PersonalPhoneLabel.Style = (Style)FindResource("tableItemLabel");
             PersonalPhoneLabel.Content = "Personal Phone";

             TextBox employeePersonalPhoneTextBox = new TextBox();
             employeePersonalPhoneTextBox.IsEnabled = false;
             employeePersonalPhoneTextBox.Style = (Style)FindResource("textBoxStyle");
             employeePersonalPhoneTextBox.Text = Phone;

             ContactPersonalPhoneWrapPanel.Children.Add(PersonalPhoneLabel);
             ContactPersonalPhoneWrapPanel.Children.Add(employeePersonalPhoneTextBox);
            wrapPanel4.Children.Add(ContactPersonalPhoneWrapPanel);

        } 
        
        private void AddBusinessPhone()
        {
            WrapPanel ContactBusinessPhoneWrapPanel = new WrapPanel();

            Label BusinessPhoneLabel = new Label();
            BusinessPhoneLabel.Style = (Style)FindResource("tableItemLabel");
            BusinessPhoneLabel.Content = "Business Phone";

            TextBox employeeBusinessPhoneTextBox = new TextBox();
            employeeBusinessPhoneTextBox.IsEnabled = false;
            employeeBusinessPhoneTextBox.Style = (Style)FindResource("textBoxStyle");
            employeeBusinessPhoneTextBox.Text = contact.GetContactPhones()[0];

            ContactBusinessPhoneWrapPanel.Children.Add(BusinessPhoneLabel);
            ContactBusinessPhoneWrapPanel.Children.Add(employeeBusinessPhoneTextBox);
            wrapPanel4.Children.Add(ContactBusinessPhoneWrapPanel);

        }
        private void AddBusinessEmail()
        {
            WrapPanel ContactBusinessEmailWrapPanel = new WrapPanel();

            Label BusinessEmailLabel = new Label();
            BusinessEmailLabel.Style = (Style)FindResource("tableItemLabel");
            BusinessEmailLabel.Content = "Business Email";

            TextBox employeeBusinessEmailTextBox = new TextBox();
            employeeBusinessEmailTextBox.IsEnabled = false;
            employeeBusinessEmailTextBox.Style = (Style)FindResource("textBoxStyle");
            employeeBusinessEmailTextBox.Text = contact.GetContactBusinessEmail();

            ContactBusinessEmailWrapPanel.Children.Add(BusinessEmailLabel);
            ContactBusinessEmailWrapPanel.Children.Add(employeeBusinessEmailTextBox);

            wrapPanel3.Children.Add(ContactBusinessEmailWrapPanel);
        } 
        private void AddPersonalEmail(String Email)
        {
            WrapPanel ContactPersonalEmailWrapPanel = new WrapPanel();

            Label PersonalEmailLabel = new Label();
            PersonalEmailLabel.Style = (Style)FindResource("tableItemLabel");
            PersonalEmailLabel.Content = "Personal Email";

            TextBox employeePersonalEmailTextBox = new TextBox();
            employeePersonalEmailTextBox.IsEnabled = false;
            employeePersonalEmailTextBox.Style = (Style)FindResource("textBoxStyle");
            employeePersonalEmailTextBox.Text = Email;

            ContactPersonalEmailWrapPanel.Children.Add(PersonalEmailLabel);
            ContactPersonalEmailWrapPanel.Children.Add(employeePersonalEmailTextBox);

            wrapPanel3.Children.Add(ContactPersonalEmailWrapPanel);
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
            Label nameLabel = (Label)sender;
            WrapPanel currentWrapPanel = (WrapPanel)nameLabel.Parent;
            Label currentLabel = (Label)currentWrapPanel.Children[1];
            TextBox currentTextBox = (TextBox)currentWrapPanel.Children[2];

            if ((loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION || loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION) && currentTextBox.Visibility == Visibility.Collapsed)
            {
                currentTextBox.Visibility = Visibility.Visible;
                currentLabel.Visibility = Visibility.Collapsed;

                currentTextBox.Text = currentLabel.Content.ToString();
            }
            else if ((loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION || loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION) && currentTextBox.Visibility == Visibility.Visible)
            {
                currentTextBox.Visibility = Visibility.Collapsed;
                currentLabel.Visibility = Visibility.Visible;

                currentLabel.Content = currentTextBox.Text;

                if (currentLabel.Content.ToString() != contact.GetContactName())
                    currentLabel.Foreground = Brushes.Red;
                else
                    currentLabel.Foreground = Brushes.Black;
            }
        }

        private void OnClickComboBoxLabels(object sender, RoutedEventArgs e)
        {
            Label nameLabel = (Label)sender;
            WrapPanel currentWrapPanel = (WrapPanel)nameLabel.Parent;
            Label currentLabel = (Label)currentWrapPanel.Children[1];
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
            else if((loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION || loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION) && currentComboBox.Visibility == Visibility.Visible)
            {
                currentComboBox.Visibility = Visibility.Collapsed;
                currentLabel.Visibility = Visibility.Visible;

                currentLabel.Content = currentComboBox.Text;

                if(currentLabel.Name == "contactGenderLabel")
                {
                    if (currentLabel.Content.ToString() != contact.GetContactGender())
                        currentLabel.Foreground = Brushes.Red;
                    else
                        currentLabel.Foreground = Brushes.Black;
                }
                else if (currentLabel.Name == "companyNameLabel")
                {
                    if (currentLabel.Content.ToString() != contact.GetCompanyName())
                        currentLabel.Foreground = Brushes.Red;
                    else
                        currentLabel.Foreground = Brushes.Black;
                }
                else if (currentLabel.Name == "companyBranchLabel")
                {
                    if (currentLabel.Content.ToString() != contact.GetCompanyCountry() + ", " + contact.GetCompanyState() + ", " + contact.GetCompanyCity() + ", " + contact.GetCompanyDistrict())
                        currentLabel.Foreground = Brushes.Red;
                    else
                        currentLabel.Foreground = Brushes.Black;
                }
                else if (currentLabel.Name == "departmentLabel")
                {
                    if (currentLabel.Content.ToString() != contact.GetContactDepartment())
                        currentLabel.Foreground = Brushes.Red;
                    else
                        currentLabel.Foreground = Brushes.Black;
                }
                else if (currentLabel.Name == "assigneeLabel")
                {
                    if (currentLabel.Content.ToString() != contact.GetSalesPerson().GetEmployeeName())
                        currentLabel.Foreground = Brushes.Red;
                    else
                        currentLabel.Foreground = Brushes.Black;
                }
            }
        }


        private void OnTextChangedEditTextBoxes(object sender, TextChangedEventArgs e)
        {
            TextBox currentTextBox = (TextBox)sender;
            WrapPanel currentWrapPanel = (WrapPanel)currentTextBox.Parent;
            Label currentLabel = (Label)currentWrapPanel.Children[1];

            if (currentTextBox.Text != currentLabel.Content.ToString())
                currentTextBox.Foreground = Brushes.Red;
            else
                currentTextBox.Foreground = Brushes.Black;
        }

        private void OnSelChangedEditComboBoxes(object sender, SelectionChangedEventArgs e)
        {
            ComboBox currentComboBox = (ComboBox)sender;
            WrapPanel currentWrapPanel = (WrapPanel)currentComboBox.Parent;
            Label currentLabel = (Label)currentWrapPanel.Children[1];

            if (currentComboBox.Text != currentLabel.Content.ToString())
                currentComboBox.Background = Brushes.Red;
            else
                currentComboBox.Background = Brushes.LightGray;
        }

        private void OnSelChangedCompanyName(object sender, SelectionChangedEventArgs e)
        {
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


        private void OnBtnClickSaveChanges(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
