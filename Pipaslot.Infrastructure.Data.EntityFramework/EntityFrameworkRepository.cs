using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Data.Queries;

namespace Pipaslot.Infrastructure.Data.EntityFramework
{
    public class EntityFrameworkRepository<TDbContext, TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>, new()
        where TDbContext : DbContext
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IEntityFrameworkDbContextFactory _dbContextFactory;

        public EntityFrameworkRepository(IUnitOfWorkFactory uowFactory, IEntityFrameworkDbContextFactory dbContextFactory)
        {
            _uowFactory = uowFactory;
            _dbContextFactory = dbContextFactory;
        }

        /// <summary>
        /// Context fore Read-Write operations
        /// </summary>
        protected TDbContext Context => _uowFactory.GetDbContext<TDbContext>();

        /// <summary>
        /// Context for Read only operations. Unit of work is not needed for this operation. If Unit of work does not exists, then is created a new context
        /// </summary>
        protected TDbContext ContextReadOnly => _uowFactory.GetDbContext<TDbContext>(false) ?? _dbContextFactory.GetReadOnlyContext<TDbContext>();
        
        public TEntity GetById(TKey id, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetByIds(new[] { id }, includes).FirstOrDefault();
        }
        
        public Task<TEntity> GetByIdAsync(TKey id, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetByIdAsync(default(CancellationToken), id, includes);
        }
        
        public async Task<TEntity> GetByIdAsync(CancellationToken cancellationToken, TKey id, params Expression<Func<TEntity, object>>[] includes)
        {
            var items = await GetByIdsAsync(cancellationToken, new[] { id }, includes);
            return items.FirstOrDefault();
        }

        /// <inheritdoc />
        /// <remarks> 
        /// This method is not suitable for large amounts of entities - the reasonable limit of number of IDs is 30. 
        /// </remarks>
        public virtual IList<TEntity> GetByIds(IEnumerable<TKey> ids, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetByIdsCore(ids, includes).ToList();
        }

        /// <inheritdoc />
        /// <remarks>
        /// This method is not suitable for large amounts of entities - the reasonable limit of number of IDs is 30.
        /// </remarks>
        public Task<IList<TEntity>> GetByIdsAsync(IEnumerable<TKey> ids, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetByIdsAsync(default(CancellationToken), ids, includes);
        }

        /// <inheritdoc />
        /// <remarks>
        /// This method is not suitable for large amounts of entities - the reasonable limit of number of IDs is 30.
        /// </remarks>
        public async Task<IList<TEntity>> GetByIdsAsync(CancellationToken cancellationToken, IEnumerable<TKey> ids, params Expression<Func<TEntity, object>>[] includes)
        {
            return await GetByIdsCore(ids, includes).ToListAsync(cancellationToken);
        }
        
        private IQueryable<TEntity> GetByIdsCore(IEnumerable<TKey> ids, Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = ContextReadOnly.Set<TEntity>();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query.Where(i => ids.Contains(i.Id));
        }

        public virtual TEntity InitializeNew()
        {
            return new TEntity();
        }
        
        public virtual void Insert(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
        }
        
        public void Insert(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities.ToList())
            {
                Insert(entity);
            }
        }
        
        public virtual void Update(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }
        
        public void Update(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities.ToList())
            {
                Update(entity);
            }
        }

        public IQuery<TEntity> CreateQuery()
        {
            return new EntityFrameworkQuery<TDbContext,TEntity>(_dbContextFactory);
        }

        public virtual void Delete(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
        }
        
        public void Delete(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities.ToList())
            {
                Delete(entity);
            }
        }
        
        public virtual void Delete(TKey id)
        {
            // try to get entity from the context
            var entity = Context.Set<TEntity>().Local.SingleOrDefault(e => e.Id.Equals(id));

            // if entity is not found in the context, create fake entity and attach it
            if (entity == null)
            {
                entity = new TEntity { Id = id };
                Context.Set<TEntity>().Attach(entity);
            }

            Delete(entity);
        }

        public void Delete(IEnumerable<TKey> ids)
        {
            foreach (var id in ids)
            {
                Delete(id);
            }
        }
    }
}
