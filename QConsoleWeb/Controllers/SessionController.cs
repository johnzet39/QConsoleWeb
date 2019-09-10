using Microsoft.AspNetCore.Mvc;
using QConsoleWeb.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using QConsoleWeb.BLL.DTO;
using QConsoleWeb.Models;

namespace QConsoleWeb.Controllers
{
    public class SessionController : Controller
    {
        private ISessionService _service;

        public SessionController(ISessionService serv)
        {
            _service = serv;
        }

        public ViewResult List()
        {
            var sessions = GetSessions();

            return View(sessions);
        }

        private object GetSessions()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<SessionDTO, Session>()).CreateMapper();
            return mapper.Map<IEnumerable<SessionDTO>, List<Session>>(_service.GetSessions());
        }
    }
    
}
