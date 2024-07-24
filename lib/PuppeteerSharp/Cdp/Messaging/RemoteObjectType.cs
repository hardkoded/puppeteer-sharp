namespace PuppeteerSharp.Cdp.Messaging
{
    /// <summary>
    /// Remote object type.
    /// </summary>
    public enum RemoteObjectType
    {
        /// <summary>
        /// Other.
        /// </summary>
        Other,

        /// <summary>
        /// Object.
        /// </summary>
#pragma warning disable CA1720 // Identifier contains type name
        Object,
#pragma warning restore CA1720 // Identifier contains type name
        /// <summary>
        /// Function.
        /// </summary>
        Function,

        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined,

        /// <summary>
        /// String.
        /// </summary>
#pragma warning disable CA1720 // Identifier contains type name
        String,
#pragma warning restore CA1720 // Identifier contains type name
        /// <summary>
        /// Number.
        /// </summary>
        Number,

        /// <summary>
        /// Boolean.
        /// </summary>
        Boolean,

        /// <summary>
        /// Symbol.
        /// </summary>
        Symbol,

        /// <summary>
        /// Bigint.
        /// </summary>
        Bigint,
    }
}
