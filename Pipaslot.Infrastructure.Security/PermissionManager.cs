using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Pipaslot.Infrastructure.Security.Attributes;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.Infrastructure.Security.Data.Queries;

namespace Pipaslot.Infrastructure.Security
{
    class PermissionManager<TKey> : IPermissionManager<TKey>
    {
        private readonly IPermissionStore<TKey> _permissionStore;
        private readonly ResourceRegistry<TKey> _resourceRegistry;
        private readonly IResourceDetailProvider<TKey> _resourceDetailProvider;
        private readonly INamingConvertor _namingConvertor;

        public PermissionManager(IPermissionStore<TKey> permissionStore, ResourceRegistry<TKey> resourceRegistry, IResourceDetailProvider<TKey> resourceDetailProvider = null, INamingConvertor namingConvertor = null)
        {
            _namingConvertor = namingConvertor ?? new DefaultNamingConvertor();
            _permissionStore = permissionStore;
            _resourceRegistry = resourceRegistry;
            _resourceDetailProvider = resourceDetailProvider ?? new DefaultResourceDetailProvider<TKey>();
        }

        #region Setup Privilege

        public virtual void Allow(IUserRole<TKey> role, Type resource, TKey resourceId, IConvertible permissionEnum)
        {
            SetPrivilege(role, resource, resourceId, permissionEnum, true);
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

        public IEnumerable<ResourceInfo> GetAllResources()
        {
            var result = new List<ResourceInfo>();
            foreach (var pair in _resourceRegistry.ResourceTypes)
            {
                var type = pair.Key;
                var resourceName = _namingConvertor.GetResourceUniqueName(type);
                var info = new ResourceInfo
                {
                    InstancesCount = _permissionStore.GetResourceInstanceCount(resourceName),
                    UniquedName = resourceName
                };
                if (type.GetCustomAttributes(typeof(NameAttribute)).FirstOrDefault() is NameAttribute nameAttr)
                {
                    info.Name = nameAttr.Name;
                }
                else
                {
                    info.Name = type.FullName;
                }
                if (type.GetCustomAttributes(typeof(DescriptionAttribute)).FirstOrDefault() is DescriptionAttribute descAttr)
                {
                    info.Description = descAttr.Description;
                }
                result.Add(info);
            }
            return result;
        }

        //todo Implement paging for future
        public IEnumerable<ResourceInstanceInfo<TKey>> GetAllResourceInstancess(string resource)
        {
            var result = new List<ResourceInstanceInfo<TKey>>();
            var instances = _permissionStore.GetAllResourceInstancesIds(resource);
            var instanceDetails = _resourceDetailProvider.GetResourceDetails(instances);
            foreach (var detail in instanceDetails)
            {
                result.Add(new ResourceInstanceInfo<TKey>
                {
                    Identifier = detail.Id,
                    UniquedName = resource,
                    Name = detail.Name,
                    Description = detail.Description
                });
            }
            return result;
        }

        public IEnumerable<PermissionInfo<TKey>> GetAllPermissions(TKey roleId, string resource, TKey resourceId)
        {
            var result = new List<PermissionInfo<TKey>>();
            var resourceType = _namingConvertor.GetResourceTypeByUniqueName(resource);
            var permissionsTypes = _resourceRegistry.GetPermissionTypes(resourceType);
            foreach (var permissionType in permissionsTypes)
            {
                foreach (var propertyInfo in permissionType.GetProperties())
                {
                    var permissionUniqueIdentifier = _namingConvertor.GetPermissionUniqueIdentifier(permissionType, propertyInfo);
                    var info = new PermissionInfo<TKey>()
                    {
                        ResourceUniquedName = resource,
                        ResourceIdentifier = resourceId,
                        UniqueIdentifier = permissionUniqueIdentifier,
                        IsAllowed = _permissionStore.IsAllowed(roleId, resource, resourceId, permissionUniqueIdentifier)
                    };
                    if (propertyInfo.GetCustomAttributes(typeof(NameAttribute)).FirstOrDefault() is NameAttribute nameAttr)
                    {
                        info.Name = nameAttr.Name;
                    }
                    if (propertyInfo.GetCustomAttributes(typeof(DescriptionAttribute)).FirstOrDefault() is DescriptionAttribute descAttr)
                    {
                        info.Description = descAttr.Description;
                    }

                    result.Add(info);
                }
            }
            return result;
        }
    }
}
