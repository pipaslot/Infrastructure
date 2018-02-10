using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Data;
using Pipaslot.Infrastructure.Security.Exceptions;
using Pipaslot.Infrastructure.Security.Tests.Models;

namespace Pipaslot.Infrastructure.Security.Tests
{
    [TestClass]
    public class PermissionManagerTest
    {
        [TestMethod]
        [ExpectedException(typeof(NotSuitablePermissionException))]
        public void Allow_PermissionIsNotAllowedForResource_ShouldFail()
        {
            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var resourceRegistry = GetRegistry();
            var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
            var namigConvertor = new DefaultNamingConvertor(resourceRegistry);
            var manager = new PermissionManager<int>(permissionStore.Object, resourceRegistry, resourceInstanceQueryFactory.Object, new DefaultNamingConvertor(resourceRegistry));

            //Act
            manager.SetPermission(1, typeof(FirstResource).FullName, default(int), namigConvertor.GetPermissionUniqueIdentifier(SecondPermissions.Edit), true);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSuitablePermissionException))]
        public void Deny_PermissionIsNotAllowedForResource_ShouldFail()
        {
            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var resourceRegistry = new ResourceRegistry();
            resourceRegistry.Register(GetType().Assembly);
            var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();
            var namigConvertor = new DefaultNamingConvertor(resourceRegistry);
            var manager = new PermissionManager<int>(permissionStore.Object, resourceRegistry, resourceInstanceQueryFactory.Object, new DefaultNamingConvertor(resourceRegistry));

            //Act
            manager.SetPermission(1, typeof(FirstResource).FullName, default(int), namigConvertor.GetPermissionUniqueIdentifier(SecondPermissions.Edit), false);
        }

        [TestMethod]
        public void GetAllResources_LoadNameAndDescriptionFromObjectAnnotation()
        {
            var instanceCount = 3;
            var resourceType = typeof(FirstResource);
            var permissionStore = new Mock<IPermissionStore<int>>();
            var tokenSource = new CancellationTokenSource();
            var resourceRegistry = new ResourceRegistry();
            resourceRegistry
                .Register(resourceType.Assembly);
            var resourceInstanceProvider = new Mock<IResourceInstanceProvider>();
            resourceInstanceProvider
                .Setup(q => q.GetInstanceCountAsync(resourceType, tokenSource.Token))
                .Returns(Task.FromResult(instanceCount));
            var manager = new PermissionManager<int>(permissionStore.Object, resourceRegistry, resourceInstanceProvider.Object, new DefaultNamingConvertor(resourceRegistry));

            var resources = manager.GetAllResourcesAsync(tokenSource.Token).Result;
            var firstResource = resources.First(r => r.UniqueName == resourceType.FullName);
            Assert.IsFalse(string.IsNullOrWhiteSpace(firstResource.Name));
            Assert.IsFalse(string.IsNullOrWhiteSpace(firstResource.Description));
            Assert.AreEqual(instanceCount, firstResource.InstancesCount);
        }

        [TestMethod]
        public void GetAllResourceInstancess_LoadNameAndDescriptionFromResourceDetailProvider()
        {
            var resourceId = 5;
            var resourceType = typeof(FirstResource);
            var providerResult = new FirstResource(resourceId, "NAME", "DESCRIPTION");
            var queryResult = new List<IResourceInstance> { providerResult };
            var tokenSource = new CancellationTokenSource();

            var permissionStoreMock = new Mock<IPermissionStore<int>>();

            var resourceRegistry = new ResourceRegistry();
            resourceRegistry.Register(resourceType.Assembly);

            var resourceInstanceProvider = new Mock<IResourceInstanceProvider>();
            resourceInstanceProvider
                .Setup(q => q.GetInstancesAsync(resourceType, 1, 10, tokenSource.Token))
                .Returns(Task.FromResult(queryResult));

            //Act
            var manager = new PermissionManager<int>(permissionStoreMock.Object, resourceRegistry, resourceInstanceProvider.Object, new DefaultNamingConvertor(resourceRegistry));
            var resources = manager.GetAllResourceInstancesAsync(resourceType.FullName, 1, 10, tokenSource.Token).Result;
            var firstResource = resources.First();

            //Assert
            Assert.AreEqual(resourceId, firstResource.Identifier);
            Assert.AreEqual(providerResult.Name, firstResource.Name);
            Assert.AreEqual(providerResult.Description, firstResource.Description);
        }

        [TestMethod]
        public void GetAllPermissionsForStaticResource_LoadAllPermissionWithAllowedAccess()
        {
            GetAllPermissionsForStaticResource_LoadAllPermission(true);
        }
        [TestMethod]
        public void GetAllPermissionsForStaticResource_LoadAllPermissionWithDisabledAccess()
        {
            GetAllPermissionsForStaticResource_LoadAllPermission(false);
        }

        private void GetAllPermissionsForStaticResource_LoadAllPermission(bool isAllowed)
        {
            var roleId = 5;
            var resourceType = typeof(FirstResource);
            var resourceName = typeof(FirstResource).FullName;

            var resourceRegistry = new ResourceRegistry();
            resourceRegistry.Register(resourceType.Assembly);
            var tokenSource = new CancellationTokenSource();

            var namingConvertor = new DefaultNamingConvertor(resourceRegistry);
            var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();

            var permissionStoreMock = new Mock<IPermissionStore<int>>();
            permissionStoreMock
                .Setup(p => p.IsAllowedAsync(roleId, resourceName, namingConvertor.GetPermissionUniqueIdentifier(StaticPermissions.Create), tokenSource.Token))
                .Returns(Task.FromResult((bool?)isAllowed));

            //Act
            var manager = new PermissionManager<int>(permissionStoreMock.Object, resourceRegistry, resourceInstanceQueryFactory.Object, new DefaultNamingConvertor(resourceRegistry));
            var permissions = manager.GetAllPermissionsAsync(roleId, resourceName, tokenSource.Token).Result.ToList();

            Assert.AreEqual(3, permissions.Count());

            var create = permissions.First();
            Assert.AreEqual(namingConvertor.GetPermissionUniqueIdentifier(StaticPermissions.Create), create.UniqueIdentifier);
            Assert.IsFalse(string.IsNullOrWhiteSpace(create.Name));
            Assert.IsFalse(string.IsNullOrWhiteSpace(create.Description));
            Assert.AreEqual(isAllowed, create.IsAllowed);
        }

        [TestMethod]
        public void GetAllPermissionsForInstanceResource_LoadAllPermissionWithAllowedAccess()
        {
            GetAllPermissionsForInstanceResource_LoadAllPermission(true);
        }
        [TestMethod]
        public void GetAllPermissionsForInstanceResource_LoadAllPermissionWithDisabledAccess()
        {
            GetAllPermissionsForInstanceResource_LoadAllPermission(false);
        }

        private void GetAllPermissionsForInstanceResource_LoadAllPermission(bool isAllowed)
        {
            var roleId = 5;
            var resourceType = typeof(FirstResource);
            var resourceName = typeof(FirstResource).FullName;
            var resourceId = 1;


            var resourceRegistry = new ResourceRegistry();
            resourceRegistry.Register(resourceType.Assembly);
            var tokenSource = new CancellationTokenSource();

            var namingConvertor = new DefaultNamingConvertor(resourceRegistry);
            var resourceInstanceQueryFactory = new Mock<IResourceInstanceProvider>();

            var permissionStoreMock = new Mock<IPermissionStore<int>>();
            permissionStoreMock
                .Setup(p => p.IsAllowedAsync(roleId, resourceName, resourceId, namingConvertor.GetPermissionUniqueIdentifier(FirstPermissions.Edit), tokenSource.Token))
                .Returns(Task.FromResult((bool?)isAllowed));

            //Act
            var manager = new PermissionManager<int>(permissionStoreMock.Object, resourceRegistry, resourceInstanceQueryFactory.Object, new DefaultNamingConvertor(resourceRegistry));
            var permissions = manager.GetAllPermissionsAsync(roleId, resourceName, resourceId, tokenSource.Token).Result.ToList();

            Assert.AreEqual(2, permissions.Count());

            var edit = permissions.First();
            Assert.AreEqual(namingConvertor.GetPermissionUniqueIdentifier(FirstPermissions.Edit), edit.UniqueIdentifier);
            Assert.IsFalse(string.IsNullOrWhiteSpace(edit.Name));
            Assert.IsFalse(string.IsNullOrWhiteSpace(edit.Description));
            Assert.AreEqual(isAllowed, edit.IsAllowed);
        }

        #region IsAllowed methods

        [TestMethod]
        public async Task Constructor_UseCustomNamingConvertor()
        {
            const int roleId = 1;
            const int resourceId = 2;
            var permission = FirstPermissions.Edit;

            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var convertor = new ConstantNamingConvertor();
            var auth = new PermissionManager<int>(permissionStore.Object, GetRegistry(), new Mock<IResourceInstanceProvider>().Object, convertor);

            var resource = new FirstResource(resourceId);
            var tokenSource = new CancellationTokenSource();

            //Pre-Condition
            permissionStore.Setup(p => p.IsAllowedAsync(new[] { roleId }, ConstantNamingConvertor.RESOURCE, resourceId, ConstantNamingConvertor.PERMISSION, tokenSource.Token))
                .Returns(Task.FromResult((bool?)true));

            //Act
            await auth.IsAllowedAsync(new[] { roleId }, resource, permission, tokenSource.Token);

            //Assertion
            permissionStore.VerifyAll();
        }

        [TestMethod]
        public void GlobalPermission_ShouldPass()
        {
            const int roleId = 1;
            var permission = StaticPermissions.Create;

            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var convertor = new DefaultNamingConvertor(new ResourceRegistry());
            var auth = new PermissionManager<int>(permissionStore.Object, GetRegistry(), new Mock<IResourceInstanceProvider>().Object, convertor);

            var tokenSource = new CancellationTokenSource();

            //Pre-Condition
            permissionStore
                .Setup(p => p.IsAllowedAsync(new[] { roleId }, convertor.GetResourceUniqueName(typeof(FirstResource)), convertor.GetPermissionUniqueIdentifier(permission), tokenSource.Token))
                .Returns(Task.FromResult((bool?)true));

            //Act
            Assert.IsTrue(auth.IsAllowedAsync(new[] { roleId }, permission, tokenSource.Token).Result);

            //Assertion
            permissionStore.VerifyAll();
        }

        [TestMethod]
        public void StaticResourcePermission_ShouldPass()
        {
            const int roleId = 1;
            var permission = FirstPermissions.Edit;

            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var convertor = new DefaultNamingConvertor(new ResourceRegistry());
            var auth = new PermissionManager<int>(permissionStore.Object, GetRegistry(), new Mock<IResourceInstanceProvider>().Object, convertor);

            var tokenSource = new CancellationTokenSource();

            //Pre-Condition
            permissionStore
                .Setup(p => p.IsAllowedAsync(new[] { roleId }, convertor.GetResourceUniqueName(typeof(FirstResource)),
                    convertor.GetPermissionUniqueIdentifier(permission), tokenSource.Token))
                .Returns(Task.FromResult((bool?)true));

            //Act
            Assert.IsTrue(auth.IsAllowedAsync(new[] { roleId }, typeof(FirstResource), permission, tokenSource.Token).Result);

            //Assertion
            permissionStore.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSuitablePermissionException))]
        public async Task StaticResourcePermission_PermissionIsNotAllowedForResource_ShouldFail()
        {
            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var convertorMock = new Mock<INamingConvertor>();
            var auth = new PermissionManager<int>(permissionStore.Object, GetRegistry(), new Mock<IResourceInstanceProvider>().Object, convertorMock.Object);

            //Act
            await auth.IsAllowedAsync(new[] { 1 }, typeof(FirstResource), SecondPermissions.Edit);
        }

        [TestMethod]
        public void ResourceInstancePermission_ShouldPass()
        {
            const int roleId = 1;
            const int resourceId = 1;
            var permission = FirstPermissions.Edit;

            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var convertor = new DefaultNamingConvertor(new ResourceRegistry());
            var auth = new PermissionManager<int>(permissionStore.Object, GetRegistry(), new Mock<IResourceInstanceProvider>().Object, convertor);

            var resource = new FirstResource(resourceId);
            var tokenSource = new CancellationTokenSource();

            //Pre-Condition
            permissionStore
                .Setup(p => p.IsAllowedAsync(new[] { roleId }, convertor.GetResourceUniqueName(resource.GetType()), resourceId,
                    convertor.GetPermissionUniqueIdentifier(permission), tokenSource.Token))
                .Returns(Task.FromResult((bool?)true));

            //Act
            Assert.IsTrue(auth.IsAllowedAsync(new[] { roleId }, resource, permission, tokenSource.Token).Result);

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
            var convertor = new DefaultNamingConvertor(new ResourceRegistry());
            var auth = new PermissionManager<int>(permissionStore.Object, GetRegistry(), new Mock<IResourceInstanceProvider>().Object, convertor);

            var tokenSource = new CancellationTokenSource();

            //Pre-Condition
            permissionStore
                .Setup(p => p.IsAllowedAsync(new[] { roleId }, convertor.GetResourceUniqueName(typeof(FirstResource)), resourceId,
                    convertor.GetPermissionUniqueIdentifier(permission), tokenSource.Token))
                .Returns(Task.FromResult((bool?)true));

            //Act
            Assert.IsTrue(auth.IsAllowedAsync(new[] { roleId }, typeof(FirstResource), resourceId, permission, tokenSource.Token).Result);

            //Assertion
            permissionStore.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSuitablePermissionException))]
        public async Task ResourceTypeAndIdPermission_PermissionIsNotAllowedForResource_ShouldFail()
        {
            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var convertorMock = new Mock<INamingConvertor>();
            var auth = new PermissionManager<int>(permissionStore.Object, GetRegistry(), new Mock<IResourceInstanceProvider>().Object, convertorMock.Object);

            //Act
            await auth.IsAllowedAsync(new[] { 1 }, typeof(FirstResource), 1, SecondPermissions.Edit);
        }

        [TestMethod]
        public void GetAllowedKeys_ShouldPass()
        {
            const int roleId = 1;
            var permission = FirstPermissions.Edit;

            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var convertor = new DefaultNamingConvertor(new ResourceRegistry());
            var auth = new PermissionManager<int>(permissionStore.Object, GetRegistry(), new Mock<IResourceInstanceProvider>().Object, convertor);
            var allowed = new List<int> { 1, 3, 5 };
            var tokenSource = new CancellationTokenSource();

            //Pre-Condition
            permissionStore
                .Setup(p => p.GetAllowedResourceIdsAsync(new[] { roleId }, convertor.GetResourceUniqueName(typeof(FirstResource)),
                    convertor.GetPermissionUniqueIdentifier(permission), tokenSource.Token))
                .Returns(Task.FromResult(allowed.AsEnumerable()));

            //Act
            var allowedResult = auth.GetAllowedKeysAsync(new[] { roleId }, typeof(FirstResource), permission, tokenSource.Token).Result;
            Assert.AreEqual(allowed, allowedResult);

            //Assertion
            permissionStore.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSuitablePermissionException))]
        public async Task GetAllowedKeys_PermissionIsNotAllowedForResource_ShouldFail()
        {
            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var convertorMock = new Mock<INamingConvertor>();
            var auth = new PermissionManager<int>(permissionStore.Object, GetRegistry(), new Mock<IResourceInstanceProvider>().Object, convertorMock.Object);

            //Act
            await auth.GetAllowedKeysAsync(new[] { 1 }, typeof(FirstResource), SecondPermissions.Edit);
        }

        [TestMethod]
        public async Task IsAllowedCached()
        {
            const int roleId = 1;
            var permission = FirstPermissions.Edit;

            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var convertor = new DefaultNamingConvertor(new ResourceRegistry());
            var auth = new PermissionManager<int>(permissionStore.Object, GetRegistry(), new Mock<IResourceInstanceProvider>().Object, convertor);

            var tokenSource = new CancellationTokenSource();

            //Pre-Condition
            permissionStore
                .Setup(p => p.IsAllowedAsync(new[] { roleId }, convertor.GetResourceUniqueName(typeof(FirstResource)), convertor.GetPermissionUniqueIdentifier(permission), tokenSource.Token))
                .Returns(Task.FromResult((bool?)true));

            //Act
            await auth.IsAllowedAsync(new[] { roleId }, typeof(FirstResource), permission, tokenSource.Token);
            await auth.IsAllowedAsync(new[] { roleId }, typeof(FirstResource), permission, tokenSource.Token);

            //Assertion
            permissionStore
                .Verify(
                    p => p.IsAllowedAsync(new[] { roleId }, convertor.GetResourceUniqueName(typeof(FirstResource)),
                        convertor.GetPermissionUniqueIdentifier(permission), tokenSource.Token), Times.AtMostOnce);

        }

        #region Mockups

        private class ConstantNamingConvertor : INamingConvertor
        {
            public const string RESOURCE = "resourceInstance";
            public const string PERMISSION = "permission";
            public string GetResourceUniqueName(Type resource)
            {
                return RESOURCE;
            }

            public Type GetResourceTypeByUniqueName(string uniqueName)
            {
                return Type.GetType(uniqueName);
            }

            public string GetPermissionUniqueIdentifier(IConvertible permissionEnum)
            {
                return PERMISSION;
            }

            public string GetPermissionUniqueIdentifier(Type permissionClass, MemberInfo property)
            {
                return PERMISSION;
            }

            public IConvertible GetPermissionByUniqueName(string uniqueName)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #endregion

        ResourceRegistry GetRegistry()
        {
            var resourceRegistry = new ResourceRegistry();
            resourceRegistry.Register(GetType().Assembly);
            return resourceRegistry;
        }
    }
}
