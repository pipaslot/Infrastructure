using System;
using Microsoft.AspNetCore.Http;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.SecurityUI.ActionAbstraction;

namespace Pipaslot.SecurityUI.Actions
{
    class RoleJsonAction : AJsonAction
    {
        protected override object GetData(HttpContext context, IServiceProvider services)
        {
            var roleQueryFactory = (IQueryFactory<IRoleQuery>)services.GetService(typeof(IQueryFactory<IRoleQuery>));
            if (roleQueryFactory == null)
            {
                throw new ApplicationException($"Can not resolve service {typeof(IQueryFactory<IRoleQuery>)} from Dependency Injection.");
            }
            var query = roleQueryFactory.Create();
            return query.Execute();
        }
    }
}
