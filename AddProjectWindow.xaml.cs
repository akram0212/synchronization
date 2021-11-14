using _01electronics_library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for AddProjectWindow.xaml
    /// </summary>
    public partial class AddProjectWindow : Window
    {
        public Employee loggedInUser;

        protected SQLServer sqlServer;
        protected String sqlQuery;
        protected CommonQueries commonQueries;
        protected CommonFunctions commonFunctions;
        protected IntegrityChecks integrityChecker;
        protected BASIC_STRUCTS.PROJECT_STRUCT project = new BASIC_STRUCTS.PROJECT_STRUCT();
        public AddProjectWindow(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;

            sqlServer = new SQLServer();
            commonQueries = new CommonQueries();
            commonFunctions = new CommonFunctions();
            integrityChecker = new IntegrityChecks();
        }

        private void OnSelChangedProjecNameTextBox(object sender, RoutedEventArgs e)
        {

        }

        private void OnSelChangedProjecIDTextBox(object sender, RoutedEventArgs e)
        {

        }

        private void OnBtnClickSaveChanges(object sender, RoutedEventArgs e)
        {
            if (!CheckProjectNameEditBox())
                return;
            if (!CheckProjectIDEditBox())
                return;
            if (!GetMaxProjectSerial())
                return;
            if (!InsertNewProject())
                return;
            this.Close();

        }
        private bool CheckProjectIDEditBox()
        {
            if (ProjecIDTextBox.Text.Length > 15)
            {
                System.Windows.Forms.MessageBox.Show("Project ID can't exceed 15 letters.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            project.project_id = ProjecIDTextBox.Text.ToString();

            return true;
        }
        private bool CheckProjectNameEditBox()
        {
            String inputString = ProjecNameTextBox.Text;
            String outputString = ProjecNameTextBox.Text;

            if (!integrityChecker.CheckProjectNameEditBox(inputString, ref outputString, false))
                return false;

            ProjecNameTextBox.Text = outputString;
            project.project_name = outputString;

            return true;
        }
        public bool GetMaxProjectSerial()
        {
            String sqlQueryPart1 = " select max(projects_info.project_serial) from erp_system.dbo.projects_info ;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlServer.GetRows(sqlQuery, queryColumns))
                return false;

            project.project_serial = sqlServer.rows[0].sql_int[0] + 1;


            return true;
        }
        
        public bool InsertNewProject()
        {
            String sqlQueryPart1 = @"insert into erp_system.dbo.projects_info
                                     values( ";
            String sqlQueryPart2 = " GETDATE());";
            String comma = ", ";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += project.project_serial;
            sqlQuery += comma;
            sqlQuery += "'" + project.project_name + "'";
            sqlQuery += comma;
            sqlQuery += "'" + project.project_id + "'";
            sqlQuery += comma;
            sqlQuery += sqlQueryPart2;

            if (!sqlServer.InsertRows(sqlQuery))
                return false;

            return true;
        }
    }
}
