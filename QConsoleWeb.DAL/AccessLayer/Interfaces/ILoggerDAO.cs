using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QConsoleWeb.DAL.AccessLayer.Entities;

namespace QConsoleWeb.DAL.AccessLayer.Interfaces
{
    public interface ILoggerDAO
    {
        //LOG LIST main
        List<LogRow> GetLogList(string ExtraQueryFull, string FirstRowsQuery);
        //build date string subquery
        string BuildExtraDateString(DateTime? dateFrom, DateTime? dateTo);
        //build 1000rows string subquery
        string BuildExtraFirstRowsString(int countRows);
        //union extra subquery strings
        string UnionExtraStrings(IList<string> str);
        //get column list for combobox
        List<string> GetColumnsList();
    }
}
