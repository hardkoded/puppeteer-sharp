namespace PuppeteerSharp
{
    /// <summary>
    /// Represents an environment.
    /// </summary>
    internal interface IEnvironment
    {
        /// <summary>
        /// Gets the CDPSession associated with this environment.
        /// </summary>
        ICDPSession Client { get; }

        /// <summary>
        /// Gets the main realm of this environment.
        /// </summary>
        Realm MainRealm { get; }
    }
}
