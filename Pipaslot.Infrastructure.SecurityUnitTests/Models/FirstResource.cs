using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Attributes;

namespace Pipaslot.Infrastructure.SecurityTests.Models
{
    [Name("First Resource Name")]
    [Description("First resourceInstance purpose description etc. etc. etc. etc.")]
    public class FirstResource : IResourceInstance<int, FirstPermissions>, IResource<StaticPermissions>
    {
        public int Id { get; set; }

        public FirstResource(int id)
        {
            Id = id;
        }

        #region IResourceInstance Implementation
        
        public int ResourceUniqueIdentifier => Id;

        #endregion
    }
}
