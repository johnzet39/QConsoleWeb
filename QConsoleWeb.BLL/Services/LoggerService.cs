using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QConsoleWeb.BLL.DTO;
using QConsoleWeb.BLL.Interfaces;
using QConsoleWeb.DAL.AccessLayer.Entities;
using QConsoleWeb.DAL.AccessLayer.DAO;
using QConsoleWeb.DAL.EF.EDM;
using QConsoleWeb.DAL.EF.UnitOfWork;
using AutoMapper;
using QConsoleWeb.DAL.AccessLayer.Interfaces;
using QConsoleWeb.DAL.AccessLayer.Manager;

namespace QConsoleWeb.BLL.Services
{
    public class LoggerService : ILoggerService
    {
        IManagerDAL _managerDAL;
        UnitOfWork _unitOfWork;

        public int LastRowsCount { get; set; }

        public LoggerService(string conn)
        {
            _managerDAL = new ManagerDAL(conn);
            _unitOfWork = new UnitOfWork(conn);
            LastRowsCount = 1000;
        }

        public string BuildExtraDateString(DateTime? dateFrom, DateTime? dateTo)
        {
            return _managerDAL.LoggerAccess.BuildExtraDateString(dateFrom, dateTo);
        }

        public string BuildExtraFirstRowsString()
        {
            return _managerDAL.LoggerAccess.BuildExtraFirstRowsString(LastRowsCount);
        }

        public List<string> GetColumnsList()
        {
            return _managerDAL.LoggerAccess.GetColumnsList();
        }

        public string UnionExtraStrings(IList<string> str)
        {
            return _managerDAL.LoggerAccess.UnionExtraStrings(str);
        }

        private string GetExtraStringUnion(DateTime? DateFrom, DateTime? DateTo, string ExtraQuery)
        {
            List<string> Extra_strings = new List<string>(); //list of all extra subqueries

            string extraDateString = BuildExtraDateString(DateFrom, DateTo); //Convert Dates to string subquery

            if (ExtraQuery != null && ExtraQuery.Trim().Length > 0)
                Extra_strings.Add(ExtraQuery); //add text query
            if (extraDateString != null && extraDateString.Trim().Length > 0)
                Extra_strings.Add(extraDateString); //add date

            if (Extra_strings.Count > 0)
                return UnionExtraStrings(Extra_strings); //extra subquery after union list of queries to one line

            else
                return "";
        }


        public List<LogRowDTO> GetLogList(DateTime? DateFrom, DateTime? DateTo, string extraQuery, bool onlyLastRows)
        {
            if ((extraQuery == null || extraQuery.Trim().Length == 0)  && DateFrom == null && DateTo == null)
            {
                //EF DAL
                List<Logtable> OrderedList;
                if (onlyLastRows)
                {
                    OrderedList = _unitOfWork.LogtableRepository.Get().OrderByDescending(r => r.gid).Take(LastRowsCount).ToList();
                }
                else
                {
                    OrderedList = _unitOfWork.LogtableRepository.Get().OrderByDescending(r => r.gid).ToList();
                }

                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Logtable, LogRowDTO>()).CreateMapper();
                return mapper.Map<IEnumerable<Logtable>, List<LogRowDTO>>(OrderedList);
            }
            else
            {
                //AccessLayer DAL
                string extraStringUnion = GetExtraStringUnion(DateFrom, DateTo, extraQuery);
                string onlyLastRowsString = "";
                if (onlyLastRows == true)
                    onlyLastRowsString = BuildExtraFirstRowsString();

                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<LogRow, LogRowDTO>()).CreateMapper();
                return mapper.Map<IEnumerable<LogRow>, List<LogRowDTO>>(_managerDAL.LoggerAccess.GetLogList(extraStringUnion, onlyLastRowsString));
            }
        }

        public int GetCountByOperation(string operation, int period)
        {
            DateTime date = DateTime.Now.AddDays(-period);
            var count = _unitOfWork.LogtableRepository.Get()
                .Where(o => o.action.ToUpper() == operation.ToUpper())
                .Where(o => o.timechange > date)
                .Count();
            return count;
        }

        public int GetCountInserts(string schema, string layer, int period)
        {

            int count = 0;
            DateTime date = DateTime.Now.AddDays(-period).Date;
            count = _unitOfWork.LogtableRepository.Get()
                .Where(o => o.tablename == layer)
                .Where(o => o.tableschema == schema)
                .Where(o => o.action.ToUpper() == "INSERT")
                .Where(o => o.timechange >= date)
                .Count();
            return count;
        }

        public int GetCountInsertsMonth(string schema, string layer, int month, int year)
        {
            int count = 0;
            count = _unitOfWork.LogtableRepository.Get()
                .Where(o => o.tablename == layer)
                .Where(o => o.tableschema == schema)
                .Where(o => o.action.ToUpper() == "INSERT")
                .Where(o => o.timechange.Year == year && o.timechange.Month == month)
                .Count();
            return count;
        }
    }
}
