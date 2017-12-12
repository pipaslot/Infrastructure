using System.Collections.Generic;

namespace Pipaslot.Infrastructure.Security.Identities
{
    /// <inheritdoc />
    /// <summary>
    /// Identity for authenticated user
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class UserIdentity<TKey> : GuestIdentity<TKey>
    {
        public UserIdentity()
        {
        }

        public UserIdentity(TKey id) : base(id)
        {
        }

        public UserIdentity(TKey id, IEnumerable<IUserRole<TKey>> roles) : base(id, roles)
        {
        }
    }
}
