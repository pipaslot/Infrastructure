using System.Security.Claims;

namespace Pipaslot.Infrastructure.Security
{
    /// <summary>
    /// Provide current user claims principal
    /// </summary>
    public interface IClaimsPrincipalProvider
    {
        ClaimsPrincipal GetClaimsPrincipal();
    }
}
