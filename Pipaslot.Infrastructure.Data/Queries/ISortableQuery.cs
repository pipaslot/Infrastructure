using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Pipaslot.Infrastructure.Data.Queries
{
    /// <inheritdoc />
    /// <summary>
    /// Sort criteria for expected result and for query result
    /// </summary>
    /// <typeparam name="TQueryableResult"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface ISortableQuery<TQueryableResult, TResult> : ISortableQuery<TResult>
    {
        /// <summary>
        /// Gets a list of sort criteria applied on this query.
        /// </summary>
        IList<Func<IQueryable<TQueryableResult>, IOrderedQueryable<TQueryableResult>>> SortCriteria { get; }

        /// <summary>
        /// Adds a specified sort criteria to the query.
        /// </summary>
        void AddSortCriteria<TKey>(Expression<Func<TQueryableResult, TKey>> field, SortDirection direction = SortDirection.Ascending);

    }

    /// <summary>
    /// Sort criteria for expected result
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface ISortableQuery<TResult> : IExecutableQuery<TResult>
    {
        /// <summary>
        /// Adds a specified sort criteria to the query.
        /// </summary>
        void AddSortCriteria(string fieldName, SortDirection direction = SortDirection.Ascending);

        /// <summary>
        /// Resets the list of sort criteria.
        /// </summary>
        void ClearSortCriteria();
    }
}
