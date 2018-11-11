using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Data.EntityFrameworkCore;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.Infrastructure.Security.EntityFrameworkCore.Entities;

namespace Pipaslot.Infrastructure.Security.EntityFrameworkCore
{
    public class PermissionStore<TKey, TDbContext> : PermissionStore<TKey, TDbContext, Privilege<TKey>>
        where TDbContext : DbContext, ISecurityPrivilegeDbContext<TKey, Privilege<TKey>>
    {
        public PermissionStore(IUnitOfWorkFactory uowFactory, IEntityFrameworkDbContextFactory dbContextFactory) : base(uowFactory, dbContextFactory)
        {
        }
    }

    public class PermissionStore<TKey, TDbContext, TPrivilege>
        : EntityFrameworkRepository<TDbContext, TPrivilege, TKey>, IPermissionStore<TKey>
        where TDbContext : DbContext, ISecurityPrivilegeDbContext<TKey, TPrivilege>
        where TPrivilege : Privilege<TKey>, new()
    {
        public PermissionStore(IUnitOfWorkFactory uowFactory, IEntityFrameworkDbContextFactory dbContextFactory) : base(uowFactory, dbContextFactory)
        {
        }

        public virtual Task<bool?> IsAllowedAsync(TKey roleId, string resource, string permission, CancellationToken token = default(CancellationToken))
        {
            var resourceId = default(TKey);
            return IsAllowedAsync(roleId, resource, resourceId, permission, token);
        }

        public virtual Task<bool?> IsAllowedAsync(IEnumerable<TKey> roleIds, string resource, string permission, CancellationToken token = default(CancellationToken))
        {
            var resourceId = default(TKey);
            return IsAllowedAsync(roleIds, resource, resourceId, permission, token);
        }

        public virtual Task<bool?> IsAllowedAsync(TKey roleId, string resource, TKey resourceId, string permission, CancellationToken token = default(CancellationToken))
        {
            return IsAllowedAsync(new[] { roleId }, resource, resourceId, permission, token);
        }

        public virtual async Task<bool?> IsAllowedAsync(IEnumerable<TKey> roleIds, string resource, TKey resourceId, string permission, CancellationToken token = default(CancellationToken))
        {
            var privilege = await ContextReadOnly.SecurityPrivilege.FirstOrDefaultAsync(p => roleIds.Contains(p.Role) &&
                                                                                        p.Resource == resource &&
                                                                                        p.ResourceInstance.Equals(resourceId) &&
                                                                                        p.Permission == permission, token);
            return privilege?.IsAllowed;
        }

        public virtual Task<IEnumerable<TKey>> GetAllowedResourceIdsAsync(TKey roleId, string resource, string permission, CancellationToken token = default(CancellationToken))
        {
            return GetAllowedResourceIdsAsync(new[] { roleId }, resource, permission, token);
        }

        public virtual async Task<IEnumerable<TKey>> GetAllowedResourceIdsAsync(IEnumerable<TKey> roleIds, string resource, string permission, CancellationToken token = default(CancellationToken))
        {
            return await ContextReadOnly.SecurityPrivilege.Where(p => roleIds.Contains(p.Role) &&
                                                                p.Resource == resource &&
                                                                p.Permission == permission &&
                                                                p.IsAllowed)
                .Select(d => d.ResourceInstance)
                .ToListAsync(token);
        }

        public virtual void SetPrivilege(TKey roleId, string resource, string permission, bool? isAllowed)
        {
            SetPrivilege(roleId, resource, default(TKey), permission, isAllowed);
        }

        public virtual void SetPrivilege(TKey roleId, string resource, TKey resourceId, string permission, bool? isAllowed)
        {
            var existing = Context.SecurityPrivilege.FirstOrDefault(p => p.Role.Equals(roleId) &&
                                                     p.Resource == resource &&
                                                     p.ResourceInstance.Equals(resourceId) &&
                                                     p.Permission == permission);
            if (isAllowed == null)
            {
                //unset
                if (existing != null)
                {
                    Context.SecurityPrivilege.Remove(existing);
                }
                return;
            }
            if (existing == null)
            {
                existing = new TPrivilege()
                {
                    Role = roleId,
                    Resource = resource,
                    ResourceInstance = resourceId,
                    Permission = permission,
                    IsAllowed = isAllowed ?? false
                };
                Context.SecurityPrivilege.Add(existing);
            }
            existing.IsAllowed = isAllowed ?? false;
        }
    }
}
