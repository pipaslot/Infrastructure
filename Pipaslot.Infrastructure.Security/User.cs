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
        private readonly IResourceDetailProvider<TKey> _resourceDetailProvider;

        public TKey Id => _identity.Id;

        public bool IsAuthenticated => _identity is UserIdentity<TKey>;

        public IEnumerable<IUserRole<TKey>> Roles => _identity.Roles;

        public User(IAuthorizator<TKey> authorizator, IIdentity<TKey> identity, IResourceDetailProvider<TKey> resourceDetailProvider)
        {
            _authorizator = authorizator;
            _identity = identity;
            _resourceDetailProvider = resourceDetailProvider;
        }

        public void CheckPermission(IConvertible permissionEnum)
        {
            if (!IsAllowed(permissionEnum))
            {
                throw new AuthorizationException(permissionEnum);
            }
        }

        public void CheckPermission(Type resource, IConvertible permissionEnum)
        {
            if (!IsAllowed(resource, permissionEnum))
            {
                throw new AuthorizationException(resource, permissionEnum);
            }
        }

        public void CheckPermission<TPermissions>(IResourceInstance<TKey, TPermissions> resourceInstance, TPermissions permissionEnum) where TPermissions : IConvertible
        {
            if (!IsAllowed(resourceInstance, permissionEnum))
            {
                throw new AuthorizationException(resourceInstance.GetType(), permissionEnum, () => _resourceDetailProvider.GetResourceDetails(resourceInstance.GetType(), new List<object>() { resourceInstance.ResourceUniqueIdentifier }).FirstOrDefault());
            }
        }

        public void CheckPermission(Type resource, TKey resourceIdentifier, IConvertible permissionEnum)
        {
            if (!IsAllowed(resource, resourceIdentifier, permissionEnum))
            {
                throw new AuthorizationException(resource, permissionEnum, () => _resourceDetailProvider.GetResourceDetails(resource, new List<object>() { resourceIdentifier }).FirstOrDefault());
            }
        }

        public virtual bool IsAllowed(IConvertible permissionEnum)
        {
            if (_identity is AdminIdentity<TKey>)
            {
                return true;
            }
            return _authorizator.IsAllowed(Roles, permissionEnum);
        }

        public bool IsAllowed(Type resource, IConvertible permissionEnum)
        {
            if (_identity is AdminIdentity<TKey>)
            {
                return true;
            }
            return _authorizator.IsAllowed(Roles, resource, permissionEnum);
        }

        public virtual bool IsAllowed<TPermissions>(IResourceInstance<TKey, TPermissions> resourceInstance, TPermissions permissionEnum) where TPermissions : IConvertible
        {
            if (_identity is AdminIdentity<TKey>)
            {
                return true;
            }
            return _authorizator.IsAllowed(Roles, resourceInstance, permissionEnum);
        }

        public virtual bool IsAllowed(Type resource, TKey resourceIdentifier, IConvertible permissionEnum)
        {
            if (_identity is AdminIdentity<TKey>)
            {
                return true;
            }
            return _authorizator.IsAllowed(Roles, resource, resourceIdentifier, permissionEnum);
        }

        public virtual IEnumerable<TKey> GetAllowedKeys(Type resource, IConvertible permissionEnum)
        {
            return _authorizator.GetAllowedKeys(Roles, resource, permissionEnum);
        }
    }
}
