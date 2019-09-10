using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QConsoleWeb.BLL.Interfaces;
using QConsoleWeb.BLL.DTO;
using QConsoleWeb.DAL.AccessLayer.Entities;
using AutoMapper;
using QConsoleWeb.DAL.AccessLayer.Manager;

namespace QConsoleWeb.BLL.Services
{
    public class SessionService : ISessionService
    {
        IManagerDAL _managerDAL;

        public SessionService(string conn)
        {
            _managerDAL = new ManagerDAL(conn);
        }

        public IEnumerable<SessionDTO> GetSessions()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Session, SessionDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Session>, List<SessionDTO>>(_managerDAL.SessionAccess.GetSessionsList());
        }
    }
}
