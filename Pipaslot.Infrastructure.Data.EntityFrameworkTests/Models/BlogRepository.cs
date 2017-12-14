using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pipaslot.Infrastructure.Data.EntityFramework;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkTests.Models
{
    public class BlogRepository : EntityFrameworkRepository<Blog,int,BloggingContext>
    {
        public BlogRepository(EntityFrameworkUnitOfWorkFactory<BloggingContext> uowFactory, IEntityFrameworkDbContextFactory<BloggingContext> dbContextFactory) : base(uowFactory, dbContextFactory)
        {
        }

        public Blog GetByName(string name)
        {
            return ContextReadOnly.Blog
                .FirstOrDefault(b => name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
