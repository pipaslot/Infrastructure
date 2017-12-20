using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pipaslot.Infrastructure.Data.EntityFramework;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkTests.Models
{
    public class BloggingContextFactory : EntityFrameworkDbContextFactory<BloggingContext>, IDisposable
    {
        private bool isInitialized;
        private readonly List<BloggingContext> _databases = new List<BloggingContext>();

        public event EventHandler<BloggingContext> OnDbInit;
        
        public BloggingContextFactory(string dbName) : base(PrepareOptions(dbName))
        {
        }

        private static DbContextOptions PrepareOptions(string dbName)
        {
            var builder = new DbContextOptionsBuilder<BloggingContext>();
            builder.UseSqlServer(
                $"Server=(local);Database=testdb_{dbName};Trusted_Connection=True;MultipleActiveResultSets=true");
            return builder.Options;
        }

        public override BloggingContext GetReadOnlyContext()
        {
            return Create();
        }

        public override BloggingContext Create()
        {
            var context = base.Create();
            if (!isInitialized)
            {
                isInitialized = true;
                context.Database.Migrate();
                OnDbInit?.Invoke(this, context);
            }
            _databases.Add(context);
            return context;
        }

        public void Dispose()
        {
            if (isInitialized)
            {
                using (var db = Create())
                {
                    db.Database.EnsureDeleted();
                }
            }
        }
    }
}
