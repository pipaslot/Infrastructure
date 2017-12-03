using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.Security
{
    public class Authorizator<TKey> : IAuthorizator<TKey>
    {
        public const string GLOBAL_RESOURCE_NAME = "global";

        protected readonly IPermissionStore<TKey> _permissionStore;
        protected readonly INamingConvertor _namingConvertor;

        public Authorizator(IPermissionStore<TKey> permissionStore, INamingConvertor namingConvertor)
        {
            _permissionStore = permissionStore;
            _namingConvertor = namingConvertor;
        }

        public virtual bool IsAllowed(IUserRole<TKey> role, IConvertible permissionEnum)
        {
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return _permissionStore.IsAllowed(role.Id, GLOBAL_RESOURCE_NAME, perm);
        }

        public bool IsAllowed(IUserRole<TKey> role, Type resource, IConvertible permissionEnum)
        {
            Helpers.CheckIfResourceHasAssignedPermission(resource, permissionEnum);
            var res = _namingConvertor.GetResourceUniqueName(resource);
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return _permissionStore.IsAllowed(role.Id, res, perm);
        }

        public virtual bool IsAllowed<TPermissions>(IUserRole<TKey> role, IResourceInstance<TKey, TPermissions> resourceInstance, TPermissions permissionEnum) where TPermissions : IConvertible
        {
            var res = _namingConvertor.GetResourceUniqueName(resourceInstance.GetType());
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return _permissionStore.IsAllowed(role.Id, res, resourceInstance.ResourceUniqueIdentifier, perm);
        }

        public virtual bool IsAllowed(IUserRole<TKey> role, Type resource, TKey resourceIdentifier, IConvertible permissionEnum)
        {
            Helpers.CheckIfResourceHasAssignedPermission(resource, permissionEnum);
            var res = _namingConvertor.GetResourceUniqueName(resource);
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return _permissionStore.IsAllowed(role.Id, res, resourceIdentifier, perm);
        }

        public virtual IEnumerable<TKey> GetAllowedKeys(IUserRole<TKey> role, Type resource, IConvertible permissionEnum)
        {
            Helpers.CheckIfResourceHasAssignedPermission(resource, permissionEnum);
            var res = _namingConvertor.GetResourceUniqueName(resource);
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return _permissionStore.GetAllowedResourceIds(role.Id, res, perm);
        }
    }
}
