using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Pipaslot.Infrastructure.SecurityUI.ActionAbstraction
{
    abstract class AJsonAction : IAction
    {
        public async Task ExecuteAsync(HttpContext context, IServiceProvider services)
        {
            var response = context.Response;

            response.ContentType = "application/json";
            var data = GetData(context, services);
                var json = JsonConvert.SerializeObject(data);
                await response.WriteAsync(json);
        }

        protected abstract object GetData(HttpContext context, IServiceProvider services);
    }
}
