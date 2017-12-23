using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.Security
{
    public class PermissionManager<TKey> : IPermissionManager<TKey>
    {
        private readonly IPermissionStore<TKey> _permissionStore;
        private readonly ResourceRegistry<TKey> _resourceRegistry;
        private readonly IQueryFactory<IResourceInstanceQuery> _resourceInstanceQueryFactory;
        private readonly INamingConvertor _namingConvertor;

        public PermissionManager(IPermissionStore<TKey> permissionStore, ResourceRegistry<TKey> resourceRegistry, IQueryFactory<IResourceInstanceQuery> resourceInstanceQueryFactory, INamingConvertor namingConvertor)
        {
            _permissionStore = permissionStore;
            _resourceRegistry = resourceRegistry;
            _namingConvertor = namingConvertor;
            _resourceInstanceQueryFactory = resourceInstanceQueryFactory;
        }

        #region Setup Privilege

        public void Allow(IUserRole<TKey> role, Type resource, IConvertible permissionEnum)
        {
            Allow(role, resource, default(TKey), permissionEnum);
        }

        public virtual void Allow(IUserRole<TKey> role, Type resource, TKey resourceId, IConvertible permissionEnum)
        {
            SetPrivilege(role, resource, resourceId, permissionEnum, true);
        }

        public void Deny(IUserRole<TKey> role, Type resource, IConvertible permissionEnum)
        {
            Deny(role, resource, default(TKey), permissionEnum);
        }

        public virtual void Deny(IUserRole<TKey> role, Type resource, TKey resourceId, IConvertible permissionEnum)
        {
            SetPrivilege(role, resource, resourceId, permissionEnum, false);
        }

        protected virtual void SetPrivilege(IUserRole<TKey> role, Type resource, TKey resourceId, IConvertible permissionEnum, bool privilege)
        {
            Helpers.CheckIfResourceHasAssignedPermission(resource, permissionEnum);
            var res = _namingConvertor.GetResourceUniqueName(resource);
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            _permissionStore.SetPrivilege(role.Id, res, resourceId, perm, privilege);
        }

        #endregion

        public virtual async Task<IEnumerable<ResourceInfo>> GetAllResourcesAsync(CancellationToken token = default(CancellationToken))
        {
            var result = new List<ResourceInfo>();
            foreach (var pair in _resourceRegistry.ResourceTypes)
            {
                var type = pair.ResourceType;
                var resourceName = _namingConvertor.GetResourceUniqueName(type);
                var instanceQuery = _resourceInstanceQueryFactory.Create();
                instanceQuery.Resource = type;
                var info = new ResourceInfo
                {
                    InstancesCount = await instanceQuery.GetTotalRowCountAsync(token),
                    UniqueName = resourceName,
                    Name = Helpers.GetResourceReadableName(type),
                    Description = Helpers.GetResourceReadableDescription(type)
                };

                result.Add(info);
            }
            return result;
        }

        //todo Implement paging for future (as query)
        public virtual async Task<IEnumerable<ResourceInstanceInfo<TKey>>> GetAllResourceInstancesAsync(string resource, CancellationToken token = default(CancellationToken))
        {
            var result = new List<ResourceInstanceInfo<TKey>>();
            var resourceType = _namingConvertor.GetResourceTypeByUniqueName(resource);
            var instanceQuery = _resourceInstanceQueryFactory.Create();
            instanceQuery.Resource = resourceType;
            var instanceDetails = await instanceQuery.ExecuteAsync(token);
            foreach (var detail in instanceDetails)
            {
                result.Add(new ResourceInstanceInfo<TKey>
                {
                    Identifier = (TKey)detail.Id,
                    UniquedName = resource,
                    Name = detail.Name,
                    Description = detail.Description
                });
            }
            return result;
        }

        #region GetAllPermissions

        public virtual async Task<IEnumerable<PermissionInfo<TKey>>> GetAllPermissionsAsync(TKey roleId, string resource, CancellationToken token = default(CancellationToken))
        {
            var result = new List<PermissionInfo<TKey>>();
            var registeredResource = GetRegisteredResource(resource);
            if (registeredResource == null) return result;
            foreach (var permissionType in registeredResource.StaticPermissions)
            {
                var enumNames = Enum.GetNames(permissionType);
                foreach (var enumName in enumNames)
                {
                    var memberInfo = permissionType.GetField(enumName);
                    var permissionUniqueIdentifier = _namingConvertor.GetPermissionUniqueIdentifier(permissionType, memberInfo);
                    var info = new PermissionInfo<TKey>()
                    {
                        ResourceUniquedName = resource,
                        ResourceIdentifier = default(TKey),
                        UniqueIdentifier = permissionUniqueIdentifier,
                        IsAllowed = await _permissionStore.IsAllowedAsync(roleId, resource, permissionUniqueIdentifier, token),
                        Name = Helpers.GetPermissonReadableName(memberInfo),
                        Description = Helpers.GetPermissonReadableDescription(memberInfo)
                    };

                    result.Add(info);
                }
            }
            return result;
        }

        public virtual async Task<IEnumerable<PermissionInfo<TKey>>> GetAllPermissionsAsync(TKey roleId, string resource, TKey resourceId, CancellationToken token = default(CancellationToken))
        {
            var result = new List<PermissionInfo<TKey>>();
            var registeredResource = GetRegisteredResource(resource);
            if (registeredResource == null) return result;
            foreach (var permissionType in registeredResource.InstancePermissions)
            {
                var enumNames = Enum.GetNames(permissionType);
                foreach (var enumName in enumNames)
                {
                    var memberInfo = permissionType.GetField(enumName);
                    var permissionUniqueIdentifier = _namingConvertor.GetPermissionUniqueIdentifier(permissionType, memberInfo);
                    var info = new PermissionInfo<TKey>()
                    {
                        ResourceUniquedName = resource,
                        ResourceIdentifier = resourceId,
                        UniqueIdentifier = permissionUniqueIdentifier,
                        IsAllowed = await _permissionStore.IsAllowedAsync(roleId, resource, resourceId, permissionUniqueIdentifier, token),
                        Name = Helpers.GetPermissonReadableName(memberInfo),
                        Description = Helpers.GetPermissonReadableDescription(memberInfo)
                    };

                    result.Add(info);
                }
            }
            return result;
        }

        private ResourceRegistry<TKey>.RegisteredResource GetRegisteredResource(string resource)
        {
            var resourceType = _namingConvertor.GetResourceTypeByUniqueName(resource);
            return _resourceRegistry.ResourceTypes.FirstOrDefault(r => r.ResourceType == resourceType);
        }


        #endregion
    }
}
