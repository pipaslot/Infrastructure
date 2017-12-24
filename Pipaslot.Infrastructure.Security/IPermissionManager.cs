using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.Security
{
    public interface IPermissionManager<in TKey> : IPermissionManager
    {
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
        /// Get all resource instances, ignores resources with default TKey
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<ResourceInstanceInfo>> GetAllResourceInstancesAsync(string resource, CancellationToken token = default(CancellationToken));

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
