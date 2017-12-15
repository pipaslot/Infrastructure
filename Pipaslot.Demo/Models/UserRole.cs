using Pipaslot.Infrastructure.Security;

namespace Pipaslot.Demo.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Role assigned to User
    /// </summary>
    public class UserRole : IUserRole<int>
    {
        #pragma warning disable 1591
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Readable role Name
        /// </summary>
        public string Name { get; set; }

        public UserRole(int id, string name = "")
        {
            Id = id;
            Name = name;
        }
    }
}
