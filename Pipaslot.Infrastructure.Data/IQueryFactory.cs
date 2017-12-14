using Pipaslot.Infrastructure.Data.Queries;

namespace Pipaslot.Infrastructure.Data
{
    public interface IQueryFactory<out TQuery>
        where TQuery : IExecutableQuery
    {
        TQuery Create();
    }
}
