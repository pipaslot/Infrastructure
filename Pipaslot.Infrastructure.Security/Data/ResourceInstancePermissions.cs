using System;
using System.Collections.Generic;

namespace Pipaslot.Infrastructure.Security.Data
{
    public class ResourceInstancePermissions
    {
        /// <summary>
        /// Resource unique name
        /// </summary>
        public object Identifier { get; set; }

        /// <summary>
        /// All privileges 
        /// </summary>
        public IEnumerable<Permission> Permissions { get; set; } = new List<Permission>();
    }
}
