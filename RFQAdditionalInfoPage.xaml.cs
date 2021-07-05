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

        private int viewAddCondition;

        private List<BASIC_STRUCTS.CONTRACT_STRUCT> contractTypes = new List<BASIC_STRUCTS.CONTRACT_STRUCT>();
        private string notes;
        private DateTime deadlineDate;

        ///////////////ADD CONSTRUCTOR///////////
        ////////////////////////////////////////
        public RFQAdditionalInfoPage(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;
            commonQueriesObject = new CommonQueries();

            InitializeComponent();
            InitializeContractTypeCombo();

            viewAddCondition = 1;
        }

        //////////////VIEW CONSTRUCTOR//////////
        ////////////////////////////////////////
        public RFQAdditionalInfoPage(ref Employee mLoggedInUser, ref RFQ mRFQ)
        {
            loggedInUser = mLoggedInUser;
            rfq = mRFQ;

            InitializeComponent();
            ConfigureUIElements();

            viewAddCondition = 0;
        }
        ////////////UI CONFIGURATION FUNCTIONS/////////////
        ///////////////////////////////////////////////////
        private void ConfigureUIElements()
        {
            contractTypeCombo.Visibility = Visibility.Collapsed;
            notesTextBox.Visibility = Visibility.Collapsed;
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
             if(viewAddCondition == 0)
             {
                 RFQBasicInfoPage basicInfoPage = new RFQBasicInfoPage(ref loggedInUser, ref rfq);
                 NavigationService.Navigate(basicInfoPage);
             }
             else
             {
                 RFQBasicInfoPage basicInfoPage = new RFQBasicInfoPage(ref loggedInUser);
                 NavigationService.Navigate(basicInfoPage);
             }
        }

        private void OnClickProductsInfo(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition == 0)
            {
                RFQProductsPage productsPage = new RFQProductsPage(ref loggedInUser, ref rfq);
                NavigationService.Navigate(productsPage);
            }
            else
            {
                RFQProductsPage productsPage = new RFQProductsPage(ref loggedInUser);
                NavigationService.Navigate(productsPage);
            }
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
        }

       
    }
}
