using QConsoleWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QConsoleWeb.Views.ViewModels
{
    public class UserViewModel
    {
        public IEnumerable<User> AssignedRoles { get; set; }
        public IEnumerable<User> Roles { get; set; }
        public User CurrentUser { get; set; }
    }
}
