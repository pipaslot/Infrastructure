﻿using Pipaslot.Infrastructure.Data;
using Pipaslot.Infrastructure.Security.Data;

namespace Pipaslot.Infrastructure.Security.EntityFramework.Entities
{
    public class Role<TKey> : IRole, IEntity<TKey>
    {
        public TKey Id { get; set; }
        object IRole.Id => Id;
        object IEntity.Id
        {
            get => Id;
            set => Id = (TKey)value;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public RoleType Type { get; set; } = RoleType.Custom;
    }
}