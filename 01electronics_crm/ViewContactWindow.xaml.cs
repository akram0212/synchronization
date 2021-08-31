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

        Contact contact;
        public ViewContactWindow(ref Employee mLoggedInUser, ref Contact mContact)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;
            contact = mContact;

            InitializeContactInfo();

        }
        private void InitializeContactInfo()
        {
            employeeFirstNameTextBox.IsEnabled = false;
            contactGenderTextBox.IsEnabled = false;
            companyNameTextBox.IsEnabled = false;
            companyBranchTextBox.IsEnabled = false;
            departmentTextBox.IsEnabled = false;

            employeeFirstNameTextBox.Text = contact.GetContactName();

            contactGenderTextBox.Text = contact.GetContactGender();
            companyNameTextBox.Text = contact.GetCompanyName();

            companyBranchTextBox.Text = contact.GetCompanyCountry() + ", " + contact.GetCompanyState() + ", " + contact.GetCompanyCity() + ", " + contact.GetCompanyDistrict();
            departmentTextBox.Text = contact.GetContactDepartment();

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

    }
}