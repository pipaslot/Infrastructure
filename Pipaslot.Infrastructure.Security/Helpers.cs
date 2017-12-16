using System;
using System.Linq;
using System.Reflection;
using Pipaslot.Infrastructure.Security.Attributes;
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

        #region Permission Attribute data extraction

        internal static string GetPermissonReadableName(IConvertible permissionEnum)
        {
            var field = permissionEnum.GetType().GetField(permissionEnum.ToString());
            return GetPermissonReadableName(field);
        }

        internal static string GetPermissonReadableName(FieldInfo field)
        {
            if (field.GetCustomAttributes(typeof(NameAttribute)).FirstOrDefault() is NameAttribute nameAttr)
            {
                return nameAttr.Name;
            }
            return string.Empty;
        }

        internal static string GetPermissonReadableDescription(IConvertible permissionEnum)
        {
            var field = permissionEnum.GetType().GetField(permissionEnum.ToString());
            return GetPermissonReadableDescription(field);
        }

        internal static string GetPermissonReadableDescription(FieldInfo field)
        {
            if (field.GetCustomAttributes(typeof(DescriptionAttribute)).FirstOrDefault() is DescriptionAttribute nameAttr)
            {
                return nameAttr.Description;
            }
            return string.Empty;
        }

        #endregion

        #region Resource Attribute data extraction

        internal static string GetResourceReadableName(Type type)
        {
            if (type.GetCustomAttributes(typeof(NameAttribute)).FirstOrDefault() is NameAttribute nameAttr)
            {
                return nameAttr.Name;
            }
            return type.FullName;
        }

        internal static string GetResourceReadableDescription(Type type)
        {
            if (type.GetCustomAttributes(typeof(DescriptionAttribute)).FirstOrDefault() is DescriptionAttribute descAttr)
            {
                return descAttr.Description;
            }
            return string.Empty;
        }

        #endregion
    }
}
