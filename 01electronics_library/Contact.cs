using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Collections;
using System.Windows;

namespace _01electronics_library
{
    public class Contact : Company
    {
        //SQL QUERY
        protected String sqlQuery;

        //SQL OBJECTS
        protected SQLServer sqlDatabase;

        //SALES PERSON INFO
        Employee salesPerson;

        //CONTACT BASIC INFO
        String contactName;
        String businessEmail;
        String gender;

        String[] contactPhones;
        String[] contactPersonalEmails;

        String department;

        List<COMPANY_ORGANISATION_MACROS.CONTACT_COMMENT_STRUCT> commentsList;

        //CONTACT IDENTIFIERS
        int contactId;
        int departmentId;

        //CONTACT ADDITIONAL INFO
        int numberOfSavedPhones;
        int numberOfSavedEmails;

        public Contact()
        {
            new Company();
            salesPerson = new Employee();

            sqlDatabase = new SQLServer();

            contactPhones = new String[COMPANY_ORGANISATION_MACROS.MAX_TELEPHONES_PER_CONTACT];
            contactPersonalEmails = new String[COMPANY_ORGANISATION_MACROS.MAX_EMAILS_PER_CONTACT];

            commentsList = new List<COMPANY_ORGANISATION_MACROS.CONTACT_COMMENT_STRUCT>();

        }
        public Contact(SQLServer mSqlDatabase)
        {
            new Company(mSqlDatabase);
            salesPerson = new Employee(ref mSqlDatabase);

            sqlDatabase = mSqlDatabase;

            contactPhones = new String[COMPANY_ORGANISATION_MACROS.MAX_TELEPHONES_PER_CONTACT];
            contactPersonalEmails = new String[COMPANY_ORGANISATION_MACROS.MAX_EMAILS_PER_CONTACT];

            commentsList = new List<COMPANY_ORGANISATION_MACROS.CONTACT_COMMENT_STRUCT>();
        }

        public bool InitializeContactInfo(int mContactId)
        {
            contactId = mContactId;

            String sqlQueryPart1 = @"select contact_person_mobile.telephone_id, 

                                    contact_person_personal_emails.email_id, 
									contact_person_info.department, 
									contact_person_info.name, 
									contact_person_info.gender, 
									contact_person_info.email, 
									departments_type.department, 
									contact_person_mobile.mobile, 
									contact_person_personal_emails.personal_email 

							from erp_system.dbo.contact_person_info 

							inner join erp_system.dbo.departments_type 
							on contact_person_info.department = departments_type.id 

							left join erp_system.dbo.contact_person_mobile 
							on contact_person_info.sales_person_id = contact_person_mobile.sales_person_id 
							and contact_person_info.branch_serial = contact_person_mobile.branch_serial 
							and contact_person_info.contact_id = contact_person_mobile.contact_id 

							left join erp_system.dbo.contact_person_personal_emails 
							on contact_person_info.sales_person_id = contact_person_personal_emails.sales_person_id 
							and contact_person_info.branch_serial = contact_person_personal_emails.branch_serial 
							and contact_person_info.contact_id = contact_person_personal_emails.contact_id 

							where contact_person_info.sales_person_id = ";

            String sqlQueryPart2 = " and contact_person_info.branch_serial = ";
            String sqlQueryPart3 = " and contact_person_info.contact_id = ";
            String sqlQueryPart4 = " order by contact_person_mobile.telephone_id, contact_person_personal_emails.email_id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += salesPerson.GetEmployeeId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetAddressSerial();
            sqlQuery += sqlQueryPart3;
            sqlQuery += contactId;
            sqlQuery += sqlQueryPart4;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 3;
            queryColumns.sql_string = 6;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            contactName = sqlDatabase.rows[0].sql_string[0];
            gender = sqlDatabase.rows[0].sql_string[1];
            businessEmail = sqlDatabase.rows[0].sql_string[2];
            department = sqlDatabase.rows[0].sql_string[3];

            departmentId = sqlDatabase.rows[0].sql_int[2];

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                numberOfSavedPhones = sqlDatabase.rows[i].sql_int[0];

                if (numberOfSavedPhones > 0)
                    contactPhones[numberOfSavedPhones - 1] = sqlDatabase.rows[i].sql_string[4];
            }

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                numberOfSavedEmails = sqlDatabase.rows[i].sql_int[1];

