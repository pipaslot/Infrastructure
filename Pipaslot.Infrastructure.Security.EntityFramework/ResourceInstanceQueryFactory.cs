using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Data.EntityFramework;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.Security.EntityFramework
{
    public class ResourceInstanceQueryFactory<TDbontext> : IQueryFactory<IResourceInstanceQuery>
        where TDbontext : DbContext
    {
        private readonly IEntityFrameworkDbContextFactory _dbContextFactory;
        private readonly Dictionary<Type, Func<TDbontext, IQueryable<ResourceInstance>>> _supportedResources = new Dictionary<Type, Func<TDbontext, IQueryable<ResourceInstance>>>();

        public ResourceInstanceQueryFactory(IEntityFrameworkDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public ResourceInstanceQueryFactory<TDbontext> AddResource(Type resource, Func<TDbontext, IQueryable<ResourceInstance>> getter)
        {
            _supportedResources.Add(resource, getter);
            return this;
        }

        public IResourceInstanceQuery Create()
        {
            return new ResourceInstanceQuery<TDbontext>(_dbContextFactory)
            {
                SupportedResources = _supportedResources
            };
        }
    }
}
