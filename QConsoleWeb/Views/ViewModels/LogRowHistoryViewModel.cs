using QConsoleWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QConsoleWeb.Views.ViewModels
{
    public class LogRowHistoryViewModel
    {
        public LogRow CurrentLogRow { get; set; }
        public IEnumerable<LogRow> HitoriedLogRows { get; set; }


    }
}
