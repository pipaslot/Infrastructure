using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Data.EntityFramework;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.Infrastructure.Security.EntityFramework.Entities;

namespace Pipaslot.Infrastructure.Security.EntityFramework
{
    public class PermissionStore<TKey, TDbContext> : EntityFrameworkRepository<TDbContext, Privilege<TKey>, TKey>, IPermissionStore<TKey>
    where TDbContext : DbContext, ISecurityDbContext<TKey>
    {
        public PermissionStore(IUnitOfWorkFactory uowFactory, IEntityFrameworkDbContextFactory dbContextFactory) : base(uowFactory, dbContextFactory)
        {
        }

        public bool IsAllowed(TKey roleId, string resource, string permission)
        {
            var resourceId = default(TKey);
            return IsAllowed(roleId, resource, resourceId, permission);
        }

        public bool IsAllowed(IEnumerable<TKey> roleIds, string resource, string permission)
        {
            var resourceId = default(TKey);
            return IsAllowed(roleIds, resource, resourceId, permission);
        }

        public bool IsAllowed(TKey roleId, string resource, TKey resourceId, string permission)
        {
            return IsAllowed(new []{ roleId }, resource, resourceId, permission);
        }

        public bool IsAllowed(IEnumerable<TKey> roleIds, string resource, TKey resourceId, string permission)
        {
            return ContextReadOnly.SecurityPrivilege.Any(p => roleIds.Contains(p.Role) &&
                                                              p.Resource == resource &&
                                                              p.ResourceInstance.Equals(resourceId) &&
                                                              p.Permission == permission &&
                                                              p.IsAllowed);
        }

        public IEnumerable<TKey> GetAllowedResourceIds(TKey roleId, string resource, string permission)
        {
            return GetAllowedResourceIds(new[] {roleId}, resource, permission);
        }

        public IEnumerable<TKey> GetAllowedResourceIds(IEnumerable<TKey> roleIds, string resource, string permission)
        {
            return ContextReadOnly.SecurityPrivilege.Where(p => roleIds.Contains(p.Role) &&
                                                                p.Resource == resource &&
                                                                p.Permission == permission &&
                                                                p.IsAllowed)
                .Select(d => d.ResourceInstance)
                .ToList();
        }

        public void SetPrivilege(TKey roleId, string resource, string permission, bool isAllowed)
        {
            SetPrivilege(roleId, resource, default(TKey), permission, isAllowed);
        }

        public void SetPrivilege(TKey roleId, string resource, TKey resourceId, string permission, bool isAllowed)
        {
            var existing = Context.SecurityPrivilege.FirstOrDefault(p => p.Role.Equals(roleId) &&
                                                     p.Resource == resource &&
                                                     p.ResourceInstance.Equals(resourceId) &&
                                                     p.Permission == permission);
            if (existing == null)
            {
                existing = new Privilege<TKey>()
                {
                    Role = roleId,
                    Resource = resource,
                    ResourceInstance = resourceId,
                    Permission = permission,
                    IsAllowed = isAllowed
                };
                Context.SecurityPrivilege.Add(existing);
            }
            existing.IsAllowed = isAllowed;
        }

        public int GetResourceInstanceCount(string resourceName)
        {
            return ContextReadOnly.SecurityPrivilege
                .Count(p => p.Resource == resourceName &&
                            !p.ResourceInstance.Equals(default(TKey)));

        }

        public List<TKey> GetAllResourceInstancesIds(string resource)
        {
            return ContextReadOnly.SecurityPrivilege
                .Where(p => p.Resource == resource &&
                            !p.ResourceInstance.Equals(default(TKey)))
                .Select(d => d.ResourceInstance)
                .ToList();
        }
    }
}
