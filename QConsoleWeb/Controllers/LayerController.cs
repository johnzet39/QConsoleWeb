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

namespace QConsoleWeb.Controllers
{
    public class LayerController : Controller
    {
        private ILayerService _service;

        public LayerController(ILayerService serv)
        {
            _service = serv;
        }

        public ViewResult List()
        {
            ViewBag.Title = "Слои и справочники";
            LayerViewModel model = new LayerViewModel();
            model.Layers = GetLayers();
            model.Dictionaries = GetDictionaries();
            return View(model);
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
