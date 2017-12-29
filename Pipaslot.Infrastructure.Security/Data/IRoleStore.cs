using System.Collections.Generic;

namespace Pipaslot.Infrastructure.Security.Data
{
    public interface IRoleStore
    {
        /// <summary>
        /// Returns primary key for unauthenticated users (guests)
        /// </summary>
        /// <returns></returns>
        object GetGuestRoleIdentifier();
        
        /// <summary>
        /// Returns all roles
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRole> GetAll();
    }
}
