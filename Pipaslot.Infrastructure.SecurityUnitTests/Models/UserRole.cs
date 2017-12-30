using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.SecurityTests.Models
{
    class UserRole : IRole<int>
    {
        object IRole.Id => Id;

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public RoleType Type { get; set; } = RoleType.Custom;

        public UserRole(int id)
        {
            Id = id;
        }
    }
}
