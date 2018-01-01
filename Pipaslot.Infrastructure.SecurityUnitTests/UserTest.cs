using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.Infrastructure.SecurityTests.Models;

namespace Pipaslot.Infrastructure.SecurityTests
{
    [TestClass]
    public class UserTest
    {
        private readonly UserRole GuestRole = new UserRole(1000);

        private readonly List<UserRole> TwoRoles = new List<UserRole>()
        {
            new UserRole(1),
            new UserRole(2)
        };
        private IEnumerable<int> TwoRolesIds => TwoRoles.Select(r => r.Id).ToList();

        #region Constructor

        [TestMethod]
        public void Constructor_MinimuParameters_UserIsUnauthorizedAndWithoutRolesByDefaultAndWithoutPermissions()
        {
            var authorizatorMock = new Mock<IAuthorizator<int>>();
            var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
            var user = new User<int>(authorizatorMock.Object, new UserIdentity(GuestRole), resourceInstanceQueryFactory.Object);

            Assert.AreEqual(0, user.Id);
            Assert.IsFalse(user.IsAuthenticated);
            Assert.IsFalse(user.IsAllowedAsync(FirstPermissions.Edit).Result);
            Assert.IsFalse(user.IsAllowedAsync(new FirstResource(1), FirstPermissions.Edit).Result);
            Assert.IsFalse(user.IsAllowedAsync(typeof(FirstResource), 1, FirstPermissions.Edit).Result);
            //Does not have any permissions for resourceInstance
            Assert.AreEqual(0, user.GetAllowedKeysAsync(typeof(FirstResource), FirstPermissions.Edit).Result.Count());
        }

        [TestMethod]
        public void Constructor_IntGenericType_UserIsAuthenticatedIfIdentityproviderReturnsUserIdentityButNotGuestIdentity()
        {
            var authorizatorMock = new Mock<IAuthorizator<int>>();
            var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
            var user = new User<int>(authorizatorMock.Object, new UserIdentity(100, TwoRoles), resourceInstanceQueryFactory.Object);

            Assert.AreEqual(100, user.Id);
            Assert.IsTrue(user.IsAuthenticated);
            Assert.IsFalse(user.IsAllowedAsync(FirstPermissions.Edit).Result);
            Assert.IsFalse(user.IsAllowedAsync(new FirstResource(1), FirstPermissions.Edit).Result);
            Assert.IsFalse(user.IsAllowedAsync(typeof(FirstResource), 1, FirstPermissions.Edit).Result);
            //Does not have any permissions for resourceInstance
            Assert.AreEqual(0, user.GetAllowedKeysAsync(typeof(FirstResource), FirstPermissions.Edit).Result.Count());
        }

        [TestMethod]
        public void Constructor_StringGenericType_UserIsAuthenticatedIfIdentityproviderReturnsUserIdentityButNotGuestIdentity()
        {
            var authorizatorMock = new Mock<IAuthorizator<string>>();
            var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
            var user = new User<string>(authorizatorMock.Object, new UserIdentity("100", new[] { new UserRoleString("123") }), resourceInstanceQueryFactory.Object);

            Assert.AreEqual("100", user.Id);
            Assert.IsTrue(user.IsAuthenticated);
            Assert.IsFalse(user.IsAllowedAsync(SecondPermissions.Edit).Result);
            Assert.IsFalse(user.IsAllowedAsync(new SecondResource("123"), SecondPermissions.Edit).Result);
            Assert.IsFalse(user.IsAllowedAsync(typeof(SecondResource), "123", SecondPermissions.Edit).Result);
            //Does not have any permissions for resourceInstance
            Assert.AreEqual(0, user.GetAllowedKeysAsync(typeof(SecondResource), SecondPermissions.Edit).Result.Count());
        }

        #endregion

        #region IAllowed

