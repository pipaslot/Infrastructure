using System;
using System.Collections.Generic;
using System.Linq;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.Security
{
    /// <summary>
    /// This object must be recreated per request or per session. Do not use as Singleton !!!
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
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
            return LoadCached(
                () => _permissionStore.IsAllowed(roleIds, GLOBAL_RESOURCE_NAME, perm),
                roleIds,
                perm,
                GLOBAL_RESOURCE_NAME);
        }

        public virtual bool IsAllowed(IEnumerable<IUserRole<TKey>> roles, Type resource, IConvertible permissionEnum)
        {
            var roleIds = roles.Select(r => r.Id).ToList();
            Helpers.CheckIfResourceHasAssignedPermission(resource, permissionEnum);
            var res = _namingConvertor.GetResourceUniqueName(resource);
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return LoadCached(
                () => _permissionStore.IsAllowed(roleIds, res, perm),
                roleIds,
                perm,
                res);
        }

        public virtual bool IsAllowed<TPermissions>(IEnumerable<IUserRole<TKey>> roles, IResourceInstance<TKey, TPermissions> resourceInstance, TPermissions permissionEnum) where TPermissions : IConvertible
        {
            var roleIds = roles.Select(r => r.Id).ToList();
            var res = _namingConvertor.GetResourceUniqueName(resourceInstance.GetType());
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return LoadCached(
                () => _permissionStore.IsAllowed(roleIds, res, resourceInstance.ResourceUniqueIdentifier, perm),
                roleIds,
                perm,
                res,
                resourceInstance.ResourceUniqueIdentifier.ToString());
        }

        public virtual bool IsAllowed(IEnumerable<IUserRole<TKey>> roles, Type resource, TKey resourceIdentifier, IConvertible permissionEnum)
        {
            var roleIds = roles.Select(r => r.Id).ToList();
            Helpers.CheckIfResourceHasAssignedPermission(resource, permissionEnum);
            var res = _namingConvertor.GetResourceUniqueName(resource);
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return LoadCached(
                () => _permissionStore.IsAllowed(roleIds, res, resourceIdentifier, perm),
                roleIds,
                perm,
                res,
                resourceIdentifier.ToString());
        }

        public virtual IEnumerable<TKey> GetAllowedKeys(IEnumerable<IUserRole<TKey>> roles, Type resource, IConvertible permissionEnum)
        {
            var roleIds = roles.Select(r => r.Id).ToList();
            Helpers.CheckIfResourceHasAssignedPermission(resource, permissionEnum);
            var res = _namingConvertor.GetResourceUniqueName(resource);
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return _permissionStore.GetAllowedResourceIds(roleIds, res, perm);
        }

        #region Local Array Cache implementation

        private readonly Dictionary<string, bool> _privilegeCache = new Dictionary<string, bool>();

        private bool LoadCached(Func<bool> callback, IEnumerable<TKey> roleIds, string permission, string resource, string resourceId = "")
        {
            var key = string.Join("###", roleIds) + "#|#|#" + resource + "#|#|#" + permission + "#|#|#" + resourceId;
            if (_privilegeCache.ContainsKey(key))
            {
                return _privilegeCache[key];
            }
            var result = callback();
            _privilegeCache.Add(key, result);
            return result;
        }

        #endregion
    }
}
