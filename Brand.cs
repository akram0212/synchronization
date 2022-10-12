using _01electronics_library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01electronics_crm
{
    public class Brand:Product
    {

        private int brandID;
        private String brandName;

        protected String BrandPhotoLocalPath;
        protected String BrandPhotoServerPath;

        public Brand() { 
        
        
        }


        public void SetBrandID(int mBrandID)
        {
            brandID = mBrandID;
        }

        public void SetBrandName(String mBrandName)
        {
            brandName = mBrandName;
        }


        public void SetBrandPhotoLocalPath(String mPath)
        {
            BrandPhotoLocalPath = mPath;
        }
        public void SetBrandPhotoServerPath(String mPath)
        {
            BrandPhotoServerPath = mPath;
        }



        public int GetBrandID()
        {
            return brandID;
        }

        public String GetBrandName()
        {
            return brandName;
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


        public string GetBrandFolderLocalPath()
        {
            BrandPhotoLocalPath = String.Empty;
            BrandPhotoLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01Electronics_ERP\\brands";
            return BrandPhotoLocalPath;
        }

        public String GetBrandFolderServerPath()
        {
            BrandPhotoServerPath = String.Empty;
            BrandPhotoServerPath += BASIC_MACROS.MODELS_PHOTOS_PATH;
            BrandPhotoServerPath += "brands/";

            return BrandPhotoServerPath;
            //folderServerPath = folderServerPath;
        }

        public String GetBrandPhotoLocalPath()
        {
            BrandPhotoLocalPath = String.Empty;
            BrandPhotoLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\products_photos\\brands\\"+ GetBrandID()+".jpg";
            return BrandPhotoLocalPath;
        }

        public String GetBrandPhotoServerPath()
        {
            return BrandPhotoServerPath;
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
            sqlQuery += sqlQueryPart3;
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







    }
}
