using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Pipaslot.Infrastructure.Security
{
    /// <summary>
    /// Tool for automatic Resource registration from assemblies
    /// </summary>
    public class ResourceRegistry
    {
        /// <summary>
        /// Registered assemblies for scaning
        /// </summary>
        private readonly List<Assembly> _scanedAsseblies = new List<Assembly>();

        /// <summary>
        /// Registered assemblies for scaning
        /// </summary>
        public List<Assembly> RegisteredAssemblies => _scanedAsseblies.ToList();

        /// <summary>
        /// Flag if resources should be reloaded because new assembly was added
        /// </summary>
        private bool _reloadAll = true;

        /// <summary>
        /// Key is resource type and value is lis of assigned permission Enums
        /// </summary>
        private List<RegisteredResource> _loadedResources = new List<RegisteredResource>();

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<Type, List<Type>> _staticPermissionsUsedInResources = new Dictionary<Type, List<Type>>();

        /// <summary>
        /// Lock object for load operation
        /// </summary>
        private readonly object _loadLock = new object();

        public ResourceRegistry()
        {
            var entry = Assembly.GetEntryAssembly();
            if (entry != null) Register(entry);
        }

        /// <summary>
        /// Register new assembly to be scaned for resources
        /// </summary>
        /// <param name="assembly"></param>
        public void Register(Assembly assembly)
        {
            if (!_scanedAsseblies.Contains(assembly))
            {
                _scanedAsseblies.Add(assembly);
                _reloadAll = true;
            }
        }

        /// <summary>
        /// Return all resources from registered assemblies implementing IResource interface with currently used generic type for TKey
        /// Key is resource type and value is lis of assigned permission Enums
        /// </summary>
        public List<RegisteredResource> ResourceTypes
        {
            get
            {
                Load();
                return _loadedResources;
            }
        }

        public Type ResolveResource(IConvertible permissionEnum)
        {
            Load();
            if (_staticPermissionsUsedInResources.TryGetValue(permissionEnum.GetType(), out var resources))
            {
                if (resources.Count == 1)
                {
                    return resources.First();
                }
                throw new ApplicationException($"Static Permission {permissionEnum.GetType().FullName} is used in {resources.Count} resources, but only single ussage is supported.");
            }
            throw new ApplicationException("No resource found for permission enum "+ permissionEnum.GetType().FullName);
        }

        /// <summary>
        /// Loads all resources from assemblies
        /// </summary>
        /// <returns></returns>
        private void Load()
        {
            if (!_reloadAll)
            {
                return;
            }
            lock (_loadLock)
            {
                if (!_reloadAll)
                {
                    return;
                }
                _loadedResources = LoadResources();
                _staticPermissionsUsedInResources = LoadPermissions(_loadedResources);
                
                _reloadAll = false;
            }
        }

        /// <summary>
        /// Load resources from all registered assemblies
        /// </summary>
        /// <returns></returns>
        private List<RegisteredResource> LoadResources()
        {
            var resourceGenericType = typeof(IResource<>);
            var resourceInstanceGenericType = typeof(IResourceInstance<>);

            var resources = new List<RegisteredResource>();
            foreach (var type in _scanedAsseblies.SelectMany(s => s.GetTypes()))
            {
                if (type.IsAbstract || type.IsInterface) continue;
                var res = new RegisteredResource(type);
                foreach (var iface in type.GetInterfaces())
                {
                    if (!iface.IsGenericType) continue;
                    var genericDef = iface.GetGenericTypeDefinition();
                    if (resourceInstanceGenericType.IsAssignableFrom(genericDef))
                    {
                        res.InstancePermissions.Add(iface.GenericTypeArguments.Last());
                    }
                    else if (resourceGenericType.IsAssignableFrom(genericDef))
                    {
                        res.StaticPermissions.Add(iface.GenericTypeArguments.Last());
                    }
                }
                if (res.StaticPermissions.Count > 0 || res.InstancePermissions.Count > 0)
                {
                    resources.Add(res);
                }
            }
            return resources;
        }

        /// <summary>
        /// Prepare cache object for permission reading from all registered resources
        /// </summary>
        /// <param name="loadedResources"></param>
        /// <returns></returns>
        private Dictionary<Type, List<Type>> LoadPermissions(List<RegisteredResource> loadedResources)
        {
            var result = new Dictionary<Type, List<Type>>();
            foreach (var registeredResource in loadedResources)
            {
                foreach (var permission in registeredResource.StaticPermissions)
                {
                    if (result.TryGetValue(permission, out var resources))
                    {
                        if (!resources.Contains(registeredResource.ResourceType))
                        {
                            resources.Add(registeredResource.ResourceType);
                        }
                    }
                    else
                    {
                        result.Add(permission,new List<Type>() { registeredResource.ResourceType});
                    }
                }
            }
            return result;
        }

        public class RegisteredResource
        {
            /// <summary>
            /// Resource type implementing IResource interface
            /// </summary>
            public Type ResourceType { get; }

            /// <summary>
            /// Permissions defined by IResource interface
            /// </summary>
            public List<Type> StaticPermissions = new List<Type>();

            /// <summary>
            /// Permissions defined by IResourceInterface interface
            /// </summary>
            public List<Type> InstancePermissions = new List<Type>();

            public RegisteredResource(Type resourceType)
            {
                ResourceType = resourceType;
            }
        }
    }
}
