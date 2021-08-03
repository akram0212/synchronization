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


namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for WorkOfferAdditionalInfoPage.xaml
    /// </summary>
    public partial class WorkOfferAdditionalInfoPage : Page
    {
        Employee loggedInUser;
        WorkOffer workOffer;

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private IntegrityChecks IntegrityChecks;

        private List<BASIC_STRUCTS.CONTRACT_STRUCT> contractTypes = new List<BASIC_STRUCTS.CONTRACT_STRUCT>();
        private List<BASIC_STRUCTS.TIMEUNIT_STRUCT> timeUnits = new List<BASIC_STRUCTS.TIMEUNIT_STRUCT>();

        private int viewAddCondition;
        private int warrantyPeriod = 0;
        private int offerValidityPeriod = 0;
        private int drawingDeadlineFrom = 0;
        private int drawingDeadlineTo = 0;
        private int isDrawing = 0;

        private string additionalDescription;
        public WorkOfferAdditionalInfoPage(ref Employee mLoggedInUser, ref WorkOffer mWorkOffer, int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;
            InitializeComponent();

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();
            IntegrityChecks = new IntegrityChecks();

            workOffer = new WorkOffer(sqlDatabase);
            workOffer = mWorkOffer;
            /////////////////////////
            ///ADD
            /////////////////////////
            if(viewAddCondition == 1)
            {
                ConfigureDrawingSubmissionUIElements();
                InitializeContractType();
                InitializeTimeUnitComboBoxes();
                HideReviseOfferButton();
            }
            //////////////////////////
            ///VIEW
            //////////////////////////
            else if(viewAddCondition == 0)
            {
                InitializeContractType();
                InitializeTimeUnitComboBoxes();

                if (workOffer.GetDrawingSubmissionDeadlineMinimum() != 0)
                    drawingConditionsCheckBox.IsChecked = true;
                
                ConfigureUIElementsView();
                SetDrawingSubmissionValues();
                SetContractTypeValue();
                SetWarrantyPeriodValues();
                SetValidityPeriodValues();
                SetAdditionalDescriptionValue();

                HideAddOfferButton();
                HideReviseOfferButton();
            }
            //////////////////////////////
            ///REVISE
            //////////////////////////////
            else if(viewAddCondition == 2)
            {
                ConfigureDrawingSubmissionUIElements();
                InitializeContractType();
                InitializeTimeUnitComboBoxes();
                SetDrawingSubmissionValues();
                SetContractTypeValue();
                SetWarrantyPeriodValues();
                SetValidityPeriodValues();
                SetAdditionalDescriptionValue();
                if (workOffer.GetDrawingSubmissionDeadlineMinimum() != 0)
                    drawingConditionsCheckBox.IsChecked = true;
                HideAddOfferButton();
            }
            ////////////////////////
            ///RESOLVE RFQ
            ///////////////////////
            else
            {
                ConfigureDrawingSubmissionUIElements();
                InitializeContractType();
                InitializeTimeUnitComboBoxes();
                HideReviseOfferButton();
            }
        }
        /////////////////////////////////
        ///CONFIGURE UI ELEMENTS FUNCTIONS
        /////////////////////////////////
        
        private void HideReviseOfferButton()
        {
            reviseOfferButton.Visibility = Visibility.Collapsed;
        }
        private void HideAddOfferButton()
        {
            addOfferButton.Visibility = Visibility.Collapsed;
        }
        private void ConfigureDrawingSubmissionUIElements()
        {
            drawingDeadlineFromTextBox.IsEnabled = false;
            drawingDeadlineToTextBox.IsEnabled = false;
            drawingDeadlineDateComboBox.IsEnabled = false;
        }

        private void ConfigureUIElementsView()
        {
            drawingConditionsCheckBox.IsEnabled = false;
            drawingDeadlineFromTextBox.IsEnabled = false;
            drawingDeadlineToTextBox.IsEnabled = false;
            drawingDeadlineDateComboBox.IsEnabled = false;
            contractTypeComboBox.IsEnabled = false;
            warrantyPeriodTextBox.IsEnabled = false;
            warrantyPeriodCombo.IsEnabled = false;
            offerValidityCombo.IsEnabled = false;
            offerValidityTextBox.IsEnabled = false;
            additionalDescriptionTextBox.IsEnabled = false;
        }
        /////////////////////////////////
        ///INITIALIZATION FUNCTIONS
        /////////////////////////////////
        private bool InitializeContractType()
        {
            if (!commonQueriesObject.GetContractTypes(ref contractTypes))
                return false;
            for(int i = 0; i < contractTypes.Count; i++)
                contractTypeComboBox.Items.Add(contractTypes[i].contractName);

            return true;
        }

        private bool InitializeTimeUnitComboBoxes()
        {
            if (!commonQueriesObject.GetTimeUnits(ref timeUnits))
                return false;
            for(int i = 0; i < timeUnits.Count(); i++)
            {
                warrantyPeriodCombo.Items.Add(timeUnits[i].timeUnit);
                offerValidityCombo.Items.Add(timeUnits[i].timeUnit);
                drawingDeadlineDateComboBox.Items.Add(timeUnits[i].timeUnit);
            }
            return true;
        }
        //////////////////////////////
        ///SET FUNCTIONS
        //////////////////////////////
        private void SetDrawingSubmissionValues()
        {
            drawingDeadlineFromTextBox.Text = workOffer.GetDrawingSubmissionDeadlineMinimum().ToString();
            drawingDeadlineToTextBox.Text = workOffer.GetDrawingSubmissionDeadlineMaximum().ToString();
            drawingDeadlineDateComboBox.Text = workOffer.GetDrawingDeadlineTimeUnit();
        }

        private void SetContractTypeValue()
        {
            contractTypeComboBox.Text = workOffer.GetOfferContractType();
        }

        private void SetWarrantyPeriodValues()
        {
            warrantyPeriodTextBox.Text = workOffer.GetWarrantyPeriod().ToString();
            warrantyPeriodCombo.Text = workOffer.GetWarrantyPeriodTimeUnit();
        }

        private void SetValidityPeriodValues()
        {
            offerValidityTextBox.Text = workOffer.GetOfferValidityPeriod().ToString();
            offerValidityCombo.Text = workOffer.GetOfferValidityTimeUnit();
        }

        private void SetAdditionalDescriptionValue()
        {
            additionalDescriptionTextBox.Text = workOffer.GetOfferNotes();
        }
        //////////////////////////////
        ///GET FUNCTIONS
        //////////////////////////////
        //////////////////////////////
        ///SELECTION CHANGED HANDLERS
        //////////////////////////////

        private void WarrantyPeriodTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(warrantyPeriodTextBox.Text, BASIC_MACROS.PHONE_STRING) && warrantyPeriodTextBox.Text != "")
                warrantyPeriod = int.Parse(warrantyPeriodTextBox.Text);
            else
            {
                warrantyPeriod = 0;
                warrantyPeriodTextBox.Text = null;
            }
        }

        private void WarrantyPeriodComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            workOffer.SetWarrantyPeriodTimeUnit(timeUnits[warrantyPeriodCombo.SelectedIndex].timeUnitId, timeUnits[warrantyPeriodCombo.SelectedIndex].timeUnit);
        }

        private void ContractTypeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            workOffer.SetOfferContractType(contractTypes[contractTypeComboBox.SelectedIndex].contractId, contractTypes[contractTypeComboBox.SelectedIndex].contractName);
        }

        private void OfferValidityComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            workOffer.SetOfferValidityTimeUnit(timeUnits[offerValidityCombo.SelectedIndex].timeUnitId, timeUnits[offerValidityCombo.SelectedIndex].timeUnit);
        }

        private void OfferValidityTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(offerValidityTextBox.Text, BASIC_MACROS.PHONE_STRING) && offerValidityTextBox.Text != "")
                offerValidityPeriod = int.Parse(offerValidityTextBox.Text);
            else
            {
                offerValidityPeriod = 0;
                warrantyPeriodTextBox.Text = null;
            }
        }

        private void AdditionalDescriptionTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            additionalDescription = additionalDescriptionTextBox.Text;
        }

        private void DrawingDeadlineFromTextBoxTextChanged(object sender, TextChangedEventArgs e)
           {
            if (IntegrityChecks.CheckInvalidCharacters(drawingDeadlineFromTextBox.Text, BASIC_MACROS.PHONE_STRING) && drawingDeadlineFromTextBox.Text != "")
                drawingDeadlineFrom = int.Parse(drawingDeadlineFromTextBox.Text);
            else
            {
                drawingDeadlineFrom = 0;
                drawingDeadlineFromTextBox.Text = null;
            }
        }

        private void DrawingDeadlineToTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(drawingDeadlineToTextBox.Text, BASIC_MACROS.PHONE_STRING) && drawingDeadlineToTextBox.Text != "")
                drawingDeadlineTo = int.Parse(drawingDeadlineToTextBox.Text);
            else
            {
                drawingDeadlineTo = 0;
                drawingDeadlineToTextBox.Text = null;
            }
        }
        private void DrawingDeadlineDateComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            workOffer.SetDrawingSubmissionDeadlineTimeUnit(timeUnits[drawingDeadlineDateComboBox.SelectedIndex].timeUnitId, timeUnits[drawingDeadlineDateComboBox.SelectedIndex].timeUnit);
        }

        //////////////////////////////
        ///CHECK BOXES EVENT HANDLERS
        //////////////////////////////

        private void DrawingConditionsCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            drawingDeadlineFromTextBox.IsEnabled = true;
            drawingDeadlineToTextBox.IsEnabled = true;
            drawingDeadlineDateComboBox.IsEnabled = true;
            isDrawing = 1;
        }

        private void DrawingConditionsCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            drawingDeadlineFromTextBox.IsEnabled = false;
            drawingDeadlineToTextBox.IsEnabled = false;
            drawingDeadlineDateComboBox.IsEnabled = false;
            isDrawing = 0;
        }

        ///BUTTON CLICKED HANDLERS
        /////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            WorkOfferBasicInfoPage basicInfoPage = new WorkOfferBasicInfoPage(ref loggedInUser, ref workOffer, viewAddCondition);
            NavigationService.Navigate(basicInfoPage);
        }

        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            WorkOfferProductsPage offerProductsPage = new WorkOfferProductsPage(ref loggedInUser, ref workOffer, viewAddCondition);
            NavigationService.Navigate(offerProductsPage);
        }

        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {
            WorkOfferPaymentAndDeliveryPage offerPaymentAndDeliveryPage = new WorkOfferPaymentAndDeliveryPage(ref loggedInUser, ref workOffer, viewAddCondition);
            NavigationService.Navigate(offerPaymentAndDeliveryPage);
        }

        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {

        }

        private void AddOfferButtonClick(object sender, RoutedEventArgs e)
        {
            workOffer.SetDrawingSubmissionDeadlineMinimum(drawingDeadlineFrom);
            workOffer.SetDrawingSubmissionDeadlineMaximum(drawingDeadlineTo);
            workOffer.SetWarrantyPeriod(warrantyPeriod);
            workOffer.SetOfferValidityPeriod(offerValidityPeriod);
            workOffer.SetOfferNotes(additionalDescription);
            //workOffer.SetOfferIssueDate(commonFunctionsObject.GetTodaysDate());

            workOffer.GetNewOfferVersion();
            workOffer.IssueNewOffer();
            //workOffer.GetNewOfferID();
            //workOffer.GetNewOfferSerial();
            


            ////////////EVERYTHING IS WORKING HERE EXCEPT OFFER_STATUS_ID IS WRONG AND PRICE CURRENCY JUST WAITING FOR THE MINI TEXT BOXES
            string sqlQuery;
            sqlQuery = "INSERT INTO erp_system.dbo.work_offers VALUES (";
            sqlQuery += "'" + workOffer.GetOfferIssueDate().ToString("yyyy-MM-dd") + "',";
            sqlQuery += workOffer.GetOfferProposerId() + ",";
            sqlQuery += workOffer.GetOfferSerial() + ",";
            sqlQuery += workOffer.GetOfferVersion() + ",";
            sqlQuery += workOffer.GetSalesPersonId() + ",";
            sqlQuery += workOffer.GetAddressSerial() + ",";
            sqlQuery += workOffer.GetContactId() + ",";
            sqlQuery += "'" + workOffer.GetOfferID() + "',";

            ////keep it this way until price combo boxes are done
            //sqlQuery += workOffer.GetCurrencyId() + ",";
            sqlQuery += 1 + ",";
            
            sqlQuery += workOffer.GetTotalPriceValue() + ",";
            sqlQuery += workOffer.GetPercentDownPayment() + ",";
            sqlQuery += workOffer.GetPercentOnDelivery() + ",";
            sqlQuery += workOffer.GetPercentOnInstallation() + ",";
            sqlQuery += isDrawing + ",";
            sqlQuery += workOffer.GetDeliveryTimeMinimum() + ",";
            sqlQuery += workOffer.GetDeliveryTimeMaximum() + ",";
            sqlQuery += workOffer.GetDeliveryTimeUnitId() + ",";
            sqlQuery += workOffer.GetDeliveryPointId() + ",";
            sqlQuery += workOffer.GetOfferContractTypeId() + ",";
            sqlQuery += workOffer.GetWarrantyPeriod() + ",";
            sqlQuery += workOffer.GetWarrantyPeriodTimeUnitId() + ",";
            sqlQuery += workOffer.GetOfferValidityPeriod() + ",";
            sqlQuery += workOffer.GetOfferValidityTimeUnitId() + ",";
            sqlQuery += workOffer.GetOfferStatusId() + ",";
            sqlQuery += "'" + workOffer.GetOfferNotes() + "');";

            if (sqlDatabase.InsertRows(sqlQuery))
                MessageBox.Show("Your Work Offer has been added successfully!");
            else
                MessageBox.Show("An error has occurred, please try again later");
        }

        private void ReviseOfferButtonClick(object sender, RoutedEventArgs e)
        {
            workOffer.SetDrawingSubmissionDeadlineMinimum(drawingDeadlineFrom);
            workOffer.SetDrawingSubmissionDeadlineMaximum(drawingDeadlineTo);
            workOffer.SetWarrantyPeriod(warrantyPeriod);
            workOffer.SetOfferValidityPeriod(offerValidityPeriod);
            workOffer.SetOfferNotes(additionalDescription);
            //workOffer.SetOfferIssueDate(commonFunctionsObject.GetTodaysDate());
            //workOffer.GetNewOfferVersion();
            workOffer.ReviseOffer();
            //workOffer.GetNewOfferSerial();
           

            string sqlQuery;
            sqlQuery = "INSERT INTO erp_system.dbo.work_offers VALUES (";
            sqlQuery += "'" + workOffer.GetOfferIssueDate().ToString("yyyy-MM-dd") + "',";
            sqlQuery += workOffer.GetOfferProposerId() + ",";
            sqlQuery += workOffer.GetOfferSerial() + ",";
            sqlQuery += workOffer.GetOfferVersion() + ",";
            sqlQuery += workOffer.GetSalesPersonId() + ",";
            sqlQuery += workOffer.GetAddressSerial() + ",";
            sqlQuery += workOffer.GetContactId() + ",";
            sqlQuery += "'" + workOffer.GetOfferID() + "',";

            ////keep it this way until price combo boxes are done
            //sqlQuery += workOffer.GetCurrencyId() + ",";
            sqlQuery += 1 + ",";

            sqlQuery += workOffer.GetTotalPriceValue() + ",";
            sqlQuery += workOffer.GetPercentDownPayment() + ",";
            sqlQuery += workOffer.GetPercentOnDelivery() + ",";
            sqlQuery += workOffer.GetPercentOnInstallation() + ",";
            sqlQuery += isDrawing + ",";
            sqlQuery += workOffer.GetDeliveryTimeMinimum() + ",";
            sqlQuery += workOffer.GetDeliveryTimeMaximum() + ",";
            sqlQuery += workOffer.GetDeliveryTimeUnitId() + ",";
            sqlQuery += workOffer.GetDeliveryPointId() + ",";
            sqlQuery += workOffer.GetOfferContractTypeId() + ",";
            sqlQuery += workOffer.GetWarrantyPeriod() + ",";
            sqlQuery += workOffer.GetWarrantyPeriodTimeUnitId() + ",";
            sqlQuery += workOffer.GetOfferValidityPeriod() + ",";
            sqlQuery += workOffer.GetOfferValidityTimeUnitId() + ",";
            sqlQuery += workOffer.GetOfferStatusId() + ",";
            sqlQuery += "'" + workOffer.GetOfferNotes() + "');";

            if (sqlDatabase.InsertRows(sqlQuery))
                MessageBox.Show("Your Work Offer has been revised successfully!");
            else
                MessageBox.Show("An error has occurred, please try again later");
        }
    }
}
