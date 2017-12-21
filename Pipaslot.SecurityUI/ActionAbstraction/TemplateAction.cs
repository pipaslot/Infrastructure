using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pipaslot.SecurityUI.ActionAbstraction
{
    class TemplateAction : AFileAction
    {
        public string RoutePrefix { get; set; }
        public string PageName { get; set; }

        public TemplateAction(string routePrefix, string pageName)
        {
            RoutePrefix = routePrefix;
            PageName = pageName;
        }

        public override async Task ExecuteAsync(HttpContext context, IServiceProvider services)
        {
            var response = context.Response;
            response.ContentType = "text/html";
            var layout = ReadResource("Templates.layout.html");
            var body = ReadResource($"Templates.{PageName}.html");
            var html = layout
                .Replace("{{pageBody}}", body)
                .Replace("{{routePrefix}}",RoutePrefix);
            
            await response.WriteAsync(html);
        }
    }
}
