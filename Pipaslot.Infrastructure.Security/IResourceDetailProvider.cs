using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pipaslot.Infrastructure.Security
{
    public interface IResourceDetailProvider<TKey> : IResourceDetailProvider
    {
        Task<IEnumerable<ResourceDetail<TKey>>> GetResourceDetailsAsync(Type resource, IEnumerable<TKey> identifiers, CancellationToken token = default(CancellationToken));
    }

    /// <summary>
    /// Provides Name and Description for resource based on stored primary key
    /// </summary>
    public interface IResourceDetailProvider
    {
        Task<IEnumerable<ResourceDetail<object>>>GetResourceDetailsAsync(Type resource, IEnumerable<object> identifiers, CancellationToken token = default(CancellationToken));
    }
}
