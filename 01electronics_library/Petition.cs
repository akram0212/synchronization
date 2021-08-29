using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01electronics_library
{
    public class Petition
    {
        //SQL QUERY
        private String sqlQuery;

        //SQL OBJECTS
        private SQLServer sqlDatabase;

        //PETITION INFO
        private int petitionSerial;

        private int petitionTypeId;
        private String petitionType;

        private Employee beneficiaryPersonnel;
        private Employee petitionRequestor;

        private DateTime requestedDate;
        private DateTime issueDate;

        private String comments;

        public Petition()
        {
            sqlDatabase = new SQLServer();

            beneficiaryPersonnel = new Employee();
            petitionRequestor = new Employee();

            comments = String.Empty;
        }

        public bool InitializePetition(int mPetitionSerial)
        {
            petitionSerial = mPetitionSerial;

            String sqlQueryPart1 = @"with get_benificiary_personnel	as (select employee_id as benificiary_id,
                                    										name as benificiary_name
                                    									from erp_system.dbo.employees_info),
                                    get_requestor					as (select employee_id as requestor_id,
                                    										name as requestor_name
                                    									from erp_system.dbo.employees_info)
                                    select attendance_excuses.id as petition_type_id,
                                    		get_benificiary_personnel.benificiary_id,
                                    		get_requestor.requestor_id,
                                    		petitions_requests.requested_day,
                                    		petitions_requests.date_added,
                                    		attendance_excuses.attendance_excuse,
											petitions_requests.comments
                                    from erp_system.dbo.petitions_requests		
                                    inner join erp_system.dbo.attendance_excuses
                                    on petitions_requests.petition_type = attendance_excuses.id
                                    inner join get_benificiary_personnel
                                    on petitions_requests.benficiary_personnel = get_benificiary_personnel.benificiary_id
                                    inner join get_requestor
                                    on petitions_requests.requested_by = get_requestor.requestor_id
                                    where petitions_requests.request_serial = ";
            String sqlQueryPart2 = ";";

            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += petitionSerial;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 3;
            queryColumns.sql_datetime = 2;
            queryColumns.sql_string = 2;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            petitionTypeId = sqlDatabase.rows[0].sql_int[0];

            if (!InitializeBenificiaryPersonnel(sqlDatabase.rows[0].sql_int[1]))
                return false;
            if (!InitializePetitionRequestor(sqlDatabase.rows[0].sql_int[2]))
                return false;

            requestedDate = sqlDatabase.rows[0].sql_datetime[0];
            issueDate = sqlDatabase.rows[0].sql_datetime[1];

            petitionType = sqlDatabase.rows[0].sql_string[0];

            comments = sqlDatabase.rows[0].sql_string[1];

            return true;
        }

        public bool InitializeBenificiaryPersonnel(int mBenificiaryPersonnelId)
        {
            if (!beneficiaryPersonnel.InitializeEmployeeInfo(mBenificiaryPersonnelId))
                return false;

            return true;
        }

        public bool InitializePetitionRequestor(int mPetitionRequestor)
        {
            if (!petitionRequestor.InitializeEmployeeInfo(mPetitionRequestor))
                return false;

            return true;
        }

        //////////////////////////////////////////////////////////////////////
        //GET FUNCTIONS
        //////////////////////////////////////////////////////////////////////
        
        public Employee GetBenificiaryPersonnel()
        {
            return beneficiaryPersonnel;
        }

        public Employee GetPetitionRequestor()
        {
            return petitionRequestor;
        }

        public int GetPetitionSerial()
        {
            return petitionSerial;
        }

        public DateTime GetRequestedDate()
        {
            return requestedDate;
        }

        public DateTime GetIssueDate()
        {
            return issueDate;
        }

        public int GetPetitionTypeId()
        {
            return petitionTypeId;
        }

        public String GetPetitionType()
        {
            return petitionType;
        }

        public String GetPetitionComments()
        {
            return comments;
        }

        //////////////////////////////////////////////////////////////////////
        //SET FUNCTIONS
        //////////////////////////////////////////////////////////////////////

        public void SetBenificiaryPersonnel(Employee mBeneficiaryPersonnel)
        {
            beneficiaryPersonnel = mBeneficiaryPersonnel;
        }

        public void SetPetitionRequestor(Employee mPetitionRequestor)
        {
            petitionRequestor = mPetitionRequestor;
        }

        public void SetPetitionSerial(int mPetitionSerial)
        {
            petitionSerial = mPetitionSerial;
        }

        public void SetRequestedDate(DateTime mRequestedDate)
        {
            requestedDate = mRequestedDate;
        }

        public void SetIssueDate(DateTime mIssueDate)
        {
            issueDate = mIssueDate;
        }

        public void SetPetitionTypeId(int mPetitionTypeId)
        {
            petitionTypeId = mPetitionTypeId;
        }

        public void SetPetitionTypeString(String mPetitionType)
        {
            petitionType = mPetitionType;
        }

        public void SetPetitionType(int mPetitionTypeId, String mPetitionType)
        {
            petitionTypeId = mPetitionTypeId;
            petitionType = mPetitionType;
        }

        public void SetPetitionComments(String mComments)
        {
            comments = mComments;
        }

        //////////////////////////////////////////////////////////////////////
        //GET ADDITIONAL INSERT DATA FUNCTIONS
        //////////////////////////////////////////////////////////////////////
        
        public bool GetNewPetitionSerial()
        {
            String sqlQueryPart1 = "select max(petitions_requests.request_serial) from erp_system.dbo.petitions_requests;";;

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            petitionSerial = sqlDatabase.rows[0].sql_int[0] + 1;

            return true;
        }

        public void SetPetitionIssueDateToToday()
        {
            DateTime currentDate = DateTime.Now;

            issueDate = currentDate;
        }

        public bool IssueNewPetition()
        {
            SetPetitionIssueDateToToday();

            if (!GetNewPetitionSerial())
                return false;

            if (!InsertIntoPetitionsRequest())
                return false;

            return true;
        }

        //////////////////////////////////////////////////////////
        /// INSERT FUNCTIONS
        //////////////////////////////////////////////////////////
        public bool InsertIntoPetitionsRequest()
        {
            String sqlQueryPart1 = "insert into erp_system.dbo.petitions_requests values("; ;
            String sqlQueryComma = ","; ;
            String sqlQueryCommaApostrophe = ",'"; ;
            String sqlQueryApostropheCommaApostrophe = "','"; ;
            String sqlQueryPart2 = "', getdate());"; ;

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetPetitionSerial();
            sqlQuery += sqlQueryComma;
            sqlQuery += GetPetitionTypeId();
            sqlQuery += sqlQueryComma;
            sqlQuery += GetBenificiaryPersonnel().GetEmployeeId();
            sqlQuery += sqlQueryComma;
            sqlQuery += GetPetitionRequestor().GetEmployeeId();
            sqlQuery += sqlQueryCommaApostrophe;
            sqlQuery += GetRequestedDate().ToString("yyyy-MM-dd");
            sqlQuery += sqlQueryApostropheCommaApostrophe;
            sqlQuery += GetPetitionComments();
            sqlQuery += sqlQueryPart2;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
    }
}
