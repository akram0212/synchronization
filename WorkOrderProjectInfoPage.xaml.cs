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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for WorkOrderProjectInfoPage.xaml
    /// </summary>
    public partial class WorkOrderProjectInfoPage : Page
    {

        Employee loggedInUser;
        WorkOrder workOrder;

        private CommonQueries commonQueriesObject;
        private CommonFunctions commonFunctionsObject;
        private SQLServer sqlDatabase;
        private IntegrityChecks IntegrityChecks = new IntegrityChecks();

        private int viewAddCondition;

        private List<BASIC_STRUCTS.PROJECT_STRUCT> projects = new List<BASIC_STRUCTS.PROJECT_STRUCT>();
        private List<BASIC_STRUCTS.PROJECT_LOCATIONS_STRUCT> projectLocations = new List<BASIC_STRUCTS.PROJECT_LOCATIONS_STRUCT>();
        private List<BASIC_STRUCTS.PROJECT_LOCATIONS_STRUCT> addedLocations = new List<BASIC_STRUCTS.PROJECT_LOCATIONS_STRUCT>();

        public WorkOrderBasicInfoPage workOrderBasicInfoPage;
        public WorkOrderProductsPage workOrderProductsPage;
        public WorkOrderPaymentAndDeliveryPage workOrderPaymentAndDeliveryPage;
        public WorkOrderAdditionalInfoPage workOrderAdditionalInfoPage;
        public WorkOrderUploadFilesPage workOrderUploadFilesPage;

        private List<BASIC_STRUCTS.trialStruct> trialList = new List<BASIC_STRUCTS.trialStruct>();
        private List<BASIC_STRUCTS.trialStruct> addedList = new List<BASIC_STRUCTS.trialStruct>();
        private List<int> skip = new List<int>();

        Grid trialGrid = new Grid();
        int rowCounter = 1;

        public WorkOrderProjectInfoPage(ref Employee mLoggedInUser, ref WorkOrder mWorkOrder, int mViewAddCondition, ref WorkOrderProductsPage mWorkOrderProductsPage)
        {
            workOrderProductsPage = mWorkOrderProductsPage;
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;

            sqlDatabase = new SQLServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            workOrder = mWorkOrder;

            InitializeComponent();

            InitializeProjectsCombo();

            if(viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                checkAllCheckBox.IsEnabled = false;
                projectCheckBox.IsChecked = true;
                projectComboBox.SelectedItem = workOrder.GetprojectName();

            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INITIALIZE FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void InitializeProjectsCombo()
        {
            commonQueriesObject.GetClientProjects(ref projects);

            for (int i = 0; i < projects.Count; i++)
                projectComboBox.Items.Add(projects[i].project_name);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////SELECTION CHANGED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnSelChangedProjectCombo(object sender, SelectionChangedEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                addedLocations.Clear();
                projectLocations.Clear();
                locationsGrid.Children.Clear();
                locationsGrid.RowDefinitions.Clear();

                workOrder.InitializeProjectInfo(projects[projectComboBox.SelectedIndex].project_serial);

                commonQueriesObject.GetProjectLocations(workOrder.GetprojectSerial(), ref projectLocations);

                for (int i = 0; i < projectLocations.Count; i++)
                {
                    CheckBox checkBox = new CheckBox();
                    checkBox.Content = projectLocations[i].branch_Info.country + "," + projectLocations[i].branch_Info.city + "," + projectLocations[i].branch_Info.state_governorate + "," + projectLocations[i].branch_Info.district;
                    checkBox.Tag = i;
                    checkBox.Style = (Style)FindResource("checkBoxStyle");
                    checkBox.Checked += OnCheckProjectLocation;
                    checkBox.Unchecked += OnUnCheckProjectLocation;
                    checkBox.Width = 500.0;

                    RowDefinition row = new RowDefinition();
                    locationsGrid.RowDefinitions.Add(row);

                    locationsGrid.Children.Add(checkBox);
                    Grid.SetRow(checkBox, i);

                }
            }
            else
            {
                projectLocations.Clear();
                workOrder.GetProjectLocations(ref projectLocations);

                for (int i = 0; i < projectLocations.Count; i++)
                {
                    CheckBox checkBox = new CheckBox();
                    checkBox.Content = projectLocations[i].branch_Info.country + "," + projectLocations[i].branch_Info.city + "," + projectLocations[i].branch_Info.state_governorate + "," + projectLocations[i].branch_Info.district;
                    checkBox.IsEnabled = false;
                    checkBox.IsChecked = true;
                    checkBox.Style = (Style)FindResource("checkBoxStyle");
                    checkBox.Width = 500.0;

                    RowDefinition row = new RowDefinition();
                    locationsGrid.RowDefinitions.Add(row);

                    locationsGrid.Children.Add(checkBox);
                    Grid.SetRow(checkBox, i);

                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////CHECK/UNCHECK HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnCheckCheckAllCheckBox(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < locationsGrid.Children.Count; i++)
            {
                CheckBox currentcheckBox = (CheckBox)locationsGrid.Children[i];
                currentcheckBox.IsChecked = true;
            }
        }

        private void OnUnCheckCheckAllCheckBox(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < locationsGrid.Children.Count; i++)
            {
                CheckBox currentcheckBox = (CheckBox)locationsGrid.Children[i];
                currentcheckBox.IsChecked = false;
            }
        }

        private void OnCheckProject(object sender, RoutedEventArgs e)
        {
            projectComboBox.IsEnabled = true;
        }

        private void OnUnCheckProject(object sender, RoutedEventArgs e)
        {
            projectComboBox.SelectedItem = null;
            projectComboBox.IsEnabled = false;
            locationsGrid.Children.Clear();
        }

        private void OnCheckProjectLocation(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            addedLocations.Add(projectLocations[((int)currentCheckBox.Tag)]);

            workOrder.SetProjectLocations(addedLocations);
        }

        private void OnUnCheckProjectLocation(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            addedLocations.Remove(projectLocations[((int)currentCheckBox.Tag)]);

            workOrder.SetProjectLocations(addedLocations);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderBasicInfoPage.workOrderProductsPage = workOrderProductsPage;
            workOrderBasicInfoPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderBasicInfoPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderBasicInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;
            workOrderBasicInfoPage.workOrderProjectInfoPage = this;

            NavigationService.Navigate(workOrderBasicInfoPage);
        }
        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderProductsPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderProductsPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderProductsPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderProductsPage.workOrderUploadFilesPage = workOrderUploadFilesPage;
            workOrderProductsPage.workOrderProjectInfoPage = this;

            NavigationService.Navigate(workOrderProductsPage);
        }
        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderPaymentAndDeliveryPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderProductsPage = workOrderProductsPage;
            workOrderPaymentAndDeliveryPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderUploadFilesPage = workOrderUploadFilesPage;
            workOrderPaymentAndDeliveryPage.workOrderProjectInfoPage = this;

            NavigationService.Navigate(workOrderPaymentAndDeliveryPage);
        }
        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {
            workOrderAdditionalInfoPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderAdditionalInfoPage.workOrderProductsPage = workOrderProductsPage;
            workOrderAdditionalInfoPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderAdditionalInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;
            workOrderAdditionalInfoPage.workOrderProjectInfoPage = this;

            NavigationService.Navigate(workOrderAdditionalInfoPage);
        }
        private void OnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                workOrderUploadFilesPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
                workOrderUploadFilesPage.workOrderProductsPage = workOrderProductsPage;
                workOrderUploadFilesPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
                workOrderUploadFilesPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
                workOrderUploadFilesPage.workOrderProjectInfoPage = this;

                NavigationService.Navigate(workOrderUploadFilesPage);
            }
        }

        private void OnBtnClickNext(object sender, RoutedEventArgs e)
        {
            workOrderProductsPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderProductsPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderProductsPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderProductsPage.workOrderUploadFilesPage = workOrderUploadFilesPage;
            workOrderProductsPage.workOrderProjectInfoPage = this;

            NavigationService.Navigate(workOrderProductsPage);
        }

        private void OnBtnClickBack(object sender, RoutedEventArgs e)
        {
            workOrderBasicInfoPage.workOrderProductsPage = workOrderProductsPage;
            workOrderBasicInfoPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderBasicInfoPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderBasicInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;
            workOrderBasicInfoPage.workOrderProjectInfoPage = this;

            NavigationService.Navigate(workOrderBasicInfoPage);
        }

        private void OnBtnClickCancel(object sender, RoutedEventArgs e)
        {
            NavigationWindow currentWindow = (NavigationWindow)this.Parent;

            currentWindow.Close();
        }

        private void OnClickTestButton(object sender, RoutedEventArgs e)
        {
            InsertIntoOrderProjectLocations();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        protected bool InsertIntoOrderProjectLocations()
        {
            String sqlQueryPart1 = "insert into erp_system.dbo.work_orders_project_locations values (";
            String sqlQueryPart2 = "getdate());";
            string sqlQuery;

            String comma = ",";
            String apostropheComma = "',";
            String commaApostrophe = ",'";
            String apostropheCommaApostrophe = "','";

            for (int i = 0; i < addedLocations.Count; i++)
            {
                sqlQuery = String.Empty;
                sqlQuery += sqlQueryPart1;
                //sqlQuery += orderSerialTextBox.Text;
                sqlQuery += comma;
                //sqlQuery += addedLocations[i].project_serial;
                sqlQuery += comma;
                sqlQuery += addedLocations[i].location_id;
                sqlQuery += comma;
                sqlQuery += sqlQueryPart2;

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;
            }
            return true;
        }

        
    }
}
