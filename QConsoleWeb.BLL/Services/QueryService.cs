using QConsoleWeb.BLL.Interfaces;
using QConsoleWeb.DAL.AccessLayer.Interfaces;
using QConsoleWeb.DAL.AccessLayer.DAO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QConsoleWeb.DAL.AccessLayer.Manager;

namespace QConsoleWeb.BLL.Services
{
    public class QueryService : IQueryService
    {
        IManagerDAL _managerDAL;
        public QueryService(string conn)
        {
            _managerDAL = new ManagerDAL(conn);
        }

        public DataTable ExecuteQuery(string queryString)
        {
            return _managerDAL.QueryAccess.ExecuteQuery(queryString);
        }
    }
}
