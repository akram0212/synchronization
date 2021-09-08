using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Web;
using System.IO;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using Spire.Doc.Interface;
using Spire.Doc.Utilities;
using Spire.Doc.Collections;
using _01electronics_library;
using System.Drawing;

using Size = System.Drawing.Size;
using System.Windows.Media.Imaging;

namespace _01electronics_crm
{

    public class WordAutomation
    {
        SQLServer sqlServer;
        FTPServer ftpServer;

        WorkOffer workOffer;
        Document doc;

        String wordFilePath;

        CommonQueries commonQueriesObject;
        CommonFunctions commonFunctionsObject;
        

        List<string> standardFeatures = new List<string>();
        List<string> applications = new List<string>();
        List<string> benefits = new List<string>();
        List<string> imagePaths = new List<string>();

        int counter;
        decimal totalPrice;

        public WordAutomation()
        {
            sqlServer = new SQLServer();
            ftpServer = new FTPServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();
            doc = new Document();
        }

        public void AutomateWorkOffer(WorkOffer mWorkOffer)
        {
            workOffer = mWorkOffer;

            wordFilePath = Directory.GetCurrentDirectory() + "\\" + workOffer.GetOfferID() + ".doc";

            counter = 0;

            if (ftpServer.DownloadFile(BASIC_MACROS.MODELS_OFFERS_PATH + workOffer.GetNoOfOfferSavedProducts() + ".doc", wordFilePath))
            {
                doc.LoadFromFile(wordFilePath);

                totalPrice = 0;

                ISection section = doc.Sections[0];

                ///////////////////////////////////////////////////////////////////////////////////////////
                /////////////COMPANY & CONTACT TABLE
                //////////////////////////////////////////////////////////////////////////////////////////
                ITable CompanyContactTable = section.Tables[counter];
                counter++;

                Spire.Doc.TableCell companyNameCell = CompanyContactTable.Rows[0].Cells[1];
                IParagraph companyNameParagraph = companyNameCell.Paragraphs[0];
                companyNameParagraph.Text = workOffer.GetCompanyName();

                Spire.Doc.TableCell contactNameCell = CompanyContactTable.Rows[1].Cells[1];
                IParagraph contactNameParagraph = contactNameCell.Paragraphs[0];
                contactNameParagraph.Text = workOffer.GetContactName();


                ///////////////////////////////////////////////////////////////////////////////////////////
                /////////////OFFER INFO TABLE
                //////////////////////////////////////////////////////////////////////////////////////////
                ITable offerInfoTable = section.Tables[counter];
                counter++;

                Spire.Doc.TableCell issueDateCell = offerInfoTable.Rows[0].Cells[1];
                IParagraph issueDateParagraph = issueDateCell.Paragraphs[0];
                issueDateParagraph.Text = workOffer.GetOfferIssueDate().ToString("yyyy-MM-dd");

                Spire.Doc.TableCell offerIDCell = offerInfoTable.Rows[1].Cells[1];
                IParagraph offerIDParagraph = offerIDCell.Paragraphs[0];
                offerIDParagraph.Text = workOffer.GetOfferID();

                /////////////////////
                ///TYPE,BRAND,MODEL AND QUANTITY TABLE
                ////////////////////

                ITable itemsTable = section.Tables[counter];
                counter++;

                for (int i = 0; i < workOffer.GetNoOfOfferSavedProducts(); i++)
                {
                    Spire.Doc.TableCell productTypeCell = itemsTable.Rows[i + 1].Cells[1];
                    IParagraph productTypeParagraph = productTypeCell.Paragraphs[0];
                    productTypeParagraph.Text = workOffer.GetOfferProductType(i + 1);

                    Spire.Doc.TableCell brandModelCell = itemsTable.Rows[i + 1].Cells[2];
                    IParagraph brandModelParagraph = brandModelCell.Paragraphs[0];
                    brandModelParagraph.Text = workOffer.GetOfferProductBrand(i + 1) + "/" + workOffer.GetOfferProductModel(i + 1);

                    Spire.Doc.TableCell quantityCell = itemsTable.Rows[i + 1].Cells[3];
                    IParagraph quantityParagraph = quantityCell.Paragraphs[0];
                    quantityParagraph.Text = workOffer.GetOfferProductQuantity(i + 1).ToString();
                }
                ///////////////////////////////////////////////////////////////////////////////////////////
                /////////////IMAGE TABLE
                //////////////////////////////////////////////////////////////////////////////////////////
                for (int i = 0; i < workOffer.GetNoOfOfferSavedProducts(); i++)
                {
                    ITable imageTable = section.Tables[counter];
                    counter++;

                    Spire.Doc.TableCell imageCell = imageTable.Rows[0].Cells[0];
                    IParagraph imageParagraph = imageCell.Paragraphs[0];

                    string imagePath = Directory.GetCurrentDirectory() + "/" + workOffer.GetOfferProductModel(i + 1) + ".jpg";
                    imagePaths.Add(imagePath);
                    if (ftpServer.DownloadFile(BASIC_MACROS.MODELS_PHOTOS_PATH + workOffer.GetOfferProductTypeId(i + 1) + "/" + workOffer.GetOfferProductBrandId(i + 1) + "/" + workOffer.GetOfferProductModelId(i + 1) + ".jpg", imagePath))
                    {
                        //BitmapImage src = new BitmapImage();
                        //src.BeginInit();
                        //src.UriSource = new Uri(imagePath, UriKind.Absolute);
                        //src.CacheOption = BitmapCacheOption.OnLoad;
                        //src.EndInit();
                        
                        Image productImage = Image.FromFile(imagePath);
                        
                        productImage = resizeImage(productImage, new Size(200, 200));
                        
                        //byte[] byteImage = (byte[])(new ImageConverter()).ConvertTo(productImage, typeof(byte[]));

                        imageParagraph.AppendPicture(productImage);

                        //File.Delete(imagePath);
                    }
                }

                ///////////////////////////////////////////////////////////////////////////////////////////
                /////////////SPECS TABLE
                //////////////////////////////////////////////////////////////////////////////////////////
                ///


                for (int i = 0; i < workOffer.GetNoOfOfferSavedProducts(); i++)
                {
                    ITable specsTableHeader = section.Tables[counter];
                    counter++;

                    Spire.Doc.TableCell headerCell = specsTableHeader.Rows[0].Cells[0];
                    IParagraph headerParagraph = headerCell.Paragraphs[0];
                    headerParagraph.Text = workOffer.GetOfferProductType(i + 1) + "-" + workOffer.GetOfferProductBrand(i + 1) + "-" + workOffer.GetOfferProductModel(i + 1);

                    commonQueriesObject.GetModelFeatures(workOffer.GetOfferProductTypeId(i + 1), workOffer.GetOfferProductBrandId(i + 1), workOffer.GetOfferProductModelId(i + 1), ref standardFeatures);

                    commonQueriesObject.GetModelApplications(workOffer.GetOfferProductTypeId(i + 1), workOffer.GetOfferProductBrandId(i + 1), workOffer.GetOfferProductModelId(i + 1), ref applications);

                    commonQueriesObject.GetModelBenefits(workOffer.GetOfferProductTypeId(i + 1), workOffer.GetOfferProductBrandId(i + 1), workOffer.GetOfferProductModelId(i + 1), ref benefits);

                    //int a = workOffer.GetOfferProductTypeId(i + 1);
                    //int b = workOffer.GetOfferProductBrandId(i + 1);
                    //int c = workOffer.GetOfferProductModelId(i + 1);

                    ITable table4 = section.Tables[counter];
                    counter++;

                    for (int j = 0; j < standardFeatures.Count(); j++)
                    {
                        Spire.Doc.TableCell currentCell = table4.Rows[j].Cells[1];
                        IParagraph currentParagraph = currentCell.Paragraphs[0];
                        currentParagraph.Text = standardFeatures[j];
                    }

                    for (int j = 7; j < applications.Count() + 7; j++)
                    {
                        Spire.Doc.TableCell currentCell = table4.Rows[j].Cells[1];
                        IParagraph currentParagraph = currentCell.Paragraphs[0];
                        currentParagraph.Text = applications[j - 7];
                    }

                    for (int j = 12; j < benefits.Count() + 12; j++)
                    {
                        Spire.Doc.TableCell currentCell = table4.Rows[j].Cells[1];
                        IParagraph currentParagraph = currentCell.Paragraphs[0];
                        currentParagraph.Text = benefits[j - 12];
                    }
                }

                counter = counter + workOffer.GetNoOfOfferSavedProducts() + 1;
                ///////////////////////////////////////////////////////////////////////////////////////////
                /////////////COMMERTIAL OFFER TABLE
                //////////////////////////////////////////////////////////////////////////////////////////

                ITable commercialOfferTable = section.Tables[counter];
                counter++;

                for (int i = 0; i <= workOffer.GetNoOfOfferSavedProducts(); i++)
                {
                    if (i != workOffer.GetNoOfOfferSavedProducts())
                    {
                        Spire.Doc.TableCell product1 = commercialOfferTable.Rows[i + 1].Cells[1];
                        IParagraph productDescriptionParagraph = product1.Paragraphs[0];
                        productDescriptionParagraph.Text = workOffer.GetOfferProductType(i + 1) + "/" + workOffer.GetOfferProductBrand(i + 1) + "/" + workOffer.GetOfferProductModel(i + 1);

                        Spire.Doc.TableCell productQuantityCell = commercialOfferTable.Rows[i + 1].Cells[2];
                        IParagraph productQuantityParagraph = productQuantityCell.Paragraphs[0];
                        productQuantityParagraph.Text = workOffer.GetOfferProductQuantity(i + 1).ToString();

                        Spire.Doc.TableCell productPriceCell = commercialOfferTable.Rows[i + 1].Cells[3];
                        IParagraph productPriceParagraph = productPriceCell.Paragraphs[0];
                        productPriceParagraph.Text = workOffer.GetProductPriceValue(i + 1).ToString() + "  " + workOffer.GetCurrency();

                        Spire.Doc.TableCell productTotalCell = commercialOfferTable.Rows[i + 1].Cells[4];
                        IParagraph productTotalParagraph = productTotalCell.Paragraphs[0];
                        Decimal total = workOffer.GetProductPriceValue(i + 1) * workOffer.GetOfferProductQuantity(i + 1);
                        productTotalParagraph.Text = total.ToString() + "  " + workOffer.GetCurrency();
                        totalPrice += total;
                    }
                    else
                    {
                        Spire.Doc.TableCell totalCell = commercialOfferTable.Rows[i + 1].Cells[4];
                        IParagraph totalParagraph = totalCell.Paragraphs[0];
                        totalParagraph.Text = totalPrice.ToString() + "  " + workOffer.GetCurrency();
                    }
                }

                counter++;
                ///////////////////////////////////////////////////////////////////////////////////////////
                /////////////DELIVERY & PAYMENT TABLE
                //////////////////////////////////////////////////////////////////////////////////////////


                ITable deliveryPaymentTable = section.Tables[counter];
                counter++;

                Spire.Doc.TableCell deliveryTimeCell = deliveryPaymentTable.Rows[0].Cells[1];
                IParagraph deliveryTimeParagraph = deliveryTimeCell.Paragraphs[0];
                string deliveryTime = workOffer.GetDeliveryTimeMinimum().ToString() + "-" + workOffer.GetDeliveryTimeMaximum().ToString() + "   " + workOffer.GetDeliveryTimeUnit();
                deliveryTimeParagraph.Text = deliveryTime;


                Spire.Doc.TableCell deliveryPointCell = deliveryPaymentTable.Rows[1].Cells[1];
                IParagraph deliveryPointParagraph = deliveryPointCell.Paragraphs[0];
                deliveryPointParagraph.Text = workOffer.GetDeliveryPoint();

                Spire.Doc.TableCell paymentCell = deliveryPaymentTable.Rows[2].Cells[1];
                IParagraph paymentParagraph = paymentCell.Paragraphs[0];
                paymentParagraph.Text = workOffer.GetPercentDownPayment() + "% DOWNPAYMENT  " + workOffer.GetPercentOnDelivery() + "% ONDELIVERY  " + workOffer.GetPercentOnInstallation() + "% ONINSTALLATION";

                Spire.Doc.TableCell contractTypeCell = deliveryPaymentTable.Rows[3].Cells[1];
                IParagraph contractTypeParagraph = contractTypeCell.Paragraphs[0];
                contractTypeParagraph.Text = workOffer.GetOfferContractType();

                Spire.Doc.TableCell warrantyCell = deliveryPaymentTable.Rows[4].Cells[1];
                IParagraph warrantyParagraph = warrantyCell.Paragraphs[0];
                warrantyParagraph.Text = workOffer.GetWarrantyPeriod() + " " + workOffer.GetWarrantyPeriodTimeUnit();

                Spire.Doc.TableCell offerValidityCell = deliveryPaymentTable.Rows[5].Cells[1];
                IParagraph offerValidityParagraph = offerValidityCell.Paragraphs[0];
                offerValidityParagraph.Text = workOffer.GetOfferValidityPeriod() + " " + workOffer.GetOfferValidityTimeUnit();


                ///////////////////////////////////////////////////////////////////////////////////////////
                /////////////TECH OFFICE REP TABLE
                ///////////////////////////////////////////////////////////////////////////////////////////
                ITable techOfficeRepTable = section.Tables[counter];
                counter++;

                if (workOffer.GetRFQSerial() != 0)
                {
                    Spire.Doc.TableCell techOfficeRepNameCell = techOfficeRepTable.Rows[1].Cells[0];
                    IParagraph techOfficeRepNameParagraph = techOfficeRepNameCell.Paragraphs[0];
                    techOfficeRepNameParagraph.Text = workOffer.GetAssigneeName();

                    Spire.Doc.TableCell techOfficeRepPositionCell = techOfficeRepTable.Rows[2].Cells[0];
                    IParagraph techOfficeRepPositionParagraph = techOfficeRepPositionCell.Paragraphs[0];
                    techOfficeRepPositionParagraph.Text = workOffer.GetAssigneeTeam() + "  " + workOffer.GetAssigneePosition();

                    Spire.Doc.TableCell techOfficeRepEmailCell = techOfficeRepTable.Rows[3].Cells[0];
                    IParagraph techOfficeRepEmailParagraph = techOfficeRepEmailCell.Paragraphs[0];
                    techOfficeRepEmailParagraph.Text = workOffer.GetRFQAssignee().GetEmployeeBusinessEmail();

                    Spire.Doc.TableCell techOfficeRepNumberCell = techOfficeRepTable.Rows[4].Cells[0];
                    IParagraph techOfficeRepNumberParagraph = techOfficeRepNumberCell.Paragraphs[0];
                    techOfficeRepNumberParagraph.Text = "Mob. " + workOffer.GetRFQAssignee().GetEmployeeBusinessPhone();
                }

                else
                {
                    Spire.Doc.TableCell techOfficeRepNameCell = techOfficeRepTable.Rows[1].Cells[0];
                    IParagraph techOfficeRepNameParagraph = techOfficeRepNameCell.Paragraphs[0];
                    techOfficeRepNameParagraph.Text = workOffer.GetOfferProposerName();

                    Spire.Doc.TableCell techOfficeRepPositionCell = techOfficeRepTable.Rows[2].Cells[0];
                    IParagraph techOfficeRepPositionParagraph = techOfficeRepPositionCell.Paragraphs[0];
                    techOfficeRepPositionParagraph.Text = workOffer.GetOfferProposerTeam() + "  " + workOffer.GetOfferProposerPosition();

                    Spire.Doc.TableCell techOfficeRepEmailCell = techOfficeRepTable.Rows[3].Cells[0];
                    IParagraph techOfficeRepEmailParagraph = techOfficeRepEmailCell.Paragraphs[0];
                    techOfficeRepEmailParagraph.Text = workOffer.GetOfferProposerbusinessEmail();

                    Spire.Doc.TableCell techOfficeRepNumberCell = techOfficeRepTable.Rows[4].Cells[0];
                    IParagraph techOfficeRepNumberParagraph = techOfficeRepNumberCell.Paragraphs[0];
                    techOfficeRepNumberParagraph.Text = "Mob. " + workOffer.GetOfferProposerCompanyPhone();
                }
                ///////////////////////////////////////////////////////////////////////////////////////////
                /////////////SALES REP TABLE
                //////////////////////////////////////////////////////////////////////////////////////////
                ITable salesRepTable = section.Tables[counter];

                Spire.Doc.TableCell salesRepNameCell = salesRepTable.Rows[1].Cells[0];
                IParagraph salesRepNameParagraph = salesRepNameCell.Paragraphs[0];
                salesRepNameParagraph.Text = workOffer.GetSalesPersonName();

                Spire.Doc.TableCell salesRepPositionCell = salesRepTable.Rows[2].Cells[0];
                IParagraph salesRepPositionParagraph = salesRepPositionCell.Paragraphs[0];
                salesRepPositionParagraph.Text = workOffer.GetSalesPersonTeam() + "  " + workOffer.GetSalesPersonPosition();

                Spire.Doc.TableCell salesRepEmailCell = salesRepTable.Rows[3].Cells[0];
                IParagraph salesRepEmailParagraph = salesRepEmailCell.Paragraphs[0];
                salesRepEmailParagraph.Text = workOffer.GetSalesPersonbusinessEmail();

                Spire.Doc.TableCell salesRepNumberCell = salesRepTable.Rows[4].Cells[0];
                IParagraph salesRepNumberParagraph = salesRepNumberCell.Paragraphs[0];
                salesRepNumberParagraph.Text = "Mob. " + workOffer.GetSalesPersonCompanyPhone();

                doc.SaveToFile(wordFilePath);

                System.Diagnostics.Process.Start(wordFilePath);
            }
        }

        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }

    }
}
