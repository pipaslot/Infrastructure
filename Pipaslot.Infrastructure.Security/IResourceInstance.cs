namespace Pipaslot.Infrastructure.Security
{
    /// <inheritdoc />
    /// <summary>
    /// Resource distinguishes every single object instance
    /// </summary>
    /// <typeparam name="IConvertible">Permissions which can be used for this object</typeparam>
    public interface IResourceInstance<IConvertible> : IResourceInstance, IResource<IConvertible>
    {

    }

    public interface IResourceInstance
    {
        /// <summary>
        /// Resource Unique Identifier which allows recognize object or their instance for security check. 
        /// If restriction for every object instance is needed, then this property should use object Primary Key like database ID
        /// </summary>
        object ResourceUniqueIdentifier { get; }

        /// <summary>
        /// Resource name
        /// </summary>
        string ResourceName { get; }

        /// <summary>
        /// Resource description
        /// </summary>
        string ResourceDescription { get; }
    }
}