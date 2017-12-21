using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pipaslot.SecurityUI.ActionAbstraction
{
    interface IAction
    {
        Task ExecuteAsync(HttpContext context, IServiceProvider services);
    }
}
