namespace Pipaslot.Infrastructure.Security.Data
{
    public enum RoleType
    {
        /// <summary>
        /// Role assigned to all visitors
        /// </summary>
        Guest,

        /// <summary>
        /// Role assigned only for authenticated users
        /// </summary>
        User,

        /// <summary>
        /// Role for administrator with all permissions
        /// </summary>
        Admin,

        /// <summary>
        /// Application specific role with full maintanance
        /// </summary>
        Custom
    }
}
