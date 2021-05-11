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
using System.Windows.Shapes;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for ViewContactInfoWindow.xaml
    /// </summary>
    public partial class ViewContactInfoWindow : Window
    {
        long contactID;
        Contact contact_Info;
        Employee loggedInUser;
        long companyID;
        public ViewContactInfoWindow(long contactID, Employee loggedInUser, long companyID)
        {
            InitializeComponent();
            this.contactID = contactID;
            this.loggedInUser = loggedInUser;
            this.companyID = companyID;
            contact_Info = new Contact();
            //contact_Info.SetSalesPerson(this.loggedInUser);
            //mSalesPerson, int mAddressSerial, int mContactId
            //this.loggedInUser, (int)companyID, 
            contact_Info.InitializeSalesPersonInfo(loggedInUser.GetEmployeeId());
            contact_Info.InitializeContactInfo(this.loggedInUser, (int)companyID, (int)this.contactID);
            this.NameText.Text = contact_Info.GetContactName();

            // Contact Gender
            WrapPanel Panel1 = new WrapPanel();
            Label GenderLabel = new Label();
            RegisterName("GenderLabel", GenderLabel);
            GenderLabel.Width = 200;
            GenderLabel.Margin = new System.Windows.Thickness(10, 0, 0, 0);
            GenderLabel.Content = "Gender: ";
            GenderLabel.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            GenderLabel.FontWeight = FontWeights.Bold;
            GenderLabel.FontSize = 14;
            GenderLabel.Padding = new System.Windows.Thickness(10);

            TextBox GenderText = new TextBox();
            RegisterName("GenderText", GenderText);
            GenderText.Margin = new System.Windows.Thickness(20, 0, 0, 0);
            GenderText.Width = 200;
            GenderText.Height = 25;
            GenderText.Padding = new Thickness(0, 0, 0, 5);

            Panel1.Children.Add(GenderLabel);
            Panel1.Children.Add(GenderText);
            GenderText.Text = contact_Info.GetContactGender();
            COL1.Children.Add(Panel1);

            // Contact GetContactBusinessEmail
            WrapPanel Panel2 = new WrapPanel();
            Label Business_EmailLabel = new Label();
            RegisterName("Business_EmailLabel", Business_EmailLabel);
            Business_EmailLabel.Width = 200;
            Business_EmailLabel.Margin = new System.Windows.Thickness(10, 0, 0, 0);
            Business_EmailLabel.Content = "Business_Email: ";
            Business_EmailLabel.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            Business_EmailLabel.FontWeight = FontWeights.Bold;
            Business_EmailLabel.FontSize = 14;
            Business_EmailLabel.Padding = new System.Windows.Thickness(10);

            TextBox Business_EmailText = new TextBox();
            RegisterName("Business_EmailText", Business_EmailText);
            Business_EmailText.Margin = new System.Windows.Thickness(20, 0, 0, 0);
            Business_EmailText.Width = 200;
            Business_EmailText.Height = 25;
            Business_EmailText.Padding = new Thickness(0, 0, 0, 5);

            Panel2.Children.Add(Business_EmailLabel);
            Panel2.Children.Add(Business_EmailText);
            Business_EmailText.Text = contact_Info.GetContactBusinessEmail();
            COL1.Children.Add(Panel2);

            // Contact GetContactPersonalEmail
            string[] contact_Personal_Emails = contact_Info.GetContactPersonalEmails();

            for (int i = 0; i < contact_Info.GetNumberOfSavedContactEmails(); i++)
            {
                WrapPanel Panel3 = new WrapPanel();
                Label Personal_EmailLabel = new Label();
                RegisterName("Personal_EmailLabel", Personal_EmailLabel);
                Personal_EmailLabel.Width = 200;
                Personal_EmailLabel.Margin = new System.Windows.Thickness(10, 0, 0, 0);
                Personal_EmailLabel.Content = "Personal_Email: ";
                Personal_EmailLabel.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
                Personal_EmailLabel.FontWeight = FontWeights.Bold;
                Personal_EmailLabel.FontSize = 14;
                Personal_EmailLabel.Padding = new System.Windows.Thickness(10);

                TextBox Personal_EmailText = new TextBox();
                RegisterName("Personal_EmailText", Personal_EmailText);
                Personal_EmailText.Margin = new System.Windows.Thickness(20, 0, 0, 0);
                Personal_EmailText.Width = 200;
                Personal_EmailText.Height = 25;
                Personal_EmailText.Padding = new Thickness(0, 0, 0, 5);

                Panel3.Children.Add(Personal_EmailLabel);
                Panel3.Children.Add(Personal_EmailText);
                Personal_EmailText.Text = contact_Personal_Emails[i];
                COL1.Children.Add(Panel3);
            }

            // Contact Department
            WrapPanel Panel4 = new WrapPanel();
            Label DepartmentLabel = new Label();
            RegisterName("DepartmentLabel", DepartmentLabel);
            DepartmentLabel.Width = 200;
            DepartmentLabel.Margin = new System.Windows.Thickness(10, 0, 0, 0);
            DepartmentLabel.Content = "Department: ";
            DepartmentLabel.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            DepartmentLabel.FontWeight = FontWeights.Bold;
            DepartmentLabel.FontSize = 14;
            DepartmentLabel.Padding = new System.Windows.Thickness(10);

            TextBox DepartmentText = new TextBox();
            RegisterName("DepartmentText", DepartmentText);
            DepartmentText.Margin = new System.Windows.Thickness(20, 0, 0, 0);
            DepartmentText.Width = 200;
            DepartmentText.Height = 25;
            DepartmentText.Padding = new Thickness(0, 0, 0, 5);

            Panel4.Children.Add(DepartmentLabel);
            Panel4.Children.Add(DepartmentText);
            DepartmentText.Text = contact_Info.GetContactDepartment();
            COL1.Children.Add(Panel4);

            // Contact GetContactPhones
            string[] contact_Phones = contact_Info.GetContactPhones();

            for (int i = 0; i < contact_Info.GetNumberOfSavedContactPhones(); i++)
            {
                WrapPanel Panel5 = new WrapPanel();
                Label Phone_NumberLabel = new Label();
                RegisterName("Phone_NumberLabel", Phone_NumberLabel);
                Phone_NumberLabel.Width = 200;
                Phone_NumberLabel.Margin = new System.Windows.Thickness(10, 0, 0, 0);
                Phone_NumberLabel.Content = "Phone_Number: ";
                Phone_NumberLabel.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
                Phone_NumberLabel.FontWeight = FontWeights.Bold;
                Phone_NumberLabel.FontSize = 14;
                Phone_NumberLabel.Padding = new System.Windows.Thickness(10);

                TextBox Phone_NumberText = new TextBox();
                RegisterName("Phone_NumberText", Phone_NumberText);
                Phone_NumberText.Margin = new System.Windows.Thickness(20, 0, 0, 0);
                Phone_NumberText.Width = 200;
                Phone_NumberText.Height = 25;
                Phone_NumberText.Padding = new Thickness(0, 0, 0, 5);

                Panel5.Children.Add(Phone_NumberLabel);
                Panel5.Children.Add(Phone_NumberText);
                Phone_NumberText.Text = contact_Phones[i];
                COL1.Children.Add(Panel5);
            }

            //contact comments
            List<COMPANY_ORGANISATION_MACROS.CONTACT_COMMENT_STRUCT> contact_Comments = contact_Info.GetContactComments();
          
            if (contact_Comments != null)
            {
                for (int i = 0; i < contact_Comments.Count(); i++)
                {
                    WrapPanel Panel6 = new WrapPanel();
                    Label CommentLabel = new Label();
                    RegisterName("CommentLabel", CommentLabel);
                    CommentLabel.Width = 200;
                    CommentLabel.Margin = new System.Windows.Thickness(10, 0, 0, 0);
                    CommentLabel.Content = "Comment: ";
                    CommentLabel.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
                    CommentLabel.FontWeight = FontWeights.Bold;
                    CommentLabel.FontSize = 14;
                    CommentLabel.Padding = new System.Windows.Thickness(10);

                    TextBox CommentText = new TextBox();
                    RegisterName("CommentText", CommentText);
                    CommentText.Margin = new System.Windows.Thickness(20, 0, 0, 0);
                    CommentText.Width = 200;
                    CommentText.Height = 25;
                    CommentText.Padding = new Thickness(0, 0, 0, 5);

                    Panel6.Children.Add(CommentLabel);
                    Panel6.Children.Add(CommentText);
                    CommentText.Text = contact_Comments[i].comment;
                    COL1.Children.Add(Panel6);
                }
            }
        }

        private void DoneBtn(object sender, RoutedEventArgs e)
        {

        }
    }
}
