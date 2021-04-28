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
using _01electronics_erp;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for UserPortalPage.xaml
    /// </summary>
    /// 
    public partial class UserPortalPage : Page
    {

        private Employee loggedInUser;

        public UserPortalPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;

            InitializeEmployeeDashboard();
        }
        
        private void InitializeEmployeeDashboard()
        {
            employeeNameLabel.Content = loggedInUser.GetEmployeeName();
            employeeBirthdateLabel.Content = loggedInUser.GetEmployeeBirthDate();
            employeeJoiningDateLabel.Content = loggedInUser.GetEmployeeJoinDate();
            employeeDepartmentLabel.Content = loggedInUser.GetEmployeeDepartment();
            employeeTeamLabel.Content = loggedInUser.GetEmployeeTeam();
            employeeBusinessEmailLabel.Content = loggedInUser.GetEmployeeBusinessEmail();
            employeePersonalEmailLabel.Content = loggedInUser.GetEmployeePersonalEmail();
        }

        private void OnButtonClickedMyProfile(object sender, RoutedEventArgs e)
        {
            UserPortalPage userPortal = new UserPortalPage(ref loggedInUser);
            this.NavigationService.Navigate(userPortal);
        }

        private void OnButtonClickedContacts(object sender, RoutedEventArgs e)
        {
            
        }

        private void OnButtonClickedOrders(object sender, RoutedEventArgs e)
        {

        }
        private void OnButtonClickedOffers(object sender, RoutedEventArgs e)
        {

        }
        private void OnButtonClickedRFQs(object sender, RoutedEventArgs e)
        {

        }
        private void OnButtonClickedVisits(object sender, RoutedEventArgs e)
        {

        }
        private void OnButtonClickedCalls(object sender, RoutedEventArgs e)
        {

        }
        private void OnButtonClickedMeetings(object sender, RoutedEventArgs e)
        {

        }
        private void OnButtonClickedStatistics(object sender, RoutedEventArgs e)
        {

        }
    }
}
