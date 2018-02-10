using System;
using System.Reflection;

namespace Pipaslot.Infrastructure.Security
{
    /// <inheritdoc />
    public class DefaultNamingConvertor : INamingConvertor
    {
        private readonly ResourceRegistry _resourceRegistry;

        public DefaultNamingConvertor(ResourceRegistry resourceRegistry)
        {
            _resourceRegistry = resourceRegistry;
        }

        /// <inheritdoc />
        public string GetResourceUniqueName(Type resource)
        {
            return resource.FullName;
        }

        /// <inheritdoc />
        public Type GetResourceTypeByUniqueName(string uniqueName)
        {
            foreach (var assembly in _resourceRegistry.RegisteredAssemblies)
            {
                var type = Type.GetType(uniqueName+", "+assembly);
                if (type != null)
                {
                    return type;
                }
            }
            throw new ArgumentOutOfRangeException($"Can not find type '{uniqueName}' in assemblies: {_resourceRegistry.RegisteredAssemblies}");
        }

        /// <inheritdoc />
        public string GetPermissionUniqueIdentifier(IConvertible permissionEnum)
        {
            return permissionEnum.GetType().FullName + "." + permissionEnum;
        }

        /// <inheritdoc />
        public string GetPermissionUniqueIdentifier(Type permissionClass, MemberInfo property)
        {
            return permissionClass.FullName + "." + property.Name;
        }

        public IConvertible GetPermissionByUniqueName(string uniqueName)
        {
            var lastDot = uniqueName.LastIndexOf('.');
            var className = uniqueName.Substring(0, lastDot);
            var permissionName = uniqueName.Substring(lastDot+1);
            foreach (var assembly in _resourceRegistry.RegisteredAssemblies)
            {
                var type = Type.GetType(className + ", " + assembly);
                if (type != null)
                {
                    return (IConvertible)Enum.Parse(type, permissionName, ignoreCase: true);
                }
            }
            throw new ArgumentOutOfRangeException($"Can not find type '{uniqueName}' in assemblies: {_resourceRegistry.RegisteredAssemblies}");

        }
    }
}
