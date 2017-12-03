using System;
using System.Collections.Generic;
using System.Text;
using Pipaslot.Infrastructure.Security.Data.Queries;

namespace Pipaslot.Infrastructure.Security
{
    public interface IPermissionManager<TKey>
    {
        /// <summary>
        /// Returns information about existing resources and their instances amount
        /// </summary>
        /// <returns></returns>
        IEnumerable<ResourceInfo> GetAllResources();

        /// <summary>
        /// Get all resource instances, ignores resources with default TKey
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        IEnumerable<ResourceInstanceInfo<TKey>> GetAllResourceInstances(string resource);

        /// <summary>
        /// Returns all permission assigned to static resource and selected role
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        IEnumerable<PermissionInfo<TKey>> GetAllPermissions(TKey roleId, string resource);

        /// <summary>
        /// Returns all permission assigned to resource instance and selected role
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="resource"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        IEnumerable<PermissionInfo<TKey>> GetAllPermissions(TKey roleId, string resource, TKey resourceId);

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
}
