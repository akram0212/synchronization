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
using _01electronics_erp;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for AddWorkOfferWindow.xaml
    /// </summary>
    public partial class AddWorkOfferWindow : Window
    {

        private Employee loggedInUser;
        private CommonQueries commonQueriesObject = new CommonQueries();
        private CommonFunctions commonFunctionsObject = new CommonFunctions();
        private SQLServer commonQueriesSqlObject = new SQLServer();

        public struct Company_Struct
        {
            public int companySerial;
            public string companyName;
        };

        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> salesEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsList = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();
        private List<Company_Struct> companyInfo = new List<Company_Struct>();
        private List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> branchInfo = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT> contactInfo = new List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT>();

        private int salesPersonID;

        public AddWorkOfferWindow(ref Employee mloggedInUser)
        {
            InitializeComponent();
            loggedInUser = mloggedInUser;

            FillSalesPersonCombo();
           
            

        }

        private void FillSalesPersonCombo()
        {
            if (!commonQueriesObject.GetTeamEmployees(COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID, ref salesEmployees))
                return;

            for (int i = 0; i < salesEmployees.Count(); i++)
            {
                string temp = salesEmployees[i].employee_name;
                salesPersonCombo.Items.Add(temp);
            }
        }

        private void FillCompanyNameCombo()
        {
            // if (!GetCompaniesQuery(loggedInUser.GetEmployeeId(), ref companyInfo))
            // return;

            if (!GetCompaniesQuery(salesPersonID, ref companyInfo))
              return;

            for (int i = 0; i < companyInfo.Count; i++)
            {
                string tempName = companyInfo[i].companyName;
                companyNameCombo.Items.Add(tempName);
            }
        }

        private void SalesPersonComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            rfqSerialCombo.Items.Clear();

            int tempSalesPersonIndex = salesPersonCombo.SelectedIndex;
            salesPersonID = salesEmployees[tempSalesPersonIndex].employee_id;

            if (!commonQueriesObject.GetRFQs(ref rfqsList))
                return;
            
            for (int i = rfqsList.Count - 1; i >= 0; i--)
            {
                if (rfqsList[i].sales_person_id == salesPersonID)
                    rfqSerialCombo.Items.Add(rfqsList[i].rfq_serial.ToString());    
            }

            FillCompanyNameCombo();
        }

        private bool GetCompaniesQuery(int mEmployeeSerial, ref List<Company_Struct> returnVector)
        {
            returnVector.Clear();

            string sqlQuery = "SELECT company_serial,company_name FROM erp_system.dbo.company_name WHERE added_by = " + mEmployeeSerial;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!commonQueriesSqlObject.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < commonQueriesSqlObject.rows.Count; i++)
            {
                Company_Struct temp = new Company_Struct();

                temp.companySerial = commonQueriesSqlObject.rows[i].sql_int[0];
                temp.companyName = commonQueriesSqlObject.rows[i].sql_string[0];

                returnVector.Add(temp);
            }
            return true;
        }

        private void CompanyNameComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
