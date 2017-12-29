using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public virtual async Task<bool> IsAllowedAsync(IEnumerable<TKey> roles, IConvertible permissionEnum, CancellationToken token = default(CancellationToken))
        {
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return await LoadCachedAsync(
                () => _permissionStore.IsAllowedAsync(roles, GLOBAL_RESOURCE_NAME, perm, token),
                roles,
                perm,
                GLOBAL_RESOURCE_NAME);
        }

        public virtual async Task<bool> IsAllowedAsync(IEnumerable<TKey> roles, Type resource, IConvertible permissionEnum, CancellationToken token = default(CancellationToken))
        {
            Helpers.CheckIfResourceHasAssignedPermission(resource, permissionEnum);
            var res = _namingConvertor.GetResourceUniqueName(resource);
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return await LoadCachedAsync(
                () => _permissionStore.IsAllowedAsync(roles, res, perm, token),
                roles,
                perm,
                res);
        }

        public virtual async Task<bool> IsAllowedAsync<TPermissions>(IEnumerable<TKey> roles, IResourceInstance<TKey, TPermissions> resourceInstance, TPermissions permissionEnum, CancellationToken token = default(CancellationToken)) where TPermissions : IConvertible
        {
            var res = _namingConvertor.GetResourceUniqueName(resourceInstance.GetType());
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return await LoadCachedAsync(
                () => _permissionStore.IsAllowedAsync(roles, res, resourceInstance.ResourceUniqueIdentifier, perm, token),
                roles,
                perm,
                res,
                resourceInstance.ResourceUniqueIdentifier.ToString());
        }

        public virtual async Task<bool> IsAllowedAsync(IEnumerable<TKey> roles, Type resource, TKey resourceIdentifier, IConvertible permissionEnum, CancellationToken token = default(CancellationToken))
        {
            Helpers.CheckIfResourceHasAssignedPermission(resource, permissionEnum);
            var res = _namingConvertor.GetResourceUniqueName(resource);
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return await LoadCachedAsync(
                () => _permissionStore.IsAllowedAsync(roles, res, resourceIdentifier, perm, token),
                roles,
                perm,
                res,
                resourceIdentifier.ToString());
        }

        public virtual Task<IEnumerable<TKey>> GetAllowedKeysAsync(IEnumerable<TKey> roles, Type resource, IConvertible permissionEnum, CancellationToken token = default(CancellationToken))
        {
            Helpers.CheckIfResourceHasAssignedPermission(resource, permissionEnum);
            var res = _namingConvertor.GetResourceUniqueName(resource);
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return _permissionStore.GetAllowedResourceIdsAsync(roles, res, perm, token);
        }

        #region Local Array Cache implementation

        private readonly Dictionary<string, bool> _privilegeCache = new Dictionary<string, bool>();

        private async Task<bool> LoadCachedAsync(Func<Task<bool?>> callback, IEnumerable<TKey> roleIds, string permission, string resource, string resourceId = "")
        {
            var key = string.Join("###", roleIds) + "#|#|#" + resource + "#|#|#" + permission + "#|#|#" + resourceId;
            if (_privilegeCache.ContainsKey(key))
            {
                return _privilegeCache[key];
            }
            var result = await callback() ?? false;
            _privilegeCache.Add(key, result);
            return result;
        }

        #endregion
    }
}
