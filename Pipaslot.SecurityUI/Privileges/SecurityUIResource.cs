using Pipaslot.Infrastructure.Security;
using Pipaslot.Infrastructure.Security.Attributes;

namespace Pipaslot.SecurityUI.Privileges
{
    /// <inheritdoc />
    /// <summary>
    /// Security resource for applying security logic for whole Security UI extension
    /// </summary>
    [Name("Security UI Middleware")]
    [Description("Role Permission management")]
    public class SecurityUIResource : IResource<SecurityUIPermissions>
    {
    }
}
