﻿using System;
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
using _01electronics_library;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for SignInPage.xaml
    /// </summary>
    public partial class SignInPage : Page
    {
        IntegrityChecks integrityChecker = new IntegrityChecks();

        String employeeEmail;
        String employeePassword;

        Employee loggedInUser;

        public SignInPage()
        {
            InitializeComponent();

            loggedInUser = new Employee();
        }

        private void OnButtonClickedSignIn(object sender, RoutedEventArgs e)
        {
            employeeEmail = employeeEmailTextBox.Text;

            if (!integrityChecker.CheckEmployeeLoginEmailEditBox(employeeEmail, ref employeeEmail, false))
               return;

            loggedInUser.InitializeEmployeeInfo(employeeEmail);

            employeePassword = employeePasswordTextBox.Password;

            //if (!integrityChecker.CheckEmployeePasswordEditBox(employeePassword, loggedInUser.GetEmployeeId()))
              //  return;

            MainWindow mainWindowOpen = new MainWindow(ref loggedInUser);

            NavigationWindow currentWindow = (NavigationWindow)this.Parent;
            currentWindow.Close();

            mainWindowOpen.Show();

            Window.GetWindow(this).Close();
        }

        private void OnButtonClickedSignUp(object sender, RoutedEventArgs e)
        {
            SignUpPage signUp = new SignUpPage();
            this.NavigationService.Navigate(signUp);
        }
    }
}
