using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using _01electronics_erp;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for RFQAdditionalInfoPage.xaml
    /// </summary>
    public partial class RFQAdditionalInfoPage : Page
    {
        Employee loggedInUser;
        RFQ rfq;
        CommonQueries commonQueriesObject;
        CommonFunctions commonFunctionsObject;
        SQLServer sqlDatabase;


        private int viewAddCondition;

        private List<BASIC_STRUCTS.CONTRACT_STRUCT> contractTypes = new List<BASIC_STRUCTS.CONTRACT_STRUCT>();

        private string notes;

        private DateTime deadlineDate;

        ///////////////ADD CONSTRUCTOR///////////
        ////////////////////////////////////////
        public RFQAdditionalInfoPage(ref Employee mLoggedInUser, ref RFQ mRFQ, int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();
            rfq = new RFQ(sqlDatabase);
            rfq = mRFQ;

            InitializeComponent();

            if(viewAddCondition == 1)
            {
                InitializeContractTypeCombo();
            }
            else
            {
                ConfigureUIElementsForView();

                SetContractTypeLabel();
                SetDeadlineDateDatePicker();
                SetNotesLabel();
            }
          
        }
        ////////////UI CONFIGURATION FUNCTIONS/////////////
        ///////////////////////////////////////////////////
        private void ConfigureUIElementsForView()
        {
            contractTypeCombo.Visibility = Visibility.Collapsed;
            deadlineDateDatePicker.IsEnabled = false;
            notesTextBox.Visibility = Visibility.Collapsed;
            addRFQButton.Visibility = Visibility.Collapsed;
        }
        //////////INITIALIZE FUNCTIONS/////////
        ///////////////////////////////////////
        private void InitializeContractTypeCombo()
        {
            if (!commonQueriesObject.GetContractTypes(ref contractTypes))
                return;

            for (int i = 0; i < contractTypes.Count(); i++)
            {
                BASIC_STRUCTS.CONTRACT_STRUCT tempContractType = contractTypes[i];
                contractTypeCombo.Items.Add(tempContractType.contractName);
            }
        }

        //////////SET FUNCTIONS//////////////
        /////////////////////////////////////
        private void SetContractTypeLabel()
        {
            contractTypeLabel.Content = rfq.GetRFQContractType();
        }

        private void SetDeadlineDateDatePicker()
        {
            //adlineDateDatePicker.SelectedDate = rfq.getrfq
        }

        private void SetNotesLabel()
        {
            notesLabel.Content = rfq.GetRFQNotes();
        }
        //////////SELECTION CHANGED//////////
        /////////////////////////////////////
        private void ContractTypeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contractTypeCombo.SelectedItem != null)
                rfq.SetRFQContractType(contractTypes[contractTypeCombo.SelectedIndex].contractId, contractTypes[contractTypeCombo.SelectedIndex].contractName);
        }
        private void DeadlineDateDatePickerSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            deadlineDate = DateTime.Parse(deadlineDateDatePicker.SelectedDate.ToString());
        }
        private void NotesTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (notesTextBox.Text != null)
                notes = notesTextBox.Text;
        }

        //////////BUTTON CLICKED/////////////
        /////////////////////////////////////
        private void OnClickBasicInfo(object sender, RoutedEventArgs e)
        {
            RFQBasicInfoPage basicInfoPage = new RFQBasicInfoPage(ref loggedInUser, ref rfq, viewAddCondition);
            NavigationService.Navigate(basicInfoPage);
        }    
        

        private void OnClickProductsInfo(object sender, RoutedEventArgs e)
        {
            RFQProductsPage productsPage = new RFQProductsPage(ref loggedInUser, ref rfq, viewAddCondition);
            NavigationService.Navigate(productsPage);
        }

        private void OnClickAdditionalInfo(object sender, RoutedEventArgs e)
        {
            //RFQAdditionalInfoPage additionalInfoPage = new RFQAdditionalInfoPage(ref loggedInUser, ref rfq);
            //NavigationService.Navigate(additionalInfoPage);
        }

        private void AddRFQButtonClick(object sender, RoutedEventArgs e)
        {
            rfq.SetRFQNotes(notes);
            rfq.SetRFQDeadlineDate(deadlineDate);
            rfq.IssueNewRFQ();
            DateTime issueDateString = DateTime.Parse(rfq.GetRFQIssueDate().ToShortDateString());
            DateTime deadlineDateString = DateTime.Parse(rfq.GetRFQDeadlineDate().ToShortDateString());

            if (rfq.GetSalesPersonId() == 0 || rfq.GetRFQSerial() == 0 || rfq.GetAssigneeId() == 0 || rfq.GetRFQID() == null || rfq.GetAddressSerial() == 0 || rfq.GetContactId() == 0 || rfq.GetRFQContractTypeId() == 0 || rfq.GetRFQStatusId() == 0)
                MessageBox.Show("Please make sure you filled all the details before you add an RFQ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                string sqlQuery;
                sqlQuery = "INSERT INTO erp_system.dbo.rfqs VALUES (" + rfq.GetRFQIssueDate().ToString("yyyy-MM-dd") + "," + rfq.GetSalesPersonId() + "," + rfq.GetRFQSerial() + "," + rfq.GetRFQVersion() + "," + rfq.GetAssigneeId() + ",'" + rfq.GetRFQID() + "'," + rfq.GetAddressSerial() + "," + rfq.GetContactId() + "," + rfq.GetRFQProduct1TypeId() + "," + rfq.GetRFQProduct1BrandId() + "," + rfq.GetRFQProduct1ModelId() + "," + rfq.GetRFQProduct1Quantity() + "," + rfq.GetRFQProduct2TypeId() + "," + rfq.GetRFQProduct2BrandId() + "," + rfq.GetRFQProduct2ModelId() + "," + rfq.GetRFQProduct2Quantity() + "," + rfq.GetRFQProduct3TypeId() + "," + rfq.GetRFQProduct3BrandId() + "," + rfq.GetRFQProduct3ModelId() + "," + rfq.GetRFQProduct3Quantity() + "," + rfq.GetRFQProduct4TypeId() + "," + rfq.GetRFQProduct4BrandId() + "," + rfq.GetRFQProduct4ModelId() + "," + rfq.GetRFQProduct4Quantity() + "," + rfq.GetRFQContractTypeId() + "," + deadlineDateString + "," + rfq.GetRFQStatusId() + ",'" + rfq.GetRFQNotes() + "');";
                int x = 0;
                sqlDatabase.InsertRows(sqlQuery);
            }
        }

       
    }
}
