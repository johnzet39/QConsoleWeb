using Microsoft.AspNetCore.Mvc;
using QConsoleWeb.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using QConsoleWeb.BLL.DTO;
using QConsoleWeb.Models;
using Microsoft.AspNetCore.Authorization;

namespace QConsoleWeb.Controllers
{
    [Authorize]
    public class SessionController : Controller
    {
        private ISessionService _service;

        public SessionController(ISessionService serv)
        {
            _service = serv;
        }

        public ViewResult Index()
        {
            ViewBag.Title = "Сессии";
            return View();
        }

        public IActionResult GetSessionsList()
        {
            var sessions = GetSessions();
            return PartialView("SessionsList", sessions);
        }

        private object GetSessions()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<SessionDTO, Session>()).CreateMapper();
            return mapper.Map<IEnumerable<SessionDTO>, List<Session>>(_service.GetSessions());
        }
    }
    
}
