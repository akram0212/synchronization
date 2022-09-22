using _01electronics_library;
using _01electronics_windows_library;
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
        List<PROCUREMENT_STRUCTS.MEASURE_UNITS_STRUCT> rating;

        public ModelUpsSpecsPage(ref Employee mLoggedInUser, ref Product mPrduct, int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            
            viewAddCondition = mViewAddCondition;

            sqlDatabase = new SQLServer();
            UPSSpecs = new List<BASIC_STRUCTS.UPS_SPECS_STRUCT>();
            rating = new List<PROCUREMENT_STRUCTS.MEASURE_UNITS_STRUCT>();
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
            initializeRatingCombobox();
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
                if (iOPhaseTextBox.Text != "" || (product.UPSSpecs.Count > 0 && product.UPSSpecs[0].io_phase != null))
                {
                    tempUPSSpecs.io_phase = iOPhaseTextBox.Text.ToString();
                }
                else
                {
                    MessageBox.Show("IO Phase is empty, please enter it.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;

                }
                if (ratedPowerTextBox.Text != "" || (product.UPSSpecs.Count > 0 && product.UPSSpecs[0].rated_power != null))
                {   
                    tempUPSSpecs.rated_power = decimal.Parse( ratedPowerTextBox.Text.ToString());
                }
                else
                {
                    MessageBox.Show("Rated Power is empty, please enter it.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;

                }
                if (ratingComboBox.SelectedIndex != -1 || (product.UPSSpecs.Count > 0 && product.UPSSpecs[0].rating != null))
                {
                    tempUPSSpecs.rating = ratingComboBox.SelectedItem.ToString();
                    tempUPSSpecs.rating_id = ratingComboBox.SelectedIndex + 1;
                }
                else
                {
                    MessageBox.Show("Rating is empty, please Chose One .", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;

                }

                if (backupTime50TextBox.Text != "" || (product.UPSSpecs.Count > 0 && product.UPSSpecs[0].backup_time_50 != null))
                {
                    tempUPSSpecs.backup_time_50 = int.Parse(backupTime50TextBox.Text.ToString());
                }
                else
                {
                    MessageBox.Show("Backup time 50% is empty, please enter time.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;

                }

                if (backupTime70TextBox.Text != "" || (product.UPSSpecs.Count > 0 && product.UPSSpecs[0].backup_time_70 != null))
                {
                    tempUPSSpecs.backup_time_70 = int.Parse(backupTime70TextBox.Text.ToString());
                }
                else
                {
                    MessageBox.Show("Backup time 70% is empty, please enter time.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (backupTime100TextBox.Text != "" || (product.UPSSpecs.Count > 0 && product.UPSSpecs[0].backup_time_100 != null))
                {
                    tempUPSSpecs.backup_time_100 = int.Parse(backupTime100TextBox.Text.ToString());
                }
                else
                {
                    MessageBox.Show("Backup time 100% is empty, please enter time.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                tempUPSSpecs.input_power_factor = inputPowerFactorTextBox.Text.ToString();
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
                if (validUntilDatePicker.SelectedDate != null || (product.UPSSpecs.Count > 0 && product.UPSSpecs[0].valid_until != null))
                {
                    tempUPSSpecs.valid_until = (DateTime)validUntilDatePicker.SelectedDate;
                }
                else
                {
                    MessageBox.Show("Date picker is not selected, please Select Date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;

                }
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
                if (iOPhaseTextBox.Text != "" || (product.UPSSpecs.Count > 0 && product.UPSSpecs[0].io_phase != null))
                {
                    tempUPSSpecs.io_phase = iOPhaseTextBox.Text.ToString();
                }
                else
                {
                    MessageBox.Show("IO Phase is empty, please enter it.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;

                }
                if (ratedPowerTextBox.Text != "" || (product.UPSSpecs.Count > 0 && product.UPSSpecs[0].rated_power != null))
                {
                    tempUPSSpecs.rated_power = decimal.Parse(ratedPowerTextBox.Text.ToString());
                }
                else
                {
                    MessageBox.Show("Rated Power is empty, please enter it.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;

                }
                if (ratingComboBox.SelectedIndex!= -1 || (product.UPSSpecs.Count > 0 && product.UPSSpecs[0].rating != null))
                {
                    tempUPSSpecs.rating = ratingComboBox.SelectedItem.ToString();
                    tempUPSSpecs.rating_id = ratingComboBox.SelectedIndex+1;
                }
                else
                {
                    MessageBox.Show("Rating is empty, please Chose One .", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;

                }
                
                if (backupTime50TextBox.Text != "" || (product.UPSSpecs.Count > 0 && product.UPSSpecs[0].backup_time_50 != null))
                {
                    tempUPSSpecs.backup_time_50 = int.Parse(backupTime50TextBox.Text.ToString());
                }
                else
                {
                    MessageBox.Show("Backup time 50% is empty, please enter time.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;

                }

                if (backupTime70TextBox.Text != "" || (product.UPSSpecs.Count > 0 && product.UPSSpecs[0].backup_time_70 != null))
                {
                    tempUPSSpecs.backup_time_70 = int.Parse(backupTime70TextBox.Text.ToString());
                }
                else
                {
                    MessageBox.Show("Backup time 70% is empty, please enter time.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (backupTime100TextBox.Text != "" || (product.UPSSpecs.Count > 0 && product.UPSSpecs[0].backup_time_100 != null))
                {
                    tempUPSSpecs.backup_time_100 = int.Parse(backupTime100TextBox.Text.ToString());
                }
                else
                {
                    MessageBox.Show("Backup time 100% is empty, please enter time.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                tempUPSSpecs.input_power_factor = inputPowerFactorTextBox.Text.ToString();
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
                if (validUntilDatePicker.SelectedDate != null || (product.UPSSpecs.Count > 0 && product.UPSSpecs[0].valid_until != null))
                {
                    tempUPSSpecs.valid_until = (DateTime)validUntilDatePicker.SelectedDate;
                }
                else
                {
                    MessageBox.Show("Date picker is not selected, please Select Date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;

                }
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
        //////////////////////////////////////////////////////////////////////
        ///INITILIZATIONS
        /////////////////////////////////////////////////////////////////////
        void initializeRatingCombobox()
        {
            rating.Clear();

            if (!commonQueriesObject.GetMeasureUnits(ref rating))
                return;

            ratingComboBox.Items.Clear();

            for (int i = 0; i < rating.Count; i++)
            {
                ratingComboBox.Items.Add(rating[i].measure_unit);
            }
        }
    }
}
