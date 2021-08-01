using _01electronics_library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01electronics_crm
{
    public class ClientVisit
    {
		private SQLServer sqlDatabase;
		private String sqlQuery;

		private CommonFunctions commonFunctions;
		private CommonQueries commonQueries;

		private Contact contact;
		Employee LoggedInUser;

		//VISIT PLAN INFO
		protected const String visitIdString = "RFQ-0001-XXXX.XXXX-DDMMYYYY";
		private String visitIdCString;

		private DateTime visitDate;
		private DateTime issueDate;

		private	String visitPurpose;
		private String visitResult;

		private String visitNotes;

		private String employeeInitials;
		private String dateID;

		private int visitSerial;
		private int visitPurposeId;
		private int visitResultId;

		public ClientVisit()
        {
			sqlDatabase = new SQLServer();

			commonFunctions = new CommonFunctions();
			commonQueries = new CommonQueries();

			contact = new Contact();

		}
		public ClientVisit(Employee mLoggedInUser)
		{
			sqlDatabase = new SQLServer();

			commonFunctions = new CommonFunctions();
			commonQueries = new CommonQueries();

			contact = new Contact();
			LoggedInUser = mLoggedInUser;

		}
		public ClientVisit(SQLServer mSqlDatabase)
		{
			sqlDatabase = mSqlDatabase;

			commonFunctions = new CommonFunctions();
			commonQueries = new CommonQueries(mSqlDatabase);

			contact = new Contact(mSqlDatabase);
		}

		public void SetDatabase(SQLServer mSqlDatabase)
		{
			sqlDatabase = mSqlDatabase;
			contact.SetDatabase(sqlDatabase);
		}

		public bool InitializeClientVisitInfo(int mVisitSerial, int mSalesPersonId)
		{
			if (!InitializeSalesPersonInfo(mSalesPersonId))
				return false;
			if (!InitializeClientVisitInfo(mVisitSerial))
				return false;

			return true;
		}

		public bool InitializeClientVisitInfo(int mVisitSerial)
		{
			visitSerial = mVisitSerial;

			String sqlQueryPart1 = @"select client_visits.branch_serial, 
									client_visits.contact_id, 

									visits_purpose.id, 
									visits_result.id, 

									client_visits.issue_date, 
									client_visits.date_of_visit, 

									visits_purpose.visit_purpose, 
									visits_result.visit_result, 

									client_visits.visit_note 

							from erp_system.dbo.client_visits 

							inner join erp_system.dbo.visits_purpose 
							on client_visits.visit_purpose = visits_purpose.id 

							inner join erp_system.dbo.visits_result 
							on client_visits.visit_result = visits_result.id 

							where client_visits.sales_person = ";
            String sqlQueryPart2 = " and client_visits.visit_serial = ";
            String sqlQueryPart3 = ";";

			sqlQuery = String.Empty;
			sqlQuery += sqlQueryPart1;
			sqlQuery += GetSalesPersonId();
			sqlQuery += sqlQueryPart2;
			sqlQuery += GetVisitSerial();
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

			visitPurposeId = sqlDatabase.rows[0].sql_int[numericCount++];
			visitResultId = sqlDatabase.rows[0].sql_int[numericCount++];

			issueDate = sqlDatabase.rows[0].sql_datetime[dateCount++];
			visitDate = sqlDatabase.rows[0].sql_datetime[dateCount++];

		    visitPurpose = sqlDatabase.rows[0].sql_string[StringCount++];
		    visitResult = sqlDatabase.rows[0].sql_string[StringCount++];

		    visitNotes = sqlDatabase.rows[0].sql_string[StringCount++];

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

		public void SetVisitPurpose(int mVisitPurposeId, String mVisitPurpose)
		{
			visitPurposeId = mVisitPurposeId;
			visitPurpose = mVisitPurpose;
		}

		public void SetVisitResult(int mVisitResultId, String mVisitResult)
		{
			visitResultId = mVisitResultId;
			visitResult = mVisitResult;
		}

		public void SetVisitNotes(String mVisitNotes)
		{
			visitNotes = mVisitNotes;
		}

		public void SetIssueDate(DateTime mIssueDate)
		{
			issueDate = mIssueDate;
		}
		public void SetVisitDate(DateTime mVisitDate)
		{
			visitDate = mVisitDate;
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

		public int GetVisitSerial()
		{
			return visitSerial;
		}
		public int GetVisitPurposeId()
		{
			return visitPurposeId;
		}
		public int GetVisitResultId()
		{
			return visitResultId;
		}

		public String GetVisitPurpose()
		{
			return visitPurpose;
		}
		public String GetVisitResult()
		{
			return visitResult;
		}

		public DateTime GetVisitDate()
		{
			return visitDate;
		}
		public DateTime GetIssueDate()
		{
			return issueDate;
		}

		public String GetVisitNotes()
		{
			return visitNotes;
		}

		//////////////////////////////////////////////////////////////////////
		//MODIFICATION FUNCTIONS
		//////////////////////////////////////////////////////////////////////

		public bool IssueNewVisit()
		{
			SetIssueDateToToday();

			if (!GetNewVisitSerial())
				return false;

			if (!InsertIntoClientVisit())
				return false;

			return true;
		}
		public void ConfirmVisit(int mVisitResultId, String mVisitResult)
		{
			visitResultId = mVisitResultId;
			visitResult = mVisitResult;
		}
		public void CancelVisit()
		{
			visitResultId = COMPANY_WORK_MACROS.CANCELLED_VISIT_PLAN;
			visitResult = "Cancelled";
		}
		public void RescheduleVisit()
		{
			visitResultId = COMPANY_WORK_MACROS.RESCHEDULED_VISIT_PLAN;
			visitResult = "Rescheduled";
		}

		//////////////////////////////////////////////////////////////////////
		//GET ADDITIONAL INSERT DATA FUNCTIONS
		//////////////////////////////////////////////////////////////////////

		public void SetIssueDateToToday()
		{
			issueDate = commonFunctions.GetTodaysDate();
		}
		public void SetVisitSerial(int mVisitSerial)
		{
			visitSerial = mVisitSerial;
		}
		public bool GetNewVisitSerial()
		{
			String sqlQueryPart1 = "select max(client_visits.visit_serial) from erp_system.dbo.client_visits where client_visits.sales_person = ";
			String sqlQueryPart2 = ";";


			sqlQuery = String.Empty;
			sqlQuery += sqlQueryPart1;
			sqlQuery += GetSalesPersonId();
			sqlQuery += sqlQueryPart2;

			BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

			queryColumns.sql_int = 1;

			if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
				return false;

			SetVisitSerial(sqlDatabase.rows[0].sql_int[0] + 1);

			return true;
		}

		//////////////////////////////////////////////////////////////////////
		//INSERT FUNCTIONS
		//////////////////////////////////////////////////////////////////////
		private bool InsertIntoClientVisit()
		{
			String sqlQueryPart1 = @"insert into erp_system.dbo.client_visits 
                             values(";
			String comma = ",";
			String sqlQueryPart3 = " );";

			sqlQuery = String.Empty;
			sqlQuery += sqlQueryPart1;
			sqlQuery += "'" + GetIssueDate() + "'";
			sqlQuery += comma;
			sqlQuery += GetSalesPersonId();
			sqlQuery += comma;
			sqlQuery += GetVisitSerial();
			sqlQuery += comma;
			sqlQuery += GetAddressSerial();
			sqlQuery += comma;
			sqlQuery += GetContactId();
			sqlQuery += comma;
			sqlQuery += GetVisitPurposeId();
			sqlQuery += comma;
			sqlQuery += GetVisitResultId();
			sqlQuery += comma;
			sqlQuery += "'" + GetVisitNotes() + "'";
			sqlQuery += comma;
			sqlQuery += "'" + GetVisitDate() + "'";
			sqlQuery += sqlQueryPart3;

			if (!sqlDatabase.InsertRows(sqlQuery))
				return false;

			return true;
		}
	}
}
