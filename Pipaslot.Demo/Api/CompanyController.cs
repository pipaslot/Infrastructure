using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pipaslot.Demo.Models.Entities;
using Pipaslot.Demo.Models.Permissions;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Data.Mvc;
using Pipaslot.Infrastructure.Security;

namespace Pipaslot.Demo.Api
{

    /// <summary>
    /// Company Controller
    /// </summary>
    [Area("Api")]
    [Route("api/[controller]")]
    public class CompanyController : RepositoryController<Company, int>
    {
        private readonly IUser<int> _user;

        /// <inheritdoc />
        public CompanyController(IUnitOfWorkFactory unitOfWorkFactory, IRepository<Company, int> repository, IUser<int> user) : base(unitOfWorkFactory, repository)
        {
            _user = user;
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<Company>> GetAll(int pageIndex, int pageSize = 10)
        {
            var ids = await _user.GetAllowedKeysAsync(typeof(Company), CompanyPermissions.View);
            return await base.GetByIds(ids);
        }

        /// <inheritdoc />
        public override async Task<Company> GetById(int id)
        {
            await _user.CheckPermissionAsync(typeof(Company), id, CompanyPermissions.View);
            return await base.GetById(id);
        }

        /// <inheritdoc />
        public override async Task<int> Create(Company entity)
        {
            await _user.CheckPermissionAsync(typeof(Company), CompanyStaticPermissions.Create);
            return await base.Create(entity);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> Update(int id, Company entity)
        {
            await _user.CheckPermissionAsync(typeof(Company), CompanyPermissions.Update);
            return await base.Update(id, entity);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> Delete(int id)
        {
            await _user.CheckPermissionAsync(typeof(Company), CompanyPermissions.Delete);
            return await base.Delete(id);
        }
    }
}
