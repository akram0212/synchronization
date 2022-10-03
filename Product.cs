using _01electronics_windows_library;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _01electronics_library
{
    public class Product
    {
        //FTP_SERVER OBJECTS
        protected FTPServer ftpServer;
        //protected String serverPath = "ftp://salma.omran%254001electronics.net@01electronics.net/ftp_server/erp_system/products_photos/";
        //protected String serverPath = "/ftp_server/erp_system/products_photos/";
        protected String photoLocalPath;
        protected String photoServerPath;

        //SQL QUERY
        protected String sqlQuery;
        protected CommonQueries commonQueries;

        //SQL OBJECTS
        protected SQLServer sqlDatabase;

        //MODEL MACROS
        public const int MAX_APPLICATIONS_PER_MODEL = 5;
        public const int MAX_BENEFITS_PER_MODEL = 6;
        public const int MAX_SUMMARY_POINTS_PER_MODEL = 8;
        public const int MAX_STANDARD_FEATURES_PER_MODEL = 7;

        
        //PRODUCT BASIC INFO
        private int categoryID;
        private int productID;
        private int brandID;
        private int modelID;
        private int summaryPointsID;

        private String productName;
        private String categoryName;
        private String brandName;
        private String modelName;
        private String summaryPoints;


        //PRODUCT ADDITIONAL INFO
        private List<String> modelApplications;
        private List<String> modelBenefits;
        private List<String> modelSummaryPoints;
        private List<String> modelStandardFeatures;

        private int numberOfSavedModelApplications;
        private int numberOfSavedModelBenefits;
        private int numberOfSavedModelSummaryPoints;
        private int numberOfSavedModelStandardFeatures;

        protected String errorMessage;

        private List<BASIC_STRUCTS.UPS_SPECS_STRUCT> UPSSpecs;

        public Product()
        {
            sqlDatabase = new SQLServer();
            ftpServer = new FTPServer();
            commonQueries = new CommonQueries();

            modelApplications = new List<String>();
            modelBenefits = new List<String>();
            modelSummaryPoints = new List<String>();
            modelStandardFeatures = new List<String>();
            UPSSpecs = new List<BASIC_STRUCTS.UPS_SPECS_STRUCT>();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //INITIALIZATION FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool InitializeModelInfo(int mProductID, int mBrandID, int mModelID)
        {
            modelSummaryPoints.Clear();
            SetProductID(mProductID);
            SetBrandID(mBrandID);
            SetModelID(mModelID);

            if (!commonQueries.GetModelApplications(productID, brandID, modelID, ref modelApplications))
                return false;
            if (!commonQueries.GetModelBenefits(productID, brandID, modelID, ref modelBenefits))
                return false;
            if (!commonQueries.GetModelFeatures(productID, brandID, modelID, ref modelStandardFeatures))
                return false;

            if()
            GetNewModelPhotoLocalPath();
            GetNewPhotoServerPath();

            return true;
        }
        public bool InitializeProductInfo()
        {

            String sqlQueryPart1 = @"SELECT product_name
		                                   ,summary_points
	
	                                FROM erp_system.dbo.products_type

	                                left join erp_system.dbo.products_summary_points
	                                on products_type.id = products_summary_points.id

	                                where products_type.id  = ";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetProductID();

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            
            
            queryColumns.sql_string = 2;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_LOW))
                return false;

            SetProductName(sqlDatabase.rows[0].sql_string[0]);
            SetsummaryPointsID(GetProductID());
            SetsummaryPoints(sqlDatabase.rows[0].sql_string[1]);

            return true;
        }
        public bool InitializeModelSummaryPoints()
        {
            modelSummaryPoints.Clear();

            String sqlQueryPart1 = @"select 
                                            models_summary_points.points_id
                                        , models_summary_points.points
                                     from  erp_system.dbo.models_summary_points
                                     where product_id = ";

            String sqlQueryPart2 = " and brand_id = ";
            String sqlQueryPart3 = "  and model_id = ";
            String sqlQueryPart4 = " ;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetProductID();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetBrandID();
            sqlQuery += sqlQueryPart3;
            sqlQuery += GetModelID();
            sqlQuery += sqlQueryPart4;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_string = 1;
            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_LOW))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                numberOfSavedModelSummaryPoints = sqlDatabase.rows[i].sql_int[0];

                if (numberOfSavedModelSummaryPoints > 0)
                    modelSummaryPoints.Add(sqlDatabase.rows[i].sql_string[0]);
            }

            return true;
        }
        

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //ISSUE FUNCTIONS
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public bool IssueNewProduct()
        {
            if (!GetNewProductID())
                return false;
            if (!InsertIntoProductTypes())
                return false;
            if (!InsertIntoProductSummaryPoints())
                return false;

            return true;
            }
        public bool IssueNewBrand(String mBrandName, ref List<int> brandProducts)
        {
            if (!GetNewBrandID())
                return false;
            if (!InsertIntoBrandTypes(mBrandName))
                return false;
            if (!InsertIntoProductBrands(ref brandProducts))
                return false;

            return true;
        }
        public bool IssueNewBrandToProduct(String mBrandName, ref List<int> brandProducts)
        {
            if (!GetNewBrandID())
                return false;
            if (!InsertIntoBrandTypes(mBrandName))
                return false;
            if (!InsertIntoProductBrands(ref brandProducts))
                return false;

            return true;
        }
        public bool IssueNewModel( ref List<String> mModelApplications, ref List<String> mModelBenefits, ref List<String> mModelStandardFeatures)
        {
            if (!GetNewModelID())
                return false;
            if (!InsertIntoBrandModels())
                return false;
            if (!InsertIntoModelSummaryPoints())
                return false;
            if (!InsertIntoUPSSpecs())
                return false;

            if (!InsertIntoModelApplications(ref mModelApplications))
                return false;
            if (!InsertIntoModelBenefits(ref mModelBenefits))
                return false;
            if (!InsertIntoModelStandardFeatures(ref mModelStandardFeatures))
                return false;
            

            GetNewModelPhotoLocalPath();
            GetNewPhotoServerPath();

            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //INSERT FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool InsertIntoProductTypes()
        {
            String sqlQueryPart1 = @" insert into erp_system.dbo.products_type
                                      values(";

            String comma = ",";
            String sqlQueryPart3 = "GETDATE()";
            String sqlQueryPart4 = ");";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetProductID();
            sqlQuery += comma;
            sqlQuery += "'" + GetProductName() + "'";
            sqlQuery += comma;
            sqlQuery += sqlQueryPart3;
            sqlQuery += comma;
            sqlQuery += GetCategoryID();
            sqlQuery += sqlQueryPart4;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            sqlQueryPart1 = @" insert into erp_system.dbo.products_brands
                                      values(";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetProductID();
            sqlQuery += comma;
            sqlQuery += 0;
            sqlQuery += comma;
            sqlQuery += sqlQueryPart3;
            sqlQuery += sqlQueryPart4;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
        public bool InsertIntoBrandTypes(String mBrandName)
        {
            String sqlQueryPart1 = @" insert into erp_system.dbo.brands_type
                                      values(";

            String comma = ",";
            String sqlQueryPart3 = "GETDATE()";
            String sqlQueryPart4 = ");";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetProductID();
            sqlQuery += comma;
            sqlQuery += "'" + mBrandName + "'";
            sqlQuery += comma;
            sqlQuery += "'" + sqlQueryPart3 + "'";
            sqlQuery += sqlQueryPart4;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            sqlQueryPart1 = @" insert into erp_system.dbo.products_brands
                                      values(";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetProductID();
            sqlQuery += comma;
            sqlQuery += 0;
            sqlQuery += comma;
            sqlQuery += "'" + sqlQueryPart3 + "'";
            sqlQuery += sqlQueryPart4;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
        public bool InsertIntoProductBrands(ref List<int> brandProducts)
        {
            for (int i = 0; i < brandProducts.Count(); i++)
            {
                String sqlQueryPart1 = @" insert into erp_system.dbo.products_brands
                                          values(";
                String comma = ",";
                String sqlQueryPart3 = "GETDATE()";
                String sqlQueryPart4 = ");";

                sqlQuery = String.Empty;
                sqlQuery += sqlQueryPart1;
                sqlQuery += brandProducts[i];
                sqlQuery += comma;
                sqlQuery += GetBrandID();
                sqlQuery += comma;
                sqlQuery += "'" + sqlQueryPart3 + "'";
                sqlQuery += sqlQueryPart4;

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;

                sqlQueryPart1 = @" insert into erp_system.dbo.brands_models
                                      values(";

                sqlQuery = String.Empty;
                sqlQuery += sqlQueryPart1;
                sqlQuery += brandProducts[i];
                sqlQuery += comma;
                sqlQuery += GetBrandID();
                sqlQuery += comma;
                sqlQuery += 0;
                sqlQuery += comma;
                sqlQuery += "'" + "others" + "'";
                sqlQuery += comma;
                sqlQuery += "'" + sqlQueryPart3 + "'";
                sqlQuery += sqlQueryPart4;

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;
            }

            return true;
        }
        public bool AddBrandToProduct()
        {
            
                String sqlQueryPart1 = @" insert into erp_system.dbo.products_brands
                                          values(";
                String comma = ",";
                String sqlQueryPart3 = "GETDATE()";
                String sqlQueryPart4 = ");";

                sqlQuery = String.Empty;
                sqlQuery += sqlQueryPart1;
                sqlQuery += GetProductID();
                sqlQuery += comma;
                sqlQuery += GetBrandID();
                sqlQuery += comma;
                sqlQuery +=  sqlQueryPart3 ;
                sqlQuery += sqlQueryPart4;

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;
            return true;
        }

        public bool InsertIntoBrandModels()
        {
            String sqlQueryPart1 = @" insert into erp_system.dbo.brands_models
                                      values(";
            String comma = ",";
            String sqlQueryPart3 = "GETDATE()";
            String sqlQueryPart4 = ");";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetProductID();
            sqlQuery += comma;
            sqlQuery += GetBrandID();
            sqlQuery += comma;
            sqlQuery += GetModelID();
            sqlQuery += comma;
            sqlQuery += "'" + GetModelName() + "'";
            sqlQuery += comma;
            sqlQuery +=  sqlQueryPart3 ;
            sqlQuery += sqlQueryPart4;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }
        public bool InsertIntoModelApplications(ref List<String> mModelApplications)
        {
            for (int i = 0; i < mModelApplications.Count(); i++)
            {
                String sqlQueryPart1 = @" insert into erp_system.dbo.model_applications
                                      values(";
                String comma = ",";
                String sqlQueryPart3 = "GETDATE()";
                String sqlQueryPart4 = ");";

                sqlQuery = String.Empty;
                sqlQuery += sqlQueryPart1;
                sqlQuery += GetProductID();
                sqlQuery += comma;
                sqlQuery += GetBrandID();
                sqlQuery += comma;
                sqlQuery += GetModelID();
                sqlQuery += comma;
                sqlQuery += i + 1;
                sqlQuery += comma;
                sqlQuery += "'" + mModelApplications[i] + "'";
                sqlQuery += comma;
                sqlQuery += sqlQueryPart3 ;
                sqlQuery += sqlQueryPart4;

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;
            }
            return true;
        }
        public bool InsertIntoModelBenefits(ref List<String> mModelBenefits)
        {
            for (int i = 0; i < mModelBenefits.Count(); i++)
            {
                String sqlQueryPart1 = @" insert into erp_system.dbo.model_benefits
                                      values(";
                String comma = ",";
                String sqlQueryPart3 = "GETDATE()";
                String sqlQueryPart4 = ");";

                sqlQuery = String.Empty;
                sqlQuery += sqlQueryPart1;
                sqlQuery += GetProductID();
                sqlQuery += comma;
                sqlQuery += GetBrandID();
                sqlQuery += comma;
                sqlQuery += GetModelID();
                sqlQuery += comma;
                sqlQuery += i + 1;
                sqlQuery += comma;
                sqlQuery += "'" + mModelBenefits[i] + "'";
                sqlQuery += comma;
                sqlQuery +=  sqlQueryPart3 ;
                sqlQuery += sqlQueryPart4;

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;
            }
            return true;
        }
        public bool InsertIntoModelStandardFeatures(ref List<String> mModelStandardFeatures)
        {
            for (int i = 0; i < mModelStandardFeatures.Count(); i++)
            {
                String sqlQueryPart1 = @" insert into erp_system.dbo.model_standard_features
                                      values(";
                String comma = ",";
                String sqlQueryPart3 = "GETDATE()";
                String sqlQueryPart4 = ");";

                sqlQuery = String.Empty;
                sqlQuery += sqlQueryPart1;
                sqlQuery += GetProductID();
                sqlQuery += comma;
                sqlQuery += GetBrandID();
                sqlQuery += comma;
                sqlQuery += GetModelID();
                sqlQuery += comma;
                sqlQuery += i + 1;
                sqlQuery += comma;
                sqlQuery += "'" + mModelStandardFeatures[i] + "'";
                sqlQuery += comma;
                sqlQuery +=  sqlQueryPart3 ;
                sqlQuery += sqlQueryPart4;

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;
            }
            return true;
        }
        public bool InsertIntoModelSummaryPoints()
        {
            for (int i = 0; i < modelSummaryPoints.Count(); i++)
            {
                String sqlQueryPart1 = @" insert into erp_system.dbo.models_summary_points
                                      values(";
                String comma = ",";
                String sqlQueryPart3 = "GETDATE()";
                String sqlQueryPart4 = ");";

                sqlQuery = String.Empty;
                sqlQuery += sqlQueryPart1;
                sqlQuery += GetProductID();
                sqlQuery += comma;
                sqlQuery += GetBrandID();
                sqlQuery += comma;
                sqlQuery += GetModelID();
                sqlQuery += comma;
                sqlQuery += i + 1;
                sqlQuery += comma;
                sqlQuery += "'" + modelSummaryPoints[i] + "'";
                sqlQuery += comma;
                sqlQuery += sqlQueryPart3;
                sqlQuery += sqlQueryPart4;

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;
            }
            return true;
        }
        public bool UpdateIntoProductSummaryPoints()
        {
            String sqlQueryPart1 = @" UPDATE erp_system.dbo.products_summary_points    
                                      SET summary_points ='";
            String sqlQueryPart2 = @"' WHERE id =";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetSummaryPoints();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetProductID();
           
            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;


            return true;
        }
        public bool UpdateIntoProductName()
        {
            String sqlQueryPart1 = @" UPDATE erp_system.dbo.products_type    
                                      SET product_name ='";
            String sqlQueryPart2 = @"' WHERE id =";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetProductName();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetProductID();

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;


            return true;
        }
        public bool InsertIntoProductSummaryPoints()
        {
            
                String sqlQueryPart1 = @" insert into erp_system.dbo.products_summary_points
                                      values(";
                String comma = ",";
                String sqlQueryPart2 = "GETDATE()";
                String sqlQueryPart3 = ");";

                sqlQuery = String.Empty;
                sqlQuery += sqlQueryPart1;
                sqlQuery += GetProductID();
                sqlQuery += comma;
                sqlQuery += "'" + GetSummaryPoints() + "'";
                sqlQuery += comma;
                sqlQuery += sqlQueryPart2;
                sqlQuery += sqlQueryPart3;

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;
            
            return true;
        }
        public bool InsertIntoUPSSpecs()
        {
            int max = UPSSpecs.Count()-1;
            if (max<0)
            {
                max = 0;
            }
           
            String sqlQueryPart1 = @"insert into erp_system.dbo.ups_specs
                                    values(";
            String comma = ",";
            String sqlQueryPart2 = "GETDATE()";
            String sqlQueryPart3 = ");";

                
            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += categoryID;
            sqlQuery += comma;
            sqlQuery += productID;
            sqlQuery += comma;
            sqlQuery += brandID;
            sqlQuery += comma;
            sqlQuery += max+1;
            sqlQuery += comma;
            sqlQuery += UPSSpecs[max].spec_id;
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].io_phase+"'" ;
            sqlQuery += comma;
            sqlQuery += UPSSpecs[max].rated_power;
            sqlQuery += comma;
            sqlQuery += UPSSpecs[max].rating_id ;
            sqlQuery += comma;
            sqlQuery += UPSSpecs[max].backup_time_50 ;
            sqlQuery += comma;
            sqlQuery +=  UPSSpecs[max].backup_time_70 ;
            sqlQuery += comma;
            sqlQuery +=  UPSSpecs[max].backup_time_100;
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].input_power_factor + "'";
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].thdi + "'";
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].input_nominal_voltage + "'";
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].input_voltage + "'";
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].voltage_tolerance + "'";
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].output_power_factor + "'";
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].thdv + "'";
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].output_nominal_voltage + "'";
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].output_dc_voltage_range + "'";
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].overload_capability + "'";
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].efficiency + "'";
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].input_connection_type + "'";
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].front_panel + "'";
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].max_power + "'";
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].certificates + "'";
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].safety + "'";
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].emc + "'";
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].environmental_aspects + "'";
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].test_performance + "'";
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].protection_degree + "'";
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].transfer_voltage_limit + "'";
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].marking + "'";
            sqlQuery += comma;
            if (UPSSpecs[max].is_valid == false)
            {
                sqlQuery += 0;
            }
            else
            {
                sqlQuery += 1;
            }
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].valid_until.ToString("yyyy-MM-dd") + "'"; ;
            sqlQuery += comma;
            sqlQuery += sqlQueryPart2;
            sqlQuery += sqlQueryPart3;

               


            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

                
            
            return true;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //PRODUCT ADDITIONAL INFO STRUCT
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public struct MODEL_DATA_STRUCT
        {
            public int id;
            public String Point;
        };

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //FTP_SERVER FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool DownloadPhotoFromServer()
        {
            if (!ftpServer.DownloadFile(photoServerPath, photoLocalPath, BASIC_MACROS.SEVERITY_LOW, ref errorMessage))
            {
                //System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        public bool UploadPhotoToServer()
        {
            if (!ftpServer.UploadFile(photoLocalPath, photoServerPath, BASIC_MACROS.SEVERITY_LOW, ref errorMessage))
            {
                //System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        //↓↓↓↓↓↓↓↓↓↓↓↓ Added By Raouf ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
        public bool DeletePhotoFromServer()
        {
            if (!ftpServer.DeleteFtpFile( photoServerPath, BASIC_MACROS.SEVERITY_LOW, ref errorMessage))
            {
                //System.Windows.Forms.MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SETTERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetCategoryID(int mCategoryID)
        {
            categoryID = mCategoryID;
        }
        public void SetProductID(int mProductID)
        {
            productID = mProductID;
        }
        public void SetBrandID(int mBrandID)
        {
            brandID = mBrandID;
        }
        public void SetModelID(int mModelID)
        {
            modelID = mModelID;

            GetNewModelPhotoLocalPath();
            GetNewPhotoServerPath();
        }
        public void SetsummaryPointsID(int msummaryPointsID)
        {
            summaryPointsID = msummaryPointsID;

        }
        public void SetCategoryName(String mCategoryName)
        {
            categoryName = mCategoryName;
        }
        public void SetProductName(String mProductName)
        {
            productName = mProductName;
        }
        public void SetBrandName(String mBrandName)
        {
            brandName = mBrandName;
        }
        public void SetModelName(String mModelName)
        {
            modelName = mModelName;
        }
        public void SetsummaryPoints(string msummaryPoints)
        {
            summaryPoints = msummaryPoints;

        }
        public void SetModelsummaryPoints(List<String> mModlSummeryPoints)
        {
             modelSummaryPoints = mModlSummeryPoints;
        }
        public void SetPhotoLocalPath(String mPath)
        {
            photoLocalPath = mPath;
        }
        public void SetPhotoServerPath(String mPath)
        {
            photoServerPath = mPath;
        }
        public void SetUPSSpecs(BASIC_STRUCTS.UPS_SPECS_STRUCT mUPSSpecs)
        {
            UPSSpecs.Add( mUPSSpecs); 
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //GETTERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int GetCategoryID()
        {
            return categoryID;
        }
        public int GetProductID()
        {
            return productID;
        }
        public int GetBrandID()
        {
            return brandID;
        }
        public int GetModelID()
        {
            return modelID;
        }
        public int GetsummaryPointsID()
        {
            return summaryPointsID;
        }
        public String GetCategoryName()
        {
            return categoryName;
        }
        public String GetProductName()
        {
            return productName;
        }
        public String GetBrandName()
        {
            return brandName;
        }
        public String GetModelName()
        {
            return modelName;
        }
        public String GetSummaryPoints()
        {
            return summaryPoints;
        }
        public List<String> GetModelSummaryPoints()
        {
            return modelSummaryPoints;
        }
        public int GetNumberOfSavedModelApplications()
        {
            return numberOfSavedModelApplications;
        }
        public int GetNumberOfSavedModelBenefits()
        {
            return numberOfSavedModelBenefits;
        }
        public int GetNumberOfSavedModelSummaryPoints()
        {
            return numberOfSavedModelSummaryPoints;
        }
        public int GetNumberOfSavedModelStandardFeatures()
        {
            return numberOfSavedModelStandardFeatures;
        }
        public String GetPhotoLocalPath()
        {
            return photoLocalPath;
        }
        public String GetPhotoServerPath()
        {
            return photoServerPath;
        }
        public void GetNewPhotoServerPath()
        {
            photoServerPath = String.Empty;
            photoServerPath += BASIC_MACROS.MODELS_PHOTOS_PATH;
            photoServerPath += GetProductID();
            photoServerPath += "/";
            photoServerPath += GetBrandID();
            photoServerPath += "/";
            photoServerPath += GetModelID();
            photoServerPath += ".jpg";

            //photoServerPath = photoServerPath;
        }
        public String GetModelFolderServerPath()
        {
            String folderServerPath = String.Empty;
            folderServerPath = String.Empty;
            folderServerPath += BASIC_MACROS.MODELS_PHOTOS_PATH;
            folderServerPath += GetProductID();
            folderServerPath += "/";
            folderServerPath += GetBrandID();
            folderServerPath += "/";

            return folderServerPath;
            //folderServerPath = folderServerPath;
        }
        public String GetProductFolderServerPath()
        {
            photoServerPath = String.Empty;
            photoServerPath += BASIC_MACROS.MODELS_PHOTOS_PATH;
            photoServerPath += "products/";

            return photoServerPath;
            //folderServerPath = folderServerPath;
        }
        public String GetBrandFolderServerPath()
        {
            photoServerPath = String.Empty;
            photoServerPath += BASIC_MACROS.MODELS_PHOTOS_PATH;
            photoServerPath += "brands/";

            return photoServerPath;
            //folderServerPath = folderServerPath;
        }
        public void GetNewModelPhotoLocalPath()
        {
            photoLocalPath = String.Empty;
            photoLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01Electronics_ERP\\" + GetProductID() + "\\" + GetBrandID() + "\\" + GetModelID() + ".jpg";
        }
        public String GetProductPhotoLocalPath()
        {
            photoLocalPath = String.Empty;
            photoLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01Electronics_ERP\\products\\" + GetProductID() + ".jpg";
            return photoLocalPath;
        }
        public String GetBrandPhotoLocalPath()
        {
            photoLocalPath = String.Empty;
            photoLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01Electronics_ERP\\brands\\" + GetBrandID() + ".jpg";
            return photoLocalPath;
        }
        public string GetModelFolderLocalPath()
        {
            photoLocalPath = String.Empty;
            photoLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01Electronics_ERP\\" + GetProductID() + "\\" + GetBrandID();
            return photoLocalPath;
        }
        public string GetProductFolderLocalPath()
        {
            photoLocalPath = String.Empty;
            photoLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01Electronics_ERP\\products";
            return photoLocalPath;
        }
        public string GetBrandFolderLocalPath()
        {
            photoLocalPath = String.Empty;
            photoLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01Electronics_ERP\\brands";
            return photoLocalPath;
        }
        public bool GetNewProductID()
        {
            String sqlQueryPart1 = @"SELECT max(id)
                                     from erp_system.dbo.products_type;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            productID = sqlDatabase.rows[0].sql_int[0] + 1;

            return true;
        }
        public bool GetNewBrandID()
        {
            String sqlQueryPart1 = @"SELECT max(id)
                                     from erp_system.dbo.brands_type;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            brandID = sqlDatabase.rows[0].sql_int[0] + 1;

            return true;
        }
        public bool GetNewModelID()
        {
            String sqlQueryPart1 = @"select max(model_id)
                                   from erp_system.dbo.brands_models
                                   where brand_id = ";
            String sqlQueryPart2 = " and product_id = ";
            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += GetBrandID();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetProductID();

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            modelID = sqlDatabase.rows[0].sql_int[0] + 1;

            return true;
        }
        public List<BASIC_STRUCTS.UPS_SPECS_STRUCT> GetUPSSpecs()
        {
            return UPSSpecs ;
        }
        


    }
}