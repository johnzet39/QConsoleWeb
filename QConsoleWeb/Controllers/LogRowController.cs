using Microsoft.AspNetCore.Mvc;
using QConsoleWeb.BLL.Interfaces;
using QConsoleWeb.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using QConsoleWeb.Models;
using QConsoleWeb.Views.ViewModels;
using Microsoft.Extensions.Configuration;
using QConsoleWeb.Components.Paging;
using Microsoft.AspNetCore.Authorization;

namespace QConsoleWeb.Controllers
{
    [Authorize]
    public class LogRowController : Controller
    {
        private readonly ILoggerService _service;
        private readonly IConfiguration _config;

        private int pageRowCount;


        public LogRowController(ILoggerService serv, IConfiguration config)
        {
            _config = config;
            _service = serv;
            _service.LastRowsCount = _config.GetValue<int>("AppSettings:LoggerTab:LastRowsCount");
            pageRowCount = _config.GetValue<int>("AppSettings:LoggerTab:PageRowCount");
        }

        [HttpGet]
        public IActionResult Index(LogRowViewModel modelview = null, int page = 1)
        {
            if (!ModelState.IsValid)
            {
                string errors = string.Join("; ",
                    ModelState.Values.Where(E => E.Errors.Count > 0)
                    .SelectMany(E => E.Errors)
                    .Select(E => E.ErrorMessage)
                    .ToArray());
                return BadRequest(errors);
            }

            LogRowViewModel model;
            if (modelview == null)
                model = new LogRowViewModel();
            else
                model = modelview;

            if (model.PageRowCount > 0)
                pageRowCount = model.PageRowCount;
            else
                model.PageRowCount = pageRowCount;

            ViewBag.Title = "Аудит";
            model.PagedLogRows = GetLogRows(model, model.OnlyLastRows).GetPaged(page, pageRowCount);
            return View(model);
        }

        public IActionResult LogRowHistory(string id)
        {
            LogRowViewModel model = new LogRowViewModel();
            ViewBag.Title = "История изменений";
            LogRowHistoryViewModel hismodel = new LogRowHistoryViewModel();
            var logrows = GetLogRows(model, onlyLastRows:false);
            hismodel.CurrentLogRow = logrows.FirstOrDefault(g => g.Gid == id);
            var cur = hismodel.CurrentLogRow;
            hismodel.HitoriedLogRows = logrows
                                        .Where(h => h.Gidnum == cur.Gidnum && h.Gid != cur.Gid)
                                        .Where(h => h.Tableschema == cur.Tableschema && h.Tablename == cur.Tablename);
            //return View(hismodel);
            return PartialView(hismodel);
        }


        private IEnumerable<LogRow> GetLogRows(LogRowViewModel model, bool onlyLastRows = true)
        {
            try
            {
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<LogRowDTO, LogRow>()).CreateMapper();
                return mapper.Map<IEnumerable<LogRowDTO>, List<LogRow>>(_service.GetLogList(model.DateFrom, model.DateTo, model.SubQuery, onlyLastRows));
            }
            catch (Exception e)
            {
                ModelState.AddModelError("SubQuery", e.Message);
                return Enumerable.Empty<LogRow>();
            }
            
        }
    }
}
