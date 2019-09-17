using QConsoleWeb.Components.Paging;
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
        public PagedResult<LogRow> PagedLogRows;

        [Display(Name = "Дата от")]
        public DateTime? DateFrom { get; set; }

        [Display(Name = "Дата до")]
        public DateTime? DateTo { get; set; }

        [Display(Name = "Дополнительные условия")]
        public string SubQuery { get; set; }

    }
}
