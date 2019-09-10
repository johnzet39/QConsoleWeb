using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QConsoleWeb.DAL.AccessLayer.Entities
{
    public class Session
    {
        public string Pid { get; set; }
        public string Application_name { get; set; }
        public DateTime Starttime { get; set; }
        public string Usename { get; set; }
        public string Descript { get; set; }
        public string Client_addr { get; set; }
        public DateTime Now { get; set; }
    }
}
