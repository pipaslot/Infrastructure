using System;

namespace Pipaslot.Infrastructure.Security.Attributes
{
    public class NameAttribute : Attribute
    {
        public NameAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}