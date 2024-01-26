namespace PuppeteerSharp
{
    /// <summary>
    ///  Used to specify User Agent Cient Hints to emulate. See https://wicg.github.io/ua-client-hints
    ///  Missing optional values will be filled in by the target with what it would normally use.
    /// </summary>
    public class UserAgentMetadata
    {
        /// <summary>
        /// Brands.
        /// </summary>
        public UserAgentBrandVersion[] Brands { get; set; }

        /// <summary>
        /// Full version.
        /// </summary>
        public string FullVersion { get; set; }

        /// <summary>
        /// Platform.
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// Platform version.
        /// </summary>
        public string PlatformVersion { get; set; }

        /// <summary>
        /// Architecture.
        /// </summary>
        public string Architecture { get; set; }

        /// <summary>
        /// Model.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Mobile.
        /// </summary>
        public bool Mobile { get; set; }
    }
}
