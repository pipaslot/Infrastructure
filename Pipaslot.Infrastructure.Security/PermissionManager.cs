using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.Security
{
    public class PermissionManager<TKey> : IPermissionManager<TKey>
    {
        private readonly IPermissionStore<TKey> _permissionStore;
        private readonly ResourceRegistry _resourceRegistry;
        private readonly IResourceInstanceProvider _resourceInstanceProvider;
        private readonly INamingConvertor _namingConvertor;

        public PermissionManager(IPermissionStore<TKey> permissionStore, ResourceRegistry resourceRegistry, IResourceInstanceProvider resourceInstanceProvider, INamingConvertor namingConvertor)
        {
            _permissionStore = permissionStore;
            _resourceRegistry = resourceRegistry;
            _namingConvertor = namingConvertor;
            _resourceInstanceProvider = resourceInstanceProvider;
        }

        #region Setup Privilege

        public void SetPermission(TKey role, string resource, string permission, bool? isEnabled)
        {
            SetPermission(role, resource, default(TKey), permission, isEnabled);
        }

        public void SetPermission(TKey role, string resource, TKey resourceId, string permission, bool? isEnabled)
        {
            var resourcetype = _namingConvertor.GetResourceTypeByUniqueName(resource);
            var convertible = _namingConvertor.GetPermissionByUniqueName(permission);
            Helpers.CheckIfResourceHasAssignedPermission(resourcetype, convertible);
            _permissionStore.SetPrivilege(role, resource, resourceId, permission, isEnabled);
        }

        #endregion

        public virtual async Task<IEnumerable<ResourceInfo>> GetAllResourcesAsync(CancellationToken token = default(CancellationToken))
        {
            var result = new List<ResourceInfo>();
            foreach (var pair in _resourceRegistry.ResourceTypes)
            {
                var type = pair.ResourceType;
                var resourceName = _namingConvertor.GetResourceUniqueName(type);
                var info = new ResourceInfo
                {
                    InstancesCount = await _resourceInstanceProvider.GetInstanceCountAsync(pair.ResourceType, token),
                    UniqueName = resourceName,
                    Name = Helpers.GetResourceReadableName(type),
                    Description = Helpers.GetResourceReadableDescription(type),
                    PermissionsCount = GetStaticPermissionsCount(resourceName)
                };

                result.Add(info);
            }
            return result.OrderBy(r => r.Name).ToList();
        }

        private int GetStaticPermissionsCount(string resource)
        {
            var count = 0;
            var registeredResource = GetRegisteredResource(resource);
            if (registeredResource == null) return count;
            foreach (var permissionType in registeredResource.StaticPermissions)
            {
                count += Enum.GetNames(permissionType).Length;
            }
            return count;
        }

        public virtual async Task<IEnumerable<ResourceInstanceInfo>> GetAllResourceInstancesAsync(string resource, int pageIndex = 1, int pageSize = 10, CancellationToken token = default(CancellationToken))
        {
            var result = new List<ResourceInstanceInfo>();
            var resourceType = _namingConvertor.GetResourceTypeByUniqueName(resource);
            var instanceDetails = await _resourceInstanceProvider.GetInstancesAsync(resourceType, pageIndex, pageSize, token);
            foreach (var detail in instanceDetails)
            {
                result.Add(new ResourceInstanceInfo
                {
                    Identifier = (TKey)detail.ResourceUniqueIdentifier,
                    Name = detail.ResourceName,
                    Description = detail.ResourceDescription,
                    PermissionsCount = GetInstancePermissionsCount(resource)
                });
            }
            return result.OrderBy(r => r.Name).ToList();
        }

        private int GetInstancePermissionsCount(string resource)
        {
            var count = 0;
            var registeredResource = GetRegisteredResource(resource);
            if (registeredResource == null) return count;
            foreach (var permissionType in registeredResource.InstancePermissions)
            {
                count += Enum.GetNames(permissionType).Length;
            }
            return count;
        }

        #region GetAllPermissions

        public virtual async Task<IEnumerable<PermissionInfo>> GetAllPermissionsAsync(object roleId, string resource, CancellationToken token = default(CancellationToken))
        {
            var result = new List<PermissionInfo>();
            var registeredResource = GetRegisteredResource(resource);
            if (registeredResource == null) return result;
            foreach (var permissionType in registeredResource.StaticPermissions)
            {
                var enumNames = Enum.GetNames(permissionType);
                foreach (var enumName in enumNames)
                {
                    var memberInfo = permissionType.GetField(enumName);
                    var permissionUniqueIdentifier = _namingConvertor.GetPermissionUniqueIdentifier(permissionType, memberInfo);
                    var info = new PermissionInfo()
                    {
                        UniqueIdentifier = permissionUniqueIdentifier,
                        IsAllowed = await _permissionStore.IsAllowedAsync((TKey)roleId, resource, permissionUniqueIdentifier, token),
                        Name = Helpers.GetPermissonReadableName(memberInfo),
                        Description = Helpers.GetPermissonReadableDescription(memberInfo)
                    };

                    result.Add(info);
                }
            }
            return result.OrderBy(r => r.Name).ToList();
        }

        public virtual async Task<IEnumerable<PermissionInfo>> GetAllPermissionsAsync(object roleId, string resource, object resourceId, CancellationToken token = default(CancellationToken))
        {
            var result = new List<PermissionInfo>();
            var registeredResource = GetRegisteredResource(resource);
            if (registeredResource == null) return result;
            foreach (var permissionType in registeredResource.InstancePermissions)
            {
                var enumNames = Enum.GetNames(permissionType);
                foreach (var enumName in enumNames)
                {
                    var memberInfo = permissionType.GetField(enumName);
                    var permissionUniqueIdentifier = _namingConvertor.GetPermissionUniqueIdentifier(permissionType, memberInfo);
                    var info = new PermissionInfo()
                    {
                        UniqueIdentifier = permissionUniqueIdentifier,
                        IsAllowed = await _permissionStore.IsAllowedAsync((TKey)roleId, resource, (TKey)resourceId, permissionUniqueIdentifier, token),
                        Name = Helpers.GetPermissonReadableName(memberInfo),
                        Description = Helpers.GetPermissonReadableDescription(memberInfo)
                    };

                    result.Add(info);
                }
            }
            return result.OrderBy(r => r.Name).ToList();
        }

        private ResourceRegistry.RegisteredResource GetRegisteredResource(string resource)
        {
            var resourceType = _namingConvertor.GetResourceTypeByUniqueName(resource);
            return _resourceRegistry.ResourceTypes.FirstOrDefault(r => r.ResourceType == resourceType);
        }


        #endregion
    }
}
