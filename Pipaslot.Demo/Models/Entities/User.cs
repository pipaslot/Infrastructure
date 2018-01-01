using System.Collections.Generic;
using Pipaslot.Infrastructure.Data;
namespace Pipaslot.Demo.Models.Entities
{
    public class User : IEntity<int>
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

        public string Login { get; set; }

        public string PasswordHash { get; set; }

        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
