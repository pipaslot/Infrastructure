namespace Pipaslot.Infrastructure.Security
{
    /// <summary>
    /// Target object to which access should be restricted
    /// </summary>
    /// <typeparam name="TKey">Storege primary key type</typeparam>
    /// <typeparam name="IConvertible">Permissions which can be used for this object</typeparam>
    public interface IResource<TKey, IConvertible>
    {
        /// <summary>
        /// Resource Unique Identifier which allows recognize object or their instance for security check. 
        /// If restriction for every object instance is needed, then this property should use object Primary Key like database ID
        /// </summary>
        TKey ResourceUniqueIdentifier { get; }
    }
}
