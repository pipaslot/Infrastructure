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
        public IEnumerable<RoleResult> GetRoles()
        {
            var result = new List<RoleResult>();
            foreach (var role in _roleStore.GetAll<IRole>())
            {
                result.Add(new RoleResult
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    ShowResources = role.Type != RoleType.Admin,
                    IsSystem = role.Type != RoleType.Custom
                });
            }
            return result;
        }
        [HttpGet("roles/{roleId}/resources/{resourceUniqueName}/permissions")]
        public Task<IEnumerable<PermissionInfo>> GetAllPermissions(TKey roleId, string resourceUniqueName)
        {
            return _permissionManager.GetAllPermissionsAsync(roleId, resourceUniqueName);
        }

        [HttpPost("roles/{roleId}/resources/{resourceUniqueName}/permissions/{permissionUniqueIdentifier}")]
        public bool SetPrivilege(TKey roleId, string resourceUniqueName, string permissionUniqueIdentifier, bool? isAllowed)
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                _permissionManager.SetPermission(roleId, resourceUniqueName, permissionUniqueIdentifier, isAllowed);
                uow.Commit();
            }
            return true;
        }

        [HttpGet("roles/{roleId}/resources/{resourceUniqueName}/{resourceId}/permissions")]
        public Task<IEnumerable<PermissionInfo>> GetAllPermissions(TKey roleId, string resourceUniqueName, TKey resourceId)
        {
            return _permissionManager.GetAllPermissionsAsync(roleId, resourceUniqueName, resourceId);
        }

        [HttpPost("roles/{roleId}/resources/{resourceUniqueName}/{resourceId}/permissions/{permissionUniqueIdentifier}")]
        public bool SetPrivilege(TKey roleId, string resourceUniqueName, TKey resourceId, string permissionUniqueIdentifier, bool? isAllowed)
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                _permissionManager.SetPermission(roleId, resourceUniqueName, resourceId, permissionUniqueIdentifier, isAllowed);
                uow.Commit();
            }
            return true;
        }

        [HttpGet("resources")]
        public Task<IEnumerable<ResourceInfo>> GetAllResource(CancellationToken token)
        {
            return _permissionManager.GetAllResourcesAsync(token);
        }
        /// <summary>
        /// Permissions available to current user
        /// </summary>
        /// <returns></returns>
        [HttpGet("user/roles")]
        public Task<IEnumerable<string>> GetAllMyRoles(CancellationToken token)
        {
            var roles = _roleStore.GetAll<IRole>();
            var roleIds = _user.Roles.Select(r => r.Id).ToList();
            var roleNames = (IEnumerable<string>)roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToList();
            return Task.FromResult(roleNames);
        }

        /// <summary>
        /// Permissions available to current user
        /// </summary>
        /// <returns></returns>
        [HttpGet("user/permissions")]
        public Task<IEnumerable<ResourcePermissions>> GetAllResourcePermissions(CancellationToken token)
        {
            var roles = _user.Roles.Select(r => (TKey)r.Id).ToList();
            return _permissionManager.GetResourcePermissionsAsync(roles, token);
        }

        /// <summary>
        /// Permissions available to current user for single resource instance
        /// </summary>
        /// <returns></returns>
        [HttpGet("user/resources/{resourceUniqueName}/{resourceInstance}/permissions")]
        public Task<IEnumerable<Permission>> GetResource(string resourceUniqueName, TKey resourceInstance, CancellationToken token)
        {
            var roles = _user.Roles.Select(r => (TKey)r.Id).ToList();
            return _permissionManager.GetResourceInstancePermissionsAsync(roles, resourceUniqueName, resourceInstance, token);
        }

        /// <summary>
        /// Permissions available to current user for multiple resource instances
        /// </summary>
        /// <returns></returns>
        [HttpGet("user/resources/{resourceUniqueName}/permissions")]
        public Task<IEnumerable<ResourceInstancePermissions>> GetResourceInstancePermissionsAsync(string resourceUniqueName, ICollection<TKey> resourceInstances, CancellationToken token)
        {
            var roles = _user.Roles.Select(r => (TKey)r.Id).ToList();
            return _permissionManager.GetResourceInstancePermissionsAsync(roles, resourceUniqueName, resourceInstances, token);
        }

        #region Output objects

        public class RoleResult
        {
            public object Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public bool ShowResources { get; set; }
            public bool IsSystem { get; set; }
        }

        #endregion
    }
}