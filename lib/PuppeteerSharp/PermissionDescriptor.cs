namespace PuppeteerSharp
{
    /// <summary>
    /// Describes a permission to set via <see cref="IBrowserContext.SetPermissionAsync"/>.
    /// </summary>
    public class PermissionDescriptor
    {
        /// <summary>
        /// Gets or sets the permission name (e.g. "geolocation", "midi", "notifications").
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the permission should only apply to user-visible operations.
        /// </summary>
        public bool? UserVisibleOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable system exclusive access (MIDI).
        /// </summary>
        public bool? Sysex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable pan-tilt-zoom controls (camera).
        /// </summary>
        public bool? PanTiltZoom { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow writing without sanitization (clipboard).
        /// </summary>
        public bool? AllowWithoutSanitization { get; set; }
    }
}
