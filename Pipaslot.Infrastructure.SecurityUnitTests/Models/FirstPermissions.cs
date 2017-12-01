using Pipaslot.Infrastructure.Security.Attributes;

namespace Pipaslot.Infrastructure.SecurityTests.Models
{
    [Name("First permissions")]
    [Description("First Description")]
    public enum FirstPermissions
    {
        [Name("Show all Name")]
        [Description("Show All Description")]
        ShowAll,

        [Name("Edit Name")]
        [Description("Edit Description")]
        Edit
    }
}
