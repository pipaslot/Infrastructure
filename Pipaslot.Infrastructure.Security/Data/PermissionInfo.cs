using System;
using System.Collections.Generic;
using System.Text;

namespace Pipaslot.Infrastructure.Security.Data
{
    public class PermissionInfo<TKey>
    {
        /// <summary>
        /// Unique name for object inherited from IResource
        /// </summary>
        public string ResourceUniquedName { get; set; }

        /// <summary>
        /// Resource instance identifier
        /// </summary>
        public TKey ResourceIdentifier { get; set; }

        /// <summary>
        /// Permission unique identifier
        /// </summary>
        public string UniqueIdentifier { get; set; }

        /// <summary>
        /// User friendly name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// User friendly description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Is allowed or is deny
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
