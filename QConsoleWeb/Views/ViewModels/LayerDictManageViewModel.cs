using QConsoleWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QConsoleWeb.Views.ViewModels
{
    public class LayerDictManageViewModel
    {
        public List<Dict> DictList { get; set; }
        [Display(Name = "Схема")]
        [Required]
        public string SchemaName { get; set; }
        [Display(Name = "Таблица")]
        [Required]
        public string TableName { get; set; }
    }
}
