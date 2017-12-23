using System;
using System.Collections.Generic;
using System.Text;

namespace Pipaslot.Infrastructure.Data
{
    /// <inheritdoc />
    public class AEntity<TKey> : IEntity<TKey>
    {
        /// <inheritdoc />
        public TKey Id { get; set; }

        object IEntity.Id
        {
            get => Id;
            set => Id = (TKey)value;
        }
    }
}
