namespace Pipaslot.Infrastructure.Security
{
    /// <summary>
    /// User Role
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IUserRole<TKey>
    {
        TKey Id { get; }
    }
}
