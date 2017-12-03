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
            var resourceGenericType = typeof(IResource<>);
            var resourceContainsresourcePermissiongenericType = resource.GetInterfaces().Any(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == resourceGenericType &&
                x.GetGenericArguments().First() == permissionEnum.GetType());
            if (!resourceContainsresourcePermissiongenericType)
            {
                throw new NotSuitablePermissionException(resource, permissionEnum);
            }
        }
    }
}
