using Pipaslot.Infrastructure.Security.Identities;

namespace Pipaslot.Infrastructure.Security
{
    /// <summary>
    /// Provides current user identity. This provider can be used as singleton shared betwean request, because this provider should return uncached GuestIdentity
    /// </summary>
    public interface IIdentityProvider<TKey>
    {
        IIdentity<TKey> GetCurrent();
    }
}
