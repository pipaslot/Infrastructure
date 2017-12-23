using Pipaslot.Infrastructure.Data;

namespace Pipaslot.Infrastructure.Security.Data
{
    public class NullResourceInstanceQueryFactory : IQueryFactory<IResourceInstanceQuery>
    {
        public IResourceInstanceQuery Create()
        {
            return new NullResourceInstanceQuery();
        }
    }
}
