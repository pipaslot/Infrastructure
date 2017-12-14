using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pipaslot.Infrastructure.Data.EntityFramework;
using Pipaslot.Infrastructure.Data.EntityFrameworkTests.Models;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkTests
{
    [TestClass]
    public class EntityFrameworkRepositoryTest
    {
        [TestMethod]
        public void ReadFromOutsideOfUnitWOrkButUpdateInUnitOfWOrkCreatedLater()
        {
            using (var dbFactory = new BloggingContextFactory("ReadFromOutsideOfUnitWOrkButUpdateInUnitOfWOrkCreatedLater"))
            {
                var uowFactory = new EntityFrameworkUnitOfWorkFactory<BloggingContext>(dbFactory, new UnitOfWorkRegistry());
                var repository = new BlogRepository(uowFactory, dbFactory);
                string defaultBlogName = "Default name";
                string newBlogName = "UpdatedBlogName";

                //Create record
                using (var uow = uowFactory.Create())
                {
                    var context = uow.Context;
                    Assert.AreEqual(0, context.Blog.Count());
                    context.Blog.Add(new Blog(defaultBlogName));
                    uow.Commit();
                    Assert.AreEqual(1, context.Blog.Count());
                }
                //Read outside from UoW
                var blog = repository.GetByName(defaultBlogName);
                Assert.IsNotNull(blog);

                //Update by another UoW
                using (var uow = uowFactory.Create())
                {
                    blog.Name = newBlogName;
                    repository.Update(blog);
                    uow.Commit();
                }

                //Read from outside UoW and get updated value

                var updatedBlog = repository.GetById(blog.Id);
                Assert.IsNotNull(updatedBlog);
                Assert.AreEqual(newBlogName, updatedBlog.Name);
            }
        }
    }
}
