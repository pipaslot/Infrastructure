using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pipaslot.Infrastructure.SecurityUI
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

        public async Task Invoke(HttpContext context)
        {

            if (!RequestingIndexPage(context.Request))
            {
                await _next(context);
                return;
            }

            await RespondWithIndexHtml(context.Response);
        }

        private bool RequestingIndexPage(HttpRequest request)
        {
            var expectedPath = $"/{_options.RoutePrefix}";
            return request.Path.Value.StartsWith(expectedPath);
        }
        private async Task RespondWithIndexHtml(HttpResponse response)
        {
            response.StatusCode = 200;
            response.ContentType = "text/html";
           await response.WriteAsync("Security Middleware");

            //using (var rawStream = _options.IndexStream())
            //{
            //    var rawText = new StreamReader(rawStream).ReadToEnd();
            //    var htmlBuilder = new StringBuilder(rawText);
            //    foreach (var entry in _options.IndexSettings.ToTemplateParameters())
            //    {
            //        htmlBuilder.Replace(entry.Key, entry.Value);
            //    }

            //    await response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
            //}
        }
    }
}
