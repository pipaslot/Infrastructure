using Pipaslot.Infrastructure.Security.EntityFrameworkCore.Entities;

namespace Pipaslot.Demo.Models.Entities
{
    public class UserRole
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int RoleId { get; set; }
        public Role<int> Role { get; set; }

        public UserRole()
        {
        }

        public UserRole(User user, Role<int> role)
        {
            User = user;
            UserId = user.Id;
            Role = role;
            RoleId = Role.Id;
        }
    }
}
