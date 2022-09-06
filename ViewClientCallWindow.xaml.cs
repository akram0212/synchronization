using _01electronics_library;
using System.Windows;

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
            companyNameTextBox.IsEnabled = false;
            companyBranchTextBox.IsEnabled = false;

            contactNameTextBox.IsEnabled = false;

            CallDateTextBox.IsEnabled = false;
            CallPurposeTextBox.IsEnabled = false;
            CallResultTextBox.IsEnabled = false;

            additionalDescriptionTextBox.IsEnabled = false;

            companyNameTextBox.Text = clientCall.GetCompanyName();
            companyBranchTextBox.Text = clientCall.GetBranch();

            contactNameTextBox.Text = clientCall.GetContactName();

            CallDateTextBox.Text = clientCall.GetCallDate().ToString();

            CallPurposeTextBox.Text = clientCall.GetCallPurpose();
            CallResultTextBox.Text = clientCall.GetCallResult();

            additionalDescriptionTextBox.Text = clientCall.GetCallNotes();

            callStatusLabel.Content = clientCall.GetCallStatus();
        }
    }
}
