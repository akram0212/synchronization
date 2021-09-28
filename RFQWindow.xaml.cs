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
using System.Windows.Navigation;
using _01electronics_library;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for RFQWindow.xaml
    /// </summary>
    public partial class RFQWindow : NavigationWindow
    {
        public RFQWindow(ref Employee mLoggedInUser, ref RFQ mRFQ, int mViewAddCondition)
        {
            InitializeComponent();
            RFQBasicInfoPage rfqsPage = new RFQBasicInfoPage(ref mLoggedInUser,ref mRFQ, mViewAddCondition);
            this.NavigationService.Navigate(rfqsPage);
        }

        public RFQWindow(ref Employee mLoggedInUser, ref RFQ mRFQ, int mViewAddCondition, bool directlyNavigateToFilesPage)
        {
            InitializeComponent();
            RFQUploadFilesPage rfqsPage = new RFQUploadFilesPage(ref mLoggedInUser, ref mRFQ, mViewAddCondition);
            this.NavigationService.Navigate(rfqsPage);
        }
    }
}
