using Pipaslot.Infrastructure.Security;

namespace Pipaslot.Infrastructure.SecurityTests.Models
{
    public class FirstResource2 : IResourceInstance<int, FirstPermissions>
    {
        public int Id { get; set; }

        public FirstResource2(int id)
        {
            Id = id;
        }

        #region IResourceInstance Implementation
        
        public int ResourceUniqueIdentifier => Id;

        #endregion
    }
}
