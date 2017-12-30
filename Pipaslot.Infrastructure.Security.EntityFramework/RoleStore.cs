using System;
using System.Collections.Generic;
using System.Linq;
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

        public TRole GetGuestRole<TRole>() where TRole : IRole
        {
            var role = ContextReadOnly.SecurityRole
                .Where(r => r.Name.Equals("Guest"))
                .Select(r => (IRole)r)
                .FirstOrDefault();
            return (TRole)role;
        }

        public ICollection<TRole> GetAll<TRole>() where TRole : IRole
        {
            return (ICollection<TRole>)ContextReadOnly.SecurityRole.ToList();
        }

        public ICollection<TRole> GetSystemRoles<TRole>() where TRole : IRole
        {
            var roles = ContextReadOnly.SecurityRole
                .Where(r => r.Type == RoleType.Guest || r.Type == RoleType.User || r.Type == RoleType.Admin)
                .ToList();
            if (roles.Count != 3 || roles.Count(r => r.Type == RoleType.Guest) != 1 || roles.Count(r => r.Type == RoleType.User) != 1 || roles.Count(r => r.Type == RoleType.Admin) != 1)
            {
                throw new ApplicationException($"System roles were not correctly configured. Were expected 3 roles but found {roles.Count}. Into application can be defined only one {nameof(RoleType.Guest)}, {nameof(RoleType.User)} and  {nameof(RoleType.Admin)} role type");
            }
            return (ICollection<TRole>)roles;
        }
    }
}
