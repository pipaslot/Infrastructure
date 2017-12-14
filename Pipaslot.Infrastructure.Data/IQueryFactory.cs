using Pipaslot.Infrastructure.Data.Queries;

namespace Pipaslot.Infrastructure.Data
{
    public interface IQueryFactory
    {
        IQuery<object> Create();
    }
}
