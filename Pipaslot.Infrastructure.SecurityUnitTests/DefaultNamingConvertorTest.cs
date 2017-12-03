using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.SecurityTests.Models;

namespace Pipaslot.Infrastructure.SecurityTests
{
    [TestClass]
    public class DefaultNamingConvertorTest
    {
        [TestMethod]
        public void ResourceConversion()
        {
            var resourceType = typeof(FirstResource);
            
            var resourceRegistry = new ResourceRegistry<int>();
            resourceRegistry.Register(resourceType.Assembly);

            var convertor = new DefaultNamingConvertor<int>(resourceRegistry);

            var resourceName = convertor.GetResourceUniqueName(resourceType);
            Assert.IsFalse(string.IsNullOrWhiteSpace(resourceName));

            var resourceType2 = convertor.GetResourceTypeByUniqueName(resourceName);
            Assert.AreEqual(resourceType, resourceType2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetResourceTypeByUniqueName_ThrowExceptinIfTypeCanNotBeFound()
        {
            var resourceRegistry = new ResourceRegistry<int>();
            var convertor = new DefaultNamingConvertor<int>(resourceRegistry);

            convertor.GetResourceTypeByUniqueName(typeof(FirstResource).FullName);
        }
    }
}
