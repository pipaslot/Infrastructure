using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.SecurityTests.Models;

namespace Pipaslot.Infrastructure.SecurityTests
{
    [TestClass]
    public class ResourceRegistryTest
    {
        [TestMethod]
        public void LoadResourcesAndPermissionsFromTestAssembly()
        {
            var registry = new ResourceRegistry<int>();
            registry.Register(typeof(FirstResource).Assembly);

            //Act
            Assert.AreEqual(2, registry.ResourceTypes.Count);

            Assert.AreEqual(1, registry.ResourceTypes.First().Value.Count);
            Assert.AreEqual(typeof(FirstPermissions), registry.ResourceTypes.First().Value.First());
        }
    }
}
