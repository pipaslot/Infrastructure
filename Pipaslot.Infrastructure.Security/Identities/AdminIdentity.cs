using System.Collections.Generic;

namespace Pipaslot.Infrastructure.Security.Identities
{
    /// <inheritdoc />
    /// <summary>
    /// Identity of authenticated user with granded all permissions
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class AdminIdentity <TKey>: UserIdentity<TKey>
    {
        public AdminIdentity()
        {
        }

        public AdminIdentity(TKey id) : base(id)
        {
        }

        public AdminIdentity(TKey id, IEnumerable<IUserRole<TKey>> roles) : base(id, roles)
        {
        }
    }
}
