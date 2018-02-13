using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Demo.Models
{
    public class AppUser : User<int>, IUser
    {
        public AppUser(IPermissionManager<int> permissionManager, IClaimsPrincipalProvider claimsPrincipalProvider, IResourceInstanceProvider resourceInstanceProvider, IRoleStore roleStore) : base(permissionManager, claimsPrincipalProvider, resourceInstanceProvider, roleStore)
        {
        }
    }
}
