using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pipaslot.Infrastructure.Data.Queries
{
    public interface IExecutableQuery<TResult>
    {
        /// <summary>
        /// Executes the query and returns the results.
        /// </summary>
        IList<TResult> Execute();

        /// <summary>
        /// Asynchronously executes the query and returns the results.
        /// </summary>
        Task<IList<TResult>> ExecuteAsync();

        /// <summary>
        /// Asynchronously executes the query and returns the results.
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task<IList<TResult>> ExecuteAsync(CancellationToken cancellationToken);
    }
}
