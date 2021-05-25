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
        private CommonFunctions commonFunctionssObject;

        private List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> productsList;
        public RFQsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;

            commonQueriesObject = new CommonQueries();
            commonFunctionssObject = new CommonFunctions();

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
                quarterCombo.Items.Add(commonFunctionObject.GetQuarterName(i + 1));
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
        }
        private bool InitializeRFQsStackPanel()
        {
            return true;
        }
    }
}
