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
    /// Interaction logic for AddOfficeMeetingWindow.xaml
    /// </summary>
    public partial class AddOfficeMeetingWindow : Window
    {
        public AddOfficeMeetingWindow(ref Employee loggedInUser)
        {
            InitializeComponent();
        }
    }
}
