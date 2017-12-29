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
using Pipaslot.Infrastructure.SecurityTests.Models;

namespace Pipaslot.Infrastructure.SecurityTests
{
    [TestClass]
    public class AuthorizatorTest
    {

        [TestMethod]
        public async Task Constructor_UseCustomNamingConvertor()
        {
            const int roleId = 1;
            const int resourceId = 2;
            var permission = FirstPermissions.Edit;

            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var convertor = new ConstantNamingConvertor();
            var auth = new Authorizator<int>(permissionStore.Object, convertor);
            
            var resource = new FirstResource(resourceId);
            var tokenSource = new CancellationTokenSource();

            //Pre-Condition
            permissionStore.Setup(p => p.IsAllowedAsync(new []{ roleId }, ConstantNamingConvertor.RESOURCE, resourceId, ConstantNamingConvertor.PERMISSION, tokenSource.Token))
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
            var permission = FirstPermissions.Edit;

            //Init
            var permissionStore = new Mock<IPermissionStore<int>>();
            var convertor = new DefaultNamingConvertor<int>(new ResourceRegistry());
            var auth = new Authorizator<int>(permissionStore.Object, convertor);
            
            var tokenSource = new CancellationTokenSource();

            //Pre-Condition
            permissionStore
                .Setup(p => p.IsAllowedAsync(new[] { roleId }, Authorizator<int>.GLOBAL_RESOURCE_NAME, convertor.GetPermissionUniqueIdentifier(permission), tokenSource.Token))
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
            var convertor = new DefaultNamingConvertor<int>(new ResourceRegistry());
            var auth = new Authorizator<int>(permissionStore.Object, convertor);
            
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
            var auth = new Authorizator<int>(permissionStore.Object, convertorMock.Object);

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
            var convertor = new DefaultNamingConvertor<int>(new ResourceRegistry());
            var auth = new Authorizator<int>(permissionStore.Object, convertor);
            
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
            var convertor = new DefaultNamingConvertor<int>(new ResourceRegistry());
            var auth = new Authorizator<int>(permissionStore.Object, convertor);
            
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
            var auth = new Authorizator<int>(permissionStore.Object, convertorMock.Object);

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
            var convertor = new DefaultNamingConvertor<int>(new ResourceRegistry());
            var auth = new Authorizator<int>(permissionStore.Object, convertor);
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
            var auth = new Authorizator<int>(permissionStore.Object, convertorMock.Object);

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
            var convertor = new DefaultNamingConvertor<int>(new ResourceRegistry());
            var auth = new Authorizator<int>(permissionStore.Object, convertor);
            
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
                    p => p.IsAllowedAsync(new[] {roleId}, convertor.GetResourceUniqueName(typeof(FirstResource)),
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
    }
}
