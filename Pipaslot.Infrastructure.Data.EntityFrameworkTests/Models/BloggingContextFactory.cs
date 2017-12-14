using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pipaslot.Infrastructure.Data.EntityFramework;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkTests.Models
{
    public class BloggingContextFactory : IEntityFrameworkDbContextFactory<BloggingContext>, IDisposable
    {
        private readonly string _dbName;
        private bool isInitialized;
        private List<BloggingContext>Dbs = new List<BloggingContext>();

        public event EventHandler<BloggingContext> OnDbInit;

        public BloggingContextFactory(string dbName)
        {
            _dbName = dbName;
        }

        public BloggingContext Create()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<BloggingContext>();

            builder.UseSqlServer($"Server=(local);Database=testdb_{_dbName};Trusted_Connection=True;MultipleActiveResultSets=true")
                .UseInternalServiceProvider(serviceProvider);

            var context = new BloggingContext(builder.Options);
            if (!isInitialized)
            {
                isInitialized = true;
                context.Database.Migrate();
                if (OnDbInit != null)
                {
                    OnDbInit(this, context);
                }
            }
            Dbs.Add(context);
            return context;
        }

        public BloggingContext GetReadOnlyContext()
        {
            return Create();
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
