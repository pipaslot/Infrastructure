﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pipaslot.SecurityUI
{
    public class SecurityUIMiddleware
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
                var router = new Router(context.Request,_options.RoutePrefix);
                var action = router.ResolveAction();
                return action.ExecuteAsync(context, services);
            }
            return _next(context);
        }
    }
}