using System.Collections.Generic;

namespace Pipaslot.Infrastructure.Security.Identities
{
    /// <summary>
    /// Identity for user which was not authenticated
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class GuestIdentity<TKey> : IIdentity<TKey>
    {
        public TKey Id { get; }

        public IEnumerable<IUserRole<TKey>> Roles { get; } = new List<IUserRole<TKey>>();
        
        public GuestIdentity()
        {
            Id = default(TKey);
        }

        public GuestIdentity(TKey id) : this(id, new List<IUserRole<TKey>>())
        {
        }

        public GuestIdentity(TKey id, IEnumerable<IUserRole<TKey>> roles)
        {
            Id = id;
            Roles = roles ?? new List<IUserRole<TKey>>();
        }
    }
}
