using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pipaslot.Infrastructure.Security;

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
            services.AddSecurity<TKey, ClaimsPrincipalProvider>();
            return services;
        }

        public static IServiceCollection AddSecurityUI<TKey, TUser>(this IServiceCollection services)
            where TUser : IUser<TKey>
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSecurity<TKey, TUser, ClaimsPrincipalProvider>();
            return services;
        }
    }
}
