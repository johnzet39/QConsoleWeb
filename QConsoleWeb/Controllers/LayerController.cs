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
            LayerViewModel model = new LayerViewModel();
            model.Layers = GetLayers();
            model.Dictionaries = GetDictionaries();
            return View(model);
        }

        [HttpGet]
        public ViewResult EditLayer(string schemaname, 
                                    string tablename, 
                                    string geomtype)
        {
            Layer layer;
            if (geomtype == null)
                layer = GetDictionaries()
                    .First(d => d.Table_schema == schemaname && d.Table_name == tablename);
            else
                layer = GetLayers()
                    .First(d => d.Table_schema == schemaname && d.Table_name == tablename);
            ViewBag.Title = "Редактирование параметров таблицы";
            return View(layer);
        }

        [HttpPost]
        public IActionResult EditLayer(Layer layer, string geomtype)
        {
            if (ModelState.IsValid)
            {
                Layer olrlayer;
                if (geomtype == null)
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

                try
                {
                    _service.ChangeLayer(
                        layer.Table_schema,
                        layer.Table_name,
                        (layer.Descript != olrlayer.Descript) ? layer.Descript : null,
                        isupdaterCompare,
                        isloggerCompare
                        );
                    TempData["message"] = $"Изменения в {layer.Table_schema}.{layer.Table_name} сохранены.";
                }
                catch(Exception e)
                {
                    TempData["error"] = $"Warning: {e.Message}";
                }

                return RedirectToAction("Index");
            }
            return View(layer);
        }

        private IEnumerable<Layer> GetLayers()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<LayerDTO, Layer>()).CreateMapper();
            return mapper.Map<IEnumerable<LayerDTO>, List<Layer>>(_service.GetLayers());
        }

        private IEnumerable<Layer> GetDictionaries()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<LayerDTO, Layer>()).CreateMapper();
            return mapper.Map<IEnumerable<LayerDTO>, List<Layer>>(_service.GetDicts());
        }
    }
}
