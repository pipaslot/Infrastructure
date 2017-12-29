namespace Pipaslot.Infrastructure.Security
{
    public enum UserIdentityType
    {
        /// <summary>
        /// Guest visiting application(unathenticated)
        /// </summary>
        Guest,

        /// <summary>
        /// Authenticated user
        /// </summary>
        User,

        /// <summary>
        /// User with all permissions
        /// </summary>
        Admin
    }
}
