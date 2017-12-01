using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pipaslot.Infrastructure.Security.Exceptions;

namespace Pipaslot.Infrastructure.Security
{
    internal static class Helpers
    {
        internal static void CheckIfResourceHasAssignedPermission(Type resource, IConvertible permissionEnum)
        {
            var resourceContainsresourcePermissiongenericType = resource.GetInterfaces().Any(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IResource<,>) &&
                x.GetGenericArguments().Skip(1).First() == permissionEnum.GetType());
            if (!resourceContainsresourcePermissiongenericType)
            {
                throw new NotSuitablePermissionException(resource, permissionEnum);
            }
        }
    }
}
