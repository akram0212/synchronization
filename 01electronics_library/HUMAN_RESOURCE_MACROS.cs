using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01electronics_library
{
    public class HUMAN_RESOURCE_MACROS
    {
        public const int WORKING_HOURS = 8;

        public const int NO_OF_ATTENDANCE_TYPES = 14;

        public const int ATTENDANCE_OK = 0;
        public const int ATTENDANCE_LATENCY_FIRST = 1;
        public const int ATTENDANCE_LATENCY_SECOND = 2;
        public const int ATTENDANCE_LATENCY_THIRD = 3;
        public const int ATTENDANCE_EARLY_LEAVE = 4;
        public const int ATTENDANCE_ABSCENT = 5;
        public const int ATTENDANCE_WEEKEND = 6;
        public const int ATTENDANCE_HOLIDAY = 7;
        public const int ATTENDANCE_VACATION = 8;
        public const int ATTENDANCE_PETITIONED_EARLY_LEAVE = 9;
        public const int ATTENDANCE_PETITIONED_LATE_ARRIVAL = 10;
        public const int ATTENDANCE_MISSION_EARLY_LEAVE = 11;
        public const int ATTENDANCE_MISSION_LATE_ARRIVAL = 12;
        public const int ATTENDANCE_MISSION_ABSENCE = 13;

        public const int VACATION = 1;
        public const int EARLY_LEAVE_PETITION = 8;
        public const int LATE_ARRIVAL_PETITION = 9;

        public const int PENDING_VACATION_STATUS = 1;
        public const int TEAM_LEAD_APPROVED_VACATION_STATUS = 2;
        public const int DEP_MANAGER_APPROVED_VACATION_STATUS = 3;
        public const int HR_APPROVED_VACATION_STATUS = 4;
        public const int TEAM_LEAD_REJECTED_VACATION_STATUS = 5;
        public const int DEP_MANAGER_REJECTED_VACATION_STATUS = 6;
        public const int HR_REJECTED_VACATION_STATUS = 7;

        public static TimeSpan CHECKIN_CUT_TIME = new TimeSpan(9, 0, 0);
        public static TimeSpan CHECKOUT_CUT_TIME = new TimeSpan(16, 15, 0);

        public static TimeSpan LATE_ARRIVAL_DEDUCTION_INTERVAL = new TimeSpan(0, 15, 0);
        public static TimeSpan LATE_ARRIVAL_FULL_DAY_DEDUCTION_CUT_TIME = new TimeSpan(9, 30, 0);

        public const DayOfWeek FRIDAY = DayOfWeek.Friday;
        public const DayOfWeek SATURDAY = DayOfWeek.Saturday;

        public struct EMPLOYEE_UPDATE_STRUCT
        {
            public String employeeName;

            public COMPANY_ORGANISATION_MACROS.DEPARTMENT_STRUCT employeeDepartment;
            public COMPANY_ORGANISATION_MACROS.TEAM_STRUCT employeeTeam;
            public COMPANY_ORGANISATION_MACROS.EMPLOYEE_POSITION_STRUCT employeePosition;

            public DateTime employeeBirthDate;
            public DateTime employeeJoinDate;
            public DateTime employeeTerminationDate;

            public String employeeBusinessEmail;
            public String employeePersonalEmail;

            public String employeeBusinessPhone;
            public String employeePersonalPhone;

            public Decimal employeeGrossSalary;
            public Decimal employeeInsuranceAndTaxes;
            public Decimal employeeNetSalary;

            public String employeeNationalId;

            public bool employeeEnrollmentStatus;

            public BASIC_STRUCTS.EDUCATIONAL_DEGREE_STRUCT employeeEducationalDegree;
            public BASIC_STRUCTS.EDUCATIONAL_MAJOR_STRUCT employeeEductationalMajor;
            public int employeeGraduationYear;

            public bool employeeNameEdited;

            public bool employeeDepartmentEdited;
            public bool employeeTeamEdited;
            public bool employeePositionEdited;

            public bool employeeBirthDateEdited;
            public bool employeeJoinDateEdited;
            public bool employeeTerminationDateEdited;

            public bool employeeBusinessEmailEdited;
            public bool employeePersonalEmailEdited;
            
            public bool employeeBusinessPhoneEdited;
            public bool employeePersonalPhoneEdited;

            public bool employeeGrossSalaryEdited;
            public bool employeeInsuranceAndTaxesEdited;
            public bool employeeNetSalaryEdited;

            public bool employeeNationalIdEdited;

            public bool currentlyEmployedEdited;

            public bool degreeEdited;
            public bool majorEdited;
            public bool graduationYearEdited;
        }

        //PAYROLL AND SALARIES STRUCTS
        public struct BANK_STRUCT
        {
            public String payroll_type;
            public int bank_id;

            public String branch_name;
            public Int16 branch_id;

            public int payroll_id;

            public ulong account_id;
            public ulong iban_id;
        };

        public struct PAYROLL_TYPE_STRUCT
        {
            public String payroll_type_name;
            public int payroll_type_id;
        }
        public struct PAYROLL_STRUCT
        {
            public String employee_name;
            public int employee_id;

            public List<BANK_STRUCT> banksList;
        }

        public struct BASIC_SALARY_STRUCT
        {
            public String employee_name;
            public int employee_id;

            public Decimal gross_salary;
            public Decimal insurance_and_tax;
            public Decimal net_salary;

            public int due_year;
            public int due_month;
        };

        public struct MONTHLY_SALARY_STRUCT
        {
            public String payroll_type;
            public int payroll_id;

            public int year;
            public int month;
            public DateTime transaction_time;

            public Decimal received_salary;
        };

        public struct SALARIES_HISTORY_STRUCT
        {
            public String employee_name;
            public int employee_id;

            public List<MONTHLY_SALARY_STRUCT> monthlyList;
        };

        public struct ATTENDANCE_STRUCT
        {
            public int attendance_id;
            public String attendance_type;
        };

        public struct ATTENDANCE_COUNT_STRUCT
        {
            public int attendance_id;
            public String attendance_type;

            public int count;
        };
        public struct MISSION_TYPE_STRUCT
        {
            public int mission_id;
            public String mission_type;
        };

        public struct ATTENDANCE_EXCUSES_STRUCT
        {
            public int excuse_id;
            public String attendance_excuse;
        }
        public struct VACATION_STATUS_STRUCT
        {
            public int status_id;
            public String vacation_status;
        }
        public struct FIRSTIN_LASTOUT_STRUCT
        {
            public DateTime workDay;
            public TimeSpan firstIn;
            public TimeSpan lastOut;
        };

        public struct EMPLOYEE_ATTENDANCE_SUMMARY_STRUCT
        {
            public String employee_name;
            public int employee_id;
            public int[] attendanceTypeCount;
        };

        public struct EMPLOYEE_ATTENDANCE_DETAILS_STRUCT
        {
            public String employee_name;
            public int employee_id;
            public FIRSTIN_LASTOUT_STRUCT[] firstInLastOutRecord;
            public ATTENDANCE_STRUCT[] attendanceStatus;
        };

        public struct EMPLOYEE_MONTH_DEDUCTION_STRUCT
        {
            public String employee_name;
            public int employee_id;

            public Decimal basic_salary;
            public Decimal attendance_deductions;
            public Decimal other_deductions;

            public int deduct_work_hour;
            public int deduct_full_day;

            public int[] attendanceCount;
        };

        public struct PETITION_REQUEST_STRUCT
        {
            public ATTENDANCE_EXCUSES_STRUCT attendance_excuse;

            public String beneficiary_personnel_name;
            public int beneficiary_personnel_id;

            public String requestor_name;
            public int requestor_id;

            public int request_serial;

            public DateTime requested_date;
            public DateTime date_added;

        }
        public struct PUBLIC_HOLIDAY_STRUCT
        {
            public String added_by_name;
            public int added_by_id;

            public String holiday_name;
            public int holiday_serial;

            public DateTime start_date;
            public DateTime end_date;
            public DateTime date_added;

        }

        public struct MISSION_STRUCT
        {
            public int mission_serial;

            public String added_by_name;
            public int added_by_id;

            public String employee_on_mission_name;
            public int employee_on_mission_id;

            public MISSION_TYPE_STRUCT mission_type;

            public String company_name;
            public String work_order;

            public DateTime mission_date;

            public DateTime date_added;

            public COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT branch;
        }

        public struct VACATION_STRUCT
        {
            public int request_serial;

            public COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT benificiary_personnel;
            public COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT requestor;

            public List<VACATION_APPROVAL_REJECTION_LIST_STRUCT> listOfApprovals;

            public ATTENDANCE_EXCUSES_STRUCT vacation_type;

            public DateTime vacation_start_date;
            public DateTime vacation_end_date;

            public DateTime request_issue_date;
            public DateTime request_expiry_date;

            public VACATION_STATUS_STRUCT vacation_status;
        }

        public struct VACATION_APPROVAL_REJECTION_LIST_STRUCT
        {
            public COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT approver;

            public DateTime approval_rejection_date;
        }

        public struct VACATION_APPROVAL_REJECTION_CLASS_STRUCT
        {
            public Employee approver;

            public DateTime approvalRejectionDate;

            public String approvalRejectionComments;
        }
    }
}
