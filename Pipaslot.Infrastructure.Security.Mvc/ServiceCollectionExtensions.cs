using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Pipaslot.Infrastructure.Security.Mvc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSecurity<TKey>(this IServiceCollection services)
        {
            services.AddSecurity<TKey, User<TKey>>();
            return services;
        }

        public static IServiceCollection AddSecurity<TKey, TUser>(this IServiceCollection services)
            where TUser: IUser<TKey>
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSecurity<TKey, TUser, ClaimsPrincipalProvider>();

            return services;
        }
    }
}
