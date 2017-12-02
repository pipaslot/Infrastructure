using Pipaslot.Infrastructure.Security.Attributes;

namespace Pipaslot.Infrastructure.SecurityTests.Models
{
    public enum SecondPermissions
    {
        [Name("Show all Name")]
        [Description("Show All Description")]
        ShowAll,

        [Name("Edit Name")]
        [Description("Edit Description")]
        Edit
    }
}
