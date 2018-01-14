using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.Infrastructure.Security.Exceptions;
using Pipaslot.Infrastructure.SecurityTests.Models;

namespace Pipaslot.Infrastructure.SecurityTests
{
    [TestClass]
    public class UserTest
    {
        private readonly IRole GuestRole = new Role
        {
            Id = 3,
            Name = "Guest",
            Type = RoleType.Guest
        };
        private readonly IRole UserRole = new Role
        {
            Id = 4,
            Name = "User",
            Type = RoleType.User
        };

        private readonly List<IRole> TwoRoles = new List<IRole>()
        {
            new Role
            {
                    Id = 1,
                    Type = RoleType.User
            },
            new Role
            {
                Id = 2,
                Type = RoleType.User
            }
        };
        private IEnumerable<int> TwoRolesIds
        {
            get
            {
                var list = TwoRoles.Select(r => (int)r.Id).ToList();
                list.Add((int)GuestRole.Id);
                list.Add((int)UserRole.Id);
                return list;
            }
        }

        #region Constructor

        [TestMethod]
        public void Constructor_MinimuParameters_UserIsUnauthorizedAndWithoutRolesByDefaultAndWithoutPermissions()
        {
            var authorizatorMock = new Mock<IPermissionManager<int>>();
            var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
            var user = new User<int>(authorizatorMock.Object, GetClaimsPrincipalProvider(), resourceInstanceQueryFactory.Object, GetRoleStore());

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
            var authorizatorMock = new Mock<IPermissionManager<int>>();
            var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
            var user = new User<int>(authorizatorMock.Object, GetClaimsPrincipalProvider("100", TwoRoles), resourceInstanceQueryFactory.Object, GetRoleStore());

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
            var store = new Mock<IRoleStore>();
            store.Setup(s => s.GetSystemRoles<IRole>()).Returns(new List<IRole>()
            {
                new Role
                {
                    Id = "3",
                    Name = "Guest",
                    Type = RoleType.Guest
                },
                new Role
                {
                    Id = "4",
                    Name = "User",
                    Type = RoleType.User
                 },
                new Role
                {
                    Id = "5",
                    Name = "Admin",
                    Type = RoleType.Admin
                }
            });
            var authorizatorMock = new Mock<IPermissionManager<string>>();
            var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
            var user = new User<string>(authorizatorMock.Object, GetClaimsPrincipalProvider("100", new[] { new Role { Id = "1" } }), resourceInstanceQueryFactory.Object, store.Object);

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
                var authorizatorMock = new Mock<IPermissionManager<int>>();
                authorizatorMock.Setup(a => a.IsAllowedAsync(TwoRolesIds, FirstPermissions.Edit, tokenSource.Token))
                    .Returns(Task.FromResult(valueTuple.role1 | valueTuple.role2));
                var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
                var user = new User<int>(authorizatorMock.Object, GetClaimsPrincipalProvider("1", TwoRoles), resourceInstanceQueryFactory.Object, GetRoleStore());

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
                var authorizatorMock = new Mock<IPermissionManager<int>>();
                authorizatorMock.Setup(a => a.IsAllowedAsync(TwoRolesIds, resource, FirstPermissions.Edit, tokenSource.Token))
                    .Returns(Task.FromResult(valueTuple.role1 | valueTuple.role2));
                var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
                var user = new User<int>(authorizatorMock.Object, GetClaimsPrincipalProvider("1", TwoRoles), resourceInstanceQueryFactory.Object, GetRoleStore());

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
                var authorizatorMock = new Mock<IPermissionManager<int>>();
                authorizatorMock.Setup(a => a.IsAllowedAsync(TwoRolesIds, resource, FirstPermissions.Edit, tokenSource.Token))
                    .Returns(Task.FromResult(valueTuple.role1 | valueTuple.role2));
                var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
                var user = new User<int>(authorizatorMock.Object, GetClaimsPrincipalProvider("1", TwoRoles), resourceInstanceQueryFactory.Object, GetRoleStore());

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
                var authorizatorMock = new Mock<IPermissionManager<int>>();
                authorizatorMock.Setup(a => a.IsAllowedAsync(TwoRolesIds, resource, resourceId, FirstPermissions.Edit, tokenSource.Token))
                    .Returns(Task.FromResult(valueTuple.role1 | valueTuple.role2));
                var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
                var user = new User<int>(authorizatorMock.Object, GetClaimsPrincipalProvider("1", TwoRoles), resourceInstanceQueryFactory.Object, GetRoleStore());

                //Act
                Assert.AreEqual(valueTuple.expected, user.IsAllowedAsync(resource, resourceId, FirstPermissions.Edit, tokenSource.Token).Result);
            }
        }

