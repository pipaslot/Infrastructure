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

        public virtual async Task<bool> IsAllowedAsync(IEnumerable<IUserRole<TKey>> roles, IConvertible permissionEnum, CancellationToken token = default(CancellationToken))
        {
            var roleIds = roles.Select(r => r.Id).ToList();
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return await LoadCachedAsync(
                () => _permissionStore.IsAllowedAsync(roleIds, GLOBAL_RESOURCE_NAME, perm, token),
                roleIds,
                perm,
                GLOBAL_RESOURCE_NAME);
        }

        public virtual async Task<bool> IsAllowedAsync(IEnumerable<IUserRole<TKey>> roles, Type resource, IConvertible permissionEnum, CancellationToken token = default(CancellationToken))
        {
            var roleIds = roles.Select(r => r.Id).ToList();
            Helpers.CheckIfResourceHasAssignedPermission(resource, permissionEnum);
            var res = _namingConvertor.GetResourceUniqueName(resource);
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return await LoadCachedAsync(
                () => _permissionStore.IsAllowedAsync(roleIds, res, perm, token),
                roleIds,
                perm,
                res);
        }

        public virtual async Task<bool> IsAllowedAsync<TPermissions>(IEnumerable<IUserRole<TKey>> roles, IResourceInstance<TKey, TPermissions> resourceInstance, TPermissions permissionEnum, CancellationToken token = default(CancellationToken)) where TPermissions : IConvertible
        {
            var roleIds = roles.Select(r => r.Id).ToList();
            var res = _namingConvertor.GetResourceUniqueName(resourceInstance.GetType());
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return await LoadCachedAsync(
                () => _permissionStore.IsAllowedAsync(roleIds, res, resourceInstance.ResourceUniqueIdentifier, perm, token),
                roleIds,
                perm,
                res,
                resourceInstance.ResourceUniqueIdentifier.ToString());
        }

        public virtual async Task<bool> IsAllowedAsync(IEnumerable<IUserRole<TKey>> roles, Type resource, TKey resourceIdentifier, IConvertible permissionEnum, CancellationToken token = default(CancellationToken))
        {
            var roleIds = roles.Select(r => r.Id).ToList();
            Helpers.CheckIfResourceHasAssignedPermission(resource, permissionEnum);
            var res = _namingConvertor.GetResourceUniqueName(resource);
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return await LoadCachedAsync(
                () => _permissionStore.IsAllowedAsync(roleIds, res, resourceIdentifier, perm, token),
                roleIds,
                perm,
                res,
                resourceIdentifier.ToString());
        }

        public virtual Task<IEnumerable<TKey>> GetAllowedKeysAsync(IEnumerable<IUserRole<TKey>> roles, Type resource, IConvertible permissionEnum, CancellationToken token = default(CancellationToken))
        {
            var roleIds = roles.Select(r => r.Id).ToList();
            Helpers.CheckIfResourceHasAssignedPermission(resource, permissionEnum);
            var res = _namingConvertor.GetResourceUniqueName(resource);
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return _permissionStore.GetAllowedResourceIdsAsync(roleIds, res, perm, token);
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
