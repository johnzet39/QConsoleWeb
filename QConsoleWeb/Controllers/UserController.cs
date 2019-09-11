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
        private IUserService _service;

        public UserController(IUserService serv)
        {
            _service = serv;
        }

        public ViewResult List()
        {
            ViewBag.Title = "Список пользователей";
            UserViewModel model = new UserViewModel();
            model.Users = GetUsers();

            return View(model);
        }

        [HttpGet]
        public ViewResult EditUser(string userid)
        {
            ViewBag.isnew = false;
            ViewBag.Title = "Редактирование пользователя";
            return View(GetUsers()
                    .FirstOrDefault(p => p.Usesysid == userid));
        }

        [HttpPost]
        public IActionResult EditUser(User user, bool isnew)
        {
            if (ModelState.IsValid)
            {
                if (user.Isrole == false && isnew && (user.Password == null || user.Password.Length <= 0))
                {
                    return View(user);
                }

                try
                {
                    if (isnew)
                        _service.CreateUserOrRole(user.Usename, user.Password, user.Descript);
                    else
                        _service.EditUserOrRole(user.Usename, user.Password, user.Descript);
                    TempData["message"] = $"{user.Usename} has been saved";
                }
                catch (Exception e)
                {
                    TempData["error"] = $"{user.Usename} has not been saved. {e.Message}";
                }
                
                return RedirectToAction("List");
            }
            else
            {
                // there is something wrong with the data values
                return View(user);
            }
        }

        public IActionResult CreateUser()
        {
            ViewBag.isnew = true;
            ViewBag.Title = "Редактирование пользователя";
            return View("EditUser", new User());
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

        //private IEnumerable<User> GetAvailableRoles()
        //{
        //    var mapper = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>()).CreateMapper();
        //    return mapper.Map<IEnumerable<UserDTO>, List<User>>(_service.GetAvailableRoles());
        //}

        //private IEnumerable<User> GetAssignedRoles()
        //{
        //    var mapper = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>()).CreateMapper();
        //    return mapper.Map<IEnumerable<UserDTO>, List<User>>(_service.GetAssignedRoles());
        //}

        private IEnumerable<User> GetUsers()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>()).CreateMapper();
            return mapper.Map<IEnumerable<UserDTO>, List<User>>(_service.GetUsers());
        }



    }
}
