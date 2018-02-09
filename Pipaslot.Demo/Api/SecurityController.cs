using System;
using Microsoft.AspNetCore.Mvc;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.SecurityUI.Controllers;

namespace Pipaslot.Demo.Api
{
    [Area("Api")]
    [Route("api/[controller]")]
    public class SecurityController : SecurityController<int>
    {
        public SecurityController(IRoleStore roleStore, IPermissionManager<int> permissionManager, IUnitOfWorkFactory unitOfWorkFactory) : base(roleStore, permissionManager, unitOfWorkFactory)
        {
        }
    }
}
