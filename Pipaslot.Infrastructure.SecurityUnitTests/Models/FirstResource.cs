﻿using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Attributes;

namespace Pipaslot.Infrastructure.SecurityTests.Models
{
    [Name("First Resource Name")]
    [Description("First resource purpose description etc. etc. etc. etc.")]
    public class FirstResource : IResource<int, FirstPermissions>
    {
        public int Id { get; set; }

        public FirstResource(int id)
        {
            Id = id;
        }

        #region IResource Implementation
        
        public int ResourceUniqueIdentifier => Id;

        #endregion
    }
}
