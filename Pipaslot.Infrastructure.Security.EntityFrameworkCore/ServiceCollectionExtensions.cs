using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.Infrastructure.Security.EntityFrameworkCore.Entities;

namespace Pipaslot.Infrastructure.Security.EntityFrameworkCore
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Minimalistic database registration for Security module
        /// </summary>
        public static IServiceCollection AddSecurityDatabase<TKey, TDbContext>(
            this IServiceCollection services)
            where TDbContext : DbContext, ISecurityPrivilegeDbContext<TKey, Privilege<TKey>>, ISecurityRoleDbContext<TKey, Role<TKey>>
        {
            return services.AddSecurityDatabase<TKey, TDbContext, RoleStore<TKey, TDbContext, Role<TKey>>, Role<TKey>, PermissionStore<TKey, TDbContext, Privilege<TKey>>, Privilege<TKey>>();
        }

        /// <summary>
        /// Minimalistic database registration for Security module (with custom role)
        /// </summary>
        public static IServiceCollection AddSecurityDatabase<TKey, TDbContext, TRole>(
            this IServiceCollection services)
            where TDbContext : DbContext, ISecurityPrivilegeDbContext<TKey, Privilege<TKey>>, ISecurityRoleDbContext<TKey, TRole>
            where TRole : Role<TKey>, new()
        {
            return services.AddSecurityDatabase<TKey, TDbContext, RoleStore<TKey, TDbContext, TRole>, TRole, PermissionStore<TKey, TDbContext, Privilege<TKey>>, Privilege<TKey>>();
        }

        /// <summary>
        /// Full database registration for Security module
        /// </summary>
        public static IServiceCollection AddSecurityDatabase<TKey, TDbContext, TRoleStore, TRole, TPermissionStore, TPrivilege>(this IServiceCollection services)
            where TDbContext : DbContext, ISecurityPrivilegeDbContext<TKey, TPrivilege>, ISecurityRoleDbContext<TKey, TRole>
            where TRole : Role<TKey>, new()
            where TPermissionStore : class, IPermissionStore<TKey>
            where TPrivilege : Privilege<TKey>, new()
        where TRoleStore : class, IRoleStore
        {
            services.AddSingleton<IPermissionStore<TKey>, TPermissionStore>();
            services.AddSingleton<IRoleStore, TRoleStore>();
            services.AddSingleton<IResourceInstanceProvider, ResourceInstanceProvider<TDbContext>>();
            return services;
        }
    }
}
