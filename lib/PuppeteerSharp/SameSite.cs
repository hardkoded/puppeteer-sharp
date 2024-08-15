using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// SameSite values in cookies.
    /// </summary>
    [DefaultEnumValue((int)None)]
    public enum SameSite
    {
        /// <summary>
        /// None.
        /// </summary>
        None,

        /// <summary>
        /// Strict.
        /// </summary>
        Strict,

        /// <summary>
        /// Lax.
        /// </summary>
        Lax,

        /// <summary>
        /// Extended.
        /// </summary>
        Extended,
    }
}
