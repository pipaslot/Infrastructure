using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Security.EntityFramework.Entities;

namespace Pipaslot.Infrastructure.Security.EntityFramework
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
