using System;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.Security
{
    class PermissionManager<TKey> : IPermissionManager<TKey>
    {

        private readonly IPermissionStore<TKey> _permissionStore;
        private readonly INamingConvertor _namingConvertor;

        public PermissionManager(IPermissionStore<TKey> permissionStore, INamingConvertor namingConvertor = null)
        {
            _namingConvertor = namingConvertor ?? new DefaultNamingConvertor();
            _permissionStore = permissionStore;
        }

        public virtual void Allow(IUserRole<TKey> role, Type resource, TKey resourceId, IConvertible permissionEnum)
        {
            SetPrivilege(role, resource, resourceId, permissionEnum, true);
        }

        public virtual void Deny(IUserRole<TKey> role, Type resource, TKey resourceId, IConvertible permissionEnum)
        {
            SetPrivilege(role, resource, resourceId, permissionEnum, false);
        }

        protected virtual void SetPrivilege(IUserRole<TKey> role, Type resource, TKey resourceId, IConvertible permissionEnum, bool privilege)
        {
            Helpers.CheckIfResourceHasAssignedPermission(resource, permissionEnum);
            var res = _namingConvertor.GetResourceUniqueName(resource);
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            _permissionStore.SetPrivilege(role.Id, res, resourceId, perm, privilege);
        }
    }
}
