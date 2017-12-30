using System;
using System.Collections.Generic;
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
            var result = new List<object>();
            foreach (var role in store.GetAll<IRole>())
            {
                result.Add(new
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    ShowResources = role.Type != RoleType.Admin,
                    IsSystem = role.Type != RoleType.Custom
                });
            }
            return result;
        }
    }
}
