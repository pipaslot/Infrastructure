using System;
using Microsoft.AspNetCore.Http;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.SecurityUI.ActionAbstraction;

namespace Pipaslot.SecurityUI.Actions
{
    class ResourceJsonAction : AJsonAction
    {
        protected override object GetData(HttpContext context, IServiceProvider services)
        {
            var store = (IPermissionStore)services.GetService(typeof(IPermissionStore));
            if (store == null)
            {
                throw new ApplicationException($"Can not resolve service {typeof(IPermissionStore)} from Dependency Injection.");
            }
            return new object(); //store.GetAllResourceInstancesIdsAsync<>()
        }
    }
}
