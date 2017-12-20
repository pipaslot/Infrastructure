using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Data.Queries;

namespace Pipaslot.Infrastructure.Data.EntityFramework
{
    class EntityFrameworkRepositoryQuery<TDbContext, TResult> : EntityFrameworkQuery<TDbContext, TResult, TResult>, IQuery<TResult>
            where TDbContext : DbContext
            where TResult : class

        {
            public EntityFrameworkRepositoryQuery(IEntityFrameworkDbContextFactory dbContextFactory) : base(dbContextFactory)
            {
            }

            protected override IList<TResult> PostProcessResults(IList<TResult> results)
            {
                return results;
            }

            protected override IQueryable<TResult> GetQueryable()
            {
                throw new NotImplementedException();
            }
        }
}
