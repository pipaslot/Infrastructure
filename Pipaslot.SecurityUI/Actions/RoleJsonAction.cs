using System;
using Microsoft.AspNetCore.Http;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.SecurityUI.ActionAbstraction;

namespace Pipaslot.SecurityUI.Actions
{
    class RoleJsonAction : AJsonAction
    {
        protected override object GetData(HttpContext context, IServiceProvider services)
        {
            var store = (IRoleStore)services.GetService(typeof(IRoleStore));
            if (store == null)
            {
                throw new ApplicationException($"Can not resolve service {typeof(IRoleStore)} from Dependency Injection.");
            }
            return store.GetAll<IRole>();
        }
    }
}
