using _01electronics_library;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01electronics_crm
{
    public class Model:Brand
    {
        private int modelID;
        private String modelName;

        protected String modelPhotoLocalPath;
        protected String modelPhotoServerPath;
        int maxSpecId=0;

        public const int MAX_APPLICATIONS_PER_MODEL = 5;
        public const int MAX_BENEFITS_PER_MODEL = 6;
        public const int MAX_STANDARD_FEATURES_PER_MODEL = 7;
        public const int MAX_SUMMARY_POINTS_PER_MODEL = 8;

        private List<String> modelApplications;
        private List<String> modelBenefits;
        private List<String> modelSummaryPoints;
        private List<String> modelStandardFeatures;
        private List<BASIC_STRUCTS.MODEl_SPEC_STRUCT> modelSpecs;

        private List<BASIC_STRUCTS.UPS_SPECS_STRUCT> UPSSpecs;
        private List<BASIC_STRUCTS.GENSET_SPEC> GENSETSpecs;

        public Model() {

            modelSpecs = new List<BASIC_STRUCTS.MODEl_SPEC_STRUCT>();
            UPSSpecs = new List<BASIC_STRUCTS.UPS_SPECS_STRUCT>();
            GENSETSpecs = new List<BASIC_STRUCTS.GENSET_SPEC>();
            modelSummaryPoints = new List<String>();

            modelApplications = new List<string>();

            modelBenefits = new List<string>();
            modelStandardFeatures = new List<string>();

        }



        ////////////////////////
        ///SETTERS
        ////////////////////////


        public void SetUPSSpecs(BASIC_STRUCTS.UPS_SPECS_STRUCT mUPSSpecs)
        {
            UPSSpecs.Add(mUPSSpecs);
        }     
        public void SetModelSpecs(BASIC_STRUCTS.MODEl_SPEC_STRUCT mModelSpecs)
        {
            modelSpecs.Add(mModelSpecs);
        }

        public void SetGensetSpec(List<BASIC_STRUCTS.GENSET_SPEC> GensetSpecs)
        {

            GENSETSpecs = GensetSpecs;


        }
        public void SetModelID(int mModelID)
        {
            modelID = mModelID;

            GetNewModelPhotoLocalPath();
            GetNewPhotoServerPath();
        }

        public void SetModelName(String mModelName)
        {
            modelName = mModelName;
        }
        public void SetModelsummaryPoints(List<String> mModlSummeryPoints)
        {
            modelSummaryPoints = mModlSummeryPoints;
        }
        public void SetModelApplications(List<string>ModelApplications) {

            modelApplications = ModelApplications;
        }

        public void SetModelBenefits(List<string>modelbenefits) {

            modelBenefits = modelbenefits;    
        }

        public void SetModelStandardFeatures(List<string>StandardFeatures) {

            modelStandardFeatures = StandardFeatures;
        }


        public void SetModelPhotoLocalPath(String mPath)
        {
            modelPhotoLocalPath = mPath;
        }
        public void SetModelPhotoServerPath(String mPath)
        {
            modelPhotoServerPath = mPath;
        }

        ////////////////
        /// GETTERS
        //////////////// 

        public int GetModelID()
        {
            return modelID;
        }
        public String GetModelName()
        {
            return modelName;
        }
        public List<String> GetModelSummaryPoints()
        {
            return modelSummaryPoints;
        }

        public List<string> GetModelApplications()
        {


            return modelApplications;

        }
        public List<string> GetModelBenefits()
        {

            return modelBenefits;

        }

        public List<string> GetModelStandardFeatures()
        {
            return modelStandardFeatures;
        }


        public int GetNumberOfSavedModelApplications()
        {
            return modelApplications.Count;
        }
        public int GetNumberOfSavedModelBenefits()
        {
            return modelBenefits.Count;
        }
        public int GetNumberOfSavedModelSummaryPoints()
        {
            return modelSummaryPoints.Count;
        }
        public int GetNumberOfSavedModelStandardFeatures()
        {
            return modelStandardFeatures.Count;
        }

        //public List<BASIC_STRUCTS.UPS_SPECS_STRUCT> GetUPSSpecs()
        //{
        //    return UPSSpecs;
        //}
        //public List<BASIC_STRUCTS.GENSET_SPEC> GetGensetSpecs()
        //{
        //    return GENSETSpecs;
        //}        
        public List<BASIC_STRUCTS.MODEl_SPEC_STRUCT> GetModelSpecs()
        {
            return modelSpecs;
        }

        public String GetModelPhotoLocalPath()
        {
            return modelPhotoLocalPath;
        }
        public String GetModelPhotoServerPath()
        {
            return modelPhotoServerPath;
        }

        public void GetNewPhotoServerPath()
        {
            modelPhotoServerPath = String.Empty;
            modelPhotoServerPath += BASIC_MACROS.MODELS_PHOTOS_PATH;
            modelPhotoServerPath += GetProductID();
            modelPhotoServerPath += "/";
            modelPhotoServerPath += GetBrandID();
            modelPhotoServerPath += "/";
            modelPhotoServerPath += GetModelID();
            modelPhotoServerPath += ".jpg";

            //photoServerPath = photoServerPath;
        }


        public bool GetUPSSpecss()
        {
        //    UPSSpecs.Clear();
        //    String sqlQueryPart1 = @"SELECT  
        //		category_id
        //		,ups_specs.product_id
        //		,ups_specs.brand_id
        //		,ups_specs.model_id
        //		,spec_id
        //		,rating
        //		,backup_time_50
        //		,backup_time_70
        //		,backup_time_100

        //		,rated_power

        //		,valid_until

        //		,io_phase
        //		,measure_units.measure_unit
        //		,input_power_factor
        //		,thdi
        //		,input_nominal_voltage
        //		,input_voltage
        //		,voltage_tolerance
        //		,output_power_factor
        //		,thdv
        //		,output_nominal_voltage
        //		,output_dc_voltage_range
        //		,overload_capability
        //		,efficiency
        //		,input_connection_type
        //		,front_panel
        //		,max_power
        //		,certificates
        //		,safety
        //		,emc
        //		,environmental_aspects
        //		,test_performance
        //		,protection_degree
        //		,transfer_voltage_limit
        //		,marking

        //		FROM erp_system.dbo.ups_specs
        //	left join erp_system.dbo.measure_units
        //	on ups_specs.rating=measure_units.id

        //	where  ups_specs.product_id =";

        //    String sqlQueryPart2 = " and ups_specs.brand_id = ";
        //    String sqlQueryPart3 = " and ups_specs.model_id= ";
        //    String sqlQueryPart4 = ";";


        //    sqlQuery = String.Empty;
        //    sqlQuery += sqlQueryPart1;
        //    sqlQuery += GetProductID();
        //    sqlQuery += sqlQueryPart2;
        //    sqlQuery += GetBrandID();
        //    sqlQuery += sqlQueryPart3;
        //    sqlQuery += GetModelID();
        //    sqlQuery += sqlQueryPart4;

        //    BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

        //    queryColumns.sql_int = 9;
        //    queryColumns.sql_money = 1;
        //    queryColumns.sql_datetime = 1;
        //    queryColumns.sql_string = 24;

        //    if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
        //        return false;

        //    for (int i = 0; i < sqlDatabase.rows.Count; i++)
        //    {
        //        BASIC_STRUCTS.UPS_SPECS_STRUCT tempItem = new BASIC_STRUCTS.UPS_SPECS_STRUCT();
        //        tempItem.spec_id = sqlDatabase.rows[i].sql_int[4];
        //        tempItem.rating_id = sqlDatabase.rows[i].sql_int[5];
        //        tempItem.backup_time_50 = sqlDatabase.rows[i].sql_int[6];
        //        tempItem.backup_time_70 = sqlDatabase.rows[i].sql_int[7];
        //        tempItem.backup_time_100 = sqlDatabase.rows[i].sql_int[8];

        //        tempItem.rated_power = sqlDatabase.rows[i].sql_money[0];

        //        tempItem.valid_until = sqlDatabase.rows[i].sql_datetime[0];

        //        tempItem.io_phase = sqlDatabase.rows[i].sql_string[0];
        //        tempItem.rating = sqlDatabase.rows[i].sql_string[1];
        //        tempItem.input_power_factor = sqlDatabase.rows[i].sql_string[2];
        //        tempItem.thdi = sqlDatabase.rows[i].sql_string[3];
        //        tempItem.input_nominal_voltage = sqlDatabase.rows[i].sql_string[4];
        //        tempItem.input_voltage = sqlDatabase.rows[i].sql_string[5];
        //        tempItem.voltage_tolerance = sqlDatabase.rows[i].sql_string[6];
        //        tempItem.output_power_factor = sqlDatabase.rows[i].sql_string[7];
        //        tempItem.thdv = sqlDatabase.rows[i].sql_string[8];
        //        tempItem.output_nominal_voltage = sqlDatabase.rows[i].sql_string[9];
        //        tempItem.output_dc_voltage_range = sqlDatabase.rows[i].sql_string[10];
        //        tempItem.overload_capability = sqlDatabase.rows[i].sql_string[11];
        //        tempItem.efficiency = sqlDatabase.rows[i].sql_string[12];
        //        tempItem.input_connection_type = sqlDatabase.rows[i].sql_string[13];
        //        tempItem.front_panel = sqlDatabase.rows[i].sql_string[14];
        //        tempItem.max_power = sqlDatabase.rows[i].sql_string[15];
        //        tempItem.certificates = sqlDatabase.rows[i].sql_string[16];
        //        tempItem.safety = sqlDatabase.rows[i].sql_string[17];
        //        tempItem.emc = sqlDatabase.rows[i].sql_string[18];
        //        tempItem.environmental_aspects = sqlDatabase.rows[i].sql_string[19];
        //        tempItem.test_performance = sqlDatabase.rows[i].sql_string[20];
        //        tempItem.protection_degree = sqlDatabase.rows[i].sql_string[21];
        //        tempItem.transfer_voltage_limit = sqlDatabase.rows[i].sql_string[22];
        //        tempItem.marking = sqlDatabase.rows[i].sql_string[23];

        //        UPSSpecs.Add(tempItem);

        //    }

            return true;
        }

        public bool GetGensetSpecss()
        {

            String sqlQuery = $@"select category_id,product_id,brand_id,model_id,spec_id,rating_unit,ltb_50_unit,ltb_60_unit,prp_50_unit,prp_60_unit,rated_power,ltb_50,ltb_60,prp_50,prp_60,valid_until,spec_name,ratedMeasure.measure_unit as ratedMeasure,ltb50measure.measure_unit as ltb50measure,ltb60measure.measure_unit as ltb60measure,prp50measure.measure_unit as prp50measure,prp60measure.measure_unit as prp60measure from genset_specs inner join measure_units as ratedMeasure on genset_specs.rating_unit=ratedMeasure.id
        inner join measure_units as ltb50measure on genset_specs.ltb_50_unit=ltb50measure.id
        inner join measure_units as ltb60measure on genset_specs.ltb_60_unit=ltb60measure.id
        inner join measure_units as prp50measure on genset_specs.prp_50_unit=prp50measure.id
        inner join measure_units as prp60measure on genset_specs.prp_60_unit=prp60measure.id
        where category_id={GetCategoryID()} and product_id={GetProductID()} and brand_id={GetBrandID()} and model_id={GetModelID()}";


            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 10;
            queryColumns.sql_money = 5;
            queryColumns.sql_datetime = 1;
            queryColumns.sql_string = 6;


            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            List<BASIC_STRUCTS.GENSET_SPEC> GensetSpecs = new List<BASIC_STRUCTS.GENSET_SPEC>();
            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                BASIC_STRUCTS.GENSET_SPEC tempItem = new BASIC_STRUCTS.GENSET_SPEC();

                tempItem.category = sqlDatabase.rows[i].sql_int[0];
                tempItem.product = sqlDatabase.rows[i].sql_int[1];
                tempItem.brand = sqlDatabase.rows[i].sql_int[2];
                tempItem.model = sqlDatabase.rows[i].sql_int[3];
                tempItem.spec_id = sqlDatabase.rows[i].sql_int[4];
                tempItem.rating_unit_id = sqlDatabase.rows[i].sql_int[5];
                tempItem.ltb_50_unit = sqlDatabase.rows[i].sql_int[6];
                tempItem.ltb_60_unit = sqlDatabase.rows[i].sql_int[7];
                tempItem.prp_50_unit = sqlDatabase.rows[i].sql_int[8];
                tempItem.prp_60_unit = sqlDatabase.rows[i].sql_int[9];

                tempItem.RatedPower = sqlDatabase.rows[i].sql_money[0];

                tempItem.ltb_50 = sqlDatabase.rows[i].sql_money[1];
                tempItem.ltb_60 = sqlDatabase.rows[i].sql_money[2];
                tempItem.prp_50 = sqlDatabase.rows[i].sql_money[3];
                tempItem.prp_60 = sqlDatabase.rows[i].sql_money[4];

                tempItem.spec_name = sqlDatabase.rows[i].sql_string[0];
                tempItem.rating_unit_name = sqlDatabase.rows[i].sql_string[1];
                tempItem.ltb_50_unit_name = sqlDatabase.rows[i].sql_string[2];
                tempItem.ltb_60_unit_name = sqlDatabase.rows[i].sql_string[3];
                tempItem.prp_50_unit_name = sqlDatabase.rows[i].sql_string[4];
                tempItem.prp_60_unit_name = sqlDatabase.rows[i].sql_string[5];
                tempItem.valid_Until = sqlDatabase.rows[i].sql_datetime[0];

                GensetSpecs.Add(tempItem);
            }

            GENSETSpecs = GensetSpecs;
            return true;

        }

        public bool InitializeModelSpecs()
        {
            String sqlQuery = $@"SELECT  
                                    added_by	
		                            ,category_id
		                            ,product_id
		                            ,brand_id
		                            ,model_id
		                            ,spec_id
		                            ,genset_rating_unit
		                            ,genset_ltb_50_unit
		                            ,genset_ltb_60_unit
		                            ,genset_prp_50_unit
		                            ,genset_prp_60_unit
		                            ,ups_rating
		                            ,ups_backup_time_50
		                            ,ups_backup_time_70
		                            ,ups_backup_time_100
		
		                            ,genset_ltb_50
		                            ,genset_ltb_60
		                            ,genset_prp_50
		                            ,genset_prp_60
									,genset_rated_power
		                            ,ups_rated_power
		
		                            ,model_specs.date_added
		                            ,valid_until

		                            ,ups_rating_power.measure_unit as ups_rating_measure_unit
		                            ,spec_name
		                            ,genset_rating_power.measure_unit as genset_rating_measure_unit
		                            ,ltb50measure.measure_unit as ltb50_measure_unit
		                            ,ltb60measure.measure_unit as ltb60_measure_unit
		                            ,prp50measure.measure_unit as prp50_measure_unit
		                            ,prp60measure.measure_unit as prp60_measure_unit
		                            ,genset_cooling
		                            ,genset_tank
		                            ,genset_load
		                            ,genset_alternator		
		                            ,ups_io_phase
		                            ,ups_input_power_factor
		                            ,ups_thdi
		                            ,ups_input_nominal_voltage
		                            ,ups_input_voltage
		                            ,ups_voltage_tolerance
		                            ,ups_output_power_factor
		                            ,ups_thdv
		                            ,ups_output_nominal_voltage
		                            ,ups_output_dc_voltage_range
		                            ,ups_overload_capability
		                            ,ups_efficiency
		                            ,ups_input_connection_type
		                            ,ups_front_panel
		                            ,ups_max_power
		                            ,ups_certificates
		                            ,ups_safety
		                            ,ups_emc
		                            ,ups_environmental_aspects
		                            ,ups_test_performance
		                            ,ups_protection_degree
		                            ,ups_transfer_voltage_limit
		                            ,ups_marking

		                            ,is_valid
    
                                FROM erp_system.dbo.model_specs

	                            left join erp_system.dbo.measure_units as ups_rating_power 
	                            on model_specs.ups_rating=ups_rating_power.id
	
	                            left join erp_system.dbo.measure_units as genset_rating_power 
	                            on model_specs.genset_ltb_50_unit=genset_rating_power.id
	
	                            left join erp_system.dbo.measure_units as ltb50measure 
	                            on model_specs.genset_ltb_50_unit=ltb50measure.id
	
	                            left join erp_system.dbo.measure_units as ltb60measure 
	                            on model_specs.genset_ltb_60_unit=ltb60measure.id
    
	                            left join erp_system.dbo.measure_units as prp50measure 
	                            on model_specs.genset_prp_50_unit=prp50measure.id
    
	                            left join erp_system.dbo.measure_units as prp60measure 
	                            on model_specs.genset_prp_60_unit=prp60measure.id

	                            where category_id={GetCategoryID()} and product_id={GetProductID()} and brand_id={GetBrandID()} and model_id={GetModelID()}and spec_id >13";


            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 15;
            queryColumns.sql_money = 6;
            queryColumns.sql_datetime = 2;
            queryColumns.sql_string = 34;
            queryColumns.sql_bit = 1;


            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                BASIC_STRUCTS.MODEl_SPEC_STRUCT tempItem = new BASIC_STRUCTS.MODEl_SPEC_STRUCT();

                tempItem.added_by = sqlDatabase.rows[i].sql_int[0];
                tempItem.category_id = sqlDatabase.rows[i].sql_int[1];
                tempItem.product_id = sqlDatabase.rows[i].sql_int[2];
                tempItem.brand_id = sqlDatabase.rows[i].sql_int[3];
                tempItem.model_id = sqlDatabase.rows[i].sql_int[4];
                tempItem.spec_id = sqlDatabase.rows[i].sql_int[5];
                tempItem.genset_rating_unit = sqlDatabase.rows[i].sql_int[6];
                tempItem.genset_ltb_50_unit = sqlDatabase.rows[i].sql_int[7];
                tempItem.genset_ltb_60_unit = sqlDatabase.rows[i].sql_int[8];
                tempItem.genset_prp_50_unit = sqlDatabase.rows[i].sql_int[9];
                tempItem.genset_prp_60_unit = sqlDatabase.rows[i].sql_int[10];
                tempItem.ups_rating = sqlDatabase.rows[i].sql_int[11];
                tempItem.ups_backup_time_50 = sqlDatabase.rows[i].sql_int[12];
                tempItem.ups_backup_time_70 = sqlDatabase.rows[i].sql_int[13];
                tempItem.ups_backup_time_100 = sqlDatabase.rows[i].sql_int[14];

                tempItem.genset_ltb_50 = sqlDatabase.rows[i].sql_money[0];
                tempItem.genset_ltb_60 = sqlDatabase.rows[i].sql_money[1];
                tempItem.genset_prp_50 = sqlDatabase.rows[i].sql_money[2];
                tempItem.genset_prp_60 = sqlDatabase.rows[i].sql_money[3];
                tempItem.genset_rated_power = sqlDatabase.rows[i].sql_int[4];
                tempItem.ups_rated_power = sqlDatabase.rows[i].sql_money[5];
                
                tempItem.date_added = sqlDatabase.rows[i].sql_datetime[0];
                tempItem.valid_until = sqlDatabase.rows[i].sql_datetime[1];
                
                
                tempItem.spec_name = sqlDatabase.rows[i].sql_string[0];
                tempItem.genset_rating_measure_unit = sqlDatabase.rows[i].sql_string[1];
                tempItem.genset_ltb50_measure_unit = sqlDatabase.rows[i].sql_string[2];
                tempItem.genset_ltb60_measure_unit = sqlDatabase.rows[i].sql_string[3];
                tempItem.genset_prp50_measure_unit = sqlDatabase.rows[i].sql_string[4];
                tempItem.genset_prp60_measure_unit = sqlDatabase.rows[i].sql_string[5];
                tempItem.genset_cooling = sqlDatabase.rows[i].sql_string[6];
                tempItem.genset_tank = sqlDatabase.rows[i].sql_string[7];
                tempItem.genset_load = sqlDatabase.rows[i].sql_string[8];
                tempItem.genset_alternator = sqlDatabase.rows[i].sql_string[9];
                tempItem.ups_io_phase = sqlDatabase.rows[i].sql_string[10];
                tempItem.ups_rating_measure_unit = sqlDatabase.rows[i].sql_string[11];
                tempItem.ups_input_power_factor = sqlDatabase.rows[i].sql_string[12];
                tempItem.ups_thdi = sqlDatabase.rows[i].sql_string[13];
                tempItem.ups_input_nominal_voltage = sqlDatabase.rows[i].sql_string[14];
                tempItem.ups_input_voltage = sqlDatabase.rows[i].sql_string[15];
                tempItem.ups_voltage_tolerance = sqlDatabase.rows[i].sql_string[16];
                tempItem.ups_output_power_factor = sqlDatabase.rows[i].sql_string[17];
                tempItem.ups_thdv = sqlDatabase.rows[i].sql_string[18];
                tempItem.ups_output_nominal_voltage = sqlDatabase.rows[i].sql_string[19];
                tempItem.ups_output_dc_voltage_range = sqlDatabase.rows[i].sql_string[20];
                tempItem.ups_overload_capability = sqlDatabase.rows[i].sql_string[21];
                tempItem.ups_efficiency = sqlDatabase.rows[i].sql_string[22];
                tempItem.ups_input_connection_type = sqlDatabase.rows[i].sql_string[23];
                tempItem.ups_front_panel = sqlDatabase.rows[i].sql_string[24];
                tempItem.ups_max_power = sqlDatabase.rows[i].sql_string[25];
                tempItem.ups_certificates = sqlDatabase.rows[i].sql_string[26];
                tempItem.ups_safety = sqlDatabase.rows[i].sql_string[27];
                tempItem.ups_emc = sqlDatabase.rows[i].sql_string[28];
                tempItem.ups_environmental_aspects = sqlDatabase.rows[i].sql_string[29];
                tempItem.ups_test_performance = sqlDatabase.rows[i].sql_string[30];
                tempItem.ups_protection_degree = sqlDatabase.rows[i].sql_string[31];
                tempItem.ups_transfer_voltage_limit = sqlDatabase.rows[i].sql_string[32];
                tempItem.ups_marking = sqlDatabase.rows[i].sql_string[33];
                
                tempItem.is_valid = sqlDatabase.rows[i].sql_bit[0];



                modelSpecs.Add(tempItem);
            }

            
            return true;
        }

        public bool InitializeModelApplications()
        {
            modelApplications.Clear();

            String sqlQueryPart1 = @"select model_applications.application 
                                    from erp_system.dbo.model_applications 
							        where model_applications.product_id = ";
            String sqlQueryPart2 = " and model_applications.brand_id = ";
            String sqlQueryPart3 = " and model_applications.model_id = ";
            String sqlQueryPart4 = ";";

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

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
                modelApplications.Add(sqlDatabase.rows[i].sql_string[0]);

            return true;
        }


        public bool InitializeModelBenefits()
        {
            modelBenefits.Clear();

            String sqlQueryPart1 = @"select model_benefits.benefit 
                                    from erp_system.dbo.model_benefits 
							        where model_benefits.product_id = ";


            String sqlQueryPart2 = " and model_benefits.brand_id = ";
            String sqlQueryPart3 = " and model_benefits.model_id = ";
            String sqlQueryPart4 = ";";

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

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
                modelBenefits.Add(sqlDatabase.rows[i].sql_string[0]);

            return true;
        }


        public bool InitializeModelFeatures()
        {
            modelStandardFeatures.Clear();

            String sqlQueryPart1 = @"select model_standard_features.feature 
                                    from erp_system.dbo.model_standard_features 
        							where model_standard_features.product_id = ";


            String sqlQueryPart2 = " and model_standard_features.brand_id = ";
            String sqlQueryPart3 = " and model_standard_features    .model_id = ";
            String sqlQueryPart4 = ";";

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

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
                modelStandardFeatures.Add(sqlDatabase.rows[i].sql_string[0]);

            return true;
        }


        public bool InitializeModelInfo(int mProductID, int mBrandID, int mModelID)
        {
            modelSummaryPoints.Clear();
            SetProductID(mProductID);
            SetBrandID(mBrandID);
            SetModelID(mModelID);

            //if (GetCategoryID() == COMPANY_WORK_MACROS.GENSET_CATEGORY_ID)
            //{
            //    //if (!GetGensetSpecss())
            //        return false;
            //}
            //else {
            //    //if (!this.GetUPSSpecss())
            //        return false;
            //}

            if (!InitializeModelSpecs())
                return false;
            if (!InitializeModelApplications())
                return false;
            if (!InitializeModelBenefits())
                return false;
            if (!InitializeModelFeatures())
                return false;

            GetNewModelPhotoLocalPath();
            GetNewPhotoServerPath();

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


        public bool GetNewSpecId() {


            string sqlQuery = $@"select max(spec_id)
                                   from erp_system.dbo.genset_specs
                                   where category_id ={GetCategoryID()} and product_id={GetProductID()} and brand_id={GetBrandID()} and model_id={GetModelID()}";

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            maxSpecId = sqlDatabase.rows[0].sql_int[0] + 1;

            return true;


        }

        public void GetNewModelPhotoLocalPath()
        {
            modelPhotoLocalPath = String.Empty;
            modelPhotoLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\products_photos\\"+GetProductID()+"\\"+GetBrandID()+"\\"+GetModelID()+".jpg";
        }
        public void GetNewModelPhotoServerPath()
        {
            modelPhotoServerPath = String.Empty;
            modelPhotoServerPath += BASIC_MACROS.MODELS_PHOTOS_PATH;
            modelPhotoServerPath += GetProductID();
            modelPhotoServerPath += "/";
            modelPhotoServerPath += GetBrandID();
            modelPhotoServerPath += "/";
            modelPhotoServerPath += GetModelID();
            modelPhotoServerPath += ".jpg";

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


        public string GetModelFolderLocalPath()
        {
            modelPhotoLocalPath = String.Empty;
            modelPhotoLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\" + GetProductID() + "\\" + GetBrandID();
            return modelPhotoLocalPath;
        }




        ////////////////////
        ///ISSUE FUNCTIONS
        ///////////////////
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
               int numberOfSavedModelSummaryPoints = sqlDatabase.rows[i].sql_int[0];

                if (numberOfSavedModelSummaryPoints > 0)
                    modelSummaryPoints.Add(sqlDatabase.rows[i].sql_string[0]);
            }

            return true;
        }

        public bool IssueNewModel()
        {
            if (!GetNewModelID())
                return false;
            if (!InsertIntoBrandModels())
                return false;
            if (!InsertIntoModelSummaryPoints())
                return false;
            if (GetCategoryID() == COMPANY_WORK_MACROS.GENSET_CATEGORY_ID) {
                if (!InsertIntoGensetSpecs())
                    return false;
            }
               else
            if (!InsertIntoUPSSpecs())
                return false;

            if (!InsertIntoModelApplications())
                return false;
            if (!InsertIntoModelBenefits())
                return false;
            if (!InsertIntoModelStandardFeatures())
                return false;


            GetNewModelPhotoLocalPath();
            GetNewModelPhotoServerPath();

            return true;
        }

        public bool InsertIntoModelApplications()
        {
            for (int i = 0; i < modelApplications.Count(); i++)
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
                sqlQuery += "'" + modelApplications[i] + "'";
                sqlQuery += comma;
                sqlQuery += sqlQueryPart3;
                sqlQuery += sqlQueryPart4;

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;
            }
            return true;
        }

        public bool InsertIntoModelBenefits()
        {
            for (int i = 0; i < modelBenefits.Count(); i++)
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
                sqlQuery += "'" + modelBenefits[i] + "'";
                sqlQuery += comma;
                sqlQuery += sqlQueryPart3;
                sqlQuery += sqlQueryPart4;

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;
            }
            return true;
        }


        public bool InsertIntoModelStandardFeatures()
        {
            for (int i = 0; i < modelStandardFeatures.Count(); i++)
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
                sqlQuery += "'" + modelStandardFeatures[i] + "'";
                sqlQuery += comma;
                sqlQuery += sqlQueryPart3;
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
            sqlQuery += sqlQueryPart3;
            sqlQuery += sqlQueryPart4;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool InsertIntoUPSSpecs()
        {
            int max = UPSSpecs.Count() - 1;
            if (max < 0)
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
            sqlQuery += GetCategoryID();
            sqlQuery += comma;
            sqlQuery += GetProductID();
            sqlQuery += comma;
            sqlQuery += GetBrandID();
            sqlQuery += comma;
            sqlQuery += GetModelID();
            sqlQuery += comma;
            sqlQuery += UPSSpecs[max].spec_id;
            sqlQuery += comma;
            sqlQuery += "'" + UPSSpecs[max].io_phase + "'";
            sqlQuery += comma;
            sqlQuery += UPSSpecs[max].rated_power;
            sqlQuery += comma;
            sqlQuery += UPSSpecs[max].rating_id;
            sqlQuery += comma;
            sqlQuery += UPSSpecs[max].backup_time_50;
            sqlQuery += comma;
            sqlQuery += UPSSpecs[max].backup_time_70;
            sqlQuery += comma;
            sqlQuery += UPSSpecs[max].backup_time_100;
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


        public bool InsertIntoGensetSpecs() {


            for (int i = 0; i < GENSETSpecs.Count; i++) {
                string sqlFormattedDate = GENSETSpecs[i].valid_Until.ToString("yyyy-MM-dd");
                string Date = DateTime.Now.ToString("yyyy-MM-dd");
         
                string sqlQuery = $@"insert into erp_system.dbo.genset_specs values ({GetCategoryID()},{GetProductID()},{GetBrandID()},{GetModelID()},{GENSETSpecs[i].spec_id},'{GENSETSpecs[i].spec_name}',{GENSETSpecs[i].RatedPower},{GENSETSpecs[i].rating_unit_id},{GENSETSpecs[i].ltb_50},{GENSETSpecs[i].ltb_50_unit},{GENSETSpecs[i].ltb_60},{GENSETSpecs[i].ltb_60_unit},{GENSETSpecs[i].prp_50},{GENSETSpecs[i].prp_50_unit},{GENSETSpecs[i].prp_60},{GENSETSpecs[i].prp_60_unit},'{GENSETSpecs[i].cooling}','{GENSETSpecs[i].tank}','{GENSETSpecs[i].load_percentage}','{GENSETSpecs[i].alternator}',1,{sqlFormattedDate},{Date})";

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;  
            }
            return true;
        }



        ////////////////////
        /// UPDATE FUNCTIONS
        ////////////////////

        public bool MoveModel(int ProductId, int brandId, int ModelId, int categoryId)
        {
            if (!GetNewModelID())
                return false;

            if (!InsertIntoBrandModels())
                return false;

            if (!UpdateModelSummaryPoints(ProductId, brandId, ModelId))
                return false;

            if (!UpdateModelUpsSpecs(ProductId, brandId, ModelId, categoryId))
                return false;

            if (!UpdateMaintenanceContractModel(ProductId, brandId, ModelId, categoryId))
                return false;

            if (!UpdateMaintenanceOfferModel(ProductId, brandId, ModelId, categoryId))
                return false;

            if (!UpdateOutGoingQuotationsModel(ProductId, brandId, ModelId, categoryId))
                return false;

            if (!UpdateWorkOrdersProductsLogModel(ProductId, brandId, ModelId, categoryId))
                return false;
            

            if (!UpdateWorkOrdersProductsInfoModel(ProductId, brandId, ModelId, categoryId))
                return false;

            if (!UpdateRfqProductsModel(ProductId, brandId, ModelId, categoryId))
                return false;

            if (!UpdateModelApplications(ProductId, brandId, ModelId))
                return false;
            if (!UpdateModelBenefits(ProductId, brandId, ModelId))
                return false;
            if (!UpdateModelStandardFeatures(ProductId, brandId, ModelId))
                return false;

            GetNewModelPhotoLocalPath();
            GetNewModelPhotoServerPath();

            return true;
        }


        public bool UpdateModelSummaryPoints(int ProductId, int brandId, int ModelId)
        {

            string sqlQuery = $@"UPDATE erp_system.dbo.models_summary_points set product_id={GetProductID()} , brand_id={GetBrandID()} 
                , model_id={GetModelID()} where product_id={ProductId} and brand_id={brandId} and model_id={ModelId}";


            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;

        }


        public bool UpdateModelBenefits(int ProductId, int brandId, int ModelId)
        {
            string sqlQuery = $@"UPDATE erp_system.dbo.model_benefits set product_id={GetProductID()} , brand_id={GetBrandID()} , model_id={GetModelID()} where product_id={ProductId} and brand_id={brandId} and model_id={ModelId}";

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;

        }


        public bool UpdateModelStandardFeatures(int ProductId, int brandId, int ModelId)
        {
            string sqlQuery = $@"UPDATE erp_system.dbo.model_standard_features set product_id={GetProductID()} , brand_id={GetBrandID()} , model_id={GetModelID()} where product_id={ProductId} and brand_id={brandId} and model_id={ModelId}";

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }


        public bool UpdateModelApplications(int ProductId, int brandId, int ModelId)
        {
            string sqlQuery = $@"UPDATE erp_system.dbo.model_applications set product_id={GetProductID()} , brand_id={GetBrandID()} , model_id={GetModelID()} where product_id={ProductId} and brand_id={brandId} and model_id={ModelId}";

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }


        public bool UpdateModelUpsSpecs(int ProductId, int brandId, int ModelId, int categoryId)
        {
            string sqlQuery = $@"UPDATE erp_system.dbo.ups_specs set category_id={GetCategoryID()} , product_id={GetProductID()} , brand_id={GetBrandID()} , model_id={GetModelID()} where category_id={categoryId} and product_id={ProductId} and brand_id={brandId} and model_id={ModelId}";

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool UpdateMaintenanceContractModel(int ProductId, int brandId, int ModelId, int categoryId)
        {
            string sqlQuery = $@"UPDATE erp_system.dbo.maintenance_contracts_products_info set product_category={GetCategoryID()} 
                            , product_type={GetProductID()} , product_brand={GetBrandID()} , product_model={GetModelID()} 
                    where product_category={categoryId} and product_type={ProductId} and product_brand={brandId} and product_model={ModelId}";

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }


        public bool UpdateMaintenanceOfferModel(int ProductId, int brandId, int ModelId, int categoryId)
        {
            string sqlQuery = $@"UPDATE erp_system.dbo.maintenance_offers_products_info set product_category={GetCategoryID()}
    , product_type={GetProductID()} , product_brand={GetBrandID()} , product_model={GetModelID()}
    where product_category={categoryId} and product_type={ProductId} and product_brand={brandId} and product_model={ModelId}";

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }


        public bool UpdateOutGoingQuotationsModel(int ProductId, int brandId, int ModelId, int categoryId)
        {
            string sqlQuery = $@"UPDATE erp_system.dbo.outgoing_quotations_items set product_category={GetCategoryID()}
, product_type={GetProductID()} , product_brand={GetBrandID()} , product_model={GetModelID()} 
where product_category={categoryId} and product_type={ProductId} and product_brand={brandId} and product_model={ModelId}";

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }


        public bool UpdateWorkOrdersProductsLogModel(int ProductId, int brandId, int ModelId, int categoryId)
        {
            string sqlQuery = $@"UPDATE erp_system.dbo.work_orders_products_edit_log set product_category={GetCategoryID()}
            , product_type={GetProductID()} , product_brand={GetBrandID()} 
            , product_model={GetModelID()} where product_category={categoryId} 
            and product_type={ProductId} and product_brand={brandId} and product_model={ModelId}";

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }


        public bool UpdateWorkOrdersProductsInfoModel(int ProductId, int brandId, int ModelId, int categoryId)
        {
            string sqlQuery = $@"UPDATE erp_system.dbo.work_orders_products_info set product_category={GetCategoryID()}
                , product_type={GetProductID()}
                , product_brand={GetBrandID()} , product_model={GetModelID()}
                where product_category={categoryId} and product_type={ProductId} and product_brand={brandId} and product_model={ModelId}";

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }


        public bool UpdateRfqProductsModel(int ProductId, int brandId, int ModelId, int categoryId)
        {
            string sqlQuery = $@"UPDATE erp_system.dbo.rfqs_products_info set product_category={GetCategoryID()}
                    , product_type={GetProductID()} , product_brand={GetBrandID()} , product_model={GetModelID()}
                    where product_category={categoryId} and product_type={ProductId} and product_brand={brandId} and product_model={ModelId}";

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }



        public bool DeleteModel()
        {
            string sqlQuery = $@"Delete from erp_system.dbo.brands_models where product_id={GetProductID()}
                 and brand_id={GetBrandID()} and model_id={GetModelID()}";

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }




    }
}
