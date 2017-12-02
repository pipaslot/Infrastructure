using System;
using System.Collections.Generic;
using System.Text;

namespace Pipaslot.Infrastructure.Security
{
    /// <summary>
    /// Provides Name and Description for resource based on stored primary key
    /// </summary>
    public interface IResourceDetailProvider<TKey>
    {
        IEnumerable<ResourceDetail<TKey>> GetResourceDetails(Type resource, List<TKey> identifiers);
    }
}
