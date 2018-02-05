using System;
using Microsoft.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkCore
{
    public class EntityFrameworkUnitOfWorkFactory<TDbContext> : AUnitOfWorkFactory<IEntityFrameworkUnitOfWork<TDbContext>>
         where TDbContext : DbContext
    {
        private readonly IEntityFrameworkDbContextFactory _dbContextFactory;

        public EntityFrameworkUnitOfWorkFactory(IEntityFrameworkDbContextFactory dbContextFactory, IUnitOfWorkRegistry registry) : base(registry)
        {
            _dbContextFactory = dbContextFactory;
        }

        protected override IEntityFrameworkUnitOfWork<TDbContext> CreateUnitOfWork()
        {
            var currentUoW = GetCurrent();
            if (currentUoW != null)
            {

                //Reuse dbContext and put flag to do not commit changes
                return new EntityFrameworkUnitOfWork<TDbContext>(currentUoW.Context);
            }
            //Return unit of work with new context
            var newContext = _dbContextFactory.Create<TDbContext>();
            var transaction = newContext.Database.BeginTransaction();
            return new EntityFrameworkUnitOfWork<TDbContext>(newContext, transaction);
        }

    }
}
