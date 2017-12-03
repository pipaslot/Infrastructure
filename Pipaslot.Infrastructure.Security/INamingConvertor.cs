using System;
using System.Collections.Generic;
using System.Reflection;
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
        /// Returns resource type acording to unique name
        /// </summary>
        /// <param name="uniqueName"></param>
        /// <returns></returns>
        Type GetResourceTypeByUniqueName(string uniqueName);

        /// <summary>
        /// Generate Unique identifier for permission
        /// </summary>
        /// <param name="permissionEnum">Requested permission</param>
        /// <returns></returns>
        string GetPermissionUniqueIdentifier(IConvertible permissionEnum);

        /// <summary>
        /// Generate Unique identifier for permission class ans property
        /// </summary>
        /// <param name="permissionClass"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        string GetPermissionUniqueIdentifier(Type permissionClass, MemberInfo property);
    }
}
