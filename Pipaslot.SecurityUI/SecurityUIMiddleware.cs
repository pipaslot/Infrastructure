using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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
                //TODO SECURE BY APPROPRIATE PERMISSIOn
                //var identity = (UserIdentity)services.GetService(typeof(UserIdentity));
                //if (identity == null)
                //{
                //    throw new ApplicationException($"Can not resolve service {typeof(UserIdentity)} from Dependency Injection.");
                //}
                //if (identity.Roles.Any(r=>r.Type == RoleType.Admin))
                //{
                //    throw new UnauthorizedAccessException("Only user with AdminIdentity can access Security Management.");
                //}
                var router = new Router<TKey>(context.Request, _options.RoutePrefix);
                var action = router.ResolveAction();
                return action.ExecuteAsync(context, services);
            }
            return _next(context);
        }
    }
}
