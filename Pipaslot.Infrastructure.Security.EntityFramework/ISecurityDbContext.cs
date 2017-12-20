using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Security.EntityFramework.Entities;

namespace Pipaslot.Infrastructure.Security.EntityFramework
{
    public interface ISecurityDbContext<TKey>
    {
        DbSet<Role<TKey>> SecurityRole { get; }
        DbSet<Privilege<TKey>> SecurityPrivilege { get; }
    }
}
