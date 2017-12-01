﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.Security
{
    public class Authorizator<TKey> : IAuthorizator<TKey>
    {
        public const string GLOBAL_RESOURCE_NAME = "global";

        protected readonly IPermissionStore<TKey> _permissionStore;
        protected readonly INamingConvertor _namingConvertor;

        public Authorizator(IPermissionStore<TKey> permissionStore, INamingConvertor namingConvertor = null)
        {
            _permissionStore = permissionStore;
            _namingConvertor = namingConvertor ?? new DefaultNamingConvertor();
        }

        public virtual bool IsAllowed(IUserRole<TKey> role, IConvertible permissionEnum)
        {
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return _permissionStore.IsAllowed(role.Id, GLOBAL_RESOURCE_NAME, default(TKey), perm);
        }

        public virtual bool IsAllowed<TPermissions>(IUserRole<TKey> role, IResource<TKey, TPermissions> resource, TPermissions permissionEnum) where TPermissions : IConvertible
        {
            var res = _namingConvertor.GetResourceUniqueName(resource.GetType());
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return _permissionStore.IsAllowed(role.Id, res, resource.ResourceUniqueIdentifier, perm);
        }

        public virtual bool IsAllowed(IUserRole<TKey> role, Type resource, TKey resourceIdentifier, IConvertible permissionEnum)
        {
            Helpers.CheckIfResourceHasAssignedPermission(resource, permissionEnum);
            var res = _namingConvertor.GetResourceUniqueName(resource);
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return _permissionStore.IsAllowed(role.Id, res, resourceIdentifier, perm);
        }

        public virtual IEnumerable<TKey> GetAllowedKeys(IUserRole<TKey> role, Type resource, IConvertible permissionEnum)
        {
            Helpers.CheckIfResourceHasAssignedPermission(resource, permissionEnum);
            var res = _namingConvertor.GetResourceUniqueName(resource);
            var perm = _namingConvertor.GetPermissionUniqueIdentifier(permissionEnum);
            return _permissionStore.GetAllowedResourceIds(role.Id, res, perm);
        }
    }
}
