using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkCoreTests.Models
{
    public class Blog : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Blog()
        {
        }

        public Blog(string name)
        {
            Name = name;
        }
    }
}
