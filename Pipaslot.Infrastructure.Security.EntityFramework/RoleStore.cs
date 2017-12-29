using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Data.EntityFramework;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.Infrastructure.Security.EntityFramework.Entities;

namespace Pipaslot.Infrastructure.Security.EntityFramework
{
    public class RoleStore<TKey, TDbContext> : EntityFrameworkRepository<TDbContext, Role<TKey>, TKey>, IRoleStore
        where TDbContext : DbContext, ISecurityDbContext<TKey>
    {
        public RoleStore(IUnitOfWorkFactory uowFactory, IEntityFrameworkDbContextFactory dbContextFactory) : base(uowFactory, dbContextFactory)
        {
        }

        public IEnumerable<IRole> GetAll()
        {
            return ContextReadOnly.SecurityRole.ToList();
        }

        public object GetGuestRoleIdentifier()
        {
            return ContextReadOnly.SecurityRole.Where(r => r.Name.Equals("Guest")).Select(r => r.Id).FirstOrDefault();
        }
    }
}
