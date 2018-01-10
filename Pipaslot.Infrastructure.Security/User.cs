using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        private readonly IClaimsPrincipalProvider _claimsPrincipalProvider;
        private readonly IResourceInstanceProvider _resourceInstanceProvider;

        public TKey Id => _identity.Id;

        public bool IsAuthenticated => _identity.Id != null && !_identity.Id.Equals(default(TKey));

        public IEnumerable<IRole> Roles => _identity.Roles;
        private bool IsAdmin => _identity.Roles.Any(r => r.Type == RoleType.Admin);
        private List<TKey> RolesIds => _identity.Roles.Select(r => (TKey)r.Id).ToList();

        private (TKey Id, IEnumerable<IRole> Roles) _identity
        {
            get
            {
                var user = _claimsPrincipalProvider.GetClaimsPrincipal();
                var roleClaims = user.FindAll(ClaimTypes.Role);
                var roles = Helpers.ClaimsToRoles(roleClaims);
                var name = user.Identity?.Name;
                if (typeof(TKey) == typeof(int))
                {
                    foreach (var role in roles)
                    {
                        role.Id = (object)ParseIntFromString(role.Id.ToString());
                    }
                    return (Id: ParseIntFromString(name), Roles: roles);
                }
                if (typeof(TKey) == typeof(string))
                {

                    return (Id: (TKey)(object)name, Roles: roles);
                }
                throw new NotSupportedException($"Generic attribute TKey of type {typeof(TKey)} is not supported. Only int and string can be used");
            }
        }

        private TKey ParseIntFromString(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? default(TKey) : (TKey)(object)int.Parse(value);
        }

        public User(IAuthorizator<TKey> authorizator, IClaimsPrincipalProvider claimsPrincipalProvider, IResourceInstanceProvider resourceInstanceProvider)
        {
            _authorizator = authorizator;
            _claimsPrincipalProvider = claimsPrincipalProvider;
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
                var instance = await _resourceInstanceProvider.GetInstanceAsync(resourceInstance.GetType(), resourceInstance.ResourceUniqueIdentifier, token);
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
                var keys = await _resourceInstanceProvider.GetAllIdsAsync(resource, token);
                if (typeof(TKey) == typeof(int))
                {
                    return keys.Select(id => (TKey)id).AsEnumerable();
                }
                if (typeof(TKey) == typeof(string))
                {

                    return keys.Select(id => (TKey)id).AsEnumerable();
                }
                throw new NotSupportedException(
                    $"Generic attribute TKey of type {typeof(TKey)} is not supported. Only int and string can be used");
            }
            var allowed = await _authorizator.GetAllowedKeysAsync(RolesIds, resource, permissionEnum, token);
            return allowed.ToList();
        }
    }
}
