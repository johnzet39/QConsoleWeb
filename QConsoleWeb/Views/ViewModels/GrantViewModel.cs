using QConsoleWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QConsoleWeb.Views.ViewModels
{
    public class GrantViewModel
    {
        public List<User> UserList { get; set; }
        public List<Grant> LayersList { get; set; }
        public List<Grant> DictsList { get; set; }
    }

}
