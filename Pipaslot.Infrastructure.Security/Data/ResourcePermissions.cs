using System;
using System.Collections.Generic;

namespace Pipaslot.Infrastructure.Security.Data
{
    public class ResourcePermissions
    {
        /// <summary>
        /// Resource unique name
        /// </summary>
        public string UniqueName { get; set; }
        
        /// <summary>
        /// All privileges 
        /// </summary>
        public IEnumerable<Permission> Permissions { get; set; } = new List<Permission>();
    }
}
