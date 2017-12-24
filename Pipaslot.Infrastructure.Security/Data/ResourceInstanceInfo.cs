using System;
using System.Collections.Generic;
using System.Text;

namespace Pipaslot.Infrastructure.Security.Data
{
    /// <summary>
    /// Summany information about resource
    /// </summary>
    public class ResourceInstanceInfo
    {
        /// <summary>
        /// Resource instance identifier
        /// </summary>
        public object Identifier { get; set; }

        /// <summary>
        /// User friendly name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// User friendly description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Number of assigned permissions
        /// </summary>
        public int PermissionsCount { get; set; }
    }
}
