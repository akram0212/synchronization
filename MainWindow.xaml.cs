using _01electronics_library;
using System.Windows.Navigation;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow(ref Employee mLoggedInUser)
        {
            InitializeComponent();


            StatisticsPage statisticsPage = new StatisticsPage(ref mLoggedInUser);
            this.NavigationService.Navigate(statisticsPage);
        }
        public MainWindow()
        {
        }
    }
}
