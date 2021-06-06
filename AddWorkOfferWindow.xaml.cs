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
using System.Windows.Shapes;
using _01electronics_erp;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for AddWorkOfferWindow.xaml
    /// </summary>
    public partial class AddWorkOfferWindow : Window
    {

        private Employee loggedInUser;
        private CommonQueries commonQueriesObject = new CommonQueries();
        private CommonFunctions commonFunctionsObject = new CommonFunctions();
        private SQLServer commonQueriesSqlObject = new SQLServer();
        private IntegrityChecks IntegrityChecks = new IntegrityChecks();

        public struct Company_Struct
        {
            public int companySerial;
            public string companyName;
        };

        private List<Company_Struct> companyInfo = new List<Company_Struct>();

        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> employeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsList = new List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> branchInfo = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT> contactInfo = new List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT>();
        private List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> products = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
        private List<COMPANY_WORK_MACROS.BRAND_STRUCT> brands = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();
        private List<COMPANY_WORK_MACROS.MODEL_STRUCT> models = new List<COMPANY_WORK_MACROS.MODEL_STRUCT>();
        private List<BASIC_STRUCTS.CONTRACT_STRUCT> contractTypes = new List<BASIC_STRUCTS.CONTRACT_STRUCT>();

        private int salesPersonID;
        private int salesPersonTeamID;
        private int rfqSerialID;

        private int companyNameID;
        private int companyBranchID;
        private int contactNameID;

        private int type1ID;
        private int type2ID;
        private int type3ID;
        private int type4ID;

        private int brand1ID;
        private int brand2ID;
        private int brand3ID;
        private int brand4ID;

        private int model1ID;
        private int model2ID;
        private int model3ID;
        private int model4ID;

        private int quantity1;
        private int quantity2;
        private int quantity3;
        private int quantity4;

        private int price1;
        private int price2;
        private int price3;
        private int price4;

        private string price1Type;
        private string price2Type;
        private string price3Type;
        private string price4Type;

        private int price1Total;
        private int price2Total;
        private int price3Total;
        private int price4Total;

        private int totalPrice;
        private int downPaymentActual;
        private int onDeliveryActual;
        private int onInstallationActual;
        private int downPaymentPercentage;
        private int onDeliveryPercentage;
        private int onInstallationPercentage;

        private int drawingSubmissionFrom;
        private int drawingSubmissionTo;
        private int drawingSubmissionType;

        private int deliveryTimeFrom;
        private int deliveryTimeTo;
        private int deliveryTimeType;
        private int deliveryPoint;

        private int contractType;
        private int warrantyPeriod;
        private int warrantyPeriodType;
        private int offerValidity;
        private int offerValidityType;

        string additionalDescription;

       

        public AddWorkOfferWindow(ref Employee mloggedInUser)
        {
            InitializeComponent();
            loggedInUser = mloggedInUser;

            SetAddWorkOfferWindow();
            InitializeSalesPersonCombo();
            InitializeProductTypeCombo();
            InitializeProductBrandCombo();
            InitializeContractTypeCombo();
            InitializeCurrencyComboBoxes();
            InitializePeriodComboBoxes();
        }

        private void EnableUIElements()
        {
            rfqSerialCombo.IsEnabled = true;

            companyNameCombo.IsEnabled = true;
            companyBranchCombo.IsEnabled = true;
            contactNameCombo.IsEnabled = true;

            type1Combo.IsEnabled = true;
            brand1Combo.IsEnabled = true;
            model1Combo.IsEnabled = true;
            quantity1TextBox.IsEnabled = true;
            pricePerItem1TextBox.IsEnabled = true;
            pricePerItem1ComboBox.IsEnabled = true;

            totalPriceCombo.IsEnabled = true;
            totalPriceTextBox.IsEnabled = true;
            downPaymentPercentageTextBox.IsEnabled = true;
            downPaymentActualTextBox.IsEnabled = true;
            onDeliveryPercentageTextBox.IsEnabled = true;
            onDeliveryPercentageTextBox.Text = "0";
            onDeliveryActualTextBox.IsEnabled = true;
            onInstallationPercentageTextBox.IsEnabled = true;
            onInstallationPercentageTextBox.Text = 0.ToString();
            onInstallationActualTextBox.IsEnabled = true;

            deliveryTimeTextBoxFrom.IsEnabled = true;
            deliveryTimeTextBoxTo.IsEnabled = true;
            deliveryTimeCombo.IsEnabled = true;
            deliveryPointCombo.IsEnabled = true;

            contractTypeCombo.IsEnabled = true;
            warrantyPeriodTextBox.IsEnabled = true;
            warrantyPeriodCombo.IsEnabled = true;
            offerValidityTextBox.IsEnabled = true;
            offerValidityCombo.IsEnabled = true;
            additionalDescriptionTextBox.IsEnabled = true;
        }

        private void SetAddWorkOfferWindow()
        {
            rfqSerialCombo.IsEnabled = false;

            companyNameCombo.IsEnabled = false;
            companyBranchCombo.IsEnabled = false;
            contactNameCombo.IsEnabled = false;

            type1Combo.IsEnabled = false;
            brand1Combo.IsEnabled = false;
            model1Combo.IsEnabled = false;
            quantity1TextBox.IsEnabled = false;
            quantity1TextBox.Text = "0";
            pricePerItem1TextBox.IsEnabled = false;
            pricePerItem1TextBox.Text = "0";
            pricePerItem1ComboBox.IsEnabled = false;

            type2Combo.IsEnabled = false;
            brand2Combo.IsEnabled = false;
            model2Combo.IsEnabled = false;
            quantity2TextBox.IsEnabled = false;
            quantity2TextBox.Text = "0";
            pricePerItem2TextBox.IsEnabled = false;
            pricePerItem2TextBox.Text = "0";
            pricePerItem2ComboBox.IsEnabled = false;

            type3Combo.IsEnabled = false;
            brand3Combo.IsEnabled = false;
            model3Combo.IsEnabled = false;
            quantity3TextBox.IsEnabled = false;
            quantity3TextBox.Text = "0";
            pricePerItem3TextBox.IsEnabled = false;
            pricePerItem3TextBox.Text = "0";
            pricePerItem3ComboBox.IsEnabled = false;

            type4Combo.IsEnabled = false;
            brand4Combo.IsEnabled = false;
            model4Combo.IsEnabled = false;
            quantity4TextBox.IsEnabled = false;
            quantity4TextBox.Text = "0";
            pricePerItem4TextBox.IsEnabled = false;
            pricePerItem4TextBox.Text = "0";
            pricePerItem4ComboBox.IsEnabled = false;

            totalPriceCombo.IsEnabled = false;
            totalPriceTextBox.IsEnabled = false;
            downPaymentPercentageTextBox.IsEnabled = false;
            downPaymentPercentageTextBox.Text= 0.ToString();
            downPaymentActualTextBox.IsEnabled = false;
            downPaymentActualTextBox.Text = 0.ToString();
            onDeliveryPercentageTextBox.IsEnabled = false;
            onDeliveryPercentageTextBox.Text = 0.ToString();
            onDeliveryActualTextBox.IsEnabled = false;
            onDeliveryActualTextBox.Text = "0";
            onInstallationPercentageTextBox.IsEnabled = false;
            onInstallationPercentageTextBox.Text = "0";
            onInstallationActualTextBox.IsEnabled = false;
            onInstallationActualTextBox.Text = "0";

            drawingSubmissionDeadlineFrom.IsEnabled = false;
            drawingSubmissionDeadlineFrom.Text = "0";
            drawingSubmissionDeadlineTo.IsEnabled = false;
            drawingSubmissionDeadlineTo.Text = "0";
            drawingSubmissionDeadlineCombo.IsEnabled = false;

            deliveryTimeTextBoxFrom.IsEnabled = false;
            deliveryTimeTextBoxFrom.Text = "0";
            deliveryTimeTextBoxTo.IsEnabled = false;
            deliveryTimeTextBoxTo.Text = "0";
            deliveryTimeCombo.IsEnabled = false;
            deliveryPointCombo.IsEnabled = false;

            contractTypeCombo.IsEnabled = false;
            warrantyPeriodTextBox.IsEnabled = false;
            warrantyPeriodTextBox.Text = "0";
            warrantyPeriodCombo.IsEnabled = false;
            offerValidityTextBox.IsEnabled = false;
            offerValidityTextBox.Text = "0";
            offerValidityCombo.IsEnabled = false;
            additionalDescriptionTextBox.IsEnabled = false;

        }

        private void InitializeSalesPersonCombo()
        {
            if (!commonQueriesObject.GetDepartmentEmployees(COMPANY_ORGANISATION_MACROS.MARKETING_AND_SALES_DEPARTMENT_ID, ref employeesList))
                return;

            for (int i = 0; i < employeesList.Count(); i++)
            {
                string temp = employeesList[i].employee_name;
                salesPersonCombo.Items.Add(temp);
            }
        }

        private void InitializeRFQSerialCombo()
        {
            if (!commonQueriesObject.GetRFQs(ref rfqsList))
                return;
            for (int i = rfqsList.Count - 1; i >= 0; i--)
            {
                if (rfqsList[i].sales_person_id == salesPersonID)
                    rfqSerialCombo.Items.Add(rfqsList[i].rfq_id);
            }
            rfqSerialCombo.IsEnabled = true;
        }

        private void InitializeCompanyNameCombo()
        {
            if (!GetCompaniesQuery(salesPersonID, ref companyInfo))
              return;

            for (int i = 0; i < companyInfo.Count; i++)
            {
                string tempName = companyInfo[i].companyName;
                companyNameCombo.Items.Add(tempName);
            }
        }

        private void InitializeCompanyBranchCombo()
        {
            int companySerial = companyNameCombo.SelectedIndex + 1;

                if (!commonQueriesObject.GetCompanyAddresses(companySerial, ref branchInfo))
                    return;

                for (int i = 0; i < branchInfo.Count; i++)
                {
                    string address;
                    address = branchInfo[i].district + ", " + branchInfo[i].city + ", " + branchInfo[i].state_governorate + ", " + branchInfo[i].country + ".";
                    companyBranchCombo.Items.Add(address);
                }

                if (branchInfo.Count == 1)
                    companyBranchCombo.SelectedItem = companyBranchCombo.Items.GetItemAt(0);
        }

        private void InitializeCompanyContactCombo()
        {
            int temp = companyBranchCombo.SelectedIndex;
            int addressSerial = branchInfo[temp].address_serial;

            if (!commonQueriesObject.GetCompanyContacts(loggedInUser.GetEmployeeId(), addressSerial, ref contactInfo))
                return;

            for (int i = 0; i < contactInfo.Count(); i++)
            {
                COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT tempContact = contactInfo[i];
                contactNameCombo.Items.Add(tempContact.contact_name);
            }

            if (contactInfo.Count() == 1)
                contactNameCombo.Items.GetItemAt(0);
        }

        private void InitializeProductTypeCombo()
        {
            if (!commonQueriesObject.GetCompanyProducts(ref products))
                return;
            for (int i = 0; i < products.Count(); i++)
            {
                COMPANY_WORK_MACROS.PRODUCT_STRUCT tempType;
                tempType = products[i];
                type1Combo.Items.Add(tempType.typeName);
                type2Combo.Items.Add(tempType.typeName);
                type3Combo.Items.Add(tempType.typeName);
                type4Combo.Items.Add(tempType.typeName);
            }      
        }

        private void InitializeProductBrandCombo()
        {
            if (!commonQueriesObject.GetCompanyBrands(ref brands))
                return;
            for (int i = 0; i < brands.Count(); i++)
            {
                COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand = brands[i];
                brand1Combo.Items.Add(tempBrand.brandName);
                brand2Combo.Items.Add(tempBrand.brandName);
                brand3Combo.Items.Add(tempBrand.brandName);
                brand4Combo.Items.Add(tempBrand.brandName);
            }   
        }

        private void InitializeModelCombo()
        {
            COMPANY_WORK_MACROS.PRODUCT_STRUCT tempProduct = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
            COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand = new COMPANY_WORK_MACROS.BRAND_STRUCT();

            tempProduct = products[type1Combo.SelectedIndex];
            tempBrand = brands[brand1Combo.SelectedIndex];

            if (!commonQueriesObject.GetCompanyModels(tempProduct, tempBrand, ref models))
                return;

            for (int i = 0; i < models.Count(); i++)
            {
                COMPANY_WORK_MACROS.MODEL_STRUCT temp = models[i];
                model1Combo.Items.Add(temp.modelName);
            }
        }

        private void InitializeCurrencyComboBoxes()
        {
            pricePerItem1ComboBox.Items.Add("Dollar");
            pricePerItem1ComboBox.Items.Add("Euro");
            pricePerItem1ComboBox.Items.Add("EGP");

            pricePerItem2ComboBox.Items.Add("Dollar");
            pricePerItem2ComboBox.Items.Add("Euro");
            pricePerItem2ComboBox.Items.Add("EGP");

            pricePerItem3ComboBox.Items.Add("Dollar");
            pricePerItem3ComboBox.Items.Add("Euro");
            pricePerItem3ComboBox.Items.Add("EGP");

            pricePerItem4ComboBox.Items.Add("Dollar");
            pricePerItem4ComboBox.Items.Add("Euro");
            pricePerItem4ComboBox.Items.Add("EGP");

            totalPriceCombo.Items.Add("Dollar");
            totalPriceCombo.Items.Add("Euro");
            totalPriceCombo.Items.Add("EGP");
        }               

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

        private void InitializePeriodComboBoxes()
        {
            drawingSubmissionDeadlineCombo.Items.Add("Days");
            drawingSubmissionDeadlineCombo.Items.Add("Weeks");
            drawingSubmissionDeadlineCombo.Items.Add("Months");
            drawingSubmissionDeadlineCombo.Items.Add("Years");

            deliveryTimeCombo.Items.Add("Days");
            deliveryTimeCombo.Items.Add("Weeks");
            deliveryTimeCombo.Items.Add("Months");
            deliveryTimeCombo.Items.Add("Years");

            warrantyPeriodCombo.Items.Add("Days");
            warrantyPeriodCombo.Items.Add("Weeks");
            warrantyPeriodCombo.Items.Add("Months");
            warrantyPeriodCombo.Items.Add("Years");

            offerValidityCombo.Items.Add("Days");
            offerValidityCombo.Items.Add("Weeks");
            offerValidityCombo.Items.Add("Months");
            offerValidityCombo.Items.Add("Years");
        }

        private decimal GetPercentage(int percentage, int total)
        {
            decimal percentileValue;
            decimal temp = percentage / 100;
            percentileValue = total * temp;
            return percentileValue;
        }
        private bool GetCompaniesQuery(int mEmployeeSerial, ref List<Company_Struct> returnVector)
        {
            returnVector.Clear();

            string sqlQuery = "SELECT company_serial,company_name FROM erp_system.dbo.company_name WHERE added_by = " + mEmployeeSerial;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!commonQueriesSqlObject.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < commonQueriesSqlObject.rows.Count; i++)
            {
                Company_Struct temp = new Company_Struct();

                temp.companySerial = commonQueriesSqlObject.rows[i].sql_int[0];
                temp.companyName = commonQueriesSqlObject.rows[i].sql_string[0];

                returnVector.Add(temp);
            }
            return true;
        }

        private void SalesPersonComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            rfqSerialCombo.Items.Clear();
            companyNameCombo.Items.Clear();
            
            int tempSalesPersonIndex = salesPersonCombo.SelectedIndex;
            salesPersonID = employeesList[tempSalesPersonIndex].employee_id;

            if (!commonQueriesObject.GetEmployeeTeam(salesPersonID, ref salesPersonTeamID))
                return;

            if(salesPersonTeamID == COMPANY_ORGANISATION_MACROS.SALES_TEAM_ID)
                InitializeRFQSerialCombo();
            
            else
                InitializeCompanyNameCombo();

            EnableUIElements();
        }

        private void rfqSerialComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            rfqSerialID = rfqSerialCombo.SelectedIndex;
        }

        private void CompanyNameComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            companyBranchCombo.Items.Clear();

            if (companyNameCombo.SelectedItem != null)
                InitializeCompanyBranchCombo();

            companyNameID = companyBranchCombo.SelectedIndex;
        }

        private void CompanyBranchComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            contactNameCombo.Items.Clear();

            if (companyBranchCombo.SelectedItem != null)
                InitializeCompanyContactCombo();

            companyBranchID = companyBranchCombo.SelectedIndex;
        }

        private void contactNameComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            contactNameID = contactNameCombo.SelectedIndex;
        }

        private void Type1ComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            model1Combo.Items.Clear();

            if (type1Combo.SelectedItem != null && brand1Combo.SelectedItem != null)
                InitializeModelCombo();
            type1ID = type1Combo.SelectedIndex;
        }

        private void Brand1ComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            model1Combo.Items.Clear();

            if (type1Combo.SelectedItem != null && brand1Combo.SelectedItem != null)
                InitializeModelCombo();
            brand1ID = brand1Combo.SelectedIndex;
        }
        private void Model1ComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            model1ID = model1Combo.SelectedIndex;
        }
        private void Quantity1TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if(IntegrityChecks.CheckInvalidCharacters(quantity1TextBox.Text, BASIC_MACROS.PHONE_STRING) && quantity1TextBox.Text != "")
                quantity1 = int.Parse(quantity1TextBox.Text);
            else
            {
               // //MessageBox.Show("Invalid Character Enterred");
                quantity1 = 0;
                quantity1TextBox.Text = null;
            }
            price1Total = (quantity1 * price1);
            totalPrice = price1Total + price2Total + price3Total + price4Total;
            totalPriceTextBox.Text = totalPrice.ToString();
        }
        private void PricePerItem1ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            price1Type = pricePerItem1ComboBox.Text;
        }

        private void PricePerItem1TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(pricePerItem1TextBox.Text, BASIC_MACROS.PHONE_STRING) && pricePerItem1TextBox.Text != "")
                price1 = int.Parse(pricePerItem1TextBox.Text);
            else
            {
                ////MessageBox.Show("Invalid Character Enterred");
                price1 = 0;
                pricePerItem1TextBox.Text = null;
            }
            price1Total = (quantity1 * price1);
            totalPrice = price1Total + price2Total + price3Total + price4Total;
            totalPriceTextBox.Text = totalPrice.ToString();
        }
        private void Type2ComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            model2Combo.Items.Clear();

            if (type2Combo.SelectedItem != null && brand2Combo.SelectedItem != null)
                InitializeModelCombo();
            type2ID = type2Combo.SelectedIndex;
        }

        private void Brand2ComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            model2Combo.Items.Clear();

            if (type2Combo.SelectedItem != null && brand2Combo.SelectedItem != null)
                InitializeModelCombo();
            brand2ID = brand2Combo.SelectedIndex;
        }
        private void Model2ComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            model2ID = model2Combo.SelectedIndex;
        }

        private void Quantity2TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(quantity2TextBox.Text, BASIC_MACROS.PHONE_STRING) && quantity2TextBox.Text != "")
                quantity2 = int.Parse(quantity2TextBox.Text);
            else
            {
                ////MessageBox.Show("Invalid Character Enterred");
                quantity2 = 0;
                quantity2TextBox.Text = null;
            }
            price2Total = (quantity2 * price2);
            totalPrice = price1Total + price2Total + price3Total + price4Total;
            totalPriceTextBox.Text = totalPrice.ToString();
        }

        private void PricePerItem2ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            price2Type = pricePerItem2ComboBox.Text;
        }

        private void PricePerItem2TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(pricePerItem2TextBox.Text, BASIC_MACROS.PHONE_STRING) && pricePerItem2TextBox.Text != "")
                    price2 = int.Parse(pricePerItem2TextBox.Text);
            else
            {
                ////MessageBox.Show("Invalid Character Enterred");
                price2 = 0;
                pricePerItem2TextBox.Text = null;
            }
            price2Total = (quantity2 * price2);
            totalPrice = price1Total + price2Total + price3Total + price4Total;
            totalPriceTextBox.Text = totalPrice.ToString();
        }

        private void Type3ComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            model3Combo.Items.Clear();

            if (type3Combo.SelectedItem != null && brand3Combo.SelectedItem != null)
                InitializeModelCombo();
            type3ID = type3Combo.SelectedIndex;
        }

        private void Brand3ComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            model3Combo.Items.Clear();

            if (type3Combo.SelectedItem != null && brand3Combo.SelectedItem != null)
                InitializeModelCombo();
            brand3ID = brand3Combo.SelectedIndex;
        }
        private void Model3ComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            model3ID = model3Combo.SelectedIndex;
        }

        private void Quantity3TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(quantity3TextBox.Text, BASIC_MACROS.PHONE_STRING) && quantity3TextBox.Text != "")
                quantity3 = int.Parse(quantity3TextBox.Text);
            else
            {
               // //MessageBox.Show("Invalid Character Enterred");
                quantity3 = 0;
                quantity3TextBox.Text = null;
            }
            price3Total = (quantity3 * price3);
            totalPrice = price1Total + price2Total + price3Total + price4Total;
            totalPriceTextBox.Text = totalPrice.ToString();
        }

        private void PricePerItem3ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            price3Type = pricePerItem3ComboBox.Text;
        }

        private void PricePerItem3TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(pricePerItem3TextBox.Text, BASIC_MACROS.PHONE_STRING) && pricePerItem3TextBox.Text != "")
                price3 = int.Parse(pricePerItem3TextBox.Text);
            else
            {
                ////MessageBox.Show("Invalid Character Enterred");
                price3 = 0;
                pricePerItem3TextBox.Text = null;
            }
            price3Total = (quantity3 * price3);
            totalPrice = price1Total + price2Total + price3Total + price4Total;
            totalPriceTextBox.Text = totalPrice.ToString();
        }

        private void Type4ComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            model4Combo.Items.Clear();

            if (type4Combo.SelectedItem != null && brand4Combo.SelectedItem != null)
                InitializeModelCombo();
            type4ID = type4Combo.SelectedIndex;
        }

        private void Brand4ComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            model4Combo.Items.Clear();

            if (type4Combo.SelectedItem != null && brand4Combo.SelectedItem != null)
                InitializeModelCombo();
            brand4ID = brand4Combo.SelectedIndex;
        }
        private void Model4ComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            model4ID = model4Combo.SelectedIndex;
        }

        private void Quantity4TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(quantity4TextBox.Text, BASIC_MACROS.PHONE_STRING) && quantity4TextBox.Text != "")
                quantity4 = int.Parse(quantity4TextBox.Text);
            else
            {
               // //MessageBox.Show("Invalid Character Enterred");
                quantity4 = 0;
                quantity4TextBox.Text = null;
            }
            price4Total = (quantity4 * price4);
            totalPrice = price1Total + price2Total + price3Total + price4Total;
            totalPriceTextBox.Text = totalPrice.ToString();
        }

        private void PricePerItem4ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            price4Type = pricePerItem4ComboBox.Text;
        }

        private void PricePerItem4TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(pricePerItem4TextBox.Text, BASIC_MACROS.PHONE_STRING) && pricePerItem4TextBox.Text != "")
                price4 = int.Parse(pricePerItem4TextBox.Text);
            else
            {
               // //MessageBox.Show("Invalid Character Enterred");
                price4 = 0;
                pricePerItem4TextBox.Text = null;
            }
            price4Total = (quantity4 * price4);
            totalPrice = price1Total + price2Total + price3Total + price4Total;
            totalPriceTextBox.Text = totalPrice.ToString();
        }
        private void DownPaymentPercentageTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IntegrityChecks.CheckInvalidCharacters(downPaymentPercentageTextBox.Text, BASIC_MACROS.PHONE_STRING) && downPaymentPercentageTextBox.Text != "")
                downPaymentPercentage = int.Parse(downPaymentPercentageTextBox.Text);
            else
            {
                downPaymentPercentage = 0;
                downPaymentPercentageTextBox.Text = null;
            }
            decimal temp = GetPercentage(downPaymentPercentage, totalPrice);
            downPaymentActualTextBox.Text = temp.ToString();
        }

        private void OnDeliveryPercentageTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

        private void OnInstallationPercentageTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Product2CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            type2Combo.IsEnabled = true;
            brand2Combo.IsEnabled = true;
            model2Combo.IsEnabled = true;
            quantity2TextBox.IsEnabled = true;
            pricePerItem2TextBox.IsEnabled = true;
            pricePerItem2ComboBox.IsEnabled = true;
        }

        private void Product2CheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            type2Combo.IsEnabled = false;
            brand2Combo.IsEnabled = false;
            model2Combo.IsEnabled = false;
            quantity2TextBox.IsEnabled = false;
            pricePerItem2TextBox.IsEnabled = false;
            pricePerItem2ComboBox.IsEnabled = false;

            type2Combo.SelectedItem = null;
            brand2Combo.SelectedItem = null;
            model2Combo.SelectedItem = null;
            quantity2TextBox.Text = 0.ToString();
            pricePerItem2TextBox.Text = 0.ToString();
            pricePerItem2ComboBox.SelectedItem = null;

            product3CheckBox.IsChecked = false;
            product4CheckBox.IsChecked = false;
        }

        private void Product3CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            if(product2CheckBox.IsChecked == true)
            {
                type3Combo.IsEnabled = true;
                brand3Combo.IsEnabled = true;
                model3Combo.IsEnabled = true;
                quantity3TextBox.IsEnabled = true;
                pricePerItem3TextBox.IsEnabled = true;
                pricePerItem3ComboBox.IsEnabled = true;
            }
        }

        private void Product3CheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            type3Combo.IsEnabled = false;
            brand3Combo.IsEnabled = false;
            model3Combo.IsEnabled = false;
            quantity3TextBox.IsEnabled = false;
            pricePerItem3TextBox.IsEnabled = false;
            pricePerItem3ComboBox.IsEnabled = false;

            type3Combo.SelectedItem = null;
            brand3Combo.SelectedItem = null;
            model3Combo.SelectedItem = null;
            quantity3TextBox.Text = 0.ToString();
            pricePerItem3TextBox.Text = 0.ToString();
            pricePerItem3ComboBox.SelectedItem = null;

            product4CheckBox.IsChecked = false;
        }

        private void Product4CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            if (product2CheckBox.IsChecked == true && product3CheckBox.IsChecked == true)
            {
                type4Combo.IsEnabled = true;
                brand4Combo.IsEnabled = true;
                model4Combo.IsEnabled = true;
                quantity4TextBox.IsEnabled = true;
                pricePerItem4TextBox.IsEnabled = true;
                pricePerItem4ComboBox.IsEnabled = true;
            }
        }

        private void Product4CheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            type4Combo.IsEnabled = false;
            brand4Combo.IsEnabled = false;
            model4Combo.IsEnabled = false;
            quantity4TextBox.IsEnabled = false;
            pricePerItem4TextBox.IsEnabled = false;
            pricePerItem4ComboBox.IsEnabled = false;

            type4Combo.SelectedItem = null;
            brand4Combo.SelectedItem = null;
            model4Combo.SelectedItem = null;
            quantity4TextBox.Text = 0.ToString();
            pricePerItem4TextBox.Text = 0.ToString();
            pricePerItem4ComboBox.SelectedItem = null;
        }

        private void DrawingConditionsCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            drawingSubmissionDeadlineFrom.IsEnabled = true;
            drawingSubmissionDeadlineTo.IsEnabled = true;
            drawingSubmissionDeadlineCombo.IsEnabled = true;
        }

        private void DrawingConditionsCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            drawingSubmissionDeadlineFrom.IsEnabled = false;
            drawingSubmissionDeadlineTo.IsEnabled = false;
            drawingSubmissionDeadlineCombo.IsEnabled = false;

            drawingSubmissionDeadlineFrom.Text = "0";
            drawingSubmissionDeadlineTo.Text = "0";
            drawingSubmissionDeadlineCombo.SelectedItem = null;
        }

       
    }
}
