using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QConsoleWeb.BLL.DTO;
using QConsoleWeb.BLL.Interfaces;
using QConsoleWeb.DAL.AccessLayer.Entities;
using QConsoleWeb.DAL.AccessLayer.Manager;
using AutoMapper;

namespace QConsoleWeb.BLL.Services
{
    public class GrantService : IGrantService
    {
        IManagerDAL _managerDAL;

        public GrantService(string conn)
        {
            _managerDAL = new ManagerDAL(conn);
        }


        public List<UserDTO> GetGroups()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<User>, List<UserDTO>>(_managerDAL.GrantAccess.GetGroups());
        }

        public List<GrantDTO> GetDicts(string grantee)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Grant, GrantDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Grant>, List<GrantDTO>>(_managerDAL.GrantAccess.GetDicts(grantee));
        }

        public List<GrantDTO> GetLayers(string grantee)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Grant, GrantDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Grant>, List<GrantDTO>>(_managerDAL.GrantAccess.GetLayers(grantee));
        }

        public List<UserDTO> GetUsers(string grosysid)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<User>, List<UserDTO>>(_managerDAL.GrantAccess.GetUsers(grosysid));
        }

        public void GrantTableToRole(string table_schema, string table_name, string role,
                                     bool IsSelect, bool IsUpdate, bool IsInsert, bool IsDelete,
                                     bool selChanged, bool updChanged, bool insChanged, bool delChanged)
        {
            _managerDAL.GrantAccess.GrantTableToRole(table_schema, table_name, role, 
                                                     IsSelect, IsUpdate, IsInsert, IsDelete,
                                                     selChanged, updChanged, insChanged, delChanged);
        }

        public List<GrantColumnDTO> GetColumns(string table_schema, string table_name, string role_name)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<GrantColumn, GrantColumnDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<GrantColumn>, List<GrantColumnDTO>>(_managerDAL.GrantAccess.GetColumns(table_schema, table_name, role_name));
        }

        public void GrantColumnsToRole(string schemaName, string tableName, string rolename, IEnumerable<string> selectList, 
                                       IEnumerable<string> updateList, IEnumerable<string> insertList,
                                       bool selChanged, bool updChanged, bool insChanged)
        {
            _managerDAL.GrantAccess.GrantColumnsToRole(schemaName, tableName, rolename, 
                                                        selectList, updateList, insertList, 
                                                        selChanged, updChanged, insChanged);
        }
    }
}
