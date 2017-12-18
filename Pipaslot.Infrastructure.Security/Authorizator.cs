using System;
using System.Collections.Generic;
using System.Linq;
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

        public virtual bool IsAllowed(IEnumerable<IUserRole<TKey>> roles, IConvertible permissionEnum)
        {
            var roleIds = roles.Select(r => r.Id).ToList();
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return _permissionStore.IsAllowed(roleIds, GLOBAL_RESOURCE_NAME, perm);
        }

        public bool IsAllowed(IEnumerable<IUserRole<TKey>> roles, Type resource, IConvertible permissionEnum)
        {
            var roleIds = roles.Select(r => r.Id).ToList();
            Helpers.CheckIfResourceHasAssignedPermission(resource, permissionEnum);
            var res = _namingConvertor.GetResourceUniqueName(resource);
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return _permissionStore.IsAllowed(roleIds, res, perm);
        }

        public virtual bool IsAllowed<TPermissions>(IEnumerable<IUserRole<TKey>> roles, IResourceInstance<TKey, TPermissions> resourceInstance, TPermissions permissionEnum) where TPermissions : IConvertible
        {
            var roleIds = roles.Select(r => r.Id).ToList();
            var res = _namingConvertor.GetResourceUniqueName(resourceInstance.GetType());
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return _permissionStore.IsAllowed(roleIds, res, resourceInstance.ResourceUniqueIdentifier, perm);
        }

        public virtual bool IsAllowed(IEnumerable<IUserRole<TKey>> roles, Type resource, TKey resourceIdentifier, IConvertible permissionEnum)
        {
            var roleIds = roles.Select(r => r.Id).ToList();
            Helpers.CheckIfResourceHasAssignedPermission(resource, permissionEnum);
            var res = _namingConvertor.GetResourceUniqueName(resource);
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return _permissionStore.IsAllowed(roleIds, res, resourceIdentifier, perm);
        }

        public virtual IEnumerable<TKey> GetAllowedKeys(IEnumerable<IUserRole<TKey>> roles, Type resource, IConvertible permissionEnum)
        {
            var roleIds = roles.Select(r => r.Id).ToList();
            Helpers.CheckIfResourceHasAssignedPermission(resource, permissionEnum);
            var res = _namingConvertor.GetResourceUniqueName(resource);
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return _permissionStore.GetAllowedResourceIds(roleIds, res, perm);
        }
    }
}
