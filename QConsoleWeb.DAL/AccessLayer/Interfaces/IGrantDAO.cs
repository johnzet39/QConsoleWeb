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
        void GrantTableToRole(string table_schema, string table_name, string role, List<string> grants_list);
    }
}
