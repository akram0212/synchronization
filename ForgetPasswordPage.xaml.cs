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
using System.Security.Cryptography;

using System.Windows.Forms;
using System.Net.Mail;
using _01electronics_library;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace _01electronics_procurement
{
    /// <summary>
    /// Interaction logic for ForgetPasswordPage.xaml
    /// </summary>
    public partial class ForgetPasswordPage : Page
    {
        IntegrityChecks integrityChecker = new IntegrityChecks();
        protected String errorMessage;

        String employeeEmail;
        int SecretCode;
        int SecretCodeConfirmation;

        public ForgetPasswordPage(ref string Email )
        {
            InitializeComponent();
            employeeEmailTextBox.Text = Email;
        }

        private void OnButtonClickedReset(object sender, RoutedEventArgs e)
        {
            employeeEmail = employeeEmailTextBox.Text;

            if (!integrityChecker.CheckEmployeeLoginEmailEditBox(employeeEmail, ref employeeEmail, false, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (employeeEmailTextBox.Text != null)
            {
                SendSecretCode(employeeEmailTextBox.Text);
            }
            
        }

        private string SendSecretCode(string emailId)
        {
            try
            {
                Random rnd = new Random();
                int Code = rnd.Next(99999, 999999);

                MailMessage mail = new MailMessage();
                mail.To.Add(emailId);
                mail.From = new MailAddress("appsupport@01electronics.net");
                mail.Subject = "Please reset your password " + emailId;
                string userMessage = "";

                userMessage = userMessage + "<br/><b>Login Id:</b> " + emailId;
                userMessage = userMessage + "<br/><b>Secret Code: </b>" + Code;

                string Body = "Dear " + emailId + ", <br/><br/>Need to reset your password?<br/></br> " + " <br/> Use your secret code! <br/> " + userMessage  + " <br/><br/>If you did not forget your password, you can ignore this email.<br/></br> " + "<br/><br/><b>Thanks<b>" + "<br/><b>01 Electronics Team<b> ";
                mail.Body = Body;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.01electronics.net"; //SMTP Server Address of gmail
                smtp.Port = 587;
                smtp.Credentials = new System.Net.NetworkCredential("appsupport@01electronics.net", "!01#elec$app#%");
                // Smtp Email ID and Password For authentication
                //smtp.EnableSsl = true;
                smtp.Send(mail);
                employeeEmailTextBox.Visibility = Visibility.Collapsed;
                employeeEmailLabel.Visibility = Visibility.Collapsed;
                ResetButton.Visibility = Visibility.Collapsed;

                SecretCodeTextBox.Visibility = Visibility.Visible;
                SecretCodeLabel.Visibility = Visibility.Visible;
                ConfirmButton.Visibility = Visibility.Visible;
                SecretCodeConfirmation = Code;
                return "Please check your email for account login detail.";
            }
            catch
            {
                return "Error............";
            }
        }

        private void OnButtonClickedConfirm(object sender, RoutedEventArgs e)
        {
            SecretCode = int.Parse(SecretCodeTextBox.Text);
            if(SecretCode == SecretCodeConfirmation)
            {
                ChangePasswordPage forgetPasswordMail = new ChangePasswordPage(ref employeeEmail);
                this.NavigationService.Navigate(forgetPasswordMail);

            }
        }

      
    }
}
