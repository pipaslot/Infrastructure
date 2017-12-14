using System;
using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Data.Queries;

namespace Pipaslot.Infrastructure.Data.EntityFramework
{
    public class EntityFrameworkQueryFactory<TQuery, TResult> : IQueryFactory<TQuery>
        where TQuery : IQuery<TResult>//TODO nahradit za IQuery interface
    {
        private readonly IEntityFrameworkDbContextFactory _dbContextFactory;

        public EntityFrameworkQueryFactory(IEntityFrameworkDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public TQuery Create()
        {
            return (TQuery)Activator.CreateInstance(typeof(TQuery), _dbContextFactory);
        }
    }
}
