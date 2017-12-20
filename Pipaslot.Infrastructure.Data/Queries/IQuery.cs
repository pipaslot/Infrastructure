using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pipaslot.Infrastructure.Data.Queries
{
    /// <summary>
    /// Query with post-process result converting database entity from query into custom data object
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TQueryableResult"></typeparam>
    public interface IQuery<TQueryableResult, TResult> : IQuery, IPageableQuery<TResult>, ISortableQuery<TQueryableResult, TResult>
    {
        /// <summary>
        /// Executes the query and returns the results.
        /// </summary>
        new IList<TResult> Execute();

        /// <summary>
        /// Asynchronously executes the query and returns the results.
        /// </summary>
        /// <param name="cancellationToken"></param>
        new Task<IList<TResult>> ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IQuery<TResult> : IQuery, IPageableQuery<TResult>, ISortableQuery<TResult, TResult>
    {
        /// <summary>
        /// Executes the query and returns the results.
        /// </summary>
        new IList<TResult> Execute();
        
        /// <summary>
        /// Asynchronously executes the query and returns the results.
        /// </summary>
        /// <param name="cancellationToken"></param>
        new Task<IList<TResult>> ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IQuery : IExecutableQuery
    {
    }

}
