using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace QConsoleWeb.Models
{
    public class AppUser : IdentityUser
    {
        //public string Country { get; set; }
    }
}
