using System;

namespace Pipaslot.Infrastructure.Security.Data
{
    public class Permission
    {
        /// <summary>
        /// Permission unique identifier
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// User has allowed permission
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
