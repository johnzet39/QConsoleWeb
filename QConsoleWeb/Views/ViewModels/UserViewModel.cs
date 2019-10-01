using QConsoleWeb.Infrastructure.Attributes;
using QConsoleWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QConsoleWeb.Views.ViewModels
{
    public class UserViewModel
    {
        public IEnumerable<User> AssignedRoles { get; set; }
        public IEnumerable<User> Roles { get; set; }
        public User CurrentUser { get; set; }

        [Display(Name = "Ip-адрес пользователя")]
        [IpAddress]
        public string Ip { get; set; }

        [Display(Name = "Метод доступа")]
        public string Method { get; set; }

        [Display(Name = "Добавить запись в pg_hba")]
        public bool ToPgHba { get; set; }
    }
}
