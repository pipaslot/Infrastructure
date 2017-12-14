using System;
using Microsoft.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.Data.EntityFramework
{
    public class EntityFrameworkUnitOfWorkFactory : EntityFrameworkUnitOfWorkFactory<DbContext>, IEntityFrameworkUnitOfWorkFactory
    {
        public EntityFrameworkUnitOfWorkFactory(IEntityFrameworkDbContextFactory dbContextFactory, IUnitOfWorkRegistry registry) : base(dbContextFactory, registry)
        {
        }
        
        IEntityFrameworkUnitOfWork IEntityFrameworkUnitOfWorkFactory.Create()
        {
            return base.Create();
        }

        IEntityFrameworkUnitOfWork IEntityFrameworkUnitOfWorkFactory.GetCurrent(int index = 0)
        {
            return base.GetCurrent(index);
        }
    }

    public class EntityFrameworkUnitOfWorkFactory<TDbContext> : AUnitOfWorkFactory<IEntityFrameworkUnitOfWork<TDbContext>>, IEntityFrameworkUnitOfWorkFactory<TDbContext>
        where TDbContext : DbContext
    {
        private readonly IEntityFrameworkDbContextFactory<TDbContext> _dbContextFactory;

        public EntityFrameworkUnitOfWorkFactory(IEntityFrameworkDbContextFactory<TDbContext> dbContextFactory, IUnitOfWorkRegistry registry) : base(registry)
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
            var newContext = _dbContextFactory.Create();
            var transaction = newContext.Database.BeginTransaction();
            return new EntityFrameworkUnitOfWork<TDbContext>((TDbContext)newContext, transaction);
        }

    }
}
