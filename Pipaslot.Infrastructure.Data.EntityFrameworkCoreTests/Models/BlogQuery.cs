using System;
using System.Linq;
using System.Linq.Expressions;
using Pipaslot.Infrastructure.Data.EntityFrameworkCore;
using Pipaslot.Infrastructure.Data.Queries;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkCoreTests.Models
{
    class BlogQuery : EntityFrameworkQuery<Blog,BloggingContext>
    {
        public BlogQuery(EntityFrameworkUnitOfWorkFactory<BloggingContext> uowFactory) : base(uowFactory)
        {
        }
        
        protected override IQueryable<Blog> GetQueryable()
        {
            return ContextReadOnly.Set<Blog>();
        }
    }
}
