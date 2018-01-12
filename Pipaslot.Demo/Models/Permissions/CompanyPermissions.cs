using Pipaslot.Infrastructure.Security.Attributes;

namespace Pipaslot.Demo.Models.Permissions
{
    /// <summary>
    /// Permissions related to Company Entity
    /// </summary>
    public enum CompanyPermissions
    {
        /// <summary>
        /// User can view single company
        /// </summary>
        [Description("Can view single Company")]
        View,

        Update,
        /// <summary>
        /// User can delete single company
        /// </summary>
        Delete
    }
}
