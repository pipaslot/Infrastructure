using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pipaslot.Infrastructure.Data.EntityFramework;
using Pipaslot.Infrastructure.Data.EntityFrameworkTests.Models;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkTests
{
    [TestClass]
    public class UnitOfWorkIntegrationTest : IDisposable
    {
        private List<BloggingContextFactory> dbFactories = new List<BloggingContextFactory>();

        [TestMethod]
        public void SingleLevelTransaction()
        {
            var uowFactory = CreateUoW("SingleLevelTransaction");
            using (var uow = uowFactory.Create())
            {
                Assert.AreEqual(0, uow.Context.Blog.Count());
                uow.Context.Blog.Add(new Blog
                {
                    Name = "MyName"
                });
                uow.Commit();
            }

            using (var uow = uowFactory.Create())
            {
                Assert.AreEqual(1, uow.Context.Blog.Count());
            }
        }

        [TestMethod]
        public void Commit_DataShouldBeAvailableInSeparatedContextsAfterCommit()
        {
            var dbFactory = CreateDbFactory("Commit_DataShouldBeAvailableInSeparatedContextsAfterCommit");
            var uowFactory = new EntityFrameworkUnitOfWorkFactory<BloggingContext>(dbFactory);
            using (var uow = uowFactory.Create())
            {
                Assert.AreEqual(0, uow.Context.Blog.Count());

                CreateRecord(uowFactory);
                //Data are not stored until commit of this UOW
                Assert.AreEqual(1, uow.Context.Blog.Count());
                var context = dbFactory.Create();//Do not dispose here the database, otherwise it will be deleted

                //Data are not available also from another separated context
                Assert.AreEqual(0, context.Blog.Count());

                //Store changes
                uow.Commit();

                //Data are also into separated context
                Assert.AreEqual(1, context.Blog.Count());

                //Changes are in UoW context
                Assert.AreEqual(1, uow.Context.Blog.Count());
            }
        }

        [TestMethod]
        public void Commit_DataShouldntBeAvailableInSeparatedContextsUntilCommit()
        {
            var dbName = "Commit_DataShouldBeUnavailableInSeparatedContextsUntilCommit";
            var uowFactory = CreateUoW(dbName);
            using (var uow = uowFactory.Create())
            {
                Assert.AreEqual(0, uow.Context.Blog.Count());

                CreateRecord(uowFactory);

                Assert.AreEqual(1, uow.Context.Blog.Count());

            }
            using (var uow = uowFactory.Create())
            {
                //Changes shouldn't be into another UoW contexts
                Assert.AreEqual(0, uow.Context.Blog.Count());
            }
        }

        private void CreateRecord(EntityFrameworkUnitOfWorkFactory<BloggingContext> uowFactory)
        {
            using (var uow = uowFactory.Create())
            {
                uow.Context.Blog.Add(new Blog
                {
                    Name = "MyName"
                });
                uow.Commit();
            }
        }

        private EntityFrameworkUnitOfWorkFactory<BloggingContext> CreateUoW(string dbName)
        {
            var dbFactory = CreateDbFactory(dbName);
            return new EntityFrameworkUnitOfWorkFactory<BloggingContext>(dbFactory);
        }

        private BloggingContextFactory CreateDbFactory(string dbName)
        {
            var dbFactory = new BloggingContextFactory(dbName);
            dbFactories.Add(dbFactory);
            return dbFactory;
        }

        public void Dispose()
        {
            foreach (var factory in dbFactories)
            {
                factory.Dispose();
            }
        }
    }
}
