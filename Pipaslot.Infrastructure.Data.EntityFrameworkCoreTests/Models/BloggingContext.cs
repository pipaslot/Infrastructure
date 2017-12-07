using Microsoft.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkCoreTests.Models
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blog { get; set; }

        public BloggingContext()
        {
        }

        public BloggingContext(DbContextOptions<BloggingContext> options)
            : base(options)
        {
        }
    }
}