using System;
using Pipaslot.Infrastructure.Data.Queries;

namespace Pipaslot.Infrastructure.Data.EntityFramework
{
    public class EntityFrameworkQueryFactory<TQuery> : IQueryFactory<TQuery>
        where TQuery : IQuery
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
