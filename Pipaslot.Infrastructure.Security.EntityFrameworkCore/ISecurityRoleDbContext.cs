using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Security.EntityFrameworkCore.Entities;

namespace Pipaslot.Infrastructure.Security.EntityFrameworkCore
{
    public interface ISecurityRoleDbContext<TKey>
    {
        DbSet<Role<TKey>> SecurityRole { get; }
    }
    public interface ISecurityPrivilegeDbContext<TKey>
    {
        DbSet<Privilege<TKey>> SecurityPrivilege { get; }
    }
}
