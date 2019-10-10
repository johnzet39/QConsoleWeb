using Microsoft.AspNetCore.Mvc;
using QConsoleWeb.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QConsoleWeb.Controllers
{
    public class StatController : Controller
    {
        private readonly ILoggerService _service;

        public StatController(ILoggerService serv)
        {
            _service = serv;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
