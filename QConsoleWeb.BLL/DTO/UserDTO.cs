using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QConsoleWeb.BLL.DTO
{
    public class UserDTO
    {
        public string Usename { get; set; }
        public string Descript { get; set; }
        public Boolean Usesuper { get; set; }
        public Boolean Isrole { get; set; }
        public string Usesysid { get; set; }
    }
}
