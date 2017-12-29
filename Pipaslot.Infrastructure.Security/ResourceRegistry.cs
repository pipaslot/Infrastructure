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
        private readonly List<RegisteredResource> _loadedResources = new List<RegisteredResource>();

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
                if (_reloadAll)
                {
                    Load();
                    _reloadAll = false;
                }
                return _loadedResources;
            }
        }

        /// <summary>
        /// Loads all resources from assemblies
        /// </summary>
        /// <returns></returns>
        private void Load()
        {
            var resourceGenericType = typeof(IResource<>);
            var resourceInstanceGenericType = typeof(IResourceInstance<,>);

            _loadedResources.Clear();

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
                    _loadedResources.Add(res);
                }
            }
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
