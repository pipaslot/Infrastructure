using System;
using System.Collections.Generic;
using System.Text;

namespace Pipaslot.Infrastructure.Security
{
    /// <summary>
    /// Identity about current user/client
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class Identity<TKey> : IIdentity<TKey>
    {
        public TKey Id { get; }
        
        public bool IsAuthenticated { get; }
        
        public IEnumerable<IUserRole<TKey>> Roles { get; }

        public Identity()
        {
            Id = default(TKey);
            Roles = new List<IUserRole<TKey>>();
        }

        public Identity(TKey id, IEnumerable<IUserRole<TKey>> roles = null)
        {
            Id = id;
            Roles = roles ?? new List<IUserRole<TKey>>();
            //If Id is not default value, then was passed valid UserId and user had been authenticated
            var defaultId = default(TKey);
            if (defaultId == null)
            {
                //Id is nullable type
                IsAuthenticated = Id != null;
            }
            else
            {
                //Id is not nullable type
                IsAuthenticated = Id.GetHashCode() != defaultId.GetHashCode();
            }
        }
    }
}
