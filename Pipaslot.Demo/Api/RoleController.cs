using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Data.Mvc;
using Pipaslot.Infrastructure.Security.EntityFrameworkCore.Entities;

namespace Pipaslot.Demo.Api
{

    /// <summary>
    /// Controller for demo data
    /// </summary>
    [Area("Api")]
    [Route("api/[controller]")]
    public class RoleController : RepositoryController<Role<int>, int>
    {
        /// <inheritdoc />
        public RoleController(IUnitOfWorkFactory unitOfWorkFactory, IRepository<Role<int>, int> repository) : base(unitOfWorkFactory, repository)
        {
        }
    }
}
