using QConsoleWeb.DAL.AccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QConsoleWeb.DAL.AccessLayer.Interfaces
{
    public interface ISessionDAO
    {
        IEnumerable<Session> GetSessionsList();
    }

}
