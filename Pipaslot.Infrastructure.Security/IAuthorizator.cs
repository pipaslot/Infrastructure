using System;
using System.Collections.Generic;

namespace Pipaslot.Infrastructure.Security
{
    public interface IAuthorizator<TKey>
    {
        /// <summary>
        /// Check if user has permission
        /// </summary>
        /// <param name="role"></param>
        /// <param name="permissionEnum"></param>
        /// <returns></returns>
        bool IsAllowed(IUserRole<TKey> role, IConvertible permissionEnum);

        /// <summary>
        /// Check if user has permission for resource
        /// </summary>
        /// <param name="role"></param>
        /// <param name="resource" ></param>
        /// < param name="permissionEnum"></param>
        /// <returns></returns>
        bool IsAllowed<TPermissions>(IUserRole<TKey> role, IResource<TKey,TPermissions> resource, TPermissions permissionEnum) where TPermissions: IConvertible;

        /// <summary>
        /// Check if user has permission for resource with specific ID
        /// </summary>
        /// <param name="role"></param>
        /// <param name="resource">IResource Class Type</param>
        /// <param name="resourceIdentifier"></param>
        /// < param name="permissionEnum"></param>
        /// <returns></returns>
        bool IsAllowed(IUserRole<TKey> role, Type resource, TKey resourceIdentifier, IConvertible permissionEnum);

        /// <summary>
        /// Return all Ids from resource for which role has required permision
        /// </summary>
        /// <param name="role"></param>
        /// <param name="resource">IResource Class Type</param>
        /// <param name="permissionEnum"></param>
        /// <returns></returns>
        IEnumerable<TKey> GetAllowedKeys(IUserRole<TKey> role, Type resource, IConvertible permissionEnum);

    }
}