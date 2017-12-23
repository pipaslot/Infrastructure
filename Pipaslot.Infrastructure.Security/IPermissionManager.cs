using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.Security
{
    public interface IPermissionManager<TKey> : IPermissionManager
    {
        /// <summary>
        /// Get all resource instances, ignores resources with default TKey
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<ResourceInstanceInfo<TKey>>> GetAllResourceInstancesAsync(string resource, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Returns all permission assigned to static resource and selected role
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="resource"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<PermissionInfo<TKey>>> GetAllPermissionsAsync(TKey roleId, string resource, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Returns all permission assigned to resource instance and selected role
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="resource"></param>
        /// <param name="resourceId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<PermissionInfo<TKey>>> GetAllPermissionsAsync(TKey roleId, string resource, TKey resourceId, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Grant Permission for role and static resource
        /// </summary>
        /// <param name="role"></param>
        /// <param name="resource"></param>
        /// <param name="permissionEnum"></param>
        void Allow(IUserRole<TKey> role, Type resource, IConvertible permissionEnum);

        /// <summary>
        /// Grant Permission for role and resource instance
        /// </summary>
        /// <param name="role"></param>
        /// <param name="resource"></param>
        /// <param name="resourceId"></param>
        /// <param name="permissionEnum"></param>
        void Allow(IUserRole<TKey> role, Type resource, TKey resourceId, IConvertible permissionEnum);

        /// <summary>
        /// Take role permission for static resource
        /// </summary>
        /// <param name="role"></param>
        /// <param name="resource"></param>
        /// <param name="permissionEnum"></param>
        void Deny(IUserRole<TKey> role, Type resource, IConvertible permissionEnum);

        /// <summary>
        /// Take role permission for resource instance
        /// </summary>
        /// <param name="role"></param>
        /// <param name="resource"></param>
        /// <param name="resourceId"></param>
        /// <param name="permissionEnum"></param>
        void Deny(IUserRole<TKey> role, Type resource, TKey resourceId, IConvertible permissionEnum);
    }

    public interface IPermissionManager
    {
        Task<IEnumerable<ResourceInfo>> GetAllResourcesAsync(CancellationToken token = default(CancellationToken));
    }
}
