using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pipaslot.Demo.Models.Entities;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Data.Mvc;

namespace Pipaslot.Demo.Api
{

    /// <summary>
    /// Company Controller
    /// </summary>
    [Area("Api")]
    [Route("api/[controller]")]
    public class CompanyController : RepositoryController<Company, int>
    {
        /// <inheritdoc />
        public CompanyController(IUnitOfWorkFactory unitOfWorkFactory, IRepository<Company, int> repository) : base(unitOfWorkFactory, repository)
        {
        }
    }
}
