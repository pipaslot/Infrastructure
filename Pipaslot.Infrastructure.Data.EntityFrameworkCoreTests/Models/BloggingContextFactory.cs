﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pipaslot.Infrastructure.Data.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkCoreTests.Models
{
    public class BloggingContextFactory : IEntityFrameworkDbContextFactory<BloggingContext>, IDisposable
    {
        private readonly string _dbName;
        private bool isInitialized;
        private List<BloggingContext>Dbs = new List<BloggingContext>();

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

            builder.UseSqlServer(
                    $"Server=(local);Database=testdb_{_dbName};Trusted_Connection=True;MultipleActiveResultSets=true")
                .UseInternalServiceProvider(serviceProvider);

            var context = new BloggingContext(builder.Options);
            if (!isInitialized)
            {
                isInitialized = true;
                context.Database.Migrate();
            }
            Dbs.Add(context);
            return context;
        }

        public void Dispose()
        {
            foreach (var db in Dbs)
            {
                db.Database.EnsureDeleted();
            }
        }
    }
}
