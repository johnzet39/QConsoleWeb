using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using QConsoleWeb.DAL.AccessLayer.Interfaces;
using QConsoleWeb.DAL.AccessLayer.Entities;


namespace QConsoleWeb.DAL.AccessLayer.DAO
{
    internal class SessionDAO : ISessionDAO
    {
        public NpgsqlConnection _sqlConnection { get; private set; }
        public string _connectionString { get; private set; }

        public SessionDAO(string connstring)
        {
            _connectionString = connstring;
            _sqlConnection = new NpgsqlConnection(_connectionString);
        }

        public IEnumerable<Session> GetSessionsList()
        {
            var listOfSessions = new List<Session>();
            string sql = "select \"pid\", \"application_name\", to_char(\"backend_start\",'DD.MM.YYYY HH24:MI:SS') as starttime, " +
                                " \"usename\", (select shobj_description(\"usesysid\", 'pg_authid')) as descript, \"client_addr\", " +
                                " to_char(now(),'DD.MM.YYYY HH24:MI:SS') as now from pg_stat_activity order by usename; ";

            using (_sqlConnection)
            {
                _sqlConnection.Open();
                using (var command = new NpgsqlCommand(sql, _sqlConnection))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            Session session = new Session
                            {
                                Pid = dataReader["pid"].ToString(),
                                Application_name = dataReader["application_name"].ToString(),
                                Starttime = DateTime.ParseExact(dataReader["starttime"].ToString(), "dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                                Usename = dataReader["usename"].ToString(),
                                Descript = dataReader["descript"].ToString(),
                                Client_addr = dataReader["client_addr"].ToString(),
                                Now = DateTime.ParseExact(dataReader["now"].ToString(), "dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)
                            };

                            listOfSessions.Add(session);
                        }
                    }
                }
            }
            return listOfSessions;
        }
    }
}
