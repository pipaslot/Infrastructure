using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pipaslot.Infrastructure.Security
{
    //TODO implement another UserIdentity for Admin user which will have granted all permission by default
    public class UserIdentity<TKey> : IUserIdentity<TKey>
    {
        private readonly IAuthorizator<TKey> _authorizator;

        public TKey Id { get; }

        public bool IsAuthenticated { get; }

        public IEnumerable<IUserRole<TKey>> Roles { get; }

        public UserIdentity(IAuthorizator<TKey> authorizator, TKey id = default(TKey), IEnumerable<IUserRole<TKey>> roles = null)
        {
            _authorizator = authorizator;
            Id = id;
            Roles = roles ?? new List<IUserRole<TKey>>();
            //If Id is not default value, then was passed valid UserId and user had been authenticated
            var defaultId = default(TKey);
            if (defaultId == null)
            {
                //Id is nullable type
                IsAuthenticated = Id != null;
            }
            else
            {
                //Id is not nullable type
                IsAuthenticated = Id.GetHashCode() != defaultId.GetHashCode();
            }
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
