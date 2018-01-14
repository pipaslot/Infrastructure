using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.Infrastructure.Security.Exceptions;

namespace Pipaslot.Infrastructure.Security
{
    public interface IUser<TKey> : IUser
    {
        /// <summary>
        /// User identifier
        /// </summary>
        TKey Id { get; }
        
        /// <summary>
        /// Roles assigned to user 
        /// </summary>
        IEnumerable<IRole> Roles { get; }
        
        /// <summary>
        ///  Check if User has got required permission, if not, then Exception is thrown.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="resourceIdentifier"></param>
        /// <param name="permissionEnum"></param>
        /// <param name="token"></param>
        /// <exception cref="AuthorizationException"></exception>
        Task CheckPermissionAsync(Type resource, TKey resourceIdentifier, IConvertible permissionEnum, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Check if user has permission for resourceInstance with specific ID
        /// </summary>
        /// <param name="resource">Resource class type</param >
        /// <param name="resourceIdentifier">Instance identifier</param>
        /// < param name="permissionEnum">Requested permission</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> IsAllowedAsync(Type resource, TKey resourceIdentifier, IConvertible permissionEnum, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Return all Ids from resourceInstance for which user has required permision
        /// </summary>
        /// <param name="resource">IResourceInstance class type</param>
        /// <param name="permissionEnum">Requested permission</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<TKey>> GetAllowedKeysAsync(Type resource, IConvertible permissionEnum, CancellationToken token = default(CancellationToken));
    }

    /// <summary>
    /// Operations working with current user
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// Flag if user was successfully authenticated
        /// </summary>
        bool IsAuthenticated { get; }
        
        /// <summary>
        /// Check if User has got required permission, if not, then Exception is thrown.
        /// </summary>
        /// <param name="permissionEnum"></param>
        /// <param name="token"></param>
        /// <exception cref="AuthorizationException"></exception>
        Task CheckPermissionAsync(IConvertible permissionEnum, CancellationToken token = default(CancellationToken));

        /// <summary>
        ///  Check if User has got required permission, if not, then Exception is thrown.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="permissionEnum"></param>
        /// <param name="token"></param>
        /// <exception cref="AuthorizationException"></exception>
        Task CheckPermissionAsync(Type resource, IConvertible permissionEnum, CancellationToken token = default(CancellationToken));

        /// <summary>
        ///  Check if User has got required permission, if not, then Exception is thrown.
        /// </summary>
        /// <typeparam name="TPermissions"></typeparam>
        /// <param name="resourceInstance"></param>
        /// <param name="permissionEnum"></param>
        /// <param name="token"></param>
        /// <exception cref="AuthorizationException"></exception>
        Task CheckPermissionAsync<TPermissions>(IResourceInstance<TPermissions> resourceInstance, TPermissions permissionEnum, CancellationToken token = default(CancellationToken)) where TPermissions : IConvertible;

        /// <summary>
        /// Check if user has permission
        /// </summary>
        /// <param name="permissionEnum">Requested permission</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> IsAllowedAsync(IConvertible permissionEnum, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Check if user has static permission for resource. This permission is NOT assigned to identifiers
        /// </summary>
        /// <param name="resource">IResource Class Type</param>
        /// < param name="permissionEnum"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> IsAllowedAsync(Type resource, IConvertible permissionEnum, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Check if user has permission for resourceInstance. Only permission defined for this resourceInstance can be used
        /// </summary>
        /// <param name="resourceInstance">Resource instance</param >
        /// < param name="permissionEnum">Requested permission</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> IsAllowedAsync<TPermissions>(IResourceInstance<TPermissions> resourceInstance, TPermissions permissionEnum, CancellationToken token = default(CancellationToken)) where TPermissions : IConvertible;
    }
}