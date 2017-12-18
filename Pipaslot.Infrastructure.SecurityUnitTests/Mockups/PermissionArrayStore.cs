using System;
using System.Collections.Generic;
using System.Linq;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.SecurityTests.Mockups
{
    public class PermissionArrayStore : IPermissionStore<int>
    {
        private readonly List<Record> _data = new List<Record>();

        public bool IsAllowed(int roleId, string resource, string permission)
        {
            return IsAllowed(roleId, resource, default(int), permission);
        }

        public bool IsAllowed(IEnumerable<int> roleIds, string resource, string permission)
        {
            return IsAllowed(roleIds, resource, default(int), permission);
        }

        public bool IsAllowed(int roleId, string resource, int resourceId, string permission)
        {
            return IsAllowed(new[] { roleId }, resource, resourceId, permission);
        }

        public bool IsAllowed(IEnumerable<int> roleIds, string resource, int resourceId, string permission)
        {
            return _data.Any(d => roleIds.Contains(d.Role) &&
                                  d.Resource == resource &&
                                  d.ResourceId == resourceId &&
                                  d.Permission == permission &&
                                  d.IsAllowed);
        }

        public IEnumerable<int> GetAllowedResourceIds(int roleId, string resource, string permission)
        {
            return GetAllowedResourceIds(new[] { roleId }, resource, permission);
        }

        public IEnumerable<int> GetAllowedResourceIds(IEnumerable<int> roleIds, string resource, string permission)
        {
            return _data.Where(d => roleIds.Contains(d.Role) &&
                                    d.Resource == resource &&
                                    d.Permission == permission &&
                                    d.IsAllowed)
                .Select(d => d.ResourceId).ToList();
        }

        public void SetPrivilege(int roleId, string resource, string permission, bool isAllowed)
        {
            SetPrivilege(roleId, resource, default(int), permission, isAllowed);
        }

        public void SetPrivilege(int roleId, string resource, int resourceId, string permission, bool isAllowed)
        {
            var existing = _data.FirstOrDefault(d => d.Role == roleId &&
                                          d.Resource == resource &&
                                          d.ResourceId == resourceId &&
                                          d.Permission == permission);
            if (existing == null)
            {
                existing = new Record(roleId, resource, resourceId, permission, isAllowed);
                _data.Add(existing);
            }
            existing.IsAllowed = isAllowed;
        }

        public int GetResourceInstanceCount(string resourceName)
        {
            return 2;
        }

        public List<int> GetAllResourceInstancesIds(string resource)
        {
            return new List<int> { 1, 2 };
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
