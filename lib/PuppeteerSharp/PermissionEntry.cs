namespace PuppeteerSharp
{
    /// <summary>
    /// Represents a permission entry to set via <see cref="IBrowserContext.SetPermissionAsync"/>.
    /// </summary>
    public class PermissionEntry
    {
        /// <summary>
        /// Gets or sets the permission descriptor.
        /// </summary>
        public PermissionDescriptor Permission { get; set; }

        /// <summary>
        /// Gets or sets the state to set the permission to.
        /// </summary>
        public PermissionState State { get; set; }
    }
}
