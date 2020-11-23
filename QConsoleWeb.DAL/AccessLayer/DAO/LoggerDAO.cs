using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using QConsoleWeb.DAL.AccessLayer.Entities;
using QConsoleWeb.DAL.AccessLayer.Interfaces;

namespace QConsoleWeb.DAL.AccessLayer.DAO
{

    internal class LoggerDAO : ILoggerDAO
    {
        private string _connectionString;

        /// <summary>
        /// Construcrot
        /// </summary>
        public LoggerDAO(string connstring)
        {
            _connectionString = connstring;
        }



        //LOG LIST main
        public List<LogRow> GetLogList(string ExtraQueryFull, string FirstRowsQuery)
        {
             string sql_query = String.Format("SELECT a.\"gid\", a.\"action\", a.\"username\", a.\"address\", to_char(a.\"timechange\",'DD.MM.YYYY HH24:MI:SS') as \"timechange\", " +
                                " a.\"tableschema\", a.\"tablename\", a.\"gidnum\", a.\"context\"   FROM logger.logtable a {0}  order by a.\"timechange\" DESC {1} ", ExtraQueryFull, FirstRowsQuery);

            return GetListOfObjects(sql_query);

        }

        //get list of Groups/Users
        private List<LogRow> GetListOfObjects(string sql_query)
        {
            var listOfObjects = new List<LogRow>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var command = new NpgsqlCommand(sql_query, conn))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var objectpsql = new LogRow();

                            objectpsql.Gid = dataReader["Gid"].ToString();
                            objectpsql.Action = dataReader["Action"].ToString();
                            objectpsql.Username = dataReader["Username"].ToString();
                            objectpsql.Address = dataReader["Address"].ToString();
                            objectpsql.Timechange = DateTime.ParseExact(dataReader["Timechange"].ToString(), "dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                            objectpsql.Tableschema = dataReader["Tableschema"].ToString();
                            objectpsql.Tablename = dataReader["Tablename"].ToString();
                            objectpsql.Gidnum = dataReader["Gidnum"].ToString();
                            objectpsql.Context = dataReader["Context"].ToString();

                            listOfObjects.Add(objectpsql);
                        }
                    }
                }

            }
            return listOfObjects;
        }


        //build date string subquery
        public string BuildExtraDateString(DateTime? dateFrom, DateTime? dateTo)
        {
            List<string> list = new List<string>();
            if (dateFrom.HasValue)
            {
                string dateFrom_formated = dateFrom.Value.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                string DateFromString = string.Format("\"timechange\" >= to_date('{0}', 'DD.MM.YYYY')", dateFrom_formated);
                list.Add(DateFromString);
            }
            if (dateTo.HasValue)
            {
                string dateTo_formated = ((DateTime)dateTo).AddDays(1).ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                string DateToString = string.Format("\"timechange\" < to_date('{0}', 'DD.MM.YYYY')", dateTo_formated);
                list.Add(DateToString);
            }
            string ExtraDateString = string.Join(" AND ", list);
            return ExtraDateString;
        }
        //build 1000rows string subquery
        public string BuildExtraFirstRowsString(int countRows)
        {
            return " limit " + countRows;
        }
        //union extra subquery strings
        public string UnionExtraStrings(IList<string> str)
        {
            string extraQueryFull = string.Join(" AND ", str);
            return " WHERE " + extraQueryFull;
        }
        //get column list for combobox
        public List<string> GetColumnsList()
        {
            return new List<string>() { "gid", "action", "username", "address", "tableschema", "tablename",  "gidnum", "context" };
        }
    }
}
