using Pipaslot.Infrastructure.Security;

namespace Pipaslot.Infrastructure.SecurityTests.Models
{
    public class SecondResource : IResource<string, SecondPermissions>
    {
        public string Id { get; set; }

        public SecondResource(string id)
        {
            Id = id;
        }

        #region IResource Implementation
        
        public string ResourceUniqueIdentifier => Id;

        #endregion
    }
}
