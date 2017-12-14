using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Pipaslot.Infrastructure.Data.Queries
{
    public abstract class Query<TResult> : Query<TResult, TResult>, IQuery<TResult>
    {
        /// <inheritdoc />
        /// <summary>
        /// When overriden in derived class, it allows to modify the materialized results of the query before they are returned
        /// to the caller.
        /// </summary>
        protected override IList<TResult> PostProcessResults(IList<TResult> results)
        {
            return results;
        }
    }

    public abstract class Query<TQueryableResult, TResult> : IQuery<TQueryableResult, TResult>
    {
        #region IPageableQuery implementation
        
        public int? Skip { get; set; }
        
        public int? Take { get; set; }


        public IPageableQuery<TResult> SetPage(int pageIndex, int pageSize = 10)
        {
            if (pageIndex <= 0) throw new ArgumentOutOfRangeException(nameof(pageIndex), "Must be greater than zero");
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize), "Must be greater than zero");
            Skip = (pageIndex - 1) * pageSize;
            Take = pageSize;
            return this;
        }
        
        public virtual int GetTotalRowCount()
        {
            var queryable = GetQueryable();
            return queryable.Count();
        }
        
        public virtual Task<int> GetTotalRowCountAsync()
        {
            return GetTotalRowCountAsync(CancellationToken.None);
        }
        
        public abstract Task<int> GetTotalRowCountAsync(CancellationToken cancellationToken);

        #endregion

        #region ISortableQuery implementation

        public IList<Func<IQueryable<TQueryableResult>, IOrderedQueryable<TQueryableResult>>> SortCriteria { get; } = new List<Func<IQueryable<TQueryableResult>, IOrderedQueryable<TQueryableResult>>>();

        public void AddSortCriteria(string fieldName, SortDirection direction = SortDirection.Ascending)
        {
            // create the expression
            var prop = typeof(TQueryableResult).GetTypeInfo().GetProperty(fieldName,BindingFlags.IgnoreCase |  BindingFlags.Public | BindingFlags.Instance);
            var param = Expression.Parameter(typeof(TQueryableResult), "i");
            var expr = Expression.Lambda(Expression.Property(param, prop), param);

            // call the method
            typeof(Query<TQueryableResult, TResult>).GetTypeInfo().GetMethod(nameof(AddSortCriteriaCore),
                    BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(prop.PropertyType)
                .Invoke(this, new object[] { expr, direction });
        }

        public void AddSortCriteria<TKey>(Expression<Func<TQueryableResult, TKey>> field, SortDirection direction = SortDirection.Ascending)
        {
            AddSortCriteriaCore(field, direction);
        }


        public void ClearSortCriteria()
        {
            SortCriteria.Clear();
        }

        private void AddSortCriteriaCore<TKey>(Expression<Func<TQueryableResult, TKey>> sortExpression, SortDirection direction)
        {
            if (direction == SortDirection.Ascending)
                SortCriteria.Add(x => x.OrderBy(sortExpression));
            else
                SortCriteria.Add(x => x.OrderByDescending(sortExpression));
        }
        
        #endregion

        #region ExecutableQuery Implementation
        
        public virtual IList<TResult> Execute()
        {
            var query = PreProcessQuery();
            var queryResults = query.ToList();
            var results = PostProcessResults(queryResults);
            return results;
        }

        async Task<IList<object>> IExecutableQuery.ExecuteAsync()
        {
            var list = await ExecuteAsync();
            return (IList<object>)list;
        }

        async Task<IList<object>> IExecutableQuery.ExecuteAsync(CancellationToken cancellationToken)
        {
            var list = await ExecuteAsync(cancellationToken);
            return (IList<object>)list;
        }

        IList<object> IExecutableQuery.Execute()
        {
            var list = Execute();
            return (IList<object>)list;
        }

        /// <summary>
        /// Asynchronously executes the query and returns the results.
        /// </summary>
        public virtual async Task<IList<TResult>> ExecuteAsync()
        {
            return await ExecuteAsync(default(CancellationToken));
        }

        /// <summary>
        ///     Asynchronously executes the query and returns the results.
        /// </summary>
        public virtual async Task<IList<TResult>> ExecuteAsync(CancellationToken cancellationToken)
        {
            var query = PreProcessQuery();
            var queryResults = await ExecuteQueryAsync(query, cancellationToken);
            var results = PostProcessResults(queryResults);
            return results;
        }

        protected abstract Task<IList<TQueryableResult>> ExecuteQueryAsync(IQueryable<TQueryableResult> query, CancellationToken cancellationToken);

        /// <summary>
        ///     When overriden in derived class, it allows to modify the materialized results of the query before they are returned
        ///     to the caller.
        /// </summary>
        protected abstract IList<TResult> PostProcessResults(IList<TQueryableResult> results);

        protected abstract IQueryable<TQueryableResult> GetQueryable();

        #endregion

        private IQueryable<TQueryableResult> PreProcessQuery()
        {
            var query = GetQueryable();

            for (var i = SortCriteria.Count - 1; i >= 0; i--)
            {
                query = SortCriteria[i](query);
            }

            if (Skip != null)
            {
                query = query.Skip(Skip.Value);
            }

            if (Take != null)
            {
                query = query.Take(Take.Value);
            }
            return query;
        }
    }
}
