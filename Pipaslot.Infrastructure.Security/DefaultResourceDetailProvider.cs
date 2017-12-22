using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pipaslot.Infrastructure.Security
{
    /// <inheritdoc />
    /// <summary>
    /// Default Resource Detail provider which returns only id as resource name
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class DefaultResourceDetailProvider<TKey> : IResourceDetailProvider<TKey>
    {
        public Task<IEnumerable<ResourceDetail<TKey>>> GetResourceDetailsAsync(Type resource, IEnumerable<TKey> identifiers, CancellationToken token = default(CancellationToken))
        {
            var result = identifiers.Select(id => new ResourceDetail<TKey>(id, id.ToString(), string.Empty));
            return Task.FromResult(result);
        }

        public Task<IEnumerable<ResourceDetail<object>>> GetResourceDetailsAsync(Type resource, IEnumerable<object> identifiers, CancellationToken token = default(CancellationToken))
        {
            var result = identifiers.Select(id => new ResourceDetail<object>(id, id.ToString(), string.Empty));
            return Task.FromResult(result);
        }
    }
}
