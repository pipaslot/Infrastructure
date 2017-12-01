using System;
using System.Collections.Generic;
using System.Text;

namespace Pipaslot.Infrastructure.Security.Exceptions
{
    public class NotSuitablePermissionException : Exception
    {
        /// <summary>
        /// Resource Class Type
        /// </summary>
        public Type Resource { get; }

        /// <summary>
        /// Permission Enum not defined for Resource
        /// </summary>
        public IConvertible Permission { get; }

        public NotSuitablePermissionException(Type resource, IConvertible permission) : base("Permission is not suitable for Resource. Please check if you are using correct PermissionEnum defined at resource interface into IResource<TKey, PermissionEnum>")
        {
            Resource = resource;
            Permission = permission;
        }


    }
}
