using Microsoft.AspNetCore.Mvc;
using QConsoleWeb.BLL.Interfaces;
using QConsoleWeb.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using QConsoleWeb.Models;
using QConsoleWeb.Models.ViewModels;

namespace QConsoleWeb.Controllers
{
    public class LogRowController : Controller
    {
        private ILoggerService _service;

        public LogRowController(ILoggerService serv)
        {
            _service = serv;
        }


        public ViewResult List()
        {
            ViewBag.Title = "Логгер";
            var logrows = GetLogRows();
            return View(logrows);
        }

        private IEnumerable<LogRow> GetLogRows()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<LogRowDTO, LogRow>()).CreateMapper();
            return mapper.Map<IEnumerable<LogRowDTO>, List<LogRow>>(_service.GetLogList(null, null, null, true));
        }
    }
}
