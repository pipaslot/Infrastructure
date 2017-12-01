using System;
using System.Collections.Generic;

namespace Pipaslot.Infrastructure.Security
{
    /// <summary>
    /// Current user working with application
    /// </summary>
    public interface IUserIdentity<TKey>
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
        /// Check if user has permission for resource. Only permission defined for this resource can be used
        /// </summary>
        /// <param name="resource">Resource instance</param >
        /// < param name="permissionEnum">Requested permission</param>
        /// <returns></returns>
        bool IsAllowed<TPermissions>(IResource<TKey, TPermissions> resource, TPermissions permissionEnum) where TPermissions: IConvertible;

        /// <summary>
        /// Check if user has permission for resource with specific ID
        /// </summary>
        /// <param name="resource">Resource class type</param >
        /// <param name="resourceIdentifier">Instance identifier</param>
        /// < param name="permissionEnum">Requested permission</param>
        /// <returns></returns>
        bool IsAllowed(Type resource, TKey resourceIdentifier, IConvertible permissionEnum);

        /// <summary>
        /// Return all Ids from resource for which user has required permision
        /// </summary>
        /// <param name="resource">IResource class type</param>
        /// <param name="permissionEnum">Requested permission</param>
        /// <returns></returns>
        IEnumerable<TKey> GetAllowedKeys(Type resource, IConvertible permissionEnum);

    }
}