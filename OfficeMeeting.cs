using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _01electronics_erp;

namespace _01electronics_crm
{
    public class OfficeMeeting
    {
		//SERVER INFO
		private SQLServer sqlDatabase;
		private String sqlQuery;

		//COMMON FUNCTIONS
		private CommonFunctions commonFunctions;

		//COMMON QUERIES
		private CommonQueries commonQueries;

		//MEETING CALLER INFO
		Employee meetingCaller;

		//CALL INFO
		private DateTime meetingDate;
		private DateTime issueDate;

		private String meetingPurpose;

		private String meetingNotes;

		private int meetingPurposeId;
		private int meetingSerial;

		public OfficeMeeting(SQLServer mSqlDatabase)
		{
			sqlDatabase = mSqlDatabase;

			commonFunctions = new CommonFunctions();
			commonQueries = new CommonQueries(mSqlDatabase);

			meetingCaller = new Employee(mSqlDatabase);

			ResetMeetingInfo();
		}
		public OfficeMeeting()
		{
			sqlDatabase = new SQLServer();

			commonFunctions = new CommonFunctions();
			commonQueries = new CommonQueries();

			meetingCaller = new Employee();

			ResetMeetingInfo();

		}

		//////////////////////////////////////////////////////////////////////
		//INITIALIZATION FUNCTIONS
		//////////////////////////////////////////////////////////////////////

		public void SetDatabase(SQLServer mSqlDatabase)
		{
			sqlDatabase = mSqlDatabase;
			meetingCaller.SetDatabase(mSqlDatabase);
		}

		public bool InitializeOfficeMeetingInfo(int mMeetingSerial)
		{
			ResetMeetingInfo();

			meetingSerial = mMeetingSerial;

			String sqlQueryPart1 = @"select employees_info.employee_id,
				office_meetings.meeting_purpose,

				office_meetings.date_of_meeting,
				office_meetings.issue_date,

				meetings_purpose.meeting_purpose,
				office_meetings.meeting_note

				from erp_system.dbo.office_meetings

				inner join erp_system.dbo.employees_info
				on office_meetings.called_by = employees_info.employee_id

				inner join erp_system.dbo.meetings_purpose
	 			on office_meetings.meeting_purpose = meetings_purpose.id
				where office_meetings.meeting_serial = ";

			String sqlQueryPart2 = ";";

			sqlQuery = String.Empty;
			sqlQuery += sqlQueryPart1;
			sqlQuery += GetMeetingSerial();
			sqlQuery += sqlQueryPart2;

			BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

			queryColumns.sql_int = 2;
			queryColumns.sql_datetime = 2;
			queryColumns.sql_string = 2;

			if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
				return false;

			int numericCount = 0;
			int StringCount = 0;
			int dateCount = 0;

			int employeeID = sqlDatabase.rows[0].sql_int[numericCount++];
			meetingPurposeId = sqlDatabase.rows[0].sql_int[numericCount++];

			meetingDate = sqlDatabase.rows[0].sql_datetime[dateCount++];
			issueDate = sqlDatabase.rows[0].sql_datetime[dateCount++];

			meetingPurpose = sqlDatabase.rows[0].sql_string[StringCount++];
			meetingNotes = sqlDatabase.rows[0].sql_string[StringCount++];

			if (!meetingCaller.InitializeEmployeeInfo(employeeID))
				return false;

			return true;
		}

		//////////////////////////////////////////////////////////////////////
		//RESET FUNCTIONS
		//////////////////////////////////////////////////////////////////////

		public void ResetMeetingInfo()
		{
			ResetMeetingSerial();
			ResetMeetingPurpose();
		}

		public void ResetMeetingSerial()
		{
			meetingSerial = 0;
		}
		public void ResetMeetingPurpose()
		{
			meetingPurposeId = 0;
			meetingPurpose = "Other";
		}

