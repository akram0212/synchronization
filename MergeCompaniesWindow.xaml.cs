using _01electronics_library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for MergeCompaniesWindow.xaml
    /// </summary>
    public partial class MergeCompaniesWindow : Window
    {
        protected Employee loggedInUser;

        protected CommonQueries commonQueries;
        protected IntegrityChecks integrityChecker;

        protected Company company;

        COMPANY_ORGANISATION_MACROS.COMPANY_LIST_STRUCT selectedCompany;

        List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> listOfEmployees;
        List<COMPANY_ORGANISATION_MACROS.COMPANY_STRUCT> employeeCompanies;
        List<BASIC_STRUCTS.PRIMARY_FIELD_STRUCT> primaryWorkFields;
        List<BASIC_STRUCTS.SECONDARY_FIELD_STRUCT> secondaryWorkFields;
        List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_MIN_STRUCT> salesPeople;
        List<KeyValuePair<int, int>> updatedObjects;
        List<int> branches;
        int newSalesPersonId;

        protected String errorMessage;

        public MergeCompaniesWindow(ref Employee mLoggedInUser, ref Company mCompany)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;
            company = mCompany;

            commonQueries = new CommonQueries();
            integrityChecker = new IntegrityChecks();

            selectedCompany = new COMPANY_ORGANISATION_MACROS.COMPANY_LIST_STRUCT();
            listOfEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
            employeeCompanies = new List<COMPANY_ORGANISATION_MACROS.COMPANY_STRUCT>();
            primaryWorkFields = new List<BASIC_STRUCTS.PRIMARY_FIELD_STRUCT>();
            secondaryWorkFields = new List<BASIC_STRUCTS.SECONDARY_FIELD_STRUCT>();
            salesPeople = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_MIN_STRUCT>();
            updatedObjects = new List<KeyValuePair<int, int>>();
            branches = new List<int>();

            initializeSecondComapnyCombo();
            initializePrimaryWorkFieldCombo();
            initializeSecondaryWorkFieldCombo(company.GetCompanyPrimaryFieldId());
            initializeSalesPersonCombo();

            //newSalesPersonId = company.GetOwnerUserId();
            CompanyNameTextBox.Text = company.GetCompanyName();

            primaryWorkFieldComboBox.Text = company.GetCompanyPrimaryField();
            secondaryWorkFieldComboBox.Text = company.GetCompanySecondaryField();
        }

        private void OnSelCompanyToBeMergedComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (!commonQueries.GetCompanyAddedBy(employeeCompanies[companyToBeMergedComboBox.SelectedIndex].company_serial, ref salesPeople))
                return;

            initializeSalesPersonCombo();

            if (salesPeople[0].employee_name != salesPeople[1].employee_name)
                salesPersonComboBox.Items.Add(salesPeople[1].employee_name);

        }

        private void OnSelprimaryWorkFieldComboBox(object sender, SelectionChangedEventArgs e)
        {
            initializeSecondaryWorkFieldCombo(primaryWorkFields[primaryWorkFieldComboBox.SelectedIndex].field_id);
            secondaryWorkFieldComboBox.SelectedIndex = 0;
        }

        private void OnSelSecondaryWorkFieldComboBox(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnSelSalesPersonComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (salesPersonComboBox.SelectedIndex == 0 && salesPeople.Count > 1)
                newSalesPersonId = salesPeople[1].employee_id;

            else if(salesPeople.Count > 1)
                newSalesPersonId = salesPeople[0].employee_id;
        }

        private void OnDoubleClickLabel(object sender, MouseButtonEventArgs e)
        {
        }

        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {
            //if (!CheckCompanyNameEditBox())
              //  return;
            if (!CheckCompanyToBeMergedComboBox())
                return;
            if (!CheckPrimaryWorkFieldComboBox())
                return;
            if (!CheckSecondaryWorkFieldComboBox())
                return;
            if (!CheckSalesPersonComboBox())
                return;

            if (!company.UpdateCompanyName())
                return;
            if (!company.UpdateCompanyWorkField())
                return;


            //if (!company.UpdateCompanyOwnerUser(company.GetOwnerUserId(), employeeCompanies[companyToBeMergedComboBox.SelectedIndex].company_serial))
            //    return;


            if (salesPeople[salesPersonComboBox.SelectedIndex].employee_id != company.GetOwnerUserId())
            {
                if (!commonQueries.MergeCompanyContactsInfo(employeeCompanies[companyToBeMergedComboBox.SelectedIndex].company_serial, company.GetOwnerUserId(), salesPeople[salesPersonComboBox.SelectedIndex].employee_id))
                    return;

                if (!UpdateBranches())
                    return;

                if (!company.UpdateCompanyOwnerUser(company.GetOwnerUserId(), company.GetCompanySerial()))
                    return;

                if (!company.DeleteCompanyField(employeeCompanies[companyToBeMergedComboBox.SelectedIndex].company_serial))
                    return;
                if (!company.DeleteCompany(employeeCompanies[companyToBeMergedComboBox.SelectedIndex].company_serial))
                    return;
            }
            else
            {
                if (newSalesPersonId != salesPeople[salesPersonComboBox.SelectedIndex].employee_id)
                {
                    if (!commonQueries.MergeCompanyContactsInfo(employeeCompanies[companyToBeMergedComboBox.SelectedIndex].company_serial, company.GetOwnerUserId(), newSalesPersonId))
                        return;

                    if (!UpdateBranches())
                        return;

                    if (!company.UpdateCompanyOwnerUser(salesPeople[salesPersonComboBox.SelectedIndex].employee_id, company.GetCompanySerial()))
                        return;

                    if (!company.DeleteCompanyField(employeeCompanies[companyToBeMergedComboBox.SelectedIndex].company_serial))
                        return;
                    if (!company.DeleteCompany(employeeCompanies[companyToBeMergedComboBox.SelectedIndex].company_serial))
                        return;

                    if (!commonQueries.MergeCompanyContactsInfo(company.GetCompanySerial(),  newSalesPersonId, company.GetOwnerUserId()))
                        return;
                }
                else
                {
                    if (!commonQueries.MergeCompanyContactsInfo(employeeCompanies[companyToBeMergedComboBox.SelectedIndex].company_serial, newSalesPersonId, company.GetOwnerUserId()))
                        return;

                    if (!UpdateBranches())
                        return;

                    if (!company.UpdateCompanyOwnerUser(company.GetOwnerUserId(), company.GetCompanySerial()))
                        return;

                    if (!company.DeleteCompanyField(employeeCompanies[companyToBeMergedComboBox.SelectedIndex].company_serial))
                        return;
                    if (!company.DeleteCompany(employeeCompanies[companyToBeMergedComboBox.SelectedIndex].company_serial))
                        return;
                }
            }

            this.Close();
        }

        private void OnTextChangedCompanyName(object sender, TextChangedEventArgs e)
        {

        }
        private bool initializeSecondComapnyCombo()
        {
            if (!commonQueries.GetCompaniesSerialsAndNames(ref employeeCompanies))
                return false;

            for (int i = 0; i < employeeCompanies.Count; i++)
            {
                if(employeeCompanies[i].company_serial != company.GetCompanySerial())
                {
                    companyToBeMergedComboBox.Items.Add(employeeCompanies[i].company_name);
                }
                else
                {
                    ComboBoxItem comboBoxItem = new ComboBoxItem();
                    comboBoxItem.Content = employeeCompanies[i].company_name;
                    comboBoxItem.IsEnabled = false;
                    companyToBeMergedComboBox.Items.Add(comboBoxItem);
                }

            }

            return true;
        } 
        private bool initializePrimaryWorkFieldCombo()
        {
            if (!commonQueries.GetPrimaryWorkFields(ref primaryWorkFields))
                return false;

            for (int i = 0; i < primaryWorkFields.Count; i++)
            {
                primaryWorkFieldComboBox.Items.Add(primaryWorkFields[i].field_name);
            }

            return true;
        }
        private bool initializeSecondaryWorkFieldCombo(int primaryFieldID)
        {
            secondaryWorkFields.Clear();
            secondaryWorkFieldComboBox.Items.Clear();

            if (!commonQueries.GetSecondaryWorkFields(primaryFieldID, ref secondaryWorkFields))
                return false;

            for (int i = 0; i < secondaryWorkFields.Count; i++)
            {
                secondaryWorkFieldComboBox.Items.Add(secondaryWorkFields[i].field_name);
            }

            return true;
        }
        private void initializeSalesPersonCombo()
        {
            salesPersonComboBox.Items.Clear();

            COMPANY_ORGANISATION_MACROS.EMPLOYEE_MIN_STRUCT tmp = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_MIN_STRUCT();
            tmp.employee_id = company.GetOwnerUserId();
            tmp.employee_name = company.GetOwnerUser();

            salesPeople.Add(tmp);
            salesPersonComboBox.Items.Add(salesPeople[0].employee_name);
            salesPersonComboBox.SelectedIndex = 0;
        }
        private bool CheckCompanyNameEditBox()
        {
            String inputString = CompanyNameTextBox.Text;
            String outputString = CompanyNameTextBox.Text;

            if (!integrityChecker.CheckCompanyNameEditBox(inputString, ref outputString, true, ref errorMessage))
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            company.SetCompanyName(outputString);
            CompanyNameTextBox.Text = company.GetCompanyName();

            return true;
        }
        private bool CheckCompanyToBeMergedComboBox()
        {
            if (companyToBeMergedComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Company to be merged must be specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
        private bool CheckPrimaryWorkFieldComboBox()
        {
            if (primaryWorkFieldComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Company's Primary Work Field must be specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }

            company.SetCompanyPrimaryField(primaryWorkFields[primaryWorkFieldComboBox.SelectedIndex].field_id, primaryWorkFieldComboBox.SelectedItem.ToString());

            return true;
        }
        private bool CheckSecondaryWorkFieldComboBox()
        {
            if (secondaryWorkFieldComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Company's Secondary Work Field must be specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }

            company.SetCompanySecondaryField(secondaryWorkFields[secondaryWorkFieldComboBox.SelectedIndex].field_id, secondaryWorkFieldComboBox.SelectedItem.ToString());

            return true;
        }
        private bool CheckSalesPersonComboBox()
        {
            if (salesPersonComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Sales Person must be specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }

            company.SetOwnerUser(salesPeople[salesPersonComboBox.SelectedIndex].employee_id, salesPersonComboBox.SelectedItem.ToString());

            return true;
        } 
        private bool UpdateBranches()
        {
            if (!commonQueries.GetCompanyBranches(employeeCompanies[companyToBeMergedComboBox.SelectedIndex].company_serial, ref branches))
                return false;

            for (int i = 0; i < branches.Count; i++)
            {
                if (!company.UpdateCompanyBranch(branches[i]))
                    return false;
            }

            return true;
        }
        
    }
}
