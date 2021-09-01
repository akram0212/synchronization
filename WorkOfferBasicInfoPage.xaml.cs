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
    /// Interaction logic for WorkOfferBasicInfoPage.xaml
    /// </summary>
    public partial class WorkOfferBasicInfoPage : Page
    {
        Employee loggedInUser;
        WorkOffer workOffer;
        

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;

        private List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> companiesList = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> companiesAddedToComboList = new List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> branchInfo = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT> contactInfo = new List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> employeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsList = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsAddedToComboList = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();

        private int viewAddCondition;
        private int salesPersonID;
        private int salesPersonTeamID;
        
        //////////////ADD CONSTRUCTOR////////////////
        /////////////////////////////////////////////
        public WorkOfferBasicInfoPage(ref Employee mLoggedInUser, ref WorkOffer mWorkOffer,int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;
            InitializeComponent();

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            //workOffer = new WorkOffer(sqlDatabase);
            workOffer = mWorkOffer;

            
            ///////////////////////////
            ////ADD
            ///////////////////////////
            if (viewAddCondition == COMPANY_WORK_MACROS.OFFER_ADD_CONDITION)
            {
                FillrfqsList(); 
                ConfigureUIElemenetsForAdd();
                InitializeSalesPersonCombo();
            }
            ////////////////////////////
            ///VIEW
            ////////////////////////////
            else if (viewAddCondition == COMPANY_WORK_MACROS.OFFER_VIEW_CONDITION)
            {
                FillrfqsList();
                ConfigureUIElementsForView();
                SetSalesPersonLabel();
                SetRFQSerialLabel();
                SetCompanyNameLabel();
                SetCompanyAddressLabel();
                SetContactPersonLabel();
            }
            /////////////////////////////
            ///REVISE
            /////////////////////////////
            else if(viewAddCondition == COMPANY_WORK_MACROS.OFFER_REVISE_CONDITION)
            {
                FillrfqsList();
                ConfigureUIElemenetsForAdd();
                InitializeSalesPersonCombo();
            }
            //////////////////////////
            ///RESOLVE RFQ
            //////////////////////////
            else
            {
                FillrfqsList();
                ConfigureUIElemenetsForAdd();
                InitializeSalesPersonCombo();
                SetSalesPersonComboValue();
                if (RFQSerialCombo.IsEnabled == true)
                {
                    InitializeRFQSerialCombo();
                    SetRFQSerialComboValue();
                }
                else
                {
                    SetCompanyNameComboValue();
                    SetContactPersonComboValue();
                }
            }
            if (viewAddCondition != COMPANY_WORK_MACROS.OFFER_VIEW_CONDITION)
            {
                SetSalesPersonComboValue();
                if (RFQSerialCombo.IsEnabled == true)
                    SetRFQSerialComboValue();
                else
                {
                    SetCompanyNameComboValue();
                    SetContactPersonComboValue();
                }
            }
        }

        ///////////////INITIALIZE FUNCTIONS///////////////
        //////////////////////////////////////////////////
        
        
        private bool InitializeSalesPersonCombo()
        {
            if (!commonQueriesObject.GetTeamEmployees(COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID, ref employeesList))
                return false;

            for (int i = 0; i < employeesList.Count(); i++)
            {
                salesPersonCombo.Items.Add(employeesList[i].employee_name);
            }
            salesPersonCombo.Items.Add(loggedInUser.GetEmployeeName());
            return true;
        }

        private bool FillrfqsList()
        {
            if (!commonQueriesObject.GetRFQs(ref rfqsList))
                return false;
            return true;
        }
        private void InitializeRFQSerialCombo()
        {
            for (int i = 0; i < rfqsList.Count; i++)
            {
                if (rfqsList[i].sales_person_id == salesPersonID && rfqsList[i].assignee_id == loggedInUser.GetEmployeeId())
                {
                    RFQSerialCombo.Items.Add(rfqsList[i].rfq_id);
                    rfqsAddedToComboList.Add(rfqsList[i]);
                }
            }
            RFQSerialCombo.IsEnabled = true;
        }

        private void InitializeCompanyNameCombo()
        {
            if (!commonQueriesObject.GetEmployeeCompanies(salesPersonID, ref companiesList))
                return;

            for (int i = 0; i < companiesList.Count; i++)
            {
                companyNameCombo.Items.Add(companiesList[i].company_name);
                companiesAddedToComboList.Add(companiesList[i]);
            }

            companyNameCombo.IsEnabled = true;
        }

        private void InitializeCompanyAddressCombo()
        {
           
            if (!commonQueriesObject.GetCompanyAddresses(companiesList[companyNameCombo.SelectedIndex].company_serial, ref branchInfo))
                return;

            for (int i = 0; i < branchInfo.Count; i++)
            {
                string address;
                address = branchInfo[i].district + ", " + branchInfo[i].city + ", " + branchInfo[i].state_governorate + ", " + branchInfo[i].country + ".";
                companyAddressCombo.Items.Add(address);
            }

            workOffer.InitializeBranchInfo(branchInfo[0].address_serial);
           
            companyAddressCombo.SelectedIndex = 0;
           
            companyAddressCombo.IsEnabled = true;
        }

        private void InitializeCompanyContactCombo()
        {
            int addressSerial = branchInfo[companyAddressCombo.SelectedIndex].address_serial;

            if (!commonQueriesObject.GetCompanyContacts(salesPersonID, addressSerial, ref contactInfo))
                return;
        
            for (int i = 0; i < contactInfo.Count(); i++)
                contactPersonNameCombo.Items.Add(contactInfo[i].contact_name);
        
            
            contactPersonNameCombo.SelectedIndex = 0;

            contactPersonNameCombo.IsEnabled = true;
        }
        ///////////CONFIGURE UI ELEMENTS////////////////////
        ///////////////////////////////////////////////////
        private void ConfigureUIElemenetsForAdd()
        {
            salesPersonLabel.Visibility = Visibility.Collapsed;
            RFQSerialLabel.Visibility = Visibility.Collapsed;
            companyNameLabel.Visibility = Visibility.Collapsed;
            companyAddressLabel.Visibility = Visibility.Collapsed;
            contactPersonNameLabel.Visibility = Visibility.Collapsed;

            salesPersonCombo.Visibility = Visibility.Visible;
            RFQSerialCombo.Visibility = Visibility.Visible;
            companyNameCombo.Visibility = Visibility.Visible;
            companyAddressCombo.Visibility = Visibility.Visible;
            contactPersonNameCombo.Visibility = Visibility.Visible;

            RFQSerialCombo.IsEnabled = false;
            companyNameCombo.IsEnabled = false;
            companyAddressCombo.IsEnabled = false;
            contactPersonNameCombo.IsEnabled = false;
        }
        private void ConfigureUIElementsForView()
        {
            salesPersonLabel.Visibility = Visibility.Visible;
            RFQSerialLabel.Visibility = Visibility.Visible;
            companyNameLabel.Visibility = Visibility.Visible;
            companyAddressLabel.Visibility = Visibility.Visible;
            contactPersonNameLabel.Visibility = Visibility.Visible;

            salesPersonCombo.Visibility = Visibility.Collapsed;
            RFQSerialCombo.Visibility = Visibility.Collapsed;
            companyNameCombo.Visibility = Visibility.Collapsed;
            companyAddressCombo.Visibility = Visibility.Collapsed;
            contactPersonNameCombo.Visibility = Visibility.Collapsed;

        }

        private void DisableCompanyNameaddressContactCombos()
        {
            companyNameCombo.IsEnabled = false;
            companyAddressCombo.IsEnabled = false;
            contactPersonNameCombo.IsEnabled = false;
        }

        private void EnableCompanyNameAddressContactCombos()
        {
            companyNameCombo.IsEnabled = true;
            companyAddressCombo.IsEnabled = true;
            contactPersonNameCombo.IsEnabled = true;
        }
        ///////////SET FUNCTIONS////////////////////////////
        ////////////////////////////////////////////////////
        private void SetSalesPersonLabel()
        {
            salesPersonLabel.Content = workOffer.GetSalesPersonName();
        }

        private void SetSalesPersonComboValue()
        {
            salesPersonCombo.SelectedItem = workOffer.GetSalesPersonName();
        }
        private void SetRFQSerialLabel()
        {
            RFQSerialLabel.Content = workOffer.GetRFQID();
        }

        private void SetRFQSerialComboValue()
        {
            RFQSerialCombo.SelectedItem = workOffer.GetRFQID();
        }

        private void SetCompanyNameLabel()
        {
            companyNameLabel.Content = workOffer.GetCompanyName();
        }

        private void SetCompanyNameComboValue()
        {
            companyNameCombo.Text = workOffer.GetCompanyName();
        }

        private void SetCompanyAddressLabel()
        {
            if (!commonQueriesObject.GetCompanyAddresses(workOffer.GetCompanySerial(), ref branchInfo))
                return;

            string address;
            if (branchInfo.Count != 0)
            {
                address = branchInfo[0].district + ", " + branchInfo[0].city + ", " + branchInfo[0].state_governorate + ", " + branchInfo[0].country + ".";
                companyAddressLabel.Content = address;
            }
        }

        private void SetContactPersonLabel()
        {
            contactPersonNameLabel.Content = workOffer.GetContactName();
        }

        private void SetContactPersonComboValue()
        {
            contactPersonNameCombo.Text= workOffer.GetContactName();
        }

        private void SetCompanyNameAddressContactFromRFQ()
        {
            
            InitializeCompanyNameCombo();
            
           
            companyNameCombo.SelectedItem = rfqsAddedToComboList[RFQSerialCombo.SelectedIndex].company_name;


            if (!commonQueriesObject.GetCompanyAddresses(companiesList[companyNameCombo.SelectedIndex].company_serial, ref branchInfo))
                return;

            for (int i = 0; i < branchInfo.Count; i++)
            {
                string address;
                address = branchInfo[i].district + ", " + branchInfo[i].city + ", " + branchInfo[i].state_governorate + ", " + branchInfo[i].country + ".";
                companyAddressCombo.Items.Add(address);
                companyAddressCombo.SelectedIndex = 0;
            }

            //InitializeCompanyContactCombo();

            contactPersonNameCombo.Items.Add(rfqsAddedToComboList[RFQSerialCombo.SelectedIndex].contact_name);
            contactPersonNameCombo.SelectedItem = rfqsAddedToComboList[RFQSerialCombo.SelectedIndex].contact_name;

            DisableCompanyNameaddressContactCombos();
        }

        ///////////GET FUNCTIONS////////////////////////////
        ////////////////////////////////////////////////////


        ///////////SELECTION CHANGED HANDLERS///////////////
        ////////////////////////////////////////////////////
        private void OnSelChangedSalesPersonCombo(object sender, SelectionChangedEventArgs e)
        {
            RFQSerialCombo.Items.Clear();
            companyNameCombo.Items.Clear();
            RFQSerialCombo.SelectedIndex = -1;
            companyNameCombo.SelectedIndex = -1;

            if (salesPersonCombo.SelectedIndex != employeesList.Count())
            {
                salesPersonID = employeesList[salesPersonCombo.SelectedIndex].employee_id;
                salesPersonTeamID = employeesList[salesPersonCombo.SelectedIndex].team.team_id;
            }
            else
            {
                salesPersonID = loggedInUser.GetEmployeeId();
                salesPersonTeamID = COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID;
            }

            if (viewAddCondition == COMPANY_WORK_MACROS.OFFER_ADD_CONDITION)
                workOffer.ResetWorkOfferInfo(salesPersonTeamID);

            workOffer.InitializeOfferProposerInfo(loggedInUser.GetEmployeeId(), salesPersonTeamID);

            if (salesPersonTeamID == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
            {
                workOffer.InitializeSalesPersonInfo(salesPersonID);
                InitializeRFQSerialCombo();
                companyNameCombo.SelectedItem = null;
                companyNameCombo.IsEnabled = false;
            }
            else
            {
                workOffer.InitializeSalesPersonInfo(loggedInUser.GetEmployeeId());
                InitializeCompanyNameCombo();
                RFQSerialCombo.SelectedItem = null;
                RFQSerialCombo.IsEnabled = false;
            }
        }
        private void OnSelChangedRFQSerialCombo(object sender, SelectionChangedEventArgs e)
        {
            if (RFQSerialCombo.SelectedItem != null)
            {
                workOffer.InitializeRFQInfo(rfqsAddedToComboList[RFQSerialCombo.SelectedIndex].rfq_serial, rfqsAddedToComboList[RFQSerialCombo.SelectedIndex].rfq_version);
                //workOffer.InitializeOfferProposerInfo(workOffer.GetAssigneeId(), COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID);
                SetCompanyNameAddressContactFromRFQ();
            }
        }
        private void OnSelChangedCompanyNameCombo(object sender, SelectionChangedEventArgs e)
        {
            companyAddressCombo.Items.Clear();

            if (companyNameCombo.SelectedItem != null)
            {
                InitializeCompanyAddressCombo();
                workOffer.InitializeCompanyInfo(companiesAddedToComboList[companyNameCombo.SelectedIndex].company_serial);
            }

            if (companyNameCombo.SelectedItem == null)
            {
                companyAddressCombo.SelectedItem = null;
                companyAddressCombo.IsEnabled = false;
            }
            
            
        }

        private void OnSelChangedCompanyAddressCombo(object sender, SelectionChangedEventArgs e)
        {
            contactPersonNameCombo.Items.Clear();

            if (companyAddressCombo.SelectedItem == null)
            {
                contactPersonNameCombo.SelectedItem = null;
                contactPersonNameCombo.IsEnabled = false;
            }
            else
                InitializeCompanyContactCombo();
           
        }

        private void OnSelChangedContactPersonCombo(object sender, SelectionChangedEventArgs e)
        {
            if (contactPersonNameCombo.SelectedItem != null)
                workOffer.InitializeContactInfo(contactInfo[contactPersonNameCombo.SelectedIndex].contact_id);

        }

        ///////////BUTTON CLICK HANDLERS/////////
        /////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            WorkOfferProductsPage workOfferProductsPage = new WorkOfferProductsPage(ref loggedInUser, ref workOffer, viewAddCondition);
            NavigationService.Navigate(workOfferProductsPage);
        }
        private void OnClickPaymentAndDelivery(object sender, MouseButtonEventArgs e)
        {
             WorkOfferPaymentAndDeliveryPage paymentAndDeliveryPage = new WorkOfferPaymentAndDeliveryPage(ref loggedInUser, ref workOffer, viewAddCondition);
            NavigationService.Navigate(paymentAndDeliveryPage);
        }

        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            WorkOfferAdditionalInfoPage offerAdditionalInfoPage = new WorkOfferAdditionalInfoPage(ref loggedInUser, ref workOffer, viewAddCondition);
            NavigationService.Navigate(offerAdditionalInfoPage);
        }

    }
}
