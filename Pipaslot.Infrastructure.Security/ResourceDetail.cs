using System;
using System.Collections.Generic;
using System.Text;

namespace Pipaslot.Infrastructure.Security
{
    public class ResourceDetail<TKey>
    {
        /// <summary>
        /// Resource primary key/Identifier
        /// </summary>
        public TKey Id { get; }

        /// <summary>
        /// Real resource name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Real resource description
        /// </summary>
        public string  Description { get; }

        public ResourceDetail(TKey id, string name, string description = null)
        {
            Id = id;
            Name = name;
            Description = description ?? string.Empty;
        }
    }
}
