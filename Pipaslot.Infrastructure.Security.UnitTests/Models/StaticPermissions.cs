using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pipaslot.Infrastructure.Security.Attributes;

namespace Pipaslot.Infrastructure.Security.Tests.Models
{
    enum StaticPermissions
    {
        [Name("Creation name")]
        [Description("Creation description")]
        Create
    }
}
