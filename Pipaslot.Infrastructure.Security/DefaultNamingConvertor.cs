using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Pipaslot.Infrastructure.Security
{
    /// <inheritdoc />
    public class DefaultNamingConvertor<TKey> : INamingConvertor
    {
        private readonly ResourceRegistry<TKey> _resourceRegistry;

        public DefaultNamingConvertor(ResourceRegistry<TKey> resourceRegistry)
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
    }
}
