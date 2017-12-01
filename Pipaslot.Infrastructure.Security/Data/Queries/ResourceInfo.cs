using System;
using System.Collections.Generic;
using System.Text;

namespace Pipaslot.Infrastructure.Security.Data.Queries
{
    /// <summary>
    /// Summany information about resource
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class ResourceInfo<TKey>
    {
        /// <summary>
        /// Assigned role ID
        /// </summary>
        public TKey RoleId { get; set; }

        /// <summary>
        /// Unique name for object inherited from IResource
        /// </summary>
        public string UniquedName { get; set; }

        /// <summary>
        /// Resource instance identifier
        /// </summary>
        public TKey Identifier { get; set; }

        /// <summary>
        /// User friendly name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// User friendly description
        /// </summary>
        public string Description { get; set; }
    }
}
