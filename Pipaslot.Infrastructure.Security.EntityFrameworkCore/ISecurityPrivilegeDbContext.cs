using System;
using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Security.EntityFrameworkCore.Entities;

namespace Pipaslot.Infrastructure.Security.EntityFrameworkCore
{
    [Obsolete]
    public interface ISecurityPrivilegeDbContext<TKey> : ISecurityPrivilegeDbContext<TKey, Privilege<TKey>>
    {
    }
    public interface ISecurityPrivilegeDbContext<TKey, TPrivilege> where TPrivilege : Privilege<TKey>
    {
        DbSet<TPrivilege> SecurityPrivilege { get; }
    }
}