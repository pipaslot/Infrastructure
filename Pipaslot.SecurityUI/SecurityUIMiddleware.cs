using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pipaslot.Infrastructure.Security.Identities;

namespace Pipaslot.SecurityUI
{
    public class SecurityUIMiddleware<TKey>
    {
        private readonly RequestDelegate _next;
        private readonly SecurityUIOptions _options;

        public SecurityUIMiddleware(RequestDelegate next, SecurityUIOptions options)
        {
            _next = next;
            _options = options;
        }

        public Task Invoke(HttpContext context, IServiceProvider services)
        {
            if (context.Request.Path.Value.StartsWith($"/{_options.RoutePrefix}"))
            {
                var identity = (IIdentity<TKey>)services.GetService(typeof(IIdentity<TKey>));
                if (identity == null)
                {
                    throw new ApplicationException($"Can not resolve service {typeof(IIdentity<TKey>)} from Dependency Injection.");
                }
                if (!(identity is AdminIdentity<TKey>))
                {
                    throw new UnauthorizedAccessException("Only user with AdminIdentity can access Security Management.");
                }
                var router = new Router<TKey>(context.Request, _options.RoutePrefix);
                var action = router.ResolveAction();
                return action.ExecuteAsync(context, services);
            }
            return _next(context);
        }
    }
}
