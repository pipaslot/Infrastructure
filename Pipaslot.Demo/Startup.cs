using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Pipaslot.Demo.Filters;
using Pipaslot.Infrastructure.Data.EntityFramework;
using Pipaslot.Infrastructure.Mvc;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.Infrastructure.Security.EntityFramework;
using Pipaslot.SecurityUI;
using Pipaslot.Demo.Models;
using Pipaslot.Demo.Models.Entities;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.EntityFramework.Entities;
using Pipaslot.Infrastructure.Security.Jwt;
using Pipaslot.Infrastructure.Security.JWT;
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
            services.AddMvc(config =>
            {
                //Handling exception
                config.Filters.Add(new ExceptionFilter());

                //Global authorize attribute
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });
               // .AddApplicationPart(typeof(Pipaslot.SecurityUI.HomeController).Assembly);
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
            services.AddScoped<IRepository<Company, int>, EntityFrameworkRepository<AppDatabase, Company, int>>();
            services.AddScoped<IRepository<Role<int>, int>, RoleStore<int, AppDatabase>>();
            services.AddScoped<UserRepository>();

            #region Security
            
            //Queries
            services.AddScoped<IResourceInstanceProvider, ResourceInstanceProvider<AppDatabase>>();

            //Configure own services for Permission Manager
            services.AddScoped<IPermissionStore<int>, PermissionStore<int, AppDatabase>>();
            services.AddScoped<IRoleStore, RoleStore<int, AppDatabase>>();

            //Add default configuration for Permission Manager
            services.AddSecurityUI<int>();

            //Add Jwt token
            services.AddJwtAuthentication(new JwtTokenParameters
            {
                Issuer = Configuration["JwtSecurityToken:Issuer"],
                Audience = Configuration["JwtSecurityToken:Audience"],
                SigningKey = Configuration["JwtSecurityToken:Key"]
            });

            #endregion

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = Configuration.GetSection("AppName").Value, Version = "v1" });
                c.DocumentFilter<LowercaseDocumentFilter>();
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme()
                {
                    In = "header",
                    Description = "Please insert JWT with Bearer into field with format 'Bearer MyAccessToken'",
                    Name = "Authorization",
                    Type = "apiKey"
                });
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
            app.UseSecurityUI<int>(options =>
            {
                options.RoutePrefix = "security";
            });

            app.UseAuthentication();
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
