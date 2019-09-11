using QConsoleWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QConsoleWeb.Views.ViewModels
{
    public class UserViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<User> AssignedRoles { get; set; }
        public IEnumerable<User> AvailableRoles { get; set; }
        public User CurrentUser { get; set; }
    }
}
