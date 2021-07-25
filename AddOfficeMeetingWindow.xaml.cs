using _01electronics_erp;
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

            CalendarDateRange cdr = new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-3));
            MeetingDatePicker.BlackoutDates.Add(cdr);

            officeMeeting = new OfficeMeeting();
            commonQueries = new CommonQueries();

            meetingPurposes = new List<COMPANY_WORK_MACROS.MEETING_PURPOSE_STRUCT>();

            InitializeMeetingPurposes();
        }

        private bool InitializeMeetingPurposes()
        {
            MeetingPurposeComboBox.Items.Clear();

            //if (!commonQueries.GetMeetingPurposes(ref meetingPurposes))
            //    return false;

            for (int i = 0; i < meetingPurposes.Count; i++)
                MeetingPurposeComboBox.Items.Add(meetingPurposes[i].purpose_name);

            return false;

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

        }
    }
}
