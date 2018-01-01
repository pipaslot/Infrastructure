using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pipaslot.Infrastructure.Security.Data
{
    public interface IResourceInstanceProvider
    {
        /// <summary>
        /// Returns count of all existing resource instances of passed type
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<int> GetInstanceCountAsync(Type resource, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Returns all instance IDs for rosource of passed type
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<List<object>> GetAllIdsAsync(Type resource, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Returns resource Details fo resource type and instance
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IResourceInstance> GetInstanceAsync(Type resource, object id, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Returns informations about instances in paged result
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<List<IResourceInstance>> GetInstancesAsync(Type resource, int pageIndex = 1, int pageSize = 20, CancellationToken token = default(CancellationToken) );
    }
}
