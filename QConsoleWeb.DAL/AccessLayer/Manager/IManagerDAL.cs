using QConsoleWeb.DAL.AccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QConsoleWeb.DAL.AccessLayer.Manager
{
    public interface IManagerDAL
    {
        ISessionDAO SessionAccess { get; }
        IUserDAO UserAccess { get; }
        ILayerDAO LayerAccess { get; }
        IGrantDAO GrantAccess { get; }
        IQueryDAO QueryAccess { get; }
        ILoggerDAO LoggerAccess { get; }
    }
}
