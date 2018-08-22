using System;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// BrowserContexts provide a way to operate multiple independent browser sessions. When a browser is launched, it has
    /// a single <see cref="BrowserContext"/> used by default. The method <see cref="Browser.NewPageAsync"/> creates a <see cref="Page"/> in the default <see cref="BrowserContext"/>
    /// </summary>
    public class BrowserContext
    {
        private readonly string _id;

        internal BrowserContext(Browser browser, string contextId)
        {
            Browser = browser;
            _id = contextId;
        }

        /// <summary>
        /// Raised when the url of a target changes
        /// </summary>
        public event EventHandler<TargetChangedArgs> TargetChanged;

        /// <summary>
        /// Raised when a target is created, for example when a new page is opened by <c>window.open</c> <see href="https://developer.mozilla.org/en-US/docs/Web/API/Window/open"/> or <see cref="NewPageAsync"/>.
        /// </summary>
        public event EventHandler<TargetChangedArgs> TargetCreated;

        /// <summary>
        /// Raised when a target is destroyed, for example when a page is closed
        /// </summary>
        public event EventHandler<TargetChangedArgs> TargetDestroyed;

        /// <summary>
        /// Returns whether BrowserContext is incognito
        /// The default browser context is the only non-incognito browser context
        /// </summary>
        /// <remarks>
        /// The default browser context cannot be closed
        /// </remarks>
        public bool IsIncognito => _id != null;

        /// <summary>
        /// Gets the browser this browser context belongs to
        /// </summary>
        public Browser Browser { get; }

        /// <summary>
        /// Gets an array of all active targets inside the browser context 
        /// </summary>
        /// <returns>An array of all active targets inside the browser context</returns>
        public Target[] Targets() => Array.FindAll(Browser.Targets(), target => target.BrowserContext == this);

        /// <summary>
        /// Creates a new page
        /// </summary>
        /// <returns>Task which resolves to a new <see cref="Page"/> object</returns>
        public Task<Page> NewPageAsync() => Browser.CreatePageInContextAsync(_id);

        /// <summary>
        /// Closes the browser context. All the targets that belong to the browser context will be closed
        /// </summary>
        /// <returns>Task</returns>
        public Task CloseAsync()
        {
            if (_id == null)
            {
                throw new PuppeteerException("Non-incognito profiles cannot be closed!");
            }
            return Browser.DisposeContextAsync(_id);
        }

        internal void OnTargetCreated(Browser browser, TargetChangedArgs args) => TargetCreated?.Invoke(browser, args);

        internal void OnTargetDestroyed(Browser browser, TargetChangedArgs args) => TargetDestroyed?.Invoke(browser, args);

        internal void OnTargetChanged(Browser browser, TargetChangedArgs args) => TargetChanged?.Invoke(browser, args);
    }
}
