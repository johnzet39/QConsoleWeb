using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using QConsoleWeb.DAL.AccessLayer.Entities;
using QConsoleWeb.DAL.AccessLayer.Interfaces;

namespace QConsoleWeb.DAL.AccessLayer.DAO
{
    // queries for create/edit layers
    internal static class LayerQueries
    {
        public static string CommentOnTable(string tableschema, string tablename, string definition)
        {
            return String.Format("COMMENT ON TABLE {0}.{1} IS '{2}';", tableschema, tablename, definition);
        }

        public static string SetUpdater(string tableschema, string tablename, Boolean isupdater)
        {
            return String.Format("SELECT logger.qfunc_addupdatefields('{0}', '{1}', {2});", tableschema, tablename, isupdater.ToString().ToUpper());
        }

        public static string SetLogger(string tableschema, string tablename, Boolean islogger)
        {
            return String.Format("SELECT logger.qfunc_loglogger('{0}', '{1}', {2});", tableschema, tablename, islogger.ToString().ToUpper());
        }

    }


    internal class LayerDAO : ILayerDAO
    {
        readonly private string _connectionString;

        public LayerDAO(string connstring)
        {
            _connectionString = connstring;
        }

        //layers
        public List<Layer> GetLayers()
        {
            string sql_query = " SELECT t.table_schema, t.table_name ,  " +
                                " (select obj_description((quote_ident(t.table_schema)||'.'||quote_ident(t.table_name))::regclass, 'pg_class')) descript, " +
                                " (select string_agg(gc.type, ', ') from geometry_columns gc where gc.f_table_schema = t.table_schema and gc.f_table_name = t.table_name)  as geomtype, " +
                                " case  " +
                                    " WHEN coalesce((select 1 from information_schema.triggers tr where tr.event_object_schema = t.table_schema AND tr.event_object_table = t.table_name AND tr.trigger_name = tr.event_object_table || '_log_update_trigger' limit 1),0) = 1 THEN  " +
                                        " true  " +
                                    " ELSE  " +
                                        " false " +
                                " end as isupdater, " +
                                " case  " +
                                    " WHEN coalesce((select 1 from information_schema.triggers tr where tr.event_object_schema = t.table_schema AND tr.event_object_table = t.table_name AND tr.trigger_name = tr.event_object_table || '_log_logger_trigger' limit 1),0) = 1 THEN  " +
                                        " true " +
                                    " ELSE  " +
                                        " false " +
                                " end as islogger " +
                               " FROM information_schema.tables t  " +
                               " WHERE EXISTS  (select 1 from geometry_columns gc where gc.f_table_schema = t.table_schema and gc.f_table_name = t.table_name limit 1) AND (t.table_schema not in  ('logger', 'tiger', 'schema_spr')) " +
                               " ORDER BY t.table_schema, t.table_name ; ";

            return GetListOfObjects(sql_query);
        }

        //dictionaries
        public List<Layer> GetDicts()
        {
            string sql_query = " SELECT t.table_schema, t.table_name ,  " +
                                " (select obj_description((quote_ident(t.table_schema)||'.'||quote_ident(t.table_name))::regclass, 'pg_class')) descript, " +
                                " (select string_agg(gc.type, ', ') from geometry_columns gc where gc.f_table_schema = t.table_schema and gc.f_table_name = t.table_name)  as geomtype, " +
                                " case  " +
                                    " WHEN coalesce((select 1 from information_schema.triggers tr where tr.event_object_schema = t.table_schema AND tr.event_object_table = t.table_name AND tr.trigger_name = tr.event_object_table || '_log_update_trigger' limit 1),0) = 1 THEN  " +
                                        " true  " +
                                    " ELSE  " +
                                        " false " +
                                " end as isupdater, " +
                                " case  " +
                                    " WHEN coalesce((select 1 from information_schema.triggers tr where tr.event_object_schema = t.table_schema AND tr.event_object_table = t.table_name AND tr.trigger_name = tr.event_object_table || '_log_logger_trigger' limit 1),0) = 1 THEN  " +
                                        " true " +
                                    " ELSE  " +
                                        " false " +
                                " end as islogger " +
                               " FROM information_schema.tables t  " +
                               " WHERE (t.table_schema = 'schema_spr') OR t.table_schema||t.table_name in (select spr.schema_name||spr.table_name from logger.dictionaries spr) " +
                               " ORDER BY t.table_schema, t.table_name ; ";

            return GetListOfObjects(sql_query);
        }

        public List<InformationSchemaTable> GetAllTables()
        {
            string sql_query = " SELECT table_schema, table_name, table_type FROM information_schema.tables WHERE table_type = 'BASE TABLE'";

            return GetListOfAllTables(sql_query);
        }

