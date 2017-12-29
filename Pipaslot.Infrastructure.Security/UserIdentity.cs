using System.Collections.Generic;

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
        public IEnumerable<object> Roles { get; }

        /// <summary>
        /// Identity type
        /// </summary>
        public UserIdentityType IdentityType { get; } = UserIdentityType.Guest;

        /// <summary>
        /// Create guest identity and specify guest role
        /// </summary>
        /// <param name="guestRole"></param>
        public UserIdentity(object guestRole)
        {
            Roles = new List<object>() { { guestRole } };
        }

        /// <summary>
        /// Create Identity for user and specify its identity type
        /// </summary>
        /// <param name="id"></param>
        /// <param name="roles"></param>
        /// <param name="identityType"></param>
        public UserIdentity(object id, IEnumerable<object> roles, UserIdentityType identityType = UserIdentityType.User)
        {
            Id = id;
            Roles = roles ?? new List<object>();
            IdentityType = id == null ? UserIdentityType.Guest : identityType;
        }
    }
}
