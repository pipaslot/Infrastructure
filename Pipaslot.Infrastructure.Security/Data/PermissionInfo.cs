namespace Pipaslot.Infrastructure.Security.Data
{
    public class PermissionInfo
    {
        /// <summary>
        /// Permission unique identifier
        /// </summary>
        public string UniqueIdentifier { get; set; }

        /// <summary>
        /// User friendly name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// User friendly description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Is allowed or is deny
        /// </summary>
        public bool? IsAllowed { get; set; }
    }
}
