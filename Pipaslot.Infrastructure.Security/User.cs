using System;
using System.Collections.Generic;
using System.Linq;
using Pipaslot.Infrastructure.Security.Identities;

namespace Pipaslot.Infrastructure.Security
{
    /// <inheritdoc />
    /// <summary>
    /// Identity about current user and all his permissions. Can be used as singleton because uncached identity is taken from IIdentityProvider
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class User<TKey> : IUser<TKey>
    {
        private readonly IAuthorizator<TKey> _authorizator;

        private readonly IIdentityProvider<TKey> _identityProvider;

        public TKey Id => _identityProvider.GetCurrent().Id;

        public bool IsAuthenticated => _identityProvider.GetCurrent() is UserIdentity<TKey>;

        public IEnumerable<IUserRole<TKey>> Roles => _identityProvider.GetCurrent().Roles;
        
        public User(IAuthorizator<TKey> authorizator, IIdentityProvider<TKey> identityProviderProvider)
        {
            _authorizator = authorizator;
            _identityProvider = identityProviderProvider;
        }

        public virtual bool IsAllowed(IConvertible permissionEnum)
        {
            var identity = _identityProvider.GetCurrent();
            if (identity is AdminIdentity<TKey>)
            {
                return true;
            }
            return identity.Roles.Any(role => _authorizator.IsAllowed(role, permissionEnum));
        }

        public bool IsAllowed(Type resource, IConvertible permissionEnum)
        {
            var identity = _identityProvider.GetCurrent();
            if (identity is AdminIdentity<TKey>)
            {
                return true;
            }
            return identity.Roles.Any(role => _authorizator.IsAllowed(role, resource, permissionEnum));
        }

        public virtual bool IsAllowed<TPermissions>(IResourceInstance<TKey, TPermissions> resourceInstance, TPermissions permissionEnum) where TPermissions : IConvertible
        {
            var identity = _identityProvider.GetCurrent();
            if (identity is AdminIdentity<TKey>)
            {
                return true;
            }
            return identity.Roles.Any(role => _authorizator.IsAllowed(role, resourceInstance, permissionEnum));
        }

        public virtual bool IsAllowed(Type resource, TKey resourceIdentifier, IConvertible permissionEnum)
        {
            var identity = _identityProvider.GetCurrent();
            if (identity is AdminIdentity<TKey>)
            {
                return true;
            }
            return identity.Roles.Any(role => _authorizator.IsAllowed(role, resource, resourceIdentifier, permissionEnum));
        }

        public virtual IEnumerable<TKey> GetAllowedKeys(Type resource, IConvertible permissionEnum)
        {
            var keys = new List<TKey>();
            foreach (var role in Roles)
            {
                var allowed = _authorizator.GetAllowedKeys(role, resource, permissionEnum);
                keys.AddRange(allowed);
            }
            return keys.Distinct();
        }

    }
}
