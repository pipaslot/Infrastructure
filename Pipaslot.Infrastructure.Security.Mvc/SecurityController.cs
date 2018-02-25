using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.Security.Mvc
{
    public class SecurityController<TKey> : Controller
    {
        private readonly IRoleStore _roleStore;
        private readonly IPermissionManager<TKey> _permissionManager;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IUser<TKey> _user;

        public SecurityController(IRoleStore roleStore, IPermissionManager<TKey> permissionManager, IUnitOfWorkFactory unitOfWorkFactory, IUser<TKey> user)
        {
            _roleStore = roleStore;
            _permissionManager = permissionManager;
            _unitOfWorkFactory = unitOfWorkFactory;
            _user = user;
        }

        /// <summary>
        /// Returns all registered roles
        /// </summary>
        /// <returns></returns>
        [HttpGet("roles")]
        public virtual Task<IEnumerable<RoleResult>> GetRolesAsync(CancellationToken token)
        {
            var result = new List<RoleResult>();
            foreach (var role in _roleStore.GetAll<IRole>())
            {
                result.Add(new RoleResult
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    IsEditable = role.Type != RoleType.Admin,
                    IsDeletable = role.Type == RoleType.Custom
                });
            }
            return Task.FromResult((IEnumerable<RoleResult>)result);
        }

        /// <summary>
        /// Get resource permissions
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="resourceUniqueName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("roles/{roleId}/resources/{resourceUniqueName}/permissions")]
        public virtual Task<IEnumerable<PermissionInfo>> GetAllPermissionsAsync(TKey roleId, string resourceUniqueName, CancellationToken token)
        {
            return _permissionManager.GetAllPermissionsAsync(roleId, resourceUniqueName, token);
        }

        /// <summary>
        /// Set Resource permission
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="resourceUniqueName"></param>
        /// <param name="permissionUniqueIdentifier"></param>
        /// <param name="isAllowed"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("roles/{roleId}/resources/{resourceUniqueName}/permissions/{permissionUniqueIdentifier}")]
        public virtual async Task<bool> SetPrivilegeAsync(TKey roleId, string resourceUniqueName, string permissionUniqueIdentifier, bool? isAllowed, CancellationToken token)
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                _permissionManager.SetPermission(roleId, resourceUniqueName, permissionUniqueIdentifier, isAllowed);
                await uow.CommitAsync(token);
            }
            return true;
        }

        /// <summary>
        /// Get all Resource instance permissions
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="resourceUniqueName"></param>
        /// <param name="resourceId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("roles/{roleId}/resources/{resourceUniqueName}/{resourceId}/permissions")]
        public virtual Task<IEnumerable<PermissionInfo>> GetAllPermissionsAsync(TKey roleId, string resourceUniqueName, TKey resourceId, CancellationToken token)
        {
            return _permissionManager.GetAllPermissionsAsync(roleId, resourceUniqueName, resourceId, token);
        }

        /// <summary>
        /// Set permission for resource instance
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="resourceUniqueName"></param>
        /// <param name="resourceId"></param>
        /// <param name="permissionUniqueIdentifier"></param>
        /// <param name="isAllowed"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("roles/{roleId}/resources/{resourceUniqueName}/{resourceId}/permissions/{permissionUniqueIdentifier}")]
        public virtual async Task<bool> SetPrivilegeAsync(TKey roleId, string resourceUniqueName, TKey resourceId, string permissionUniqueIdentifier, bool? isAllowed, CancellationToken token)
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                _permissionManager.SetPermission(roleId, resourceUniqueName, resourceId, permissionUniqueIdentifier, isAllowed);
                await uow.CommitAsync(token);
            }
            return true;
        }

        /// <summary>
        /// Get all secured resources
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("resources")]
        public virtual Task<IEnumerable<ResourceInfo>> GetAllResourceAsync(CancellationToken token)
        {
            return _permissionManager.GetAllResourcesAsync(token);
        }

        /// <summary>
        /// Get all instances for resource
        /// </summary>
        /// <param name="resourceUniqueName"></param>
        /// <returns></returns>
        [HttpGet("resources/{resourceUniqueName}")]
        public virtual Task<IEnumerable<ResourceInstanceInfo>> GetResourceInstancesAsync(string resourceUniqueName, CancellationToken token)
        {
            return _permissionManager.GetAllResourceInstancesAsync(resourceUniqueName, 1, 10, token);
        }

        /// <summary>
        /// Permissions available to current user
        /// </summary>
        /// <returns></returns>
        [HttpGet("user/roles")]
        public virtual Task<IEnumerable<string>> GetAllMyRolesAsync(CancellationToken token)
        {
            var roles = _roleStore.GetAll<IRole>();
            var roleIds = _user.Roles.Select(r => r.Id).ToList();
            var roleNames = (IEnumerable<string>)roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToList();
            return Task.FromResult(roleNames);
        }

        /// <summary>
        /// Permissions allowed to current user. Returns resources and all allowed static permissions
        /// </summary>
        /// <returns></returns>
        [HttpGet("user/permissions")]
        public virtual Task<Dictionary<string, List<string>>> GetStaticPermissionsAsync(CancellationToken token)
        {
            return _user.GetStaticPermissionsAsync(token);
        }

        /// <summary>
        /// Permissions available to current user for single resource instance
        /// </summary>
        /// <returns></returns>
        [HttpGet("user/resources/{resourceUniqueName}/{resourceInstance}/permissions")]
        public virtual Task<IEnumerable<string>> GetResourcePermissionsAsync(string resourceUniqueName, TKey resourceInstance, CancellationToken token)
        {
            return _user.GetResourcePermissionsAsync(resourceUniqueName, resourceInstance, token);
        }

        /// <summary>
        /// Permissions available to current user for multiple resource instances
        /// </summary>
        /// <returns></returns>
        [HttpGet("user/resources/{resourceUniqueName}/permissions")]
        public virtual Task<Dictionary<TKey, List<string>>> GetResourcePermissionsAsync(string resourceUniqueName, List<TKey> resourceInstances, CancellationToken token)
        {
            if (!resourceInstances.Any() && Request.Query.ContainsKey(nameof(resourceInstances)))
            {
                var instances = Request.Query[nameof(resourceInstances)];
                var stringIds = instances.ToString().Split(',');
                if (typeof(TKey) == typeof(string))
                {
                    resourceInstances = stringIds.Select(s => (TKey)(object)s).ToList();
                }
                else if (typeof(TKey) == typeof(int))
                {
                    foreach (var strId in stringIds)
                    {
                        if (int.TryParse(strId, out var intId))
                        {
                            resourceInstances.Add((TKey)(object)intId);
                        }
                        else
                        {
                            throw new InvalidCastException("Can not convert parameter "+nameof(resourceInstances)+" to list of integers.");
                        }
                    }
                }
                else
                {
                    throw new NotSupportedException("Method is not supported for key with type "+ typeof(TKey));
                }
            }
            return _user.GetResourcePermissionsAsync(resourceUniqueName, resourceInstances, token);
        }

        #region Output objects

        public class RoleResult
        {
            public object Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public bool IsEditable { get; set; }
            public bool IsDeletable { get; set; }
        }

        #endregion
    }
}