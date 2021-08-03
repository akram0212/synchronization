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
using _01electronics_library;

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
                ConfigureUIElementsForAdd();
                InitializeContractTypeCombo();
            }
            else if (viewAddCondition == 0)
            {
                ConfigureUIElementsForView();

                SetContractTypeLabel();
                SetDeadlineDateDatePicker();
                SetNotesLabel();
            }
            else
            {
                ConfigureUIElementsForRevise();
                InitializeContractTypeCombo();
                SetContractTypeCombo();
                SetDeadlineDateDatePicker();
                SetNotesTextBox();
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
            reviseRFQButton.Visibility = Visibility.Collapsed;

            contractTypeLabel.Visibility = Visibility.Visible;
            notesLabel.Visibility = Visibility.Visible;
        }
        private void ConfigureUIElementsForAdd()
        {
            contractTypeCombo.Visibility = Visibility.Visible;
            deadlineDateDatePicker.IsEnabled = true;
            notesTextBox.Visibility = Visibility.Visible;
            addRFQButton.Visibility = Visibility.Visible;
            reviseRFQButton.Visibility = Visibility.Collapsed;

            contractTypeLabel.Visibility = Visibility.Collapsed;
            notesLabel.Visibility = Visibility.Collapsed;
        }
        private void ConfigureUIElementsForRevise()
        {
            contractTypeCombo.Visibility = Visibility.Visible;
            deadlineDateDatePicker.IsEnabled = true;
            notesTextBox.Visibility = Visibility.Visible;
            addRFQButton.Visibility = Visibility.Collapsed;
            reviseRFQButton.Visibility = Visibility.Visible;

            contractTypeLabel.Visibility = Visibility.Collapsed;
            notesLabel.Visibility = Visibility.Collapsed;
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
        
        private void SetContractTypeCombo()
        {
            contractTypeCombo.Text = rfq.GetRFQContractType();
        }
        private void SetContractTypeLabel()
        {
            contractTypeLabel.Content = rfq.GetRFQContractType();
        }

        private void SetDeadlineDateDatePicker()
        {
            deadlineDateDatePicker.SelectedDate = rfq.GetRFQDeadlineDate();
        }

        private void SetNotesTextBox()
        {
            notesTextBox.Text = rfq.GetRFQNotes();
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

            if (rfq.GetSalesPersonId() == 0 || rfq.GetRFQSerial() == 0 || rfq.GetAssigneeId() == 0 || rfq.GetRFQID() == null || rfq.GetAddressSerial() == 0 || rfq.GetContactId() == 0 || rfq.GetRFQContractTypeId() == 0 || rfq.GetRFQStatusId() == 0)
                System.Windows.Forms.MessageBox.Show("Please make sure you filled all the details before you add an RFQ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                string sqlQuery;
                sqlQuery = "INSERT INTO erp_system.dbo.rfqs VALUES (";
                sqlQuery += "'" + rfq.GetRFQIssueDate().ToString("yyyy-MM-dd") + "',";
                sqlQuery += rfq.GetSalesPersonId() + ",";
                sqlQuery += rfq.GetRFQSerial() + ",";
                sqlQuery += rfq.GetRFQVersion() + ",";
                sqlQuery += rfq.GetAssigneeId() + ",";
                sqlQuery += "'" + rfq.GetRFQID() + "',";
                sqlQuery += rfq.GetAddressSerial() + ",";
                sqlQuery += rfq.GetContactId() + ",";
                sqlQuery += rfq.GetRFQProduct1TypeId() + ",";
                sqlQuery += rfq.GetRFQProduct1BrandId() + ",";
                sqlQuery += rfq.GetRFQProduct1ModelId() + ",";
                sqlQuery += rfq.GetRFQProduct1Quantity() + ",";
                sqlQuery += rfq.GetRFQProduct2TypeId() + ",";
                sqlQuery += rfq.GetRFQProduct2BrandId() + ",";
                sqlQuery += rfq.GetRFQProduct2ModelId() + ",";
                sqlQuery += rfq.GetRFQProduct2Quantity() + ",";
                sqlQuery += rfq.GetRFQProduct3TypeId() + ",";
                sqlQuery += rfq.GetRFQProduct3BrandId() + ",";
                sqlQuery += rfq.GetRFQProduct3ModelId() + ",";
                sqlQuery += rfq.GetRFQProduct3Quantity() + ",";
                sqlQuery += rfq.GetRFQProduct4TypeId() + ",";
                sqlQuery += rfq.GetRFQProduct4BrandId() + ",";
                sqlQuery += rfq.GetRFQProduct4ModelId() + ",";
                sqlQuery += rfq.GetRFQProduct4Quantity() + ",";
                sqlQuery += rfq.GetRFQContractTypeId() + ",";
                sqlQuery += "'" + rfq.GetRFQDeadlineDate().ToString("yyyy-MM-dd") + "',";
                sqlQuery += rfq.GetRFQStatusId() + ",";
                sqlQuery += "'" + rfq.GetRFQNotes() + "');";

                if (sqlDatabase.InsertRows(sqlQuery))
                    System.Windows.Forms.MessageBox.Show("Your RFQ has been Added succefully");
                else
                    System.Windows.Forms.MessageBox.Show("An error has occurred please contact your system administrator");
            }
        }

        private void ReviseRFQButtonClick(object sender, RoutedEventArgs e)
        {
            rfq.SetRFQNotes(notes);
            rfq.SetRFQDeadlineDate(deadlineDate);
            rfq.ReviseRFQ();

            string sqlQuery;
            sqlQuery = "INSERT INTO erp_system.dbo.rfqs VALUES (";
            sqlQuery += "'" + rfq.GetRFQIssueDate().ToString("yyyy-MM-dd") + "',";
            sqlQuery += rfq.GetSalesPersonId() + ",";
            sqlQuery += rfq.GetRFQSerial() + ",";
            sqlQuery += rfq.GetRFQVersion() + ",";
            sqlQuery += rfq.GetAssigneeId() + ",";
            sqlQuery += "'" + rfq.GetRFQID() + "',";
            sqlQuery += rfq.GetAddressSerial() + ",";
            sqlQuery += rfq.GetContactId() + ",";
            sqlQuery += rfq.GetRFQProduct1TypeId() + ",";
            sqlQuery += rfq.GetRFQProduct1BrandId() + ",";
            sqlQuery += rfq.GetRFQProduct1ModelId() + ",";
            sqlQuery += rfq.GetRFQProduct1Quantity() + ",";
            sqlQuery += rfq.GetRFQProduct2TypeId() + ",";
            sqlQuery += rfq.GetRFQProduct2BrandId() + ",";
            sqlQuery += rfq.GetRFQProduct2ModelId() + ",";
            sqlQuery += rfq.GetRFQProduct2Quantity() + ",";
            sqlQuery += rfq.GetRFQProduct3TypeId() + ",";
            sqlQuery += rfq.GetRFQProduct3BrandId() + ",";
            sqlQuery += rfq.GetRFQProduct3ModelId() + ",";
            sqlQuery += rfq.GetRFQProduct3Quantity() + ",";
            sqlQuery += rfq.GetRFQProduct4TypeId() + ",";
            sqlQuery += rfq.GetRFQProduct4BrandId() + ",";
            sqlQuery += rfq.GetRFQProduct4ModelId() + ",";
            sqlQuery += rfq.GetRFQProduct4Quantity() + ",";
            sqlQuery += rfq.GetRFQContractTypeId() + ",";
            sqlQuery += "'" + rfq.GetRFQDeadlineDate().ToString("yyyy-MM-dd") + "',";
            sqlQuery += rfq.GetRFQStatusId() + ",";
            sqlQuery += "'" + rfq.GetRFQNotes() + "');";

            if (sqlDatabase.InsertRows(sqlQuery))
                System.Windows.Forms.MessageBox.Show("Your RFQ has been revised succefully");
            else
                System.Windows.Forms.MessageBox.Show("An error has occurred please contact your system administrator");
        }
    }
}
