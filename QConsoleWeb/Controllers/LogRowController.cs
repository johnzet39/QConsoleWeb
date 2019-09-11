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

namespace QConsoleWeb.Controllers
{
    public class LogRowController : Controller
    {
        private readonly ILoggerService _service;
        private readonly IConfiguration _config;

        //private DateTime? _dateFrom;
        //private DateTime? _dateTo;
        //private string _subQuery;
        private LogRowViewModel _model;


        public LogRowController(ILoggerService serv, IConfiguration config)
        {
            _config = config;
            _service = serv;
            _service.LastRowsCount = _config.GetValue<int>("AppSettings:LoggerTab:LastRowsCount");
            _model = new LogRowViewModel();
        }

        [HttpGet]
        public ViewResult List(int page = 1)
        {
            ViewBag.Title = "Логгер";
            _model.LogRows = GetLogRows();
            _model.PagedLogRows = GetLogRows().GetPaged(page, 100);
            return View(_model);
        }

        [HttpPost]
        public IActionResult List(LogRowViewModel model)
        {
            if (model.DateFrom == null & model.DateTo == null & model.SubQuery == null)
                return RedirectToAction("List");
            if (ModelState.IsValid)
            {
                _model.DateFrom = model.DateFrom;
                _model.DateTo = model.DateTo;
                _model.SubQuery = model.SubQuery;
                try
                {
                    _model.LogRows = GetLogRows();
                    return View(_model);
                }
                catch (Exception e)
                {
                    TempData["error"] = $"{e.Message}";
                    return RedirectToAction("List");
                }
            }
            else
            {
                return RedirectToAction("List");
            }
        }

        public IActionResult LogRowHistory(string gid)
        {
            ViewBag.Title = "История изменений";
            LogRowHistoryViewModel hismodel = new LogRowHistoryViewModel();
            var logrows = GetLogRows(onlyLastRows:false);
            hismodel.CurrentLogRow = logrows.FirstOrDefault(g => g.Gid == gid);
            var cur = hismodel.CurrentLogRow;
            hismodel.HitoriedLogRows = logrows
                                        .Where(h => h.Gidnum == cur.Gidnum && h.Gid != cur.Gid)
                                        .Where(h => h.Tableschema == cur.Tableschema && h.Tablename == cur.Tablename);
            return View(hismodel);
        }


        private IEnumerable<LogRow> GetLogRows(bool onlyLastRows = true)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<LogRowDTO, LogRow>()).CreateMapper();
            return mapper.Map<IEnumerable<LogRowDTO>, List<LogRow>>(_service.GetLogList(_model.DateFrom, _model.DateTo, _model.SubQuery, onlyLastRows));
        }
    }
}
