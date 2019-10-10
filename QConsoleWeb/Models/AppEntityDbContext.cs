using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QConsoleWeb.Models
{
    public class AppEntityDbContext : IdentityDbContext<AppUser>
    {
        public AppEntityDbContext(DbContextOptions<AppEntityDbContext> options)
            : base(options)
        {
        }

        //public DbSet<AppUser> AppUsers { get; set; } 
    }
}
