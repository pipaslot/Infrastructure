using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pipaslot.Infrastructure.Security;

namespace Pipaslot.Infrastructure.SecurityTests.Models
{
    public class UserRole : IUserRole<int>
    {
        public int Id { get; }

        public UserRole(int id)
        {
            Id = id;
        }

    }
}
