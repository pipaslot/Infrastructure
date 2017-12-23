using System;
using Microsoft.AspNetCore.Http;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.SecurityUI.ActionAbstraction;

namespace Pipaslot.SecurityUI.Actions
{
    class ResourceInstanceJsonAction : AJsonAction
    {
        private readonly string _resourceName;

        public ResourceInstanceJsonAction(string resourceName)
        {
            _resourceName = resourceName;
        }

        protected override object GetData(HttpContext context, IServiceProvider services)
        {
            var queryFactory = (IQueryFactory<IResourceInstanceQuery>)services.GetService(typeof(IQueryFactory<IResourceInstanceQuery>));
            if (queryFactory == null)
            {
                throw new ApplicationException($"Can not resolve service {typeof(IQueryFactory<IResourceInstanceQuery>)} from Dependency Injection.");
            }
            var namingConvertor = (INamingConvertor)services.GetService(typeof(INamingConvertor));
            if (namingConvertor == null)
            {
                throw new ApplicationException($"Can not resolve service {typeof(INamingConvertor)} from Dependency Injection.");
            }
            var query = queryFactory.Create();
            query.Resource = namingConvertor.GetResourceTypeByUniqueName(_resourceName);
            return query.Execute();
        }
    }
}
