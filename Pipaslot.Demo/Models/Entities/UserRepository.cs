using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Data.EntityFramework;

namespace Pipaslot.Demo.Models.Entities
{
    public class UserRepository : EntityFrameworkRepository<AppDatabase,User,int>
    {
        public UserRepository(IUnitOfWorkFactory uowFactory, IEntityFrameworkDbContextFactory dbContextFactory) : base(uowFactory, dbContextFactory)
        {
        }

        public User GetByLogin(string login)
        {
            return ContextReadOnly.User
                .Include(u => u.UserRoles)
                .ThenInclude(ur=>ur.Role)
                .FirstOrDefault(u => u.Login.Equals(login));
        }
    }
}