        [TestMethod]
        public void IsAllowedWithPermissionOnly_AtLeastOneRoleMustHavePermission()
        {
            var sequence = new List<(bool expected, bool role1, bool role2)>
            {
                (false, false, false),
                (true, true, false),
                (true, false, true),
                (true, true, true),
            };
            var tokenSource = new CancellationTokenSource();
            foreach (var valueTuple in sequence)
            {
                var authorizatorMock = new Mock<IAuthorizator<int>>();
                authorizatorMock.Setup(a => a.IsAllowedAsync(TwoRolesIds, FirstPermissions.Edit, tokenSource.Token))
                    .Returns(Task.FromResult(valueTuple.role1 | valueTuple.role2));
                var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
                var user = new User<int>(authorizatorMock.Object, new UserIdentity(1, TwoRoles), resourceInstanceQueryFactory.Object);

                //Act
                Assert.AreEqual(valueTuple.expected, user.IsAllowedAsync(FirstPermissions.Edit, tokenSource.Token).Result);
            }
        }

        [TestMethod]
        public void IsAllowed_WithStaticResourceType_AtLeastOneRoleMustHavePermission()
        {
            var sequence = new List<(bool expected, bool role1, bool role2)>
            {
                (false, false, false),
                (true, true, false),
                (true, false, true),
                (true, true, true),
            };
            var resource = typeof(FirstResource);
            var tokenSource = new CancellationTokenSource();
            foreach (var valueTuple in sequence)
            {
                var authorizatorMock = new Mock<IAuthorizator<int>>();
                authorizatorMock.Setup(a => a.IsAllowedAsync(TwoRolesIds, resource, FirstPermissions.Edit, tokenSource.Token))
                    .Returns(Task.FromResult(valueTuple.role1 | valueTuple.role2));
                var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
                var user = new User<int>(authorizatorMock.Object, new UserIdentity(1, TwoRoles), resourceInstanceQueryFactory.Object);

                //Act
                Assert.AreEqual(valueTuple.expected, user.IsAllowedAsync(resource, FirstPermissions.Edit, tokenSource.Token).Result);
            }
        }

        [TestMethod]
        public void IsAllowed_WithResource_AtLeastOneRoleMustHavePermission()
        {
            var sequence = new List<(bool expected, bool role1, bool role2)>
            {
                (false, false, false),
                (true, true, false),
                (true, false, true),
                (true, true, true),
            };
            var resource = new FirstResource(1);
            var tokenSource = new CancellationTokenSource();
            foreach (var valueTuple in sequence)
            {
                var authorizatorMock = new Mock<IAuthorizator<int>>();
                authorizatorMock.Setup(a => a.IsAllowedAsync(TwoRolesIds, resource, FirstPermissions.Edit, tokenSource.Token))
                    .Returns(Task.FromResult(valueTuple.role1 | valueTuple.role2));
                var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
                var user = new User<int>(authorizatorMock.Object, new UserIdentity(1, TwoRoles), resourceInstanceQueryFactory.Object);

                //Act
                Assert.AreEqual(valueTuple.expected, user.IsAllowedAsync(resource, FirstPermissions.Edit, tokenSource.Token).Result);
            }
        }

        [TestMethod]
        public void IsAllowed_WithResourceType_AtLeastOneRoleMustHavePermission()
        {
            var sequence = new List<(bool expected, bool role1, bool role2)>
            {
                (false, false, false),
                (true, true, false),
                (true, false, true),
                (true, true, true),
            };
            var resource = typeof(FirstResource);
            var resourceId = 1;
            var tokenSource = new CancellationTokenSource();
            foreach (var valueTuple in sequence)
            {
                var authorizatorMock = new Mock<IAuthorizator<int>>();
                authorizatorMock.Setup(a => a.IsAllowedAsync(TwoRolesIds, resource, resourceId, FirstPermissions.Edit, tokenSource.Token))
                    .Returns(Task.FromResult(valueTuple.role1 | valueTuple.role2));
                var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
                var user = new User<int>(authorizatorMock.Object, new UserIdentity(1, TwoRoles), resourceInstanceQueryFactory.Object);

                //Act
                Assert.AreEqual(valueTuple.expected, user.IsAllowedAsync(resource, resourceId, FirstPermissions.Edit, tokenSource.Token).Result);
            }
        }

