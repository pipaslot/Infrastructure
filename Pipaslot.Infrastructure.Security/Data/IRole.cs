﻿namespace Pipaslot.Infrastructure.Security.Data
{
    public interface IRole<out TKey> : IRole
    {
        new TKey Id { get; }
    }

    public interface IRole
    {
        object Id { get; set; }

        /// <summary>
        /// Role name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Role description
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Specificate role type 
        /// </summary>
        RoleType Type { get; }
    }
}
