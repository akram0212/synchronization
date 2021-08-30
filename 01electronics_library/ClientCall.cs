using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01electronics_library
{
    public class ClientCall
    {
		//SERVER INFO
		private SQLServer sqlDatabase;
		private String sqlQuery;

		//COMMON FUNCTIONS
		private CommonFunctions commonFunctions;

		//COMMON QUERIES
		private CommonQueries commonQueries;

		//CONTACT PERSON INFO
		private Contact contact;

		//CALL INFO
		private DateTime callDate;
		private DateTime issueDate;

		private String callPurpose;
		private String callResult;
	
		private String callNotes;
		
		private String employeeInitials;
		private String dateID;
		
		private int callSerial;
		private int callPurposeId;
		private int callResultId;

		public ClientCall(SQLServer mSqlDatabase)
		{
			sqlDatabase = mSqlDatabase;

			commonFunctions = new CommonFunctions();
			commonQueries = new CommonQueries(mSqlDatabase);

			contact = new Contact(mSqlDatabase);
		}
		public ClientCall()
		{
			sqlDatabase = new SQLServer();

			commonFunctions = new CommonFunctions();
			commonQueries = new CommonQueries();

			contact = new Contact();

		}

		public void SetDatabase(SQLServer mSqlDatabase)
		{
			sqlDatabase = mSqlDatabase;
			contact.SetDatabase(mSqlDatabase);
		}

		public bool InitializeClientCallInfo(int mCallSerial, int mSalesPersonId)
		{
			if (!InitializeSalesPersonInfo(mSalesPersonId))
				return false;
			if (!InitializeClientCallInfo(mCallSerial))
				return false;

			return true;
		}
		public bool InitializeClientCallInfo(int mCallSerial)
		{
			callSerial = mCallSerial;

			String sqlQueryPart1 = @"select client_calls.branch_serial,

									client_calls.contact_id, 

									calls_purpose.id, 
									calls_result.id, 

									client_calls.issue_date, 
									client_calls.date_of_call, 

									calls_purpose.call_purpose, 
									calls_result.call_result, 

									client_calls.call_note 

							from erp_system.dbo.client_calls 

							inner join erp_system.dbo.calls_purpose 
							on client_calls.call_purpose = calls_purpose.id 

							inner join erp_system.dbo.calls_result 
							on client_calls.call_result = calls_result.id 

							where client_calls.sales_person = ";
			String sqlQueryPart2 = " and client_calls.call_serial = ";
			String sqlQueryPart3 = ";";

			sqlQuery = String.Empty;
			sqlQuery += sqlQueryPart1;
			sqlQuery += GetSalesPersonId();
			sqlQuery += sqlQueryPart2;
			sqlQuery += GetCallSerial();
			sqlQuery += sqlQueryPart3;

			BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

			queryColumns.sql_int = 4;
			queryColumns.sql_datetime = 2;
			queryColumns.sql_string = 3;

			if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
				return false;

			int numericCount = 0;
			int StringCount = 0;
			int dateCount = 0;

			int branchSerial = sqlDatabase.rows[0].sql_int[numericCount++];
			int contactId = sqlDatabase.rows[0].sql_int[numericCount++];

			callPurposeId = sqlDatabase.rows[0].sql_int[numericCount++];
			callResultId = sqlDatabase.rows[0].sql_int[numericCount++];

			issueDate = sqlDatabase.rows[0].sql_datetime[dateCount++];
			callDate = sqlDatabase.rows[0].sql_datetime[dateCount++];

			callPurpose = sqlDatabase.rows[0].sql_string[StringCount++];
			callResult = sqlDatabase.rows[0].sql_string[StringCount++];

			callNotes = sqlDatabase.rows[0].sql_string[StringCount++];

			if (!InitializeBranchInfo(branchSerial))
				return false;
			if (!InitializeContactInfo(contactId))
				return false;

			return true;
		}
		public bool InitializeSalesPersonInfo(int mEmployeeId)
		{
			return contact.InitializeSalesPersonInfo(mEmployeeId);
		}
		public bool InitializeCompanyInfo(int mCompanySerial)
		{
			return contact.InitializeCompanyInfo(mCompanySerial);
		}
		public bool InitializeBranchInfo(int mAddressSerial)
		{
			return contact.InitializeBranchInfo(mAddressSerial);
		}
		public bool InitializeContactInfo(int mContactId)
		{
			return contact.InitializeContactInfo(mContactId);
		}

		//////////////////////////////////////////////////////////////////////
		//SET FUNCTIONS
		//////////////////////////////////////////////////////////////////////

		public void SetCallPurpose(int mCallPurposeId, String mCallPurpose)
		{
			callPurposeId = mCallPurposeId;
			callPurpose = mCallPurpose;
		}

		public void SetCallResult(int mCallResultId, String mCallResult)
		{
			callResultId = mCallResultId;
			callResult = mCallResult;
		}

		public void SetCallNotes(String mCallNotes)
		{
			callNotes = mCallNotes;
		}

		public void SetIssueDate(DateTime mIssueDate)
		{
			issueDate = mIssueDate;
		}
		public void SetCallDate(DateTime mCallDate)
		{
			callDate = mCallDate;
		}

		//////////////////////////////////////////////////////////////////////
		//RETURN FUNCTIONS
		//////////////////////////////////////////////////////////////////////

		public int GetCompanySerial()
		{
			return contact.GetCompanySerial();
		}
		public String GetCompanyName()
		{
			return contact.GetCompanyName();
		}
		public int GetAddressSerial()
		{
			return contact.GetAddressSerial();
		}
		public String GetBranch()
		{
			return contact.GetCompanyDistrict() + ", " + contact.GetCompanyCity() + ", " + contact.GetCompanyState() + ", " + contact.GetCompanyCountry();
		}
		public int GetContactId()
		{
			return contact.GetContactId();
		}
		public String GetContactName()
		{
			return contact.GetContactName();
		}
		public int GetSalesPersonId()
		{
			return contact.GetSalesPersonId();
		}

		public int GetCallSerial()
		{
			return callSerial;
		}
		public int GetCallPurposeId()
		{
			return callPurposeId;
		}
		public int GetCallResultId()
		{
			return callResultId;
		}

		public String GetCallPurpose()
		{
			return callPurpose;
		}
		public String GetCallResult()
		{
			return callResult;
		}

		public DateTime GetCallDate()
		{
			return callDate;
		}
		public DateTime GetIssueDate()
		{
			return issueDate;
		}

		public String GetCallNotes()
		{
			return callNotes;
		}

		//////////////////////////////////////////////////////////////////////
		//MODIFICATION FUNCTIONS
		//////////////////////////////////////////////////////////////////////

		public bool IssueNewCall()
		{
			SetIssueDateToToday();

			if (!GetNewCallSerial())
				return false;

			if (!InsertIntoClientCall())
				return false;

			return true;
		}
		public void ConfirmCall(int mCallResultId, String mCallResult)
		{
			callResultId = mCallResultId;
			callResult = mCallResult;
		}
		public void CancelCall()
		{
			callResultId = COMPANY_WORK_MACROS.CANCELLED_VISIT_PLAN;
			callResult = "Cancelled";
		}
		public void RescheduleCall()
		{
			callResultId = COMPANY_WORK_MACROS.RESCHEDULED_VISIT_PLAN;
			callResult = "Rescheduled";
		}

		//////////////////////////////////////////////////////////////////////
		//GET ADDITIONAL INSERT DATA FUNCTIONS
		//////////////////////////////////////////////////////////////////////

		public void SetIssueDateToToday()
		{
			issueDate = commonFunctions.GetTodaysDate();
		}

		public bool GetNewCallSerial()
		{
			String sqlQueryPart1 = "select max(client_calls.call_serial) from erp_system.dbo.client_calls where client_calls.sales_person = ";
			String sqlQueryPart2 = ";";

			sqlQuery = String.Empty;
			sqlQuery += sqlQueryPart1;
			sqlQuery += GetSalesPersonId();
			sqlQuery += sqlQueryPart2;

			BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

			queryColumns.sql_int = 1;

			if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
				return false;

			callSerial = sqlDatabase.rows[0].sql_int[0] + 1;

			return true;
		}

		//////////////////////////////////////////////////////////////////////
		//INSERT FUNCTIONS
		//////////////////////////////////////////////////////////////////////
		private bool InsertIntoClientCall()
		{
			String sqlQueryPart1 = @"insert into erp_system.dbo.client_Calls 
                             values(";
			String comma = ",";
			String sqlQueryPart3 = " );";

			sqlQuery = String.Empty;
			sqlQuery += sqlQueryPart1;
			sqlQuery += "'" + GetIssueDate() + "'";
			sqlQuery += comma;
			sqlQuery += GetSalesPersonId();
			sqlQuery += comma;
			sqlQuery += GetCallSerial();
			sqlQuery += comma;
			sqlQuery += GetAddressSerial();
			sqlQuery += comma;
			sqlQuery += GetContactId();
			sqlQuery += comma;
			sqlQuery += GetCallPurposeId();
			sqlQuery += comma;
			sqlQuery += GetCallResultId();
			sqlQuery += comma;
			sqlQuery += "'" + GetCallNotes() + "'";
			sqlQuery += comma;
			sqlQuery += "'" + GetCallDate() + "'";
			sqlQuery += sqlQueryPart3;

			if (!sqlDatabase.InsertRows(sqlQuery))
				return false;

			return true;
		}
	}
}
