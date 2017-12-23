using System;
using Pipaslot.Infrastructure.Data.Queries;

namespace Pipaslot.Infrastructure.Security.Data
{
    /// <summary>
    /// Provides informations about instances of resource with permission er every single resource instance
    /// </summary>
    public interface IResourceInstanceQuery : IQuery<ResourceInstance>
    {
        /// <summary>
        /// Resource type for specification one type of resource
        /// </summary>
        Type Resource { get; set; }

        /// <summary>
        /// Primary key used for finding single resource
        /// </summary>
        object ResourceIdentifier { get; set; }
    }
}
