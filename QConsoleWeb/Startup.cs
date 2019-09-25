using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QConsoleWeb.BLL.Interfaces;
using QConsoleWeb.BLL.Services;
using QConsoleWeb.Data;

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
                app.UseStatusCodePages();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            //app.UseExceptionHandler("/Home/Error");
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {

                routes.MapRoute(
                    name: null,
                    template: "{controller}/{action}/{schemaname}/{tablename}/{geomtype?}"
                    );



                routes.MapRoute(
                    name: "default",
                    template: "{controller=User}/{action=List}/{id?}"
                    //defaults: new { action = "List"}
                    );
            });

        }
    }
}
