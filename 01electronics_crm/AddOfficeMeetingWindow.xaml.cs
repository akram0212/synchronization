using _01electronics_library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
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
        CommonQueries commonQueries;
        Employee loggedInUser;
        OfficeMeeting officeMeeting;
        List<COMPANY_WORK_MACROS.MEETING_PURPOSE_STRUCT> meetingPurposes;

        public AddOfficeMeetingWindow(ref Employee mloggedInUser)
        {
            InitializeComponent();
            loggedInUser = mloggedInUser;

            DayOfWeek today = DateTime.Today.DayOfWeek;

            DayOfWeek firstDay = DayOfWeek.Sunday;
            DayOfWeek lastDay = DayOfWeek.Saturday;

            int minDate;
            int maxDate;

            minDate = today - firstDay + 1;
            maxDate = lastDay - today + 1;
            CalendarDateRange cdr = new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-minDate));
            MeetingDatePicker.BlackoutDates.Add(cdr);

            CalendarDateRange cdr2 = new CalendarDateRange(DateTime.Today.AddDays(maxDate), DateTime.MaxValue);
            MeetingDatePicker.BlackoutDates.Add(cdr2);

            officeMeeting = new OfficeMeeting();
            commonQueries = new CommonQueries();

            meetingPurposes = new List<COMPANY_WORK_MACROS.MEETING_PURPOSE_STRUCT>();

            InitializeMeetingPurposes();
        }

        private bool InitializeMeetingPurposes()
        {
            MeetingPurposeComboBox.Items.Clear();

            if (!commonQueries.GetMeetingPurposes(ref meetingPurposes))
               return false;

            for (int i = 0; i < meetingPurposes.Count; i++)
                MeetingPurposeComboBox.Items.Add(meetingPurposes[i].purpose_name);

            return false;

        }

        private bool CheckMeetingDatePicker()
        {
            if (MeetingDatePicker.SelectedDate == null)
            {
                System.Windows.Forms.MessageBox.Show("Meeting Date must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            officeMeeting.SetMeetingDate(DateTime.Parse(MeetingDatePicker.SelectedDate.ToString()));

            return true;
        }
        private bool CheckMeetingPurposeComboBox()
        {
            if (MeetingPurposeComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Meeting Purpose must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            officeMeeting.SetMeetingPurpose(meetingPurposes[MeetingPurposeComboBox.SelectedIndex].purpose_id, meetingPurposes[MeetingPurposeComboBox.SelectedIndex].purpose_name);
            
            return true;
        }
        private void OnSelChangedMeetingDate(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnSelChangedMeetingPurpose(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnSelChangedadditionalDescription(object sender, RoutedEventArgs e)
        {

        }

        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {
            if (!CheckMeetingDatePicker())
                return;
            if (!CheckMeetingPurposeComboBox())
                return;

            officeMeeting.SetMeetingCaller(loggedInUser);
            officeMeeting.SetMeetingNotes(additionalDescriptionTextBox.Text.ToString());
            officeMeeting.IssueNewMeeting();

            this.Close();
        }
    }
}
