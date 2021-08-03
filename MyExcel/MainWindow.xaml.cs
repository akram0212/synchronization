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
using System.Windows.Navigation;
using System.Windows.Shapes;
using _01electronics_erp;
using Spire.Doc;
using Spire.Doc.Documents;



namespace MyExcel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
   

    public partial class MainWindow : Window
    {
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqs = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();

        Microsoft.Office.Interop.Excel.Application excel;
        Microsoft.Office.Interop.Excel.Workbook workwbBook;
        Microsoft.Office.Interop.Excel.Worksheet ws;
        Microsoft.Office.Interop.Excel.Range rng;

        CommonFunctions commonFunctionsObject;
        CommonQueries commonQueriesObject;

        SQLServer sqlServer;
        RFQ rfqObject;

        public MainWindow()
        {
            InitializeComponent();

            commonFunctionsObject = new CommonFunctions();
            commonQueriesObject = new CommonQueries();
            sqlServer = new SQLServer();
            rfqObject = new RFQ(sqlServer);

            commonQueriesObject.GetRFQs(ref rfqs);

            Document doc = new Document();
            doc.LoadFromFile("D:/01electronics_crm/MyExcel/1_sameh.doc");

            Spire.Doc.Documents.Paragraph paraInserted = new Spire.Doc.Documents.Paragraph(doc);
            paraInserted.AppendText("Hello, this is a new paragraph.");

            doc.Sections[1].Paragraphs.Insert(1, paraInserted);
            doc.SaveToFile("D:/01electronics_crm/MyExcel/1_sameh.doc");

            System.Diagnostics.Process.Start("D:/01electronics_crm/MyExcel/1_sameh.doc");


            List<rfq> rfqsListToExport = new List<rfq>();
            for(int i = 0; i< rfqs.Count; i++)
            {
                rfqsListToExport.Add(new rfq() { assigneeID = rfqs[i].assignee_id, assigneeName = rfqs[i].assignee_name, branchSerial = rfqs[i].branch_serial, companyName = rfqs[i].company_name, companySerial = rfqs[i].company_serial, contactID = rfqs[i].contact_id, contactName = rfqs[i].contact_name, contractType = rfqs[i].contract_type, contractTypeID = rfqs[i].contract_type_id, deadlineDate = rfqs[i].deadline_date, failureReason = rfqs[i].failure_reason, failureReasonID = rfqs[i].failure_reason_id, issueDate = rfqs[i].issue_date });
            }

            dataGrid.ItemsSource = rfqsListToExport;
            //, products = rfqs[i].products, rfqID = rfqs[i].rfq_id, rfqSerial = rfqs[i].rfq_serial, rfqStatus = rfqs[i].rfq_status, rfqStatusID = rfqs[i].rfq_status_id, rfqVersion = rfqs[i].rfq_version, salesPersonId = rfqs[i].sales_person_id, salesPersonName = rfqs[i].sales_person_name
        }

        public class rfq
        {
            public int assigneeID { get; set; }

            public string assigneeName { get; set; }

            public int branchSerial { get; set; }

            public string companyName { get; set; }

            public int companySerial { get; set; }

            public int contactID { get; set; }

            public string contactName { get; set; }

            public string contractType { get; set; }

            public int contractTypeID { get; set; }
            public string deadlineDate { get; set; }

            public string failureReason { get; set; }

            public int failureReasonID { get; set; }

            public string issueDate { get; set; }

            /*public List<COMPANY_WORK_MACROS.RFQ_PRODUCT_STRUCT> products { get; set; }

            public string rfqID { get; set; }

            public int rfqSerial { get; set; }

            public string rfqStatus { get; set; }

            public int rfqStatusID { get; set; }

            public int rfqVersion { get; set; }

            public int salesPersonId { get; set; }

            public string salesPersonName { get; set; }
            */
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application excel = null;
            Microsoft.Office.Interop.Excel.Workbook wb = null;
            object missing = Type.Missing;
            Microsoft.Office.Interop.Excel.Worksheet ws = null;
            Microsoft.Office.Interop.Excel.Range rng = null;

            excel = new Microsoft.Office.Interop.Excel.Application();
            wb = excel.Workbooks.Add();
            ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.ActiveSheet;
            ws.Columns.AutoFit();
            ws.Columns.EntireColumn.ColumnWidth = 25;

            // Header row
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                ws.Range["A1"].Offset[0, i].Value = dataGrid.Columns[i].Header;
            }

            // Data Rows
            for (int i = 0; i < rfqs.Count; i++)
            {
                ws.Range["A2"].Offset[i, 0].Value = rfqs[i].assignee_id ;
                ws.Range["A2"].Offset[i, 1].Value = rfqs[i].assignee_name ;
                ws.Range["A2"].Offset[i, 2].Value = rfqs[i].branch_serial;
                ws.Range["A2"].Offset[i, 3].Value = rfqs[i].company_name;
                ws.Range["A2"].Offset[i, 4].Value = rfqs[i].company_serial;
                ws.Range["A2"].Offset[i, 5].Value = rfqs[i].contact_id;
                ws.Range["A2"].Offset[i, 6].Value = rfqs[i].contact_name;
                ws.Range["A2"].Offset[i, 7].Value = rfqs[i].contract_type;
                ws.Range["A2"].Offset[i, 8].Value = rfqs[i].contract_type_id;
                ws.Range["A2"].Offset[i, 9].Value = rfqs[i].deadline_date;
                ws.Range["A2"].Offset[i, 10].Value = "Null";
                ws.Range["A2"].Offset[i, 11].Value = rfqs[i].failure_reason_id;
                ws.Range["A2"].Offset[i, 12].Value = rfqs[i].issue_date;
            }

            excel.Visible = true;
            wb.Activate();
        }
    }
}
