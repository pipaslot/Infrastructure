using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.Data.EntityFramework
{
    public class EntityFrameworkSimpleQuery<TResult, TDbContext> : EntityFrameworkQuery<TResult, TDbContext>
        where TDbContext : DbContext
        where TResult : class
    {
        public EntityFrameworkSimpleQuery(IEntityFrameworkDbContextFactory dbContextFactory) : base(dbContextFactory)
        {
        }

        protected override IQueryable<TResult> GetQueryable()
        {
            return ContextReadOnly.Set<TResult>();
        }
    }
}
