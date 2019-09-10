using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QConsoleWeb.BLL.DTO;
using QConsoleWeb.BLL.Interfaces;
using QConsoleWeb.DAL.AccessLayer.Entities;
using AutoMapper;
using QConsoleWeb.DAL.AccessLayer.Manager;

namespace QConsoleWeb.BLL.Services
{
    public class UserService : IUserService
    {
        IManagerDAL _managerDAL;

        public UserService(string conn)
        {
            _managerDAL = new ManagerDAL(conn);
        }

        public IEnumerable<UserDTO> GetUsers()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<User>, List<UserDTO>>(_managerDAL.UserAccess.GetUsers());
        }

        public DataTable GetAssignedRoles(string oid)
        {
            return _managerDAL.UserAccess.GetAssignedRoles(oid);
        }

        public DataTable GetAvailableRoles(string oid)
        {
            return _managerDAL.UserAccess.GetAvailableRoles(oid);
        }

        public void GrantRole(string userName, string roleName)
        {
            _managerDAL.UserAccess.GrantRole(userName, roleName);
        }

        public void RevokeRole(string userName, string roleName)
        {
            _managerDAL.UserAccess.RevokeRole(userName, roleName);
        }

        public void RemoveRoleOrUser(string userName)
        {
            _managerDAL.UserAccess.RemoveRoleOrUser(userName);
        }

        public void CreateUserOrRole(string userName, string passWord, string definition)
        {
            _managerDAL.UserAccess.CreateUserOrRole(userName, passWord, definition);
        }

        public void EditUserOrRole(string userName, string passWord, string definition)
        {
            _managerDAL.UserAccess.EditUserOrRole(userName, passWord, definition);
        }
    }
}
