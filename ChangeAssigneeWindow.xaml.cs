using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using _01electronics_library;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for ChangeAssigneeWindow.xaml
    /// </summary>
    public partial class ChangeAssigneeWindow : Window
    {
        CommonQueries commonQueries;
        SQLServer sqlServer;

        List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> preSalesEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        List<COMPANY_WORK_MACROS.FAILURE_REASON_STRUCT> failureReasons = new List<COMPANY_WORK_MACROS.FAILURE_REASON_STRUCT>();
        

        int condition;

        int updateSerial;

        String oldAssignee;

        RFQ rfq;
        OutgoingQuotation outgoingQuotation;

        public ChangeAssigneeWindow(ref RFQ mrfq)
        {
            InitializeComponent();
            condition = 0;
            rfq = mrfq;

            commonQueries = new CommonQueries();
            sqlServer = new SQLServer();

            commonQueries.GetTeamEmployees(COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID, ref preSalesEmployees);

            PreSalesEngineersComboBox.Items.Clear();

            for (int i = 0; i < preSalesEmployees.Count(); i++)
                PreSalesEngineersComboBox.Items.Add(preSalesEmployees[i].employee_name);

            oldAssignee = rfq.GetAssigneeName();
            PreSalesEngineersComboBox.SelectedItem = oldAssignee;

            
        }

        public ChangeAssigneeWindow(ref OutgoingQuotation mWorkOffer, List<COMPANY_WORK_MACROS.FAILURE_REASON_STRUCT> mFailureReasons)
        {
            InitializeComponent();

            condition = 1;
            outgoingQuotation = mWorkOffer;
            failureReasons = mFailureReasons;
            
            commonQueries = new CommonQueries();
            sqlServer = new SQLServer();

            headerLabel.Content = "CHOOSE FAILURE REASON";
            comboBoxLabel.Content = "Failure Reasons";

            for (int i = 0; i < mFailureReasons.Count; i++)
                PreSalesEngineersComboBox.Items.Add(mFailureReasons[i].reason_name);

        }

        public ChangeAssigneeWindow(ref RFQ mRFQ, List<COMPANY_WORK_MACROS.FAILURE_REASON_STRUCT> mFailureReasons)
        {
            InitializeComponent();

            condition = 2;
            rfq = mRFQ;
            failureReasons = mFailureReasons;

            commonQueries = new CommonQueries();
            sqlServer = new SQLServer();

            headerLabel.Content = "CHOOSE FAILURE REASON";
            comboBoxLabel.Content = "Failure Reasons";

            for (int i = 0; i < mFailureReasons.Count; i++)
                PreSalesEngineersComboBox.Items.Add(mFailureReasons[i].reason_name);

        }
        private void OnSelChangedChangeAssignee(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void OnBtnClickSaveChanges(object sender, RoutedEventArgs e)
        {
            if(PreSalesEngineersComboBox.Text != oldAssignee && condition == 0)
            {
                GetNewUpdateSerial();
                    
                
                if (!InsertIntoUpdatedRFQs())
                    return;

                if (!UpdateRFQsAssignee())
                    return;
            }

            else if(condition == 1)
            {
                outgoingQuotation.RejectOffer(failureReasons[PreSalesEngineersComboBox.SelectedIndex].reason_id, failureReasons[PreSalesEngineersComboBox.SelectedIndex].reason_name);
            }
            else if(condition == 2)
            {
                rfq.RejectRFQ(failureReasons[PreSalesEngineersComboBox.SelectedIndex].reason_id, failureReasons[PreSalesEngineersComboBox.SelectedIndex].reason_name);
            }

            this.Close();
        }

        private bool GetNewUpdateSerial()
        {
            String sqlQueryPart1 = "select max(updated_rfqs_assignees.update_serial) from erp_system.dbo.updated_rfqs_assignees where updated_rfqs_assignees.sales_person = ";
            String sqlQueryPart2 = " and updated_rfqs_assignees.rfq_serial = ";
            String sqlQueryPart3 = " and updated_rfqs_assignees.rfq_version = ";
            String sqlQueryPart4 = ";";

            string sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += rfq.GetSalesPersonId();
            sqlQuery += sqlQueryPart2;
            sqlQuery += rfq.GetRFQSerial();
            sqlQuery += sqlQueryPart3;
            sqlQuery += rfq.GetRFQVersion();
            sqlQuery += sqlQueryPart4;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlServer.GetRows(sqlQuery, queryColumns))
                return false;

            updateSerial = sqlServer.rows[0].sql_int[0] + 1;

            return true;
        }

        private bool InsertIntoUpdatedRFQs()
        {

            String sqlQueryPart1 = "insert into erp_system.dbo.updated_rfqs_assignees values (";
            String sqlQueryPart2 = ");";

            String comma = ",";
            String apostropheComma = "',";
            String commaApostrophe = ",'";
            String apostropheCommaApostrophe = "','";

            string sqlQuery = null;

            sqlQuery += sqlQueryPart1;
            sqlQuery += rfq.GetSalesPersonId();
            sqlQuery += comma;
            sqlQuery += rfq.GetRFQSerial();
            sqlQuery += comma;
            sqlQuery += rfq.GetRFQVersion();
            sqlQuery += comma;
            sqlQuery += updateSerial;
            sqlQuery += comma;
            sqlQuery += rfq.GetAssigneeId();
            sqlQuery += comma;
            sqlQuery += preSalesEmployees[PreSalesEngineersComboBox.SelectedIndex].employee_id;
            sqlQuery += comma;
            sqlQuery += "getdate()";
            sqlQuery += sqlQueryPart2;

            if (!sqlServer.InsertRows(sqlQuery))
                return false;

            return true;
        }

        private bool UpdateRFQsAssignee()
        {
            String sqlQueryPart1 = @"update erp_system.dbo.rfqs set [assigned_engineer] = ";
            String sqlQueryPart2 = " where sales_person = ";
            String sqlQueryPart3 = " And rfq_serial = ";
            String sqlQueryPart4 = " And rfq_version = ";

            string sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += preSalesEmployees[PreSalesEngineersComboBox.SelectedIndex].employee_id;
            sqlQuery += sqlQueryPart2;
            sqlQuery += rfq.GetSalesPersonId();
            sqlQuery += sqlQueryPart3;
            sqlQuery += rfq.GetRFQSerial();
            sqlQuery += sqlQueryPart4;
            sqlQuery += rfq.GetRFQVersion();

            if (!sqlServer.InsertRows(sqlQuery))
                return false;

            return true;
        }
    }
}
