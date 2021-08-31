using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Collections;
using System.Windows;
using System.Globalization;

namespace _01electronics_library
{
    public class CommonQueries
    {
        private String sqlQuery;
        private SQLServer sqlDatabase;

        private CommonFunctions commonFunctionsObject;

        public CommonQueries()
        {
            sqlDatabase = new SQLServer();
            commonFunctionsObject = new CommonFunctions();
        }
        public CommonQueries(SQLServer mSqlDatabase)
        {
            SetDatabase(mSqlDatabase);
            commonFunctionsObject = new CommonFunctions();
        }

        public void SetDatabase(SQLServer mSqlDatabase)
        {
            sqlDatabase = mSqlDatabase;
        }

        //////////////////////////////////////////////////////////////////////
        //GET COMPANY INFO FUNCTIONS
        //////////////////////////////////////////////////////////////////////

        public bool GetCompaniesSerialsAndNames(ref List<COMPANY_ORGANISATION_MACROS.COMPANY_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select company_serial, company_name from erp_system.dbo.company_name order by company_name;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.COMPANY_STRUCT tempItem;

                tempItem.company_serial = sqlDatabase.rows[i].sql_int[0];
                tempItem.company_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }
        public bool GetCompanyAddresses(int mCompanySerial, ref List<COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"with get_countries_and_states as(	select countries.id as country_id, 
                                                                        states_governorates.id as state_id, 
																	countries.country, 
																	states_governorates.state_governorate 
																from erp_system.dbo.states_governorates 
																inner join erp_system.dbo.countries 
																on states_governorates.country = countries.id 
															), 
							get_cities_and_districts as (select cities.state_governorate as state_id, 
																	cities.id as city_id, 
																	districts.id as district_id, 
																	cities.city, 
																	districts.district 
																from erp_system.dbo.cities 
																inner join erp_system.dbo.districts 
																on cities.id = districts.city 
															) 
							select company_address.address_serial, 
							company_address.address, 
							get_cities_and_districts.district, 
							get_cities_and_districts.city, 
							get_countries_and_states.state_governorate, 
							get_countries_and_states.country 
							from get_cities_and_districts 
							inner join get_countries_and_states 
							on get_cities_and_districts.state_id = get_countries_and_states.state_id 
							inner join erp_system.dbo.company_address 
							on get_cities_and_districts.district_id = company_address.address 
							where company_address.company_serial = ";

            String sqlQueryPart2 = " order by country, state_governorate, city, district;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += mCompanySerial.ToString();
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 2;
            queryColumns.sql_string = 4;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT tempItem;

                tempItem.address_serial = sqlDatabase.rows[i].sql_int[0];
                tempItem.address = sqlDatabase.rows[i].sql_int[1];

                tempItem.district = sqlDatabase.rows[i].sql_string[0];
                tempItem.city = sqlDatabase.rows[i].sql_string[1];
                tempItem.state_governorate = sqlDatabase.rows[i].sql_string[2];
                tempItem.country = sqlDatabase.rows[i].sql_string[3];

                returnVector.Add(tempItem);
            }

