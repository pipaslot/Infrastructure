using System.Threading;
using System.Threading.Tasks;

namespace Pipaslot.Infrastructure.Data.Queries
{
    public interface IPageableQuery<TResult> : IQuery<TResult>
    {
        /// <summary>
        /// Gets or sets a number of rows to be skipped. If this value is null, the paging will be applied.
        /// </summary>
        int? Skip { get; set; }

        /// <summary>
        /// Gets or sets the page size. If this value is null, the paging will not be applied.
        /// </summary>
        int? Take { get; set; }

        /// <summary>
        /// Setup Skip and Take parameters to met pagging requirements
        /// </summary>
        /// <param name="pageIndex">Must be greather than zero</param>
        /// <param name="pageSize">Must be greather than zero</param>
        /// <returns></returns>
        IPageableQuery<TResult> SetPage(int pageIndex, int pageSize = 10);

        /// <summary>
        /// Gets the total row count without respect to paging.
        /// </summary>
        int GetTotalRowCount();

        /// <summary>
        /// Gets the total row count without respect to paging.
        /// </summary>
        Task<int> GetTotalRowCountAsync();

        /// <summary>
        /// Gets the total row count without respect to paging.
        /// </summary>
        Task<int> GetTotalRowCountAsync(CancellationToken cancellationToken);
    }
}
