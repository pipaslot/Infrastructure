
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pipaslot.Infrastructure.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.EntityFrameworkCoreTests
{
    [TestClass]
    public class ModelBuilderExtensionsTest
    {
        [TestMethod]
        public void ConvertClassName11() => AssertTableName("MyEntity", "MyEntity");

        [TestMethod]
        public void ConvertClassName12() => AssertTableName("Entities.MyEntity", "Entities_MyEntity");

        [TestMethod]
        public void ConvertClassName13() => AssertTableName("Root.Entities.MyEntity", "Entities_MyEntity");

        [TestMethod]
        public void ConvertClassName14() => AssertTableName("Extra.Root.Entities.MyEntity", "Entities_MyEntity");


        [TestMethod]
        public void ConvertClassName21() => AssertTableName("MyEntity", "MyEntity", new[] { "Entities" });

        [TestMethod]
        public void ConvertClassName22() => AssertTableName("Entities.MyEntity", "MyEntity", new[] { "Entities" });

        [TestMethod]
        public void ConvertClassName23() => AssertTableName("Root.Entities.MyEntity", "Root_MyEntity", new[] { "Entities" });

        [TestMethod]
        public void ConvertClassName24() => AssertTableName("Extra.Root.Entities.MyEntity", "Root_MyEntity", new[] { "Entities" });

        [TestMethod]
        public void ConvertClassName25() => AssertTableName("Extra.Root.Entities.MyEntity", "Extra_MyEntity", new[] { "Root", "Entities" });

        [TestMethod]
        public void ConvertClassName26() => AssertTableName("Extra.Root.Entities.MyEntity", "Extra_MyEntity", new[] { "Entities", "Root" });

        #region Assertions

        private void AssertTableName(string name, string expected)
        {
            var result = ModelBuilderExtensions.GetTableNameWithNamespace(name, new string[0]);
            Assert.AreEqual(expected, result);
        }

        private void AssertTableName(string name, string expected, string[] ignoredNamespaces)
        {
            var result = ModelBuilderExtensions.GetTableNameWithNamespace(name, ignoredNamespaces);
            Assert.AreEqual(expected, result);
        }

        #endregion
    }
}
