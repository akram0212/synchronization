using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _01electronics_hr;
using _01electronics_library;

namespace _01electronics_crm
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
        private int productID;
        private int brandID;
        private int modelID;

        private String productName;
        private String brandName;
        private String modelName;

        //PRODUCT ADDITIONAL INFO
        private List<String> modelApplications;
        private List<String> modelBenefits;
        public List<String> modelSummaryPoints;
        private List<String> modelStandardFeatures;

        private int numberOfSavedModelApplications;
        private int numberOfSavedModelBenefits;
        private int numberOfSavedModelSummaryPoints;
        private int numberOfSavedModelStandardFeatures;

        public Product()
        {
            sqlDatabase = new SQLServer();
            ftpServer = new FTPServer();
            commonQueries = new CommonQueries();

            modelApplications = new List<String>();
            modelBenefits = new List<String>();
            modelSummaryPoints = new List<String>();
            modelStandardFeatures = new List<String>();
        }

        //////////////////////////////////////////////////////////////////////
        //INITIALIZATION FUNCTIONS
        //////////////////////////////////////////////////////////////////////
        public bool InitializeProductInfo(int mProductID, int mBrandID, int mModelID)
        {
            SetProductID(mProductID);
            SetBrandID(mBrandID);
            SetModelID(mModelID);

            if (!commonQueries.GetModelApplications(productID, brandID, modelID, ref modelApplications))
                return false;
            if (!commonQueries.GetModelBenefits(productID, brandID, modelID, ref modelBenefits))
                return false; 
            if (!commonQueries.GetModelFeatures(productID, brandID, modelID, ref modelStandardFeatures))
                return false;

            GetNewPhotoLocalPath();
            GetNewPhotoServerPath();

            return true;
        }
        public bool InitializeModelSummaryPoints(int mProductID, int mBrandID, int mModelID)
        {
            modelSummaryPoints.Clear();

            String sqlQueryPart1 = @"select models_summary.points_id, models_summary.points
                                     from  erp_system.dbo.models_summary
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
            sqlQuery += mModelID;
            sqlQuery += sqlQueryPart4;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_string = 1;
            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                numberOfSavedModelSummaryPoints = sqlDatabase.rows[i].sql_int[0];

                if (numberOfSavedModelSummaryPoints > 0)
                    modelSummaryPoints.Add(sqlDatabase.rows[i].sql_string[0]);
            }

            return true;
        }

        //////////////////////////////////////////////////////////////////////
        //ISSUE FUNCTIONS
        //////////////////////////////////////////////////////////////////////
        public bool IssueNewProduct(String mProductName)
        {
            if (!GetNewProductID())
                return false;
            if (!InsertIntoProductTypes(mProductName))
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
        public bool IssueNewModel(String mModelName, ref List<String> mModelApplications, ref List<String> mModelBenefits, ref List<String> mModelStandardFeatures)
        {
            if (!GetNewModelID())
                return false;
            if (!InsertIntoBrandModels(mModelName))
                return false; 
            if (!InsertIntoModelApplications(ref mModelApplications))
                return false; 
            if (!InsertIntoModelBenefits(ref mModelBenefits))
                return false;
            if (!InsertIntoModelStandardFeatures(ref mModelStandardFeatures))
                return false;

            GetNewPhotoLocalPath();
            GetNewPhotoServerPath();

            return true;
        }

        //////////////////////////////////////////////////////////////////////
        //INSERT FUNCTIONS
        //////////////////////////////////////////////////////////////////////
        public bool InsertIntoProductTypes(String mProductName)
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
            sqlQuery += "'" + mProductName + "'";
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
            for(int i = 0; i < brandProducts.Count(); i++)
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
        public bool InsertIntoBrandModels(String mModelName)
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
            sqlQuery += "'" + mModelName + "'";
            sqlQuery += comma;
            sqlQuery += "'" + sqlQueryPart3 + "'";
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
                sqlQuery += i+1;
                sqlQuery += comma;
                sqlQuery += "'" + mModelApplications[i] + "'";
                sqlQuery += comma;
                sqlQuery += "'" + sqlQueryPart3 + "'";
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
                sqlQuery += i+1;
                sqlQuery += comma;
                sqlQuery += "'" + mModelBenefits[i] + "'";
                sqlQuery += comma;
                sqlQuery += "'" + sqlQueryPart3 + "'";
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
                sqlQuery += i+1;
                sqlQuery += comma;
                sqlQuery += "'" + mModelStandardFeatures[i] + "'";
                sqlQuery += comma;
                sqlQuery += "'" + sqlQueryPart3 + "'";
                sqlQuery += sqlQueryPart4;

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;
            }
            return true;
        }
        public bool InsertIntoModelSummaryPoints(ref List<String> mModelSummaryPoints)
        {
            for (int i = 0; i < mModelSummaryPoints.Count(); i++)
            {
                String sqlQueryPart1 = @" insert into erp_system.dbo.products_summary
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
                sqlQuery += i+1;
                sqlQuery += comma;
                sqlQuery += "'" + mModelSummaryPoints[i] + "'";
                sqlQuery += comma;
                sqlQuery += "'" + sqlQueryPart3 + "'";
                sqlQuery += sqlQueryPart4;

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;
            }
            return true;
        }

        //////////////////////////////////////////////////////////////////////
        //PRODUCT ADDITIONAL INFO STRUCT
        //////////////////////////////////////////////////////////////////////
        public struct MODEL_DATA_STRUCT
        {
            public int id;
            public String Point;
        };

        //////////////////////////////////////////////////////////////////////
        //FTP_SERVER FUNCTIONS
        //////////////////////////////////////////////////////////////////////
        public bool DownloadPhotoFromServer()
        {
            if (!ftpServer.DownloadFile(photoServerPath, photoLocalPath))
                return false;

            return true;
        }
        public bool UploadPhotoToServer()
        {
            if (!ftpServer.UploadFile(photoLocalPath, photoServerPath))
                return false;

            return true;
        }

        //////////////////////////////////////////////////////////////////////
        //SETTERS
        //////////////////////////////////////////////////////////////////////
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

            GetNewPhotoLocalPath();
            GetNewPhotoServerPath();
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
        public void SetPhotoLocalPath(String mPath)
        {
            photoLocalPath = mPath;
        }
        public void SetPhotoServerPath(String mPath)
        {
            photoServerPath = mPath;
        }
        //////////////////////////////////////////////////////////////////////
        //GETTERS
        //////////////////////////////////////////////////////////////////////
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
        public void GetNewPhotoLocalPath()
        {
            photoLocalPath = String.Empty;
            photoLocalPath = BASIC_MACROS.LOCAL_PHOTOS_PATH + GetProductID() + "-" + GetBrandID() + "-" + GetModelID() + ".jpg";
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
    }
}