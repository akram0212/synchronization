﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01electronics_erp
{
    public class Employee
    {
        //SQL QUERY
        private String sqlQuery;

        //SQL OBJECTS
        private SQLServer sqlDatabase;
         
        //EMPLOYEE INFO
        private int employeeId;
        
        private int departmentId;
        private int teamId;
        private int positionId;

        private int educationalQualificationId;
        private int majorId;

        private int graduationYear;

        private Decimal salary;

        private List<COMPANY_ORGANISATION_MACROS.BANK_STRUCT> payrollInfo;

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
        
        private DateTime birthDateStruct;
        private DateTime joinDateStruct;
        
        private String birthDate;
        private String joinDate;
        
        private String educationalQualification;
        private String major;

        public Employee()
        {
            sqlDatabase = new SQLServer();
        }

        public Employee(SQLServer mSqlDatabase)
        {
            sqlDatabase = mSqlDatabase;
        }

        public void SetDatabase(SQLServer mSqlDatabase)
        {
            sqlDatabase = mSqlDatabase;
        }

        public bool InitializeEmployeeInfo(String mBusinessEmail)
        {
            businessEmail = mBusinessEmail;

            String sqlQueryPart1 = @"select employees_info.employee_id, 
		employees_info.employee_department, 
		employees_info.employee_team, 
		employees_info.employee_position, 

		employees_educational_qualifications.graduation_year, 
		employees_educational_qualifications.certificate, 
		employees_educational_qualifications.major,

		employees_info.birth_date, 
		employees_info.join_date, 

		employees_info.name, 
		employees_info.gender, 
		departments_type.department, 
		teams_type.team, 
		positions_type.position, 

		employees_personal_emails.email, 
		employees_business_phones.phone, 
		employees_personal_phones.phone, 

		educational_degrees.educational_degree, 
		educational_majors.educational_major, 

        employees_initials.employee_initial

		from erp_system.dbo.employees_info 
		inner join erp_system.dbo.departments_type 
		on employees_info.employee_department = departments_type.id 
		inner join erp_system.dbo.teams_type 
		on employees_info.employee_team = teams_type.id 
		inner join erp_system.dbo.positions_type 
		on employees_info.employee_position = positions_type.id 
		inner join erp_system.dbo.employees_business_emails 
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
		on educational_degrees.id = employees_info.employee_id
		left join erp_system.dbo.educational_majors 
		on employees_info.employee_id = educational_majors.id
		where employees_business_emails.email = '";
            String sqlQueryPart2 = "';";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += businessEmail;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 7;
            queryColumns.sql_bigint = 0;
            queryColumns.sql_money = 0;
            queryColumns.sql_datetime = 2;
            queryColumns.sql_string = 11;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            employeeId = sqlDatabase.rows[0].sql_int[0];
            departmentId = sqlDatabase.rows[0].sql_int[1];
            teamId = sqlDatabase.rows[0].sql_int[2];
            positionId = sqlDatabase.rows[0].sql_int[3];
            graduationYear = sqlDatabase.rows[0].sql_int[4];

            educationalQualificationId = sqlDatabase.rows[0].sql_int[5];
            majorId = sqlDatabase.rows[0].sql_int[6];

            birthDateStruct = sqlDatabase.rows[0].sql_datetime[0];
            joinDateStruct = sqlDatabase.rows[0].sql_datetime[1];

            joinDate = joinDateStruct.ToString();
            birthDate = birthDateStruct.ToString();

            name = sqlDatabase.rows[0].sql_string[0];
            gender = sqlDatabase.rows[0].sql_string[1];

            department = sqlDatabase.rows[0].sql_string[2];
            team = sqlDatabase.rows[0].sql_string[3];
            position = sqlDatabase.rows[0].sql_string[4];

            personalEmail = sqlDatabase.rows[0].sql_string[5];
            businessPhone = sqlDatabase.rows[0].sql_string[6];
            personalPhone = sqlDatabase.rows[0].sql_string[7];
            
            educationalQualification = sqlDatabase.rows[0].sql_string[8];
            major = sqlDatabase.rows[0].sql_string[9];

            initials = sqlDatabase.rows[0].sql_string[10];

            return true;
        }

        public bool InitializeEmployeeInfo(int mEmployeeId)
        {
            employeeId = mEmployeeId;

            String sqlQueryPart1 = @"select employees_info.employee_department, 
		employees_info.employee_team, 
		employees_info.employee_position, 

		employees_educational_qualifications.graduation_year, 
		employees_educational_qualifications.certificate, 
		employees_educational_qualifications.major, 

		employees_info.birth_date, 
		employees_info.join_date, 

		employees_info.name, 
		employees_info.gender, 
		departments_type.department, 
		teams_type.team, 
		positions_type.position, 

		employees_business_emails.email,
		employees_personal_emails.email, 
		employees_business_phones.phone, 
		employees_personal_phones.phone, 

		educational_degrees.educational_degree, 
		educational_majors.educational_major, 

        employees_initials.employee_initial

		from erp_system.dbo.employees_info 
		inner join erp_system.dbo.departments_type 
		on employees_info.employee_department = departments_type.id 
		inner join erp_system.dbo.teams_type 
		on employees_info.employee_team = teams_type.id 
		inner join erp_system.dbo.positions_type 
		on employees_info.employee_position = positions_type.id 
		inner join erp_system.dbo.employees_business_emails 
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
		where employees_info.employee_id = ";
        
            String sqlQueryPart2 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += employeeId;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 6;
            queryColumns.sql_datetime = 2;
            queryColumns.sql_string = 12;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            departmentId = sqlDatabase.rows[0].sql_int[0];
            teamId = sqlDatabase.rows[0].sql_int[1];
            positionId = sqlDatabase.rows[0].sql_int[2];
            graduationYear = sqlDatabase.rows[0].sql_int[3];

            educationalQualificationId = sqlDatabase.rows[0].sql_int[4];
            majorId = sqlDatabase.rows[0].sql_int[5];

            birthDateStruct = sqlDatabase.rows[0].sql_datetime[0];
            joinDateStruct = sqlDatabase.rows[0].sql_datetime[1];

            joinDate = joinDateStruct.ToString();
            birthDate = birthDateStruct.ToString();

            name = sqlDatabase.rows[0].sql_string[0];
            gender = sqlDatabase.rows[0].sql_string[1];

            department = sqlDatabase.rows[0].sql_string[2];
            team = sqlDatabase.rows[0].sql_string[3];
            position = sqlDatabase.rows[0].sql_string[4];

            businessEmail = sqlDatabase.rows[0].sql_string[5];
            personalEmail = sqlDatabase.rows[0].sql_string[6];

            businessPhone = sqlDatabase.rows[0].sql_string[7];
            personalPhone = sqlDatabase.rows[0].sql_string[8];

            educationalQualification = sqlDatabase.rows[0].sql_string[9];
            major = sqlDatabase.rows[0].sql_string[10];

            initials = sqlDatabase.rows[0].sql_string[11];

      
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

        public void SetEmployeeCompanyEmail(String mBusinessEmail)
        {
            businessEmail = mBusinessEmail;
        }
        public void SetEmployeePersonalEmail(String mPersonalEmail)
        {
            personalEmail = mPersonalEmail;
        }

        public void SetEmployeeCompanyPhone(String mBusinessPhone)
        {
            businessPhone = mBusinessPhone;
        }

        public void SetEmployeeBirthDate(String mBirthDate)
        {
            birthDate = mBirthDate;
        }
        public void SetEmployeeJoinDate(String mJoinDate)
        {
            joinDate = mJoinDate;
        }

        public void SetEmployeeEducationalQualification(int mEducationalQualificationId, String mEducationalQualification)
        {
            educationalQualificationId = mEducationalQualificationId;
            educationalQualification = mEducationalQualification;
        }

        public void SetEmployeeMajor(int mMajorId, String mMajor)
        {
            majorId = mMajorId;
            major = mMajor;
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

        public String GetEmployeeBirthDate()
        {
            return birthDate;
        }
        public String GetEmployeeJoinDate()
        {
            return joinDate;
        }

        public String GetEmployeeEducationalQualification()
        {
            return educationalQualification;
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
        public int GetEmployeeEducationalQualificationId()
        {
            return educationalQualificationId;
        }
        public int GetEmployeeMajorId()
        {
            return majorId;
        }
        public DateTime GetEmployeeBirthDateStruct()
        {
            return birthDateStruct;
        }
        public DateTime GetEmployeeJoinDateStruct()
        {
            return joinDateStruct;
        }
    }
}
