using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Windows;
using System.Xml.Serialization;

namespace QConsoleWeb.DAL.Entities
{
    [Serializable]
    public class ConnectionBaseDAL
    {
        [XmlElement("Host")]
        public string Host { get; set ; }

        [XmlElement("Port")]
        public string Port { get; set; }

        [XmlElement("Username")]
        public string Username { get; set; }

        [XmlElement("Database")]
        public string Database { get; set; }

        [XmlAttribute("ConnName")]
        public string ConnName { get; set; }

        [XmlIgnore]
        public string Password { get; set; }

        [XmlIgnore]
        public string ApplicationName { get; set; }

        public void SetConnectionFromString(string connString, string connName) //create ConnectionBase from connection string
        {
            ConnName = connName.Trim();
            NpgsqlConnectionStringBuilder sb = new NpgsqlConnectionStringBuilder(connString);
            Host = sb["Host"].ToString().Trim();
            Port = sb["Port"].ToString().Trim();
            Username = sb["User Id"].ToString().Trim();
            Database = sb["DataBase"].ToString().Trim();
            ApplicationName = sb["ApplicationName"].ToString().Trim();
        }

        public string BuildConnectionString() //return ConnectionString from ConnectionBase
        {
            NpgsqlConnectionStringBuilder sb = new NpgsqlConnectionStringBuilder();
            sb.Host = Host;
            sb.Port = Convert.ToInt32(Port);
            sb.Username = Username;
            sb.Database = Database;
            sb.Password = Password;
            sb.ApplicationName = ApplicationName;
            NpgsqlConnection conn = new NpgsqlConnection(sb.ConnectionString);
            return conn.ConnectionString;
        }

        public Exception TestConnectionAccess(string _connectionString)// test connection
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    return null;
                }
            }
            catch(Exception e)
            {
                return e;
            }
        }
    }

    public class CollectionSettingsDAL
    {
        [XmlArray("CollectionConnectionBase"), XmlArrayItem("Item")]
        public List<ConnectionBaseDAL> Collection { get; set; }
    }
}
