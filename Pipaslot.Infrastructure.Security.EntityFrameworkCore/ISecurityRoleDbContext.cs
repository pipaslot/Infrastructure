using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Security.EntityFrameworkCore.Entities;

namespace Pipaslot.Infrastructure.Security.EntityFrameworkCore
{
    public interface ISecurityRoleDbContext<TKey> : ISecurityRoleDbContext<TKey, Role<TKey>>
    {
    }

    public interface ISecurityRoleDbContext<TKey, TRole> where TRole : Role<TKey>
    {
        DbSet<TRole> SecurityRole { get; }
    }
}
