using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Pipaslot.Infrastructure.Security.Mvc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSecurity<TKey>(this IServiceCollection services, Action<ResourceRegistry> resourceRegistration = null)
        {
            services.AddSecurity<TKey, User<TKey>>(resourceRegistration);
            return services;
        }

        public static IServiceCollection AddSecurity<TKey, TUser>(this IServiceCollection services, Action<ResourceRegistry> resourceRegistration = null)
            where TUser : IUser<TKey>
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSecurity<TKey, TUser, ClaimsPrincipalProvider>(resourceRegistration);

            return services;
        }
    }
}
