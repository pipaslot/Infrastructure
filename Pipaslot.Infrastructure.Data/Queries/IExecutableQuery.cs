using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pipaslot.Infrastructure.Data.Queries
{
    public interface IExecutableQuery<TResult> : IExecutableQuery
    {
        /// <summary>
        /// Executes the query and returns the results.
        /// </summary>
        new IList<TResult> Execute();

        /// <summary>
        /// Asynchronously executes the query and returns the results.
        /// </summary>
        new Task<IList<TResult>> ExecuteAsync();

        /// <summary>
        /// Asynchronously executes the query and returns the results.
        /// </summary>
        /// <param name="cancellationToken"></param>
        new Task<IList<TResult>> ExecuteAsync(CancellationToken cancellationToken);
    }

    public interface IExecutableQuery
    {
        /// <summary>
        /// Executes the query and returns the results.
        /// </summary>
        IList<object> Execute();

        /// <summary>
        /// Asynchronously executes the query and returns the results.
        /// </summary>
        Task<IList<object>> ExecuteAsync();

        /// <summary>
        /// Asynchronously executes the query and returns the results.
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task<IList<object>> ExecuteAsync(CancellationToken cancellationToken);
    }
}
