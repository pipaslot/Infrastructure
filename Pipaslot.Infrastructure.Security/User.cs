using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Security.Data;

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
        private readonly UserIdentity _identity;
        private readonly IResourceInstanceProvider _resourceInstanceProvider;

        public TKey Id => (TKey)(_identity.Id ?? default(TKey));

        public bool IsAuthenticated => _identity.Id != null && !_identity.Id.Equals(default(TKey));

        public IEnumerable<IRole> Roles => _identity.Roles.Select(r => r).ToList();
        private bool IsAdmin => _identity.Roles.Any(r => r.Type == RoleType.Admin);
        private List<TKey> RolesIds => _identity.Roles.Select(r => (TKey)r.Id).ToList();

        public User(IAuthorizator<TKey> authorizator, UserIdentity identity, IResourceInstanceProvider resourceInstanceProvider)
        {
            _authorizator = authorizator;
            _identity = identity;
            _resourceInstanceProvider = resourceInstanceProvider;
        }

        public virtual async Task CheckPermissionAsync(IConvertible permissionEnum, CancellationToken token = default(CancellationToken))
        {
            if (!await IsAllowedAsync(permissionEnum, token))
            {
                throw new AuthorizationException(permissionEnum);
            }
        }

        public virtual async Task CheckPermissionAsync(Type resource, IConvertible permissionEnum,
            CancellationToken token = default(CancellationToken))
        {
            if (!await IsAllowedAsync(resource, permissionEnum, token))
            {
                throw new AuthorizationException(resource, permissionEnum);
            }
        }

        public virtual async Task CheckPermissionAsync<TPermissions>(IResourceInstance<TPermissions> resourceInstance, TPermissions permissionEnum,
            CancellationToken token = default(CancellationToken)) where TPermissions : IConvertible
        {
            if (!await IsAllowedAsync(resourceInstance, permissionEnum, token))
            {
                var instance = await _resourceInstanceProvider.GetInstanceAsync(resourceInstance.GetType(), (TKey)resourceInstance.ResourceUniqueIdentifier, token);
                throw new AuthorizationException(resourceInstance.GetType(), permissionEnum, instance);
            }
        }

        public virtual async Task CheckPermissionAsync(Type resource, TKey resourceIdentifier, IConvertible permissionEnum,
            CancellationToken token = default(CancellationToken))
        {
            if (!await IsAllowedAsync(resource, resourceIdentifier, permissionEnum, token))
            {
                var instance = await _resourceInstanceProvider.GetInstanceAsync(resource, resourceIdentifier, token);
                throw new AuthorizationException(resource, permissionEnum, instance);
            }
        }

        public virtual async Task<bool> IsAllowedAsync(IConvertible permissionEnum, CancellationToken token = default(CancellationToken))
        {
            return IsAdmin || await _authorizator.IsAllowedAsync(RolesIds, permissionEnum, token);
        }

        public virtual async Task<bool> IsAllowedAsync(Type resource, IConvertible permissionEnum, CancellationToken token = default(CancellationToken))
        {
            return IsAdmin || await _authorizator.IsAllowedAsync(RolesIds, resource, permissionEnum, token);
        }

        public virtual async Task<bool> IsAllowedAsync<TPermissions>(IResourceInstance<TPermissions> resourceInstance, TPermissions permissionEnum,
            CancellationToken token = default(CancellationToken)) where TPermissions : IConvertible
        {
            return IsAdmin || await _authorizator.IsAllowedAsync(RolesIds, resourceInstance, permissionEnum, token);
        }

        public virtual async Task<bool> IsAllowedAsync(Type resource, TKey resourceIdentifier, IConvertible permissionEnum,
            CancellationToken token = default(CancellationToken))
        {
            return IsAdmin || await _authorizator.IsAllowedAsync(RolesIds, resource, resourceIdentifier, permissionEnum, token);
        }

        public virtual async Task<IEnumerable<TKey>> GetAllowedKeysAsync(Type resource, IConvertible permissionEnum,
            CancellationToken token = default(CancellationToken))
        {
            if (IsAdmin)
            {
                var result = await _resourceInstanceProvider.GetAllIdsAsync(resource,token);
                return result.Select(id => (TKey)id).AsEnumerable();
            }
            return await _authorizator.GetAllowedKeysAsync(RolesIds, resource, permissionEnum, token);
        }
    }
}
