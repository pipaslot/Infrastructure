using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pipaslot.Infrastructure.Security.Attributes;

namespace Pipaslot.Infrastructure.SecurityTests.Models
{
    enum StaticPermissions
    {
        [Name("Creation name")]
        [Description("Creation description")]
        Create
    }
}
