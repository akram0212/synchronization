using _01electronics_library;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for ReassignContactsWindow.xaml
    /// </summary>
    public partial class ReassignContactsWindow : Window
    {
        private CommonQueries commonQueries;
        private Employee loggedInUser;
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> employees;
        private SQLServer sql;
        private String sqlQuery;


        private struct contact_database_table_struct
        {
            public int sales_person_id;
            public int branch_serial;
            public int contact_id;
            public String email;
            public String name;
            public String gender;
            public int title;
        };

        private List<contact_database_table_struct> employeeContacts;


        public ReassignContactsWindow(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;
            commonQueries = new CommonQueries();
            employees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
            sql = new SQLServer();

            employeeContacts = new List<contact_database_table_struct>();

            InitializeComponent();

            if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION)
            {
                if (!commonQueries.GetAllTeamEmployees(loggedInUser.GetEmployeeTeamId(), ref employees))
                    return;
            }
            else if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)
            {
                if (!commonQueries.GetAllDepartmentEmployees(loggedInUser.GetEmployeeDepartmentId(), ref employees))
                    return;
            }

            for (int i = 0; i < employees.Count; i++)
            {
                ComboBoxItem oldEmployeeName = new ComboBoxItem();

                oldEmployeeName.Content = employees[i].employee_name;
                if (employees[i].employement_status_id < 4)
                    oldEmployeeName.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                else
                    oldEmployeeName.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));

                ComboBoxItem newEmployeeName = new ComboBoxItem();

                newEmployeeName.Content = employees[i].employee_name;
                if (employees[i].employement_status_id < 4)
                    newEmployeeName.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                else
                    newEmployeeName.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));

                oldAssigneeCombo.Items.Add(oldEmployeeName);
                newAssigneeCombo.Items.Add(newEmployeeName);

            }
        }

        private void OnSelChangedOldAssignee(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnSelChangedNewAssignee(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnClickFinish(object sender, RoutedEventArgs e)
        {
            if (!GetAllEmployeesContacts(ref employeeContacts))
                return;

            if (!UpdateOldAssigneeContacts())
                return;

            if (!InsertNewAssigneeContacts())
                return;

            if (!UpdateContactPhones())
                return;

            if (!UpdateContactEmails())
                return;

            this.Close();
        }


        private bool GetAllEmployeesContacts(ref List<contact_database_table_struct> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select sales_person_id
                                            ,branch_serial
                                            ,contact_id
                                            ,department
                                            ,email
                                            ,name
                                            ,gender
	                                        from erp_system.dbo.contact_person_info
	                                        where sales_person_id = ";
            String sqlQueryPart2 = " and is_valid = 1";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += employees[oldAssigneeCombo.SelectedIndex].employee_id;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 4;
            queryColumns.sql_string = 3;

            if (!sql.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sql.rows.Count; i++)
            {
                contact_database_table_struct tempItem = new contact_database_table_struct();

                tempItem.sales_person_id = sql.rows[i].sql_int[0];
                tempItem.branch_serial = sql.rows[i].sql_int[1];
                tempItem.contact_id = sql.rows[i].sql_int[2];
                tempItem.title = sql.rows[i].sql_int[3];

                tempItem.email = sql.rows[i].sql_string[0];
                tempItem.name = sql.rows[i].sql_string[1];
                tempItem.gender = sql.rows[i].sql_string[2];


                returnVector.Add(tempItem);
            }

            return true;
        }

        private bool UpdateOldAssigneeContacts()
        {
            String sqlQueryPart1 = @"update erp_system.dbo.contact_person_info
                                     set is_valid = 0
	                                        where sales_person_id = ";
            String sqlQueryPart2 = " and branch_serial = ";
            String sqlQueryPart3 = " and contact_id = ";

            for (int i = 0; i < employeeContacts.Count; i++)
            {
                sqlQuery = String.Empty;
                sqlQuery += sqlQueryPart1;
                sqlQuery += employeeContacts[i].sales_person_id;
                sqlQuery += sqlQueryPart2;
                sqlQuery += employeeContacts[i].branch_serial;
                sqlQuery += sqlQueryPart3;
                sqlQuery += employeeContacts[i].contact_id;

                if (!sql.InsertRows(sqlQuery))
                    return false;
            }

            return true;
        }

        private bool InsertNewAssigneeContacts()
        {
            String sqlQueryPart1 = @"insert into erp_system.dbo.contact_person_info
                                     values(";
            String comma = ", ";

            for (int i = 0; i < employeeContacts.Count; i++)
            {
                sqlQuery = String.Empty;
                sqlQuery += sqlQueryPart1;
                sqlQuery += employees[newAssigneeCombo.SelectedIndex].employee_id;
                sqlQuery += comma;
                sqlQuery += employeeContacts[i].branch_serial;
                sqlQuery += comma;
                sqlQuery += employeeContacts[i].contact_id;
                sqlQuery += comma;
                sqlQuery += "'" + employeeContacts[i].email + "'";
                sqlQuery += comma;
                sqlQuery += "'" + employeeContacts[i].name + "'";
                sqlQuery += comma;
                sqlQuery += "'" + employeeContacts[i].gender + "'";
                sqlQuery += comma;
                sqlQuery += employeeContacts[i].title;
                sqlQuery += ",1, getdate());";

                if (!sql.InsertRows(sqlQuery))
                    return false;
            }

            return true;
        }

        private bool UpdateContactPhones()
        {
            String sqlQueryPart1 = @"update erp_system.dbo.contact_person_mobile
                                     set sales_person_id = ";
            String sqlQueryPart2 = " where sales_person_id = ";
            String sqlQueryPart3 = " and branch_serial = ";
            String sqlQueryPart4 = " and contact_id = ";

            for (int i = 0; i < employeeContacts.Count; i++)
            {
                sqlQuery = String.Empty;
                sqlQuery += sqlQueryPart1;
                sqlQuery += employees[newAssigneeCombo.SelectedIndex].employee_id;
                sqlQuery += sqlQueryPart2;
                sqlQuery += employeeContacts[i].sales_person_id;
                sqlQuery += sqlQueryPart3;
                sqlQuery += employeeContacts[i].branch_serial;
                sqlQuery += sqlQueryPart4;
                sqlQuery += employeeContacts[i].contact_id;

                if (!sql.InsertRows(sqlQuery))
                    return false;
            }

            return true;
        }

        private bool UpdateContactEmails()
        {
            String sqlQueryPart1 = @"update erp_system.dbo.contact_person_personal_emails
                                     set sales_person_id = ";
            String sqlQueryPart2 = " where sales_person_id = ";
            String sqlQueryPart3 = " and branch_serial = ";
            String sqlQueryPart4 = " and contact_id = ";

            for (int i = 0; i < employeeContacts.Count; i++)
            {
                sqlQuery = String.Empty;
                sqlQuery += sqlQueryPart1;
                sqlQuery += employees[newAssigneeCombo.SelectedIndex].employee_id;
                sqlQuery += sqlQueryPart2;
                sqlQuery += employeeContacts[i].sales_person_id;
                sqlQuery += sqlQueryPart3;
                sqlQuery += employeeContacts[i].branch_serial;
                sqlQuery += sqlQueryPart4;
                sqlQuery += employeeContacts[i].contact_id;

                if (!sql.InsertRows(sqlQuery))
                    return false;
            }

            return true;
        }
    }
}
