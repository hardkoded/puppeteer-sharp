namespace PuppeteerSharp
{
    /// <summary>
    /// Network restrictions that can be applied to a browser at runtime using <see cref="IBrowser.RestrictNetworkAsync(NetworkRestrictions)"/>.
    /// </summary>
    public class NetworkRestrictions
    {
        /// <summary>
        /// A list of <see href="https://urlpattern.spec.whatwg.org/">URLPattern</see> strings to block.
        /// Mutually exclusive with <see cref="Allowlist"/>.
        /// Passing an empty array or <c>null</c> means no restriction.
        /// </summary>
        /// <remarks>
        /// The blocklist does not apply to WebTransport and WebRTC at this time.
        /// See <see cref="ConnectOptions.BlockList"/>.
        /// </remarks>
        public string[] BlockList { get; set; }

        /// <summary>
        /// A list of <see href="https://urlpattern.spec.whatwg.org/">URLPattern</see> strings to allow.
        /// Mutually exclusive with <see cref="BlockList"/>.
        /// Passing an empty array or <c>null</c> means no restriction.
        /// </summary>
        /// <remarks>
        /// The allowlist does not apply to WebTransport and to TURN over TCP at
        /// this time. It blocks all WebRTC traffic over UDP.
        /// See <see cref="ConnectOptions.Allowlist"/>.
        /// </remarks>
        public string[] Allowlist { get; set; }
    }
}
