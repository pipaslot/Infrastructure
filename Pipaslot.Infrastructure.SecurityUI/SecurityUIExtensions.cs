using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Pipaslot.Infrastructure.Security;

namespace Pipaslot.Infrastructure.SecurityUI
{
    public static class SecurityUIExtensions
    {
        public static IApplicationBuilder UseSecurityUI(this IApplicationBuilder builder, Action<SecurityUIOptions> setup = null)
        {
            var options = new SecurityUIOptions();
            setup?.Invoke(options);
            return builder.UseMiddleware<SecurityUIMiddleware>(options);
        }

        public static IServiceCollection AddSecurityUI<TKey>(this IServiceCollection services)
        {
            RegisterIfNotExists<ResourceRegistry<TKey>, ResourceRegistry<TKey>>(services);
            RegisterIfNotExists<INamingConvertor,DefaultNamingConvertor<TKey>>(services);
            RegisterIfNotExists<IPermissionManager<TKey>, PermissionManager<TKey>>(services);
            RegisterIfNotExists<IAuthorizator<TKey>, Authorizator<TKey>>(services);
            RegisterIfNotExists<IUser<TKey>, User<TKey>>(services);
            return services;
        }

        private static void RegisterIfNotExists<TInterface, TImplementation>(IServiceCollection services)
        {
            var serviceType = typeof(TInterface);
            if (services.All(s => s.ServiceType != serviceType))
            {
                var descriptor = new ServiceDescriptor(serviceType, typeof(TImplementation), ServiceLifetime.Singleton);
                services.Add(descriptor);
            }
        }
    }
}
