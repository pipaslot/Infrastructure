using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Attributes;

namespace Pipaslot.Infrastructure.SecurityTests.Models
{

    [Name("Second Resource Name")]
    [Description("Second resource purpose description etc. etc. etc. etc.")]
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
