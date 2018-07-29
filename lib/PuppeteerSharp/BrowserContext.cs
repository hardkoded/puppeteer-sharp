using System;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
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

        public bool IsIncognito => _id != null;

        public Browser Browser { get; }

        public Target[] Targets() => Array.FindAll(Browser.Targets(), target => target.BrowserContext == this);

        /// <summary>
        /// Creates a new page
        /// </summary>
        /// <returns>Task which resolves to a new <see cref="Page"/> object</returns>
        public Task<Page> NewPageAsync() => Browser.CreatePageInContextAsync(_id);

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
