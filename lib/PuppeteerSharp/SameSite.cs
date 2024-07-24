using System.Text.Json;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// SameSite values in cookies.
    /// </summary>
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
