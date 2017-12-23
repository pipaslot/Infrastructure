using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pipaslot.Infrastructure.Data.Queries;

namespace Pipaslot.Infrastructure.Security.Data
{
    public class NullResourceInstanceQuery : AQuery<ResourceInstance>, IResourceInstanceQuery
    {
        public override Task<int> GetTotalRowCountAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        protected override Task<IEnumerable<ResourceInstance>> ExecuteQueryAsync(IQueryable<ResourceInstance> query, CancellationToken cancellationToken)
        {
            var result = new List<ResourceInstance>();
            return Task.FromResult((IEnumerable<ResourceInstance>)result);
        }

        protected override IQueryable<ResourceInstance> GetQueryable()
        {
            return new List<ResourceInstance>().AsQueryable();
        }

        public Type Resource { get; set; }

        public object ResourceIdentifier { get; set; }
    }
}
