using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Security.Data;
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
        private readonly IQueryFactory<IResourceInstanceQuery> _resourcenstanceQueryFactory;

        public TKey Id => _identity.Id;

        public bool IsAuthenticated => _identity is UserIdentity<TKey>;

        public IEnumerable<IUserRole<TKey>> Roles => _identity.Roles;

        public User(IAuthorizator<TKey> authorizator, IIdentity<TKey> identity, IQueryFactory<IResourceInstanceQuery> resourcenstanceQueryFactory)
        {
            _authorizator = authorizator;
            _identity = identity;
            _resourcenstanceQueryFactory = resourcenstanceQueryFactory;
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
        
        public virtual async Task CheckPermissionAsync<TPermissions>(IResourceInstance<TKey, TPermissions> resourceInstance, TPermissions permissionEnum,
            CancellationToken token = default(CancellationToken)) where TPermissions : IConvertible
        {
            if (!await IsAllowedAsync(resourceInstance, permissionEnum, token))
            {
                throw new AuthorizationException(resourceInstance.GetType(), permissionEnum, async() =>
                {
                    var query = _resourcenstanceQueryFactory.Create();
                    query.Resource = resourceInstance.GetType();
                    query.ResourceIdentifier = resourceInstance.ResourceUniqueIdentifier;
                    var details = await query.ExecuteAsync(token);
                    return details.FirstOrDefault();
                });
            }
        }

        public virtual async Task CheckPermissionAsync(Type resource, TKey resourceIdentifier, IConvertible permissionEnum,
            CancellationToken token = default(CancellationToken))
        {
            if (!await IsAllowedAsync(resource, resourceIdentifier, permissionEnum, token))
            {
                throw new AuthorizationException(resource, permissionEnum, async () =>
                {
                    var query = _resourcenstanceQueryFactory.Create();
                    query.Resource = resource;
                    query.ResourceIdentifier = resourceIdentifier;
                    var details = await query.ExecuteAsync(token);
                    return details.FirstOrDefault();
                });
            }
        }

        public virtual async Task<bool> IsAllowedAsync(IConvertible permissionEnum, CancellationToken token = default(CancellationToken))
        {
            if (_identity is AdminIdentity<TKey>)
            {
                return true;
            }
            return await _authorizator.IsAllowedAsync(Roles, permissionEnum, token);
        }

        public virtual async Task<bool> IsAllowedAsync(Type resource, IConvertible permissionEnum, CancellationToken token = default(CancellationToken))
        {
            if (_identity is AdminIdentity<TKey>)
            {
                return true;
            }
            return await _authorizator.IsAllowedAsync(Roles, resource, permissionEnum, token);
        }

        public virtual async Task<bool> IsAllowedAsync<TPermissions>(IResourceInstance<TKey, TPermissions> resourceInstance, TPermissions permissionEnum,
            CancellationToken token = default(CancellationToken)) where TPermissions : IConvertible
        {
            if (_identity is AdminIdentity<TKey>)
            {
                return true;
            }
            return await _authorizator.IsAllowedAsync(Roles, resourceInstance, permissionEnum, token);
        }
        
        public virtual async Task<bool> IsAllowedAsync(Type resource, TKey resourceIdentifier, IConvertible permissionEnum,
            CancellationToken token = default(CancellationToken))
        {
            if (_identity is AdminIdentity<TKey>)
            {
                return true;
            }
            return await _authorizator.IsAllowedAsync(Roles, resource, resourceIdentifier, permissionEnum, token);
        }

        public virtual Task<IEnumerable<TKey>> GetAllowedKeysAsync(Type resource, IConvertible permissionEnum,
            CancellationToken token = default(CancellationToken))
        {
            return _authorizator.GetAllowedKeysAsync(Roles, resource, permissionEnum, token);
        }
    }
}
