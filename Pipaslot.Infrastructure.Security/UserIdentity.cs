using System.Collections.Generic;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.Security
{
    public class UserIdentity
    {
        /// <summary>
        /// User primary key / Identifier
        /// </summary>
        public object Id { get; }

        /// <summary>
        /// Assigned role primary keys
        /// </summary>
        public IEnumerable<IRole> Roles { get; }
        
        /// <summary>
        /// Create guest identity and specify guest role
        /// </summary>
        /// <param name="guestRole"></param>
        public UserIdentity(IRole guestRole)
        {
            Roles = new List<IRole>() { { guestRole } };
        }

        /// <summary>
        /// Create Identity for user and specify its identity type
        /// </summary>
        /// <param name="id"></param>
        /// <param name="roles"></param>
        public UserIdentity(object id, IEnumerable<IRole> roles)
        {
            Id = id;
            Roles = roles ?? new List<IRole>();
        }
    }
}
