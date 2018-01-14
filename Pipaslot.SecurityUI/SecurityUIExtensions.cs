using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
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
            RegisterIfNotExists<IClaimsPrincipalProvider, ClaimsPrincipalProvider>(services, ServiceLifetime.Singleton);
            RegisterIfNotExists<ResourceRegistry>(services, ServiceLifetime.Singleton);
            RegisterIfNotExists<INamingConvertor, DefaultNamingConvertor<TKey>>(services, ServiceLifetime.Singleton);
            RegisterIfNotExists<IResourceInstanceProvider, NullResourceInstanceProvider>(services, ServiceLifetime.Singleton);
            RegisterIfNotExists<IPermissionManager<TKey>, PermissionManager<TKey>>(services, ServiceLifetime.Singleton);
            services.AddSingleton(s => (IPermissionManager)s.GetService(typeof(IPermissionManager<TKey>)));
            RegisterIfNotExists<IUser<TKey>, User<TKey>>(services, ServiceLifetime.Singleton);
            services.AddSingleton(s => (IUser)s.GetService(typeof(IUser<TKey>)));

            return services;
        }

        private static void RegisterIfNotExists<TInterface, TImplementation>(IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var serviceType = typeof(TInterface);
            if (services.All(s => s.ServiceType != serviceType))
            {
                var descriptor = new ServiceDescriptor(serviceType, typeof(TImplementation), lifetime);
                services.Add(descriptor);
            }
        }


        private static void RegisterIfNotExists<TImplementation>(IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var serviceType = typeof(TImplementation);
            if (services.All(s => s.ServiceType != serviceType))
            {
                var descriptor = new ServiceDescriptor(serviceType, typeof(TImplementation), lifetime);
                services.Add(descriptor);
            }
        }
    }
}
