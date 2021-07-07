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
    /// Interaction logic for AddRFQWindow.xaml
    /// </summary>
    public partial class AddRFQWindow : Window
    {
        private Employee loggedInUser;
        private RFQ rfqObject;


        private List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> rfqsList;
       

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;

        public struct Company_Struct
        {
           public int companySerial;
           public String companyName;
        };

        private List<Company_Struct> companyInfo = new List<Company_Struct>();
        private List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> branchInfo = new List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT> contactInfo = new List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT>();
        private List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> products = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();
        private List<COMPANY_WORK_MACROS.BRAND_STRUCT> brands = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();
        private List<COMPANY_WORK_MACROS.MODEL_STRUCT> models = new List<COMPANY_WORK_MACROS.MODEL_STRUCT>();
        private List<BASIC_STRUCTS.CONTRACT_STRUCT> contractTypes = new List<BASIC_STRUCTS.CONTRACT_STRUCT>();
        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> preSalesEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
        public AddRFQWindow(ref Employee mLoggedInUser)
        {
            InitializeComponent();
            
            loggedInUser = mLoggedInUser;

            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();
            sqlDatabase = new SQLServer();

            rfqObject = new RFQ(sqlDatabase);

            InitializeWindow();

            
            if (!GetCompaniesQuery(loggedInUser.GetEmployeeId(), ref companyInfo))
                return;
            
            for(int i = 0; i < companyInfo.Count; i++)
            {
                String tempName = companyInfo[i].companyName;
                companyNameCombo.Items.Add(tempName);
            }

            FillProductCombo();
            FillBrandCombo();
            FillQuantityCombo();
            FillContractTypeCombo();
            FillAssignedToCombo();
        }

        void InitializeWindow()
        {
            companyBranchCombo.IsEnabled = false;
            contactNameCombo.IsEnabled = false;
            
            product1TypeCombo.IsEnabled = false;
            product1BrandCombo.IsEnabled = false;
            product1ModelCombo.IsEnabled = false;
            product1QuantityCombo.IsEnabled = false;

            product2TypeCombo.IsEnabled = false;
            product2BrandCombo.IsEnabled = false;
            product2ModelCombo.IsEnabled = false;
            product2QuantityCombo.IsEnabled = false;

            product3TypeCombo.IsEnabled = false;
            product3BrandCombo.IsEnabled = false;
            product3ModelCombo.IsEnabled = false;
            product3QuantityCombo.IsEnabled = false;

            product4TypeCombo.IsEnabled = false;
            product4BrandCombo.IsEnabled = false;
            product4ModelCombo.IsEnabled = false;
            product4QuantityCombo.IsEnabled = false;

            product3CheckBox.IsEnabled = false;
            product4CheckBox.IsEnabled = false;

            contractTypeCombo.IsEnabled = false;
            assignedToCombo.IsEnabled = false;
            
        }
        private void CompanyNameComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            companyBranchCombo.Items.Clear();

            if(companyNameCombo.SelectedItem != null)
            {
                companyBranchCombo.IsEnabled = true;

                int companySerial = companyNameCombo.SelectedIndex + 1;

                if (!commonQueriesObject.GetCompanyAddresses(companySerial, ref branchInfo))
                    return;
                
                for(int i = 0; i < branchInfo.Count; i++)
                {
                    String address;
                    address = branchInfo[i].district + ", " + branchInfo[i].city + ", " + branchInfo[i].state_governorate + ", " + branchInfo[i].country + ".";
                    companyBranchCombo.Items.Add(address);
                }

                if (branchInfo.Count == 1)
                    companyBranchCombo.SelectedItem = companyBranchCombo.Items.GetItemAt(0);
                
                product1TypeCombo.IsEnabled = true;
                product1BrandCombo.IsEnabled = true;
                product1ModelCombo.IsEnabled = true;
                product1QuantityCombo.IsEnabled = true;
            }
        }

        private void CompanyBranchComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            contactNameCombo.Items.Clear();

            if (companyBranchCombo.SelectedItem != null)
            {
                contactNameCombo.IsEnabled = true;
                int temp = companyBranchCombo.SelectedIndex;
                int addressSerial = branchInfo[temp].address_serial;
                
                if (!commonQueriesObject.GetCompanyContacts(loggedInUser.GetEmployeeId(), addressSerial, ref contactInfo))
                    return;
                
                for(int i = 0; i < contactInfo.Count(); i++)
                {
                    COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT tempContact = contactInfo[i];
                    contactNameCombo.Items.Add(tempContact.contact_name);
                }

                if (contactInfo.Count() == 1)
                    contactNameCombo.Items.GetItemAt(0);
            }
        }

        private void ContactNameComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void Product1TypeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            product1ModelCombo.Items.Clear();

            if (product1TypeCombo.SelectedItem != null)
            {
                contractTypeCombo.IsEnabled = true;
                if(product1BrandCombo.SelectedItem != null)
                {
                    COMPANY_WORK_MACROS.PRODUCT_STRUCT tempProduct = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
                    COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand = new COMPANY_WORK_MACROS.BRAND_STRUCT();

                    tempProduct = products[product1TypeCombo.SelectedIndex];
                    tempBrand = brands[product1BrandCombo.SelectedIndex];

                    if (!commonQueriesObject.GetCompanyModels(tempProduct, tempBrand, ref models))
                        return;

                    for (int i = 0; i < models.Count(); i++)
                    {
                        COMPANY_WORK_MACROS.MODEL_STRUCT temp = models[i];
                        product1ModelCombo.Items.Add(temp.modelName);
                    }
                }
            }
        }

        private void Product1BrandComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            product1ModelCombo.Items.Clear();

            if (product1TypeCombo.SelectedItem != null)
            {
                COMPANY_WORK_MACROS.PRODUCT_STRUCT tempProduct = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
                COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand = new COMPANY_WORK_MACROS.BRAND_STRUCT();

                tempProduct = products[product1TypeCombo.SelectedIndex];
                tempBrand = brands[product1BrandCombo.SelectedIndex];

                if (!commonQueriesObject.GetCompanyModels(tempProduct, tempBrand, ref models))
                    return;

                for (int i = 0; i < models.Count(); i++)
                {
                    COMPANY_WORK_MACROS.MODEL_STRUCT temp = models[i];
                    product1ModelCombo.Items.Add(temp.modelName);
                }
            }
        }

        private void Product1ModelComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Product1QuantityComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Product2TypeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            product2ModelCombo.Items.Clear();

            if (product2TypeCombo.SelectedItem != null)
            {
                if (product2BrandCombo.SelectedItem != null)
                {
                    COMPANY_WORK_MACROS.PRODUCT_STRUCT tempProduct = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
                    COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand = new COMPANY_WORK_MACROS.BRAND_STRUCT();

                    tempProduct = products[product2TypeCombo.SelectedIndex];
                    tempBrand = brands[product2BrandCombo.SelectedIndex];

                    if (!commonQueriesObject.GetCompanyModels(tempProduct, tempBrand, ref models))
                        return;

                    for (int i = 0; i < models.Count(); i++)
                    {
                        COMPANY_WORK_MACROS.MODEL_STRUCT temp = models[i];
                        product1ModelCombo.Items.Add(temp.modelName);
                    }
                }
            }
        }

        private void Product2BrandComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            product2ModelCombo.Items.Clear();

            if (product2TypeCombo.SelectedItem != null)
            {
                COMPANY_WORK_MACROS.PRODUCT_STRUCT tempProduct = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
                COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand = new COMPANY_WORK_MACROS.BRAND_STRUCT();

                tempProduct = products[product2TypeCombo.SelectedIndex];
                tempBrand = brands[product2BrandCombo.SelectedIndex];

                if (!commonQueriesObject.GetCompanyModels(tempProduct, tempBrand, ref models))
                    return;

                for (int i = 0; i < models.Count(); i++)
                {
                    COMPANY_WORK_MACROS.MODEL_STRUCT temp = models[i];
                    product2ModelCombo.Items.Add(temp.modelName);
                }
            }
        }

        private void Product2ModelComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Product2QuantityComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Product3TypeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            product3ModelCombo.Items.Clear();

            if (product3TypeCombo.SelectedItem != null)
            { 
                if (product3BrandCombo.SelectedItem != null)
                {
                    COMPANY_WORK_MACROS.PRODUCT_STRUCT tempProduct = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
                    COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand = new COMPANY_WORK_MACROS.BRAND_STRUCT();

                    tempProduct = products[product3TypeCombo.SelectedIndex];
                    tempBrand = brands[product3BrandCombo.SelectedIndex];

                    if (!commonQueriesObject.GetCompanyModels(tempProduct, tempBrand, ref models))
                        return;

                    for (int i = 0; i < models.Count(); i++)
                    {
                        COMPANY_WORK_MACROS.MODEL_STRUCT temp = models[i];
                        product1ModelCombo.Items.Add(temp.modelName);
                    }
                }
            }
        }

        private void Product3BrandComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            product3ModelCombo.Items.Clear();

            if (product3TypeCombo.SelectedItem != null)
            {
                COMPANY_WORK_MACROS.PRODUCT_STRUCT tempProduct = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
                COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand = new COMPANY_WORK_MACROS.BRAND_STRUCT();

                tempProduct = products[product3TypeCombo.SelectedIndex];
                tempBrand = brands[product3BrandCombo.SelectedIndex];

                if (!commonQueriesObject.GetCompanyModels(tempProduct, tempBrand, ref models))
                    return;

                for (int i = 0; i < models.Count(); i++)
                {
                    COMPANY_WORK_MACROS.MODEL_STRUCT temp = models[i];
                    product3ModelCombo.Items.Add(temp.modelName);
                }
            }
        }

        private void Product3ModelComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Product3QuantityComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Product4TypeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            product4ModelCombo.Items.Clear();
            if (product4BrandCombo.SelectedItem != null)
            {
                COMPANY_WORK_MACROS.PRODUCT_STRUCT tempProduct = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
                COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand = new COMPANY_WORK_MACROS.BRAND_STRUCT();

                tempProduct = products[product4TypeCombo.SelectedIndex];
                tempBrand = brands[product4BrandCombo.SelectedIndex];

                if (!commonQueriesObject.GetCompanyModels(tempProduct, tempBrand, ref models))
                    return;

                for (int i = 0; i < models.Count(); i++)
                {
                    COMPANY_WORK_MACROS.MODEL_STRUCT temp = models[i];
                    product1ModelCombo.Items.Add(temp.modelName);
                }
            }
        }

        private void Product4BrandComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            product4ModelCombo.Items.Clear();

            if (product4TypeCombo.SelectedItem != null)
            {
                COMPANY_WORK_MACROS.PRODUCT_STRUCT tempProduct = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
                COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand = new COMPANY_WORK_MACROS.BRAND_STRUCT();

                tempProduct = products[product4TypeCombo.SelectedIndex];
                tempBrand = brands[product4BrandCombo.SelectedIndex];

                if (!commonQueriesObject.GetCompanyModels(tempProduct, tempBrand, ref models))
                    return;

                for (int i = 0; i < models.Count(); i++)
                {
                    COMPANY_WORK_MACROS.MODEL_STRUCT temp = models[i];
                    product4ModelCombo.Items.Add(temp.modelName);
                }
            }
        }

        private void Product4ModelComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Product4QuantityComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ContractTypeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(contractTypeCombo.SelectedItem != null)
            {
                assignedToCombo.IsEnabled = true;
            }
        }

        private void AssignedToComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void DeadlineDateComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Product4CheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            product4TypeCombo.IsEnabled = false;
            product4BrandCombo.IsEnabled = false;
            product4ModelCombo.IsEnabled = false;
            product4QuantityCombo.IsEnabled = false;
        }

        private void Product4CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            product4TypeCombo.IsEnabled = true;
            product4BrandCombo.IsEnabled = true;
            product4ModelCombo.IsEnabled = true;
            product4QuantityCombo.IsEnabled = true;
        }

        private void Product3CheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            if (product4CheckBox.IsChecked == false)
            {       
                product3TypeCombo.IsEnabled = false;
                product3BrandCombo.IsEnabled = false;
                product3ModelCombo.IsEnabled = false;
                product3QuantityCombo.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Please make sure that product 4 is unchecked first");
                product3CheckBox.IsChecked = true;
            }
        }

        private void Product3CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            product4CheckBox.IsEnabled = true;
            product3TypeCombo.IsEnabled = true;
            product3BrandCombo.IsEnabled = true;
            product3ModelCombo.IsEnabled = true;
            product3QuantityCombo.IsEnabled = true;
        }

        private void Product2CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            product3CheckBox.IsEnabled = true;
            product2TypeCombo.IsEnabled = true;
            product2BrandCombo.IsEnabled = true;
            product2ModelCombo.IsEnabled = true;
            product2QuantityCombo.IsEnabled = true;
            
        }

        private void Product2CheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            if (product3CheckBox.IsChecked == false)
            {
                if (product4CheckBox.IsChecked == false)
                {
                    product2TypeCombo.IsEnabled = false;
                    product2BrandCombo.IsEnabled = false;
                    product2ModelCombo.IsEnabled = false;
                    product2QuantityCombo.IsEnabled = false;
                }
            }
            else
            {
                MessageBox.Show("Please make sure that products 3 and 4 are unchecked first");
                product2CheckBox.IsChecked = true;
            }
        }

        public bool GetCompaniesQuery(int mEmployeeSerial, ref List<Company_Struct> returnVector)
        {
            returnVector.Clear();

            String sqlQuery = "SELECT company_serial,company_name FROM erp_system.dbo.company_name WHERE added_by = " + mEmployeeSerial;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                Company_Struct temp = new Company_Struct();

                temp.companySerial = sqlDatabase.rows[i].sql_int[0];
                temp.companyName = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(temp);
            }
            return true;
        }

        private void FillProductCombo()
        {
            if (!commonQueriesObject.GetCompanyProducts(ref products))
                return;
            for (int i = 0; i < products.Count(); i++)
            {
                COMPANY_WORK_MACROS.PRODUCT_STRUCT tempType;
                tempType = products[i];
                product1TypeCombo.Items.Add(tempType.typeName);
                product2TypeCombo.Items.Add(tempType.typeName);
                product3TypeCombo.Items.Add(tempType.typeName);
                product4TypeCombo.Items.Add(tempType.typeName);
            }
        }

        private void FillBrandCombo()
        {
            if (!commonQueriesObject.GetCompanyBrands(ref brands))
                return;
            for(int i = 0; i < brands.Count(); i++)
            {
                COMPANY_WORK_MACROS.BRAND_STRUCT tempBrand = brands[i];
                product1BrandCombo.Items.Add(tempBrand.brandName);
                product2BrandCombo.Items.Add(tempBrand.brandName);
                product3BrandCombo.Items.Add(tempBrand.brandName);
                product4BrandCombo.Items.Add(tempBrand.brandName);
            }
        }

        private void FillQuantityCombo()
        {
            for(int i = 1; i < 21; i++)
            {
                product1QuantityCombo.Items.Add(i);
                product2QuantityCombo.Items.Add(i);
                product3QuantityCombo.Items.Add(i);
                product4QuantityCombo.Items.Add(i);
            }
        }

        private void FillContractTypeCombo()
        {
            if (!commonQueriesObject.GetContractTypes(ref contractTypes))
                return;

            for (int i = 0; i < contractTypes.Count(); i++)
            {
                BASIC_STRUCTS.CONTRACT_STRUCT tempContractType = contractTypes[i];
                contractTypeCombo.Items.Add(tempContractType.contractName);
            }

        }

        private void FillAssignedToCombo()
        {
            if (!commonQueriesObject.GetTeamEmployees(COMPANY_ORGANISATION_MACROS.TECHNICAL_OFFICE_TEAM_ID, ref preSalesEmployees))
                return;

            for(int i = 0; i < preSalesEmployees.Count(); i++)
            {
                COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT temp = preSalesEmployees[i];
                assignedToCombo.Items.Add(temp.employee_name);
            }
        }

        private void InserQuerytRFQ()
        {
            String sqlQuery;
            int selectedIndex;

            DateTime tempIssueDate;

            DateTime tempDeadLineDate = DateTime.Parse(deadlineDateTextBlock.Text);

            int tempSalesPersonId = loggedInUser.GetEmployeeId();
            int tempRFQSerial;

            int tempRFQVersion = 1;

            selectedIndex = assignedToCombo.SelectedIndex;
            int tempAssigneeEngineerId = preSalesEmployees[selectedIndex].employee_id;

            int tempRFQId;

            selectedIndex = companyBranchCombo.SelectedIndex;
            int tempBranchSerial = branchInfo[selectedIndex].address_serial;

            selectedIndex = contactNameCombo.SelectedIndex;
            int tempContactId = contactInfo[selectedIndex].contact_id;

            selectedIndex = product1TypeCombo.SelectedIndex;
            int tempProduct1Type = products[selectedIndex].typeId;

            selectedIndex = product1BrandCombo.SelectedIndex;
            int tempProduct1Brand = brands[selectedIndex].brandId;

            selectedIndex = product1ModelCombo.SelectedIndex;
            int tempProduct1Model = models[selectedIndex].modelId;

            int tempProduct1Quantity = Int32.Parse(product1QuantityCombo.SelectedItem.ToString());


            selectedIndex = product2TypeCombo.SelectedIndex;
            int tempProduct2Type = products[selectedIndex].typeId;

            selectedIndex = product2BrandCombo.SelectedIndex;
            int tempProduct2Brand = brands[selectedIndex].brandId;

            selectedIndex = product2ModelCombo.SelectedIndex;
            int tempProduct2Model = models[selectedIndex].modelId;

            int tempProduct2Quantity = Int32.Parse(product2QuantityCombo.SelectedItem.ToString());

            selectedIndex = product3TypeCombo.SelectedIndex;
            int tempProduct3Type = products[selectedIndex].typeId;

            selectedIndex = product3BrandCombo.SelectedIndex;
            int tempProduct3Brand = brands[selectedIndex].brandId;

            selectedIndex = product3ModelCombo.SelectedIndex;
            int tempProduct3Model = models[selectedIndex].modelId;

            int tempProduct3Quantity = Int32.Parse(product3QuantityCombo.SelectedItem.ToString());

            selectedIndex = product4TypeCombo.SelectedIndex;
            int tempProduct4Type = products[selectedIndex].typeId;

            selectedIndex = product4BrandCombo.SelectedIndex;
            int tempProduct4Brand = brands[selectedIndex].brandId;

            selectedIndex = product4ModelCombo.SelectedIndex;
            int tempProduct4Model = models[selectedIndex].modelId;

            int tempProduct4Quantity = Int32.Parse(product4QuantityCombo.SelectedItem.ToString());

            selectedIndex = contractTypeCombo.SelectedIndex;
            int tempContractType = contractTypes[selectedIndex].contractId;

            int tempRFQStatus;
            String tempRFQNotes;

            sqlQuery = "INSERT INTO erp_system.dbo.rfqs VALUES ("+ commonFunctionsObject.GetTodaysDate() + "," + tempSalesPersonId + "," + "," + tempRFQVersion + "," + tempAssigneeEngineerId + "," + "," + tempBranchSerial + "," + tempContactId + "," + tempProduct1Type + "," + tempProduct1Brand + "," + tempProduct1Model + "," + tempProduct1Quantity + "," + tempProduct2Type + "," + tempProduct2Brand + "," + tempProduct2Model + "," + tempProduct2Quantity + "," + tempProduct3Type + "," + tempProduct3Brand + "," + tempProduct3Model + "," + tempProduct3Quantity + "," + tempProduct4Type + "," + tempProduct4Brand + "," + tempProduct4Model + "," + tempProduct4Quantity + "," + tempContractType + "," + tempDeadLineDate + "," + "," + ",";
        }

        private void ApplyButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
