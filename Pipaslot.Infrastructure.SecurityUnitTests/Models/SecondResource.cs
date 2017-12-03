using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Attributes;

namespace Pipaslot.Infrastructure.SecurityTests.Models
{

    [Name("Second Resource Name")]
    [Description("Second resourceInstance purpose description etc. etc. etc. etc.")]
    public class SecondResource : IResourceInstance<string, SecondPermissions>
    {
        public string Id { get; set; }

        public SecondResource(string id)
        {
            Id = id;
        }

        #region IResourceInstance Implementation
        
        public string ResourceUniqueIdentifier => Id;

        #endregion
    }
}
