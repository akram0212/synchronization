using _01electronics_library;
using System;
using System.Windows;

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

            if (officeMeeting.GetMeetingNotes().ToString() != "NULL")
                additionalDescriptionTextBox.Text = officeMeeting.GetMeetingNotes().ToString();

            approvalRejectionLabel.Content = selectedMeeting.GetMeetingStatus();
        }
    }
}
