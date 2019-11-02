using QConsoleWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QConsoleWeb.Views.ViewModels
{
    public class GrantColumnsViewModel
    {
        public List<GrantColumn> Columns { get; set; }
        public string Rolename { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
    }
}