        public List<LayerGrants> GetGrantsToLayer(string schemaname, string tablename)
        {
            string sql_query = String.Format(
@"select schemaname, tablename, groname,
	isselect,
	isinsert,
	isupdate,
	isdelete,
	case when isselect <> true then
		(select string_agg(cp.column_name, ', ') from INFORMATION_SCHEMA.column_privileges cp 
		 	where cp.privilege_type='SELECT' and cp.grantee=groname and cp.table_schema=schemaname and cp.table_name=tablename group by cp.grantee, cp.table_schema, cp.table_name)
		else null
	end as columns_select,
	case when isinsert <> true then
		(select string_agg(cp.column_name, ', ') from INFORMATION_SCHEMA.column_privileges cp 
		 	where cp.privilege_type='INSERT' and cp.grantee=groname and cp.table_schema=schemaname and cp.table_name=tablename group by cp.grantee, cp.table_schema, cp.table_name)
		else null
	end as columns_insert,
	case when isupdate <> true then
		(select string_agg(cp.column_name, ', ') from INFORMATION_SCHEMA.column_privileges cp 
		 	where cp.privilege_type='UPDATE' and cp.grantee=groname and cp.table_schema=schemaname and cp.table_name=tablename group by cp.grantee, cp.table_schema, cp.table_name)
		else null
	end as columns_update
from
(select
	  a.table_schema as schemaname,a.table_name as tablename,b.groname,
	  HAS_TABLE_PRIVILEGE(b.groname,a.table_schema||'.'||a.table_name, 'select') as isselect,
	  HAS_TABLE_PRIVILEGE(b.groname,a.table_schema||'.'||a.table_name, 'insert') as isinsert,
	  HAS_TABLE_PRIVILEGE(b.groname,a.table_schema||'.'||a.table_name, 'update') as isupdate,
	  HAS_TABLE_PRIVILEGE(b.groname,a.table_schema||'.'||a.table_name, 'delete') as isdelete
	  from information_schema.tables a , pg_group b 
	where a.table_name='{1}' and a.table_schema='{0}') sub", schemaname, tablename);
            return GetListOfLayerGrants(sql_query);
        }

        private List<Layer> GetListOfObjects(string sql_query)
        {
            var listOfObjects = new List<Layer>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var command = new NpgsqlCommand(sql_query, conn))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var objectpsql = new Layer();

                            objectpsql.Table_schema = dataReader["table_schema"].ToString();
                            objectpsql.Table_name = dataReader["table_name"].ToString();
                            objectpsql.Descript = dataReader["descript"].ToString();
                            objectpsql.Geomtype = dataReader["geomtype"].ToString();
                            objectpsql.Isupdater = Convert.ToBoolean(dataReader["isupdater"]);
                            objectpsql.Islogger = Convert.ToBoolean(dataReader["islogger"]);

                            listOfObjects.Add(objectpsql);
                        }
                    }
                }
            }
            return listOfObjects;
        }


        private List<LayerGrants> GetListOfLayerGrants(string sql_query)
        {
            var listOfObjects = new List<LayerGrants>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var command = new NpgsqlCommand(sql_query, conn))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var objectpsql = new LayerGrants();

                            objectpsql.schemaname = dataReader["schemaname"].ToString();
                            objectpsql.tablename = dataReader["tablename"].ToString();
                            objectpsql.groname = dataReader["groname"].ToString();
                            objectpsql.isselect = Convert.ToBoolean(dataReader["isselect"]);
                            objectpsql.isupdate = Convert.ToBoolean(dataReader["isupdate"]);
                            objectpsql.isinsert = Convert.ToBoolean(dataReader["isinsert"]);
                            objectpsql.isdelete = Convert.ToBoolean(dataReader["isdelete"]);
                            objectpsql.columns_select = dataReader["columns_select"].ToString();
                            objectpsql.columns_update = dataReader["columns_update"].ToString();
                            objectpsql.columns_insert = dataReader["columns_insert"].ToString();

                            listOfObjects.Add(objectpsql);
                        }
                    }
                }
            }
            return listOfObjects;
        }

        private List<InformationSchemaTable> GetListOfAllTables(string sql_query)
        {
            var listOfObjects = new List<InformationSchemaTable>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var command = new NpgsqlCommand(sql_query, conn))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var objectpsql = new InformationSchemaTable();

                            objectpsql.Table_schema = dataReader["table_schema"].ToString();
                            objectpsql.Table_name = dataReader["table_name"].ToString();
                            objectpsql.Table_type = dataReader["table_type"].ToString();

                            listOfObjects.Add(objectpsql);
                        }
                    }
                }
            }
            return listOfObjects;
        }

        //Change layer
        public void ChangeLayer(string tableschema, string tablename, string descript, bool? isupdater, bool? islogger)
        {
            List<String> sql_queries = new List<String>();

            if (descript != null) sql_queries.Add(LayerQueries.CommentOnTable(tableschema, tablename, descript));
            if (isupdater != null) sql_queries.Add(LayerQueries.SetUpdater(tableschema, tablename, (bool)isupdater));
            if (islogger != null) sql_queries.Add(LayerQueries.SetLogger(tableschema, tablename, (bool)islogger));

            try
            {
                ExecuteSqlNonQuery(sql_queries);
            }
            catch
            {
                throw;
            }
        }

        //Execute queries
        private void ExecuteSqlNonQuery(List<string> sql_queries)
        {
            string current_query = null;
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    foreach (string sql_query in sql_queries)
                    {
                        current_query = sql_query;
                        using (var command = new NpgsqlCommand(sql_query, conn))
                        {
                            (command.ExecuteNonQuery()).ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(current_query + "\nException: " + ex.Message.ToString());
            }
        }

        public int GetCountOfPeriod(string tableshcema, string tablename, int days)
        {
            int count = 0;
            string sql_query = String.Format("SELECT count(*) from {0}.{1} where update_time > now() - INTERVAL '{2} DAYS'", tableshcema, tablename, days.ToString());

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var command = new NpgsqlCommand(sql_query, conn))
                {
                    count = Int32.Parse(command.ExecuteScalar().ToString());
                }
            }

            return count;
        }
    }
}
