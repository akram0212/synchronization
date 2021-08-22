using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01electronics_library
{
    public class Mission
    {
        //SQL QUERY
        private String sqlQuery;

        //SQL OBJECTS
        private SQLServer sqlDatabase;

        //PETITION INFO
        private int missionSerial;

        private String workOrderNo;

        private String missionType;
        private int missionTypeId;

        private Company company;

        private Employee employeeOnMission;
        private Employee missionRequestor;

        private DateTime missionDate;
        private DateTime issueDate;

        private String comments;

        public Mission()
        {
            sqlDatabase = new SQLServer();

            employeeOnMission = new Employee();
            missionRequestor = new Employee();

            company = new Company();

            comments = String.Empty;
        }

        public bool InitializeMission(int mMissionSerial)
        {
            missionSerial = mMissionSerial;

            String sqlQueryPart1 = @"with get_employee_on_mission	as (select employee_id as benificiary_id,
                                    										name as benificiary_name
                                    									from erp_system.dbo.employees_info),
                                    	get_requestor				as (select employee_id as requestor_id,
                                    										name as requestor_name
                                    									from erp_system.dbo.employees_info)
                                    
                                    select missions.employee_id,
                                    	missions.branch_serial,
                                    	missions.added_by,
                                    	missions.mission_type,

                                    	missions.mission_date,
                                    	missions.date_added,
                                    
                                        mission_types.id
                                    	missions.work_order,
                                    	missions.comments

                                    from erp_system.dbo.missions
                                    inner join get_employee_on_mission
                                    on missions.employee_id = get_employee_on_mission.benificiary_id
                                    inner join get_requestor
                                    on missions.added_by = get_requestor.requestor_name
                                    inner join erp_system.dbo.company_address
                                    on missions.branch_serial = company_address.address_serial
                                    inner join erp_system.dbo.company_name
                                    on company_address.company_serial = company_name.company_serial
                                    inner join erp_system.dbo.mission_types
									on missions.mission_type = mission_types.id
                                    where missions.mission_serial = ";
            String sqlQueryPart2 = ";";

            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += missionSerial;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 4;
            queryColumns.sql_datetime = 2;
            queryColumns.sql_string =4;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            if (!InitializeMissionCompany(sqlDatabase.rows[0].sql_int[1]))
                return false;
            if (!InitializeEmployeeOnMission(sqlDatabase.rows[0].sql_int[2]))
                return false;
            if (!InitializeMissionRequestor(sqlDatabase.rows[0].sql_int[3]))
                return false;

            missionTypeId = sqlDatabase.rows[0].sql_int[4];

            missionDate = sqlDatabase.rows[0].sql_datetime[0];
            issueDate = sqlDatabase.rows[0].sql_datetime[1];

            missionType = sqlDatabase.rows[0].sql_string[0];
            workOrderNo = sqlDatabase.rows[0].sql_string[1];
            comments = sqlDatabase.rows[0].sql_string[2];

            return true;
        }

        public bool InitializeEmployeeOnMission(int mEmployeeOnMissionId)
        {
            if (!employeeOnMission.InitializeEmployeeInfo(mEmployeeOnMissionId))
                return false;

            return true;
        }
        public bool InitializeMissionRequestor(int mMissionRequestor)
        {
            if (!missionRequestor.InitializeEmployeeInfo(mMissionRequestor))
                return false;

            return true;
        }
        public bool InitializeMissionCompany(int mBranchSerial)
        {
            if (!company.InitializeBranchInfo(mBranchSerial))
                return false;

            return true;
        }

        //////////////////////////////////////////////////////////////////////
        //GET FUNCTIONS
        //////////////////////////////////////////////////////////////////////

        public Employee GetEmployeeOnMission()
        {
            return employeeOnMission;
        }
        public Employee GetMissionRequestor()
        {
            return missionRequestor;
        }
        public Company GetMissionCompany()
        {
            return company;
        }

        public int GetMissionSerial()
        {
            return missionSerial;
        }
        public int GetMissionTypeId()
        {
            return missionTypeId;
        }
        public int GetMissionCompanySerial()
        {
            return company.GetCompanySerial();
        }
        public int GetMissionBranchSerial()
        {
            return company.GetAddressSerial();
        }
        
        public int GetMissionCompanyAddressId()
        {
            return company.GetCompanyDistrictId();
        }
        
        public DateTime GetMissionDate()
        {
            return missionDate;
        }
        public DateTime GetIssueDate()
        {
            return issueDate;
        }

        public String GetMissionType()
        {
            return missionType;
        }
        public String GetMissionCompanyName()
        {
            return company.GetCompanyName();
        }
        public String GetMissionWorkOrderNo()
        {
            return workOrderNo;
        }
        public String GetMissionComments()
        {
            return comments;
        }

        //////////////////////////////////////////////////////////////////////
        //SET FUNCTIONS
        //////////////////////////////////////////////////////////////////////


        public void SetEmployeeOnMission(Employee mBeneficiaryPersonnel)
        {
            employeeOnMission = mBeneficiaryPersonnel;
        }
        public void SetMissionRequestor(Employee mMissionRequestor)
        {
            missionRequestor = mMissionRequestor;
        }
        public void SetMissionCompany(Company mCompany)
        {
            company = mCompany;
        }

        public void SetMissionSerial(int mMissionSerial)
        {
            missionSerial = mMissionSerial;
        }
        public void SetMissionTypeId(int mMissionTypeId)
        {
            missionTypeId = mMissionTypeId;
        }

        public void SetMissionDate(DateTime mMissionDate)
        {
            missionDate = mMissionDate;
        }
        public void SetIssueDate(DateTime mIssueDate)
        {
            issueDate = mIssueDate;
        }

        public void SetMissionType(int mMissionTypeId, String mMissionType)
        {
            missionTypeId = missionTypeId;
            missionType = mMissionType;
        }
        public void SetMissionType(String mMissionType)
        {
            missionType = mMissionType;
        }
        public void SetMissionWorkOrderNo(String mWorkOrderNo)
        {
            workOrderNo = mWorkOrderNo;
        }
        public void SetMissionComments(String mComments)
        {
            comments = mComments;
        }

        //////////////////////////////////////////////////////////////////////
        //GET ADDITIONAL INSERT DATA FUNCTIONS
        //////////////////////////////////////////////////////////////////////

        public bool GetNewMissionSerial()
        {
            String sqlQueryPart1 = "select max(missions.mission_serial) from erp_system.dbo.missions;"; ;

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            missionSerial = sqlDatabase.rows[0].sql_int[0] + 1;

            return true;
        }

        public void SetMissionIssueDateToToday()
        {
            DateTime currentDate = DateTime.Now;

            issueDate = currentDate;
        }

        public bool IssueNewMission()
        {
            SetMissionIssueDateToToday();

            if (!GetNewMissionSerial())
                return false;

            return true;
        }
    }
}
