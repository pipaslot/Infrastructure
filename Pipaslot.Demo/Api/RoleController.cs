using Microsoft.AspNetCore.Mvc;
using Pipaslot.Demo.Models.Entities;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Data.Mvc;

namespace Pipaslot.Demo.Api
{

    /// <summary>
    /// Controller for demo data
    /// </summary>
    [Area("Api")]
    [Route("api/[controller]")]
    public class RoleController : RepositoryController<Role, int>
    {
        /// <inheritdoc />
        public RoleController(IUnitOfWorkFactory unitOfWorkFactory, IRepository<Role, int> repository) : base(unitOfWorkFactory, repository)
        {
        }
    }
}
