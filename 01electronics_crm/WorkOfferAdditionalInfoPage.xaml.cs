﻿using System;
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
using Microsoft.Win32;
using _01electronics_library;
using System.ComponentModel;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for WorkOfferAdditionalInfoPage.xaml
    /// </summary>
    public partial class WorkOfferAdditionalInfoPage : Page
    {
        Employee loggedInUser;
        WorkOffer workOffer;
        WordAutomation wordAutomation;

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private IntegrityChecks integrityChecks;
        protected FTPServer fTPObject;

        private List<BASIC_STRUCTS.CONTRACT_STRUCT> contractTypes = new List<BASIC_STRUCTS.CONTRACT_STRUCT>();
        private List<BASIC_STRUCTS.TIMEUNIT_STRUCT> timeUnits = new List<BASIC_STRUCTS.TIMEUNIT_STRUCT>();

        private int viewAddCondition;
        private int warrantyPeriod = 0;
        private int offerValidityPeriod = 0;
        private int drawingDeadlineFrom = 0;
        private int drawingDeadlineTo = 0;
        private int isDrawing = 0;

        private string additionalDescription;

        protected String serverFolderPath;
        protected String serverFileName;

        protected String localFolderPath;
        protected String localFileName;

        protected BackgroundWorker uploadBackground;
        protected BackgroundWorker downloadBackground;

        public WorkOfferAdditionalInfoPage(ref Employee mLoggedInUser, ref WorkOffer mWorkOffer, int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;
            InitializeComponent();

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();
            integrityChecks = new IntegrityChecks();
            wordAutomation = new WordAutomation();
            fTPObject = new FTPServer();

            workOffer = mWorkOffer;


            uploadBackground = new BackgroundWorker();
            uploadBackground.DoWork += BackgroundUpload;
            uploadBackground.ProgressChanged += OnUploadProgressChanged;
            uploadBackground.RunWorkerCompleted += OnUploadBackgroundComplete;
            uploadBackground.WorkerReportsProgress = true;

            downloadBackground = new BackgroundWorker();
            downloadBackground.DoWork += BackgroundDownload;
            downloadBackground.ProgressChanged += OnDownloadProgressChanged;
            downloadBackground.RunWorkerCompleted += OnDownloadBackgroundComplete;
            downloadBackground.WorkerReportsProgress = true;

            ConfigureDrawingSubmissionUIElements();


            /////////////////////////
            ///ADD
            /////////////////////////
            if (viewAddCondition == COMPANY_WORK_MACROS.OFFER_ADD_CONDITION)
            {
                ConfigureDrawingSubmissionUIElements();
                InitializeContractType();
                InitializeTimeUnitComboBoxes();
                
            }
            //////////////////////////
            ///VIEW
            //////////////////////////
            else if (viewAddCondition == COMPANY_WORK_MACROS.OFFER_VIEW_CONDITION)
            {
                InitializeContractType();
                InitializeTimeUnitComboBoxes();

                //if (workOffer.GetDrawingSubmissionDeadlineMinimum() != 0)
                //    drawingConditionsCheckBox.IsChecked = true;

                ConfigureUIElementsView();
                SetDrawingSubmissionValues();
                SetContractTypeValue();
                SetWarrantyPeriodValues();
                SetValidityPeriodValues();
                SetAdditionalDescriptionValue(); 
            }
            //////////////////////////////
            ///REVISE
            //////////////////////////////
            else if (viewAddCondition == COMPANY_WORK_MACROS.OFFER_REVISE_CONDITION)
            {
                ConfigureDrawingSubmissionUIElements();
                InitializeContractType();
                InitializeTimeUnitComboBoxes();
                
                //if (workOffer.GetDrawingSubmissionDeadlineMinimum() != 0)
                //    drawingConditionsCheckBox.IsChecked = true;
                
            }
            ////////////////////////
            ///RESOLVE RFQ
            ///////////////////////
            else
            {
                //ConfigureDrawingSubmissionUIElements();
                InitializeContractType();
                InitializeTimeUnitComboBoxes();
                
            }

            if (viewAddCondition != COMPANY_WORK_MACROS.OFFER_VIEW_CONDITION)
            {
                SetDrawingSubmissionValues();
                SetContractTypeValue();
                SetWarrantyPeriodValues();
                SetValidityPeriodValues();
                SetAdditionalDescriptionValue();
            }
        }
        /////////////////////////////////
        ///CONFIGURE UI ELEMENTS FUNCTIONS
        /////////////////////////////////

        
       
        private void ConfigureDrawingSubmissionUIElements()
        {
            drawingDeadlineFromTextBox.IsEnabled = false;
            drawingDeadlineToTextBox.IsEnabled = false;
            drawingDeadlineDateComboBox.IsEnabled = false;
        }

        private void ConfigureUIElementsView()
        {
            //drawingConditionsCheckBox.IsEnabled = false;
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
            for (int i = 0; i < contractTypes.Count; i++)
                contractTypeComboBox.Items.Add(contractTypes[i].contractName);

            return true;
        }

        private bool InitializeTimeUnitComboBoxes()
        {
            if (!commonQueriesObject.GetTimeUnits(ref timeUnits))
                return false;
            for (int i = 0; i < timeUnits.Count(); i++)
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
            if (integrityChecks.CheckInvalidCharacters(warrantyPeriodTextBox.Text, BASIC_MACROS.PHONE_STRING) && warrantyPeriodTextBox.Text != "")
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
            if (integrityChecks.CheckInvalidCharacters(offerValidityTextBox.Text, BASIC_MACROS.PHONE_STRING) && offerValidityTextBox.Text != "")
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
            if (integrityChecks.CheckInvalidCharacters(drawingDeadlineFromTextBox.Text, BASIC_MACROS.PHONE_STRING) && drawingDeadlineFromTextBox.Text != "")
                drawingDeadlineFrom = int.Parse(drawingDeadlineFromTextBox.Text);
            else
            {
                drawingDeadlineFrom = 0;
                drawingDeadlineFromTextBox.Text = null;
            }
        }

        private void DrawingDeadlineToTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(drawingDeadlineToTextBox.Text, BASIC_MACROS.PHONE_STRING) && drawingDeadlineToTextBox.Text != "")
                drawingDeadlineTo = int.Parse(drawingDeadlineToTextBox.Text);
            else
            {
                drawingDeadlineTo = 0;
                drawingDeadlineToTextBox.Text = null;
            }
        }
        private void DrawingDeadlineDateComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (drawingDeadlineDateComboBox.SelectedItem != null)
            {
                workOffer.SetHasDrawings(true);
                workOffer.SetDrawingSubmissionDeadlineTimeUnit(timeUnits[drawingDeadlineDateComboBox.SelectedIndex].timeUnitId, timeUnits[drawingDeadlineDateComboBox.SelectedIndex].timeUnit);
            }
            else
            {
                workOffer.SetHasDrawings(false);
            }
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
            
         
        }



        private void ReviseOfferButtonClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void DrawingDeadlineDateFromWhenComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void WarrantyPeriodFromWhenComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnButtonClickAutomateWorkOffer(object sender, RoutedEventArgs e)
        {
            workOffer.SetDrawingSubmissionDeadlineMinimum(drawingDeadlineFrom);
            workOffer.SetDrawingSubmissionDeadlineMaximum(drawingDeadlineTo);
            workOffer.SetWarrantyPeriod(warrantyPeriod);
            workOffer.SetOfferValidityPeriod(offerValidityPeriod);
            workOffer.SetOfferNotes(additionalDescription);
            
            if (!workOffer.GetNewOfferSerial())
                return;

            if (!workOffer.GetNewOfferVersion())
                return;

            workOffer.SetOfferIssueDateToToday();

            workOffer.GetNewOfferID();

            wordAutomation.AutomateWorkOffer(workOffer);
        }

        private void OnButtonClickOk(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.OFFER_ADD_CONDITION || viewAddCondition == COMPANY_WORK_MACROS.OFFER_RESOLVE_CONDITION)
            {
                workOffer.SetDrawingSubmissionDeadlineMinimum(drawingDeadlineFrom);
                workOffer.SetDrawingSubmissionDeadlineMaximum(drawingDeadlineTo);
                workOffer.SetWarrantyPeriod(warrantyPeriod);
                workOffer.SetOfferValidityPeriod(offerValidityPeriod);
                workOffer.SetOfferNotes(additionalDescription);

                if (workOffer.GetSalesPersonId() == 0)
                    MessageBox.Show("You need to choose sales person before adding a work offer!");
                else if (workOffer.GetCompanyName() == null)
                    MessageBox.Show("You need to choose a company before adding a work offer!");
                else if (workOffer.GetAddressSerial() == 0)
                    MessageBox.Show("You need to choose company address before adding a work offer!");
                else if (workOffer.GetContactId() == 0)
                    MessageBox.Show("You need to choose a contact before adding a work offer!");
                else if (workOffer.GetOfferProduct1TypeId() != 0 && workOffer.GetProduct1PriceValue() == 0)
                    MessageBox.Show("You need to add a price for product 1 before adding a work offer!");
                else if (workOffer.GetOfferProduct2TypeId() != 0 && workOffer.GetProduct2PriceValue() == 0)
                    MessageBox.Show("You need to add a price for product 2 before adding a work offer!");
                else if (workOffer.GetOfferProduct3TypeId() != 0 && workOffer.GetProduct3PriceValue() == 0)
                    MessageBox.Show("You need to add a price for product 3 before adding a work offer!");
                else if (workOffer.GetOfferProduct4TypeId() != 0 && workOffer.GetProduct4PriceValue() == 0)
                    MessageBox.Show("You need to add a price for product 4 before adding a work offer!");
                else if (workOffer.GetPercentDownPayment() + workOffer.GetPercentOnDelivery() + workOffer.GetPercentOnInstallation() < 100)
                    MessageBox.Show("Down payement, on delivery and on installation percentages total is less than 100%!!");
                else if (workOffer.GetDeliveryTimeMinimum() == 0 || workOffer.GetDeliveryTimeMaximum() == 0)
                    MessageBox.Show("You need to set delivery time min and max before adding a work offer!");
                else if (workOffer.GetDeliveryPointId() == 0)
                    MessageBox.Show("You need to set delivery point before adding a work offer!");
                else if (workOffer.GetOfferContractTypeId() == 0)
                    MessageBox.Show("You need to set contract type before adding a work offer!");
                else if (workOffer.GetWarrantyPeriod() == 0 || workOffer.GetWarrantyPeriodTimeUnitId() == 0)
                    MessageBox.Show("You need to set warranty period before adding a work offer!");
                else if (workOffer.GetOfferValidityPeriod() == 0 || workOffer.GetOfferValidityTimeUnitId() == 0)
                    MessageBox.Show("You need to set validity period before adding a work offer!");
                else
                {
                    if (workOffer.IssueNewOffer())
                        MessageBox.Show("WorkOffer added succefully!");

                    WorkOfferWindow workOfferWindow = new WorkOfferWindow(ref loggedInUser, ref workOffer, viewAddCondition);

                    NavigationWindow currentWindow = (NavigationWindow)this.Parent;
                    currentWindow.Close();
                }
            }
            if (viewAddCondition == COMPANY_WORK_MACROS.OFFER_REVISE_CONDITION)
            {
                workOffer.SetDrawingSubmissionDeadlineMinimum(drawingDeadlineFrom);
                workOffer.SetDrawingSubmissionDeadlineMaximum(drawingDeadlineTo);
                workOffer.SetWarrantyPeriod(warrantyPeriod);
                workOffer.SetOfferValidityPeriod(offerValidityPeriod);
                workOffer.SetOfferNotes(additionalDescription);

                if (workOffer.GetSalesPersonId() == 0)
                    MessageBox.Show("You need to choose sales person before adding a work offer!");
                else if (workOffer.GetCompanyName() == null)
                    MessageBox.Show("You need to choose a company before adding a work offer!");
                else if (workOffer.GetAddressSerial() == 0)
                    MessageBox.Show("You need to choose company address before adding a work offer!");
                else if (workOffer.GetContactId() == 0)
                    MessageBox.Show("You need to choose a contact before adding a work offer!");
                else if (workOffer.GetOfferProduct1TypeId() != 0 && workOffer.GetProduct1PriceValue() == 0)
                    MessageBox.Show("You need to add a price for product 1 before adding a work offer!");
                else if (workOffer.GetOfferProduct2TypeId() != 0 && workOffer.GetProduct2PriceValue() == 0)
                    MessageBox.Show("You need to add a price for product 2 before adding a work offer!");
                else if (workOffer.GetOfferProduct3TypeId() != 0 && workOffer.GetProduct3PriceValue() == 0)
                    MessageBox.Show("You need to add a price for product 3 before adding a work offer!");
                else if (workOffer.GetOfferProduct4TypeId() != 0 && workOffer.GetProduct4PriceValue() == 0)
                    MessageBox.Show("You need to add a price for product 4 before adding a work offer!");
                else if (workOffer.GetPercentDownPayment() + workOffer.GetPercentOnDelivery() + workOffer.GetPercentOnInstallation() < 100)
                    MessageBox.Show("Down payement, on delivery and on installation percentages total is less than 100%!!");
                else if (workOffer.GetDeliveryTimeMinimum() == 0 || workOffer.GetDeliveryTimeMaximum() == 0)
                    MessageBox.Show("You need to set delivery time min and max before adding a work offer!");
                else if (workOffer.GetDeliveryPointId() == 0)
                    MessageBox.Show("You need to set delivery point before adding a work offer!");
                else if (workOffer.GetOfferContractTypeId() == 0)
                    MessageBox.Show("You need to set contract type before adding a work offer!");
                else if (workOffer.GetWarrantyPeriod() == 0 || workOffer.GetWarrantyPeriodTimeUnitId() == 0)
                    MessageBox.Show("You need to set warranty period before adding a work offer!");
                else if (workOffer.GetOfferValidityPeriod() == 0 || workOffer.GetOfferValidityTimeUnitId() == 0)
                    MessageBox.Show("You need to set validity period before adding a work offer!");


                else
                {
                    if(workOffer.ReviseOffer())
                        MessageBox.Show("Offer Revised successfully!");

                    WorkOfferWindow workOfferWindow = new WorkOfferWindow(ref loggedInUser, ref workOffer, viewAddCondition);

                    NavigationWindow currentWindow = (NavigationWindow)this.Parent;
                    currentWindow.Close();
                }
                
            }
        }

        private void OnBtnClickBrowse(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.OFFER_VIEW_CONDITION)
            {
                workOffer.GetNewOfferSerial();
                workOffer.GetNewOfferVersion();
                workOffer.GetNewOfferID();

                OpenFileDialog uploadFile = new OpenFileDialog();

                if (uploadFile.ShowDialog() == false)
                    return;

                if (!integrityChecks.CheckFileEditBox(uploadFile.FileName))
                    return;

                serverFolderPath = BASIC_MACROS.OFFER_FILES_PATH;
                serverFileName = workOffer.GetOfferID() + ".pdf";
                integrityChecks.RemoveExtraSpaces(serverFileName, ref serverFileName);

                localFolderPath = uploadFile.FileName;
                localFileName = null;

                offerFilePath.Visibility = Visibility.Collapsed;
                uploadFileProgressBar.Visibility = Visibility.Visible;

                uploadBackground.RunWorkerAsync();
            }
            else
            {
                System.Windows.Forms.FolderBrowserDialog downloadFile = new System.Windows.Forms.FolderBrowserDialog();

                if (downloadFile.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                    return;

                if (!integrityChecks.CheckFileEditBox(downloadFile.SelectedPath))
                    return;

                serverFolderPath = BASIC_MACROS.OFFER_FILES_PATH;
                serverFileName = workOffer.GetOfferID() + ".pdf";
                integrityChecks.RemoveExtraSpaces(serverFileName, ref serverFileName);

                localFolderPath = downloadFile.SelectedPath;
                localFileName = workOffer.GetOfferID() + ".pdf";
                integrityChecks.RemoveExtraSpaces(localFileName, ref localFileName);

                offerFilePath.Visibility = Visibility.Collapsed;
                uploadFileProgressBar.Visibility = Visibility.Visible;

                downloadBackground.RunWorkerAsync();
            }
        }

        protected void BackgroundUpload(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker uploadBackground = sender as BackgroundWorker;

            uploadBackground.ReportProgress(50);
            if (!fTPObject.UploadFile(localFolderPath + localFileName, serverFolderPath + "/" + serverFileName))
                return;

            uploadBackground.ReportProgress(75);
            

            uploadBackground.ReportProgress(100);
        }

        protected void OnUploadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            uploadFileProgressBar.Value = e.ProgressPercentage;
        }

        protected void OnUploadBackgroundComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            offerFilePath.Visibility = Visibility.Visible;
            uploadFileProgressBar.Visibility = Visibility.Collapsed;

            BrushConverter brush = new BrushConverter();
            offerFilePath.Text= "SUBMITTED";
            offerFilePath.Foreground = (Brush)brush.ConvertFrom("#FF0000");

            browseButton.Content = "Update";
            browseButton.IsEnabled = true;
        }


        protected void BackgroundDownload(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker downloadBackground = sender as BackgroundWorker;
            
            downloadBackground.ReportProgress(50);
            if (!fTPObject.DownloadFile(serverFolderPath + "/" + serverFileName, localFolderPath + "/" + localFileName)) 
                return;

            downloadBackground.ReportProgress(100);
        }

        protected void OnDownloadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            uploadFileProgressBar.Value = e.ProgressPercentage;
        }

        protected void OnDownloadBackgroundComplete(object sender, RunWorkerCompletedEventArgs e)
        {
           
                offerFilePath.Visibility = Visibility.Visible;
                uploadFileProgressBar.Visibility = Visibility.Collapsed;

                offerFilePath.Text = "SUCCESS!";
                
            
        }
    }
}