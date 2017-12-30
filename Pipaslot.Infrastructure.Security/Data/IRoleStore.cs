using System.Collections.Generic;

namespace Pipaslot.Infrastructure.Security.Data
{
    public interface IRoleStore
    {
        /// <summary>
        /// Returns primary key for unauthenticated users (guests)
        /// </summary>
        /// <returns></returns>
        TRole GetGuestRole<TRole>() where TRole : IRole;

        /// <summary>
        /// Returns all roles
        /// </summary>
        /// <returns></returns>
        ICollection<TRole> GetAll<TRole>() where TRole : IRole;

        /// <summary>
        /// Returns all roles with type to be Guest, User or Admin
        /// </summary>
        /// <returns></returns>
        ICollection<TRole> GetSystemRoles<TRole>() where TRole : IRole;
    }
}
