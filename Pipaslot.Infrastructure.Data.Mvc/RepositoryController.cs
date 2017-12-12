using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Pipaslot.Infrastructure.Data.Queries;

namespace Pipaslot.Infrastructure.Data.Mvc
{
    public abstract class RepositoryController<TEntity, TKey> : Controller
        where TEntity : IEntity<TKey>
    {
        private readonly IRepository<TEntity, TKey> _repository;
        private readonly Func<IQuery<TEntity>> _queryFactory;

        protected RepositoryController(IRepository<TEntity, TKey> repository, Func<IQuery<TEntity>> queryFactory)
        {
            _repository = repository;
            _queryFactory = queryFactory;
        }

        /// <summary>
        /// Get all 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IList<TEntity> GetAll()
        {
            var query = _queryFactory();
            return query.Execute();
        }

        /// <summary>
        /// Get all with applied paging
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IList<TEntity> GetAll(int pageIndex, int pageSize = 10)
        {
            var query = _queryFactory();
            query.SetPage(pageIndex, pageSize);
            return query.Execute();
        }

        /// <summary>
        /// Get entity by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public TEntity GetById(TKey id)
        {
            return _repository.GetById(id);
        }

        /// <summary>
        /// Create entity
        /// </summary>
        [HttpPut]
        public void Create(TEntity entity)
        {
            entity.Id = default(TKey);
            _repository.Insert(entity);
        }

        /// <summary>
        /// Update entity
        /// </summary>
        [HttpPost]
        public void Update(TEntity entity)
        {
            _repository.Update(entity);
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete]
        public void Delete(TKey id)
        {
            _repository.Delete(id);
        }
    }
}
