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
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace QConsoleWeb.Controllers
{
    [Authorize]
    public class LayerController : Controller
    {
        private ILayerService _service;

        public LayerController(ILayerService serv)
        {
            _service = serv;
        }

        public IActionResult Index()
        {
            ViewBag.Title = "Слои и справочники";
            return View();
        }

        public IActionResult GetLayersPartial()
        {
            LayerViewModel model = new LayerViewModel();
            model.Layers = GetLayers();
            return PartialView("LayerSummaryPartial", model.Layers);
        }

        public IActionResult GetDictsPartial()
        {
            LayerViewModel model = new LayerViewModel();
            model.Dictionaries = GetDictionaries();
            return PartialView("DictSummaryPartial", model.Dictionaries);
        }

        [HttpGet]
        public IActionResult EditLayer(string schemaname, 
                                    string tablename, 
                                    string geomtype)
        {
            LayerEditViewModel vm = new LayerEditViewModel();
            try
            {
                if (geomtype == null)
                    vm.CurrentLayer = GetDictionaries()
                        .First(d => d.Table_schema == schemaname && d.Table_name == tablename);
                else
                    vm.CurrentLayer = GetLayers()
                        .First(d => d.Table_schema == schemaname && d.Table_name == tablename);

                //vm.LayerGrantsList = GetGrantsToLayer(schemaname, tablename);
            }
            catch(Exception e)
            {
                ModelState.AddModelError("errormessage", e.Message);
            }
            return PartialView(vm);
        }

        public IActionResult GetGrantsToLayerPartial(string schemaname,
                                                     string tablename)
        {
            var grants = GetGrantsToLayer(schemaname, tablename);
            return PartialView(grants);
        }

        [HttpPost]
        public JsonResult EditLayer(LayerEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Layer layer = model.CurrentLayer;
                Layer olrlayer;
                if (model.CurrentLayer.Geomtype == null)
                    olrlayer = GetDictionaries()
                        .FirstOrDefault(d => d.Table_schema == layer.Table_schema && d.Table_name == layer.Table_name);
                else
                    olrlayer = GetLayers()
                        .FirstOrDefault(d => d.Table_schema == layer.Table_schema && d.Table_name == layer.Table_name);

                bool? isupdaterCompare = null;
                if (layer.Isupdater != olrlayer.Isupdater)
                    isupdaterCompare = layer.Isupdater;
                bool? isloggerCompare = null;
                if (layer.Islogger != olrlayer.Islogger)
                    isloggerCompare = layer.Islogger;

                string descript = null;
                if (layer.Descript != olrlayer.Descript)
                    if (layer.Descript == null)
                        descript = "";
                    else
                        descript = layer.Descript;

                try
                {
                    _service.ChangeLayer(
                        layer.Table_schema,
                        layer.Table_name,
                        descript,
                        isupdaterCompare,
                        isloggerCompare
                        );
                }
                catch(Exception e)
                {
                    return Json(new { error = e.Message });
                }
            }
            else
            {
                string messages = string.Join("; ", ViewData.ModelState.Values
                    .SelectMany(x => x.Errors)
                            .Select(x => x.ErrorMessage));
                return Json(new { error = messages });
            }
            return Json(new { status = "ok" });
        }

        public ViewResult DictListManage()
        {
            var model = new LayerDictManageViewModel();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<DictionaryDTO, Dict>()).CreateMapper();
            var orderlist = mapper.Map<IEnumerable<DictionaryDTO>, List<Dict>>(_service.GetListOfDictionaries())
                .OrderBy(x => x.Schema_name)
                .ThenBy(x => x.Table_name);
            model.DictList = orderlist.ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult AddDictManage(LayerDictManageViewModel model)
        {
            if (ModelState.IsValid)
            {
                _service.AddTableToDictionaries(model.SchemaName, model.TableName);
                return RedirectToAction(nameof(DictListManage));
            }
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<DictionaryDTO, Dict>()).CreateMapper();
            model.DictList = mapper.Map<IEnumerable<DictionaryDTO>, List<Dict>>(_service.GetListOfDictionaries())
                .OrderBy(x => x.Schema_name)
                .ThenBy(x => x.Table_name)
                .ToList();
            return View("DictListManage", model);
            
        }

        public IActionResult RemoveDictManage(int id)
        {
            try
            {
                _service.RemoveTableFromDictionaries(id);
            }
            catch (Exception e)
            {
                TempData["error"] = $"Warning: {e.Message}";
            }
            return RedirectToAction(nameof(DictListManage));
        }


        private IEnumerable<Layer> GetLayers()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<LayerDTO, Layer>()).CreateMapper();
            return mapper.Map<IEnumerable<LayerDTO>, List<Layer>>(_service.GetLayers());
        }

        private IEnumerable<LayerGrants> GetGrantsToLayer(string schemaname, string tablename)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<LayerGrantsDTO, LayerGrants>()).CreateMapper();
            return mapper.Map<IEnumerable<LayerGrantsDTO>, List<LayerGrants>>(_service.GetGrantsToLayer(schemaname, tablename));
        }

        private IEnumerable<Layer> GetDictionaries()
        {
            try
            {
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<LayerDTO, Layer>()).CreateMapper();
                return mapper.Map<IEnumerable<LayerDTO>, List<Layer>>(_service.GetDicts());
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return null;
            }
}
    }
}
