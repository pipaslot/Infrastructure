﻿using Microsoft.AspNetCore.Mvc;
using Pipaslot.Demo.Models.Entities;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Data.Mvc;
using Pipaslot.Infrastructure.Data.Queries;

namespace Pipaslot.Demo.Api
{

    /// <summary>
    /// Controller for demo data
    /// </summary>
    [Area("Api")]
    [Route("api/[controller]")]
    public class SimpleRecordController : RepositoryController<SimpleRecord, int>
    {
        /// <inheritdoc />
        public SimpleRecordController(IUnitOfWorkFactory unitOfWorkFactory, IRepository<SimpleRecord, int> repository, IQueryFactory<IQuery<SimpleRecord>> queryFactory) : base(unitOfWorkFactory, repository, queryFactory)
        {
        }
    }
}