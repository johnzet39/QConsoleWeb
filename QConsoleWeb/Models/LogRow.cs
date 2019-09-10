using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QConsoleWeb.Models
{
    public class LogRow
    {
        [Display(Name = "gid")]
        public string Gid { get; set; }

        [Display(Name = "Операция")]
        public string Action { get; set; }

        [Display(Name = "Пользователь")]
        public string Username { get; set; }

        [Display(Name = "Адрес")]
        public string Address { get; set; }

        [Display(Name = "Дата изменения")]
        public DateTime Timechange { get; set; }

        [Display(Name = "Таблица")]
        public string Tablename { get; set; }

        [Display(Name = "Схема")]
        public string Tableschema { get; set; }

        [Display(Name = "gidnum")]
        public string Gidnum { get; set; }

        [Display(Name = "Содежание")]
        public string Context { get; set; }
    }
}
