using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Data.EntityFrameworkCore;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.Security.EntityFrameworkCore
{
    public class ResourceInstanceProvider<TDbContext> : IResourceInstanceProvider
        where TDbContext : DbContext
    {
        private readonly IEntityFrameworkDbContextFactory _dbContextFactory;

        public ResourceInstanceProvider(IEntityFrameworkDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public Task<int> GetInstanceCountAsync(Type resource, CancellationToken token = default(CancellationToken))
        {
            if (typeof(IResourceInstance).IsAssignableFrom(resource))
            {
                var method = GetType().GetMethod(nameof(GetInstanceCountCore));
                var genericMethod = method.MakeGenericMethod(resource);
                return (Task<int>)genericMethod.Invoke(this, new object[] { token });
            }
            return Task.FromResult(0);
        }

        public Task<int> GetInstanceCountCore<TResource>(CancellationToken token = default(CancellationToken))
            where TResource : class, IResourceInstance
        {
            var queryable = GetQuery<TResource>();
            return queryable.CountAsync(token);
        }

        public Task<List<object>> GetAllIdsAsync(Type resource, CancellationToken token = default(CancellationToken))
        {
            var method = GetType().GetMethod(nameof(GetAllIdsCore));
            var genericMethod = method.MakeGenericMethod(resource);
            return (Task<List<object>>)genericMethod.Invoke(this, new object[] { token });
        }

        public Task<List<object>> GetAllIdsCore<TResource>(CancellationToken token = default(CancellationToken))
            where TResource : class, IResourceInstance
        {
            var queryable = GetQuery<TResource>();
            return queryable.Select(r => r.ResourceUniqueIdentifier).ToListAsync(token);
        }

        public Task<IResourceInstance> GetInstanceAsync(Type resource, object id, CancellationToken token = default(CancellationToken))
        {
            var method = GetType().GetMethod(nameof(GetInstanceCore));
            var genericMethod = method.MakeGenericMethod(resource);
            return (Task<IResourceInstance>)genericMethod.Invoke(this, new object[] { id, token });
        }

        public async Task<IResourceInstance> GetInstanceCore<TResource>(object id, CancellationToken token = default(CancellationToken))
            where TResource : class, IResourceInstance
        {
            var queryable = GetQuery<TResource>();
            var result = await queryable.FirstOrDefaultAsync(r => r.ResourceUniqueIdentifier.Equals(id), token);
            return result;
        }

        public Task<List<IResourceInstance>> GetInstancesAsync(Type resource, int pageIndex = 1, int pageSize = 20,
            CancellationToken token = default(CancellationToken))
        {
            if (typeof(IResourceInstance).IsAssignableFrom(resource))
            {
                var method = GetType().GetMethod(nameof(GetInstancesCore));
                var genericMethod = method.MakeGenericMethod(resource);
                return (Task<List<IResourceInstance>>)genericMethod.Invoke(this, new object[] { pageIndex, pageSize, token });
            }
            return Task.FromResult(new List<IResourceInstance>());
        }

        public async Task<List<IResourceInstance>> GetInstancesCore<TResource>(int pageIndex = 1, int pageSize = 20,
            CancellationToken token = default(CancellationToken))
            where TResource : class, IResourceInstance
        {
            var queryable = GetQuery<TResource>();
            var result = await queryable.ToListAsync(token);
            return result.Select(r => (IResourceInstance)r).ToList();
        }

        private DbSet<TResult> GetQuery<TResult>() where TResult : class, IResourceInstance
        {
            var context = _dbContextFactory.GetReadOnlyContext<TDbContext>();
            return context.Set<TResult>();
        }

    }
}
