namespace Pipaslot.Infrastructure.Security
{
    public class ResourceDetail<TKey> : ResourceDetail
    {
        /// <summary>
        /// Resource primary key/Identifier
        /// </summary>
        public new TKey Id { get; }

        public ResourceDetail(TKey id, string name, string description = null) : base(id, name, description)
        {
            Id = id;
        }
    }

    public class ResourceDetail
    {
        /// <summary>
        /// Resource primary key/Identifier
        /// </summary>
        public object Id { get; }

        /// <summary>
        /// Real resource name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Real resource description
        /// </summary>
        public string Description { get; }

        public ResourceDetail(object id, string name, string description = null)
        {
            Id = id;
            Name = name;
            Description = description ?? string.Empty;
        }
    }
}