                if (numberOfSavedEmails > 0)
                    contactPersonalEmails[numberOfSavedEmails - 1] = sqlDatabase.rows[i].sql_string[5];
            }


            QueryContactComment(GetSalesPersonId(), GetAddressSerial(), GetContactId());

            return true;
        }

        public bool InitializeContactInfo(Employee mSalesPerson, int mAddressSerial, int mContactId)
        {
            salesPerson = mSalesPerson;

            if (!InitializeBranchInfo(mAddressSerial))
                return false;

            if (!InitializeContactInfo(mContactId))
                return false;

            return true;
        }

        public bool InitializeContactInfo(int mEmployeeId, int mAddressSerial, int mContactId)
        {
            if (!InitializeSalesPersonInfo(mEmployeeId))
                return false;

            if (!InitializeBranchInfo(mAddressSerial))
                return false;

            if (!InitializeContactInfo(mContactId))
                return false;

            return true;
        }

        public bool InitializeContactInfo(int mAddressSerial, int mContactId)
        {
            if (!InitializeBranchInfo(mAddressSerial))
                return false;

            if (!InitializeContactInfo(mContactId))
                return false;

            return true;
        }
        public bool InitializeSalesPersonInfo(int mEmployeeId)
        {
            if (!salesPerson.InitializeEmployeeInfo(mEmployeeId))
                return false;

            return true;
        }

        public bool QueryContactComment(int salesPersonId, int branchSerial, int contactId)
        {
            String sqlQueryPart1 = @"select contact_person_comment.date_added, contact_person_comment.comment_on_contact from erp_system.dbo.contact_person_comment where contact_person_comment.contact_id = ";
            String sqlQueryPart2 = @" and contact_person_comment.sales_person_id = ";
            String sqlQueryPart3 = @" and contact_person_comment.branch_serial = ";
            String sqlQueryPart4 = @" order by contact_person_comment.date_added DESC;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += salesPersonId;
            sqlQuery += sqlQueryPart2;
            sqlQuery += branchSerial;
            sqlQuery += sqlQueryPart3;
            sqlQuery += contactId;
            sqlQuery += sqlQueryPart4;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_datetime = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_LOW))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.CONTACT_COMMENT_STRUCT tempItem;

                tempItem.commentDate = sqlDatabase.rows[i].sql_datetime[0].ToString();
                tempItem.comment = sqlDatabase.rows[i].sql_string[0];

                commentsList.Add(tempItem);
            }

            return true;
        }

        public void SetSalesPerson(Employee mSalesPerson)
        {
            salesPerson = mSalesPerson;
        }
        public void SetContactId(int mContactId)
        {
            contactId = mContactId;
        }
        public void SetContactDepartment(int mDepartmentId, String mDepartment)
        {
            departmentId = mDepartmentId;
            department = mDepartment;
        }

        public void SetContactName(String mName)
        {
            contactName = mName;
        }
        public void SetContactBusinessEmail(String mBusinessEmail)
        {
            businessEmail = mBusinessEmail;
        }
        public void SetContactGender(String mGender)
        {
            gender = mGender;
        }

        public void AddNewContactPhone(String newPhone)
        {
            contactPhones[numberOfSavedPhones++] = newPhone;
        }
        public void AddNewContactEmail(String newEmail)
        {
            contactPersonalEmails[numberOfSavedEmails++] = newEmail;
        }

        public void UpdateContactPhone(int phoneId, String phoneNumber)
        {
            contactPhones[phoneId - 1] = phoneNumber;
        }
        public void UpdateContactEmail(int emailId, String email)
        {
            contactPersonalEmails[emailId - 1] = email;
        }

        public void RemoveContactPhone(int phoneId)
        {
            for (int i = phoneId - 1; i < COMPANY_ORGANISATION_MACROS.MAX_TELEPHONES_PER_CONTACT - 1; i++)
                contactPhones[i] = contactPhones[i + 1];

            contactPhones[COMPANY_ORGANISATION_MACROS.MAX_TELEPHONES_PER_CONTACT - 1] = "";

            numberOfSavedPhones--;
        }
        public void RemoveContactEmail(int emailId)
        {
            for (int i = emailId - 1; i < COMPANY_ORGANISATION_MACROS.MAX_EMAILS_PER_CONTACT - 1; i++)
                contactPersonalEmails[i] = contactPersonalEmails[i + 1];

            contactPersonalEmails[COMPANY_ORGANISATION_MACROS.MAX_EMAILS_PER_CONTACT - 1] = "";

            numberOfSavedEmails--;
        }

        public void AddNewContactComment(String contactComment)
        {
            COMPANY_ORGANISATION_MACROS.CONTACT_COMMENT_STRUCT commentStruct;

            commentStruct.commentDate = DateTime.Now.ToString();
            commentStruct.comment = contactComment;

            commentsList.Add(commentStruct);
        }

        public Company GetContactCompany()
        {
            return this;
        }
        public bool GetNewContactId()
        {
            String sqlQueryPart1 = @"select max(contact_person_info.contact_id) from erp_system.dbo.contact_person_info 
                                        where contact_person_info.sales_person_id = ";
            String sqlQueryPart2 = @" and contact_person_info.branch_serial = ";
            String sqlQueryPart3 = @";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetSalesPersonId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetAddressSerial();
            sqlQuery += sqlQueryPart3;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            contactId = sqlDatabase.rows[0].sql_int[0] + 1;

            return true;
        }

        public int GetSalesPersonId()
        {
            return salesPerson.GetEmployeeId();
        }
        public int GetSalesPersonTeamId()
        {
            return salesPerson.GetEmployeeTeamId();
        }

        public int GetContactId()
        {
            return contactId;
        }
        public int GetContactDepartmentId()
        {
            return departmentId;
        }

        public int GetNumberOfSavedContactPhones()
        {
            return numberOfSavedPhones;
        }
        public int GetNumberOfSavedContactEmails()
        {
            return numberOfSavedEmails;
        }

        public ref Employee GetSalesPerson()
        {
            return ref salesPerson;
        }

        public String GetContactName()
        {
            return contactName;
        }
        public String GetContactDepartment()
        {
            return department;
        }

        public String GetContactBusinessEmail()
        {
            return businessEmail;
        }
        public String GetContactGender()
        {
            return gender;
        }

        public String[] GetContactPhones()
        {
            return contactPhones;
        }
        public String[] GetContactPersonalEmails()
        {
            return contactPersonalEmails;
        }

        public bool IssueNewContact()
        {
            if (!GetNewContactId())
                return false;

            if (!InsertIntoContactInfo())
                return false;

            return true;
        }

        //////////////////////////////////////////////////////////////////////
        //QUERY FUNCTIONS
        //////////////////////////////////////////////////////////////////////
        public bool InsertIntoContactInfo()
        {
            String sqlQueryPart1 = @"insert into erp_system.dbo.contact_person_info values(";
            String sqlQueryPart2 = ", ";
            String sqlQueryPart3 = "getdate()) ;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetSalesPersonId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetAddressSerial();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetContactId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += "'" + GetContactBusinessEmail() + "'";
            sqlQuery += sqlQueryPart2;
            sqlQuery += "'" + GetContactName() + "'";
            sqlQuery += sqlQueryPart2;
            sqlQuery += "'" + GetContactGender() + "'";
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetContactDepartmentId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += 1;
            sqlQuery += sqlQueryPart2;
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
        public bool InsertIntoContactPersonalEmail(int mEmailId, String mContactEmail)
        {
            String sqlQueryPart1 = @"insert into erp_system.dbo.contact_person_personal_emails values(";
            String sqlQueryPart2 = ", ";
            String sqlQueryPart3 = "getdate()) ;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetSalesPersonId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetAddressSerial();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetContactId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += mEmailId;
            sqlQuery += sqlQueryPart2;
            sqlQuery += "'" + mContactEmail + "'";
            sqlQuery += sqlQueryPart2;
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
        public bool InsertIntoContactMobile(int mPhoneId, String mContactPhone)
        {
            String sqlQueryPart1 = @"insert into erp_system.dbo.contact_person_mobile values(";
            String sqlQueryPart2 = ", ";
            String sqlQueryPart3 = "getdate()) ;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetSalesPersonId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetAddressSerial();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetContactId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += mPhoneId;
            sqlQuery += sqlQueryPart2;
            sqlQuery += "'" + mContactPhone + "'";
            sqlQuery += sqlQueryPart2;
            sqlQuery += sqlQueryPart3;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;


            return true;
        }

        public List<COMPANY_ORGANISATION_MACROS.CONTACT_COMMENT_STRUCT> GetContactComments()
        {
            return commentsList;
        }
    }
}