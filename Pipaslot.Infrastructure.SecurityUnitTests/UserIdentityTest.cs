using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.SecurityTests.Mockups;
using Pipaslot.Infrastructure.SecurityTests.Models;

namespace Pipaslot.Infrastructure.SecurityTests
{
    [TestClass]
    public class UserIdentityTest
    {
        [TestMethod]
        public void Constructor_MinimuParameters_UserIsUnauthorizedAndWithoutRolesByDefaultAndWithoutPermissions()
        {
            var authorizatorMock = new Mock<IAuthorizator<int>>();
            var user = new UserIdentity<int>(authorizatorMock.Object);

            Assert.AreEqual(0, user.Id);
            Assert.IsFalse(user.IsAuthenticated);
            Assert.IsFalse(user.IsAllowed(FirstPermissions.Edit));
            Assert.IsFalse(user.IsAllowed(new FirstResource(1), FirstPermissions.Edit));
            Assert.IsFalse(user.IsAllowed(typeof(FirstResource), 1, FirstPermissions.Edit));
            //Does not have any permissions for resource
            Assert.AreEqual(0, user.GetAllowedKeys(typeof(FirstResource), FirstPermissions.Edit).Count());
        }

        [TestMethod]
        public void Constructor_IntGenericType_FilledIdMeansThatUserWasAuthenticatedButShillDoesNotHaveAnyPermissions()
        {
            var authorizatorMock = new Mock<IAuthorizator<int>>();
            var user = new UserIdentity<int>(authorizatorMock.Object, 100);

            Assert.AreEqual(100, user.Id);
            Assert.IsTrue(user.IsAuthenticated);
            Assert.IsFalse(user.IsAllowed(FirstPermissions.Edit));
            Assert.IsFalse(user.IsAllowed(new FirstResource(1), FirstPermissions.Edit));
            Assert.IsFalse(user.IsAllowed(typeof(FirstResource), 1, FirstPermissions.Edit));
            //Does not have any permissions for resource
            Assert.AreEqual(0, user.GetAllowedKeys(typeof(FirstResource), FirstPermissions.Edit).Count());
        }

        [TestMethod]
        public void Constructor_StringGenericType_FilledIdMeansThatUserWasAuthenticatedButStillDoesNotHaveAnyPermissions()
        {
            var authorizatorMock = new Mock<IAuthorizator<string>>();
            var user = new UserIdentity<string>(authorizatorMock.Object, "100");

            Assert.AreEqual("100", user.Id);
            Assert.IsTrue(user.IsAuthenticated);
            Assert.IsFalse(user.IsAllowed(SecondPermissions.Edit));
            Assert.IsFalse(user.IsAllowed(new SecondResource("123"), SecondPermissions.Edit));
            Assert.IsFalse(user.IsAllowed(typeof(SecondResource), "123", SecondPermissions.Edit));
            //Does not have any permissions for resource
            Assert.AreEqual(0, user.GetAllowedKeys(typeof(SecondResource), SecondPermissions.Edit).Count());
        }

        [TestMethod]
        public void IsAllowedWithPermissionOnly_AtLeastOneRoleMustHavePermission()
        {
            var role1 = new UserRole(1);
            var role2 = new UserRole(2);
            var sequence = new List<(bool expected, bool role1, bool role2)>
            {
                (false, false, false),
                (true, true, false),
                (true, false, true),
                (true, true, true),
            };
            foreach (var valueTuple in sequence)
            {
                var authorizatorMock = new Mock<IAuthorizator<int>>();
                authorizatorMock.Setup(a => a.IsAllowed(role1, FirstPermissions.Edit))
                    .Returns(valueTuple.role1);
                authorizatorMock.Setup(a => a.IsAllowed(role2, FirstPermissions.Edit))
                    .Returns(valueTuple.role2);
                var user = new UserIdentity<int>(authorizatorMock.Object, 1, new[] { role1, role2 });

                //Act
                Assert.AreEqual(valueTuple.expected, user.IsAllowed(FirstPermissions.Edit));
            }
        }

        [TestMethod]
        public void IsAllowedWithResource_AtLeastOneRoleMustHavePermission()
        {
            var role1 = new UserRole(1);
            var role2 = new UserRole(2);
            var sequence = new List<(bool expected, bool role1, bool role2)>
            {
                (false, false, false),
                (true, true, false),
                (true, false, true),
                (true, true, true),
            };
            var resource = new FirstResource(1);
            foreach (var valueTuple in sequence)
            {
                var authorizatorMock = new Mock<IAuthorizator<int>>();
                authorizatorMock.Setup(a => a.IsAllowed(role1, resource, FirstPermissions.Edit))
                    .Returns(valueTuple.role1);
                authorizatorMock.Setup(a => a.IsAllowed(role2, resource, FirstPermissions.Edit))
                    .Returns(valueTuple.role2);
                var user = new UserIdentity<int>(authorizatorMock.Object, 1, new[] { role1, role2 });

                //Act
                Assert.AreEqual(valueTuple.expected, user.IsAllowed(resource, FirstPermissions.Edit));
            }
        }

        [TestMethod]
        public void IsAllowedWithResourceType_AtLeastOneRoleMustHavePermission()
        {
            var role1 = new UserRole(1);
            var role2 = new UserRole(2);
            var sequence = new List<(bool expected, bool role1, bool role2)>
            {
                (false, false, false),
                (true, true, false),
                (true, false, true),
                (true, true, true),
            };
            var resource = typeof(FirstResource);
            var resourceId = 1;
            foreach (var valueTuple in sequence)
            {
                var authorizatorMock = new Mock<IAuthorizator<int>>();
                authorizatorMock.Setup(a => a.IsAllowed(role1, resource, resourceId, FirstPermissions.Edit))
                    .Returns(valueTuple.role1);
                authorizatorMock.Setup(a => a.IsAllowed(role2, resource, resourceId, FirstPermissions.Edit))
                    .Returns(valueTuple.role2);
                var user = new UserIdentity<int>(authorizatorMock.Object, 1, new[] { role1, role2 });

                //Act
                Assert.AreEqual(valueTuple.expected, user.IsAllowed(resource, resourceId, FirstPermissions.Edit));
            }
        }

        [TestMethod]
        public void GetAllowed_ReturnDistinctOfIDs()
        {
            var role1 = new UserRole(1);
            var role2 = new UserRole(2);

            var resource = typeof(FirstResource);
            var authorizatorMock = new Mock<IAuthorizator<int>>();
            authorizatorMock.Setup(a => a.GetAllowedKeys(role1, resource, FirstPermissions.Edit))
                .Returns(new List<int> { 1, 2 });
            authorizatorMock.Setup(a => a.GetAllowedKeys(role2, resource, FirstPermissions.Edit))
                .Returns(new List<int> { 2, 3 });
            var user = new UserIdentity<int>(authorizatorMock.Object, 1, new[] { role1, role2 });

            //Act
            var result = user.GetAllowedKeys(resource, FirstPermissions.Edit).ToList();
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, result);

        }
    }
}
