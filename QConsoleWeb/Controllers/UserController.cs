﻿using Microsoft.AspNetCore.Mvc;
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
using System.Text;
using System.IO;

namespace QConsoleWeb.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _service;
        private readonly IConfiguration _config;

        public UserController(IUserService serv, IConfiguration config)
        {
            _config = config;
            _service = serv;
        }

        public ViewResult List()
        {
            ViewBag.Title = "Список пользователей";
            var users = GetUsers();

            return View(users);
        }

        [HttpGet]
        public ViewResult EditUser(string userid)
        {
            UserViewModel model = new UserViewModel();
            User current = GetUsers()
                    .FirstOrDefault(p => p.Usesysid == userid);
            model.CurrentUser = current;
            model.AssignedRoles = GetAssignedRoles(current.Usesysid);
            model.Roles = GetUsers().Where(m => m.Isrole && m.Usesysid != userid);

            ViewBag.Title = "Редактирование пользователя";
            return View(model);
        }

        [HttpPost]
        public IActionResult EditUser(UserViewModel model, List<string> ischeckedlist)
        {
            if (ModelState.IsValid)
            {
                //is definition changed?
                User oldlayer = GetUsers().FirstOrDefault(m => m.Usesysid == model.CurrentUser.Usesysid);
                string descript = null;
                if (oldlayer != null)
                    descript = (oldlayer.Descript != model.CurrentUser.Descript) ? model.CurrentUser.Descript : null;

                try
                {
                    _service.EditUserOrRole(model.CurrentUser.Usename, model.CurrentUser.Password, descript);
                    AcceptPrivilegies(model.CurrentUser, ischeckedlist);

                    TempData["message"] = $"{model.CurrentUser.Usename} сохранен.";
                }
                catch (Exception e)
                {
                    TempData["error"] = $"Warning: {e.Message}";
                }
                return RedirectToAction("List");
            }
            else
            {
                return View(model);
            }
        }

        public IActionResult CreateUser()
        {
            UserViewModel model = new UserViewModel()
            {
                Roles = GetUsers().Where(m => m.Isrole)
            };
            ViewBag.MethodDefault = _config.GetSection("AppSettings:UserTab:Pg_hba:method_default").Get<string>();
            ViewBag.Title = "Создание пользователя";
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateUser(UserViewModel model, List<string> ischeckedlist)
        {
            if (ModelState.IsValid)
            {
                if (model.CurrentUser.Isrole == false && (model.CurrentUser.Password == null || model.CurrentUser.Password.Length <= 0)) //заглушка если не введен пароль для нового пользователя
                {
                    model.Roles = GetUsers().Where(m => m.Isrole);
                    return View(model);
                }

                try
                {
                    _service.CreateUserOrRole(model.CurrentUser.Usename, model.CurrentUser.Password, model.CurrentUser.Descript);
                    AcceptPrivilegies(model.CurrentUser.Usename, ischeckedlist);
                    if (model.ToPgHba)
                        CreateUserPgHba(model);
                }
                catch (Exception e)
                {
                    TempData["error"] = $"Warning: {e.Message}";
                }
                return RedirectToAction("List");
            }
            ViewBag.MethodDefault = _config.GetSection("AppSettings:UserTab:Pg_hba:method_default").Get<string>();
            model.Roles = GetUsers().Where(m => m.Isrole);
            return View(model);
        }

        private void AcceptPrivilegies(User curUser, List<string> ischeckedList)
        {
            var assignedList = GetAssignedRoles(curUser.Usesysid).Select(o => o.Usename).ToList();
            ischeckedList.RemoveAll(o => o == "false");

            var ToGrantList = ischeckedList.Except(assignedList).ToList();
            var ToRevokeList = assignedList.Except(ischeckedList).ToList();

            foreach (string grantRole in ToGrantList)
                _service.GrantRole(curUser.Usename, grantRole);
            foreach (string revokeRole in ToRevokeList)
                _service.RevokeRole(curUser.Usename, revokeRole);
        }

        private void AcceptPrivilegies(string newUserName, List<string> ischeckedList)
        {
            ischeckedList.RemoveAll(o => o == "false");
            foreach (string rolestring in ischeckedList)
                _service.GrantRole(newUserName, rolestring);
        }

        private void CreateUserPgHba(UserViewModel model)
        {
            if (model.Ip == null)
                return;

            string username = model.CurrentUser.Usename;
            string ip = model.Ip;
            string method = model.Method ?? _config
                .GetSection("AppSettings:UserTab:Pg_hba:method_default").Get<string>();
            string additional_roles_postfix = _config
                .GetSection("AppSettings:UserTab:Pg_hba:additional_roles_postfix").Get<string>();
            string section_line = _config
                .GetSection("AppSettings:UserTab:Pg_hba:section_line").Get<string>();
            string database_name = _config
                .GetSection("AppSettings:UserTab:Pg_hba:database_name").Get<string>();

            var pgHbas = _config.GetSection("AppSettings:UserTab:Pg_hba:pg_hbaPaths").Get<List<string>>();
            string pghba0 = pgHbas[0];
            List<string> textLines = System.IO.File.ReadAllLines(pghba0).ToList();
            int index_IPv4 = textLines.IndexOf(section_line);

            string lineToAdd = $"host     {database_name}   {username}{additional_roles_postfix}     {ip}/32    {method}";

            char[] delimiterChars = { ' ', '\t' };
            for (int i = index_IPv4; i < textLines.Count(); i++)
            {
                if (!textLines[i].StartsWith("host")
                    && !textLines[i].StartsWith("# host")
                    && !textLines[i].StartsWith("#host"))
                    continue;
                string[] words = textLines[i].Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                string compared_word = words[2];
                if (String.Compare(username.ToLower(), compared_word.ToLower()) < 0)
                {
                    textLines.Insert(i, lineToAdd);
                    foreach (string pghba in pgHbas)
                        System.IO.File.WriteAllLines(pghba, textLines);
                    return;
                }
                continue;
            }
            textLines.Insert(textLines.Count(), lineToAdd);
        }

        [HttpGet]
        public IActionResult DeleteUser(string userid)
        {
            User user = GetUsers().FirstOrDefault(p => p.Usesysid == userid);
                try
                {

                    _service.RemoveRoleOrUser(user.Usename);
                    TempData["message"] = $"Пользователь {user.Usename} был удален.";

                }
                catch (Exception e)
                {
                    TempData["error"] = $"Пользователь {user.Usename} не был удален. {e.Message}";
                }
            return RedirectToAction("List");
        }

        private IEnumerable<User> GetAvailableRoles(string oid)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>()).CreateMapper();
            return mapper.Map<IEnumerable<UserDTO>, List<User>>(_service.GetAvailableRolesObject(oid));
        }

        private IEnumerable<User> GetAssignedRoles(string oid)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>()).CreateMapper();
            return mapper.Map<IEnumerable<UserDTO>, List<User>>(_service.GetAssignedRolesObject(oid));
        }

        private IEnumerable<User> GetUsers()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>()).CreateMapper();
            return mapper.Map<IEnumerable<UserDTO>, List<User>>(_service.GetUsers());
        }

        public ViewResult EditPgHba()
        {
            var pgHbas = _config.GetSection("AppSettings:UserTab:Pg_hba:pg_hbaPaths").Get<List<string>>();
            UserPgHbaViewModel model = new UserPgHbaViewModel();
            string pghbaText = string.Empty;
            string fileDate = string.Empty;

            try
            {
                pghbaText = System.IO.File.ReadAllText(pgHbas[0]);
                fileDate = GetFileDate(pgHbas[0]);
            }

            catch (FileNotFoundException)
            {
                pghbaText = "Файл не найден";
            }
            catch (DirectoryNotFoundException)
            {
                pghbaText = "Файл не найден";
            }
            catch (Exception e)
            {
                pghbaText = e.Message;
            }
            
            model.PgHbaContext = pghbaText;
            model.FileDate = fileDate;

            return View(model);
        }

        [HttpPost]
        public IActionResult EditPgHba(UserPgHbaViewModel model)
        {
            string pghbaText = model.PgHbaContext;
            var pgHbas = _config.GetSection("AppSettings:UserTab:Pg_hba:pg_hbaPaths").Get<List<string>>();

            string errors = string.Empty;
            foreach (string pghba in pgHbas)
            {
                try
                {
                    var fileStream = new FileStream(pghba, FileMode.Create, FileAccess.Write);
                    Encoding encodingUtf8WoBOM = new UTF8Encoding(false);
                    using (var streamWriter = new StreamWriter(fileStream, encodingUtf8WoBOM))
                    {
                        streamWriter.Write(pghbaText);
                    }
                }
                catch (Exception e)
                {
                   errors += $"Не удалось сохранить файл конфигурации {pghba}. ({e.Message}).\n";
                }
            }

            if (errors.Length > 0)
                TempData["error"] = errors;


            return RedirectToAction("List");
        }

        private string GetFileDate(string filePath)
        {
            string dtString;
            DateTime dt = System.IO.File.GetLastWriteTime(filePath);
            dtString = string.Format("({0})", dt.ToString("dd.MM.yyyy HH:mm:ss"));

            return dtString;
        }

    }
}
