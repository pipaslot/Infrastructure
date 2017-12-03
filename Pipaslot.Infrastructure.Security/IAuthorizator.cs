using System;
using System.Collections.Generic;

namespace Pipaslot.Infrastructure.Security
{
    public interface IAuthorizator<TKey>
    {
        /// <summary>
        /// Check if user has global permission without resource
        /// </summary>
        /// <param name="role"></param>
        /// <param name="permissionEnum"></param>
        /// <returns></returns>
        bool IsAllowed(IUserRole<TKey> role, IConvertible permissionEnum);
        
        /// <summary>
        /// Check if user has static permission for resource. This permission is NOT assigned to identifiers
        /// </summary>
        /// <param name="role"></param>
        /// <param name="resource">IResource Class Type</param>
        /// < param name="permissionEnum"></param>
        /// <returns></returns>
        bool IsAllowed(IUserRole<TKey> role, Type resource, IConvertible permissionEnum);

        /// <summary>
        /// Check if user has permission for resourceInstance
        /// </summary>
        /// <param name="role"></param>
        /// <param name="resourceInstance" ></param>
        /// < param name="permissionEnum"></param>
        /// <returns></returns>
        bool IsAllowed<TPermissions>(IUserRole<TKey> role, IResourceInstance<TKey,TPermissions> resourceInstance, TPermissions permissionEnum) where TPermissions: IConvertible;

        /// <summary>
        /// Check if user has permission for resourceInstance with specific ID
        /// </summary>
        /// <param name="role"></param>
        /// <param name="resource">IResourceInstance Class Type</param>
        /// <param name="resourceIdentifier"></param>
        /// < param name="permissionEnum"></param>
        /// <returns></returns>
        bool IsAllowed(IUserRole<TKey> role, Type resource, TKey resourceIdentifier, IConvertible permissionEnum);

        /// <summary>
        /// Return all Ids from resourceInstance for which role has required permision
        /// </summary>
        /// <param name="role"></param>
        /// <param name="resource">IResourceInstance Class Type</param>
        /// <param name="permissionEnum"></param>
        /// <returns></returns>
        IEnumerable<TKey> GetAllowedKeys(IUserRole<TKey> role, Type resource, IConvertible permissionEnum);

    }
}