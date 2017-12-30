using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pipaslot.Infrastructure.Security
{
    public interface IAuthorizator<TKey>
    {
        /// <summary>
        /// Check if user has global permission without resource
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="permissionEnum"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> IsAllowedAsync(IEnumerable<TKey> roles, IConvertible permissionEnum, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Check if user has static permission for resource. This permission is NOT assigned to identifiers
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="resource">IResource Class Type</param>
        /// <param name="permissionEnum"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> IsAllowedAsync(IEnumerable<TKey> roles, Type resource, IConvertible permissionEnum, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Check if user has permission for resourceInstance
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="resourceInstance" ></param>
        /// <param name="permissionEnum"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> IsAllowedAsync<TPermissions>(IEnumerable<TKey> roles, IResourceInstance<TKey, TPermissions> resourceInstance, TPermissions permissionEnum, CancellationToken token = default(CancellationToken)) where TPermissions : IConvertible;
        
        /// <summary>
        /// Check if user has permission for resourceInstance with specific ID
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="resource">IResourceInstance Class Type</param>
        /// <param name="resourceIdentifier"></param>
        /// <param name="permissionEnum"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> IsAllowedAsync(IEnumerable<TKey> roles, Type resource, TKey resourceIdentifier, IConvertible permissionEnum, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Return all Ids from resourceInstance for which role has required permision
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="resource">IResourceInstance Class Type</param>
        /// <param name="permissionEnum"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<TKey>> GetAllowedKeysAsync(IEnumerable<TKey> roles, Type resource, IConvertible permissionEnum, CancellationToken token = default(CancellationToken));

    }
}