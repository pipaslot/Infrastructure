using System;
using Microsoft.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.Data.EntityFramework
{
    public class EntityFrameworkQueryFactory<TDbContext, TQuery, TResult> : IQueryFactory<TQuery>
        where TQuery : EntityFrameworkQuery<TResult, TDbContext>//TODO nahradit za IQuery interface
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
