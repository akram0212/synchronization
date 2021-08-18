
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

using _01electronics_library;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        private Employee loggedInUser;
        private CommonQueries commonQueries;
        private List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> products;
        public ProductsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;
            commonQueries = new CommonQueries();
            products = new List<COMPANY_WORK_MACROS.PRODUCT_STRUCT>();

            InitializeProducts();
            SetUpPageUIElements();
        }
        private void InitializeProducts()
        {
            if (!commonQueries.GetCompanyProducts(ref products))
                return;
        }

        public void SetUpPageUIElements()
        {
            for(int i = 0; i < products.Count(); i++)
            {
                RowDefinition rowI = new RowDefinition();
                ProductsGrid.RowDefinitions.Add(rowI);

                Grid gridI = new Grid();

                RowDefinition imageRow = new RowDefinition();
                gridI.RowDefinitions.Add(imageRow);

                String[] productName = new String[3];
                try
                {
                    productName = products[i].typeName.Split(' ');
                    Image productImage = new Image();
                    BitmapImage src = new BitmapImage();
                    src.BeginInit();
                    src.UriSource = new Uri(productName[0] + "_" + productName[1] + "_cover_photo.jpg", UriKind.Relative);
                    src.EndInit();
                    productImage.Source = src;
                    productImage.Width = 900;
                    productImage.Height = 400;
                    productImage.MouseDown += ImageMouseDown;
                    productImage.Tag = products[i].typeId.ToString();
                    //productImage.Name = products[i].typeName;

                    gridI.Children.Add(productImage);
                    Grid.SetRow(productImage, 0);

                    TextBlock imageTextBlock = new TextBlock();
                    imageTextBlock.Background = new SolidColorBrush(Color.FromRgb(237, 237, 237));
                    imageTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                    imageTextBlock.Width = 350;
                    imageTextBlock.Height = 150;
                    imageTextBlock.Margin = new Thickness(100, -20, 0, 0);
                    imageTextBlock.Padding = new Thickness(15,15,15,15);
                    imageTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
                    imageTextBlock.FontSize = 16;
                    imageTextBlock.TextWrapping = TextWrapping.Wrap;
                    imageTextBlock.Text = "  " + products[i].typeName;
                    imageTextBlock.Text += "\n\n";

                    imageTextBlock.Text += "A non-interruptible clean and stabilized form of power to protect your industry machines data centers and all your electrical devices.";
                    gridI.Children.Add(imageTextBlock);
                    Grid.SetRow(imageTextBlock, 0);
                    ProductsGrid.Children.Add(gridI);
                    Grid.SetRow(gridI, i);
                }
                catch
                {
                    productName[0] = products[i].typeName;
                    Image productImage = new Image();
                    BitmapImage src = new BitmapImage();
                    src.BeginInit();
                    src.UriSource = new Uri(productName[0] + "_cover_photo.jpg", UriKind.Relative);
                    src.EndInit();
                    productImage.Source = src;
                    productImage.Width = 900;
                    productImage.Height = 400;
                    productImage.MouseDown += ImageMouseDown;
                    productImage.Tag = products[i].typeId.ToString();
                    //productImage.Name = products[i].typeName;
                    gridI.Children.Add(productImage);
                    Grid.SetRow(productImage, 0);

                    TextBlock imageTextBlock = new TextBlock();
                    imageTextBlock.Background = new SolidColorBrush(Color.FromRgb(237, 237, 237));
                    imageTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                    imageTextBlock.Width = 350;
                    imageTextBlock.Height = 150;
                    imageTextBlock.Margin = new Thickness(100, -20, 0, 0);
                    imageTextBlock.Padding = new Thickness(15, 15, 15, 15);
                    imageTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
                    imageTextBlock.FontSize = 16;
                    imageTextBlock.TextWrapping = TextWrapping.Wrap;
                    imageTextBlock.Text = "  "+ products[i].typeName;
                    imageTextBlock.Text += "\n\n";

                    imageTextBlock.Text += "A non-interruptible clean and stabilized form of power to protect your industry machines data centers and all your electrical devices.";
                    Grid.SetRow(imageTextBlock, 0);

                    gridI.Children.Add(imageTextBlock);
                    ProductsGrid.Children.Add(gridI);
                    Grid.SetRow(gridI, i);

                }
                
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
        //MOUSE DOWN HANDLERS
        /////////////////////////////////////////////////////////////////
        ///
        private void ImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            String tmp = currentImage.Tag.ToString();

            Product selectedProduct = new Product();
            selectedProduct.SetProductID(int.Parse(tmp));

            BrandsPage productsPage = new BrandsPage(ref loggedInUser, ref selectedProduct);
            this.NavigationService.Navigate(productsPage);
        }
       
    }
}
