using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using QConsoleWeb.BLL.Interfaces;
using QConsoleWeb.BLL.Services;
using QConsoleWeb.Data;

namespace QConsoleWeb
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string _connectionString = "Host=localhost;Port=5432;Username=admin;Database=QGIS_BASE;Password=vfhfreqz_14;Application Name=\"QConsole WEB\"";
            services.AddEntityFrameworkNpgsql();

            services.AddMvc();
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
            }

            app.UseStaticFiles();
            //app.UseMvcWithDefaultRoute();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Session}/{action=List}/{id?}");
            });
        }
    }
}
