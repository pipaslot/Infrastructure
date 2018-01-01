using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pipaslot.Infrastructure.Security.Data
{
    public class NullResourceInstanceProvider : IResourceInstanceProvider
    {
        public Task<int> GetInstanceCountAsync(Type resource, CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult(0);
        }

        public Task<List<object>> GetAllIdsAsync(Type resource, CancellationToken token = default(CancellationToken))
        {
            var result = new List<object>();
            return Task.FromResult(result);
        }

        public Task<IResourceInstance> GetInstanceAsync(Type resource, object id, CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult((IResourceInstance)null);
        }

        public Task<List<IResourceInstance>> GetInstancesAsync(Type resource, int pageIndex = 1, int pageSize = 20,
            CancellationToken token = default(CancellationToken))
        {
            var result = new List<IResourceInstance>();
            return Task.FromResult(result);
        }
    }
}
