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
using _01electronics_erp;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for ViewClientCallWindow.xaml
    /// </summary>
    public partial class ViewClientCallWindow : Window
    {
        ClientCall clientCall;
        public ViewClientCallWindow(ref ClientCall mClientCall)
        {
            InitializeComponent();

            clientCall = mClientCall;
        }
    }
}
