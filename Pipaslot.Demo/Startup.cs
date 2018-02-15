using System;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Pipaslot.Demo.Filters;
using Pipaslot.Infrastructure.Data.EntityFrameworkCore;
using Pipaslot.Infrastructure.Mvc;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.Infrastructure.Security.EntityFrameworkCore;
using Pipaslot.SecurityUI;
using Pipaslot.Demo.Models;
using Pipaslot.Demo.Models.Entities;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Security.Jwt;
using Swashbuckle.AspNetCore.Swagger;
using IUser = Pipaslot.Demo.Models.IUser;

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
            #region Mvc

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
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ControllerViewLocationExpander("App"));
            });

            #endregion

            #region Data access

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
            services.AddSingleton<IUnitOfWorkRegistry, UnitOfWorkRegistry>();
            services.AddSingleton<IUnitOfWorkFactory, EntityFrameworkUnitOfWorkFactory<AppDatabase>>();

            //Repositories
            services.AddSingleton<IRepository<Company, int>, EntityFrameworkRepository<AppDatabase, Company, int>>();
            services.AddSingleton<IRepository<Models.Entities.Role, int>, RoleStore<int, AppDatabase, Models.Entities.Role>>();
            services.AddSingleton<UserRepository>();

            #endregion

            #region Security

            //Queries
            services.AddSingleton<IResourceInstanceProvider, ResourceInstanceProvider<AppDatabase>>();

            //Configure own services for Permission Manager
            services.AddSingleton<IPermissionStore<int>, PermissionStore<int, AppDatabase>>();
            services.AddSingleton<IRoleStore, RoleStore<int, AppDatabase, Models.Entities.Role>>();

            //Add default configuration for Permission Manager
            services.AddSecurityUI<int, AppUser>();
            //Register alias to IUser<int> from Security project
            services.AddSingleton(s => (IUser)s.GetService(typeof(AppUser)));

            //Add Jwt token
            services.AddJwtAuthentication(new JwtTokenParameters
            {
                Issuer = Configuration["JwtSecurityToken:Issuer"],
                Audience = Configuration["JwtSecurityToken:Audience"],
                SigningKey = Configuration["JwtSecurityToken:SigningKey"],
                ExpirationInMinutes = Configuration.GetValue<int>("JwtSecurityToken:ExpirationInMinutes")
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

            app.UseAuthentication();

            //Add Middleware for Permission Manager. Must be placed between Authentication and Mvc
            app.UseSecurityUI<int>(options =>
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
