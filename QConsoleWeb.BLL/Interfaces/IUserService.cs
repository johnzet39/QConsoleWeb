using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QConsoleWeb.BLL.DTO;

namespace QConsoleWeb.BLL.Interfaces
{
    public interface IUserService
    {
        IEnumerable<UserDTO> GetUsers();
        DataTable GetAssignedRoles(string oid);
        IEnumerable<UserDTO> GetAssignedRolesObject(string oid);
        DataTable GetAvailableRoles(string oid);
        IEnumerable<UserDTO> GetAvailableRolesObject(string oid);
        void GrantRole(string userName, string roleName);
        void RevokeRole(string userName, string roleName);
        void RemoveRoleOrUser(string userName);
        void CreateUserOrRole(string userName, string passWord, string definition);
        void EditUserOrRole(string userName, string passWord, string definition);
    }
}
