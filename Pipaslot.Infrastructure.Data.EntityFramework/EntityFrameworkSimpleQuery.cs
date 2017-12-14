using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.Data.EntityFramework
{
    public class EntityFrameworkSimpleQuery<TKey, TResult, TDbContext> : EntityFrameworkQuery<TResult, TDbContext>
        where TDbContext : DbContext
        where TResult : class, IEntity<TKey>
    {
        public EntityFrameworkSimpleQuery(IEntityFrameworkDbContextFactory<TDbContext> dbContextFactory) : base(dbContextFactory)
        {
        }

        protected override IQueryable<TResult> GetQueryable()
        {
            return ContextReadOnly.Set<TResult>();
        }
    }
}
