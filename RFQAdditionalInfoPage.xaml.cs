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

        public RFQBasicInfoPage rfqBasicInfoPage;
        public RFQProductsPage rfqProductsPage;
        public RFQUploadFilesPage rfqUploadFilesPage;

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

                cancelButton.IsEnabled = false;
                finishButton.IsEnabled = false;
                nextButton.IsEnabled = true;
            }
            else
            {
                ConfigureUIElementsForRevise();
                InitializeContractTypeCombo();

                SetContractTypeCombo();
                SetDeadlineDateDatePicker();
                SetNotesTextBox();
            }

            if (viewAddCondition != COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
                nextButton.IsEnabled = false; 

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////UI CONFIGURATION FUNCTIONS/////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ConfigureUIElementsForView()
        {
            contractTypeCombo.Visibility = Visibility.Collapsed;
            deadlineDateDatePicker.IsEnabled = false;
            notesTextBox.Visibility = Visibility.Collapsed;
            //addRFQButton.Visibility = Visibility.Collapsed;

            contractTypeLabel.Visibility = Visibility.Visible;
            notesLabel.Visibility = Visibility.Visible;
        }
        private void ConfigureUIElementsForAdd()
        {
            contractTypeCombo.Visibility = Visibility.Visible;
            deadlineDateDatePicker.IsEnabled = true;
            notesTextBox.Visibility = Visibility.Visible;
            //addRFQButton.Visibility = Visibility.Visible;

            contractTypeLabel.Visibility = Visibility.Collapsed;
            notesLabel.Visibility = Visibility.Collapsed;
        }
        private void ConfigureUIElementsForRevise()
        {
            contractTypeCombo.Visibility = Visibility.Visible;
            deadlineDateDatePicker.IsEnabled = true;
            notesTextBox.Visibility = Visibility.Visible;
            //addRFQButton.Visibility = Visibility.Visible;

            contractTypeLabel.Visibility = Visibility.Collapsed;
            notesLabel.Visibility = Visibility.Collapsed;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        //////////INITIALIZE FUNCTIONS/////////
        ///////////////////////////////////////////////////////////////////////////////////////////////
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

        ///////////////////////////////////////////////////////////////////////////////////////////////
        //////////SET FUNCTIONS//////////////
        /////////////////////////////////////////////////////////////////////////////////////////////

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

        ///////////////////////////////////////////////////////////////////////////////////////////////
        //////////SELECTION CHANGED//////////
        ///////////////////////////////////////////////////////////////////////////////////////////////
        private void OnSelChangedContractTypeCombo(object sender, SelectionChangedEventArgs e)
        {
            if (contractTypeCombo.SelectedItem != null)
                rfq.SetRFQContractType(contractTypes[contractTypeCombo.SelectedIndex].contractId, contractTypes[contractTypeCombo.SelectedIndex].contractName);
        }
        private void OnSelChangedDeadlineDate(object sender, SelectionChangedEventArgs e)
        {
            rfq.SetRFQDeadlineDate(DateTime.Parse(deadlineDateDatePicker.SelectedDate.ToString()));

        }
        private void OnTextChangedNotes(object sender, TextChangedEventArgs e)
        {
            rfq.SetRFQNotes(notesTextBox.Text);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        //////////BUTTON CLICKED/////////////
        /////////////////////////////////////////////////////////////////////////////////////////////

        private void OnBtnClickFinish(object sender, RoutedEventArgs e)
        {
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
                    if (!rfq.IssueNewRFQ())
                        return;
                }
                else if(viewAddCondition == COMPANY_WORK_MACROS.RFQ_REVISE_CONDITION)
                {
                    if (!rfq.ReviseRFQ())
                        return;
                }

                NavigationWindow currentWindow = (NavigationWindow)this.Parent;
                currentWindow.Close();
            }
        }

        private void OnBtnClickNext(object sender, RoutedEventArgs e)
        {
            rfqUploadFilesPage.rfqBasicInfoPage = rfqBasicInfoPage;
            rfqUploadFilesPage.rfqProductsPage = rfqProductsPage;
            rfqUploadFilesPage.rfqAdditionalInfoPage = this;

            NavigationService.Navigate(rfqUploadFilesPage);
        }

        private void OnBtnClickBack(object sender, RoutedEventArgs e)
        {
            rfqProductsPage.rfqAdditionalInfoPage = this;
            rfqProductsPage.rfqBasicInfoPage = rfqBasicInfoPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
                rfqProductsPage.rfqUploadFilesPage = rfqUploadFilesPage;

            NavigationService.Navigate(rfqProductsPage);
        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;
            currentWindow.Close();
        }

        private void OnBtnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            rfqBasicInfoPage.rfqProductsPage = rfqProductsPage;
            rfqBasicInfoPage.rfqAdditionalInfoPage = this;

            if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
                rfqBasicInfoPage.rfqUploadFilesPage = rfqUploadFilesPage;

            NavigationService.Navigate(rfqBasicInfoPage);
        }

        private void OnBtnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            rfqProductsPage.rfqAdditionalInfoPage = this;
            rfqProductsPage.rfqBasicInfoPage = rfqBasicInfoPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
                rfqProductsPage.rfqUploadFilesPage = rfqUploadFilesPage;

            NavigationService.Navigate(rfqProductsPage);
        }

        private void OnBtnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.RFQ_VIEW_CONDITION)
            {
                rfqUploadFilesPage.rfqBasicInfoPage = rfqBasicInfoPage;
                rfqUploadFilesPage.rfqProductsPage = rfqProductsPage;
                rfqUploadFilesPage.rfqAdditionalInfoPage = this;

                NavigationService.Navigate(rfqUploadFilesPage);
            }
        }
    }
}
