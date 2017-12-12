using System.Collections.Generic;

namespace Pipaslot.Infrastructure.Security.Identities
{
    public interface IIdentity<TKey>
    {
        /// /// <summary>
        /// Database User Id
        /// </summary>
        TKey Id { get; }
        
        /// <summary>
        /// All user roles
        /// </summary>
        IEnumerable<IUserRole<TKey>> Roles { get; }
    }
}