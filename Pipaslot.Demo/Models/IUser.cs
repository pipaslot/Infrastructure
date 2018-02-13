using Pipaslot.Infrastructure.Security;

namespace Pipaslot.Demo.Models
{
    /// <summary>
    /// Custom User interface without generic parameter
    /// </summary>
    public interface IUser : IUser<int>
    {
    }
}
