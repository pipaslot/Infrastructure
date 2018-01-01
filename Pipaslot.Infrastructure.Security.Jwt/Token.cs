using System;

namespace Pipaslot.Infrastructure.Security.JWT
{
    public class Token
    {
        public string Value { get; set; }

        public DateTime Expiration { get; set; }
    }
}
