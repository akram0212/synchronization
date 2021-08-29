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
    /// Interaction logic for ViewOfficeMeetingWindow.xaml
    /// </summary>
    public partial class ViewOfficeMeetingWindow : Window
    {
        OfficeMeeting officeMeeting;
        public ViewOfficeMeetingWindow(ref OfficeMeeting selectedMeeting)
        {
            InitializeComponent();
            officeMeeting = selectedMeeting;

            MeetingDatePicker.IsEnabled = false;
            SalesEngineerTextBox.IsEnabled = false;
            MeetingPurposeTextBox.IsEnabled = false;
            additionalDescriptionTextBox.IsEnabled = false;

            MeetingDatePicker.SelectedDate = DateTime.Parse(officeMeeting.GetIssueDate().ToString());

            SalesEngineerTextBox.Text = officeMeeting.GetMeetingCallerName().ToString();

            MeetingPurposeTextBox.Text = officeMeeting.GetMeetingPurpose().ToString();

            if(officeMeeting.GetMeetingNotes().ToString() != "NULL")
                additionalDescriptionTextBox.Text = officeMeeting.GetMeetingNotes().ToString();
        }
    }
}
