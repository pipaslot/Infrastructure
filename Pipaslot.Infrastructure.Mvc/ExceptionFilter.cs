using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Pipaslot.Infrastructure.Mvc
{
    /// <inheritdoc />
    /// <summary>
    /// Catch all exceptions and converts them to json response
    /// </summary>
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is Exception)
            {
                var ex = context.Exception;
                context.Exception = null;
                var apiError = new ApiError(ex.Message);

                if (context.HttpContext.Response.StatusCode == 200)
                {
                    context.HttpContext.Response.StatusCode = 500;
                }
                context.Result = new JsonResult(apiError);
            }

            base.OnException(context);
        }

        /// <summary>
        /// Structure of Response
        /// </summary>
        public class ApiError
        {
            public string Message { get; set; }

            public ApiError(string message)
            {
                Message = message;
            }
        }
    }
}
