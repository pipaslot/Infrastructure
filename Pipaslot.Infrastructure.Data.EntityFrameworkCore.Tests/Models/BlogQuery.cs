using System.Linq;
using Pipaslot.Infrastructure.Data.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkCore.Tests.Models
{
    class BlogQuery : EntityFrameworkQuery<BloggingContext, Blog>
    {
        public BlogQuery(IEntityFrameworkDbContextFactory dbContextFactory) : base(dbContextFactory)
        {
        }

        protected override IQueryable<Blog> GetQueryable()
        {
            return ContextReadOnly.Set<Blog>();
        }
    }
}
