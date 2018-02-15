namespace Pipaslot.Demo.Models.Entities
{
    public class UserRole
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public UserRole()
        {
        }

        public UserRole(User user, Role role)
        {
            User = user;
            UserId = user.Id;
            Role = role;
            RoleId = Role.Id;
        }
    }
}
