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
using Spire.Doc.Collections;
using _01electronics_library;
using System.Drawing;

using Size = System.Drawing.Size;
using System.Windows.Media.Imaging;
using _01electronics_windows_library;
using System.Windows.Forms;

namespace _01electronics_crm
{

    public class WordAutomation
    {
        SQLServer sqlServer;
        FTPServer ftpServer;

        Quotation quotation;
        WorkOrder workOrder;
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

        protected String errorMessage;

        public WordAutomation()
        {
            sqlServer = new SQLServer();
            ftpServer = new FTPServer();
            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();
            doc = new Document();
        }

        public void AutomateWorkOffer(Quotation mWorkOffer)
        {
            quotation = mWorkOffer;

            wordFilePath = Directory.GetCurrentDirectory() + "\\" + quotation.GetOfferID() + ".doc";

            counter = 0;

            if (ftpServer.DownloadFile(BASIC_MACROS.MODELS_OUTGOING_QUOTATION_PATH + quotation.GetNoOfOfferSavedProducts() + ".doc", wordFilePath, BASIC_MACROS.SEVERITY_LOW, ref errorMessage))
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
                companyNameParagraph.Text = quotation.GetCompanyName();

                Spire.Doc.TableCell contactNameCell = CompanyContactTable.Rows[1].Cells[1];
                IParagraph contactNameParagraph = contactNameCell.Paragraphs[0];
                contactNameParagraph.Text = quotation.GetContactName();


                ///////////////////////////////////////////////////////////////////////////////////////////
                /////////////OUTGOING_QUOTATION INFO TABLE
                //////////////////////////////////////////////////////////////////////////////////////////
                ITable offerInfoTable = section.Tables[counter];
                counter++;

                Spire.Doc.TableCell issueDateCell = offerInfoTable.Rows[0].Cells[1];
                IParagraph issueDateParagraph = issueDateCell.Paragraphs[0];
                issueDateParagraph.Text = quotation.GetOfferIssueDate().ToString("yyyy-MM-dd");

                Spire.Doc.TableCell offerIDCell = offerInfoTable.Rows[1].Cells[1];
                IParagraph offerIDParagraph = offerIDCell.Paragraphs[0];
                offerIDParagraph.Text = quotation.GetOfferID();

                /////////////////////
                ///TYPE,BRAND,MODEL AND QUANTITY TABLE
                ////////////////////

                ITable itemsTable = section.Tables[counter];
                counter++;

                for (int i = 0; i < quotation.GetNoOfOfferSavedProducts(); i++)
                {
                    Spire.Doc.TableCell productTypeCell = itemsTable.Rows[i + 1].Cells[1];
                    IParagraph productTypeParagraph = productTypeCell.Paragraphs[0];
                    productTypeParagraph.Text = quotation.GetOfferProductType(i + 1);

                    Spire.Doc.TableCell brandModelCell = itemsTable.Rows[i + 1].Cells[2];
                    IParagraph brandModelParagraph = brandModelCell.Paragraphs[0];
                    brandModelParagraph.Text = quotation.GetOfferProductBrand(i + 1) + "/" + quotation.GetOfferProductModel(i + 1);

                    Spire.Doc.TableCell quantityCell = itemsTable.Rows[i + 1].Cells[3];
                    IParagraph quantityParagraph = quantityCell.Paragraphs[0];
                    quantityParagraph.Text = quotation.GetOfferProductQuantity(i + 1).ToString();
                }
                ///////////////////////////////////////////////////////////////////////////////////////////
                /////////////IMAGE TABLE
                //////////////////////////////////////////////////////////////////////////////////////////
                for (int i = 0; i < quotation.GetNoOfOfferSavedProducts(); i++)
                {
                    ITable imageTable = section.Tables[counter];
                    counter++;

                    Spire.Doc.TableCell imageCell = imageTable.Rows[0].Cells[0];
                    IParagraph imageParagraph = imageCell.Paragraphs[0];

                    string imagePath = Directory.GetCurrentDirectory() + "/" + quotation.GetOfferProductModel(i + 1) + ".jpg";
                    imagePaths.Add(imagePath);
                    if (ftpServer.DownloadFile(BASIC_MACROS.MODELS_PHOTOS_PATH + quotation.GetOfferProductTypeId(i + 1) + "/" + quotation.GetOfferProductBrandId(i + 1) + "/" + quotation.GetOfferProductModelId(i + 1) + ".jpg", imagePath, BASIC_MACROS.SEVERITY_HIGH, ref errorMessage))
                    {
                        //BitmapImage src = new BitmapImage();
                        //src.BeginInit();
                        //src.UriSource = new Uri(imagePath, UriKind.Absolute);
                        //src.CacheOption = BitmapCacheOption.OnLoad;
                        //src.EndInit();
                        
                        Image productImage = Image.FromFile(imagePath);
                        
                        //productImage = resizeImage(productImage, new Size(200, 200));
                        
                        //byte[] byteImage = (byte[])(new ImageConverter()).ConvertTo(productImage, typeof(byte[]));

                        imageParagraph.AppendPicture(productImage);

                        //File.Delete(imagePath);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                ///////////////////////////////////////////////////////////////////////////////////////////
                /////////////SPECS TABLE
                //////////////////////////////////////////////////////////////////////////////////////////
                ///


                for (int i = 0; i < quotation.GetNoOfOfferSavedProducts(); i++)
                {
                    ITable specsTableHeader = section.Tables[counter];
                    counter++;

                    Spire.Doc.TableCell headerCell = specsTableHeader.Rows[0].Cells[0];
                    IParagraph headerParagraph = headerCell.Paragraphs[0];
                    headerParagraph.Text = quotation.GetOfferProductType(i + 1) + "-" + quotation.GetOfferProductBrand(i + 1) + "-" + quotation.GetOfferProductModel(i + 1);

                    commonQueriesObject.GetModelFeatures(quotation.GetOfferProductTypeId(i + 1), quotation.GetOfferProductBrandId(i + 1), quotation.GetOfferProductModelId(i + 1), ref standardFeatures);

                    commonQueriesObject.GetModelApplications(quotation.GetOfferProductTypeId(i + 1), quotation.GetOfferProductBrandId(i + 1), quotation.GetOfferProductModelId(i + 1), ref applications);

                    commonQueriesObject.GetModelBenefits(quotation.GetOfferProductTypeId(i + 1), quotation.GetOfferProductBrandId(i + 1), quotation.GetOfferProductModelId(i + 1), ref benefits);

                    //int a = quotation.GetOfferProductTypeId(i + 1);
                    //int b = quotation.GetOfferProductBrandId(i + 1);
                    //int c = quotation.GetOfferProductModelId(i + 1);

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

                counter = counter + quotation.GetNoOfOfferSavedProducts() + 1;
                ///////////////////////////////////////////////////////////////////////////////////////////
                /////////////COMMERTIAL OUTGOING_QUOTATION TABLE
                //////////////////////////////////////////////////////////////////////////////////////////

                ITable commercialOfferTable = section.Tables[counter];
                counter++;

                for (int i = 0; i <= quotation.GetNoOfOfferSavedProducts(); i++)
                {
                    if (i != quotation.GetNoOfOfferSavedProducts())
                    {
                        Spire.Doc.TableCell product1 = commercialOfferTable.Rows[i + 1].Cells[1];
                        IParagraph productDescriptionParagraph = product1.Paragraphs[0];
                        productDescriptionParagraph.Text = quotation.GetOfferProductType(i + 1) + "/" + quotation.GetOfferProductBrand(i + 1) + "/" + quotation.GetOfferProductModel(i + 1);

                        Spire.Doc.TableCell productQuantityCell = commercialOfferTable.Rows[i + 1].Cells[2];
                        IParagraph productQuantityParagraph = productQuantityCell.Paragraphs[0];
                        productQuantityParagraph.Text = quotation.GetOfferProductQuantity(i + 1).ToString();

                        Spire.Doc.TableCell productPriceCell = commercialOfferTable.Rows[i + 1].Cells[3];
                        IParagraph productPriceParagraph = productPriceCell.Paragraphs[0];
                        productPriceParagraph.Text = quotation.GetProductPriceValue(i + 1).ToString() + "  " + quotation.GetCurrency();

                        Spire.Doc.TableCell productTotalCell = commercialOfferTable.Rows[i + 1].Cells[4];
                        IParagraph productTotalParagraph = productTotalCell.Paragraphs[0];
                        Decimal total = quotation.GetProductPriceValue(i + 1) * quotation.GetOfferProductQuantity(i + 1);
                        productTotalParagraph.Text = total.ToString() + "  " + quotation.GetCurrency();
                        totalPrice += total;
                    }
                    else
                    {
                        Spire.Doc.TableCell totalCell = commercialOfferTable.Rows[i + 1].Cells[4];
                        IParagraph totalParagraph = totalCell.Paragraphs[0];
                        totalParagraph.Text = totalPrice.ToString() + "  " + quotation.GetCurrency();
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
                string deliveryTime = quotation.GetDeliveryTimeMinimum().ToString() + "-" + quotation.GetDeliveryTimeMaximum().ToString() + "   " + quotation.GetDeliveryTimeUnit();
                deliveryTimeParagraph.Text = deliveryTime;


                Spire.Doc.TableCell deliveryPointCell = deliveryPaymentTable.Rows[1].Cells[1];
                IParagraph deliveryPointParagraph = deliveryPointCell.Paragraphs[0];
                deliveryPointParagraph.Text = quotation.GetDeliveryPoint();

                Spire.Doc.TableCell paymentCell = deliveryPaymentTable.Rows[2].Cells[1];
                IParagraph paymentParagraph = paymentCell.Paragraphs[0];
                paymentParagraph.Text = quotation.GetPercentDownPayment() + "% DOWNPAYMENT  " + quotation.GetPercentOnDelivery() + "% ONDELIVERY  " + quotation.GetPercentOnInstallation() + "% ONINSTALLATION";

                Spire.Doc.TableCell contractTypeCell = deliveryPaymentTable.Rows[3].Cells[1];
                IParagraph contractTypeParagraph = contractTypeCell.Paragraphs[0];
                contractTypeParagraph.Text = quotation.GetOfferContractType();

                Spire.Doc.TableCell warrantyCell = deliveryPaymentTable.Rows[4].Cells[1];
                IParagraph warrantyParagraph = warrantyCell.Paragraphs[0];
                warrantyParagraph.Text = quotation.GetWarrantyPeriod() + " " + quotation.GetWarrantyPeriodTimeUnit();

                Spire.Doc.TableCell offerValidityCell = deliveryPaymentTable.Rows[5].Cells[1];
                IParagraph offerValidityParagraph = offerValidityCell.Paragraphs[0];
                offerValidityParagraph.Text = quotation.GetOfferValidityPeriod() + " " + quotation.GetOfferValidityTimeUnit();


                ///////////////////////////////////////////////////////////////////////////////////////////
                /////////////TECH OFFICE REP TABLE
                ///////////////////////////////////////////////////////////////////////////////////////////
                ITable techOfficeRepTable = section.Tables[counter];
                counter++;

                if (quotation.GetRFQSerial() != 0)
                {
                    Spire.Doc.TableCell techOfficeRepNameCell = techOfficeRepTable.Rows[1].Cells[0];
                    IParagraph techOfficeRepNameParagraph = techOfficeRepNameCell.Paragraphs[0];
                    techOfficeRepNameParagraph.Text = quotation.GetAssigneeName();

                    Spire.Doc.TableCell techOfficeRepPositionCell = techOfficeRepTable.Rows[2].Cells[0];
                    IParagraph techOfficeRepPositionParagraph = techOfficeRepPositionCell.Paragraphs[0];
                    techOfficeRepPositionParagraph.Text = quotation.GetAssigneeTeam() + "  " + quotation.GetAssigneePosition();

                    Spire.Doc.TableCell techOfficeRepEmailCell = techOfficeRepTable.Rows[3].Cells[0];
                    IParagraph techOfficeRepEmailParagraph = techOfficeRepEmailCell.Paragraphs[0];
                    techOfficeRepEmailParagraph.Text = quotation.GetRFQAssignee().GetEmployeeBusinessEmail();

                    Spire.Doc.TableCell techOfficeRepNumberCell = techOfficeRepTable.Rows[4].Cells[0];
                    IParagraph techOfficeRepNumberParagraph = techOfficeRepNumberCell.Paragraphs[0];
                    techOfficeRepNumberParagraph.Text = "Mob. " + quotation.GetRFQAssignee().GetEmployeeBusinessPhone();
                }

                else
                {
                    Spire.Doc.TableCell techOfficeRepNameCell = techOfficeRepTable.Rows[1].Cells[0];
                    IParagraph techOfficeRepNameParagraph = techOfficeRepNameCell.Paragraphs[0];
                    techOfficeRepNameParagraph.Text = quotation.GetOfferProposerName();

                    Spire.Doc.TableCell techOfficeRepPositionCell = techOfficeRepTable.Rows[2].Cells[0];
                    IParagraph techOfficeRepPositionParagraph = techOfficeRepPositionCell.Paragraphs[0];
                    techOfficeRepPositionParagraph.Text = quotation.GetOfferProposerTeam() + "  " + quotation.GetOfferProposerPosition();

                    Spire.Doc.TableCell techOfficeRepEmailCell = techOfficeRepTable.Rows[3].Cells[0];
                    IParagraph techOfficeRepEmailParagraph = techOfficeRepEmailCell.Paragraphs[0];
                    techOfficeRepEmailParagraph.Text = quotation.GetOfferProposerbusinessEmail();

                    Spire.Doc.TableCell techOfficeRepNumberCell = techOfficeRepTable.Rows[4].Cells[0];
                    IParagraph techOfficeRepNumberParagraph = techOfficeRepNumberCell.Paragraphs[0];
                    techOfficeRepNumberParagraph.Text = "Mob. " + quotation.GetOfferProposerCompanyPhone();
                }
                ///////////////////////////////////////////////////////////////////////////////////////////
                /////////////SALES REP TABLE
                //////////////////////////////////////////////////////////////////////////////////////////
                ITable salesRepTable = section.Tables[counter];

                Spire.Doc.TableCell salesRepNameCell = salesRepTable.Rows[1].Cells[0];
                IParagraph salesRepNameParagraph = salesRepNameCell.Paragraphs[0];
                salesRepNameParagraph.Text = quotation.GetSalesPersonName();

                Spire.Doc.TableCell salesRepPositionCell = salesRepTable.Rows[2].Cells[0];
                IParagraph salesRepPositionParagraph = salesRepPositionCell.Paragraphs[0];
                salesRepPositionParagraph.Text = quotation.GetSalesPersonTeam() + "  " + quotation.GetSalesPersonPosition();

                Spire.Doc.TableCell salesRepEmailCell = salesRepTable.Rows[3].Cells[0];
                IParagraph salesRepEmailParagraph = salesRepEmailCell.Paragraphs[0];
                salesRepEmailParagraph.Text = quotation.GetSalesPersonbusinessEmail();

                Spire.Doc.TableCell salesRepNumberCell = salesRepTable.Rows[4].Cells[0];
                IParagraph salesRepNumberParagraph = salesRepNumberCell.Paragraphs[0];
                salesRepNumberParagraph.Text = "Mob. " + quotation.GetSalesPersonCompanyPhone();

                doc.SaveToFile(wordFilePath);

                System.Diagnostics.Process.Start(wordFilePath);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void AutomateWorkOrder(WorkOrder mWorkOrder)
        {
            workOrder = mWorkOrder;

            wordFilePath = Directory.GetCurrentDirectory() + "\\" + workOrder.GetOrderID() + ".doc";

            counter = 0;

            if (ftpServer.DownloadFile(BASIC_MACROS.MODELS_ORDERS_PATH + workOrder.GetNoOfOrderSavedProducts() + ".doc", wordFilePath, BASIC_MACROS.SEVERITY_LOW, ref errorMessage))
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
                companyNameParagraph.Text = workOrder.GetCompanyName();

                Spire.Doc.TableCell contactNameCell = CompanyContactTable.Rows[1].Cells[1];
                IParagraph contactNameParagraph = contactNameCell.Paragraphs[0];
                contactNameParagraph.Text = workOrder.GetContactName();


                ///////////////////////////////////////////////////////////////////////////////////////////
                /////////////Order INFO TABLE
                //////////////////////////////////////////////////////////////////////////////////////////
                ITable OrderInfoTable = section.Tables[counter];
                counter++;

                Spire.Doc.TableCell issueDateCell = OrderInfoTable.Rows[0].Cells[1];
                IParagraph issueDateParagraph = issueDateCell.Paragraphs[0];
                issueDateParagraph.Text = workOrder.GetOrderIssueDate().ToString("yyyy-MM-dd");

                Spire.Doc.TableCell OrderIDCell = OrderInfoTable.Rows[1].Cells[1];
                IParagraph OrderIDParagraph = OrderIDCell.Paragraphs[0];
                OrderIDParagraph.Text = workOrder.GetOrderID();

                /////////////////////
                ///TYPE,BRAND,MODEL AND QUANTITY TABLE
                ////////////////////

                ITable itemsTable = section.Tables[counter];
                counter++;

                for (int i = 0; i < workOrder.GetNoOfOrderSavedProducts(); i++)
                {
                    Spire.Doc.TableCell productTypeCell = itemsTable.Rows[i + 1].Cells[1];
                    IParagraph productTypeParagraph = productTypeCell.Paragraphs[0];
                    productTypeParagraph.Text = workOrder.GetOrderProductType(i + 1);

                    Spire.Doc.TableCell brandModelCell = itemsTable.Rows[i + 1].Cells[2];
                    IParagraph brandModelParagraph = brandModelCell.Paragraphs[0];
                    brandModelParagraph.Text = workOrder.GetOrderProductBrand(i + 1) + "/" + workOrder.GetOrderProductModel(i + 1);

                    Spire.Doc.TableCell quantityCell = itemsTable.Rows[i + 1].Cells[3];
                    IParagraph quantityParagraph = quantityCell.Paragraphs[0];
                    quantityParagraph.Text = workOrder.GetOrderProductQuantity(i + 1).ToString();
                }
                ///////////////////////////////////////////////////////////////////////////////////////////
                /////////////IMAGE TABLE
                //////////////////////////////////////////////////////////////////////////////////////////
                for (int i = 0; i < workOrder.GetNoOfOrderSavedProducts(); i++)
                {
                    ITable imageTable = section.Tables[counter];
                    counter++;

                    Spire.Doc.TableCell imageCell = imageTable.Rows[0].Cells[0];
                    IParagraph imageParagraph = imageCell.Paragraphs[0];

                    string imagePath = Directory.GetCurrentDirectory() + "/" + workOrder.GetOrderProductModel(i + 1) + ".jpg";
                    imagePaths.Add(imagePath);
                    if (ftpServer.DownloadFile(BASIC_MACROS.MODELS_PHOTOS_PATH + workOrder.GetOrderProductTypeId(i + 1) + "/" + workOrder.GetOrderProductBrandId(i + 1) + "/" + workOrder.GetOrderProductModelId(i + 1) + ".jpg", imagePath, BASIC_MACROS.SEVERITY_HIGH, ref errorMessage))
                    {
                        //BitmapImage src = new BitmapImage();
                        //src.BeginInit();
                        //src.UriSource = new Uri(imagePath, UriKind.Absolute);
                        //src.CacheOption = BitmapCacheOption.OnLoad;
                        //src.EndInit();

                        Image productImage = Image.FromFile(imagePath);

                        //productImage = resizeImage(productImage, new Size(200, 200));

                        //byte[] byteImage = (byte[])(new ImageConverter()).ConvertTo(productImage, typeof(byte[]));

                        imageParagraph.AppendPicture(productImage);

                        //File.Delete(imagePath);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                ///////////////////////////////////////////////////////////////////////////////////////////
                /////////////SPECS TABLE
                //////////////////////////////////////////////////////////////////////////////////////////
                ///


                for (int i = 0; i < workOrder.GetNoOfOrderSavedProducts(); i++)
                {
                    ITable specsTableHeader = section.Tables[counter];
                    counter++;

                    Spire.Doc.TableCell headerCell = specsTableHeader.Rows[0].Cells[0];
                    IParagraph headerParagraph = headerCell.Paragraphs[0];
                    headerParagraph.Text = workOrder.GetOrderProductType(i + 1) + "-" + workOrder.GetOrderProductBrand(i + 1) + "-" + workOrder.GetOrderProductModel(i + 1);

                    commonQueriesObject.GetModelFeatures(workOrder.GetOrderProductTypeId(i + 1), workOrder.GetOrderProductBrandId(i + 1), workOrder.GetOrderProductModelId(i + 1), ref standardFeatures);

                    commonQueriesObject.GetModelApplications(workOrder.GetOrderProductTypeId(i + 1), workOrder.GetOrderProductBrandId(i + 1), workOrder.GetOrderProductModelId(i + 1), ref applications);

                    commonQueriesObject.GetModelBenefits(workOrder.GetOrderProductTypeId(i + 1), workOrder.GetOrderProductBrandId(i + 1), workOrder.GetOrderProductModelId(i + 1), ref benefits);

                    //int a = workOrder.GetOrderProductTypeId(i + 1);
                    //int b = workOrder.GetOrderProductBrandId(i + 1);
                    //int c = workOrder.GetOrderProductModelId(i + 1);

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

                counter = counter + workOrder.GetNoOfOrderSavedProducts() + 1;
                ///////////////////////////////////////////////////////////////////////////////////////////
                /////////////COMMERTIAL Order TABLE
                //////////////////////////////////////////////////////////////////////////////////////////

                ITable commercialOrderTable = section.Tables[counter];
                counter++;

                for (int i = 0; i <= workOrder.GetNoOfOrderSavedProducts(); i++)
                {
                    if (i != workOrder.GetNoOfOrderSavedProducts())
                    {
                        Spire.Doc.TableCell product1 = commercialOrderTable.Rows[i + 1].Cells[1];
                        IParagraph productDescriptionParagraph = product1.Paragraphs[0];
                        productDescriptionParagraph.Text = workOrder.GetOrderProductType(i + 1) + "/" + workOrder.GetOrderProductBrand(i + 1) + "/" + workOrder.GetOrderProductModel(i + 1);

                        Spire.Doc.TableCell productQuantityCell = commercialOrderTable.Rows[i + 1].Cells[2];
                        IParagraph productQuantityParagraph = productQuantityCell.Paragraphs[0];
                        productQuantityParagraph.Text = workOrder.GetOrderProductQuantity(i + 1).ToString();

                        Spire.Doc.TableCell productPriceCell = commercialOrderTable.Rows[i + 1].Cells[3];
                        IParagraph productPriceParagraph = productPriceCell.Paragraphs[0];
                        productPriceParagraph.Text = workOrder.GetProductPriceValue(i + 1).ToString() + "  " + workOrder.GetCurrency();

                        Spire.Doc.TableCell productTotalCell = commercialOrderTable.Rows[i + 1].Cells[4];
                        IParagraph productTotalParagraph = productTotalCell.Paragraphs[0];
                        Decimal total = workOrder.GetProductPriceValue(i + 1) * workOrder.GetOrderProductQuantity(i + 1);
                        productTotalParagraph.Text = total.ToString() + "  " + workOrder.GetCurrency();
                        totalPrice += total;
                    }
                    else
                    {
                        Spire.Doc.TableCell totalCell = commercialOrderTable.Rows[i + 1].Cells[4];
                        IParagraph totalParagraph = totalCell.Paragraphs[0];
                        totalParagraph.Text = totalPrice.ToString() + "  " + workOrder.GetCurrency();
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
                string deliveryTime = workOrder.GetDeliveryTimeMinimum().ToString() + "-" + workOrder.GetDeliveryTimeMaximum().ToString() + "   " + workOrder.GetDeliveryTimeUnit();
                deliveryTimeParagraph.Text = deliveryTime;


                Spire.Doc.TableCell deliveryPointCell = deliveryPaymentTable.Rows[1].Cells[1];
                IParagraph deliveryPointParagraph = deliveryPointCell.Paragraphs[0];
                deliveryPointParagraph.Text = workOrder.GetDeliveryPoint();

                Spire.Doc.TableCell paymentCell = deliveryPaymentTable.Rows[2].Cells[1];
                IParagraph paymentParagraph = paymentCell.Paragraphs[0];
                paymentParagraph.Text = workOrder.GetPercentDownPayment() + "% DOWNPAYMENT  " + workOrder.GetPercentOnDelivery() + "% ONDELIVERY  " + workOrder.GetPercentOnInstallation() + "% ONINSTALLATION";

                Spire.Doc.TableCell contractTypeCell = deliveryPaymentTable.Rows[3].Cells[1];
                IParagraph contractTypeParagraph = contractTypeCell.Paragraphs[0];
                contractTypeParagraph.Text = workOrder.GetOrderContractType();

                Spire.Doc.TableCell warrantyCell = deliveryPaymentTable.Rows[4].Cells[1];
                IParagraph warrantyParagraph = warrantyCell.Paragraphs[0];
                warrantyParagraph.Text = workOrder.GetWarrantyPeriod() + " " + workOrder.GetWarrantyPeriodTimeUnit();


                ///////////////////////////////////////////////////////////////////////////////////////////
                /////////////TECH OFFICE REP TABLE
                ///////////////////////////////////////////////////////////////////////////////////////////
                ITable techOfficeRepTable = section.Tables[counter];
                counter++;

                if (workOrder.GetRFQSerial() != 0)
                {
                    Spire.Doc.TableCell techOfficeRepNameCell = techOfficeRepTable.Rows[1].Cells[0];
                    IParagraph techOfficeRepNameParagraph = techOfficeRepNameCell.Paragraphs[0];
                    techOfficeRepNameParagraph.Text = workOrder.GetAssigneeName();

                    Spire.Doc.TableCell techOfficeRepPositionCell = techOfficeRepTable.Rows[2].Cells[0];
                    IParagraph techOfficeRepPositionParagraph = techOfficeRepPositionCell.Paragraphs[0];
                    techOfficeRepPositionParagraph.Text = workOrder.GetAssigneeTeam() + "  " + workOrder.GetAssigneePosition();

                    Spire.Doc.TableCell techOfficeRepEmailCell = techOfficeRepTable.Rows[3].Cells[0];
                    IParagraph techOfficeRepEmailParagraph = techOfficeRepEmailCell.Paragraphs[0];
                    techOfficeRepEmailParagraph.Text = workOrder.GetRFQAssignee().GetEmployeeBusinessEmail();

                    Spire.Doc.TableCell techOfficeRepNumberCell = techOfficeRepTable.Rows[4].Cells[0];
                    IParagraph techOfficeRepNumberParagraph = techOfficeRepNumberCell.Paragraphs[0];
                    techOfficeRepNumberParagraph.Text = "Mob. " + workOrder.GetRFQAssignee().GetEmployeeBusinessPhone();
                }

                else
                {
                    Spire.Doc.TableCell techOfficeRepNameCell = techOfficeRepTable.Rows[1].Cells[0];
                    IParagraph techOfficeRepNameParagraph = techOfficeRepNameCell.Paragraphs[0];
                    techOfficeRepNameParagraph.Text = workOrder.GetOfferProposerName();

                    Spire.Doc.TableCell techOfficeRepPositionCell = techOfficeRepTable.Rows[2].Cells[0];
                    IParagraph techOfficeRepPositionParagraph = techOfficeRepPositionCell.Paragraphs[0];
                    techOfficeRepPositionParagraph.Text = workOrder.GetOfferProposerTeam() + "  " + workOrder.GetOfferProposerPosition();

                    Spire.Doc.TableCell techOfficeRepEmailCell = techOfficeRepTable.Rows[3].Cells[0];
                    IParagraph techOfficeRepEmailParagraph = techOfficeRepEmailCell.Paragraphs[0];
                    techOfficeRepEmailParagraph.Text = workOrder.GetOfferProposerbusinessEmail();

                    Spire.Doc.TableCell techOfficeRepNumberCell = techOfficeRepTable.Rows[4].Cells[0];
                    IParagraph techOfficeRepNumberParagraph = techOfficeRepNumberCell.Paragraphs[0];
                    techOfficeRepNumberParagraph.Text = "Mob. " + workOrder.GetOfferProposerCompanyPhone();
                }
                ///////////////////////////////////////////////////////////////////////////////////////////
                /////////////SALES REP TABLE
                //////////////////////////////////////////////////////////////////////////////////////////
                ITable salesRepTable = section.Tables[counter];

                Spire.Doc.TableCell salesRepNameCell = salesRepTable.Rows[1].Cells[0];
                IParagraph salesRepNameParagraph = salesRepNameCell.Paragraphs[0];
                salesRepNameParagraph.Text = workOrder.GetSalesPersonName();

                Spire.Doc.TableCell salesRepPositionCell = salesRepTable.Rows[2].Cells[0];
                IParagraph salesRepPositionParagraph = salesRepPositionCell.Paragraphs[0];
                salesRepPositionParagraph.Text = workOrder.GetSalesPersonTeam() + "  " + workOrder.GetSalesPersonPosition();

                Spire.Doc.TableCell salesRepEmailCell = salesRepTable.Rows[3].Cells[0];
                IParagraph salesRepEmailParagraph = salesRepEmailCell.Paragraphs[0];
                salesRepEmailParagraph.Text = workOrder.GetSalesPersonbusinessEmail();

                Spire.Doc.TableCell salesRepNumberCell = salesRepTable.Rows[4].Cells[0];
                IParagraph salesRepNumberParagraph = salesRepNumberCell.Paragraphs[0];
                salesRepNumberParagraph.Text = "Mob. " + workOrder.GetSalesPersonCompanyPhone();

                doc.SaveToFile(wordFilePath);

                System.Diagnostics.Process.Start(wordFilePath);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }

    }
}
