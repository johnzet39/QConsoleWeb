﻿using System;
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

        public static string SetDocFiles(string tableschema, string tablename, Boolean isdocfiles, string df_tablename=null)
        {
            if (df_tablename == null)
                return String.Format("SELECT schema_docfiles.qfunc_docfiles_creater('{0}', '{1}', {2});", tableschema, tablename, isdocfiles.ToString().ToUpper());
            else
                return String.Format("SELECT schema_docfiles.qfunc_docfiles_creater('{0}', '{1}', {2}, '{3}');", tableschema, tablename, isdocfiles.ToString().ToUpper(), df_tablename);
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
                                " end as islogger, " +

                                    " (SELECT string_agg(r.table_name, ', ') " +
                                    " FROM information_schema.constraint_column_usage       u " +
                                    " INNER JOIN information_schema.referential_constraints fk " +
                                    "            ON u.constraint_catalog = fk.unique_constraint_catalog " +
                                    "                AND u.constraint_schema = fk.unique_constraint_schema " +
                                    "                AND u.constraint_name = fk.unique_constraint_name " +
                                    " INNER JOIN information_schema.key_column_usage        r " +
                                    "            ON r.constraint_catalog = fk.constraint_catalog " +
                                    "                AND r.constraint_schema = fk.constraint_schema " +
                                    "                AND r.constraint_name = fk.constraint_name " +
                                    " WHERE " +
                                    "     u.table_schema = t.table_schema AND " +
                                    "     u.table_name = t.table_name AND " +
                                    "     r.table_name like 'df_%' AND " +
                                    "     r.table_schema = 'schema_docfiles') " +
                                " as docfiles_table " +
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
                                " end as islogger, " +

                                    " (SELECT string_agg(r.table_name, ', ') " +
                                    " FROM information_schema.constraint_column_usage       u " +
                                    " INNER JOIN information_schema.referential_constraints fk " +
                                    "            ON u.constraint_catalog = fk.unique_constraint_catalog " +
                                    "                AND u.constraint_schema = fk.unique_constraint_schema " +
                                    "                AND u.constraint_name = fk.unique_constraint_name " +
                                    " INNER JOIN information_schema.key_column_usage        r " +
                                    "            ON r.constraint_catalog = fk.constraint_catalog " +
                                    "                AND r.constraint_schema = fk.constraint_schema " +
                                    "                AND r.constraint_name = fk.constraint_name " +
                                    " WHERE " +
                                    "     u.table_schema = t.table_schema AND " +
                                    "     u.table_name = t.table_name AND " +
                                    "     r.table_name like 'df_%' AND " +
                                    "     r.table_schema = 'schema_docfiles') " +
                                " as docfiles_table " +
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
@"
    with colsprivs as 
        (select cpr.column_name, cpr.privilege_type, cpr.grantee, cpr.table_schema, cpr.table_name 
            from  INFORMATION_SCHEMA.column_privileges cpr where cpr.table_schema='{0}' and cpr.table_name='{1}'),
    
    tabsprivs as 
        (select grantee, table_schema, table_name, privilege_type 
            from INFORMATION_SCHEMA.table_privileges cpr where cpr.table_schema='{0}' and cpr.table_name='{1}' ),
    
    pgclass_ad as 
         (select a.relname, a.oid, a.relnamespace, s.nspname 
            from pg_class a
            join pg_namespace s ON s.oid = a.relnamespace 
            where relname='{1}' and relnamespace = (SELECT to_regnamespace('{0}')::oid))

select * from ( 

    select schemaname, tablename, groname,      
        isselect,
        isinsert,
        isupdate,
        isdelete,
        case when isselect <> true then
            (select string_agg(cp.column_name, ', ') from colsprivs cp
                where cp.privilege_type='SELECT' and cp.grantee=groname group by cp.grantee, cp.table_schema, cp.table_name)
            else null
        end as columns_select,
        case when isinsert <> true then
            (select string_agg(cp.column_name, ', ') from colsprivs cp
                where cp.privilege_type='INSERT' and cp.grantee=groname group by cp.grantee, cp.table_schema, cp.table_name)
            else null
        end as columns_insert,
        case when isupdate <> true then
            (select string_agg(cp.column_name, ', ') from colsprivs cp
                where cp.privilege_type='UPDATE' and cp.grantee=groname group by cp.grantee, cp.table_schema, cp.table_name)
            else null
        end as columns_update
    from
        (select
              pgc.nspname as schemaname, pgc.relname as tablename, b.rolname as groname,
              exists (select true from tabsprivs where tabsprivs.privilege_type = 'SELECT' and tabsprivs.grantee = b.rolname) as  isselect,
              exists (select true from tabsprivs where tabsprivs.privilege_type = 'INSERT' and tabsprivs.grantee = b.rolname) as  isinsert,
              exists (select true from tabsprivs where tabsprivs.privilege_type = 'UPDATE' and tabsprivs.grantee = b.rolname) as  isupdate,
              exists (select true from tabsprivs where tabsprivs.privilege_type = 'DELETE' and tabsprivs.grantee = b.rolname) as  isdelete
              from pgclass_ad as pgc , pg_roles b 
              where b.rolname not in ('postgres')
        ) sub
) al
where isselect = true or isinsert = true or isupdate = true or isdelete = true or columns_select is not null or columns_insert is not null or columns_update is not null
order by groname
"
                                                , schemaname, tablename);
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
                            objectpsql.Docfiles_table = dataReader["docfiles_table"].ToString();

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
        public void ChangeLayer(string tableschema, string tablename, string descript, bool? isupdater, bool? islogger, string nameDocFilesTable)
        {
            List<String> sql_queries = new List<String>();

            if (descript != null) sql_queries.Add(LayerQueries.CommentOnTable(tableschema, tablename, descript));
            if (isupdater != null) sql_queries.Add(LayerQueries.SetUpdater(tableschema, tablename, (bool)isupdater));
            if (islogger != null) sql_queries.Add(LayerQueries.SetLogger(tableschema, tablename, (bool)islogger));
            if (nameDocFilesTable != null) {
                if (nameDocFilesTable.Trim().Length == 0) nameDocFilesTable = null;
                sql_queries.Add(LayerQueries.SetDocFiles(tableschema, tablename, true, nameDocFilesTable));
            }

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
