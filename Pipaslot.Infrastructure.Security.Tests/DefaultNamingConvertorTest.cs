using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Tests.Models;

namespace Pipaslot.Infrastructure.Security.Tests
{
    [TestClass]
    public class DefaultNamingConvertorTest
    {
        [TestMethod]
        public void ResourceConversion()
        {
            var resourceType = typeof(FirstResource);
            
            var resourceRegistry = new ResourceRegistry();
            resourceRegistry.Register(resourceType.Assembly);

            var convertor = new DefaultNamingConvertor(resourceRegistry);

            var resourceName = convertor.GetResourceUniqueName(resourceType);
            Assert.IsFalse(string.IsNullOrWhiteSpace(resourceName));

            var resourceType2 = convertor.GetResourceTypeByUniqueName(resourceName);
            Assert.AreEqual(resourceType, resourceType2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetResourceTypeByUniqueName_ThrowExceptinIfTypeCanNotBeFound()
        {
            var resourceRegistry = new ResourceRegistry();
            var convertor = new DefaultNamingConvertor(resourceRegistry);

            convertor.GetResourceTypeByUniqueName(typeof(FirstResource).FullName);
        }
    }
}
