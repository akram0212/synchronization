using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Excel;

using _01electronics_erp;
using Google.Protobuf.WellKnownTypes;
using System.Data;
using System.Runtime.InteropServices;

namespace ExportingToExcelTrial2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private SQLServer sqlDatabase;
        private CommonQueries commonQueriesObject;
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqs = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> trialList = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();

        public Microsoft.Office.Interop.Excel.Application APP;
        public Microsoft.Office.Interop.Excel.Workbook WB;
        public Microsoft.Office.Interop.Excel.Worksheet WS;

        public MainWindow()
        {
            InitializeComponent();
            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();

            this.APP = new Microsoft.Office.Interop.Excel.Application();
            this.Open("C:\\MyExcel.xlsx", WS.Index);
            this.CreateHeader();
            this.InsertData();
            this.Close();
            APP = new Microsoft.Office.Interop.Excel.Application();
            WB = APP.Workbooks.Add(1);
            this.WS = (Microsoft.Office.Interop.Excel.Worksheet)WB.Sheets[1];


            Trial();
        }
        public bool Query()
        {

            if (!commonQueriesObject.GetRFQs(ref rfqs))
                return false;
           
            return true; 
        }

       

        private Worksheet Open(string Location, int workSheet)
        {
            this.WB = this.APP.Workbooks.Open(Location);
            this.WS = (Microsoft.Office.Interop.Excel.Worksheet)WB.Sheets[workSheet];
            return this.WS;
        }
        private void CreateHeader()
        {
            int ind = 1;
            foreach (object ob in this.data.Columns.Select(cs => cs.Header).ToList())
            {
                this.WS.Cells[1, ind] = ob.ToString();
                ind++;
            }
        }
        private void InsertData()
        {
            int ind = 2;
            foreach (Field field in data.ItemsSource)
            {
                DataRow DR = DR.Row;
                for (int ind1 = 1; ind1 <= data.Columns.Count; ind1++)
                {
                    WS.Cells[ind][ind1] = DR[ind1 - 1];
                }
                ind++;
            }
        }
        private void Close()
        {
            if (this.APP.ActiveWorkbook != null)
                this.APP.ActiveWorkbook.Save();
            if (this.APP != null)
            {
                if (this.WB != null)
                {
                    if (this.WS != null)
                        Marshal.ReleaseComObject(this.WS);
                    this.WB.Close(false, System.Type.Missing, System.Type.Missing);
                    Marshal.ReleaseComObject(this.WB);
                }
                this.APP.Quit();
                Marshal.ReleaseComObject(this.APP);
            }
        }
        public void Trial()
        {
            Query();

            data.ItemsSource = rfqs;

            MainWindow main = new MainWindow();
            //main.Show();
        }
    }
}