        [TestMethod]
        public void IsAllowed_AdminIdentityHasAllPermissions()
        {
            var authorizatorMock = new Mock<IPermissionManager<int>>();
            var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
            var adminRole = new Role { Type = RoleType.Admin };
            var user = new User<int>(authorizatorMock.Object, GetClaimsPrincipalProvider("1", new[] { adminRole }), resourceInstanceQueryFactory.Object, GetRoleStore());

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
            var authorizatorMock = new Mock<IPermissionManager<int>>();
            var tokenSource = new CancellationTokenSource();
            authorizatorMock.Setup(a => a.GetAllowedKeysAsync(TwoRolesIds, resource, FirstPermissions.Edit, tokenSource.Token))
                .Returns(Task.FromResult((new List<int> { 1, 2, 3 }).AsEnumerable()));
            var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
            var user = new User<int>(authorizatorMock.Object, GetClaimsPrincipalProvider("1", TwoRoles), resourceInstanceQueryFactory.Object, GetRoleStore());

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
                var authorizatorMock = new Mock<IPermissionManager<int>>();
                authorizatorMock.Setup(a => a.IsAllowedAsync(new[] { TwoRolesIds.First() }, FirstPermissions.Edit, tokenSource.Token))
                    .Returns(Task.FromResult(valueTuple.role1));
                authorizatorMock.Setup(a => a.IsAllowedAsync(new[] { TwoRolesIds.Last() }, FirstPermissions.Edit, tokenSource.Token))
                    .Returns(Task.FromResult(valueTuple.role2));
                var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
                var user = new User<int>(authorizatorMock.Object, GetClaimsPrincipalProvider("1", TwoRoles), resourceInstanceQueryFactory.Object, GetRoleStore());

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
                var authorizatorMock = new Mock<IPermissionManager<int>>();
                authorizatorMock.Setup(a => a.IsAllowedAsync(new[] { TwoRolesIds.First() }, resource, FirstPermissions.Edit, tokenSource.Token))
                    .Returns(Task.FromResult(valueTuple.role1));
                authorizatorMock.Setup(a => a.IsAllowedAsync(new[] { TwoRolesIds.Last() }, resource, FirstPermissions.Edit, tokenSource.Token))
                    .Returns(Task.FromResult(valueTuple.role2));
                var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
                var user = new User<int>(authorizatorMock.Object, GetClaimsPrincipalProvider("1", TwoRoles), resourceInstanceQueryFactory.Object, GetRoleStore());

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
                var authorizatorMock = new Mock<IPermissionManager<int>>();
                authorizatorMock.Setup(a => a.IsAllowedAsync(TwoRolesIds, resource, FirstPermissions.Edit, tokenSource.Token))
                    .Returns(Task.FromResult(valueTuple.role1 | valueTuple.role2));
                var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
                var user = new User<int>(authorizatorMock.Object, GetClaimsPrincipalProvider("1", TwoRoles), resourceInstanceQueryFactory.Object, GetRoleStore());

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
                var authorizatorMock = new Mock<IPermissionManager<int>>();
                authorizatorMock.Setup(a => a.IsAllowedAsync(TwoRolesIds, resource, resourceId, FirstPermissions.Edit, tokenSource.Token))
                    .Returns(Task.FromResult(valueTuple.role1 | valueTuple.role2));
                var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
                var user = new User<int>(authorizatorMock.Object, GetClaimsPrincipalProvider("1", TwoRoles), resourceInstanceQueryFactory.Object, GetRoleStore());

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


        private IClaimsPrincipalProvider GetClaimsPrincipalProvider(string userId = null, IEnumerable<IRole> roles = null)
        {
            var claims = new List<Claim>();
            if (userId != null)
            {
                claims.Add(new Claim(ClaimTypes.Name, userId));
            }
            if (roles != null)
            {
                claims.AddRange(Helpers.RolesToClaims(roles));
            }
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            var context = new Mock<IClaimsPrincipalProvider>();
            context.Setup(c => c.GetClaimsPrincipal()).Returns(user);

            return context.Object;
        }

        private IRoleStore GetRoleStore()
        {
            var store = new Mock<IRoleStore>();
            store.Setup(s => s.GetSystemRoles<IRole>()).Returns(new List<IRole>()
            {
               GuestRole,
               UserRole,
                new Role
                {
                    Name = "Admin",
                    Type = RoleType.Admin
                }
            });
            return store.Object;
        }

    }
}
