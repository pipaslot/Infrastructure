using Pipaslot.Infrastructure.Security;

namespace Pipaslot.Infrastructure.SecurityTests.Models
{
    public class FirstResource2 : IResourceInstance<FirstPermissions>
    {
        public int Id { get; set; }

        public FirstResource2(int id)
        {
            Id = id;
        }

        #region IResourceInstance Implementation

        object IResourceInstance.ResourceUniqueIdentifier => Id;

        public string ResourceName => "";

        public string ResourceDescription => "";

        #endregion
    }
}
