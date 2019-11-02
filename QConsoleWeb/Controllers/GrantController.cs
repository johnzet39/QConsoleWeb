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
            if (Request.Headers["User-Agent"].ToString().IndexOf("Windows NT", StringComparison.OrdinalIgnoreCase) >= 0)
                return View(groups);
            else
                return View("IndexMobile", groups);
        }

        [HttpGet]
        public IActionResult EditGroup(string userid, string rolename, bool ismobile = false)
        {
            GrantViewModel model = new GrantViewModel();
            model.UserList = GetUsers(userid).ToList();
            model.LayersList = GetLayers(rolename).ToList();
            model.DictsList = GetDicts(rolename).ToList();

            ViewBag.Title = "Редактирование группы";
            ViewBag.Rolename = rolename;
            if (ismobile)
                return View("EditGroupMobile", model);
            else
                return PartialView(model);
        }

        [HttpPost]
        public IActionResult EditGroup(GrantViewModel model, string rolename)
        {
            var old_layers = GetLayers(rolename).OrderBy(r => r.Table_schema).ThenBy(r => r.Table_name).ToList();
            var old_dicts = GetDicts(rolename).OrderBy(r => r.Table_schema).ThenBy(r => r.Table_name).ToList();

            var layers = model.LayersList.OrderBy(r => r.Table_schema).ThenBy(r => r.Table_name).ToList();
            var dicts = model.DictsList.OrderBy(r => r.Table_schema).ThenBy(r => r.Table_name).ToList();

            var layerGranters = CompareGrants(old_layers, layers, out bool selChanged, out bool updChanged,
                                                      out bool insChanged, out bool delChanged);
            var dictGranters = CompareGrants(old_dicts, dicts, out bool dict_selChanged, out bool dict_updChanged,
                                                      out bool dict_insChanged, out bool dict_delChanged);

            if (layerGranters?.Count() > 0 || dictGranters?.Count() > 0)
            {
                try
                {
                    foreach (var gran in layerGranters)
                        _service.GrantTableToRole(gran.TableSchema, gran.TableName, rolename, 
                            gran.IsSelect, gran.IsUpdate, gran.IsInsert, gran.IsDelete,
                            selChanged, updChanged, insChanged, delChanged);
                    foreach (var gran in dictGranters)
                        _service.GrantTableToRole(gran.TableSchema, gran.TableName, rolename,
                            gran.IsSelect, gran.IsUpdate, gran.IsInsert, gran.IsDelete,
                            dict_selChanged, dict_updChanged, dict_insChanged, dict_delChanged);
                    TempData["message"] = $"Сохранено.";
                }
                catch (Exception e)
                {
                    TempData["error"] = $"Warning. {e.Message}";
                }
            }
            return RedirectToAction("Index");
        }

        private List<Granter> CompareGrants(List<Grant> old_layers, List<Grant> layers, 
                                            out bool selChanged, out bool updChanged, out bool insChanged, out bool delChanged)
        {
            selChanged = false;
            updChanged = false;
            insChanged = false;
            delChanged = false;

            List<Granter> granters = new List<Granter>();
            for (int i = 0; i < layers.Count(); i++)
            {
                bool hasChanges = false;

                if (layers[i].IsSelect != old_layers[i].IsSelect)
                    selChanged = true;
                if (layers[i].IsUpdate != old_layers[i].IsUpdate)
                    updChanged = true;
                if (layers[i].IsInsert != old_layers[i].IsInsert)
                    insChanged = true;
                if (layers[i].IsDelete != old_layers[i].IsDelete)
                    delChanged = true;

                if (selChanged || updChanged || insChanged || delChanged)
                    hasChanges = true;

                if (hasChanges)
                {
                    Granter granter = new Granter
                    {
                        TableSchema = old_layers[i].Table_schema,
                        TableName = old_layers[i].Table_name,
                    };

                    if (layers[i].IsSelect)
                        granter.IsSelect = true;
                    if (layers[i].IsUpdate)
                        granter.IsUpdate = true;
                    if (layers[i].IsInsert)
                        granter.IsInsert = true;
                    if (layers[i].IsDelete)
                        granter.IsDelete = true;

                    granters.Add(granter);
                }
            }
            return granters;
        }

        [HttpGet]
        public IActionResult EditColumns(string schema, string table, string rolename)
        {
            List<GrantColumn> columns = GetColumns(schema, table, rolename).ToList();
            var model = new GrantColumnsViewModel
            {
                Columns = columns,
                TableName = table,
                SchemaName = schema,
                Rolename = rolename
            };

            return PartialView("EditGrantsColumnsModalPartial", model);
        }

        [HttpPost]
        public IActionResult EditColumns(GrantColumnsViewModel model)
        {
            var old_columns = GetColumns(model.SchemaName, model.TableName, model.Rolename)
                                .OrderBy(r => r.Column_name).ToList();
            var columns = model.Columns.OrderBy(r => r.Column_name).ToList();

            var columnGranters = CompareColumnsGrants(old_columns, columns, 
                                                      out bool selChanged, out bool updChanged, 
                                                      out bool insChanged);

            if (columnGranters?.Count() > 0)
            {
                try
                {
                    List<string> selectList = new List<string>();
                    List<string> updateList = new List<string>();
                    List<string> insertList = new List<string>();

                    foreach (var gran in columnGranters)
                    {
                        if (gran.IsSelect)
                            selectList.Add(gran.ColumnName);
                        if (gran.IsUpdate)
                            updateList.Add(gran.ColumnName);
                        if (gran.IsInsert)
                            insertList.Add(gran.ColumnName);
                    }
                    _service.GrantColumnsToRole(model.SchemaName, model.TableName, model.Rolename, 
                                                selectList, updateList, insertList,
                                                selChanged, updChanged, insChanged);
                    //TempData["message"] = $"Сохранено.";
                    if (Request.Headers["User-Agent"].ToString().IndexOf("Windows NT", StringComparison.OrdinalIgnoreCase) >= 0)
                        return Json(new { ok = true });
                    else
                        return RedirectToAction("Index");
                    
                }
                catch (Exception e)
                {
                    //TempData["error"] = $"Warning. {e.Message}";
                    return Json(new { ok = false, message = e.Message });
                }
            }
            return Json(new { ok = true });
        }

        private List<ColumnGranter> CompareColumnsGrants(List<GrantColumn> old_columns, List<GrantColumn> columns, 
                                                        out bool selChanged, out bool updChanged, out bool insChanged)
        {
            selChanged = false;
            updChanged = false;
            insChanged = false;

            List<ColumnGranter> granters = new List<ColumnGranter>();
            for (int i = 0; i < columns.Count(); i++)
            {
                bool hasChanges = false;

                if (columns[i].IsSelect != old_columns[i].IsSelect)
                    selChanged = true;
                if (columns[i].IsUpdate != old_columns[i].IsUpdate)
                    updChanged = true;
                if (columns[i].IsInsert != old_columns[i].IsInsert)
                    insChanged = true;

                if (selChanged || updChanged || insChanged)
                    hasChanges = true;

                if (hasChanges)
                {
                    ColumnGranter columnGranter = new ColumnGranter
                    {
                        ColumnName = columns[i].Column_name
                    };

                    if (columns[i].IsSelect)
                        columnGranter.IsSelect = true;
                    if (columns[i].IsUpdate)
                        columnGranter.IsUpdate = true;
                    if (columns[i].IsInsert)
                        columnGranter.IsInsert = true;

                    granters.Add(columnGranter);
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

        private IEnumerable<GrantColumn> GetColumns(string table_schema, string table_name, string role_name)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<GrantColumnDTO, GrantColumn>()).CreateMapper();
            return mapper.Map<IEnumerable<GrantColumnDTO>, List<GrantColumn>>(_service.GetColumns(table_schema, table_name, role_name));
        }
    }

    public class Granter
    {
        public string TableSchema { get; set; }
        public string TableName { get; set; }
        public bool IsSelect { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsInsert { get; set; }
        public bool IsDelete { get; set; }
    }

    public class ColumnGranter
    {
        public string ColumnName { get; set; }
        public bool IsSelect { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsInsert { get; set; }
    }
}
