using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Pipaslot.Infrastructure.Security
{
    /// <summary>
    /// Tool for automatic Resource registration from assemblies
    /// </summary>
    public class ResourceRegistry<TKey>
    {
        /// <summary>
        /// Registered assemblies for scaning
        /// </summary>
        private readonly List<Assembly> _scanedAsseblies = new List<Assembly>();

        /// <summary>
        /// Flag if resources should be reloaded because new assembly was added
        /// </summary>
        private bool _reloadAll = true;

        /// <summary>
        /// Key is resource type and value is lis of assigned permission Enums
        /// </summary>
        private readonly Dictionary<Type, List<Type>> _loadedResources = new Dictionary<Type, List<Type>>();

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
        public Dictionary<Type, List<Type>> ResourceTypes
        {
            get
            {
                if (_reloadAll)
                {
                    Load();
                }
                return _loadedResources;
            }
        }

        /// <summary>
        /// Returns all permission types for resource type
        /// </summary>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        public List<Type> GetPermissionTypes(Type resourceType)
        {
            if (ResourceTypes.ContainsKey(resourceType))
            {
                return ResourceTypes[resourceType];
            }
            return new List<Type>();
        }

        /// <summary>
        /// Loads all resources from assemblies
        /// </summary>
        /// <returns></returns>
        private void Load()
        {
            var resourceGenericType = typeof(IResource<,>);
            var keyType = typeof(TKey);

            _loadedResources.Clear();

            foreach (var type in _scanedAsseblies.SelectMany(s => s.GetTypes()))
            {
                if (type.IsAbstract || type.IsInterface) continue;
                var isResource = false;
                var permissions = new List<Type>();
                foreach (var iface in type.GetInterfaces())
                {
                    if (iface.IsGenericType
                        && resourceGenericType.IsAssignableFrom(iface.GetGenericTypeDefinition())
                        && iface.GenericTypeArguments.First() == keyType)
                    {
                        isResource = true;
                        permissions.Add(iface.GenericTypeArguments.Skip(1).First());
                    }
                }
                if (isResource)
                {
                    _loadedResources.Add(type, permissions.Distinct().ToList());
                }
            }
        }
    }
}
