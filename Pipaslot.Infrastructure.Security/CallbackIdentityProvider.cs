using System;
using System.Collections.Generic;
using System.Text;
using Pipaslot.Infrastructure.Security.Identities;

namespace Pipaslot.Infrastructure.Security
{
    /// <inheritdoc />
    /// <summary>
    /// GuestIdentity provider realised as direct
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class CallbackIdentityProvider<TKey> : IIdentityProvider<TKey>
    {
        private readonly Func<IIdentity<TKey>> _callback;

        public CallbackIdentityProvider(Func<IIdentity<TKey>> callback)
        {
            _callback = callback;
        }

        public IIdentity<TKey> GetCurrent()
        {
            return _callback();
        }
    }
}
