using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QConsoleWeb.DAL.AccessLayer.Entities;

namespace QConsoleWeb.DAL.AccessLayer.Interfaces
{
    public interface IGrantDAO
    {
        //groups
        List<User> GetGroups();
        //users in selected group
        List<User> GetUsers(string grosysid);
        //layers list for selected grantee (role)
        List<Grant> GetLayers(string grantee);
        //dicts list for selected grantee (role)
        List<Grant> GetDicts(string grantee);
        //Grant privileges to selected role
        void GrantTableToRole(string table_schema, string table_name, string role,
                              bool IsSelect, bool IsUpdate, bool IsInsert, bool IsDelete,
                              bool selChanged, bool updChanged, bool insChanged, bool delChanged);
        List<GrantColumn> GetColumns(string table_schema, string table_name, string role_name);
        void GrantColumnsToRole(string schemaName, string tableName, string rolename, IEnumerable<string> selectList, 
                                IEnumerable<string> updateList, IEnumerable<string> insertList,
                                bool selChanged, bool updChanged, bool insChanged);
    }
}
