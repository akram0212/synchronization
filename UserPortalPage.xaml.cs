using _01electronics_library;
using _01electronics_windows_library;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for UserPortalPage.xaml
    /// </summary>
    /// 
    public partial class UserPortalPage : Page
    {
        private Employee loggedInUser;

        private FTPServer ftpServer;

        protected String errorMessage;

        public UserPortalPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;
            ftpServer = new FTPServer();

            InitializeEmployeeDashboard();
        }

        private void InitializeEmployeeDashboard()
        {
            InitializeEmployeePhoto();

            employeeNameLabel.Content = loggedInUser.GetEmployeeName();
            employeeBirthdateLabel.Content = loggedInUser.GetEmployeeBirthDate();
            employeeJoiningDateLabel.Content = loggedInUser.GetEmployeeJoinDate();
            employeeDepartmentLabel.Content = loggedInUser.GetEmployeeDepartment();
            employeeTeamLabel.Content = loggedInUser.GetEmployeeTeam();
            employeeBusinessEmailLabel.Content = loggedInUser.GetEmployeeBusinessEmail();
            employeePersonalEmailLabel.Content = loggedInUser.GetEmployeePersonalEmail();
        }
        private void InitializeEmployeePhoto()
        {
            string imagePath = Directory.GetCurrentDirectory() + "\\" + loggedInUser.GetEmployeeId() + ".jpg";

            try
            {
                Image employeePhoto = new Image();

                employeePhoto.Margin = new Thickness(24);
                employeePhoto.Width = 245;
                employeePhoto.Height = 245;

                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(imagePath, UriKind.Absolute);
                src.EndInit();
                employeePhoto.Source = src;

                PhotoGrid.Children.Add(employeePhoto);
            }
            catch
            {
                if (ftpServer.DownloadFile(BASIC_MACROS.EMPLOYEES_PHOTOS_PATH + loggedInUser.GetEmployeeId() + ".jpg", imagePath, BASIC_MACROS.SEVERITY_LOW, ref errorMessage))
                {
                    Image employeePhoto = new Image();

                    employeePhoto.Margin = new Thickness(24);
                    employeePhoto.Width = 245;
                    employeePhoto.Height = 245;

                    BitmapImage src = new BitmapImage();
                    src.BeginInit();
                    src.UriSource = new Uri(imagePath, UriKind.Absolute);
                    src.EndInit();
                    employeePhoto.Source = src;

                    PhotoGrid.Children.Add(employeePhoto);
                }
                else
                {
                    // System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //EXTERNAL TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
        private void OnButtonClickedProducts(object sender, MouseButtonEventArgs e)
        {
            CategoriesPage productsPage = new CategoriesPage(ref loggedInUser);
            this.NavigationService.Navigate(productsPage);
        }
        private void OnButtonClickedWorkOrders(object sender, RoutedEventArgs e)
        {
            WorkOrdersPage workOrders = new WorkOrdersPage(ref loggedInUser);
            this.NavigationService.Navigate(workOrders);
        }
        private void OnButtonClickedWorkOffers(object sender, RoutedEventArgs e)
        {
            QuotationsPage workOffers = new QuotationsPage(ref loggedInUser);
            this.NavigationService.Navigate(workOffers);
        }
        private void OnButtonClickedRFQs(object sender, RoutedEventArgs e)
        {
            RFQsPage rFQsPage = new RFQsPage(ref loggedInUser);
            this.NavigationService.Navigate(rFQsPage);
        }
        private void OnButtonClickedVisits(object sender, RoutedEventArgs e)
        {
            ClientVisitsPage clientVisitsPage = new ClientVisitsPage(ref loggedInUser);
            this.NavigationService.Navigate(clientVisitsPage);
        }
        private void OnButtonClickedCalls(object sender, RoutedEventArgs e)
        {
            ClientCallsPage clientCallsPage = new ClientCallsPage(ref loggedInUser);
            this.NavigationService.Navigate(clientCallsPage);
        }
        private void OnButtonClickedMeetings(object sender, RoutedEventArgs e)
        {
            OfficeMeetingsPage officeMeetingsPage = new OfficeMeetingsPage(ref loggedInUser);
            this.NavigationService.Navigate(officeMeetingsPage);
        }

        private void OnButtonClickedMyProfile(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnButtonClickedProjects(object sender, MouseButtonEventArgs e)
        {
            ProjectsPage projectsPage = new ProjectsPage(ref loggedInUser);
            this.NavigationService.Navigate(projectsPage);
        }

        private void OnButtonClickedMaintenanceContracts(object sender, MouseButtonEventArgs e)
        {
            MaintenanceContractsPage maintenanceContractsPage = new MaintenanceContractsPage(ref loggedInUser);
            this.NavigationService.Navigate(maintenanceContractsPage);
        }
        private void OnButtonClickedMaintenanceOffer(object sender, MouseButtonEventArgs e)
        {
            MaintenanceOffersPage maintenanceOffersPage = new MaintenanceOffersPage(ref loggedInUser);
            this.NavigationService.Navigate(maintenanceOffersPage);
        }

        private void OnBtnClickedStatistics(object sender, MouseButtonEventArgs e)
        {
            StatisticsPage statisticsPage = new StatisticsPage(ref loggedInUser);
            this.NavigationService.Navigate(statisticsPage);
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //BTN CLICKED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
