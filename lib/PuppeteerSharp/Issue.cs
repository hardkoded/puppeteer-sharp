using System.Text.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// Represents a DevTools issue.
    /// </summary>
    public class Issue
    {
        /// <summary>
        /// Gets the issue code.
        /// </summary>
        public string Code { get; init; }

        /// <summary>
        /// Gets the issue details.
        /// </summary>
        public JsonElement Details { get; init; }
    }
}
