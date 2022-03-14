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
using Wangkanai.Detection;

namespace QConsoleWeb.Controllers
{
    [Authorize]
    public class GrantController : Controller
    {
        private readonly IDevice _device;
        private readonly IGrantService _service;

        public GrantController(IGrantService serv, IDetection detection)
        {
            _device = detection.Device;
            _service = serv;
        }

        public ViewResult Index()
        {
            ViewBag.Title = "Привилегии";
            var groups = GetGroups();
            //if (_device.Type.ToString().ToLower() == "desktop")
            //    return View(groups);
            //else
            //    return View("IndexMobile", groups);
            return View(groups);
        }

        [HttpGet]
        public IActionResult EditGroup(string userid, string rolename, bool ispart)
        {
            GrantViewModel model = new GrantViewModel();
            model.UserList = GetUsers(userid).ToList();
            model.LayersList = GetLayers(rolename).ToList();
            model.DictsList = GetDicts(rolename).ToList();

            ViewBag.Title = "Редактирование группы";
            ViewBag.Rolename = rolename;
            ViewBag.Userid = userid;
            //if (_device.Type.ToString().ToLower() == "desktop")
            //    return PartialView(model);
            //else
            //    return View("EditGroupMobile", model);
            if (ispart)
            {
                return PartialView("EditGroupPart", model);
            }
            else {
                return PartialView(model);
            }

        }

        [HttpPost]
        [RequestFormLimits(ValueCountLimit = 10000)]
        public IActionResult EditGroup(GrantViewModel model, string rolename)
        {
            var old_layers = GetLayers(rolename).OrderBy(r => r.Table_schema).ThenBy(r => r.Table_name).ToList();
            var old_dicts = GetDicts(rolename).OrderBy(r => r.Table_schema).ThenBy(r => r.Table_name).ToList();


            var layers = new List<Grant>();
            if (model.LayersList != null)
                layers = model.LayersList.OrderBy(r => r.Table_schema).ThenBy(r => r.Table_name).ToList();
            var layerGranters = CompareGrants(old_layers, layers);


            var dicts = new List<Grant>();
            if (model.DictsList != null)
                dicts = model.DictsList.OrderBy(r => r.Table_schema).ThenBy(r => r.Table_name).ToList();
            var dictGranters = CompareGrants(old_dicts, dicts);

            try
            {
                if (layerGranters?.Count() > 0)
                {
                    foreach (var gran in layerGranters)
                        _service.GrantTableToRole(gran.TableSchema, gran.TableName, rolename,
                            gran.IsSelect, gran.IsUpdate, gran.IsInsert, gran.IsDelete,
                            gran.selChanged, gran.updChanged, gran.insChanged, gran.delChanged);
                }
                if (dictGranters?.Count() > 0)
                {
                    foreach (var gran in dictGranters)
                        _service.GrantTableToRole(gran.TableSchema, gran.TableName, rolename,
                            gran.IsSelect, gran.IsUpdate, gran.IsInsert, gran.IsDelete,
                            gran.selChanged, gran.updChanged, gran.insChanged, gran.delChanged);
                }
                if (layerGranters?.Count() > 0 || dictGranters?.Count() > 0)
                    TempData["message"] = $"Сохранено.";
            }
            catch (Exception e)
            {
                TempData["error"] = $"Warning. {e.Message}";
            }

            return RedirectToAction("Index");
        }

        private List<Granter> CompareGrants(List<Grant> old_layers, List<Grant> layers)
        {
            List<Granter> granters = new List<Granter>();
            for (int i = 0; i < layers.Count(); i++)
            {
                var layer = layers[i];
                var oldlayer = old_layers[i];

                bool hasChanges = false;

                bool selChanged = false;
                bool updChanged = false;
                bool insChanged = false;
                bool delChanged = false;

                if (layer.IsSelect != oldlayer.IsSelect)
                    selChanged = true;
                if (layer.IsUpdate != oldlayer.IsUpdate)
                    updChanged = true;
                if (layer.IsInsert != oldlayer.IsInsert)
                    insChanged = true;
                if (layer.IsDelete != oldlayer.IsDelete)
                    delChanged = true;

                if (selChanged || updChanged || insChanged || delChanged)
                    hasChanges = true;

                if (hasChanges)
                {
                    Granter granter = new Granter
                    {
                        TableSchema = oldlayer.Table_schema,
                        TableName = oldlayer.Table_name,
                        IsSelect = layer.IsSelect,
                        IsUpdate = layer.IsUpdate,
                        IsInsert = layer.IsInsert,
                        IsDelete = layer.IsDelete,
                        selChanged = selChanged,
                        updChanged = updChanged,
                        insChanged = insChanged,
                        delChanged = delChanged
                    };
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

            CompareColumnsGrants(old_columns, columns, out bool selChanged, out bool updChanged, out bool insChanged);

            if (selChanged || updChanged || insChanged)
            {
                try
                {
                    List<string> selectList = new List<string>();
                    List<string> updateList = new List<string>();
                    List<string> insertList = new List<string>();

                    foreach (var column in columns)
                    {
                        if (column.IsSelect)
                            selectList.Add(column.Column_name);
                        if (column.IsUpdate)
                            updateList.Add(column.Column_name);
                        if (column.IsInsert)
                            insertList.Add(column.Column_name);
                    }
                    _service.GrantColumnsToRole(model.SchemaName, model.TableName, model.Rolename, 
                                                selectList, updateList, insertList,
                                                selChanged, updChanged, insChanged);
                    //TempData["message"] = $"Сохранено.";

                    //if (_device.Type.ToString().ToLower() == "desktop")
                    //    return Json(new { ok = true, mobile = false });
                    //else
                    //{
                    //    TempData["message"] = $"Сохранено.";
                    //    return Json(new { ok = true, mobile = true,  newurl = Url.Action("Index") });
                    //}

                    return Json(new { ok = true, mobile = false });
                }
                catch (Exception e)
                {
                    //TempData["error"] = $"Warning. {e.Message}";
                    return Json(new { ok = false, message = e.Message });
                }
            }
            return Json(new { ok = true });
        }

        private void CompareColumnsGrants(List<GrantColumn> old_columns, List<GrantColumn> columns,
                                                         out bool selChanged, out bool updChanged, out bool insChanged)
        {
            selChanged = false;
            updChanged = false;
            insChanged = false;

            List<ColumnGranter> granters = new List<ColumnGranter>();
            for (int i = 0; i < columns.Count(); i++)
            {
                var column = columns[i];
                var oldcolumn = old_columns[i];

                if (column.IsSelect != oldcolumn.IsSelect)
                    selChanged = true;
                if (column.IsUpdate != oldcolumn.IsUpdate)
                    updChanged = true;
                if (column.IsInsert != oldcolumn.IsInsert)
                    insChanged = true;

                //if (hasChanges)
                //{
                //    ColumnGranter columnGranter = new ColumnGranter
                //    {
                //        ColumnName = column.Column_name,
                //        IsSelect = column.IsSelect,
                //        IsUpdate = column.IsUpdate,
                //        IsInsert = column.IsInsert
                //    };
                //    granters.Add(columnGranter);

                //}
            }
            return;
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
        public bool selChanged { get; set; }
        public bool updChanged { get; set; }
        public bool insChanged { get; set; }
        public bool delChanged { get; set; }
    }

    public class ColumnGranter
    {
        public string ColumnName { get; set; }
        public bool IsSelect { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsInsert { get; set; }
        //public bool selChanged { get; set; }
        //public bool updChanged { get; set; }
        //public bool insChanged { get; set; }
    }
}
