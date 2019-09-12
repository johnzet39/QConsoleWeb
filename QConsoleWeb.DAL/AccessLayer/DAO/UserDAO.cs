using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using QConsoleWeb.DAL.AccessLayer.Entities;
using QConsoleWeb.DAL.AccessLayer.Interfaces;

namespace QConsoleWeb.DAL.AccessLayer.DAO
{
    // queries for create/edit users
    internal static class UserQueries
    {
        public static string CreateRole(string username)
        {
            return String.Format("CREATE ROLE {0};", username);
        }

        public static string CreateRole(string username, string pass)
        {
            return String.Format("CREATE ROLE {0} WITH LOGIN PASSWORD '{1}';", username, pass);
        }

        public static string GrantRole(string username, string role)
        {
            return String.Format("GRANT {0} TO {1};", role, username);
        }

        public static string RevokeRole(string username, string role)
        {
            return String.Format("REVOKE {0} FROM {1};", role, username);
        }

        public static string CommentOnRole(string username, string definition)
        {
            return String.Format("COMMENT ON ROLE {0} IS '{1}';", username, definition);
        }

        public static string AlterRolePassword(string username, string pass)
        {
            return String.Format("ALTER ROLE {0} WITH LOGIN PASSWORD '{1}';", username, pass);
        }

        public static string DropRole(string username)
        {
            return String.Format("DROP ROLE {0};", username);
        }

    }


    internal class UserDAO : IUserDAO
    {
        private string _connectionString;

        public UserDAO(string connstring)
        {
            _connectionString = connstring;
        }

        //all users
        public IEnumerable<User> GetUsers()
        {
            var listOfUsers = new List<User>();
            string sql_query = "SELECT pu.rolname as usename, (select shobj_description(pu.oid, 'pg_authid')) as descript, pu.rolsuper as usesuper, " +
                               " CASE WHEN EXISTS(SELECT 1 from pg_user p where p.usesysid = pu.oid) THEN 0 else 1 END as isrole, pu.oid as usesysid " +
                               " FROM pg_roles pu WHERE pu.rolname not in ('pg_signal_backend') ORDER BY isrole desc, pu.rolname; ";
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var command = new NpgsqlCommand(sql_query, conn))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var userpsql = new User();

                            userpsql.Usename = dataReader["usename"].ToString();
                            userpsql.Descript = dataReader["descript"].ToString();
                            userpsql.Usesuper = Convert.ToBoolean(dataReader["usesuper"]);
                            userpsql.Isrole = Convert.ToBoolean(dataReader["isrole"]);
                            userpsql.Usesysid = dataReader["usesysid"].ToString();

                            listOfUsers.Add(userpsql);
                        }
                    }
                }
            }
            return listOfUsers;  
        }

        //assigned roles for user
        public DataTable GetAssignedRoles(string oid)
        {
            string sql_query = "SELECT pr.rolname, (select shobj_description(pr.oid, 'pg_authid')) as descript, am.admin_option, pr.oid " +
                               " from pg_auth_members am LEFT JOIN pg_roles pr ON pr.oid = am.roleid WHERE am.member = " + oid;
            return GetQueryResult(sql_query);
        }

        public IEnumerable<User> GetAssignedRolesObject(string oid)
        {
            var listOfUsers = new List<User>();
            string sql_query = "SELECT pr.rolname, (select shobj_description(pr.oid, 'pg_authid')) as descript, am.admin_option, pr.oid " +
                               " from pg_auth_members am LEFT JOIN pg_roles pr ON pr.oid = am.roleid WHERE am.member = " + oid;
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var command = new NpgsqlCommand(sql_query, conn))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var userpsql = new User();

                            userpsql.Usename = dataReader["rolname"].ToString();
                            userpsql.Descript = dataReader["descript"].ToString();
                            userpsql.Usesuper = Convert.ToBoolean(dataReader["admin_option"]);
                            userpsql.Usesysid = dataReader["oid"].ToString();

                            listOfUsers.Add(userpsql);
                        }
                    }
                }
            }
            return listOfUsers;
        }

        //available roles for user
        public DataTable GetAvailableRoles(string oid)
        {
            string sql_query = "select pg.groname, (select shobj_description(pg.grosysid, 'pg_authid')) as descript, pg.grosysid , pg.grolist " +
                               " from pg_group pg WHERE pg.groname not in ('pg_signal_backend') AND  pg.grosysid <> " + oid + " AND pg.grosysid not in  (select pr.oid from pg_auth_members am " +
                               " LEFT JOIN pg_roles pr ON pr.oid = am.roleid WHERE am.member = " + oid + ")";
            return GetQueryResult(sql_query);
        }

        public IEnumerable<User> GetAvailableRolesObject(string oid)
        {
            var listOfUsers = new List<User>();
            string sql_query = "select pg.groname, (select shobj_description(pg.grosysid, 'pg_authid')) as descript, pg.grosysid , pg.grolist " +
                               " from pg_group pg WHERE pg.groname not in ('pg_signal_backend') AND  pg.grosysid <> " + oid + " AND pg.grosysid not in  (select pr.oid from pg_auth_members am " +
                               " LEFT JOIN pg_roles pr ON pr.oid = am.roleid WHERE am.member = " + oid + ")";
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var command = new NpgsqlCommand(sql_query, conn))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var userpsql = new User();

                            userpsql.Usename = dataReader["groname"].ToString();
                            userpsql.Descript = dataReader["descript"].ToString();
                            userpsql.Usesysid = dataReader["grosysid"].ToString();

                            listOfUsers.Add(userpsql);
                        }
                    }
                }
            }
            return listOfUsers;
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
                    var dataReader = command.ExecuteReader();
                    dataTable.Load(dataReader);
                    dataReader.Close();
                }
            }
            return dataTable;

        }

        //grant available role
        public void GrantRole(string userName, string roleName)
        {
            string sql_query = UserQueries.GrantRole(userName, roleName);
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var command = new NpgsqlCommand(sql_query, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        //revoke assigned role
        public void RevokeRole(string userName, string roleName)
        {
            string sql_query = UserQueries.RevokeRole(userName, roleName);
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var command = new NpgsqlCommand(sql_query, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        //create new role/user
        public void CreateUserOrRole(string Username, string Password, string Definition)
        {
            List<String> sql_queries = new List<String>();

            if (Password != null)
            {
                sql_queries.Add(UserQueries.CreateRole(Username, Password));
            }
            else
            {
                sql_queries.Add(UserQueries.CreateRole(Username));
            }
            if (Definition?.Length > 0)
            {
                sql_queries.Add(UserQueries.CommentOnRole(Username, Definition));
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

        //edit new role/user
        public void EditUserOrRole(string Username, string Password, string Definition)
        {
            List<String> sql_queries = new List<String>();

            if (!(Password is null) && Password.Length != 0) sql_queries.Add(UserQueries.AlterRolePassword(Username, Password));
            if (Definition != null) sql_queries.Add(UserQueries.CommentOnRole(Username, Definition));


            try
            {
                ExecuteSqlNonQuery(sql_queries);
            }
            catch
            {
                throw;
            }
        }

        //remove role/user
        public void RemoveRoleOrUser(string Username)
        {
            string sql_query = UserQueries.DropRole(Username);
            try
            {
                ExecuteSqlNonQuery(new List<string> { sql_query });
            }
            catch
            {
                throw;
            }
        }

        //Execute queries
        private void ExecuteSqlNonQuery(List<string> sql_queries)
        {
            //string cmdres = null;
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





    }
}
