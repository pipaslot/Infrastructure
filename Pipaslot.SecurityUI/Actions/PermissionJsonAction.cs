using System;
using Microsoft.AspNetCore.Http;
using Pipaslot.Infrastructure.Security;
using Pipaslot.SecurityUI.ActionAbstraction;

namespace Pipaslot.SecurityUI.Actions
{
    class PermissionJsonAction<TKey> : AJsonAction
    {
        private readonly TKey _role;
        private readonly string _resource;
        private readonly TKey _resourceId;

        public PermissionJsonAction(TKey role, string resource, TKey resourceId)
        {
            _role = role;
            _resource = resource;
            _resourceId = resourceId;
        }

        protected override object GetData(HttpContext context, IServiceProvider services)
        {
            var manager = (IPermissionManager)services.GetService(typeof(IPermissionManager));
            if (manager == null)
            {
                throw new ApplicationException($"Can not resolve service {typeof(IPermissionManager)} from Dependency Injection.");
            }
            if (!_resourceId.Equals(default(TKey)))
            {
                return manager.GetAllPermissionsAsync(_role, _resource, _resourceId).Result;
            }
            return manager.GetAllPermissionsAsync(_role, _resource).Result;
        }
    }
}
