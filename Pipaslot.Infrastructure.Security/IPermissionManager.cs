using System;
using System.Collections.Generic;
using System.Text;
using Pipaslot.Infrastructure.Security.Data.Queries;

namespace Pipaslot.Infrastructure.Security
{
    public interface IPermissionManager<TKey>
    {
        ///// <summary>
        ///// Register Resource en 
        ///// </summary>
        ///// <param name="resource"></param>
        //void RegisterPermission(Type resource);

        //IEnumerable<ResourceInfo<TKey>> GetAllResources();

        //IEnumerable<PermissionInfo<TKey>> GetAllPermissions(TKey roleId, string resource, TKey resourceId);

        void Allow(IUserRole<TKey> role, Type resource, TKey resourceId, IConvertible permissionEnum);

        void Deny(IUserRole<TKey> role, Type resource, TKey resourceId, IConvertible permissionEnum);
    }
}
