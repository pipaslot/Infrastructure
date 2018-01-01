using System;
using System.Text;
using System.Threading.Tasks;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.Security
{
    public class AuthorizationException : UnauthorizedAccessException
    {
        /// <summary>
        /// Required permission
        /// </summary>
        public IConvertible Permission { get; }

        /// <summary>
        /// User friendly name for required permission
        /// </summary>
        public string PermissionName => Helpers.GetPermissonReadableName(Permission);

        /// <summary>
        /// Resource type
        /// </summary>
        public Type Resource { get; }

        /// <summary>
        /// User friendly resource name
        /// </summary>
        public string ResourceName => Helpers.GetResourceReadableName(Resource);

        /// <summary>
        /// Info about concrete object for whic his not the permission granted
        /// </summary>
        private readonly IResourceInstance _resourceDetail;

        /// <summary>
        /// User friendly resource instance name
        /// </summary>
        public string ResourceInstanceName =>  _resourceDetail?.ResourceName ?? "";

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

        public AuthorizationException(Type resource, IConvertible permissionEnum, IResourceInstance resourceDetail)
        {
            _resourceDetail = resourceDetail;
            Resource = resource;
            Permission = permissionEnum;
        }

        public override string Message
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append($"You do not have permission: '{PermissionName}'");
                if (Resource != null)
                {
                    sb.Append($" for resource: '{ResourceName}'");
                }
                if (!string.IsNullOrWhiteSpace(ResourceInstanceName))
                {
                    sb.Append($" with resource identifier: '{ResourceInstanceName}'");
                }
                return sb.ToString();
            }
        }
    }
}