		//////////////////////////////////////////////////////////////////////
		//SET FUNCTIONS
		//////////////////////////////////////////////////////////////////////

		public void SetMeetingCaller(int mMeetingCallerId)
		{
			meetingCaller.InitializeEmployeeInfo(mMeetingCallerId);
		}
		public void SetMeetingCaller(Employee mMeetingCaller)
		{
			meetingCaller = mMeetingCaller;
		}

		public void SetMeetingPurpose(int mMeetingPurposeId, String mMeetingPurpose)
		{
			meetingPurposeId = mMeetingPurposeId;
			meetingPurpose = mMeetingPurpose;
		}

		public void SetMeetingNotes(String mMeetingNotes)
		{
			meetingNotes = mMeetingNotes;
		}

		public void SetIssueDate(DateTime mIssueDate)
		{
			issueDate = mIssueDate;
		}
		public void SetMeetingDate(DateTime mMeetingDate)
		{
			meetingDate = mMeetingDate;
		}

		//////////////////////////////////////////////////////////////////////
		//RETURN FUNCTIONS
		//////////////////////////////////////////////////////////////////////

		public int GetMeetingSerial()
		{
			return meetingSerial;
		}

		public int GetMeetingCallerId()
		{
			return meetingCaller.GetEmployeeId();
		}

		public int GetMeetingPurposeId()
		{
			return meetingPurposeId;
		}

		public String GetMeetingCallerName()
		{
			return meetingCaller.GetEmployeeName();
		}

		public String GetMeetingPurpose()
		{
			return meetingPurpose;
		}

		public DateTime GetMeetingDate()
		{
			return meetingDate;
		}
		public DateTime GetIssueDate()
		{
			return issueDate;
		}

		public String GetMeetingNotes()
		{
			return meetingNotes;
		}


		//////////////////////////////////////////////////////////////////////
		//MODIFICATION FUNCTIONS
		//////////////////////////////////////////////////////////////////////

		public bool IssueNewMeeting()
		{
			SetIssueDateToToday();

			if (!GetNewMeetingSerial())
				return false;

			if (!InsertIntoOfficeMeeting())
				return false;

			return true;
		}

		//////////////////////////////////////////////////////////////////////
		//GET ADDITIONAL INSERT DATA FUNCTIONS
		//////////////////////////////////////////////////////////////////////

		public void SetIssueDateToToday()
		{
			issueDate = commonFunctions.GetTodaysDate();
		}

		public bool GetNewMeetingSerial()
		{
			String sqlQueryPart1 = "select max(office_meetings.meeting_serial) from erp_system.dbo.office_meetings;";


			sqlQuery = String.Empty;
			sqlQuery += sqlQueryPart1;

			BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

			queryColumns.sql_int = 1;

			if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
				return false;

			meetingSerial = sqlDatabase.rows[0].sql_int[0] + 1;

			return true;
		}

		//////////////////////////////////////////////////////////////////////
		//INSERT FUNCTIONS
		//////////////////////////////////////////////////////////////////////
		private bool InsertIntoOfficeMeeting()
		{
			String sqlQueryPart1 = @"insert into erp_system.dbo.client_Calls 
                             values(";
			String comma = ",";
			String sqlQueryPart3 = " );";

			sqlQuery = String.Empty;
			sqlQuery += sqlQueryPart1;
			sqlQuery += "'" + GetIssueDate() + "'";
			sqlQuery += comma;
			sqlQuery += GetMeetingSerial();
			sqlQuery += comma;
			sqlQuery += GetMeetingCallerId();
			sqlQuery += comma;
			sqlQuery += GetMeetingPurposeId();
			sqlQuery += comma;
			sqlQuery += "'" + GetMeetingNotes() + "'";
			sqlQuery += comma;
			sqlQuery += "'" + GetMeetingDate() + "'";
			sqlQuery += sqlQueryPart3;

			if (!sqlDatabase.InsertRows(sqlQuery))
				return false;

			return true;
		}
	}
}
