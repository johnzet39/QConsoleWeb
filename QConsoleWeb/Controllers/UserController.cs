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
    public class UserController : Controller
    {
        private readonly IUserService _service;

        public UserController(IUserService serv)
        {
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

            ViewBag.isnew = false;
            ViewBag.Title = "Редактирование пользователя";
            return View(model);
        }

        [HttpPost]
        public IActionResult EditUser(UserViewModel model, bool isnew, List<string> ischeckedlist)
        {
            if (ModelState.IsValid)
            {
                if (model.CurrentUser.Isrole == false && isnew && (model.CurrentUser.Password == null || model.CurrentUser.Password.Length <= 0)) //заглушка если не введен пароль для нового пользователя
                    return View(model);

                //is definition changed?
                User oldlayer = GetUsers().FirstOrDefault(m => m.Usesysid == model.CurrentUser.Usesysid);
                string descript = null;
                if (oldlayer != null)
                    descript = (oldlayer.Descript != model.CurrentUser.Descript) ? model.CurrentUser.Descript : null;

                try
                {
                    if (isnew)
                    {
                        _service.CreateUserOrRole(model.CurrentUser.Usename, model.CurrentUser.Password, model.CurrentUser.Descript);
                        AcceptPrivilegies(model.CurrentUser.Usename, ischeckedlist);
                    }
                    else
                    {
                        _service.EditUserOrRole(model.CurrentUser.Usename, model.CurrentUser.Password, descript);
                        AcceptPrivilegies(model.CurrentUser, ischeckedlist);
                    }

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

        public IActionResult CreateUser()
        {
            UserViewModel model = new UserViewModel()
            {
                Roles = GetUsers().Where(m => m.Isrole)
            };
            ViewBag.isnew = true;
            ViewBag.Title = "Редактирование пользователя";
            return View("EditUser", model);
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



    }
}
