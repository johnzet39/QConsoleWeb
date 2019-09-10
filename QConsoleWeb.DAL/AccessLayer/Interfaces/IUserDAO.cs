using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QConsoleWeb.DAL.AccessLayer.Entities;

namespace QConsoleWeb.DAL.AccessLayer.Interfaces
{
    public interface IUserDAO
    {
        //all users
        IEnumerable<User> GetUsers();

        //assigned roles for user
        DataTable GetAssignedRoles(string oid);

        //available roles for user
        DataTable GetAvailableRoles(string oid);

        //grant available role
        void GrantRole(string userName, string roleName);

        //revoke assigned role
        void RevokeRole(string userName, string roleName);

        //remove role/user
        void RemoveRoleOrUser(string userName);

        //create new role/user
        void CreateUserOrRole(string userName, string passWord, string definition);

        //edit role/user
        void EditUserOrRole(string userName, string passWord, string definition);
    }
}
