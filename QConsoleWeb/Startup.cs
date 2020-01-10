using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QConsoleWeb.BLL.Interfaces;
using QConsoleWeb.BLL.Services;
using QConsoleWeb.Data;
using QConsoleWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace QConsoleWeb
{
    public class Startup
    {
        public IHostingEnvironment HostingEnvironment { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            this.HostingEnvironment = env;
            this.Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string _connectionString = Configuration.GetConnectionString("CONNECTION_BASE");
            string _connectionIdentity = Configuration.GetConnectionString("CONNECTION_IDENTITY");
            int _loginTimeout = Configuration.GetValue<int>("AppSettings:Login:LoginTimeout");

            services.AddEntityFrameworkNpgsql()
                .AddDbContext<AppIdentityDbContext>(opt =>
                    opt.UseNpgsql(_connectionIdentity));

            services.AddIdentity<AppUser, IdentityRole>(opts => {
                opts.Password.RequiredLength = 1;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            // Changing the loging URL
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(_loginTimeout); //время действия куков.
                options.SlidingExpiration = true; //продление времени действия куков. Применяется с середины периода.
            });

            //services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.FromMinutes(1));

            services.AddDetection();
            services.AddDetectionCore() //device detection
                .AddDevice();
            services.AddMvc()
                .AddMvcOptions(options =>
                {
                    options.Filters.Add(new AuthorizeFilter()); //set [Autorizate] global
                });

            //services.AddTransient<IUserService, FakeUserService>();
            services.AddTransient<IUserService, UserService>(serviceProvider => new UserService(_connectionString));
            services.AddTransient<ISessionService, SessionService>(serviceProvider => new SessionService(_connectionString));
            services.AddTransient<ILayerService, LayerService>(serviceProvider => new LayerService(_connectionString));
            services.AddTransient<IGrantService, GrantService>(serviceProvider => new GrantService(_connectionString));
            services.AddTransient<ILoggerService, LoggerService>(serviceProvider => new LoggerService(_connectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            AppIdentityDbContext.CreateAdminAccount(app.ApplicationServices, Configuration).Wait();

            app.Map("/error", ap => ap.Run(async context =>
            {
                await context.Response.WriteAsync("Ops...");
            }));

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: null,
                    template: "{controller}/{action}/{schemaname}/{tablename}/{geomtype?}"
                    );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=User}/{action=Index}/{id?}"
                    //defaults: new { action = "List"}
                    );
            });

        }
    }
}
