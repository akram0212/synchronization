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
    /// Interaction logic for RFQsPage.xaml
    /// </summary>
    public partial class RFQsPage : Page
    {
        private Employee loggedInUser;

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;

        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsList;
        //private List<COMPANY_WORK_MACROS.WORK_OFFER_MAX_STRUCT> offersList;

        private List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> productTypes;
        private List<COMPANY_WORK_MACROS.BRAND_STRUCT> brandTypes;
        public RFQsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;

            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            rfqsList = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();
            //offersList = new List<COMPANY_WORK_MACROS.WORK_OFFER_MAX_STRUCT>();

            productTypes = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
            brandTypes = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();

            InitializeYearsComboBox();
            InitializeQuartersComboBox();

            if (!InitializeProductsComboBox())
                return;

            if (!InitializeBrandsComboBox())
                return;

            if (!InitializeRFQsStackPanel())
                return;
        }

        private void InitializeYearsComboBox()
        {
            int initialYear = 2020;
            int finalYear = Int32.Parse(DateTime.Now.Year.ToString());
            for (; initialYear <= finalYear; initialYear++)
                yearCombo.Items.Add(initialYear);
        }
        private void InitializeQuartersComboBox()
        {
            //INSTEAD OF HARD CODING YOUR COMBO, 
            //THIS FUNCTION IS BETTER SO EVERYTIME IN OUR PROJECT WE NEED TO LIST QUARTERS
            //WE ARE SURE THAT ALL HAVE THE SAME FORMAT
            for (int i = 0; i < BASIC_MACROS.NO_OF_QUARTERS; i++)
                quarterCombo.Items.Add(commonFunctionsObject.GetQuarterName(i + 1));
            //ALSO IF YOU NOTICE, I DIDN'T EVEN USE i < 4, I USED A PRE-DEFINED MACRO INSTEAD, SO THE CODE IS ALWAYS READABLE
        }

        //THIS FUNCTIONS ACCESS SQL SERVER, SO YOU SHALL ALWAYS CHECK IF THE QUERY IS COMPLETED SUCCESSFULLY
        // IF NOT YOU SHALL STOP DATA ACCESS FOR THE CODE NOT TO CRASH
        private bool InitializeProductsComboBox()
        {
            if (!commonQueriesObject.GetCompanyProducts(ref productTypes))
                return false;

            for (int i = 0; i < productTypes.Count; i++)
                productCombo.Items.Add(productTypes[i].typeName);

            return true;
        }

        private bool InitializeBrandsComboBox()
        {
            //INTENTIONALLY LEFT IT EMPTY FOR YOU TO FILL
            return true;
        }
        private bool InitializeRFQsStackPanel()
        {
            if (!commonQueriesObject.GetRFQs(ref rfqsList))
                return false;
            
            //if (!commonQueriesObject.GetWorkOffers(ref offersList))
            //    return false;

            return true;
        }

        private void OnButtonClickedMyProfile(object sender, RoutedEventArgs e)
        {
            UserPortalPage userPortal = new UserPortalPage(ref loggedInUser);
            this.NavigationService.Navigate(userPortal);
        }

        private void OnButtonClickedContacts(object sender, RoutedEventArgs e)
        {
            ContactsPage contacts = new ContactsPage(ref loggedInUser);
            this.NavigationService.Navigate(contacts);
        }

        private void OnButtonClickedOrders(object sender, RoutedEventArgs e)
        {

        }
        private void OnButtonClickedOffers(object sender, RoutedEventArgs e)
        {

        }
        private void OnButtonClickedRFQs(object sender, RoutedEventArgs e)
        {
            RFQsPage rfqs = new RFQsPage(ref loggedInUser);
            this.NavigationService.Navigate(rfqs);
        }
        private void OnButtonClickedVisits(object sender, RoutedEventArgs e)
        {

        }
        private void OnButtonClickedCalls(object sender, RoutedEventArgs e)
        {

        }
        private void OnButtonClickedMeetings(object sender, RoutedEventArgs e)
        {

        }
        private void OnButtonClickedStatistics(object sender, RoutedEventArgs e)
        {

        }

        private void OnBtnClickedAdd(object sender, RoutedEventArgs e)
        {
        }

        private void OnBtnClickedView(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
