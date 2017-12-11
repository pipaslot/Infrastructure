using System;
using System.Collections.Generic;
using System.Linq;

namespace Pipaslot.Infrastructure.Security
{
    /// <inheritdoc />
    /// <summary>
    /// Identity about current user and all his permissions
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class UserIdentity<TKey> : IUserIdentity<TKey>
    {
        private readonly IAuthorizator<TKey> _authorizator;
        private readonly IIdentity<TKey> _identity;

        public TKey Id => _identity.Id;

        public bool IsAuthenticated => _identity.IsAuthenticated;

        public IEnumerable<IUserRole<TKey>> Roles => _identity.Roles;

        public UserIdentity(IAuthorizator<TKey> authorizator, TKey userId) : this(authorizator, new Identity<TKey>(userId))
        {
        }

        public UserIdentity(IAuthorizator<TKey> authorizator, TKey userId, IEnumerable<IUserRole<TKey>> roles) : this(authorizator, new Identity<TKey>(userId, roles))
        {
        }

        public UserIdentity(IAuthorizator<TKey> authorizator, IIdentity<TKey> identity = null)
        {
            _authorizator = authorizator;
            _identity = identity ?? new Identity<TKey>();
        }

        public virtual bool IsAllowed(IConvertible permissionEnum)
        {
            return Roles.Any(role => _authorizator.IsAllowed(role, permissionEnum));
        }

        public bool IsAllowed(Type resource, IConvertible permissionEnum)
        {
            return Roles.Any(role => _authorizator.IsAllowed(role, resource, permissionEnum));
        }

        public virtual bool IsAllowed<TPermissions>(IResourceInstance<TKey, TPermissions> resourceInstance, TPermissions permissionEnum) where TPermissions : IConvertible
        {
            return Roles.Any(role => _authorizator.IsAllowed(role, resourceInstance, permissionEnum));
        }

        public virtual bool IsAllowed(Type resource, TKey resourceIdentifier, IConvertible permissionEnum)
        {
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
