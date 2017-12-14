using System;
using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Data.Queries;

namespace Pipaslot.Infrastructure.Data.EntityFramework
{
    public class EntityFrameworkQueryFactory<TDbContext, TQuery, TResult> : IQueryFactory<TQuery>
        where TQuery : IQuery<TResult>, IExecutableQuery
        where TDbContext : DbContext
    {
        private readonly IEntityFrameworkDbContextFactory<TDbContext> _dbContextFactory;

        public EntityFrameworkQueryFactory(IEntityFrameworkDbContextFactory<TDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public TQuery Create()
        {
            return (TQuery)Activator.CreateInstance(typeof(TQuery), _dbContextFactory);
        }
    }
}
