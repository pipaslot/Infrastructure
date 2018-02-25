using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.Infrastructure.Security.Exceptions;

namespace Pipaslot.Infrastructure.Security
{
    /// <inheritdoc />
    /// <summary>
    /// Identity about current user and all his permissions. Can be used as singleton because uncached identity is taken from IIdentityProvider
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class User<TKey> : IUser<TKey>
    {
        private readonly IPermissionManager<TKey> _permissionManager;
        private readonly IClaimsPrincipalProvider _claimsPrincipalProvider;
        private readonly IResourceInstanceProvider _resourceInstanceProvider;
        private readonly IRoleStore _roleStore;

        public TKey Id => _identity.Id;
        public bool IsAuthenticated => _identity.Id != null && !_identity.Id.Equals(default(TKey));
        public IEnumerable<IRole> Roles => _identity.Roles;

        private List<TKey> RolesIds => _identity.Roles.Select(r => (TKey)r.Id).ToList();
        private bool IsAdmin => _identity.Roles.Any(r => r.Type == RoleType.Admin);

        private (TKey Id, IEnumerable<IRole> Roles) _identity
        {
            get
            {
                var user = _claimsPrincipalProvider.GetClaimsPrincipal();
                var roleClaims = user.FindAll(ClaimTypes.Role);
                //Load assigned not system user roles from claims
                var roles = Helpers.ClaimsToRoles(roleClaims);
                //Auto assign guest role
                roles.Add(SystemRoles.First(r=>r.Type == RoleType.Guest));
                var name = user.Identity?.Name;
                //If username is not empty, add User role
                if (!string.IsNullOrWhiteSpace(name))
                {
                    roles.Add(SystemRoles.First(r => r.Type == RoleType.User));
                }
                if (typeof(TKey) == typeof(int))
                {
                    foreach (var role in roles)
                    {
                        role.Id = ParseIntFromString(role.Id?.ToString(),"User ID");
                    }
                    return (Id: ParseIntFromString(name, "Role ID"), Roles: roles);
                }
                if (typeof(TKey) == typeof(string))
                {

                    return (Id: (TKey)(object)name, Roles: roles);
                }
                throw new NotSupportedException($"Generic attribute TKey of type {typeof(TKey)} is not supported. Only int and string can be used");
            }
        }

        /// <summary>
        /// System role cache
        /// </summary>
        private List<IRole> _systemRolesCache;

        /// <summary>
        /// Load system roles from database and cache them
        /// </summary>
        private List<IRole> SystemRoles => _systemRolesCache ?? (_systemRolesCache = _roleStore.GetSystemRoles<IRole>().ToList());

        private TKey ParseIntFromString(string value, string field)
        {
            try
            {
                return string.IsNullOrWhiteSpace(value) ? default(TKey) : (TKey) (object) int.Parse(value);
            }
            catch (FormatException)
            {
                throw new ArgumentOutOfRangeException($"{field}: Expected integer value but got '{value}'");
            }
        }

        public User(IPermissionManager<TKey> permissionManager, IClaimsPrincipalProvider claimsPrincipalProvider, IResourceInstanceProvider resourceInstanceProvider, IRoleStore roleStore)
        {
            _permissionManager = permissionManager;
            _claimsPrincipalProvider = claimsPrincipalProvider;
            _resourceInstanceProvider = resourceInstanceProvider;
            _roleStore = roleStore;
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
            return IsAdmin || await _permissionManager.IsAllowedAsync(RolesIds, permissionEnum, token);
        }

        public virtual async Task<bool> IsAllowedAsync(Type resource, IConvertible permissionEnum, CancellationToken token = default(CancellationToken))
        {
            return IsAdmin || await _permissionManager.IsAllowedAsync(RolesIds, resource, permissionEnum, token);
        }

        public virtual async Task<bool> IsAllowedAsync<TPermissions>(IResourceInstance<TPermissions> resourceInstance, TPermissions permissionEnum,
            CancellationToken token = default(CancellationToken)) where TPermissions : IConvertible
        {
            return IsAdmin || await _permissionManager.IsAllowedAsync(RolesIds, resourceInstance, permissionEnum, token);
        }

        public virtual async Task<bool> IsAllowedAsync(Type resource, TKey resourceIdentifier, IConvertible permissionEnum,
            CancellationToken token = default(CancellationToken))
        {
            return IsAdmin || await _permissionManager.IsAllowedAsync(RolesIds, resource, resourceIdentifier, permissionEnum, token);
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
            var allowed = await _permissionManager.GetAllowedKeysAsync(RolesIds, resource, permissionEnum, token);
            return allowed.ToList();
        }

        public async Task<Dictionary<string, List<string>>> GetStaticPermissionsAsync(CancellationToken token)
        {
            var isAdmin = IsAdmin;
            var resources = await _permissionManager.GetResourcePermissionsAsync(RolesIds, token);
            var result = new Dictionary<string, List<string>>();
            foreach (var res in resources)
            {
                var permissions = new List<string>();
                foreach (var permission in res.Permissions)
                {
                    if (permission.IsAllowed || isAdmin)
                    {
                        permissions.Add(permission.Identifier);
                    }
                }
                if (permissions.Any())
                {
                    result.Add(res.UniqueName,permissions);
                }
            }
            return result;
        }

        public async Task<IEnumerable<string>> GetResourcePermissionsAsync(string resourceUniqueName, TKey resourceInstance, CancellationToken token)
        {
            var isAdmin = IsAdmin;
            var permissions = await _permissionManager.GetResourceInstancePermissionsAsync(RolesIds, resourceUniqueName, resourceInstance, token);
            var result = new List<string>();
            foreach (var permission in permissions)
            {
                if (permission.IsAllowed || isAdmin)
                {
                    result.Add(permission.Identifier);
                }
            }
            return result;
        }

        public async Task<Dictionary<TKey, List<string>>> GetResourcePermissionsAsync(string resourceUniqueName, ICollection<TKey> resourceInstances, CancellationToken token)
        {
            var isAdmin = IsAdmin;
            var instances = await _permissionManager.GetResourceInstancePermissionsAsync(RolesIds, resourceUniqueName, resourceInstances, token);
            var result = new Dictionary<TKey, List<string>>();
            foreach (var res in instances)
            {
                var permissions = new List<string>();
                foreach (var permission in res.Permissions)
                {
                    if (permission.IsAllowed || isAdmin)
                    {
                        permissions.Add(permission.Identifier);
                    }
                }
                if (permissions.Any())
                {
                    result.Add((TKey)res.Identifier, permissions);
                }
            }
            return result;
        }
    }
}
