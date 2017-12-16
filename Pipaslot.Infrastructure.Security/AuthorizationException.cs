using System;
using System.Text;

namespace Pipaslot.Infrastructure.Security
{
    public class AuthorizationException : UnauthorizedAccessException
    {
        public IConvertible Permission { get; }
        public string PermissionName => Helpers.GetPermissonReadableName(Permission);
        private readonly Func<ResourceDetail> _resourceDetailGetter;
        public Type Resource { get; }
        public string ResourceName => Helpers.GetResourceReadableName(Resource);
        private string _resourceInstnanceName;

        public string ResourceInstanceName
        {
            get
            {
                if (_resourceInstnanceName == null)
                {
                    _resourceInstnanceName = _resourceDetailGetter()?.Name ?? "";
                }
                return _resourceInstnanceName;
            }
        }

        public AuthorizationException(IConvertible permissionEnum)
        {
            Permission = permissionEnum;
        }

        public AuthorizationException(Type resource, IConvertible permissionEnum)
        {
            Resource = resource;
            Permission = permissionEnum;
        }

        public AuthorizationException(Type resource, IConvertible permissionEnum, Func<ResourceDetail> resourceDetailGetter)
        {
            _resourceDetailGetter = resourceDetailGetter;
            Resource = resource;
            Permission = permissionEnum;
        }

        public override string Message
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append($"You do not have permission: \"{PermissionName}\"");
                if (Resource != null)
                {
                    sb.Append($" for resource: \"{ResourceName}\"");
                }
                if (!string.IsNullOrWhiteSpace(ResourceInstanceName))
                {
                    sb.Append($" with resource identifier: \"{ResourceInstanceName}\"");
                }
                return sb.ToString();
            }
        }
    }
}
