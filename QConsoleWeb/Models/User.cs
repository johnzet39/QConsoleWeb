using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QConsoleWeb.Models
{
    public class User
    {
        [Required(ErrorMessage = "Введите имя пользователя/роли")]
        [Display(Name = "Имя пользователя/роли")]
        public string Usename { get; set; }

        [Display(Name = "Описание")]
        public string Descript { get; set; }
        public Boolean Usesuper { get; set; }

        [Display(Name = "Групповая роль")]
        public Boolean Isrole { get; set; }
        public string Usesysid { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Подтверждение пароля")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}
