using QConsoleWeb.BLL.DTO;
using QConsoleWeb.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace QConsoleWeb.Data
{
    public class FakeUserService : IUserService
    {
        static List<UserDTO> ListDTO;
        static  FakeUserService()
        {
            ListDTO = new List<UserDTO>
           {
               new UserDTO
               {
                   Usename = "User1",
                   Descript = "User1",
                   Isrole = false,
                   Usesuper = false,
                   Usesysid = "1"
               },
               new UserDTO
               {
                   Usename = "User2",
                   Descript = "User2",
                   Isrole = false,
                   Usesuper = false,
                   Usesysid = "2"
               },
               new UserDTO
               {
                   Usename = "User3",
                   Descript = "User3",
                   Isrole = false,
                   Usesuper = false,
                   Usesysid = "3"
               },
               new UserDTO
               {
                   Usename = "User4",
                   Descript = "User4",
                   Isrole = false,
                   Usesuper = true,
                   Usesysid = "4"
               },
               new UserDTO
               {
                   Usename = "Role5",
                   Descript = "Role1",
                   Isrole = true,
                   Usesuper = false,
                   Usesysid = "5"
               },
               new UserDTO
               {
                   Usename = "Role6",
                   Descript = "Role6",
                   Isrole = true,
                   Usesuper = false,
                   Usesysid = "6"
               },
           };
        }

        public void CreateUserOrRole(string userName, string passWord, string definition)
        {
            UserDTO us = new UserDTO
            {
                Usename = userName,
                Descript = definition,
                Isrole = (((passWord == null) || (passWord.Length == 0)) ? true : false),
                Usesuper = false,
                Usesysid = Guid.NewGuid().ToString()
            };
            ListDTO.Add(us);
        }

        public void EditUserOrRole(string userName, string passWord, string definition)
        {
            List<UserDTO> temp = new List<UserDTO>();
            temp = ListDTO.ToList();

            int index = -1;
            index = temp.FindIndex(r => r.Usename == userName);
            if (index >= 0)
            {
                temp[index] = new UserDTO
                {
                    Usename = userName,
                    Descript = definition
                };
            }
            ListDTO = temp;
        }

        public DataTable GetAssignedRoles(string oid)
        {
            throw new NotImplementedException();
        }

        public DataTable GetAvailableRoles(string oid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserDTO> GetUsers()
        {
            return ListDTO;
        }

        public void GrantRole(string userName, string roleName)
        {
            throw new NotImplementedException();
        }

        public void RemoveRoleOrUser(string userName)
        {
            List<UserDTO> temp = new List<UserDTO>();
            temp = ListDTO.ToList();

            var itemToRemove = ListDTO.SingleOrDefault(r => r.Usename == userName);
            if (itemToRemove != null)
                temp.Remove(itemToRemove);

            ListDTO = temp;
        }

        public void RevokeRole(string userName, string roleName)
        {
            throw new NotImplementedException();
        }
    }
}
