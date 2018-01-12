using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Attributes;
using Pipaslot.SecurityUI.Privileges;

namespace Pipaslot.SecurityUI
{
    [Name("Security UI Extension")]
    [Description("Role and Permissio nmanagement")]
    public class SecurityUIMiddleware<TKey>
    {
        private readonly RequestDelegate _next;
        private readonly SecurityUIOptions _options;
        private bool _isInitialized;

        public SecurityUIMiddleware(RequestDelegate next, SecurityUIOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(HttpContext context, IServiceProvider services)
        {
            if (!_isInitialized)
            {
                Initialize(services);
                _isInitialized = true;
            }
            if (context.Request.Path.Value.StartsWith($"/{_options.RoutePrefix}"))
            {
                try
                {
                    var user = (IUser) services.GetService(typeof(IUser));
                    var router = new Router<TKey>(context.Request, _options.RoutePrefix, user);
                    var action = router.ResolveAction();
                    await action.ExecuteAsync(context, services);
                }
                catch (AuthorizationException)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                }
            }
            else
            {
                await _next(context);
            }
        }

        private void Initialize(IServiceProvider services)
        {
            var registry = (ResourceRegistry)services.GetService(typeof(ResourceRegistry));
            registry.Register(typeof(SecurityUIMiddleware<TKey>).Assembly);
        }
    }
}
