using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Pipaslot.Demo.Filters;
using Pipaslot.Infrastructure.Data.EntityFramework;
using Pipaslot.Infrastructure.Mvc;
using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.Infrastructure.Security.EntityFramework;
using Pipaslot.Infrastructure.Security.Identities;
using Pipaslot.Infrastructure.SecurityUI;
using Pipaslot.Demo.Models;
using Pipaslot.Demo.Models.Entities;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Data.Queries;
using Swashbuckle.AspNetCore.Swagger;

namespace Pipaslot.Demo
{
    /// <summary>
    /// Configure application
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Application configuration from appsettings.json
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Store application configuration from appsettings.json
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configure application services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ControllerViewLocationExpander("App"));
            });

            //Database migrations context
            services.AddDbContext<AppDatabase>(o => o.UseSqlServer(Configuration.GetSection("ConnectionString").Value));

            //Database factories
            services.AddSingleton<IEntityFrameworkDbContextFactory>(_ =>
            {
                var options = new DbContextOptionsBuilder<AppDatabase>();
                options.UseSqlServer(Configuration.GetSection("ConnectionString").Value);
                return new EntityFrameworkDbContextFactory<AppDatabase>(options.Options);
            });

            //Unit of work
            services.AddScoped<IUnitOfWorkRegistry, UnitOfWorkRegistry>();
            services.AddScoped<IUnitOfWorkFactory, EntityFrameworkUnitOfWorkFactory<AppDatabase>>();

            //Repositories
            services.AddScoped<IRepository<SimpleRecord, int>, EntityFrameworkRepository<AppDatabase, SimpleRecord, int>>();

            //Queries
            services.AddScoped<IQueryFactory<IQuery<SimpleRecord>>, EntityFrameworkQueryFactory<EntityFrameworkQuery<AppDatabase, SimpleRecord>>>();

            //Configure own services for Permission Manager
            services.AddScoped<IPermissionStore<int>, PermissionStore<int, AppDatabase>>();
            services.AddScoped<IIdentity<int>>(s =>
              //TODO implementovat zabezpečovací logiku
               new GuestIdentity<int>()
            );
            //Add default configuration for Permission Manager
            services.AddSecurityUI<int>();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = Configuration.GetSection("AppName").Value, Version = "v1" });
                c.DocumentFilter<LowercaseDocumentFilter>();

                //Set the comments path for the swagger json and ui.
                var documentationFile = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Pipaslot.Demo.xml");
                c.IncludeXmlComments(documentationFile);
            });
        }

        /// <summary>
        /// Configure Pipeline
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // SWAGGER
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pipaslot.Demo");
                c.RoutePrefix = "api";
            });

            //Add Middleware for Permission Manager
            app.UseSecurityUI(options =>
            {
                options.RoutePrefix = "security";
            });

            app.UseMvc(routes =>
            {
                // API 
                routes.MapRoute("Api", "{area:exists}/{controller}/{action}/{id?}");

                // APP
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
