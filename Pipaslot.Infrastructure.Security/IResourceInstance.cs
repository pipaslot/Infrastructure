namespace Pipaslot.Infrastructure.Security
{
    /// <inheritdoc />
    /// <summary>
    /// Resource distinguishes every single object instance
    /// </summary>
    /// <typeparam name="TKey">Storege primary key type</typeparam>
    /// <typeparam name="IConvertible">Permissions which can be used for this object</typeparam>
    public interface IResourceInstance<TKey, IConvertible> : IResource<IConvertible>
    {
        /// <summary>
        /// Resource Unique Identifier which allows recognize object or their instance for security check. 
        /// If restriction for every object instance is needed, then this property should use object Primary Key like database ID
        /// </summary>
        TKey ResourceUniqueIdentifier { get; }
    }
}