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
    /// Interaction logic for ViewCompanyWindow.xaml
    /// </summary>
    public partial class ViewCompanyWindow : Window
    {
        public ViewCompanyWindow()
        {
            InitializeComponent();
            companyNameTextBox.IsEnabled = false;
            primaryWorkFieldTextBox.IsEnabled = false;
            secondaryWorkFieldTextBox.IsEnabled = false;
            countryTextBox.IsEnabled = false;
            stateTextBox.IsEnabled = false;
            cityTextBox.IsEnabled = false;
            districtTextBox.IsEnabled = false;
            telephoneTextBox.IsEnabled = false;
            faxTextBox.IsEnabled = false;
        }

        private void OnTextChangedCompanyName(object sender, TextChangedEventArgs e)
        {

        }
    }
}
