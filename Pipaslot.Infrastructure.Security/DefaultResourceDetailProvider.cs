using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipaslot.Infrastructure.Security
{
    /// <inheritdoc />
    /// <summary>
    /// Default Resource Detail provider which returns only id as resource name
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class DefaultResourceDetailProvider<TKey> : IResourceDetailProvider<TKey>
    {
        public IEnumerable<ResourceDetail<TKey>> GetResourceDetails(List<TKey> identifiers)
        {
            return identifiers.Select(id => new ResourceDetail<TKey>(id, id.ToString(), string.Empty));
        }
    }
}
