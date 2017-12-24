namespace Pipaslot.Infrastructure.Security.Data
{
    public class ResourceInstance
    {
        public object Id { get; set; }

        /// <summary>
        /// Role name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Role description
        /// </summary>
        public string Description { get; set; }

        public ResourceInstance()
        {
        }

        public ResourceInstance(object id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
