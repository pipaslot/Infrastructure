using System;
using Microsoft.AspNetCore.Http;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Security;
using Pipaslot.SecurityUI.ActionAbstraction;

namespace Pipaslot.SecurityUI.Actions
{
    class PrivilegeJsonAction<TKey> : AJsonAction
    {
        private readonly TKey _role;
        private readonly string _resource;
        private readonly TKey _resourceId;
        private readonly string _permission;
        private readonly bool? _isAllowed;

        public PrivilegeJsonAction(TKey role, string resource, TKey resourceId, string permission, bool? isAllowed)
        {
            _role = role;
            _resource = resource;
            _resourceId = resourceId;
            _permission = permission;
            _isAllowed = isAllowed;
        }

        protected override object GetData(HttpContext context, IServiceProvider services)
        {
            var unitOfWorkFactory = (IUnitOfWorkFactory)services.GetService(typeof(IUnitOfWorkFactory));
            if (unitOfWorkFactory == null)
            {
                throw new ApplicationException($"Can not resolve service {typeof(IUnitOfWorkFactory)} from Dependency Injection.");
            }
            var manager = (IPermissionManager<TKey>)services.GetService(typeof(IPermissionManager<TKey>));
            if (manager == null)
            {
                throw new ApplicationException($"Can not resolve service {typeof(IPermissionManager<TKey>)} from Dependency Injection.");
            }
            using (var uow = unitOfWorkFactory.Create())
            {
                if (!_resourceId.Equals(default(TKey)))
                {
                    manager.SetPermission(_role, _resource, _resourceId, _permission, _isAllowed);
                }
                else
                {
                    manager.SetPermission(_role, _resource, _permission, _isAllowed);
                }
                uow.Commit();
            }
            return true;
        }
    }
}
