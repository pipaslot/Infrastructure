namespace Pipaslot.Infrastructure.Security.Data
{
    public interface IRole<TKey>
    {
        TKey Id { get; }

        /// <summary>
        /// Role name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Role description
        /// </summary>
        string Description { get; }
    }
}
