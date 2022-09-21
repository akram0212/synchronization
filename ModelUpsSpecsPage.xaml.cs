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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for ModelUpsSpecsPage.xaml
    /// </summary>
    public partial class ModelUpsSpecsPage : Page
    {
        Employee loggedInUser;
        Product product;
        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private int viewAddCondition;

        public ModelAdditionalInfoPage modelAdditionalInfoPage;
        public ModelBasicInfoPage modelBasicInfoPage;
        public ModelUploadFilesPage modelUploadFilesPage;

        protected List<BASIC_STRUCTS.UPS_SPECS_STRUCT> UPSSpecs;

        public ModelUpsSpecsPage(ref Employee mLoggedInUser, ref Product mPrduct, int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            sqlDatabase = new SQLServer();
            UPSSpecs = new List<BASIC_STRUCTS.UPS_SPECS_STRUCT>();

            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            product = mPrduct;
            InitializeComponent();

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
            {

            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {

            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //////////BUTTON CLICKED///////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnBtnClickBack(object sender, RoutedEventArgs e)
        {
            modelBasicInfoPage.modelUpsSpecsPage = this;
            modelBasicInfoPage.modelAdditionalInfoPage = modelAdditionalInfoPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                modelBasicInfoPage.modelUploadFilesPage = modelUploadFilesPage;

            NavigationService.Navigate(modelBasicInfoPage);
        }

        private List<BASIC_STRUCTS.UPS_SPECS_STRUCT> GetUPSSpecs()
        {
            return UPSSpecs;
            
        }

        private void OnBtnClickNext(object sender, RoutedEventArgs e)
        {
            

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION )
            {
                BASIC_STRUCTS.UPS_SPECS_STRUCT tempUPSSpecs = new BASIC_STRUCTS.UPS_SPECS_STRUCT();
                tempUPSSpecs.spec_id = 1;
                tempUPSSpecs.io_phase = iOPhaseTextBox.Text.ToString();
                tempUPSSpecs.rated_power = ratedPowerTextBox.Text.ToString();
                // tempUPSSpecs.rating = ratingComboBox.SelectedItem.ToString();
                tempUPSSpecs.backup_time_50 = int.Parse(backupTime50TextBox.Text.ToString());
                tempUPSSpecs.backup_time_70 = int.Parse(backupTime70TextBox.Text.ToString());
                tempUPSSpecs.backup_time_100 = int.Parse(backupTime100TextBox.Text.ToString());
                tempUPSSpecs.input_power_factor = inputPowerFactorTextBox.ToString();
                tempUPSSpecs.thdi = thdiTextBox.Text.ToString();
                tempUPSSpecs.input_nominal_voltage = inputNominalVoltageTextBox.Text.ToString();
                tempUPSSpecs.input_voltage = inputVoltageTextBox.Text.ToString();
                tempUPSSpecs.voltage_tolerance = voltageToleranceTextBox.Text.ToString();
                tempUPSSpecs.output_power_factor = outputPowerFactorTextBox.Text.ToString();
                tempUPSSpecs.thdv = thdvTextBox.Text.ToString();
                tempUPSSpecs.output_nominal_voltage = outputNominalVoltageTextBox.Text.ToString();
                tempUPSSpecs.output_dc_voltage_range = outputDCVoltageRangeTextBox.Text.ToString();
                tempUPSSpecs.overload_capability = overloadCapabilityTextBox.Text.ToString();
                tempUPSSpecs.efficiency = efficiencyTextBox.Text.ToString();
                tempUPSSpecs.input_connection_type = inputConnectionTypeTextBox.Text.ToString();
                tempUPSSpecs.front_panel = frontPanelTextBox.Text.ToString();
                tempUPSSpecs.max_power = maxPowerTextBox.Text.ToString();
                tempUPSSpecs.certificates = certificatesTextBox.Text.ToString();
                tempUPSSpecs.safety = safetyTextBox.Text.ToString();
                tempUPSSpecs.emc = emcTextBox.Text.ToString();
                tempUPSSpecs.environmental_aspects = environmentalAspectsTextBox.Text.ToString();
                tempUPSSpecs.test_performance = testPerformanceTextBox.Text.ToString();
                tempUPSSpecs.protection_degree = protectionDegreeTextBox.Text.ToString();
                tempUPSSpecs.transfer_voltage_limit = transferVoltageLimitTextBox.Text.ToString();
                tempUPSSpecs.marking = markingTextBox.Text.ToString();
                tempUPSSpecs.is_valid = true;
                tempUPSSpecs.valid_until = (DateTime)validUntilDatePicker.SelectedDate;
                UPSSpecs.Clear();
                UPSSpecs.Add(tempUPSSpecs);
                product.UPSSpecs = UPSSpecs;

            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {

            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_UPDATE_CONDITION)
            {

            }





            modelAdditionalInfoPage.modelBasicInfoPage = modelBasicInfoPage;
            modelAdditionalInfoPage.modelUpsSpecsPage = this;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                modelAdditionalInfoPage.modelUploadFilesPage = modelUploadFilesPage;

            NavigationService.Navigate(modelAdditionalInfoPage);

        }
        private void OnBtnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            modelBasicInfoPage.modelUpsSpecsPage = this;
            modelBasicInfoPage.modelAdditionalInfoPage = modelAdditionalInfoPage;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                modelBasicInfoPage.modelUploadFilesPage = modelUploadFilesPage;

            NavigationService.Navigate(modelBasicInfoPage);
        }
        private void OnBtnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
            {
                BASIC_STRUCTS.UPS_SPECS_STRUCT tempUPSSpecs = new BASIC_STRUCTS.UPS_SPECS_STRUCT();
                tempUPSSpecs.spec_id = 1;
                tempUPSSpecs.io_phase = iOPhaseTextBox.Text.ToString();
                tempUPSSpecs.rated_power = ratedPowerTextBox.Text.ToString();
                // tempUPSSpecs.rating = ratingComboBox.SelectedItem.ToString();
                //tempUPSSpecs.backup_time_50 = int.Parse(backupTime50TextBox.ToString());
                //tempUPSSpecs.backup_time_70 = int.Parse(backupTime70TextBox.ToString());
                //tempUPSSpecs.backup_time_100 = int.Parse(backupTime100TextBox.ToString());
                tempUPSSpecs.input_power_factor = inputPowerFactorTextBox.ToString();
                tempUPSSpecs.thdi = thdiTextBox.Text.ToString();
                tempUPSSpecs.input_nominal_voltage = inputNominalVoltageTextBox.Text.ToString();
                tempUPSSpecs.input_voltage = inputVoltageTextBox.Text.ToString();
                tempUPSSpecs.voltage_tolerance = voltageToleranceTextBox.Text.ToString();
                tempUPSSpecs.output_power_factor = outputPowerFactorTextBox.Text.ToString();
                tempUPSSpecs.thdv = thdvTextBox.Text.ToString();
                tempUPSSpecs.output_nominal_voltage = outputNominalVoltageTextBox.Text.ToString();
                tempUPSSpecs.output_dc_voltage_range = outputDCVoltageRangeTextBox.Text.ToString();
                tempUPSSpecs.overload_capability = overloadCapabilityTextBox.Text.ToString();
                tempUPSSpecs.efficiency = efficiencyTextBox.Text.ToString();
                tempUPSSpecs.input_connection_type = inputConnectionTypeTextBox.Text.ToString();
                tempUPSSpecs.front_panel = frontPanelTextBox.Text.ToString();
                tempUPSSpecs.max_power = maxPowerTextBox.Text.ToString();
                tempUPSSpecs.certificates = certificatesTextBox.Text.ToString();
                tempUPSSpecs.safety = safetyTextBox.Text.ToString();
                tempUPSSpecs.emc = emcTextBox.Text.ToString();
                tempUPSSpecs.environmental_aspects = environmentalAspectsTextBox.Text.ToString();
                tempUPSSpecs.test_performance = testPerformanceTextBox.Text.ToString();
                tempUPSSpecs.protection_degree = protectionDegreeTextBox.Text.ToString();
                tempUPSSpecs.transfer_voltage_limit = transferVoltageLimitTextBox.Text.ToString();
                tempUPSSpecs.marking = markingTextBox.Text.ToString();
                tempUPSSpecs.is_valid = true;
                tempUPSSpecs.valid_until = (DateTime)validUntilDatePicker.SelectedDate;
                UPSSpecs.Clear();
                UPSSpecs.Add(tempUPSSpecs);
                product.UPSSpecs = UPSSpecs;


            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {

            }
            else if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_UPDATE_CONDITION)
            {

            }
            modelAdditionalInfoPage.modelBasicInfoPage = modelBasicInfoPage;
            modelAdditionalInfoPage.modelUpsSpecsPage = this;

            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
                modelAdditionalInfoPage.modelUploadFilesPage = modelUploadFilesPage;

            NavigationService.Navigate(modelAdditionalInfoPage);
        }
        private void OnBtnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                modelUploadFilesPage.modelBasicInfoPage = modelBasicInfoPage;
                modelUploadFilesPage.modelUpsSpecsPage = this;
                modelUploadFilesPage.modelAdditionalInfoPage = modelAdditionalInfoPage;

                NavigationService.Navigate(modelUploadFilesPage);
            }
        }
        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;
            currentWindow.Close();
        }

        private void OnDropUploadFilesStackPanel(object sender, DragEventArgs e)
        {

        }

        private void OnSelChangedvalidUntilDate(object sender, SelectionChangedEventArgs e)
        {

        }

        
    }
}
