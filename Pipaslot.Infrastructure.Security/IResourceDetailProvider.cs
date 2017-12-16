using System;
using System.Collections.Generic;

namespace Pipaslot.Infrastructure.Security
{
    public interface IResourceDetailProvider<TKey> : IResourceDetailProvider
    {
        IEnumerable<ResourceDetail<TKey>> GetResourceDetails(Type resource, List<TKey> identifiers);
    }

    /// <summary>
    /// Provides Name and Description for resource based on stored primary key
    /// </summary>
    public interface IResourceDetailProvider
    {
        IEnumerable<ResourceDetail<object>>GetResourceDetails(Type resource, List<object> identifiers);
    }
}
