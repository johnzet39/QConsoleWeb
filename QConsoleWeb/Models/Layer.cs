using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QConsoleWeb.Models
{

    public class Layer
    {
        [Display(Name = "Наименование схемы")]
        public string Table_schema { get; set; }
        [Display(Name = "Наименование таблицы")]
        public string Table_name { get; set; }
        [Display(Name = "Описание таблицы")]
        public string Descript { get; set; }
        [Display(Name = "Тип геометрии")]
        public string Geomtype { get; set; }
        [Display(Name = "Фиксация изменений")]
        public Boolean Isupdater { get; set; }
        [Display(Name = "Аудит")]
        public Boolean Islogger { get; set; }
    }

    public class InformationSchemaTable
    {
        [Display(Name = "Наименование схемы")]
        public string Table_schema { get; set; }
        [Display(Name = "Наименование таблицы")]
        public string Table_name { get; set; }
        [Display(Name = "Тип")]
        public string Table_type { get; set; }
    }
}
