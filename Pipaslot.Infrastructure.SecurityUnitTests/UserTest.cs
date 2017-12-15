using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Identities;
using Pipaslot.Infrastructure.SecurityTests.Mockups;
using Pipaslot.Infrastructure.SecurityTests.Models;

namespace Pipaslot.Infrastructure.SecurityTests
{
    [TestClass]
    public class UserTest
    {
        #region Constructor

        [TestMethod]
        public void Constructor_MinimuParameters_UserIsUnauthorizedAndWithoutRolesByDefaultAndWithoutPermissions()
        {
            var authorizatorMock = new Mock<IAuthorizator<int>>();
            var user = new User<int>(authorizatorMock.Object, new GuestIdentity<int>());

            Assert.AreEqual(0, user.Id);
            Assert.IsFalse(user.IsAuthenticated);
            Assert.IsFalse(user.IsAllowed(FirstPermissions.Edit));
            Assert.IsFalse(user.IsAllowed(new FirstResource(1), FirstPermissions.Edit));
            Assert.IsFalse(user.IsAllowed(typeof(FirstResource), 1, FirstPermissions.Edit));
            //Does not have any permissions for resourceInstance
            Assert.AreEqual(0, user.GetAllowedKeys(typeof(FirstResource), FirstPermissions.Edit).Count());
        }

        [TestMethod]
        public void Constructor_IntGenericType_UserIsAuthenticatedIfIdentityproviderReturnsUserIdentityButNotGuestIdentity()
        {
            var authorizatorMock = new Mock<IAuthorizator<int>>();
            var user = new User<int>(authorizatorMock.Object, new UserIdentity<int>(100));

            Assert.AreEqual(100, user.Id);
            Assert.IsTrue(user.IsAuthenticated);
            Assert.IsFalse(user.IsAllowed(FirstPermissions.Edit));
            Assert.IsFalse(user.IsAllowed(new FirstResource(1), FirstPermissions.Edit));
            Assert.IsFalse(user.IsAllowed(typeof(FirstResource), 1, FirstPermissions.Edit));
            //Does not have any permissions for resourceInstance
            Assert.AreEqual(0, user.GetAllowedKeys(typeof(FirstResource), FirstPermissions.Edit).Count());
        }

        [TestMethod]
        public void Constructor_StringGenericType_UserIsAuthenticatedIfIdentityproviderReturnsUserIdentityButNotGuestIdentity()
        {
            var authorizatorMock = new Mock<IAuthorizator<string>>();
            var user = new User<string>(authorizatorMock.Object, new UserIdentity<string>("100"));

            Assert.AreEqual("100", user.Id);
            Assert.IsTrue(user.IsAuthenticated);
            Assert.IsFalse(user.IsAllowed(SecondPermissions.Edit));
            Assert.IsFalse(user.IsAllowed(new SecondResource("123"), SecondPermissions.Edit));
            Assert.IsFalse(user.IsAllowed(typeof(SecondResource), "123", SecondPermissions.Edit));
            //Does not have any permissions for resourceInstance
            Assert.AreEqual(0, user.GetAllowedKeys(typeof(SecondResource), SecondPermissions.Edit).Count());
        }

        #endregion

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
                var user = new User<int>(authorizatorMock.Object,  new GuestIdentity<int>(1, new[] { role1, role2 }));

                //Act
                Assert.AreEqual(valueTuple.expected, user.IsAllowed(FirstPermissions.Edit));
            }
        }

        [TestMethod]
        public void IsAllowedWithStaticResourceType_AtLeastOneRoleMustHavePermission()
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
            foreach (var valueTuple in sequence)
            {
                var authorizatorMock = new Mock<IAuthorizator<int>>();
                authorizatorMock.Setup(a => a.IsAllowed(role1, resource, FirstPermissions.Edit))
                    .Returns(valueTuple.role1);
                authorizatorMock.Setup(a => a.IsAllowed(role2, resource, FirstPermissions.Edit))
                    .Returns(valueTuple.role2);
                var user = new User<int>(authorizatorMock.Object, new GuestIdentity<int>(1, new[] { role1, role2 }));

                //Act
                Assert.AreEqual(valueTuple.expected, user.IsAllowed(resource,  FirstPermissions.Edit));
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
                var user = new User<int>(authorizatorMock.Object, new GuestIdentity<int>(1, new[] { role1, role2 }));

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
                var user = new User<int>(authorizatorMock.Object, new GuestIdentity<int>(1, new[] { role1, role2 }));

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
            var user = new User<int>(authorizatorMock.Object, new GuestIdentity<int>(1, new[] { role1, role2 }));

            //Act
            var result = user.GetAllowedKeys(resource, FirstPermissions.Edit).ToList();
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, result);

        }

        [TestMethod]
        public void IsAllowed_AdminIdentityHasAllPermissions()
        {
            var authorizatorMock = new Mock<IAuthorizator<int>>();
            var user = new User<int>(authorizatorMock.Object, new AdminIdentity<int>(1));

            //Act
            Assert.IsTrue(user.IsAllowed(FirstPermissions.Edit));
            Assert.IsTrue(user.IsAllowed(typeof(FirstResource), FirstPermissions.Edit));
            Assert.IsTrue(user.IsAllowed(typeof(FirstResource), 1, FirstPermissions.Edit));
            Assert.IsTrue(user.IsAllowed(new FirstResource(1), FirstPermissions.Edit));
        }
    }
}
