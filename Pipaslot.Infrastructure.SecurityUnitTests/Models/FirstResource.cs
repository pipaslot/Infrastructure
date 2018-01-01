using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Attributes;

namespace Pipaslot.Infrastructure.SecurityTests.Models
{
    [Name("First Resource Name")]
    [Description("First resourceInstance purpose description etc. etc. etc. etc.")]
    public class FirstResource : IResourceInstance<FirstPermissions>, IResource<StaticPermissions>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public FirstResource(int id)
        {
            Id = id;
        }

        public FirstResource(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        #region IResourceInstance Implementation

        object IResourceInstance.ResourceUniqueIdentifier => Id;
        
        public string ResourceName => Name;

        public string ResourceDescription => Description;

        #endregion

    }
}
