using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Pipaslot.Infrastructure.Data.Tests
{
    [TestClass]
    public class UnitOfWorkRegistryTest
    {
        [TestMethod]
        public void GetCurrent_withoutRegisteredReturnsNull()
        {
            var registry = new UnitOfWorkRegistry();
            Assert.IsNull(registry.GetCurrent());
            Assert.IsNull(registry.GetCurrent(1));
            Assert.IsNull(registry.GetCurrent(2));
        }

        [TestMethod]
        public void Register_asDefaultReturnsLastUnitOfWOrk()
        {
            var mock1 = new Mock<IUnitOfWork>();
            var mock2 = new Mock<IUnitOfWork>();
            var registry = new UnitOfWorkRegistry();

            registry.Register(mock1.Object);
            Assert.AreEqual(mock1.Object, registry.GetCurrent());
            Assert.IsNull(registry.GetCurrent(1));

            registry.Register(mock2.Object);
            Assert.AreEqual(mock2.Object, registry.GetCurrent());
            Assert.AreEqual(mock1.Object, registry.GetCurrent(1));
            Assert.IsNull(registry.GetCurrent(2));
        }

        [TestMethod]
        public void Unregister_CorrectWorkflow()
        {
            var mock1 = new Mock<IUnitOfWork>();
            var mock2 = new Mock<IUnitOfWork>();
            var registry = new UnitOfWorkRegistry();

            registry.Register(mock1.Object);
            registry.Register(mock2.Object);

            registry.Unregister(mock2.Object);
            registry.Unregister(mock1.Object);
            Assert.IsNull(registry.GetCurrent());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Unregister_UnexistingUoW_ThrowsException()
        {
            var mock1 = new Mock<IUnitOfWork>();
            var registry = new UnitOfWorkRegistry();

            registry.Unregister(mock1.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Unregister_WrongOrder_ThrowsException()
        {
            var mock1 = new Mock<IUnitOfWork>();
            var mock2 = new Mock<IUnitOfWork>();
            var registry = new UnitOfWorkRegistry();

            registry.Register(mock1.Object);
            registry.Register(mock2.Object);

            registry.Unregister(mock1.Object);
        }

        [TestMethod]
        public async Task WorkForAsyncOperations()
        {
            var mock1 = new Mock<IUnitOfWork>();
            var registry = new UnitOfWorkRegistry();

            registry.Register(mock1.Object);

            Assert.AreEqual(mock1.Object, registry.GetCurrent());
            await WorkForAsyncOperations_asyncOperation(registry);

            Assert.AreEqual(mock1.Object, registry.GetCurrent());
            registry.Unregister(mock1.Object);

            Assert.IsNull(registry.GetCurrent());
        }

        private Task WorkForAsyncOperations_asyncOperation(UnitOfWorkRegistry registry)
        {
            var mock2 = new Mock<IUnitOfWork>();

            registry.Register(mock2.Object);
            Assert.AreEqual(mock2.Object, registry.GetCurrent());

            registry.Unregister(mock2.Object);
            return Task.FromResult(true);
        }
    }
}
