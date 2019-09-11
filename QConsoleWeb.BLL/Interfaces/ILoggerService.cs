using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QConsoleWeb.BLL.DTO;

namespace QConsoleWeb.BLL.Interfaces
{
    public interface ILoggerService
    {
        int LastRowsCount { get; set; }
        //LOG LIST main
        List<LogRowDTO> GetLogList(DateTime? DateFrom, DateTime? DateTo, string extraQuery, bool onlyLastRows);
        //build date string subquery
        string BuildExtraDateString(DateTime? dateFrom, DateTime? dateTo);
        //build 1000rows string subquery
        string BuildExtraFirstRowsString();
        //union extra subquery strings
        string UnionExtraStrings(IList<string> str);
        //get column list for combobox
        List<string> GetColumnsList();
        //get count of rows with selected OPERATION type
        int GetCountByOperation(string operation, int period);
        //get count of inserts
        int GetCountInserts(string schema, string layer, int period);
        //get count of inserts in month
        int GetCountInsertsMonth(string schema, string layer, int month, int year);
    }
}
