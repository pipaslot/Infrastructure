﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Pipaslot.Infrastructure.Security.Data.Queries
{
    /// <summary>
    /// Summany information about resource
    /// </summary>
    public class ResourceInfo
    {
        /// <summary>
        /// Unique name for object inherited from IResource
        /// </summary>
        public string UniquedName { get; set; }

        /// <summary>
        /// Resource instance identifier
        /// </summary>
        public int InstancesCount{ get; set; }

        /// <summary>
        /// User friendly name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// User friendly description
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
