using Pipaslot.Infrastructure.Data;

namespace Pipaslot.Demo.Models.Entities
{
    /// <inheritdoc />
    /// <summary>
    /// Unusefull entity only for DbControllerTesting
    /// </summary>
    public class SimpleRecord : IEntity<int>
    {
        #pragma warning disable 1591
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Count { get; set; }
    }
}
