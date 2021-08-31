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
            
            //YOU DONT NEED TO INITIALIZE RFQ IF YOU ARE GOING TO LINK IT TO ANOTHER ONE
            //rfq = new RFQ(sqlDatabase);
            rfq = mRFQ;

            InitializeComponent();

            if(viewAddCondition == COMPANY_WORK_MACROS.RFQ_ADD_CONDITION)
            {
                ConfigureUIElementsForAdd();
                InitializeContractTypeCombo();
            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
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
            }
            if (viewAddCondition != COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
            {
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

            contractTypeLabel.Visibility = Visibility.Visible;
            notesLabel.Visibility = Visibility.Visible;
        }
        private void ConfigureUIElementsForAdd()
        {
            contractTypeCombo.Visibility = Visibility.Visible;
            deadlineDateDatePicker.IsEnabled = true;
            notesTextBox.Visibility = Visibility.Visible;
            addRFQButton.Visibility = Visibility.Visible;

            contractTypeLabel.Visibility = Visibility.Collapsed;
            notesLabel.Visibility = Visibility.Collapsed;
        }
        private void ConfigureUIElementsForRevise()
        {
            contractTypeCombo.Visibility = Visibility.Visible;
            deadlineDateDatePicker.IsEnabled = true;
            notesTextBox.Visibility = Visibility.Visible;
            addRFQButton.Visibility = Visibility.Visible;

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
        private void OnSelChangedContractTypeCombo(object sender, SelectionChangedEventArgs e)
        {
            if (contractTypeCombo.SelectedItem != null)
                rfq.SetRFQContractType(contractTypes[contractTypeCombo.SelectedIndex].contractId, contractTypes[contractTypeCombo.SelectedIndex].contractName);
        }
        private void OnSelChangedDeadlineDate(object sender, SelectionChangedEventArgs e)
        {
            deadlineDate = DateTime.Parse(deadlineDateDatePicker.SelectedDate.ToString());
        }
        private void OnTextChangedNotes(object sender, TextChangedEventArgs e)
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


            //YOUR MESSAGE MUST BE SPECIFIC
            //YOU SHALL CHECK UI ELEMENTS IN ORDER AND THEN WRITE A MESSAGE IF ERROR IS TO BE FOUND
            if (rfq.GetSalesPersonId() == 0)
                System.Windows.Forms.MessageBox.Show("Please make sure you filled all the details before you add an RFQ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (rfq.GetAssigneeId() == 0)
                System.Windows.Forms.MessageBox.Show("Please make sure that you chose an assignee for the rfq!");
            else if (rfq.GetAddressSerial() == 0)
                System.Windows.Forms.MessageBox.Show("Please make sure that you chose an address for the rfq!");
            else if (rfq.GetContactId() == 0)
                System.Windows.Forms.MessageBox.Show("Please make sure that you chose a contact for the rfq!");
            else if (rfq.GetRFQContractTypeId() == 0)
                System.Windows.Forms.MessageBox.Show("Please make sure that you chose a contract type for the rfq!");
            else if (rfq.GetRFQStatusId() == 0)
                System.Windows.Forms.MessageBox.Show("Status ID can't be 0 for an RFQ! Contact your system administrator!");
            else
            {
                if(viewAddCondition == COMPANY_WORK_MACROS.RFQ_ADD_CONDITION)
                {
                    if (rfq.IssueNewRFQ())
                    {
                        //ENTER AN ERROR MESSAGE HERE
                        //THEN CLOSE THE WINDOW
                        System.Windows.Forms.MessageBox.Show("RFQ added successfully!");

                        RFQWindow rfqWindow = new RFQWindow(ref loggedInUser, ref rfq, viewAddCondition);

                        NavigationWindow currentWindow = (NavigationWindow)this.Parent;
                        currentWindow.Close();

                    }
                }
                else if(viewAddCondition == COMPANY_WORK_MACROS.RFQ_REVISE_CONDITION)
                {
                    if (rfq.ReviseRFQ())
                    {
                        //ENTER AN ERROR MESSAGE HERE
                        //THEN CLOSE THE WINDOW
                        System.Windows.Forms.MessageBox.Show("RFQ revised successfully!");

                        RFQWindow rfqWindow = new RFQWindow(ref loggedInUser, ref rfq, viewAddCondition);

                        NavigationWindow currentWindow = (NavigationWindow)this.Parent;
                        currentWindow.Close();
                    }
                }
            }
        }

    }
}
