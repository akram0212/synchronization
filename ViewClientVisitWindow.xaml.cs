using _01electronics_library;
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
    /// Interaction logic for ViewClientVisitWindow.xaml
    /// </summary>
    public partial class ViewClientVisitWindow : Window
    {
        ClientVisit visitInfo;
        public ViewClientVisitWindow(ref ClientVisit mVisitInfo)
        {
            visitInfo = mVisitInfo;
            InitializeComponent();

            companyNameTextBox.IsEnabled = false;
            companyBranchTextBox.IsEnabled = false;

            contactNameTextBox.IsEnabled = false;

            visitDateTextBox.IsEnabled = false;
            visitPurposeTextBox.IsEnabled = false;
            visitResultTextBox.IsEnabled = false;
            
            additionalDescriptionTextBox.IsEnabled = false;

            
            companyNameTextBox.Text = visitInfo.GetCompanyName();
            companyBranchTextBox.Text = visitInfo.GetBranch();

            contactNameTextBox.Text = visitInfo.GetContactName();

            visitDateTextBox.Text = visitInfo.GetVisitDate().ToString();

            visitPurposeTextBox.Text = visitInfo.GetVisitPurpose();
            visitResultTextBox.Text = visitInfo.GetVisitResult();

            additionalDescriptionTextBox.Text = visitInfo.GetVisitNotes();

            visitStatusLabel.Content = visitInfo.GetVisitStatus();

        }
      
    }
}
