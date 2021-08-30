using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01electronics_library
{
    public class Employee
    {
        //SQL QUERY
        private String sqlQuery;

        //SQL OBJECTS
        private SQLServer sqlDatabase;


        protected IntegrityChecks integrityChecker;

        //EMPLOYEE INFO
        private int employeeId;
        
        private int departmentId;
        private int teamId;
        private int positionId;

        private int educationalQualificationId;
        private int majorId;

        private int graduationYear;

        private List<HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT> basicSalary;

        private List<HUMAN_RESOURCE_MACROS.BANK_STRUCT> payrollInfo;
        private List<BASIC_STRUCTS.DOCUMENT_TYPE_STRUCT> documentsSubmitted;

        private String businessEmail;
        private String personalEmail;
        
        private String businessPhone;
        private String personalPhone;

        private String name;
        private String initials;
        
        private String gender;
        
        private String department;
        private String team;
        private String position;

        private String nationalId;

        private DateTime birthDateStruct;
        private DateTime joinDateStruct;
        private DateTime terminationDateStruct;
        
        private String birthDate;
        private String joinDate;
        private String terminationDate;
        
        private String educationalQualification;
        private String major;

        private bool currentlyEnrolled;

        protected string inputString;
        protected string modifiedString;

        public Employee()
        {
            sqlDatabase = new SQLServer();

            payrollInfo = new List<HUMAN_RESOURCE_MACROS.BANK_STRUCT>();
            documentsSubmitted = new List<BASIC_STRUCTS.DOCUMENT_TYPE_STRUCT>();

            basicSalary = new List<HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT>();

            integrityChecker = new IntegrityChecks();
        }

        public Employee(SQLServer mSqlDatabase)
        {
            sqlDatabase = mSqlDatabase;

            payrollInfo = new List<HUMAN_RESOURCE_MACROS.BANK_STRUCT>();
            documentsSubmitted = new List<BASIC_STRUCTS.DOCUMENT_TYPE_STRUCT>();

            basicSalary = new List<HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT>();

            integrityChecker = new IntegrityChecks();
        }

        public void SetDatabase(SQLServer mSqlDatabase)
        {
            sqlDatabase = mSqlDatabase;
        }

        public Employee(ref SQLServer mSqlDatabase)
        {
            sqlDatabase = mSqlDatabase;

            payrollInfo = new List<HUMAN_RESOURCE_MACROS.BANK_STRUCT>();
            documentsSubmitted = new List<BASIC_STRUCTS.DOCUMENT_TYPE_STRUCT>();

            basicSalary = new List<HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT>();

            integrityChecker = new IntegrityChecks();
        }

        public bool InitializeEmployeeInfo(String mBusinessEmail)
        {
            businessEmail = mBusinessEmail;

            String sqlQueryPart1 = @"select employees_info.employee_id, 
		        employees_info.employee_department, 
		        employees_info.employee_team, 
		        employees_info.employee_position, 
		        employees_educational_qualifications.graduation_year, 

		        educational_degrees.id, 
		        educational_majors.id, 

		        employees_basic_salaries.due_month, 
		        employees_basic_salaries.due_year, 

		        employees_basic_salaries.gross_salary, 
		        employees_basic_salaries.insurance_and_taxes, 
		        employees_basic_salaries.net_salary,

		        employees_info.birth_date, 
		        employees_info.join_date, 
		        employees_termination_info.resignation_date,

		        employees_info.name, 
		        employees_info.gender, 
		        departments_type.department, 
		        teams_type.team, 
		        positions_type.position, 

		        employees_personal_emails.email, 
		        employees_business_phones.phone, 
		        employees_personal_phones.phone, 
		        
		        employees_national_id.national_id,

		        educational_degrees.educational_degree, 
		        educational_majors.educational_major, 

		        employees_initials.employee_initial, 

		        employees_info.currently_enrolled

		        from erp_system.dbo.employees_info 
		        left join erp_system.dbo.departments_type 
		        on employees_info.employee_department = departments_type.id 
		        left join erp_system.dbo.teams_type 
		        on employees_info.employee_team = teams_type.id 
		        left join erp_system.dbo.positions_type 
		        on employees_info.employee_position = positions_type.id 
		        left join erp_system.dbo.employees_business_emails 
		        on employees_info.employee_id = employees_business_emails.id 
		        left join erp_system.dbo.employees_personal_emails 
		        on employees_info.employee_id = employees_personal_emails.id 
		        left join erp_system.dbo.employees_business_phones 
		        on employees_info.employee_id = employees_business_phones.id 
		        left join erp_system.dbo.employees_personal_phones 
		        on employees_info.employee_id = employees_personal_phones.id 
		        left join erp_system.dbo.employees_initials 
		        on employees_info.employee_id = employees_initials.id 
		        left join erp_system.dbo.employees_educational_qualifications 
		        on employees_info.employee_id = employees_educational_qualifications.employee_id 
		        left join erp_system.dbo.educational_degrees 
		        on employees_educational_qualifications.certificate = educational_degrees.id 
		        left join erp_system.dbo.educational_majors 
		        on employees_educational_qualifications.major = educational_majors.id 
		        left join erp_system.dbo.employees_national_id
		        on employees_info.employee_id = employees_national_id.id
		        left join erp_system.dbo.employees_basic_salaries 
		        on employees_info.employee_id = employees_basic_salaries.employee_id
				left join erp_system.dbo.employees_termination_info
		        on employees_info.employee_id = employees_termination_info.id
		        where employees_business_emails.email = '";
            String sqlQueryPart2 = "' order by employees_basic_salaries.due_year, employees_basic_salaries.due_month;";

            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += businessEmail;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 9;
            queryColumns.sql_bigint = 0;
            queryColumns.sql_money = 3;
            queryColumns.sql_datetime = 3;
            queryColumns.sql_string = 12;
            queryColumns.sql_bit = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            employeeId = sqlDatabase.rows[0].sql_int[0];
            departmentId = sqlDatabase.rows[0].sql_int[1];
            teamId = sqlDatabase.rows[0].sql_int[2];
            positionId = sqlDatabase.rows[0].sql_int[3];
            graduationYear = sqlDatabase.rows[0].sql_int[4];

            educationalQualificationId = sqlDatabase.rows[0].sql_int[5];
            majorId = sqlDatabase.rows[0].sql_int[6];

            for (int i = 0; i < sqlDatabase.rows.Count; i++) 
            {
                HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT currentBasicSalary = new HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT();

                currentBasicSalary.gross_salary = sqlDatabase.rows[i].sql_money[0];
                currentBasicSalary.insurance_and_tax = sqlDatabase.rows[i].sql_money[1];
                currentBasicSalary.net_salary = sqlDatabase.rows[i].sql_money[2];
                currentBasicSalary.due_year = sqlDatabase.rows[i].sql_int[7];
                currentBasicSalary.due_month = sqlDatabase.rows[i].sql_int[8];

                basicSalary.Add(currentBasicSalary);
            }
            
            birthDateStruct = sqlDatabase.rows[0].sql_datetime[0];
            joinDateStruct = sqlDatabase.rows[0].sql_datetime[1];
            terminationDateStruct = sqlDatabase.rows[0].sql_datetime[2];

            joinDate = joinDateStruct.Date.ToString();
            birthDate = birthDateStruct.Date.ToString();
            terminationDate = terminationDateStruct.Date.ToString();
            
            name = sqlDatabase.rows[0].sql_string[0];
            gender = sqlDatabase.rows[0].sql_string[1];

            department = sqlDatabase.rows[0].sql_string[2];
            team = sqlDatabase.rows[0].sql_string[3];
            position = sqlDatabase.rows[0].sql_string[4];

            personalEmail = sqlDatabase.rows[0].sql_string[5];
            businessPhone = sqlDatabase.rows[0].sql_string[6];
            personalPhone = sqlDatabase.rows[0].sql_string[7];
            
            nationalId = sqlDatabase.rows[0].sql_string[8];

            educationalQualification = sqlDatabase.rows[0].sql_string[9];
            major = sqlDatabase.rows[0].sql_string[10];

            initials = sqlDatabase.rows[0].sql_string[11];

            currentlyEnrolled = sqlDatabase.rows[0].sql_bit[0];

            InitializeEmployeePayrollInfo();
            InitializeEmployeeSubmittedDocuments();

            return true;
        }

        public bool InitializeEmployeeInfo(int mEmployeeId)
        {
            employeeId = mEmployeeId;

            String sqlQueryPart1 = @"select employees_info.employee_department, 
        		employees_info.employee_team, 
        		employees_info.employee_position, 
        		employees_educational_qualifications.graduation_year, 
        
        		educational_degrees.id, 
        		educational_majors.id, 
 
		        employees_basic_salaries.due_month, 
		        employees_basic_salaries.due_year, 

                employees_basic_salaries.gross_salary, 
		        employees_basic_salaries.insurance_and_taxes, 
		        employees_basic_salaries.net_salary, 

        		employees_info.birth_date, 
        		employees_info.join_date,
        		employees_termination_info.resignation_date, 
        
        		employees_info.name, 
        		employees_info.gender, 
        
        		departments_type.department, 
        		teams_type.team, 
        		positions_type.position, 
        
        		employees_business_emails.email, 
        		employees_personal_emails.email, 
        
        		employees_business_phones.phone, 
        		employees_personal_phones.phone, 
        		
        		employees_national_id.national_id,
        
        		educational_degrees.educational_degree, 
        		educational_majors.educational_major, 
        
        		employees_initials.employee_initial, 
        
        		employees_info.currently_enrolled 
        
        		from erp_system.dbo.employees_info 
        		left join erp_system.dbo.departments_type 
        		on employees_info.employee_department = departments_type.id 
        		left join erp_system.dbo.teams_type 
        		on employees_info.employee_team = teams_type.id 
        		left join erp_system.dbo.positions_type 
        		on employees_info.employee_position = positions_type.id 
        		left join erp_system.dbo.employees_business_emails 
        		on employees_info.employee_id = employees_business_emails.id 
        		left join erp_system.dbo.employees_personal_emails 
        		on employees_info.employee_id = employees_personal_emails.id 
        		left join erp_system.dbo.employees_business_phones 
        		on employees_info.employee_id = employees_business_phones.id 
        		left join erp_system.dbo.employees_personal_phones 
        		on employees_info.employee_id = employees_personal_phones.id 
        		left join erp_system.dbo.employees_initials 
        		on employees_info.employee_id = employees_initials.id 
        		left join erp_system.dbo.employees_educational_qualifications 
        		on employees_info.employee_id = employees_educational_qualifications.employee_id 
        		left join erp_system.dbo.educational_degrees 
        		on employees_educational_qualifications.certificate = educational_degrees.id 
        		left join erp_system.dbo.educational_majors 
        		on employees_educational_qualifications.major = educational_majors.id 
                left join erp_system.dbo.employees_national_id
        		on employees_info.employee_id = employees_national_id.id
                left join erp_system.dbo.employees_basic_salaries 
		        on employees_info.employee_id = employees_basic_salaries.employee_id 
                left join erp_system.dbo.employees_termination_info
        		on employees_info.employee_id = employees_termination_info.id
        	    where employees_info.employee_id = ";
            String sqlQueryPart2 = "order by employees_basic_salaries.due_year, employees_basic_salaries.due_month;";

            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += employeeId;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 8;
            queryColumns.sql_bigint = 0;
            queryColumns.sql_money = 3;
            queryColumns.sql_datetime = 3;
            queryColumns.sql_string = 13;
            queryColumns.sql_bit = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            departmentId = sqlDatabase.rows[0].sql_int[0];
            teamId = sqlDatabase.rows[0].sql_int[1];
            positionId = sqlDatabase.rows[0].sql_int[2];
            graduationYear = sqlDatabase.rows[0].sql_int[3];

            educationalQualificationId = sqlDatabase.rows[0].sql_int[4];
            majorId = sqlDatabase.rows[0].sql_int[5];

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT currentBasicSalary = new HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT();

                currentBasicSalary.gross_salary = sqlDatabase.rows[i].sql_money[0];
                currentBasicSalary.insurance_and_tax = sqlDatabase.rows[i].sql_money[1];
                currentBasicSalary.net_salary = sqlDatabase.rows[i].sql_money[2];
                currentBasicSalary.due_year = sqlDatabase.rows[i].sql_int[6];
                currentBasicSalary.due_month = sqlDatabase.rows[i].sql_int[7];
                
                basicSalary.Add(currentBasicSalary);
            }

            birthDateStruct = sqlDatabase.rows[0].sql_datetime[0];
            joinDateStruct = sqlDatabase.rows[0].sql_datetime[1];
            terminationDateStruct = sqlDatabase.rows[0].sql_datetime[2];

            joinDate = joinDateStruct.ToString();
            birthDate = birthDateStruct.ToString();
            terminationDate = terminationDateStruct.ToString();

            name = sqlDatabase.rows[0].sql_string[0];
            gender = sqlDatabase.rows[0].sql_string[1];

            department = sqlDatabase.rows[0].sql_string[2];
            team = sqlDatabase.rows[0].sql_string[3];
            position = sqlDatabase.rows[0].sql_string[4];

            businessEmail = sqlDatabase.rows[0].sql_string[5];
            personalEmail = sqlDatabase.rows[0].sql_string[6];

            businessPhone = sqlDatabase.rows[0].sql_string[7];
            personalPhone = sqlDatabase.rows[0].sql_string[8];

            nationalId = sqlDatabase.rows[0].sql_string[9];

            educationalQualification = sqlDatabase.rows[0].sql_string[10];
            major = sqlDatabase.rows[0].sql_string[11];

            initials = sqlDatabase.rows[0].sql_string[12];

            currentlyEnrolled = sqlDatabase.rows[0].sql_bit[0];

            InitializeEmployeePayrollInfo();
            InitializeEmployeeSubmittedDocuments();

            return true;
        }

        public bool InitializeEmployeePayrollInfo()
        {
            String sqlQueryPart1 = @"select payroll_types.id as bank_id, 
                                    	   employees_payroll_info.bank_payroll_id,
                                           employees_payroll_info.account_id, 
                                    	   employees_payroll_info.iban_id,
                                           payroll_types.payroll_type
                                    from erp_system.dbo.employees_payroll_info 
                                    left join erp_system.dbo.employees_basic_salaries 
                                    on employees_payroll_info.employee_id = employees_basic_salaries.employee_id 
                                    inner join erp_system.dbo.payroll_types 
                                    on employees_payroll_info.payroll_type = payroll_types.id 
                                    where employees_payroll_info.employee_id = ";
            String sqlQueryPart2 = " order by employees_payroll_info.payroll_type;";

            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += employeeId;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 2;
            queryColumns.sql_bigint = 2;
            queryColumns.sql_money = 0;
            queryColumns.sql_datetime = 0;
            queryColumns.sql_string = 1;
            queryColumns.sql_bit = 0;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            payrollInfo.Clear();

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                HUMAN_RESOURCE_MACROS.BANK_STRUCT tempItem = new HUMAN_RESOURCE_MACROS.BANK_STRUCT();

                tempItem.bank_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.payroll_id = sqlDatabase.rows[i].sql_int[1];
                tempItem.account_id = (ulong)sqlDatabase.rows[i].sql_bigint[0];
                tempItem.iban_id = (ulong)sqlDatabase.rows[i].sql_bigint[1];

                tempItem.payroll_type = sqlDatabase.rows[i].sql_string[0];

                payrollInfo.Add(tempItem);
            }

            return true;
        }

        public bool InitializeEmployeeSubmittedDocuments()
        {
            String sqlQueryPart1 = @"select employement_documents_type.id,
                                    		employement_documents_type.document_type
                                    from erp_system.dbo.employees_submitted_documents
                                    inner join erp_system.dbo.employement_documents_type
                                    on employees_submitted_documents.document_type = employement_documents_type.id
                                    where employees_submitted_documents.employee_id = ";
            String sqlQueryPart2 = " order by employees_submitted_documents.document_type;";

            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += employeeId;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_bigint = 0;
            queryColumns.sql_money = 0;
            queryColumns.sql_datetime = 0;
            queryColumns.sql_string = 1;
            queryColumns.sql_bit = 0;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_LOW))
                return false;

            documentsSubmitted.Clear();

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                BASIC_STRUCTS.DOCUMENT_TYPE_STRUCT tempItem = new BASIC_STRUCTS.DOCUMENT_TYPE_STRUCT();

                tempItem.document_type_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.document_type_name = sqlDatabase.rows[i].sql_string[0];

                documentsSubmitted.Add(tempItem);
            }

            return true;
        }

        //////////////////////////////////////////////////////////////////////
        //SET FUNCTIONS
        //////////////////////////////////////////////////////////////////////

        public void SetEmployeeName(String mName)
        {
            name = mName;
        }
        public void SetEmployeeInitials(String mInitials)
        {
            initials = mInitials;
        }

        public void SetEmployeeGender(String mGender)
        {
            gender = mGender;
        }

        public void SetEmployeeDepartment(int mDepartmentId, String mDepartment)
        {
            departmentId = mDepartmentId;
            department = mDepartment;
        }
        public void SetEmployeeTeam(int mTeamId, String mTeam)
        {
            teamId = mTeamId;
            team = mTeam;
        }
        public void SetEmployeePosition(int mPositionId, String mPosition)
        {
            positionId = mPositionId;
            position = mPosition;
        }

        public void SetEmployeeBusinessEmail(String mBusinessEmail)
        {
            businessEmail = mBusinessEmail;
        }
        public void SetEmployeePersonalEmail(String mPersonalEmail)
        {
            personalEmail = mPersonalEmail;
        }

        public void SetEmployeeBusinessPhone(String mBusinessPhone)
        {
            businessPhone = mBusinessPhone;
        }
        public void SetEmployeePersonalPhone(String mPersonalPhone)
        {
            personalPhone = mPersonalPhone;
        }

        public void SetEmployeeNationalId(String mNationalId)
        {
            nationalId = mNationalId;
        }

        public void SetEmployeeEducationalDegree(int mEducationalQualificationId, String mEducationalQualification)
        {
            educationalQualificationId = mEducationalQualificationId;
            educationalQualification = mEducationalQualification;
        }
        public void SetEmployeeMajor(int mMajorId, String mMajor)
        {
            majorId = mMajorId;
            major = mMajor;
        }
        public void SetEmployeeGraduationYear(int mGraduationYear)
        {
            graduationYear = mGraduationYear;
        }
        
        public void SetEmployeeBasicSalary(List<HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT> mBasicSalary)
        {
            basicSalary = mBasicSalary;
        }
        
        public void SetEmployeePayrollInfo(List<HUMAN_RESOURCE_MACROS.BANK_STRUCT> mPayrollInfo)
        {
            payrollInfo = mPayrollInfo;
        }
        
        public void SetEmployeeSubmittedDocuments(List<BASIC_STRUCTS.DOCUMENT_TYPE_STRUCT> mDocumentsSubmitted)
        {
            documentsSubmitted = mDocumentsSubmitted;
        }
        public void AddNewSubmittedDocument(BASIC_STRUCTS.DOCUMENT_TYPE_STRUCT mDocument)
        {
            documentsSubmitted.Add(mDocument);
        }

        public void SetEmployeeBirthDateStruct(DateTime mBirthDateStruct)
        {
            birthDateStruct = mBirthDateStruct;
            birthDate = birthDateStruct.Date.ToString("yyyy-MM-dd");
        }
        public void SetEmployeeJoinDateStruct(DateTime mJoinDateStruct)
        {
            joinDateStruct = mJoinDateStruct;
            joinDate = joinDateStruct.Date.ToString("yyyy-MM-dd");
        }
        public void SetEmployeeTerminationDateStruct(DateTime mTerminationDateStruct)
        {
            terminationDateStruct = mTerminationDateStruct;
            terminationDate = terminationDateStruct.Date.ToString("yyyy-MM-dd");
        }

        public void SetEmployeeBirthDate(String mBirthDate)
        {
            birthDateStruct = Convert.ToDateTime(mBirthDate);
            birthDate = birthDateStruct.Date.ToString("yyyy-MM-dd");
            
        }
        public void SetEmployeeJoinDate(String mJoinDate)
        {
            joinDateStruct = Convert.ToDateTime(mJoinDate);
            joinDate = joinDateStruct.Date.ToString("yyyy-MM-dd");
        }
        public void SetEmployeeTerminationDate(String mTerminationDate)
        {
            terminationDateStruct = Convert.ToDateTime(mTerminationDate);
            terminationDate = terminationDateStruct.Date.ToString("yyyy-MM-dd");
            
        }

        public void SetBasicSalary(HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT mBasicSalary)
        {
            basicSalary.Add(mBasicSalary);
        }

        public void SetCurrentlyEnrolled(bool mCurrentlyEnrolled)
        {
            currentlyEnrolled = mCurrentlyEnrolled;
        }

        //////////////////////////////////////////////////////////////////////
        //RETURN FUNCTIONS
        //////////////////////////////////////////////////////////////////////

        public String GetEmployeeName()
        {
            return name;
        }
        public String GetEmployeeInitials()
        {
            return initials;
        }

        public String GetEmployeeGender()
        {
            return gender;
        }

        public String GetEmployeeDepartment()
        {
            return department;
        }
        public String GetEmployeeTeam()
        {
            return team;
        }
        public String GetEmployeePosition()
        {
            return position;
        }

        public String GetEmployeeBusinessEmail()
        {
            return businessEmail;
        }
        public String GetEmployeePersonalEmail()
        {
            return personalEmail;
        }

        public String GetEmployeeBusinessPhone()
        {
            return businessPhone;
        }
        public String GetEmployeePersonalPhone()
        {
            return personalPhone;
        }

        public String GetEmployeeNationalId()
        {
            return nationalId;
        }

        public DateTime GetEmployeeBirthDateStruct()
        {
            return birthDateStruct;
        }
        public DateTime GetEmployeeJoinDateStruct()
        {
            return joinDateStruct;
        }
        public DateTime GetEmployeeTerminationDateStruct()
        {
            return terminationDateStruct;
        }

        public String GetEmployeeBirthDate()
        {
            return birthDate;
        }
        public String GetEmployeeJoinDate()
        {
            return joinDate;
        }
        public String GetEmployeeTerminationDate()
        {
            return terminationDate;
        }

        public String GetEmployeeEducationalQualification()
        {
            return educationalQualification;
        }
        public String GetEmployeeMajor()
        {
            return major;
        }
        public int GetEmployeeGraduationYear()
        {
            return graduationYear;
        }

        public int GetEmployeeId()
        {
            return employeeId;
        }
        public int GetEmployeeDepartmentId()
        {
            return departmentId;
        }
        public int GetEmployeeTeamId()
        {
            return teamId;
        }
        public int GetEmployeePositionId()
        {
            return positionId;
        }

        public int GetEmployeeEducationalDegreeId()
        {
            return educationalQualificationId;
        }
        public int GetEmployeeMajorId()
        {
            return majorId;
        }

        public List<HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT> GetEmployeeBasicSalary()
        {
            return basicSalary;
        }
        public HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT GetEmployeeLatestBasicSalary()
        {
            return basicSalary[basicSalary.Count - 1];
        }
        public Decimal GetEmployeeGrossSalary()
        {
            return basicSalary[basicSalary.Count - 1].gross_salary;
        }
        public Decimal GetEmployeeInsuranceAndTaxes()
        {
            return basicSalary[basicSalary.Count - 1].insurance_and_tax;
        }
        public Decimal GetEmployeeNetSalary()
        {
            return basicSalary[basicSalary.Count - 1].net_salary;
        }

        public List<HUMAN_RESOURCE_MACROS.BANK_STRUCT> GetEmployeePayrollInfo()
        {
            return payrollInfo;
        }
        public List<BASIC_STRUCTS.DOCUMENT_TYPE_STRUCT> GetEmployeeDocumentsSubmitted()
        {
            return documentsSubmitted;
        }

        public bool GetCurrentEmployementStatus()
        {
            return currentlyEnrolled;
        }

        /////////////////////////////////////////////////////////////////////////////////////
        /// <GET ADDITIONAL DATA FUNCTIONS>
        ///////////////////////////////////////////////////////////////////////////////////// 
        public bool GetNewEmployeeId()
        {
            String sqlQueryPart1 = "select max(employee_id) from erp_system.dbo.employees_info;";

            sqlQuery = string.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            employeeId = sqlDatabase.rows[0].sql_int[0] + 1;

            return true;
        }

        /////////////////////////////////////////////////////////////////////////////////////
        /// <APPLY CHANGES FUNCTIONS>
        ///////////////////////////////////////////////////////////////////////////////////// 
        public bool AddNewEmployee()
        {
            if (!GetNewEmployeeId())
                return false;

            if (!InsertIntoEmployeesInfo())
                return false;

            if (!InsertIntoEmployeesBasicSalaries())
                return false;

            if (!InsertIntoEmployeeNationalId())
                return false;

            if (!InsertIntoEmployeeExcludedAttendance())
                return false;

            if (!InsertIntoEmployeeEducationalQualifications())
                return false;

            if (!InsertIntoEmployeeBusinessEmail())
                return false;

            if (!InsertIntoEmployeePersonalEmail())
                return false;

            if (!InsertIntoEmployeeBusinessPhone())
                return false;

            if (!InsertIntoEmployeePersonalPhone())
                return false;

            return true;
        }
        public bool TerminateEmployee()
        {
            SetCurrentlyEnrolled(false);

            if (!UpdateEmployeeEnrollmentStatus())
                return false;

            if (!InsertIntoEmployeeTerminationInfo())
                return false;

            return true;
        }

        public bool AddNewBasicSalary(HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT mBasicSalary)
        {
            basicSalary.Add(mBasicSalary);

            if (!InsertIntoEmployeesBasicSalaries())
                return false;

            return true;
        }
        public void AddNewEmployeePayroll(HUMAN_RESOURCE_MACROS.BANK_STRUCT mPayroll)
        {
            payrollInfo.Add(mPayroll);
        }
        /////////////////////////////////////////////////////////////////////////////////////
        /// <INSERT FUNCTIONS>
        ///////////////////////////////////////////////////////////////////////////////////// 
        protected bool InsertIntoEmployeesInfo()
        {
            String sqlQueryPart1 = @"insert into erp_system.dbo.employees_info
                                     values(";
            String sqlQueryPart2 = ", getdate());";

            String comma = " ,";
            String apostropheComma = " ',";
            String commaApostrophe = " ,'";
            String apostropheCommaApostrophe = "','";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeeId();
            sqlQuery += commaApostrophe;
            sqlQuery += GetEmployeeName();
            sqlQuery += apostropheCommaApostrophe;
            sqlQuery += GetEmployeeGender();
            sqlQuery += apostropheComma;
            sqlQuery += GetEmployeeDepartmentId();
            sqlQuery += comma;
            sqlQuery += GetEmployeeTeamId();
            sqlQuery += comma;
            sqlQuery += GetEmployeePositionId();
            sqlQuery += commaApostrophe;
            sqlQuery += GetEmployeeBirthDate();
            sqlQuery += apostropheCommaApostrophe;
            sqlQuery += GetEmployeeJoinDate();
            sqlQuery += apostropheComma;
            if (GetCurrentEmployementStatus())
                sqlQuery += 1;
            else
                sqlQuery += 0;
            sqlQuery += sqlQueryPart2;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
        protected bool InsertIntoEmployeesBasicSalaries()
        {
            String sqlQueryPart1 = @"insert into erp_system.dbo.employees_basic_salaries
                                     values(";
            String sqlQueryPart2 = ", getdate());";

            String comma = ", ";

            DateTime currentDate = DateTime.Now;

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeeId();
            sqlQuery += comma;
            sqlQuery += Convert.ToInt32(GetEmployeeGrossSalary());
            sqlQuery += comma;
            sqlQuery += Convert.ToInt32(GetEmployeeInsuranceAndTaxes());
            sqlQuery += comma;
            sqlQuery += Convert.ToInt32(GetEmployeeNetSalary());
            sqlQuery += comma;
            sqlQuery += currentDate.Year;
            sqlQuery += comma;
            sqlQuery += currentDate.Month;
            sqlQuery += sqlQueryPart2;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;

        }
        protected bool InsertIntoEmployeeNationalId()
        {
            String sqlQueryPart1 = @"insert into erp_system.dbo.employees_national_id
                                     values(";
            String sqlQueryPart2 = ", ";
            String sqlQueryPart3 = ", getdate());";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetEmployeeNationalId();
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
        protected bool InsertIntoEmployeeExcludedAttendance()
        {
            String sqlQueryPart1 = @"insert into erp_system.dbo.employees_excluded_attendance
                                     values(";
            String sqlQueryPart2 = ", 0, getdate());";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryPart2;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
        protected bool InsertIntoEmployeeEducationalQualifications()
        {
            String sqlQueryPart1 = @"insert into erp_system.dbo.employees_educational_qualifications
                                     values(";
            String sqlQueryComma = ", ";
            String sqlQueryPart3 = ", getdate());";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryComma;
            sqlQuery += GetEmployeeEducationalDegreeId();
            sqlQuery += sqlQueryComma;
            sqlQuery += GetEmployeeMajorId();
            sqlQuery += sqlQueryComma;
            sqlQuery += GetEmployeeGraduationYear();
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
        protected bool InsertIntoEmployeeTerminationInfo()
        {
            String sqlQueryPart1 = @"insert into erp_system.dbo.employees_termination_info
                                     values(";
            String sqlQueryComma = ", ";
            String sqlQueryPart3 = ", getdate());";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryComma;
            sqlQuery += GetEmployeeTerminationDate();
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        protected bool InsertIntoEmployeeBusinessEmail()
        {
            String sqlQueryPart1 = @"insert into erp_system.dbo.employees_business_emails values (";
            String sqlQueryPart2 = ",'";
            String sqlQueryPart3 = "', getdate());";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetEmployeeBusinessEmail();
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
        protected bool InsertIntoEmployeePersonalEmail()
        {
            String sqlQueryPart1 = @"insert into erp_system.dbo.employees_personal_emails values (";
            String sqlQueryPart2 = ",'";
            String sqlQueryPart3 = "', getdate());";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetEmployeePersonalEmail();
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        protected bool InsertIntoEmployeeBusinessPhone()
        {
            String sqlQueryPart1 = @"insert into erp_system.dbo.employees_business_phones values (";
            String sqlQueryPart2 = ",'";
            String sqlQueryPart3 = "', getdate());";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetEmployeeBusinessPhone();
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
        protected bool InsertIntoEmployeePersonalPhone()
        {
            String sqlQueryPart1 = @"insert into erp_system.dbo.employees_personal_phones values (";
            String sqlQueryPart2 = ",'";
            String sqlQueryPart3 = "', getdate());";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetEmployeePersonalPhone();
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        /////////////////////////////////////////////////////////////////////////////////////
        /// <PUBLIC UPDATE FUNCTIONS>
        ///////////////////////////////////////////////////////////////////////////////////// 

        public bool UpdateEmployeeName(String mName)
        {
            SetEmployeeName(mName);

            if (!UpdateEmployeeName())
                return false;

            return true;
        }
        public bool UpdateEmployeeNationalId(String mNationalId)
        {
            SetEmployeeNationalId(mNationalId);

            if (!UpdateEmployeeNationalId())
                return false;

            return true;
        }
        
        public bool UpdateEmployeeDepartment(int mDepartmentId, String mDepartment)
        {
            SetEmployeeDepartment(mDepartmentId, mDepartment);

            if (!UpdateEmployeeDepartment())
                return false;

            return true;
        }
        public bool UpdateEmployeeTeam(int mTeamId, String mTeam)
        {
            SetEmployeeTeam(mTeamId, mTeam);

            if (!UpdateEmployeeTeam())
                return false;

            return true;
        }
        public bool UpdateEmployeePosition(int mPositionId, String mPosition)
        {
            SetEmployeePosition(mPositionId, mPosition);

            if (!UpdateEmployeePosition())
                return false;

            return true;
        }

        public bool UpdateEmployeeJoinDate(String mJoinDate)
        {
            SetEmployeeJoinDate(mJoinDate);

            if (!UpdateEmployeeJoinDate())
                return false;

            return true;
        }
        public bool UpdateEmployeeBirthDate(String mBirthDate)
        {
            SetEmployeeBirthDate(mBirthDate);

            if (!UpdateEmployeeBirthDate())
                return false;

            return true;
        }
        public bool UpdateEmployeeTerminationDate(String mTerminationDate)
        {
            SetEmployeeTerminationDate(mTerminationDate);

            if (!UpdateEmployeeTerminationDate())
                return false;

            return true;
        }

        public bool UpdateEmployeeBusinessEmail(String mBussinessEmail)
        {
            SetEmployeeBusinessEmail(mBussinessEmail);

            if (!UpdateEmployeeBusinessEmail())
                return false;

            return true;
        }
        public bool UpdateEmployeePersonalEmail(String mPersonalEmail)
        {
            SetEmployeePersonalEmail(mPersonalEmail);

            if (!UpdateEmployeePersonalEmail())
                return false;

            return true;
        }

        public bool UpdateEmployeeBusinessPhone(String mBussinessPhone)
        {
            SetEmployeeBusinessPhone(mBussinessPhone);

            if (!UpdateEmployeeBusinessPhone())
                return false;

            return true;
        }
        public bool UpdateEmployeePersonalPhone(String mPersonalPhone)
        {
            SetEmployeePersonalPhone(mPersonalPhone);

            if (!UpdateEmployeePersonalPhone())
                return false;

            return true;
        }

        public bool UpdateEmployeeBasicSalary(Decimal mGrossSalary, Decimal mInsuranceAndTaxes, Decimal mNetSalary)
        {
            HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT newBasicSalary = new HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT();
            newBasicSalary.gross_salary = mGrossSalary;
            newBasicSalary.insurance_and_tax = mInsuranceAndTaxes;
            newBasicSalary.net_salary = mNetSalary;

            newBasicSalary.due_year = DateTime.Now.Year;
            newBasicSalary.due_month = DateTime.Now.Month;

            basicSalary.Add(newBasicSalary);

            if (!InsertIntoEmployeesBasicSalaries())
                return false;

            return true;
        }
        public bool UpdateEmployeeEducationalQualifications(BASIC_STRUCTS.EDUCATIONAL_DEGREE_STRUCT mDegree, BASIC_STRUCTS.EDUCATIONAL_MAJOR_STRUCT mMajor, int mGraduationYear)
        {
            SetEmployeeEducationalDegree(mDegree.edu_degree_id, mDegree.edu_degree_name);
            SetEmployeeMajor(mMajor.edu_major_id, mMajor.edu_major_name);
            SetEmployeeGraduationYear(mGraduationYear);

            if (!UpdateEmployeeEducationalQualifications())
                return false;

            return true;
        }

        /////////////////////////////////////////////////////////////////////////////////////
        /// <PROTECTED UPDATE FUNCTIONS>
        ///////////////////////////////////////////////////////////////////////////////////// 
        protected bool UpdateEmployeeName()
        {
            String sqlQueryPart1 = @"update erp_system.dbo.employees_info
                                     set employees_info.name = '";
            String sqlQueryPart2 = " ' where employees_info.employee_id = ";
            String sqlQueryPart3 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeeName();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
        protected bool UpdateEmployeeNationalId()
        {
            String sqlQueryPart1 = @"update erp_system.dbo.employees_national_id
                                     set employees_national_id.national_id = ";
            String sqlQueryPart2 = " where employees_national_id.id = ";
            String sqlQueryPart3 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeeNationalId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
        
        protected bool UpdateEmployeeDepartment()
        {
            String sqlQueryPart1 = @"update erp_system.dbo.employees_info
                                     set employees_info.employee_department = ";
            String sqlQueryPart2 = " where employees_info.employee_id = ";
            String sqlQueryPart3 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeeDepartmentId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
        protected bool UpdateEmployeeTeam()
        {
            String sqlQueryPart1 = @"update erp_system.dbo.employees_info
                                     set employees_info.employee_team = ";
            String sqlQueryPart2 = " where employees_info.employee_id = ";
            String sqlQueryPart3 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeeTeamId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
        protected bool UpdateEmployeePosition()
        {
            String sqlQueryPart1 = @"update erp_system.dbo.employees_info
                                     set employees_info.employee_position = ";
            String sqlQueryPart2 = " where employees_info.employee_id = ";
            String sqlQueryPart3 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeePositionId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        protected bool UpdateEmployeeBirthDate()
        {
            String sqlQueryPart1 = @"update erp_system.dbo.employees_info
                                     set employees_info.birth_date = '";
            String sqlQueryPart2 = " ' where employees_info.employee_id = ";
            String sqlQueryPart3 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeeBirthDate();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
        protected bool UpdateEmployeeJoinDate()
        {
            String sqlQueryPart1 = @"update erp_system.dbo.employees_info
                                     set employees_info.join_date = '";
            String sqlQueryPart2 = "' where employees_info.employee_id = ";
            String sqlQueryPart3 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeeJoinDate();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
        protected bool UpdateEmployeeTerminationDate()
        {
            String sqlQueryPart1 = @"update erp_system.dbo.employees_termination_info
                                     set employees_termination_info.resignation_date = '";
            String sqlQueryPart2 = " ' where employees_termination_info.id = ";
            String sqlQueryPart3 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeeTerminationDate();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        protected bool UpdateEmployeeBusinessEmail()
        {
            String sqlQueryPart1 = @"update erp_system.dbo.employees_business_emails
                                     set employees_business_emails.email = '";
            String sqlQueryPart2 = "' where employees_business_emails.id = ";
            String sqlQueryPart3 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeeBusinessEmail();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
        protected bool UpdateEmployeePersonalEmail()
        {
            String sqlQueryPart1 = @"update erp_system.dbo.employees_personal_emails
                                     set employees_personal_emails.email = '";
            String sqlQueryPart2 = "' where employees_personal_emails.id = ";
            String sqlQueryPart3 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeePersonalEmail();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        protected bool UpdateEmployeeBusinessPhone()
        {
            String sqlQueryPart1 = @"update erp_system.dbo.employees_business_phones
                                     set employees_business_phones.phone = '";
            String sqlQueryPart2 = "' where employees_business_phones.id = ";
            String sqlQueryPart3 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeeBusinessPhone();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
        protected bool UpdateEmployeePersonalPhone()
        {
            String sqlQueryPart1 = @"update erp_system.dbo.employees_personal_phones
                                     set employees_personal_phones.phone = '";
            String sqlQueryPart2 = "' where employees_personal_phones.id = ";
            String sqlQueryPart3 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeePersonalPhone();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        protected bool UpdateEmployeeEnrollmentStatus()
        {
            String sqlQueryPart1 = @"update erp_system.dbo.employees_info
                                     set employees_info.currently_enrolled = ";
            String sqlQueryPart2 = " where employees_info.employee_id = ";
            String sqlQueryPart3 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            if (GetCurrentEmployementStatus())
                sqlQuery += 1;
            else
                sqlQuery += 0;
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        protected bool UpdateEmployeeEducationalQualifications()
        {
            String sqlQueryPart1 = @"update erp_system.dbo.employees_educational_qualifications
                                     set employees_educational_qualifications.certificate = ";
            String sqlQueryPart2 = " and employees_educational_qualifications.major = ";
            String sqlQueryPart3 = " and employees_educational_qualifications.graduation_year = ";
            String sqlQueryPart4 = " where employees_educational_qualifications.employee_id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetEmployeeEducationalDegreeId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetEmployeeMajorId();
            sqlQuery += sqlQueryPart3;
            sqlQuery += GetEmployeeGraduationYear();
            sqlQuery += sqlQueryPart3;
            sqlQuery += GetEmployeeId();
            sqlQuery += sqlQueryPart4;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
    }
}
