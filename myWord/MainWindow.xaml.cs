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
using _01electronics_library;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using Spire.Doc.Interface;





namespace myWord
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SQLServer sqlServer;
        CommonQueries commonQueriesObject;
        CommonFunctions commonFunctionsObject;
        WorkOffer workOffer;

        List<string> standardFeatures = new List<string>();
        List<string> applications = new List<string>();
        List<string> benefits = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            sqlServer = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();
            workOffer = new WorkOffer(sqlServer);

            workOffer.InitializeSalesWorkOfferInfo(28, 1, 36);
            
            Document doc = new Document("D:/01electronics_crm/MyExcel/1.doc");

            ISection section = doc.Sections[0];

            ITable table = section.Tables[0];

            Spire.Doc.TableCell cell1 = table.Rows[0].Cells[1];
            IParagraph p1 = cell1.Paragraphs[0];
            p1.Text = workOffer.GetOfferProposerName();

            Spire.Doc.TableCell cell2 = table.Rows[1].Cells[1];
            IParagraph p2 = cell2.Paragraphs[0];
            p2.Text = workOffer.GetSalesPersonName();

            
            
            ITable table2 = section.Tables[1];

            Spire.Doc.TableCell cell3 = table2.Rows[0].Cells[1];
            IParagraph p3 = cell3.Paragraphs[0];
            p3.Text = workOffer.GetOfferIssueDate().ToString("yyyy-MM-dd");

           /* Spire.Doc.TableCell cell4 = table2.Rows[1].Cells[1];
            IParagraph p4 = cell4.Paragraphs[0];
            p4.Text = workOffer.GetOfferID();
           */

            ITable table3 = section.Tables[2];

            Spire.Doc.TableCell cell5 = table3.Rows[1].Cells[1];
            IParagraph p5 = cell5.Paragraphs[0];
            p5.Text = workOffer.GetOfferProduct1Type();

            Spire.Doc.TableCell cell6 = table3.Rows[1].Cells[2];
            IParagraph p6 = cell6.Paragraphs[0];
            p6.Text = workOffer.GetOfferProduct1Brand() + "/" + workOffer.GetOfferProduct1Model();

            Spire.Doc.TableCell cell7 = table3.Rows[1].Cells[3];
            IParagraph p7 = cell7.Paragraphs[0];
            p7.Text = workOffer.GetOfferProduct1Quantity().ToString();


            commonQueriesObject.GetModelFeatures(workOffer.GetOfferProduct1TypeId(), workOffer.GetOfferProduct1BrandId(), workOffer.GetOfferProduct1ModelId(), ref standardFeatures);

            commonQueriesObject.GetModelApplications(workOffer.GetOfferProduct1TypeId(), workOffer.GetOfferProduct1BrandId(), workOffer.GetOfferProduct1ModelId(), ref applications);

            commonQueriesObject.GetModelBenefits(workOffer.GetOfferProduct1TypeId(), workOffer.GetOfferProduct1BrandId(), workOffer.GetOfferProduct1ModelId(), ref benefits);

            ITable table4 = section.Tables[4];

            for(int i=0; i < standardFeatures.Count(); i++)
            {
                Spire.Doc.TableCell currentCell = table4.Rows[i].Cells[1];
                IParagraph currentParagraph = currentCell.Paragraphs[0];
                currentParagraph.Text = standardFeatures[i];
            }

            for(int i = 7; i < applications.Count() + 7; i++)
            {
                Spire.Doc.TableCell currentCell = table4.Rows[i].Cells[1];
                IParagraph currentParagraph = currentCell.Paragraphs[0];
                currentParagraph.Text = applications[i - 7];
            }

            for(int i = 12; i < benefits.Count() + 12; i++)
            {
                Spire.Doc.TableCell currentCell = table4.Rows[i].Cells[1];
                IParagraph currentParagraph = currentCell.Paragraphs[0];
                currentParagraph.Text = benefits[i - 12];
            }

            ITable table5 = section.Tables[7];

            Spire.Doc.TableCell currentCell1 = table5.Rows[1].Cells[1];
            IParagraph currentParagraph1 = currentCell1.Paragraphs[0];
            currentParagraph1.Text = workOffer.GetOfferProduct1Type();

            Spire.Doc.TableCell current1Cell1 = table5.Rows[1].Cells[2];
            IParagraph current1Paragraph1 = current1Cell1.Paragraphs[0];
            current1Paragraph1.Text = workOffer.GetOfferProduct1Quantity().ToString();

            Spire.Doc.TableCell current2Cell1 = table5.Rows[1].Cells[3];
            IParagraph current2Paragraph1 = current2Cell1.Paragraphs[0];
            current2Paragraph1.Text = workOffer.GetProduct1PriceValue().ToString();

            Spire.Doc.TableCell current3Cell1 = table5.Rows[1].Cells[4];
            IParagraph current3Paragraph1 = current3Cell1.Paragraphs[0];
            int total = workOffer.GetProduct1PriceValue() * workOffer.GetOfferProduct1Quantity();
            current3Paragraph1.Text = total.ToString();

            ITable table6 = section.Tables[7];

            Spire.Doc.TableCell currentCell2 = table6.Rows[2].Cells[4];
            IParagraph currentParagraph2 = currentCell2.Paragraphs[0];
            currentParagraph2.Text = total.ToString();

            ITable table7 = section.Tables[9];

            Spire.Doc.TableCell currentCell3 = table7.Rows[0].Cells[1];
            IParagraph currentParagraph3 = currentCell3.Paragraphs[0];
            string deliveryTime = workOffer.GetDeliveryTimeMinimum().ToString() + "-" + workOffer.GetDeliveryTimeMaximum().ToString();
            currentParagraph3.Text = deliveryTime;

            Spire.Doc.TableCell current1Cell3 = table7.Rows[2].Cells[1];
            IParagraph current1Paragraph3 = current1Cell3.Paragraphs[0];
            current1Paragraph3.Text = workOffer.GetDeliveryPoint();

            ITable table8 = section.Tables[10];

            Spire.Doc.TableCell currentCell4 = table8.Rows[1].Cells[0];
            IParagraph currentParagraph4 = currentCell4.Paragraphs[0];
            currentParagraph4.Text = "representative Sameh";

            ITable table9 = section.Tables[11];

            Spire.Doc.TableCell currentCell5 = table9.Rows[1].Cells[0];
            IParagraph currentParagraph5 = currentCell5.Paragraphs[0];
            currentParagraph5.Text = "Sales Sameh";

            doc.SaveToFile("D:/01electronics_crm/MyExcel/1.doc");

            System.Diagnostics.Process.Start("D:/01electronics_crm/MyExcel/1.doc");


        }
    }
}
