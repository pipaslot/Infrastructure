using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Pipaslot.Infrastructure.Data.Mvc
{
    public abstract class RepositoryController<TEntity, TKey> : Controller
        where TEntity : IEntity<TKey>
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IRepository<TEntity, TKey> _repository;

        protected RepositoryController(IUnitOfWorkFactory unitOfWorkFactory, IRepository<TEntity, TKey> repository)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _repository = repository;
        }

        /// <summary>
        /// Get all 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public virtual IEnumerable<TEntity> GetAll()
        {
            var query = _repository.CreateQuery();
            return query.Execute();
        }

        /// <summary>
        /// Get all with applied paging
        /// </summary>
        /// <returns></returns>
        [HttpGet("paged/{pageIndex}/{pageSize}")]
        public virtual IEnumerable<TEntity> GetAll(int pageIndex, int pageSize = 10)
        {
            var query = _repository.CreateQuery();
            query.SetPage(pageIndex, pageSize);
            return query.Execute();
        }

        /// <summary>
        /// Get entity by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public virtual TEntity GetById(TKey id)
        {
            return _repository.GetById(id);
        }

        /// <summary>
        /// Create entity
        /// </summary>
        [HttpPost]
        public virtual TKey Create([FromBody]TEntity entity)
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                entity.Id = default(TKey);
                _repository.Insert(entity);
                uow.Commit();
                return entity.Id;
            }
        }

        /// <summary>
        /// Update entity. Ignores Id from received object.
        /// </summary>
        [HttpPost("{id}")]
        public virtual IActionResult Update(TKey id, [FromBody]TEntity entity)
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                entity.Id = id;
                _repository.Update(entity);
                uow.Commit();
            }
            return Ok(true);
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public virtual IActionResult Delete(TKey id)
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                _repository.Delete(id);
                uow.Commit();
            }
            return Ok(true);
        }
    }
}
