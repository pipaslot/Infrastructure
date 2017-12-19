using System.Collections.Generic;
using System.Linq;
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
    public class PermissionManagerTest
    {
        [TestMethod]
        [ExpectedException(typeof(NotSuitablePermissionException))]
        public void Allow_PermissionIsNotAllowedForResource_ShouldFail()
        {
            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var resourceRegistry = new Mock<ResourceRegistry<int>>();
            var manager = new PermissionManager<int>(permissionStore.Object, resourceRegistry.Object, new DefaultResourceDetailProvider<int>(), new DefaultNamingConvertor<int>(resourceRegistry.Object));

            //Act
            manager.Allow(new UserRole(1), typeof(FirstResource), default(int), SecondPermissions.Edit);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSuitablePermissionException))]
        public void Deny_PermissionIsNotAllowedForResource_ShouldFail()
        {
            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var resourceRegistry = new Mock<ResourceRegistry<int>>();
            var manager = new PermissionManager<int>(permissionStore.Object, resourceRegistry.Object, new DefaultResourceDetailProvider<int>(), new DefaultNamingConvertor<int>(resourceRegistry.Object));

            //Act
            manager.Deny(new UserRole(1), typeof(FirstResource), default(int), SecondPermissions.Edit);
        }

        [TestMethod]
        public void GetAllResources_LoadNameAndDescriptionFromObjectAnnotation()
        {
            var instanceCount = 3;
            var resourceType = typeof(FirstResource);
            var permissionStore = new Mock<IPermissionStore<int>>();
            var tokenSource = new CancellationTokenSource();
            permissionStore
                .Setup(p => p.GetResourceInstanceCountAsync(resourceType.FullName,tokenSource.Token))
                .Returns(Task.FromResult(instanceCount));
            var resourceRegistry = new ResourceRegistry<int>();
            resourceRegistry
                .Register(resourceType.Assembly);
            var manager = new PermissionManager<int>(permissionStore.Object, resourceRegistry, new DefaultResourceDetailProvider<int>(), new DefaultNamingConvertor<int>(resourceRegistry));

            var resources = manager.GetAllResourcesAsync(tokenSource.Token).Result;
            var firstResource = resources.First(r => r.UniquedName == resourceType.FullName);
            Assert.IsFalse(string.IsNullOrWhiteSpace(firstResource.Name));
            Assert.IsFalse(string.IsNullOrWhiteSpace(firstResource.Description));
            Assert.AreEqual(instanceCount, firstResource.InstancesCount);
        }

        [TestMethod]
        public void GetAllResourceInstancess_LoadNameAndDescriptionFromResourceDetailProvider()
        {
            var resourceId = 5;
            var resourceIdList = new List<int> { resourceId };
            var resourceType = typeof(FirstResource);
            var providerResult = new ResourceDetail<int>(resourceId, "NAME", "DESCRIPTION");
            var tokenSource = new CancellationTokenSource();

            var permissionStoreMock = new Mock<IPermissionStore<int>>();
            permissionStoreMock
                .Setup(p => p.GetAllResourceInstancesIdsAsync(resourceType.FullName, tokenSource.Token))
                .Returns(Task.FromResult(new List<int> { resourceId }));

            var resourceRegistry = new ResourceRegistry<int>();
            resourceRegistry.Register(resourceType.Assembly);

            var resourceDetailProviderMock = new Mock<IResourceDetailProvider<int>>();
            resourceDetailProviderMock
                .Setup(r => r.GetResourceDetailsAsync(resourceType, resourceIdList, tokenSource.Token))
                .Returns(Task.FromResult((new List<ResourceDetail<int>>{providerResult}).AsEnumerable()));

            //Act
            var manager = new PermissionManager<int>(permissionStoreMock.Object, resourceRegistry, resourceDetailProviderMock.Object, new DefaultNamingConvertor<int>(resourceRegistry));
            var resources = manager.GetAllResourceInstancesAsync(resourceType.FullName, tokenSource.Token).Result;
            var firstResource = resources
                .First(r => r.UniquedName == resourceType.FullName);

            //Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(firstResource.UniquedName));
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
            
            var resourceRegistry = new ResourceRegistry<int>();
            resourceRegistry.Register(resourceType.Assembly);
            var tokenSource = new CancellationTokenSource();

            var namingConvertor = new DefaultNamingConvertor<int>(resourceRegistry);

            var resourceDetailProviderMock = new Mock<IResourceDetailProvider<int>>();

            var permissionStoreMock = new Mock<IPermissionStore<int>>();
            permissionStoreMock
                .Setup(p => p.IsAllowedAsync(roleId, resourceName, namingConvertor.GetPermissionUniqueIdentifier(StaticPermissions.Create), tokenSource.Token))
                .Returns(Task.FromResult(isAllowed));

            //Act
            var manager = new PermissionManager<int>(permissionStoreMock.Object, resourceRegistry, resourceDetailProviderMock.Object, new DefaultNamingConvertor<int>(resourceRegistry));
            var permissions = manager.GetAllPermissionsAsync(roleId, resourceName, tokenSource.Token).Result.ToList();

            Assert.AreEqual(3, permissions.Count());

            var create = permissions.Last();
            Assert.AreEqual(resourceName, create.ResourceUniquedName);
            Assert.AreEqual(default(int), create.ResourceIdentifier);
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


            var resourceRegistry = new ResourceRegistry<int>();
            resourceRegistry.Register(resourceType.Assembly);
            var tokenSource = new CancellationTokenSource();

            var namingConvertor = new DefaultNamingConvertor<int>(resourceRegistry);

            var resourceDetailProviderMock = new Mock<IResourceDetailProvider<int>>();

            var permissionStoreMock = new Mock<IPermissionStore<int>>();
            permissionStoreMock
                .Setup(p => p.IsAllowedAsync(roleId, resourceName, resourceId, namingConvertor.GetPermissionUniqueIdentifier(FirstPermissions.Edit), tokenSource.Token))
                .Returns(Task.FromResult(isAllowed));

            //Act
            var manager = new PermissionManager<int>(permissionStoreMock.Object, resourceRegistry, resourceDetailProviderMock.Object, new DefaultNamingConvertor<int>(resourceRegistry));
            var permissions = manager.GetAllPermissionsAsync(roleId, resourceName, resourceId, tokenSource.Token).Result.ToList();

            Assert.AreEqual(2, permissions.Count());

            var edit = permissions.Last();
            Assert.AreEqual(resourceName, edit.ResourceUniquedName);
            Assert.AreEqual(resourceId, edit.ResourceIdentifier);
            Assert.AreEqual(namingConvertor.GetPermissionUniqueIdentifier(FirstPermissions.Edit), edit.UniqueIdentifier);
            Assert.IsFalse(string.IsNullOrWhiteSpace(edit.Name));
            Assert.IsFalse(string.IsNullOrWhiteSpace(edit.Description));
            Assert.AreEqual(isAllowed, edit.IsAllowed);
        }
    }
}
