using System.Collections.Generic;
using System.Linq;
using Pipaslot.Infrastructure.Data.EntityFramework;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkTests.Models
{
    public class BlogWithRemappingQuery : EntityFrameworkQuery<Blog, BlogIdName, BloggingContext>
    {
        public BlogWithRemappingQuery(IEntityFrameworkDbContextFactory dbContextFactory) : base(dbContextFactory)
        {
        }

        protected override IList<BlogIdName> PostProcessResults(IList<Blog> results)
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
