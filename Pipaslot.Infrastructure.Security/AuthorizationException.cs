using System;
using System.Text;
using System.Threading.Tasks;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.Security
{
    public class AuthorizationException : UnauthorizedAccessException
    {
        public IConvertible Permission { get; }
        public string PermissionName => Helpers.GetPermissonReadableName(Permission);
        private readonly Func<Task<ResourceInstance>> _resourceDetailGetter;
        public Type Resource { get; }
        public string ResourceName => Helpers.GetResourceReadableName(Resource);
        private string _resourceInstnanceName;

        public string ResourceInstanceName => _resourceInstnanceName ?? (_resourceInstnanceName = _resourceDetailGetter().Result?.Name ?? "");

        public AuthorizationException(string message) : base(message)
        {
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

        public AuthorizationException(Type resource, IConvertible permissionEnum, Func<Task<ResourceInstance>> resourceDetailGetter)
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
