using _01electronics_library;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

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

        private List<PROJECT_MACROS.PROJECT_STRUCT> projects = new List<PROJECT_MACROS.PROJECT_STRUCT>();
        private List<BASIC_STRUCTS.ADDRESS_STRUCT> projectLocations = new List<BASIC_STRUCTS.ADDRESS_STRUCT>();
        private List<BASIC_STRUCTS.ADDRESS_STRUCT> orderProjectLocations = new List<BASIC_STRUCTS.ADDRESS_STRUCT>();
        private List<BASIC_STRUCTS.ADDRESS_STRUCT> addedLocations = new List<BASIC_STRUCTS.ADDRESS_STRUCT>();

        public WorkOrderBasicInfoPage workOrderBasicInfoPage;
        public WorkOrderProductsPage workOrderProductsPage;
        public WorkOrderPaymentAndDeliveryPage workOrderPaymentAndDeliveryPage;
        public WorkOrderAdditionalInfoPage workOrderAdditionalInfoPage;
        public WorkOrderUploadFilesPage workOrderUploadFilesPage;


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

            if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
            {
                projectCheckBox.IsChecked = true;
                projectCheckBox.IsEnabled = false;
                projectComboBox.SelectedItem = workOrder.GetprojectName();
                projectComboBox.IsEnabled = false;
                checkAllCheckBox.IsChecked = true;
                checkAllCheckBox.IsEnabled = false;
            }
            //else if (viewAddCondition == COMPANY_WORK_MACROS.ORDER_REVISE_CONDITION)
            //{
            //    workOrder.GetProjectLocations(ref orderProjectLocations);
            //    projectCheckBox.IsChecked = true;
            //    projectComboBox.SelectedItem = workOrder.GetprojectName();
            //    InitializeProjectLocationsGridRevise();
            //}
            else
            {
                projectCheckBox.IsEnabled = true;
                projectCheckBox.IsChecked = true;

                projectComboBox.SelectedItem = workOrder.GetprojectName();

                checkAllCheckBox.IsEnabled = true;
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

        private void InitializeProjectLocationsGridRevise()
        {
            //projectLocations.Clear();
            //addedLocations.Clear();
            //locationsGrid.Children.Clear();
            //locationsGrid.RowDefinitions.Clear();
            for (int i = 0; i < locationsGrid.Children.Count; i++)
            {
                CheckBox currentCheckBox = (CheckBox)locationsGrid.Children[i];
                int locationId = projectLocations[int.Parse(currentCheckBox.Tag.ToString())].location_id;


                if (orderProjectLocations.Exists(s1 => s1.location_id == locationId))
                    currentCheckBox.IsChecked = true;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////SELECTION CHANGED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnSelChangedProjectCombo(object sender, SelectionChangedEventArgs e)
        {
            if (projectComboBox.SelectedItem != null)
            {
                checkAllCheckBox.IsChecked = false;

                if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                {
                    addedLocations.Clear();
                    projectLocations.Clear();
                    locationsGrid.Children.Clear();
                    locationsGrid.RowDefinitions.Clear();

                    workOrder.InitializeProjectInfo(projects[projectComboBox.SelectedIndex].project_serial);

                    if (!commonQueriesObject.GetProjectLocations(workOrder.GetprojectSerial(), ref projectLocations))
                        return;

                    List<BASIC_STRUCTS.ADDRESS_STRUCT> temp = new List<BASIC_STRUCTS.ADDRESS_STRUCT>();

                    workOrder.GetProjectLocations(ref temp);

                    for (int i = 0; i < projectLocations.Count; i++)
                    {
                        CheckBox checkBox = new CheckBox();
                        checkBox.Content = projectLocations[i].country.country_name + "," + projectLocations[i].state_governorate.state_name + "," + projectLocations[i].city.city_name + "," + projectLocations[i].district.district_name;
                        checkBox.Tag = i;
                        checkBox.Style = (Style)FindResource("checkBoxStyle");
                        checkBox.Checked += OnCheckProjectLocation;
                        checkBox.Unchecked += OnUnCheckProjectLocation;
                        checkBox.Width = 500.0;

                        if (temp.Exists(x1 => x1.location_id == projectLocations[i].location_id))
                            checkBox.IsChecked = true;

                        RowDefinition row = new RowDefinition();
                        locationsGrid.RowDefinitions.Add(row);

                        locationsGrid.Children.Add(checkBox);
                        Grid.SetRow(checkBox, i);

                    }
                }
                else
                {
                    projectLocations.Clear();
                    addedLocations.Clear();
                    locationsGrid.Children.Clear();
                    locationsGrid.RowDefinitions.Clear();
                    workOrder.GetProjectLocations(ref orderProjectLocations);

                    for (int i = 0; i < orderProjectLocations.Count; i++)
                    {
                        CheckBox checkBox = new CheckBox();
                        checkBox.Content = orderProjectLocations[i].country.country_name + "," + orderProjectLocations[i].state_governorate.state_name + "," + orderProjectLocations[i].city.city_name + "," + orderProjectLocations[i].district.district_name;
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
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////CHECK/UNCHECK HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnCheckCheckAllCheckBox(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < locationsGrid.Children.Count; i++)
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
            checkAllCheckBox.IsChecked = false;
        }

        private void OnCheckProjectLocation(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            addedLocations.Add(projectLocations[((int)currentCheckBox.Tag)]);

        }

        private void OnUnCheckProjectLocation(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            addedLocations.Remove(projectLocations[((int)currentCheckBox.Tag)]);

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///INTERNAL TABS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnClickBasicInfo(object sender, MouseButtonEventArgs e)
        {

            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                workOrder.SetProjectLocations(addedLocations);

            workOrderBasicInfoPage.workOrderProductsPage = workOrderProductsPage;
            workOrderBasicInfoPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderBasicInfoPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderBasicInfoPage.workOrderUploadFilesPage = workOrderUploadFilesPage;
            workOrderBasicInfoPage.workOrderProjectInfoPage = this;

            NavigationService.Navigate(workOrderBasicInfoPage);
        }
        private void OnClickProductsInfo(object sender, MouseButtonEventArgs e)
        {

            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                workOrder.SetProjectLocations(addedLocations);

            workOrderProductsPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderProductsPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderProductsPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderProductsPage.workOrderUploadFilesPage = workOrderUploadFilesPage;
            workOrderProductsPage.workOrderProjectInfoPage = this;

            NavigationService.Navigate(workOrderProductsPage);
        }
        private void OnClickPaymentAndDeliveryInfo(object sender, MouseButtonEventArgs e)
        {

            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                workOrder.SetProjectLocations(addedLocations);

            workOrderPaymentAndDeliveryPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderProductsPage = workOrderProductsPage;
            workOrderPaymentAndDeliveryPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderPaymentAndDeliveryPage.workOrderUploadFilesPage = workOrderUploadFilesPage;
            workOrderPaymentAndDeliveryPage.workOrderProjectInfoPage = this;

            NavigationService.Navigate(workOrderPaymentAndDeliveryPage);
        }
        private void OnClickAdditionalInfo(object sender, MouseButtonEventArgs e)
        {

            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                workOrder.SetProjectLocations(addedLocations);

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
            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                workOrder.SetProjectLocations(addedLocations);

            workOrderProductsPage.workOrderBasicInfoPage = workOrderBasicInfoPage;
            workOrderProductsPage.workOrderPaymentAndDeliveryPage = workOrderPaymentAndDeliveryPage;
            workOrderProductsPage.workOrderAdditionalInfoPage = workOrderAdditionalInfoPage;
            workOrderProductsPage.workOrderUploadFilesPage = workOrderUploadFilesPage;
            workOrderProductsPage.workOrderProjectInfoPage = this;

            NavigationService.Navigate(workOrderProductsPage);
        }

        private void OnBtnClickBack(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition != COMPANY_WORK_MACROS.ORDER_VIEW_CONDITION)
                workOrder.SetProjectLocations(addedLocations);

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



        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///SET FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetProjectComboBox()
        {
            projectComboBox.SelectedItem = workOrder.GetprojectName();
            if (projectComboBox.SelectedItem != null)
            {
                projectCheckBox.IsChecked = true;
                checkAllCheckBox.IsChecked = true;
                checkAllCheckBox.IsEnabled = false;
                projectCheckBox.IsEnabled = false;
            }
        }
    }
}
