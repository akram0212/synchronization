using _01electronics_erp;
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
using System.Windows.Shapes;

namespace _01electronics_crm
{
    /// <summary>
    /// Interaction logic for ViewCompanyInfoWindow.xaml
    /// </summary>
    public partial class ViewCompanyInfoWindow : Window
    {
        long companyID;
        Company company_Info;
        public ViewCompanyInfoWindow(long companyID)
        {
            InitializeComponent();
            this.companyID = companyID;
            company_Info = new Company();
            
            //Company Name
            company_Info.InitializeCompanyInfo((int)companyID);
           // company_Info.QueryCompanyPhones();
            company_Info.InitializeBranchInfo((int)companyID);
            this.NameText.Text = company_Info.GetCompanyName();

            // Primary Field
            WrapPanel Panel1 = new WrapPanel();
            Label PrimaryFieldLabel = new Label();
            RegisterName("PrimaryFieldLabel", PrimaryFieldLabel);
            PrimaryFieldLabel.Width = 200;
            PrimaryFieldLabel.Margin = new System.Windows.Thickness(10, 0, 0, 0);
            PrimaryFieldLabel.Content = "Primary Field: ";
            PrimaryFieldLabel.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            PrimaryFieldLabel.FontWeight = FontWeights.Bold;
            PrimaryFieldLabel.FontSize = 14;
            PrimaryFieldLabel.Padding = new System.Windows.Thickness(10);

            TextBox PrimaryFieldText = new TextBox();
            RegisterName("PrimaryFieldText", PrimaryFieldText);
            PrimaryFieldText.Margin = new System.Windows.Thickness(20, 0, 0, 0);
            PrimaryFieldText.Width = 200;
            PrimaryFieldText.Height = 25;
            PrimaryFieldText.Padding = new Thickness(0, 0, 0, 5);

            Panel1.Children.Add(PrimaryFieldLabel);
            Panel1.Children.Add(PrimaryFieldText);
            PrimaryFieldText.Text = company_Info.GetCompanyPrimaryField();
            COL1.Children.Add(Panel1);

            // secondary Field
            WrapPanel Panel2 = new WrapPanel();
            Label SecondaryFieldLabel = new Label();
            RegisterName("SecondaryFieldLabel", SecondaryFieldLabel);
            SecondaryFieldLabel.Width = 200;
            SecondaryFieldLabel.Margin = new System.Windows.Thickness(10, 0, 0, 0);
            SecondaryFieldLabel.Content = "Secondary Field: ";
            SecondaryFieldLabel.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            SecondaryFieldLabel.FontWeight = FontWeights.Bold;
            SecondaryFieldLabel.FontSize = 14;
            SecondaryFieldLabel.Padding = new System.Windows.Thickness(10);

            TextBox SecondaryFieldLabelText = new TextBox();
            RegisterName("SecondaryFieldLabelText", SecondaryFieldLabelText);
            SecondaryFieldLabelText.Margin = new System.Windows.Thickness(20, 0, 0, 0);
            SecondaryFieldLabelText.Width = 200;
            SecondaryFieldLabelText.Height = 25;
            SecondaryFieldLabelText.Padding = new Thickness(0, 0, 0, 5);

            Panel2.Children.Add(SecondaryFieldLabel);
            Panel2.Children.Add(SecondaryFieldLabelText);
            SecondaryFieldLabelText.Text = company_Info.GetCompanySecondaryField();
            COL1.Children.Add(Panel2);

            //company country
            company_Info.GetCompanyAddress();

            WrapPanel Panel4 = new WrapPanel();
            Label CountryLabel = new Label();
            RegisterName("CountryLabel", CountryLabel);
            CountryLabel.Width = 200;
            CountryLabel.Margin = new System.Windows.Thickness(10, 0, 0, 0);
            CountryLabel.Content = "Country: ";
            CountryLabel.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            CountryLabel.FontWeight = FontWeights.Bold;
            CountryLabel.FontSize = 14;
            CountryLabel.Padding = new System.Windows.Thickness(10);

            TextBox CountryText = new TextBox();
            RegisterName("CountryText", CountryText);
            CountryText.Margin = new System.Windows.Thickness(20, 0, 0, 0);
            CountryText.Width = 200;
            CountryText.Height = 25;
            CountryText.Padding = new Thickness(0, 0, 0, 5);

            Panel4.Children.Add(CountryLabel);
            Panel4.Children.Add(CountryText);
            CountryText.Text = company_Info.GetCompanyCountry();
            COL1.Children.Add(Panel4);

            //Company state 
            WrapPanel Panel5 = new WrapPanel();
            Label StateLabel = new Label();
            RegisterName("StateLabel", StateLabel);
            StateLabel.Width = 200;
            StateLabel.Margin = new System.Windows.Thickness(10, 0, 0, 0);
            StateLabel.Content = "State: ";
            StateLabel.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            StateLabel.FontWeight = FontWeights.Bold;
            StateLabel.FontSize = 14;
            StateLabel.Padding = new System.Windows.Thickness(10);

            TextBox StateText = new TextBox();
            RegisterName("StateLabelText", StateText);
            StateText.Margin = new System.Windows.Thickness(20, 0, 0, 0);
            StateText.Width = 200;
            StateText.Height = 25;
            StateText.Padding = new Thickness(0, 0, 0, 5);

            Panel5.Children.Add(StateLabel);
            Panel5.Children.Add(StateText);
            StateText.Text = company_Info.GetCompanyState();
            COL1.Children.Add(Panel5);

            //company city
            WrapPanel Panel6 = new WrapPanel();
            Label CityLabel = new Label();
            RegisterName("CityLabel", CityLabel);
            CityLabel.Width = 200;
            CityLabel.Margin = new System.Windows.Thickness(10, 0, 0, 0);
            CityLabel.Content = "City: ";
            CityLabel.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            CityLabel.FontWeight = FontWeights.Bold;
            CityLabel.FontSize = 14;
            CityLabel.Padding = new System.Windows.Thickness(10);

            TextBox CityText = new TextBox();
            RegisterName("CityLabelText", CityText);
            CityText.Margin = new System.Windows.Thickness(20, 0, 0, 0);
            CityText.Width = 200;
            CityText.Height = 25;
            CityText.Padding = new Thickness(0, 0, 0, 5);

            Panel6.Children.Add(CityLabel);
            Panel6.Children.Add(CityText);
            CityText.Text = company_Info.GetCompanyCity();
            COL1.Children.Add(Panel6);

            //company District
            WrapPanel Panel7 = new WrapPanel();
            Label DistrictLabel = new Label();
            RegisterName("DistrictLabel", DistrictLabel);
            DistrictLabel.Width = 200;
            DistrictLabel.Margin = new System.Windows.Thickness(10, 0, 0, 0);
            DistrictLabel.Content = "District: ";
            DistrictLabel.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
            DistrictLabel.FontWeight = FontWeights.Bold;
            DistrictLabel.FontSize = 14;
            DistrictLabel.Padding = new System.Windows.Thickness(10);

            TextBox DistrictText = new TextBox();
            RegisterName("DistrictLabelText", DistrictText);
            DistrictText.Margin = new System.Windows.Thickness(20, 0, 0, 0);
            DistrictText.Width = 200;
            DistrictText.Height = 25;
            DistrictText.Padding = new Thickness(0, 0, 0, 5);

            Panel7.Children.Add(DistrictLabel);
            Panel7.Children.Add(DistrictText);
            DistrictText.Text = company_Info.GetCompanyDistrict();
            COL1.Children.Add(Panel7);

            //Company telephone
            List<string> company_Telephones = new List<string>();
            company_Info.QueryCompanyPhones();
            company_Telephones = company_Info.GetCompanyPhones();
            for (int i = 0; i < company_Info.GetNumberOfSavedCompanyPhones(); i++)
            {
                WrapPanel Panel8 = new WrapPanel();
                Label CompanyTelephoneLabel = new Label();
                RegisterName("CompanyTelephoneLabel", CompanyTelephoneLabel);
                CompanyTelephoneLabel.Width = 200;
                CompanyTelephoneLabel.Margin = new System.Windows.Thickness(10, 0, 0, 0);
                CompanyTelephoneLabel.Content = "Company Telephone # " + (i + 1) + ": ";
                CompanyTelephoneLabel.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
                CompanyTelephoneLabel.FontWeight = FontWeights.Bold;
                CompanyTelephoneLabel.FontSize = 14;
                CompanyTelephoneLabel.Padding = new System.Windows.Thickness(10);

                TextBox CompanyTelephoneText = new TextBox();
                RegisterName("CompanyTelephoneText", CompanyTelephoneText);
                CompanyTelephoneText.Margin = new System.Windows.Thickness(20, 0, 0, 0);
                CompanyTelephoneText.Width = 200;
                CompanyTelephoneText.Height = 25;
                CompanyTelephoneText.Padding = new Thickness(0, 0, 0, 5);

                Panel8.Children.Add(CompanyTelephoneLabel);
                Panel8.Children.Add(CompanyTelephoneText);
                CompanyTelephoneText.Text = company_Telephones[i];
                COL1.Children.Add(Panel8);

            }

            // Company faxes
            List<string> company_Faxes = new List<string>();
            company_Faxes = company_Info.GetCompanyFaxes();
            for (int i = 0; i < company_Info.GetNumberOfSavedCompanyFaxes(); i++)
            {
                WrapPanel Panel9 = new WrapPanel();
                Label CompanyFaxesLabel = new Label();
                RegisterName("CompanyTelephoneLabel", CompanyFaxesLabel);
                CompanyFaxesLabel.Width = 200;
                CompanyFaxesLabel.Margin = new System.Windows.Thickness(10, 0, 0, 0);
                CompanyFaxesLabel.Content = "Company Telephone # " + (i + 1) + ": ";
                CompanyFaxesLabel.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#105A97"));
                CompanyFaxesLabel.FontWeight = FontWeights.Bold;
                CompanyFaxesLabel.FontSize = 14;
                CompanyFaxesLabel.Padding = new System.Windows.Thickness(10);

                TextBox CompanyFaxesText = new TextBox();
                RegisterName("CompanyFaxesText", CompanyFaxesText);
                CompanyFaxesText.Margin = new System.Windows.Thickness(20, 0, 0, 0);
                CompanyFaxesText.Width = 200;
                CompanyFaxesText.Height = 25;
                CompanyFaxesText.Padding = new Thickness(0, 0, 0, 5);

                Panel9.Children.Add(CompanyFaxesLabel);
                Panel9.Children.Add(CompanyFaxesText);
                CompanyFaxesText.Text = company_Faxes[i];
                COL1.Children.Add(Panel9);

            }


        }

        private void DoneBtn(object sender, RoutedEventArgs e)
        {

        }
    }
}
