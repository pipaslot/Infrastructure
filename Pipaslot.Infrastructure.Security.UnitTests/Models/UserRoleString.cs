using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.SecurityTests.Models
{
    class UserRoleString : IRole<string>
    {
        object IRole.Id => Id;

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public RoleType Type { get; set; } = RoleType.Custom;

        public UserRoleString(string id)
        {
            Id = id;
        }
    }
}