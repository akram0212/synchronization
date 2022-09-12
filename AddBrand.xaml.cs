using _01electronics_library;
using _01electronics_windows_library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for AddBrand.xaml
    /// </summary>
    public partial class AddBrand : Window
    {
        private CommonQueries commonQueriesObject;

        private List<COMPANY_WORK_MACROS.BRAND_STRUCT> brands = new List<COMPANY_WORK_MACROS.BRAND_STRUCT>();
        private Employee loggedInUser;

        protected String errorMessage;
        protected SQLServer sqlDatabase;

        protected IntegrityChecks integrityChecks;
        protected FTPServer ftpObject;

        protected int counter;
        protected int viewAddCondition;


        protected BackgroundWorker uploadBackground;
        protected BackgroundWorker downloadBackground;

        protected String serverFolderPath;
        protected String serverFileName;

        protected String localFolderPath;
        protected String localFileName;

        protected bool fileUploaded;
        protected bool fileDownloaded;
        protected bool uploadThisFile = false;
        protected bool checkFileInServer = false;
        protected bool canEdit = false;

        WrapPanel wrapPanel = new WrapPanel();

        private Grid previousSelectedFile;
        private Grid currentSelectedFile;
        private Grid overwriteFileGrid;
        private Label currentLabel;

        protected Product product;

        List<string> ftpFiles;

        ProgressBar progressBar = new ProgressBar();
        public AddBrand(ref Product pProduct, ref Employee mLoggedInUser, ref int mViewAddCondition)
        {

            InitializeComponent();
            commonQueriesObject = new CommonQueries();
            counter = 0;
            loggedInUser = mLoggedInUser;
            canEdit = false;
            InitializeComponent();
            sqlDatabase = new SQLServer();
            ftpObject = new FTPServer();
            integrityChecks = new IntegrityChecks();
            product = pProduct;
            viewAddCondition = mViewAddCondition;


            ftpFiles = new List<string>(); progressBar.Style = (Style)FindResource("ProgressBarStyle");
            progressBar.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            progressBar.Width = 200;

            uploadBackground = new BackgroundWorker();
            //uploadBackground.DoWork += BackgroundUpload;
            //uploadBackground.ProgressChanged += OnUploadProgressChanged;
            //uploadBackground.RunWorkerCompleted += OnUploadBackgroundComplete;
            uploadBackground.WorkerReportsProgress = true;

            uploadFilesStackPanel.Children.Clear();

            downloadBackground = new BackgroundWorker();
            //downloadBackground.DoWork += BackgroundDownload;
            //downloadBackground.ProgressChanged += OnDownloadProgressChanged;
            //downloadBackground.RunWorkerCompleted += OnDownloadBackgroundComplete;
            downloadBackground.WorkerReportsProgress = true;

            serverFolderPath = product.GetProductFolderServerPath();
            commonQueriesObject.GetCompanyBrands(ref brands);
            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
            {
            }
            else
            {
                ContactProfileHeader.Content = "VIEW PRODUCT";
                serverFileName = (String)product.GetProductID().ToString() + ".jpg";
                localFolderPath = product.GetProductPhotoLocalPath();
                uploadBackground.RunWorkerAsync();
                uploadFilesStackPanel.Children.Clear();
                uploadFilesStackPanel.Children.Add(wrapPanel);
            }
            checkEmployee();

            
            InitializeBrandsComboBox();


            }
        
        /// /////////////////////////////////////////////////////////////////
        /// ////////cHECKERS
        /// /////////////////////////////////////////////////////////////////
       
        private void checkEmployee()
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_VIEW_CONDITION)
            {
                product.SetBrandName(brands[product.GetBrandID()].brandName);
                BrandNameComboBox.SelectedIndex= product.GetBrandID();  
                BrandNameLabel.Content = product.GetBrandName();
                BrandNameComboBox.Visibility = Visibility.Collapsed;
                BrandNameLabel.Visibility = Visibility.Visible;

                
                picHint.Visibility = Visibility.Hidden;


                saveChangesButton.IsEnabled = false;
                if (loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.ERP_SYSTEM_DEVELOPMENT_TEAM_ID ||
               loggedInUser.GetEmployeeTeamId() == COMPANY_ORGANISATION_MACROS.BUSINESS_DEVELOPMENT_TEAM_ID ||
               (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION && loggedInUser.GetEmployeeDepartmentId() == COMPANY_ORGANISATION_MACROS.BUSINESS_DEVELOPMENT_DEPARTMENT_ID))
                {
                    canEdit = true;
                    editPictureButton.Visibility = Visibility.Visible;
                }
            }



        }
        /// /////////////////////////////////////////////////////////////////
        /// ////////Btn Click & Mouse Leave
        /// /////////////////////////////////////////////////////////////////
        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition == COMPANY_WORK_MACROS.PRODUCT_ADD_CONDITION)
            {
                product.SetBrandName(BrandNameComboBox.SelectedItem.ToString());
                product.SetBrandID(brands[BrandNameComboBox.SelectedIndex].brandId);
                product.AddBrandToProduct();
                this.Close();
            }
         
        }

        private void onBtnEditClick(object sender, RoutedEventArgs e)
        {

        }
        private void ProductNameMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
        private void productName_MouseLeave(object sender, MouseEventArgs e)
        {

        }
        private void OnDropUploadFilesStackPanel(object sender, DragEventArgs e)
        {

        }


        /// /////////////////////////////////////////////////////////////////
        /// //////// INITIALIZE
        /// /////////////////////////////////////////////////////////////////

        private bool InitializeBrandsComboBox()
        {
            for (int i = 0; i < brands.Count; i++)
                BrandNameComboBox.Items.Add(brands[i].brandName);
            return true; 
        }

  

        private void BrandNameComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            product.SetBrandID(BrandNameComboBox.SelectedIndex+1);
            wrapPanel.Children.Clear();
            uploadFilesStackPanel.Children.Clear();
            try
            {
                Image brandLogo = new Image();
                //string src = String.Format(@"/01electronics_crm;component/photos/brands/" + brandsList[i].brandId + ".jpg
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(product.GetBrandPhotoLocalPath(), UriKind.Relative);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                brandLogo.Source = src;
                brandLogo.HorizontalAlignment = HorizontalAlignment.Stretch;
                brandLogo.VerticalAlignment = VerticalAlignment.Stretch;
                //brandLogo.Width = 300;
                //brandLogo.Margin = new Thickness(80, 100, 12, 12);




                //if(brandsList[i].brandId == 0)
                //{
                //    Label othersLabel = new Label();
                //    othersLabel.Content = brandsList[i].brandName;
                //    othersLabel.Style = (Style)FindResource("tableHeaderItem");
                //    gridI.Children.Add(othersLabel);
                //}    


                wrapPanel.Children.Add(brandLogo);

                uploadFilesStackPanel.Children.Add(wrapPanel);

            }


            catch
            {


                product.SetPhotoServerPath(product.GetBrandFolderServerPath() + "/" + product.GetBrandID()+ ".jpg");
                if (product.DownloadPhotoFromServer())
                {

                    Image brandLogo = new Image();
                    //string src = String.Format(@"/01electronics_crm;component/photos/brands/" + brandsList[i].brandId + ".jpg
                    BitmapImage src = new BitmapImage();
                    src.BeginInit();
                    src.UriSource = new Uri(product.GetBrandPhotoLocalPath(), UriKind.Relative);
                    src.CacheOption = BitmapCacheOption.OnLoad;
                    src.EndInit();
                    brandLogo.Source = src;
                    brandLogo.HorizontalAlignment = HorizontalAlignment.Stretch;
                    brandLogo.VerticalAlignment = VerticalAlignment.Stretch;
                    //brandLogo.Width = 300;
                    //brandLogo.Margin = new Thickness(80, 100, 12, 12);


                    //if(brandsList[i].brandId == 0)
                    //{
                    //    Label othersLabel = new Label();
                    //    othersLabel.Content = brandsList[i].brandName;
                    //    othersLabel.Style = (Style)FindResource("tableHeaderItem");
                    //    gridI.Children.Add(othersLabel);
                    //}    



                    wrapPanel.Children.Add(brandLogo);

                    uploadFilesStackPanel.Children.Add(wrapPanel);


                }
            }
        }
    }
}
