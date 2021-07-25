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
    /// Interaction logic for ViewOfficeMeetingWindow.xaml
    /// </summary>
    public partial class ViewOfficeMeetingWindow : Window
    {
        public ViewOfficeMeetingWindow(ref OfficeMeeting selectedMeeting)
        {
            InitializeComponent();
        }
    }
}
