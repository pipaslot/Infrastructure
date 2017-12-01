using System;
using System.Collections.Generic;
using System.Text;

namespace Pipaslot.Infrastructure.Security
{
    /// <summary>
    /// Provides naming conversion from object to string representation which can be stored later
    /// </summary>
    public interface INamingConvertor
    {
        /// <summary>
        /// Generate unique name for resource type
        /// </summary>
        /// <param name="resource">Resource class type</param>
        /// <returns></returns>
        string GetResourceUniqueName(Type resource);

        /// <summary>
        /// Generate Unique identifier for permission
        /// </summary>
        /// <param name="permissionEnum">Requested permission</param>
        /// <returns></returns>
        string GetPermissionUniqueIdentifier(IConvertible permissionEnum);
    }
}
