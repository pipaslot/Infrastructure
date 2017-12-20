using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Data.EntityFramework;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.Infrastructure.Security.EntityFramework.Entities;

namespace Pipaslot.Infrastructure.Security.EntityFramework
{
    public class RoleQuery<TDbContext, TKey> : EntityFrameworkQuery<TDbContext, IRole>, IRoleQuery
        where TDbContext : DbContext, ISecurityDbContext<TKey>
    {
        private readonly IEntityFrameworkDbContextFactory _dbContextFactory;

        public RoleQuery(IEntityFrameworkDbContextFactory dbContextFactory) : base(dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        protected override IQueryable<IRole> GetQueryable()
        {
            return _dbContextFactory.GetReadOnlyContext<TDbContext>().Set<Role<TKey>>();
        }
    }
}
