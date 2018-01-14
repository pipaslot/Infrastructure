namespace Pipaslot.Infrastructure.Security.Data
{
    public class Role : IRole
    {
        public object Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public RoleType Type { get; set; } = RoleType.Custom;
    }
}
