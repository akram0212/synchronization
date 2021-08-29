using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _01electronics_library
{
    public class Vacation
    {
        //SQL QUERY
        private String sqlQuery;

        //SQL OBJECTS
        private SQLServer sqlDatabase;

        //PETITION INFO
        private int vacationSerial;

        private String vacationType;
        private int vacationTypeId;

        private String vacationStatus;
        private int vacationStatusId;

        private Employee beneficiaryPersonnel;
        private Employee vacationRequestor;

        private List<HUMAN_RESOURCE_MACROS.VACATION_APPROVAL_REJECTION_CLASS_STRUCT> vacationApprovalsRejections;

        private DateTime vacationStartDate;
        private DateTime vacationEndDate;

        private DateTime issueDate;
        private DateTime expiryDate;

        private String requestComments;

        public Vacation()
        {
            sqlDatabase = new SQLServer();

            beneficiaryPersonnel = new Employee();
            vacationRequestor = new Employee();
            vacationApprovalsRejections = new List<HUMAN_RESOURCE_MACROS.VACATION_APPROVAL_REJECTION_CLASS_STRUCT>();

            requestComments = String.Empty;

            ResetVacationInfo();
        }

        public bool InitializeVacation(int mVacationSerial)
        {
            vacationSerial = mVacationSerial;

            String sqlQueryPart1 = @"with get_benificiary_personnel	as (select employee_id as benificiary_id,
                                    	 									name as benificiary_name
                                    	 								from erp_system.dbo.employees_info),
                                    	 get_requestor				as (select employee_id as requestor_id,
                                    	 									name as requestor_name
                                    	 								from erp_system.dbo.employees_info),
                                    	get_approver				as (select employee_id as approver_id,
                                    	 									name as approver_name
                                    	 								from erp_system.dbo.employees_info)
                                    
                                    select get_benificiary_personnel.benificiary_id,
                                    		get_requestor.requestor_id,
                                    		get_approver.approver_id,
                                    		attendance_excuses.id,
                                    		vacation_leave_request_status.id,
                                    
                                    		vacation_leave_requests.leave_start_date,
                                    		vacation_leave_requests.leave_end_date,
                                    		
                                    		vacation_leave_requests.date_added as issue_date,
                                    		vacation_leave_approvals_rejections.date_added as approval_date,
                                    		vacation_leave_requests.expiry_date,
                                    		
                                    		attendance_excuses.attendance_excuse,
                                    		vacation_leave_request_status.request_status,
                                    		vacation_leave_requests.comments as request_comment,
                                    		vacation_leave_approvals_rejections.comments as approval_comments
                                    
                                    from erp_system.dbo.vacation_leave_requests
                                    
                                    left join erp_system.dbo.vacation_leave_approvals_rejections
                                    on vacation_leave_requests.request_serial = vacation_leave_approvals_rejections.request_serial
                                    
                                    inner join erp_system.dbo.attendance_excuses
                                    on vacation_leave_requests.request_type = attendance_excuses.id
                                    
                                    inner join erp_system.dbo.vacation_leave_request_status
                                    on vacation_leave_requests.request_status = vacation_leave_request_status.id
                                    
                                    inner join get_benificiary_personnel
                                    on vacation_leave_requests.benficiary_personnel = get_benificiary_personnel.benificiary_id
                                    inner join get_requestor
                                    on vacation_leave_requests.requested_by = get_requestor.requestor_id
                                    left join get_approver
                                    on vacation_leave_approvals_rejections.approving_personnel = get_approver.approver_id
                                    where vacation_leave_requests.request_serial = ";
            String sqlQueryPart2 = ";";

            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += vacationSerial;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 5;
            queryColumns.sql_datetime = 5;
            queryColumns.sql_string = 4;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            if (!InitializeBeneficiaryPersonnel(sqlDatabase.rows[0].sql_int[0]))
                return false;
            if (!InitializeVacationRequestor(sqlDatabase.rows[0].sql_int[1]))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                if (sqlDatabase.rows[i].sql_int[2] != 0)
                    if (!InitializeVacationApproval(sqlDatabase.rows[i].sql_int[2], sqlDatabase.rows[i].sql_string[3], sqlDatabase.rows[i].sql_datetime[3]))
                        return false;

            }
            vacationTypeId = sqlDatabase.rows[0].sql_int[3];
            vacationStatusId = sqlDatabase.rows[0].sql_int[4];

            vacationStartDate = sqlDatabase.rows[0].sql_datetime[0];
            vacationEndDate = sqlDatabase.rows[0].sql_datetime[1];

            issueDate = sqlDatabase.rows[0].sql_datetime[2];

            expiryDate = sqlDatabase.rows[0].sql_datetime[4];

            vacationType = sqlDatabase.rows[0].sql_string[0];
            vacationStatus = sqlDatabase.rows[0].sql_string[1];

            requestComments = sqlDatabase.rows[0].sql_string[2];

            return true;
        }

        public bool InitializeBeneficiaryPersonnel(int mBeneficiaryPersonnelId)
        {
            if (!beneficiaryPersonnel.InitializeEmployeeInfo(mBeneficiaryPersonnelId))
                return false;

            return true;
        }
        public bool InitializeVacationRequestor(int mVacationRequestor)
        {
            if (!vacationRequestor.InitializeEmployeeInfo(mVacationRequestor))
                return false;

            return true;
        }
        public bool InitializeVacationApproval(int mVacationApprover, String mApprovalRejectionComment, DateTime mApprovalRejectionDate)
        {
            HUMAN_RESOURCE_MACROS.VACATION_APPROVAL_REJECTION_CLASS_STRUCT currentApprovalItem = new HUMAN_RESOURCE_MACROS.VACATION_APPROVAL_REJECTION_CLASS_STRUCT();
            currentApprovalItem.approver = new Employee();

            if (!currentApprovalItem.approver.InitializeEmployeeInfo(mVacationApprover))
                return false;

            currentApprovalItem.approvalRejectionComments = mApprovalRejectionComment;
            currentApprovalItem.approvalRejectionDate = mApprovalRejectionDate;

            vacationApprovalsRejections.Add(currentApprovalItem);

            return true;
        }

        //////////////////////////////////////////////////////////////////////
        //RESET FUNCTIONS
        //////////////////////////////////////////////////////////////////////

        public void ResetVacationInfo()
        {
            ResetVacationType();
            ResetVacationStatus();
        }

        public void ResetVacationType()
        {
            vacationTypeId = 0;
            vacationType = "0";
        }
        public void ResetVacationStatus()
        {
            vacationStatusId = HUMAN_RESOURCE_MACROS.PENDING_VACATION_STATUS;
            vacationStatus = "Pending";
        }

        //////////////////////////////////////////////////////////////////////
        //GET FUNCTIONS
        //////////////////////////////////////////////////////////////////////

        public Employee GetBeneficiaryPersonnel()
        {
            return beneficiaryPersonnel;
        }
        public Employee GetVacationRequestor()
        {
            return vacationRequestor;
        }

        public List<HUMAN_RESOURCE_MACROS.VACATION_APPROVAL_REJECTION_CLASS_STRUCT> GetVacationApprovalsRejections()
        {
            return vacationApprovalsRejections;
        }
        public List<Employee> GetVacationApprovers()
        {
            List<Employee> returnVector = new List<Employee>();

            for (int i = 0; i < vacationApprovalsRejections.Count; i++)
                returnVector.Add(vacationApprovalsRejections[i].approver);

            return returnVector;
        }
        public List<String> GetVacationApprovalRejectionComments()
        {
            List<String> returnVector = new List<String>();

            for (int i = 0; i < vacationApprovalsRejections.Count; i++)
                returnVector.Add(vacationApprovalsRejections[i].approvalRejectionComments);

            return returnVector;
        }
        public List<DateTime> GetVacationApprovalRejectionDates()
        {
            List<DateTime> returnVector = new List<DateTime>();

            for (int i = 0; i < vacationApprovalsRejections.Count; i++)
                returnVector.Add(vacationApprovalsRejections[i].approvalRejectionDate);

            return returnVector;
        }

        public int GetVacationSerial()
        {
            return vacationSerial;
        }
        public int GetVacationTypeId()
        {
            return vacationTypeId;
        }

        public int GetVacationStatusId()
        {
            return vacationStatusId;
        }

        public DateTime GetVacationStartDate()
        {
            return vacationStartDate;
        }
        public DateTime GetVacationEndDate()
        {
            return vacationEndDate;
        }

        public DateTime GetIssueDate()
        {
            return issueDate;
        }
        public DateTime GetExpiryDate()
        {
            return expiryDate;
        }

        public String GetVacationType()
        {
            return vacationType;
        }
        public String GetVacationStatus()
        {
            return vacationStatus;
        }

        public String GetRequestComments()
        {
            return requestComments;
        }


        //////////////////////////////////////////////////////////////////////
        //SET FUNCTIONS
        //////////////////////////////////////////////////////////////////////


        public void SetBeneficiaryPersonnel(Employee mBeneficiaryPersonnel)
        {
            beneficiaryPersonnel = mBeneficiaryPersonnel;
        }
        public void SetVacationRequestor(Employee mVacationRequestor)
        {
            vacationRequestor = mVacationRequestor;
        }
        public void SetVacationApprovals(List<HUMAN_RESOURCE_MACROS.VACATION_APPROVAL_REJECTION_CLASS_STRUCT> mVacationApprovals)
        {
            vacationApprovalsRejections = mVacationApprovals;
        }

        public void SetVacationSerial(int mVacationSerial)
        {
            vacationSerial = mVacationSerial;
        }
        public void SetVacationTypeId(int mVacationTypeId)
        {
            vacationTypeId = mVacationTypeId;
        }

        public void SetVacationStartDate(DateTime mStartDate)
        {
            vacationStartDate = mStartDate;
        }
        public void SetVacationEndDate(DateTime mEndDate)
        {
            vacationEndDate = mEndDate;
        }

        public void SetIssueDate(DateTime mIssueDate)
        {
            issueDate = mIssueDate;
        }
        public void SetExpiryDate(DateTime mExpiryDate)
        {
            expiryDate = mExpiryDate;
        }

        public void SetVacationType(int mVacationTypeId, String mVacationType)
        {
            vacationTypeId = mVacationTypeId;
            vacationType = mVacationType;
        }
        public void SetVacationType(String mVacationType)
        {
            vacationType = mVacationType;
        }

        public void SetVacationStatus(int mVacationStatusId, String mVacationStatus)
        {
            vacationStatusId = mVacationStatusId;
            vacationStatus = mVacationStatus;
        }
        public void SetVacationStatus(String mVacationStatus)
        {
            vacationStatus = mVacationStatus;
        }

        public void SetRequestComments(String mRequestComments)
        {
            requestComments = mRequestComments;
        }


        //////////////////////////////////////////////////////////////////////
        //MODIFICATION FUNCTIONS
        //////////////////////////////////////////////////////////////////////

        public bool AddVacationApproval(HUMAN_RESOURCE_MACROS.VACATION_APPROVAL_REJECTION_CLASS_STRUCT mVacationApproval)
        {
            if (vacationApprovalsRejections.Exists(approval_item => approval_item.approver == mVacationApproval.approver))
            {
                System.Windows.Forms.MessageBox.Show("There is already an approval on this vacation request from the current user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (vacationApprovalsRejections.Exists(approval_item => approval_item.approver.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.HUMAN_RESOURCES_DEPARTMENT_ID))
            {
                System.Windows.Forms.MessageBox.Show("There is already an approval on this vacation request from Human Resources.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (vacationApprovalsRejections.Exists(approval_item => approval_item.approver.GetEmployeePositionId() < mVacationApproval.approver.GetEmployeePositionId()))
            {
                System.Windows.Forms.MessageBox.Show("There is already an approval on this vacation request from a superior user account.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                vacationApprovalsRejections.Add(mVacationApproval);

                ConfirmVacationStatus(mVacationApproval.approver);

                if (!InsertIntoVacationsLeavesApprovalsRejections(mVacationApproval))
                    return false;

                if (!UpdateVacationsLeavesRequests())
                    return false;

                return true;
            }
        }
        public bool AddVacationApproval(Employee mApprover, String mApprovalRejectionComment)
        {
            HUMAN_RESOURCE_MACROS.VACATION_APPROVAL_REJECTION_CLASS_STRUCT tempApprovalItem = new HUMAN_RESOURCE_MACROS.VACATION_APPROVAL_REJECTION_CLASS_STRUCT();

            DateTime currentDate = DateTime.Now;

            tempApprovalItem.approver = mApprover;
            tempApprovalItem.approvalRejectionComments = mApprovalRejectionComment;
            tempApprovalItem.approvalRejectionDate = currentDate;

            return AddVacationApproval(tempApprovalItem);
        }

        public bool AddVacationRejection(HUMAN_RESOURCE_MACROS.VACATION_APPROVAL_REJECTION_CLASS_STRUCT mVacationRejection)
        {
            if (vacationApprovalsRejections.Exists(approval_item => approval_item.approver == mVacationRejection.approver))
            {
                System.Windows.Forms.MessageBox.Show("There is already a rejection on this vacation request from the current user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (vacationApprovalsRejections.Exists(approval_item => approval_item.approver.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.HUMAN_RESOURCES_DEPARTMENT_ID))
            {
                System.Windows.Forms.MessageBox.Show("There is already a rejection on this vacation request from Human Resources.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (vacationApprovalsRejections.Exists(approval_item => approval_item.approver.GetEmployeePositionId() < mVacationRejection.approver.GetEmployeePositionId()))
            {
                System.Windows.Forms.MessageBox.Show("There is already a rejection on this vacation request from a superior user account.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                vacationApprovalsRejections.Add(mVacationRejection);

                RejectVacationStatus(mVacationRejection.approver);

                if (!InsertIntoVacationsLeavesApprovalsRejections(mVacationRejection))
                    return false;

                if (!UpdateVacationsLeavesRequests())
                    return false;

                return true;
            }
        }
        public bool AddVacationRejection(Employee mRejector, String mApprovalRejectionComment)
        {
            HUMAN_RESOURCE_MACROS.VACATION_APPROVAL_REJECTION_CLASS_STRUCT tempApprovalItem = new HUMAN_RESOURCE_MACROS.VACATION_APPROVAL_REJECTION_CLASS_STRUCT();

            DateTime currentDate = DateTime.Now;

            tempApprovalItem.approver = mRejector;
            tempApprovalItem.approvalRejectionComments = mApprovalRejectionComment;
            tempApprovalItem.approvalRejectionDate = currentDate;

            return AddVacationRejection(tempApprovalItem);
        }

        private void ConfirmVacationStatus(Employee mApprover)
        {
            if (mApprover.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.HUMAN_RESOURCES_DEPARTMENT_ID)
            {
                vacationStatusId = HUMAN_RESOURCE_MACROS.HR_APPROVED_VACATION_STATUS;
                vacationStatus = "Approved by Human Resources";
            }
            else if (mApprover.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)
            {
                vacationStatusId = HUMAN_RESOURCE_MACROS.DEP_MANAGER_APPROVED_VACATION_STATUS;
                vacationStatus = "Approved by Department Manager";
            }
            else if (mApprover.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION)
            {
                vacationStatusId = HUMAN_RESOURCE_MACROS.TEAM_LEAD_APPROVED_VACATION_STATUS;
                vacationStatus = "Approved by Team Lead";
            }
        }
        private void RejectVacationStatus(Employee mRejector)
        {
            if (mRejector.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.HUMAN_RESOURCES_DEPARTMENT_ID)
            {
                vacationStatusId = HUMAN_RESOURCE_MACROS.HR_REJECTED_VACATION_STATUS;
                vacationStatus = "Rejected by Human Resources";
            }
            else if (mRejector.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)
            {
                vacationStatusId = HUMAN_RESOURCE_MACROS.DEP_MANAGER_REJECTED_VACATION_STATUS;
                vacationStatus = "Rejected by Department Manager";
            }
            else if (mRejector.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION)
            {
                vacationStatusId = HUMAN_RESOURCE_MACROS.TEAM_LEAD_REJECTED_VACATION_STATUS;
                vacationStatus = "Rejected by Team Lead";
            }
        }

        public bool IssueNewVacationRequest()
        {
            SetVacationIssueDateToToday();
            SetVacationExpiryDateToToday();

            if (!GetNewVacationSerial())
                return false;

            if (!InsertIntoVacationsLeavesRequests())
                return false;

            if (GetVacationRequestor().GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION || GetVacationRequestor().GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION || GetVacationRequestor().GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.HUMAN_RESOURCES_DEPARTMENT_ID)
            {
                HUMAN_RESOURCE_MACROS.VACATION_APPROVAL_REJECTION_CLASS_STRUCT tempApprovalItem = new HUMAN_RESOURCE_MACROS.VACATION_APPROVAL_REJECTION_CLASS_STRUCT();
                tempApprovalItem.approver = GetVacationRequestor();
                tempApprovalItem.approvalRejectionComments = GetRequestComments();
                tempApprovalItem.approvalRejectionDate = GetIssueDate();

                AddVacationApproval(tempApprovalItem);
            }

            return true;
        }

        //////////////////////////////////////////////////////////////////////
        //GET ADDITIONAL INSERT DATA FUNCTIONS
        //////////////////////////////////////////////////////////////////////

        public void SetVacationIssueDateToToday()
        {
            DateTime currentDate = DateTime.Now;

            issueDate = currentDate;
        }
        public void SetVacationExpiryDateToToday()
        {
            DateTime currentDate = DateTime.Now;

            expiryDate = currentDate;
        }
        public bool GetNewVacationSerial()
        {
            String sqlQueryPart1 = "select max(vacation_leave_requests.request_serial) from erp_system.dbo.vacation_leave_requests;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            vacationSerial = sqlDatabase.rows[0].sql_int[0] + 1;

            return true;
        }


        //////////////////////////////////////////////////////////
        /// INSERT FUNCTIONS
        //////////////////////////////////////////////////////////
        public bool InsertIntoVacationsLeavesRequests()
        {
            String sqlQueryPart1 = "insert into erp_system.dbo.vacation_leave_requests values(";
            String sqlQueryComma = ",";
            String sqlQueryCommaApostrophe = ",'";
            String sqlQueryApostropheCommaApostrophe = "','";
            String sqlQueryApostropheComma = "',";
            String sqlQueryPart2 = "', getdate());";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetVacationSerial();
            sqlQuery += sqlQueryComma;
            sqlQuery += GetBeneficiaryPersonnel().GetEmployeeId();
            sqlQuery += sqlQueryComma;
            sqlQuery += GetVacationRequestor().GetEmployeeId();
            sqlQuery += sqlQueryComma;
            sqlQuery += GetVacationTypeId();
            sqlQuery += sqlQueryCommaApostrophe;
            sqlQuery += GetVacationStartDate().ToString("yyyy-MM-dd");
            sqlQuery += sqlQueryApostropheCommaApostrophe;
            sqlQuery += GetVacationEndDate().ToString("yyyy-MM-dd");
            sqlQuery += sqlQueryApostropheCommaApostrophe;
            sqlQuery += GetExpiryDate().ToString("yyyy-MM-dd");
            sqlQuery += sqlQueryApostropheComma;
            sqlQuery += GetVacationStatusId();
            sqlQuery += sqlQueryCommaApostrophe;
            sqlQuery += GetRequestComments();
            sqlQuery += sqlQueryPart2;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
        public bool InsertIntoVacationsLeavesApprovalsRejections(HUMAN_RESOURCE_MACROS.VACATION_APPROVAL_REJECTION_CLASS_STRUCT mApprovalRejection)
        {
            String sqlQueryPart1 = "insert into erp_system.dbo.vacation_leave_approvals_rejections values(";
            String sqlQueryComma = ",";
            String sqlQueryCommaApostrophe = ",'";
            String sqlQueryPart2 = "', getdate());";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetVacationSerial();
            sqlQuery += sqlQueryComma;
            sqlQuery += GetVacationTypeId();
            sqlQuery += sqlQueryComma;
            sqlQuery += mApprovalRejection.approver.GetEmployeeId();
            sqlQuery += sqlQueryCommaApostrophe;
            sqlQuery += mApprovalRejection.approvalRejectionComments;
            sqlQuery += sqlQueryPart2;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool UpdateVacationsLeavesRequests()
        {
            String sqlQueryPart1 = @"update erp_system.dbo.vacation_leave_requests
                                    set vacation_leave_requests.request_status = ";
            String sqlQueryPart2 = " where vacation_leave_requests.request_serial = ";
            String sqlQueryPart3 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetVacationStatusId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetVacationSerial();
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
    }
}