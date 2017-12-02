using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Pipaslot.Infrastructure.Security
{
    /// <inheritdoc />
    public class DefaultNamingConvertor : INamingConvertor
    {
        /// <inheritdoc />
        public string GetResourceUniqueName(Type resource)
        {
            return resource.FullName;
        }

        /// <inheritdoc />
        public Type GetResourceTypeByUniqueName(string uniqueName)
        {
            return Type.GetType(uniqueName);
        }

        /// <inheritdoc />
        public string GetPermissionUniqueIdentifier(IConvertible permissionEnum)
        {
            return permissionEnum.GetType().FullName + "." + permissionEnum;
        }

        /// <inheritdoc />
        public string GetPermissionUniqueIdentifier(Type permissionClass, PropertyInfo property)
        {
            return permissionClass.FullName + "." + property.Name;
        }
    }
}
