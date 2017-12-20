using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pipaslot.Infrastructure.Data.EntityFramework;
using Pipaslot.Infrastructure.Data.EntityFrameworkTests.Models;
using Pipaslot.Infrastructure.Data.Queries;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkTests
{
    [TestClass]
    public class EntityFrameworkQueryIntegrationTest : IDisposable
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

        public EntityFrameworkQueryIntegrationTest()
        {
            _dbFactory = new BloggingContextFactory("EntitiFrameworkQueryTest" + Guid.NewGuid());
            _dbFactory.OnDbInit += (sender, context) =>
            {
                context.Blog.AddRange(_defaultData);
                context.SaveChanges();
            };
        }

        public void Dispose()
        {
            _dbFactory.Dispose();
        }

        [TestMethod]
        public void Execute()
        {
            var query = new BlogQuery(_dbFactory);
            var result = query.Execute();
            Assert.AreEqual(_defaultData.Count, result.Count);
        }

        [TestMethod]
        public async Task ExecuteAsync()
        {
            var query = new BlogQuery(_dbFactory);
            var result = await query.ExecuteAsync();
            Assert.AreEqual(_defaultData.Count, result.Count);
        }

        [TestMethod]
        public void GetTotalRowCount()
        {
            var query = new BlogQuery(_dbFactory);

            Assert.AreEqual(_defaultData.Count, query.GetTotalRowCount());
        }

        [TestMethod]
        public async Task GetTotalRowCountAsync()
        {
            var query = new BlogQuery(_dbFactory);

            var result = await query.GetTotalRowCountAsync();
            Assert.AreEqual(_defaultData.Count, result);
        }

        [TestMethod]
        public void Skip()
        {
            const int skipNumber = 10;
            var query = new BlogQuery(_dbFactory);
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
            var query = new BlogQuery(_dbFactory);
            query.Take = takeNumber;

            var result = query.Execute();
            Assert.AreEqual(takeNumber, result.Count);

            var first = result.First();
            Assert.AreEqual(_defaultData.First().Name, first.Name);
        }

        [TestMethod]
        public void AddSortCriteria_Function()
        {
            var query = new BlogQuery(_dbFactory);
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
            var query = new BlogQuery(_dbFactory);
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
            var query = new BlogWithRemappingQuery(_dbFactory);
            var result = query.Execute();
            foreach (var blog in _defaultData)
            {
                var existing = result.FirstOrDefault(b => b.IdName == BlogIdName.JoinIdAndName(blog.Id, blog.Name));
                Assert.IsNotNull(existing);
            }
        }

        #region Expected Query construction

        [TestMethod]
        public void ExpectedCustomQueryIsConstructible()
        {
            var factory = ResolveQueryFactory();
            Assert.IsNotNull(factory);
            var query = factory.Create();
            Assert.IsNotNull(query);
            Assert.IsTrue(query.GetTotalRowCount() > 0);
        }
        /// <summary>
        /// Prepare concrete implementation
        /// </summary>
        /// <returns></returns>
        private IQueryFactory<IQuery<IBlog>> ResolveQueryFactory()
        {
            return new EntityFrameworkQueryFactory<BlogQuery1<BloggingContext>>(_dbFactory);
        }

        [TestMethod]
        public void ExpectedCustomQueryIsConstructible2()
        {
            var factory = ResolveQueryFactory2();
            Assert.IsNotNull(factory);
            var query = factory.Create();
            Assert.IsNotNull(query);
            Assert.IsTrue(query.GetTotalRowCount() > 0);
        }
        /// <summary>
        /// Prepare concrete implementation
        /// </summary>
        /// <returns></returns>
        private IQueryFactory<IExpectedQuery> ResolveQueryFactory2()
        {
            return new EntityFrameworkQueryFactory<BlogQuery1<BloggingContext>>(_dbFactory);
        }

        #region Test classes

        public interface IExpectedQuery : IQuery<IBlog>
        {
        }

        public class BlogQuery1<TDbContext> : EntityFrameworkQuery<TDbContext, IBlog>, IExpectedQuery
            where TDbContext : DbContext, IBlogDatabase
        {
            public BlogQuery1(IEntityFrameworkDbContextFactory dbContextFactory) : base(dbContextFactory)
            {
            }
            

            //protected override IList<IBlog> PostProcessResults(IList<Blog> results)
            //{
            //    return (IList<IBlog>) results;
            //}

            protected override IQueryable<IBlog> GetQueryable()
            {
                return ContextReadOnly.Set<Blog>();
            }

            //public IList<Func<IQueryable<IBlog>, IOrderedQueryable<IBlog>>> SortCriteria => (IList<Func<IQueryable<IBlog>, IOrderedQueryable<IBlog>>>) base.SortCriteria;

            //public void AddSortCriteria<TKey>(Expression<Func<IBlog, TKey>> field, SortDirection direction = SortDirection.Ascending)
            //{
            //    throw new NotImplementedException();
            //}
        }

        #endregion
        
        #endregion
    }
}
