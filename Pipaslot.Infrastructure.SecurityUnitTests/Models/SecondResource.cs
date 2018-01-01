using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Attributes;

namespace Pipaslot.Infrastructure.SecurityTests.Models
{

    [Name("Second Resource Name")]
    [Description("Second resourceInstance purpose description etc. etc. etc. etc.")]
    public class SecondResource : IResourceInstance< SecondPermissions>
    {
        public string Id { get; set; }

        public SecondResource(string id)
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
