using Pipaslot.Infrastructure.Data;

namespace Pipaslot.Infrastructure.Security.EntityFrameworkCore.Entities
{
    public class Privilege<TKey> : IEntity<TKey>
    {
        public TKey Id { get; set; }
        object IEntity.Id
        {
            get => Id;
            set => Id = (TKey)value;
        }

        public TKey Role { get; set; }

        public string Resource{ get; set; }

        public TKey ResourceInstance { get; set; }

        public string Permission { get; set; }

        public bool IsAllowed { get; set; }
    }
}
