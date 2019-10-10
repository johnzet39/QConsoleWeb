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
    public class GrantController : Controller
    {
        private readonly IGrantService _service;

        public GrantController(IGrantService serv)
        {
            _service = serv;
        }

        public ViewResult Index()
        {
            ViewBag.Title = "Привилегии";
            var groups = GetGroups();

            return View(groups);
        }

        

        [HttpGet]
        public ViewResult EditGroup(string userid, string rolename)
        {
            GrantViewModel model = new GrantViewModel();
            model.UserList = GetUsers(userid).ToList();
            model.LayersList = GetLayers(rolename).ToList();
            model.DictsList = GetDicts(rolename).ToList();

            ViewBag.Title = "Редактирование пользователя";
            ViewBag.Rolename = rolename;
            return View(model);
        }

        [HttpPost]
        public IActionResult EditGroup(GrantViewModel model, string rolename)
        {
            var old_layers = GetLayers(rolename).OrderBy(r => r.Table_schema).ThenBy(r => r.Table_name).ToList();
            var old_dicts = GetDicts(rolename).OrderBy(r => r.Table_schema).ThenBy(r => r.Table_name).ToList();

            var layers = model.LayersList.OrderBy(r => r.Table_schema).ThenBy(r => r.Table_name).ToList();
            var dicts = model.DictsList.OrderBy(r => r.Table_schema).ThenBy(r => r.Table_name).ToList();

            var layerGranters = CompareGrants(old_layers, layers);
            var dictGranters = CompareGrants(old_dicts, dicts);

            if (layerGranters?.Count() > 0 || dictGranters?.Count() > 0)
            {
                try
                {
                    foreach (var gran in layerGranters)
                        _service.GrantTableToRole(gran.TableSchema, gran.TableName, rolename, gran.GrantList);
                    foreach (var gran in dictGranters)
                        _service.GrantTableToRole(gran.TableSchema, gran.TableName, rolename, gran.GrantList);
                    TempData["message"] = $"Сохранено.";
                }
                catch (Exception e)
                {
                    TempData["error"] = $"Warning. {e.Message}";
                }
            }
            return RedirectToAction("Index");
        }

        private List<Granter> CompareGrants(List<Grant> old_layers, List<Grant> layers)
        {
            List<Granter> granters = new List<Granter>();
            for (int i = 0; i < layers.Count(); i++)
            {
                List<string> grantsList = new List<string>();
                if (layers[i].IsSelect != old_layers[i].IsSelect)
                    grantsList.Add("SELECT");
                if (layers[i].IsUpdate != old_layers[i].IsUpdate)
                    grantsList.Add("UPDATE");
                if (layers[i].IsInsert != old_layers[i].IsInsert)
                    grantsList.Add("INSERT");
                if (layers[i].IsDelete != old_layers[i].IsDelete)
                    grantsList.Add("DELETE");

                if (grantsList.Count() > 0)
                {
                    granters.Add(new Granter
                    {
                        TableSchema = old_layers[i].Table_schema,
                        TableName = old_layers[i].Table_name,
                        GrantList = grantsList
                    });
                }
            }
            return granters;
        }

        private IEnumerable<User> GetUsers(string userid)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>()).CreateMapper();
            return mapper.Map<IEnumerable<UserDTO>, List<User>>(_service.GetUsers(userid));
        }

        private IEnumerable<Grant> GetLayers(string rolename)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<GrantDTO, Grant>()).CreateMapper();
            return mapper.Map<IEnumerable<GrantDTO>, List<Grant>>(_service.GetLayers(rolename));
        }

        private IEnumerable<Grant> GetDicts(string rolename)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<GrantDTO, Grant>()).CreateMapper();
            return mapper.Map<IEnumerable<GrantDTO>, List<Grant>>(_service.GetDicts(rolename));
        }

        private IEnumerable<User> GetGroups()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>()).CreateMapper();
            return mapper.Map<IEnumerable<UserDTO>, List<User>>(_service.GetGroups());
        }
    }

    public class Granter
    {
        public string TableSchema { get; set; }
        public string TableName { get; set; }
        public List<string> GrantList{ get; set; }
    }
}
