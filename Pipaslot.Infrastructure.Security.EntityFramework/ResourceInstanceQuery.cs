using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Data.EntityFramework;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.Security.EntityFramework
{
    /// <inheritdoc />
    /// <summary>
    /// ResourceInstance query providing real time info about all per instance secured resources 
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public class ResourceInstanceQuery<TDbContext> : EntityFrameworkQuery<TDbContext, ResourceInstance>, IResourceInstanceQuery
        where TDbContext : DbContext
    {
        /// <summary>
        /// Queryable getters for all Resources implementing IResourceInstance interface
        /// </summary>
        public Dictionary<Type, Func<TDbContext, IQueryable<ResourceInstance>>> SupportedResources = new Dictionary<Type, Func<TDbContext, IQueryable<ResourceInstance>>>();

        public Type Resource { get; set; }

        public object ResourceIdentifier { get; set; }

        private readonly IEntityFrameworkDbContextFactory _dbContextFactory;

        public ResourceInstanceQuery(IEntityFrameworkDbContextFactory dbContextFactory) : base(dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        protected override IQueryable<ResourceInstance> GetQueryable()
        {
            if (SupportedResources.ContainsKey(Resource))
            {
                var action = SupportedResources[Resource];
                var context = _dbContextFactory.GetReadOnlyContext<TDbContext>();
                var query = action(context);
                if (ResourceIdentifier != null)
                {
                    query = query.Where(r => r.Id == ResourceIdentifier);
                }
                return query;
            }
            throw new ApplicationException($"Instance Resource for type {Resource} is nto implemented");
        }
    }
}
