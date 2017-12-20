namespace Pipaslot.Infrastructure.Data
{

    /// <summary>
    /// Represents an entity with single-column unique ID.
    /// </summary>
    public interface IEntity<TKey> : IEntity
    {
        /// <summary>
        /// Gets or sets the unique identification of the entity.
        /// </summary>
        new TKey Id { get; set; }
    }

    public interface IEntity
    {
        object Id { get; set; }
    }
}
