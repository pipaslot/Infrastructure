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
            Assert.AreEqual(3, registry.ResourceTypes.Count);

            var firstResource = registry.ResourceTypes.First(r=>r.ResourceType == typeof(FirstResource));
            Assert.AreEqual(1, firstResource.InstancePermissions.Count);
            Assert.AreEqual(2, firstResource.StaticPermissions.Count);
            Assert.IsTrue(firstResource.InstancePermissions.Contains(typeof(FirstPermissions)));
            Assert.IsTrue(firstResource.StaticPermissions.Contains(typeof(FirstPermissions)));
            Assert.IsTrue(firstResource.StaticPermissions.Contains(typeof(StaticPermissions)));
        }
    }
}
