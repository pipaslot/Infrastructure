using System;
using System.Collections.Generic;

namespace Pipaslot.Infrastructure.Security
{
    /// <summary>
    /// Operations working with current user
    /// </summary>
    public interface IUser<TKey>
    {
        /// <summary>
        /// User identifier
        /// </summary>
        TKey Id { get; }

        /// <summary>
        /// Flag if user was successfully authenticated
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Roles assigned to user 
        /// </summary>
        IEnumerable<IUserRole<TKey>> Roles { get; }

        /// <summary>
        /// Check if user has permission
        /// </summary>
        /// <param name="permissionEnum">Requested permission</param>
        /// <returns></returns>
        bool IsAllowed(IConvertible permissionEnum);

        /// <summary>
        /// Check if user has static permission for resource. This permission is NOT assigned to identifiers
        /// </summary>
        /// <param name="resource">IResource Class Type</param>
        /// < param name="permissionEnum"></param>
        /// <returns></returns>
        bool IsAllowed(Type resource, IConvertible permissionEnum);

        /// <summary>
        /// Check if user has permission for resourceInstance. Only permission defined for this resourceInstance can be used
        /// </summary>
        /// <param name="resourceInstance">Resource instance</param >
        /// < param name="permissionEnum">Requested permission</param>
        /// <returns></returns>
        bool IsAllowed<TPermissions>(IResourceInstance<TKey, TPermissions> resourceInstance, TPermissions permissionEnum) where TPermissions: IConvertible;

        /// <summary>
        /// Check if user has permission for resourceInstance with specific ID
        /// </summary>
        /// <param name="resource">Resource class type</param >
        /// <param name="resourceIdentifier">Instance identifier</param>
        /// < param name="permissionEnum">Requested permission</param>
        /// <returns></returns>
        bool IsAllowed(Type resource, TKey resourceIdentifier, IConvertible permissionEnum);

        /// <summary>
        /// Return all Ids from resourceInstance for which user has required permision
        /// </summary>
        /// <param name="resource">IResourceInstance class type</param>
        /// <param name="permissionEnum">Requested permission</param>
        /// <returns></returns>
        IEnumerable<TKey> GetAllowedKeys(Type resource, IConvertible permissionEnum);

    }
}