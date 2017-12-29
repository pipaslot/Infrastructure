using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.SecurityUI
{
    public static class SecurityUIExtensions
    {
        public static IApplicationBuilder UseSecurityUI<TKey>(this IApplicationBuilder builder, Action<SecurityUIOptions> setup = null)
        {
            var options = new SecurityUIOptions();
            setup?.Invoke(options);
            return builder.UseMiddleware<SecurityUIMiddleware<TKey>>(options);
        }

        public static IServiceCollection AddSecurityUI<TKey>(this IServiceCollection services)
        {
            //Default identity is guest identity
            services.AddScoped<UserIdentity>(s =>
            {
                var store = (IRoleStore) s.GetService(typeof(IRoleStore));
                var guest = store.GetGuestRoleIdentifier();
                return new UserIdentity(guest,new List<object>(),UserIdentityType.Admin);
            });
            RegisterIfNotExists<ResourceRegistry>(services);
            RegisterIfNotExists<INamingConvertor, DefaultNamingConvertor<TKey>>(services);
            RegisterIfNotExists<IQueryFactory<IResourceInstanceQuery>, NullResourceInstanceQueryFactory>(services);
            RegisterIfNotExists<IPermissionManager<TKey>, PermissionManager<TKey>>(services);
            RegisterIfNotExists<IPermissionManager, PermissionManager<TKey>>(services);
            RegisterIfNotExists<IAuthorizator<TKey>, Authorizator<TKey>>(services);
            RegisterIfNotExists<IUser<TKey>, User<TKey>>(services);
            return services;
        }

        private static void RegisterIfNotExists<TInterface, TImplementation>(IServiceCollection services)
        {
            var serviceType = typeof(TInterface);
            if (services.All(s => s.ServiceType != serviceType))
            {
                var descriptor = new ServiceDescriptor(serviceType, typeof(TImplementation), ServiceLifetime.Scoped);
                services.Add(descriptor);
            }
        }


        private static void RegisterIfNotExists<TImplementation>(IServiceCollection services)
        {
            var serviceType = typeof(TImplementation);
            if (services.All(s => s.ServiceType != serviceType))
            {
                var descriptor = new ServiceDescriptor(serviceType, typeof(TImplementation), ServiceLifetime.Scoped);
                services.Add(descriptor);
            }
        }
    }
}
