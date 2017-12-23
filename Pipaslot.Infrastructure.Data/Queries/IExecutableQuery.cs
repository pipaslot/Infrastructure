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
        new IEnumerable<TResult> Execute();
        
        /// <summary>
        /// Asynchronously executes the query and returns the results.
        /// </summary>
        /// <param name="cancellationToken"></param>
        new Task<IEnumerable<TResult>> ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IExecutableQuery
    {
        /// <summary>
        /// Executes the query and returns the results.
        /// </summary>
        IEnumerable<object> Execute();

        /// <summary>
        /// Asynchronously executes the query and returns the results.
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task<IEnumerable<object>> ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
