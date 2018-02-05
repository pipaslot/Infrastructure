using System.Collections.Generic;
using System.Linq;
using Pipaslot.Infrastructure.Data.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkCore.Tests.Models
{
    public class BlogWithRemappingQuery : EntityFrameworkQuery<BloggingContext, Blog, BlogIdName>
    {
        public BlogWithRemappingQuery(IEntityFrameworkDbContextFactory dbContextFactory) : base(dbContextFactory)
        {
        }

        protected override IEnumerable<BlogIdName> PostProcessResults(IEnumerable<Blog> results)
        {
            return results.Select(b => new BlogIdName(b.Id, b.Name)).ToList();
        }

        protected override IQueryable<Blog> GetQueryable()
        {
            return ContextReadOnly.Set<Blog>();
        }
    }

    public class BlogIdName
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string IdName { get; set; }

        public BlogIdName(int id, string name)
        {
            ID = id;
            Name = name;
            IdName = JoinIdAndName(id, name);
        }

        public static string JoinIdAndName(int id, string name)
        {
            return id + name;
        }
    }
}
