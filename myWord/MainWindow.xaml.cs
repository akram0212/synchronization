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
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;


namespace myWord
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Document doc = new Document();
            doc.LoadFromFile("D:/01electronics_crm/MyExcel/1_sameh.doc");
            
            Spire.Doc.Documents.Paragraph paragraph = new Spire.Doc.Documents.Paragraph(doc);
            
            
            paragraph.AppendText("ahmed sameh3");
            doc.Sections[0].Paragraphs.Insert(0, paragraph);
            doc.Replace("Evaluation", "", true, true);


            doc.SaveToFile("D:/01electronics_crm/MyExcel/1_sameh.doc");

            System.Diagnostics.Process.Start("D:/01electronics_crm/MyExcel/1_sameh.doc");


        }
    }
}
