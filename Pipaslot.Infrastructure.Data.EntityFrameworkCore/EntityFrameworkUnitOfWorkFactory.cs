using Microsoft.EntityFrameworkCore;
using System;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkCore
{
    public class EntityFrameworkUnitOfWorkFactory<TDbContext> : AUnitOfWorkFactory<EntityFrameworkUnitOfWork<TDbContext>>
        where TDbContext : DbContext
    {
        private readonly IEntityFrameworkDbContextFactory<TDbContext> _dbContextFactory;

        public EntityFrameworkUnitOfWorkFactory(IEntityFrameworkDbContextFactory<TDbContext> dbContextFactory)
            : this(dbContextFactory, new UnitOfWorkRegistry())
        {

        }

        public EntityFrameworkUnitOfWorkFactory(IEntityFrameworkDbContextFactory<TDbContext> dbContextFactory, IUnitOfWorkRegistry registry) : base(registry)
        {
            _dbContextFactory = dbContextFactory;
        }

        protected override EntityFrameworkUnitOfWork<TDbContext> CreateUnitOfWork()
        {
            var currentUoW = GetCurrent(false);
            if (currentUoW != null)
            {

                //Reuse dbContext and put flag to do not commit changes
                return new EntityFrameworkUnitOfWork<TDbContext>(currentUoW.Context);
            }
            //Return unit of work with new context
            var newContext = _dbContextFactory.Create();
            var transaction = newContext.Database.BeginTransaction();
            return new EntityFrameworkUnitOfWork<TDbContext>(newContext, transaction);
        }

        /// <summary>
        /// Create Read only context for Queries
        /// </summary>
        /// <returns></returns>
        public TDbContext GetReadOnlyContext()
        {
            var uow = GetCurrent(false);
            if (uow != null)
            {
                return uow.Context;
            }
            var context = _dbContextFactory.Create();
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            return context;
        }
    }
}
