using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.Security
{
    public interface IPermissionManager<TKey> : IPermissionManager
    {
        /// <summary>
        /// Automatically resolve resource and check if user has permission.
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="permissionEnum"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> IsAllowedAsync(IEnumerable<TKey> roles, IConvertible permissionEnum, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Check if user has static permission for resource. This permission is NOT assigned to identifiers
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="resource">IResource Class Type</param>
        /// <param name="permissionEnum"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> IsAllowedAsync(IEnumerable<TKey> roles, Type resource, IConvertible permissionEnum, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Check if user has permission for resourceInstance
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="resourceInstance" ></param>
        /// <param name="permissionEnum"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> IsAllowedAsync<TPermissions>(IEnumerable<TKey> roles, IResourceInstance<TPermissions> resourceInstance, TPermissions permissionEnum, CancellationToken token = default(CancellationToken)) where TPermissions : IConvertible;

        /// <summary>
        /// Check if user has permission for resourceInstance with specific ID
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="resource">IResourceInstance Class Type</param>
        /// <param name="resourceIdentifier"></param>
        /// <param name="permissionEnum"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> IsAllowedAsync(IEnumerable<TKey> roles, Type resource, TKey resourceIdentifier, IConvertible permissionEnum, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Return all Ids from resourceInstance for which role has required permision
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="resource">IResourceInstance Class Type</param>
        /// <param name="permissionEnum"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<TKey>> GetAllowedKeysAsync(IEnumerable<TKey> roles, Type resource, IConvertible permissionEnum, CancellationToken token = default(CancellationToken));
        
        /// <summary>
        /// Grant Permission for role and static resource
        /// </summary>
        /// <param name="role"></param>
        /// <param name="resource"></param>
        /// <param name="permission"></param>
        /// <param name="isEnabled"></param>
        void SetPermission(TKey role, string resource, string permission, bool? isEnabled);

        /// <summary>
        /// Grant Permission for role and resource instance
        /// </summary>
        /// <param name="role"></param>
        /// <param name="resource"></param>
        /// <param name="resourceId"></param>
        /// <param name="permission"></param>
        /// <param name="isEnabled"></param>
        void SetPermission(TKey role, string resource, TKey resourceId, string permission, bool? isEnabled);

    }

    public interface IPermissionManager
    {
        /// <summary>
        /// Get all resource types
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<ResourceInfo>> GetAllResourcesAsync(CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Get all resource instances in paged result, ignores resources with default TKey
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="pageSize"></param>
        /// <param name="token"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        Task<IEnumerable<ResourceInstanceInfo>> GetAllResourceInstancesAsync(string resource, int pageIndex = 1, int pageSize = 10, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Returns all permission assigned to static resource and selected role
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="resource"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<PermissionInfo>> GetAllPermissionsAsync(object roleId, string resource, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Returns all permission assigned to resource instance and selected role
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="resource"></param>
        /// <param name="resourceId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<PermissionInfo>> GetAllPermissionsAsync(object roleId, string resource, object resourceId, CancellationToken token = default(CancellationToken));

    }
}
