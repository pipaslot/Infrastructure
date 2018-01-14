using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipaslot.Infrastructure.Security
{
    public class PermissionCache<TKey>
    {

        private readonly List<RoleList<TKey>> _roleLists = new List<RoleList<TKey>>();

        public async Task<bool> LoadAsync(Func<Task<bool?>> callback, IEnumerable<TKey> roleIds, string permission, string resource, string resourceId = null)
        {
            var newRoles = new RoleList<TKey>(roleIds);
            //Find or create roles
            var existingRoles = _roleLists.FirstOrDefault(r => r.Key == newRoles.Key);
            if (existingRoles == null)
            {
                _roleLists.Add(newRoles);
                existingRoles = newRoles;
            }
            //Find or create resource
            var existingResource = existingRoles.Resources.FirstOrDefault(r => r.Name.Equals(resource));
            if (existingResource == null)
            {
                existingResource = new Resource(resource);
                existingRoles.Resources.Add(existingResource);
            }
            //Find or create instance
            var existingInstance = existingResource.Instances.FirstOrDefault(i => i.Id == resourceId);
            if (existingInstance == null)
            {
                existingInstance = new Instance(resourceId);
                existingResource.Instances.Add(existingInstance);
            }
            //Find or create permission
            var existingPermission = existingInstance.Permissions.FirstOrDefault(p => p.Name == permission);
            if (existingPermission == null)
            {
                var isAllowed = await callback() ?? false;
                existingPermission = new Permission(permission, isAllowed);
                existingInstance.Permissions.Add(existingPermission);
            }
            return existingPermission.IsAllowed;
        }

        /// <summary>
        /// Remove cached record
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="resource"></param>
        /// <param name="resourceId"></param>
        /// <param name="permission"></param>
        public void Clear(TKey roleId, string resource, string resourceId, string permission)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
            {
                resourceId = null;
            }
            var roleLists = _roleLists.Where(r => r.Ids.Contains(roleId));
            foreach (var roleList in roleLists)
            {
                var resources = roleList.Resources.Where(r => r.Name.Equals(resource));
                foreach (var res in resources)
                {
                    var instances = res.Instances.Where(i => i.Id == resourceId);
                    foreach (var ins in instances)
                    {
                        ins.Permissions.RemoveAll(p => p.Name == permission);
                    }
                }
            }
        }

        internal class RoleList<TKey>
        {
            public string Key { get; }
            public List<TKey> Ids { get; }
            public List<Resource> Resources { get; } = new List<Resource>();

            public RoleList(IEnumerable<TKey> ids)
            {
                Ids = ids.OrderBy(i => i).ToList();
                Key = string.Join("#", Ids);
            }
        }

        internal class Resource
        {
            public string Name { get; }
            public List<Instance> Instances { get; } = new List<Instance>();

            public Resource(string name)
            {
                Name = name;
            }
        }

        internal class Instance
        {
            public string Id { get; }
            public List<Permission> Permissions { get; } = new List<Permission>();

            public Instance(string id)
            {
                Id = id;
            }
        }

        internal class Permission
        {
            public string Name { get; }
            public bool IsAllowed { get; }

            public Permission(string name, bool isAllowed)
            {
                Name = name;
                IsAllowed = isAllowed;
            }
        }
    }
}
