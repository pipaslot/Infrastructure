using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pipaslot.Infrastructure.Security.Data
{
    /// <summary>
    /// Storage used for storing privileges for resource permissions
    /// </summary>
    public interface IPermissionStore<TKey>
    {
        /// <summary>
        /// Check if role has assigned static permission
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="resource"></param>
        /// <param name="permission"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> IsAllowedAsync(TKey roleId, string resource, string permission, CancellationToken token = default(CancellationToken));
        

        /// <summary>
        /// Check if role has assigned static permission
        /// </summary>
        /// <param name="roleIds"></param>
        /// <param name="resource"></param>
        /// <param name="permission"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> IsAllowedAsync(IEnumerable<TKey> roleIds, string resource, string permission,CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Check if role has assigned instance permission
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="resource"></param>
        /// <param name="resourceId"></param>
        /// <param name="permission"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> IsAllowedAsync(TKey roleId, string resource, TKey resourceId, string permission, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Check if role has assigned static permission
        /// </summary>
        /// <param name="roleIds"></param>
        /// <param name="resource"></param>
        /// <param name="resourceId"></param>
        /// <param name="permission"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> IsAllowedAsync(IEnumerable<TKey> roleIds, string resource, TKey resourceId, string permission, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Returns all resource ID for which user has assigner permission
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="resource"></param>
        /// <param name="permission"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<TKey>> GetAllowedResourceIdsAsync(TKey roleId, string resource, string permission, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Returns all resource ID for which user has assigner permission
        /// </summary>
        /// <param name="roleIds"></param>
        /// <param name="resource"></param>
        /// <param name="permission"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<TKey>> GetAllowedResourceIdsAsync(IEnumerable<TKey> roleIds, string resource, string permission, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Allow or Deny permission for user and static resource
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="resource"></param>
        /// <param name="permission"></param>
        /// <param name="isAllowed"></param>
        void SetPrivilege(TKey roleId, string resource, string permission, bool isAllowed);

        /// <summary>
        /// Allow or Deny permission for user and resource instance
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="resource"></param>
        /// <param name="resourceId"></param>
        /// <param name="permission"></param>
        /// <param name="isAllowed"></param>
        void SetPrivilege(TKey roleId, string resource, TKey resourceId, string permission, bool isAllowed);
    }
}
