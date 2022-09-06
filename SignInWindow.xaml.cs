using System.Windows.Navigation;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for SignInWindow.xaml
    /// </summary>
    public partial class SignInWindow : NavigationWindow
    {
        public SignInWindow()
        {
            InitializeComponent();

            SignInPage signIn = new SignInPage();
            this.NavigationService.Navigate(signIn);
        }

    }
}
