using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QConsoleWeb.DAL.AccessLayer.Entities
{
    public class Layer
    {
        public string Table_schema { get; set; }
        public string Table_name { get; set; }
        public string Descript { get; set; }
        public string Geomtype { get; set; }
        public Boolean Isupdater { get; set; }
        public Boolean Islogger { get; set; }
    }

    public class InformationSchemaTable
    {
        public string Table_schema { get; set; }
        public string Table_name { get; set; }
        public string Table_type { get; set; }
    }

    public class LayerGrants
    {
        public string schemaname { get; set; }
        public string tablename { get; set; }
        public string groname { get; set; }
        public bool isselect { get; set; }
        public bool isinsert { get; set; }
        public bool isupdate { get; set; }
        public bool isdelete { get; set; }
        public string columns_select { get; set; }
        public string columns_update { get; set; }
        public string columns_insert { get; set; }
    }
}