        [TestMethod]
        public void IsAllowed_AdminIdentityHasAllPermissions()
        {
            var authorizatorMock = new Mock<IAuthorizator<int>>();
            var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
            var adminRole = new UserRole(1)
            {
                Type = RoleType.Admin
            };
            var user = new User<int>(authorizatorMock.Object, new UserIdentity(1, new[] { adminRole }), resourceInstanceQueryFactory.Object);

            //Act
            Assert.IsTrue(user.IsAllowedAsync(FirstPermissions.Edit).Result);
            Assert.IsTrue(user.IsAllowedAsync(typeof(FirstResource), FirstPermissions.Edit).Result);
            Assert.IsTrue(user.IsAllowedAsync(typeof(FirstResource), 1, FirstPermissions.Edit).Result);
            Assert.IsTrue(user.IsAllowedAsync(new FirstResource(1), FirstPermissions.Edit).Result);
        }

        #endregion

        [TestMethod]
        public void GetAllowed_ReturnDistinctOfIDs()
        {
            var resource = typeof(FirstResource);
            var authorizatorMock = new Mock<IAuthorizator<int>>();
            var tokenSource = new CancellationTokenSource();
            authorizatorMock.Setup(a => a.GetAllowedKeysAsync(TwoRolesIds, resource, FirstPermissions.Edit, tokenSource.Token))
                .Returns(Task.FromResult((new List<int> { 1, 2, 3 }).AsEnumerable()));
            var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
            var user = new User<int>(authorizatorMock.Object, new UserIdentity(1, TwoRoles), resourceInstanceQueryFactory.Object);

            //Act
            var result = user.GetAllowedKeysAsync(resource, FirstPermissions.Edit, tokenSource.Token).Result.ToList();
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, result);

        }

        #region CheckPermission

        public async Task CheckPermission_WithPermissionOnly_AtLeastOneRoleMustHavePermission()
        {
            var sequence = new List<(bool expected, bool role1, bool role2)>
            {
                (false, false, false),
                (true, true, false),
                (true, false, true),
                (true, true, true),
            };
            var tokenSource = new CancellationTokenSource();
            foreach (var valueTuple in sequence)
            {
                var authorizatorMock = new Mock<IAuthorizator<int>>();
                authorizatorMock.Setup(a => a.IsAllowedAsync(new[] { TwoRolesIds.First() }, FirstPermissions.Edit, tokenSource.Token))
                    .Returns(Task.FromResult(valueTuple.role1));
                authorizatorMock.Setup(a => a.IsAllowedAsync(new[] { TwoRolesIds.Last() }, FirstPermissions.Edit, tokenSource.Token))
                    .Returns(Task.FromResult(valueTuple.role2));
                var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
                var user = new User<int>(authorizatorMock.Object, new UserIdentity(1, TwoRoles), resourceInstanceQueryFactory.Object);

                //Act
                if (valueTuple.expected)
                {
                    //Shouldn't throw an exception
                    await user.CheckPermissionAsync(FirstPermissions.Edit, tokenSource.Token);
                }
                else
                {
                    //Should thrown an exception
                    try
                    {
                        await user.CheckPermissionAsync(FirstPermissions.Edit, tokenSource.Token);
                        Assert.Fail();
                    }
                    catch (AuthorizationException) { }
                }
            }
        }

