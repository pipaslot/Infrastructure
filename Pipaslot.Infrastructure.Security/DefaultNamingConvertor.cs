using System;
using System.Collections.Generic;
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
        public string GetPermissionUniqueIdentifier(IConvertible permissionEnum)
        {
            return permissionEnum.GetType().FullName + "." + permissionEnum;
        }
    }
}
