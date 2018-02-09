using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.SecurityUI.Controllers
{
    public class SecurityController<TKey> : Controller
    {
        private readonly IRoleStore _roleStore;
        private readonly IPermissionManager<TKey> _permissionManager;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public SecurityController(IRoleStore roleStore, IPermissionManager<TKey> permissionManager, IUnitOfWorkFactory unitOfWorkFactory)
        {
            _roleStore = roleStore;
            _permissionManager = permissionManager;
            _unitOfWorkFactory = unitOfWorkFactory;
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

        [HttpGet("resources")]
        public Task<IEnumerable<ResourceInfo>> GetAllResource()
        {
            return _permissionManager.GetAllResourcesAsync();
        }

        [HttpGet("resources/{resourceUniqueName}")]
        public Task<IEnumerable<ResourceInstanceInfo>> GetResource(string resourceUniqueName)
        {
            return _permissionManager.GetAllResourceInstancesAsync(resourceUniqueName);
        }

        [HttpGet("resources/{resourceUniqueName}/permissions")]
        public Task<IEnumerable<PermissionInfo>> GetAllPermissions(TKey roleId, string resourceUniqueName)
        {
            return _permissionManager.GetAllPermissionsAsync(roleId, resourceUniqueName);
        }

        [HttpPost("resources/{resourceUniqueName}/permissions/{permissionUniqueIdentifier}")]
        public bool SetPrivilege(TKey roleId, string resourceUniqueName, string permissionUniqueIdentifier, bool? isAllowed)
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                _permissionManager.SetPermission(roleId, resourceUniqueName, permissionUniqueIdentifier, isAllowed);
                uow.Commit();
            }
            return true;
        }

        [HttpGet("resources/{resourceUniqueName}/{resourceId}/permissions")]
        public Task<IEnumerable<PermissionInfo>> GetAllPermissions(TKey roleId, string resourceUniqueName, TKey resourceId)
        {
            return _permissionManager.GetAllPermissionsAsync(roleId, resourceUniqueName, resourceId);
        }

        [HttpPost("resources/{resourceUniqueName}/{resourceId}/permissions/{permissionUniqueIdentifier}")]
        public bool SetPrivilege(TKey roleId, string resourceUniqueName, TKey resourceId, string permissionUniqueIdentifier, bool? isAllowed)
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                 _permissionManager.SetPermission(roleId, resourceUniqueName, resourceId, permissionUniqueIdentifier, isAllowed);
                uow.Commit();
            }
            return true;
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