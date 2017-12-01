using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.Infrastructure.Security.Exceptions;
using Pipaslot.Infrastructure.SecurityTests.Models;

namespace Pipaslot.Infrastructure.SecurityTests
{
    [TestClass]
    public class AuthorizatorTest
    {

        [TestMethod]
        public void Constructor_UseCustomNamingConvertor()
        {
            const int roleId = 1;
            const int resourceId = 2;
            var permission = FirstPermissions.Edit;

            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var convertor = new ConstantNamingConvertor();
            var auth = new Authorizator<int>(permissionStore.Object, convertor);

            var role = new UserRole(roleId);
            var resource = new FirstResource(resourceId);

            //Pre-Condition
            permissionStore.Setup(p => p.IsAllowed(roleId, ConstantNamingConvertor.RESOURCE, resourceId, ConstantNamingConvertor.PERMISSION));

            //Act
            auth.IsAllowed(role, resource, permission);

            //Assertion
            permissionStore.VerifyAll();
        }

        [TestMethod]
        public void GlobalPermission_ShouldPass()
        {
            const int roleId = 1;
            var permission = FirstPermissions.Edit;

            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var convertor = new DefaultNamingConvertor();
            var auth = new Authorizator<int>(permissionStore.Object);

            var role = new UserRole(roleId);

            //Pre-Condition
            permissionStore
                .Setup(p => p.IsAllowed(roleId, Authorizator<int>.GLOBAL_RESOURCE_NAME, default(int),
                    convertor.GetPermissionUniqueIdentifier(permission)))
                .Returns(true);

            //Act
            Assert.IsTrue(auth.IsAllowed(role, permission));

            //Assertion
            permissionStore.VerifyAll();
        }

        [TestMethod]
        public void ResourceInstancePermission_ShouldPass()
        {
            const int roleId = 1;
            const int resourceId = 1;
            var permission = FirstPermissions.Edit;

            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var convertor = new DefaultNamingConvertor();
            var auth = new Authorizator<int>(permissionStore.Object);

            var role = new UserRole(roleId);
            var resource = new FirstResource(resourceId);

            //Pre-Condition
            permissionStore
                .Setup(p => p.IsAllowed(roleId, convertor.GetResourceUniqueName(resource.GetType()), resourceId,
                    convertor.GetPermissionUniqueIdentifier(permission)))
                .Returns(true);

            //Act
            Assert.IsTrue(auth.IsAllowed(role, resource, permission));

            //Assertion
            permissionStore.VerifyAll();
        }

        [TestMethod]
        public void ResourceTypeAndIdPermission_ShouldPass()
        {
            const int roleId = 1;
            const int resourceId = 1;
            var permission = FirstPermissions.Edit;

            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var convertor = new DefaultNamingConvertor();
            var auth = new Authorizator<int>(permissionStore.Object);

            var role = new UserRole(roleId);

            //Pre-Condition
            permissionStore
                .Setup(p => p.IsAllowed(roleId, convertor.GetResourceUniqueName(typeof(FirstResource)), resourceId,
                    convertor.GetPermissionUniqueIdentifier(permission)))
                .Returns(true);

            //Act
            Assert.IsTrue(auth.IsAllowed(role, typeof(FirstResource), resourceId, permission));

            //Assertion
            permissionStore.VerifyAll();
        }
        
        [TestMethod]
        [ExpectedException(typeof(NotSuitablePermissionException))]
        public void ResourceTypeAndIdPermission_PermissionIsNotAllowedForResource_ShouldFail()
        {
            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var auth = new Authorizator<int>(permissionStore.Object);

            //Act
            auth.IsAllowed(new UserRole(1), typeof(FirstResource), 1, SecondPermissions.Edit);
        }

        [TestMethod]
        public void GetAllowedKeys_ShouldPass()
        {
            const int roleId = 1;
            var permission = FirstPermissions.Edit;

            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var convertor = new DefaultNamingConvertor();
            var auth = new Authorizator<int>(permissionStore.Object);
            var role = new UserRole(roleId);
            var allowed = new List<int> { 1, 3, 5 };

            //Pre-Condition
            permissionStore
                .Setup(p => p.GetAllowedResourceIds(roleId, convertor.GetResourceUniqueName(typeof(FirstResource)),
                    convertor.GetPermissionUniqueIdentifier(permission)))
                .Returns(allowed);

            //Act
            var allowedResult = auth.GetAllowedKeys(role, typeof(FirstResource), permission);
            Assert.AreEqual(allowed, allowedResult);

            //Assertion
            permissionStore.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSuitablePermissionException))]
        public void GetAllowedKeys_PermissionIsNotAllowedForResource_ShouldFail()
        {
            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var auth = new Authorizator<int>(permissionStore.Object);

            //Act
            auth.GetAllowedKeys(new UserRole(1), typeof(FirstResource), SecondPermissions.Edit);
        }

        #region Mockups

        private class ConstantNamingConvertor : INamingConvertor
        {
            public const string RESOURCE = "resource";
            public const string PERMISSION = "permission";
            public string GetResourceUniqueName(Type resource)
            {
                return RESOURCE;
            }

            public string GetPermissionUniqueIdentifier(IConvertible permissionEnum)
            {
                return PERMISSION;
            }
        }
        
        #endregion
    }
}
