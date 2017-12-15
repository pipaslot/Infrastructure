using System;
using System.Linq;
using Pipaslot.Infrastructure.Data.EntityFramework;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkTests.Models
{
    public class BlogRepository : EntityFrameworkRepository<BloggingContext, Blog, int>
    {
        public BlogRepository(IUnitOfWorkFactory uowFactory, IEntityFrameworkDbContextFactory dbContextFactory) : base(uowFactory, dbContextFactory)
        {
        }

        public Blog GetByName(string name)
        {
            return ContextReadOnly.Blog
                .FirstOrDefault(b => name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
