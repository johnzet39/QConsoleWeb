using Npgsql;
using QConsoleWeb.DAL.AccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QConsoleWeb.DAL.AccessLayer.DAO
{
    internal class QueryDAO : IQueryDAO
    {
        private string _connectionString;
        public QueryDAO(string connstring)
        {
            _connectionString = connstring;
        }


        public DataTable ExecuteQuery(string queryString)
        {
            try
            {
                return GetQueryResult(queryString);
            }
            catch
            {
                throw;
            }
        }

        // return DataTable for SQL query
        private DataTable GetQueryResult(string sql)
        {
            DataTable dataTable = new DataTable();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var command = new NpgsqlCommand(sql, conn))
                {
                    command.AllResultTypesAreUnknown = true;
                    var dataReader = command.ExecuteReader();
                    dataTable.Load(dataReader);
                    dataReader.Close();
                }
            }
            return dataTable;
        }
    }
}