            return true;
        }
        public bool GetCompanyAddress(int mBranchSerial, ref COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT returnVector)
        {
            returnVector.address_serial = mBranchSerial;

            String sqlQueryPart1 = @"with get_countries_and_states as(	select countries.id as country_id, 
                                                                        states_governorates.id as state_id, 
																	countries.country, 
																	states_governorates.state_governorate 
																from erp_system.dbo.states_governorates 
																inner join erp_system.dbo.countries 
																on states_governorates.country = countries.id 
															), 
							get_cities_and_districts as (select cities.state_governorate as state_id, 
																	cities.id as city_id, 
																	districts.id as district_id, 
																	cities.city, 
																	districts.district 
																from erp_system.dbo.cities 
																inner join erp_system.dbo.districts 
																on cities.id = districts.city 
															) 
							select company_address.address, 
							get_cities_and_districts.district, 
							get_cities_and_districts.city, 
							get_countries_and_states.state_governorate, 
							get_countries_and_states.country 
							from get_cities_and_districts 
							inner join get_countries_and_states 
							on get_cities_and_districts.state_id = get_countries_and_states.state_id 
							inner join erp_system.dbo.company_address 
							on get_cities_and_districts.district_id = company_address.address 
							where company_address.address_serial = ";

            String sqlQueryPart2 = " order by country, state_governorate, city, district;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += mBranchSerial.ToString();
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 4;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            returnVector.address = sqlDatabase.rows[0].sql_int[0];

            returnVector.district = sqlDatabase.rows[0].sql_string[0];
            returnVector.city = sqlDatabase.rows[0].sql_string[1];
            returnVector.state_governorate = sqlDatabase.rows[0].sql_string[2];
            returnVector.country = sqlDatabase.rows[0].sql_string[3];

            return true;
        }
        public bool GetExistingCompanyName(String companyDomain, ref String returnVector)
        {
            returnVector = String.Empty;

            String sqlQueryPart3 = "select company_name.company_name from erp_system.dbo.company_name where company_name.domain_name = '";
            String sqlQueryPart4 = "';";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart3;
            sqlQuery += companyDomain;
            sqlQuery += sqlQueryPart4;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 0;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            returnVector = sqlDatabase.rows[0].sql_string[0];

            return true;
        }

        //////////////////////////////////////////////////////////////////////
        //GET BASIC INFO FUNCTIONS
        //////////////////////////////////////////////////////////////////////

        public bool GetDepartmentsType(ref List<COMPANY_ORGANISATION_MACROS.DEPARTMENT_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select departments_type.id, departments_type.department from erp_system.dbo.departments_type order by departments_type.department;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.DEPARTMENT_STRUCT tempItem;

                tempItem.department_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.department_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }
        public bool GetTeamsType(ref List<COMPANY_ORGANISATION_MACROS.TEAM_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select teams_type.id, teams_type.team from erp_system.dbo.teams_type order by teams_type.id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.TEAM_STRUCT tempItem;

                tempItem.team_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.team_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }
        public bool GetPositionTypes(ref List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_POSITION_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select positions_type.id, positions_type.position from erp_system.dbo.positions_type order by positions_type.id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.EMPLOYEE_POSITION_STRUCT tempItem;

                tempItem.position_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.position_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }

        public bool GetEducationalDegree(ref List<BASIC_STRUCTS.EDUCATIONAL_DEGREE_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select educational_degrees.id, educational_degrees.educational_degree from erp_system.dbo.educational_degrees order by educational_degrees.id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                BASIC_STRUCTS.EDUCATIONAL_DEGREE_STRUCT tempItem;

                tempItem.edu_degree_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.edu_degree_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }

        public bool GetEducationalMajors(ref List<BASIC_STRUCTS.EDUCATIONAL_MAJOR_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select educational_majors.id, educational_majors.educational_major from erp_system.dbo.educational_majors order by educational_majors.id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                BASIC_STRUCTS.EDUCATIONAL_MAJOR_STRUCT tempItem;

                tempItem.edu_major_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.edu_major_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }
        public bool GetDepartmentTeams(int departmentId, ref List<COMPANY_ORGANISATION_MACROS.TEAM_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select teams_type.id, teams_type.team 
                                    from erp_system.dbo.teams_type 
                                    where 
                                    (teams_type.id >= ";
            String sqlQueryPart2 = @" and teams_type.id < ";
            String sqlQueryPart3 = @") or
                                    (teams_type.id >= ";
            String sqlQueryPart4 = @" and teams_type.id < ";
            String sqlQueryPart5 = @") or
                                    (teams_type.id >= ";
            String sqlQueryPart6 = @" and teams_type.id < ";
            String sqlQueryPart7 = @")
                                    order by teams_type.team;";


            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += departmentId * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_TEAMS_PER_DEP;
            sqlQuery += sqlQueryPart2;
            sqlQuery += departmentId * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_TEAMS_PER_DEP + COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_TEAMS_PER_DEP;
            sqlQuery += sqlQueryPart3;
            sqlQuery += departmentId * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_TEAMS_PER_DEP * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_SUBTEAMS_PER_TEAM;
            sqlQuery += sqlQueryPart4;
            sqlQuery += departmentId * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_TEAMS_PER_DEP * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_SUBTEAMS_PER_TEAM + COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_TEAMS_PER_DEP * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_SUBTEAMS_PER_TEAM;
            sqlQuery += sqlQueryPart5;
            sqlQuery += departmentId * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_TEAMS_PER_DEP * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_SUBTEAMS_PER_TEAM * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_DIVISIONS_PER_SUBTEAM;
            sqlQuery += sqlQueryPart6;
            sqlQuery += departmentId * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_TEAMS_PER_DEP * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_SUBTEAMS_PER_TEAM * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_DIVISIONS_PER_SUBTEAM + COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_TEAMS_PER_DEP * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_SUBTEAMS_PER_TEAM * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_DIVISIONS_PER_SUBTEAM;
            sqlQuery += sqlQueryPart7;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.TEAM_STRUCT tempItem;

                tempItem.team_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.team_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }

        public bool GetPrimaryWorkFields(ref List<BASIC_STRUCTS.PRIMARY_FIELD_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select generic_work_fields.id, generic_work_fields.generic_work_field from erp_system.dbo.generic_work_fields order by generic_work_fields.id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                BASIC_STRUCTS.PRIMARY_FIELD_STRUCT tempItem;

                tempItem.field_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.field_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;

        }
        public bool GetSecondaryWorkFields(int primaryFieldId, List<BASIC_STRUCTS.SECONDARY_FIELD_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select specific_work_fields.id, specific_work_fields.specific_work_field from erp_system.dbo.specific_work_fields where specific_work_fields.generic_work_field = ";
            String sqlQueryPart2 = " order by specific_work_fields.id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += primaryFieldId;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                BASIC_STRUCTS.SECONDARY_FIELD_STRUCT tempItem;

                tempItem.field_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.field_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }

        public bool GetAllCountries(ref List<BASIC_STRUCTS.COUNTRY_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select countries.id, countries.country from erp_system.dbo.countries order by countries.country;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                BASIC_STRUCTS.COUNTRY_STRUCT tempItem;

                tempItem.country_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.country_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }
        public bool GetAllCountryStates(int countryId, ref List<BASIC_STRUCTS.STATE_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select states_governorates.id, states_governorates.state_governorate from erp_system.dbo.states_governorates where states_governorates.country = ";
            String sqlQueryPart2 = "order by states_governorates.state_governorate;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += countryId;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                BASIC_STRUCTS.STATE_STRUCT tempItem;

                tempItem.state_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.state_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }
        public bool GetAllStateCities(int stateId, ref List<BASIC_STRUCTS.CITY_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select cities.id, cities.city from erp_system.dbo.cities where cities.state_governorate = ";
            String sqlQueryPart2 = "order by cities.city;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += stateId;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                BASIC_STRUCTS.CITY_STRUCT tempItem;

                tempItem.city_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.city_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }
        public bool GetAllCityDistricts(int cityId, ref List<BASIC_STRUCTS.DISTRICT_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select districts.id, districts.district from erp_system.dbo.districts where districts.city = ";
            String sqlQueryPart2 = "order by districts.district;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += cityId;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                BASIC_STRUCTS.DISTRICT_STRUCT tempItem;

                tempItem.district_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.district_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }

        public bool GetVisitPurposes(ref List<COMPANY_WORK_MACROS.VISIT_PURPOSE_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select visits_purpose.id, visits_purpose.visit_purpose from erp_system.dbo.visits_purpose where visits_purpose.id > 0 order by id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_WORK_MACROS.VISIT_PURPOSE_STRUCT purposeItem;

                purposeItem.purpose_id = sqlDatabase.rows[i].sql_int[0];
                purposeItem.purpose_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(purposeItem);
            }

            COMPANY_WORK_MACROS.VISIT_PURPOSE_STRUCT tempItem;

            tempItem.purpose_id = 0;
            tempItem.purpose_name = "Other";

            returnVector.Add(tempItem);

            return true;
        }
        public bool GetVisitResults(ref List<COMPANY_WORK_MACROS.VISIT_RESULT_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select visits_result.id, visits_result.visit_result from erp_system.dbo.visits_result where visits_result.id > 0 order by visits_result.id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_WORK_MACROS.VISIT_RESULT_STRUCT resultItem;

                resultItem.result_id = sqlDatabase.rows[i].sql_int[0];
                resultItem.result_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(resultItem);
            }

            COMPANY_WORK_MACROS.VISIT_RESULT_STRUCT tempItem;

            tempItem.result_id = 0;
            tempItem.result_name = "Other";

            returnVector.Add(tempItem);

            return true;
        }

        public bool GetCallPurposes(ref List<COMPANY_WORK_MACROS.CALL_PURPOSE_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select calls_purpose.id, calls_purpose.call_purpose from erp_system.dbo.calls_purpose where calls_purpose.id > 0 order by id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_WORK_MACROS.CALL_PURPOSE_STRUCT purposeItem;

                purposeItem.purpose_id = sqlDatabase.rows[i].sql_int[0];
                purposeItem.purpose_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(purposeItem);
            }

            COMPANY_WORK_MACROS.CALL_PURPOSE_STRUCT tempItem;

            tempItem.purpose_id = 0;
            tempItem.purpose_name = "Other";

            returnVector.Add(tempItem);

            return true;
        }
        public bool GetCallResults(ref List<COMPANY_WORK_MACROS.CALL_RESULT_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select calls_result.id, calls_result.call_result from erp_system.dbo.calls_result where calls_result.id > 0 order by calls_result.id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_WORK_MACROS.CALL_RESULT_STRUCT resultItem;

                resultItem.result_id = sqlDatabase.rows[i].sql_int[0];
                resultItem.result_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(resultItem);
            }

            COMPANY_WORK_MACROS.CALL_RESULT_STRUCT tempItem;

            tempItem.result_id = 0;
            tempItem.result_name = "Other";

            returnVector.Add(tempItem);

            return true;
        }

        public bool GetMeetingPurposes(ref List<COMPANY_WORK_MACROS.MEETING_PURPOSE_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select meetings_purpose.id, meetings_purpose.meeting_purpose from erp_system.dbo.meetings_purpose where meetings_purpose.id > 0 order by meetings_purpose.id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_WORK_MACROS.MEETING_PURPOSE_STRUCT purposeItem = new COMPANY_WORK_MACROS.MEETING_PURPOSE_STRUCT();

                purposeItem.purpose_id = sqlDatabase.rows[i].sql_int[0];
                purposeItem.purpose_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(purposeItem);
            }

            COMPANY_WORK_MACROS.MEETING_PURPOSE_STRUCT tempItem = new COMPANY_WORK_MACROS.MEETING_PURPOSE_STRUCT();

            tempItem.purpose_id = 0;
            tempItem.purpose_name = "Other";

            returnVector.Add(tempItem);

            return true;
        }

        public bool GetRFQFailureReasons(ref List<COMPANY_WORK_MACROS.FAILURE_REASON_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select rfq_failure_reasons.id, rfq_failure_reasons.failure_reason from erp_system.dbo.rfq_failure_reasons order by rfq_failure_reasons.id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_WORK_MACROS.FAILURE_REASON_STRUCT reasonItem;

                reasonItem.reason_id = sqlDatabase.rows[i].sql_int[0];
                reasonItem.reason_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(reasonItem);
            }

            return true;
        }
        public bool GetOfferFailureReasons(ref List<COMPANY_WORK_MACROS.FAILURE_REASON_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select offer_failure_reasons.id, offer_failure_reasons.failure_reason from erp_system.dbo.offer_failure_reasons order by offer_failure_reasons.id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_WORK_MACROS.FAILURE_REASON_STRUCT reasonItem;

                reasonItem.reason_id = sqlDatabase.rows[i].sql_int[0];
                reasonItem.reason_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(reasonItem);
            }

            return true;
        }

        public bool GetContractTypes(ref List<BASIC_STRUCTS.CONTRACT_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select contracts_type.id, contracts_type.contract_type from erp_system.dbo.contracts_type order by contracts_type.id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                BASIC_STRUCTS.CONTRACT_STRUCT tempItem;

                tempItem.contractId = sqlDatabase.rows[i].sql_int[0];
                tempItem.contractName = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }
        public bool GetDeliveryPoints(ref List<BASIC_STRUCTS.DELIVERY_POINT_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select delivery_points.id, delivery_points.delivery_point from erp_system.dbo.delivery_points order by delivery_points.delivery_point;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                BASIC_STRUCTS.DELIVERY_POINT_STRUCT tempItem;

                tempItem.pointId = sqlDatabase.rows[i].sql_int[0];
                tempItem.pointName = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }

        public bool GetCurrencyTypes(ref List<BASIC_STRUCTS.CURRENCY_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select currencies_type.id, currencies_type.currency from erp_system.dbo.currencies_type order by currencies_type.currency;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                BASIC_STRUCTS.CURRENCY_STRUCT tempItem;

                tempItem.currencyId = sqlDatabase.rows[i].sql_int[0];
                tempItem.currencyName = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }
        public bool GetTimeUnits(ref List<BASIC_STRUCTS.TIMEUNIT_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select time_units.id, time_units.time_unit from erp_system.dbo.time_units order by time_units.id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                BASIC_STRUCTS.TIMEUNIT_STRUCT tempItem;

                tempItem.timeUnitId = sqlDatabase.rows[i].sql_int[0];
                tempItem.timeUnit = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }

        public bool GetPayrollTypes(ref List<HUMAN_RESOURCE_MACROS.PAYROLL_TYPE_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select payroll_types.id, payroll_types.payroll_type from erp_system.dbo.payroll_types order by payroll_types.id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                HUMAN_RESOURCE_MACROS.PAYROLL_TYPE_STRUCT tempItem;

                tempItem.payroll_type_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.payroll_type_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }
        //////////////////////////////////////////////////////////////////////
        //GET HUMAN RESOURCES BASIC FUNCTIONS
        //////////////////////////////////////////////////////////////////////
        ///
        public bool GetAttendanceTypes(ref List<HUMAN_RESOURCE_MACROS.ATTENDANCE_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select attendance_types.id, attendance_types.attendance_type from erp_system.dbo.attendance_types order by attendance_types.id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                HUMAN_RESOURCE_MACROS.ATTENDANCE_STRUCT tempItem;

                tempItem.attendance_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.attendance_type = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }
        public bool GetHRDocumentsType(ref List<BASIC_STRUCTS.DOCUMENT_TYPE_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select employement_documents_type.id, employement_documents_type.document_type from erp_system.dbo.employement_documents_type order by employement_documents_type.id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                BASIC_STRUCTS.DOCUMENT_TYPE_STRUCT tempItem;

                tempItem.document_type_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.document_type_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }


        public bool GetPetitionsTypes(ref List<HUMAN_RESOURCE_MACROS.ATTENDANCE_EXCUSES_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select attendance_excuses.id,
                                      		attendance_excuses.attendance_excuse
                                     from erp_system.dbo.attendance_excuses
                                     where attendance_excuses.id = ";
            String sqlQueryPart2 = @" or attendance_excuses.id = ";
            String sqlQueryPart3 = @";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += HUMAN_RESOURCE_MACROS.EARLY_LEAVE_PETITION;
            sqlQuery += sqlQueryPart2;
            sqlQuery += HUMAN_RESOURCE_MACROS.LATE_ARRIVAL_PETITION;
            sqlQuery += sqlQueryPart3;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                HUMAN_RESOURCE_MACROS.ATTENDANCE_EXCUSES_STRUCT tempItem;
                tempItem.excuse_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.attendance_excuse = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }
        public bool GetMissionTypes(ref List<HUMAN_RESOURCE_MACROS.MISSION_TYPE_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select mission_types.id, mission_types.mission_type from erp_system.dbo.mission_types where mission_types.id > 0 order by mission_types.mission_type;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                HUMAN_RESOURCE_MACROS.MISSION_TYPE_STRUCT missionItem;

                missionItem.mission_id = sqlDatabase.rows[i].sql_int[0];
                missionItem.mission_type = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(missionItem);
            }

            HUMAN_RESOURCE_MACROS.MISSION_TYPE_STRUCT tempItem;

            tempItem.mission_id = 0;
            tempItem.mission_type = "Other";

            returnVector.Add(tempItem);

            return true;
        }

        public bool GetVacationsTypes(ref List<HUMAN_RESOURCE_MACROS.ATTENDANCE_EXCUSES_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select attendance_excuses.id,
                                      		attendance_excuses.attendance_excuse
                                     from erp_system.dbo.attendance_excuses
                                     where attendance_excuses.id < ";
            String sqlQueryPart2 = @" and attendance_excuses.id > 0 
                                     order by attendance_excuses.attendance_excuse;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += HUMAN_RESOURCE_MACROS.EARLY_LEAVE_PETITION;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                HUMAN_RESOURCE_MACROS.ATTENDANCE_EXCUSES_STRUCT currentAttendanceItem;
                currentAttendanceItem.excuse_id = sqlDatabase.rows[i].sql_int[0];
                currentAttendanceItem.attendance_excuse = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(currentAttendanceItem);
            }

            HUMAN_RESOURCE_MACROS.ATTENDANCE_EXCUSES_STRUCT tempItem;

            tempItem.excuse_id = 0;
            tempItem.attendance_excuse = "Other";

            returnVector.Add(tempItem);

            return true;
        }

        //////////////////////////////////////////////////////////////////////
        //GET EMPLOYEE INFO FUNCTIONS
        //////////////////////////////////////////////////////////////////////

        public bool GetCompanyEmployees(ref List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select employees_info.employee_id, 
                                        teams_type.id,
                                        departments_type.id, 
                                        positions_type.id,
										employees_info.name,
                                        teams_type.team,
                                        departments_type.department,
                                        positions_type.position

								from erp_system.dbo.employees_info

                                left join erp_system.dbo.teams_type
                                on employees_info.employee_team = teams_type.id

                                left join erp_system.dbo.departments_type
                                on employees_info.employee_department = departments_type.id

                                left join erp_system.dbo.positions_type
                                on employees_info.employee_position = positions_type.id

                                where employees_info.currently_enrolled = 1

                                order by employees_info.name;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 4;
            queryColumns.sql_string = 4;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT tempItem = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT();
                tempItem.team = new COMPANY_ORGANISATION_MACROS.TEAM_STRUCT();
                tempItem.department = new COMPANY_ORGANISATION_MACROS.DEPARTMENT_STRUCT();

                tempItem.employee_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.team.team_id = sqlDatabase.rows[i].sql_int[1];
                tempItem.department.department_id = sqlDatabase.rows[i].sql_int[2];
                tempItem.position.position_id = sqlDatabase.rows[i].sql_int[3];

                tempItem.employee_name = sqlDatabase.rows[i].sql_string[0];
                tempItem.team.team_name = sqlDatabase.rows[i].sql_string[1];
                tempItem.department.department_name = sqlDatabase.rows[i].sql_string[2];
                tempItem.position.position_name = sqlDatabase.rows[i].sql_string[3];

                returnVector.Add(tempItem);
            }

            return true;
        }
        public bool GetDepartmentEmployees(int departmentId, ref List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select employees_info.employee_id, 
                                        employees_info.employee_team, 
										employees_info.name 
								from erp_system.dbo.employees_info 
								where employees_info.employee_department = ";
            String sqlQueryPart2 = @" and employees_info.currently_enrolled = 1 
                                     order by employees_info.name;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += departmentId;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 2;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT tempItem = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT();

                tempItem.team = new COMPANY_ORGANISATION_MACROS.TEAM_STRUCT();
                tempItem.department = new COMPANY_ORGANISATION_MACROS.DEPARTMENT_STRUCT();
                tempItem.position = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_POSITION_STRUCT();

                tempItem.employee_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.team.team_id = sqlDatabase.rows[i].sql_int[1];

                tempItem.employee_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }
        public bool GetTeamEmployees(int teamId, ref List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select employees_info.employee_id, 
                                            employees_info.employee_team, 
								    		employees_info.name 
								    from erp_system.dbo.employees_info 
								    where (employees_info.employee_team = ";
            String sqlQueryPart2 = @" or (employees_info.employee_team >= ";
            String sqlQueryPart3 = @" and employees_info.employee_team < ";
            String sqlQueryPart4 = @")
								    or (employees_info.employee_team >= ";
            String sqlQueryPart5 = @"and employees_info.employee_team < ";
            String sqlQueryPart6 = @"))
								    and employees_info.currently_enrolled = 1 
                                     order by employees_info.name; ";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += teamId;
            sqlQuery += sqlQueryPart2;
            sqlQuery += teamId * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_SUBTEAMS_PER_TEAM;
            sqlQuery += sqlQueryPart3;
            sqlQuery += teamId * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_SUBTEAMS_PER_TEAM + COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_SUBTEAMS_PER_TEAM;
            sqlQuery += sqlQueryPart4;
            sqlQuery += teamId * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_SUBTEAMS_PER_TEAM * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_DIVISIONS_PER_SUBTEAM;
            sqlQuery += sqlQueryPart5;
            sqlQuery += teamId * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_SUBTEAMS_PER_TEAM * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_DIVISIONS_PER_SUBTEAM + COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_SUBTEAMS_PER_TEAM * COMPANY_ORGANISATION_MACROS.MAX_NUMBER_OF_DIVISIONS_PER_SUBTEAM;
            sqlQuery += sqlQueryPart6;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 2;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT tempItem = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT();

                tempItem.team = new COMPANY_ORGANISATION_MACROS.TEAM_STRUCT();
                tempItem.department = new COMPANY_ORGANISATION_MACROS.DEPARTMENT_STRUCT();
                tempItem.position = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_POSITION_STRUCT();

                tempItem.employee_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.team.team_id = sqlDatabase.rows[i].sql_int[1];
                tempItem.employee_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        ///PETITIONS & LEAVES
        //////////////////////////////////////////////////////////////////////////////////////////////

        public bool GetPetitionsRequests(DateTime startDate, DateTime endDate, ref List<HUMAN_RESOURCE_MACROS.PETITION_REQUEST_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"with get_benificiary_personnel	as (select employee_id as benificiary_id,
                                    										name as benificiary_name
                                    									from erp_system.dbo.employees_info),
                                    get_requestor					as (select employee_id as requestor_id,
                                    										name as requestor_name
                                    									from erp_system.dbo.employees_info)
                                    select attendance_excuses.id as vacation_leave_id,
                                    		get_benificiary_personnel.benificiary_id,
                                    		get_requestor.requestor_id,
                                    		petitions_requests.requested_day,
                                    		petitions_requests.date_added,
                                    		attendance_excuses.attendance_excuse,
                                    		get_benificiary_personnel.benificiary_name,
                                    		get_requestor.requestor_name
                                    from erp_system.dbo.petitions_requests		
                                    inner join erp_system.dbo.attendance_excuses
                                    on petitions_requests.petition_type = attendance_excuses.id
                                    inner join get_benificiary_personnel
                                    on petitions_requests.benficiary_personnel = get_benificiary_personnel.benificiary_id
                                    inner join get_requestor
                                    on petitions_requests.requested_by = get_requestor.requestor_id
                                    where petitions_requests.requested_day >= '";
            String sqlQueryPart2 = "' and petitions_requests.requested_day <= '";
            String sqlQueryPart3 = "' order by benificiary_id, requested_day;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += startDate.ToString("yyyy-MM-dd");
            sqlQuery += sqlQueryPart2;
            sqlQuery += endDate.ToString("yyyy-MM-dd");
            sqlQuery += sqlQueryPart3;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 3;
            queryColumns.sql_datetime = 2;
            queryColumns.sql_string = 3;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                HUMAN_RESOURCE_MACROS.PETITION_REQUEST_STRUCT tempItem = new HUMAN_RESOURCE_MACROS.PETITION_REQUEST_STRUCT();

                tempItem.attendance_excuse.excuse_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.beneficiary_personnel_id = sqlDatabase.rows[i].sql_int[1];
                tempItem.requestor_id = sqlDatabase.rows[i].sql_int[2];

                tempItem.requested_date = sqlDatabase.rows[i].sql_datetime[0].Date;
                tempItem.date_added = sqlDatabase.rows[i].sql_datetime[1];

                tempItem.attendance_excuse.attendance_excuse = sqlDatabase.rows[i].sql_string[0];
                tempItem.beneficiary_personnel_name = sqlDatabase.rows[i].sql_string[1];
                tempItem.requestor_name = sqlDatabase.rows[i].sql_string[2];

                returnVector.Add(tempItem);
            }
            return true;
        }
        public bool GetPublicHolidays(DateTime startDate, DateTime endDate, ref List<HUMAN_RESOURCE_MACROS.PUBLIC_HOLIDAY_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select public_holidays.holiday_serial,
                                    		public_holidays.added_by,
                                    	public_holidays.start_date,
                                    	public_holidays.end_date,
                                    	public_holidays.date_added,
                                    	public_holidays.holiday_name,
                                    	employees_info.name
                                    from erp_system.dbo.public_holidays
                                    inner join erp_system.dbo.employees_info
                                    on public_holidays.added_by = employees_info.employee_id
                                    where public_holidays.start_date >= '";
            String sqlQueryPart2 = "' and public_holidays.end_date <= '";
            String sqlQueryPart3 = "' order by public_holidays.start_date;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += startDate.ToString("yyyy-MM-dd");
            sqlQuery += sqlQueryPart2;
            sqlQuery += endDate.ToString("yyyy-MM-dd");
            sqlQuery += sqlQueryPart3;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 2;
            queryColumns.sql_datetime = 3;
            queryColumns.sql_string = 2;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                HUMAN_RESOURCE_MACROS.PUBLIC_HOLIDAY_STRUCT tempItem = new HUMAN_RESOURCE_MACROS.PUBLIC_HOLIDAY_STRUCT();

                tempItem.holiday_serial = sqlDatabase.rows[i].sql_int[0];
                tempItem.holiday_name = sqlDatabase.rows[i].sql_string[0];

                tempItem.added_by_id = sqlDatabase.rows[i].sql_int[1];
                tempItem.added_by_name = sqlDatabase.rows[i].sql_string[1];

                tempItem.start_date = sqlDatabase.rows[i].sql_datetime[0];
                tempItem.end_date = sqlDatabase.rows[i].sql_datetime[1];
                tempItem.date_added = sqlDatabase.rows[i].sql_datetime[1];

                returnVector.Add(tempItem);
            }
            return true;
        }
        public bool GetMissions(DateTime startDate, DateTime endDate, ref List<HUMAN_RESOURCE_MACROS.MISSION_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"with get_employee_on_mission	as (select employee_id as benificiary_id,
                                    										name as benificiary_name
                                    									from erp_system.dbo.employees_info),
                                    	get_requestor				as (select employee_id as requestor_id,
                                    										name as requestor_name
                                    									from erp_system.dbo.employees_info)
                                    
                                    select missions.mission_serial,
                                    	missions.branch_serial,
                                    	missions.employee_id,
										missions.mission_type,
                                    	missions.added_by,
                                    	
										company_address.address,
										
                                    	missions.mission_date,
                                    	missions.date_added,
                                    
                                    	company_name.company_name,
                                    	get_employee_on_mission.benificiary_name,
                                    	get_requestor.requestor_name,
										mission_types.mission_type,
                                    	missions.work_order,

										districts.district,
										cities.city,
										states_governorates.state_governorate,
										countries.country
                                    
                                    from erp_system.dbo.missions
                                    inner join get_employee_on_mission
                                    on missions.employee_id = get_employee_on_mission.benificiary_id
                                    inner join get_requestor
                                    on missions.added_by = get_requestor.requestor_id
                                    inner join erp_system.dbo.company_address
                                    on missions.branch_serial = company_address.address_serial
                                    inner join erp_system.dbo.company_name
                                    on company_address.company_serial = company_name.company_serial
									inner join erp_system.dbo.mission_types
									on missions.mission_type = mission_types.id
									inner join erp_system.dbo.districts
									on company_address.address = districts.id
									inner join erp_system.dbo.cities
									on districts.city = cities.id
									inner join erp_system.dbo.states_governorates
									on cities.state_governorate = states_governorates.id
									inner join erp_system.dbo.countries
									on states_governorates.country = countries.id
									where missions.mission_date >= '";
            String sqlQueryPart2 = "' and missions.mission_date <= '";
            String sqlQueryPart3 = "' order by missions.mission_date, get_employee_on_mission.benificiary_name;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += startDate.ToString("yyyy-MM-dd");
            sqlQuery += sqlQueryPart2;
            sqlQuery += endDate.ToString("yyyy-MM-dd");
            sqlQuery += sqlQueryPart3;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 6;
            queryColumns.sql_datetime = 2;
            queryColumns.sql_string = 9;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                HUMAN_RESOURCE_MACROS.MISSION_STRUCT tempItem = new HUMAN_RESOURCE_MACROS.MISSION_STRUCT();
                tempItem.branch = new COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT();
                tempItem.mission_type = new HUMAN_RESOURCE_MACROS.MISSION_TYPE_STRUCT();

                tempItem.mission_serial = sqlDatabase.rows[i].sql_int[0];
                tempItem.branch.address_serial = sqlDatabase.rows[i].sql_int[1];
                tempItem.employee_on_mission_id = sqlDatabase.rows[i].sql_int[2];
                tempItem.mission_type.mission_id = sqlDatabase.rows[i].sql_int[3];
                tempItem.added_by_id = sqlDatabase.rows[i].sql_int[4];

                tempItem.branch.address = sqlDatabase.rows[i].sql_int[5];

                tempItem.company_name = sqlDatabase.rows[i].sql_string[0];
                tempItem.employee_on_mission_name = sqlDatabase.rows[i].sql_string[1];
                tempItem.added_by_name = sqlDatabase.rows[i].sql_string[2];
                tempItem.mission_type.mission_type = sqlDatabase.rows[i].sql_string[3];
                tempItem.work_order = sqlDatabase.rows[i].sql_string[4];

                tempItem.branch.district = sqlDatabase.rows[i].sql_string[5];
                tempItem.branch.city = sqlDatabase.rows[i].sql_string[6];
                tempItem.branch.state_governorate = sqlDatabase.rows[i].sql_string[7];
                tempItem.branch.country = sqlDatabase.rows[i].sql_string[8];

                tempItem.mission_date = sqlDatabase.rows[i].sql_datetime[0];
                tempItem.date_added = sqlDatabase.rows[i].sql_datetime[1];

                returnVector.Add(tempItem);
            }
            return true;
        }
        public bool GetVacationsRequests(DateTime startDate, DateTime endDate, ref List<HUMAN_RESOURCE_MACROS.VACATION_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"with get_benificiary_personnel	as (select employee_id as benificiary_id,
											employees_info.employee_department as benificiary_dep,
											employees_info.employee_team as benificiary_team,
											employees_info.employee_position as benificiary_pos,
                                    	 									name as benificiary_name
                                    	 								from erp_system.dbo.employees_info),
                                    	 get_requestor				as (select employee_id as requestor_id,
																				employees_info.employee_department as requestor_dep,
																				employees_info.employee_team as requestor_team,
																				employees_info.employee_position as requestor_pos,
                                    	 									name as requestor_name
                                    	 								from erp_system.dbo.employees_info),
                                    	get_approver				as (select employee_id as approver_id,
																				employees_info.employee_department as approver_dep,
																				employees_info.employee_team as approver_team,
																				employees_info.employee_position as approver_pos,
                                    	 									name as approver_name
                                    	 								from erp_system.dbo.employees_info)
                                    
                                    select vacation_leave_requests.request_serial,
                                    		get_benificiary_personnel.benificiary_id,
											get_benificiary_personnel.benificiary_dep,
											get_benificiary_personnel.benificiary_team,
											get_benificiary_personnel.benificiary_pos,
                                    		get_requestor.requestor_id,
											get_requestor.requestor_dep,
											get_requestor.requestor_team,
											get_requestor.requestor_pos,
                                    		get_approver.approver_id,
											get_approver.approver_dep,
											get_approver.approver_team,
											get_approver.approver_pos,
                                    		attendance_excuses.id,
                                    		vacation_leave_request_status.id,
                                    
                                    		vacation_leave_requests.leave_start_date,
                                    		vacation_leave_requests.leave_end_date,
                                    		
                                    		vacation_leave_requests.date_added as issue_date,
                                    		vacation_leave_approvals_rejections.date_added as approval_date,
                                    		vacation_leave_requests.expiry_date,
                                    		
                                    		get_benificiary_personnel.benificiary_name,
                                    		get_requestor.requestor_name,
                                    		get_approver.approver_name,
                                    		attendance_excuses.attendance_excuse,
                                    		vacation_leave_request_status.request_status
                                    
                                    from erp_system.dbo.vacation_leave_requests
                                    
                                    left join erp_system.dbo.vacation_leave_approvals_rejections
                                    on vacation_leave_requests.request_serial = vacation_leave_approvals_rejections.request_serial
                                    
                                    inner join erp_system.dbo.attendance_excuses
                                    on vacation_leave_requests.request_type = attendance_excuses.id
                                    
                                    inner join erp_system.dbo.vacation_leave_request_status
                                    on vacation_leave_requests.request_status = vacation_leave_request_status.id
                                    
                                    inner join get_benificiary_personnel
                                    on vacation_leave_requests.benficiary_personnel = get_benificiary_personnel.benificiary_id
                                    inner join get_requestor
                                    on vacation_leave_requests.requested_by = get_requestor.requestor_id
                                    left join get_approver
                                    on vacation_leave_approvals_rejections.approving_personnel = get_approver.approver_id

									where (vacation_leave_requests.leave_start_date >= '";
            String sqlQueryPart2 = "' and vacation_leave_requests.leave_start_date <= '";
            String sqlQueryPart3 = "') or (vacation_leave_requests.leave_end_date >= '";
            String sqlQueryPart4 = "' and vacation_leave_requests.leave_end_date <= '";
            String sqlQueryPart5 = "') order by vacation_leave_requests.request_serial;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += startDate.ToString("yyyy-MM-dd");
            sqlQuery += sqlQueryPart2;
            sqlQuery += endDate.ToString("yyyy-MM-dd");
            sqlQuery += sqlQueryPart3;
            sqlQuery += startDate.ToString("yyyy-MM-dd");
            sqlQuery += sqlQueryPart4;
            sqlQuery += endDate.ToString("yyyy-MM-dd");
            sqlQuery += sqlQueryPart5;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 15;
            queryColumns.sql_datetime = 5;
            queryColumns.sql_string = 5;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                HUMAN_RESOURCE_MACROS.VACATION_STRUCT tempItem = new HUMAN_RESOURCE_MACROS.VACATION_STRUCT();
                tempItem.vacation_type = new HUMAN_RESOURCE_MACROS.ATTENDANCE_EXCUSES_STRUCT();
                tempItem.vacation_status = new HUMAN_RESOURCE_MACROS.VACATION_STATUS_STRUCT();

                tempItem.benificiary_personnel = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT();
                tempItem.requestor = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT();
                tempItem.listOfApprovals = new List<HUMAN_RESOURCE_MACROS.VACATION_APPROVAL_REJECTION_LIST_STRUCT>();

                tempItem.request_serial = sqlDatabase.rows[i].sql_int[0];

                tempItem.benificiary_personnel.employee_name = sqlDatabase.rows[i].sql_string[0];
                tempItem.benificiary_personnel.employee_id = sqlDatabase.rows[i].sql_int[1];
                tempItem.benificiary_personnel.department.department_id = sqlDatabase.rows[i].sql_int[2];
                tempItem.benificiary_personnel.team.team_id = sqlDatabase.rows[i].sql_int[3];
                tempItem.benificiary_personnel.position.position_id = sqlDatabase.rows[i].sql_int[4];

                tempItem.requestor.employee_name = sqlDatabase.rows[i].sql_string[1];
                tempItem.requestor.employee_id = sqlDatabase.rows[i].sql_int[5];
                tempItem.requestor.department.department_id = sqlDatabase.rows[i].sql_int[6];
                tempItem.requestor.team.team_id = sqlDatabase.rows[i].sql_int[7];
                tempItem.requestor.position.position_id = sqlDatabase.rows[i].sql_int[8];

                tempItem.vacation_type.excuse_id = sqlDatabase.rows[i].sql_int[13];
                tempItem.vacation_type.attendance_excuse = sqlDatabase.rows[i].sql_string[3];

                tempItem.vacation_status.status_id = sqlDatabase.rows[i].sql_int[14];
                tempItem.vacation_status.vacation_status = sqlDatabase.rows[i].sql_string[4];

                tempItem.vacation_start_date = sqlDatabase.rows[i].sql_datetime[0];
                tempItem.vacation_end_date = sqlDatabase.rows[i].sql_datetime[1];

                tempItem.request_issue_date = sqlDatabase.rows[i].sql_datetime[2];
                tempItem.request_expiry_date = sqlDatabase.rows[i].sql_datetime[3];


                if (i > 0 && returnVector.Last().request_serial == tempItem.request_serial && sqlDatabase.rows[i].sql_int[9] != 0)
                {
                    HUMAN_RESOURCE_MACROS.VACATION_APPROVAL_REJECTION_LIST_STRUCT currentApproverItem = new HUMAN_RESOURCE_MACROS.VACATION_APPROVAL_REJECTION_LIST_STRUCT();

                    currentApproverItem.approver.employee_name = sqlDatabase.rows[i].sql_string[2];
                    currentApproverItem.approver.employee_id = sqlDatabase.rows[i].sql_int[9];
                    currentApproverItem.approver.department.department_id = sqlDatabase.rows[i].sql_int[10];
                    currentApproverItem.approver.team.team_id = sqlDatabase.rows[i].sql_int[11];
                    currentApproverItem.approver.position.position_id = sqlDatabase.rows[i].sql_int[12];

                    currentApproverItem.approval_rejection_date = sqlDatabase.rows[i].sql_datetime[4];

                    returnVector.Last().listOfApprovals.Add(currentApproverItem);
                    continue;
                }
                else if (sqlDatabase.rows[i].sql_int[9] != 0)
                {
                    HUMAN_RESOURCE_MACROS.VACATION_APPROVAL_REJECTION_LIST_STRUCT currentApproverItem = new HUMAN_RESOURCE_MACROS.VACATION_APPROVAL_REJECTION_LIST_STRUCT();

                    currentApproverItem.approver.employee_name = sqlDatabase.rows[i].sql_string[2];
                    currentApproverItem.approver.employee_id = sqlDatabase.rows[i].sql_int[9];
                    currentApproverItem.approver.department.department_id = sqlDatabase.rows[i].sql_int[10];
                    currentApproverItem.approver.team.team_id = sqlDatabase.rows[i].sql_int[11];
                    currentApproverItem.approver.position.position_id = sqlDatabase.rows[i].sql_int[12];

                    currentApproverItem.approval_rejection_date = sqlDatabase.rows[i].sql_datetime[4];

                    tempItem.listOfApprovals.Add(currentApproverItem);
                }


                returnVector.Add(tempItem);

            }
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        ///ATTENDANCE
        //////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetEmployeesAttendanceSummary(DateTime startDate, DateTime endDate, ref List<HUMAN_RESOURCE_MACROS.EMPLOYEE_ATTENDANCE_SUMMARY_STRUCT> returnVector)
        {
            returnVector.Clear();


            String sqlQueryPart1 = @"select * from erp_system.dbo.attendance_summary('";
            String sqlQueryPart2 = @"','";
            String sqlQueryPart3 = @"') order by attendance_summary.employee_name, attendance_summary.attendance_id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += startDate.Date.ToString("yyyy-MM-dd");
            sqlQuery += sqlQueryPart2;
            sqlQuery += endDate.Date.ToString("yyyy-MM-dd");
            sqlQuery += sqlQueryPart3;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 3;
            queryColumns.sql_string = 2;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;


            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                HUMAN_RESOURCE_MACROS.EMPLOYEE_ATTENDANCE_SUMMARY_STRUCT currentEmployeeRecord = new HUMAN_RESOURCE_MACROS.EMPLOYEE_ATTENDANCE_SUMMARY_STRUCT();
                currentEmployeeRecord.attendanceTypeCount = new int[HUMAN_RESOURCE_MACROS.NO_OF_ATTENDANCE_TYPES];

                currentEmployeeRecord.employee_id = sqlDatabase.rows[i].sql_int[0];
                currentEmployeeRecord.employee_name = sqlDatabase.rows[i].sql_string[0];

                for (int j = 0; j < HUMAN_RESOURCE_MACROS.NO_OF_ATTENDANCE_TYPES; j++)
                    currentEmployeeRecord.attendanceTypeCount[j] = 0;

                if (i > 0 && returnVector.Last().employee_id == currentEmployeeRecord.employee_id)
                {
                    returnVector.Last().attendanceTypeCount[sqlDatabase.rows[i].sql_int[1]] = sqlDatabase.rows[i].sql_int[2];
                }
                else
                {
                    currentEmployeeRecord.attendanceTypeCount[sqlDatabase.rows[i].sql_int[1]] = sqlDatabase.rows[i].sql_int[2];
                    returnVector.Add(currentEmployeeRecord);
                }


            }

            return true;
        }
        public bool GetEmployeesAttendanceDetials(DateTime startDate, DateTime endDate, ref List<HUMAN_RESOURCE_MACROS.EMPLOYEE_ATTENDANCE_DETAILS_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select employees_attendance.employee_id,
                                    		employees_attendance.attendance_type,
                                    		employees_attendance.calendar_date,
                                    		employees_attendance.first_in,
                                    		employees_attendance.last_out,
                                    		employees_info.name,
                                    		attendance_types.attendance_type
                                    from erp_system.dbo.employees_attendance
                                    inner join erp_system.dbo.employees_info
                                    on employees_attendance.employee_id = employees_info.employee_id
                                    inner join erp_system.dbo.attendance_types
                                    on employees_attendance.attendance_type = attendance_types.id
                                    where employees_info.currently_enrolled = 1
                                    and employees_attendance.calendar_date >= '";
            String sqlQueryPart2 = @"' and employees_attendance.calendar_date <='";
            String sqlQueryPart3 = @"' order by employees_info.name;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += startDate.Date.ToString("yyyy-MM-dd");
            sqlQuery += sqlQueryPart2;
            sqlQuery += endDate.Date.ToString("yyyy-MM-dd");
            sqlQuery += sqlQueryPart3;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 2;
            queryColumns.sql_datetime = 1;
            queryColumns.sql_time = 2;
            queryColumns.sql_string = 2;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                HUMAN_RESOURCE_MACROS.EMPLOYEE_ATTENDANCE_DETAILS_STRUCT tempEmployeeRecordItem = new HUMAN_RESOURCE_MACROS.EMPLOYEE_ATTENDANCE_DETAILS_STRUCT();
                tempEmployeeRecordItem.firstInLastOutRecord = new HUMAN_RESOURCE_MACROS.FIRSTIN_LASTOUT_STRUCT[commonFunctionsObject.GetDayDifference(startDate, endDate) + 1];
                tempEmployeeRecordItem.attendanceStatus = new HUMAN_RESOURCE_MACROS.ATTENDANCE_STRUCT[commonFunctionsObject.GetDayDifference(startDate, endDate) + 1];

                tempEmployeeRecordItem.employee_id = sqlDatabase.rows[i].sql_int[0];
                tempEmployeeRecordItem.employee_name = sqlDatabase.rows[i].sql_string[0];

                DateTime currentDay = new DateTime(sqlDatabase.rows[i].sql_datetime[0].Year,
                                                       sqlDatabase.rows[i].sql_datetime[0].Month,
                                                       sqlDatabase.rows[i].sql_datetime[0].Day);

                if (i > 0 && returnVector.Last().employee_id == tempEmployeeRecordItem.employee_id)
                {
                    returnVector.Last().firstInLastOutRecord[commonFunctionsObject.GetDayDifference(startDate, currentDay)].workDay = new DateTime(sqlDatabase.rows[i].sql_datetime[0].Year,
                                                                                                                                                    sqlDatabase.rows[i].sql_datetime[0].Month,
                                                                                                                                                    sqlDatabase.rows[i].sql_datetime[0].Day);

                    returnVector.Last().firstInLastOutRecord[commonFunctionsObject.GetDayDifference(startDate, currentDay)].firstIn = new TimeSpan(sqlDatabase.rows[i].sql_time[0].Hours,
                                                                                                                                                    sqlDatabase.rows[i].sql_time[0].Minutes,
                                                                                                                                                    sqlDatabase.rows[i].sql_time[0].Seconds);

                    returnVector.Last().firstInLastOutRecord[commonFunctionsObject.GetDayDifference(startDate, currentDay)].lastOut = new TimeSpan(sqlDatabase.rows[i].sql_time[1].Hours,
                                                                                                                                               sqlDatabase.rows[i].sql_time[1].Minutes,
                                                                                                                                               sqlDatabase.rows[i].sql_time[1].Seconds);

                    returnVector.Last().attendanceStatus[commonFunctionsObject.GetDayDifference(startDate, currentDay)].attendance_id = sqlDatabase.rows[i].sql_int[1];
                    returnVector.Last().attendanceStatus[commonFunctionsObject.GetDayDifference(startDate, currentDay)].attendance_type = sqlDatabase.rows[i].sql_string[1];

                }
                else
                {
                    tempEmployeeRecordItem.firstInLastOutRecord[commonFunctionsObject.GetDayDifference(startDate, currentDay)].workDay = new DateTime(sqlDatabase.rows[i].sql_datetime[0].Year,
                                                                                                                                                sqlDatabase.rows[i].sql_datetime[0].Month,
                                                                                                                                                sqlDatabase.rows[i].sql_datetime[0].Day);

                    tempEmployeeRecordItem.firstInLastOutRecord[commonFunctionsObject.GetDayDifference(startDate, currentDay)].firstIn = new TimeSpan(sqlDatabase.rows[i].sql_time[0].Hours,
                                                                                                                                                    sqlDatabase.rows[i].sql_time[0].Minutes,
                                                                                                                                                    sqlDatabase.rows[i].sql_time[0].Seconds);

                    tempEmployeeRecordItem.firstInLastOutRecord[commonFunctionsObject.GetDayDifference(startDate, currentDay)].lastOut = new TimeSpan(sqlDatabase.rows[i].sql_time[1].Hours,
                                                                                                                                                    sqlDatabase.rows[i].sql_time[1].Minutes,
                                                                                                                                                    sqlDatabase.rows[i].sql_time[1].Seconds);


                    tempEmployeeRecordItem.attendanceStatus[commonFunctionsObject.GetDayDifference(startDate, currentDay)].attendance_id = sqlDatabase.rows[i].sql_int[1];
                    tempEmployeeRecordItem.attendanceStatus[commonFunctionsObject.GetDayDifference(startDate, currentDay)].attendance_type = sqlDatabase.rows[i].sql_string[1];

                    returnVector.Add(tempEmployeeRecordItem);
                }
            }
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        ///SALARIES & PAYROLL
        //////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetEmployeesPayrollInfo(ref List<HUMAN_RESOURCE_MACROS.PAYROLL_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select employees_info.employee_id,
                               		employees_payroll_info.payroll_type,
                               		employees_payroll_info.bank_payroll_id,
                                    employees_payroll_info.branch_code,
									employees_payroll_info.account_id,
                                    employees_payroll_info.iban_id,
                               		employees_info.name,
                               		payroll_types.payroll_type
                                from erp_system.dbo.employees_payroll_info
                               inner join erp_system.dbo.employees_info
                               on employees_payroll_info.employee_id = employees_info.employee_id
                               inner join erp_system.dbo.payroll_types
                               on employees_payroll_info.payroll_type = payroll_types.id
							   where employees_payroll_info.payroll_type > 0
                               and employees_info.currently_enrolled = 1
                               order by employees_info.name, employees_payroll_info.payroll_type;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 3;
            queryColumns.sql_smallint = 1;
            queryColumns.sql_bigint = 2;
            queryColumns.sql_string = 2;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                HUMAN_RESOURCE_MACROS.PAYROLL_STRUCT tempItem = new HUMAN_RESOURCE_MACROS.PAYROLL_STRUCT();
                tempItem.banksList = new List<HUMAN_RESOURCE_MACROS.BANK_STRUCT>();

                tempItem.employee_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.employee_name = sqlDatabase.rows[i].sql_string[0];

                if (i > 0 && returnVector.Last().employee_id == tempItem.employee_id)
                {
                    HUMAN_RESOURCE_MACROS.BANK_STRUCT tempBankItem = new HUMAN_RESOURCE_MACROS.BANK_STRUCT();

                    tempBankItem.bank_id = sqlDatabase.rows[i].sql_int[1];
                    tempBankItem.branch_id = sqlDatabase.rows[i].sql_smallint[0];
                    tempBankItem.payroll_id = sqlDatabase.rows[i].sql_int[2];

                    tempBankItem.payroll_type = sqlDatabase.rows[i].sql_string[1];

                    tempBankItem.account_id = (ulong)sqlDatabase.rows[i].sql_bigint[0];
                    tempBankItem.iban_id = (ulong)sqlDatabase.rows[i].sql_bigint[1];

                    returnVector.Last().banksList.Add(tempBankItem);
                }
                else
                {
                    HUMAN_RESOURCE_MACROS.BANK_STRUCT tempBankItem = new HUMAN_RESOURCE_MACROS.BANK_STRUCT();

                    tempBankItem.bank_id = sqlDatabase.rows[i].sql_int[1];
                    tempBankItem.branch_id = sqlDatabase.rows[i].sql_smallint[0];
                    tempBankItem.payroll_id = sqlDatabase.rows[i].sql_int[2];

                    tempBankItem.payroll_type = sqlDatabase.rows[i].sql_string[1];

                    tempBankItem.account_id = (ulong)sqlDatabase.rows[i].sql_bigint[0];
                    tempBankItem.iban_id = (ulong)sqlDatabase.rows[i].sql_bigint[1];

                    tempItem.banksList.Add(tempBankItem);
                    returnVector.Add(tempItem);
                }
            }

            return true;
        }
        public bool GetEmployeesBasicSalaries(ref List<HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select employees_info.employee_id,
                                    employees_basic_salaries.due_year,
                                    employees_basic_salaries.due_month,
                               		employees_basic_salaries.gross_salary,
									employees_basic_salaries.insurance_and_taxes,
									employees_basic_salaries.net_salary,
                                    employees_info.name
                                from erp_system.dbo.employees_basic_salaries
                               inner join erp_system.dbo.employees_info
                               on employees_basic_salaries.employee_id = employees_info.employee_id
                               where employees_info.currently_enrolled = 1
                               order by employees_info.name, employees_basic_salaries.due_year desc, employees_basic_salaries.due_month desc";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 3;
            queryColumns.sql_money = 3;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT tempItem = new HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT();
                tempItem.employee_id = sqlDatabase.rows[i].sql_int[0];

                tempItem.due_year = sqlDatabase.rows[i].sql_int[1];
                tempItem.due_month = sqlDatabase.rows[i].sql_int[2];

                tempItem.gross_salary = sqlDatabase.rows[i].sql_money[0];
                tempItem.insurance_and_tax = sqlDatabase.rows[i].sql_money[1];
                tempItem.net_salary = sqlDatabase.rows[i].sql_money[2];

                tempItem.employee_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }
        public bool GetEmployeesMonthlySalaries(ref List<HUMAN_RESOURCE_MACROS.SALARIES_HISTORY_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select employees_monthly_salaries.employee_id,
                                    		employees_monthly_salaries.payroll_type,
                                    		employees_monthly_salaries.due_year,
                                    		employees_monthly_salaries.due_month,
                                    		employees_monthly_salaries.salary_due,
                                    		employees_monthly_salaries.date_added,
                                    		employees_info.name,
                                    		payroll_types.payroll_type
                                    from erp_system.dbo.employees_monthly_salaries
                                    inner join erp_system.dbo.employees_info
                                    on employees_monthly_salaries.employee_id = employees_info.employee_id
                                    inner join erp_system.dbo.payroll_types
                                    on employees_monthly_salaries.payroll_type = payroll_types.id
                                    where employees_info.currently_enrolled = 1
                                    order by employees_info.name";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 4;
            queryColumns.sql_money = 1;
            queryColumns.sql_datetime = 1;
            queryColumns.sql_string = 2;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                HUMAN_RESOURCE_MACROS.SALARIES_HISTORY_STRUCT tempItem;
                tempItem.monthlyList = new List<HUMAN_RESOURCE_MACROS.MONTHLY_SALARY_STRUCT>();

                tempItem.employee_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.employee_name = sqlDatabase.rows[i].sql_string[0];

                if (i > 0 && tempItem.employee_id == returnVector.Last().employee_id)
                {
                    HUMAN_RESOURCE_MACROS.MONTHLY_SALARY_STRUCT tempMonthSalaryItem = new HUMAN_RESOURCE_MACROS.MONTHLY_SALARY_STRUCT();

                    tempMonthSalaryItem.payroll_id = sqlDatabase.rows[i].sql_int[1];

                    tempMonthSalaryItem.year = sqlDatabase.rows[i].sql_int[2];
                    tempMonthSalaryItem.month = sqlDatabase.rows[i].sql_int[3];

                    tempMonthSalaryItem.received_salary = sqlDatabase.rows[i].sql_money[0];

                    tempMonthSalaryItem.payroll_type = sqlDatabase.rows[i].sql_string[1];

                    tempMonthSalaryItem.transaction_time = sqlDatabase.rows[i].sql_datetime[0];

                    returnVector.Last().monthlyList.Add(tempMonthSalaryItem);
                }
                else
                {
                    HUMAN_RESOURCE_MACROS.MONTHLY_SALARY_STRUCT tempMonthSalaryItem = new HUMAN_RESOURCE_MACROS.MONTHLY_SALARY_STRUCT();

                    tempMonthSalaryItem.payroll_id = sqlDatabase.rows[i].sql_int[1];

                    tempMonthSalaryItem.year = sqlDatabase.rows[i].sql_int[2];
                    tempMonthSalaryItem.month = sqlDatabase.rows[i].sql_int[3];

                    tempMonthSalaryItem.received_salary = sqlDatabase.rows[i].sql_money[0];

                    tempMonthSalaryItem.payroll_type = sqlDatabase.rows[i].sql_string[1];

                    tempMonthSalaryItem.transaction_time = sqlDatabase.rows[i].sql_datetime[0];

                    tempItem.monthlyList.Add(tempMonthSalaryItem);
                    returnVector.Add(tempItem);
                }
            }

            return true;
        }

        public bool GetEmployeesMonthDeductionsDetails(DateTime startDate, DateTime endDate, ref List<HUMAN_RESOURCE_MACROS.EMPLOYEE_MONTH_DEDUCTION_STRUCT> returnVector)
        {
            returnVector.Clear();

            List<HUMAN_RESOURCE_MACROS.EMPLOYEE_ATTENDANCE_SUMMARY_STRUCT> attendanceList = new List<HUMAN_RESOURCE_MACROS.EMPLOYEE_ATTENDANCE_SUMMARY_STRUCT>();
            List<HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT> salariesList = new List<HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT>();
            List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_EXCLUDED_ATTENDANCE_STRUCT> excludedEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_EXCLUDED_ATTENDANCE_STRUCT>();

            if (!GetEmployeesAttendanceSummary(startDate, endDate, ref attendanceList))
                return false;
            if (!GetEmployeesBasicSalaries(ref salariesList))
                return false;
            if (!GetEmployeesExcludedAttendance(ref excludedEmployeesList))
                return false;

            for (int i = 0; i < attendanceList.Count; i++)
            {
                HUMAN_RESOURCE_MACROS.EMPLOYEE_MONTH_DEDUCTION_STRUCT currentEmployeeDeductionRecord = new HUMAN_RESOURCE_MACROS.EMPLOYEE_MONTH_DEDUCTION_STRUCT();
                currentEmployeeDeductionRecord.attendanceCount = new int[HUMAN_RESOURCE_MACROS.ATTENDANCE_ABSCENT - HUMAN_RESOURCE_MACROS.ATTENDANCE_LATENCY_FIRST + 1];

                currentEmployeeDeductionRecord.employee_id = attendanceList[i].employee_id;
                currentEmployeeDeductionRecord.employee_name = attendanceList[i].employee_name;

                TimeSpan nullTimeSpan = new TimeSpan(0, 0, 0);

                currentEmployeeDeductionRecord.deduct_full_day = 0;
                currentEmployeeDeductionRecord.deduct_work_hour = 0;

                currentEmployeeDeductionRecord.basic_salary = salariesList.Find(salary_item => salary_item.employee_id == currentEmployeeDeductionRecord.employee_id).net_salary;

                for (int j = HUMAN_RESOURCE_MACROS.ATTENDANCE_LATENCY_FIRST; j <= HUMAN_RESOURCE_MACROS.ATTENDANCE_ABSCENT; j++)
                {
                    if (j == HUMAN_RESOURCE_MACROS.ATTENDANCE_LATENCY_THIRD)
                    {
                        currentEmployeeDeductionRecord.deduct_work_hour = attendanceList[i].attendanceTypeCount[j];
                    }
                    else if (j == HUMAN_RESOURCE_MACROS.ATTENDANCE_LATENCY_SECOND)
                    {
                        currentEmployeeDeductionRecord.deduct_work_hour = attendanceList[i].attendanceTypeCount[j] * 2;
                    }
                    else if (j == HUMAN_RESOURCE_MACROS.ATTENDANCE_LATENCY_FIRST || j == HUMAN_RESOURCE_MACROS.ATTENDANCE_EARLY_LEAVE || j == HUMAN_RESOURCE_MACROS.ATTENDANCE_ABSCENT)
                    {
                        currentEmployeeDeductionRecord.deduct_full_day += attendanceList[i].attendanceTypeCount[j];
                    }

                    currentEmployeeDeductionRecord.attendanceCount[j - HUMAN_RESOURCE_MACROS.ATTENDANCE_LATENCY_FIRST] = attendanceList[i].attendanceTypeCount[j];
                }

                Decimal currentEmployeeDailyRate = currentEmployeeDeductionRecord.basic_salary / commonFunctionsObject.GetMonthLength(startDate.Month, startDate.Year);
                Decimal currentEmployeeHourlyRate = currentEmployeeDailyRate / HUMAN_RESOURCE_MACROS.WORKING_HOURS;

                if (excludedEmployeesList[i].is_excluded)
                    currentEmployeeDeductionRecord.attendance_deductions = 0;
                else
                    currentEmployeeDeductionRecord.attendance_deductions = (currentEmployeeDailyRate * currentEmployeeDeductionRecord.deduct_full_day) + (currentEmployeeHourlyRate * currentEmployeeDeductionRecord.deduct_work_hour);

                returnVector.Add(currentEmployeeDeductionRecord);
            }

            return true;
        }
        public bool GetEmployeesMonthDeductionsReport(DateTime startDate, DateTime endDate, ref List<HUMAN_RESOURCE_MACROS.EMPLOYEE_MONTH_DEDUCTION_STRUCT> returnVector)
        {
            returnVector.Clear();

            List<HUMAN_RESOURCE_MACROS.EMPLOYEE_ATTENDANCE_SUMMARY_STRUCT> attendanceList = new List<HUMAN_RESOURCE_MACROS.EMPLOYEE_ATTENDANCE_SUMMARY_STRUCT>();
            List<HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT> salariesList = new List<HUMAN_RESOURCE_MACROS.BASIC_SALARY_STRUCT>();
            List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_EXCLUDED_ATTENDANCE_STRUCT> excludedEmployeesList = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_EXCLUDED_ATTENDANCE_STRUCT>();

            if (!GetEmployeesAttendanceSummary(startDate, endDate, ref attendanceList))
                return false;
            if (!GetEmployeesBasicSalaries(ref salariesList))
                return false;
            if (!GetEmployeesExcludedAttendance(ref excludedEmployeesList))
                return false;

            for (int i = 0; i < attendanceList.Count; i++)
            {
                HUMAN_RESOURCE_MACROS.EMPLOYEE_MONTH_DEDUCTION_STRUCT currentEmployeeDeductionRecord = new HUMAN_RESOURCE_MACROS.EMPLOYEE_MONTH_DEDUCTION_STRUCT();

                currentEmployeeDeductionRecord.employee_id = attendanceList[i].employee_id;
                currentEmployeeDeductionRecord.employee_name = attendanceList[i].employee_name;

                TimeSpan nullTimeSpan = new TimeSpan(0, 0, 0);

                currentEmployeeDeductionRecord.deduct_full_day = 0;
                currentEmployeeDeductionRecord.deduct_work_hour = 0;

                currentEmployeeDeductionRecord.basic_salary = salariesList.Find(salary_item => salary_item.employee_id == currentEmployeeDeductionRecord.employee_id).net_salary;

                for (int j = HUMAN_RESOURCE_MACROS.ATTENDANCE_LATENCY_FIRST; j <= HUMAN_RESOURCE_MACROS.ATTENDANCE_ABSCENT; j++)
                {
                    if (j == HUMAN_RESOURCE_MACROS.ATTENDANCE_LATENCY_THIRD)
                    {
                        currentEmployeeDeductionRecord.deduct_work_hour += attendanceList[i].attendanceTypeCount[j];
                    }
                    else if (j == HUMAN_RESOURCE_MACROS.ATTENDANCE_LATENCY_SECOND)
                    {
                        currentEmployeeDeductionRecord.deduct_work_hour += attendanceList[i].attendanceTypeCount[j] * 2;
                    }
                    else if (j == HUMAN_RESOURCE_MACROS.ATTENDANCE_LATENCY_FIRST || j == HUMAN_RESOURCE_MACROS.ATTENDANCE_EARLY_LEAVE || j == HUMAN_RESOURCE_MACROS.ATTENDANCE_ABSCENT)
                    {
                        currentEmployeeDeductionRecord.deduct_full_day += attendanceList[i].attendanceTypeCount[j];
                    }
                }

                Decimal currentEmployeeDailyRate = currentEmployeeDeductionRecord.basic_salary / commonFunctionsObject.GetMonthLength(startDate.Month, startDate.Year);
                Decimal currentEmployeeHourlyRate = currentEmployeeDailyRate / HUMAN_RESOURCE_MACROS.WORKING_HOURS;

                if (excludedEmployeesList[i].is_excluded)
                    currentEmployeeDeductionRecord.attendance_deductions = 0;
                else
                    currentEmployeeDeductionRecord.attendance_deductions = (currentEmployeeDailyRate * currentEmployeeDeductionRecord.deduct_full_day) + (currentEmployeeHourlyRate * currentEmployeeDeductionRecord.deduct_work_hour);

                returnVector.Add(currentEmployeeDeductionRecord);
            }

            return true;
        }

        public bool GetEmployeesExcludedAttendance(ref List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_EXCLUDED_ATTENDANCE_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select employees_excluded_attendance.employee_id,
                                    		employees_info.name,
                                            employees_excluded_attendance.is_excluded
                                    from erp_system.dbo.employees_excluded_attendance
                                    inner join erp_system.dbo.employees_info
                                    on employees_excluded_attendance.employee_id = employees_info.employee_id
                                    where employees_info.currently_enrolled = 1
                                    order by employees_info.name;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_bit = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.EMPLOYEE_EXCLUDED_ATTENDANCE_STRUCT tempItem = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_EXCLUDED_ATTENDANCE_STRUCT();

                tempItem.employee_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.employee_name = sqlDatabase.rows[i].sql_string[0];
                tempItem.is_excluded = sqlDatabase.rows[i].sql_bit[0];

                returnVector.Add(tempItem);
            }

            return true;
        }
        public bool GetEmployeeEmailCount(String employeeEmail, ref int returnValue)
        {
            returnValue = 0;

            String sqlQueryPart1 = "select count(employees_business_emails.email) from erp_system.dbo.employees_business_emails where employees_business_emails.email = '";
            String sqlQueryPart2 = "';";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += employeeEmail;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            returnValue = sqlDatabase.rows[0].sql_int[0];

            return true;
        }
        public bool GetEmployeePasswordCount(String employeeEmail, ref int returnValue)
        {
            returnValue = 0;

            String sqlQueryPart1 = "select count(employees_passwords.id) from erp_system.dbo.employees_passwords inner join erp_system.dbo.employees_business_emails on employees_passwords.id = employees_business_emails.id where employees_business_emails.email = '";
            String sqlQueryPart2 = "';";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += employeeEmail;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            returnValue = sqlDatabase.rows[0].sql_int[0];

            return true;
        }

        public bool GetTeamLead(int teamId, ref int returnValue)
        {
            returnValue = 0;

            String sqlQueryPart1 = @"select employees_info.employee_id 
                                    from erp_system.dbo.employees_info 
							        where employees_info.employee_team = ";
            String sqlQueryPart2 = "and employees_info.employee_position = ";
            String sqlQueryPart3 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += teamId;
            sqlQuery += sqlQueryPart2;
            sqlQuery += COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION;
            sqlQuery += sqlQueryPart3;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            returnValue = sqlDatabase.rows[0].sql_int[0];

            return true;
        }
        public bool GetEmployeeTeam(int employeeId, ref int returnValue)
        {
            returnValue = 0;

            String sqlQueryPart1 = @"select employees_info.employee_team 
                                    from erp_system.dbo.employees_info 
        							where employees_info.employee_id = ";


            String sqlQueryPart2 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += employeeId;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            returnValue = sqlDatabase.rows[0].sql_int[0];

            return true;
        }

        public bool GetEmployeeInitials(int employeeId, ref String returnValue)
        {
            returnValue = String.Empty;

            String sqlQueryPart1 = "select employees_initials.employee_initial from erp_system.dbo.employees_initials where employees_initials.id = ";
            String sqlQueryPart2 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += employeeId;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            returnValue = sqlDatabase.rows[0].sql_string[0];

            return true;
        }
        public bool GetEmployeePassword(int employeeId, ref String returnValue)
        {
            returnValue = String.Empty;

            String sqlQueryPart1 = "select password from erp_system.dbo.employees_passwords where employees_passwords.id = ";
            String sqlQueryPart2 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += employeeId;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            returnValue = sqlDatabase.rows[0].sql_string[0];

            return true;
        }

        //////////////////////////////////////////////////////////////////////
        //GET CONTACT INFO FUNCTIONS
        //////////////////////////////////////////////////////////////////////

        public bool GetCompanyContacts(int salesPersonId, int addressSerial, ref List<COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select contact_person_info.contact_id, contact_person_info.name from erp_system.dbo.contact_person_info where contact_person_info.sales_person_id = ";
            String sqlQueryPart2 = " and contact_person_info.branch_serial = ";
            String sqlQueryPart3 = @" and contact_person_info.is_valid = 1 
                                    order by contact_person_info.name; ";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += salesPersonId;
            sqlQuery += sqlQueryPart2;
            sqlQuery += addressSerial;
            sqlQuery += sqlQueryPart3;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.CONTACT_BASIC_STRUCT tempItem;
                tempItem.contact_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.contact_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }
        public bool GetCompanyContacts(int addressSerial, ref List<COMPANY_ORGANISATION_MACROS.CONTACT_PRO_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select	contact_person_info.contact_id, 
                
                                    departments_type.id as contact_department_id, 

									employees_info.employee_id as sales_person_id, 
									teams_type.id as sales_person_team_id, 

									contact_person_info.name as contact_person_name, 
									departments_type.department as contact_department_name, 

									employees_info.name as sales_person_name, 
									teams_type.team as sales_person_team_name, 

									contact_person_info.gender, 
									contact_person_mobile.mobile, 
									contact_person_info.email, 
									contact_person_personal_emails.personal_email 

							from erp_system.dbo.contact_person_info 

							inner join erp_system.dbo.employees_info 
							on contact_person_info.sales_person_id = employees_info.employee_id 

							inner join erp_system.dbo.departments_type 
							on contact_person_info.department = departments_type.id 

							inner join erp_system.dbo.teams_type 
							on employees_info.employee_team = teams_type.id 

							left join erp_system.dbo.contact_person_mobile 
							on contact_person_info.sales_person_id = contact_person_mobile.sales_person_id 
							and contact_person_info.branch_serial = contact_person_mobile.branch_serial 
							and contact_person_info.contact_id = contact_person_mobile.contact_id 

							left join erp_system.dbo.contact_person_personal_emails 
							on contact_person_info.sales_person_id = contact_person_personal_emails.sales_person_id 
							and contact_person_info.branch_serial = contact_person_personal_emails.branch_serial 
							and contact_person_info.contact_id = contact_person_personal_emails.contact_id 

							inner join erp_system.dbo.company_address 
							on contact_person_info.branch_serial = company_address.address_serial 

							where company_address.address_serial = ";
            String sqlQueryPart2 = @" and contact_person_info.is_valid = 1 
                                        order by contact_person_info.name; ";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += addressSerial.ToString();
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 4;
            queryColumns.sql_string = 8;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.CONTACT_PRO_STRUCT tempItem;

                tempItem.sales_person.team = new COMPANY_ORGANISATION_MACROS.TEAM_STRUCT();
                tempItem.sales_person = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT();
                tempItem.company = new COMPANY_ORGANISATION_MACROS.COMPANY_STRUCT();
                tempItem.branch = new COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT();
                tempItem.contact_department = new COMPANY_ORGANISATION_MACROS.DEPARTMENT_STRUCT();

                tempItem.contact_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.contact_name = sqlDatabase.rows[i].sql_string[0];

                tempItem.contact_department.department_id = sqlDatabase.rows[i].sql_int[1];
                tempItem.contact_department.department_name = sqlDatabase.rows[i].sql_string[1];

                tempItem.sales_person.employee_id = sqlDatabase.rows[i].sql_int[2];
                tempItem.sales_person.employee_name = sqlDatabase.rows[i].sql_string[2];

                tempItem.sales_person.team.team_id = sqlDatabase.rows[i].sql_int[3];
                tempItem.sales_person.team.team_name = sqlDatabase.rows[i].sql_string[3];

                tempItem.gender = sqlDatabase.rows[i].sql_string[4];
                tempItem.mobile = sqlDatabase.rows[i].sql_string[5];

                tempItem.business_email = sqlDatabase.rows[i].sql_string[6];
                tempItem.personal_email = sqlDatabase.rows[i].sql_string[7];

                returnVector.Add(tempItem);
            }

            return true;
        }

        public bool GetEmployeeCompanies(int salesPersonId, ref List<COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select distinct company_name.company_serial, 
									company_name.company_name 
							from erp_system.dbo.company_name 
							inner join erp_system.dbo.company_field_of_work 
							on company_name.company_serial = company_field_of_work.company_serial 
							inner join erp_system.dbo.company_address 
							on company_name.company_serial = company_address.company_serial 
							left join erp_system.dbo.contact_person_info 
							on company_address.address_serial = contact_person_info.branch_serial 
							where contact_person_info.sales_person_id = ";

            String sqlQueryPart2 = @" or company_name.added_by = ";
            String sqlQueryPart3 = @" order by company_name.company_name";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += salesPersonId.ToString();
            sqlQuery += sqlQueryPart2;
            sqlQuery += salesPersonId.ToString();
            sqlQuery += sqlQueryPart3;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT temp = new COMPANY_ORGANISATION_MACROS.COMPANY_MIN_LIST_STRUCT();

                temp.company_serial = sqlDatabase.rows[i].sql_int[0];
                temp.company_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(temp);
            }
            return true;
        }
        public bool GetEmployeeCompanies(int salesPersonId, ref List<COMPANY_ORGANISATION_MACROS.COMPANY_MAX_LIST_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select distinct company_name.company_serial, 
									company_address.address_serial, 
									company_address.address, 
									company_name.company_name 
							from erp_system.dbo.company_name 
							inner join erp_system.dbo.company_field_of_work 
							on company_name.company_serial = company_field_of_work.company_serial 
							inner join erp_system.dbo.company_address 
							on company_name.company_serial = company_address.company_serial 
							left join erp_system.dbo.contact_person_info 
							on company_address.address_serial = contact_person_info.branch_serial 
							where contact_person_info.sales_person_id = ";

            String sqlQueryPart2 = @" or company_name.added_by = ";
            String sqlQueryPart3 = @" order by company_name.company_name";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += salesPersonId.ToString();
            sqlQuery += sqlQueryPart2;
            sqlQuery += salesPersonId.ToString();
            sqlQuery += sqlQueryPart3;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 2;
            queryColumns.sql_string = 1;

            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.COMPANY_MAX_LIST_STRUCT temp = new COMPANY_ORGANISATION_MACROS.COMPANY_MAX_LIST_STRUCT();

                temp.company_serial = sqlDatabase.rows[i].sql_int[0];
                temp.address_serial = sqlDatabase.rows[i].sql_int[1];
                temp.address = sqlDatabase.rows[i].sql_int[2];
                temp.company_name = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(temp);
            }
            return true;
        }
        public bool GetEmployeeCompanies(int salesPersonId, ref List<COMPANY_ORGANISATION_MACROS.COMPANY_LIST_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select distinct company_name.company_serial, 
                                    company_address.address, 
									company_field_of_work.work_field, 
									company_address.address_serial, 
									company_name.company_name
                            from erp_system.dbo.company_name
                            inner join erp_system.dbo.company_field_of_work
                            on company_name.company_serial = company_field_of_work.company_serial

                            inner join erp_system.dbo.company_address
                            on company_name.company_serial = company_address.company_serial

                            left join erp_system.dbo.contact_person_info
                            on company_address.address_serial = contact_person_info.branch_serial

                            where contact_person_info.sales_person_id = ";

            String sqlQueryPart2 = " or company_name.added_by = ";

            String sqlQueryPart3 = " order by company_name;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += salesPersonId;
            sqlQuery += sqlQueryPart2;
            sqlQuery += salesPersonId;
            sqlQuery += sqlQueryPart3;


            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 4;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns, BASIC_MACROS.SEVERITY_LOW))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                if (!returnVector.Exists(companyItem => companyItem.company_serial == sqlDatabase.rows[i].sql_int[0]))
                {
                    COMPANY_ORGANISATION_MACROS.COMPANY_LIST_STRUCT tempCompanyItem = new COMPANY_ORGANISATION_MACROS.COMPANY_LIST_STRUCT();
                    tempCompanyItem.company_serial = sqlDatabase.rows[i].sql_int[0];

                    COMPANY_ORGANISATION_MACROS.BRANCH_MIN_STRUCT branchObject = new COMPANY_ORGANISATION_MACROS.BRANCH_MIN_STRUCT();
                    branchObject.address_serial = sqlDatabase.rows[i].sql_int[3];
                    branchObject.address = sqlDatabase.rows[i].sql_int[1];

                    tempCompanyItem.work_field = sqlDatabase.rows[i].sql_int[2];
                    tempCompanyItem.company_name = sqlDatabase.rows[i].sql_string[0];

                    tempCompanyItem.branchesList = new List<COMPANY_ORGANISATION_MACROS.BRANCH_MIN_STRUCT>();
                    tempCompanyItem.branchesList.Add(branchObject);

                    returnVector.Add(tempCompanyItem);
                }
                else
                {
                    COMPANY_ORGANISATION_MACROS.BRANCH_MIN_STRUCT branchObject = new COMPANY_ORGANISATION_MACROS.BRANCH_MIN_STRUCT();
                    branchObject.address = sqlDatabase.rows[i].sql_int[1];
                    branchObject.address_serial = sqlDatabase.rows[i].sql_int[3];

                    returnVector.Find(companyItem => companyItem.company_serial == sqlDatabase.rows[i].sql_int[0]).branchesList.Add(branchObject);
                }

            }

            return true;
        }
        public bool GetEmployeeContacts(int salesPersonId, ref List<COMPANY_ORGANISATION_MACROS.CONTACT_PRO_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select	contact_person_info.contact_id, 
                                    departments_type.id as contact_department_id, 

									employees_info.employee_id as sales_person_id, 
									teams_type.id as sales_person_team_id, 

									company_address.address_serial, 
									company_name.company_serial, 

									contact_person_info.name as contact_person_name, 
									departments_type.department as contact_department_name, 

									employees_info.name as sales_person_name, 
									teams_type.team as sales_person_team_name, 

									company_name.company_name, 

									contact_person_info.gender, 
									contact_person_mobile.mobile, 
									contact_person_info.email, 
									contact_person_personal_emails.personal_email 

							from erp_system.dbo.contact_person_info 

							inner join erp_system.dbo.employees_info 
							on contact_person_info.sales_person_id = employees_info.employee_id 

							inner join erp_system.dbo.departments_type 
							on contact_person_info.department = departments_type.id 

							inner join erp_system.dbo.teams_type 
							on employees_info.employee_team = teams_type.id 

							left join erp_system.dbo.contact_person_mobile 
							on contact_person_info.sales_person_id = contact_person_mobile.sales_person_id 
							and contact_person_info.branch_serial = contact_person_mobile.branch_serial 
							and contact_person_info.contact_id = contact_person_mobile.contact_id 

							left join erp_system.dbo.contact_person_personal_emails 
							on contact_person_info.sales_person_id = contact_person_personal_emails.sales_person_id 
							and contact_person_info.branch_serial = contact_person_personal_emails.branch_serial 
							and contact_person_info.contact_id = contact_person_personal_emails.contact_id 

							inner join erp_system.dbo.company_address 
							on contact_person_info.branch_serial = company_address.address_serial 

							where contact_person_info.sales_person_id = ";


            String sqlQueryPart2 = @" and contact_person_info.is_valid = 1 
                                    order by contact_person_info.name; ";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += salesPersonId;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 6;
            queryColumns.sql_string = 9;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.CONTACT_PRO_STRUCT tempItem;

                tempItem.sales_person.team = new COMPANY_ORGANISATION_MACROS.TEAM_STRUCT();
                tempItem.sales_person = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT();
                tempItem.company = new COMPANY_ORGANISATION_MACROS.COMPANY_STRUCT();
                tempItem.branch = new COMPANY_ORGANISATION_MACROS.BRANCH_STRUCT();
                tempItem.contact_department = new COMPANY_ORGANISATION_MACROS.DEPARTMENT_STRUCT();

                tempItem.contact_id = sqlDatabase.rows[i].sql_int[0];
                tempItem.contact_name = sqlDatabase.rows[i].sql_string[0];

                tempItem.contact_department.department_id = sqlDatabase.rows[i].sql_int[1];
                tempItem.contact_department.department_name = sqlDatabase.rows[i].sql_string[1];

                tempItem.sales_person.employee_id = sqlDatabase.rows[i].sql_int[2];
                tempItem.sales_person.employee_name = sqlDatabase.rows[i].sql_string[2];

                tempItem.sales_person.team.team_id = sqlDatabase.rows[i].sql_int[3];
                tempItem.sales_person.team.team_name = sqlDatabase.rows[i].sql_string[3];

                tempItem.branch.address_serial = sqlDatabase.rows[i].sql_int[4];
                tempItem.company.company_serial = sqlDatabase.rows[i].sql_int[5];
                tempItem.company.company_name = sqlDatabase.rows[i].sql_string[4];

                tempItem.gender = sqlDatabase.rows[i].sql_string[5];
                tempItem.mobile = sqlDatabase.rows[i].sql_string[6];

                tempItem.business_email = sqlDatabase.rows[i].sql_string[7];
                tempItem.personal_email = sqlDatabase.rows[i].sql_string[8];

                returnVector.Add(tempItem);
            }

            return true;
        }
        public bool GetEmployeeContacts(int salesPersonId, ref List<COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT> returnVector)
        {
            returnVector.Clear();

            String QueryPart1 = @"select contact_person_info.contact_id, 
                                    company_address.company_serial, 
									company_address.address_serial, 
									company_address.address, 
									contact_person_info.name, 
									departments_type.department
                            from erp_system.dbo.contact_person_info
                            inner join erp_system.dbo.company_address
                            on contact_person_info.branch_serial = company_address.address_serial

                            inner join erp_system.dbo.departments_type
                            on contact_person_info.department = departments_type.id

                            where contact_person_info.sales_person_id = ";

            String QueryPart2 = @" and contact_person_info.is_valid = 1 
                                  order by company_address.address; ";

            sqlQuery = String.Empty;
            sqlQuery += QueryPart1;
            sqlQuery += salesPersonId;
            sqlQuery += QueryPart2;
            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns2 = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns2.sql_int = 4;
            queryColumns2.sql_string = 2;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns2, BASIC_MACROS.SEVERITY_HIGH))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT tempCompanyItem = new COMPANY_ORGANISATION_MACROS.CONTACT_LIST_STRUCT();
                tempCompanyItem.contact_id = sqlDatabase.rows[i].sql_int[0];
                tempCompanyItem.company_serial = sqlDatabase.rows[i].sql_int[1];
                tempCompanyItem.address_serial = sqlDatabase.rows[i].sql_int[2];
                tempCompanyItem.address = sqlDatabase.rows[i].sql_int[3];
                tempCompanyItem.contact_name = sqlDatabase.rows[i].sql_string[0];
                tempCompanyItem.department = sqlDatabase.rows[i].sql_string[1];

                returnVector.Add(tempCompanyItem);
            }

            return true;
        }

        public bool GetEmployeeContacts(int salesPersonId, ref List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT> returnVector)
        {
            returnVector.Clear();

            String QueryPart1 = @"select contact_person_info.contact_id, 
                                    company_address.company_serial, 
                                    company_address.address_serial,
									contact_person_info.name 
                            from erp_system.dbo.contact_person_info
                            inner join erp_system.dbo.company_address
                            on contact_person_info.branch_serial = company_address.address_serial

                            inner join erp_system.dbo.departments_type
                            on contact_person_info.department = departments_type.id

                            where contact_person_info.sales_person_id = ";

            String QueryPart2 = @" and contact_person_info.is_valid = 1 
                                  order by company_address.address; ";

            sqlQuery = String.Empty;
            sqlQuery += QueryPart1;
            sqlQuery += salesPersonId;
            sqlQuery += QueryPart2;
            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns2 = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns2.sql_int = 3;
            queryColumns2.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns2, BASIC_MACROS.SEVERITY_LOW))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT tempCompanyItem = new COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT();
                tempCompanyItem.contact.contact_id = sqlDatabase.rows[i].sql_int[0];
                tempCompanyItem.company_serial = sqlDatabase.rows[i].sql_int[1];
                tempCompanyItem.address_serial = sqlDatabase.rows[i].sql_int[2];
                tempCompanyItem.contact.contact_name = sqlDatabase.rows[i].sql_string[0];
                tempCompanyItem.sales_person_id = salesPersonId;

                returnVector.Add(tempCompanyItem);
            }

            return true;
        }
        public bool GetContactBusinessEmailCount(String contactEmail, ref int returnValue)
        {
            returnValue = 0;

            String sqlQueryPart1 = "select count(contact_person_info.email) from erp_system.dbo.contact_person_info where contact_person_info.email = '";
            String sqlQueryPart2 = "';";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += contactEmail;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            returnValue = sqlDatabase.rows[0].sql_int[0];

            return true;
        }
        public bool GetContactBusinessEmailCount(String contactEmail, int salesPersonId, int addressSerial, int contactId, ref int returnValue)
        {
            returnValue = 0;

            String sqlQueryPart1 = "select count(contact_person_info.email) from erp_system.dbo.contact_person_info where contact_person_info.email = '";
            String sqlQueryPart2 = "' and sales_person_id != ";
            String sqlQueryPart3 = " and branch_serial != ";
            String sqlQueryPart4 = " and contact_id != ";
            String sqlQueryPart5 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += contactEmail;
            sqlQuery += sqlQueryPart2;
            sqlQuery += salesPersonId;
            sqlQuery += sqlQueryPart3;
            sqlQuery += addressSerial;
            sqlQuery += sqlQueryPart4;
            sqlQuery += contactId;
            sqlQuery += sqlQueryPart5;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            returnValue = sqlDatabase.rows[0].sql_int[0];

            return true;
        }

        public bool GetContactPhoneCount(String contactPhone, ref int returnValue)
        {
            String sqlQueryPart1 = "select count(contact_person_mobile.mobile) from erp_system.dbo.contact_person_mobile where contact_person_mobile.mobile = '";
            String sqlQueryPart2 = "';";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += contactPhone;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            returnValue = sqlDatabase.rows[0].sql_int[0];

            return true;
        }
        public bool GetContactPhoneCount(String contactPhone, int salesPersonId, int addressSerial, int contactId, ref int returnValue)
        {
            returnValue = 0;

            String sqlQueryPart1 = "select count(contact_person_mobile.mobile) from erp_system.dbo.contact_person_mobile where contact_person_mobile.mobile = '";
            String sqlQueryPart2 = "' and sales_person_id != ";
            String sqlQueryPart3 = " and branch_serial != ";
            String sqlQueryPart4 = " and contact_id != ";
            String sqlQueryPart5 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += contactPhone;
            sqlQuery += sqlQueryPart2;
            sqlQuery += salesPersonId;
            sqlQuery += sqlQueryPart3;
            sqlQuery += addressSerial;
            sqlQuery += sqlQueryPart4;
            sqlQuery += contactId;
            sqlQuery += sqlQueryPart5;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            returnValue = sqlDatabase.rows[0].sql_int[0];

            return true;
        }


        public bool GetContactPersonalEmailCount(String contactEmail, ref int returnValue)
        {
            returnValue = 0;

            String sqlQueryPart1 = "select count(contact_person_personal_emails.personal_email) from erp_system.dbo.contact_person_personal_emails where contact_person_personal_emails.personal_email = '";
            String sqlQueryPart2 = "';";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += contactEmail;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            returnValue = sqlDatabase.rows[0].sql_int[0];

            return true;
        }
        public bool GetContactPersonalEmailCount(String contactEmail, int salesPersonId, int addressSerial, int contactId, ref int returnValue)
        {
            returnValue = 0;

            String sqlQueryPart1 = "select count(contact_person_personal_emails.personal_email) from erp_system.dbo.contact_person_personal_emails where contact_person_personal_emails.personal_email = '";
            String sqlQueryPart2 = "' and sales_person_id != ";
            String sqlQueryPart3 = " and branch_serial != ";
            String sqlQueryPart4 = " and contact_id != ";
            String sqlQueryPart5 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += contactEmail;
            sqlQuery += sqlQueryPart2;
            sqlQuery += salesPersonId;
            sqlQuery += sqlQueryPart3;
            sqlQuery += addressSerial;
            sqlQuery += sqlQueryPart4;
            sqlQuery += contactId;
            sqlQuery += sqlQueryPart5;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            returnValue = sqlDatabase.rows[0].sql_int[0];

            return true;
        }

        public bool GetCompanyDomainCount(String companyDomain, ref int returnValue)
        {
            returnValue = 0;

            String sqlQueryPart1 = @"select count(company_name.domain_name) 
                                    from erp_system.dbo.company_name where company_name.domain_name like '";
            String sqlQueryPart2 = "';";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += companyDomain;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            returnValue = sqlDatabase.rows[0].sql_int[0];

            return true;
        }

        //////////////////////////////////////////////////////////////////////
        //GET DOMAIN INFO FUNCTIONS
        //////////////////////////////////////////////////////////////////////

        public bool GetOriginalTLDs(ref List<String> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select * from erp_system.dbo.original_tlds;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
                returnVector.Add(sqlDatabase.rows[i].sql_string[0]);

            return true;
        }

        public bool GetCountryTLD(int countryId, ref String returnValue)
        {
            returnValue = String.Empty;

            String sqlQueryPart1 = "select tld from erp_system.dbo.country_tlds where country = '";
            String sqlQueryPart2 = "';";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += countryId;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            returnValue = sqlDatabase.rows[0].sql_string[0];

            return true;
        }

        //////////////////////////////////////////////////////////////////////
        //GET PRODUCTS INFO FUNCTIONS
        //////////////////////////////////////////////////////////////////////

        public bool GetCompanyProducts(ref List<COMPANY_WORK_MACROS.PRODUCT_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select products_type.id, products_type.product_name from erp_system.dbo.products_type where products_type.id > 0 order by product_name;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_WORK_MACROS.PRODUCT_STRUCT tempItem;

                tempItem.typeId = sqlDatabase.rows[i].sql_int[0];
                tempItem.typeName = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            COMPANY_WORK_MACROS.PRODUCT_STRUCT lastItem;

            lastItem.typeId = 0;
            lastItem.typeName = "Others";

            returnVector.Add(lastItem);

            return true;
        }
        public bool GetCompanyBrands(ref List<COMPANY_WORK_MACROS.BRAND_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = "select brands_type.id, brands_type.brand_name from erp_system.dbo.brands_type where brands_type.id > 0 order by brands_type.id;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_WORK_MACROS.BRAND_STRUCT tempItem;

                tempItem.brandId = sqlDatabase.rows[i].sql_int[0];
                tempItem.brandName = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            COMPANY_WORK_MACROS.BRAND_STRUCT lastItem;

            lastItem.brandId = 0;
            lastItem.brandName = "Others";

            returnVector.Add(lastItem);

            return true;
        }
        public bool GetCompanyModels(COMPANY_WORK_MACROS.PRODUCT_STRUCT product, COMPANY_WORK_MACROS.BRAND_STRUCT brand, ref List<COMPANY_WORK_MACROS.MODEL_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select brands_models.model_id, brands_models.brand_model 
                                    from erp_system.dbo.brands_models 
							        where brands_models.product_id = ";


            String sqlQueryPart2 = " and brands_models.brand_id = ";
            String sqlQueryPart3 = " and brands_models.model_id > 0 order by brands_models.model_id; ";


            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += product.typeId;
            sqlQuery += sqlQueryPart2;
            sqlQuery += brand.brandId;
            sqlQuery += sqlQueryPart3;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 1;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_WORK_MACROS.MODEL_STRUCT tempItem;

                tempItem.modelId = sqlDatabase.rows[i].sql_int[0];
                tempItem.modelName = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            COMPANY_WORK_MACROS.MODEL_STRUCT lastItem;

            lastItem.modelId = 0;
            lastItem.modelName = "Others";

            returnVector.Add(lastItem);

            return true;
        }

        public bool GetModelApplications(int mProductId, int mBrandId, int mModelId, ref List<String> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select model_applications.application 
                                    from erp_system.dbo.model_applications 
							        where model_applications.product_id = ";
            String sqlQueryPart2 = " and model_applications.brand_id = ";
            String sqlQueryPart3 = " and model_applications.model_id = ";
            String sqlQueryPart4 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += mProductId;
            sqlQuery += sqlQueryPart2;
            sqlQuery += mBrandId;
            sqlQuery += sqlQueryPart3;
            sqlQuery += mModelId;
            sqlQuery += sqlQueryPart4;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
                returnVector.Add(sqlDatabase.rows[i].sql_string[0]);

            return true;
        }
        public bool GetModelBenefits(int mProductId, int mBrandId, int mModelId, ref List<String> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select model_benefits.benefit 
                                    from erp_system.dbo.model_benefits 
							        where model_benefits.product_id = ";


            String sqlQueryPart2 = " and model_benefits.brand_id = ";
            String sqlQueryPart3 = " and model_benefits.model_id = ";
            String sqlQueryPart4 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += mProductId;
            sqlQuery += sqlQueryPart2;
            sqlQuery += mBrandId;
            sqlQuery += sqlQueryPart3;
            sqlQuery += mModelId;
            sqlQuery += sqlQueryPart4;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
                returnVector.Add(sqlDatabase.rows[i].sql_string[0]);

            return true;
        }
        public bool GetModelFeatures(int mProductId, int mBrandId, int mModelId, ref List<String> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select model_standard_features.feature 
                                    from erp_system.dbo.model_standard_features 
        							where model_standard_features.product_id = ";


            String sqlQueryPart2 = " and model_standard_features.brand_id = ";
            String sqlQueryPart3 = " and model_standard_features.model_id = ";
            String sqlQueryPart4 = ";";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += mProductId;
            sqlQuery += sqlQueryPart2;
            sqlQuery += mBrandId;
            sqlQuery += sqlQueryPart3;
            sqlQuery += mModelId;
            sqlQuery += sqlQueryPart4;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
                returnVector.Add(sqlDatabase.rows[i].sql_string[0]);

            return true;
        }

        //////////////////////////////////////////////////////////////////////
        //GET COMPANY WORK FUNCTIONS
        //////////////////////////////////////////////////////////////////////

        public bool GetRFQs(ref List<COMPANY_WORK_MACROS.RFQ_MAX_STRUCT> returnVector)
        {

            returnVector.Clear();

            String sqlQueryPart1 =
             @"with get_company_info	as	(	select	company_name.company_serial, 
                                                    company_address.address_serial, 
		    										company_name.company_name 
		    								from erp_system.dbo.company_address 
		    								inner join erp_system.dbo.company_name 
		    								on company_address.company_serial = company_name.company_serial 
		    								), 
		    get_sales_person as (select employees_info.employee_id as sales_person_id, 
		    									employees_info.name as sales_person_name 
		    							from erp_system.dbo.employees_info 
		    							), 
		    get_technical_person as (select employees_info.employee_id as technical_person_id, 
		    									employees_info.name as technical_person_name 
		    							from erp_system.dbo.employees_info 
		    							), 
		    get_product1_type as (select products_type.id as product1_type_id, 
		    											products_type.product_name as product1_type 
		    									from erp_system.dbo.products_type 
		    ), 

		    get_product1_brand as (select brands_type.id as product1_brand_id, 
		    	brands_type.brand_name as product1_brand 
		    	from erp_system.dbo.brands_type 
		    ), 
		    get_product1_model as (select brands_models.model_id as product1_model_id, 
		    	brands_models.product_id as product1_type_id, 
		    	brands_models.brand_id as product1_brand_id, 
		    	brands_models.brand_model as product1_model 
		    	from erp_system.dbo.brands_models 
		    ), 

		    get_product2_type as (select products_type.id as product2_type_id, 
		    	products_type.product_name as product2_type 
		    	from erp_system.dbo.products_type 
		    ), 
		    get_product2_brand as (select brands_type.id as product2_brand_id, 
		    	brands_type.brand_name as product2_brand 
		    	from erp_system.dbo.brands_type 
		    ), 
		    get_product2_model as (select brands_models.model_id as product2_model_id, 
		    	brands_models.product_id as product2_type_id, 
		    	brands_models.brand_id as product2_brand_id, 
		    	brands_models.brand_model as product2_model 
		    	from erp_system.dbo.brands_models 
		    ), 

		    get_product3_type as (select products_type.id as product3_type_id, 
		    	products_type.product_name as product3_type 
		    	from erp_system.dbo.products_type 
		    ), 
		    get_product3_brand as (select brands_type.id as product3_brand_id, 
		    	brands_type.brand_name as product3_brand 
		    	from erp_system.dbo.brands_type 
		    ), 
		    get_product3_model as (select brands_models.model_id as product3_model_id, 
		    	brands_models.product_id as product3_type_id, 
		    	brands_models.brand_id as product3_brand_id, 
		    	brands_models.brand_model as product3_model 
		    	from erp_system.dbo.brands_models 
		    ), 

		    get_product4_type as (select products_type.id as product4_type_id, 
		    	products_type.product_name as product4_type 
		    	from erp_system.dbo.products_type 
		    ), 
		    get_product4_brand as (select brands_type.id as product4_brand_id, 
		    	brands_type.brand_name as product4_brand 
		    	from erp_system.dbo.brands_type 
		    ), 
		    get_product4_model as (select brands_models.model_id as product4_model_id, 
		    	brands_models.product_id as product4_type_id, 
		    	brands_models.brand_id as product4_brand_id, 
		    	brands_models.brand_model as product4_model 
		    	from erp_system.dbo.brands_models 
		    ) 

		    select rfqs.sales_person, 
		    		rfqs.rfq_serial, 
		    		rfqs.rfq_version, 

		    		rfqs.assigned_engineer, 

		    		get_company_info.company_serial, 
		    		get_company_info.address_serial, 
		    		contact_person_info.contact_id, 

		    		rfqs.product1_type, 
		    		rfqs.product1_brand, 
		    		rfqs.product1_model, 

		    		rfqs.product2_type, 
		    		rfqs.product2_brand, 
		    		rfqs.product2_model, 

		    		rfqs.product3_type, 
		    		rfqs.product3_brand, 
		    		rfqs.product3_model, 

		    		rfqs.product4_type, 
		    		rfqs.product4_brand, 
		    		rfqs.product4_model, 

		    		contracts_type.id as contract_type_id, 
		    		rfqs_status.id as rfq_status_id, 
		    		rfq_failure_reasons.id as failure_reason_id, 

		    		rfqs.issue_date, 
		    		rfqs.deadline_date, 

		    		rfqs.rfq_id, 

		    		get_sales_person.sales_person_name, 
		    		get_technical_person.technical_person_name, 

		    		get_company_info.company_name, 
		    		contact_person_info.name, 

		    		get_product1_type.product1_type, 
		    		get_product1_brand.product1_brand, 
		    		get_product1_model.product1_model, 

		    		get_product2_type.product2_type, 
		    		get_product2_brand.product2_brand, 
		    		get_product2_model.product2_model, 

		    		get_product3_type.product3_type, 
		    		get_product3_brand.product3_brand, 
		    		get_product3_model.product3_model, 

		    		get_product4_type.product4_type, 
		    		get_product4_brand.product4_brand, 
		    		get_product4_model.product4_model, 

		    		contracts_type.contract_type, 
		    		rfqs_status.rfq_status, 
		    		rfq_failure_reasons.failure_reason 

		    from erp_system.dbo.rfqs 

		    inner join get_sales_person 
		    on rfqs.sales_person = get_sales_person.sales_person_id 

		    inner join get_technical_person 
		    on rfqs.assigned_engineer = get_technical_person.technical_person_id 

		    inner join erp_system.dbo.employees_initials 
		    on get_sales_person.sales_person_id = employees_initials.id 

		    inner join get_company_info 
		    on rfqs.branch_serial = get_company_info.address_serial 

		    inner join erp_system.dbo.contact_person_info 
		    on rfqs.contact_id = contact_person_info.contact_id 
		    and rfqs.branch_serial = contact_person_info.branch_serial 
		    and rfqs.sales_person = contact_person_info.sales_person_id 

		    left join get_product1_type 
		    on rfqs.product1_type = get_product1_type.product1_type_id 
		    left join get_product1_brand 
		    on rfqs.product1_brand = get_product1_brand.product1_brand_id 
		    left join get_product1_model 
		    on rfqs.product1_model = get_product1_model.product1_model_id 
		    and rfqs.product1_type = get_product1_model.product1_type_id 
		    and rfqs.product1_brand = get_product1_model.product1_brand_id 

		    left join get_product2_type 
		    on rfqs.product2_type = get_product2_type.product2_type_id 
		    left join get_product2_brand 
		    on rfqs.product2_brand = get_product2_brand.product2_brand_id 
		    left join get_product2_model 
		    on rfqs.product2_model = get_product2_model.product2_model_id 
		    and rfqs.product2_type = get_product2_model.product2_type_id 
		    and rfqs.product2_brand = get_product2_model.product2_brand_id 

		    left join get_product3_type 
		    on rfqs.product3_type = get_product3_type.product3_type_id 
		    left join get_product3_brand 
		    on rfqs.product3_brand = get_product3_brand.product3_brand_id 
		    left join get_product3_model 
		    on rfqs.product3_model = get_product3_model.product3_model_id 
		    and rfqs.product3_type = get_product3_model.product3_type_id 
		    and rfqs.product3_brand = get_product3_model.product3_brand_id 

		    left join get_product4_type 
		    on rfqs.product4_type = get_product4_type.product4_type_id 
		    left join get_product4_brand 
		    on rfqs.product4_brand = get_product4_brand.product4_brand_id 
		    left join get_product4_model 
		    on rfqs.product4_model = get_product4_model.product4_model_id 
		    and rfqs.product4_type = get_product4_model.product4_type_id 
		    and rfqs.product4_brand = get_product4_model.product4_brand_id 

		    inner join erp_system.dbo.contracts_type 
		    on rfqs.contract_type = contracts_type.id 

		    inner join erp_system.dbo.rfqs_status 
		    on rfqs.rfq_status = rfqs_status.id 

		    left join erp_system.dbo.unsuccessful_rfqs 
		    on rfqs.rfq_serial = unsuccessful_rfqs.rfq_serial 
		    and rfqs.sales_person = unsuccessful_rfqs.sales_person 

		    left join erp_system.dbo.rfq_failure_reasons 
		    on unsuccessful_rfqs.failure_reason = rfq_failure_reasons.id 

		    order by rfqs.issue_date desc, employees_initials.employee_initial, rfqs.rfq_serial desc, rfqs.rfq_version desc;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 22;
            queryColumns.sql_datetime = 2;
            queryColumns.sql_string = 20;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_WORK_MACROS.RFQ_MAX_STRUCT RFQItem = new COMPANY_WORK_MACROS.RFQ_MAX_STRUCT();

                RFQItem.products = new List<COMPANY_WORK_MACROS.RFQ_PRODUCT_STRUCT>();

                int numericCount = 0;
                int StringCount = 0;

                RFQItem.rfq_id = sqlDatabase.rows[i].sql_string[StringCount++];

                RFQItem.sales_person_name = sqlDatabase.rows[i].sql_string[StringCount++];
                RFQItem.assignee_name = sqlDatabase.rows[i].sql_string[StringCount++];

                RFQItem.company_name = sqlDatabase.rows[i].sql_string[StringCount++];
                RFQItem.contact_name = sqlDatabase.rows[i].sql_string[StringCount++];


                RFQItem.issue_date = sqlDatabase.rows[i].sql_datetime[0].ToString();
                RFQItem.deadline_date = sqlDatabase.rows[i].sql_datetime[1].ToString();


                RFQItem.sales_person_id = sqlDatabase.rows[i].sql_int[numericCount++];
                RFQItem.rfq_serial = sqlDatabase.rows[i].sql_int[numericCount++];
                RFQItem.rfq_version = sqlDatabase.rows[i].sql_int[numericCount++];

                RFQItem.assignee_id = sqlDatabase.rows[i].sql_int[numericCount++];

                RFQItem.company_serial = sqlDatabase.rows[i].sql_int[numericCount++];
                RFQItem.branch_serial = sqlDatabase.rows[i].sql_int[numericCount++];
                RFQItem.contact_id = sqlDatabase.rows[i].sql_int[numericCount++];

                for (int j = 0; j < COMPANY_WORK_MACROS.MAX_RFQ_PRODUCTS; j++)
                {
                    COMPANY_WORK_MACROS.RFQ_PRODUCT_STRUCT tempProductItem;

                    tempProductItem.productType = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
                    tempProductItem.productBrand = new COMPANY_WORK_MACROS.BRAND_STRUCT();
                    tempProductItem.productModel = new COMPANY_WORK_MACROS.MODEL_STRUCT();

                    tempProductItem.productType.typeId = sqlDatabase.rows[i].sql_int[numericCount++];
                    tempProductItem.productType.typeName = sqlDatabase.rows[i].sql_string[StringCount++];

                    tempProductItem.productBrand.brandId = sqlDatabase.rows[i].sql_int[numericCount++];
                    tempProductItem.productBrand.brandName = sqlDatabase.rows[i].sql_string[StringCount++];

                    tempProductItem.productModel.modelId = sqlDatabase.rows[i].sql_int[numericCount++];
                    tempProductItem.productModel.modelName = sqlDatabase.rows[i].sql_string[StringCount++];

                    tempProductItem.productNumber = j;
                    tempProductItem.productQuantity = 0;

                    RFQItem.products.Add(tempProductItem);
                }

                RFQItem.contract_type_id = sqlDatabase.rows[i].sql_int[numericCount++];
                RFQItem.rfq_status_id = sqlDatabase.rows[i].sql_int[numericCount++];
                RFQItem.failure_reason_id = sqlDatabase.rows[i].sql_int[numericCount++];

                RFQItem.contract_type = sqlDatabase.rows[i].sql_string[StringCount++];
                RFQItem.rfq_status = sqlDatabase.rows[i].sql_string[StringCount++];
                RFQItem.failure_reason = sqlDatabase.rows[i].sql_string[StringCount++];

                returnVector.Add(RFQItem);
            }

            return true;
        }
        public bool GetRFQs(int salesPersonId, int assigneeId, int rfqStatus, ref List<COMPANY_WORK_MACROS.RFQ_BASIC_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select rfqs.rfq_serial, 
        									rfqs.rfq_version, 
        									rfqs.rfq_id 
        							from erp_system.dbo.rfqs 
        							inner join erp_system.dbo.company_address 
        							on rfqs.branch_serial = company_address.address_serial 
        							where rfqs.sales_person = ";
            String sqlQueryPart2 = " and rfqs.assigned_engineer = ";
            String sqlQueryPart3 = " and rfqs.rfq_status = ";
            String sqlQueryPart4 = " order by rfqs.issue_date;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;
            sqlQuery += salesPersonId;
            sqlQuery += sqlQueryPart2;
            sqlQuery += assigneeId;
            sqlQuery += sqlQueryPart3;
            sqlQuery += rfqStatus;
            sqlQuery += sqlQueryPart4;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 2;
            queryColumns.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                COMPANY_WORK_MACROS.RFQ_BASIC_STRUCT tempItem = new COMPANY_WORK_MACROS.RFQ_BASIC_STRUCT();

                tempItem.rfq_serial = sqlDatabase.rows[i].sql_int[0];
                tempItem.rfq_version = sqlDatabase.rows[i].sql_int[1];
                tempItem.rfq_id = sqlDatabase.rows[i].sql_string[0];

                returnVector.Add(tempItem);
            }

            return true;
        }

        public bool GetWorkOffers(ref List<COMPANY_WORK_MACROS.WORK_OFFER_MAX_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 =
                @"with get_company_info	as	(	select company_name.company_serial, 
        		company_address.address_serial, 
        		company_name.company_name 
        		from erp_system.dbo.company_address 
        		inner join erp_system.dbo.company_name 
        		on company_address.company_serial = company_name.company_serial 
        		), 
        		get_sales_person	as(select employees_info.employee_id as sales_person_id, 
        			employees_info.name as sales_person_name 
        			from erp_system.dbo.employees_info 
        		), 
        		get_offer_proposer	as(select employees_info.employee_id as offer_proposer_id, 
        			employees_info.name as offer_proposer_name 
        			from erp_system.dbo.employees_info 
        		), 
        		get_product_type	as(select products_type.id as product_type_id, 
        			products_type.product_name as product_type 
        			from erp_system.dbo.products_type 
        		), 
        		get_product_brand	as(select brands_type.id as product_brand_id, 
        			brands_type.brand_name as product_brand 
        			from erp_system.dbo.brands_type 
        		), 
        		get_product_model	as(select brands_models.model_id as product_model_id, 
        			brands_models.product_id as product_type_id, 
        			brands_models.brand_id as product_brand_id, 
        			brands_models.brand_model as product_model 
        			from erp_system.dbo.brands_models 
        		) 
        
        		select	work_offers.offer_proposer, 
        		work_offers.offer_serial, 
        		work_offers.offer_version, 
        
        		get_sales_person.sales_person_id, 
        		get_company_info.company_serial, 
        		get_company_info.address_serial, 
        		contact_person_info.contact_id, 
        
        		work_offers_products_info.product_number, 
        		get_product_type.product_type_id, 
        		get_product_brand.product_brand_id, 
        		get_product_model.product_model_id, 
        
        		contracts_type.id as contract_type_id, 
        		offers_status.id as offer_status_id, 
        		offer_failure_reasons.id as failure_reason_id, 
        
        		work_offers.issue_date, 
        
        		work_offers.offer_id, 
        
        		get_sales_person.sales_person_name, 
        		get_offer_proposer.offer_proposer_name, 
        
        		get_company_info.company_name, 
        		contact_person_info.name, 
        
        	 	get_product_type.product_type, 
        		get_product_brand.product_brand, 
        		get_product_model.product_model, 
        
        		contracts_type.contract_type, 
        		offers_status.offer_status, 
        		offer_failure_reasons.failure_reason 
        
        		from erp_system.dbo.work_offers 
        
        		inner join get_company_info 
        		on work_offers.branch_serial = get_company_info.address_serial 
        
        		inner join erp_system.dbo.contact_person_info 
        		on work_offers.sales_person = contact_person_info.sales_person_id 
        		and work_offers.branch_serial = contact_person_info.branch_serial 
        		and work_offers.contact_id = contact_person_info.contact_id 
        
        		inner join get_sales_person 
        		on work_offers.sales_person = get_sales_person.sales_person_id 
        
        		inner join get_offer_proposer 
        		on work_offers.offer_proposer = get_offer_proposer.offer_proposer_id 
        
        		inner join erp_system.dbo.employees_initials 
        		on get_offer_proposer.offer_proposer_id = employees_initials.id 
        
        		inner join erp_system.dbo.work_offers_products_info 
        		on work_offers.offer_proposer = work_offers_products_info.offer_proposer 
        		and work_offers.offer_serial = work_offers_products_info.offer_serial 
        		and work_offers.offer_version = work_offers_products_info.offer_version 
        
        		inner join get_product_type 
        		on work_offers_products_info.product_type = get_product_type.product_type_id 
        		inner join get_product_brand 
        		on work_offers_products_info.product_brand = get_product_brand.product_brand_id 
        		left join get_product_model 
        		on work_offers_products_info.product_model = get_product_model.product_model_id 
        		and work_offers_products_info.product_type = get_product_model.product_type_id 
        		and work_offers_products_info.product_brand = get_product_model.product_brand_id 
        
        		inner join erp_system.dbo.contracts_type 
        		on work_offers.contract_type = contracts_type.id 
        
        		inner join erp_system.dbo.offers_status 
        		on work_offers.offer_status = offers_status.id 
        
        		left join erp_system.dbo.unsucessful_work_offers 
        		on work_offers.offer_proposer = unsucessful_work_offers.offer_proposer 
        		and work_offers.offer_serial = unsucessful_work_offers.offer_serial 
        		and work_offers.offer_version = unsucessful_work_offers.offer_version 
        
        		left join erp_system.dbo.offer_failure_reasons 
        		on unsucessful_work_offers.failure_reason = offer_failure_reasons.id 
        
        		order by work_offers.issue_date desc, employees_initials.employee_initial, offer_serial, offer_version, product_number;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 14;
            queryColumns.sql_datetime = 1;
            queryColumns.sql_string = 11;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                int numericCount = 0;
                int StringCount = 0;

                COMPANY_WORK_MACROS.WORK_OFFER_MAX_STRUCT offerItem = new COMPANY_WORK_MACROS.WORK_OFFER_MAX_STRUCT();

                offerItem.products = new List<COMPANY_WORK_MACROS.OFFER_PRODUCT_STRUCT>();

                COMPANY_WORK_MACROS.OFFER_PRODUCT_STRUCT tempProductItem = new COMPANY_WORK_MACROS.OFFER_PRODUCT_STRUCT();

                tempProductItem.productType = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
                tempProductItem.productBrand = new COMPANY_WORK_MACROS.BRAND_STRUCT();
                tempProductItem.productModel = new COMPANY_WORK_MACROS.MODEL_STRUCT();

                offerItem.offer_id = sqlDatabase.rows[i].sql_string[StringCount++];

                offerItem.sales_person_name = sqlDatabase.rows[i].sql_string[StringCount++];
                offerItem.offer_proposer_name = sqlDatabase.rows[i].sql_string[StringCount++];

                offerItem.company_name = sqlDatabase.rows[i].sql_string[StringCount++];
                offerItem.contact_name = sqlDatabase.rows[i].sql_string[StringCount++];

                offerItem.issue_date = sqlDatabase.rows[i].sql_datetime[0].ToString();

                offerItem.offer_proposer_id = sqlDatabase.rows[i].sql_int[numericCount++];
                offerItem.offer_serial = sqlDatabase.rows[i].sql_int[numericCount++];
                offerItem.offer_version = sqlDatabase.rows[i].sql_int[numericCount++];

                offerItem.sales_person_id = sqlDatabase.rows[i].sql_int[numericCount++];
                offerItem.company_serial = sqlDatabase.rows[i].sql_int[numericCount++];
                offerItem.branch_serial = sqlDatabase.rows[i].sql_int[numericCount++];
                offerItem.contact_id = sqlDatabase.rows[i].sql_int[numericCount++];

                int product_number = (int)sqlDatabase.rows[i].sql_int[numericCount++];

                tempProductItem.productType.typeId = sqlDatabase.rows[i].sql_int[numericCount++];
                tempProductItem.productType.typeName = sqlDatabase.rows[i].sql_string[StringCount++];

                tempProductItem.productBrand.brandId = sqlDatabase.rows[i].sql_int[numericCount++];
                tempProductItem.productBrand.brandName = sqlDatabase.rows[i].sql_string[StringCount++];

                tempProductItem.productModel.modelId = sqlDatabase.rows[i].sql_int[numericCount++];
                tempProductItem.productModel.modelName = sqlDatabase.rows[i].sql_string[StringCount++];

                tempProductItem.productNumber = product_number;
                tempProductItem.productQuantity = 0;

                if (i > 0 && returnVector.Last().offer_proposer_id == offerItem.offer_proposer_id && returnVector.Last().offer_serial == offerItem.offer_serial && returnVector.Last().offer_version == offerItem.offer_version)
                {
                    returnVector.Last().products.Add(tempProductItem);
                    returnVector.Last().products.Sort();
                }
                else
                {
                    offerItem.products.Add(tempProductItem);

                    offerItem.contract_type_id = sqlDatabase.rows[i].sql_int[numericCount++];
                    offerItem.offer_status_id = sqlDatabase.rows[i].sql_int[numericCount++];
                    offerItem.failure_reason_id = sqlDatabase.rows[i].sql_int[numericCount++];

                    offerItem.contract_type = sqlDatabase.rows[i].sql_string[StringCount++];
                    offerItem.offer_status = sqlDatabase.rows[i].sql_string[StringCount++];
                    offerItem.failure_reason = sqlDatabase.rows[i].sql_string[StringCount++];

                    returnVector.Add(offerItem);
                }

            }

            return true;
        }
        public bool GetWorkOrders(ref List<COMPANY_WORK_MACROS.WORK_ORDER_MAX_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 =
                @"with get_company_info	as	(	select company_name.company_serial, 
        		company_address.address_serial, 
        		company_name.company_name 
        		from erp_system.dbo.company_address 
        		inner join erp_system.dbo.company_name 
        		on company_address.company_serial = company_name.company_serial 
        		), 
        		get_sales_person		as(select employees_info.employee_id as sales_person_id, 
        			employees_info.name as sales_person_name 
        			from erp_system.dbo.employees_info 
        		), 
        		get_offer_proposer	as(select employees_info.employee_id as offer_proposer_id, 
        			employees_info.name as offer_proposer_name 
        			from erp_system.dbo.employees_info 
        		), 
        		get_product_type		as(select products_type.id as product_type_id, 
        			products_type.product_name as product_type 
        			from erp_system.dbo.products_type 
        		), 
        		get_product_brand		as(select brands_type.id as product_brand_id, 
        			brands_type.brand_name as product_brand 
        			from erp_system.dbo.brands_type 
        		), 
        		get_product_model		as(select brands_models.model_id as product_model_id, 
        			brands_models.product_id as product_type_id, 
        			brands_models.brand_id as product_brand_id, 
        			brands_models.brand_model as product_model 
        			from erp_system.dbo.brands_models 
        		) 
        
        		select 	get_sales_person.sales_person_id, 
        		work_orders.order_serial, 
        
        		get_offer_proposer.offer_proposer_id, 
        
        		get_company_info.company_serial, 
        		get_company_info.address_serial, 
        		contact_person_info.contact_id, 
        
        		work_offers_products_info.product_number, 
        		get_product_type.product_type_id, 
        		get_product_brand.product_brand_id, 
        		get_product_model.product_model_id, 
        
        		contracts_type.id as contract_type_id, 
        		orders_status.id as order_status_id, 
        
        		work_orders.issue_date, 
        
        		work_orders.order_id, 
        
        		get_sales_person.sales_person_name, 
        		get_offer_proposer.offer_proposer_name, 
        
        		get_company_info.company_name, 
        		contact_person_info.name, 
        
        		get_product_type.product_type, 
        		get_product_brand.product_brand, 
        		get_product_model.product_model, 
        
        		contracts_type.contract_type, 
        		orders_status.order_status 
        
        		from erp_system.dbo.work_orders 
        
        		inner join erp_system.dbo.work_offers 
        		on work_orders.offer_proposer = work_offers.offer_proposer 
        		and work_orders.offer_serial = work_offers.offer_serial 
        		and work_orders.offer_version = work_offers.offer_version 
        
        		inner join get_company_info 
        		on work_offers.branch_serial = get_company_info.address_serial 
        
        		inner join erp_system.dbo.contact_person_info 
        		on work_offers.sales_person = contact_person_info.sales_person_id 
        		and work_offers.branch_serial = contact_person_info.branch_serial 
        		and work_offers.contact_id = contact_person_info.contact_id 
        
        		inner join get_sales_person 
        		on work_offers.sales_person = get_sales_person.sales_person_id 
        
        		inner join get_offer_proposer 
        		on work_offers.offer_proposer = get_offer_proposer.offer_proposer_id 
        
        		inner join erp_system.dbo.employees_initials 
        		on get_offer_proposer.offer_proposer_id = employees_initials.id 
        
        		inner join erp_system.dbo.work_offers_products_info 
        		on work_offers.offer_proposer = work_offers_products_info.offer_proposer 
        		and work_offers.offer_serial = work_offers_products_info.offer_serial 
        		and work_offers.offer_version = work_offers_products_info.offer_version 
        
        		inner join get_product_type 
        		on work_offers_products_info.product_type = get_product_type.product_type_id 
        		inner join get_product_brand 
        		on work_offers_products_info.product_brand = get_product_brand.product_brand_id 
        		left join get_product_model 
        		on work_offers_products_info.product_model = get_product_model.product_model_id 
        		and work_offers_products_info.product_type = get_product_model.product_type_id 
        		and work_offers_products_info.product_brand = get_product_model.product_brand_id 
        
        		inner join erp_system.dbo.contracts_type 
        		on work_offers.contract_type = contracts_type.id 
        
        		inner join erp_system.dbo.orders_status 
        		on work_orders.order_status = orders_status.id 
        
        		order by work_orders.issue_date desc, employees_initials.employee_initial, work_orders.order_serial, work_offers_products_info.product_number;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 12;
            queryColumns.sql_datetime = 1;
            queryColumns.sql_string = 10;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                int numericCount = 0;
                int StringCount = 0;

                COMPANY_WORK_MACROS.WORK_ORDER_MAX_STRUCT orderItem = new COMPANY_WORK_MACROS.WORK_ORDER_MAX_STRUCT();

                orderItem.products = new List<COMPANY_WORK_MACROS.ORDER_PRODUCT_STRUCT>();

                COMPANY_WORK_MACROS.ORDER_PRODUCT_STRUCT tempProductItem = new COMPANY_WORK_MACROS.ORDER_PRODUCT_STRUCT();

                tempProductItem.productType = new COMPANY_WORK_MACROS.PRODUCT_STRUCT();
                tempProductItem.productBrand = new COMPANY_WORK_MACROS.BRAND_STRUCT();
                tempProductItem.productModel = new COMPANY_WORK_MACROS.MODEL_STRUCT();

                orderItem.order_id = sqlDatabase.rows[i].sql_string[StringCount++];

                orderItem.sales_person_name = sqlDatabase.rows[i].sql_string[StringCount++];
                orderItem.offer_proposer_name = sqlDatabase.rows[i].sql_string[StringCount++];

                orderItem.company_name = sqlDatabase.rows[i].sql_string[StringCount++];
                orderItem.contact_name = sqlDatabase.rows[i].sql_string[StringCount++];

                orderItem.issue_date = sqlDatabase.rows[i].sql_datetime[0].ToString();

                orderItem.sales_person_id = sqlDatabase.rows[i].sql_int[numericCount++];
                orderItem.order_serial = sqlDatabase.rows[i].sql_int[numericCount++];

                orderItem.offer_proposer_id = sqlDatabase.rows[i].sql_int[numericCount++];

                orderItem.company_serial = sqlDatabase.rows[i].sql_int[numericCount++];
                orderItem.branch_serial = sqlDatabase.rows[i].sql_int[numericCount++];
                orderItem.contact_id = sqlDatabase.rows[i].sql_int[numericCount++];

                int product_number = sqlDatabase.rows[i].sql_int[numericCount++];

                tempProductItem.productType.typeId = sqlDatabase.rows[i].sql_int[numericCount++];
                tempProductItem.productType.typeName = sqlDatabase.rows[i].sql_string[StringCount++];

                tempProductItem.productBrand.brandId = sqlDatabase.rows[i].sql_int[numericCount++];
                tempProductItem.productBrand.brandName = sqlDatabase.rows[i].sql_string[StringCount++];

                tempProductItem.productModel.modelId = sqlDatabase.rows[i].sql_int[numericCount++];
                tempProductItem.productModel.modelName = sqlDatabase.rows[i].sql_string[StringCount++];

                tempProductItem.productNumber = product_number;
                tempProductItem.productQuantity = 0;

                if (i > 0 && returnVector.Last().sales_person_id == orderItem.sales_person_id && returnVector.Last().order_serial == orderItem.order_serial)
                {
                    returnVector.Last().products.Add(tempProductItem);
                    returnVector.Last().products.Sort();
                }
                else
                {
                    orderItem.products.Add(tempProductItem);

                    orderItem.contract_type_id = sqlDatabase.rows[i].sql_int[numericCount++];
                    orderItem.order_status_id = sqlDatabase.rows[i].sql_int[numericCount++];

                    orderItem.contract_type = sqlDatabase.rows[i].sql_string[StringCount++];
                    orderItem.order_status = sqlDatabase.rows[i].sql_string[StringCount++];

                    returnVector.Add(orderItem);
                }
            }

            return true;
        }


        public bool GetClientVisits(ref List<COMPANY_WORK_MACROS.CLIENT_VISIT_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select employees_info.employee_id, 
                                    client_visits.visit_serial, 
                                    client_visits.visit_purpose, 
                                    client_visits.visit_result, 

                                    issue_date, 
                                    date_of_visit, 

                                    employees_info.name,
                                    company_name.company_name, 
                                    contact_person_info.name,
	                                visits_purpose.visit_purpose, 
                                    visits_result.visit_result
                                    from erp_system.dbo.client_visits
                                    
                                    inner join erp_system.dbo.contact_person_info 
                                    on client_visits.branch_serial = contact_person_info.branch_serial
                                    and client_visits.sales_person = contact_person_info.sales_person_id
                                    and client_visits.contact_id = contact_person_info.contact_id
                                    
                                    inner join erp_system.dbo.company_address
                                    on client_visits.branch_serial = company_address.address_serial
                                    
                                    inner join erp_system.dbo.company_name
                                    on company_address.company_serial = company_name.company_serial
                                    
                                    inner join erp_system.dbo.visits_purpose
                                    on client_visits.visit_purpose = visits_purpose.id
                                    
                                    inner join erp_system.dbo.visits_result
                                    on client_visits.visit_result = visits_result.id
                                    
                                    inner join  erp_system.dbo.employees_info
                                    on client_visits.sales_person = employees_info.employee_id
                                    
                                    order by client_visits.date_of_visit DESC;";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 4;
            queryColumns.sql_datetime = 2;
            queryColumns.sql_string = 5;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                int numericCount = 0;
                int StringCount = 0;

                COMPANY_WORK_MACROS.CLIENT_VISIT_STRUCT visitItem;

                visitItem.sales_person_id = sqlDatabase.rows[i].sql_int[numericCount++];
                visitItem.visit_serial = sqlDatabase.rows[i].sql_int[numericCount++];

                visitItem.visit_purpose_id = sqlDatabase.rows[i].sql_int[numericCount++];
                visitItem.visit_result_id = sqlDatabase.rows[i].sql_int[numericCount++];

                visitItem.issue_date = sqlDatabase.rows[i].sql_datetime[0].ToString();
                visitItem.visit_date = sqlDatabase.rows[i].sql_datetime[1].ToString();

                visitItem.sales_person_name = sqlDatabase.rows[i].sql_string[StringCount++];

                visitItem.company_name = sqlDatabase.rows[i].sql_string[StringCount++];
                visitItem.contact_name = sqlDatabase.rows[i].sql_string[StringCount++];

                visitItem.visit_purpose = sqlDatabase.rows[i].sql_string[StringCount++];
                visitItem.visit_result = sqlDatabase.rows[i].sql_string[StringCount++];

                returnVector.Add(visitItem);
            }

            return true;
        }
        public bool GetClientCalls(ref List<COMPANY_WORK_MACROS.CLIENT_CALL_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"with get_company_name	as	(	select company_address.address_serial, 

                                                                    company_name.company_name 
															from erp_system.dbo.company_address 
															inner join erp_system.dbo.company_name 
															on company_address.company_serial = company_name.company_serial 
															) 
		    select client_calls.sales_person, 
		    client_calls.call_serial, 
		    calls_purpose.id, 
		    calls_result.id, 

		    client_calls.issue_date, 
		    client_calls.date_of_call, 

		    employees_info.name, 
		    get_company_name.company_name, 
		    contact_person_info.name, 
		    calls_purpose.call_purpose, 
		    calls_result.call_result 

		    from erp_system.dbo.client_calls 

		    inner join erp_system.dbo.employees_info 
		    on client_calls.sales_person = employees_info.employee_id 

		    inner join get_company_name 
		    on client_calls.branch_serial = get_company_name.address_serial 

		    inner join erp_system.dbo.calls_purpose 
		    on client_calls.call_purpose = calls_purpose.id 

		    inner join erp_system.dbo.calls_result 
		    on client_calls.call_result = calls_result.id 

		    inner join erp_system.dbo.contact_person_info 
		    on client_calls.sales_person = contact_person_info.sales_person_id 
		    and client_calls.branch_serial = contact_person_info.branch_serial 
		    and client_calls.contact_id = contact_person_info.contact_id 

		    order by client_calls.date_of_call DESC; ";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 4;
            queryColumns.sql_datetime = 2;
            queryColumns.sql_string = 5;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                int numericCount = 0;
                int StringCount = 0;

                COMPANY_WORK_MACROS.CLIENT_CALL_STRUCT callItem;

                callItem.sales_person_id = sqlDatabase.rows[i].sql_int[numericCount++];
                callItem.call_serial = sqlDatabase.rows[i].sql_int[numericCount++];

                callItem.call_purpose_id = sqlDatabase.rows[i].sql_int[numericCount++];
                callItem.call_result_id = sqlDatabase.rows[i].sql_int[numericCount++];

                callItem.issue_date = sqlDatabase.rows[i].sql_datetime[0].ToString();
                callItem.call_date = sqlDatabase.rows[i].sql_datetime[1].ToString();

                callItem.sales_person_name = sqlDatabase.rows[i].sql_string[StringCount++];

                callItem.company_name = sqlDatabase.rows[i].sql_string[StringCount++];
                callItem.contact_name = sqlDatabase.rows[i].sql_string[StringCount++];

                callItem.call_purpose = sqlDatabase.rows[i].sql_string[StringCount++];
                callItem.call_result = sqlDatabase.rows[i].sql_string[StringCount++];

                returnVector.Add(callItem);
            }

            return true;
        }

        public bool GetOfficeMeetings(ref List<COMPANY_WORK_MACROS.OFFICE_MEETING_BASIC_STRUCT> returnVector)
        {
            returnVector.Clear();

            String sqlQueryPart1 = @"select office_meetings.meeting_serial,
employees_info.employee_id, 

            office_meetings.meeting_purpose, 

		    office_meetings.date_of_meeting, 
		    office_meetings.issue_date, 

		    employees_info.name, 
		    meetings_purpose.meeting_purpose, 
		    office_meetings.meeting_note 

		    from erp_system.dbo.office_meetings 

		    inner join erp_system.dbo.employees_info 
		    on office_meetings.called_by = employees_info.employee_id 

		    inner join erp_system.dbo.meetings_purpose 
		    on office_meetings.meeting_purpose = meetings_purpose.id 

		    order by office_meetings.date_of_meeting DESC; ";

            sqlQuery = String.Empty;
            sqlQuery += sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();

            queryColumns.sql_int = 3;
            queryColumns.sql_datetime = 2;
            queryColumns.sql_string = 3;

            if (!sqlDatabase.GetRows(sqlQuery, queryColumns))
                return false;


            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                int numericCount = 0;
                int stringCount = 0;

                COMPANY_WORK_MACROS.OFFICE_MEETING_BASIC_STRUCT meetingItem = new COMPANY_WORK_MACROS.OFFICE_MEETING_BASIC_STRUCT();
                meetingItem.meeting_purpose = new COMPANY_WORK_MACROS.MEETING_PURPOSE_STRUCT();

                meetingItem.meeting_serial = sqlDatabase.rows[i].sql_int[numericCount++];

                meetingItem.meeting_caller.employee_id = sqlDatabase.rows[i].sql_int[numericCount++];
                meetingItem.meeting_purpose.purpose_id = sqlDatabase.rows[i].sql_int[numericCount++];

                meetingItem.issue_date = sqlDatabase.rows[i].sql_datetime[0].ToString();
                meetingItem.meeting_date = sqlDatabase.rows[i].sql_datetime[1].ToString();

                meetingItem.meeting_caller.employee_name = sqlDatabase.rows[i].sql_string[stringCount++];
                meetingItem.meeting_purpose.purpose_name = sqlDatabase.rows[i].sql_string[stringCount++];

                meetingItem.meeting_note = sqlDatabase.rows[i].sql_string[stringCount++];

                returnVector.Add(meetingItem);
            }

            return true;
        }
    }

}
