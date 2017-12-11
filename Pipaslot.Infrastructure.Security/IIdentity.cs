using System.Collections.Generic;

namespace Pipaslot.Infrastructure.Security
{
    public interface IIdentity<TKey>
    {
        /// /// <summary>
        /// Database User Id
        /// </summary>
        TKey Id { get; }

        /// <summary>
        /// User is authenticated. Id is not default value or null
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// All user roles
        /// </summary>
        IEnumerable<IUserRole<TKey>> Roles { get; }
    }
}