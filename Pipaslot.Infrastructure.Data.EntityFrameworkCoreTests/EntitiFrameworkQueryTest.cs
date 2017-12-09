using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pipaslot.Infrastructure.Data.EntityFrameworkCore;
using Pipaslot.Infrastructure.Data.EntityFrameworkCoreTests.Models;
using Pipaslot.Infrastructure.Data.Queries;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkCoreTests
{
    [TestClass]
    public class EntitiFrameworkQueryTest : IDisposable
    {
        private readonly List<Blog> _defaultData = new List<Blog>
        {
            new Blog("Blog 01"),
            new Blog("Blog 02"),
            new Blog("Blog 03"),
            new Blog("Blog 04"),
            new Blog("Blog 05"),
            new Blog("Blog 06"),
            new Blog("Blog 07"),
            new Blog("Blog 08"),
            new Blog("Blog 09"),
            new Blog("Blog 10"),
            new Blog("Blog 11"),
            new Blog("Blog 12"),
            new Blog("Blog 13"),
            new Blog("Blog 14"),
            new Blog("Blog 15"),
            new Blog("Blog 16"),
            new Blog("Blog 17"),
            new Blog("Blog 18"),
            new Blog("Blog 19"),
            new Blog("Blog 20")
        };

        private readonly BloggingContextFactory _dbFactory;
        private readonly EntityFrameworkUnitOfWorkFactory<BloggingContext> _uowFactory;

        public EntitiFrameworkQueryTest()
        {
            _dbFactory = new BloggingContextFactory("EntitiFrameworkQueryTest" + Guid.NewGuid());
            _dbFactory.OnDbInit += (sender, context) =>
            {
                context.Blog.AddRange(_defaultData);
                context.SaveChanges();
            };
            _uowFactory = new EntityFrameworkUnitOfWorkFactory<BloggingContext>(_dbFactory);
        }

        public void Dispose()
        {
            _dbFactory.Dispose();
        }

        [TestMethod]
        public void Execute()
        {
            var query = new BlogQuery(_uowFactory);
            var result = query.Execute();
            Assert.AreEqual(_defaultData.Count, result.Count);
        }

        [TestMethod]
        public async Task ExecuteAsync()
        {
            var query = new BlogQuery(_uowFactory);
            var result = await query.ExecuteAsync();
            Assert.AreEqual(_defaultData.Count, result.Count);
        }

        [TestMethod]
        public void GetTotalRowCount()
        {
            var query = new BlogQuery(_uowFactory);

            Assert.AreEqual(_defaultData.Count, query.GetTotalRowCount());
        }

        [TestMethod]
        public async Task GetTotalRowCountAsync()
        {
            var query = new BlogQuery(_uowFactory);

            var result = await query.GetTotalRowCountAsync();
            Assert.AreEqual(_defaultData.Count, result);
        }

        [TestMethod]
        public void Skip()
        {
            const int skipNumber = 10;
            var query = new BlogQuery(_uowFactory);
            query.Skip = skipNumber;

            var result = query.Execute();
            Assert.AreEqual(_defaultData.Count - 10, result.Count);

            var first = result.First();
            Assert.AreEqual(skipNumber + 1, first.Id);
        }


        [TestMethod]
        public void Take()
        {
            const int takeNumber = 10;
            var query = new BlogQuery(_uowFactory);
            query.Take = takeNumber;

            var result = query.Execute();
            Assert.AreEqual(takeNumber, result.Count);

            var first = result.First();
            Assert.AreEqual(_defaultData.First().Name, first.Name);
        }


        [TestMethod]
        public void AddSortCriteria_Function()
        {
            var query = new BlogQuery(_uowFactory);
            query.AddSortCriteria(b => b.Name);

            var result1 = query.Execute();
            var first = result1.First();
            Assert.AreEqual(_defaultData.First().Name, first.Name);

            query.ClearSortCriteria();
            query.AddSortCriteria(b => b.Name, SortDirection.Descending);
            var result2 = query.Execute();
            var last = result2.First();
            Assert.AreEqual(_defaultData.Last().Name, last.Name);
        }


        [TestMethod]
        public void AddSortCriteria_propertyName()
        {
            var query = new BlogQuery(_uowFactory);
            query.AddSortCriteria("name");

            var result1 = query.Execute();
            var first = result1.First();
            Assert.AreEqual(_defaultData.First().Name, first.Name);

            query.ClearSortCriteria();
            query.AddSortCriteria("name", SortDirection.Descending);
            var result2 = query.Execute();
            var last = result2.First();
            Assert.AreEqual(_defaultData.Last().Name, last.Name);
        }


        [TestMethod]
        public void PostProcessResult_ShouldReturnTransformedObjectBlogIdNameWhichHavePropertyCreatedFromDbData()
        {
            var query = new BlogWithRemappingQuery(_uowFactory);
            var result = query.Execute();
            foreach (var blog in _defaultData)
            {
                var existing = result.FirstOrDefault(b => b.IdName == BlogIdName.JoinIdAndName(blog.Id, blog.Name));
                Assert.IsNotNull(existing);
            }
        }
    }
}
