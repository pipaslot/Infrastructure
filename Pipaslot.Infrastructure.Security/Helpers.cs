using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Pipaslot.Infrastructure.Security.Attributes;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.Infrastructure.Security.Exceptions;

namespace Pipaslot.Infrastructure.Security
{
    public static class Helpers
    {
        /// <summary>
        /// Delimiter used for role claim to joint role parameters into one string
        /// </summary>
        public static string RoleClaimFieldDelimiter = "|#|";
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
            return field.Name;
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
            return type.Name;
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

        #region Role to Claim conversion

        public static List<Claim> RolesToClaims(IEnumerable<IRole> roles)
        {
            var claims = new List<Claim>();
            foreach (var role in roles)
            {
                var roleClaim = RoleToClaim(role);
                claims.Add(roleClaim);
            }
            return claims;
        }

        public static Claim RoleToClaim(IRole role)
        {
            var roleClaim = string.Join(RoleClaimFieldDelimiter, role.Name, role.Id?.ToString(), ((int)role.Type).ToString());
            return new Claim(ClaimTypes.Role, roleClaim);
        }

        public static List<IRole> ClaimsToRoles(IEnumerable<Claim> claims)
        {
            var roles = new List<IRole>();
            foreach (var claim in claims)
            {
                var role = ClaimToRole(claim);
                roles.Add(role);
            }
            return roles;
        }

        public static IRole ClaimToRole(Claim claim)
        {
            var roleFields = claim.Value.Split(new [] { RoleClaimFieldDelimiter },StringSplitOptions.None);
            var role = new Role { Name = roleFields[0] };
            if (roleFields.Length >= 2)
            {
                role.Id = roleFields[1];
            }
            if (roleFields.Length >= 3 && Enum.TryParse<RoleType>(roleFields[2], out var type))
            {
                role.Type = type;
            }
            return role;
        }
        #endregion
    }
}
