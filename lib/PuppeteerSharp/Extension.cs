using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// Represents a browser extension installed in the browser.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is experimental and may change in future releases.
    /// </para>
    /// </remarks>
    public abstract class Extension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Extension"/> class.
        /// </summary>
        /// <param name="id">The extension ID.</param>
        /// <param name="version">The extension version.</param>
        /// <param name="name">The extension name.</param>
        /// <exception cref="ArgumentException">Thrown when id or version is null or empty.</exception>
        protected Extension(string id, string version, string name)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Extension ID is required.", nameof(id));
            }

            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentException("Extension version is required.", nameof(version));
            }

            Id = id;
            Version = version;
            Name = name;
        }

        /// <summary>
        /// Gets the extension ID.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the extension version.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets the extension name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Returns the list of currently active service workers of the extension.
        /// </summary>
        /// <returns>A task that resolves to an array of <see cref="WebWorker"/> instances.</returns>
        public abstract Task<WebWorker[]> WorkersAsync();

        /// <summary>
        /// Returns the list of visible pages of the extension.
        /// </summary>
        /// <returns>A task that resolves to an array of <see cref="IPage"/> instances.</returns>
        public abstract Task<IPage[]> PagesAsync();

        /// <summary>
        /// Triggers the extension's default action for the given page.
        /// </summary>
        /// <param name="page">The page to trigger the action on.</param>
        /// <returns>A task that completes when the action has been triggered.</returns>
        public abstract Task TriggerActionAsync(IPage page);
    }
}
