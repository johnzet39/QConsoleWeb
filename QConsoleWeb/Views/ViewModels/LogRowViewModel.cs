using QConsoleWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QConsoleWeb.Views.ViewModels
{
    public class LogRowViewModel
    {
        public IEnumerable<LogRow> LogRows;

        [Display(Name = "Дата с")]
        public DateTime? DateFrom { get; set; }

        [Display(Name = "по")]
        public DateTime? DateTo { get; set; }

        [Display(Name = "Дополнительные условия")]
        public string SubQuery { get; set; }

    }
}
