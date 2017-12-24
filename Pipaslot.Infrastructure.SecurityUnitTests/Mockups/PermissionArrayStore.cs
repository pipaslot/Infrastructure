using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.SecurityTests.Mockups
{
    public class PermissionArrayStore : IPermissionStore<int>
    {
        private readonly List<Record> _data = new List<Record>();

        public Task<bool?> IsAllowedAsync(int roleId, string resource, string permission, CancellationToken token = default(CancellationToken))
        {
            return IsAllowedAsync(roleId, resource, default(int), permission, token);
        }

        public Task<bool?> IsAllowedAsync(IEnumerable<int> roleIds, string resource, string permission, CancellationToken token = default(CancellationToken))
        {
            return IsAllowedAsync(roleIds, resource, default(int), permission, token);
        }

        public Task<bool?> IsAllowedAsync(int roleId, string resource, int resourceId, string permission, CancellationToken token = default(CancellationToken))
        {
            return IsAllowedAsync(new[] { roleId }, resource, resourceId, permission, token);
        }

        public Task<bool?> IsAllowedAsync(IEnumerable<int> roleIds, string resource, int resourceId, string permission, CancellationToken token = default(CancellationToken))
        {
            var result = _data.FirstOrDefault(d => roleIds.Contains(d.Role) &&
                                        d.Resource == resource &&
                                        d.ResourceId == resourceId &&
                                        d.Permission == permission &&
                                        d.IsAllowed);
            return Task.FromResult(result?.IsAllowed);
        }

        public Task<IEnumerable<int>> GetAllowedResourceIdsAsync(int roleId, string resource, string permission, CancellationToken token = default(CancellationToken))
        {
            return GetAllowedResourceIdsAsync(new[] { roleId }, resource, permission, token);
        }

        public Task<IEnumerable<int>> GetAllowedResourceIdsAsync(IEnumerable<int> roleIds, string resource, string permission, CancellationToken token = default(CancellationToken))
        {
            var result = _data.Where(d => roleIds.Contains(d.Role) &&
                                          d.Resource == resource &&
                                          d.Permission == permission &&
                                          d.IsAllowed)
                .Select(d => d.ResourceId).ToList().AsEnumerable();
            return Task.FromResult(result);
        }

        public void SetPrivilege(int roleId, string resource, string permission, bool? isAllowed)
        {
            SetPrivilege(roleId, resource, default(int), permission, isAllowed);
        }

        public void SetPrivilege(int roleId, string resource, int resourceId, string permission, bool? isAllowed)
        {
            var existing = _data.FirstOrDefault(d => d.Role == roleId &&
                                          d.Resource == resource &&
                                          d.ResourceId == resourceId &&
                                          d.Permission == permission);
            if (isAllowed == null)
            {
                if (existing != null) _data.Remove(existing);
                return;
            }
            if (existing == null)
            {
                existing = new Record(roleId, resource, resourceId, permission, isAllowed ?? false);
                _data.Add(existing);
            }
            existing.IsAllowed = isAllowed ?? false;
        }

        internal class Record
        {
            public int Role { get; }
            public string Resource { get; }
            public int ResourceId { get; }
            public string Permission { get; }
            public bool IsAllowed { get; set; }

            public Record(int role, string resource, int resourceId, string permission, bool isAllowed)
            {
                Role = role;
                Resource = resource;
                ResourceId = resourceId;
                Permission = permission;
                IsAllowed = isAllowed;
            }
        }

    }
}
