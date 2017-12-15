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

        private readonly IIdentity<TKey> _identity;

        public TKey Id => _identity.Id;

        public bool IsAuthenticated => _identity is UserIdentity<TKey>;

        public IEnumerable<IUserRole<TKey>> Roles => _identity.Roles;
        
        public User(IAuthorizator<TKey> authorizator, IIdentity<TKey> identity)
        {
            _authorizator = authorizator;
            _identity = identity;
        }

        public virtual bool IsAllowed(IConvertible permissionEnum)
        {
            if (_identity is AdminIdentity<TKey>)
            {
                return true;
            }
            return Roles.Any(role => _authorizator.IsAllowed(role, permissionEnum));
        }

        public bool IsAllowed(Type resource, IConvertible permissionEnum)
        {
            if (_identity is AdminIdentity<TKey>)
            {
                return true;
            }
            return Roles.Any(role => _authorizator.IsAllowed(role, resource, permissionEnum));
        }

        public virtual bool IsAllowed<TPermissions>(IResourceInstance<TKey, TPermissions> resourceInstance, TPermissions permissionEnum) where TPermissions : IConvertible
        {
            if (_identity is AdminIdentity<TKey>)
            {
                return true;
            }
            return Roles.Any(role => _authorizator.IsAllowed(role, resourceInstance, permissionEnum));
        }

        public virtual bool IsAllowed(Type resource, TKey resourceIdentifier, IConvertible permissionEnum)
        {
            if (_identity is AdminIdentity<TKey>)
            {
                return true;
            }
            return Roles.Any(role => _authorizator.IsAllowed(role, resource, resourceIdentifier, permissionEnum));
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
