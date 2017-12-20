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
    }

    public interface IQuery<TResult> : IQuery, IPageableQuery<TResult>, ISortableQuery<TResult, TResult>
    {
    }

    public interface IQuery : IExecutableQuery
    {
    }

}
