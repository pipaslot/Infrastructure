using System.Linq;
using Pipaslot.Infrastructure.Data.EntityFramework;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkTests.Models
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
