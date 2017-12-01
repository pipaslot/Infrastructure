using Pipaslot.Infrastructure.Security;

namespace Pipaslot.Infrastructure.SecurityTests.Models
{
    public class FirstResource : IResource<int, FirstPermissions>
    {
        public int Id { get; set; }

        public FirstResource(int id)
        {
            Id = id;
        }

        #region IResource Implementation
        
        public int ResourceUniqueIdentifier => Id;

        #endregion
    }
}
