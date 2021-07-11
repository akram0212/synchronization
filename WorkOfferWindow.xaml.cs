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
using System.Windows.Shapes;
using System.Windows.Navigation;
using _01electronics_erp;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for WorkOfferWindow.xaml
    /// </summary>
    public partial class WorkOfferWindow : NavigationWindow
    {
        public WorkOfferWindow(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            WorkOfferBasicInfoPage workOfferBasicInfoPage = new WorkOfferBasicInfoPage(ref mLoggedInUser);
            this.NavigationService.Navigate(workOfferBasicInfoPage);
        }
    }
}
