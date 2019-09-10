using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QConsoleWeb.DAL.AccessLayer.Interfaces
{
    public interface IQueryDAO
    {
        DataTable ExecuteQuery(string queryString);
    }
}
