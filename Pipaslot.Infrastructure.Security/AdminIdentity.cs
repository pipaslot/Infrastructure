using System;
using System.Collections.Generic;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.Security
{
    /// <inheritdoc />
    /// <summary>
    /// Identity for user with all system permission. Be careful with operation and providing this identity. 
    /// This identity is recomended only for Development environment.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class AdminIdentity<TKey> : IUserIdentity<TKey>
    {
        private readonly IPermissionStore<TKey> _permissionStore;
        private readonly IIdentity<TKey> _identity;
        private readonly INamingConvertor _namingConvertor;

        public TKey Id => _identity.Id;

        public bool IsAuthenticated => _identity.IsAuthenticated;

        public IEnumerable<IUserRole<TKey>> Roles => _identity.Roles;

        public AdminIdentity(IPermissionStore<TKey> permissionStore, INamingConvertor namingConvertor, IIdentity<TKey> identity = null)
        {
            _permissionStore = permissionStore;
            _identity = identity ?? new Identity<TKey>();
            _namingConvertor = namingConvertor;
        }

        /// <summary>
        /// Admin has all permissions
        /// </summary>
        /// <param name="permissionEnum"></param>
        /// <returns></returns>
        public bool IsAllowed(IConvertible permissionEnum)
        {
            return true;
        }

        /// <summary>
        /// Admin has all permissions
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="permissionEnum"></param>
        /// <returns></returns>
        public bool IsAllowed(Type resource, IConvertible permissionEnum)
        {
            return true;
        }

        /// <summary>
        /// Admin has all permissions
        /// </summary>
        /// <typeparam name="TPermissions"></typeparam>
        /// <param name="resourceInstance"></param>
        /// <param name="permissionEnum"></param>
        /// <returns></returns>
        public bool IsAllowed<TPermissions>(IResourceInstance<TKey, TPermissions> resourceInstance, TPermissions permissionEnum) where TPermissions : IConvertible
        {
            return true;
        }

        /// <summary>
        /// Admin has all permissions
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="resourceIdentifier"></param>
        /// <param name="permissionEnum"></param>
        /// <returns></returns>
        public bool IsAllowed(Type resource, TKey resourceIdentifier, IConvertible permissionEnum)
        {
            return true;
        }

        /// <summary>
        /// Returns all existing keys for resource without regard to permission.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="permissionEnum"></param>
        /// <returns></returns>
        public IEnumerable<TKey> GetAllowedKeys(Type resource, IConvertible permissionEnum)
        {
            var resourceName = _namingConvertor.GetResourceUniqueName(resource);
            return _permissionStore.GetAllResourceInstancesIds(resourceName);
        }
    }
}
