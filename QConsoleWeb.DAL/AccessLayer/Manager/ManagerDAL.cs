using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QConsoleWeb.DAL.AccessLayer.DAO;
using QConsoleWeb.DAL.AccessLayer.Interfaces;

namespace QConsoleWeb.DAL.AccessLayer.Manager
{
    public class ManagerDAL : IManagerDAL
    {
        private string _conn;
        public ManagerDAL(string conn)
        {
            _conn = conn;
        }


        ISessionDAO _sessionAccess;
        public ISessionDAO SessionAccess
        {
            get
            {
                if (_sessionAccess == null)
                    _sessionAccess = new SessionDAO(_conn);
                return _sessionAccess;
            }
        }

        IUserDAO _userAccess;
        public IUserDAO UserAccess
        {
            get
            {
                if (_userAccess == null)
                    _userAccess = new UserDAO(_conn);
                return _userAccess;
            }
        }

        ILayerDAO _layerAccess;
        public ILayerDAO LayerAccess
        {
            get
            {
                if (_layerAccess == null)
                    _layerAccess = new LayerDAO(_conn);
                return _layerAccess;
            }
        }

        IGrantDAO _grantAccess;
        public IGrantDAO GrantAccess
        {
            get
            {
                if (_grantAccess == null)
                    _grantAccess = new GrantDAO(_conn);
                return _grantAccess;
            }
        }

        IQueryDAO _queryAccess;
        public IQueryDAO QueryAccess
        {
            get
            {
                if (_queryAccess == null)
                    _queryAccess = new QueryDAO(_conn);
                return _queryAccess;
            }
        }

        ILoggerDAO _loggerAccess;
        public ILoggerDAO LoggerAccess
        {
            get
            {
                if (_loggerAccess == null)
                    _loggerAccess = new LoggerDAO(_conn);
                return _loggerAccess;
            }
        }
    }
}
