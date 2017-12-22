using Microsoft.EntityFrameworkCore;
using Pipaslot.Demo.Models.Entities;
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

        public DbSet<Company> Company{ get; set; }

        #region ISecurityDbContext Implementation
        
        public DbSet<Role<int>> SecurityRole { get; set; }

        /// <summary>
        /// Entitied for ISecurityDbContext
        /// </summary>
        public DbSet<Privilege<int>> SecurityPrivilege { get; set; }

        #endregion
    }
}