        public async Task CheckPermission_WithStaticResourceType_AtLeastOneRoleMustHavePermission()
        {
            var sequence = new List<(bool expected, bool role1, bool role2)>
            {
                (false, false, false),
                (true, true, false),
                (true, false, true),
                (true, true, true),
            };
            var resource = typeof(FirstResource);
            var tokenSource = new CancellationTokenSource();
            foreach (var valueTuple in sequence)
            {
                var authorizatorMock = new Mock<IAuthorizator<int>>();
                authorizatorMock.Setup(a => a.IsAllowedAsync(new[] { TwoRolesIds.First() }, resource, FirstPermissions.Edit, tokenSource.Token))
                    .Returns(Task.FromResult(valueTuple.role1));
                authorizatorMock.Setup(a => a.IsAllowedAsync(new[] { TwoRolesIds.Last() }, resource, FirstPermissions.Edit, tokenSource.Token))
                    .Returns(Task.FromResult(valueTuple.role2));
                var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
                var user = new User<int>(authorizatorMock.Object, new UserIdentity(1, TwoRoles), resourceInstanceQueryFactory.Object);

                //Act
                if (valueTuple.expected)
                {
                    //Shouldn't throw an exception
                    await user.CheckPermissionAsync(resource, FirstPermissions.Edit, tokenSource.Token);
                }
                else
                {
                    //Should thrown an exception
                    try
                    {
                        await user.CheckPermissionAsync(resource, FirstPermissions.Edit, tokenSource.Token);
                        Assert.Fail();
                    }
                    catch (AuthorizationException) { }
                }
            }
        }

        [TestMethod]
        public async Task CheckPermission_WithResource_AtLeastOneRoleMustHavePermission()
        {
            var sequence = new List<(bool expected, bool role1, bool role2)>
            {
                (false, false, false),
                (true, true, false),
                (true, false, true),
                (true, true, true),
            };
            var resource = new FirstResource(1);
            var tokenSource = new CancellationTokenSource();
            foreach (var valueTuple in sequence)
            {
                var authorizatorMock = new Mock<IAuthorizator<int>>();
                authorizatorMock.Setup(a => a.IsAllowedAsync(TwoRolesIds, resource, FirstPermissions.Edit, tokenSource.Token))
                    .Returns(Task.FromResult(valueTuple.role1 | valueTuple.role2));
                var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
                var user = new User<int>(authorizatorMock.Object, new UserIdentity(1, TwoRoles), resourceInstanceQueryFactory.Object);

                //Act
                if (valueTuple.expected)
                {
                    //Shouldn't throw an exception
                    await user.CheckPermissionAsync(resource, FirstPermissions.Edit, tokenSource.Token);
                }
                else
                {
                    //Should thrown an exception
                    try
                    {
                        await user.CheckPermissionAsync(resource, FirstPermissions.Edit, tokenSource.Token);
                        Assert.Fail();
                    }
                    catch (AuthorizationException) { }
                }
            }
        }

        [TestMethod]
        public async Task CheckPermission_WithResourceType_AtLeastOneRoleMustHavePermission()
        {
            var sequence = new List<(bool expected, bool role1, bool role2)>
            {
                (false, false, false),
                (true, true, false),
                (true, false, true),
                (true, true, true),
            };
            var resource = typeof(FirstResource);
            var resourceId = 1;
            var tokenSource = new CancellationTokenSource();
            foreach (var valueTuple in sequence)
            {
                var authorizatorMock = new Mock<IAuthorizator<int>>();
                authorizatorMock.Setup(a => a.IsAllowedAsync(TwoRolesIds, resource, resourceId, FirstPermissions.Edit, tokenSource.Token))
                    .Returns(Task.FromResult(valueTuple.role1 | valueTuple.role2));
                var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
                var user = new User<int>(authorizatorMock.Object, new UserIdentity(1, TwoRoles), resourceInstanceQueryFactory.Object);

                //Act
                if (valueTuple.expected)
                {
                    //Shouldn't throw an exception
                    await user.CheckPermissionAsync(resource, resourceId, FirstPermissions.Edit, tokenSource.Token);
                }
                else
                {
                    //Should thrown an exception
                    try
                    {
                        await user.CheckPermissionAsync(resource, resourceId, FirstPermissions.Edit, tokenSource.Token);
                        Assert.Fail();
                    }
                    catch (AuthorizationException) { }
                }
            }
        }

        #endregion
    }
}
