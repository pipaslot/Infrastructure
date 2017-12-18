using System;
using System.Collections.Generic;

namespace Pipaslot.Infrastructure.Security
{
    public interface IAuthorizator<TKey>
    {
        /// <summary>
        /// Check if user has global permission without resource
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="permissionEnum"></param>
        /// <returns></returns>
        bool IsAllowed(IEnumerable<IUserRole<TKey>> roles, IConvertible permissionEnum);

        /// <summary>
        /// Check if user has static permission for resource. This permission is NOT assigned to identifiers
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="resource">IResource Class Type</param>
        /// < param name="permissionEnum"></param>
        /// <returns></returns>
        bool IsAllowed(IEnumerable<IUserRole<TKey>> roles, Type resource, IConvertible permissionEnum);

        /// <summary>
        /// Check if user has permission for resourceInstance
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="resourceInstance" ></param>
        /// < param name="permissionEnum"></param>
        /// <returns></returns>
        bool IsAllowed<TPermissions>(IEnumerable<IUserRole<TKey>> roles, IResourceInstance<TKey,TPermissions> resourceInstance, TPermissions permissionEnum) where TPermissions: IConvertible;

        /// <summary>
        /// Check if user has permission for resourceInstance with specific ID
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="resource">IResourceInstance Class Type</param>
        /// <param name="resourceIdentifier"></param>
        /// < param name="permissionEnum"></param>
        /// <returns></returns>
        bool IsAllowed(IEnumerable<IUserRole<TKey>> roles, Type resource, TKey resourceIdentifier, IConvertible permissionEnum);

        /// <summary>
        /// Return all Ids from resourceInstance for which role has required permision
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="resource">IResourceInstance Class Type</param>
        /// <param name="permissionEnum"></param>
        /// <returns></returns>
        IEnumerable<TKey> GetAllowedKeys(IEnumerable<IUserRole<TKey>> roles, Type resource, IConvertible permissionEnum);

    }
}