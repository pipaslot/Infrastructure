using Microsoft.EntityFrameworkCore;
using Pipaslot.Demo.Models.Entities;
using Pipaslot.Infrastructure.Security.EntityFrameworkCore;
using Pipaslot.Infrastructure.Security.EntityFrameworkCore.Entities;

namespace Pipaslot.Demo.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Application database
    /// </summary>
    public class AppDatabase : DbContext, ISecurityRoleDbContext<int, Role>, ISecurityPrivilegeDbContext<int, Privilege<int>>
    {
        /// <inheritdoc />
        /// <summary>
        /// Create Database from DbContextOptions
        /// </summary>
        /// <param name="options"></param>
        public AppDatabase(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        /// Application user
        /// </summary>
        public DbSet<User> User { get; set; }

        /// <summary>
        /// Company
        /// </summary>
        public DbSet<Company> Company{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>()
                .HasKey(t => new { t.UserId, t.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(pt => pt.User)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(pt => pt.UserId);
            modelBuilder.Entity<User>()
                .HasMany(u => u.UserRoles)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId);
            //modelBuilder.Entity<UserRole>()
            //    .HasOne(pt => pt.Role)
            //.WithMany(t => t.UserRoles)
            // .HasForeignKey(pt => pt.RoleId);
            base.OnModelCreating(modelBuilder);
        }

        #region ISecurityDbContext Implementation

        /// <summary>
        /// Entity for ISecurityDbContext
        /// </summary>
        public DbSet<Role> SecurityRole { get; set; }

        /// <summary>
        /// Entity for ISecurityDbContext
        /// </summary>
        public DbSet<Privilege<int>> SecurityPrivilege { get; set; }

        #endregion
    }
}
