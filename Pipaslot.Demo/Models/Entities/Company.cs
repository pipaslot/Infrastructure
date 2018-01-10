using Pipaslot.Demo.Models.Permissions;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Security;

namespace Pipaslot.Demo.Models.Entities
{
    /// <inheritdoc />
    /// <summary>
    /// Company Entity
    /// </summary>
    public class Company : IEntity<int>, IResourceInstance<CompanyPermissions>, IResource<CompanyStaticPermissions>
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }
        object IEntity.Id
        {
            get => Id;
            set => Id = (int)value;
        }
        /// <summary>
        /// Company name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Company details
        /// </summary>
        public string Description { get; set; }

        public object ResourceUniqueIdentifier => Id;

        public string ResourceName => Name;

        public string ResourceDescription => Description;

    }
}
