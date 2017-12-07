using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pipaslot.Infrastructure.Data.EntityFrameworkCore;
using Pipaslot.Infrastructure.Data.EntityFrameworkCoreTests.Models;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkCoreTests
{
    [TestClass]
    public class UnitOfWorkIntegrationTest
    {
        [TestMethod]
        public void SingleLevelTransaction()
        {
            var uowFactory = CreateUoW("SingleLevelTransaction");
            using (var uow = uowFactory.Create())
            {
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
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetCurrent_WhithoutCreationOfPreviousOne_ShouldThrowException()
        {
            var uowFactory = CreateUoW("SingleLevelTransaction");
            uowFactory.GetCurrent();
        }

        [TestMethod]
        public void Commit_DataShouldBeAvailableInSeparatedContextsAfterCommit()
        {
            var dbName = "Commit_DataShouldBeAvailableInSeparatedContextsAfterCommit";
            var dbFactory = new BloggingContextFactory(dbName);
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

        private EntityFrameworkUnitOfWorkFactory<BloggingContext> CreateUoW(string dbName)
        {
            var dbFactory = new BloggingContextFactory(dbName);
            return new EntityFrameworkUnitOfWorkFactory<BloggingContext>(dbFactory);
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
    }
}
