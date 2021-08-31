using System;
using System.Collections.Generic;
using System.IO;
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
//using System.Drawing;
//using Size = System.Drawing.Size;
using _01electronics_library;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for UserPortalPage.xaml
    /// </summary>
    /// 
    public partial class UserPortalPage : Page
    {
        FTPServer ftpServer;
        private Employee loggedInUser;

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

            if (ftpServer.DownloadFile(BASIC_MACROS.EMPLOYEES_PHOTOS_PATH + loggedInUser.GetEmployeeId() + ".jpg", imagePath))
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
        }
        /////////////////////////////////////////////////////////////////
        //EXTERNAL TABS
        /////////////////////////////////////////////////////////////////

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
            ProductsPage productsPage = new ProductsPage(ref loggedInUser);
            this.NavigationService.Navigate(productsPage);
        }
        private void OnButtonClickedWorkOrders(object sender, RoutedEventArgs e)
        {
            WorkOrdersPage workOrders = new WorkOrdersPage(ref loggedInUser);
            this.NavigationService.Navigate(workOrders);
        }
        private void OnButtonClickedWorkOffers(object sender, RoutedEventArgs e)
        {
            WorkOffersPage workOffers = new WorkOffersPage(ref loggedInUser);
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
        private void OnButtonClickedStatistics(object sender, RoutedEventArgs e)
        {

        }

        /////////////////////////////////////////////////////////////////
        //BTN CLICKED HANDLERS
        /////////////////////////////////////////////////////////////////
    }
}
