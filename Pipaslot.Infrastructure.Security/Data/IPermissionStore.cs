using System;
using System.Collections.Generic;
using System.Text;

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
        /// <returns></returns>
        bool IsAllowed(TKey roleId, string resource, string permission);

        /// <summary>
        /// Check if role has assigned instance permission
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="resource"></param>
        /// <param name="resourceId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        bool IsAllowed(TKey roleId, string resource, TKey resourceId, string permission);
        
        /// <summary>
        /// Returns all resource ID for which user has assigner permission
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="resource"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        IEnumerable<TKey> GetAllowedResourceIds(TKey roleId, string resource, string permission);

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

        /// <summary>
        /// Count all resource instances, but ignores default key value
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        int GetResourceInstanceCount(string resourceName);

        /// <summary>
        /// Returns all stored Instance Ids for resource
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        List<TKey> GetAllResourceInstancesIds(string resource);
    }
}
