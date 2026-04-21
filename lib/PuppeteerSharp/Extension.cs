using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// Represents an Extension instance installed in the browser.
    /// </summary>
    /// <remarks>
    /// <para>This API is experimental.</para>
    /// </remarks>
    public abstract class Extension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Extension"/> class.
        /// </summary>
        /// <param name="id">The extension ID.</param>
        /// <param name="version">The extension version.</param>
        /// <param name="name">The extension name.</param>
        protected Extension(string id, string version, string name)
        {
            Id = id;
            Version = version;
            Name = name;
        }

        /// <summary>
        /// Gets the ID of the extension.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the version of the extension.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets the name of the extension.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Returns the list of the currently active service workers of the extension.
        /// </summary>
        /// <returns>A task that resolves to the list of web workers.</returns>
        public abstract Task<IReadOnlyList<WebWorker>> WorkersAsync();

        /// <summary>
        /// Returns the list of the visible pages of the extension.
        /// </summary>
        /// <returns>A task that resolves to the list of pages.</returns>
        public abstract Task<IReadOnlyList<IPage>> PagesAsync();

        /// <summary>
        /// Triggers an extension default action on the given page.
        /// </summary>
        /// <param name="page">The page to trigger the action on.</param>
        /// <returns>A task that completes when the action is triggered.</returns>
        public abstract Task TriggerActionAsync(IPage page);
    }
}
