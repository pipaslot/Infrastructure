using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            var query = _repository.CreateQuery();
            return await query.ExecuteAsync();
        }

        /// <summary>
        /// Get all with applied paging
        /// </summary>
        /// <returns></returns>
        [HttpGet("paged/{pageIndex}/{pageSize}")]
        public virtual async Task<IEnumerable<TEntity>> GetAll(int pageIndex, int pageSize = 10)
        {
            var query = _repository.CreateQuery();
            query.SetPage(pageIndex, pageSize);
            return await query.ExecuteAsync();
        }

        /// <summary>
        /// Get entity by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public virtual async Task<TEntity> GetById(TKey id)
        {
            return await _repository.GetByIdAsync(id);
        }

        /// <summary>
        /// Create entity
        /// </summary>
        [HttpPost]
        public virtual async Task<TKey> Create([FromBody]TEntity entity)
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                entity.Id = default(TKey);
                _repository.Insert(entity);
                await uow.CommitAsync();
                return entity.Id;
            }
        }

        /// <summary>
        /// Update entity. Ignores Id from received object.
        /// </summary>
        [HttpPost("{id}")]
        public virtual async Task<IActionResult> Update(TKey id, [FromBody]TEntity entity)
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                entity.Id = id;
                _repository.Update(entity);
                await uow.CommitAsync();
            }
            return Ok(true);
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(TKey id)
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                _repository.Delete(id);
                await uow.CommitAsync();
            }
            return Ok(true);
        }
    }
}
