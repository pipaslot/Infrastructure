using System;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Demo.Api
{
    /// <summary>
    /// Permission management
    /// </summary>
    public class SecurityController : Infrastructure.Security.Mvc.SecurityController<int>
    {
        public SecurityController(IRoleStore roleStore, IPermissionManager<int> permissionManager, IUnitOfWorkFactory unitOfWorkFactory) : base(roleStore, permissionManager, unitOfWorkFactory)
        {
        }
    }
}
