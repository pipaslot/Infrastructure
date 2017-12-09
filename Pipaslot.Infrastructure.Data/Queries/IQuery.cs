using System;
using System.Collections.Generic;
using System.Text;

namespace Pipaslot.Infrastructure.Data.Queries
{
    public interface IQuery<TResult> : IPageableQuery<TResult>, ISortableQuery<TResult>
    {
    }

    /// <summary>
    /// Query with post-process result converting database entity from query into custom data object
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IQuery<TQueryableResult, TResult> : IPageableQuery<TResult>, ISortableQuery<TQueryableResult, TResult>
    {
    }
}
