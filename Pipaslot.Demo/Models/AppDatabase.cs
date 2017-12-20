using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Security.EntityFramework;
using Pipaslot.Infrastructure.Security.EntityFramework.Entities;

namespace Pipaslot.Demo.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Application database
    /// </summary>
    public class AppDatabase : DbContext, ISecurityDbContext<int>
    {
        /// <inheritdoc />
        /// <summary>
        /// Create Database from DbContextOptions
        /// </summary>
        /// <param name="options"></param>
        public AppDatabase(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Role<int>> SecurityRole { get; }

        /// <summary>
        /// Entitied for ISecurityDbContext
        /// </summary>
        public DbSet<Privilege<int>> SecurityPrivilege { get; }
    }
}
