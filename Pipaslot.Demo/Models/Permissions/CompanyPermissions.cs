using Pipaslot.Infrastructure.Security.Attributes;

namespace Pipaslot.Demo.Models.Permissions
{
    /// <summary>
    /// Permissions related to Company Entity
    /// </summary>
    [Name("Companies")]
    public enum CompanyPermissions
    {
        /// <summary>
        /// User can view single company
        /// </summary>
        [Name("View Company")]
        [Description("Can view single Company")]
        View
    }
}
