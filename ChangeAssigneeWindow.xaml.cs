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
    /// Interaction logic for ChangeAssigneeWindow.xaml
    /// </summary>
    public partial class ChangeAssigneeWindow : Window
    {
        CommonQueries commonQueries;



        public ChangeAssigneeWindow(string mOldAssigneeName)
        {
            commonQueries = new CommonQueries();

            InitializeComponent();
           
        }

        private void OnSelChangedChangeAssignee(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnBtnClickSaveChanges(object sender, RoutedEventArgs e)
        {

        }
    }
}
