using System;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.Security
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSecurity<TKey, TClaimsPrincipalProvider>(this IServiceCollection services)
            where TClaimsPrincipalProvider : IClaimsPrincipalProvider
        {
            services.AddSecurity<TKey, User<TKey>, TClaimsPrincipalProvider>();
            return services;
        }

        public static IServiceCollection AddSecurity<TKey, TUser, TClaimsPrincipalProvider>(this IServiceCollection services)
            where TUser : IUser<TKey>
            where TClaimsPrincipalProvider : IClaimsPrincipalProvider
        {
            RegisterIfNotExists<IClaimsPrincipalProvider, TClaimsPrincipalProvider>(services, ServiceLifetime.Singleton);
            RegisterIfNotExists<ResourceRegistry>(services, ServiceLifetime.Singleton);
            RegisterIfNotExists<INamingConvertor, DefaultNamingConvertor>(services, ServiceLifetime.Singleton);
            RegisterIfNotExists<IResourceInstanceProvider, NullResourceInstanceProvider>(services, ServiceLifetime.Singleton);
            RegisterIfNotExists<IPermissionManager<TKey>, PermissionManager<TKey>>(services, ServiceLifetime.Singleton);
            services.AddSingleton(s => (IPermissionManager)s.GetService(typeof(IPermissionManager<TKey>)));
            RegisterIfNotExists<TUser>(services, ServiceLifetime.Singleton);
            services.AddSingleton(s => (IUser<TKey>)s.GetService(typeof(TUser)));
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
