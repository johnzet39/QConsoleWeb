using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QConsoleWeb.BLL.DTO
{
    public class LogRowDTO
    {
        public string Gid { get; set; }
        public string Action { get; set; }
        public string Username { get; set; }
        public string Address { get; set; }
        public DateTime Timechange { get; set; }
        public string Tablename { get; set; }
        public string Tableschema { get; set; }
        public string Gidnum { get; set; }
        public string Context { get; set; }
    }
}
